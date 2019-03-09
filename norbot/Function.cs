using System;
using Amazon.Lambda.Core;
using Amazon.Lambda.LexEvents;

[assembly: LambdaSerializerAttribute(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace norbot
{
    public class Function
    {
        public LexResponse FunctionHandler(LexEvent lexEvent, ILambdaContext context)
        {
            IIntentProcessor process;

            switch (lexEvent.CurrentIntent.Name)
            {
                case "GetBin":
                    process = new GetBinIntentProcessor();
                    break;
                case "Debug":
                    process = new DebugIntentProcessor();
                    break;
                default:
                    throw new Exception($"Intent with name {lexEvent.CurrentIntent.Name} not supported");
            }
            return process.Process(lexEvent, context);
        }

    }
}
