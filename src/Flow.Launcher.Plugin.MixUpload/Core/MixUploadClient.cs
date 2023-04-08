using System.Text.Json;
using Flow.Launcher.Plugin.MixUpload.Core.Responses;

namespace Flow.Launcher.Plugin.MixUpload.Core;

public class MixUploadClient : IDisposable
{
    const string Address = "https://mixupload.com/search/maintracks_json";
    readonly HttpClient _client;

    public MixUploadClient()
    {
        _client = new HttpClient();
    }

    public async Task<List<MixUpladResponse>> SearchAsync(string keyword)
    {
        var content = new MultipartFormDataContent();
        content.Add(new StringContent(keyword), "search");

        var request = new HttpRequestMessage(HttpMethod.Post, Address)
        {
            Content = content
        };
        var response = await _client.SendAsync(request).ConfigureAwait(false);


        //if (!response.IsSuccessStatusCode)
        //    return new BitlyGeneralErrorResponse(response.StatusCode.ToString());
        var stringResponse = await response.Content.ReadAsStringAsync();

        //if (stringResponse.Contains("errors"))
        //{
        //    var jsonErrorResponse = JsonSerializer.Deserialize<BitlyErrorResponse>(stringResponse);
        //    return jsonErrorResponse;
        //}
        var jsonResponse = JsonSerializer.Deserialize<List<MixUpladResponse>>(stringResponse);
        return jsonResponse;
        
    }


    public void Dispose()
    {
        _client?.Dispose();
    }
}