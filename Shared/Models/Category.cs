using System.ComponentModel.DataAnnotations;

namespace Shared.Models;

public class Category
{
#pragma warning disable CS8618
    [Key]
    public int CategoryId { get; set; }
    
    [Required]
    [MaxLength(256)]
    public string ThumbnailImagePath { get; set; }
    
    [Required]
    [MaxLength(128)]
    public string Name { get; set; }
    
    [Required]
    [MaxLength(1024)]
    public string Description { get; set; }
    
#pragma warning restore CS8618
}