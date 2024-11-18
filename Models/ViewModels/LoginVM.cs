using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace OnlineMarketplace.Models;

public class LoginVM
{
    [DisplayName("Email address")]
    [EmailAddress]
    [Required(ErrorMessage = "Email address is required")]
    public string EmailAddress { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [DisplayName("Remember me")]
    public bool RememberMe { get; set; }
}
