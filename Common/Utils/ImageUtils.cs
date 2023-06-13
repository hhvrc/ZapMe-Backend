using OneOf;
using ZapMe.DTOs;

namespace ZapMe.Utils;

public static class ImageUtils
{
    public readonly record struct ParseResult(uint Width, uint Height, uint FrameCount, string MimeType);

    /// <summary>
    /// Parses and rewrites an image from a stream
    /// </summary>
    /// <param name="inputStream"></param>
    /// <param name="outputStream"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Parse result or ErrorDetails(400)</returns>
    public static async Task<OneOf<ParseResult, ErrorDetails>> ParseAndRewriteFromStreamAsync(Stream inputStream, Stream? outputStream, CancellationToken cancellationToken = default)
    {
        try
        {
            using Image image = await Image.LoadAsync(inputStream, cancellationToken);

            int width = image.Width;
            int height = image.Height;
            int frameCount = image.Frames.Count;

            if (height < 0 || width < 0 || frameCount <= 0)
            {
                return new ErrorDetails(StatusCodes.Status400BadRequest, "Malformed image", "Image has invalid dimensions");
            }

            // Clear metadata
            image.Metadata.ExifProfile = null;
            image.Metadata.IccProfile = null;
            image.Metadata.IptcProfile = null;
            image.Metadata.XmpProfile = null;

            // Write
            if (frameCount == 1)
            {
                if (outputStream is not null) await image.SaveAsWebpAsync(outputStream, cancellationToken);
                return new ParseResult((uint)width, (uint)height, (uint)frameCount, "image/webp");
            }
            else
            {
                if (outputStream is not null) await image.SaveAsGifAsync(outputStream, cancellationToken);
                return new ParseResult((uint)width, (uint)height, (uint)frameCount, "image/gif");
            }
        }
        catch (Exception)
        {
        }

        return new ErrorDetails(StatusCodes.Status400BadRequest, "Invalid or Unsupported image", "We were unable to parse the iamge, please check its filetype and integrity");
    }
}
