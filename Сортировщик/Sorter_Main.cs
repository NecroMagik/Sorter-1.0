using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using System.IO;
using Guna.UI2.WinForms;
using Сортировщик;

namespace Сортировщик
{
    public partial class Sorter_Main : Form
    {
        private FileManager fileManager;
        private Buttons buttons;
        public Sorter_Main()
        {
            InitializeComponent();
            InitializeClasses();
        }

        private void InitializeClasses()
        {
            // Создаем экземпляр FileManager
            fileManager = new FileManager(button1, label2, label3, label4, label5, checkBox1);

            // Связываем FileManager с Buttons
            buttons = new Buttons(
                fileManager,
                label1,            // Label для отображения статуса
                guna2ProgressBar1,           // Прогресс-бар
                checkBox1, // Чекбокс поиска в подпапках
                button3,                // Кнопка 3 (для изменения размеров окна)
                button5,
                checkBox2, checkBox3, checkBox4, checkBox5 // Чекбоксы категорий
            );

            // Привязка кнопок к методам Buttons
            button1.Click += (s, e) => buttons.HandleSelectFolderButtonClick();
            button2.Click += (s, e) => buttons.HandleSearchFilesButtonClick();
            button3.Click += (s, e) => buttons.HandleResizeButtonClick(this, groupBox1, groupBox2);
            button5.Click += (s, e) => buttons.HandleResetPathsButtonClick();
            button6.Click += (s, e) => buttons.HandleLanguageButtonClick();
        }
        // Дополнительная обработка для кнопок 9-12
        private void button9_Click(object sender, EventArgs e)
        {
            buttons.HandleSelectCategoryPath("Фото", label2);
        }

        private void button10_Click(object sender, EventArgs e)
        {
            buttons.HandleSelectCategoryPath("Видео", label3);
        }

        private void button11_Click(object sender, EventArgs e)
        {
            buttons.HandleSelectCategoryPath("Музыка", label4);
        }

        private void button12_Click(object sender, EventArgs e)
        {
            buttons.HandleSelectCategoryPath("Документы", label5);
        }

        public void INFORMATION()
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            var status = "Пересборка";
            var LastUp = "09.12.2024";
            this.Text = $"Сортировщик файлов   {version}";
            ver = version.ToString();
            LasTUPe = LastUp;
            Stat = status;
        }
        string Stat;
        string LasTUPe;
        string ver;

        private void Sorter_Main_Load(object sender, EventArgs e)       //Загрузка приложения
        {
            INFORMATION();
            this.Size = new System.Drawing.Size(395, 272);
        }

        

        private void button4_Click(object sender, EventArgs e)          //О приложении
        {
            MessageBox.Show($"Версия приложения: {ver}\n" +
                $"Статус: {Stat}\n" +
                $"Последние изменения: {LasTUPe}"

                , "О приложении", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private async void button8_Click(object sender, EventArgs e)    //Кнопка обновления приложения
        {
            var updater = new Updater();

            try
            {
                // Текущая версия приложения
                string currentVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();

                // Папка для временных файлов
                string tempFolder = Path.Combine(Path.GetTempPath(), "Sorter");
                if (!Directory.Exists(tempFolder))
                {
                    Directory.CreateDirectory(tempFolder);
                }

                // Показываем прогрессбар
                guna2ProgressBar1.Style = ProgressBarStyle.Marquee;

                // Скачиваем манифест
                string manifestPath = await updater.DownloadManifestAsync(tempFolder);

                // Читаем манифест
                Updater.UpdateManifest manifest = updater.ReadManifest(manifestPath);
                string latestVersion = manifest.Version;

                if (string.IsNullOrEmpty(latestVersion))
                {
                    MessageBox.Show("Ошибка при получении версии. Попробуйте позже.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Сравнение версий
                if (string.Compare(currentVersion, latestVersion) < 0)
                {
                    // Получение списка изменений
                    string changelog = await updater.GetChangelogAsync();
                    changelog = changelog ?? "Нет доступного списка изменений.";

                    // Предложение обновления
                    DialogResult result = MessageBox.Show(
                        $"Доступна новая версия {latestVersion}.\n\nСписок изменений:\n{changelog}\n\nОбновить сейчас?",
                        "Обновление доступно",
                        MessageBoxButtons.YesNo,
                    MessageBoxIcon.Information);

                    if (result == DialogResult.Yes)
                    {
                        guna2ProgressBar1.Style = ProgressBarStyle.Continuous;
                        guna2ProgressBar1.Value = 0;

                        // Запускаем процесс обновления
                        await updater.DownloadAndInstallUpdateAsync(progressBar1);
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
                guna2ProgressBar1.Style = ProgressBarStyle.Continuous;
                guna2ProgressBar1.Value = 0;
            }
        }

       
    }
}
