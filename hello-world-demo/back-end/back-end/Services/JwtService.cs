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

        public JwtService(UserManager<User> userManager, SignInManager<User> signInManager, IConfiguration configuration) {
            _userManager = userManager;
            _configuration = configuration;
            _signInManager = signInManager;
        }

        public async Task<LoginResponseModel?> Authenticate(LoginRequestModel request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
                return null;

            var UserAccount = await _userManager.FindByEmailAsync(request.Email);
            if (UserAccount == null)
                return null;

            var result = await _signInManager.CheckPasswordSignInAsync(UserAccount, request.Password, false);
            if (!result.Succeeded)
                return null;

            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];
            var secret = _configuration["Jwt:Secret"];
            var tokenValidityMins = _configuration.GetValue<int>("Jwt:TokenValidityMins");
            var tokenExpiryTimeStamp = DateTime.UtcNow.AddMinutes(tokenValidityMins);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, UserAccount.Id.ToString()),
                new Claim(ClaimTypes.Email, request.Email),
                new Claim(ClaimTypes.Role, UserAccount.UserType.ToString()),
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
                    Id  = UserAccount.Id,
                    FirstName =  UserAccount.FirstName,
                    LastName = UserAccount.LastName,
                    Email = UserAccount.Email,
                    UserType = UserAccount.UserType
                }
            };

        }
    }
}
