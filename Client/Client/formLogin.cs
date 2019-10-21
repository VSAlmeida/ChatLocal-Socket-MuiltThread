using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    public partial class formLogin : Form
    {
        public formLogin()
        {
            InitializeComponent();
        }
        public String Textb()
        {
            return textBox1.Text;
        }
        public String TextIP()
        {
            return textBox2.Text;
        }
        public String TextPort()
        {
            return textBox3.Text;
        }
        public void slblU(String v)
        {
            lblName.Text = v;
        }
    }
}
