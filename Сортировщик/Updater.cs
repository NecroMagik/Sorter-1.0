using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Windows.Forms;
using Сортировщик;

public class Updater
{
    private string manifestUrll;

    private string ManifestUrl()
    {
        if (radioButton1.Checked)
        {
            manifestUrll = "https://github.com/NecroMagik/Sorter-1.0/raw/refs/heads/Release/releases/update_manifest.json";

        }
        else if (radioButton2.Checked)
        {
            manifestUrll = "https://github.com/NecroMagik/Sorter-1.0/raw/refs/heads/Release/releases/Alpha_Manifest.json";

        }
        else if (radioButton3.Checked)
        {
            manifestUrll = "https://github.com/NecroMagik/Sorter-1.0/raw/refs/heads/Release/releases/Rebuild_Manifest.json";
        }
        return null;
    }


    private readonly string changelogUrl = "https://raw.githubusercontent.com/NecroMagik/Sorter-1.0/refs/heads/Release/releases/ChangeLog.txt";
    private ProgressBar progressBar;
    private RadioButton radioButton1;
    private RadioButton radioButton2;
    private RadioButton radioButton3;

    public Updater(ProgressBar progressBar, RadioButton radioButton1, RadioButton radioButton2, RadioButton radioButton3)
    {
        this.progressBar = progressBar;
        this.radioButton1 = radioButton1;
        this.radioButton2 = radioButton2;
        this.radioButton3 = radioButton3;
    }

    // Загрузка манифеста
    public async Task<string> DownloadManifestAsync(string tempFolder)
    {
        ManifestUrl();
        using (HttpClient client = new HttpClient())
        {
            string manifestData = await client.GetStringAsync(manifestUrll);
            string manifestPath = Path.Combine(tempFolder, "update_manifest.json");
            File.WriteAllText(manifestPath, manifestData);
            return manifestPath;
        }
    }

    // Чтение манифеста
    public UpdateManifest ReadManifest(string manifestPath)
    {
        string manifestData = File.ReadAllText(manifestPath);
        return JsonConvert.DeserializeObject<UpdateManifest>(manifestData);
    }

