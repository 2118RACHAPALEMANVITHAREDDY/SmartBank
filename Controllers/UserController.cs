
using Microsoft.AspNetCore.Mvc;
using SmartBank.DTOs;
using SmartBank.Models;
using System.Text.RegularExpressions;
using System.Linq;

namespace SmartBank.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly EFCoreDbContext _context;

        public UserController(EFCoreDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult CreateUser(UserDTO userDto)
        {
            // Validate Password
            if (!Regex.IsMatch(userDto.Password, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$"))
            {
                return BadRequest("Password must be at least 8 characters long and include a mix of uppercase, lowercase, numbers, and special characters.");
            }

            // Validate AccountNo
            if (!Regex.IsMatch(userDto.AccountNo, @"^\d{14}$"))
            {
                return BadRequest("Account number must be exactly 14 digits.");
            }

            // Validate Adharcard
            if (!Regex.IsMatch(userDto.Adharcard, @"^\d{12}$"))
            {
                return BadRequest("Adharcard must be exactly 12 digits.");
            }

            // Validate Pancard
            if (!Regex.IsMatch(userDto.Pancard, @"^[A-Za-z0-9]{10}$"))
            {
                return BadRequest("Pancard must be exactly 10 alphanumeric characters.");
            }

            // Validate PhoneNo
            if (!Regex.IsMatch(userDto.PhoneNo, @"^\d{10}$"))
            {
                return BadRequest("Phone number must be exactly 10 digits.");
            }

            // Validate Email
            if (!Regex.IsMatch(userDto.Email, @"^[a-zA-Z0-9._%+-]+@gmail\.com$"))
            {
                return BadRequest("Email must be a valid @gmail.com address.");
            }

            var user = new User
            {
                Username = userDto.Username,
                Email = userDto.Email,
                Password = userDto.Password,
                Adharcard = userDto.Adharcard,
                Pancard = userDto.Pancard,
                PhoneNo = userDto.PhoneNo,
                AccountNo = userDto.AccountNo
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            return Ok(user);
        }

        [HttpGet]
        public IActionResult GetUsers()
        {
            var users = _context.Users.ToList();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public IActionResult GetUserById(int id)
        {
            var user = _context.Users.Find(id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateUser(int id, UserDTO userDto)
        {
            var user = _context.Users.Find(id);
            if (user == null)
            {
                return NotFound();
            }

            user.Username = userDto.Username;
            user.Email = userDto.Email;
            user.Password = userDto.Password;
            user.Adharcard = userDto.Adharcard;
            user.Pancard = userDto.Pancard;
            user.PhoneNo = userDto.PhoneNo;
            user.AccountNo = userDto.AccountNo;

            _context.Users.Update(user);
            _context.SaveChanges();

            return Ok(user);
        }

        
    }
}





