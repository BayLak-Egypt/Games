using HtmlAgilityPack;
using Newtonsoft.Json;  // تأكد من تثبيت مكتبة Newtonsoft.Json عبر NuGet
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace mohaa_server_tool
{
    public partial class status : Form
    {
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int wMsg, bool wParam, int lParam);
        private const int WM_SETREDRAW = 0x0B;
        private bool isClosing = false;
        public string IP { get; set; }
        public string ServerName { get; set; }
        public string ServerMap { get; set; }

        public string IPOnly => IP?.Split(':')[0] ?? string.Empty;
        public string PORTOnly => (IP?.Contains(':') == true) ? IP.Split(':')[1] : "N/A";

        private List<int> PortsToCheck = new List<int> { 12203, 12204, 12205, 12206, 12207, 12208, 12209, 12250 };
        private List<int> ImportantPorts = new List<int>
        {
            21, 22, 25, 53, 80, 110, 143, 443, 3306, 3389, 5432, 8080, 8443
        };
        private HashSet<int> OpenPorts = new HashSet<int>();
        private HashSet<int> OpenImportantPorts = new HashSet<int>();
        private int TimeoutMilliseconds = 300;
        private Timer updateTimer;  // تعريف المؤقت
        private const int maxRetries = 30; // أقصى عدد لإعادة المحاولة

        public status()
        {
            InitializeComponent();
            this.button6.Enabled = false; // الزر مغلق في البداية
            this.button9.Enabled = false; // الزر مغلق في البداية

            // تهيئة المؤقت ليتم التحديث كل 1 ثانية
            updateTimer = new Timer();
            updateTimer.Interval = 1000; // تحديث كل 1 ثانية (1000 ميلي ثانية)
            updateTimer.Tick += UpdateTimer_Tick; // ربط حدث التحديث بالمؤقت
        }
        public void ApplyLanguage(string lang, string groupName)
        {
            try
            {
                string filePath = Path.Combine(Application.StartupPath, "Languages", $"{lang}.json");
                if (!File.Exists(filePath)) return;

                var languageData = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(File.ReadAllText(filePath));
                if (languageData == null || !languageData.ContainsKey(groupName)) return;

                var texts = languageData[groupName];

                foreach (var kv in texts)
                {
                    if (kv.Key == "FormTitle") { this.Text = kv.Value; continue; }

                    var ctl = this.Controls.Find(kv.Key, true).FirstOrDefault();
                    if (ctl != null)
                    {
                        // إذا كان النص يحتوي على ":"، نترجم ما قبلها فقط ونترك البيانات كما هي
                        if (ctl is Label && ctl.Text.Contains(":"))
                        {
                            string[] split = ctl.Text.Split(new[] { ':' }, 2);
                            string data = split.Length > 1 ? split[1] : "";
                            ctl.Text = $"{kv.Value}: {data.Trim()}";
                        }
                        else
                        {
                            ctl.Text = kv.Value;
                        }

                        // حفظ النص المترجم في Tag لاستخدامه في التحديثات التلقائية (مثل الـ Ping)
                        ctl.Tag = kv.Value;
                    }
                }
            }
            catch { }
        }
        private void ApplyThemeToAllLabels(Control parent, Color foreColor, Color backColor)
        {
            foreach (Control ctrl in parent.Controls)
            {
                if (ctrl is Label lbl)
                {
                    lbl.ForeColor = foreColor;
                    // إذا كنت تريد خلفية شفافة لـ Label فوق GroupBox
                    lbl.BackColor = Color.Transparent;
                }

                // إذا كان العنصر حاوية (مثل GroupBox)، ابحث داخله أيضاً
                if (ctrl.HasChildren)
                {
                    ApplyThemeToAllLabels(ctrl, foreColor, backColor);
                }
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

            // 1. تطبيق الثيم الأساسي (الألوان العامة والـ ListView)
            ThemeManager.ApplyTheme(this, lstPlayers, themeName);

            // 2. تحديث الـ Labels والـ ListBox لأنها تحتاج معاملة خاصة في كودك
            ApplyThemeToAllLabels(this, this.ForeColor, this.BackColor);
            ApplyListBoxThemeSafe(txtServerInfo, lstPlayers.BackColor, lstPlayers.ForeColor);

            this.ResumeLayout(true);
            this.Refresh();
        }
        private void status_Load(object sender, EventArgs e)
        {
            var main = Application.OpenForms["MainForm"] as MainForm;
            if (main?.CurrentSettings != null)
            {
                this.SuspendLayout();

                ThemeManager.ApplyTheme(
                    this,
                    lstPlayers, // ListView الرئيسي
                    main.CurrentSettings.SelectedTheme
                );
                // بعد تطبيق الثيم الأساسي، نمر على كل العناصر لتلوين الـ Labels يدوياً
                ApplyThemeToAllLabels(this, this.ForeColor, this.BackColor);
                this.ResumeLayout(true);
                this.Refresh();
            }

            SendStatusRequest();
            label1.Text = "IP: " + IPOnly;
            label2.Text = "Name: " + ServerName;
            label5.Text = "Server Port: " + PORTOnly;
            label12.Text = "Port other: Checking...";
            label7.Text = "Sudomain: Checking...";
            label3.Text = "Location: ";
            label4.Text = "ISP: ";

            // قائمة الاستبدالات (تعريفها قبل استخدامها)
            Dictionary<string, string> replacements = new Dictionary<string, string>
{
    { "mohdm1", "Southern France - Mohdm1" },
    { "mohdm2", "Destroyed Village - Mohdm2" },
    { "mohdm3", "Remagen - Mohdm3" },
    { "mohdm4", "The Crossroads - Mohdm4" },
    { "mohdm5", "Snowy Park - Mohdm5" },
    { "mohdm6", "Stalingrad - Mohdm6" },
    { "mohdm7", "Algiers - Mohdm7" } 
};


            label6.Text = "Map: " + CultureInfo.CurrentCulture.TextInfo
                .ToTitleCase(replacements.Aggregate(Path.GetFileName(ServerMap).ToLower(),
                (current, pair) => current.Replace(pair.Key, pair.Value)));

            label8.Text = "Org: "; // الآن هذا السطر يعمل بدون أن يتم استبداله
            label11.Text = "Time Started: " + DateTime.Now.ToString("HH:mm:ss"); // استخدم Label مختلف

            _ = Task.Run(() => CheckOpenPorts(IPOnly));
            _ = Task.Run(() => GetSubdomain(IPOnly));
            _ = Task.Run(() => GetIPDetails(IPOnly));
            _ = Task.Run(() => GetPing(IPOnly));
        }
        private void UpdateSaveButtonState()
        {
            if (txtServerInfo.InvokeRequired)
            {
                txtServerInfo.Invoke(new Action(UpdateSaveButtonState));
            }
            else
            {
                button9.Enabled = txtServerInfo.Items.Count > 0;
            }
        }

        private void GetPing(string ip)
        {
            try
            {
                using (Ping ping = new Ping())
                {
                    PingReply reply = ping.Send(ip, 1000); 

                    if (reply.Status == IPStatus.Success)
                    {
                        UpdateLabel(label13, $"Ping: {reply.RoundtripTime} ms");
                    }
                    else
                    {
                        UpdateLabel(label13, "Ping: N/A");
                    }
                }
            }
            catch
            {
                UpdateLabel(label13, "Ping: Error");
            }
        }
        private void CheckOpenPorts(string ip)
        {
            Parallel.ForEach(PortsToCheck, port =>
            {
                if (IsPortOpen(ip, port, TimeoutMilliseconds))
                {
                    lock (OpenPorts)
                    {
                        if (OpenPorts.Add(port))
                        {
                            AppendPortToUI(label5, port);
                        }
                    }
                }
            });

            Parallel.ForEach(ImportantPorts, port =>
            {
                if (IsPortOpen(ip, port, TimeoutMilliseconds))
                {
                    lock (OpenImportantPorts)
                    {
                        if (OpenImportantPorts.Add(port))
                        {
                            AppendPortToUI(label12, port);
                        }
                    }
                }
            });

            FinalUpdateUI();
        }

        private bool IsPortOpen(string ip, int port, int timeout)
        {
            try
            {
                using (TcpClient tcpClient = new TcpClient())
                {
                    var result = tcpClient.BeginConnect(ip, port, null, null);
                    bool success = result.AsyncWaitHandle.WaitOne(TimeSpan.FromMilliseconds(timeout));
                    if (success && tcpClient.Connected)
                    {
                        tcpClient.EndConnect(result);
                        return true;
                    }
                }
            }
            catch
            {
                // تجاهل الخطأ إذا كان المنفذ مغلقًا
            }
            return false;
        }

        private void AppendPortToUI(Label label, int port)
        {
            if (label.IsDisposed || !label.IsHandleCreated)
                return; // تجنب الوصول إلى كائن تم التخلص منه

            if (label.InvokeRequired)
            {
                try
                {
                    label.Invoke((MethodInvoker)(() => AppendPortToUI(label, port)));
                }
                catch (ObjectDisposedException)
                {
                    // تم التخلص من الكائن، لذا لا تفعل شيئًا
                }
                return;
            }

            string portStr = port.ToString();
            if (!label.Text.Split(',').Select(p => p.Trim()).Contains(portStr))
            {
                label.Text = label.Text.TrimEnd("Checking...".ToCharArray()) +
                             (string.IsNullOrEmpty(label.Text) ? "" : ", ") +
                             portStr;
            }
        }

        private void FinalUpdateUI()
        {
            if (label12.IsDisposed || this.IsDisposed) // Ensure the control and the form aren't disposed
            {
                return; // If either is disposed, skip the update
            }

            if (label12.InvokeRequired)
            {
                label12.Invoke(new Action(FinalUpdateUI));
            }
            else
            {
                if (!OpenImportantPorts.Any())
                {
                    label12.Text = "Port other: N/A";
                }
            }
        }

        private void GetSubdomain(string ip)
        {
            try
            {
                var hostEntry = Dns.GetHostEntry(ip);
                string subdomain = hostEntry.HostName;

                if (!string.IsNullOrEmpty(subdomain) && subdomain != ip)
                {
                    UpdateLabel(label7, "Sudomain: " + subdomain);
                    EnableButton(button6); // فعل الزر الآن
                }
                else
                {
                    UpdateLabel(label7, "Sudomain: N/A");
                    button6.Enabled = false; // الزر يظل مغلق
                }
            }
            catch
            {
                UpdateLabel(label7, "Sudomain: N/A");
                button6.Enabled = false; // الزر يظل مغلق
            }
        }
        private void EnableButton(Button btn)
        {
            if (btn.InvokeRequired)
            {
                btn.Invoke(new Action<Button>(EnableButton), btn);
            }
            else
            {
                btn.Enabled = true;
            }
        }


        private void GetIPDetails(string ip)
        {
            try
            {
                using (WebClient client = new WebClient())
                {
                    string json = client.DownloadString($"http://ip-api.com/json/{ip}");
                    dynamic result = JsonConvert.DeserializeObject(json);

                    if (result.status == "success")
                    {
                        UpdateLabel(label3, $"Location: {result.country}, {result.city}");
                        UpdateLabel(label4, $"ISP: {result.isp}");
                        UpdateLabel(label8, $"Org: {result.org}");
                    }
                    else
                    {
                        UpdateLabel(label3, "Location: N/A");
                        UpdateLabel(label4, "ISP: N/A");
                        UpdateLabel(label8, "Org: N/A");
                    }
                }
            }
            catch
            {
                UpdateLabel(label3, "Location: Error");
                UpdateLabel(label4, "ISP: Error");
                UpdateLabel(label8, "Org: Error");
            }
        }
        private void UpdateLabel(Label label, string text)
        {
            // إذا كان الفورم في طور الإغلاق أو تم تدمير الأداة، اخرج فوراً
            if (isClosing || label.IsDisposed || !label.IsHandleCreated) return;

            if (label.InvokeRequired)
            {
                try
                {
                    label.Invoke(new Action<Label, string>(UpdateLabel), label, text);
                }
                catch (ObjectDisposedException) { /* تم إغلاق الفورم أثناء الاستدعاء */ }
            }
            else
            {
                label.Text = text;
            }
        }
        // هذا الحدث يحدث عند مرور ثانية في المؤقت
        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
            // عندما يكون الـ CheckBox مفعل، قم بإرسال طلب للـ server
            if (chkAutoUpdate.Checked)
            {
                SendStatusRequest();
            }
        }

        private async void SendStatusRequest()
        {
            string cmd = "getstatus";
            byte[] message = BuildMessage(cmd);
            bool success = false;
            int retries = 0;

            while (retries < maxRetries && !success)
            {
                // توقف فوراً إذا تم إغلاق الفورم
                if (this.IsDisposed) return;

                try
                {
                    using (UdpClient client = new UdpClient())
                    {
                        client.Client.ReceiveTimeout = 2000;
                        await Task.Run(() => client.Send(message, message.Length, IPOnly, Convert.ToInt16(PORTOnly)));

                        IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
                        byte[] received = await Task.Run(() => client.Receive(ref remoteEndPoint));

                        // فحص بعد كل عملية await
                        if (this.IsDisposed) return;

                        string cleaned = CleanData(received);
                        string[] lines = cleaned.Split(new[] { '\n' }, StringSplitOptions.None);
                        string serverInfo = lines.Length > 1 ? lines[1] : "";

                        List<string> players = new List<string>();
                        for (int i = 2; i < lines.Length; i++)
                            if (!string.IsNullOrWhiteSpace(lines[i])) players.Add(lines[i]);

                        var serverData = ParseServerInfo(serverInfo);

                        // الاستدعاء الآن آمن
                        UpdateUI(serverData, players);
                        success = true;
                    }
                }
                catch (SocketException)
                {
                    retries++;
                    if (!this.IsDisposed)
                    {
                        UpdateStatusLabel($"خطأ في الاتصال، المحاولة {retries} من {maxRetries}.");
                        if (retries < maxRetries) await Task.Delay(2000);
                    }
                }
                catch (Exception) { break; } // أي خطأ غير متوقع يوقف الحلقة
            }
        }
        private void ApplyListBoxThemeSafe(ListBox lb, Color backColor, Color foreColor)
        {
            if (lb == null || lb.IsDisposed) return;

            if (lb.InvokeRequired)
            {
                lb.Invoke(new Action(() => ApplyListBoxThemeSafe(lb, backColor, foreColor)));
                return;
            }

            lb.BackColor = backColor;
            lb.ForeColor = foreColor;
        }



        private void UpdateUI(Dictionary<string, string> serverData, List<string> players)
        {
            // 1. فحص الأمان الأول: هل الفورم أو الأدوات أصبحت Dispose؟
            if (this.IsDisposed || lstPlayers.IsDisposed || txtServerInfo.IsDisposed)
                return;

            if (InvokeRequired)
            {
                try
                {
                    // نستخدم BeginInvoke بدلاً من Invoke ليكون الاستدعاء غير متزامن ولا يعطل المهمة الخلفية
                    this.BeginInvoke(new Action(() => UpdateUI(serverData, players)));
                }
                catch (ObjectDisposedException) { /* تجاهل: الفورم أُغلق أثناء المحاولة */ }
                return;
            }

            // 2. تجميد الرسم (فقط إذا كانت المقابض موجودة)
            if (!lstPlayers.IsHandleCreated || !txtServerInfo.IsHandleCreated) return;

            try
            {
                SendMessage(lstPlayers.Handle, WM_SETREDRAW, false, 0);
                SendMessage(txtServerInfo.Handle, WM_SETREDRAW, false, 0);

                // --- تحديث قائمة اللاعبين ---
                var parsedPlayers = players.Select(p => {
                    var parts = p.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    return parts.Length >= 2 ?
                           new { Ping = parts[0], Name = parts[parts.Length - 1].Trim('"') } : null;
                }).Where(p => p != null).ToList();

                int i = 0;
                for (; i < Math.Min(lstPlayers.Items.Count, parsedPlayers.Count); i++)
                {
                    var item = lstPlayers.Items[i];
                    if (item.Text != parsedPlayers[i].Ping) item.Text = parsedPlayers[i].Ping;
                    if (item.SubItems[1].Text != parsedPlayers[i].Name) item.SubItems[1].Text = parsedPlayers[i].Name;
                }

                for (; i < parsedPlayers.Count; i++)
                {
                    ListViewItem newItem = new ListViewItem(parsedPlayers[i].Ping);
                    newItem.SubItems.Add(parsedPlayers[i].Name);
                    lstPlayers.Items.Add(newItem);
                }

                while (lstPlayers.Items.Count > parsedPlayers.Count)
                    lstPlayers.Items.RemoveAt(lstPlayers.Items.Count - 1);
               ApplyListBoxThemeSafe(txtServerInfo, lstPlayers.BackColor, lstPlayers.ForeColor);


                // --- تحديث معلومات السيرفر ---
                var serverLines = serverData.Select(kv => $"{kv.Key} = {kv.Value}").ToList();
                int j = 0;
                for (; j < Math.Min(txtServerInfo.Items.Count, serverLines.Count); j++)
                {
                    string newLine = serverLines[j];
                    if (txtServerInfo.Items[j].ToString() != newLine)
                        txtServerInfo.Items[j] = newLine;
                }
                for (; j < serverLines.Count; j++) txtServerInfo.Items.Add(serverLines[j]);
                while (txtServerInfo.Items.Count > serverLines.Count) txtServerInfo.Items.RemoveAt(txtServerInfo.Items.Count - 1);
            }
            catch (Exception ex) when (ex is ObjectDisposedException || ex is InvalidOperationException)
            {
                // تم الإغلاق أثناء المعالجة، نخرج بهدوء
            }
            finally
            {
                // 3. فك التجميد (بشرط بقاء الأدوات حية)
                if (!lstPlayers.IsDisposed && lstPlayers.IsHandleCreated)
                {
                    SendMessage(lstPlayers.Handle, WM_SETREDRAW, true, 0);
                    lstPlayers.Invalidate();
                }

                if (!txtServerInfo.IsDisposed && txtServerInfo.IsHandleCreated)
                {
                    SendMessage(txtServerInfo.Handle, WM_SETREDRAW, true, 0);
                    txtServerInfo.Invalidate();
                }

                if (!this.IsDisposed) UpdateSaveButtonState();
            }

        }


        // بناء الرسالة (getstatus) للإرسال
        private byte[] BuildMessage(string cmd)
        {
            byte[] header = new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0x02 };
            byte[] commandBytes = Encoding.UTF8.GetBytes(cmd);
            byte[] result = new byte[header.Length + commandBytes.Length];
            Buffer.BlockCopy(header, 0, result, 0, header.Length);
            Buffer.BlockCopy(commandBytes, 0, result, header.Length, commandBytes.Length);
            return result;
        }

        // تنظيف البيانات المستلمة
        private string CleanData(byte[] data)
        {
            StringBuilder builder = new StringBuilder();
            foreach (byte b in data)
            {
                if ((b >= 32 && b <= 126) || b == 10 || b == 13)
                    builder.Append((char)b);
            }
            return builder.ToString();
        }

        // تحليل بيانات السيرفر
        private Dictionary<string, string> ParseServerInfo(string infoLine)
        {
            string[] parts = infoLine.Split('\\');
            Dictionary<string, string> dict = new Dictionary<string, string>();

            for (int i = 1; i < parts.Length - 1; i += 2)
            {
                dict[parts[i]] = parts[i + 1];
            }

            return dict;
        }

        // الحدث الذي يتم عند تفعيل أو إلغاء تفعيل الـ CheckBox
        private void chkAutoUpdate_CheckedChanged(object sender, EventArgs e)
        {
            // إذا كان الـ CheckBox مفعل، بدء المؤقت لإرسال الطلبات كل ثانية
            if (chkAutoUpdate.Checked)
            {
                updateTimer.Start(); // بدء التحديث التلقائي
            }
            else
            {
                updateTimer.Stop(); // إيقاف التحديث التلقائي
            }
        }

        // دالة لتحديث الـ Label أو StatusStrip لعرض الحالة
        private void UpdateStatusLabel(string message)
        {
            // تأكد من أن يتم التحديث في واجهة المستخدم بشكل آمن
            if (InvokeRequired)
            {
                this.Invoke(new Action<string>(UpdateStatusLabel), message);
            }
            else
            {
                // تحديث Label أو StatusStrip

            }
        }
        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("===== Server Information =====\n");

                foreach (Control ctrl in groupBox1.Controls)
                    if (ctrl is Label lbl && !string.IsNullOrWhiteSpace(lbl.Text))
                        sb.AppendLine(lbl.Text);

                Clipboard.SetText(sb.ToString());
            }
            catch { }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("===== Game Information =====\n");
                sb.AppendLine(label2.Text);
                sb.AppendLine(label6.Text + "\n");

                if (txtServerInfo.Items.Count > 0)
                {
                    sb.AppendLine("== Server Commands ==");
                    foreach (var item in txtServerInfo.Items)
                        sb.AppendLine(item.ToString());
                    sb.AppendLine();
                }

                Clipboard.SetText(sb.ToString());
            }
            catch { }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                if (lstPlayers.Items.Count == 0) return;

                int maxNameLength = lstPlayers.Items.Cast<ListViewItem>()
                    .Select(i => i.SubItems[1].Text.Length)
                    .DefaultIfEmpty(10)
                    .Max();

                StringBuilder sb = new StringBuilder();
                sb.AppendLine("===== Players =====");
                sb.AppendLine($"{"Player Name".PadRight(maxNameLength)} | Ping");
                sb.AppendLine(new string('-', maxNameLength + 8));

                foreach (ListViewItem item in lstPlayers.Items)
                {
                    string name = item.SubItems[1].Text;
                    string ping = item.SubItems[0].Text + " ms";
                    sb.AppendLine($"{name.PadRight(maxNameLength)} | {ping}");
                }

                Clipboard.SetText(sb.ToString());
            }
            catch { }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            try
            {
                string domainText = label7.Text.Replace("Sudomain: ", "").Trim();
                if (!string.IsNullOrWhiteSpace(domainText) && domainText != "Checking..." && domainText != "N/A")
                    Clipboard.SetText(domainText);
            }
            catch { }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            try
            {
                string ipText = label1.Text.Replace("IP: ", "").Trim();
                if (!string.IsNullOrWhiteSpace(ipText) && ipText != "N/A")
                    Clipboard.SetText(ipText);
            }
            catch { }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            try
            {
                string serverName = label2.Text.Replace("Name: ", "").Trim();
                if (!string.IsNullOrWhiteSpace(serverName) && serverName != "Unknown")
                    Clipboard.SetText(serverName);
            }
            catch { }
        }


        private void button9_Click(object sender, EventArgs e)
        {
            try
            {
                // 1. Convert ListBox to Dictionary
                var settings = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                foreach (var item in txtServerInfo.Items)
                {
                    string line = item.ToString();
                    if (line.Contains("="))
                    {
                        var parts = line.Split(new[] { '=' }, 2);
                        settings[parts[0].Trim()] = parts[1].Trim();
                    }
                }

                HashSet<string> writtenKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                StringBuilder sb = new StringBuilder();

                // Function to write a setting with default value and optional comment
                void WriteSetting(string key, string defaultValue, string comment = "")
                {
                    string value = settings.ContainsKey(key) ? settings[key] : defaultValue;
                    if (!string.IsNullOrEmpty(comment)) sb.AppendLine($"// {comment}");

                    if (key.ToLower().Contains("hostname") || key.ToLower().Contains("password") || key.ToLower().Contains("text"))
                        sb.AppendLine($"set {key} \"{value}\"");
                    else
                        sb.AppendLine($"set {key} {value}");

                    sb.AppendLine("");
                    writtenKeys.Add(key);
                }

                // --- Start writing the file ---
                sb.AppendLine("// =====================");
                sb.AppendLine("// server.cfg");
                sb.AppendLine("// =====================\n");

                // Basic server settings
                WriteSetting("sv_hostname", "No Name", "Server name displayed in server browser");
                WriteSetting("g_gametype", "2", "Game type: 1=FFA | 2=TDM | 3=Round | 4=Objective");
                WriteSetting("sv_maxclients", "43", "Maximum number of players");
                WriteSetting("fraglimit", "0", "Score limit for winning the round");
                WriteSetting("timelimit", "0", "Time limit for round (0 = unlimited)");
                WriteSetting("rconpassword", "123", "Remote console password (RCON)");
                WriteSetting("sv_privatepassword", "123", "Private server password");
                WriteSetting("sv_fps", "20", "Server FPS");
                WriteSetting("sv_allowdownload", "0", "Allow clients to download files");
                WriteSetting("sv_floodprotect", "1", "Flood protection for chat messages");
                WriteSetting("g_realismmode", "0", "Enable realism mode");
                WriteSetting("g_teamdamage", "0", "Enable/disable friendly fire");
                WriteSetting("sv_dmspeedmult", "1.1", "Modify player run speed");
                WriteSetting("sv_invulnerabletime", "3", "Invulnerable time after spawn (seconds)");
                WriteSetting("g_healthdrop", "1", "Drop health pack on player death");
                WriteSetting("g_teambalance", "0", "Enable automatic team balancing");
                WriteSetting("countdown", "1", "Enable countdown before round start");
                WriteSetting("countdown_time", "10:00", "Countdown duration");
                WriteSetting("countdown_death_print", "1", "Show messages when players die during countdown");
                WriteSetting("clock", "1", "Show in-game clock on map");
                WriteSetting("mapfix", "1", "Enable map fix/patch");
                WriteSetting("g_inactivespectate", "200", "Time before inactive player becomes spectator (seconds)");
                WriteSetting("g_inactivekick", "0", "Time before inactive player is kicked (0 = disabled)");
                WriteSetting("sv_team_spawn_interval", "0", "Team spawn delay in seconds");
                WriteSetting("dmflags", "67371016", "General game flags");
                WriteSetting("g_forceready", "0", "Force players to be ready");
                WriteSetting("g_forcespawn", "0", "Force automatic respawn");
                WriteSetting("g_teamkillwarn", "2", "Team kills before warning");
                WriteSetting("g_teamkillkick", "3", "Team kills before kick");
                WriteSetting("g_teamswitchdelay", "30", "Delay before switching teams");
                WriteSetting("g_allowjointime", "30", "Allowed join time after round starts");
                WriteSetting("sv_privateclients", "11", "Number of private slots with password");
                WriteSetting("sv_keywords", "", "Server keywords for browser filtering");
                WriteSetting("net_port", "12203", "Server port");
                WriteSetting("sv_precache", "1", "Enable resource precaching");
                WriteSetting("cheats", "0", "Allow cheats");
                WriteSetting("sv_runspeed", "293", "Player run speed");

                // --- Map Rotation (x1, x2, ...) ---
                sb.AppendLine("// --- Map Rotation ---");
                if (settings.ContainsKey("sv_maplist"))
                {
                    string[] maps = settings["sv_maplist"].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < maps.Length; i++)
                    {
                        int next = (i + 1) % maps.Length;
                        sb.AppendLine($"set x{i + 1} \"map {maps[i]}; set nextmap vstr x{next + 1}\"  // Map {i + 1}");
                    }
                    writtenKeys.Add("sv_maplist");
                }
                sb.AppendLine("seta sv_maplist \"\"  // Reset maplist");
                sb.AppendLine("vstr x1  // Start first map\n");

                // --- Additional/Custom Settings from ListBox ---
                sb.AppendLine("// --- Additional & Custom Settings");
                foreach (var setting in settings)
                {
                    if (!writtenKeys.Contains(setting.Key))
                    {
                        sb.AppendLine($"set {setting.Key} \"{setting.Value}\"");
                        sb.AppendLine("");
                    }
                }

                sb.AppendLine("// ===================== End of Config =====================");

                // Save file
                using (SaveFileDialog sfd = new SaveFileDialog { Filter = "Config files|*.cfg", FileName = "server.cfg" })
                {
                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        File.WriteAllText(sfd.FileName, sb.ToString(), Encoding.Default);
                        MessageBox.Show("Config file saved successfully with comments!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                StringBuilder sb = new StringBuilder();

                // ===== Server Information =====
                sb.AppendLine("===== Server Information =====\n");
                foreach (Control ctrl in groupBox1.Controls)
                {
                    if (ctrl is Label lbl && !string.IsNullOrWhiteSpace(lbl.Text))
                        sb.AppendLine(lbl.Text);
                }

                // ===== Game Information =====
                sb.AppendLine("\n===== Game Information =====\n");
                sb.AppendLine(label2.Text);
                sb.AppendLine(label6.Text + "\n");

                if (txtServerInfo.Items.Count > 0)
                {
                    sb.AppendLine("== Server Commands ==");
                    foreach (var item in txtServerInfo.Items)
                        sb.AppendLine(item.ToString());
                    sb.AppendLine();
                }

                // ===== Players =====
                if (lstPlayers.Items.Count > 0)
                {
                    int maxNameLength = lstPlayers.Items.Cast<ListViewItem>()
                        .Select(i => i.SubItems[1].Text.Length)
                        .DefaultIfEmpty(10)
                        .Max();

                    sb.AppendLine("===== Players =====");
                    sb.AppendLine($"{"Player Name".PadRight(maxNameLength)} | Ping");
                    sb.AppendLine(new string('-', maxNameLength + 8));

                    foreach (ListViewItem item in lstPlayers.Items)
                    {
                        string name = item.SubItems[1].Text;
                        string ping = item.SubItems[0].Text + " ms";
                        sb.AppendLine($"{name.PadRight(maxNameLength)} | {ping}");
                    }
                }

                // ===== Domain =====
                string domainText = label7.Text.Replace("Sudomain: ", "").Trim();
                if (!string.IsNullOrWhiteSpace(domainText) && domainText != "Checking..." && domainText != "N/A")
                    sb.AppendLine("\nDomain: " + domainText);

                // ===== IP =====
                string ipText = label1.Text.Replace("IP: ", "").Trim();
                if (!string.IsNullOrWhiteSpace(ipText) && ipText != "N/A")
                    sb.AppendLine("IP: " + ipText);

                // ===== Server Name =====
                string serverName = label2.Text.Replace("Name: ", "").Trim();
                if (!string.IsNullOrWhiteSpace(serverName) && serverName != "Unknown")
                    sb.AppendLine("Server Name: " + serverName);

                // نسخ كل شيء للحافظة
                Clipboard.SetText(sb.ToString());

                MessageBox.Show("All information copied to clipboard!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error copying information: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                StringBuilder sb = new StringBuilder();

                // ===== Server Information =====
                sb.AppendLine("===== Server Information =====\n");
                foreach (Control ctrl in groupBox1.Controls)
                {
                    if (ctrl is Label lbl && !string.IsNullOrWhiteSpace(lbl.Text))
                        sb.AppendLine(lbl.Text);
                }

                // ===== Game Information =====
                sb.AppendLine("\n===== Game Information =====\n");
                sb.AppendLine(label2.Text);
                sb.AppendLine(label6.Text + "\n");

                if (txtServerInfo.Items.Count > 0)
                {
                    sb.AppendLine("== Server Commands ==");
                    foreach (var item in txtServerInfo.Items)
                        sb.AppendLine(item.ToString());
                    sb.AppendLine();
                }

                // ===== Players =====
                if (lstPlayers.Items.Count > 0)
                {
                    int maxNameLength = lstPlayers.Items.Cast<ListViewItem>()
                        .Select(i => i.SubItems[1].Text.Length)
                        .DefaultIfEmpty(10)
                        .Max();

                    sb.AppendLine("===== Players =====");
                    sb.AppendLine($"{"Player Name".PadRight(maxNameLength)} | Ping");
                    sb.AppendLine(new string('-', maxNameLength + 8));

                    foreach (ListViewItem item in lstPlayers.Items)
                    {
                        string name = item.SubItems[1].Text;
                        string ping = item.SubItems[0].Text + " ms";
                        sb.AppendLine($"{name.PadRight(maxNameLength)} | {ping}");
                    }
                }

                // ===== Domain =====
                string domainText = label7.Text.Replace("Sudomain: ", "").Trim();
                if (!string.IsNullOrWhiteSpace(domainText) && domainText != "Checking..." && domainText != "N/A")
                    sb.AppendLine("\nDomain: " + domainText);

                // ===== IP =====
                string ipText = label1.Text.Replace("IP: ", "").Trim();
                if (!string.IsNullOrWhiteSpace(ipText) && ipText != "N/A")
                    sb.AppendLine("IP: " + ipText);

                // ===== Server Name =====
                string serverName = label2.Text.Replace("Name: ", "").Trim();
                if (!string.IsNullOrWhiteSpace(serverName) && serverName != "Unknown")
                    sb.AppendLine("Server Name: " + serverName);

                // Save As file
                using (SaveFileDialog sfd = new SaveFileDialog())
                {
                    sfd.Filter = "Text Files|*.txt|All Files|*.*";
                    sfd.FileName = "ServerInfo.txt";

                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        File.WriteAllText(sfd.FileName, sb.ToString(), Encoding.UTF8);
                        MessageBox.Show("Information saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving information: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void status_FormClosing(object sender, FormClosingEventArgs e)
        {
            isClosing = true; // تفعيل وضع الإغلاق
            updateTimer?.Stop(); // إيقاف المؤقت فوراً
        }
    }
}
