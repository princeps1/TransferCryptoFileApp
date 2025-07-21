using System.Net.Sockets;
using System.Net;
using System.Text;

public class TcpSocketService : BackgroundService
{
    private TcpListener _listener;
    private bool _isServer = false;
    private int _port = 9000;

    public void StartServer(int port)
    {
        _port = port;
        _isServer = true;
        _listener = new TcpListener(IPAddress.Any, _port);
        _listener.Start();
    }

    public void StopServer()
    {
        _isServer = false;
        _listener?.Stop();
    }

    //Poziva se automatski kad se app startuje
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if (_isServer && _listener != null)
            {
                if (_listener.Pending())
                {
                    var client = await _listener.AcceptTcpClientAsync();
                    _ = HandleClientAsync(client);
                }
            }
            await Task.Delay(100, stoppingToken);
        }
    }

    // --- FILE SENDING LOGIC ---
    public async Task SendFileAsync(string host, int port, IFormFile file)
    {
        using var client = new TcpClient();
        await client.ConnectAsync(host, port);
        using var stream = client.GetStream();

        // Send file name length and file name
        var fileName = Path.GetFileName(file.FileName);
        var fileNameBytes = Encoding.UTF8.GetBytes(fileName);
        await stream.WriteAsync(BitConverter.GetBytes(fileNameBytes.Length));
        await stream.WriteAsync(fileNameBytes);

        // Send file length and file bytes
        await stream.WriteAsync(BitConverter.GetBytes((int)file.Length));
        await file.CopyToAsync(stream);
    }

    // --- FILE RECEIVING LOGIC ---
    private async Task HandleClientAsync(TcpClient client)
    {
        using var stream = client.GetStream();
        var buffer = new byte[4];

        // Read file name length
        await stream.ReadAsync(buffer, 0, 4);
        int fileNameLen = BitConverter.ToInt32(buffer, 0);

        // Read file name
        var fileNameBytes = new byte[fileNameLen];
        await stream.ReadAsync(fileNameBytes, 0, fileNameLen);
        var fileName = Encoding.UTF8.GetString(fileNameBytes);

        // Read file length
        await stream.ReadAsync(buffer, 0, 4);
        int fileLen = BitConverter.ToInt32(buffer, 0);

        // Read file bytes and save
        var saveDir = Path.Combine(Directory.GetCurrentDirectory(), "ReceivedFiles");
        Directory.CreateDirectory(saveDir);
        var savePath = Path.Combine(saveDir, fileName);

        using var fileStream = new FileStream(savePath, FileMode.Create, FileAccess.Write);
        int totalRead = 0;
        var fileBuffer = new byte[4096];
        while (totalRead < fileLen)
        {
            int toRead = Math.Min(fileBuffer.Length, fileLen - totalRead);
            int read = await stream.ReadAsync(fileBuffer, 0, toRead);
            if (read == 0) break;
            await fileStream.WriteAsync(fileBuffer, 0, read);
            totalRead += read;
        }
    }
}
