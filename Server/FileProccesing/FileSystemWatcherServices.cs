using WebTemplate.Algorithms;

public class FileSystemWatcherService
{
    private readonly string _targetDirectory;
    private readonly string _outputDirectory;
    private readonly ILogger<FileSystemWatcherService> _logger;
    private FileSystemWatcher _watcher;

    private static string algorithmType;

    public FileSystemWatcherService(string targetDirectory, string outputDirectory, ILogger<FileSystemWatcherService> logger)
    {
        _targetDirectory = targetDirectory;
        _outputDirectory = outputDirectory;
        _logger = logger;
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



               if(algorithmType == "Railfence cipher")
               {
                    // Čekaj dok fajl ne bude spreman
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

                    // Procesiranje fajla
                    byte[] fileContentInBytes = await File.ReadAllBytesAsync(e.FullPath);
                    byte[] encodedContent = Railfence_cipher.EncodeFile(fileContentInBytes);
                    string outputFilePath = Path.Combine(_outputDirectory, e.Name);
                    await File.WriteAllBytesAsync(outputFilePath, encodedContent);

                    //////////// ****DEKODIRANJE PRAVO*****
                    //byte[] decodedContentInBytes = Railfence_cipher.DecodeFile(encodedContent);
                    //string decodedFile = Convert.ToBase64String(decodedContentInBytes);
                    //string outputDecodedFilePath = Path.Combine(_outputDirectory, e.Name);
                    //await File.WriteAllBytesAsync(outputDecodedFilePath, decodedContentInBytes);

                    // Brisanje fajla
                    DeleteFileWithRetry(e.FullPath);
                }
                else
                {
                    throw new InvalidOperationException($"Unsupported algorithm: {algorithmType}");
                }
                

                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing file {FileName}", e.Name);
            }
        });
    }

    public static void SetAlgorithmType(string data)
    {
        algorithmType = data;
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


    // Retry logic for file deletion
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
                Thread.Sleep(1000); // Retry after 1 second
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