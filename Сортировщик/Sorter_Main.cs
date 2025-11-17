using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using System.IO;

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
            fileManager = new FileManager(button1, label2, label3, label4, label5, checkBox1, guna2ProgressBar1);
            // Связываем FileManager с Buttons
            buttons = new Buttons(
                fileManager,
                label1,            // Label для отображения статуса
                guna2ProgressBar1,           // Прогресс-бар
                checkBox1, // Чекбокс поиска в подпапках
                button3,                // Кнопка 3 (для изменения размеров окна)
                button5,
                radioButton1,
                radioButton2,
                radioButton3,
                checkBox2, checkBox3, checkBox4, checkBox5 // Чекбоксы категорий
            );

            // Привязка кнопок к методам Buttons
            button1.Click += (s, e) => buttons.HandleSelectFolderButtonClick();
            button2.Click += (s, e) => buttons.HandleSearchFilesButtonClick();
            button3.Click += (s, e) => buttons.HandleResizeButtonClick(this, groupBox1, groupBox2);
            button4.Click += (s, e) => buttons.HandleAboutButtonClick(ver, Stat, LasTUPe);
            button5.Click += (s, e) => buttons.HandleResetPathsButtonClick();
            button8.Click += (s, e) => buttons.HandleUpdateButtonClick(guna2ProgressBar1);

            button9.Click += (s, e) =>  buttons.HandleSelectCategoryPath("Фото", label2);
            button10.Click += (s, e) => buttons.HandleSelectCategoryPath("Видео", label3);
            button11.Click += (s, e) => buttons.HandleSelectCategoryPath("Музыка", label4);
            button12.Click += (s, e) => buttons.HandleSelectCategoryPath("Документы", label5);

        }
        string Stat;
        string LasTUPe;
        string ver;
        public void INFORMATION()
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            var status = "Бета-5.5 (Ветка: Rebuild)";
            var LastUp = "14.11.2025";
            this.Text = $"Сортировщик файлов   {version}";
            ver = version.ToString();
            LasTUPe = LastUp;
            Stat = status;
        }
        

        private void Sorter_Main_Load(object sender, EventArgs e)       //Загрузка приложения
        {
            INFORMATION();
            this.Size = new System.Drawing.Size(428, 313);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // Проверяем причину закрытия (чтобы не блокировать завершение процесса, например, при выключении ПК)
            if (e.CloseReason == CloseReason.UserClosing)
            {
                // Показываем сообщение пользователю
                DialogResult result = MessageBox.Show(
                    "Вы уверены, что хотите закрыть приложение?",
                    "Подтверждение закрытия",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                // Если пользователь выбрал "Нет", отменяем закрытие
                if (result == DialogResult.No)
                {
                    e.Cancel = true;
                }
            }

            base.OnFormClosing(e);
        }

        private void button8_Click(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {

        }
    }
}
