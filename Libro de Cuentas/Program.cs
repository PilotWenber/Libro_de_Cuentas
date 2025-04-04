using System;
using System.Windows.Forms;
using Libro_de_Cuentas.Data;

namespace Libro_de_Cuentas
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Database.Inicializar(); // Inicializa la DB

#if NETCOREAPP || NET5_0_OR_GREATER
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
#endif
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
