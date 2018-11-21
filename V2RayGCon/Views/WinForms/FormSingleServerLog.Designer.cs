namespace V2RayGCon.Views.WinForms
{
    partial class FormSingleServerLog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormSingleServerLog));
            this.rtBoxLogger = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // rtBoxLogger
            // 
            resources.ApplyResources(this.rtBoxLogger, "rtBoxLogger");
            this.rtBoxLogger.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.rtBoxLogger.Name = "rtBoxLogger";
            this.rtBoxLogger.ReadOnly = true;
            this.rtBoxLogger.TextChanged += new System.EventHandler(this.rtBoxLogger_TextChanged);
            // 
            // FormSingleServerLog
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.rtBoxLogger);
            this.Name = "FormSingleServerLog";
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.RichTextBox rtBoxLogger;
    }
}
