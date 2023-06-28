using ZapMe.Enums.Errors;

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
