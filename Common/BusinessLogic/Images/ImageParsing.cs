using OneOf;
using ZapMe.DTOs;
using ZapMe.Enums.Errors;

namespace ZapMe.Utils;

public static class ImageParsing
{
    /// <summary>
    /// Parses and rewrites an image from a stream
    /// </summary>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="NotSupportedException"></exception>
    public static async Task<OneOf<ImageMetaData, ImageParseError>> ParseAndRewriteFromStreamAsync(Stream inputStream, Stream? outputStream, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(inputStream);
        ArgumentNullException.ThrowIfNull(outputStream);

        try
        {
            using Image image = await Image.LoadAsync(inputStream, cancellationToken);

            int width = image.Width;
            int height = image.Height;
            int frameCount = image.Frames.Count;

            if (width < 0 || height < 0 || frameCount <= 0)
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
                return new ImageMetaData((uint)width, (uint)height, (uint)frameCount, "image/webp");
            }
            else
            {
                if (outputStream is not null) await image.SaveAsGifAsync(outputStream, cancellationToken);
                return new ImageMetaData((uint)width, (uint)height, (uint)frameCount, "image/gif");
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