    // Метод для получения списка изменений
    public async Task<string> GetChangelogAsync()
    {
        try
        {
            using (HttpClient client = new HttpClient())
            {
                string changelog = await client.GetStringAsync(changelogUrl);
                return changelog.Replace("\n", Environment.NewLine); // Приводим переносы строк к корректному формату
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при загрузке changelog: {ex.Message}");
            return null; // В случае ошибки возвращаем null
        }
    }

    // Загрузка и установка обновления
    public async Task DownloadAndInstallUpdateAsync()
    {
        string logDirectory = @"C:\Users\NecroMagik\ZeN\UpdateLog";
        string logFilePath = Path.Combine(logDirectory, $"{DateTime.Now:yyyy-MM-dd}.log");

        // Создаем директорию для логов если не существует
        Directory.CreateDirectory(logDirectory);

        using (StreamWriter logWriter = new StreamWriter(logFilePath, true))
        using (HttpClient client = new HttpClient())
        {
            await LogMessageAsync(logWriter, "INFO", "Начало процесса обновления");

            try
            {
                string tempFolder = Path.Combine(Path.GetTempPath(), "Sorter");

                await LogMessageAsync(logWriter, "MANIFEST", "Загрузка манифеста обновления");
                string manifestPath = await DownloadManifestAsync(tempFolder);
                UpdateManifest manifest = ReadManifest(manifestPath);
                await LogMessageAsync(logWriter, "MANIFEST", $"Манифест загружен. Файлов для обновления: {manifest.Files.Count}");

                int progressStep = 100 / manifest.Files.Count;
                progressBar.Value = 0;

                foreach (var file in manifest.Files)
                {
                    await LogMessageAsync(logWriter, "DOWNLOAD", $"Начало загрузки файла: {file.Name}");

                    string filePath = Path.Combine(tempFolder, file.Name);
                    Directory.CreateDirectory(Path.GetDirectoryName(filePath));

                    using (var response = await client.GetAsync(file.Url))
                    {
                        if (!response.IsSuccessStatusCode)
                        {
                            string errorMsg = $"Ошибка загрузки файла {file.Name}. HTTP статус: {response.StatusCode}";
                            await LogMessageAsync(logWriter, "DOWNLOAD_ERROR", errorMsg);
                            throw new Exception($"Ошибка загрузки файла. Подробности в логе: {logFilePath}");
                        }

                        byte[] fileData = await response.Content.ReadAsByteArrayAsync();
                        await LogMessageAsync(logWriter, "DOWNLOAD", $"Файл {file.Name} загружен. Размер: {fileData.Length} байт");

                        await LogMessageAsync(logWriter, "CHECKSUM", $"Проверка контрольной суммы файла: {file.Name}");
                        var checksumResult = VerifyChecksum(fileData, file.Checksum);
                        if (!checksumResult.IsValid)
                        {
                            string checksumError = $"Контрольная сумма файла {file.Name} не совпадает!\n" +
                                                  $"Ожидалось: {file.Checksum}\n" +
                                                  $"Получено:  {checksumResult.ActualChecksum}";
                            await LogMessageAsync(logWriter, "CHECKSUM_ERROR", checksumError);
                            throw new Exception($"Ошибка проверки целостности файла. Проверьте лог: {logFilePath}");
                        }
                        await LogMessageAsync(logWriter, "CHECKSUM", $"Контрольная сумма файла {file.Name} проверена успешно");

                        File.WriteAllBytes(filePath, fileData);
                        await LogMessageAsync(logWriter, "DOWNLOAD", $"Файл {file.Name} сохранен по пути: {filePath}");
                    }

                    progressBar.Value += progressStep;
                    await LogMessageAsync(logWriter, "PROGRESS", $"Прогресс обновления: {progressBar.Value}%");
                }

                await LogMessageAsync(logWriter, "INSTALLER", "Поиск файла установщика");
                string setupFilePath = Path.Combine(tempFolder, "Setup.exe");
                if (!File.Exists(setupFilePath))
                {
                    string setupError = "Файл Setup.exe не найден!";
                    await LogMessageAsync(logWriter, "INSTALLER_ERROR", setupError);
                    throw new Exception($"Файл установщика не найден. Проверьте лог: {logFilePath}");
                }

                await LogMessageAsync(logWriter, "INSTALLER", $"Запуск установщика: {setupFilePath}");
                LaunchInstallerAndExit(setupFilePath);

                await LogMessageAsync(logWriter, "INFO", "Процесс обновления завершен успешно");
            }
            catch (HttpRequestException ex)
            {
                await LogMessageAsync(logWriter, "NETWORK_ERROR", $"Сетевая ошибка: {ex.Message}");
                throw new Exception($"Ошибка сети при обновлении. Проверьте лог: {logFilePath}");
            }
            catch (IOException ex)
            {
                await LogMessageAsync(logWriter, "IO_ERROR", $"Ошибка ввода-вывода: {ex.Message}");
                throw new Exception($"Ошибка файловой системы. Проверьте лог: {logFilePath}");
            }
            catch (Exception ex)
            {
                // Если это уже наше исключение с ссылкой на лог, просто пробрасываем дальше
                if (ex.Message.Contains("лог"))
                    throw;

                await LogMessageAsync(logWriter, "GENERAL_ERROR", $"Общая ошибка: {ex.Message}");
                throw new Exception($"Произошла ошибка при обновлении. Проверьте лог: {logFilePath}");
            }
        }
    }


    // Вспомогательный метод для логирования
    private async Task LogMessageAsync(StreamWriter writer, string category, string message)
    {
        string logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{category}] {message}";
        await writer.WriteLineAsync(logEntry);
        await writer.FlushAsync(); // Обеспечиваем немедленную запись в файл
    }

    // Класс для результата проверки контрольной суммы
    private class ChecksumResult
    {
        public bool IsValid { get; set; }
        public string ActualChecksum { get; set; }
    }

    // Метод для проверки контрольной суммы с возвратом результата
    private ChecksumResult VerifyChecksum(byte[] data, string expectedChecksum)
    {
        using (var sha256 = System.Security.Cryptography.SHA256.Create())
        {
            byte[] hash = sha256.ComputeHash(data);
            string actualChecksum = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();

            return new ChecksumResult
            {
                IsValid = actualChecksum.Equals(expectedChecksum, StringComparison.OrdinalIgnoreCase),
                ActualChecksum = actualChecksum
            };
        }
    }


    private void LaunchInstallerAndExit(string setupFilePath)
    {
        Process.Start(setupFilePath);
        Form1 sorter = new Form1();

        // Удаление ClickOnce-приложения
        try
        {
            string appDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Apps", "2.0");
            if (Directory.Exists(appDataPath))
            {
                sorter.Visible = false;
                Directory.Delete(appDataPath, true);
            }
        }
        catch
        {

        }

        Environment.Exit(0);
    }

    public class UpdateManifest
    {
        public string Version { get; set; }
        public List<UpdateFile> Files { get; set; }
    }

    public class UpdateFile
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public string Checksum { get; set; }
    }
}
