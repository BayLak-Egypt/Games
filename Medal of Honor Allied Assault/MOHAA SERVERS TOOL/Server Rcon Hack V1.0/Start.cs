using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows.Forms;

namespace mohaa_server_tool
{
    public partial class Start : Form
    {
        public string IP { get; set; }
        private string configPath = Path.Combine(Application.StartupPath, "config.json");
       
        public Start()
        {
            InitializeComponent();
            this.Load += Start_Load;
        }

        private void Start_Load(object sender, EventArgs e)
        {
            var main = Application.OpenForms["MainForm"] as MainForm;
            if (main?.CurrentSettings is AppConfig settings)
            {
                this.BeginInvoke((MethodInvoker)delegate
                {
                    // فصل الأحداث لتجنب triggers غير مرغوبة
                    checkBox8.CheckedChanged -= checkBox8_CheckedChanged;
                    checkBox17.CheckedChanged -= checkBox17_CheckedChanged;

                    this.SuspendLayout();
                    try
                    {
                        // -------------------------
                        // النصوص
                        // -------------------------
                        textBox1.Text = settings.LastPlayerName ?? "";
                        textBox2.Text = settings.LastFPS ?? "0";

                        // -------------------------
                        // CheckBoxes الأساسية
                        // -------------------------
                        checkBox1.Checked = settings.WinMode;
                        checkBox2.Checked = settings.NoRecoil;
                        checkBox3.Checked = settings.ThirdPerson;
                        checkBox4.Checked = settings.AntiForce;
                        checkBox5.Checked = settings.NoSun;
                        checkBox6.Checked = settings.NoTree;
                        checkBox7.Checked = settings.HideWindows;
                        checkBox8.Checked = settings.SkyColor;
                        checkBox9.Checked = settings.ExitInStart;
                        checkBox10.Checked = settings.DevMode;
                        checkBox11.Checked = settings.FpsGreen;
                        checkBox12.Checked = settings.Bit16;
                        checkBox13.Checked = settings.NoRockets;
                        checkBox14.Checked = settings.Sandbags;
                        checkBox15.Checked = settings.Wallhack;
                        checkBox16.Checked = settings.FiledToView;
                        checkBox17.Checked = settings.ShotColorEnabled;

                        // -------------------------
                        // Sky Color
                        // -------------------------
                        button2.Tag = settings.SelectedSkyColor;
                        button2.Enabled = settings.SkyColor;
                        if (settings.SkyColor && !string.IsNullOrEmpty(settings.SelectedSkyColor))
                            LoadSkyColorFromSettings(settings.SelectedSkyColor);

                        // -------------------------
                        // Shot Color
                        // -------------------------
                        button3.Enabled = settings.ShotColorEnabled;
                        string savedShotColor = string.IsNullOrEmpty(settings.SelectedShotColor) ? "Default" : settings.SelectedShotColor;
                        button3.Tag = savedShotColor;
                        LoadShotColorFromSettings(savedShotColor);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("Load Error (Start_Load): " + ex.Message);
                    }
                    finally
                    {
                        // إعادة ربط الأحداث بعد التحميل
                        checkBox8.CheckedChanged += checkBox8_CheckedChanged;
                        checkBox17.CheckedChanged += checkBox17_CheckedChanged;
                        this.ResumeLayout(true);
                    }
                });
            }
        }


