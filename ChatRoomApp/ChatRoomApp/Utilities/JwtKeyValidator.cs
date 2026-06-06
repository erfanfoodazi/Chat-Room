using ChatRoomApp.Configuration;

namespace ChatRoomApp.Utilities;

public static class JwtKeyValidator
{
    private const string DefaultKeyWarning = "CHANGE-THIS-TO-A-SECURE-RANDOM-KEY";
    private const string DevelopmentKeyWarning = "SuperSecureDevKey";

    public static void ValidateJwtSettings(JwtSettings settings, IWebHostEnvironment environment)
    {
        if (settings == null)
            throw new ArgumentNullException(nameof(settings));

        if (string.IsNullOrWhiteSpace(settings.Key))
            throw new InvalidOperationException("JWT Key is not configured.");

        if (string.IsNullOrWhiteSpace(settings.Issuer))
            throw new InvalidOperationException("JWT Issuer is not configured.");

        if (string.IsNullOrWhiteSpace(settings.Audience))
            throw new InvalidOperationException("JWT Audience is not configured.");

        if (settings.ExpiryMinutes <= 0)
            throw new InvalidOperationException("JWT ExpiryMinutes must be greater than 0.");

        // Warn about insecure keys in development
        if (!environment.IsProduction())
        {
            if (settings.Key.Contains(DefaultKeyWarning) || settings.Key.Contains(DevelopmentKeyWarning))
            {
                Console.WriteLine("⚠ WARNING: Using default or development JWT key. Generate a secure random key for production.");
                Console.WriteLine("💡 Tip: Use 'openssl rand -base64 32' to generate a secure 256-bit key.");
            }
        }
        else
        {
            // Production validation
            if (settings.Key.Length < 32)
                throw new InvalidOperationException("JWT Key must be at least 32 characters long for production.");

            if (settings.Key.Contains(DefaultKeyWarning) || settings.Key.Contains(DevelopmentKeyWarning))
                throw new InvalidOperationException("Cannot use default or development JWT key in production.");
        }
    }

    public static string GenerateSecureKeyHint()
    {
        return "Generate a secure JWT key using: openssl rand -base64 32";
    }
}