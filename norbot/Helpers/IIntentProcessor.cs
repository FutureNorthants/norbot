using Amazon.Lambda.Core;
using Amazon.Lambda.LexEvents;

namespace norbot
{
    public interface IIntentProcessor
    {
        LexResponse Process(LexEvent lexEvent, ILambdaContext context);
    }
}
