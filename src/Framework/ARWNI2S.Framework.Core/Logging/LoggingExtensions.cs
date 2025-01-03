using Microsoft.Extensions.Logging;

namespace ARWNI2S.Framework.Logging
{
    //TODO: Add more logging extensions
    public static class LoggingExtensions
    {
        public static Task WarningAsync(this ILogger logger, string message)
        {
            logger.LogWarning(message, null);
            return Task.CompletedTask;
        }

        public static void Warning(this ILogger logger, string message)
        {
            logger.LogWarning(message, null);
        }
    }
}
