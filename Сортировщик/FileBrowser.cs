using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Сортировщик
{
    public partial class FileBrowser : Form
    {
        private readonly Dictionary<string, List<string>> filesByCategory;
        private readonly Func<Task> transferAction; // Делегат для выполнения переноса
        public bool TransferConfirmed { get; private set; } // Флаг, подтверждён ли перенос
        private int dm = -1; // Текущая тема (0 - светлая, 1 - тёмная)
        private string darkp = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\ZeN\Sorter\dark.txt";

        public FileBrowser(Dictionary<string, List<string>> files, Func<Task> onTransferConfirmed)
        {
            InitializeComponent();
            filesByCategory = files;
            transferAction = onTransferConfirmed;
            TransferConfirmed = false; // Изначально перенос не подтверждён
        }

        private void FileBrowser_Load(object sender, EventArgs e)
        {
            // Установка темы
            if (File.Exists(darkp))
            {
                dm = Convert.ToInt32(File.ReadAllText(darkp));
            }
            ApplyTheme();

            // Заполняем список файлов
            PopulateFileList();
        }

        /// <summary>
        /// Применяет тему (светлую/тёмную) к элементам интерфейса.
        /// </summary>
        private void ApplyTheme()
        {
            if (dm == 1) // Тёмная тема
            {
                this.BackColor = ColorTranslator.FromHtml("#252525");
                listView1.BackColor = ColorTranslator.FromHtml("#252525");
                listView1.ForeColor = Color.White;
                button1.BackColor = Color.Khaki;
                button2.BackColor = Color.LightBlue;
            }
            else // Светлая тема
            {
                this.BackColor = SystemColors.ControlLight;
                listView1.BackColor = SystemColors.Window;
                listView1.ForeColor = Color.Black;
                button1.BackColor = Color.Khaki;
                button2.BackColor = Color.LightBlue;
            }
        }

        /// <summary>
        /// Заполняет ListView списком найденных файлов.
        /// </summary>
        private void PopulateFileList()
        {
            listView1.Items.Clear();

            foreach (var category in filesByCategory)
            {
                // Добавляем категорию как группу
                var categoryItem = new ListViewItem($"Категория: {category.Key}");
                categoryItem.Font = new Font(categoryItem.Font, FontStyle.Bold);
                categoryItem.BackColor = (dm == 1) ? ColorTranslator.FromHtml("#333333") : Color.LightGray; // Цвет для категории
                categoryItem.ForeColor = (dm == 1) ? Color.White : Color.Black; // Цвет текста категории
                listView1.Items.Add(categoryItem);

                // Добавляем файлы
                foreach (var file in category.Value)
                {
                    var fileItem = new ListViewItem($"  - {Path.GetFileName(file)}");
                    listView1.Items.Add(fileItem);
                }
            }
        }

        /// <summary>
        /// Кнопка "Перенести" - выполняет перенос файлов.
        /// </summary>
        private async void button1_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Вы уверены, что хотите перенести найденные файлы?",
                                "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                TransferConfirmed = true; // Устанавливаем флаг
                await transferAction?.Invoke(); // Выполняем передачу файлов
                this.Close(); // Закрываем окно
            }
        }

        /// <summary>
        /// Кнопка "Закрыть" - закрывает окно без подтверждения переноса.
        /// </summary>
        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
