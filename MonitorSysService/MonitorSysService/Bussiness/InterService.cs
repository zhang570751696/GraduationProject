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
using System.Threading;
using System.Threading.Tasks;

namespace MonitorSysService
{
    class InterService
    {
        /// <summary>
        /// 接收委托
        /// </summary>
        private delegate void RecDelegate(Object obj);

        /// <summary>
        /// 发送线程
        /// </summary>
        /// <param name="obj"></param>
        private delegate void SenDelegate(Object obj);

        /// <summary>
        /// 服务对象
        /// </summary>
        private static InterService server;

        /// <summary>
        /// socket套接字
        /// </summary>
        private static Socket socket;

        /// <summary>
        /// 接收到的图片
        /// </summary>
        private static Image<Bgr, Byte> _image;

        private static MThreadParaModel threadModel;

        /// <summary>
        /// 服务开启
        /// </summary>
        public static void ServerStart()
        {
            if (socket == null)
            { 
                InitServer();
            }

            while (true)
            {
                // 确认连接
                Socket client = socket.Accept();

                // 获得客户端节点对象
                IPEndPoint clientep = (IPEndPoint)client.RemoteEndPoint;

                //if (client != null || clientep != null)
                //{
                //    threadModel = new MThreadParaModel();
                //    threadModel.ServerSocket = socket;
                //    threadModel.ClientSocket = client;
                //    threadModel.IsRecv = true;
                    
                //    Thread recThread = new Thread(ReciveThread);
                //    recThread.Start(threadModel);
                    
                //    Thread senThread = new Thread(SendThread);
                //    senThread.Start(threadModel);
                //}
            }
        }

        /// <summary>
        /// 服务关闭
        /// </summary>
        public static void ServerClose()
        {
            // 关闭套接字
            socket.Close();
            socket = null;
        }

        /// <summary>
        /// 服务初始化
        /// </summary>
        private static void InitServer()
        {
            // 创建一个网格端点
            IPEndPoint ipep = new IPEndPoint(IPAddress.Any, int.Parse("8888"));

            // 创建一个套接字
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
 
            // 绑定套接字到端口
            socket.Bind(ipep);

            // 开始监听（并堵塞该线程）
            socket.Listen(10);
        }

        /// <summary>
        /// 接收线程
        /// </summary>
        private static void ReciveThread(Object obj)
        {
            RecDelegate r = new RecDelegate(ReciveDelegate);
            r.Invoke(obj);
        }

        /// <summary>
        /// 接收委托实现
        /// </summary>
        private static void ReciveDelegate(Object obj)
        {
            MThreadParaModel threadparaModel = obj as MThreadParaModel;
            try
            {
                if (threadparaModel.IsRecv)
                {
                    byte[] buf = new byte[20];
                    int contlen = threadparaModel.ClientSocket.Receive(buf, 0, buf.Length, SocketFlags.None);
                    int cont = BitConverter.ToInt32(buf, 0);
                    int size = 0;
                    MemoryStream stream = new MemoryStream();
                    while (size < cont)
                    {
                        byte[] bits = new byte[1024];
                        int r = threadparaModel.ClientSocket.Receive(bits, bits.Length, SocketFlags.None);
                        if (r <= 0) break;
                        stream.Write(bits, 0, r);
                        size += r;
                    }

                    Bitmap bm = (Bitmap)Image.FromStream(stream);
                    _image = new Image<Bgr, byte>(bm);
                    stream.Close();

                }
            }
            catch (Exception ex)
            {
                throw new MyException("接收失败");
            }

            // 接收完后进行图片处理(未写)
        }

        /// <summary>
        /// 发送线程
        /// </summary>
        private static void SendThread(Object obj)
        {
            SenDelegate s = new SenDelegate(SendDelegate);
            s.Invoke(obj);
        }

        /// <summary>
        /// 发送委托实现
        /// </summary>
        private static void SendDelegate(Object obj)
        {
            MThreadParaModel threadparaModle = obj as MThreadParaModel;
            while (true)
            {
                try
                {
                    if (_image != null)
                    {
                        Bitmap bit = _image.ToBitmap();
                        MemoryStream ms = null;
                        ms = new MemoryStream();
                        bit.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                        byte[] message = ms.GetBuffer();
                        

                        // 向服务端发送图像流的长度大小
                        int size = message.Length;
                        // 图片大小
                        byte[] imageSize = System.Text.Encoding.Unicode.GetBytes(size.ToString());

                        //新建一个NetWorkStream对象发送数据
                        //NetworkStream netStream = new NetworkStream(threadparaModle.ClientSocket);

                        //netStream.Write(imageSize, 0, imageSize.Length);
                        threadparaModle.ClientSocket.Send(imageSize);

                        // 循环发送文件内容
                        while (true)
                        {
                            byte[] bits = new byte[1024];
                            int r = ms.Read(bits, 0, bits.Length);
                            if (r <= 0) break;
                            threadparaModle.ClientSocket.Send(bits, r, SocketFlags.None);
                        }
                        threadparaModle.ClientSocket.Close();
                        ms.Position = 0;
                        ms.Close();
                    }
                    break;
                }
                catch (Exception ex)
                {
                }
            }
        }

    }
}
