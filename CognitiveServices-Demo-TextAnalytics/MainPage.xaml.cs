using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Analyse a text string to extract the sentiment and the key phrases
// TO DO: Also extract the detected language
namespace CognitiveServices_Demo_TextAnalytics
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void btnAnalyze_Click(object sender, RoutedEventArgs e)
        {
            string myText = txtSource.Text;
            int i = 1;
            string sentimentPercentage = "";
            string allKeyPhrases = "";

            // Create JSON message, this is expected by the service
            StringBuilder sb = new StringBuilder();
            sb.Append("{\"documents\":[");
            // We could send one or more strings of text, add a comma between each if you edit this code
            sb.Append("{\"id\":\"" + i.ToString() + "\",\"text\":\"" + myText + "\"}");
            // Close JSON message, don't forget to remove any trailing comma as needed if you send more strings
            sb.Append("]}");

            // Call the Text Analytics service to get the sentiment
            TextAnalyticsResult taResults = await TextAnalyticsService.GetSentiment(sb.ToString());

            if (taResults != null && taResults.Sentiment != null)
            { 
                // Calculate average sentiment score
                // This sample only sends one string, but could support many, so this code would still work
                double scoreSum = 0.0;
                int scoreCount = 0;
                foreach (SDocument sdoc in taResults.Sentiment.SDocuments)
                {
                    scoreSum += sdoc.score;
                    scoreCount++;
                }
                double averageSentimentScore = scoreSum / scoreCount;
                sentimentPercentage = averageSentimentScore.ToString("0.000");
            } else
            {
                // Handle error
                await new Windows.UI.Popups.MessageDialog("Oops! Something went wrong when retrieving the sentiment score.").ShowAsync();
            }

            if (taResults != null && taResults.KeyPhrases != null)
            {
                // Extract all key phrases from first document
                foreach (string phrase in taResults.KeyPhrases.KPDocuments[0].keyPhrases)
                {
                    allKeyPhrases += phrase + ", ";
                }
            } else
            {
                // Handle error
                await new Windows.UI.Popups.MessageDialog("Oops! Something went wrong when retrieving the key phrases.").ShowAsync();
            }

            // Display results
            lblSentiment.Text = "Sentiment: " + sentimentPercentage.ToString();
            lblKeyPhrases.Text = "Key Phrases: " + allKeyPhrases;
        }
    }
}
