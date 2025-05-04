using System;
using System.Windows.Forms;
using GameCollection.Forms;

namespace GameCollection
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new GameMainMenu()); 
        }
    }
}