using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Media;
using System.Windows.Forms;


namespace mohaa_server_tool
{
    public partial class about : Form
    {
        private int xPos;
        private SoundPlayer player;
        private Timer timer;

        public about()
        {
            InitializeComponent();
        }

        private void about_Load(object sender, EventArgs e)
        {

            // لا حاجة لتعريف مسار string أو التأكد من وجود الملف بـ File.Exists
            try
            {
                // جلب ملف الصوت من الريسورس مباشرة
                // استبدل Mr_Robot باسم الملف كما يظهر عندك في صفحة الـ Resources
                player = new SoundPlayer(Properties.Resources.Mr_Robot);

                // تشغيل الصوت بشكل متكرر
                player.PlayLooping();
            }
            catch (Exception ex)
            {
                MessageBox.Show("خطأ في تشغيل الصوت: " + ex.Message);
            }
            // إعداد المؤقت لتحريك النص
            timer = new Timer();
            timer.Interval = 20; // تحديد التوقيت (20 ميللي ثانية) للحصول على حركة سلسة
            timer.Tick += (s, ev) =>
            {
                label1.Location = new Point(xPos, label1.Location.Y); // تحريك النص
                xPos += 5; // تحريك النص بمقدار 5 بيكسل
                if (xPos > this.Width) // إعادة النص من اليسار عند خروجه من اليمين
                {
                    xPos = -label1.Width;
                }
            };
            timer.Start();
        }

        // تأكد من إيقاف الصوت بشكل صحيح عندما يغلق النموذج
        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            if (player != null)
            {
                player.Stop();  // إيقاف الصوت عند غلق النموذج
                player.Dispose(); // تحرير الموارد
            }
            if (timer != null)
            {
                timer.Stop(); // إيقاف المؤقت عند غلق النموذج
                timer.Dispose(); // تحرير الموارد
            }
            base.OnFormClosed(e);
        }
    }
}
