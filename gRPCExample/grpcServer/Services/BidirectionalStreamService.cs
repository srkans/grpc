using Grpc.Core;

namespace grpcServer.Services;

public class BidirectionalStreamService: grpcBidirectional.BidirectionalStream.BidirectionalStreamBase
{
    private readonly ILogger<GreeterService> _logger;
    public BidirectionalStreamService(ILogger<GreeterService> logger)
    {
        _logger = logger;
    }

    public override async Task SendMessage(IAsyncStreamReader<grpcBidirectional.MessageRequest> requestStream, IServerStreamWriter<grpcBidirectional.MessageResponse> responseStream, ServerCallContext context)
    {
        var Task1 = Task.Run(async () =>
        {
            while(await requestStream.MoveNext(context.CancellationToken))
            {
                _logger.LogWarning("request alindi");
                Console.WriteLine($"Message : {requestStream.Current.Message} | Name : {requestStream.Current.Name}");
            }
        });

        for(int i = 1; i <= 10; i++)
        {
            await Task.Delay(500);
            await responseStream.WriteAsync(new grpcBidirectional.MessageResponse{Message = "mesaj " + i});
        }       

        await Task1;
    }
}