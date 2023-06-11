using System.Net.Http.Headers;
using System.Text;

namespace System.Net.Http;

public static class HttpClientExtensions
{
    private static void AddHeaders(StringBuilder sb, HttpHeaders headers)
    {
        foreach ((string key, IEnumerable<string> value) in headers)
        {
            foreach (string val in value)
            {
                sb.AppendLine($"{key}: {val}");
            }
        }
    }

    public static async Task<string> ToRawString(this HttpRequestMessage request)
    {
        StringBuilder sb = new StringBuilder();

        sb.AppendLine($"{request.Method} {request.RequestUri} HTTP/{request.Version}");

        AddHeaders(sb, request.Headers);

        if (request.Content?.Headers != null)
        {
            AddHeaders(sb, request.Content.Headers);
        }

        sb.AppendLine();

        string body = await (request.Content?.ReadAsStringAsync() ?? Task.FromResult(String.Empty));
        if (!String.IsNullOrWhiteSpace(body))
        {
            sb.AppendLine(body);
        }

        return sb.ToString();
    }

    public static async Task<string> ToRawString(this HttpResponseMessage response)
    {
        StringBuilder sb = new StringBuilder();

        sb.AppendLine($"HTTP/{response.Version} {(int)response.StatusCode} {response.ReasonPhrase}");

        AddHeaders(sb, response.Headers);
        AddHeaders(sb, response.Content.Headers);

        sb.AppendLine();

        string body = await response.Content.ReadAsStringAsync();
        if (!String.IsNullOrWhiteSpace(body))
        {
            sb.AppendLine(body);
        }

        return sb.ToString();
    }
}
