using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;

namespace Сортировщик
{
    public partial class Sorter_Main : Form
    {
        private FileManager fileManager;
        private Buttons buttons;
        public Sorter_Main()
        {
            InitializeComponent();
            fileManager = new FileManager();
            buttons = new Buttons(fileManager, label1, checkBox1, checkBox2, checkBox3, checkBox4, checkBox5);
        }

        public void INFORMATION()
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            var status = "Пересборка";
            var LastUp = "04.12.2024";
            this.Text = $"Сортировщик файлов   {version}";
            //label3.Text = $"Статус: {status}";
            //label4.Text = $"Последние изменения: {LastUp}";
        }

        private void Sorter_Main_Load(object sender, EventArgs e)
        {
            INFORMATION();
            this.Size = new System.Drawing.Size(500, 290);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            buttons.HandleSelectFolderButtonClick();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            buttons.HandleMoveFilesButtonClick();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Size = new System.Drawing.Size(647, 290);
            groupBox1.Visible = true;
        }

    }
}
