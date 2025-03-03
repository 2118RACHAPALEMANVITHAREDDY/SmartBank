 using System.ComponentModel.DataAnnotations;

namespace SmartBank.DTOs
{
    public class SignUpDTO
    {
            public int UserId { get; set; }
            public string Username { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }

            public string Adharcard { get; set; }
            public string Pancard { get; set; }
            public string PhoneNo { get; set; }

    }
}
