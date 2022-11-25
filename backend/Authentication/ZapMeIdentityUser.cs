using Microsoft.AspNetCore.Identity;

namespace ZapMe.Authentication;

public sealed class ZapMeIdentityUser : IdentityUser<Guid>
{
    /// <inheritdoc/>
    public override Guid Id { get; set; } = default!;

    /// <inheritdoc/>
    public override string? UserName { get; set; }

    /// <inheritdoc/>
    public override string? Email { get; set; }

    /// <inheritdoc/>
    public override bool EmailConfirmed { get; set; }

    /// <inheritdoc/>
    public override string? PasswordHash { get; set; }

    /// <inheritdoc/>
    public override bool LockoutEnabled { get; set; }

    /// <inheritdoc/>
    public override DateTimeOffset? LockoutEnd { get; set; }

    /// <inheritdoc/>
    public override int AccessFailedCount { get; set; }

    /// <inheritdoc/>
    public override bool TwoFactorEnabled { get; set; }

    public override string? NormalizedUserName { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public override string? NormalizedEmail { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public override string? SecurityStamp { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public override string? ConcurrencyStamp { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public override string? PhoneNumber { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public override bool PhoneNumberConfirmed { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
}
