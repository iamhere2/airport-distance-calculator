using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace AirportDistanceCalculator.CommonUtils.Http
{
    /// <summary>Provides extension methods for <see cref="HttpResponseMessage"/></summary>
    public static class HttpResponseMessageExtensions
    {
        /// <summary>Returns JSON document from successfull <see cref="HttpResponseMessage"/></summary>
        public static async Task<JsonDocument> GetJson(this HttpResponseMessage response)
        {
            if (response is null)
            {
                throw new ArgumentNullException(nameof(response));
            }

            if (!response.IsSuccessStatusCode)
            {
                throw new ArgumentOutOfRangeException(nameof(response),
                    $"Response was not successfull. Status code: {response.StatusCode}");
            }

            var mediaType = response.Content.Headers.ContentType.MediaType;
            if (!mediaType.Contains("application/json", StringComparison.InvariantCulture))
            {
                throw new ArgumentOutOfRangeException(nameof(response),
                    $"Response media type is not JSON: \"{mediaType}\"");
            }

            var stream = await response.Content.ReadAsStreamAsync();
            return await JsonDocument.ParseAsync(stream);
        }
    }
}
