using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;

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
        /// 发送图像数据到服务器端
        /// </summary>
        /// <param name="image"></param>
        public void SendMessage(Image<Bgr, Byte> image)
        {
            byte[] message = image.Bytes;

            //新建一个NetWorkStream对象发送数据
            NetworkStream netStream = new NetworkStream(clientSocket);
            
            // 向服务端发送message内容
            netStream.Write(message, 0, message.Length);
        }

        /// <summary>
        /// 接收服务端发送的图像数据
        /// </summary>
        /// <returns></returns>
        public Image<Bgr, Byte> GetMessage()
        {
            try
            {
                byte[] buf = new byte[20480];
                int size = clientSocket.Receive(buf, 0, buf.Length, SocketFlags.None);
                MemoryStream stream = null;
                Bitmap bitmap = null;
                try
                {
                    stream = new MemoryStream(buf, 0, size);
                   // bitmap = new Bitmap();
                    bitmap = new Bitmap(stream);
                }
                catch (Exception ex)
                {
                    //吃掉异常
                }
                finally
                {
                    stream.Close();
                }

                Image<Bgr, Byte> image = null;
                if (bitmap != null)
                {
                    image = new Image<Bgr, byte>(bitmap);
                }
                return image;
            }
            catch(Exception ex)
            {
                throw new MyException("接收出现错误");
            }
        }

        #endregion
    }
}
