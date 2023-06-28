namespace ZapMe.Enums.Errors;

public enum ImageUploadError
{
    PayloadSizeInvalid,
    PayloadSizeTooLarge,
    PayloadChecksumMismatch,
    ImageDimensionsInvalid,
    ImageDataInvalid,
    ImageFormatUnsupported,
    ImageDimensionsTooLarge
}