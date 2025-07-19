// TransferFileUI/DataAccess/IData.cs
using Refit;
using TransferFileUI.Controllers;
using TransferFileUI.DataAccess.Models;
using TransferFileUI.Models;
public interface IData
{
    [Multipart]
    [Post("/api/Service/upload")]
    Task<UploadResponse> UploadFile([AliasAs("file")] StreamPart file);

    [Post("/api/Service/checkbox")]
    Task<string> SetCheckbox([Body] AlgorithmRequest model);
}


// a simple DTO for the upload response
namespace TransferFileUI.DataAccess.Models
{
    public class UploadResponse
    {
        public string Message { get; set; }
    }
}
