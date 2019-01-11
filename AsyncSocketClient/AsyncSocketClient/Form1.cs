using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;


namespace AsyncSocketClient
{
    public partial class Form1 : Form
    {
        private Graphics g;
        private static Brush drawBrush = new SolidBrush(Color.Red);
        private static Brush wipeBrush = new SolidBrush(SystemColors.Control);
        private static Brush curBrush = wipeBrush;

        public Form1()
        {
            InitializeComponent();
            g = Graphics.FromHwnd(Handle);
        }

        private Socket hostSocket;
        private Thread thread;
        private string localIP = string.Empty;//"127.0.0.1";
        private string computrHostName = string.Empty;

        private void Form1_Load(object sender, EventArgs e)
        {
            computrHostName = Dns.GetHostName();
            IPHostEntry hostname = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in hostname.AddressList)
            {
                if (ip.AddressFamily.ToString() == "InterNetwork")
                {
                    localIP = ip.ToString();
                }
            }
            Text = Text + " | " + localIP;

            //liveScreen_Click(sender, e);
        }

        //private void liveScreen_Click(object sender, EventArgs e)
        //{
        //    connectSocket();
        //}

        private void connectSocket()
        {
            Socket receiveSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint hostIpEndPoint = new IPEndPoint(IPAddress.Parse(localIP), 10001);
            //Connection node
            receiveSocket.Bind(hostIpEndPoint);
            receiveSocket.Listen(10);
            MessageBox.Show("started to receive...");
            hostSocket = receiveSocket.Accept();
            thread = new Thread(new ThreadStart(trreadimage))
            {
                IsBackground = true
            };
            thread.Start();
        }

        private void trreadimage()
        {
            int dataSize;
            string imageName = "Image-" + System.DateTime.Now.Ticks + ".JPG";
            try
            {

                dataSize = 0;
                byte[] b = new byte[1024 * 10000];  //Picture of great
                dataSize = hostSocket.Receive(b);
                if (dataSize > 0)
                {
                    MemoryStream ms = new MemoryStream(b);
                    Image img = Image.FromStream(ms);
                    img.Save(imageName, System.Drawing.Imaging.ImageFormat.Jpeg);
                    //panel1.BackgroundImage = img;
                    //pictureBox1.Image = img;
                    BackgroundImage = img;
                    ms.Close();
                }

            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message);
                //thread.Abort();
            }
            System.Threading.Thread.Sleep(50);
            trreadimage();
        }

        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            curBrush = curBrush == drawBrush ? wipeBrush : drawBrush;
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            g.FillEllipse(curBrush, e.X, e.Y, 4, 4);
        }

        private void Form1_MouseEnter(object sender, EventArgs e)
        {
            connectSocket();
        }
    }
}
