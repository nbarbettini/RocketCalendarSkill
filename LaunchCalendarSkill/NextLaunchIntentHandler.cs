using System;
using System.Linq;
using System.Threading.Tasks;
using Alexa.NET;
using Alexa.NET.Request.Type;
using Alexa.NET.Response;
using Amazon.Lambda.Core;

namespace LaunchCalendarSkill
{
    public sealed class NextLaunchIntentHandler
    {
        private LaunchLibraryApi.Launch[] _cachedUpcomingLaunches;
        private DateTimeOffset? _cacheTimestamp;

        public NextLaunchIntentHandler()
        {
            _cachedUpcomingLaunches = new LaunchLibraryApi.Launch[0];
        }

        public async Task<SkillResponse> HandleAsync(IntentRequest request, ILambdaLogger logger)
        {
            var agencyName = request.Intent.Slots?.FirstOrDefault(slot => slot.Key == "agency").Value?.Value;
            var agencyNameExists = !string.IsNullOrEmpty(agencyName);

            var agencyId = GetAgencyId(agencyName);

            if (agencyNameExists && agencyId == null)
            {
                // The user said something weird
                var response = ResponseBuilder.Tell(new PlainTextOutputSpeech
                {
                    Text = "I didn't quite get that. Which launch do you want to learn about?"
                });
                response.Response.ShouldEndSession = false;
                return response;
            }

            var responseSpeech = string.Empty;

            try
            {
                var upcomingLaunches = await GetUpcomingLaunchesFromCache(logger);

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
                logger.LogLine($"Exception caught: {ex.GetType().Name}");
                logger.LogLine(ex.Message);
                responseSpeech = "Sorry, I wasn't able to retrieve the next launch.";
            }

            return ResponseBuilder.Tell(new SsmlOutputSpeech()
            {
                Ssml = $"<speak>{SsmlSanitizer.Sanitize(responseSpeech)}</speak>"
            });
        }

        private async Task<LaunchLibraryApi.Launch[]> GetUpcomingLaunchesFromCache(ILambdaLogger lambdaLogger)
        {
            bool shouldBeRefreshed = _cacheTimestamp == null || DateTimeOffset.UtcNow.Subtract(_cacheTimestamp.Value) > TimeSpan.FromHours(4);

            if (shouldBeRefreshed)
            {
                lambdaLogger.LogLine("Cache is stale/empty, getting latest launch data...");
                _cachedUpcomingLaunches = await GetUpcomingLaunches(lambdaLogger);
                _cacheTimestamp = DateTimeOffset.UtcNow;
            }

            return _cachedUpcomingLaunches;
        }

        private Task<LaunchLibraryApi.Launch[]> GetUpcomingLaunches(ILambdaLogger lambdaLogger)
        {
            var launchLibraryClient = new LaunchLibraryApi.LaunchLibraryClient();
            return launchLibraryClient.GetLaunches(startDate: DateTimeOffset.UtcNow, limit: 100, logger: new LambdaLoggerAdapter(lambdaLogger));
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

                case "BLUE ORIGIN": return 141;

                default: return null;
            }
        }
    }
}
