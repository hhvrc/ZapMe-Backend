using OneOf;

namespace ZapMe.Utils;

public static class ImageUtils
{
    public readonly record struct ImageParseResult(uint Width, uint Height, uint FrameCount, string MimeType);
    public enum ImageParseError
    {
        ImageDimensionsInvalid,
        ImageDataInvalid,
        ImageFormatUnsupported,
    }

    /// <summary>
    /// Parses and rewrites an image from a stream
    /// </summary>
    /// <param name="inputStream"></param>
    /// <param name="outputStream"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Parse result or ErrorDetails(400)</returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="NotSupportedException"></exception>
    public static async Task<OneOf<ImageParseResult, ImageParseError>> ParseAndRewriteFromStreamAsync(Stream inputStream, Stream? outputStream, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(inputStream);
        ArgumentNullException.ThrowIfNull(outputStream);

        try
        {
            using Image image = await Image.LoadAsync(inputStream, cancellationToken);

            int width = image.Width;
            int height = image.Height;
            int frameCount = image.Frames.Count;

            if (height < 0 || width < 0 || frameCount <= 0)
            {
                return ImageParseError.ImageDimensionsInvalid;
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
                return new ImageParseResult((uint)width, (uint)height, (uint)frameCount, "image/webp");
            }
            else
            {
                if (outputStream is not null) await image.SaveAsGifAsync(outputStream, cancellationToken);
                return new ImageParseResult((uint)width, (uint)height, (uint)frameCount, "image/gif");
            }
        }
        catch (InvalidImageContentException)
        {
            return ImageParseError.ImageDataInvalid;
        }
        catch (UnknownImageFormatException)
        {
            return ImageParseError.ImageFormatUnsupported;
        }
    }
}
