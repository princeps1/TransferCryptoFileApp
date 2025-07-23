using TransferFileUI.Models;

[ApiController]
[Route("api/[controller]")]
public class FswController : ControllerBase
{
    private readonly string _target;
    private readonly FSWService _watcherService;
    private readonly IWebHostEnvironment _env;
    public FswController(FSWService watcherService,IWebHostEnvironment env)
    {
        _watcherService = watcherService;
        _env = env;

        string rootPath = _env.ContentRootPath;
        string parentPath = Directory.GetParent(rootPath)!.FullName;
        _target= Path.Combine(parentPath, "Target");

        if (!Directory.Exists(_target))
            Directory.CreateDirectory(_target);
    }

    [HttpPost("checkbox")]
    public IActionResult SetCheckbox([FromBody] AlgorithmRequest model)
    {
        if (_watcherService.SetAlgorithmType(model.AlgorithmType))
            return Ok($"Promenjen checkbox na {model.AlgorithmType}");
        else
            return BadRequest("Greska!");
    }


    [HttpPost("upload")]
    public async Task<IActionResult> UploadFile([FromForm] IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded");

        var filePath = Path.Combine(_target, file.FileName);
        await using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);

        return Ok(new { message = "Fajl je sačuvan" });
    }
}
