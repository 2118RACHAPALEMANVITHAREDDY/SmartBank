namespace SmartBank.DTOs
{
    public class AdminDTO
    {
        public int RequestId { get; set; }
        public int? UserId { get; set; }
        public string RequestType { get; set; }
        public string Status { get; set; }
    }
}
