using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace PA.File
{
    public partial class FolderBrowser : UserControl
    {
        public FolderBrowser()
        {
            InitializeComponent();
        }

        public string SelectedPath => dialog.SelectedPath;

        public event EventHandler FolderSelected;

        protected virtual void OnFolderSelected(EventArgs e)
        {
            folder.ForeColor = SystemColors.WindowText;

            if (FolderSelected != null) FolderSelected(this, e);
        }

        private void browse_Click(object sender, EventArgs e)
        {
            OnClick(e);
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                folder.Text = dialog.SelectedPath;
                OnFolderSelected(e);
            }
        }

        private void folder_Validated(object sender, EventArgs e)
        {
            OnValidated(e);
            dialog.SelectedPath = folder.Text;
            OnFolderSelected(e);
        }

        private void folder_Validating(object sender, CancelEventArgs e)
        {
            OnValidating(e);
            if (!e.Cancel && !Directory.Exists(folder.Text))
            {
                e.Cancel = true;
                folder.ForeColor = Color.Red;
            }
        }
    }
}