namespace BeShare.Api.Models
{
    public class BlackList
    {
        public int Id { get; set; }
        public string MD5Hash { get; set; }
        public DateTime BlacklistedDate { get; set; }
        public string Reason { get; set; }
    }
}
