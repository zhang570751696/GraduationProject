using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MonitorSysService
{
    public partial class Form1 : Form
    {
        /// <summary>
        ///  服务线程
        /// </summary>
        private Thread serverThread;

        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 窗体加载时发生
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {
            Control.CheckForIllegalCrossThreadCalls = true;
            // 开启服务线程
            serverThread = new Thread(new ThreadStart(this.StartReceive));
            serverThread.Start();
        }

        /// <summary>
        /// 服务关闭时发生
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            //InterService.ServerClose();
            //serverThread.Abort();
        }

        #region 功能方法
        private void StartReceive()
        {
           // InterService.ServerStart();
            Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            EndPoint point = new IPEndPoint(IPAddress.Any, 6666);
            sock.Bind(point);
            sock.Listen(10);

            while (true)
            {
                Socket client = sock.Accept();
                byte[] bitLen = new byte[20];
                int len = client.Receive(bitLen, bitLen.Length, SocketFlags.None);
                string contentStr = System.Text.Encoding.Default.GetString(bitLen, 0, len); ;
                long contlen = Convert.ToInt32(contentStr);
                MessageBox.Show(contentStr);

                //long size = 0;
                //MemoryStream ms = new MemoryStream();
                //while (size < contlen)
                //{
                //    byte[] bits = new byte[1024];
                //    int r = client.Receive(bits, bits.Length, SocketFlags.None);
                //    if (r <= 0)
                //    {
                //        break;
                //    }
                //    ms.Write(bits, 0, r);
                //    size += r;
                //}
                //client.Close();
                //ShowImage(ms);
                //ms.Close();
            }
        }

        private void ShowImage(MemoryStream ms)
        {
            Image img = Image.FromStream(ms);
            pictureBox1.Image = null;
            pictureBox1.Image = img;
        }

        #endregion

       
    }
}
