using ZapMe.Constants;
using ZapMe.DTOs;
using ZapMe.Enums.Errors;
using ZapMe.Helpers;

namespace ZapMe.Mappers;

public static class ImageUploadErrorMapper
{
    public static ErrorDetails PayloadSizeInvalid => HttpErrors.Generic(StatusCodes.Status400BadRequest, "Invalid image", "Image size is invalid");
    public static ErrorDetails PayloadSizeTooLarge => HttpErrors.Generic(StatusCodes.Status413PayloadTooLarge, "Payload too large", "Image too large, max " + ImageConstants.MaxImageSizeString);
    public static ErrorDetails PayloadChecksumMismatch => HttpErrors.Generic(StatusCodes.Status400BadRequest, "Checksum mismatch", "The provided checksum does not match the image data");
    public static ErrorDetails ImageDimensionsInvalid => throw new NotImplementedException();
    public static ErrorDetails ImageDataInvalid => throw new NotImplementedException();
    public static ErrorDetails ImageFormatUnsupported => throw new NotImplementedException();
    public static ErrorDetails ImageDimensionsTooLarge => HttpErrors.Generic(StatusCodes.Status413PayloadTooLarge, "Payload too large", "Image too large, max 1024x1024");
    public static ErrorDetails InternalError => throw new NotImplementedException();

    public static ErrorDetails MapToErrorDetails(ImageUploadError imageUploadError)
    {
        return imageUploadError switch
        {
            ImageUploadError.PayloadSizeInvalid => PayloadSizeInvalid,
            ImageUploadError.PayloadSizeTooLarge => PayloadSizeTooLarge,
            ImageUploadError.PayloadChecksumMismatch => PayloadChecksumMismatch,
            ImageUploadError.ImageDimensionsInvalid => ImageDimensionsInvalid,
            ImageUploadError.ImageDataInvalid => ImageDataInvalid,
            ImageUploadError.ImageFormatUnsupported => ImageFormatUnsupported,
            ImageUploadError.ImageDimensionsTooLarge => ImageDimensionsTooLarge,
            _ => throw new ArgumentOutOfRangeException(nameof(imageUploadError), imageUploadError, null)
        };
    }
}
