using Microsoft.AspNetCore.Identity;

namespace Identity.Api;

public class ApplicationIdentityUser : IdentityUser
{
    [PersonalData]
    public string FirstName { get; set; }
    [PersonalData]
    public string LastName { get; set; }
    public string Address { get; set; }
    public bool IsAdmin { get; set; }
}