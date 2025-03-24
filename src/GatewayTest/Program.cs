
using System.Net.Http.Json;

class Program
{
    static async Task Main(string[] args)
    {
        var httpClient = new HttpClient();
        var requestData = new { UserName = "First", Password = "psw" }; // Замените на реальные данные

        try
        {
            httpClient.Timeout = TimeSpan.FromSeconds(150);
            //var response = await httpClient.GetAsync("http://localhost:5143/api/currency/getCurrencies");
            var response = await httpClient.PostAsJsonAsync("http://localhost:5143/api/UserAuthentication/login", requestData);
            Console.WriteLine("wait end");
            response.EnsureSuccessStatusCode();

            var tokenResponse = await response.Content.ReadAsStringAsync();
            Console.WriteLine("JWT Token: " + tokenResponse);
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine("Request error: " + e);
        }
    }
}