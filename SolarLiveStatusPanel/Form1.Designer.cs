using CustomControls.RJControls;

namespace SolarLiveStatusPanel
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            menuStrip1 = new MenuStrip();
            fileToolStripMenuItem = new ToolStripMenuItem();
            refreshTokenToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator1 = new ToolStripSeparator();
            exitToolStripMenuItem = new ToolStripMenuItem();
            tableLayoutPanel1 = new TableLayoutPanel();
            groupBox1 = new GroupBox();
            labelGenerating = new Label();
            groupBox2 = new GroupBox();
            labelLoad = new Label();
            groupBoxExport = new GroupBox();
            labelExportImport = new Label();
            groupBoxHotWater = new GroupBox();
            tableLayoutPanel2 = new TableLayoutPanel();
            labelHotWater = new Label();
            checkBoxHotWater = new RJToggleButton();
            timerUI = new System.Windows.Forms.Timer(components);
            timerGetData = new System.Windows.Forms.Timer(components);
            menuStrip1.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            groupBoxExport.SuspendLayout();
            groupBoxHotWater.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            SuspendLayout();
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(800, 24);
            menuStrip1.TabIndex = 0;
            menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { refreshTokenToolStripMenuItem, toolStripSeparator1, exitToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(37, 20);
            fileToolStripMenuItem.Text = "File";
            // 
            // refreshTokenToolStripMenuItem
            // 
            refreshTokenToolStripMenuItem.Name = "refreshTokenToolStripMenuItem";
            refreshTokenToolStripMenuItem.Size = new Size(148, 22);
            refreshTokenToolStripMenuItem.Text = "Refresh Token";
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(145, 6);
            // 
            // exitToolStripMenuItem
            // 
            exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            exitToolStripMenuItem.Size = new Size(148, 22);
            exitToolStripMenuItem.Text = "Exit";
            exitToolStripMenuItem.Click += exitToolStripMenuItem_Click;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
            tableLayoutPanel1.ColumnCount = 1;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Controls.Add(groupBox1, 0, 0);
            tableLayoutPanel1.Controls.Add(groupBox2, 0, 1);
            tableLayoutPanel1.Controls.Add(groupBoxExport, 0, 2);
            tableLayoutPanel1.Controls.Add(groupBoxHotWater, 0, 3);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 24);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 4;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 33.33408F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 33.332962F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 33.33296F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 71F));
            tableLayoutPanel1.Size = new Size(800, 531);
            tableLayoutPanel1.TabIndex = 3;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(labelGenerating);
            groupBox1.Dock = DockStyle.Fill;
            groupBox1.Location = new Point(4, 4);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(792, 145);
            groupBox1.TabIndex = 4;
            groupBox1.TabStop = false;
            groupBox1.Text = "Generatin from Panels";
            // 
            // labelGenerating
            // 
            labelGenerating.Dock = DockStyle.Fill;
            labelGenerating.Location = new Point(3, 19);
            labelGenerating.Name = "labelGenerating";
            labelGenerating.Size = new Size(786, 123);
            labelGenerating.TabIndex = 3;
            labelGenerating.Text = "999.99 KW";
            labelGenerating.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(labelLoad);
            groupBox2.Dock = DockStyle.Fill;
            groupBox2.Location = new Point(4, 156);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(792, 145);
            groupBox2.TabIndex = 5;
            groupBox2.TabStop = false;
            groupBox2.Text = "Current Load";
            // 
            // labelLoad
            // 
            labelLoad.Dock = DockStyle.Fill;
            labelLoad.Location = new Point(3, 19);
            labelLoad.Name = "labelLoad";
            labelLoad.Size = new Size(786, 123);
            labelLoad.TabIndex = 1;
            labelLoad.Text = "999.99 KW";
            labelLoad.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // groupBoxExport
            // 
            groupBoxExport.Controls.Add(labelExportImport);
            groupBoxExport.Dock = DockStyle.Fill;
            groupBoxExport.Location = new Point(4, 308);
            groupBoxExport.Name = "groupBoxExport";
            groupBoxExport.Size = new Size(792, 145);
            groupBoxExport.TabIndex = 6;
            groupBoxExport.TabStop = false;
            groupBoxExport.Text = "Export";
            // 
            // labelExportImport
            // 
            labelExportImport.Dock = DockStyle.Fill;
            labelExportImport.ForeColor = SystemColors.ControlText;
            labelExportImport.Location = new Point(3, 19);
            labelExportImport.Name = "labelExportImport";
            labelExportImport.Size = new Size(786, 123);
            labelExportImport.TabIndex = 2;
            labelExportImport.Text = "999.99 KW";
            labelExportImport.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // groupBoxHotWater
            // 
            groupBoxHotWater.Controls.Add(tableLayoutPanel2);
            groupBoxHotWater.Dock = DockStyle.Fill;
            groupBoxHotWater.Location = new Point(4, 460);
            groupBoxHotWater.Name = "groupBoxHotWater";
            groupBoxHotWater.Size = new Size(792, 67);
            groupBoxHotWater.TabIndex = 8;
            groupBoxHotWater.TabStop = false;
            groupBoxHotWater.Text = "Hot Water Switch";
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.ColumnCount = 2;
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3333321F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 66.6666641F));
            tableLayoutPanel2.Controls.Add(labelHotWater, 1, 0);
            tableLayoutPanel2.Controls.Add(checkBoxHotWater, 0, 0);
            tableLayoutPanel2.Dock = DockStyle.Fill;
            tableLayoutPanel2.Location = new Point(3, 19);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 1;
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel2.Size = new Size(786, 45);
            tableLayoutPanel2.TabIndex = 7;
            // 
            // labelHotWater
            // 
            labelHotWater.Dock = DockStyle.Fill;
            labelHotWater.Location = new Point(264, 0);
            labelHotWater.MaximumSize = new Size(0, 65);
            labelHotWater.Name = "labelHotWater";
            labelHotWater.Size = new Size(519, 45);
            labelHotWater.TabIndex = 0;
            labelHotWater.Text = "On, 99.99 KW, 9.99 left";
            labelHotWater.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // checkBoxHotWater
            // 
            checkBoxHotWater.Appearance = Appearance.Button;
            checkBoxHotWater.AutoSize = true;
            checkBoxHotWater.Dock = DockStyle.Fill;
            checkBoxHotWater.ForeColor = SystemColors.ControlText;
            checkBoxHotWater.Location = new Point(3, 3);
            checkBoxHotWater.MinimumSize = new Size(45, 22);
            checkBoxHotWater.Name = "checkBoxHotWater";
            checkBoxHotWater.OffBackColor = Color.Gray;
            checkBoxHotWater.OffToggleColor = Color.Gainsboro;
            checkBoxHotWater.OnBackColor = Color.Red;
            checkBoxHotWater.OnToggleColor = Color.WhiteSmoke;
            checkBoxHotWater.Size = new Size(255, 39);
            checkBoxHotWater.TabIndex = 1;
            checkBoxHotWater.TextAlign = ContentAlignment.MiddleCenter;
            checkBoxHotWater.UseVisualStyleBackColor = true;
            checkBoxHotWater.Click += checkBoxHotWater_Click;
            // 
            // timerUI
            // 
            timerUI.Enabled = true;
            timerUI.Interval = 1000;
            timerUI.Tick += timerUI_Tick;
            // 
            // timerGetData
            // 
            timerGetData.Enabled = true;
            timerGetData.Interval = 10000;
            timerGetData.Tick += timerGetData_Tick;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSize = true;
            ClientSize = new Size(800, 555);
            Controls.Add(tableLayoutPanel1);
            Controls.Add(menuStrip1);
            MainMenuStrip = menuStrip1;
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            KeyDown += Form1_KeyDown;
            Resize += Form1_SizeChanged;
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            tableLayoutPanel1.ResumeLayout(false);
            groupBox1.ResumeLayout(false);
            groupBox2.ResumeLayout(false);
            groupBoxExport.ResumeLayout(false);
            groupBoxHotWater.ResumeLayout(false);
            tableLayoutPanel2.ResumeLayout(false);
            tableLayoutPanel2.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MenuStrip menuStrip1;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem refreshTokenToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem exitToolStripMenuItem;
        private TableLayoutPanel tableLayoutPanel1;
        private Label labelGenerating;
        private Label labelLoad;
        private Label labelExportImport;
        private GroupBox groupBox1;
        private GroupBox groupBox2;
        private GroupBox groupBoxExport;
        private System.Windows.Forms.Timer timerUI;
        private System.Windows.Forms.Timer timerGetData;
        private TableLayoutPanel tableLayoutPanel2;
        private Label labelHotWater;
        private RJToggleButton checkBoxHotWater;
        private GroupBox groupBoxHotWater;
    }
}
