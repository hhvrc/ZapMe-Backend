using ZapMe.DTOs;
using static ZapMe.Utils.ImageUtils;

namespace ZapMe.Mappers;

public static class ImageParseErrorMapper
{
    public static ErrorDetails MapToErrorDetails(ImageParseError uploadError)
    {
        return uploadError switch
        {
            ImageParseError.ImageDimensionsInvalid => throw new NotImplementedException(),
            ImageParseError.ImageDataInvalid => throw new NotImplementedException(),
            ImageParseError.ImageFormatUnsupported => throw new NotImplementedException(),
            _ => throw new ArgumentOutOfRangeException(nameof(uploadError), uploadError, null)
        };
    }
}
