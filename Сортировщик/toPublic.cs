using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Сортировщик
{
    public class toPublic
    {
        private Label label;
        private Buttons button;
        private ProgressBar progressBar;
        private CheckBox checkBox;

        public toPublic(Label label, Buttons button, ProgressBar progressBar, CheckBox checkBox)
        {
            this.label = label;
            this.button = button;
            this.progressBar = progressBar;
            this.checkBox = checkBox;
        }

        public string labelText
        {
            get { return label.Text; }
            set { label.Text = value; }
        }

        public void UpdateProgress(int progress)
        {
            progressBar.Value = progress;
        }
    }
}
