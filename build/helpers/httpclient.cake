using System.Net.Http;
using System.Text.Json;

public static async Task<T> GetAsync<T>(this HttpClient client, string uri)
{
    await using var stream = await client.GetStreamAsync(uri);
    return await JsonSerializer.DeserializeAsync<T>(
        stream,
        new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );
}