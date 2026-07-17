using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text.Json;
using System.Windows.Forms;

public static class ThemeManager
{
    public static void ApplyTheme(Form targetForm, ListView listView, string themeName, bool globalShowColors = true)
    {
        if (targetForm == null) return;

        try
        {
            string filePath = Path.Combine(Application.StartupPath, "themes", themeName + ".json");
            if (!File.Exists(filePath)) return;

            string jsonString = File.ReadAllText(filePath);
            var theme = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonString);

            if (theme != null)
            {
                Color backColor = theme.ContainsKey("BackColor") ? ColorTranslator.FromHtml(theme["BackColor"]) : targetForm.BackColor;
                Color textColor = theme.ContainsKey("TextColor") ? ColorTranslator.FromHtml(theme["TextColor"]) : targetForm.ForeColor;
                Color listBack = theme.ContainsKey("ListBackColor") ? ColorTranslator.FromHtml(theme["ListBackColor"]) : backColor;

                targetForm.BackColor = backColor;
                targetForm.ForeColor = textColor;

                if (listView != null)
                {
                    listView.BackColor = listBack;
                    listView.ForeColor = textColor;

                    foreach (ListViewItem item in listView.Items)
                    {
                        // تنظيف الألوان: جعل الصف يتبع الثيم فقط
                        // التلوين بالأخضر أو الـ RPG سيتم من MainForm فقط لعدم التداخل
                        item.BackColor = listBack;
                        item.ForeColor = textColor;
                        item.UseItemStyleForSubItems = true;
                    }
                }

                // تلوين باقي الأدوات
                foreach (Control c in targetForm.Controls)
                {
                    if (c is ListView) continue;
                    c.BackColor = backColor;
                    c.ForeColor = textColor;
                    if (c.HasChildren) ApplyToChildren(c, backColor, textColor);
                }
            }
        }
        catch (Exception ex) { Console.WriteLine("Theme Error: " + ex.Message); }
    }

    private static void ApplyToChildren(Control parent, Color back, Color fore)
    {
        foreach (Control c in parent.Controls)
        {
            if (c is ListView) continue;
            c.BackColor = Color.Transparent;
            c.ForeColor = fore;
            if (c.HasChildren) ApplyToChildren(c, back, fore);
        }
    }
}