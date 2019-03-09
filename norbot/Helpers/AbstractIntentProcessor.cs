using Amazon.Lambda.Core;
using Amazon.Lambda.LexEvents;
using System.Collections.Generic;

namespace norbot
{
    public abstract class AbstractIntentProcessor : IIntentProcessor
    {

        internal const string MESSAGE_CONTENT_TYPE = "PlainText";

        public abstract LexResponse Process(LexEvent lexEvent, ILambdaContext context);

        protected LexResponse Close(IDictionary<string, string> sessionAttributes, string fulfillmentState, LexResponse.LexMessage message)
        {
            return new LexResponse
            {
                SessionAttributes = sessionAttributes,
                DialogAction = new LexResponse.LexDialogAction
                {
                    Type = "Close",
                    FulfillmentState = fulfillmentState,
                    Message = message
                }
            };
        }

        protected LexResponse Delegate(IDictionary<string, string> sessionAttributes, IDictionary<string, string> slots)
        {
            return new LexResponse
            {
                SessionAttributes = sessionAttributes,
                DialogAction = new LexResponse.LexDialogAction
                {
                    Type = "Delegate",
                    Slots = slots
                }
            };
        }

        protected LexResponse ElicitSlot(IDictionary<string, string> sessionAttributes, string intentName, IDictionary<string, string> slots, string slotToElicit, LexResponse.LexMessage message)
        {
            return new LexResponse
            {
                SessionAttributes = sessionAttributes,
                DialogAction = new LexResponse.LexDialogAction
                {
                    Type = "ElicitSlot",
                    IntentName = intentName,
                    Slots = slots,
                    SlotToElicit = slotToElicit,
                    Message = message
                }
            };
        }

        protected LexResponse ConfirmIntent(IDictionary<string, string> sessionAttributes, string intentName, IDictionary<string, string> slots, LexResponse.LexMessage message)
        {
            return new LexResponse
            {
                SessionAttributes = sessionAttributes,
                DialogAction = new LexResponse.LexDialogAction
                {
                    Type = "ConfirmIntent",
                    IntentName = intentName,
                    Slots = slots,
                    Message = message
                }
            };
        }
    }
}
