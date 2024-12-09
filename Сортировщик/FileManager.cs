using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using System;
using System.Linq;
using Сортировщик;

public class FileManager
{
    private readonly Dictionary<string, string[]> fileCategories = new Dictionary<string, string[]>
    {
        { "Фото", new[] { ".jpg", ".jpeg", ".png", ".bmp", ".gif", ".tiff", ".webp" } },
        { "Видео", new[] { ".mp4", ".avi", ".mkv", ".mov", ".wmv", ".flv" } },
        { "Музыка", new[] { ".mp3", ".wav", ".flac", ".aac", ".ogg", ".m4a" } },
        { "Документы", new[] { ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".txt", ".ppt", ".pptx" } }
    };

    public string SelectedFolder { get; private set; }
    public bool SearchSubfolders { get; set; }
    public Dictionary<string, List<string>> FoundFiles { get; private set; }
    public Dictionary<string, string> DefaultPaths { get; private set; } // Пути по умолчанию

    public FileManager()
    {
        FoundFiles = new Dictionary<string, List<string>>();
        InitializeDefaultPaths();
    }

    public void InitializeDefaultPaths()
    {
        string userPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        DefaultPaths = new Dictionary<string, string>
        {
            { "Фото", Path.Combine(userPath, "Pictures") },
            { "Видео", Path.Combine(userPath, "Videos") },
            { "Музыка", Path.Combine(userPath, "Music") },
            { "Документы", Path.Combine(userPath, "Documents") }
        };
    }

    public void SelectFolder(Label label)
    {
        using (var folderDialog = new FolderBrowserDialog())
        {
            if (folderDialog.ShowDialog() == DialogResult.OK)
            {
                SelectedFolder = folderDialog.SelectedPath;
                label.Text = $"Выбрана папка: {SelectedFolder}";
            }
            else
            {
                label.Text = "Папка не выбрана.";
            }
        }
    }

    public void SearchFiles(bool searchSubfolders, List<string> activeCategories, ProgressBar progressBar, Label label)
    {
        if (string.IsNullOrEmpty(SelectedFolder))
        {
            MessageBox.Show("Сначала выберите папку для поиска.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        FoundFiles.Clear();
        SearchSubfolders = searchSubfolders;

        try
        {
            progressBar.Value = 0;
            progressBar.Maximum = activeCategories.Count;

            foreach (var category in activeCategories)
            {
                if (fileCategories.ContainsKey(category))
                {
                    string[] extensions = fileCategories[category];
                    var files = Directory.EnumerateFiles(
                        SelectedFolder,
                        "*.*",
                        SearchSubfolders ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly
                    )
                    .Where(file => extensions.Contains(Path.GetExtension(file).ToLower()))
                    .ToList();
                    FoundFiles[category] = files;

                    progressBar.Value += 1;
                }
            }

            if (FoundFiles.Values.All(list => list.Count == 0))
            {
                DialogResult result = MessageBox.Show(
                    "Файлы не найдены. Выполнить поиск в подпапках?",
                    "Файлы не найдены",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    SearchFiles(true, activeCategories, progressBar, label);
                }
                else
                {
                    label.Text = "Файлы не найдены.";
                }


}
            else
            {
                label.Text = $"Найдено файлов: {FoundFiles.Values.Sum(list => list.Count)}";
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка при поиске файлов: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    public void ResetDefaultPaths()
    {
        InitializeDefaultPaths();
    }

    public async Task MoveFiles(Dictionary<string, string> categoryPaths, ProgressBar progressBar, Label label)
    {
        if (FoundFiles.Count == 0 || FoundFiles.Values.All(list => list.Count == 0))
        {
            MessageBox.Show("Сначала выполните поиск файлов.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        progressBar.Value = 0;
        progressBar.Maximum = FoundFiles.Values.Sum(files => files.Count);

        try
        {
            foreach (var category in FoundFiles)
            {
                if (!categoryPaths.TryGetValue(category.Key, out string targetFolder))
                {
                    targetFolder = DefaultPaths[category.Key];
                }

                Directory.CreateDirectory(targetFolder);

                foreach (var file in category.Value)
                {
                    string destinationPath = Path.Combine(targetFolder, Path.GetFileName(file));
                    if (File.Exists(destinationPath))
                    {
                        destinationPath = Path.Combine(
                            targetFolder,
                            $"{Path.GetFileNameWithoutExtension(file)}_копия{Path.GetExtension(file)}"
                        );
                    }

                    File.Move(file, destinationPath);
                    progressBar.Value += 1;
                }
            }

            label.Text = "Файлы успешно перемещены.";
            MessageBox.Show("Перемещение файлов завершено.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка при перемещении файлов: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}