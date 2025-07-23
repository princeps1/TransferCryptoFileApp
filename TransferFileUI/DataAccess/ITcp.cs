using Microsoft.AspNetCore.Mvc;
using Refit;
using TransferFileUI.Models;

namespace TransferFileUI.DataAccess;

public interface ITcp
{

    [Post("/api/Tcp/become-server")]
    Task<string> BecomeServer([AliasAs("port")] int port);


    [Post("/api/Tcp/stop-server")]
    Task<string> StopServer();


    [Multipart]
    [Post("/api/Tcp/send-file")]
    Task<string> SendFile([AliasAs("host")] string host,[AliasAs("port")] int port,[AliasAs("file")] StreamPart file);
}
