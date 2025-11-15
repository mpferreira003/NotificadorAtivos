using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

public class ResponseResult
{
    public List<RequestJson> results { get; set; } = new();
    public string? requestedAt { get; set; }
    public string? took { get; set; }
}

public class RequestJson
{
    public string? currency { get; set; }
    public long? marketCap { get; set; }
    public string? shortName { get; set; }
    public string? longName { get; set; }
    public double? regularMarketPrice { get; set; }
    public double? regularMarketDayHigh { get; set; }
    public double? regularMarketDayLow { get; set; }
    public string? symbol { get; set; }
}

public class Provider
{
    private static HttpClient client = new HttpClient();
    
    public async Task<double?> GetPrice(string ativo){
        string url = $"https://brapi.dev/api/quote/{ativo}";
        try
        {
            string json = await client.GetStringAsync(url);
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var data = JsonSerializer.Deserialize<ResponseResult>(json, options);
            if (data.results != null && data.results.Count > 0){
                return data.results[0].regularMarketPrice;
            }
            return null;
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Erro HTTP: {ex.Message}");
            return null;
        }
    }
}
