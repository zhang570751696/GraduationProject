using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MonitorSysService
{
    /// <summary>
    /// 通信中往线程传递model
    /// </summary>
    class MThreadParaModel
    {
        #region 私有成员
        /// <summary>
        /// 服务器端socket
        /// </summary>
        private Socket serverSocket;

        /// <summary>
        /// 客户端socket
        /// </summary>
        private Socket clientSocket;

        /// <summary>
        /// 接收阶段还是发送阶段
        /// </summary>
        private bool isRecv;
       
        #endregion

        #region 默认构造方法

        public MThreadParaModel()
        { }

        #endregion

        #region 公共成员

        /// <summary>
        /// 服务器端socket
        /// </summary>
        public Socket ServerSocket
        {
            get { return this.serverSocket; }
            set { this.serverSocket = value; }
        }

        /// <summary>
        /// 客户端socket
        /// </summary>
        public Socket ClientSocket
        {
            get { return this.clientSocket; }
            set { this.clientSocket = value; }
        }

        /// <summary>
        /// 接收阶段还是发送阶段
        /// </summary>
        public bool IsRecv
        {
            get { return this.isRecv; }
            set { this.isRecv = value; }
        }

        #endregion
    }
}
