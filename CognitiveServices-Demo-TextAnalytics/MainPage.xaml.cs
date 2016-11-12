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
using Microsoft.Toolkit.Uwp.Services.Twitter;
using Windows.UI.Popups;

// Analyse a text string to extract the sentiment and the key phrases
// TO DO: Also extract the detected language
namespace CognitiveServices_Demo_TextAnalytics
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        // TO DO: IMPORTANT - These are my Twitter keys and I may change them when I feel like it.
        // Go get your own at http://dev.twitter.com.
        string twConsumerKey = "ZSxGGzitmQsyqydCmmmDp9i7D";
        string twConsumerSecret = "yyGudrTlBtGCh9KtUASD685xPXC2TgTb6jx8z8dJ5v7alBeI4C";
        string twCallbackUri = "http://ageofmobility.com";  // Dummy callback url because I'm not really using it

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

        private async void btnTwitterConnect_Click(object sender, RoutedEventArgs e)
        {
            TwitterService.Instance.Initialize(twConsumerKey, twConsumerSecret, twCallbackUri);

            if (!await TwitterService.Instance.LoginAsync())
            {
                var error = new MessageDialog("Unable to log to Twitter");
                await error.ShowAsync();
                return;
            }

            TwitterUser user;
            try
            {
                user = await TwitterService.Instance.GetUserAsync();
            }
            catch (TwitterException ex)
            {
                if ((ex.Errors?.Errors?.Length > 0) && (ex.Errors.Errors[0].Code == 89))
                {
                    await new MessageDialog("Invalid or expired token. Logging out. Re-connect for new token.").ShowAsync();
                    TwitterService.Instance.Logout();
                    return;
                }
                else
                {
                    throw ex;
                }
            }

            ListView.ItemsSource = await TwitterService.Instance.GetUserTimeLineAsync(user.ScreenName, 50);
        }

        private void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            Tweet tw = (Tweet)e.ClickedItem;

            txtSource.Text = tw.Text;
        }
    }
}
