using System.Net.Http.Headers;

namespace ApiGateway;

public static class RequestForwardExtensions
{
    public static async Task ForwardRequest(HttpContext context, IHttpClientFactory httpClientFactory,
        string serviceName)
    {
        var client = CreateRequestMessageToService(context, httpClientFactory, serviceName, out var requestMessage);

        SetContentTypeFromEnteredRequest(context, requestMessage);

        CopyHeadersFromEnteredRequest(context, requestMessage);

        await TrySendRequestAndCopyResponseToCurrentContext(context, client, requestMessage);
    }

    private static async Task TrySendRequestAndCopyResponseToCurrentContext(HttpContext context, HttpClient client,
        HttpRequestMessage requestMessage)
    {
        try
        {
            var response = await client.SendAsync(requestMessage);

            // Проверяем, не начата ли отправка ответа
            await SetDataFromResponseToCurrentHttpContext(context, response);
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

    private static async Task SetDataFromResponseToCurrentHttpContext(HttpContext context, HttpResponseMessage response)
    {
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

    private static HttpClient CreateRequestMessageToService(HttpContext context, IHttpClientFactory httpClientFactory,
        string serviceName, out HttpRequestMessage requestMessage)
    {
        var client = httpClientFactory.CreateClient(serviceName);

        var requestPath = context.Request.Path.ToString().Replace($"/{serviceName.ToLower()}", "");
        var requestUri = $"{requestPath}{context.Request.QueryString}";

        requestMessage = new HttpRequestMessage
        {
            Method = new HttpMethod(context.Request.Method),
            RequestUri = new Uri(client.BaseAddress, requestUri),
            Content = new StreamContent(context.Request.Body)
        };
        return client;
    }

    private static void CopyHeadersFromEnteredRequest(HttpContext context, HttpRequestMessage requestMessage)
    {
        // Копируем заголовки из входящего запроса
        foreach (var header in context.Request.Headers)
        {
            if (!requestMessage.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray()))
            {
                requestMessage.Content.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray());
            }
        }
    }

    private static void SetContentTypeFromEnteredRequest(HttpContext context, HttpRequestMessage requestMessage)
    {
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
    }
}