using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace Client
{
    public partial class formMain : Form
    {
        public TcpClient clientSocket;
        public NetworkStream serverStream = default(NetworkStream);
        string readData = null;
        Thread ctThread;
        String name = null;
        String ip = null;
        String port = null;
        List<string> nowChatting = new List<string>();
        List<string> chat = new List<string>();
        public void setName(String title)
        {
            this.Text = title;
            name = title;
        }
        public formMain()
        {
            InitializeComponent();
        }
        private void btnSend_Click(object sender, EventArgs e)
        {
            try
            {
                if (!input.Text.Equals(""))
                {
                    chat.Add("gChat");
                    chat.Add(input.Text);
                    byte[] outStream = ObjectToByteArray(chat);
                    serverStream.Write(outStream, 0, outStream.Length);
                    serverStream.Flush();
                    input.Text = "";
                    chat.Clear();
                }
            }
            catch (Exception)
            {
                btnConnect.Enabled = true;
            }
        }
        public void setIP(String ip_)
        {
            this.Text = ip_;
            ip = ip_;
        }
        public void setPort(String port_)
        {
            this.Text = port_;
            port = port_;
        }
        private void btnConnect_Click(object sender, EventArgs e)
        {
            clientSocket = new TcpClient();
            try
            {
                clientSocket.Connect(ip, int.Parse(port));
                readData = "Conectado =) ";
                msg();
                serverStream = clientSocket.GetStream();
                byte[] outStream = Encoding.ASCII.GetBytes(name + "$");
                serverStream.Write(outStream, 0, outStream.Length);
                serverStream.Flush();
                btnConnect.Enabled = false;
                ctThread = new Thread(getMessage);
                ctThread.Start();
            }
            catch (Exception)
            {
                MessageBox.Show("Inicie o Servidor! ");
            }
        }
        public void getUsers(List<string> parts)
        {
            this.Invoke((MethodInvoker)delegate
            {
                listBox1.Items.Clear();
                for (int i = 1; i < parts.Count; i++)
                {
                    listBox1.Items.Add(parts[i]);
                }
            });
        }

        private void getMessage()
        {
            try
            {
                while (true)
                {
                    serverStream = clientSocket.GetStream();
                    byte[] inStream = new byte[10025];
                    serverStream.Read(inStream, 0, inStream.Length);
                    List<string> parts = null;
                    if (!SocketConnected(clientSocket))
                    {
                        MessageBox.Show("Desconectado");
                        ctThread.Abort();
                        clientSocket.Close();
                        btnConnect.Enabled = true;
                    }
                    parts = (List<string>)ByteArrayToObject(inStream);
                    switch (parts[0])
                    {
                        case "userList":
                            getUsers(parts);
                            break;
                        case "gChat":
                            readData = "" + parts[1];
                            msg();
                            break;
                        case "pChat":
                            managePrivateChat(parts);
                            break;
                    }
                    if (readData[0].Equals('\0'))
                    {
                        readData = "Reconectar";
                        msg();
                        this.Invoke((MethodInvoker)delegate
                        {
                            btnConnect.Enabled = true;
                        });

                        ctThread.Abort();
                        clientSocket.Close();
                        break;
                    }
                    chat.Clear();
                }
            }
            catch (Exception ex)
            {
                ctThread.Abort();
                clientSocket.Close();
                btnConnect.Enabled = true;
                Console.WriteLine(ex);
            }
        }
        private void msg()
        {
            if (this.InvokeRequired)
                this.Invoke(new MethodInvoker(msg));
            else
                history.Text = history.Text + Environment.NewLine + " >> " + readData;
        }
        private void formMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult dialog = MessageBox.Show("Deseja mesmo sair? ", "Sair", MessageBoxButtons.YesNo);
            if (dialog == DialogResult.Yes)
            {
                try
                {
                    ctThread.Abort();
                    clientSocket.Close();
                }
                catch (Exception) 
                { 
                }
                Application.ExitThread();
            }
            else if (dialog == DialogResult.No)
            {
                e.Cancel = true;
            }
        }
        public void managePrivateChat(List<string> parts)
        {
            this.Invoke((MethodInvoker)delegate // To Write the Received data
            {
                if (parts[3].Equals("new"))
                {
                    formPrivate privateC = new formPrivate(parts[2], clientSocket, name);
                    nowChatting.Add(parts[2]);
                    privateC.Text = "Chat privado com " + parts[2];
                    privateC.Show();
                }
                else
                {
                    if (Application.OpenForms["formPrivate"] != null)
                    {
                        (Application.OpenForms["formPrivate"] as formPrivate).setHistory(parts[3]);
                    }
                }
            });
        }
        public byte[] ObjectToByteArray(object _Object)
        {
            using (var stream = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, _Object);
                return stream.ToArray();
            }
        }
        public Object ByteArrayToObject(byte[] arrBytes)
        {
            using (var memStream = new MemoryStream())
            {
                var binForm = new BinaryFormatter();
                memStream.Write(arrBytes, 0, arrBytes.Length);
                memStream.Seek(0, SeekOrigin.Begin);
                var obj = binForm.Deserialize(memStream);
                return obj;
            }
        }
        bool SocketConnected(TcpClient s)
        {
            bool flag = false;
            try
            {
                bool part1 = s.Client.Poll(10, SelectMode.SelectRead);
                bool part2 = (s.Available == 0);
                if (part1 && part2)
                {
                    indicator.BackColor = Color.Red;
                    this.Invoke((MethodInvoker)delegate
                    {
                        btnConnect.Enabled = true;
                    });
                    flag = false;
                }
                else
                {
                    indicator.BackColor = Color.Green;
                    flag = true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return flag;
        }
        private void privateChatToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex != -1)
            {
                String clientName = listBox1.GetItemText(listBox1.SelectedItem);
                chat.Clear();
                chat.Add("pChat");
                chat.Add(clientName);
                chat.Add(name);
                chat.Add("new");
                byte[] outStream = ObjectToByteArray(chat);
                serverStream.Write(outStream, 0, outStream.Length);
                serverStream.Flush();
                formPrivate privateChat = new formPrivate(clientName, clientSocket, name);
                nowChatting.Add(clientName);
                privateChat.Text = "Chat Privado " + clientName;
                privateChat.Show();
                chat.Clear();
            }
        }
        private void btnClr_Click(object sender, EventArgs e)
        {
            history.Clear();
        }
        private void history_TextChanged(object sender, EventArgs e)
        {
            history.SelectionStart = history.TextLength;
            history.ScrollToCaret();
        }
    }
}
