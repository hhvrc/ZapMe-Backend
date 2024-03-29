﻿using System.Security.Cryptography;
using System.Text;
using ZapMe.Constants;

namespace ZapMe.Utils;

public static class HashingUtils
{
    /// <summary>
    /// Hashes the given stream and outputs the digest hash as 64 characters of uppercase hex
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static string Sha256_Hex(Stream data)
    {
        data.Position = 0;
        Span<byte> digest = stackalloc byte[HashConstants.Sha256LengthBin];
        SHA256.HashData(data, digest);
        return Convert.ToHexString(digest);
    }

    /// <summary>
    /// Hashes the given stream and outputs the digest hash as 64 characters of uppercase hex
    /// </summary>
    /// <param name="data"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task<string> Sha256_HexAsync(Stream data, CancellationToken cancellationToken = default)
    {
        data.Position = 0;
        byte[] digest = new byte[HashConstants.Sha256LengthBin];
        await SHA256.HashDataAsync(data, digest, cancellationToken);
        return Convert.ToHexString(digest);
    }

    /// <summary>
    /// Hashes the given bytes and outputs the digest hash as 64 characters of uppercase hex
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static string Sha256_Hex(Span<byte> data)
    {
        Span<byte> digest = stackalloc byte[HashConstants.Sha256LengthBin];
        SHA256.HashData(data, digest);
        return Convert.ToHexString(digest);
    }

    /// <summary>
    /// Hashes the given string and outputs the digest hash as 64 characters of uppercase hex
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static string Sha256_Hex(string data)
    {
        return Sha256_Hex(Encoding.UTF8.GetBytes(data));
    }

    /// <summary>
    /// Hashes the given stream and outputs the digest hash as 32 bytes
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static byte[] Sha256_Bytes(Stream data)
    {
        data.Position = 0;
        byte[] digest = new byte[HashConstants.Sha256LengthBin];
        SHA256.HashData(data, digest);
        return digest;
    }

    /// <summary>
    /// Hashes the given stream and outputs the digest hash as 32 bytes
    /// </summary>
    /// <param name="data"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task<byte[]> Sha256_BytesAsync(Stream data, CancellationToken cancellationToken = default)
    {
        data.Position = 0;
        byte[] digest = new byte[HashConstants.Sha256LengthBin];
        await SHA256.HashDataAsync(data, digest, cancellationToken);
        return digest;
    }

    /// <summary>
    /// Hashes the given bytes and outputs the digest hash as 32 bytes
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static byte[] Sha256_Bytes(Span<byte> data)
    {
        byte[] digest = new byte[HashConstants.Sha256LengthBin];
        SHA256.HashData(data, digest);
        return digest;
    }

    /// <summary>
    /// Hashes the given string and outputs the digest hash as 32 bytes
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static byte[] Sha256_Bytes(string data)
    {
        return Sha256_Bytes(Encoding.UTF8.GetBytes(data));
    }

    /// <summary>
    /// Hashes the given stream and outputs the digest hash as 64 characters of uppercase hex
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static string Sha256_Base64(Stream data)
    {
        return Convert.ToBase64String(Sha256_Bytes(data));
    }

    /// <summary>
    /// Hashes the given stream and outputs the digest hash as 64 characters of uppercase hex
    /// </summary>
    /// <param name="data"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task<string> Sha256_Base64Async(Stream data, CancellationToken cancellationToken = default)
    {
        return Convert.ToBase64String(await Sha256_BytesAsync(data, cancellationToken));
    }

    /// <summary>
    /// Hashes the given bytes and outputs the digest hash as 64 characters of uppercase hex
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static string Sha256_Base64(Span<byte> data)
    {
        return Convert.ToBase64String(Sha256_Bytes(data));
    }

    /// <summary>
    /// Hashes the given string and outputs the digest hash as 64 characters of uppercase hex
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static string Sha256_Base64(string data)
    {
        return Convert.ToBase64String(Sha256_Bytes(data));
    }

}
