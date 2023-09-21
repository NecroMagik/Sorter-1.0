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
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        string cloud = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\ZeN\Sorter\customcloud.txt";
        //int F;
        //string Fs;
        int dm = -1;     // Тёмный режим
        string dmo;
        string darkp = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\ZeN\Sorter\dark.txt";


        #region == DARK MODE ==

        public void Dark()
        {
            dm = 1;
            
                
                this.BackColor = ColorTranslator.FromHtml("#252525");
            
            
            button1.BackColor = Color.Khaki;
            textBox1.BackColor = ColorTranslator.FromHtml("#252525");
            textBox1.ForeColor = SystemColors.Control;
            

            dmo = "1";
            File.WriteAllText(darkp, dmo);
        }

        public void Light()
        {
            dm = -1;
            this.BackColor = SystemColors.Control;
            button1.BackColor = Color.Khaki;
            textBox1.BackColor = SystemColors.Control;
            textBox1.ForeColor = SystemColors.ControlText;
            

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
        private void Form2_Load(object sender, EventArgs e)
        {
            if (File.Exists(darkp))     //Проверка темы
            {
                dmo = File.ReadAllText(darkp);
                dm = Convert.ToInt32(dmo);
                
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
                        Refresh();
                    }
                }
            }
            else
            {
                Light();
                Refresh();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (StreamWriter writer = new StreamWriter(cloud))
            {
                writer.Write(textBox1.Text);
            }
            Application.Restart();
        }
    }
}
