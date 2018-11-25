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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
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
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Controls.Add(this.lvStatsTable);
            this.panel1.Name = "panel1";
            // 
            // lvStatsTable
            // 
            resources.ApplyResources(this.lvStatsTable, "lvStatsTable");
            this.lvStatsTable.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.lvName,
            this.lvCurDown,
            this.lvCurUp,
            this.lvTotalDown,
            this.lvTotalUp});
            this.lvStatsTable.GridLines = true;
            this.lvStatsTable.Name = "lvStatsTable";
            this.lvStatsTable.Sorting = System.Windows.Forms.SortOrder.Descending;
            this.lvStatsTable.UseCompatibleStateImageBehavior = false;
            this.lvStatsTable.View = System.Windows.Forms.View.Details;
            // 
            // lvName
            // 
            resources.ApplyResources(this.lvName, "lvName");
            // 
            // lvCurDown
            // 
            resources.ApplyResources(this.lvCurDown, "lvCurDown");
            // 
            // lvCurUp
            // 
            resources.ApplyResources(this.lvCurUp, "lvCurUp");
            // 
            // lvTotalDown
            // 
            resources.ApplyResources(this.lvTotalDown, "lvTotalDown");
            // 
            // lvTotalUp
            // 
            resources.ApplyResources(this.lvTotalUp, "lvTotalUp");
            // 
            // FormMain
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel1);
            this.Name = "FormMain";
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