using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using ChatRoomApp.Configuration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace ChatRoomApp.Authentication;

public class JwtAuthenticationHandler : AuthenticationHandler<JwtAuthenticationOptions>
{
    private readonly JwtSettings _jwtSettings;

    public JwtAuthenticationHandler(
        IOptionsMonitor<JwtAuthenticationOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        IOptions<JwtSettings> jwtSettings)
        : base(options, logger, encoder)
    {
        _jwtSettings = jwtSettings.Value;
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var token = ExtractToken();
        if (string.IsNullOrWhiteSpace(token))
            return Task.FromResult(AuthenticateResult.NoResult());

        try
        {
            var principal = ValidateToken(token);
            var ticket = new AuthenticationTicket(principal, JwtAuthenticationDefaults.AuthenticationScheme);
            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
        catch (Exception ex)
        {
            return Task.FromResult(AuthenticateResult.Fail(ex));
        }
    }

    private string? ExtractToken()
    {
        var authorization = Request.Headers.Authorization.ToString();
        if (authorization.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            return authorization["Bearer ".Length..].Trim();

        if (Request.Path.StartsWithSegments("/chathub"))
            return Request.Query["access_token"];

        return null;
    }

    private ClaimsPrincipal ValidateToken(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        
        // Read token without validation first to check algorithm
        var jwtToken = handler.ReadJwtToken(token);
        
        // Ensure the token uses the expected algorithm (HS256)
        var alg = jwtToken.Header.Alg;
        if (alg != SecurityAlgorithms.HmacSha256)
        {
            throw new SecurityTokenException($"Invalid token algorithm: {alg}. Expected: {SecurityAlgorithms.HmacSha256}");
        }
        
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = _jwtSettings.Issuer,
            ValidAudience = _jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key)),
            ClockSkew = TimeSpan.FromMinutes(5), // Allow 5 minutes clock skew for server time differences
            ValidAlgorithms = new[] { SecurityAlgorithms.HmacSha256 }
        };

        return handler.ValidateToken(token, validationParameters, out _);
    }
}
