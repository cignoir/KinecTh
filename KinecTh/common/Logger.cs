using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace KinecTh
{
    class Logger
    {
        TextBox textBox;

        public Logger(TextBox textBox)
        {
            this.textBox = textBox;
        }

        public void log(string msg)
        {
            Logger.log(textBox, msg);
        }

        public static void log(TextBox textBox, string msg)
        {

            Regex r = new Regex(".*" + msg + @"[.]*");
            if (r.Match(textBox.Text).Success)
            {
                textBox.AppendText(".");
            }
            else
            {
                textBox.AppendText("\r\n" + msg);
            }
            textBox.ScrollToCaret();
        }
    }
}
