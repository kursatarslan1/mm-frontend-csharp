import java.io.PrintStream;
import java.nio.charset.StandardCharsets;
import java.util.regex.Pattern;
import zemberek.morphology.TurkishMorphology;
import zemberek.morphology.analysis.SingleAnalysis;

public class TestZemberek {
    public static void main(String[] args) {
        try {
            System.setOut(new PrintStream(System.out, true, StandardCharsets.UTF_8));

            // Girdi metni args[0] olarak alınır
            String text = args[0];
            TurkishMorphology morphology = TurkishMorphology.createWithDefaults();
            
            // Metni kelimelere ayır
            String[] words = Pattern.compile("\\s+").split(text);  // Kelimeleri ayırmak için boşlukları kullanıyoruz.

            // Her kelimeyi analiz et
            for (String word : words) {
                // Her bir kelimeyi analiz et
                var analysisResult = morphology.analyze(word);

                // Analiz sonuçlarını yazdır
                for (SingleAnalysis analysis : analysisResult) {
                    System.out.println("Word: " + word);
                    System.out.println("Stem (Kök): " + analysis.getStem());  // Kök kelimeyi al
                    System.out.println("Lemmas: " + analysis.getLemmas());  // Lemma bilgisi
                }
            }

        } catch (Exception e) {
            e.printStackTrace();
        }
    }
}
