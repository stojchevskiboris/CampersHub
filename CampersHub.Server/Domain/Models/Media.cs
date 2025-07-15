namespace CampersHub.Server.Domain.Models
{
    public class Media
    {
        public int Id { get; set; }
        public Post RelatedPost { get; set; }
        public string Url { get; set; }
        public string FileType { get; set; }
        public int FileSize { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }
    }
}