        private void SaveCurrentSettings()
        {
            try
            {
                var main = Application.OpenForms["MainForm"] as MainForm;
                if (main?.CurrentSettings is AppConfig s)
                {
                    // النصوص
                    s.LastPlayerName = textBox1.Text;
                    s.LastFPS = textBox2.Text;

                    // CheckBoxes
                    s.WinMode = checkBox1.Checked;
                    s.NoRecoil = checkBox2.Checked;
                    s.ThirdPerson = checkBox3.Checked;
                    s.AntiForce = checkBox4.Checked;
                    s.NoSun = checkBox5.Checked;
                    s.NoTree = checkBox6.Checked;
                    s.HideWindows = checkBox7.Checked;
                    s.SkyColor = checkBox8.Checked;
                    s.ShotColorEnabled = checkBox17.Checked;
                    s.FiledToView = checkBox16.Checked;
                    s.ExitInStart = checkBox9.Checked;
                    s.DevMode = checkBox10.Checked;
                    s.FpsGreen = checkBox11.Checked;
                    s.Bit16 = checkBox12.Checked;
                    s.NoRockets = checkBox13.Checked;
                    s.Sandbags = checkBox14.Checked;
                    s.Wallhack = checkBox15.Checked;

                    // حفظ ألوان الزر
                    if (button2.Tag != null)
                        s.SelectedSkyColor = button2.Tag.ToString();

                    if (button3.Tag != null)
                        s.SelectedShotColor = button3.Tag.ToString();

                    // كتابة الملف JSON
                    var options = new JsonSerializerOptions { WriteIndented = true };
                    string json = JsonSerializer.Serialize(s, options);
                    File.WriteAllText(configPath, json);

                    Debug.WriteLine("Settings Saved Successfully!");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"CRITICAL ERROR: {ex.Message}");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

            this.Hide();
            try
            {
                // 1. حفظ الإعدادات
                SaveCurrentSettings();

                // 2. التحقق من وجود ملف اللعبة
                if (!System.IO.File.Exists("MOHAA.exe"))
                {
                    MessageBox.Show("خطأ: ملف MOHAA.exe غير موجود!", "تنبيه");
                    return;
                }

                // 3. تجهيز الأوامر (Arguments)
                string playerName = textBox1.Text.Replace("\"", "").Replace(" ", "_");
                string fullscreen = checkBox1.Checked ? "0" : "1";
                string greenFps = checkBox11.Checked ? "1" : "0";
                string picmipValue = checkBox12.Checked ? "10" : "0";

                string arguments = $"+set com.maxfps {textBox2.Text} " +
                                   $"+set name \"{playerName}\" " +
                                   $"+set r_fullscreen {fullscreen} " +
                                   $"+set cl_greenfps {greenFps} " +
                                   $"+set cg_drawFPS {greenFps} " +
                                   $"+set r_picmip {picmipValue} ";

                if (checkBox10.Checked)
                    arguments += "+set developer 1 +set thereisnomonkey 1 +set cheats 1 +set ui_console 1 ";

                if (checkBox15.Checked)
                    arguments += "+set dm_playergermanmodel \"german_waffenss_shutze\" +set dm_playermodel \"american_ranger\" ";

                arguments += $"+connect {IP}";

                // 4. إعدادات تشغيل اللعبة
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = "MOHAA.exe",
                    Arguments = arguments,
                    WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory
                };

                Process gameProcess = Process.Start(startInfo);

                // 5. التحكم في سلوك البرنامج بناءً على زر 9 (checkBox9)
                if (checkBox9.Checked)
                {
                    // --- الحالة الأولى: زر 9 مفعل (الوضع المتقدم / الإخفاء) ---

                    if (gameProcess != null)
                    {
                        gameProcess.EnableRaisingEvents = true;
                        gameProcess.Exited += (s, ev) =>
                        {
                            // عند إغلاق اللعبة، يتم إنهاء البرنامج بالكامل كما طلبت
                            Application.Exit();
                        };
                    }

                    // فحص الميزات الخلفية للإخفاء التام
                    if (checkBox4.Checked || checkBox16.Checked || checkBox3.Checked)
                    {
                        var allForms = Application.OpenForms.Cast<Form>().ToList();
                        foreach (Form f in allForms)
                        {
                            f.Invoke((MethodInvoker)delegate {
                                f.ShowInTaskbar = false;
                                f.Hide();
                            });
                        }
                    }
                    else
                    {
                        // إذا لم يكن هناك ميزات خلفية، نخرج فوراً (لأن زر 9 مفعل)
                        Application.Exit();
                    }
                }
                else
                {
                    // --- الحالة الثانية: زر 9 غير مفعل (الوضع الطبيعي) ---

                    // نصغر النافذة الحالية فقط
                    this.WindowState = FormWindowState.Minimized;

                    // ملحوظة: هنا لم نضع حدث Exited، فبالتالي البرنامج لن يغلق عند خروجك من اللعبة
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"حدث خطأ: {ex.Message}");
            }

            if (checkBox4.Checked) AntiForce.Toggle(true);
            if (checkBox16.Checked) ZoomPerson.Toggle(true);
            if (checkBox3.Checked) ThirdPerson.Toggle(true);
        }
        private void ApplyThemeToAllControls(Control container, Color themeColor)
        {
            foreach (Control ctrl in container.Controls)
            {
                if (ctrl is Label || ctrl is CheckBox || ctrl is GroupBox)
                {
                    ctrl.ForeColor = themeColor;
                    if (ctrl is Label || ctrl is CheckBox) ctrl.BackColor = Color.Transparent;
                }
                if (ctrl.HasChildren) ApplyThemeToAllControls(ctrl, themeColor);
            }
        }

