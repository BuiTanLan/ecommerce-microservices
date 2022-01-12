namespace Identity.Core.Models;

public class RefreshToken
{
    public Guid UserId { get; set; }
    public string Token { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiredAt { get; set; }
    public string CreatedByIp { get; set; }
    public bool IsExpired => DateTime.Now >= ExpiredAt;
    public bool IsRevoked => RevokedAt != null;
    public bool IsActive => !IsRevoked && !IsExpired;
    public DateTime? RevokedAt { get; set; }
    public ApplicationUser ApplicationUser { get; set; }
}
