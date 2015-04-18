using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using Emgu.Util;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Xml;

namespace MonitorSystemClient
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        #region 私有变量
        /// <summary>
        /// 连接服务器
        /// </summary>
        private InitInternet server;

        /// <summary>
        /// videoBox
        /// </summary>
        private ImageBox _videoBox = null;

        /// <summary>
        /// Video
        /// </summary>
        private Video video;
        #endregion

        #region 委托

        public delegate void PlayVideoDelegate(string videoPath);

        public delegate void CloseVideoDelegate(string videoPath);

        #endregion

        #region 线程
        private Thread myThread = null;
        private Thread closeThread = null;
        #endregion

        /// <summary>
        /// MainWidow
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }

        #region 实现方法

        /// <summary>
        /// 播放委托
        /// </summary>
        /// <param name="video"></param>
        public void PlayVideoInvoke(string videoPath)
        {
            if (myThread == null)
            {
                myThread = new Thread(PlayVideoThread);
                myThread.IsBackground = true;
                myThread.Start(videoPath);
                if (closeThread != null)
                {
                    if (closeThread.IsAlive)
                    {
                        closeThread.Abort();
                        closeThread = null;
                    }
                }
            }
            else
            {
                myThread.Resume();
            }
        }

        /// <summary>
        ///  关闭委托
        /// </summary>
        /// <param name="videoPath"></param>
        public void CloseVideoInvoke(string videoPath)
        {
            if (closeThread == null)
            {
                closeThread = new Thread(CloseVideoThread);
                closeThread.IsBackground = true;
                closeThread.Start(videoPath);

                if (myThread != null)
                {
                    if (myThread.IsAlive)
                    {
                        myThread.Abort();
                        myThread = null;
                    }
                }
            }
            else
            {
                closeThread.Resume();
            }
        }

        /// <summary>
        /// 播放线程
        /// </summary>
        /// <param name="obj"></param>
        private void PlayVideoThread(object obj)
        {
            PlayVideoDelegate d = new PlayVideoDelegate(PlayVideo);
            this.Dispatcher.BeginInvoke(d, (string)obj);
        }

        /// <summary>
        /// 关闭线程
        /// </summary>
        /// <param name="obj"></param>
        private void CloseVideoThread(object obj)
        {
            CloseVideoDelegate d = new CloseVideoDelegate(CloseVideo);
            this.Dispatcher.BeginInvoke(d, (string)obj);
        }

        /// <summary>
        /// 播放视频
        /// </summary>
        /// <param name="videoPath"></param>
        private void PlayVideo(string videoPath)
        {
            try
            {
                video.OpenCapture(videoPath, _videoBox);
                video.PlayVideo(server);
            }
            catch (MyException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 关闭视频
        /// </summary>
        /// <param name="videoPath"></param>
        private void CloseVideo(string videoPath)
        {
            try
            {
                video.CloseVideo(videoPath);
                _videoBox.Image = null;
            }
            catch (MyException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 加载窗体时激发
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                _videoBox = picHost.Child as Emgu.CV.UI.ImageBox;
                _videoBox.Height = 280;
                _videoBox.Width = 400;
                TvTestDataBind();
                video = new Video();
                server = new InitInternet();
                server.InitConnect();
            }
            catch (MyException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 数据绑定
        /// </summary>
        private void TvTestDataBind()
        {
            IList<MonitorCameraTreeModel> treeList = new List<MonitorCameraTreeModel>();

            try
            {
                // 加载xml文件
                XmlDocument doc = new XmlDocument();
                XmlReaderSettings settings = new XmlReaderSettings();
                // 忽略文档里面的注释
                settings.IgnoreProcessingInstructions = true;
                XmlReader reader = XmlReader.Create(@"../../data.xml", settings);
                doc.Load(reader);
                //doc.Load("data.xml");

                // 得到根节点videoInfo
                XmlNode xn = doc.SelectSingleNode("videoInfo");

                // 得到根节点的所有子节点
                XmlNodeList xnList = xn.ChildNodes;
                foreach (var xnl in xnList)
                {
                    MonitorCameraTreeModel camerModel = new MonitorCameraTreeModel();
                    // 将节点转换为元素，便于得到节点的属性值
                    XmlElement xe = (XmlElement)xnl;
                    // 得到Type和id两个属性的属性值
                    camerModel.Id = xe.GetAttribute("id").ToString();
                    // 得到Book节点的所有子节点
                    XmlNodeList xnl0 = xe.ChildNodes;
                    camerModel.Name = xnl0.Item(0).InnerText;
                    camerModel.IsChecked = Convert.ToBoolean(xnl0.Item(2).InnerText);
                    camerModel.IsExpanded = Convert.ToBoolean(xnl0.Item(3).InnerText);
                    if (xnl0.Item(4).HasChildNodes)
                    {
                        XmlNodeList xnl1 = xnl0.Item(4).ChildNodes;
                        foreach (var xnl2 in xnl1)
                        {
                            MonitorCameraTreeModel child = new MonitorCameraTreeModel();

                            XmlElement xe1 = (XmlElement)xnl2;
                            XmlNodeList xnl3 = xe1.ChildNodes;
                            child.Id = xnl3.Item(0).InnerText;
                            child.Name = xnl3.Item(1).InnerText;
                            child.Icon = xnl3.Item(2).InnerText;
                            child.VideoPath = xnl3.Item(3).InnerText;
                            child.Parent = camerModel;
                            camerModel.Children.Add(child);
                        }
                    }

                    treeList.Add(camerModel);
                }

                reader.Close();
            }
            catch (Exception)
            {
                throw new MyException("解析xml文件出错");
            }

            ztvTest.ItemsSourceData = treeList;
        }

        private void btnSelectId_Click(object sender, RoutedEventArgs e)
        {
            //IList<MonitorCameraTreeModel> treeList = ztvTest.CheckedItemsIgnoreRelation();

            //MessageBox.Show(GetIds(treeList));
            MessageBox.Show("有待解决！");
        }

        /// <summary>
        /// 窗口关闭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closed(object sender, EventArgs e)
        {
            try
            {
                if (server.IsConnect)
                {
                    server.CloseConnect();
                }
            }
            catch (MyException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        #endregion
    }
}
