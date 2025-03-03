using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartBank.DTOs;
using SmartBank.Models;
using System.Linq;

namespace SmartBank.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CheckbookController : ControllerBase
    {
        private readonly EFCoreDbContext _context;

        public CheckbookController(EFCoreDbContext context)
        {
            _context = context;
        }

        
        [HttpPost]
        public IActionResult RequestCheckbook(int userId)
        {
            try
            {
                var requestedAsset = _context.Asset
                    .Where(a => a.UserId == userId && a.AssetType == "Checkbook")
                    .OrderByDescending(a => a.AssetId) // Order by primary key in descending order
                    .FirstOrDefault();
                var requestUser = _context.Users
                    .Where(e => e.UserId == userId)
                    .OrderByDescending(e => e.UserId) // Order by primary key in descending order
                    .FirstOrDefault();

                if (requestedAsset == null)
                {
                    return BadRequest("You must request a checkbook from the assets first.");
                }

                if (requestedAsset.Status != "Approved")
                {
                    return BadRequest("The requested asset is not approved.");
                }

                var checkbook = new Checkbook
                {
                    AssetId = requestedAsset.AssetId,
                    CheckbookNumber = GenerateCheckbookNumber(),
                    IssueDate = DateTime.Now,
                    AccountNo = requestUser.AccountNo,
                    Username = requestUser.Username // Ensure this property is set
                };

                _context.Checkbooks.Add(checkbook);
                _context.SaveChanges();

                // Delete the asset request after creating the checkbook
                _context.Asset.Remove(requestedAsset);
                _context.SaveChanges();

                return Ok(new CheckbookDTO
                {
                    CheckbooksId = checkbook.CheckbooksId,
                    AssetId = checkbook.AssetId,
                    CheckbookNumber = checkbook.CheckbookNumber,
                    IssueDate = checkbook.IssueDate
                });
            }
            catch (DbUpdateException dbEx)
            {
                // Log the exception (you can use a logging framework like Serilog, NLog, etc.)
                
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while updating the database. Please try again later.");
            }
            catch (Exception ex)
            {
                // Log the exception (you can use a logging framework like Serilog, NLog, etc.)
                
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred. Please try again later.");
            }
        }

        private string GenerateCheckbookNumber()
        {
            // Generate a unique checkbook number
            var random = new Random();
            var checkbookNumber = new char[10];
            for (int i = 0; i < checkbookNumber.Length; i++)
            {
                checkbookNumber[i] = (char)('0' + random.Next(10));
            }
            return new string(checkbookNumber);
        }
    }
}