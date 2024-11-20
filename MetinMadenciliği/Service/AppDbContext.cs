using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MetinMadenciliği.Service
{
    public class AppDbContext
    {
        private readonly PreprocessingService _preprocessingService;
        public AppDbContext()
        {
            var httpClient = new HttpClient();
            var apiUrl = "http://localhost:3000/";

            _preprocessingService = new PreprocessingService(httpClient, apiUrl);
        }

        public async Task<(string removePunctuationResult, string lowerCaseResult, string zemberekResult, string stopWordsResult, string findUniquesResult)> PrepAuto(string text)
        {
            return await _preprocessingService.FullAutoPrepAsync(text);
        }
    }
}
