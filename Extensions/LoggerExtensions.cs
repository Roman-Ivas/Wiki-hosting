using System.Diagnostics.CodeAnalysis;

namespace viki_01.Extensions;

[SuppressMessage("Usage", "CA2254:Шаблон должен быть статическим выражением")] // warning cannot be solved properly for such functionality and should be ignored
public static class LoggerExtensions
{
    public static void LogActionInformation<T>(this ILogger<T> logger,
        string httpMethod,
        string actionName,
        string message,
        params object[] args)
    {
        logger.LogInformation($"[{httpMethod}] {actionName}: {message}", args);
    }

    public static void LogActionWarning<T>(this ILogger<T> logger,
        string httpMethod,
        string actionName,
        string message,
        params object[] args)
    {
        logger.LogWarning($"[{httpMethod}] {actionName}: {message}", args);
    }

    public static void LogActionError<T>(this ILogger<T> logger,
        string httpMethod,
        string actionName,
        string message,
        params object[] args)
    {
        logger.LogError($"[{httpMethod}] {actionName}: {message}", args);
    }

    public static void LogActionDebug<T>(this ILogger<T> logger,
        string httpMethod,
        string actionName,
        string message,
        params object[] args)
    {
        logger.LogDebug($"[{httpMethod}] {actionName}: {message}", args);
    }

    public static void LogActionCritical<T>(this ILogger<T> logger,
        string httpMethod,
        string actionName,
        string message,
        params object[] args)
    {
        logger.LogCritical($"[{httpMethod}] {actionName}: {message}", args);
    }
}