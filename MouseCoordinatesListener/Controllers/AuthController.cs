using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MouseCoordinatesListener.Models;

namespace MouseCoordinatesListener.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private RepositoryContext _context;

        public AuthController(RepositoryContext context)
        {
            _context = context;
        }
        
        [HttpGet(Name = "signin")]
        public async Task<IActionResult> SignIn (User user)
        {
            if (ModelState.IsValid)
            {
                User findUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == user.Email && u.Password == user.Password);

                if (findUser == null)
                    return NotFound();
               
                await Authenticate(user.Email);
            }
            return Ok(user);
        }
        
        [HttpPost(Name = "register")]
        public async Task<IActionResult> Register([FromBody]User user)
        {
            if (ModelState.IsValid)
            {
                User findUser = await _context.Users.
                    FirstOrDefaultAsync(u => u.Email == user.Email);
                if (findUser == null)
                {
                    _context.Users.Add(new User {Email = user.Email, Password = user.Password});
                    
                    await Authenticate(user.Email); 
                    
                    return CreatedAtRoute("User", new { id = user.Id }, user);
                }
            }

            return Ok();
        }

        private async Task Authenticate(string userName)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, userName)
            };
				ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", 
                ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("SignIn", "Auth");
        }
    }
}