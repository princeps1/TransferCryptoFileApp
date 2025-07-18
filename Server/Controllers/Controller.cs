[ApiController]
[Route("[controller]")]
public class FileController : ControllerBase
{
    private readonly string _target;
    private readonly FileSystemWatcherService _watcherService;

    public FileController(FileSystemWatcherService watcherService)
    {
        _watcherService = watcherService;
        _target = Path.Combine(
                              "C:", "Users", "matej", "Desktop",
                              "Zastita informacija", "Projekat",
                              "TransferCryptoFileApp", "Target"
                           );

        if (!Directory.Exists(_target))
            Directory.CreateDirectory(_target);
    }

    [HttpPost("checkbox")]
    public IActionResult SetCheckbox([FromBody] string algorithmType)
    {
        if (_watcherService.SetAlgorithmType(algorithmType))
            return Ok($"Promenjen checkbox na {algorithmType}");
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
