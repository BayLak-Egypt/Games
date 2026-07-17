using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks; // مكتبة المهام الخلفية
using System.Windows.Forms;

namespace mohaa_server_tool
{
    public partial class ViewFiles : Form
    {
        public string IP { get; set; }
        private string _mode = "DRIVES";
        private string _currentPath = "";
        private bool _isWorking = false; // لمنع التكرار والتهنيج

        private const string ICON_DRIVE = "💽 ";
        private const string ICON_FOLDER = "📁 ";
        private const string ICON_FILE = "📄 ";
        private ImageList _imageList;
        public ViewFiles()
        {
            InitializeComponent();
            SetupImageList(); // إعداد الأيقونات فور تشغيل الفورم

            this.btnConnect.Click += async (s, e) => await ConnectAsync();
            this.btnBack.Click += async (s, e) => await GoUpAsync();
            this.btnRefresh.Click += async (s, e) => await RefreshListAsync();
            this.listView1.DoubleClick += async (s, e) => await ListView1_DoubleClickAsync(s, e);
        }

        private void SetupImageList()
        {
            _imageList = new ImageList();
            _imageList.ImageSize = new Size(16, 16);
            _imageList.ColorDepth = ColorDepth.Depth32Bit;

            // استدعاء الصور بالأسماء التي اخترتها في الـ Resources
            _imageList.Images.Add("drive", Properties.Resources.drive);   // Index 0
            _imageList.Images.Add("folder", Properties.Resources.folder); // Index 1
            _imageList.Images.Add("file", Properties.Resources.file);     // Index 2

            listView1.SmallImageList = _imageList;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!string.IsNullOrEmpty(this.IP) && this.IP.Contains(":"))
            {
                string[] parts = this.IP.Split(':');
                txtIP.Text = parts[0];
                txtPort.Text = parts[1];
            }
            else if (!string.IsNullOrEmpty(this.IP))
            {
                txtIP.Text = this.IP;
            }
        }

