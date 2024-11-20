using System;
using System.Data;
using System.IO;
using OfficeOpenXml;
using System.Windows.Forms;
using System.ComponentModel;
using LicenseContext = OfficeOpenXml.LicenseContext;
using MetinMadenciliği.Service;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace MetinMadenciliği
{
    public partial class Form1 : Form
    {
        AppDbContext db = new AppDbContext();
        ZemberekService zemberekService = new ZemberekService();
        public Form1()
        {
            InitializeComponent();
            // EPPlus lisansını ayarla
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string filePath = openFileDialog1.FileName;
                label1.Text = "Dosya Adı: " + openFileDialog1.SafeFileName;
                try
                {
                    using (var package = new ExcelPackage(new FileInfo(filePath)))
                    {
                        var worksheet = package.Workbook.Worksheets[0]; // İlk sayfa
                        DataTable dataTable = new DataTable();

                        // Header'ları ekle
                        for (int col = 1; col <= worksheet.Dimension.End.Column; col++)
                        {
                            dataTable.Columns.Add(worksheet.Cells[1, col].Text);
                        }

                        // Verileri ekle
                        for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
                        {
                            DataRow newRow = dataTable.NewRow();
                            for (int col = 1; col <= worksheet.Dimension.End.Column; col++)
                            {
                                newRow[col - 1] = worksheet.Cells[row, col].Text;
                            }
                            dataTable.Rows.Add(newRow);
                        }

                        dataGridView1.DataSource = dataTable;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Hata: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private async void button2_Click(object sender, EventArgs e)
        {

            try
            {
                if (dataGridView1.DataSource is DataTable dataTable)
                {
                    // "Icerik", "Kategori", "KategoriId" ve "Id" kolonlarının varlığını kontrol et
                    if (!dataTable.Columns.Contains("Icerik") ||
                        !dataTable.Columns.Contains("Kategori") ||
                        !dataTable.Columns.Contains("KategoriId") ||
                        !dataTable.Columns.Contains("Id"))
                    {
                        MessageBox.Show("Gerekli kolonlardan biri eksik.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // Icerik kolonundaki text'leri al
                    var textList = dataTable.AsEnumerable()
                                             .Select(row => row.Field<string>("Icerik"))
                                             .Where(text => !string.IsNullOrEmpty(text))
                                             .ToList();

                    // 10'ar gruplar halinde işlem yap
                    for (int i = 0; i < textList.Count; i += 10)
                    {
                        var batch = textList.Skip(i).Take(10).ToList();

                        foreach (var text in batch)
                        {
                            // FullAutoPrepAsync metodunu çağır ve yanıtı al
                            var (removePunctuationResult, lowerCaseResult, zemberekResult, stopWordsResult, findUniquesResult) = await db.PrepAuto(text);

                            // Gelen her bir sonucu işleyin ve ilgili kolonlara ekleyin
                            foreach (DataRow row in dataTable.Rows)
                            {
                                if (row["Icerik"].ToString() == text)
                                {
                                    row["TemizlenmişMetin"] = removePunctuationResult + "\n\n\n\n" ?? string.Empty;
                                    row["KüçükHarfeDönüştürülmüşMetin"] = lowerCaseResult + "\n\n\n\n" ?? string.Empty;
                                    row["Kök/GövdesiTespitEdilmişMetin"] = zemberekResult + "\n\n\n\n" ?? string.Empty;
                                    row["GereksizKelimeleriKaldırılmışMetin"] = stopWordsResult + "\n\n\n\n" ?? string.Empty;
                                    row["SadeceBenzersizKelimelerleMetin"] = findUniquesResult + "\n\n\n\n" ?? string.Empty;
                                }
                            }
                        }
                    }

                    // Dosya kaydetme penceresini açın
                    if (openFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        string filePath = openFileDialog1.FileName;

                        // Mevcut dosyayı yükleyin
                        using (var package = new ExcelPackage(new FileInfo(filePath)))
                        {
                            var sheet1 = package.Workbook.Worksheets[0]; // İlk sayfa

                            // Veri güncelleme işlemi
                            int rowIndex = 2;
                            foreach (DataRow row in dataTable.Rows)
                            {
                                sheet1.Cells[rowIndex, 1].Value = row["Kategori"]; // Kategori
                                sheet1.Cells[rowIndex, 2].Value = row["KategoriId"]; // KategoriId
                                sheet1.Cells[rowIndex, 3].Value = row["Id"]; // Id
                                sheet1.Cells[rowIndex, 4].Value = row["Icerik"];
                                sheet1.Cells[rowIndex, 5].Value = row["TemizlenmişMetin"];
                                sheet1.Cells[rowIndex, 6].Value = row["KüçükHarfeDönüştürülmüşMetin"];
                                sheet1.Cells[rowIndex, 7].Value = row["Kök/GövdesiTespitEdilmişMetin"];
                                sheet1.Cells[rowIndex, 8].Value = row["GereksizKelimeleriKaldırılmışMetin"];
                                sheet1.Cells[rowIndex, 9].Value = row["SadeceBenzersizKelimelerleMetin"];
                                rowIndex++;
                            }

                            // Yeni eklenen vektörel ifade sütununu doldurun
                            var rootWords = new List<string>();
                            foreach (DataRow row in dataTable.Rows)
                            {
                                var uniqueWords = row["SadeceBenzersizKelimelerleMetin"].ToString();
                                if (!string.IsNullOrEmpty(uniqueWords))
                                {
                                    var words = uniqueWords.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                                    foreach (var word in words)
                                    {
                                        rootWords.Add(word);
                                    }
                                }
                            }

                            // Kök kelimelere göre vektörel ifadeleri ekleyin
                            rowIndex = 2;
                            foreach (DataRow row in dataTable.Rows)
                            {
                                var uniqueWords = row["SadeceBenzersizKelimelerleMetin"].ToString();
                                if (!string.IsNullOrEmpty(uniqueWords))
                                {
                                    var words = uniqueWords.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                                    HashSet<string> rootWordSet = new HashSet<string>(rootWords);

                                    string vectorExpression = "";

                                    foreach (var rootWord in rootWordSet)
                                    {
                                        if (words.Contains(rootWord))
                                        {
                                            vectorExpression += " 1 ";
                                        }
                                        else
                                        {
                                            vectorExpression += " 0 ";
                                        }
                                    }

                                    sheet1.Cells[rowIndex, 10].Value = vectorExpression.Trim();  // Vektörel ifade
                                }
                                rowIndex++;
                            }

                            // Değişiklikleri mevcut dosyaya kaydedin
                            package.Save();

                            MessageBox.Show("Veriler başarıyla güncellendi ve mevcut dosyaya kaydedildi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Lütfen önce bir Excel dosyasını yükleyin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



    }
}
