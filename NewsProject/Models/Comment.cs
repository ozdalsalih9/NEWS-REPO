namespace NewsProject.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public int NewsId { get; set; }
        public int UserId { get; set; }
        public DateTime CommentDate { get; set; }
    }
}
