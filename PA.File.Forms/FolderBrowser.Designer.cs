namespace PA.File
{
    partial class FolderBrowser
    {
        /// <summary> 
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur de composants

        /// <summary> 
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas 
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.folder = new System.Windows.Forms.TextBox();
            this.browse = new System.Windows.Forms.Button();
            this.dialog = new System.Windows.Forms.FolderBrowserDialog();
            this.SuspendLayout();
            // 
            // folder
            // 
            this.folder.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.folder.Location = new System.Drawing.Point(0, 2);
            this.folder.Name = "folder";
            this.folder.Size = new System.Drawing.Size(164, 20);
            this.folder.TabIndex = 0;
            this.folder.Validating += new System.ComponentModel.CancelEventHandler(this.folder_Validating);
            this.folder.Validated += new System.EventHandler(this.folder_Validated);
            // 
            // browse
            // 
            this.browse.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.browse.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.browse.Location = new System.Drawing.Point(170, 0);
            this.browse.Name = "browse";
            this.browse.Size = new System.Drawing.Size(75, 23);
            this.browse.TabIndex = 1;
            this.browse.Text = "Browse";
            this.browse.UseVisualStyleBackColor = false;
            this.browse.Click += new System.EventHandler(this.browse_Click);
            // 
            // FolderBrowser
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.browse);
            this.Controls.Add(this.folder);
            this.MinimumSize = new System.Drawing.Size(0, 23);
            this.Name = "FolderBrowser";
            this.Size = new System.Drawing.Size(245, 23);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.TextBox folder;
        private System.Windows.Forms.FolderBrowserDialog dialog;
        private System.Windows.Forms.Button browse;

    }
}
