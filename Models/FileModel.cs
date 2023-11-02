using System.ComponentModel.DataAnnotations;

namespace HCIHelp.Models
{
    public class FileModel
    {
        [Required(ErrorMessage = "Please select file.")]
        [Display(Name = "Browse File")]
        public IFormFile[]? files { get; set; }

    }
}
