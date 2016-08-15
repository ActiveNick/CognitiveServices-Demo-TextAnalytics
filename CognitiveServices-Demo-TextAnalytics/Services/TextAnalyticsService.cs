using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using static Newtonsoft.Json.JsonConvert;

namespace CognitiveServices_Demo_TextAnalytics
{
    public static class TextAnalyticsService
    {
        /// <summary>
        /// Azure portal URL.
        /// </summary>
        private const string BaseUrl = "https://westus.api.cognitive.microsoft.com/";

        /// <summary>
        /// Your account key goes here.  Request access to the TextAnalytics service from https://www.microsoft.com/cognitive.  
        /// </summary>
        private const string AccountKey = "0c724dce83d7467a99e18f572b9edf1e";

        public static async Task<TextAnalyticsResult> GetSentiment(string textToProcess)
        {
            TextAnalyticsResult taResult = new TextAnalyticsResult();
            //List<double> sentimentScores = new List<double>();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUrl);

                // Request headers.
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", AccountKey);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                byte[] byteData = Encoding.UTF8.GetBytes(textToProcess);

                // Detect sentiment:
                var uri = "text/analytics/v2.0/sentiment";
                string json = await CallEndpoint(client, uri, byteData);
                Debug.WriteLine("Sentiment response:" + json.ToString());

                if (string.IsNullOrWhiteSpace(json))
                {
                    taResult.Sentiment = null;
                }
                else
                {
                    taResult.Sentiment = DeserializeObject<SentimentResult>(json);
                }

                // Get Key phrases
                uri = "text/analytics/v2.0/keyPhrases";
                json = await CallEndpoint(client, uri, byteData);
                Debug.WriteLine("Key phrases response:" + json.ToString());

                if (string.IsNullOrWhiteSpace(json))
                {
                    taResult.Sentiment = null;
                }
                else
                {
                    taResult.KeyPhrases = DeserializeObject<KeyPhrasesResult>(json);
                }
                return taResult;
            }
        }

        private static async Task<String> CallEndpoint(HttpClient client, string uri, byte[] byteData)
        {
            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var response = await client.PostAsync(uri, content);
                return await response.Content.ReadAsStringAsync();
            }
        }
    }
}
