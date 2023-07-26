using Grpc.Net.Client;
using grpcFileTransferClient;

var channel = GrpcChannel.ForAddress("https://localhost:7269");

var client = new FileService.FileServiceClient(channel);

string downloadPath = @"F:\grpc\grpc-streaming\grpcDownloadClient\downloads";

var fileInfo = new grpcFileTransferClient.FileInfo
{
    FileExtension = ".pdf",
    FileName = "Architecting-Modern-Web-Applications-with-ASP.NET-Core-and-Azure",
};

FileStream fileStream = null;

var request = client.FileDownload(fileInfo);

CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

int count = 0;
decimal chunkSize = 0;

while(await request.ResponseStream.MoveNext(cancellationTokenSource.Token))
{
    if(count++ == 0)
    {
        fileStream = new FileStream(@$"{downloadPath}\{request.ResponseStream.Current.Info.FileName}{request.ResponseStream.Current.Info.FileExtension}", FileMode.CreateNew);

        fileStream.SetLength(request.ResponseStream.Current.FileSize);
    }

    var buffer = request.ResponseStream.Current.Buffer.ToByteArray();

    await fileStream.WriteAsync(buffer, 0, request.ResponseStream.Current.ReadedByte);

    Console.WriteLine($"{Math.Round(((chunkSize += request.ResponseStream.Current.ReadedByte) *100 )/request.ResponseStream.Current.FileSize)}%");
}

Console.WriteLine("Yuklendi...");

await fileStream.DisposeAsync();
fileStream.Close();
