using System.ComponentModel.DataAnnotations;

namespace UPFCON.Requests;



public class PaperDto
{
    
    [Required, MaxLength(100)] public required string Title { get; set; }
    
    [Required, MaxLength(4096)] public required string Abstract { get; set; }
    
    [Required] public DateTime PublicationDate { get; set; }
    
    [Required] public required IFormFile PaperFile { get; set; }
    
    [Required, MaxLength(255)] public required string Keywords {get; set;}
    
    [Required]
    public IList<ContributorDto> Contributors { get; set; } = new List<ContributorDto>();
    
    public PaperDto(){}

  
}