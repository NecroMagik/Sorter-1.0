using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using Guna.UI2.WinForms;
using Сортировщик;

public class Buttons
{
    private FileManager fileManager;
    private Label statusLabel;
    private Guna2ProgressBar guna2ProgressBar1;
    private CheckBox searchSubfoldersCheckBox;
    private CheckBox[] categoryCheckBoxes;
    private Button button3;
    private Button button5;
    private RadioButton radioButton1;
    private RadioButton radioButton2;
    private RadioButton radioButton3;

    public Buttons(FileManager fileManager, Label statusLabel, Guna2ProgressBar guna2ProgressBar1, CheckBox searchSubfoldersCheckBox, Button button3, Button button5, RadioButton radioButton1, RadioButton radioButton2, RadioButton radioButton3, params CheckBox[] categoryCheckBoxes)
    {
       this.fileManager = fileManager;
        this.statusLabel = statusLabel;
        this.searchSubfoldersCheckBox = searchSubfoldersCheckBox;
        this.categoryCheckBoxes = categoryCheckBoxes;
        this.button3 = button3;
        this.button5 = button5;
        this.radioButton1 = radioButton1;
        this.radioButton2 = radioButton2;
        this.radioButton3 = radioButton3;
        this.guna2ProgressBar1 = guna2ProgressBar1;
        guna2ProgressBar1.Value = 0;
    }

    public void HandleAboutButtonClick(string version, string status, string lastUpdate)
    {
        MessageBox.Show(
            $"Версия: {version}\n" +
            $"Статус: {status}\n" +
            $"Последнее обновление: {lastUpdate}",
            "О приложении",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information
        );
    }

    // Метод для кнопки 8 (Проверка обновлений)
    public async void HandleUpdateButtonClick(Guna2ProgressBar guna2ProgressBar1)
    {
        var updater = new Updater(guna2ProgressBar1, radioButton1, radioButton2, radioButton3);

        try
        {
            // Текущая версия приложения
            string currentVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();

            // Проверка обновлений
            string tempFolder = Path.Combine(Path.GetTempPath(), "Sorter");
            if (!Directory.Exists(tempFolder))
            {
                Directory.CreateDirectory(tempFolder);
            }

            guna2ProgressBar1.Value = 0; // Сбрасываем прогресс

            string manifestPath = await updater.DownloadManifestAsync(tempFolder);
            Updater.UpdateManifest manifest = updater.ReadManifest(manifestPath);

            if (string.Compare(currentVersion, manifest.Version) < 0)
            {
                string changelog = await updater.GetChangelogAsync();
                DialogResult result = MessageBox.Show(
                    $"Доступна новая версия: {manifest.Version}\n\nСписок изменений:\n{changelog}\n\nУстановить обновление?",
                    "Обновление доступно",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Information
                );

                if (result == DialogResult.Yes)
                {
                    guna2ProgressBar1.Value = 0; // Сбрасываем прогресс
                    await updater.DownloadAndInstallUpdateAsync();
                }
            }
            else
            {
                MessageBox.Show("У вас установлена последняя версия приложения.", "Обновлений нет", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка при проверке обновлений: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            guna2ProgressBar1.Value = 0; // Сбрасываем прогресс
        }
    }

    public void HandleSelectFolderButtonClick()
    {
        fileManager.SelectFolder(statusLabel);
    }

    public void HandleSearchFilesButtonClick()
    {
        List<string> activeCategories = GetActiveCategories();
        fileManager.SearchFiles(searchSubfoldersCheckBox.Checked, activeCategories, guna2ProgressBar1, statusLabel);
    }

    public async void HandleMoveFilesButtonClick(Dictionary<string, string> categoryPaths)
    {
        await fileManager.MoveFiles(categoryPaths, guna2ProgressBar1, statusLabel);
    }

    public void HandleResizeButtonClick(Form form, GroupBox groupBox1, GroupBox groupBox2)
    {
        if (form.Size.Height == 670)
        {
            form.Size = new System.Drawing.Size(428, 313);
            groupBox1.Visible = false;
            groupBox2.Visible = false;
        }
        else
        {
            form.Size = new System.Drawing.Size(428, 670);
            groupBox1.Visible = true;
            groupBox2.Visible = true;
        }
    }

    public void HandleResetPathsButtonClick()
    {
        button5.Visible = false;
        fileManager.ResetDefaultPaths();
        button3.Size = new System.Drawing.Size(389, 73); // Сбросить размер
    }

    public void HandleSelectCategoryPath(string category, Label label)
    {
        using (var folderDialog = new FolderBrowserDialog())
        {
            if (folderDialog.ShowDialog() == DialogResult.OK)
            {
                fileManager.DefaultPaths[category] = folderDialog.SelectedPath;
                label.Text = $"Пользовательский путь: {folderDialog.SelectedPath}";
                MessageBox.Show($"Путь для категории {category} установлен в: {folderDialog.SelectedPath}",
                                "Путь изменён",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show($"Выбор пути для категории {category} отменён.",
                                "Отмена",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
            }
        }
        if (button5.Visible == false)
        {
            button5.Visible = true;
            button3.Size = new System.Drawing.Size(235, 73);
        }
    }

    private List<string> GetActiveCategories()
    {
        var activeCategories = new List<string>();
        if (categoryCheckBoxes.Length >= 1 && categoryCheckBoxes[0].Checked) activeCategories.Add("Фото");
        if (categoryCheckBoxes.Length >= 2 && categoryCheckBoxes[1].Checked) activeCategories.Add("Видео");
        if (categoryCheckBoxes.Length >= 3 && categoryCheckBoxes[2].Checked) activeCategories.Add("Музыка");
        if (categoryCheckBoxes.Length >= 4 && categoryCheckBoxes[3].Checked) activeCategories.Add("Документы");
        return activeCategories;
    }
}
