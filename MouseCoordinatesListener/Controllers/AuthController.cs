using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using MouseCoordinatesListener.Models;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace MouseCoordinatesListener.Controllers
{
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly RepositoryContext _context;
        private readonly IConfiguration _configuration;  
        private readonly ILogger<AuthController> _logger;
        
        public AuthController(RepositoryContext ?context, 
            IConfiguration configuration,
            ILogger<AuthController> logger)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpPost("/api/signin")]
        public async Task<IActionResult> SignIn ([FromBody]User user)
        {
            _logger.LogInformation(_configuration.GetConnectionString("mousecoord"));
            _logger.LogInformation($"Аутентификация пользователя: {user.Email}");

            var section = _configuration.GetSection("Role:Admin");
            var admins = section.Get<List<string>>();

            User findUser = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == user.Email && u.Password == user.Password);
            
            var authClaims = new List<Claim>  
            {  
                new Claim(ClaimTypes.Name, findUser.Email),  
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),  
            };

            authClaims.Add(admins.Contains(user.Email)
                ? new Claim(ClaimTypes.Role, Role.Admin.ToString())
                : new Claim(ClaimTypes.Role, Role.User.ToString()));

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));  
            
            var token = new JwtSecurityToken(  
                issuer: _configuration["JWT:ValidIssuer"],  
                audience: _configuration["JWT:ValidAudience"],  
                expires: DateTime.Now.AddHours(3),  
                claims: authClaims,  
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)  
            );  
        
            return Ok(new  
            {  
                token = new JwtSecurityTokenHandler().WriteToken(token),  
                expiration = token.ValidTo,
                role = authClaims[2].Value,
                findUser
            });  
        }
        
        [AllowAnonymous]
        [HttpPost("/api/register")]
        public async Task<IActionResult> Register([FromBody]User user)
        {
            if (ModelState.IsValid)
            {
                User findUser = await _context.Users.
                    FirstOrDefaultAsync(u => u.Email == user.Email);
                if (findUser == null)
                {
                    _context.Users.Add(new User {
                        Email = user.Email, Password = user.Password, Name = user.Name, Age = user.Age, 
                        Skype = user.Skype, WhatsApp = user.WhatsApp, PhoneNumber = user.PhoneNumber});

                    await _context.SaveChangesAsync();
                }
            }
           
            return Ok();
        }
    }
}