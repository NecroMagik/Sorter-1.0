using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

public class FileManager
{
    private readonly Dictionary<string, string[]> fileCategories = new Dictionary<string, string[]>
    {
        { "Фото", new[] { ".jpg", ".jpeg", ".png", ".bmp", ".gif", ".tiff", ".webp" } },
        { "Видео", new[] { ".mp4", ".avi", ".mkv", ".mov", ".wmv", ".flv" } },
        { "Музыка", new[] { ".mp3", ".wav", ".flac", ".aac", ".ogg", ".m4a" } },
        { "Документы", new[] { ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".txt", ".ppt", ".pptx" } }
    };

    public string SelectedFolder { get; private set; } // Выбранная папка
    public bool SearchSubfolders { get; private set; } // Флаг для поиска в подпапках
    public Dictionary<string, List<string>> FoundFiles { get; private set; } // Найденные файлы по категориям

    public FileManager()
    {
        FoundFiles = new Dictionary<string, List<string>>();
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

    public void SearchFiles(bool searchSubfolders, List<string> activeCategories, Label label)
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
            foreach (var category in activeCategories)
            {
                if (fileCategories.ContainsKey(category))
                {
                    string[] extensions = fileCategories[category];
                    var files = Directory.EnumerateFiles(
                            SelectedFolder,
                            "*.*",
                            SearchSubfolders ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly)
                        .Where(file => extensions.Contains(Path.GetExtension(file).ToLower()))
                        .ToList();

                    FoundFiles[category] = files;
                }
            }

            // Проверка, есть ли найденные файлы
            if (FoundFiles.Values.All(list => list.Count == 0))
            {
                // Если поиск не дал результатов
                if (!SearchSubfolders)
                {
                    // Предложить выполнить поиск в подпапках
                    DialogResult result = MessageBox.Show(
                        "Файлы не найдены. Выполнить поиск в подпапках?",
                        "Файлы не найдены",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        // Активировать поиск в подпапках
                        SearchSubfolders = true;

                        // Найти чекбокс динамически
                        var form = Application.OpenForms.Cast<Form>().FirstOrDefault();
                        var checkBox = form?.Controls.OfType<CheckBox>().FirstOrDefault(cb => cb.Name == "checkBox1");
                        if (checkBox != null)
                        {
                            checkBox.Checked = true; // Активировать чекбокс
                        }

                        // Запустить поиск снова
                        SearchFiles(true, activeCategories, label);
                        return;
                    }
                    else
                    {
                        label.Text = "Файлы не найдены.";
                        return;
                    }
                }
                else
                {
                    // Если поиск в подпапках уже активен и файлы не найдены
                    MessageBox.Show("Файлы не найдены даже в подпапках. Попробуйте выбрать другую папку.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    label.Text = "Файлы не найдены.";
                    return;
                }
            }

            // Если файлы найдены
            label.Text = "Файлы успешно найдены.";
        }
        catch (UnauthorizedAccessException ex)
        {
            MessageBox.Show($"Ошибка доступа к файлу/папке: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка при поиске файлов: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }


    public void MoveFiles(string targetFolder, List<string> activeCategories, Label label)
    {
        if (FoundFiles.Count == 0 || FoundFiles.Values.All(list => list.Count == 0))
        {
            MessageBox.Show("Сначала выполните поиск файлов.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        try
        {
            foreach (var category in activeCategories)
            {
                if (FoundFiles.ContainsKey(category))
                {
                    string categoryFolder = Path.Combine(targetFolder, category);
                    Directory.CreateDirectory(categoryFolder);

                    foreach (var file in FoundFiles[category])
                    {
                        string destinationPath = Path.Combine(categoryFolder, Path.GetFileName(file));

                        if (File.Exists(destinationPath))
                        {
                            destinationPath = Path.Combine(
                                categoryFolder,
                                $"{Path.GetFileNameWithoutExtension(file)}_копия{Path.GetExtension(file)}");
                        }

                        File.Move(file, destinationPath);
                    }
                }
            }

            label.Text = "Файлы успешно перемещены.";
            MessageBox.Show("Перемещение файлов завершено.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка при переносе файлов: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
