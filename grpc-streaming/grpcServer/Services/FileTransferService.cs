using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

namespace grpcFileTransferServer.Services
{
    public class FileTransferService : FileService.FileServiceBase
    {
        private readonly ILogger<FileTransferService> _logger;
        public FileTransferService(ILogger<FileTransferService> logger)
        {
            _logger = logger;
        }

        public override Task<Empty> FileUpload(IAsyncStreamReader<BytesContent> requestStream, ServerCallContext context)
        {
            return base.FileUpload(requestStream, context);
        }
    }
}