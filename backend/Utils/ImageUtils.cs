using ZapMe.Data.Models;
using OneOf;
using System.Security.Cryptography;

namespace ZapMe.Utils;

public static class ImageUtils
{
    public readonly record struct ParseResult(bool Ok, uint Width, uint Height, string Hash, ulong Phash);

    public static async Task<ParseResult> ParseFromStreamAsync(Stream stream)
    {
        try
        {
            stream.Position = 0;
            using Image image = await Image.LoadAsync(stream);

            int width = image.Width;
            int height = image.Height;
            int frames = image.Frames.Count;

            if (height > 0 && width > 0)
            {
                string hash = await HashingUtils.Sha256_StringAsync(stream);
                return new ParseResult(true, (uint)width, (uint)height, hash, 0);
            }
        }
        catch (Exception)
        {
        }

        return new ParseResult(false, 0, 0, "", 0);
    }
}
