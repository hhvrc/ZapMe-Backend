using static ZapMe.Services.Interfaces.IImageManager;
using static ZapMe.Utils.ImageUtils;

namespace ZapMe.Mappers;

public static class ImageParseErrorMapper
{
    public static ImageUploadError MapToUploadError(ImageParseError uploadError)
    {
        return uploadError switch
        {
            ImageParseError.ImageDimensionsInvalid => ImageUploadError.ImageDimensionsInvalid,
            ImageParseError.ImageDataInvalid => ImageUploadError.ImageDataInvalid,
            ImageParseError.ImageFormatUnsupported => ImageUploadError.ImageFormatUnsupported,
            _ => throw new ArgumentOutOfRangeException(nameof(uploadError), uploadError, null)
        };
    }
}
