using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace KinecTh
{
    static class Program
    {
        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            try
            {
                Application.Run(new MainForm());
            } catch(ObjectDisposedException e){
                Console.WriteLine(e.Message);
            }
        }
    }
}
