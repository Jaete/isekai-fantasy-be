namespace IsekaiFantasyBE.Models.Users;

public sealed record UserStatus(string Value)
{
    public static readonly UserStatus Active = new("active");
    public static readonly UserStatus Banned = new ("banned");
    public static readonly UserStatus Silenced = new("silenced");
    
    public override string ToString() => Value;
    public static implicit operator string(UserStatus status) => status.Value;
}