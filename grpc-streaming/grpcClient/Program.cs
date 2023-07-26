using Google.Protobuf;
using Grpc.Net.Client;
using grpcFileTransferClient;

var channel = GrpcChannel.ForAddress("https://localhost:7269");

var client = new FileService.FileServiceClient(channel);

string file = @"C:\Users\serka\OneDrive\Masaüstü\Architecting-Modern-Web-Applications-with-ASP.NET-Core-and-Azure.pdf";

using FileStream fileStream= new FileStream(file, FileMode.Open);

var content = new BytesContent
{
    FileSize = fileStream.Length,
    ReadedByte = 0,
    Info = new grpcFileTransferClient.FileInfo{FileName = Path.GetFileNameWithoutExtension(fileStream.Name), FileExtension = Path.GetExtension(fileStream.Name)}
};

var upload = client.FileUpload();

byte[] buffer = new byte[2048];

while((content.ReadedByte = await fileStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
{
    content.Buffer = ByteString.CopyFrom(buffer);

    await upload.RequestStream.WriteAsync(content);
}

await upload.RequestStream.CompleteAsync();

fileStream.Close();
