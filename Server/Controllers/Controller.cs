[ApiController]
[Route("[controller]")]
public class FileController : ControllerBase
{
    private readonly string Target;

    public FileController()
    {
        Target = "C:\\Users\\matej\\Desktop\\Zastita informacija\\Projekat\\CryptoFileApp\\Target";
        if (!Directory.Exists(Target))
        {
            Directory.CreateDirectory(Target);
        }
    }

    [HttpPost("upload")]
    public async Task<IActionResult> UploadFile([FromForm] IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded");

        var filePath = Path.Combine(Target, file.FileName);
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        return Ok(new { message = "Fajl je sačuvan" });
    }

}
