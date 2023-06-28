using ZapMe.DTOs;
using ZapMe.Enums.Errors;

namespace ZapMe.Mappers;

public static class ImageParseErrorMapper
{
    public static ErrorDetails MapToErrorDetails(ImageParseError imageParseError)
    {
        return imageParseError switch
        {
            ImageParseError.ImageDimensionsInvalid => throw new NotImplementedException(),
            ImageParseError.ImageDataInvalid => throw new NotImplementedException(),
            ImageParseError.ImageFormatUnsupported => throw new NotImplementedException(),
            _ => throw new ArgumentOutOfRangeException(nameof(imageParseError), imageParseError, null)
        };
    }
}
