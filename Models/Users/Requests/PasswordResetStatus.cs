namespace IsekaiFantasyBE.Models.Users.Requests;

public sealed record PasswordResetStatus(string Value)
{
    public static readonly PasswordResetStatus Pending = new ("Pending");
    public static readonly PasswordResetStatus Done = new("Done");
    
    public override string ToString() => Value;
    public static implicit operator string(PasswordResetStatus status) => status.Value;
}