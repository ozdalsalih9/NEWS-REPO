namespace NewsProject.Models;

public class News
{
    public int Id { get; set; }
    public string Header { get; set; }
    public string Content { get; set; }
    public DateTime PublishDate { get; set; }
    public int CategoryId { get; set; }
    public int AuthorId { get; set; }
    public int Views { get; set; }
    public string? Image { get; set; }

    // Navigation properties
    public Author Author { get; set; }
    public Category Category { get; set; } // Category navigation property
}