        // دالة الإرسال أصبحت تستقبل البيانات فقط ولا تغير في الواجهة مباشرة
        private string SendRcon(string command)
        {
            try
            {
                using (UdpClient client = new UdpClient())
                {
                    client.Client.ReceiveTimeout = 2500; // تقليل وقت الانتظار قليلاً للسرعة
                    IPEndPoint ep = new IPEndPoint(IPAddress.Parse(txtIP.Text.Trim()), int.Parse(txtPort.Text.Trim()));

                    byte[] header = { 0xff, 0xff, 0xff, 0xff, 0x02 };
                    byte[] payload = Encoding.ASCII.GetBytes($"rcon {txtPass.Text} {command}");
                    byte[] packet = header.Concat(payload).ToArray();

                    client.Send(packet, packet.Length, ep);

                    IPEndPoint remoteEp = new IPEndPoint(IPAddress.Any, 0);
                    byte[] response = client.Receive(ref remoteEp);

                    return Encoding.UTF8.GetString(response).Replace("print", "").Trim('\0', '\xff', '\r', '\n', ' ');
                }
            }
            catch { return ""; }
        }
        private async Task ConnectAsync()
        {
            if (_isWorking) return;
            _isWorking = true;
            btnConnect.Enabled = false;

            try
            {
                await Task.Run(() => DetectDrives());
            }
            finally
            {
                // نستخدم SafeInvoke هنا لأن العملية قد تنتهي بعد إغلاق الفورم
                SafeInvoke(() => {
                    btnConnect.Enabled = true;
                    _isWorking = false;
                });
            }
        }
        private void DetectDrives()
        {
            _mode = "DRIVES";

            SafeInvoke(() => {
                listView1.Items.Clear();
                lblPath.Text = "Path: Scanning Drives...";
                progressBar1.Value = 0;
            });

            string letters = "CDEFGHIJKLMNOPQRSTUVWXYZ";
            bool foundAny = false;

            for (int i = 0; i < letters.Length; i++)
            {
                // فحص: هل المستخدم أغلق النافذة أثناء البحث؟
                if (this.IsDisposed) return;

                char letter = letters[i];
                string outRcon = SendRcon($"dir {letter}:\\");

                if (outRcon.ToLower().Contains("directory of"))
                {
                    foundAny = true;
                    SafeInvoke(() => {
                        var item = new ListViewItem(letter + ":\\", 0);
                        item.SubItems.AddRange(new string[] { "", "", "" });
                        item.ForeColor = Color.OrangeRed;
                        item.Tag = "DRIVE";
                        listView1.Items.Add(item);
                    });
                }

                SafeInvoke(() => {
                    int progress = (int)(((double)(i + 1) / letters.Length) * 100);
                    progressBar1.Value = progress;
                });
            }

            // فحص أخير قبل عرض النتيجة أو الرسالة
            if (this.IsDisposed) return;

            SafeInvoke(() => {
                lblPath.Text = "Path: My Computer";
                progressBar1.Value = 100;

                if (!foundAny)
                {
                    MessageBox.Show("No drives found. Check IP/Port or Rconpassword.", "Connection Error",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            });
        }
        private void ParseDir(string text)
        {
            this.Invoke((MethodInvoker)delegate {
                listView1.Items.Clear();
                bool started = false;
                string[] lines = text.Split(new[] { "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string line in lines)
                {
                    string trimmed = line.Trim();
                    if (string.IsNullOrEmpty(trimmed)) continue;

                    // تحديد مسار المجلد الحالي
                    if (trimmed.ToLower().Contains("directory of"))
                    {
                        _currentPath = trimmed.Substring(trimmed.ToLower().IndexOf("of") + 2).Trim();
                        lblPath.Text = "Path: " + _currentPath;
                        started = true;
                        continue;
                    }

                    if (!started) continue;

                    // Regex المطور لمعالجة الوقت والتاريخ والحجم والاسم
                    var match = Regex.Match(trimmed, @"^(\d{1,2}/\d{1,2}/\d{4})\s+(\d{1,2}:\d{2})\s*([ap]?)\s+(<DIR>|[\d,.]+k?m?g?|[\d,]+)\s+(.*)$", RegexOptions.IgnoreCase);

                    if (match.Success)
                    {
                        string date = match.Groups[1].Value;
                        string timePart = match.Groups[2].Value;
                        string amPmPart = match.Groups[3].Value.ToLower();
                        string sizeRaw = match.Groups[4].Value;
                        string name = match.Groups[5].Value.Trim();

                        if (name == "." || name == "..") continue;

                        // تحويل التنسيق إلى AM/PM
                        string formattedTime = timePart;
                        if (amPmPart == "a") formattedTime += " AM";
                        else if (amPmPart == "p") formattedTime += " PM";
                        else if (!string.IsNullOrEmpty(amPmPart)) formattedTime += " " + amPmPart.ToUpper();

                        bool isDir = sizeRaw.ToUpper().Contains("<DIR>");

                        // اختيار الأيقونة بناءً على الأسماء في ImageList
                        // 1 للمجلد (folder) و 2 للملف (file)
                        int imageIndex = isDir ? 1 : 2;

                        // إنشاء العنصر بالاسم والأيقونة مباشرة
                        ListViewItem item = new ListViewItem(name, imageIndex);

                        // توزيع البيانات على الأعمدة
                        item.SubItems.Add(isDir ? "" : sizeRaw); // الحجم
                        item.SubItems.Add(formattedTime);       // الوقت المنسق
                        item.SubItems.Add(date);                // التاريخ

                        if (isDir)
                        {
                            item.ForeColor = Color.DodgerBlue;
                            item.Tag = "DIR";
                        }
                        else
                        {
                            item.Tag = "FILE";
                        }

                        listView1.Items.Add(item);
                    }

                    // تحديث الإحصائيات (عدد الملفات والحجم)
                    var statsMatch = Regex.Match(trimmed, @"(\d+\s+file\(s\).*bytes)", RegexOptions.IgnoreCase);
                    if (statsMatch.Success)
                    {
                        lblStats.Text = statsMatch.Groups[1].Value;
                    }
                }
            });
        }
        private async Task ListView1_DoubleClickAsync(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0 || _isWorking) return;
            var selected = listView1.SelectedItems[0];

            // الاسم الآن هو النص المكتوب مباشرة بدون Substring
            string cleanName = selected.Text;

            if (selected.Tag.ToString() == "DRIVE" || selected.Tag.ToString() == "DIR")
            {
                _isWorking = true;
                await Task.Run(() => {
                    string cmd = (selected.Tag.ToString() == "DRIVE") ? $"cd {cleanName}" : $"cd \"{cleanName}\"";
                    SendRcon(cmd);
                    _mode = "FILES";
                    ParseDir(SendRcon("dir"));
                });
                _isWorking = false;
            }
        }
        private async Task GoUpAsync()
        {
            if (_mode == "DRIVES" || _isWorking) return;
            _isWorking = true;

            await Task.Run(() => {
                if (_currentPath.Length <= 3) { DetectDrives(); }
                else { SendRcon("cd .."); ParseDir(SendRcon("dir")); }
            });
            _isWorking = false;
        }

        private async Task RefreshListAsync()
        {
            if (_isWorking) return;
            _isWorking = true;

            await Task.Run(() => {
                if (_mode == "DRIVES") DetectDrives();
                else ParseDir(SendRcon("dir"));
            });
            _isWorking = false;
        }
        private void SafeInvoke(Action action)
        {
            // إذا تم إغلاق الفورم أو لم يتم إنشاء الـ Handle الخاص به، اخرج فوراً
            if (this.IsDisposed || !this.IsHandleCreated) return;

            try
            {
                if (this.InvokeRequired)
                    this.Invoke(action);
                else
                    action();
            }
            catch (ObjectDisposedException) { /* تم إغلاق الفورم، تجاهل الخطأ بسلام */ }
            catch (InvalidOperationException) { /* النافذة لم تعد موجودة */ }
        }

        private void ViewFiles_Load(object sender, EventArgs e)
        {
            var main = Application.OpenForms["MainForm"] as MainForm;
            if (main?.CurrentSettings != null)
            {
                this.SuspendLayout();

                // 1. تطبيق الثيم الأساسي (الخلفية والأزرار)
                ThemeManager.ApplyTheme(this, null, main.CurrentSettings.SelectedTheme);

                // 2. تطبيق الثيم المخصص للأدوات داخل الحاويات
                ApplyAdvancedTheme(this, this.ForeColor, this.BackColor);

                this.ResumeLayout(true);
            }
        }
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
                // 1. تطبيق الثيم الأساسي عبر المانجر
                ThemeManager.ApplyTheme(this, null, themeName);

                // 2. تحديث الأدوات المتقدمة (ListView, TextBoxes, Labels)
                ApplyAdvancedTheme(this, this.ForeColor, this.BackColor);
            }
            finally
            {
                this.ResumeLayout(true);
                this.Refresh();
            }
        }
        private void ApplyAdvancedTheme(Control container, Color foreColor, Color backColor)
        {
            foreach (Control ctrl in container.Controls)
            {
                // تلوين النصوص والـ Labels والـ CheckBoxes
                if (ctrl is Label || ctrl is CheckBox || ctrl is GroupBox)
                {
                    ctrl.ForeColor = foreColor;
                    if (ctrl is Label || ctrl is CheckBox) ctrl.BackColor = Color.Transparent;
                }

                // تخصيص الـ ListView ليتناسب مع الثيم الداكن أو الفاتح
                if (ctrl is ListView lv)
                {
                    lv.BackColor = backColor;
                    lv.ForeColor = foreColor;
                    // تحسين مظهر الحواف في الثيمات الداكنة
                    lv.BorderStyle = BorderStyle.FixedSingle;
                }

                // تخصيص الـ TextBox
                if (ctrl is TextBox txt)
                {
                    txt.BackColor = ControlPaint.Light(backColor, 0.2f); // جعل الخلفية أفتح قليلاً من الفورم
                    txt.ForeColor = foreColor;
                }

                if (ctrl.HasChildren) ApplyAdvancedTheme(ctrl, foreColor, backColor);
            }
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                // Option A: Copy just the name
                // string textToCopy = listView1.SelectedItems[0].Text;

                // Option B: Copy the full remote path (More useful for RCON)
                string name = listView1.SelectedItems[0].Text;
                string fullPath = _currentPath.EndsWith("\\") ? _currentPath + name : _currentPath + "\\" + name;

                Clipboard.SetText(fullPath);
            }
        }

        private void copyAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.Items.Count == 0) return;

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Directory of: {_currentPath}");
            sb.AppendLine("Name\tSize\tTime\tDate");
            sb.AppendLine("--------------------------------------------------");

            foreach (ListViewItem item in listView1.Items)
            {
                // Joins the main text and all sub-items (Size, Time, Date) with tabs
                string row = $"{item.Text}\t{item.SubItems[1].Text}\t{item.SubItems[2].Text}\t{item.SubItems[3].Text}";
                sb.AppendLine(row);
            }

            Clipboard.SetText(sb.ToString());
            MessageBox.Show("All directory information copied to clipboard.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}