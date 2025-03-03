namespace SmartBank.DTOs
{
    public class CheckbookDTO
    {
        public int CheckbooksId { get; set; }
        public int? AssetId { get; set; }
        public string CheckbookNumber { get; set; }
        public DateTime IssueDate { get; set; }

        //public int AccountNo { get; set; }
        //public int Username { get; set; }
    
    }

    public class CreditCardDTO
    {
        public int CreditCardId { get; set; }
        public int? AssetId { get; set; }
        public string CardNumber { get; set; }
        public DateTime ExpiryDate { get; set; }

        public string AccountNo { get; set; }
        public string Username { get; set; }

        public int CibilScore { get; set; }
    }

    public class DebitCardDTO
    {
        public int DebitCardId { get; set; }
        public int? AssetId { get; set; }
        public string CardNumber { get; set; }
        public DateTime ExpiryDate { get; set; }

        public string AccountNo { get; set; }
        public string Username { get; set; }


    }
}