using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using System.Net.Http;
using System.Threading;

namespace Сортировщик
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public void INFORMATION()  //Информация о приложении
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            var status = "Реализация переноса";
            var LastUp = "21.11.2024";
            var AppName = "Alpha";
            this.Text = $"Сортировщик ({version})  --  {AppName}";
            label1.Text = $"Версия: {version}";
            label2.Text = $"Статус: {status}";
            label3.Text = $"Последние изменения: {LastUp}";
        }

        private void Form1_Load(object sender, EventArgs e)  //Загрузка формы
        {
            INFORMATION();

            b2 = 1;
            tabset = 0;
            #region ПОДСКАЗКИ
            checkBox5.Checked = false;
            #endregion

            string appDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "ZeN", "Sorter");     // Проверяем, существует ли директория приложения
            if (!Directory.Exists(appDirectory))
            {
                Directory.CreateDirectory(appDirectory);
            }

            this.Size = new System.Drawing.Size(336, 195);     //Задать размер формы
            tabControl1.TabPages.Clear();
            groupBox2.Visible = false;     //Скрыть синхронизацию папок.

            if (File.Exists(Langt))     //Проверка языка
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
                Refresh();
                tabControl1.TabPages.Add(tabPage5);
            }

            if (File.Exists(darkp))     //Проверка темы
            {
                dmo = File.ReadAllText(darkp);
                dm = Convert.ToInt32(dmo);
                label6.Text = "Проверка тёмного режима:   " + dmo;
                if (dm == 1)
                {
                    Dark();
                    Refresh();
                }
                else
                {
                    if (dm == -1)
                    {
                        Light();
                    }
                }
            }
            else
            {
                Light();
            }

            if (File.Exists(CCloud))     //Проверка облака, которого нет в списке
            {
                label4.Text = File.ReadAllText(CCloud);
                button21.Text = label4.Text;
            }

            if (File.Exists(b2s))     //Проверка триггера для переноса
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

            if (button21.Text == "Облако*")
            {
                label7.Text = "тест пути: " + Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\Изображения";
            }
            else
            {
                label7.Text = "тест пути: " + Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\" + label4.Text + @"\Изображения";
            }

            // Отображаем путь по умолчанию
            textBox1.Text = "По умолчанию: " + folderPath;

            progressBar1.Visible = false;
            label9.Text = "Значение открытой панели" + tabset.ToString();
        }

        #region ГЛОБАЛЬНЫЕ ПЕРЕМЕННЫЕ

        int tabset = 0;     //Переменная настроек (скрыть/показать)
        ToolTip toolTip = new ToolTip();

        int gdrive;
        int onedrive;
        int yadrive;

        int dev = 0;     // Режим разработчика
        int dm = -1;     // Тёмный режим
        int L;           // Локализация
        string CCloud = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\ZeN\Sorter\customcloud.txt";     //Облако отсутствует в списке

        string b2s = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\ZeN\Sorter\b2s.txt";     //Триггер переноса файлов.txt
        int b2 = 0;     //Проверка триггера для переноса
        int groupcloud = 0;     //Переменнвая вызова папок синхронезации и подтверждения облака

        #endregion

        //Обновление приложения
        #region U P D A T E S

        //Прописать алгоритм проверки обновлений на выделенном сервере

        public Version GetCurrentVersion()
        {
            return Assembly.GetExecutingAssembly().GetName().Version;
        }


        public async Task<string> GetLatestVersionFromGitHubAsync()
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string url = "https://raw.githubusercontent.com/NecroMagik/Sorter-1.0/tree/Release/releases/version.txt";
                    string response = await client.GetStringAsync(url);
                    return response.Trim(); // Убираем возможные пробелы или переносы строк
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при проверке обновлений: {ex.Message}");
                    return null;
                }
            }
        }

        public async Task CheckForUpdatesAsync()
        {
            Version currentVersion = GetCurrentVersion();  // Получаем текущую версию
            string latestVersionStr = await GetLatestVersionFromGitHubAsync();  // Загружаем последнюю версию с GitHub

            if (!string.IsNullOrEmpty(latestVersionStr) && Version.TryParse(latestVersionStr, out Version latestVersion))
            {
                if (latestVersion > currentVersion)  // Если версия на GitHub новее
                {
                    DialogResult result = MessageBox.Show($"Доступна новая версия: {latestVersion}. Хотите обновить?", "Обновление", MessageBoxButtons.YesNo);

                    if (result == DialogResult.Yes)
                    {
                        //wait DownloadUpdateAsync(latestVersion.ToString());  // Метод для загрузки обновления
                    }
                }
                else
                {
                    MessageBox.Show("У вас установлена последняя версия.");
                }
            }
            else
            {
                MessageBox.Show("Не удалось получить версию с GitHub.", "Ошибка 404", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button3_Click(object sender, EventArgs e)     //Проверка обновления
        {
            GetLatestVersionFromGitHubAsync();
            CheckForUpdatesAsync();

            // Проверка веток репозитория и взаимодействие с ними
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

        #region --path--

        string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\Downloads";     //Берётся значение из проводника
        string doc = (Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));    //Документы
        string mus = (Environment.GetFolderPath(Environment.SpecialFolder.MyMusic));        //Музыка
        string pic = (Environment.GetFolderPath(Environment.SpecialFolder.MyPictures));     //Картинки
        string vid = (Environment.GetFolderPath(Environment.SpecialFolder.MyVideos));       //Видосы

        string Langt = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\ZeN\Sorter\lang.txt";
        string Lang;
        string dmo;
        string Cloudpath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\ZeN\Sorter\cloudpath.txt";
        string cloudR = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\ZeN\Sorter\cloudt.txt";
        string darkp = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\ZeN\Sorter\dark.txt";

        #endregion

        #region Standart Methods

        private void InitializeProgressBar(int maxValue)
        {
            progressBar1.Visible = true; // Делаем прогресс-бар видимым
            progressBar1.Value = 0; // Сбрасываем прогресс
            progressBar1.Maximum = maxValue; // Устанавливаем максимальное значение
        }

        int abort = 0;

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

            // Все форматы
            var allFormats = new[]
            {
        "*.jpg", "*.jpeg", "*.png", "*.bmp", "*.gif",
        "*.mp3", "*.wav", "*.flac", "*.aac", "*.ogg",
        "*.mp4", "*.avi", "*.mkv", "*.mov",
        "*.doc", "*.docx", "*.pdf", "*.xls", "*.xlsx", "*.txt"
    };

            InitializeProgressBar(allFormats.Length); // Инициализация прогресс-бара

            int processedFormats = 0;

            foreach (var ext in allFormats)
            {
                try
                {
                    if (Array.Exists(new[] { "*.jpg", "*.jpeg", "*.png", "*.bmp", "*.gif" }, e => e == ext))
                        categories["Фотографии"].AddRange(SafeGetFiles(path, ext, searchOption));
                    if (Array.Exists(new[] { "*.mp3", "*.wav", "*.flac", "*.aac", "*.ogg" }, e => e == ext))
                        categories["Музыка"].AddRange(SafeGetFiles(path, ext, searchOption));
                    if (Array.Exists(new[] { "*.mp4", "*.avi", "*.mkv", "*.mov" }, e => e == ext))
                        categories["Видео"].AddRange(SafeGetFiles(path, ext, searchOption));
                    if (Array.Exists(new[] { "*.doc", "*.docx", "*.pdf", "*.xls", "*.xlsx", "*.txt" }, e => e == ext))
                        categories["Документы"].AddRange(SafeGetFiles(path, ext, searchOption));
                }
                catch (Exception ex)
                {
                    // Логируем ошибку, если требуется
                    Console.WriteLine($"Ошибка при поиске файлов с расширением {ext}: {ex.Message}");
                }

                processedFormats++;
                progressBar1.Value = processedFormats;
                await Task.Delay(10); // Эмуляция задержки для плавного обновления UI
            }

            return categories;
        }


        private IEnumerable<string> SafeGetFiles(string path, string searchPattern, SearchOption searchOption)
        {
            try
            {
                return Directory.GetFiles(path, searchPattern, searchOption); // Попытка получить файлы
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine($"Нет доступа к папке: {path}"); // Логирование
                return Enumerable.Empty<string>(); // Возвращаем пустой список
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

        /// <summary>
        /// Перемещает файлы в системные папки в зависимости от их категории.
        /// </summary>
        /// <param name="filesByCategory">Словарь с категориями и файлами.</param>
        private async Task TransferFilesAsync(Dictionary<string, List<string>> filesByCategory)
        {
            // Определение целевых папок
            string picturesPath = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            string musicPath = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
            string videosPath = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos);
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            var destinationPaths = new Dictionary<string, string>
    {
        { "Фотографии", picturesPath },
        { "Музыка", musicPath },
        { "Видео", videosPath },
        { "Документы", documentsPath }
    };

            // Подсчитываем общее количество файлов
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
                        if (!File.Exists(destination)) // Проверяем, существует ли файл в целевой папке
                        {
                            File.Move(file, destination);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при перемещении файла {file}: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    processedFiles++;
                    progressBar1.Value = processedFiles; // Обновляем прогресс
                    await Task.Delay(10); // Эмуляция задержки для плавного обновления UI
                }
            }

            MessageBox.Show("Файлы успешно перемещены!", "Готово", MessageBoxButtons.OK, MessageBoxIcon.Information);
            progressBar1.Visible = false; // Скрываем прогресс-бар после завершения
        }


        #endregion

        #region buttons

        private void button1_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
            {
                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    folderPath = folderDialog.SelectedPath; // Сохраняем путь к выбранной папке
                    textBox1.Text = "Выбрано: " + folderPath; // Отображаем путь
                    button2.BackColor = Color.LightGreen; // Подсветка кнопки "Выполнить перенос"
                }
            }
        }

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


        int p;
        private async void button2_Click(object sender, EventArgs e)
        {
            // Шаг 1. Проверяем, выбрана ли папка
            if (string.IsNullOrEmpty(folderPath))
            {
                MessageBox.Show("Выберите папку перед началом выполнения!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                button1.BackColor = Color.LightCoral; // Подсветка кнопки "Выбор папки"
                return;
            }

            // Шаг 2. Показываем пользователю, что идёт поиск файлов
            progressBar1.Visible = true;
            progressBar1.Value = 0;
            textBox1.Text = "Идёт поиск файлов...";

            // Шаг 3. Асинхронный поиск файлов
            var filesByCategory = await SearchFilesAsync(folderPath, checkBox5.Checked);

            // Шаг 4. Проверяем, найдены ли файлы
            if (!filesByCategory.Values.Any(list => list.Count > 0))
            {
                progressBar1.Visible = false; // Скрываем прогресс-бар

                // Если файлы не найдены, спрашиваем пользователя о дополнительных действиях
                if (!checkBox5.Checked)
                {
                    if (MessageBox.Show("Файлы не найдены. Искать в подпапках?", "Внимание", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        checkBox5.Checked = true; // Активируем поиск в подпапках
                        return; // Повторяем процесс
                    }
                }
                MessageBox.Show("Файлы не найдены. Выберите другую папку.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Шаг 5. Скрываем прогресс-бар, если поиск завершён успешно
            progressBar1.Visible = false;

            // Шаг 6. Подсчитываем количество файлов каждой категории
            var summary = new StringBuilder();
            summary.AppendLine("Найдены следующие файлы:");
            foreach (var category in filesByCategory)
            {
                summary.AppendLine($"{category.Key}: {category.Value.Count} файлов");
            }

            // Показываем уведомление с количеством файлов
            var result = MessageBox.Show($"{summary}\n\nХотите просмотреть файлы перед переносом?",
                                          "Результаты поиска",
                                          MessageBoxButtons.YesNo,
                                          MessageBoxIcon.Question);

            // Если пользователь выбрал "Да", открываем FileBrowser
            if (result == DialogResult.Yes)
            {
                ShowFileListInNewWindow(filesByCategory);
            }
            else
            {
                // Если пользователь отказался от просмотра, подтверждаем перенос
                var confirmTransfer = MessageBox.Show("Вы хотите сразу начать перенос файлов?",
                                                      "Подтверждение переноса",
                                                      MessageBoxButtons.YesNo,
                                                      MessageBoxIcon.Question);
                if (confirmTransfer == DialogResult.Yes)
                {
                    await TransferFilesAsync(filesByCategory); // Асинхронный перенос файлов
                    MessageBox.Show("Файлы успешно перемещены!", "Готово", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }






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

        public void Dark()
        {
            dm = 1;
            if (dev == 1)
            {
                BackColor = Color.OrangeRed;
            }
            else
            {
                this.BackColor = ColorTranslator.FromHtml("#252525");
            }
            tabControl1.BackColor = ColorTranslator.FromHtml("#252525");
            tabPage1.BackColor = ColorTranslator.FromHtml("#252525");     //Настройки
            tabPage2.BackColor = ColorTranslator.FromHtml("#252525");     //О приложении
            tabPage3.BackColor = ColorTranslator.FromHtml("#252525");     //Другое расположение
            tabPage5.BackColor = ColorTranslator.FromHtml("#252525");     //Выбор языка
            tabPage6.BackColor = ColorTranslator.FromHtml("#252525");     //Облачные сервисы
            tabPage4.BackColor = Color.OrangeRed;                         //Debug mode
            button7.BackColor = Color.Lime;
            button7.BackColor = Color.LightGreen;
            button1.BackColor = Color.Khaki;
            textBox1.BackColor = ColorTranslator.FromHtml("#252525");
            textBox1.ForeColor = SystemColors.Control;
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
            button3.BackColor = Color.LightBlue;
            button4.BackColor = Color.LightBlue;
            button5.BackColor = Color.LightBlue;
            button6.BackColor = Color.LightBlue;
            button8.BackColor = Color.LightBlue;
            button9.BackColor = Color.Coral;

            checkBox1.ForeColor = SystemColors.Control;
            checkBox2.ForeColor = SystemColors.Control;
            checkBox3.ForeColor = SystemColors.Control;
            checkBox4.ForeColor = SystemColors.Control;
            checkBox5.ForeColor = SystemColors.Control;

            dmo = "1";
            File.WriteAllText(darkp, dmo);

            label1.ForeColor = SystemColors.Control;
            label2.ForeColor = SystemColors.Control;
            label3.ForeColor = SystemColors.Control;
            label5.ForeColor = SystemColors.Control;
            label6.ForeColor = SystemColors.Control;
            label7.ForeColor = SystemColors.Control;
            label8.ForeColor = SystemColors.Control;
            label9.ForeColor = SystemColors.Control;

        }

        public void Light()
        {
            dm = -1;
            if (dev == 1)
            {
                BackColor = Color.Orange;
            }
            else
            {
                this.BackColor = SystemColors.ControlLight;
            }
            tabPage1.BackColor = SystemColors.ControlLight;
            tabPage2.BackColor = SystemColors.ControlLight;
            tabPage3.BackColor = SystemColors.ControlLight;
            tabPage5.BackColor = SystemColors.ControlLight;
            tabPage6.BackColor = SystemColors.ControlLight;
            tabPage4.BackColor = Color.Orange;
            button7.BackColor = Color.PeachPuff;
            button1.BackColor = Color.Khaki;
            textBox1.BackColor = SystemColors.Control;
            textBox1.ForeColor = SystemColors.ControlText;
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
            button3.BackColor = Color.LightBlue;
            button4.BackColor = Color.LightBlue;
            button5.BackColor = Color.LightBlue;
            button6.BackColor = Color.LightBlue;
            button7.BackColor = Color.PeachPuff;
            button8.BackColor = Color.LightBlue;
            button9.BackColor = Color.Coral;

            checkBox1.ForeColor = SystemColors.ControlText;
            checkBox2.ForeColor = SystemColors.ControlText;
            checkBox3.ForeColor = SystemColors.ControlText;
            checkBox4.ForeColor = SystemColors.ControlText;
            checkBox5.ForeColor = SystemColors.ControlText;

            label3.ForeColor = SystemColors.ControlText;
            label5.ForeColor = SystemColors.ControlText;
            label6.ForeColor = SystemColors.ControlText;
            label7.ForeColor = SystemColors.ControlText;
            label8.ForeColor = SystemColors.ControlText;
            label9.ForeColor = SystemColors.ControlText;
            label1.ForeColor = SystemColors.ControlText;
            label2.ForeColor = SystemColors.ControlText;

            dmo = "-1";
            File.WriteAllText(darkp, dmo);
        }

        private void button7_Click(object sender, EventArgs e)     //Тёмный мод
        {
            if (dm == -1)
            {
                Dark();
                Refresh();
            }
            else
            {
                Light();
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
            EnableDebugMode();
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


        private void button30_Click(object sender, EventArgs e)
        {

        }

        private void button29_Click(object sender, EventArgs e)
        {

        }

        private void button28_Click(object sender, EventArgs e)
        {

        }

        private void button27_Click(object sender, EventArgs e)
        {

        }


    }
}
