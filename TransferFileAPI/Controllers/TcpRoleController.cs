[ApiController]
[Route("api/[controller]")]
public class TcpRoleController : ControllerBase
{
    private readonly TcpSocketService _tcpService;

    public TcpRoleController(TcpSocketService tcpService)
    {
        _tcpService = tcpService;
    }

    [HttpPost("become-server")]
    public IActionResult BecomeServer([FromQuery] int port)
    {
        _tcpService.StartServer(port);
        return Ok("Now acting as server.");
    }

    [HttpPost("stop-server")]
    public IActionResult StopServer()
    {
        _tcpService.StopServer();
        return Ok("Server stopped.");
    }

    // --- SEND FILE ENDPOINT ---
    [HttpPost("send-file")]
    public async Task<IActionResult> SendFile([FromQuery] string host, [FromQuery] int port, [FromForm] IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded");

        await _tcpService.SendFileAsync(host, port, file);
        return Ok("File sent.");
    }
}
