using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using System.Net.Http;
using System.Threading;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolBar;

namespace Сортировщик
{
    public partial class Form1 : Form
    {
        private System.Windows.Forms.ToolTip toolTip;
        public Form1()
        {
            InitializeComponent();
            toolTip = new System.Windows.Forms.ToolTip
            {
                InitialDelay = 500,
                ReshowDelay = 100,
                AutoPopDelay = 5000,
                ShowAlways = true
            };
            toolTip.SetToolTip(button32, "Показать список изменений");
            toolTip.SetToolTip(button1, "Выберите папку для сортировки");
        }

        public void INFORMATION()  //Информация о приложении
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            var status = "Альфа-Тест завершается переходом на Бета";
            var LastUp = "02.12.2024";
            var AppName = "Alpha {3}";
            this.Text = $"Сортировщик ({version})  --  {AppName}";
            label1.Text = $"Версия: {version}";
            label2.Text = $"Статус: {status}";
            label3.Text = $"Последние изменения: {LastUp}";
        }

        




        //=========================================     Н А Ч А Л О     ===========================================



        private void Form1_Load(object sender, EventArgs e)  //Загрузка формы
        {

            
            INFORMATION();
            InitializeDefaultPath();
            LoadCustomPaths();

            tabControl1.ItemSize = new Size(0, 1);
            // Проверка текущей темы
            if (File.Exists(darkp))
            {
                string savedTheme = File.ReadAllText(darkp);
                if (savedTheme == "1")
                {
                    Dark();
                    tabControl1.Parent.Refresh();
                    Refresh();
                }
                else
                {
                    Light();
                    tabControl1.Parent.Refresh();
                    Refresh();
                }
            }
            else
            {
                // По умолчанию светлая тема
                ApplyTheme(SystemColors.ControlLight, SystemColors.Control, SystemColors.ControlText, Color.Orange);
                Refresh();
            }
        

        textBox2.ScrollBars = ScrollBars.Vertical;
            b2 = 1;
            tabset = 0;

            #region ПОДСКАЗКИ
            checkBox5.Checked = false;
            toolTip.SetToolTip(button31, "Нажмите, чтобы настроить параметры переноса");
            #endregion

            // Установка начального состояния чекбоксов
            checkBox6.Visible = false;
            checkBox7.Visible = false;
            checkBox8.Visible = false;
            checkBox9.Visible = false;

            // По умолчанию активны
            checkBox6.Checked = true;
            checkBox7.Checked = true;
            checkBox8.Checked = true;
            checkBox9.Checked = true;

            // Добавляем подсказку


            // Новый путь к директории приложения
            string appDataBasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ZeN", "Sorter");
            if (!Directory.Exists(appDataBasePath))
            {
                Directory.CreateDirectory(appDataBasePath);
            }

            this.Size = new System.Drawing.Size(336, 195); // Задать размер формы
            tabControl1.TabPages.Clear();
            groupBox2.Visible = false; // Скрыть синхронизацию папок.

            

            if (File.Exists(Langt)) // Проверка языка
            {
                Lang = File.ReadAllText(Langt);
                L = Convert.ToInt32(Lang);
                label5.Text = "Проверка переменной языка:   " + Lang;
            }
            else
            {
                button1.Text = "";
                button1.Enabled = false;
                button2.Text = "";
                button2.Enabled = false;
                button4.Text = "";
                button4.Enabled = false;
                button9.Text = "";
                button9.Enabled = false;
                set = 1;
                this.Size = new Size(336, 470);
                tabControl1.TabPages.Add(tabPage5);
                Refresh();
            }

            if (File.Exists(CCloud)) // Проверка облака, которого нет в списке
            {
                label4.Text = File.ReadAllText(CCloud);
                button21.Text = label4.Text;
            }

            if (File.Exists(b2s)) // Проверка триггера для переноса
            {
                string a = File.ReadAllText(b2s);
                b2 = Convert.ToInt32(a);
                if (b2 == 0)
                {
                    button2.BackColor = Color.PeachPuff;
                }
                else
                {
                    if (b2 == 1)
                    {
                        button2.BackColor = Color.LightGreen;
                    }
                }
            }
            else
            {
                b2 = 1;
                string a = b2.ToString();
                using (StreamWriter writer = new StreamWriter(b2s))
                {
                    writer.Write(a);
                }
                button2.BackColor = Color.LightGreen; // Подсветка кнопки "Выполнить перенос"
            }

            //Проверка самой кнопки переноса


            if (button21.Text == "Облако*")
            {
                label7.Text = "тест пути: " + Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            }
            else
            {
                label7.Text = "тест пути: " + Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), label4.Text, "Изображения");
            }

            progressBar1.Visible = false;
            label9.Text = "Значение открытой панели" + tabset.ToString();
        }


        #region =====  ГЛОБАЛЬНЫЕ ПЕРЕМЕННЫЕ  =====

        // Глобальные переменные для управления категориями
        bool isPhotosEnabled = true;  // По умолчанию активны
        bool isVideosEnabled = true;
        bool isMusicEnabled = true;
        bool isDocumentsEnabled = true;

        
        int tabset = 0;     //Переменная настроек (скрыть/показать)
        

        int gdrive;
        int onedrive;
        int yadrive;

        int dev = 0;     // Режим разработчика
        int dm = -1;     // Тёмный режим
        int L;           // Локализация
        private static string appDataBasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ZeN", "Sorter");
        string selectedPathFile = Path.Combine(appDataBasePath, "selectedPath.txt");
        string CCloud = Path.Combine(appDataBasePath, "customcloud.txt");     //Облако отсутствует в списке
        string Langt = Path.Combine(appDataBasePath, "lang.txt");     //Проверка наличия языка
        string darkp = Path.Combine(appDataBasePath, "dark.txt");     //Проверка тёмной темы
        string b2s = Path.Combine(appDataBasePath, "b2s.txt");     //Триггер переноса файлов.txt
        int b2 = 0;     //Проверка триггера для переноса
        int groupcloud = 0;     //Переменнвая вызова папок синхронезации и подтверждения облака


        #endregion

        //Обновление приложения
        #region        ========= U P D A T E S =========



