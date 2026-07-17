using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows.Forms;


namespace mohaa_server_tool
{
    public partial class settings : Form
    {
        private string configPath = Path.Combine(Application.StartupPath, "config.json");
        public AppConfig CurrentSettings { get; set; }

        public settings()
        {
            InitializeComponent();
        }

        private void settings_Load(object sender, EventArgs e)
        {
            LoadThemesToCombo(comboBox1);
            LoadLanguagesToCombo(comboBox2);
            LoadConfig();
            ApplyCurrentTheme();
        }

        private void ApplyCurrentTheme()
        {
            var main = Application.OpenForms["MainForm"] as MainForm;
            string currentTheme = main?.CurrentSettings?.SelectedTheme ?? "default";

            this.SuspendLayout();
            ThemeManager.ApplyTheme(this, null, currentTheme, true);
            ApplyThemeRecursively(this, this.ForeColor);
            this.ResumeLayout(true);
        }

        private void ApplyThemeRecursively(Control container, Color foreColor)
        {
            foreach (Control ctrl in container.Controls)
            {
                if (ctrl is Label || ctrl is CheckBox || ctrl is GroupBox)
                {
                    ctrl.ForeColor = foreColor;
                    if (ctrl is Label || ctrl is CheckBox) ctrl.BackColor = Color.Transparent;
                }
                if (ctrl.HasChildren) ApplyThemeRecursively(ctrl, foreColor);
            }
        }

        // --- وظيفة الحفظ المركزية (تم تعديلها لحل مشكلة اللون الأخضر) ---
        private void SaveConfig()
        {
            try
            {
                var config = new AppConfig
                {
                    ShowColors = checkBox3.Checked,
                    PingEnabled = checkBox2.Checked,
                    RpgEnabled = checkBox4.Checked,
                    ShowSearch = checkBox5.Checked,
                    ShowOptionInStart = checkBox1.Checked,
                    SelectedTheme = comboBox1.Text,
                    SelectedLanguage = comboBox2.Text
                };

                this.CurrentSettings = config;
                string jsonString = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(configPath, jsonString);

                var mainForm = Application.OpenForms["MainForm"] as MainForm;
                if (mainForm != null)
                {
                    mainForm.CurrentSettings = config;

                    // 1. إذا تم إغلاق خيار الألوان، نقوم بمسح اللون الأخضر من كل الصفوف فوراً
                    if (!config.ShowColors)
                    {
                        mainForm.StopRPG(); // إيقاف التايمر أولاً
                        foreach (ListViewItem item in mainForm.listView1.Items)
                        {
                            // إعادة الصفوف للون خلفية الليست فيو الافتراضي حسب الثيم
                            item.BackColor = mainForm.listView1.BackColor;
                            item.ForeColor = mainForm.listView1.ForeColor;
                        }
                    }

                    // 2. تحديث الثيم العام
                    ThemeManager.ApplyTheme(mainForm, mainForm.listView1, config.SelectedTheme, config.ShowColors);

                    // 3. تحديث باقي العناصر
                    mainForm.searchTextBox.Visible = config.ShowSearch;
                    mainForm.label1.Visible = config.ShowSearch;

                    // 4. تشغيل RPG فقط إذا كان الخيارين مفعلين
                    if (config.ShowColors && config.RpgEnabled)
                        mainForm.StartRPG();
                    else
                        mainForm.StopRPG();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving settings: " + ex.Message);
            }
        }

        private void LoadConfig()
        {
            if (File.Exists(configPath))
            {
                try
                {
                    string jsonString = File.ReadAllText(configPath);
                    var config = JsonSerializer.Deserialize<AppConfig>(jsonString);
                    if (config != null)
                    {
                        this.CurrentSettings = config;
                        checkBox3.Checked = config.ShowColors;
                        checkBox2.Checked = config.PingEnabled;
                        checkBox4.Checked = config.RpgEnabled;
                        checkBox5.Checked = config.ShowSearch;
                        checkBox1.Checked = config.ShowOptionInStart;
                        comboBox1.SelectedIndex = Math.Max(0, comboBox1.FindStringExact(config.SelectedTheme));
                        comboBox2.SelectedIndex = Math.Max(0, comboBox2.FindStringExact(config.SelectedLanguage));
                    }
                }
                catch { this.CurrentSettings = new AppConfig(); }
            }
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e) => SaveConfig();
        private void checkBox4_CheckedChanged(object sender, EventArgs e) => SaveConfig();
        private void checkBox5_CheckedChanged(object sender, EventArgs e) => SaveConfig();
        private void checkBox1_CheckedChanged(object sender, EventArgs e) => SaveConfig();

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            SaveConfig();
            var mainForm = Application.OpenForms["MainForm"] as MainForm;

            if (mainForm != null)
            {
                // 1. تحديث الإعدادات داخل الفورم الرئيسي فوراً
                mainForm.CurrentSettings.PingEnabled = checkBox2.Checked;

                // 2. البحث عن العمود بالاسم البرمجي (Name)
                var pingCol = mainForm.listView1.Columns.Cast<ColumnHeader>()
                                      .FirstOrDefault(c => c.Name == "PingCol");

                if (checkBox2.Checked)
                {
                    if (pingCol == null)
                    {
                        // إنشاء العمود "خام" بدون نص ثابت
                        pingCol = new ColumnHeader();
                        pingCol.Name = "PingCol";
                        pingCol.Width = 70;
                        pingCol.TextAlign = HorizontalAlignment.Center;
                        mainForm.listView1.Columns.Add(pingCol);
                    }

                    // 3. المفتاح السحري: نادي دالة اللغة اللي عندك في MainForm
                    // هي هتدور على العمود اللي اسمه PingCol وتحط له الكلمة العربي من الـ JSON
                    mainForm.ApplyLanguage(mainForm.CurrentSettings.SelectedLanguage);
                }
                else
                {
                    if (pingCol != null)
                    {
                        mainForm.listView1.Columns.Remove(pingCol);
                    }
                }

                // 4. ضبط عرض الأعمدة عشان الجدول يتفرد
                mainForm.AdjustListViewColumns(mainForm.listView1);
            }
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)


