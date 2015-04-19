using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using Emgu.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        /// videoBoxOne
        /// </summary>
        private ImageBox _videoBox1 = null;

        /// <summary>
        /// videoBoxTwo
        /// </summary>
        private ImageBox _videoBox2= null;

        /// <summary>
        /// videoBoxThree
        /// </summary>
        private ImageBox _videoBox3= null;

        /// <summary>
        /// videoBoxFour
        /// </summary>
        private ImageBox _videoBox4 = null;

        /// <summary>
        /// Video
        /// </summary>
        private Video video;

        /// <summary>
        /// video2
        /// </summary>
        private Video video2;

        /// <summary>
        /// video3
        /// </summary>
        private Video video3;

        /// <summary>
        /// video4
        /// </summary>
        private Video video4;

        /// <summary>
        /// backgroundwork
        /// </summary>
        private BackgroundWorker backgroundWorker;

        private BackgroundWorker backgroundWorker2;

        private BackgroundWorker backgroundWorker3;

        private BackgroundWorker backgroundWorker4;

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
            if (!backgroundWorker.IsBusy)
            {
                backgroundWorker.RunWorkerAsync(videoPath);
            }
            else if (!backgroundWorker2.IsBusy)
            {
                backgroundWorker2.RunWorkerAsync(videoPath);
            }
            else if (!backgroundWorker3.IsBusy)
            {
                backgroundWorker3.RunWorkerAsync(videoPath);
            }

            else if (!backgroundWorker4.IsBusy)
            {
                backgroundWorker4.RunWorkerAsync(videoPath);
            }
            else
            {
                throw new MyException("打开的视频以达到最大限度");
            }
        }

        /// <summary>
        ///  关闭委托
        /// </summary>
        /// <param name="videoPath"></param>
        public void CloseVideoInvoke(string videoPath)
        {
            if (video.VideoPath == videoPath)
            {
                backgroundWorker.CancelAsync();
            }
            else if (video2.VideoPath == videoPath)
            {
                backgroundWorker2.CancelAsync();
            }
            else if (video3.VideoPath == videoPath)
            {
                backgroundWorker3.CancelAsync();
            }
            else if (video4.VideoPath == videoPath)
            {
                backgroundWorker4.CancelAsync();
            }
            else
            {
                throw new MyException("未找到该视频路径");
            }
        }

        /// <summary>
        /// 播放视频
        /// </summary>
        /// <param name="videoPath"></param>
        /// <param name="worker"></param>
        private void PlayVideo(string videoPath, BackgroundWorker worker, Video _video,ImageBox imagebox)
        {
            try
            {
                if (_video.Cap == null)
                {
                    _video.GetCapture(videoPath);
                    while (true)
                    {
                        Image<Bgr, Byte> frame = _video.Cap.QueryFrame();
                        if (frame != null)
                        {
                            //为使播放顺畅，添加以下延时
                            System.Threading.Thread.Sleep((int)(1000.0 / video.VideoFps - 5));
                            if (server.IsConnect)
                            {
                                server.SendMessage(frame);
                                imagebox.Image = server.GetMessage();
                            }
                            else
                            {
                                imagebox.Image = frame;
                            }
                        }
                        else
                        {
                            imagebox.Image = null;
                            break;
                        }

                        if (worker.CancellationPending)
                        {
                            imagebox.Image = null;
                            break;
                        }
                    }
                }
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
        private void CloseVideo(Video _video, ImageBox imagebox)
        {
            try
            {
                if (_video != null)
                {
                    _video.CloseVideo();
                    imagebox.Image = null;
                    _video = null;
                }
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
                InitImageBox();

                TvTestDataBind();

                video = new Video();
                video2 = new Video();
                video3 = new Video();
                video4 = new Video();

                InitBackgroundworker();
                InitBackgroundworker2();
                InitBackgroundworker3();
                InitBackgroundWorker4();

                server = new InitInternet();
                server.InitConnect();
            }
            catch (MyException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 初始化ImageBox
        /// </summary>
        private void InitImageBox()
        {
             _videoBox1 = cam_ibox_One;
            _videoBox1.Height = 280;
            _videoBox1.Width = 400;

            _videoBox2 = cam_ibox_Two;
            _videoBox2.Height = 280;
            _videoBox2.Width = 400;

            _videoBox3 = cam_ibox_Three;
            _videoBox3.Height = 280;
            _videoBox3.Width = 400;

            _videoBox4 = cam_ibox_Four;
            _videoBox4.Height = 280;
            _videoBox4.Width = 400;
        }

        /// <summary>
        /// 初始化backgroundworker
        /// </summary>
        private void InitBackgroundworker()
        {
            backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork += backgroundWorker_DoWork;
            backgroundWorker.RunWorkerCompleted += backgroundWorker_RunWorkerCompleted;
            backgroundWorker.WorkerReportsProgress = true;
            backgroundWorker.WorkerSupportsCancellation = true;
        }

        /// <summary>
        /// 初始化backgroundworker2
        /// </summary>
        private void InitBackgroundworker2()
        {
            backgroundWorker2 = new BackgroundWorker();
            backgroundWorker2.DoWork += backgroundWorker2_DoWork;
            backgroundWorker2.RunWorkerCompleted += backgroundWorker2_RunWorkerCompleted;
            backgroundWorker2.WorkerReportsProgress = true;
            backgroundWorker2.WorkerSupportsCancellation = true;
        }

        /// <summary>
        /// 初始化backgroundworker3
        /// </summary>
        private void InitBackgroundworker3()
        {
            backgroundWorker3 = new BackgroundWorker();
            backgroundWorker3.DoWork += backgroundWorker3_DoWork;
            backgroundWorker3.RunWorkerCompleted += backgroundWorker3_RunWorkerCompleted;
            backgroundWorker3.WorkerReportsProgress = true;
            backgroundWorker3.WorkerSupportsCancellation = true;
        }

        /// <summary>
        /// 初始化backgroundworker4
        /// </summary>
        private void InitBackgroundWorker4()
        {
            backgroundWorker4 = new BackgroundWorker();
            backgroundWorker4.DoWork += backgroundWorker4_DoWork;
            backgroundWorker4.RunWorkerCompleted += backgroundWorker4_RunWorkerCompleted;
            backgroundWorker4.WorkerReportsProgress = true;
            backgroundWorker4.WorkerSupportsCancellation = true;
        }

        private void backgroundWorker4_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            CloseVideo(video4, _videoBox4);
        }

        private void backgroundWorker4_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            PlayVideo(e.Argument.ToString(), worker, video4, _videoBox4);
          
        }

        private void backgroundWorker3_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            CloseVideo(video3, _videoBox3);
        }

        private void backgroundWorker3_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            PlayVideo(e.Argument.ToString(), worker, video3, _videoBox3);
        }

        private void backgroundWorker2_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            CloseVideo(video2, _videoBox2);
        }

        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            PlayVideo(e.Argument.ToString(), worker, video2, _videoBox2);
          
        }

        /// <summary>
        /// 操作完成、取消、异常时执行的操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            CloseVideo(video, _videoBox1);
        }

        /// <summary>
        /// 调用RunWorkerAsync时发生
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            PlayVideo(e.Argument.ToString(), worker, video, _videoBox1);
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
                if (!video.IsClose)
                {
                    video.CloseVideo();
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
