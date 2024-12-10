using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using System;
using System.Linq;
using System.Drawing;
using Сортировщик;
using Guna.UI2.WinForms;

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

    private Button button1;
    private Label label2;
    private Label label3;
    private Label label4;
    private Label label5;
    private CheckBox checkBox1;

    public FileManager(Button button1, Label label2, Label label3, Label label4, Label label5, CheckBox checkBox1, Guna.UI2.WinForms.Guna2ProgressBar guna2ProgressBar1)
    {
        if(guna2ProgressBar1 == null)
        {
            throw new ArgumentNullException(nameof(guna2ProgressBar1));
        }
        FoundFiles = new Dictionary<string, List<string>>();
        this.button1 = button1;
        this.label2 = label2;
        this.label3 = label3;
        this.label4 = label4;
        this.label5 = label5;
        this.checkBox1 = checkBox1;
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
        label2.Text = $"Def: {DefaultPaths["Фото"]}";
        label3.Text = $"Def: {DefaultPaths["Видео"]}";
        label4.Text = $"Def: {DefaultPaths["Музыка"]}";
        label5.Text = $"Def: {DefaultPaths["Документы"]}";
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

    public async void SearchFiles(bool searchSubfolders, List<string> activeCategories, Guna.UI2.WinForms.Guna2ProgressBar guna2ProgressBar1, Label label)
    {
        if (string.IsNullOrEmpty(SelectedFolder))
        {
            MessageBox.Show("Сначала выберите папку для поиска.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            BlinkButton(button1);
            return;
        }

        FoundFiles.Clear();
        SearchSubfolders = searchSubfolders;

        try
        {
            guna2ProgressBar1.Value = 0;
            guna2ProgressBar1.Maximum = activeCategories.Count;

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

                    guna2ProgressBar1.Value += 1;
                }
            }

            if (FoundFiles.Values.All(list => list.Count == 0))
            {
                if (!searchSubfolders)
                {
                    // Первый запуск: предложить поиск в подпапках
                    DialogResult result = MessageBox.Show(
                        "Файлы не найдены. Выполнить поиск в подпапках?",
                        "Файлы не найдены",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        checkBox1.Checked = true; // Активируем чекбокс
                        SearchFiles(true, activeCategories, guna2ProgressBar1, label);
                    }
                    else
                    {
                        label.Text = "Файлы не найдены.";
                    }
                }
                else
                {
                    MessageBox.Show("К сожалению, файлы отсутствуют в данной папке. Попробуйте выбрать другую.","Ошибка",MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    BlinkButton(button1);
                    label.Text = "Файлы не найдены.";

                }


}
            else
            {
                string resultText = "Найдено файлов:\n";
                foreach (var category in FoundFiles)
                {
                    resultText += $"{category.Key} - {category.Value.Count}\n";
                }
                DialogResult SortResult = MessageBox.Show(resultText + "\n\n Подтвердите сортировку файлов", "Результаты поска",MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if(SortResult == DialogResult.Yes)
                {
                    await MoveFiles(DefaultPaths, guna2ProgressBar1, label);
                    label.Text = $"Найдено файлов: {FoundFiles.Values.Sum(list => list.Count)}, Выполняется сортировка";
                }

                label.Text = $"Найдено файлов: {FoundFiles.Values.Sum(list => list.Count)}, Сортировка отменена";
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка при поиске файлов: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    public async void BlinkButton(Button button1)
    {
        Color originalColor = button1.BackColor;
        for(int i = 0; i < 6; i++)
        {
            button1.BackColor = (i % 2 == 0) ? Color.FromArgb(80, 95, 95) : originalColor;
            await Task.Delay(300);        
        }
        button1.BackColor = originalColor;
    }

    public void ResetDefaultPaths()
    {
        InitializeDefaultPaths();
    }

    public async Task MoveFiles(Dictionary<string, string> categoryPaths, Guna.UI2.WinForms.Guna2ProgressBar progressBar, Label label)
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
                        DialogResult result = MessageBox.Show(
                            $"Файл {Path.GetFileName(file)} уже существует в папке {targetFolder}.\n" +
                            "Вы хотите заменить его?\n" +
                            "Да -- Заменить файл\n" +
                            "Нет -- Продолжить с пропуском этого файла\n" +
                            "Отмена -- Сортировка будет прервана",
                            "Конфликт файлов",
                            MessageBoxButtons.YesNoCancel,
                            MessageBoxIcon.Question
                        );

                        if (result == DialogResult.Cancel)
                        {
                            label.Text = "Перемещение файлов отменено.";
                            return;
                        }
                        else if (result == DialogResult.No)
                        {
                            progressBar.Value += 1;
                            continue; // Пропустить файл
                        }
                    }

                    // Перемещаем файл
                    File.Move(file, destinationPath);
                    progressBar.Value += 1;

                    // Эмуляция задержки для наглядности прогресса
                    await Task.Delay(100);
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