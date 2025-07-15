namespace CampersHub.Server.Domain.Models
{
    public class Post
    {
        public int Id { get; set; }
        public User Author { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public List<Media> MediaFiles { get; set; }
        public List<PostTag> Tags { get; set; }
        public bool IsPinned { get; set; }
        public int Order { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }

        public Post()
        {
            MediaFiles = new List<Media>();
            Tags = new List<PostTag>();
        }
    }


}
