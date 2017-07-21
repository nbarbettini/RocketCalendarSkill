using System;
using Amazon.Lambda.Core;
using LaunchCalendarSkill.LaunchLibraryApi;

namespace LaunchCalendarSkill
{
    public sealed class LambdaLoggerAdapter : ISimpleLogger
    {
        private readonly ILambdaLogger _lambdaLogger;

        public LambdaLoggerAdapter(ILambdaLogger lambdaLogger)
        {
            _lambdaLogger = lambdaLogger;
        }

        public void LogLine(string message) => _lambdaLogger.LogLine(message);
    }
}
