using Alexa.NET;
using Alexa.NET.Request;
using Alexa.NET.Request.Type;
using Alexa.NET.Response;
using Amazon.Lambda.Core;
using System;
using System.Linq;
using System.Threading.Tasks;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace LaunchCalendarSkill
{
    public class Function
    {
        private readonly NextLaunchIntentHandler _nextLaunchIntentHandler;

        public Function()
        {
            _nextLaunchIntentHandler = new NextLaunchIntentHandler();
        }

        public Task<SkillResponse> FunctionHandler(SkillRequest input, ILambdaContext context)
        {
            var logger = context.Logger;

            if (input.Request.RequestId == "EdwRequestId.ping")
            {
                logger.LogLine("Pong!");
            }

            logger.LogLine($"Incoming {input.Request.Type}");
            switch (input.Request)
            {
                case LaunchRequest launchRequest:
                    logger.LogLine("Incoming launch request");
                    return HandleWelcomeAsync(launchRequest, logger);

                case IntentRequest intentRequest:
                    logger.LogLine($"Incoming intent {intentRequest.Intent.Name}");
                    return HandleIntentAsync(intentRequest, logger);

                case SessionEndedRequest sessionEndRequest:
                    logger.LogLine("Session ending: " + sessionEndRequest.Reason.ToString());
                    return Task.FromResult(ResponseBuilder.Empty());

                default:
                    throw new NotImplementedException();
            }
        }

        private Task<SkillResponse> HandleWelcomeAsync(LaunchRequest launchRequest, ILambdaLogger logger)
        {
            var response = ResponseBuilder.Tell(new PlainTextOutputSpeech()
            {
                Text = "What launch do you want to learn about?"
            });
            response.Response.ShouldEndSession = false;

            return Task.FromResult(response);
        }

        private Task<SkillResponse> HandleIntentAsync(IntentRequest request, ILambdaLogger logger)
        {
            switch (request.Intent.Name)
            {
                case "NextLaunchIntent": return _nextLaunchIntentHandler.HandleAsync(request, logger);
                case "AMAZON.HelpIntent": return HandleHelpIntentAsync(request, logger);
                case "AMAZON.StopIntent": return HandleStopIntentAsync(request, logger);
                case "AMAZON.CancelIntent": return HandleStopIntentAsync(request, logger);

                default:
                    logger.LogLine($"Unknown intent request name '{request.Intent.Name}'");

                    var response = ResponseBuilder.Tell(new PlainTextOutputSpeech
                    {
                        Text = "I'm sorry, I'm not sure what you mean."
                    });

                    return Task.FromResult(response);
            }
        }

        private Task<SkillResponse> HandleHelpIntentAsync(IntentRequest request, ILambdaLogger logger)
        {
            var response = ResponseBuilder.Tell(new PlainTextOutputSpeech
            {
                Text = "I can tell you when and where upcoming launches are, and also when agencies like NASA or Space X are launching next. What launch do you want to know about?"
            });
            response.Response.ShouldEndSession = false;

            return Task.FromResult(response);
        }

        private Task<SkillResponse> HandleStopIntentAsync(IntentRequest request, ILambdaLogger logger)
        {
            var response = ResponseBuilder.Empty();
            response.Response.ShouldEndSession = true;

            return Task.FromResult(response);
        }
    }
}
