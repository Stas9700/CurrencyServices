using CurrencyService.Grpc;
using Google.Protobuf;
using Grpc.Core;

namespace ApiGateway;

public static class GrpcExtensions
{
    public static async Task GrpsRequestHandler(string? method, HttpContext context, UserCurrenciesService.UserCurrenciesServiceClient client)
    {
        switch (method)
        {
            case "getusercurrencies":
                await HandleJsonGrpcRequest<GetUserCurrenciesRequest, GetUserCurrenciesResponse>(
                        context, client, req => client.GetUserCurrenciesAsync(req));
                break;

            case "getcurrencies":
                await HandleJsonGrpcRequest<GetCurrenciesRequest, GetCurrenciesResponse>(
                    context, client, req => client.GetCurrenciesAsync(req));
                break;

            case "setusercurrency":
                await HandleJsonGrpcRequest<SetUserCurrencyRequest, SetUserCurrencyResponse>(
                        context, client, req => client.SetUserCurrencyAsync(req));
                break;

            default:
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                await context.Response.WriteAsync($"gRPC method {method} not found");
                break;
        }
    }

// Метод для обработки JSON gRPC запросов
    public static async Task HandleJsonGrpcRequest<TRequest, TResponse>(
        HttpContext context,
        UserCurrenciesService.UserCurrenciesServiceClient client,
        Func<TRequest, AsyncUnaryCall<TResponse>> grpcMethod)
        where TRequest : IMessage<TRequest>, new()
        where TResponse : IMessage<TResponse>
    {
        using var reader = new StreamReader(context.Request.Body);
        var json = await reader.ReadToEndAsync();
        var request = JsonParser.Default.Parse<TRequest>(json);
    
        var response = await grpcMethod(request);
    
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(JsonFormatter.Default.Format(response));
    }
    
    public static int MapGrpcStatusCodeToHttp(this StatusCode grpcCode) => grpcCode switch
    {
        StatusCode.OK => StatusCodes.Status200OK,
        StatusCode.InvalidArgument => StatusCodes.Status400BadRequest,
        StatusCode.NotFound => StatusCodes.Status404NotFound,
        StatusCode.AlreadyExists => StatusCodes.Status409Conflict,
        StatusCode.PermissionDenied => StatusCodes.Status403Forbidden,
        StatusCode.Unauthenticated => StatusCodes.Status401Unauthorized,
        StatusCode.Unimplemented => StatusCodes.Status501NotImplemented,
        StatusCode.Unavailable => StatusCodes.Status503ServiceUnavailable,
        _ => StatusCodes.Status500InternalServerError
    };
}