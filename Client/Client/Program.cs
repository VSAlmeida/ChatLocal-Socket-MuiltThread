using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    static class Program
    {
        /// <summary>
        /// Ponto de entrada principal para o aplicativo.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            formLogin fLogin = new formLogin();
            Boolean flag = true;
            if (fLogin.ShowDialog() == DialogResult.OK)
            {
                if (fLogin.Textb() != "")
                {
                    flag = false;
                    formMain form = new formMain();
                    form.setName(fLogin.Textb());
                    form.setIP(fLogin.TextIP());
                    form.setPort(fLogin.TextPort());
                    Application.Run(form);
                }
                else 
                { 
                    fLogin.slblU("Entre"); 
                }
            }
            else 
            {
                Application.Exit(); 
            }
        }
    }
}
