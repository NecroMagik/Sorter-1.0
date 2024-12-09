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
    private readonly string manifestUrl = "https://github.com/NecroMagik/Sorter-1.0/raw/refs/heads/Release/releases/update_manifest.json";
    private readonly string changelogUrl = "https://raw.githubusercontent.com/NecroMagik/Sorter-1.0/refs/heads/Release/releases/ChangeLog.txt";

    // Загрузка манифеста
    public async Task<string> DownloadManifestAsync(string tempFolder)
    {
        using (HttpClient client = new HttpClient())
        {
            string manifestData = await client.GetStringAsync(manifestUrl);
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
    public async Task DownloadAndInstallUpdateAsync(ProgressBar progressBar)
    {
        using (HttpClient client = new HttpClient())
        {
            string tempFolder = Path.Combine(Path.GetTempPath(), "Sorter");
            string manifestPath = await DownloadManifestAsync(tempFolder);
            UpdateManifest manifest = ReadManifest(manifestPath);

            int progressStep = 100 / manifest.Files.Count;
            progressBar.Value = 0;

            foreach (var file in manifest.Files)
            {
                string filePath = Path.Combine(tempFolder, file.Name);
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));

                using (var response = await client.GetAsync(file.Url))
                {
                    response.EnsureSuccessStatusCode();
                    byte[] fileData = await response.Content.ReadAsByteArrayAsync();

                    if (!VerifyChecksum(fileData, file.Checksum))
                        throw new Exception($"Контрольная сумма файла {file.Name} не совпадает!");

                    File.WriteAllBytes(filePath, fileData);
                }

                progressBar.Value += progressStep;
            }

            string setupFilePath = Path.Combine(tempFolder, "Setup.exe");
            if (!File.Exists(setupFilePath))
                throw new FileNotFoundException("Файл Setup.exe не найден!");

            LaunchInstallerAndExit(setupFilePath);
        }
    }

    private void LaunchInstallerAndExit(string setupFilePath)
    {
        Process.Start(setupFilePath);

        // Удаление ClickOnce-приложения
        try
        {
            string appDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Apps", "2.0");
            if (Directory.Exists(appDataPath))
            {
                Directory.Delete(appDataPath, true);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка при удалении старой версии: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        Sorter_Main form1 = new Sorter_Main();
        form1.Visible = false;
        Environment.Exit(0);
    }

    private bool VerifyChecksum(byte[] fileData, string expectedChecksum)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] hash = sha256.ComputeHash(fileData);
            string computedChecksum = BitConverter.ToString(hash).Replace("-", "").ToLower();
            return computedChecksum == expectedChecksum;
        }
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
