using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows.Forms;
using mohaa_server_tool;

public static class LanguageManager
{
    public static void ApplyLanguage(Form targetForm, string lang, string groupName)
    {
        if (targetForm == null || string.IsNullOrEmpty(lang)) return;

        string langFile = Path.Combine(Application.StartupPath, "language", lang + ".js");
        if (!File.Exists(langFile)) return;

        try
        {
            string json = File.ReadAllText(langFile);
            var langObj = JsonSerializer.Deserialize<LanguageFile>(json);
            if (langObj == null) return;

            Dictionary<string, string> texts = null;
            switch (groupName.ToLower())
            {
                case "main": texts = langObj.main; break;
                case "settings": texts = langObj.settings; break;
                case "status": texts = langObj.status; break;
                case "viewfiles": texts = langObj.viewfiles; break;
                case "brute": texts = langObj.brute; break;
            }

            if (texts == null) return;

            foreach (var kv in texts)
            {
                // 1. ترجمة عنوان الفورم
                if (kv.Key.Equals("form_title", StringComparison.OrdinalIgnoreCase))
                {
                    targetForm.Text = kv.Value;
                    continue;
                }

                // 2. ترجمة الأعمدة
                if (kv.Key.StartsWith("column_", StringComparison.OrdinalIgnoreCase))
                {
                    UpdateColumns(targetForm, kv.Key, kv.Value);
                    continue;
                }

                // 3. ترجمة القوائم (MenuStrip & ContextMenuStrip) - الحل الجديد هنا
                if (UpdateMenuItems(targetForm, kv.Key, kv.Value))
                {
                    continue;
                }

                // 4. ترجمة الأدوات العادية
                Control ctl = targetForm.Controls.Find(kv.Key, true).FirstOrDefault();
                if (ctl != null)
                {
                    ctl.Text = kv.Value;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Language Error: " + ex.Message);
        }
    }

    // دالة ترجمة القوائم (Strip Menu Items)
    private static bool UpdateMenuItems(Form frm, string key, string text)
    {
        // البحث في المنيو الرئيسي للفورم
        if (frm.MainMenuStrip != null)
        {
            if (TryTranslateStrip(frm.MainMenuStrip.Items, key, text)) return true;
        }

        // البحث في جميع الـ ContextMenuStrip المعرفة في الكود
        // نستخدم الـ Reflection لجلب أي ContextMenu مسجل في الكلاس
        var fields = frm.GetType().GetFields(System.Reflection.BindingFlags.Instance |
                                            System.Reflection.BindingFlags.NonPublic |
                                            System.Reflection.BindingFlags.Public);

        foreach (var field in fields)
        {
            if (field.FieldType == typeof(ContextMenuStrip))
            {
                var cms = (ContextMenuStrip)field.GetValue(frm);
                if (cms != null)
                {
                    if (TryTranslateStrip(cms.Items, key, text)) return true;
                }
            }
        }
        return false;
    }

    // دالة فرعية للبحث المتكرر داخل القوائم (Recursive)
    private static bool TryTranslateStrip(ToolStripItemCollection items, string key, string text)
    {
        foreach (ToolStripItem item in items)
        {
            if (item.Name == key)
            {
                item.Text = text;
                return true;
            }
            if (item is ToolStripMenuItem menuItem && menuItem.DropDownItems.Count > 0)
            {
                if (TryTranslateStrip(menuItem.DropDownItems, key, text)) return true;
            }
        }
        return false;
    }

    private static void UpdateColumns(Form frm, string key, string text)
    {
        string[] parts = key.Split('_');
        if (parts.Length < 2) return;
        if (int.TryParse(parts[1], out int colIndex))
        {
            var allListViews = GetAllControls(frm).OfType<ListView>();
            foreach (var lv in allListViews)
            {
                if (colIndex < lv.Columns.Count) lv.Columns[colIndex].Text = text;
            }
        }
    }

    private static IEnumerable<Control> GetAllControls(Control container)
    {
        var controls = container.Controls.Cast<Control>();
        return controls.SelectMany(c => GetAllControls(c)).Concat(controls);
    }

    public static void ApplyLanguageToAllOpenForms(string lang)
    {
        var openForms = Application.OpenForms.Cast<Form>().ToList();
        foreach (Form frm in openForms)
        {
            if (frm is MainForm) ApplyLanguage(frm, lang, "main");
            else if (frm is settings) ApplyLanguage(frm, lang, "settings");
            else if (frm is status) ApplyLanguage(frm, lang, "status");
            else if (frm is ViewFiles) ApplyLanguage(frm, lang, "viewfiles");
            else if (frm.Name == "Form1") ApplyLanguage(frm, lang, "brute");
        }
    }
}