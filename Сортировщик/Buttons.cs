using System.Collections.Generic;
using System.Windows.Forms;
using Сортировщик;

public class Buttons
{
    private FileManager fileManager;
    private Label statusLabel;
    private ProgressBar progressBar;
    private CheckBox searchSubfoldersCheckBox;
    private CheckBox[] categoryCheckBoxes;
    private Button button3;

    public Buttons(FileManager fileManager, Label statusLabel, ProgressBar progressBar, CheckBox searchSubfoldersCheckBox, Button button3, params CheckBox[] categoryCheckBoxes)
    {
        this.fileManager = fileManager;
        this.statusLabel = statusLabel;
        this.progressBar = progressBar;
        this.searchSubfoldersCheckBox = searchSubfoldersCheckBox;
        this.categoryCheckBoxes = categoryCheckBoxes;
        this.button3 = button3;
    }

    public void HandleSelectFolderButtonClick()
    {
        fileManager.SelectFolder(statusLabel);
    }

    public void HandleSearchFilesButtonClick()
    {
        List<string> activeCategories = GetActiveCategories();
        fileManager.SearchFiles(searchSubfoldersCheckBox.Checked, activeCategories, progressBar, statusLabel);
    }

    public void HandleMoveFilesButtonClick(Dictionary<string, string> categoryPaths)
    {
        fileManager.MoveFiles(categoryPaths, progressBar, statusLabel);
    }

    public void HandleResizeButtonClick(Form form, GroupBox groupBox1, GroupBox groupBox2)
    {
        if (form.Size.Height == 272)
        {
            form.Size = new System.Drawing.Size(395, 475);
            groupBox1.Visible = true;
            groupBox2.Visible = true;
        }
        else
        {
            form.Size = new System.Drawing.Size(395, 272);
            groupBox1.Visible = false;
            groupBox2.Visible = false;
        }
    }

    public void HandleResetPathsButtonClick()
    {
        fileManager.ResetDefaultPaths();
        button3.Size = new System.Drawing.Size(395, 475); // Сбросить размер
    }

    public void HandleLanguageButtonClick()
    {
        // Позже реализуем локализацию
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
