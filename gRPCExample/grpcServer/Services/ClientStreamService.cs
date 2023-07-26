using Grpc.Core;
using grpcServer;
using grpcServerTest;

namespace grpcServer.Services;

public class ClientStreamService: ClientStream.ClientStreamBase
{
    private readonly ILogger<GreeterService> _logger;
    public ClientStreamService(ILogger<GreeterService> logger)
    {
        _logger = logger;
    }

    public override async Task<grpcServerTest.MessageResponse> SendMessage(IAsyncStreamReader<grpcServerTest.MessageRequest> requestStream, ServerCallContext context)
    {
        while(await requestStream.MoveNext(context.CancellationToken))
        {
            _logger.LogWarning("request alindi");

            Console.WriteLine($"Message : {requestStream.Current.Message} , Name : {requestStream.Current.Name}");
        }
        

        return new grpcServerTest.MessageResponse
        {
            Message = "Response mesajidir"
        };
    }
}