        private void Start_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveCurrentSettings();
        }
        private void ToggleMod(bool isChecked, string fileName, byte[] resourceFile)
        {
            // الحصول على مسار الفولدر اللي فيه البرنامج حالياً
            string currentPath = Application.StartupPath;

            // تحديد مسار فولدر main في نفس مكان البرنامج
            string destinationFolder = Path.Combine(currentPath, "main");
            string destinationPath = Path.Combine(destinationFolder, fileName);

            try
            {
                if (isChecked)
                {
                    // إنشاء فولدر main لو مش موجود بجانب البرنامج
                    if (!Directory.Exists(destinationFolder))
                    {
                        Directory.CreateDirectory(destinationFolder);
                    }

                    // استخراج الملف من الـ Resources وكتابته على الهارد
                    File.WriteAllBytes(destinationPath, resourceFile);
                }
                else
                {
                    // حذف الملف لو شلنا علامة الصح
                    if (File.Exists(destinationPath))
                    {
                        File.Delete(destinationPath);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("حدث خطأ أثناء التعامل مع الملف: " + ex.Message);
            }
        }
        private void checkBox2_CheckedChanged(object sender, EventArgs e) => ToggleMod(checkBox2.Checked, "no_recoil.pk3", Properties.Resources.no_recoil);
        private void checkBox15_CheckedChanged(object sender, EventArgs e) => ToggleMod(checkBox15.Checked, "wallhack.pk3", Properties.Resources.wallhack);
        private void checkBox6_CheckedChanged(object sender, EventArgs e) => ToggleMod(checkBox6.Checked, "no_trees_bushes.pk3", Properties.Resources.no_trees_bushes);
        private void checkBox7_CheckedChanged(object sender, EventArgs e) => ToggleMod(checkBox7.Checked, "no_window.pk3", Properties.Resources.no_window);
        private void checkBox5_CheckedChanged(object sender, EventArgs e) => ToggleMod(checkBox5.Checked, "no_sunflare.pk3", Properties.Resources.no_sunflare);
        private void checkBox13_CheckedChanged(object sender, EventArgs e) => ToggleMod(checkBox13.Checked, "no_rockets.pk3", Properties.Resources.no_rockets);
        private void checkBox14_CheckedChanged(object sender, EventArgs e) => ToggleMod(checkBox14.Checked, "sandbags_transparen.pk3", Properties.Resources.sandbags_transparen);

        private void SetSkyColor(string fileName, byte[] resourceFile, Color btnColor)
        {
            try
            {
                string dest = Path.Combine(Application.StartupPath, "main", "zz_sky_mod.pk3");
                Directory.CreateDirectory(Path.GetDirectoryName(dest));

                // تحديث الملف على القرص
                if (File.Exists(dest)) File.Delete(dest);
                if (resourceFile != null) File.WriteAllBytes(dest, resourceFile);

                // تحديث الواجهة (تغيير اللون والنص وحساب التباين)
                button2.Invoke((MethodInvoker)delegate {
                    button2.Tag = fileName;
                    button2.BackColor = btnColor;
                    button2.ForeColor = (btnColor.R * 0.299 + btnColor.G * 0.587 + btnColor.B * 0.114) > 186 ? Color.Black : Color.White;
                    button2.Text = "Sky: " + fileName.Replace(".pk3", "").Replace("sky-", "");
                });

                SaveCurrentSettings();
            }
            catch (Exception ex) { MessageBox.Show($"Error: {ex.Message}"); }
        }

        // 2. دالة التحميل الذكية (بدون Switch Case)
        private void LoadSkyColorFromSettings(string savedFileName)
        {
            if (string.IsNullOrEmpty(savedFileName)) return;

            // قاموس لربط أسماء الملفات بالموارد والألوان
            var skyData = new Dictionary<string, (byte[] Res, Color Clr)>
            {
                ["sky-black.pk3"] = (Properties.Resources.sky_black, Color.Black),
                ["sky-blue.pk3"] = (Properties.Resources.sky_blue, Color.DeepSkyBlue),
                ["sky-gray.pk3"] = (Properties.Resources.sky_gray, Color.Gray),
                ["sky-green.pk3"] = (Properties.Resources.sky_green, Color.LimeGreen),
                ["sky-orange.pk3"] = (Properties.Resources.sky_orange, Color.Orange),
                ["sky-purple.pk3"] = (Properties.Resources.sky_purple, Color.Purple),
                ["sky-red.pk3"] = (Properties.Resources.sky_red, Color.Red),
                ["sky-white.pk3"] = (Properties.Resources.sky_white, Color.White),
                ["sky-yellow.pk3"] = (Properties.Resources.sky_yellow, Color.Yellow)
            };

            if (skyData.ContainsKey(savedFileName))
                SetSkyColor(savedFileName, skyData[savedFileName].Res, skyData[savedFileName].Clr);
        }

        // 3. التحكم في التفعيل والحذف
        private void checkBox8_CheckedChanged(object sender, EventArgs e)
        {
            button2.Enabled = checkBox8.Checked;
            if (checkBox8.Checked)
            {
                if (button2.Tag != null) LoadSkyColorFromSettings(button2.Tag.ToString());
            }
            else
            {
                button2.BackColor = SystemColors.Control;
                button2.ForeColor = Color.Black;
                button2.Text = "Sky Color";
                try { File.Delete(Path.Combine(Application.StartupPath, "main", "zz_sky_mod.pk3")); } catch { }
            }
            SaveCurrentSettings();
        }

        // 4. زر القائمة المختصر (Loop بدلاً من تكرار الأسطر)
        private void button2_Click(object sender, EventArgs e)
        {
            ContextMenuStrip skyMenu = new ContextMenuStrip();
            string[] colors = { "Black", "Blue", "Gray", "Green", "Orange", "Purple", "Red", "White", "Yellow" };

            foreach (var c in colors)
            {
                string fileName = $"sky-{c.ToLower()}.pk3";
                // جلب المورد واللون ديناميكياً
                var res = (byte[])Properties.Resources.ResourceManager.GetObject(fileName.Replace("-", "_").Replace(".pk3", ""));
                var col = Color.FromName(c == "Blue" ? "DeepSkyBlue" : c); // معالجة خاصة للأزرق

                skyMenu.Items.Add($"{c} Sky", null, (s, ev) => SetSkyColor(fileName, res, col));
            }
            skyMenu.Show(button2, 0, button2.Height);
        }
        // عند تغيير حالة الأزرار
        private void checkBox3_CheckedChanged(object sender, EventArgs e) => ThirdPerson.Toggle(checkBox3.Checked);
        private void checkBox16_CheckedChanged(object sender, EventArgs e) => ZoomPerson.Toggle(checkBox4.Checked);
        private void checkBox4_CheckedChanged(object sender, EventArgs e) => AntiForce.Toggle(checkBox16.Checked);
        private void SetShotColor(string colorName, byte[] resourceFile, Color btnColor)
        {
            try
            {
                string mainDir = Path.Combine(Application.StartupPath, "main");
                Directory.CreateDirectory(mainDir);

                // حذف الملف القديم shoot-color.pk3 إذا موجود
                string shotColorFile = Path.Combine(mainDir, "shoot-color.pk3");
                if (File.Exists(shotColorFile))
                {
                    try { File.Delete(shotColorFile); } catch { }
                }

                // كتابة الملفات الجديدة
                string shotShow = Path.Combine(mainDir, "shoot_show.pk3");
                if (!File.Exists(shotShow)) // لا يعيد كتابة shoot_show إذا موجود
                    File.WriteAllBytes(shotShow, Properties.Resources.shoot_show);

                if (resourceFile != null)
                    File.WriteAllBytes(shotColorFile, resourceFile);

                // تحديث الزر
                button3.Invoke((MethodInvoker)delegate {
                    button3.Tag = colorName;
                    button3.BackColor = btnColor;
                    button3.ForeColor = (btnColor.R * 0.299 + btnColor.G * 0.587 + btnColor.B * 0.114) > 186 ? Color.Black : Color.White;
                    button3.Text = "Shot: " + colorName;
                });

                SaveCurrentSettings();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void LoadShotColorFromSettings(string savedFileName)
        {
            if (string.IsNullOrEmpty(savedFileName)) return;

            var shotData = new Dictionary<string, (byte[] Res, Color Clr)>
            {
                ["shoot_color-black.pk3"] = (Properties.Resources.shoot_color_black, Color.Black),
                ["shoot_color-blue.pk3"] = (Properties.Resources.shoot_color_blue, Color.DeepSkyBlue),
                ["shoot_color-gray.pk3"] = (Properties.Resources.shoot_color_gray, Color.Gray),
                ["shoot_color-green.pk3"] = (Properties.Resources.shoot_color_green, Color.LimeGreen),
                ["shoot_color-orange.pk3"] = (Properties.Resources.shoot_color_orange, Color.Orange),
                ["shoot_color-purple.pk3"] = (Properties.Resources.shoot_color_purple, Color.Purple),
                ["shoot_color-red.pk3"] = (Properties.Resources.shoot_color_red, Color.Red),
                ["shoot_color-white.pk3"] = (Properties.Resources.shoot_color_white, Color.White),
                ["shoot_color-yellow.pk3"] = (Properties.Resources.shoot_color_yellow, Color.Yellow),
                ["shoot_color-default.pk3"] = (Properties.Resources.shoot_color_default, SystemColors.Control) // لون افتراضي
            };

            if (shotData.ContainsKey(savedFileName))
                SetShotColor(savedFileName, shotData[savedFileName].Res, shotData[savedFileName].Clr);
        }
        private void checkBox17_CheckedChanged(object sender, EventArgs e)
        {
            button3.Enabled = checkBox17.Checked;
            string mainDir = Path.Combine(Application.StartupPath, "main");
            string shotShow = Path.Combine(mainDir, "shoot_show.pk3");
            string shotColorFile = Path.Combine(mainDir, "shoot-color.pk3");

            if (checkBox17.Checked)
            {
                if (!Directory.Exists(mainDir))
                    Directory.CreateDirectory(mainDir);

                // كتابة shoot_show.pk3 إذا غير موجود
                if (!File.Exists(shotShow))
                    File.WriteAllBytes(shotShow, Properties.Resources.shoot_show);

                // كتابة shoot-color.pk3 للون الحالي
                if (button3.Tag != null)
                {
                    string colorName = button3.Tag.ToString();
                    var shotData = new Dictionary<string, byte[]>
                    {
                        ["Black"] = Properties.Resources.shoot_color_black,
                        ["Blue"] = Properties.Resources.shoot_color_blue,
                        ["Gray"] = Properties.Resources.shoot_color_gray,
                        ["Green"] = Properties.Resources.shoot_color_green,
                        ["Orange"] = Properties.Resources.shoot_color_orange,
                        ["Purple"] = Properties.Resources.shoot_color_purple,
                        ["Red"] = Properties.Resources.shoot_color_red,
                        ["White"] = Properties.Resources.shoot_color_white,
                        ["Yellow"] = Properties.Resources.shoot_color_yellow,
                        ["Default"] = Properties.Resources.shoot_color_default
                    };

                    if (shotData.ContainsKey(colorName))
                    {
                        File.WriteAllBytes(shotColorFile, shotData[colorName]);
                    }
                }

                // تحميل الزر للعرض الصحيح
                if (button3.Tag != null)
                    LoadShotColorFromSettings(button3.Tag.ToString());
            }
            else
            {
                // إعادة الزر للوضع الافتراضي
                button3.BackColor = SystemColors.Control;
                button3.ForeColor = Color.Black;
                button3.Text = "Shot Color";

                // حذف الملفات
                try
                {
                    if (File.Exists(shotShow)) File.Delete(shotShow);
                    if (File.Exists(shotColorFile)) File.Delete(shotColorFile);
                }
                catch { }
            }

            SaveCurrentSettings();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ContextMenuStrip shotMenu = new ContextMenuStrip();
            string[] colors = { "Black", "Blue", "Gray", "Green", "Orange", "Purple", "Red", "White", "Yellow", "Default" };

            foreach (var c in colors)
            {
                string fileName = $"shoot_color-{c.ToLower()}.pk3";
                var res = (byte[])Properties.Resources.ResourceManager.GetObject(fileName.Replace("-", "_").Replace(".pk3", ""));
                var col = c == "Blue" ? Color.DeepSkyBlue : (c == "Default" ? SystemColors.Control : Color.FromName(c));

                shotMenu.Items.Add($"{c} Shot", null, (s, ev) => SetShotColor(fileName, res, col));
            }

            shotMenu.Show(button3, 0, button3.Height);
        }
    }
}