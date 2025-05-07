namespace NewsProject.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public int NewsId { get; set; }
        public int UserId { get; set; }
        public DateTime CommentDate { get; set; }

        public bool CanBeDeletedBy(int userId, bool isAdmin)
        {
            return isAdmin || UserId == userId;
        }

        public virtual User User { get; set; }
    }
}
