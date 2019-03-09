using Amazon.Lambda.Core;
using Amazon.Lambda.LexEvents;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace norbot
{
    public class GetBinIntentProcessor : AbstractIntentProcessor
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

            LambdaLogger.Log(string.Format("Postcode is {0}", slots["Postcode"]));
 


            if (string.Equals(lexEvent.InvocationSource, "DialogCodeHook", StringComparison.Ordinal))
            {
                if (slots.All(x => x.Value == null))
                {
                    return Delegate(sessionAttributes, slots);
                }
                ValidationResult validationCheck = Validators.IsValidPostcode(slots["Postcode"]);
                if (validationCheck.IsValid)
                {
                    LambdaLogger.Log(string.Format("Postcode is valid"));
                    validationCheck = Validators.IsValidAreaPostcode(slots["Postcode"]);
                    if (validationCheck.IsValid)
                    {
                        LambdaLogger.Log(string.Format("Postcode is in area"));
                        return Delegate(sessionAttributes, slots);
                    }
                    else
                    {
                        LambdaLogger.Log(string.Format("Postcode is not in area"));
                        slots["Postcode"] = null;
                        return ElicitSlot(sessionAttributes, lexEvent.CurrentIntent.Name, slots, validationCheck.ViolationSlot, validationCheck.Message);
                    }
                }
                else
                {
                    LambdaLogger.Log(string.Format("Postcode is invalid"));
                    slots["Postcode"] = null;
                    return ElicitSlot(sessionAttributes, lexEvent.CurrentIntent.Name, slots, validationCheck.ViolationSlot, validationCheck.Message);
                }
            }
            else
            {
                var binFinderClient = new System.Net.WebClient();
                var binFinderJSON = binFinderClient.DownloadString("https://selfserve.northampton.gov.uk/mycouncil/BinRoundFinder?postcode=" + slots["Postcode"]);
                dynamic binFinderResponse = JsonConvert.DeserializeObject(binFinderJSON);
                string binFinderResult = binFinderResponse.result;
                string binFinderDescription = binFinderResponse.description;
                string collectionDay = binFinderResponse.day;
                string collectionType = binFinderResponse.type;
                string collectionCalendar = binFinderResponse.url;
                string collectionTypeDesc = "";
                string collectionTypeAlternate = "";
                LambdaLogger.Log(string.Format("Day is {0}", collectionDay));
                switch (collectionType)
                {
                    case "black":
                        collectionTypeDesc = "Black Wheelie Bin";
                        collectionTypeAlternate = "Your collections alternate between Black and Brown Wheelie bins.";
                        collectionTypeAlternate += "\n\nWe will collect all of your recycling every week.";
                        break;
                    case "brown":
                        collectionTypeDesc = "Brown Wheelie Bin";
                        collectionTypeAlternate = "Your collections alternate between Black and Brown Wheelie bins.";
                        collectionTypeAlternate += "\n\nWe will collect all of your recycling every week.";
                        break;
                    case "bags":
                        collectionTypeDesc = "Green Bags";
                        collectionTypeAlternate = "We will collect your Green Bags and all of your recycling every week";
                        break;
                    default:
                        //notify!!
                        break;
                }

                return Close(
                            sessionAttributes,
                            "Fulfilled",
                            new LexResponse.LexMessage
                            {
                                ContentType = MESSAGE_CONTENT_TYPE,
                                Content = String.Format(collectionDay)
                            }
                        );
            }


        }
    }
}
