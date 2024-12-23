using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public class FileSystemWatcherService
{
    private readonly string _targetDirectory;
    private readonly string _outputDirectory;
    private FileSystemWatcher _watcher;
    private readonly ILogger<FileSystemWatcherService> _logger;

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
                byte[] encodedContent = EncodeFile(fileContentInBytes);
                string outputFilePath = Path.Combine(_outputDirectory, e.Name);
                await File.WriteAllBytesAsync(outputFilePath, encodedContent);

                //////////// ****DEKODIRANJE PRAVO*****
                //byte[] decodedContentInBytes = DecodeFile(encodedContent);
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


    private List<int> MakeKey(int Depth)
    {
        List<int> Key = new List<int>();
        int m = 0;
        int factor = Depth * 2 - 2;

        // Create the key
        try
        {
            for (int i = 0; i < Depth; i++)
            {
                Key.Add(factor - m);
                m += 2;
            }
            if (Key.Count == 0)
                throw new ArgumentException("Depth must be greater than 0.");
            Key[Key.Count - 1] = Key[0];
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating key.");
            throw;
        }
        return Key;
    }

    private byte[] DecodeFile(byte[] encryptedContent)
    {
        List<byte> DecodedContent = new List<byte>(new byte[encryptedContent.Length]);
        int Depth = 2;
        List<int> Key = MakeKey(Depth); // Create key

        int[] rowLengths = new int[Depth];
        int left = 0, right = Depth - 1;

        // Calculate row lengths
        for (int i = 0; i < Depth; i++)
        {
            int currentIndex = i;
            int j = 0;
            do
            {
                rowLengths[i]++;
                currentIndex += (j % 2 == 0) ? Key[left] : Key[right];
                j++;
            } while (currentIndex < encryptedContent.Length);

            left++;
            right--;
        }

        // Set row start indices
        int[] rowStartIndices = new int[Depth]; rowStartIndices[0] = 0;
        for (int i = 1; i < Depth; i++)
        {
            rowStartIndices[i] = rowStartIndices[i - 1] + rowLengths[i - 1];
        }

        // Decode content
        left = 0;
        right = Depth - 1;
        for (int i = 0; i < Depth; i++)
        {
            int rowIndex = rowStartIndices[i];
            int j = 0;
            int currentIndex = i;

            do
            {
                DecodedContent[currentIndex] = encryptedContent[rowIndex++];
                currentIndex += (j % 2 == 0) ? Key[left] : Key[right];
                j++;
            } while (currentIndex < encryptedContent.Length);

            left++;
            right--;
        }

        return DecodedContent.ToArray();
    }

    private byte[] EncodeFile(byte[] content)
    {
        List<byte> EncryptedContent = new List<byte>();

        int Depth = 2;
        List<int> Key = MakeKey(Depth); // Create key

        int left = 0, right = Depth - 1;
        // Encode content
        for (int i = 0; i < Depth; i++) // Number of rows
        {
            int j = 0;
            int index = i;
            do
            {
                if ((j % 2) == 0)
                {
                    EncryptedContent.Add(content[index]);
                    index += Key[left];
                }
                else
                {
                    EncryptedContent.Add(content[index]);
                    index += Key[right];
                }
                j++;
            } while (index < content.Count());
            left++;
            right--;
        }
        return EncryptedContent.ToArray();
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