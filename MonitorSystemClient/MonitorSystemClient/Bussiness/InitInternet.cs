using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MonitorSystemClient
{
    /// <summary>
    /// 与服务器连接初始化
    /// </summary>
    class InitInternet
    {
        /// <summary>
        /// 客户端socket
        /// </summary>
        private Socket clientSocket;

        /// <summary>
        /// 是否连接成功
        /// </summary>
        private bool isConnect;

        /// <summary>
        /// 是否连接成功
        /// </summary>
        public bool IsConnect
        {
            get { return this.isConnect; }
            set { this.isConnect = value; }
        }

        /// <summary>
        /// 默认构造方法
        /// </summary>
        public InitInternet()
        {
            this.isConnect = false;
        }

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
        public  void CloseConnect()
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
    }
}
