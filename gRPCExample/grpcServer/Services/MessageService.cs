using Grpc.Core;
using grpcServer;

namespace grpcServer.Services;

public class MessageService : Message.MessageBase
{
    private readonly ILogger<GreeterService> _logger;
    public MessageService(ILogger<GreeterService> logger)
    {
        _logger = logger;
    }

    public override async Task SendMessage(MessageRequest request, IServerStreamWriter<MessageResponse> responseStream, ServerCallContext context)
    {
        _logger.LogWarning("mesaj istegi server'a geldi");

        Console.WriteLine($"Message : {request.Message} | Name : {request.Name}");

        for(int i = 0; i<10; i++)
        {
            await responseStream.WriteAsync(new MessageResponse{Message = $"{i} no'lu mesaj iletildi"});
            await Task.Delay(200);
        }
        
    }
}