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

namespace Сортировщик
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)  //Загрузка формы
        {
            b2 = 1;
            tabset = 0;
            #region ПОДСКАЗКИ
            checkBox5.Checked = true;
            checkBox5.Checked = false;
            #endregion

            if(!Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\ZeN\Sorter")) //Проверка присутствия директории
            {
                Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\ZeN\Sorter");
            }

            this.Size = new System.Drawing.Size(336, 195);     //Задать размер формы
            tabControl1.TabPages.Remove(tabPage1);
            tabControl1.TabPages.Remove(tabPage2);
            tabControl1.TabPages.Remove(tabPage3);
            tabControl1.TabPages.Remove(tabPage4);
            tabControl1.TabPages.Remove(tabPage5);
            tabControl1.TabPages.Remove(tabPage6);
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
                button2.Text = "";
                button4.Text = "";
                button9.Text = "";
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

            if(button21.Text == "Облако*")
            {
                label7.Text = "тест пути: " + Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\Изображения";
            }
            else
            {
                label7.Text = "тест пути: " + Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\" + label4.Text + @"\Изображения";
            }

            textBox1.Text = "По умолчанию: " + folderPath;

            progressBar1.Visible = false;
            label9.Text = tabset.ToString();
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

        //рописать алгоритм проверки обновлений на выделенном сервере

        private void button3_Click(object sender, EventArgs e)     //Проверка обновления
        {
            MessageBox.Show("Скоро станет доступно");

            // Проверка веток репозитория и взаимодействие с ними
        }

        #endregion

        //Диалоги
        #region == DIALOGS ==

        bool ConfirmDialogRU() //Диалог выхода
        {
            DialogResult confirm = MessageBox.Show("Во избежание случайного закрытия программы пожалуйста подтвердите выход из приложения.", "Вы уверены?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
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
            if(confirm == DialogResult.Yes)
            {
                return true;
            }
            else
            {
                return false ;
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


        string[] photo_format = { ".png", ".gif", ".jpg" };
        #endregion

        #region Standart Methods

        private void MovePic_Standart()
        {

        }

        #endregion
        
        #region buttons

        private void button1_Click(object sender, EventArgs e) //Выбор папки
        {
            using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
            {
                if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    // Получаем путь к выбранной папке
                    folderPath = folderBrowserDialog.SelectedPath;
                    // Отображаем выбранный путь в TextBox
                    textBox1.Text ="Выбрано: " + folderPath;
                    //b2 = 1;
                    button2.BackColor = Color.LightGreen;
                    Refresh();
                }
            }
        }

        int p;
        private void button2_Click(object sender, EventArgs e)   //Поиск файйлов и перенос
        {
            int abort = 0;
            if (b2 == 0)
            {
                MessageBox.Show("Выберите папку", "Внимание");
            }
            else
            {
                if (b2 == 1)
                {
                    progressBar1.Visible = true;
                    //Формат файлов
                    
                    //Поиск картинок в папке или везде
                    string[] matching_Photo = Directory.GetFiles(folderPath, "*.*", checkBox5.Checked ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly)
                    .Where(file => photo_format.Any(x => file.EndsWith(x, StringComparison.OrdinalIgnoreCase)))
                    .ToArray();
                    //Вывод списка картинрок
                    if(matching_Photo.Length > 0)
                    {
                        string message = $"Найдены файлы в {(checkBox5.Checked ? "указанной папке и её подпапках. Показать их?" : "указанной папке. Показать их")}\n";
                        DialogResult result1 = MessageBox.Show(message, "Я кое-что нашёл!", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result1 == DialogResult.Yes)
                        {
                            FileBrowser F3 = new FileBrowser(matching_Photo);
                            F3.ShowDialog();
                        }
                        

                        DialogResult result2 = MessageBox.Show("Выполнить перенос?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result2 == DialogResult.Yes)
                        {
                            //Прописать скрипт переноса картинок
                        }
                        else
                        {
                            MessageBox.Show("Прервано пользователем", "Отмена", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            abort = 1;
                        }
                    }
                    else
                    {
                        MessageBox.Show("К сожалению, файлы не были обнаружены =(" +
                            $"\nПопробуйте {(checkBox5.Checked ? "выбрать другую папку" : "поискать во всей папке, выбрав пункт (Искать везде)")}");
                        abort = 1;
                    }


                    if(abort == 0)
                    {
                        while (progressBar1.Value < 100)
                        {
                            if (progressBar1.Value >= 99)
                            {
                                progressBar1.Value = 100;
                                MessageBox.Show("Проверка завершена");
                                progressBar1.Visible = false;
                            }
                            else
                            {
                                progressBar1.Value++;
                            }
                        }
                        progressBar1.Value = 0;
                    }
                    else
                    {
                        progressBar1.Visible = false;
                        progressBar1.Value = 0;
                    }
                }
            }
        }

        #endregion

        #endregion                                 

        //Настройки
        #region == SETTINGS ==

        int set = 0;
        

        private void button4_Click(object sender, EventArgs e)   //Показать или убрать панель НАСТРОЙКИ
        {
            if(set == 0)
            {
                button4.Text = "↑Убрать панель↑";
                set = 1;
                this.Size = new Size(336, 480);
                tabPage1.Visible = true;
                Refresh();
                if(tabset == 0)
                {
                    tabControl1.TabPages.Add(tabPage1);
                }
                //tabControl1.TabPages.Remove(tabPage2);
                //tabControl1.TabPages.Remove(tabPage3);
                //tabControl1.TabPages.Remove(tabPage4);
                //tabControl1.TabPages.Remove(tabPage5);
                //tabControl1.TabPages.Remove(tabPage6); 
            }
            else
            {
                if(set == 1)
                {
                    button4.Text = "↓Показать панель↓";
                    set = 0;
                    this.Size = new Size(336, 195);
                    tabPage1.Visible = false;
                    Refresh();
                    tabControl1.TabPages.Remove(tabPage1);
                }
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
            tabset = 1;
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
            tabset = 0;
            tabControl1.TabPages.Remove(tabPage6);
            tabControl1.TabPages.Add(tabPage1);
            tabControl1.SelectedTab = tabPage1;
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
            tabset = 1;
            tabControl1.TabPages.Add(tabPage2);
            tabControl1.SelectedTab = tabPage2;
            tabControl1.TabPages.Remove(tabPage1);
        }

        private void button15_Click(object sender, EventArgs e)     //Назад из О приложении
        {
            tabset = 0;
            tabControl1.TabPages.Add(tabPage1);
            tabControl1.SelectedTab = tabPage1;
            tabControl1.TabPages.Remove(tabPage2);
        }

        #endregion

        #region Другое расположение
        private void button16_Click(object sender, EventArgs e)     //Другое расположение
        {
            tabset = 1;
            tabControl1.TabPages.Add(tabPage3);
            tabControl1.SelectedTab = tabPage3;
            tabControl1.TabPages.Remove(tabPage1);
        }
        private void button26_Click(object sender, EventArgs e)     //Выход из другого расположения
        {
            tabset = 0;
            tabControl1.TabPages.Add(tabPage1);
            tabControl1.SelectedTab = tabPage1;
            tabControl1.TabPages.Remove(tabPage3);
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

        private void button13_Click(object sender, EventArgs e)
        {
            Lang = "1";
            

            using (StreamWriter writer = new StreamWriter(Langt))
            {
                writer.Write(Lang);
            }
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
            if(dev == 1)
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
                if(b2 == 1)
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
            //label5.ForeColor = SystemColors.Control;
            //label6.ForeColor = SystemColors.Control;
            //label7.ForeColor = SystemColors.Control;
            //label8.ForeColor = SystemColors.Control;
            //label9.ForeColor = SystemColors.Control;

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

            dmo = "-1";
            File.WriteAllText(darkp, dmo);
        }

        private void button7_Click(object sender, EventArgs e)     //Тёмный мод
        {
            if(dm == -1)
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
        private void button11_Click(object sender, EventArgs e)     //ТЕСТИНГ
        {
            combine_testing_dev = new[] { Langt, "\n", darkp };
            FileBrowser f3 = new FileBrowser(combine_testing_dev);
            f3.ShowDialog();
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
            if(d1 <= 5)
            {
                if (dev == 1)
                {
                    MessageBox.Show("Режим разработчика активирован", "ВНИМАНИЕ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    tabControl1.TabPages.Add(tabPage4);
                    if (dm == -1)
                    {
                        BackColor = Color.Orange;
                        tabset = 1;
                        Refresh();
                    }
                    if (dm == 1)
                    {
                        BackColor = Color.OrangeRed;
                        Refresh();
                    }
                    d1 = 8;
                    tabset = 1;
                }
                else
                {
                    if (d1 < 5)
                    {
                        d1++;
                        //label1.Text = (label1.Text + " (" + Convert.ToString(d1) + ")");
                    }
                    else
                    {
                        dev = 1;
                    }
                }
            }
            else
            {
                MessageBox.Show("Но вы уже разработчик!", "ВНИМАНИЕ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void button14_Click(object sender, EventArgs e)     //Закрыть Dev мод
        {
            tabControl1.TabPages.Remove(tabPage4);
            dev = 0;
            d1 = 5;
            if (dm == -1)
            {
                Light();
                Refresh();
            }
            else
            {
                Dark();
                Refresh();
            }
        }









        #endregion

        
        public void TTV()
        {
            toolTip.InitialDelay = 500;
            toolTip.UseFading = true;
            toolTip.UseAnimation = true;
        }
        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            
            if (checkBox5.Checked == true)
            {
                
                toolTip.SetToolTip(button2, "Выполняет сортировку файлов в указанной папке и её подпапках");
            }
            else
            {
                
                toolTip.SetToolTip(button2, "Выполняет сортировку файлов внутри указанной папки, не учитывая подпапок");
            }
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

        private void button12_Click(object sender, EventArgs e)
        {
            MessageBox.Show("ttre");
        }
    }
}
