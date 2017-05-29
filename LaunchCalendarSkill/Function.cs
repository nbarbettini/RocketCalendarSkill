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
        public Task<SkillResponse> FunctionHandler(SkillRequest input, ILambdaContext context)
        {
            var logger = context.Logger;

            switch (input.Request)
            {
                case LaunchRequest launchRequest:
                    logger.LogLine($"Incoming LaunchRequest");
                    return HandleWelcomeAsync(launchRequest, logger);

                case IntentRequest intentRequest:
                    logger.LogLine($"Incoming IntentRequest {intentRequest.Intent.Name}");
                    return HandleIntentAsync(intentRequest, logger);
            }

            throw new NotImplementedException("Unknown request type.");
        }

        private Task<SkillResponse> HandleWelcomeAsync(LaunchRequest launchRequest, ILambdaLogger logger)
        {
            var response = ResponseBuilder.Tell(new PlainTextOutputSpeech()
            {
                Text = "Welcome! You can ask when the next launch is."
            });

            return Task.FromResult(response);
        }

        private async Task<SkillResponse> HandleIntentAsync(IntentRequest intentRequest, ILambdaLogger logger)
        {
            bool recognizedName = intentRequest.Intent.Name.Equals("NextLaunchIntent", StringComparison.Ordinal);
            if (!recognizedName) throw new ArgumentException("Unkonwn intent");

            var launchLibraryClient = new LaunchLibraryApi.LaunchLibraryClient();

            var responseSpeech = string.Empty;

            try
            {
                var upcomingLaunches = await launchLibraryClient.GetLaunches(startDate: DateTimeOffset.UtcNow, limit: 1);
                var launch = upcomingLaunches.SingleOrDefault();

                responseSpeech = upcomingLaunches.Any()
                    ? $"{launch.Rocket.Name} will be launching from {launch.Location.Name} no earlier than {launch.Net}."
                    : "There aren't any upcoming launches.";
            }
            catch (Exception ex)
            {
                logger.LogLine(ex.Message);
                responseSpeech = "Sorry, I wasn't able to retrieve the next launch.";
            }

            return ResponseBuilder.Tell(new PlainTextOutputSpeech()
            {
                Text = responseSpeech
            });
        }
    }

}
