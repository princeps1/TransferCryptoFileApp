using Microsoft.AspNetCore.Mvc;
using Refit;
using TransferFileUI.DataAccess;
using TransferFileUI.Models;


namespace TransferFileUI.Controllers;

[ApiController]
[Route("Tcp")]
public class TcpController : ControllerBase
{
    private readonly ITcp _dataService;

    public TcpController(ITcp dataService)
    {
        _dataService = dataService;
    }

    [HttpPost("SendFile")]
    public async Task<IActionResult> SendFile(
        [FromForm] string host,
        [FromForm] int port,
        [FromForm] IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded");

        try
        {
            await using var ms = new MemoryStream();
            await file.CopyToAsync(ms);
            ms.Position = 0;

            var streamPart = new StreamPart(ms, file.FileName, file.ContentType);
            var response = await _dataService.SendFile(host, port, streamPart);

            return Ok(new { message = response });
        }
        catch (Exception ex)
        {
            
            return StatusCode(500, new { error = ex.ToString() });
        }
    }


    [HttpPost("BecomeServer")]
    public async Task<IActionResult> BecomeServer([FromForm] int port)
    {
        var msg = await _dataService.BecomeServer(port);
        return Ok(new { message = msg });
    }

    [HttpPost("StopServer")]
    public async Task<IActionResult> StopServer()
    {
        var msg = await _dataService.StopServer();
        return Ok(new { message = msg });
    }
}




