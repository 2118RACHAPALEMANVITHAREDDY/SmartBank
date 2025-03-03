namespace SmartBank.DTOs
{
    public class TransactionDTO
    {
        public int TransactionId { get; set; }
        public int? UserId { get; set; }
        //public string Username { get; set; }
        public string TransactionType { get; set; }
        public decimal Amount { get; set; }
        public DateTime TransactionDate { get; set; }
        public string ModeOfTransaction { get; set; }

        // Method to calculate the balance
        public static decimal CheckBalance(int userId, List<TransactionDTO> transactions)
        {
            var userTransactions = transactions.Where(t => t.UserId == userId);
            var balance = userTransactions.Sum(t => t.TransactionType == "Credit" ? t.Amount : -t.Amount);
            return balance;
        }
    }
}
