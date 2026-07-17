namespace QuakeRcon
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.btnSend = new System.Windows.Forms.Button();
            this.txtWordlistPath = new System.Windows.Forms.TextBox();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.lblRemainingPasswords = new System.Windows.Forms.Label();
            this.lblTotalPasswords = new System.Windows.Forms.Label();
            this.lblSelectedIP = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.lblMessage = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnSend
            // 
            this.btnSend.Location = new System.Drawing.Point(287, 64);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(75, 23);
            this.btnSend.TabIndex = 3;
            this.btnSend.Text = "Send";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // txtWordlistPath
            // 
            this.txtWordlistPath.Location = new System.Drawing.Point(12, 25);
            this.txtWordlistPath.Name = "txtWordlistPath";
            this.txtWordlistPath.ReadOnly = true;
            this.txtWordlistPath.Size = new System.Drawing.Size(269, 20);
            this.txtWordlistPath.TabIndex = 5;
            // 
            // btnBrowse
            // 
            this.btnBrowse.Location = new System.Drawing.Point(287, 22);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(75, 23);
            this.btnBrowse.TabIndex = 6;
            this.btnBrowse.Text = "Browse";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(12, 64);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(269, 23);
            this.progressBar1.TabIndex = 7;
            // 
            // lblRemainingPasswords
            // 
            this.lblRemainingPasswords.AutoSize = true;
            this.lblRemainingPasswords.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRemainingPasswords.ForeColor = System.Drawing.Color.Black;
            this.lblRemainingPasswords.Location = new System.Drawing.Point(9, 104);
            this.lblRemainingPasswords.Name = "lblRemainingPasswords";
            this.lblRemainingPasswords.Size = new System.Drawing.Size(81, 15);
            this.lblRemainingPasswords.TabIndex = 8;
            this.lblRemainingPasswords.Text = "Remaining: 0";
            // 
            // lblTotalPasswords
            // 
            this.lblTotalPasswords.AutoSize = true;
            this.lblTotalPasswords.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotalPasswords.ForeColor = System.Drawing.Color.Black;
            this.lblTotalPasswords.Location = new System.Drawing.Point(9, 90);
            this.lblTotalPasswords.Name = "lblTotalPasswords";
            this.lblTotalPasswords.Size = new System.Drawing.Size(110, 15);
            this.lblTotalPasswords.TabIndex = 9;
            this.lblTotalPasswords.Text = "Total Passwords: 0";
            // 
            // lblSelectedIP
            // 
            this.lblSelectedIP.AutoSize = true;
            this.lblSelectedIP.Location = new System.Drawing.Point(12, 9);
            this.lblSelectedIP.Name = "lblSelectedIP";
            this.lblSelectedIP.Size = new System.Drawing.Size(90, 13);
            this.lblSelectedIP.TabIndex = 10;
            this.lblSelectedIP.Text = "Target IP: 0.0.0.0";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(287, 93);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 12;
            this.button1.Text = "Stop";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // lblMessage
            // 
            this.lblMessage.AutoSize = true;
            this.lblMessage.Location = new System.Drawing.Point(12, 48);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(49, 13);
            this.lblMessage.TabIndex = 13;
            this.lblMessage.Text = "Status?..";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(368, 121);
            this.Controls.Add(this.lblMessage);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.lblSelectedIP);
            this.Controls.Add(this.lblTotalPasswords);
            this.Controls.Add(this.lblRemainingPasswords);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.btnBrowse);
            this.Controls.Add(this.txtWordlistPath);
            this.Controls.Add(this.btnSend);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximumSize = new System.Drawing.Size(384, 160);
            this.MinimumSize = new System.Drawing.Size(384, 160);
            this.Name = "Form1";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Rconpassword  BruteForce";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.TextBox txtWordlistPath;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label lblRemainingPasswords;
        private System.Windows.Forms.Label lblTotalPasswords;
        private System.Windows.Forms.Label lblSelectedIP;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label lblMessage;
    }
}
