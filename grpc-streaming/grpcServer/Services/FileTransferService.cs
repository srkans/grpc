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
                    if(count == 0)
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
                _logger.LogError($"Hata olustu : {e.Message}");
            }

            await fileStream.DisposeAsync();

            fileStream.Close();

            return new Empty();
        }

        public override async Task FileDownload(FileInfo request, IServerStreamWriter<BytesContent> responseStream, ServerCallContext context)
        {
            
        }
    }
}