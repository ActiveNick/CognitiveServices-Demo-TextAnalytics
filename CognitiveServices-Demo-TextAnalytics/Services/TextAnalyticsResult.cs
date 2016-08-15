using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CognitiveServices_Demo_TextAnalytics
{
    public class TextAnalyticsResult
    {
        public SentimentResult Sentiment { get; set; }
        public KeyPhrasesResult KeyPhrases { get; set; }
    }

    public class SDocument
    {
        public double score { get; set; }
        public string id { get; set; }
    }

    public class SentimentResult
    {
        [JsonProperty("documents")]
        public List<SDocument> SDocuments { get; set; }

        [JsonProperty("errors")]
        public List<object> errors { get; set; }
    }

    public class KPDocument
    {
        public List<string> keyPhrases { get; set; }
        public string id { get; set; }
    }

    public class KeyPhrasesResult
    {
        [JsonProperty("documents")]
        public List<KPDocument> KPDocuments { get; set; }

        [JsonProperty("errors")]
        public List<object> KPErrors { get; set; }
    }
}
