using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using ZapMe.Constants;
using ZapMe.Data.Models;
using ZapMe.Enums;
using ZapMe.Utils;

namespace ZapMe.Data;

public static class DataSeeders
{
    private static readonly DateTime _CreationTime = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    public static void Seed(ZapMeContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (!context.Images.Any())
            SeedImages(context);
        
        if (!context.Accounts.Any())
            SeedAccounts(context);

        if (File.Exists("passwords.json"))
        {
            Console.WriteLine("Please save the passwords contained in \"passwords.json\" SECURELY, and delete them COMPLETELY from the server.");
            Console.WriteLine("Server will refuse to start as long as that file exists in order to avoid passwords getting leaked");
            Console.WriteLine("Server shutting down...");
            Environment.Exit(0);
        }
    }

    private static readonly Guid _DefaultImageId = Guid.Parse("00000000-0000-0000-0000-000000000001");
    private static void SeedImages([NotNull] ZapMeContext context)
    {
        context.Images.Add(new ImageEntity
        {
            Id = _DefaultImageId,
            Height = 0,
            Width = 0,
            SizeBytes = 0,
            HashSha256 = "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855",
            HashPerceptual = 0,
            UploaderId = null,
            Uploader = null
        });
        context.SaveChanges();
    }

    private static void SeedAccounts([NotNull] ZapMeContext context)
    {
        string[] accounts = new string[] {
            "Admin",
            "System",
            "Support",
            "Moderator",
            "Root",
            "Owner",
            "Developer",
            "Tester",
            "Bot",
            "Backup",
            App.AppName,
            App.AppCreator,
            "SuperUser",
            "Staff",
            "Guest",
            "Anonymous",
            "User",
            "Service",
            "Api",
            "Status"
        };

        Dictionary<string, object> accountPasswords = new Dictionary<string, object>();
        
        for (int i = 0; i < accounts.Length; i++)
        {
            Guid id = Guid.Parse($"00000000-0000-0000-0000-{i + 1:X12}");
            string name = accounts[i];
            string email = $"{name.ToLower()}@{App.Domain}";
            string password = PasswordUtils.GeneratePassword();
            string passwordHash = PasswordUtils.HashPassword(password);

            Console.WriteLine($"Creating account {name} ({email})");

            accountPasswords.Add(name, new { email , password });

            context.Accounts.Add(new AccountEntity
            {
                Id = id,
                Name = name,
                Email = email,
                EmailVerified = true,
                PasswordHash = passwordHash,
                AcceptedTosVersion = Int32.MaxValue,
                ProfilePictureId = _DefaultImageId,
                OnlineStatus = UserOnlineStatus.Online,
                OnlineStatusText = "I'm online!",
                CreatedAt = _CreationTime,
                UpdatedAt = _CreationTime,
                LastOnline = _CreationTime,
            });
        }

        context.SaveChanges();
        File.WriteAllText("passwords.json", JsonSerializer.Serialize(accountPasswords, new JsonSerializerOptions { WriteIndented = true }));
    }
}