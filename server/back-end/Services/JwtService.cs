using back_end.Models;
using back_end.Models.Api;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace back_end.Services
{
    public class JwtService
    {
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;
        private readonly SignInManager<User> _signInManager;
        private ILogger<JwtService> _logger;

        public JwtService(UserManager<User> userManager, SignInManager<User> signInManager, IConfiguration configuration, ILogger<JwtService> logger)
        {
            _userManager = userManager;
            _configuration = configuration;
            _signInManager = signInManager;
            _logger = logger;
        }

        public async Task<LoginResponseModel?> Authenticate(LoginRequestModel request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
                {
                    _logger.LogWarning("Authentication failed: Email or Password is empty.");
                    return null;
                }

                var userAccount = await _userManager.FindByEmailAsync(request.Email);
                if (userAccount == null)
                {
                    _logger.LogWarning($"Authentication failed: No user found with email {request.Email}");
                    return null;
                }

                var result = await _signInManager.CheckPasswordSignInAsync(userAccount, request.Password, false);
                if (!result.Succeeded)
                {
                    _logger.LogWarning($"Authentication failed: Invalid credentials for email {request.Email}");
                    return null;
                }

                var issuer = _configuration["Jwt:Issuer"];
                var audience = _configuration["Jwt:Audience"];
                var secret = _configuration["Jwt:Secret"];
                var tokenValidityMins = _configuration.GetValue<int>("Jwt:TokenValidityMins");
                var tokenExpiryTimeStamp = DateTime.UtcNow.AddMinutes(tokenValidityMins);

                if (string.IsNullOrEmpty(issuer) || string.IsNullOrEmpty(audience) || string.IsNullOrEmpty(secret))
                {
                    _logger.LogError("JWT configuration is missing required values.");
                    throw new InvalidOperationException("Invalid JWT configuration.");
                }

                var claims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Sub, userAccount.Id.ToString()),
                    new Claim(ClaimTypes.Email, request.Email),
                    new Claim(ClaimTypes.Role, userAccount.UserType.ToString()),
                };

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = tokenExpiryTimeStamp,
                    Issuer = issuer,
                    Audience = audience,
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
                        SecurityAlgorithms.HmacSha256Signature)
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var securityToken = tokenHandler.CreateToken(tokenDescriptor);
                var accessToken = tokenHandler.WriteToken(securityToken);

                return new LoginResponseModel
                {
                    Token = accessToken,
                    SignedInUser = new UserRes
                    {
                        Id = userAccount.Id,
                        FirstName = userAccount.FirstName,
                        LastName = userAccount.LastName,
                        Email = userAccount.Email,
                        UserType = userAccount.UserType
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during authentication for email {Email}", request.Email);
                return null;
            }
        }

    }
}
