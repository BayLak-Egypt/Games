using HtmlAgilityPack;
using Microsoft.VisualBasic;
using QuakeRcon;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace mohaa_server_tool
{

    public partial class MainForm : Form
    {
        public AppConfig CurrentSettings = new AppConfig();
        private List<ListViewItem> allServers = new List<ListViewItem>();
        private System.Windows.Forms.Timer refreshTimer;
        private string addedServersFile = "added_servers.txt";
        private HashSet<string> pinnedServers = new HashSet<string>();
        // --- المتغيرات الخاصة بالـ RPG ---
        private System.Windows.Forms.Timer rpgTimer = new System.Windows.Forms.Timer();
        private int hue = 0;
        private System.Windows.Forms.Timer pingTimer; // تايمر منفصل للبينج فقط
        private string serversText = "Servers";
        private string blockedText = "Blocked";

        // غير السطر ده خليه public
        public Dictionary<string, string> columnTexts = new Dictionary<string, string>();
        public MainForm()
        {
           
            InitializeComponent();
            InitializeListView();
            LoadAddedServersFromFile();
            LoadAll(); // تحميل السيرفرات من الموقع

            // إعداد Timer للتحديث التلقائي
            refreshTimer = new System.Windows.Forms.Timer();
            refreshTimer.Interval = 6000; // كل 1000 مللي ثانية = 1 ثانية
            refreshTimer.Tick += RefreshTimer_Tick;
            refreshTimer.Start(); // بدء التشغيل


            pingTimer = new System.Windows.Forms.Timer();
            pingTimer.Interval = 1500; // تحديث البينج كل ثانية ونصف ليكون سريعاً
            pingTimer.Tick += PingTimer_Tick;
            pingTimer.Start();
            this.Load += (s, e) => AdjustColumnWidths(); // عند الفتح
            listView1.Resize += (s, e) => AdjustColumnWidths(); // ع
            LoadConfiguration();

        }
        private void LoadConfiguration()
        {
            string configPath = Path.Combine(Application.StartupPath, "config.json");
            try
            {
                if (File.Exists(configPath))
                {
                    string json = File.ReadAllText(configPath);
                    this.CurrentSettings = JsonSerializer.Deserialize<AppConfig>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                }
            }
            catch { this.CurrentSettings = new AppConfig(); }

            if (this.CurrentSettings != null)
            {
                searchTextBox.Visible = CurrentSettings.ShowSearch;
                label1.Visible = CurrentSettings.ShowSearch;

                string themeName = string.IsNullOrEmpty(CurrentSettings.SelectedTheme) ? "default" : CurrentSettings.SelectedTheme;
                ThemeManager.ApplyTheme(this, listView1, themeName);

                listView1.BeginUpdate();
                if (rpgTimer.Enabled) rpgTimer.Stop();
                foreach (ListViewItem item in listView1.Items) ApplyRowColor(item);

                if (CurrentSettings.ShowColors && CurrentSettings.RpgEnabled) StartRPG();
                listView1.EndUpdate();

                // هذه الدالة الآن هي المسؤولة الوحيدة عن الأعمدة (إضافة/حذف + لغة)
                ApplyLanguage(CurrentSettings.SelectedLanguage, "main");
            }
        }
        // داخل MainForm.cs
        public void ApplyLanguage(string lang, string groupName = "main", Form targetForm = null)
        {
            Form frm = targetForm ?? this;
            string langFile = Path.Combine(Application.StartupPath, "language", lang + ".js");
            if (!File.Exists(langFile)) return;

            try
            {
                string json = File.ReadAllText(langFile);
                LanguageFile langObj = JsonSerializer.Deserialize<LanguageFile>(json);
                if (langObj == null) return;

                Dictionary<string, string> texts = (groupName.Equals("settings", StringComparison.OrdinalIgnoreCase))
                    ? langObj.settings : langObj.main;

                if (texts == null) return;

                // تحديث القاموس العام للأعمدة
                this.columnTexts = texts.Where(kv => kv.Key.StartsWith("column_")).ToDictionary(kv => kv.Key, kv => kv.Value);

                foreach (var kv in texts)
                {
                    if (frm == this)
                    {
                        if (kv.Key == "button1") { serversText = kv.Value; continue; }
                        if (kv.Key == "button2") { blockedText = kv.Value; continue; }
                    }
                    Control ctl = frm.Controls.Find(kv.Key, true).FirstOrDefault();
                    if (ctl != null) ctl.Text = kv.Value;
                }

                if (frm == this)
                {
                    UpdateServersCount();
                    UpdateBlockedCount();

                    // هنا الاستدعاء السليم للدالة المستقلة
                    UpdateColumns(listView1);
                    if (blockedListView != null) UpdateColumns(blockedListView);

                    AdjustColumnWidths();
                    UpdateMenuLanguage(texts);
                }
            }
            catch (Exception ex) { Console.WriteLine("ApplyLanguage Error: " + ex.Message); }
        }

        // دالة مساعدة للمنيو (منفصلة)
        private void UpdateMenuLanguage(Dictionary<string, string> texts)
        {
            Action<ToolStripItemCollection> processItems = null;
            processItems = (items) => {
                foreach (ToolStripItem item in items)
                {
                    if (texts.ContainsKey(item.Name)) item.Text = texts[item.Name];
                    if (item is ToolStripMenuItem mi && mi.DropDownItems.Count > 0) processItems(mi.DropDownItems);
                }
            };
            if (this.MainMenuStrip != null) processItems(this.MainMenuStrip.Items);
            if (this.contextMenuStrip1 != null) processItems(this.contextMenuStrip1.Items);
        }
        public void UpdateColumns(ListView lv)
        {
            if (lv == null) return;

            bool pingEnabled = CurrentSettings?.PingEnabled ?? false;

            // البحث عن العمود بالاسم البرمجي
            ColumnHeader pingCol = lv.Columns.Cast<ColumnHeader>().FirstOrDefault(c => c.Name == "PingCol");

            if (pingEnabled)
            {
                // إذا كان مفعلاً وغير موجود -> نضيفه
                if (pingCol == null)
                {
                    pingCol = new ColumnHeader { Name = "PingCol", Width = 70, TextAlign = HorizontalAlignment.Center };
                    lv.Columns.Add(pingCol);
                }
                // تعيين النص من القاموس المترجم
                if (columnTexts != null && columnTexts.ContainsKey("column_8"))
                    pingCol.Text = columnTexts["column_8"];
                else
                    pingCol.Text = "Ping";
            }
            else
            {
                // إذا كان معطلاً وموجوداً -> نحذفه فوراً
                if (pingCol != null) lv.Columns.Remove(pingCol);
            }

            // ترجمة بقية الأعمدة بناءً على الموجود فعلياً
            for (int i = 0; i < lv.Columns.Count; i++)
            {
                if (lv.Columns[i].Name == "PingCol") continue;
                string key = $"column_{i}";
                if (columnTexts != null && columnTexts.ContainsKey(key))
                    lv.Columns[i].Text = columnTexts[key];
            }
        }
        // --- دالة تشغيل الـ RPG ---
        public void StartRPG()
        {
            // تفعيل خاصية منع الرعشة برمجياً لضمان النعومة
            typeof(Control).GetProperty("DoubleBuffered", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                           ?.SetValue(listView1, true, null);

            rpgTimer.Interval = 150; // سرعة التغيير (تجنب السرعات العالية جداً لمنع الرعشة)
            rpgTimer.Tick -= RpgTimer_Tick; // التأكد من عدم تكرار الاشتراك في الحدث
            rpgTimer.Tick += RpgTimer_Tick;
            rpgTimer.Start();
        }
        // 1. دالة تحديث الألوان - قمت بتعديلها لتعتمد على لون نص الثيم
        public void UpdateListColors()
        {
            if (listView1.InvokeRequired)
            {
                listView1.BeginInvoke(new Action(UpdateListColors));
                return;
            }

            listView1.BeginUpdate();

            // ربط ألوان القائمة بألوان الفورم (الثيم)
            listView1.ForeColor = this.ForeColor;
            listView1.BackColor = listView1.BackColor;

            foreach (ListViewItem item in listView1.Items)
            {
                ApplyRowColor(item);
            }

            listView1.EndUpdate();
            listView1.Refresh();
        }

        private void ApplyRowColor(ListViewItem item)
        {
            item.UseItemStyleForSubItems = true;
            string ipPort = item.SubItems.Count > 3 ? item.SubItems[3].Text : "";

            // جلب الإعدادات الحالية
            bool showColors = CurrentSettings?.ShowColors ?? true;
            bool rpgEnabled = CurrentSettings?.RpgEnabled ?? false;
            bool isRpgRunning = (rpgTimer != null && rpgTimer.Enabled);

            // التحقق من حالة السيرفر (مثبت أو مضاف)
            bool isPinned = pinnedServers.Contains(ipPort) || (item.Tag?.ToString() == "Pinned");
            bool isAddedServer = addedServers.Any(s => $"{s.IP}:{s.Port}" == ipPort);

            // --- الحالة 1: المستخدم أغلق خيار الألوان تماماً ---
            if (!showColors)
            {
                item.BackColor = listView1.BackColor;
                item.ForeColor = listView1.ForeColor;
                return;
            }

            // --- الحالة 2: السيرفر مثبت (Pinned) ---
            if (isPinned)
            {
                // إذا كان الـ RPG يعمل، نترك التايمر يلون (لا نفعل شيئاً هنا)
                if (isRpgRunning && rpgEnabled) return;

                // إذا كان الـ RPG متوقف، نعطيه اللون الأزرق المعتاد للمثبتات
                item.BackColor = Color.LightBlue;
                item.ForeColor = Color.Black;
                return;
            }

            // --- الحالة 3: السيرفر مضاف (Added) وليس مثبت ---
            if (isAddedServer)
            {
                string status = item.SubItems[0].Text;
                if (status == "Offline")
                {
                    item.BackColor = Color.Red;
                    item.ForeColor = Color.Black;
                }
                else
                {
                    // هنا التعديل: نلون بالأخضر فقط إذا لم يكن الـ RPG يعمل على تلوين الصفوف
                    item.BackColor = Color.LightGreen;
                    item.ForeColor = Color.Black;
                }
            }
            else
            {
                // سيرفر عادي (ليس مضافاً ولا مثبتاً)
                item.BackColor = listView1.BackColor;
                item.ForeColor = listView1.ForeColor;
            }
        }

        // تعديل دالة StopRPG للتأكد من مسح ألوان الـ RPG فوراً
        public void StopRPG()
        {
            if (rpgTimer != null) rpgTimer.Stop();

            // إعادة ضبط ألوان كل الصفوف فور التوقف لضمان اختفاء ألوان قوس قزح أو الأخضر العالق
            listView1.BeginUpdate();
            foreach (ListViewItem item in listView1.Items)
            {
                ApplyRowColor(item);
            }
            listView1.EndUpdate();
        }
        public void ApplyUniformThemeColor()
        {
            listView1.BeginUpdate();
            foreach (ListViewItem item in listView1.Items)
            {
                item.BackColor = listView1.BackColor;
                item.ForeColor = listView1.ForeColor;
            }
            listView1.EndUpdate();
        }

       
        // --- منطق تغيير الألوان كل تكة ---
        private void RpgTimer_Tick(object sender, EventArgs e)
        {
            bool isRpgAllowed = false;

            if (CurrentSettings != null)
            {
                isRpgAllowed = CurrentSettings.ShowColors && CurrentSettings.RpgEnabled;
            }
            else
            {
                var settingForm = (settings)Application.OpenForms["settings"];
                if (settingForm != null)
                {
                    isRpgAllowed = settingForm.checkBox3.Checked && settingForm.checkBox4.Checked;
                }
            }

            if (!isRpgAllowed) return;

            hue = (hue + 10) % 360;
            Color rainbowColor = ColorFromHSV(hue, 0.5, 1.0);

            listView1.BeginUpdate();

            foreach (ListViewItem item in listView1.Items)
            {
                string ipPort = item.SubItems.Count > 3 ? item.SubItems[3].Text : "";

                if (pinnedServers.Contains(ipPort) || (item.Tag != null && item.Tag.ToString() == "Pinned"))
                {
                    item.BackColor = rainbowColor;
                    item.ForeColor = Color.Black;
                }
                else
                {
                    ApplyRowColor(item);
                }
            }

            listView1.EndUpdate();
        }

        // --- دالة تحويل درجات HSV إلى ألوان RGB ---
        public static Color ColorFromHSV(double hue, double saturation, double value)
        {
            int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
            double f = hue / 60 - Math.Floor(hue / 60);
            value = value * 255;
            int v = Convert.ToInt32(value);
            int p = Convert.ToInt32(value * (1 - saturation));
            int q = Convert.ToInt32(value * (1 - f * saturation));
            int t = Convert.ToInt32(value * (1 - (1 - f) * saturation));

            if (hi == 0) return Color.FromArgb(255, v, t, p);
            if (hi == 1) return Color.FromArgb(255, q, v, p);
            if (hi == 2) return Color.FromArgb(255, p, v, t);
            if (hi == 3) return Color.FromArgb(255, p, q, v);
            if (hi == 4) return Color.FromArgb(255, t, p, v);
            return Color.FromArgb(255, v, p, q);
        }

        public async Task<string> GetPingMs(string ipPort)
        {
            try
            {
                string ip = ipPort.Contains(":") ? ipPort.Split(':')[0] : ipPort;
                using (var pinger = new System.Net.NetworkInformation.Ping())
                {
                    // مهلة انتظار قصيرة جداً للسرعة
                    var reply = await pinger.SendPingAsync(ip, 500);
                    if (reply.Status == System.Net.NetworkInformation.IPStatus.Success)
                        return reply.RoundtripTime.ToString() + "ms";
                }
            }
            catch { }
            return "999 ms";
        }
        private void ListView1_Resize(object sender, EventArgs e)
        {
            ListView lv = (ListView)sender;
            int totalWidth = lv.ClientSize.Width; // المساحة المتاحة فعلياً داخل القائمة

            // إذا أردت توزيع المساحة بنسب مئوية (مثال):
            // نفترض أن لديك 8 أعمدة
            if (lv.Columns.Count >= 8)
            {
                lv.Columns[0].Width = (int)(totalWidth * 0.10); // Status 10%
                lv.Columns[2].Width = (int)(totalWidth * 0.30); // Name 30%
                lv.Columns[3].Width = (int)(totalWidth * 0.20); // IP 20%
                                                                // الباقي يتم توزيعه حسب رغبتك...
            }
        }

        private void AdjustColumnWidths()
        {
            // تأكد من أن الاتنين موجودين
            if (listView1 != null) AdjustListViewColumns(listView1);
            if (blockedListView != null) AdjustListViewColumns(blockedListView);
        }

        // دالة عامة لضبط الأعمدة لأي ListView
        public void AdjustListViewColumns(ListView lv)
        {
            if (lv == null || lv.Columns.Count == 0) return;

            // منع الرعشة أثناء إعادة ضبط الأحجام
            lv.BeginUpdate();

            int totalWidth = lv.ClientSize.Width;

            // 1. تحديد حالة البينج
            var pingCol = lv.Columns.Cast<ColumnHeader>().FirstOrDefault(c => c.Name == "PingCol");
            bool isPingEnabled = CurrentSettings?.PingEnabled ?? false;

            // 2. توزيع المساحات الثابتة (لأول 7 أعمدة)
            // تأكدنا من استخدام IndexOf لضمان عدم حدوث Error لو نقص عدد الأعمدة
            if (lv.Columns.Count >= 1) lv.Columns[0].Width = (int)(totalWidth * 0.08); // Status
            if (lv.Columns.Count >= 2) lv.Columns[1].Width = (int)(totalWidth * 0.08); // Version
            if (lv.Columns.Count >= 3) lv.Columns[2].Width = (int)(totalWidth * 0.25); // Name
            if (lv.Columns.Count >= 4) lv.Columns[3].Width = (int)(totalWidth * 0.15); // IP
            if (lv.Columns.Count >= 5) lv.Columns[4].Width = (int)(totalWidth * 0.10); // Type
            if (lv.Columns.Count >= 6) lv.Columns[5].Width = (int)(totalWidth * 0.10); // Map
            if (lv.Columns.Count >= 7) lv.Columns[6].Width = (int)(totalWidth * 0.12); // Country

            // 3. المنطق الذكي لعمود اللاعبين والبينج
            if (isPingEnabled && pingCol != null)
            {
                // إذا كان البينج مفعل:
                if (lv.Columns.Count > 7) lv.Columns[7].Width = (int)(totalWidth * 0.07); // Players
                pingCol.Width = -2; // البينج يملأ الباقي
            }
            else
            {
                // إذا كان البينج معطل (مهم جداً للترجمة العربية):
                if (pingCol != null) pingCol.Width = 0; // إخفاء قسري لعمود البينج

                // عمود اللاعبين (أو آخر عمود مرئي) يملأ المساحة المتبقية
                if (lv.Columns.Count > 7)
                {
                    lv.Columns[7].Width = -2;
                }
                else if (lv.Columns.Count == 7)
                {
                    lv.Columns[6].Width = -2;
                }
            }

            lv.EndUpdate();
        }
        private void PingTimer_Tick(object sender, EventArgs e)
        {
            bool pingEnabled = CurrentSettings?.PingEnabled ?? false;
            if (!pingEnabled) return;

            foreach (ListViewItem item in listView1.Items)
            {
                // تأكد أن السيرفر له IP صالح
                if (item.SubItems.Count < 4) continue;
                string ipPort = item.SubItems[3].Text;

                Task.Run(async () => {
                    string result = await GetPingMs(ipPort);

                    if (this.IsHandleCreated)
                    {
                        this.BeginInvoke(new Action(() => {
                            // التعديل هنا: التأكد من وجود عمود كافٍ قبل التحديث
                            // بنزود عدد الـ SubItems لو قل عن 9 (عشان نوصل لـ index 8)
                            while (item.SubItems.Count <= 8)
                            {
                                item.SubItems.Add("");
                            }

                            if (item.SubItems[8].Text != result)
                            {
                                item.SubItems[8].Text = result;

                                // تلوين البنج
                                if (result.Contains("ms"))
                                {
                                    if (int.TryParse(result.Replace("ms", "").Trim(), out int ms))
                                    {
                                        item.SubItems[8].ForeColor = ms < 100 ? Color.LimeGreen : (ms < 200 ? Color.Gold : Color.Red);
                                    }
                                }
                            }
                        }));
                    }
                });
            }
        }

        private void RefreshTimer_Tick(object sender, EventArgs e)
        {
            // شغل الريفرش على الـ UI thread
            if (listView1.InvokeRequired)
            {
                listView1.BeginInvoke(new Action(RefreshAddedServers));
            }
            else
            {
                RefreshAddedServers();
            }
        }

        private async void LoadAll()
        {
            await LoadData();             // ننتظر تحميل السيرفرات
            LoadBlockedServersByIP();     // بعد كده نفلتر المحظور
            LoadPinnedServersByIP();
            UpdateServersCount();
            UpdateBlockedCount();
        }

        private void UpdateServersCount()
        {
            // تحديث نص button1
            button1.Text = $"{serversText} ({listView1.Items.Count})";

            // بعد تحديث النص، تحريك button2
            UpdateButton2Position();
        }

        private void UpdateBlockedCount()
        {
            // تحديث نص button2
            button2.Text = $"{blockedText} ({blockedListView.Items.Count})";

            // بعد تحديث النص، تحريك button2
            UpdateButton2Position();
        }
        private void UpdateButton2Position()
        {
            Size textSize = TextRenderer.MeasureText(button1.Text, button1.Font);
            int spacing = 15;
            button2.Left = button1.Left + textSize.Width + spacing;
        }

        private void InitializeListView()
        {
 
        }
        private void LoadPinnedServersByIP()
        {
            string filePath = "pinned_ips.txt";
            if (!File.Exists(filePath)) return;

            pinnedServers = new HashSet<string>(File.ReadAllLines(filePath));

            foreach (ListViewItem item in listView1.Items)
            {
                string ipPort = item.SubItems[3].Text;
                if (pinnedServers.Contains(ipPort))
                {
                    item.Tag = "Pinned"; // Tag يوضح أن السيرفر مثبت

                    if (CurrentSettings != null && !CurrentSettings.ShowColors)
                    {
                        // ShowColors = false → طبق لون موحد حسب الثيم
                        Color uniformColor = listView1.BackColor; // اللون الموحد حسب الثيم
                        item.BackColor = uniformColor;
                        item.ForeColor = listView1.ForeColor;
                    }
                    else
                    {
                        // ShowColors = true → السيرفرات المثبتة تأخذ لونها الخاص
                        // RPG يظل له الأولوية في تلوينها
                        if (rpgTimer != null && rpgTimer.Enabled)
                        {
                            // اترك التايمر يلونها
                            continue;
                        }
                        else
                        {
                            item.BackColor = Color.LightBlue;
                            item.ForeColor = Color.Black;
                        }
                    }
                }
            }
        }


        private void LoadBlockedServersByIP()
        {
            string filePath = "blocked_ips.txt";
            if (!File.Exists(filePath)) return;

            var blockedIPs = new HashSet<string>(File.ReadAllLines(filePath));


            for (int i = listView1.Items.Count - 1; i >= 0; i--) // نعكس التكرار عشان نحذف بأمان
            {
                var item = listView1.Items[i];
                string ip = item.SubItems[3].Text;

                if (blockedIPs.Contains(ip))
                {
                    var clonedItem = (ListViewItem)item.Clone();
                    clonedItem.BackColor = Color.Red;
                    clonedItem.ForeColor = Color.White;

                    blockedListView.Items.Add(clonedItem);
                    listView1.Items.RemoveAt(i);
                }
            }
        }


        private async Task LoadData()
        {
            try
            {
                var url = "https://www.mohaaservers.tk/";
                using (var httpClient = new HttpClient())
                {
                    var htmlContent = await httpClient.GetStringAsync(url);
                    var htmlDocument = new HtmlAgilityPack.HtmlDocument();
                    htmlDocument.LoadHtml(htmlContent);

                    var classesToRemove = new[] { "expander players", "tablesorter-header-inner", "long", "centerTitle", "tablesorter-headerRow" };
                    foreach (var className in classesToRemove)
                    {
                        var nodes = htmlDocument.DocumentNode.SelectNodes($"//*[contains(@class, '{className}')]");
                        nodes?.ToList().ForEach(node => node.Remove());
                    }

                    DisplayInListView(htmlDocument.DocumentNode);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DisplayInListView(HtmlNode node)
        {
            var table = node.SelectSingleNode("//table");
            if (table == null) return;

            var rows = table.SelectNodes(".//tr");
            if (rows == null) return;

            // البدء في تحديث القائمة برمجياً لمنع الرعشة وتحسين الأداء
            listView1.Invoke(new Action(() =>
            {
                listView1.BeginUpdate();

                foreach (var row in rows)
                {
                    var cells = row.SelectNodes("td");
                    if (cells == null || cells.Count < 8) continue;

                    string ipPort = cells[4].InnerText.Trim();

                    // البحث عن السيرفر في القائمة الحالية
                    var existingItem = listView1.Items.Cast<ListViewItem>()
                        .FirstOrDefault(x => x.SubItems.Count > 3 && x.SubItems[3].Text == ipPort);

                    if (existingItem != null)
                    {
                        // تحديث البيانات
                        existingItem.SubItems[0].Text = cells[0].InnerText.Trim();
                        existingItem.SubItems[1].Text = cells[1].InnerText.Trim();
                        existingItem.SubItems[2].Text = cells[3].InnerText.Trim();
                        existingItem.SubItems[4].Text = cells[5].InnerText.Trim();
                        existingItem.SubItems[5].Text = cells[6].InnerText.Trim();
                        existingItem.SubItems[7].Text = cells[7].InnerText.Trim();

                        // تطبيق لون الثيم على العناصر المحدثة
                        ApplyRowColor(existingItem);
                    }
                    else
                    {
                        // إنشاء سيرفر جديد
                        var item = new ListViewItem(cells[0].InnerText.Trim());
                        item.SubItems.Add(cells[1].InnerText.Trim()); // Version
                        item.SubItems.Add(cells[3].InnerText.Trim()); // Name
                        item.SubItems.Add(cells[4].InnerText.Trim()); // IP:Port
                        item.SubItems.Add(cells[5].InnerText.Trim()); // Game Type
                        item.SubItems.Add(cells[6].InnerText.Trim()); // Map
                        item.SubItems.Add(cells[2].InnerText.Trim()); // Country (حسب ترتيبك)
                        item.SubItems.Add(cells[7].InnerText.Trim()); // Players

                        // إضافة السيرفر للقائمة
                        listView1.Items.Add(item);

                        // إضافة نسخة لـ allServers للبحث لاحقاً
                        allServers.Add((ListViewItem)item.Clone());

                        // تطبيق لون الثيم فوراً عند الإضافة
                        ApplyRowColor(item);
                    }
                }

                listView1.EndUpdate();
            }));
        }



        private async void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            await LoadData();
            LoadPinnedServersByIP();
            RefreshAddedServers();
        }


        private void statusToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                var selectedItem = listView1.SelectedItems[0];
                if (selectedItem.SubItems.Count > 3)
                {
                    var serverIP = selectedItem.SubItems[3].Text;
                    var serverName = selectedItem.SubItems[2].Text;
                    var serverMap = selectedItem.SubItems[5].Text;

                    if (!string.IsNullOrWhiteSpace(serverIP))
                    {
                        status menu = new status();
                        menu.IP = serverIP;
                        menu.ServerName = serverName;
                        menu.ServerMap = serverMap;

                        menu.Show();
                    }
                    else
                    {
                        MessageBox.Show("The selected item does not contain a valid IP address.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else
                {
                    MessageBox.Show("The selected item does not have enough details.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                MessageBox.Show("No item selected.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void scanFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                // استخدام Loop عشان لو اخترت كذا سيرفر يفتح نافذة لكل واحد
                foreach (ListViewItem selectedItem in listView1.SelectedItems)
                {
                    if (selectedItem.SubItems.Count > 3)
                    {
                        var serverIP = selectedItem.SubItems[3].Text;

                        // إنشاء نسخة جديدة من نافذة عرض الملفات
                        ViewFiles menu = new ViewFiles();
                        menu.IP = serverIP;

                        // تغيير العنوان عشان تعرف كل نافذة بتاعة أنهي IP
                        menu.Text = "Scanning Files - " + serverIP;

                        // استخدام Show عشان تفتح نوافذ متعددة
                        menu.Show();
                    }
                    else
                    {
                        MessageBox.Show("Selected item does not contain an IP address.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
            else
            {
                MessageBox.Show("No item selected.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void rconpasswordHackToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                // لو عايز تفتح نافذة لكل عنصر انت مختاره في الـ ListView
                foreach (ListViewItem selectedItem in listView1.SelectedItems)
                {
                    if (selectedItem.SubItems.Count > 3) // التأكد إن الـ Index موجود
                    {
                        var serverIP = selectedItem.SubItems[3].Text;

                        // إنشاء نسخة جديدة من الفورم
                        Form1 form2 = new Form1();
                        form2.IP = serverIP;

                        // استخدام Show بدلاً من ShowDialog لفتح نوافذ متعددة غير مقيدة
                        form2.Show();
                    }
                    else
                    {
                        MessageBox.Show("Selected item does not contain an IP address.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
            else
            {
                MessageBox.Show("No item selected.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // إضافة معالج startToolStripMenuItem_Click
        private void startToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // 1. التحقق من اختيار سيرفر من القائمة
            if (listView1.SelectedItems.Count == 0) return;

            var selectedItem = listView1.SelectedItems[0];
            string serverIP = selectedItem.SubItems[3].Text.Trim();
            string exePath = "MOHAA.exe";

            // --- الإضافة المطلوبة: التحقق إذا كانت اللعبة مفتوحة بالفعل ---
            string processName = "MOHAA"; // اسم ملف اللعبة بدون .exe
            if (System.Diagnostics.Process.GetProcessesByName(processName).Length > 0)
            {
                MessageBox.Show("The game is already running! Close it first.",
                                "Game Running", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // توقف هنا ولا تفتح أي نافذة أو تشغل اللعبة
            }
            // -------------------------------------------------------

            // 2. التحقق من الإعدادات
            bool showMenu = CurrentSettings?.ShowOptionInStart ?? true;

            if (showMenu)
            {
                // البحث عن نسخة "Start" موجودة في الذاكرة (حتى لو مخفية)
                Start existingMenu = Application.OpenForms.OfType<Start>().FirstOrDefault();

                if (existingMenu != null)
                {
                    // لو النافذة موجودة ومخفية (Hidden)، نقوم بإظهارها وتحديث الـ IP
                    existingMenu.IP = serverIP;
                    existingMenu.Show();
                    existingMenu.BringToFront();
                    if (existingMenu.WindowState == FormWindowState.Minimized)
                        existingMenu.WindowState = FormWindowState.Normal;
                }
                else
                {
                    // لو لم تكن مفتوحة نهائياً، ننشئ نسخة جديدة
                    Start menu = new Start();
                    menu.IP = serverIP;
                    menu.Show();
                }
            }
            else
            {
                // 3. التشغيل المباشر للعبة (في حال تم تعطيل المنيو من الإعدادات)
                if (!System.IO.File.Exists(exePath))
                {
                    MessageBox.Show($"Could not find {exePath}!", "File Not Found",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                try
                {
                    System.Diagnostics.Process.Start(exePath, $"+connect {serverIP}");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}");
                }
            }
        }
        private Tuple<string, string, string, string, string> GetServerStatus(string ip, int port)
        {
            UdpClient sock = null;
            try
            {
                sock = new UdpClient();
                sock.Client.ReceiveTimeout = 800;

                // بناء الباكت getstatus
                byte[] header = new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0x02 };
                byte[] cmd = Encoding.ASCII.GetBytes("getstatus");
                byte[] packet = new byte[header.Length + cmd.Length];
                Array.Copy(header, 0, packet, 0, header.Length);
                Array.Copy(cmd, 0, packet, header.Length, cmd.Length);

                // إرسال الباكت للسيرفر
                sock.Send(packet, packet.Length, ip, port);

                // استقبال الرد
                IPEndPoint ep = null;
                byte[] response = sock.Receive(ref ep);

                // تنظيف البيانات
                string clean = Encoding.UTF8.GetString(
                    response.Where(b => (b >= 32 && b <= 126) || b == 10).ToArray()
                );

                var lines = clean.Split('\n');
                if (lines.Length < 2)
                    return Tuple.Create("Offline", "", "", "Unknown", "0/0");

                string infoLine = lines[1];
                int playersCount = lines.Skip(2).Count(l => !string.IsNullOrWhiteSpace(l));

                var parts = infoLine.Split('\\');

                // دالة مساعدة للحصول على قيمة من السيرفر
                string Get(string key)
                {
                    for (int i = 1; i < parts.Length - 1; i += 2)
                        if (parts[i] == key) return parts[i + 1];
                    return "";
                }

                // تحديد نسخة اللعبة
                string version =
                    infoLine.Contains("Allied Assault") ? "MOHAA" :
                    infoLine.Contains("Spearhead") ? "MOHSH" :
                    infoLine.Contains("Breakthrough") ? "MOHBT" :
                    "Unknown";

                string maxPlayers = Get("sv_maxclients");

                // ترتيب Tuple: ServerName, Map, GameType, GameVersion, Players
                return Tuple.Create(Get("sv_hostname"), Get("mapname"), Get("g_gametypestring"), version, playersCount + "/" + maxPlayers);
            }
            catch
            {
                return Tuple.Create("Offline", "", "", "Unknown", "0/0");
            }
            finally
            {
                if (sock != null)
                    sock.Close();
            }
        }
        private string GetLocationFromIP(string ip)
        {
            try
            {
                using (var client = new System.Net.WebClient())
                {
                    string json = client.DownloadString($"http://ip-api.com/json/{ip}");

                    // تحويل JSON إلى Dictionary
                    var data = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(json);

                    string country = data.ContainsKey("country") ? data["country"].ToString() : "Unknown";
                    string city = data.ContainsKey("city") ? data["city"].ToString() : "";

                    return string.IsNullOrWhiteSpace(city) ? country : city + ", " + country;
                }
            }
            catch
            {
                // لو فشل الاتصال أو ما فيه نت
                return "Unknown";
            }
        }

        // تخزين السيرفرات اللي المستخدم أضافها
        List<(string IP, int Port)> addedServers = new List<(string, int)>();

        private void addIPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (Form prompt = new Form())
            {
                // 1. إعدادات النافذة الأساسية
                prompt.Width = 320;
                prompt.Height = 260;
                prompt.Text = "Add Server";
                prompt.BackColor = this.BackColor; // مطابقة لون خلفية برنامجك
                prompt.ForeColor = this.ForeColor; // مطابقة لون نص برنامجك
                prompt.FormBorderStyle = FormBorderStyle.FixedDialog;
                prompt.StartPosition = FormStartPosition.CenterScreen;
                prompt.MaximizeBox = false;
                prompt.MinimizeBox = false;

                // 2. إنشاء العناصر مع نصوص افتراضية (لضمان الظهور الفوري)
                Label lblIp = new Label() { Name = "lblIp", Text = "IP Address:", Left = 20, Top = 20, Width = 260, Font = new Font("Segoe UI", 9, FontStyle.Bold) };
                TextBox txtIp = new TextBox() { Name = "txtIp", Left = 20, Top = 45, Width = 260, Text = "127.0.0.1" };

                Label lblPort = new Label() { Name = "lblPort", Text = "Port:", Left = 20, Top = 85, Width = 260, Font = new Font("Segoe UI", 9, FontStyle.Bold) };
                TextBox txtPort = new TextBox() { Name = "txtPort", Left = 20, Top = 110, Width = 260, Text = "12203" };

                Button btnConfirm = new Button() { Name = "btnConfirm", Text = "Add", Left = 160, Width = 110, Top = 165, Height = 35, DialogResult = DialogResult.OK, FlatStyle = FlatStyle.Flat };
                Button btnCancel = new Button() { Name = "btnCancel", Text = "Cancel", Left = 20, Width = 110, Top = 165, Height = 35, DialogResult = DialogResult.Cancel, FlatStyle = FlatStyle.Flat };

                // 3. إضافة العناصر للنافذة
                prompt.Controls.AddRange(new Control[] { lblIp, txtIp, lblPort, txtPort, btnConfirm, btnCancel });

                // 4. تطبيق اللغة (إذا كانت دالة الترجمة تغير النصوص)
                try
                {
                    ApplyLanguage(CurrentSettings?.SelectedLanguage ?? "en", "main", prompt);
                }
                catch { /* في حالة فشل الترجمة، ستبقى النصوص الافتراضية واضحة */ }

                // 5. ضبط الألوان "بقوة" لضمان الرؤية (Dark/Light Mode)
                bool isDark = (prompt.BackColor.R + prompt.BackColor.G + prompt.BackColor.B) / 3 < 128;
                Color forcedText = isDark ? Color.White : Color.Black;
                Color inputBack = isDark ? Color.FromArgb(45, 45, 48) : Color.White;
                Color btnBack = isDark ? Color.FromArgb(63, 63, 70) : Color.FromArgb(225, 225, 225);

                foreach (Control ctl in prompt.Controls)
                {
                    ctl.ForeColor = forcedText;
                    ctl.BringToFront(); // لجعل العناصر فوق أي خلفية

                    if (ctl is TextBox txt)
                    {
                        txt.BackColor = inputBack;
                        txt.BorderStyle = BorderStyle.FixedSingle;
                    }
                    else if (ctl is Button btn)
                    {
                        btn.BackColor = btnBack;
                        btn.FlatAppearance.BorderColor = forcedText;
                        btn.FlatAppearance.BorderSize = 1;
                    }
                }

                // 6. عرض النافذة ومعالجة النتيجة
                if (prompt.ShowDialog() == DialogResult.OK)
                {
                    string ip = txtIp.Text.Trim();
                    string portStr = txtPort.Text.Trim();

                    if (!string.IsNullOrWhiteSpace(ip) && int.TryParse(portStr, out int port))
                    {
                        string ipPort = $"{ip}:{port}";

                        // التأكد من عدم تكرار السيرفر
                        bool isDuplicate = listView1.Items.Cast<ListViewItem>()
                            .Any(x => x.SubItems.Count > 3 && x.SubItems[3].Text == ipPort);

                        if (isDuplicate)
                        {
                            MessageBox.Show("Server already exists.");
                            return;
                        }

                        // تنفيذ الإضافة
                        addedServers.Add((ip, port));
                        File.AppendAllLines(addedServersFile, new[] { ipPort });
                        AddServerToListView(ip, port);
                        UpdateServersCount();
                    }
                    else
                    {
                        MessageBox.Show("Invalid IP or Port.");
                    }
                }
            }
        }
        private void AddServerToListView(string ip, int port)
        {
            string country = GetLocationFromIP(ip);
            try
            {
                var info = GetServerStatus(ip, port);
                string status = info.Item1 == "Offline" ? "Offline" : "Online";

                var item = new ListViewItem(status);
                item.SubItems.Add(info.Item4);
                item.SubItems.Add(info.Item1);
                item.SubItems.Add($"{ip}:{port}");
                item.SubItems.Add(info.Item3);
                item.SubItems.Add(info.Item2);
                item.SubItems.Add(country);
                item.SubItems.Add(info.Item5);

                // تحديد اللون حسب ShowColors وPinned وRPG
                bool isPinned = pinnedServers != null && pinnedServers.Contains($"{ip}:{port}");
                bool showColors = CurrentSettings != null ? CurrentSettings.ShowColors : true;

                if (!showColors)
                {
                    // اللون الموحد حسب الثيم
                    item.BackColor = listView1.BackColor;
                    item.ForeColor = listView1.ForeColor;
                }
                else
                {
                    if (rpgTimer.Enabled)
                    {
                        // RPG شغال، التايمر يلونهم
                        // نترك اللون الافتراضي
                    }
                    else if (isPinned)
                    {
                        item.BackColor = Color.LightBlue;
                        item.ForeColor = Color.Black;
                        item.Tag = "Pinned";
                    }
                    else
                    {
                        item.BackColor = (status == "Offline") ? Color.Red : Color.LightGreen;
                        item.ForeColor = Color.Black;
                    }
                }

                listView1.Items.Insert(0, item);
            }
            catch
            {
                MessageBox.Show("Server Offline or No Response", "Error");
            }
        }

        private void LoadAddedServersFromFile()
        {
            if (!File.Exists(addedServersFile)) return;

            foreach (var line in File.ReadAllLines(addedServersFile))
            {
                var parts = line.Split(':');
                if (parts.Length != 2) continue;

                string ip = parts[0].Trim();
                if (!int.TryParse(parts[1], out int port)) continue;

                addedServers.Add((ip, port));
                AddServerToListView(ip, port);
            }
        }
        private void RefreshAddedServers()
        {
            bool isSearching = !string.IsNullOrEmpty(searchTextBox.Text);
            var settingForm = (settings)Application.OpenForms["settings"];

            // استخدام ShowColors من MainForm بدلاً من الفورم (أفضل للحفاظ على القيمة الحقيقية)
            bool showColors = CurrentSettings != null ? CurrentSettings.ShowColors
                            : (settingForm != null ? settingForm.checkBox3.Checked : true);

            foreach (var server in addedServers)
            {
                string ipPort = $"{server.IP}:{server.Port}";
                var item = listView1.Items.Cast<ListViewItem>()
                                          .FirstOrDefault(x => x.SubItems.Count > 3 && x.SubItems[3].Text == ipPort);
                if (item == null) continue;

                try
                {
                    var info = GetServerStatus(server.IP, server.Port);
                    string status = info.Item1 == "Offline" ? "Offline" : "Online";

                    // تحديث نصوص الحالة دائماً
                    item.SubItems[0].Text = status;
                    item.SubItems[1].Text = info.Item4;
                    item.SubItems[2].Text = info.Item1;
                    item.SubItems[4].Text = info.Item3;
                    item.SubItems[5].Text = info.Item2;
                    item.SubItems[7].Text = info.Item5;

                    // --- منطق الألوان ---
                    if (!showColors)
                    {
                        // ShowColors=false → كل الصفوف تاخد لون موحد حسب الثيم
                        Color uniformColor = listView1.BackColor;
                        item.BackColor = uniformColor;
                        item.ForeColor = listView1.ForeColor;
                    }
                    else
                    {
                        // ShowColors=true
                        bool isPinned = item.Tag != null && item.Tag.ToString() == "Pinned";

                        if (rpgTimer.Enabled)
                        {
                            // لو RPG شغال، اترك التايمر يلونهم
                            continue;
                        }
                        else if (isPinned)
                        {
                            // السيرفر المثبت ياخد اللون الخاص به
                            item.BackColor = Color.LightBlue;
                            item.ForeColor = Color.Black;
                        }
                        else if (!isSearching)
                        {
                            // الصفوف العادية حسب حالة Online/Offline
                            item.BackColor = (status == "Online") ? Color.LightGreen : Color.Red;
                            item.ForeColor = Color.Black;
                        }
                    }
                }
                catch
                {
                    item.SubItems[0].Text = "Offline";

                    if (!showColors)
                    {
                        Color uniformColor = listView1.BackColor;
                        item.BackColor = uniformColor;
                        item.ForeColor = listView1.ForeColor;
                    }
                    else
                    {
                        if (rpgTimer.Enabled) continue;

                        bool isPinned = item.Tag != null && item.Tag.ToString() == "Pinned";

                        if (isPinned)
                        {
                            item.BackColor = Color.LightBlue;
                            item.ForeColor = Color.Black;
                        }
                        else
                        {
                            item.BackColor = Color.Red;
                            item.ForeColor = Color.Black;
                        }
                    }
                }
            }
            ApplyLanguage(CurrentSettings.SelectedLanguage);

        }

        private void button1_Click(object sender, EventArgs e)
        {
            listView1.Visible = true;
            blockedListView.Visible = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            blockedListView.Visible = true; // تأكد إنه ظاهر
            listView1.Visible = false; // ممكن تخفي القائمة الأصلية مؤقتاً لو تحب
        }
        private void pinningToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                var item = listView1.SelectedItems[0];
                string ipPort = item.SubItems[3].Text;

                if (!pinnedServers.Contains(ipPort))
                {
                    // --- حالة التثبيت (Pin) ---
                    pinnedServers.Add(ipPort);
                    item.Tag = "Pinned";
                    File.WriteAllLines("pinned_ips.txt", pinnedServers);
                }
                else
                {
                    // --- حالة إلغاء التثبيت (Unpin) ---
                    pinnedServers.Remove(ipPort);
                    item.Tag = null; // مسح التاج ضروري لكي لا يراه تايمر الـ RPG
                    File.WriteAllLines("pinned_ips.txt", pinnedServers);
                }

                // تحديث لون الصف فوراً بناءً على الثيم المطبق حالياً
                ApplyRowColor(item);
            }
            RefreshAddedServers();
        }
        private void blockedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0)
            {
                MessageBox.Show("يرجى تحديد سيرفر واحد أو أكثر لحظره.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string blockFilePath = "blocked_ips.txt";
            var blockedIPs = File.Exists(blockFilePath)
                ? File.ReadAllLines(blockFilePath).ToList()
                : new List<string>();

            foreach (ListViewItem selectedItem in listView1.SelectedItems.Cast<ListViewItem>().ToList())
            {
                string ip = selectedItem.SubItems[3].Text;

                if (!blockedIPs.Contains(ip))
                {
                    blockedIPs.Add(ip);
                }

                // نسخة من العنصر ونضيفها لقائمة المحظورين
                ListViewItem blockedItem = (ListViewItem)selectedItem.Clone();
                blockedItem.BackColor = Color.Red;
                blockedListView.Items.Add(blockedItem);

                // نحذفه من قائمة السيرفرات
                listView1.Items.Remove(selectedItem);
            }

            File.WriteAllLines(blockFilePath, blockedIPs.Distinct());
            UpdateBlockedCount();

        }


        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                var selectedItem = listView1.SelectedItems[0];
                string ipPort = selectedItem.SubItems[3].Text; // نأخذ الـ IP:Port من العمود الرابع

                // 1. حذف السيرفر من الـ ListView
                listView1.Items.Remove(selectedItem);

                // 2. حذف السيرفر من القائمة البرمجية (addedServers)
                // نقوم بالبحث عن السيرفر داخل القائمة وحذفه
                addedServers.RemoveAll(s => $"{s.IP}:{s.Port}" == ipPort);

                // 3. تحديث الملف النصي (إعادة كتابته بالكامل بدون السيرفر المحذوف)
                try
                {
                    // تحويل القائمة المتبقية إلى نصوص بصيغة IP:Port
                    var linesToSave = addedServers.Select(s => $"{s.IP}:{s.Port}").ToArray();

                    // إعادة كتابة الملف بالكامل
                    File.WriteAllLines(addedServersFile, linesToSave);

                    UpdateServersCount(); // تحديث عداد السيرفرات في الواجهة
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error updating file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("No server selected to remove.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void unbanToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string blockFilePath = "blocked_ips.txt";
            var blockedList = File.Exists(blockFilePath)
                ? File.ReadAllLines(blockFilePath).ToList()
                : new List<string>();

            bool updated = false;

            foreach (ListViewItem selected in blockedListView.SelectedItems.Cast<ListViewItem>().ToList())
            {
                string ipPort = selected.SubItems[3].Text;

                if (blockedList.Contains(ipPort))
                {
                    blockedList.Remove(ipPort);
                    updated = true;

                    // إنشاء نسخة جديدة من العنصر لإعادته للقائمة الأصلية
                    ListViewItem restoredItem = (ListViewItem)selected.Clone();
                    restoredItem.BackColor = Color.White; // اللون الطبيعي
                    restoredItem.ForeColor = Color.Black;
                    restoredItem.Tag = null;

                    // تحديد إذا كان السيرفر مضاف يدويًا
                    var isAdded = addedServers.Any(x => $"{x.IP}:{x.Port}" == ipPort);

                    if (isAdded)
                    {
                        listView1.Items.Insert(0, restoredItem); // مضاف يدويًا → أول القائمة
                    }
                    else
                    {
                        listView1.Items.Add(restoredItem);       // من الموقع → آخر القائمة
                    }

                    blockedListView.Items.Remove(selected);

                    // ✅ تأكد انه موجود في addedServers لو كان مضاف يدويًا
                    if (isAdded)
                    {
                        var parts = ipPort.Split(':');
                        if (parts.Length == 2 && !addedServers.Any(x => $"{x.IP}:{x.Port}" == ipPort))
                        {
                            if (int.TryParse(parts[1], out int port))
                            {
                                addedServers.Add((parts[0], port));
                                File.AppendAllLines(addedServersFile, new[] { ipPort });
                            }
                        }
                    }
                }
            }

            if (updated)
            {
                File.WriteAllLines(blockFilePath, blockedList.Distinct());
            }

            UpdateBlockedCount();
            LoadPinnedServersByIP();
        }


        private void searchTextBox_TextChanged(object sender, EventArgs e)
        {
            string searchText = searchTextBox.Text.ToLower();  // تحويل النص إلى صغير لتسهيل المقارنة
            SearchServers(searchText);
        }
        private void SearchServers(string searchText)
        {
            bool isSearching = !string.IsNullOrWhiteSpace(searchText);
            var settingsForm = (settings)Application.OpenForms["settings"];
            bool showColors = (settingsForm != null) ? settingsForm.checkBox3.Checked : true;

            // أثناء البحث نوقف الـ RPG
            if (isSearching && rpgTimer.Enabled)
                rpgTimer.Stop();

            listView1.BeginUpdate();

            foreach (ListViewItem item in listView1.Items)
            {
                string serverName = item.SubItems[2].Text.ToLower();

                if (!isSearching)
                {
                    // إعادة اللون الطبيعي الأساسي قبل تطبيق المنطق
                    item.BackColor = listView1.BackColor;
                    item.ForeColor = listView1.ForeColor;

                    if (showColors)
                    {
                        ApplyRowColor(item); // اللون الحقيقي حسب الحالة (Pinned / Online / Offline / RPG)
                    }
                }
                else
                {
                    // أثناء البحث: تمييز النتائج فقط
                    if (serverName.Contains(searchText.ToLower()))
                    {
                        item.BackColor = Color.LightGreen;
                        item.ForeColor = Color.Black;
                    }
                    else
                    {
                        item.BackColor = listView1.BackColor;
                        item.ForeColor = listView1.ForeColor;
                    }
                }
            }

            listView1.EndUpdate();

            // إعادة تشغيل RPG بشكل ذكي بعد انتهاء البحث
            bool shouldRpgRun = false;

            // المصدر الأساسي: الإعدادات المحفوظة
            if (CurrentSettings != null)
            {
                shouldRpgRun = CurrentSettings.ShowColors && CurrentSettings.RpgEnabled;
            }
            else
            {
                // fallback لو الفورم مفتوح
                if (settingsForm != null)
                    shouldRpgRun = settingsForm.checkBox3.Checked && settingsForm.checkBox4.Checked;
            }

            if (!isSearching && shouldRpgRun)
            {
                if (!rpgTimer.Enabled)
                    rpgTimer.Start();

                // تحديث فوري للون قبل التكة الأولى
                RpgTimer_Tick(null, EventArgs.Empty);
            }
            ApplyLanguage(CurrentSettings.SelectedLanguage);

        }


        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            about menu = new about();
            menu.ShowDialog();
        }

        // 1. تعريف المتغير هنا (خارج الدالة) ليكون مرجعاً دائماً
        settings frm;

        public string SelectedLanguage { get; internal set; }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // 2. فحص: هل النافذة غير موجودة أو تم إغلاقها؟
            if (frm == null || frm.IsDisposed)
            {
                frm = new settings();
                frm.Show();
            }
            else
            {
                // 3. إذا كانت مفتوحة، لا تفتح واحدة جديدة بل "نبه" الموجودة
                frm.Activate();
                frm.BringToFront(); // تظهر أمام المستخدم فوراً
            }
        }
        private void MainForm_Load(object sender, EventArgs e)
        {
            // السطر ده وظيفته يمنع الرعشة نهائياً في القائمة مهما كان عدد التحديثات
            typeof(Control).GetProperty("DoubleBuffered",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .SetValue(listView1, true, null);
        }
    }
}
