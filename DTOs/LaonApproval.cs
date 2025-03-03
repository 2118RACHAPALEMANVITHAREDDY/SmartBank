namespace SmartBank.DTOs
{
    public class LoanApprovalResponseDTO
    {
        public string Status { get; set; }
        public double InterestRate { get; set; }
        public double ApprovedAmount { get; set; }
    }
}
