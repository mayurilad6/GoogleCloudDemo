using System.ComponentModel.DataAnnotations;

namespace HCIHelp.Models
{
    public class ComplaintModel
    {
        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        public string Complaint { get; set; }

        [Required(ErrorMessage = "Please select file.")]
        [Display(Name = "Browse File")]
        public IFormFile[]? files { get; set; }
    }
}
