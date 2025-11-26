using System;
using Domain.Interfaces;

namespace Infrastructure.Logging;

public class ConsoleLogger : ILogger
{
    private readonly bool _isEnabled;

    public ConsoleLogger(bool isEnabled = true)
    {
        _isEnabled = isEnabled;
    }

    public void LogInformation(string message)
    {
        if (!_isEnabled) return;
        
        Console.WriteLine($"[INFO] {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} - {message}");
    }

    public void LogWarning(string message)
    {
        if (!_isEnabled) return;
        
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"[WARN] {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} - {message}");
        Console.ResetColor();
    }

    public void LogError(string message, Exception exception = null)
    {
        if (!_isEnabled) return;
        
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"[ERROR] {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} - {message}");
        
        if (exception != null)
        {
            Console.WriteLine($"Exception: {exception.Message}");
            Console.WriteLine($"StackTrace: {exception.StackTrace}");
        }
        
        Console.ResetColor();
    }
}
