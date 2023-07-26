
using System.Net;
using Grpc.Net.Client;
using grpcBidirectionalClient;

var channel = GrpcChannel.ForAddress("https://localhost:7027");

var streamClient = new BidirectionalStream.BidirectionalStreamClient(channel);

var request = streamClient.SendMessage();

//stream'i bitirdikten sonra response aliyoruz

await Task.Run(async () => 
{
    for (int i = 1; i <= 10; i++)
    {
        await Task.Delay(500);

        await request.RequestStream.WriteAsync(new grpcBidirectionalClient.MessageRequest{Name = "Serkan", Message = "Mesaj" + i});
    }
});


CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

while(await request.ResponseStream.MoveNext(cancellationTokenSource.Token))
{
    Console.WriteLine(request.ResponseStream.Current.Message);
}


await request.RequestStream.CompleteAsync();