// Проверка обновлений
private async void button3_Click(object sender, EventArgs e)
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
            progressBar1.Visible = true;
            progressBar1.Style = ProgressBarStyle.Marquee;

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
                    progressBar1.Style = ProgressBarStyle.Continuous;
                    progressBar1.Value = 0;

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
            // Скрываем ProgressBar в любом случае
            progressBar1.Visible = false;
        }
    }

    // Отображение полного списка изменений
    private async void button32_Click(object sender, EventArgs e)
    {
        string changelogFullUrl = "https://raw.githubusercontent.com/NecroMagik/Sorter-1.0/refs/heads/Release/releases/ChangeLog_Full.txt";

        using (HttpClient client = new HttpClient())
        {
            try
            {
                // Загружаем changelog с сервера
                string changelog = await client.GetStringAsync(changelogFullUrl);

                // Приводим переносы строк к корректному формату
                changelog = changelog.Replace("\n", Environment.NewLine);

                // Отображаем текст в TextBox
                textBox2.Text = changelog;
                textBox2.Visible = true;

                // Меняем текст кнопки и обработчик
                button32.Text = "Скрыть изменения";
                button32.Click -= button32_Click;
                button32.Click += button32_Hide_Click;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке списка изменений: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    // Метод для скрытия TextBox2
    private void button32_Hide_Click(object sender, EventArgs e)
    {
        // Скрываем поле и возвращаем текст кнопки
        textBox2.Visible = false;
        button32.Text = "Показать изменения";

        // Настраиваем кнопку для повторного показа TextBox
        button32.Click -= button32_Hide_Click;
        button32.Click += button32_Click;
    }

    #endregion





    #region == Перехват команды Alt+F4 ==

    protected override void WndProc(ref Message m)
        {
            const int WM_CLOSE = 0x0010;
            if (m.Msg == WM_CLOSE)
            {
                if (ConfirmDialogRU())
                {
                    Application.Exit();
                }
                return;
            }
            base.WndProc(ref m);
        }

        #endregion

        //Диалоги
        #region == DIALOGS ==

        bool ConfirmDialogRU() //Диалог выхода
        {
            DialogResult confirm = MessageBox.Show("Во избежание случайного закрытия программы пожалуйста подтвердите выход из приложения.", "Закрыть приложение?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm == DialogResult.Yes)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        bool DeleteData()
        {
            DialogResult confirm = MessageBox.Show("Это удалит все введённые вами данные приложения. \nВ случае подтверждения приложение перезапустится", "Вы уверены?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm == DialogResult.Yes)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion

        // Ядро кода
        #region == CORE ==                                                                        == CORE ==                                                                        ...

        #region ----path----

        string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\Downloads";     //Берётся значение из проводника
        string doc = (Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));    //Документы
        string mus = (Environment.GetFolderPath(Environment.SpecialFolder.MyMusic));        //Музыка
        string pic = (Environment.GetFolderPath(Environment.SpecialFolder.MyPictures));     //Картинки
        string vid = (Environment.GetFolderPath(Environment.SpecialFolder.MyVideos));       //Видосы

        
        string Lang;
        string dmo;
        string Cloudpath =  Path.Combine(appDataBasePath,"cloudpath.txt");
        string cloudR =     Path.Combine(appDataBasePath,"cloudt.txt");

        #endregion

        #region ----Standart Methods----

        private void SaveSelectedPath(string path)
        {
            try
            {
                File.WriteAllText(selectedPathFile, path);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении пути: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InitializeProgressBar(int maxValue)
        {
            progressBar1.Visible = true; // Делаем прогресс-бар видимым
            progressBar1.Value = 0; // Сбрасываем прогресс
            progressBar1.Maximum = maxValue; // Устанавливаем максимальное значение
        }

        int abort = 0;

        //-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Выполняет поиск файлов в папке и подпапках (опционально) по категориям: фотографии, музыка, видео, документы.
        /// </summary>
        /// <param name="path">Путь к папке.</param>
        /// <param name="includeSubfolders">Флаг поиска в подпапках.</param>
        /// <returns>Словарь категорий с соответствующими списками файлов.</returns>
        private async Task<Dictionary<string, List<string>>> SearchFilesAsync(string path, bool includeSubfolders)
        {
            var categories = new Dictionary<string, List<string>>
    {
        { "Фотографии", new List<string>() },
        { "Музыка", new List<string>() },
        { "Видео", new List<string>() },
        { "Документы", new List<string>() }
    };

            var searchOption = includeSubfolders ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

            // Поиск только в выбранных категориях
            if (ShouldProcessCategory("Фотографии"))
            {
                categories["Фотографии"].AddRange(SafeGetFiles(path, "*.jpg;*.jpeg;*.png;*.bmp;*.gif", searchOption));
            }
            if (ShouldProcessCategory("Музыка"))
            {
                categories["Музыка"].AddRange(SafeGetFiles(path, "*.mp3;*.wav;*.flac;*.aac;*.ogg", searchOption));
            }
            if (ShouldProcessCategory("Видео"))
            {
                categories["Видео"].AddRange(SafeGetFiles(path, "*.mp4;*.avi;*.mkv;*.mov;*.webm", searchOption));
            }
            if (ShouldProcessCategory("Документы"))
            {
                categories["Документы"].AddRange(SafeGetFiles(path, "*.doc;*.docx;*.pdf;*.xls;*.xlsx;*.txt", searchOption));
            }

            return categories;
        }


        //-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        private IEnumerable<string> SafeGetFiles(string path, string searchPattern, SearchOption searchOption)
        {
            try
            {
                return Directory.EnumerateFiles(path, searchPattern, searchOption);
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine($"Нет доступа к папке: {path}");
                return Enumerable.Empty<string>();
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Ошибка при доступе к папке: {path}, сообщение: {ex.Message}");
                return Enumerable.Empty<string>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Неизвестная ошибка: {ex.Message}");
                return Enumerable.Empty<string>();
            }
        }


        //-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Генерирует подробный список найденных файлов для отображения.
        /// </summary>
        /// <param name="filesByCategory">Словарь категорий с файлами.</param>
        /// <returns>Список строк с названием файлов и их категориями.</returns>
        private string GenerateFileList(Dictionary<string, List<string>> filesByCategory)
        {
            StringBuilder fileList = new StringBuilder();

            foreach (var category in filesByCategory)
            {
                fileList.AppendLine($"Категория: {category.Key} ({category.Value.Count} файлов)");

                foreach (var file in category.Value)
                {
                    fileList.AppendLine($"  - {Path.GetFileName(file)}"); // Добавляем имя файла
                }

                fileList.AppendLine(); // Пустая строка для разделения категорий
            }

            return fileList.ToString();
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Перемещает файлы в системные папки в зависимости от их категории.
        /// </summary>
        /// <param name="filesByCategory">Словарь с категориями и файлами.</param>
        private async Task TransferFilesAsync(Dictionary<string, List<string>> filesByCategory)
        {
            // Папки по умолчанию
            string defaultPicturesPath = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            string defaultMusicPath = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
            string defaultVideosPath = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos);
            string defaultDocumentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            // Пользовательские пути (если есть)
            var destinationPaths = new Dictionary<string, string>
    {
        { "Фотографии", LoadCustomPath("Photos", defaultPicturesPath) },
        { "Музыка", LoadCustomPath("Music", defaultMusicPath) },
        { "Видео", LoadCustomPath("Videos", defaultVideosPath) },
        { "Документы", LoadCustomPath("Documents", defaultDocumentsPath) }
    };

            // Логика переноса файлов
            int totalFiles = filesByCategory.Values.Sum(list => list.Count);
            InitializeProgressBar(totalFiles);

            int processedFiles = 0;

            foreach (var category in filesByCategory)
            {
                foreach (var file in category.Value)
                {
                    string fileName = Path.GetFileName(file);
                    string destination = Path.Combine(destinationPaths[category.Key], fileName);

                    try
                    {
                        if (!File.Exists(destination))
                        {
                            File.Move(file, destination);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при перемещении файла {file}: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    processedFiles++;
                    progressBar1.Value = processedFiles;
                    await Task.Delay(10); // Эмуляция задержки
                }
            }

            MessageBox.Show("Файлы успешно перемещены!", "Готово", MessageBoxButtons.OK, MessageBoxIcon.Information);
            progressBar1.Visible = false;
        }






        private string LoadCustomPath(string category, string defaultPath)
        {
            string configFilePath = Path.Combine(appDataBasePath, $"{category}_Path.txt");
            return File.Exists(configFilePath) ? File.ReadAllText(configFilePath) : defaultPath;
        }


        /// <summary>
        /// Определяет, следует ли обрабатывать указанную категорию, исходя из состояния checkBox.
        /// </summary>
        /// <param name="category">Название категории.</param>
        /// <returns>True, если категория включена, иначе False.</returns>
        private bool ShouldProcessCategory(string category)
        {
            return (category == "Фотографии" && checkBox6.Checked) ||
                   (category == "Видео" && checkBox7.Checked) ||
                   (category == "Музыка" && checkBox8.Checked) ||
                   (category == "Документы" && checkBox9.Checked);
        }


        //-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Открывает окно FileBrowser с переданным списком файлов и логикой переноса.
        /// </summary>
        /// <param name="filesByCategory">Словарь категорий с файлами.</param>
        private async void ShowFileListInNewWindow(Dictionary<string, List<string>> filesByCategory)
        {
            // Создаём новое окно FileBrowser
            var fileBrowser = new FileBrowser(
                filesByCategory,
                async () => await TransferFilesAsync(filesByCategory) // Передаём метод для асинхронного переноса
            );

            // Открываем FileBrowser как модальное окно
            fileBrowser.ShowDialog();

            // Проверяем, подтвердил ли пользователь перенос файлов
            if (fileBrowser.TransferConfirmed)
            {
                MessageBox.Show("Файлы успешно перемещены!", "Готово", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Перенос отменён.", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void InitializeDefaultPath()     // Инициализирует путь для переноса.
        {
            // Создаём директорию для хранения пути, если она не существует
            string appDataPath = Path.GetDirectoryName(selectedPathFile);
            if (!Directory.Exists(appDataPath))
            {
                Directory.CreateDirectory(appDataPath);
            }

            // Проверяем существование файла пути
            if (File.Exists(selectedPathFile))
            {
                folderPath = File.ReadAllText(selectedPathFile).Trim();

                // Если файл пуст или путь недоступен, возвращаем путь по умолчанию
                if (string.IsNullOrEmpty(folderPath) || !Directory.Exists(folderPath))
                {
                    folderPath = GetDefaultPath();
                    SaveSelectedPath(folderPath);
                }
            }
            else
            {
                // Устанавливаем путь по умолчанию, если файла нет
                folderPath = GetDefaultPath();
                SaveSelectedPath(folderPath);
            }

            // Отображаем текущий путь
            textBox1.Text = $"Выбрано: {folderPath}";
        }

        private string GetDefaultPath()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
        }

        #endregion

        #region ----buttons----

        private void button1_Click(object sender, EventArgs e)
        {
            // Сбрасываем путь до значения по умолчанию
            string defaultFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");

            using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
            {
                var result = folderDialog.ShowDialog();

                if (result == DialogResult.OK)
                {
                    // Сохраняем новый путь и обновляем UI
                    folderPath = folderDialog.SelectedPath;
                    SaveSelectedPath(folderPath);
                    textBox1.Text = "Выбрано: " + folderPath;

                    // Обновляем флаг и подсветку
                    b2 = 1;
                    string a = b2.ToString();
                    using (StreamWriter writer = new StreamWriter(Path.Combine(appDataBasePath, "b2s.txt")))
                    {
                        writer.Write(a);
                    }
                    button2.BackColor = Color.LightGreen;
                }
                else if (result == DialogResult.Cancel)
                {
                    // Возвращаем путь и текстовое поле к значению по умолчанию
                    folderPath = defaultFolderPath;
                    textBox1.Text = "По умолчанию: " + folderPath;
                }
            }
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            // Проверка: выбрана ли папка
            if (string.IsNullOrEmpty(folderPath))
            {
                MessageBox.Show("Выберите папку перед началом выполнения!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                button1.BackColor = Color.LightCoral; // Подсветка кнопки "Выбор папки"
                return;
            }

            // Показ прогресс-бара
            progressBar1.Visible = true;
            progressBar1.Value = 0;
            textBox1.Text = "Идёт поиск файлов...";

            // Асинхронный поиск файлов
            var filesByCategory = await SearchFilesAsync(folderPath, checkBox5.Checked);

            // Фильтрация только по выбранным категориям
            var selectedFiles = filesByCategory
                .Where(category => ShouldProcessCategory(category.Key))
                .ToDictionary(category => category.Key, category => category.Value);

            // Проверка: найдены ли файлы
            if (!selectedFiles.Values.Any(list => list.Count > 0))
            {
                progressBar1.Visible = false; // Скрываем прогресс-бар
                MessageBox.Show("Файлы не найдены для выбранных категорий.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox1.Text = "Выбрано: " + folderPath;
                return;
            }

            // Перенос файлов
            await TransferFilesAsync(selectedFiles);
        }


        #region ----ДРУГОЕ РАСПОЛОЖЕНИЕ----

        private void SelectCustomPath(string category, Label label)
        {
            using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
            {
                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    string selectedPath = folderDialog.SelectedPath;

                    // Сохраняем путь в файл
                    string configFilePath = Path.Combine(appDataBasePath, $"{category}_Path.txt");
                    File.WriteAllText(configFilePath, selectedPath);

                    // Отображаем путь в метке
                    label.Text = $": {selectedPath}";

                    // Показываем кнопку "Сбросить пути" и уменьшаем размер кнопки "Выбрать папку"
                    button33.Visible = true;
                    button26.Size = new Size(133, 38);
                }
            }
        }

        private void LoadCustomPaths()
        {
            // Категории и метки
            var pathsAndLabels = new Dictionary<string, Label>
    {
        { "Photos", label11 },
        { "Videos", label12 },
        { "Music", label13 },
        { "Documents", label14 }
    };

            bool customPathExists = false;

            foreach (var entry in pathsAndLabels)
            {
                string configFilePath = Path.Combine(appDataBasePath, $"{entry.Key}_Path.txt");
                if (File.Exists(configFilePath))
                {
                    string savedPath = File.ReadAllText(configFilePath);
                    entry.Value.Text = $": {savedPath}";
                    customPathExists = true;
                }
                else
                {
                    //entry.Value.Text = ": По умолчанию";
                }
            }

            // Устанавливаем видимость и размеры кнопок
            button33.Visible = customPathExists;
            button26.Size = customPathExists ? new Size(133, 38) : new Size(282, 38);
        }


        private void ResetCustomPaths()
        {
            // Список файлов конфигурации для кастомных путей
            var customPathFiles = new[]
            {
        Path.Combine(appDataBasePath, "Photos_Path.txt"),
        Path.Combine(appDataBasePath, "Videos_Path.txt"),
        Path.Combine(appDataBasePath, "Music_Path.txt"),
        Path.Combine(appDataBasePath, "Documents_Path.txt")
    };

            // Удаляем файлы, если они существуют
            foreach (var pathFile in customPathFiles)
            {
                if (File.Exists(pathFile))
                {
                    File.Delete(pathFile);
                }
            }

            // Сбрасываем метки
            label11.Text = "";
            label12.Text = "";
            label13.Text = "";
            label14.Text = "";

            // Прячем кнопку "Сбросить пути"
            button33.Visible = false;

            // Восстанавливаем размер кнопки "Выбрать папку"
            button26.Size = new Size(282, 38);
        }

        private void button27_Click(object sender, EventArgs e) // Фото
        {
            SelectCustomPath("Photos", label11);
        }

        private void button28_Click(object sender, EventArgs e) // Видео
        {
            SelectCustomPath("Videos", label12);
        }

        private void button29_Click(object sender, EventArgs e) // Музыка
        {
            SelectCustomPath("Music", label13);
        }

        private void button30_Click(object sender, EventArgs e) // Документы
        {
            SelectCustomPath("Documents", label14);
        }

        private void button33_Click(object sender, EventArgs e)
        {
            ResetCustomPaths();
        }

        #endregion
        #endregion
        #endregion

        //Настройки
        #region == SETTINGS ==

        int set = 0;
        private void CloseTabPage(TabPage tabPage)
        {
            if (tabControl1.TabPages.Contains(tabPage))
            {
                tabControl1.TabPages.Remove(tabPage); // Удаляем вкладку
            }

            if (!tabControl1.TabPages.Contains(tabPage1)) // Возврат на основную вкладку
            {
                tabControl1.TabPages.Add(tabPage1);
            }
        }

        private TabPage currentTabPage; // Хранение текущей активной вкладки

        private void button4_Click(object sender, EventArgs e)
        {
            if (set == 0)
            {
                // Расширяем форму и показываем панель настроек
                button4.Text = "↑Убрать панель↑";
                set = 1;
                this.Size = new Size(336, 480);

                // Если вкладка не выбрана, устанавливаем текущую
                if (currentTabPage == null)
                {
                    currentTabPage = tabPage1; // Устанавливаем основную вкладку по умолчанию
                }

                if (!tabControl1.TabPages.Contains(currentTabPage))
                {
                    tabControl1.TabPages.Add(currentTabPage);
                }

                tabControl1.SelectedTab = currentTabPage; // Переключаемся на сохранённую вкладку

                // Добавляем Debug Mode, если активен
                if (dev == 1 && !tabControl1.TabPages.Contains(tabPage4))
                {
                    tabControl1.TabPages.Add(tabPage4);
                }

                Refresh();
            }
            else
            {
                // Сворачиваем панель и сохраняем текущую вкладку
                button4.Text = "↓Показать панель↓";
                set = 0;
                this.Size = new Size(336, 195);

                currentTabPage = tabControl1.SelectedTab; // Сохраняем активную вкладку
                tabControl1.TabPages.Clear(); // Убираем все вкладки
                Refresh();
            }
        }

        private void button9_Click(object sender, EventArgs e)  //Выход
        {
            if (ConfirmDialogRU())
            {
                Application.Exit();
            }
        }

        #region облачные сервисы

        private void button8_Click(object sender, EventArgs e)     //Облачные сервисы
        {

            tabControl1.TabPages.Add(tabPage6);
            tabControl1.SelectedTab = tabPage6;
            tabControl1.TabPages.Remove(tabPage1);
        }

        private void button17_Click(object sender, EventArgs e)     //Не использую = 1
        {
            groupcloud = 1;
            groupBox2.Visible = true;
            button25.PerformClick();
        }

        private void button18_Click(object sender, EventArgs e)     //Гугл диск = 2
        {
            groupcloud = 2;
            groupBox2.Visible = true;
        }

        private void button24_Click(object sender, EventArgs e)     //Проверка библиотек
        {
            MessageBox.Show("Фотографии: " + pic + "\n" + "\nВидео: " + vid + "\n" + "\nМузыка: " + mus + "\n" + "\nДокументы: " + doc, "Пути библиотек:");
        }

        private void button23_Click(object sender, EventArgs e)     //Назад из облачных сервисов
        {
            CloseTabPage(tabPage6);
        }

        private void button22_Click(object sender, EventArgs e)     //Кнопка Сброс
        {
            File.Delete(CCloud);
            button21.Text = "Облако*";
        }
        #region Выбор папок

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox5.Checked)
            {
                toolTip.SetToolTip(button2, "Выполняет сортировку файлов в указанной папке и её подпапках");
            }
            else
            {
                toolTip.SetToolTip(button2, "Выполняет сортировку файлов только в указанной папке");
            }
        }

        // Обработчики событий для каждого чекбокса
        private void checkBox6_CheckedChanged(object sender, EventArgs e) // Фото
        {
            isPhotosEnabled = checkBox6.Checked;
        }

        private void checkBox7_CheckedChanged(object sender, EventArgs e) // Видео
        {
            isVideosEnabled = checkBox7.Checked;
        }

        private void checkBox8_CheckedChanged(object sender, EventArgs e) // Музыка
        {
            isMusicEnabled = checkBox8.Checked;
        }

        private void checkBox9_CheckedChanged(object sender, EventArgs e) // Документы
        {
            isDocumentsEnabled = checkBox9.Checked;
        }


        private void button31_Click(object sender, EventArgs e)
        {
            // Переключение видимости кнопки и чекбоксов
            button8.Visible = !button8.Visible;

            // Показываем или скрываем чекбоксы в зависимости от состояния button8
            checkBox6.Visible = !button8.Visible;
            checkBox7.Visible = !button8.Visible;
            checkBox8.Visible = !button8.Visible;
            checkBox9.Visible = !button8.Visible;
        }

        private void button25_Click(object sender, EventArgs e)     //кнапка подтверждения
        {
            switch (groupcloud)
            {
                case 1:
                    //прописать метод для отсутствия облачных сервисов
                    groupBox2.Visible = false;
                    break;
                case 2:
                    //прописать метод для гугл клауд
                    groupBox2.Visible = false;
                    break;
                case 3:
                    //прописать метод для Яндекс диска
                    groupBox2.Visible = false;
                    break;
                case 4:
                    //прописать метод для Ван Драйв
                    groupBox2.Visible = false;
                    break;
            }
            MessageBox.Show("gotovo");
        }

        #endregion
        #endregion

        #region о приложении

        private void button5_Click(object sender, EventArgs e)     //О приложении
        {

            tabControl1.TabPages.Add(tabPage2);
            tabControl1.SelectedTab = tabPage2;
            tabControl1.TabPages.Remove(tabPage1);
        }

        private void button15_Click(object sender, EventArgs e)     //Назад из О приложении
        {
            CloseTabPage(tabPage2);
        }

        #endregion

        #region Другое расположение
        private void button16_Click(object sender, EventArgs e)     //Другое расположение
        {

            tabControl1.TabPages.Add(tabPage3);
            tabControl1.SelectedTab = tabPage3;
            tabControl1.TabPages.Remove(tabPage1);
        }
        private void button26_Click(object sender, EventArgs e)     //Выход из другого расположения
        {
            CloseTabPage(tabPage3);
        }
        #endregion

        int guide = 0;
        private void button21_Click(object sender, EventArgs e)
        {
            if (guide == 0)
            {
                MessageBox.Show("В следующем окне вам нужно вписать название облака, как оно прописано в пути к синхронизируующемся библиотекам. При подтверждении изменения приложение перезапустится", "Подсказка");
                Form2 f2 = new Form2();
                f2.ShowDialog();
            }
        }

        #endregion

        //Локализация
        #region ==LOCALISATION==

        //int loc;

        private void button6_Click(object sender, EventArgs e)     //Смена языка  ::   Локализация
        {
            MessageBox.Show("Смена языка не доступна из-за санкций. Обратитесь к разработчику", "Ошибка 993", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void button12_Click(object sender, EventArgs e)     //Кнопка английского языка
        {
            MessageBox.Show("Смена языка не доступна из-за санкций. Обратитесь к разработчику", "Ошибка 993", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void button13_Click(object sender, EventArgs e)     //Кнопка русского языка
        {
            Lang = "1";


            using (StreamWriter writer = new StreamWriter(Langt))
            {
                writer.Write(Lang);
            }
            button1.Enabled = true;
            button2.Enabled = true;
            button3.Enabled = true;
            button4.Enabled = true;
            button9.Enabled = true;
            button1.Text = "Выбрать папку";
            button2.Text = "Выполнить";
            button9.Text = "Выход";
            tabControl1.TabPages.Remove(tabPage5);
            button4.Text = "Показать панель";
            set = 0;
            this.Size = new Size(336, 195);
            tabPage1.Visible = false;
            Refresh();
        }

        #endregion

        //Режим смены темы
        #region == DARK MODE ==

        // Список всех элементов для изменения цветов
        private void ApplyTheme(Color backgroundColor, Color controlColor, Color textColor, Color specialColor)
        {
            // Общий фон
            this.BackColor = dev == 1 ? specialColor : backgroundColor;

            // Прямое обновление каждой вкладки
            tabPage1.BackColor = backgroundColor;
            tabPage2.BackColor = backgroundColor;
            tabPage3.BackColor = backgroundColor;
            tabPage4.BackColor = dev == 1 ? specialColor : backgroundColor; // Специальный цвет для Debug Mode
            tabPage5.BackColor = backgroundColor;
            tabPage6.BackColor = backgroundColor;

            tabPage1.ForeColor = textColor;
            tabPage2.ForeColor = textColor;
            tabPage3.ForeColor = textColor;
            tabPage4.ForeColor = textColor;
            tabPage5.ForeColor = textColor;
            tabPage6.ForeColor = textColor;

            // Buttons
            foreach (System.Windows.Forms.Button button in new[]
            {
                button1, button2, button3, button4, button5, button6, button7, button8,
                button9, button10, button11, button12, button13, button14, button15, button16,
                button17, button18, button19, button20, button21, button22, button23, button24,
                button25, button26, button27, button28, button29, button30, button31, button32, button33 })
            {
                if (button == button1) button.BackColor = Color.Khaki;
                else if (button == button2) button.BackColor = b2 == 0 ? Color.PeachPuff : Color.LightGreen;
                else if (button == button4) button.BackColor = Color.Turquoise;
                else if (button == button7) button.BackColor = dm == -1 ? Color.PeachPuff : Color.LightGreen;
                //else if (button == button8) { button.BackColor = Color.Gray; button8.ForeColor = Color.Gray; }
                else if (button == button9) button.BackColor = Color.Coral;
                else if (button == button33) button.BackColor = Color.Coral;
                else button.BackColor = dm == -1 ? Color.LightCyan : Color.LightBlue;

                button.ForeColor = SystemColors.ControlText;
            }

            // CheckBoxes
            foreach (CheckBox checkBox in new[] { checkBox1, checkBox2, checkBox3, checkBox4, checkBox5, checkBox6, checkBox7, checkBox8, checkBox9 })
            {
                checkBox.ForeColor = textColor;
            }

            // Labels
            foreach (Label label in new[] { label1, label2, label3, label5, label6, label7, label8, label9 })
            {
                label.ForeColor = textColor;
            }

            // TextBoxes
            textBox1.BackColor = backgroundColor;
            textBox1.ForeColor = textColor;
            textBox2.BackColor = backgroundColor;
            textBox2.ForeColor = textColor;

            Refresh(); // Принудительно обновляем интерфейс
        }




        // Темная тема
        public void Dark()
        {
            dm = 1;
            SaveThemePreference(dm); // Сохраняем выбор темы
            ApplyTheme(ColorTranslator.FromHtml("#252525"), Color.LightBlue, SystemColors.Control, Color.OrangeRed);
        }

        // Светлая тема
        public void Light()
        {
            dm = -1;
            SaveThemePreference(dm); // Сохраняем выбор темы
            ApplyTheme(SystemColors.ControlLight, Color.LightBlue, SystemColors.ControlText, Color.Orange);
        }

        // Метод сохранения состояния темы
        private void SaveThemePreference(int theme)
        {
            try
            {
                File.WriteAllText(darkp, theme.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения темы: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Переключение темы через кнопку
        private void button7_Click(object sender, EventArgs e)
        {
            if (dm == -1)
            {
                Dark();
                tabControl1.Parent.Refresh();
                Refresh();
            }
            else
            {
                Light();
                tabControl1.Parent.Refresh();
                Refresh();
            }
        }

        #endregion


        #region DEV MODE
        string[] combine_testing_dev;

        private bool IsDebugModeActive()
        {
            return tabControl1.TabPages.Contains(tabPage4);
        }


        private void EnableDebugMode()
        {
            // Проверка на повторное включение
            if (tabControl1.TabPages.Contains(tabPage4))
            {
                MessageBox.Show("Режим разработчика уже активирован!", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Проверка, включён ли Debug Mode
            if (dev == 1)
            {
                MessageBox.Show("Режим разработчика активирован", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Добавляем вкладку Debug Mode
                if (!tabControl1.TabPages.Contains(tabPage4))
                {
                    tabControl1.TabPages.Add(tabPage4);
                }

                currentTabPage = tabPage4; // Устанавливаем Debug Mode как текущую вкладку
                tabControl1.SelectedTab = tabPage4;

                // Применяем тему для режима разработчика
                if (dm == -1)
                {
                    BackColor = Color.Orange;
                }
                else
                {
                    BackColor = Color.OrangeRed;
                }

                d1 = 8; // Блокируем повторное включение через счётчик
                Refresh();
            }
            else
            {
                // Если Debug Mode ещё не активирован, активируем его
                if (d1 < 5)
                {
                    d1++;
                }
                else
                {
                    dev = 1;
                    EnableDebugMode();
                }
            }
        }

        private void DisableDebugMode()
        {
            if (tabControl1.TabPages.Contains(tabPage4))
            {
                tabControl1.TabPages.Remove(tabPage4); // Убираем вкладку Debug Mode
            }

            dev = 0; // Отключаем режим разработчика
            d1 = 5; // Сбрасываем счётчик

            // Возвращаем стандартную тему
            if (dm == -1)
            {
                Light();
            }
            else
            {
                Dark();
            }

            // Возвращаемся к основной вкладке
            if (currentTabPage == tabPage4) // Если текущая вкладка была Debug Mode
            {
                currentTabPage = tabPage1; // Переключаемся на основную вкладку
            }

            Refresh();
        }


        private void button11_Click(object sender, EventArgs e)     //ТЕСТИНГ
        {

        }

        private void button10_Click(object sender, EventArgs e)     //Сбросить значения
        {
            if (DeleteData())
            {
                File.Delete(Langt);
                File.Delete(Cloudpath);
                File.Delete(darkp);
                File.Delete(CCloud);
                MessageBox.Show("Готово");
                Application.Restart();
            }
            else
            {
                MessageBox.Show("Отменено");
            }
        }

        int d1 = 0;
        private void label1_Click(object sender, EventArgs e)     //РЕЖИМ РАЗРАБОТЧИКА
        {
            //EnableDebugMode();
        }

        private void button14_Click(object sender, EventArgs e)     //Закрыть Dev мод
        {
            DisableDebugMode();
        }

        #endregion


        public void TTV()
        {
            toolTip.InitialDelay = 500;
            toolTip.UseFading = true;
            toolTip.UseAnimation = true;
        }
    }
}
