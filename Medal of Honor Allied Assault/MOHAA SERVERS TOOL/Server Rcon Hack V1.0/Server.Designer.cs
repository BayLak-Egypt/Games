namespace mohaa_server_tool
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.listView1 = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader8 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.searchTextBox = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.blockedListView = new System.Windows.Forms.ListView();
            this.columnHeader9 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader10 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader11 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader12 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader13 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader14 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader15 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader16 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.contextMenuStrip2 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.startToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rconpasswordHackToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bOTSTRACKV51ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.scanFilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addIPToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pinningToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.blockedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.refreshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.unbanToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.contextMenuStrip2.SuspendLayout();
            this.SuspendLayout();
            // 
            // listView1
            // 
            this.listView1.Activation = System.Windows.Forms.ItemActivation.OneClick;
            this.listView1.BackColor = System.Drawing.SystemColors.Window;
            this.listView1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader5,
            this.columnHeader6,
            this.columnHeader7,
            this.columnHeader8});
            this.listView1.ContextMenuStrip = this.contextMenuStrip1;
            this.listView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView1.FullRowSelect = true;
            this.listView1.HideSelection = false;
            this.listView1.HoverSelection = true;
            this.listView1.Location = new System.Drawing.Point(0, 31);
            this.listView1.Margin = new System.Windows.Forms.Padding(0);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(942, 388);
            this.listView1.TabIndex = 0;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Status";
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Game";
            this.columnHeader2.Width = 70;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Server Name";
            this.columnHeader3.Width = 250;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Server IP Address";
            this.columnHeader4.Width = 140;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "GameType";
            this.columnHeader5.Width = 120;
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "Map";
            this.columnHeader6.Width = 80;
            // 
            // columnHeader7
            // 
            this.columnHeader7.Text = "Location";
            this.columnHeader7.Width = 150;
            // 
            // columnHeader8
            // 
            this.columnHeader8.Text = "Players";
            this.columnHeader8.Width = 50;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.startToolStripMenuItem,
            this.statusToolStripMenuItem,
            this.rconpasswordHackToolStripMenuItem,
            this.bOTSTRACKV51ToolStripMenuItem,
            this.scanFilesToolStripMenuItem,
            this.addIPToolStripMenuItem,
            this.removeToolStripMenuItem,
            this.pinningToolStripMenuItem,
            this.blockedToolStripMenuItem,
            this.refreshToolStripMenuItem,
            this.settingsToolStripMenuItem,
            this.aboutToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(210, 268);
            // 
            // panel1
            // 
            this.panel1.AutoSize = true;
            this.panel1.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.searchTextBox);
            this.panel1.Controls.Add(this.button2);
            this.panel1.Controls.Add(this.button1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(942, 31);
            this.panel1.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.Red;
            this.label1.Location = new System.Drawing.Point(582, 10);
            this.label1.Margin = new System.Windows.Forms.Padding(1);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Search:";
            // 
            // searchTextBox
            // 
            this.searchTextBox.Location = new System.Drawing.Point(628, 6);
            this.searchTextBox.Margin = new System.Windows.Forms.Padding(1);
            this.searchTextBox.Name = "searchTextBox";
            this.searchTextBox.Size = new System.Drawing.Size(134, 20);
            this.searchTextBox.TabIndex = 5;
            this.searchTextBox.TextChanged += new System.EventHandler(this.searchTextBox_TextChanged);
            // 
            // button2
            // 
            this.button2.AutoSize = true;
            this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button2.Location = new System.Drawing.Point(85, 3);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(76, 25);
            this.button2.TabIndex = 4;
            this.button2.Text = "Blocked(0)";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button1
            // 
            this.button1.AutoSize = true;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Location = new System.Drawing.Point(3, 3);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(76, 25);
            this.button1.TabIndex = 3;
            this.button1.Text = "Servers(0)";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // blockedListView
            // 
            this.blockedListView.Activation = System.Windows.Forms.ItemActivation.OneClick;
            this.blockedListView.BackColor = System.Drawing.SystemColors.Window;
            this.blockedListView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.blockedListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader9,
            this.columnHeader10,
            this.columnHeader11,
            this.columnHeader12,
            this.columnHeader13,
            this.columnHeader14,
            this.columnHeader15,
            this.columnHeader16});
            this.blockedListView.ContextMenuStrip = this.contextMenuStrip2;
            this.blockedListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.blockedListView.FullRowSelect = true;
            this.blockedListView.HideSelection = false;
            this.blockedListView.HoverSelection = true;
            this.blockedListView.Location = new System.Drawing.Point(0, 31);
            this.blockedListView.Margin = new System.Windows.Forms.Padding(0);
            this.blockedListView.Name = "blockedListView";
            this.blockedListView.Size = new System.Drawing.Size(942, 388);
            this.blockedListView.TabIndex = 3;
            this.blockedListView.UseCompatibleStateImageBehavior = false;
            this.blockedListView.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader9
            // 
            this.columnHeader9.Text = "Status";
            // 
            // columnHeader10
            // 
            this.columnHeader10.Text = "Game";
            this.columnHeader10.Width = 70;
            // 
            // columnHeader11
            // 
            this.columnHeader11.Text = "Server Name";
            this.columnHeader11.Width = 250;
            // 
            // columnHeader12
            // 
            this.columnHeader12.Text = "Server IP Address";
            this.columnHeader12.Width = 140;
            // 
            // columnHeader13
            // 
            this.columnHeader13.Text = "GameType";
            this.columnHeader13.Width = 120;
            // 
            // columnHeader14
            // 
            this.columnHeader14.Text = "Map";
            this.columnHeader14.Width = 80;
            // 
            // columnHeader15
            // 
            this.columnHeader15.Text = "Location";
            this.columnHeader15.Width = 150;
            // 
            // columnHeader16
            // 
            this.columnHeader16.Text = "Players";
            this.columnHeader16.Width = 50;
            // 
            // contextMenuStrip2
            // 
            this.contextMenuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.unbanToolStripMenuItem});
            this.contextMenuStrip2.Name = "contextMenuStrip2";
            this.contextMenuStrip2.Size = new System.Drawing.Size(181, 48);
            // 
            // startToolStripMenuItem
            // 
            this.startToolStripMenuItem.Image = global::mohaa_server_tool.Properties.Resources.start;
            this.startToolStripMenuItem.Name = "startToolStripMenuItem";
            this.startToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
            this.startToolStripMenuItem.Text = "Start";
            this.startToolStripMenuItem.Click += new System.EventHandler(this.startToolStripMenuItem_Click);
            // 
            // statusToolStripMenuItem
            // 
            this.statusToolStripMenuItem.Image = global::mohaa_server_tool.Properties.Resources.status;
            this.statusToolStripMenuItem.Name = "statusToolStripMenuItem";
            this.statusToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
            this.statusToolStripMenuItem.Text = "Status";
            this.statusToolStripMenuItem.Click += new System.EventHandler(this.statusToolStripMenuItem_Click);
            // 
            // rconpasswordHackToolStripMenuItem
            // 
            this.rconpasswordHackToolStripMenuItem.Image = global::mohaa_server_tool.Properties.Resources.bruteforce;
            this.rconpasswordHackToolStripMenuItem.Name = "rconpasswordHackToolStripMenuItem";
            this.rconpasswordHackToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
            this.rconpasswordHackToolStripMenuItem.Text = "Rconpassword bruteforce";
            this.rconpasswordHackToolStripMenuItem.Click += new System.EventHandler(this.rconpasswordHackToolStripMenuItem_Click);
            // 
            // bOTSTRACKV51ToolStripMenuItem
            // 
            this.bOTSTRACKV51ToolStripMenuItem.Image = global::mohaa_server_tool.Properties.Resources.bot;
            this.bOTSTRACKV51ToolStripMenuItem.Name = "bOTSTRACKV51ToolStripMenuItem";
            this.bOTSTRACKV51ToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
            this.bOTSTRACKV51ToolStripMenuItem.Text = "Bot Track";
            this.bOTSTRACKV51ToolStripMenuItem.Visible = false;
            // 
            // scanFilesToolStripMenuItem
            // 
            this.scanFilesToolStripMenuItem.Image = global::mohaa_server_tool.Properties.Resources.folder;
            this.scanFilesToolStripMenuItem.Name = "scanFilesToolStripMenuItem";
            this.scanFilesToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
            this.scanFilesToolStripMenuItem.Text = "Scan Files";
            this.scanFilesToolStripMenuItem.Click += new System.EventHandler(this.scanFilesToolStripMenuItem_Click);
            // 
            // addIPToolStripMenuItem
            // 
            this.addIPToolStripMenuItem.Image = global::mohaa_server_tool.Properties.Resources.add;
            this.addIPToolStripMenuItem.Name = "addIPToolStripMenuItem";
            this.addIPToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
            this.addIPToolStripMenuItem.Text = "Add IP";
            this.addIPToolStripMenuItem.Click += new System.EventHandler(this.addIPToolStripMenuItem_Click);
            // 
            // removeToolStripMenuItem
            // 
            this.removeToolStripMenuItem.Image = global::mohaa_server_tool.Properties.Resources.remove;
            this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
            this.removeToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
            this.removeToolStripMenuItem.Text = "Remove";
            this.removeToolStripMenuItem.Click += new System.EventHandler(this.removeToolStripMenuItem_Click);
            // 
            // pinningToolStripMenuItem
            // 
            this.pinningToolStripMenuItem.Image = global::mohaa_server_tool.Properties.Resources.pin;
            this.pinningToolStripMenuItem.Name = "pinningToolStripMenuItem";
            this.pinningToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
            this.pinningToolStripMenuItem.Text = "Pinning";
            this.pinningToolStripMenuItem.Click += new System.EventHandler(this.pinningToolStripMenuItem_Click);
            // 
            // blockedToolStripMenuItem
            // 
            this.blockedToolStripMenuItem.Image = global::mohaa_server_tool.Properties.Resources.block;
            this.blockedToolStripMenuItem.Name = "blockedToolStripMenuItem";
            this.blockedToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
            this.blockedToolStripMenuItem.Text = "Blocked";
            this.blockedToolStripMenuItem.Click += new System.EventHandler(this.blockedToolStripMenuItem_Click);
            // 
            // refreshToolStripMenuItem
            // 
            this.refreshToolStripMenuItem.Image = global::mohaa_server_tool.Properties.Resources.refresh;
            this.refreshToolStripMenuItem.Name = "refreshToolStripMenuItem";
            this.refreshToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
            this.refreshToolStripMenuItem.Text = "Refresh";
            this.refreshToolStripMenuItem.Click += new System.EventHandler(this.refreshToolStripMenuItem_Click);
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.Image = global::mohaa_server_tool.Properties.Resources.settings;
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
            this.settingsToolStripMenuItem.Text = "Settings";
            this.settingsToolStripMenuItem.Click += new System.EventHandler(this.settingsToolStripMenuItem_Click);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Image = global::mohaa_server_tool.Properties.Resources.about;
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // unbanToolStripMenuItem
            // 
            this.unbanToolStripMenuItem.Image = global::mohaa_server_tool.Properties.Resources.unlocked;
            this.unbanToolStripMenuItem.Name = "unbanToolStripMenuItem";
            this.unbanToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.unbanToolStripMenuItem.Text = "Unban";
            this.unbanToolStripMenuItem.Click += new System.EventHandler(this.unbanToolStripMenuItem_Click);
            // 
            // MainForm
            // 
            this.BackColor = System.Drawing.SystemColors.ControlDark;
            this.ClientSize = new System.Drawing.Size(942, 419);
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.blockedListView);
            this.Controls.Add(this.panel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.Text = "MOHAA SERVERS TOOL";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.contextMenuStrip1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.contextMenuStrip2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.ColumnHeader columnHeader7;
        private System.Windows.Forms.ColumnHeader columnHeader8;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem refreshToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rconpasswordHackToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem bOTSTRACKV51ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem startToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addIPToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem statusToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem scanFilesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem blockedToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pinningToolStripMenuItem;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ListView blockedListView;
        private System.Windows.Forms.ColumnHeader columnHeader9;
        private System.Windows.Forms.ColumnHeader columnHeader10;
        private System.Windows.Forms.ColumnHeader columnHeader11;
        private System.Windows.Forms.ColumnHeader columnHeader12;
        private System.Windows.Forms.ColumnHeader columnHeader13;
        private System.Windows.Forms.ColumnHeader columnHeader14;
        private System.Windows.Forms.ColumnHeader columnHeader15;
        private System.Windows.Forms.ColumnHeader columnHeader16;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip2;
        private System.Windows.Forms.ToolStripMenuItem unbanToolStripMenuItem;
        public System.Windows.Forms.TextBox searchTextBox;
        public System.Windows.Forms.Label label1;
        public System.Windows.Forms.ListView listView1;
    }
}
