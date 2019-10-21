using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace Server
{
    public partial class Form1 : Form
    {
        TcpListener listener = new TcpListener(IPAddress.Parse(GetIP()), 0);
        TcpClient cliente;
        Dictionary<string, TcpClient> listaCliente = new Dictionary<string, TcpClient>();
        CancellationTokenSource cancellation = new CancellationTokenSource();
        List<string> chat = new List<string>();
        public Form1()
        {
            InitializeComponent();
        }

        private static string GetIP()
        {
            string strHostName = "";
            strHostName = System.Net.Dns.GetHostName();
            IPHostEntry ipEntry = System.Net.Dns.GetHostEntry(strHostName);
            IPAddress[] addr = ipEntry.AddressList;
            return addr[addr.Length - 1].ToString();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            cancellation = new CancellationTokenSource();
            startServer();
        }

        public void Log(string _log)
        {          
            this.Invoke((MethodInvoker)delegate
            {
                textBox1.AppendText(">>" + _log + Environment.NewLine);
            });
        }

        public async void startServer()
        {
            listener.Start();
            Log("Server Start, IP: " + listener.LocalEndpoint);
            Log("Esperando coneções");
            try
            {
                int contador = 0;
                while (true)
                {
                    contador++;
                    cliente = await Task.Run(() => listener.AcceptTcpClientAsync(), cancellation.Token);
                    byte[] name = new byte[50];
                    NetworkStream stre = cliente.GetStream(); 
                    stre.Read(name, 0, name.Length);
                    String user = Encoding.ASCII.GetString(name);
                    user = user.Substring(0, user.IndexOf("$"));
                    listaCliente.Add(user, cliente);
                    listBox1.Items.Add(user);
                    Log("Novo usuario: " + user + " - " + cliente.Client.RemoteEndPoint);
                    announce(user + " Conectou ", user, false);
                    await Task.Delay(1000).ContinueWith(t => sendUsersList());
                    var c = new Thread(() => ServerReceive(cliente, user));
                    c.Start();
                }
            }
            catch (Exception)
            {
                listener.Stop();
            }
        }

        public void announce(string msg, string uName, bool flag)
        {
            try
            {
                foreach (var aux in listaCliente)
                {
                    TcpClient broadcastSocket;
                    broadcastSocket = (TcpClient)aux.Value;
                    NetworkStream broadcastStream = broadcastSocket.GetStream();
                    Byte[] broadcastBytes = null;
                    if (flag)
                    {
                        chat.Add("gChat");
                        chat.Add(uName + ": " + msg);
                        broadcastBytes = ObjectToByteArray(chat);
                    }
                    else
                    {
                        chat.Add("gChat");
                        chat.Add(msg);
                        broadcastBytes = ObjectToByteArray(chat);
                    }
                    broadcastStream.Write(broadcastBytes, 0, broadcastBytes.Length);
                    broadcastStream.Flush();
                    chat.Clear();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
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

        public byte[] ObjectToByteArray(Object obj)
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }



        public void ServerReceive(TcpClient clientn, String user)
        {
            byte[] data = new byte[1000];
            while (true)
            {
                try
                {
                    NetworkStream stream = clientn.GetStream(); 
                    stream.Read(data, 0, data.Length);
                    List<string> parts = (List<string>)ByteArrayToObject(data);
                    switch (parts[0])
                    {
                        case "gChat":
                            this.Invoke((MethodInvoker)delegate
                            {
                                textBox1.Text += user + ": " + parts[1] + Environment.NewLine;
                            });
                            announce(parts[1], user, true);
                            break;
                        case "pChat":
                            privateChat(parts);
                            break;
                    }
                    parts.Clear();
                }
                catch (Exception)
                {
                    Log("Cliente Desconectado: " + user);
                    announce("Cliente Desconectado: " + user + "$", user, false);
                    listaCliente.Remove(user);
                    this.Invoke((MethodInvoker)delegate
                    {
                        listBox1.Items.Remove(user);
                    });
                    sendUsersList();
                    break;
                }
            }
        }

        private void disconnectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                TcpClient workerSocket = null;
                String clientName = listBox1.GetItemText(listBox1.SelectedItem);
                workerSocket = (TcpClient)listaCliente.FirstOrDefault(x => x.Key == clientName).Value; //find the client by user in dictionary
                workerSocket.Close();
            }
            catch (SocketException ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public void sendUsersList()
        {
            try
            {
                byte[] userList = new byte[1024];
                string[] clist = listBox1.Items.OfType<string>().ToArray();
                List<string> users = new List<string>();
                users.Add("userList");
                foreach (String name in clist)
                {
                    users.Add(name);
                }
                userList = ObjectToByteArray(users);
                foreach (var aux in listaCliente)
                {
                    TcpClient broadcastSocket;
                    broadcastSocket = (TcpClient)aux.Value;
                    NetworkStream broadcastStream = broadcastSocket.GetStream();
                    broadcastStream.Write(userList, 0, userList.Length);
                    broadcastStream.Flush();
                    users.Clear();
                }
            }
            catch (SocketException ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void privateChat(List<string> text)
        {
            try
            {
                byte[] byData = ObjectToByteArray(text);
                TcpClient workerSocket = null;
                workerSocket = (TcpClient)listaCliente.FirstOrDefault(x => x.Key == text[1]).Value; 
                NetworkStream stm = workerSocket.GetStream();
                stm.Write(byData, 0, byData.Length);
                stm.Flush();
            }
            catch (SocketException ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            textBox1.SelectionStart = textBox1.TextLength;
            textBox1.ScrollToCaret();
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            try
            {
                listener.Stop();
                Log("Server Stop");
                foreach (var aux in listaCliente)
                {
                    TcpClient broadcastSocket;
                    broadcastSocket = (TcpClient)aux.Value;
                    broadcastSocket.Close();
                }
            }
            catch (SocketException ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            listener.Stop();
        }
    }
}
