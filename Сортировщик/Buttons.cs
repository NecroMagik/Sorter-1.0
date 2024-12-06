using System;
using System.Collections.Generic;
using System.Windows.Forms;

public class Buttons
{
    private readonly FileManager fileManager;
    private readonly Label statusLabel;
    private readonly CheckBox searchSubfoldersCheckBox;
    private readonly CheckBox[] categoryCheckBoxes;

    public Buttons(FileManager fileManager, Label statusLabel, CheckBox searchSubfoldersCheckBox, params CheckBox[] categoryCheckBoxes)
    {
        this.fileManager = fileManager;
        this.statusLabel = statusLabel;
        this.searchSubfoldersCheckBox = searchSubfoldersCheckBox;
        this.categoryCheckBoxes = categoryCheckBoxes;
    }

    public void HandleSelectFolderButtonClick()
    {
        fileManager.SelectFolder(statusLabel);
        var activeCategories = GetActiveCategories();

        if (activeCategories.Count == 0)
        {
            MessageBox.Show("Выберите хотя бы одну категорию для поиска.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        fileManager.SearchFiles(searchSubfoldersCheckBox.Checked, activeCategories, statusLabel);
    }

    public void HandleMoveFilesButtonClick()
    {
        if (string.IsNullOrEmpty(fileManager.SelectedFolder))
        {
            MessageBox.Show("Выберите папку для перемещения.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        using (var folderDialog = new FolderBrowserDialog())
        {
            if (folderDialog.ShowDialog() == DialogResult.OK)
            {
                var activeCategories = GetActiveCategories();
                if (activeCategories.Count == 0)
                {
                    MessageBox.Show("Выберите хотя бы одну категорию для переноса.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                fileManager.MoveFiles(folderDialog.SelectedPath, activeCategories, statusLabel);
            }
        }
    }

    public void HandleShowCategoriesButtonClick(Button categoryButton)
    {
        foreach (var checkBox in categoryCheckBoxes)
        {
            checkBox.Visible = !checkBox.Visible;
        }

        categoryButton.Text = categoryCheckBoxes[0].Visible ? "Скрыть категории" : "Показать категории";
    }

    private List<string> GetActiveCategories()
    {
        var activeCategories = new List<string>();
        if (categoryCheckBoxes.Length > 0 && categoryCheckBoxes[0].Checked) activeCategories.Add("Фото");
        if (categoryCheckBoxes.Length > 1 && categoryCheckBoxes[1].Checked) activeCategories.Add("Видео");
        if (categoryCheckBoxes.Length > 2 && categoryCheckBoxes[2].Checked) activeCategories.Add("Музыка");
        if (categoryCheckBoxes.Length > 3 && categoryCheckBoxes[3].Checked) activeCategories.Add("Документы");
        return activeCategories;
    }
}
