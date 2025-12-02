using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Security.Claims;

namespace Horta_Api.Infrastructure.Middleware
{
    public class WebSocketAuthMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<WebSocketAuthMiddleware> _logger;

        public WebSocketAuthMiddleware(RequestDelegate next, ILogger<WebSocketAuthMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.WebSockets.IsWebSocketRequest)
            {
                _logger.LogInformation("Interceptando requisição WebSocket: {Path}", context.Request.Path);

                if (context.Request.Path.StartsWithSegments("/ws"))
                {
                    var isAuthenticated = await ValidateWebSocketTokenAsync(context);
                    if (!isAuthenticated)
                    {
                        await RejectWebSocketConnectionAsync(context, "Não autorizado");
                        return;
                    }
                }
            }

            await _next(context);
        }

        private async Task<bool> ValidateWebSocketTokenAsync(HttpContext context)
        {
            try
            {
                // Tenta obter o token de diferentes formas
                var accessToken = GetTokenFromRequest(context);

                if (string.IsNullOrEmpty(accessToken))
                {
                    _logger.LogWarning("Token de acesso não fornecido para WebSocket");
                    return false;
                }

                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET");

                if (string.IsNullOrEmpty(jwtSecret))
                {
                    _logger.LogError("JWT_SECRET não configurado");
                    return false;
                }

                var key = Encoding.UTF8.GetBytes(jwtSecret);

                tokenHandler.ValidateToken(accessToken,
                    new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero
                    },
                    out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var userId = jwtToken.Claims.FirstOrDefault(x => x.Type == "id")?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogWarning("Token válido mas sem userId");
                    return false;
                }

                
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, userId),
                    new Claim(ClaimTypes.Name, jwtToken.Claims.FirstOrDefault(x => x.Type == "name")?.Value ?? ""),
                    new Claim("email", jwtToken.Claims.FirstOrDefault(x => x.Type == "email")?.Value ?? "")
                };

                
                foreach (var claim in jwtToken.Claims.Where(c => c.Type != "id" && c.Type != "name" && c.Type != "email"))
                {
                    claims.Add(new Claim(claim.Type, claim.Value));
                }

                var appIdentity = new ClaimsIdentity(claims, "jwt");
                context.User = new ClaimsPrincipal(appIdentity);

                _logger.LogInformation("Usuário autenticado via WebSocket: {UserId}", userId);
                return true;
            }
            catch (SecurityTokenExpiredException)
            {
                _logger.LogWarning("Token expirado em requisição WebSocket");
                return false;
            }
            catch (SecurityTokenValidationException ex)
            {
                _logger.LogWarning("Token inválido em requisição WebSocket: {Message}", ex.Message);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado na validação do token WebSocket");
                return false;
            }
        }

        private string GetTokenFromRequest(HttpContext context)
        {

            var tokenFromQuery = context.Request.Query["access_token"].FirstOrDefault();
            if (!string.IsNullOrEmpty(tokenFromQuery))
                return tokenFromQuery;

            var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
            if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
                return authHeader.Substring(7);

            var protocolHeader = context.Request.Headers["Sec-WebSocket-Protocol"].FirstOrDefault();
            if (!string.IsNullOrEmpty(protocolHeader))
            {
                var parts = protocolHeader.Split(',');
                if (parts.Length == 2 && parts[0].Trim() == "access_token")
                    return parts[1].Trim();
            }

            return null;
        }

        private async Task RejectWebSocketConnectionAsync(HttpContext context, string reason)
        {
            context.Response.StatusCode = 401;
            context.Response.Headers.Add("Content-Type", "text/plain");
            await context.Response.WriteAsync($"WebSocket connection rejected: {reason}");
            _logger.LogWarning("WebSocket connection rejected for {Path}: {Reason}", context.Request.Path, reason);
        }
    }
}