        {

            // 1. حفظ الإعداد الجديد في ملف الـ JSON

            SaveConfig();



            string selectedTheme = comboBox1.Text;



            // 2. تحديث جميع النوافذ المفتوحة

            foreach (Form frm in Application.OpenForms.Cast<Form>().ToList())

            {

                // فحص MainForm

                if (frm is MainForm main)

                {

                    ThemeManager.ApplyTheme(main, main.listView1, selectedTheme);

                }

                // فحص فورم الحالة (status)

                else if (frm is status st)

                {

                    st.RefreshThemeExplicitly(selectedTheme);

                }

                // فحص فورم عرض الملفات (ViewFiles)

                else if (frm is ViewFiles vf)

                {

                    // تأكد أن هذه الكتلة موجودة مرة واحدة فقط بهذا الاسم 'vf'

                    vf.RefreshThemeExplicitly(selectedTheme);

                }

                // فحص فورم الإعدادات (settings)

                else if (frm is settings s)

                {

                    ThemeManager.ApplyTheme(s, null, selectedTheme);

                    ApplyThemeRecursively(s, s.ForeColor);

                }

                // فحص فورم التخمين (QuakeRcon.Form1)

                else if (frm is QuakeRcon.Form1 bruteForm)

                {

                    bruteForm.RefreshThemeExplicitly(selectedTheme);

                }

            }

        }
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            // 1. حفظ الإعدادات في ملف JSON
            SaveConfig();

            string lang = comboBox2.Text;

            // 2. تحديث كل الفورمز المفتوحة باستخدام ToList لضمان الوصول للجميع
            var openForms = Application.OpenForms.Cast<Form>().ToList();

            foreach (Form frm in openForms)
            {
                if (frm is MainForm main)
                    LanguageManager.ApplyLanguage(main, lang, "main");

                else if (frm is settings s)
                    LanguageManager.ApplyLanguage(s, lang, "settings");

                else if (frm is status st)
                    LanguageManager.ApplyLanguage(st, lang, "status");

                else if (frm is ViewFiles vf)
                    LanguageManager.ApplyLanguage(vf, lang, "viewfiles");

                else if (frm is QuakeRcon.Form1 brute)
                    LanguageManager.ApplyLanguage(brute, lang, "brute");
            }
        }

        private void LoadThemesToCombo(ComboBox combo)
        {
            string path = Path.Combine(Application.StartupPath, "themes");
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            combo.Items.Clear();
            foreach (var file in Directory.GetFiles(path, "*.json"))
                combo.Items.Add(Path.GetFileNameWithoutExtension(file));
        }

        private void LoadLanguagesToCombo(ComboBox combo)
        {
            string path = Path.Combine(Application.StartupPath, "language");
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            combo.Items.Clear();
            foreach (var file in Directory.GetFiles(path, "*.js"))
                combo.Items.Add(Path.GetFileNameWithoutExtension(file));
        }
    }

    public class AppConfig
    {
        // الإعدادات العامة (القديمة)
        public bool ShowColors { get; set; } = true;
        public bool PingEnabled { get; set; } = false;
        public bool RpgEnabled { get; set; } = false;
        public bool ShowSearch { get; set; } = true;
        public bool ShowOptionInStart { get; set; } = true;
        public string SelectedTheme { get; set; } = "default";
        public string SelectedLanguage { get; set; } = "default";

        // إعدادات فورم الـ Start (الحفظ الجديد)
        public string LastPlayerName { get; set; } = "Unknown";
        public string LastFPS { get; set; } = "0";
        public bool WinMode { get; set; } = false;      // checkBox1
        public bool NoRecoil { get; set; } = false;     // checkBox2
        public bool ThirdPerson { get; set; } = false;  // checkBox3
        public bool AntiForce { get; set; } = false;    // checkBox4
        public bool NoSun { get; set; } = false;        // checkBox5
        public bool NoTree { get; set; } = false;       // checkBox6
        public bool HideWindows { get; set; } = false;  // checkBox7
        public bool NoRockets { get; set; } = false;    // checkBox13
        public bool Sandbags { get; set; } = false;     // checkBox14
        public bool Wallhack { get; set; } = false;     // checkBox15
        public bool SkyColor { get; set; } = false;     // checkBox8
        public bool ShotColor { get; set; } = false;
        public bool ExitInStart { get; set; } = false;  // checkBox9
        public bool DevMode { get; set; } = false;      // checkBox10
        public bool FpsGreen { get; set; } = false;     // checkBox11
        public bool Bit16 { get; set; } = false;        // checkBox12
        
        public bool FiledToView { get; set; } = false;
        public string SelectedSkyColor { get; set; } = "sky-black.pk3"; // قيمة افتراضية

        // ✅ Shot Color
        public bool ShotColorEnabled { get; set; } = false;  // checkbox17
        public string SelectedShotColor { get; set; } = "Default"; // قيمة افتراضية
    }

}