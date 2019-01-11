using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;

// This is the code for your desktop app.
// Press Ctrl+F5 (or go to Debug > Start Without Debugging) to run your app.

namespace AsyncSocketServer
{
    public partial class Form1 : Form
    {
        private Graphics g;
        private static Brush drawBrush = new SolidBrush(Color.Red);
        public Form1()
        {
            InitializeComponent();
            g = Graphics.FromHwnd(Handle);
        }

        private Socket sendsocket;

        private void goLive_Click(object sender, EventArgs e)
        {
            try
            {
                sendsocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                //The instantiation of socket, IP for 192.168.1.106, 10001 for Port
                //IPAddress ipAddress = 192.168.1.106;
                IPEndPoint ipendpiont = new IPEndPoint(IPAddress.Parse("https://www.google.com".Trim()), 10001);
                //IPEndPoint ipendpiont = new IPEndPoint(IPAddress.Any, 10001);
                sendsocket.Connect(ipendpiont);
                //Establishment of end point
                Thread th = new Thread(new ThreadStart(threadimage))
                {
                    IsBackground = true
                };
                th.Start();
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message);
                return;
            }
            Hide();    //Hidden form
        }

        private Bitmap GetScreen()
        {
            Bitmap bitmap = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            Graphics g = Graphics.FromImage(bitmap);
            g.CopyFromScreen(0, 0, 0, 0, bitmap.Size);
            return bitmap;
        }

        private void threadimage()
        {
            try
            {
                MemoryStream ms = new MemoryStream();
                GetScreen().Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);   //Here I use the BMP format
                byte[] b = ms.ToArray();

                sendsocket.Send(b);
                ms.Close();


            }
            catch (Exception)
            {
                // MessageBox.Show(ee.Message);//return;
            }

            Thread.Sleep(50);
            threadimage();
        }

        private void Form1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            var originalImage = Image.FromFile(@"D:\download\money6.jpg");
            BackgroundImage = originalImage;
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            g.FillEllipse(drawBrush, e.X, e.Y, 4, 4);
        }
    }
}
