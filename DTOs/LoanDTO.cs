namespace SmartBank.DTOs
{
    public class LoanDTO
    {
        public int UserId { get; set; }
        public double LoanAmount { get; set; }
        public int LoanTerm { get; set; } // in months
        public double Income { get; set; }
        public int CibilScore { get; set; }
    }
}


