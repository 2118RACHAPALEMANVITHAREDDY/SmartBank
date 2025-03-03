using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using SmartBank.Models;
using SmartBank.DTOs;

namespace SmartBank.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly EFCoreDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(EFCoreDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        

        [HttpPost("signup")]
        public async Task<IActionResult> Signup(SignUpDTO signupdto)
        {
            // Check if the username or email already exists
            if (await _context.Users.AnyAsync(u => u.Username == signupdto.Username || u.Email == signupdto.Email))
            {
                return BadRequest("Username or Email already exists.");
            }

            var user = new User
            {
                Username = signupdto.Username,
                Email = signupdto.Email,
                Password = signupdto.Password,
                Adharcard = signupdto.Adharcard,
                Pancard = signupdto.Pancard,
                PhoneNo = signupdto.PhoneNo,
                AccountNo = await GenerateUniqueAccountNumber(),
                Role = "User", // Default role
                Balance = 0 // Initial balance
            };

            // Add the user to the database
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok("User registered successfully.");
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO loginDTO)
        {
            // Find the user by email
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == loginDTO.Email);

            if (user == null || (loginDTO.Password != user.Password))
            {
                return Unauthorized("Invalid email or password.");
            }

            // Generate JWT token
            var token = GenerateJwtToken(user);

            // Authentication successful
            return Ok(new { token });
        }

       

        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["jwtSettings:key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["jwtSettings:issuer"],
                audience: _configuration["jwtSettings:audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private async Task<string> GenerateUniqueAccountNumber()
        {
            string accountNumber;
            do
            {
                accountNumber = GenerateAccountNumber();
            } while (await _context.Users.AnyAsync(u => u.AccountNo == accountNumber));

            return accountNumber;
        }

        private string GenerateAccountNumber()
        {
            var random = new Random();
            var accountNumber = new char[14];
            for (int i = 0; i < accountNumber.Length; i++)
            {
                accountNumber[i] = (char)('0' + random.Next(10));
            }
            return new string(accountNumber);
        }
    }
}