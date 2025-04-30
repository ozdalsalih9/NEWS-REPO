using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace NewsProject.Models;

public class News
{
    public int Id { get; set; }
    public string Header { get; set; }
    public string Content { get; set; }
    public DateTime PublishDate { get; set; }
    [Display(Name = "Category")]
    public int CategoryId { get; set; }
    [Display(Name = "Author")]
    public int AuthorId { get; set; }
    public int Views { get; set; }
    public string? Image { get; set; }

    // Navigation properties
    [ValidateNever]
    public Author Author { get; set; }

    [ValidateNever]
    public Category Category { get; set; } // Category navigation property
}
