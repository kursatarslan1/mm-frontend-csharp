using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MetinMadenciliği.Service
{
    public class PreprocessingService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiUrl;

        public PreprocessingService(HttpClient httpClient, string apiUrl)
        {
            _httpClient = httpClient;
            _apiUrl = apiUrl;
        }

        public async Task<(string removePunctuationResult, string lowerCaseResult, string zemberekResult, string stopWordsResult, string findUniquesResult)> FullAutoPrepAsync(string text)
        {
            var requestBody = new
            {
                text = text,
            };

            var json = JsonConvert.SerializeObject(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync(_apiUrl + "prep/auto", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseData = await response.Content.ReadAsStringAsync();
                    dynamic jsonResponse = JsonConvert.DeserializeObject(responseData);

                    // Yanıtın içindeki her bir değeri alıp ayrı değişkenlere atıyoruz
                    string removePunctuationResult = jsonResponse.result.removePunctuationResult;
                    string lowerCaseResult = jsonResponse.result.lowerCaseResult;
                    string zemberekResult = jsonResponse.result.zemberekResult;
                    string stopWordsResult = jsonResponse.result.stopWordsResult;
                    string findUniquesResult = jsonResponse.result.findUniquesResult;

                    return (removePunctuationResult, lowerCaseResult, zemberekResult, stopWordsResult, findUniquesResult);
                }
                else
                {
                    throw new Exception("API isteği başarısız oldu: " + response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return (null, null, null, null, null); // Hata durumunda null döndürüyoruz
            }
        }
    }
}
