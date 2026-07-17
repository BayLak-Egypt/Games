namespace mohaa_server_tool
{
    partial class ViewFiles
    {
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// تنظيف الموارد المستخدمة.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ViewFiles));
            this.topFrame = new System.Windows.Forms.Panel();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.btnBack = new System.Windows.Forms.Button();
            this.btnConnect = new System.Windows.Forms.Button();
            this.txtPass = new System.Windows.Forms.TextBox();
            this.lblPassHeader = new System.Windows.Forms.Label();
            this.txtPort = new System.Windows.Forms.TextBox();
            this.txtIP = new System.Windows.Forms.TextBox();
            this.infoBar = new System.Windows.Forms.Panel();
            this.lblStats = new System.Windows.Forms.Label();
            this.lblPath = new System.Windows.Forms.Label();
            this.listView1 = new System.Windows.Forms.ListView();
            this.colName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colSize = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colTime = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colDate = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.topFrame.SuspendLayout();
            this.infoBar.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // topFrame
            // 
            this.topFrame.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.topFrame.Controls.Add(this.btnRefresh);
            this.topFrame.Controls.Add(this.btnBack);
            this.topFrame.Controls.Add(this.btnConnect);
            this.topFrame.Controls.Add(this.txtPass);
            this.topFrame.Controls.Add(this.lblPassHeader);
            this.topFrame.Controls.Add(this.txtPort);
            this.topFrame.Controls.Add(this.txtIP);
            this.topFrame.Dock = System.Windows.Forms.DockStyle.Top;
            this.topFrame.Location = new System.Drawing.Point(0, 0);
            this.topFrame.Name = "topFrame";
            this.topFrame.Size = new System.Drawing.Size(628, 42);
            this.topFrame.TabIndex = 2;
            // 
            // btnRefresh
            // 
            this.btnRefresh.Location = new System.Drawing.Point(531, 8);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(85, 25);
            this.btnRefresh.TabIndex = 0;
            this.btnRefresh.Text = "Refresh";
            // 
            // btnBack
            // 
            this.btnBack.Location = new System.Drawing.Point(450, 8);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(75, 25);
            this.btnBack.TabIndex = 1;
            this.btnBack.Text = "Up";
            // 
            // btnConnect
            // 
            this.btnConnect.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(167)))), ((int)(((byte)(69)))));
            this.btnConnect.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnConnect.ForeColor = System.Drawing.Color.White;
            this.btnConnect.Location = new System.Drawing.Point(369, 8);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(75, 25);
            this.btnConnect.TabIndex = 2;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = false;
            // 
            // txtPass
            // 
            this.txtPass.Location = new System.Drawing.Point(243, 11);
            this.txtPass.Name = "txtPass";
            this.txtPass.PasswordChar = '●';
            this.txtPass.Size = new System.Drawing.Size(120, 20);
            this.txtPass.TabIndex = 3;
            // 
            // lblPassHeader
            // 
            this.lblPassHeader.AutoSize = true;
            this.lblPassHeader.Location = new System.Drawing.Point(161, 15);
            this.lblPassHeader.Name = "lblPassHeader";
            this.lblPassHeader.Size = new System.Drawing.Size(81, 13);
            this.lblPassHeader.TabIndex = 4;
            this.lblPassHeader.Text = "Rconpassword:";
            // 
            // txtPort
            // 
            this.txtPort.Location = new System.Drawing.Point(105, 11);
            this.txtPort.Name = "txtPort";
            this.txtPort.Size = new System.Drawing.Size(50, 20);
            this.txtPort.TabIndex = 5;
            // 
            // txtIP
            // 
            this.txtIP.Enabled = false;
            this.txtIP.Location = new System.Drawing.Point(9, 11);
            this.txtIP.Name = "txtIP";
            this.txtIP.Size = new System.Drawing.Size(90, 20);
            this.txtIP.TabIndex = 6;
            // 
            // infoBar
            // 
            this.infoBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.infoBar.Controls.Add(this.lblStats);
            this.infoBar.Controls.Add(this.lblPath);
            this.infoBar.Dock = System.Windows.Forms.DockStyle.Top;
            this.infoBar.Location = new System.Drawing.Point(0, 42);
            this.infoBar.Name = "infoBar";
            this.infoBar.Size = new System.Drawing.Size(628, 30);
            this.infoBar.TabIndex = 1;
            // 
            // lblStats
            // 
            this.lblStats.Location = new System.Drawing.Point(366, 5);
            this.lblStats.Name = "lblStats";
            this.lblStats.Size = new System.Drawing.Size(204, 20);
            this.lblStats.TabIndex = 0;
            this.lblStats.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblPath
            // 
            this.lblPath.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblPath.Location = new System.Drawing.Point(3, 10);
            this.lblPath.Name = "lblPath";
            this.lblPath.Size = new System.Drawing.Size(350, 20);
            this.lblPath.TabIndex = 1;
            this.lblPath.Text = "Path: Disconnected";
            // 
            // listView1
            // 
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colName,
            this.colSize,
            this.colTime,
            this.colDate});
            this.listView1.ContextMenuStrip = this.contextMenuStrip1;
            this.listView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView1.FullRowSelect = true;
            this.listView1.GridLines = true;
            this.listView1.HideSelection = false;
            this.listView1.Location = new System.Drawing.Point(0, 72);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(628, 379);
            this.listView1.TabIndex = 0;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            // 
            // colName
            // 
            this.colName.Text = "Name";
            this.colName.Width = 282;
            // 
            // colSize
            // 
            this.colSize.Text = "Size";
            this.colSize.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.colSize.Width = 108;
            // 
            // colTime
            // 
            this.colTime.Text = "Time";
            this.colTime.Width = 65;
            // 
            // colDate
            // 
            this.colDate.Text = "Date";
            this.colDate.Width = 169;
            // 
            // progressBar1
            // 
            this.progressBar1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.progressBar1.Location = new System.Drawing.Point(0, 451);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(628, 10);
            this.progressBar1.TabIndex = 3;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyToolStripMenuItem,
            this.copyAllToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(181, 70);
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Image = global::mohaa_server_tool.Properties.Resources.copy;
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.copyToolStripMenuItem.Text = "copy";
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
            // 
            // copyAllToolStripMenuItem
            // 
            this.copyAllToolStripMenuItem.Image = global::mohaa_server_tool.Properties.Resources.copyall;
            this.copyAllToolStripMenuItem.Name = "copyAllToolStripMenuItem";
            this.copyAllToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.copyAllToolStripMenuItem.Text = "copy all";
            this.copyAllToolStripMenuItem.Click += new System.EventHandler(this.copyAllToolStripMenuItem_Click);
            // 
            // ViewFiles
            // 
            this.BackColor = System.Drawing.SystemColors.ControlLight;
            this.ClientSize = new System.Drawing.Size(628, 461);
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.infoBar);
            this.Controls.Add(this.topFrame);
            this.Controls.Add(this.progressBar1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ViewFiles";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Scan Files";
            this.Load += new System.EventHandler(this.ViewFiles_Load);
            this.topFrame.ResumeLayout(false);
            this.topFrame.PerformLayout();
            this.infoBar.ResumeLayout(false);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        // العناصر الرسومية (UI Components)
        private System.Windows.Forms.Panel topFrame, infoBar;
        private System.Windows.Forms.TextBox txtIP, txtPort, txtPass;
        private System.Windows.Forms.Button btnConnect, btnBack, btnRefresh;
        private System.Windows.Forms.Label lblPassHeader, lblPath, lblStats;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader colName, colSize, colTime, colDate;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyAllToolStripMenuItem;
    }
}