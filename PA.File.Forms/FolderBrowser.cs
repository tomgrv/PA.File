using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace PA.File
{
    public partial class FolderBrowser : UserControl
    {
        public string SelectedPath
        {
            get
            {
                return this.dialog.SelectedPath;
            }
        }

        public FolderBrowser()
        {
            InitializeComponent();
        }

        public event EventHandler FolderSelected;

        protected virtual void OnFolderSelected(EventArgs e)
        {
            this.folder.ForeColor = System.Drawing.SystemColors.WindowText;

            if (this.FolderSelected != null)
            {
                this.FolderSelected(this, e);
            }
        }

        private void browse_Click(object sender, EventArgs e)
        {
            this.OnClick(e);
            if (this.dialog.ShowDialog() == DialogResult.OK)
            {
                this.folder.Text = this.dialog.SelectedPath;
                this.OnFolderSelected(e);
            }
        }

        private void folder_Validated(object sender, EventArgs e)
        {
            this.OnValidated(e);
            this.dialog.SelectedPath = this.folder.Text;
            this.OnFolderSelected(e);
        }

        private void folder_Validating(object sender, CancelEventArgs e)
        {
            this.OnValidating(e);
            if (!e.Cancel && !Directory.Exists(this.folder.Text))
            {
                e.Cancel = true;
                this.folder.ForeColor = Color.Red;
            }
        }
    }
}