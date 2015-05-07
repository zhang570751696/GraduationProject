using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace MonitorSystemClient
{
    /// <summary>
    /// 与服务器连接初始化
    /// </summary>
    class InitInternet
    {
        #region 私有变量
        /// <summary>
        /// 客户端socket
        /// </summary>
        private Socket clientSocket;

        /// <summary>
        /// 是否连接成功
        /// </summary>
        private bool isConnect;

        #endregion

        #region 公共变量

        /// <summary>
        /// 是否连接成功
        /// </summary>
        public bool IsConnect
        {
            get { return this.isConnect; }
            set { this.isConnect = value; }
        }

        #endregion

        #region 构造方法
        /// <summary>
        /// 默认构造方法
        /// </summary>
        public InitInternet()
        {
            this.isConnect = false;
        }

        #endregion 

        #region 实现方法
        /// <summary>
        /// 初始化连接
        /// </summary>
        /// <returns></returns>
        public void InitConnect()
        {
            try
            {
                IPAddress remoteHost = IPAddress.Parse("127.0.0.1");
                // IP地址跟端口的组合
                IPEndPoint iep = new IPEndPoint(remoteHost, 8888);
                // 把地址绑定到Socket
                clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                clientSocket.Connect(iep);
                this.isConnect = true;
            }
            catch (Exception ex)
            {
                throw new MyException("连接服务器失败！");
            }
        }

        /// <summary>
        /// 关闭连接
        /// </summary>
        public void CloseConnect()
        {
            try
            {
                clientSocket.Shutdown(SocketShutdown.Both);
                clientSocket.Close();
                this.isConnect = false;
            }
            catch (Exception ex)
            {
                throw new MyException("与服务器断开连接失败！");
            }
        }

        /// <summary>
        /// 仅供测试使用(模拟网络传输)
        /// </summary>
        /// <param name="image"> 测试图像</param>
        /// <returns>解析后的图像</returns>
        public Image<Bgr, Byte> getImage(Image<Bgr, Byte> image)
        {
            Bitmap bit = image.ToBitmap();
            MemoryStream ms = null;
            try
            {
                ms = new MemoryStream();
                bit.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                byte[] message = ms.GetBuffer();
                ms.Close();
                MemoryStream ms1 = new MemoryStream(message);
                Bitmap bm = (Bitmap)Image.FromStream(ms1);

                Image<Bgr, Byte> im = new Image<Bgr, byte>(bm);
                ms1.Close();
                return im;
            }
              
            catch(Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// 发送图像数据到服务器端
        /// </summary>
        /// <param name="image">待传输的图像</param>
        public void SendMessage(Image<Bgr, Byte> image)
        {
            try
            {
                Bitmap bit = image.ToBitmap();
                MemoryStream ms = null;
                ms = new MemoryStream();
                bit.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                byte[] message = ms.GetBuffer();
                long length = ms.Length;
                ms.Close();

                // 向服务端发送图像流的长度大小
                //int sendLength = 1024;
                //int size = message.Length;
                // 图片大小
                byte[] imageSize = System.Text.Encoding.Unicode.GetBytes(length.ToString());

                NetworkStream netstream = new NetworkStream(clientSocket);
                netstream.Write(imageSize, 0, imageSize.Length);


                MemoryStream ms1 = new MemoryStream(message);
                // 循环发送文件内容
                while (true)
                {
                    byte[] bits = new byte[1024];
                    int r = ms1.Read(bits, 0, bits.Length);
                    if (r <= 0) break;
                    clientSocket.Send(bits, r, SocketFlags.None);
                }

                ms1.Position = 0;
                ms1.Close();
                
            }
            catch (Exception ex)
            {
                throw new MyException("发送图像数据失败");
            }
        }

        /// <summary>
        /// 接收服务端发送的图像数据
        /// </summary>
        /// <returns>接收到的图像</returns>
        public Image<Bgr, Byte> GetMessage()
        {
            try
            {
                byte[] buf = new byte[20];
                int contlen = clientSocket.Receive(buf, 0, buf.Length, SocketFlags.None);
                int cont = BitConverter.ToInt32(buf, 0);
                int size = 0;
                MemoryStream stream = new MemoryStream();
                while (size < cont)
                {
                    byte[] bits = new byte[1024];
                    int r = clientSocket.Receive(bits, 0, bits.Length, SocketFlags.None);
                    if (r <= 0) break;
                    stream.Write(bits, 0, r);
                    size += r;
                }

                //MemoryStream ms1 = new MemoryStream(buf);
                Bitmap bm = (Bitmap)Image.FromStream(stream);
                Image<Bgr, Byte> im = new Image<Bgr, byte>(bm);
                stream.Close();

                return im;
            }
            catch(Exception ex)
            {
                throw new MyException("接收出现错误");
            }
        }

        #endregion
    }
}
