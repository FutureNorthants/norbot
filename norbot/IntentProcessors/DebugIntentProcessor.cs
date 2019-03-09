using Amazon.Lambda.Core;
using Amazon.Lambda.LexEvents;
using System;
using System.Collections.Generic;

namespace norbot
{
    public class DebugIntentProcessor : AbstractIntentProcessor
    {
        public override LexResponse Process(LexEvent lexEvent, ILambdaContext context)
        {
            IDictionary<string, string> sessionAttributes = lexEvent.SessionAttributes ?? new Dictionary<string, string>();
            IDictionary<string, string> requestAttributes = lexEvent.RequestAttributes ?? new Dictionary<string, string>();
            IDictionary<string, string> slots = lexEvent.CurrentIntent.Slots;

            LambdaLogger.Log("Debug Intent Processor");
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

            return Close(
                        sessionAttributes,
                        "Fulfilled",
                        new LexResponse.LexMessage
                        {
                            ContentType = MESSAGE_CONTENT_TYPE,
                            Content = String.Format("Norbot is responding. All debug messages written to log file.")
                        }
                    );
        }
    }
}
