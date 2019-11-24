using Amazon.Lambda.Core;
using Amazon.Lambda.LexEvents;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace norbot
{
    public class DefaultIntentProcessor : AbstractIntentProcessor
    {
        public override LexResponse Process(LexEvent lexEvent, ILambdaContext context)
        {
            IDictionary<string, string> sessionAttributes = lexEvent.SessionAttributes ?? new Dictionary<string, string>();
            IDictionary<string, string> requestAttributes = lexEvent.RequestAttributes ?? new Dictionary<string, string>();
            IDictionary<string, string> slots = lexEvent.CurrentIntent.Slots;

            LambdaLogger.Log("GetBin Intent Processor");
            LambdaLogger.Log("Bot Name =" + lexEvent.Bot.Name);
            LambdaLogger.Log("Bot Aliase =" + lexEvent.Bot.Alias);
            LambdaLogger.Log("Bot Version =" + lexEvent.Bot.Version);
            LambdaLogger.Log("user ID =" + lexEvent.UserId);
            LambdaLogger.Log("Transcription =" + lexEvent.InputTranscript);
            LambdaLogger.Log("Invocation Source =" + lexEvent.InvocationSource);
            LambdaLogger.Log("Output Dialog Mode =" + lexEvent.OutputDialogMode);

            foreach (KeyValuePair<string, string> entries in sessionAttributes)
            {
                LambdaLogger.Log(string.Format("Session Attribute = {0}, Value = {1}", entries.Key, entries.Value));
            }

            foreach (KeyValuePair<string, string> entries in requestAttributes)
            {
                LambdaLogger.Log(string.Format("Request Attribute = {0}, Value = {1}", entries.Key, entries.Value));
            }

            foreach (KeyValuePair<string, string> entries in slots)
            {
                LambdaLogger.Log(string.Format("Slot = {0}, Value = {1}", entries.Key, entries.Value));
            }

            String faqResponse = ReadObjectDataAsync(lexEvent.InputTranscript).Result;

            if (faqResponse.Equals(""))
            {
                return Close(
                       sessionAttributes,
                       "Failed",
                       new LexResponse.LexMessage
                       {
                           ContentType = MESSAGE_CONTENT_TYPE,
                           Content = String.Format("Please wait for one of my human colleagues to join this chat.")
                       });
            }
            else
            {
                return Close(
                       sessionAttributes,
                       "Fulfilled",
                       new LexResponse.LexMessage
                       {
                           ContentType = MESSAGE_CONTENT_TYPE,
                           Content = String.Format(faqResponse)
                       });
            }
        }

        private async Task<String> ReadObjectDataAsync(String transcription)
        {
            String faqResponse = "";
            String qnaAnswer;
            String qnaScore;
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", Environment.GetEnvironmentVariable("Authorization"));
                HttpResponseMessage responseMessage = await client.PostAsync(
                     Environment.GetEnvironmentVariable("qnaURL"),
                     new StringContent("{'question':'" + transcription + "'}", Encoding.UTF8, "application/json"));
                responseMessage.EnsureSuccessStatusCode();
                string responseBody = await responseMessage.Content.ReadAsStringAsync();
                Console.WriteLine($"Response3A from QnA {responseBody}");
                dynamic jsonResponse = JObject.Parse(responseBody);
                Console.WriteLine($"Answer from QnA {jsonResponse.answers[0].answer}");
                Console.WriteLine($"Answer from QnA {jsonResponse.answers[0].score}");
                qnaAnswer = jsonResponse.answers[0].answer;
                qnaScore = jsonResponse.answers[0].score;
                Console.WriteLine($"qnaScore >>{qnaScore}<<");
                int score = 0;
                if (!Int32.TryParse(qnaScore.Substring(0, 2), out score))
                {
                    Console.WriteLine($"Unable to parse");
                    score = -1;
                }

                Console.WriteLine($"Score {score}");
                if (score > 50)
                {
                    faqResponse = qnaAnswer;
                }
            }
            return faqResponse;
        }
    }
}
