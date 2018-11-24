namespace Statistics.Views.WinForms
{
    partial class FormMain
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.lvStatsTable = new System.Windows.Forms.ListView();
            this.lvName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lvCurDown = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lvCurUp = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lvTotalDown = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lvTotalUp = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.lvStatsTable);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(800, 450);
            this.panel1.TabIndex = 0;
            // 
            // lvStatsTable
            // 
            this.lvStatsTable.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.lvName,
            this.lvCurDown,
            this.lvCurUp,
            this.lvTotalDown,
            this.lvTotalUp});
            this.lvStatsTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvStatsTable.GridLines = true;
            this.lvStatsTable.Location = new System.Drawing.Point(0, 0);
            this.lvStatsTable.Name = "lvStatsTable";
            this.lvStatsTable.Size = new System.Drawing.Size(800, 450);
            this.lvStatsTable.Sorting = System.Windows.Forms.SortOrder.Descending;
            this.lvStatsTable.TabIndex = 0;
            this.lvStatsTable.UseCompatibleStateImageBehavior = false;
            this.lvStatsTable.View = System.Windows.Forms.View.Details;
            // 
            // lvName
            // 
            this.lvName.Text = "Name";
            this.lvName.Width = 106;
            // 
            // lvCurDown
            // 
            this.lvCurDown.Text = "Down (KiB/s)";
            this.lvCurDown.Width = 150;
            // 
            // lvCurUp
            // 
            this.lvCurUp.Text = "Up (KiB/s)";
            this.lvCurUp.Width = 141;
            // 
            // lvTotalDown
            // 
            this.lvTotalDown.Text = "Down (MiB)";
            this.lvTotalDown.Width = 142;
            // 
            // lvTotalUp
            // 
            this.lvTotalUp.Text = "Up (MiB)";
            this.lvTotalUp.Width = 158;
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.panel1);
            this.Name = "FormMain";
            this.Text = "FormMain";
            this.Shown += new System.EventHandler(this.FormMain_Shown);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ListView lvStatsTable;
        private System.Windows.Forms.ColumnHeader lvName;
        private System.Windows.Forms.ColumnHeader lvCurDown;
        private System.Windows.Forms.ColumnHeader lvTotalDown;
        private System.Windows.Forms.ColumnHeader lvCurUp;
        private System.Windows.Forms.ColumnHeader lvTotalUp;
    }
}