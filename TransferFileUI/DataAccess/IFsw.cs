using Refit;
using TransferFileUI.Models;

namespace TransferFileUI.DataAccess;

public interface IFsw
{
    [Multipart]
    [Post("/api/Fsw/upload")]
    Task<UploadResponse> UploadFile([AliasAs("file")] StreamPart file);

    [Post("/api/Fsw/checkbox")]
    Task<string> SetCheckbox([Body] AlgorithmRequest model);

    [Multipart]
    [Post("/api/Fsw/uploadDecrypt")]
    Task<UploadResponse> UploadFileDecrypt([AliasAs("file")] StreamPart file);
}
