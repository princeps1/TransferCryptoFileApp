public class FSWService
{
    private readonly string _targetDirectory;
    private readonly string _outputDirectory;
    private readonly ILogger<FSWService> _logger;
    private FileSystemWatcher? _watcher;

    private static AlgorithmType algorithmType;

    private readonly IFactory _factory;
    private readonly IWebHostEnvironment _env;

    public FSWService(IWebHostEnvironment env, ILogger<FSWService> logger, IFactory factory)
    {
        _env = env;
        _logger = logger;
        _factory = factory;

        string rootPath = _env.ContentRootPath;
        string parentPath = Directory.GetParent(rootPath)!.FullName;
        _targetDirectory = Path.Combine(parentPath, "Target");
        _outputDirectory = Path.Combine(parentPath, "X"); 
    }



    public void StartWatching()
    {
        if (!Directory.Exists(_targetDirectory))
            throw new DirectoryNotFoundException($"Target directory '{_targetDirectory}' does not exist.");

        if (!Directory.Exists(_outputDirectory))
            Directory.CreateDirectory(_outputDirectory);

        _watcher = new FileSystemWatcher(_targetDirectory)
        {
            NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite,
            Filter = "*.*" // Monitoring all file types
        };

        _watcher.Created += OnFileCreated;
        _watcher.EnableRaisingEvents = true;

        Console.WriteLine($"Started watching directory: {_targetDirectory}");
    }

    private async void OnFileCreated(object sender, FileSystemEventArgs e)
    {
        await Task.Run(async () =>
        {
            try
            {
                Console.WriteLine($"New file detected: {e.Name}");

                var service = _factory.GetService(algorithmType);


                //Čekaj dok fajl ne bude spreman
                int retries = 5;
                while (retries > 0 && !IsFileReady(e.FullPath))
                {
                    await Task.Delay(1000); // Sačekaj 1 sekundu
                    retries--;
                }

                if (!IsFileReady(e.FullPath))
                {
                    _logger.LogError("File is still in use after retries: {FileName}", e.Name);
                    return;
                }

                //////////// ****KODIRANJE PRAVO*****
                byte[] fileContentInBytes = await File.ReadAllBytesAsync(e.FullPath);
                byte[] encodedContent = service.Encrypt(fileContentInBytes);
                string extension = Path.GetExtension(e.FullPath);
                string name = Path.GetFileNameWithoutExtension(e.FullPath);
                string FileName = string.Concat(name, "-",algorithmType, extension);
                string outputFilePath = Path.Combine(_outputDirectory, FileName);
                await File.WriteAllBytesAsync(outputFilePath, encodedContent);

                //////////// ****DEKODIRANJE PRAVO - Railfence cipher*****
                //byte[] decodedContentInBytes = service.Decrypt(encodedContent);
                //string decodedFile = Convert.ToBase64String(decodedContentInBytes);
                //string outputDecodedFilePath = Path.Combine(_outputDirectory, e.Name);
                //await File.WriteAllBytesAsync(outputDecodedFilePath, decodedContentInBytes);


                // Brisanje fajla
                DeleteFileWithRetry(e.FullPath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing file {FileName}", e.Name);
            }
        });
    }

    public bool SetAlgorithmType(string data)
    {
        if (Enum.TryParse<AlgorithmType>(data, out var parsed))
        {
            algorithmType = parsed;
            Console.WriteLine($"Algorithm selected: {algorithmType}");
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool IsFileReady(string filePath)
    {
        try
        {
            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.None))
            {
                return true;
            }
        }
        catch (IOException)
        {
            return false;
        }
    }

    private void DeleteFileWithRetry(string filePath)
    {
        int retryCount = 3;
        while (retryCount > 0)
        {
            try
            {
                File.Delete(filePath);
                return;
            }
            catch (IOException)
            {
                retryCount--;
                Thread.Sleep(1000);
            }
        }
    }

    public void StopWatching()
    {
        if (_watcher != null)
        {
            _watcher.EnableRaisingEvents = false;
            _watcher.Dispose();
            Console.WriteLine("Stopped watching directory.");
        }
    }
}