using Microsoft.AspNetCore.Identity;

public class ApplicationUser : IdentityUser
{
    public required string Name { get; set; }
    public required string  Surname { get; set; }
    public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;
}