using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

namespace grpcFileTransferServer.Services
{
    public class FileTransferService : FileService.FileServiceBase
    {
        private readonly ILogger<FileTransferService> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public FileTransferService(ILogger<FileTransferService> logger, IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
        }

        public override async Task<Empty> FileUpload(IAsyncStreamReader<BytesContent> requestStream, ServerCallContext context)
        {
            //stream'in yapilacagi dizini belirliyoruz.
            string path = Path.Combine(_webHostEnvironment.WebRootPath,"files");
            
            //yoksa dizini olustur
            if(!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            FileStream fileStream = null;

            try
            {
                int count = 0;
                decimal chunkSize = 0;

                while(await requestStream.MoveNext())
                {
                    if(count++ == 0) 
                    {
                        fileStream = new FileStream($"{path}/{requestStream.Current.Info.FileName}{requestStream.Current.Info.FileExtension}", FileMode.CreateNew);

                        fileStream.SetLength(requestStream.Current.FileSize);
                    }

                    var buffer = requestStream.Current.Buffer.ToByteArray(); //data byte string olarak geliyor.

                    await fileStream.WriteAsync(buffer, 0, buffer.Length);

                    Console.WriteLine($"{Math.Round(((chunkSize += requestStream.Current.ReadedByte) *100 )/requestStream.Current.FileSize)}%");
                }
            }
            catch(Exception e)
            {
                _logger.LogError($"Error message : {e.Message}");
            }

            await fileStream.DisposeAsync();

            fileStream.Close();

            return new Empty();
        }

        public override async Task FileDownload(FileInfo request, IServerStreamWriter<BytesContent> responseStream, ServerCallContext context)
        {
            string path = Path.Combine(_webHostEnvironment.WebRootPath,"files");

            using FileStream fileStream = new FileStream($"{path}/{request.FileName}{request.FileExtension}",FileMode.Open, FileAccess.Read);

            byte[] buffer = new byte[2048]; //max 2048 tanımlanabiliyor her bir buffer

            BytesContent content = new BytesContent
            {
                FileSize = fileStream.Length,
                Info = new grpcFileTransferServer.FileInfo{FileName = Path.GetFileNameWithoutExtension(fileStream.Name), FileExtension = Path.GetExtension(fileStream.Name)},
                ReadedByte = 0
            };

            while((content.ReadedByte = await fileStream.ReadAsync(buffer, 0, buffer.Length))>0)
            {
                content.Buffer = ByteString.CopyFrom(buffer);

                await responseStream.WriteAsync(content);
            }

            fileStream.Close();//dispose surecinde close ediliyor aslinda fazladan yazdik
        }
    }
}