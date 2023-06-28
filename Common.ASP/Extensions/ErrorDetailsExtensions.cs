using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using ZapMe.DTOs;
using static System.Net.Mime.MediaTypeNames;

namespace ZapMe;

public static class ErrorDetailsExtensions
{
    public static ObjectResult ToActionResult(this ErrorDetails errorDetails) => new ObjectResult(errorDetails) { StatusCode = errorDetails.HttpStatusCode, ContentTypes = { Application.Json } };

    public static Task Write(this ErrorDetails errorDetails, HttpResponse response, JsonSerializerOptions? options = null)
    {
        response.StatusCode = errorDetails.HttpStatusCode;
        response.ContentType = Application.Json;
        return response.WriteAsJsonAsync(errorDetails, options);
    }
}
