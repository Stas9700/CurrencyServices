using System.Net.Http.Headers;

internal class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

// Регистрация HttpClient для каждого микросервиса
        builder.Services.AddHttpClient("UserAuthentication",
            client => { client.BaseAddress = new Uri("http://localhost:5058"); });

        builder.Services.AddHttpClient("CurrencyService",
            client => { client.BaseAddress = new Uri("http://localhost:5000"); });


        var app = builder.Build();

        app.UseRouting();

// Маршрутизация запросов
        app.Map("api/UserAuthentication/{**path}",
            async (HttpContext context, IHttpClientFactory httpClientFactory) =>
            {
                await ForwardRequest(context, httpClientFactory, "UserAuthentication");
            });

        app.Map("api/currency/{**path}",
            async (HttpContext context, IHttpClientFactory httpClientFactory) =>
            {
                await ForwardRequest(context, httpClientFactory, "CurrencyService");
            });

        app.Run();
    }

    private static async Task ForwardRequest(HttpContext context, IHttpClientFactory httpClientFactory,
        string serviceName)
    {
        var client = httpClientFactory.CreateClient(serviceName);

        var requestPath = context.Request.Path.ToString().Replace($"/{serviceName.ToLower()}", "");
        var requestUri = $"{requestPath}{context.Request.QueryString}";

        var requestMessage = new HttpRequestMessage
        {
            Method = new HttpMethod(context.Request.Method),
            RequestUri = new Uri(client.BaseAddress, requestUri),
            Content = new StreamContent(context.Request.Body)
        };

        // Устанавливаем Content-Type из входящего запроса
        if (!string.IsNullOrEmpty(context.Request.ContentType))
        {
            var contentType = context.Request.ContentType.Split(';').First().Trim();
            requestMessage.Content.Headers.ContentType = new MediaTypeHeaderValue(contentType);
        }
        else
        {
            requestMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        }

        // Копируем заголовки из входящего запроса
        foreach (var header in context.Request.Headers)
        {
            if (!requestMessage.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray()))
            {
                requestMessage.Content.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray());
            }
        }

        try
        {
            var response = await client.SendAsync(requestMessage);

            // Проверяем, не начата ли отправка ответа
            if (!context.Response.HasStarted)
            {
                // Устанавливаем статус код
                context.Response.StatusCode = (int)response.StatusCode;

                // Устанавливаем Content-Type
                context.Response.Headers.ContentType = "application/json; charset=utf-8";

                // Буферизируем тело ответа
                var responseBody = await response.Content.ReadAsByteArrayAsync();

                // Устанавливаем Content-Length
                context.Response.Headers.ContentLength = responseBody.Length;

                // Копируем тело ответа
                await context.Response.Body.WriteAsync(responseBody, 0, responseBody.Length);
            }
            else
            {
                // Логируем ошибку или выполняем другие действия
                Console.WriteLine("Cannot modify headers, response has already started.");
            }
        }
        catch (HttpRequestException ex)
        {
            // Если микросервис недоступен, возвращаем 503 Service Unavailable
            context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
            await context.Response.WriteAsync($"Service is unavailable. {ex.Message}");
        }
        catch (HttpIOException ex)
        {
            // Если ответ обрывается, возвращаем 502 Bad Gateway
            context.Response.StatusCode = StatusCodes.Status502BadGateway;
            await context.Response.WriteAsync($"Response ended prematurely. {ex.Message}");
        }
    }
}