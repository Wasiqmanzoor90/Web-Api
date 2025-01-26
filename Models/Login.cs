using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class Login
    {
        [EmailAddress]
public required string Email {set; get;}
public required string Password {set; get;}

    }
}
