using Alexa.NET;
using Alexa.NET.Request;
using Alexa.NET.Request.Type;
using Alexa.NET.Response;
using Amazon.Lambda.Core;
using System;
using System.Collections.Generic;
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
            logger.LogLine($"Incoming {input.Request.Type}");

            switch (input.Request)
            {
                case LaunchRequest launchRequest:
                    return HandleWelcomeAsync(launchRequest, logger);

                case IntentRequest intentRequest:
                    logger.LogLine($"Incoming intent {intentRequest.Intent.Name}");
                    return HandleIntentAsync(intentRequest, logger);

                default:
                    throw new NotImplementedException();
            }
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
            bool validIntent = "NextLaunchIntent".Equals(intentRequest.Intent.Name, StringComparison.Ordinal);
            if (!validIntent) throw new ArgumentException("Unknown intent.");

            var agencyName = intentRequest.Intent.Slots?.FirstOrDefault(slot => slot.Key == "agency").Value?.Value;
            var agencyId = GetAgencyId(agencyName);

            var responseSpeech = string.Empty;

            try
            {
                var launchLibraryClient = new LaunchLibraryApi.LaunchLibraryClient();
                var limit = agencyId == null ? 1 : 25;
                var upcomingLaunches = await launchLibraryClient.GetLaunches(startDate: DateTimeOffset.UtcNow, limit: limit);

                var launch = agencyId == null
                    ? upcomingLaunches.FirstOrDefault()
                    : upcomingLaunches.FirstOrDefault(l => l.Rocket.Agencies.Any(a => a.Id == agencyId));

                if (launch == null)
                {
                    responseSpeech = $"I can't find any upcoming {(agencyId == null ? "" : agencyName)} launches.";
                }
                else
                {
                    responseSpeech = $"{launch.Rocket.Name} will be launching the {launch.Missions.First().Name} mission <break strength=\"medium\"/> from {launch.Location.Name} <break strength=\"medium\"/> no earlier than <say-as interpret-as=\"date\">????{launch.Net.Value.ToString("MMdd")}</say-as>.";
                }
                
            }
            catch (Exception ex)
            {
                logger.LogLine(ex.Message);
                responseSpeech = "Sorry, I wasn't able to retrieve the next launch.";
            }

            return ResponseBuilder.Tell(new SsmlOutputSpeech()
            {
                Ssml = $"<speak>{responseSpeech}</speak>"
            });
        }

        private static int? GetAgencyId(string agencyName)
        {
            if (string.IsNullOrEmpty(agencyName)) return null;

            switch (agencyName.ToUpper())
            {
                case "SPACE X":
                case "SPACEX": return 121;

                case "NASA": return 44;
                case "JAXA": return 37;

                case "ORBITAL ATK":
                case "ORBITAL": return 179;

                case "UNITED LAUNCH ALLIANCE":
                case "ULA": return 124;

                default: return null;
            }
        }
    }

}
