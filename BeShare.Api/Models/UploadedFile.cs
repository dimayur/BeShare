namespace BeShare.Api.Models
{
    public class UploadedFile
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }
        public string UploadDate { get; set; }
        public User User { get; set; }
    }
}
