namespace SmartBank.DTOs
{
    public class AssetDTO
    {
        public int AssetId { get; set; }
        public int UserId { get; set; }
        public string AssetType { get; set; }
        public DateTime CreationDate { get; set; }
    }
}

