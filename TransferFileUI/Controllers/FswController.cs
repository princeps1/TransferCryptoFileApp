using Microsoft.AspNetCore.Mvc;
using Refit;
using TransferFileUI.DataAccess;
using TransferFileUI.Models;

namespace TransferFileUI.Controllers;

[ApiController]
[Route("Fsw")]
public class FswController : Controller
{
    private readonly IFsw _dataService;

    public FswController(IFsw dataService)
    {
        _dataService = dataService;
    }

    [HttpPost("Checkbox")]
    public async Task<IActionResult> SetCheckbox([FromBody] AlgorithmRequest model)
    {
        var message = await _dataService.SetCheckbox(model);
        return Ok(message);
    }

    [HttpPost("Upload")]
    public async Task<IActionResult> UploadFile([FromForm] IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded");

        await using var ms = new MemoryStream();
        await file.CopyToAsync(ms);
        ms.Position = 0;

        var streamPart = new StreamPart(ms, file.FileName, file.ContentType);

        var response = await _dataService.UploadFile(streamPart);

        return Ok(new { message = response.Message });
    }

    [HttpPost("uploadDecrypt")]
    public async Task<IActionResult> uploadDecrypt([FromForm] IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded");

        await using var ms = new MemoryStream();
        await file.CopyToAsync(ms);
        ms.Position = 0;

        var streamPart = new StreamPart(ms, file.FileName, file.ContentType);

        var response = await _dataService.UploadFileDecrypt(streamPart);

        return Ok(new { message = response.Message });
    }
}

