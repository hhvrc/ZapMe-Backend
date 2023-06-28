namespace ZapMe.DTOs;

public readonly record struct ImageMetaData(uint Width, uint Height, uint FrameCount, string MimeType);