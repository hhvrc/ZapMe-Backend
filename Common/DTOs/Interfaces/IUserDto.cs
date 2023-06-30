using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZapMe.Enums;

namespace ZapMe.DTOs.Interfaces;

/// <summary>
/// This is only here to ensure that the User derived DTOs don't differ in structure
/// </summary>
public interface IUserDto
{
    public Guid Id { get; init; }
    public string Username { get; init; }
    public Uri? AvatarUrl { get; init; }
    public Uri? BannerUrl { get; init; }
    public UserStatus Status { get; init; }
    public string StatusText { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime LastOnline { get; init; }
}
