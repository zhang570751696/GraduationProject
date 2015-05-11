using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace MonitorSystemClient
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        #region 私有变量

        /// <summary>
        /// videoBoxOne
        /// </summary>
        private ImageBox _videoBox1 = null;

        /// <summary>
        /// Video
        /// </summary>
        private Video video;

        /// <summary>
        /// backgroundwork
        /// </summary>
        private BackgroundWorker backgroundWorker;

        /// <summary>
        /// 是否全屏
        /// </summary>
        private bool boxOneIsFull;

        /// <summary>
        /// 是否开启检测
        /// </summary>
        private bool IsOpenChecked;

        /// <summary>
        /// WPF的定时器使用DispatchTimer类对象
        /// </summary>
        private DispatcherTimer dTimer;

        /// <summary>
        /// 开始检测时间
        /// </summary>
        private DateTime startTime;

        /// <summary>
        /// 初始化图片加载位置
        /// </summary>
        private string picPath = @"../../Images/Display/display.png";

        #endregion

        #region 构造方法

        /// <summary>
        /// MainWidow
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }
        #endregion

        #region 实现方法

        /// <summary>
        /// 播放委托
        /// </summary>
        /// <param name="model">数据模型</param>
        public void PlayVideoInvoke(MonitorCameraTreeModel model)
        {
            if (!backgroundWorker.IsBusy)
            {
                DMessagePie.OnDisplayEvent("打开视频:" + model.Name);
                this.Lable_VideoPath.Content = model.VideoPath;
                this.Lable_VideoName.Content = model.Name;
                
                //定时器使用委托
                dTimer.Tick += new EventHandler(dTimer_Tick);

                // 设置时间 TimeSpan(时，分，秒)
                dTimer.Interval = new TimeSpan(0, 0, 1);

                startTime = DateTime.Now;
                
                //启动DispatcherTimer对象dTime
                dTimer.Start();

                backgroundWorker.RunWorkerAsync(model.VideoPath);
            }
            else
            {
                throw new MyException("已经打开了视频，请先关闭视频");
            }
        }

        /// <summary>
        /// 定时器委托
        /// </summary>
        /// <param name="sender">对象</param>
        /// <param name="e">事件</param>
        private void dTimer_Tick(object sender, EventArgs e)
        {
            DateTime currentTime = DateTime.Now;
            TimeSpan ts = currentTime - startTime;
            this.Lable_PlayTime.Content = string.Format("{0}:{1}:{2}", ts.Hours, ts.Minutes, ts.Seconds);
        }

        /// <summary>
        ///  关闭委托
        /// </summary>
        /// <param name="videoPath">视频路径</param>
        public void CloseVideoInvoke(MonitorCameraTreeModel model)
        {
            if (video.VideoPath == model.VideoPath && backgroundWorker.IsBusy)
            {
                DMessagePie.OnDisplayEvent("关闭视频：" + model.Name);
                this.Lable_VideoName.Content = "未选择视频";
                this.Lable_VideoPath.Content = "未选择视频";
                this.Lable_PlayTime.Content = "00:00:00";
                dTimer.Stop();

                this.IsOpenChecked = false;
                this.buttonChecked.Content = "开启检测";
                this.deteInfo.Text = "未开启检测";
                backgroundWorker.CancelAsync();
            }
            else
            {
                // 啥都不用管
                // throw new MyException("未找到该视频路径");
            }
        }

        /// <summary>
        /// 播放视频
        /// </summary>
        /// <param name="videoPath">视频路径</param>
        private void PlayVideo(string videoPath)
        {
            try
            {
                Capture cap = video.GetCapture(videoPath);
                int count = 1;
                while (!backgroundWorker.CancellationPending)
                {
                    Image<Bgr, Byte> frame = cap.QueryFrame();
                    if (frame != null)
                    {
                        frame = frame.Resize(600, 500, Emgu.CV.CvEnum.INTER.CV_INTER_LINEAR);
                        // 如果开启检测
                        if (IsOpenChecked)
                        {
                            if ((count % 3) == 0)
                            {
                                MDeteInfo model = HeadDet.GetHead(frame);
                                _videoBox1.Image = model.Frame;
                                backgroundWorker.ReportProgress(1, model.HeadCount);
                                count = 1;
                            }
                            count++;
                        }
                        else 
                        {
                            //为使播放顺畅，添加以下延时
                            System.Threading.Thread.Sleep((int)(1000.0 / video.VideoFps - 5));
                            _videoBox1.Image = frame;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
                this.DisplayPic(this.picPath);
            }
            catch (Exception ex)
            {
                this.CloseVideo(video);
                throw new MyException("视频解析错误!");
            }
        }

        /// <summary>
        /// 显示图片
        /// </summary>
        /// <param name="picPath">图片路径</param>
        private void DisplayPic(string picPath)
        {
            IntPtr image = CvInvoke.cvLoadImage(picPath, Emgu.CV.CvEnum.LOAD_IMAGE_TYPE.CV_LOAD_IMAGE_ANYCOLOR);
            Image<Bgr, Byte> dest = new Image<Bgr, byte>(CvInvoke.cvGetSize(image));
            CvInvoke.cvCopy(image, dest, IntPtr.Zero);
            _videoBox1.Image = HeadDet.GetHead(dest).Frame;  
        }

        /// <summary>
        /// 关闭视频
        /// </summary>
        /// <param name="videoPath">视频路径</param>
        private void CloseVideo(Video _video)
        {
            try
            {
                if (_video != null)
                {
                    _video.CloseVideo();
                    _video = null;
                }
                if (!backgroundWorker.CancellationPending)
                {
                    backgroundWorker.CancelAsync();
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
        /// <param name="sender">对象</param>
        /// <param name="e">事件</param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                InitImageBox();
                TvTestDataBind();
                video = new Video();
                dTimer = new DispatcherTimer(); 
                IsOpenChecked = false;
                DMessagePie.DisplayEvent += new DMessagePie.DisplayMessageDelegate(this.DisplayMessage);
                DMessagePie.OnDisplayEvent("打开视频监控系统");
                
                InitBackgroundworker();
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
            _videoBox1.Height = 1000;
            _videoBox1.Width = 600;
            this.DisplayPic(this.picPath);
            boxOneIsFull = false;
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
            backgroundWorker.ProgressChanged += backgroundWorker_ProgressChanged;
        }

        /// <summary>
        /// 报告进度
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            int message =(int)e.UserState;
            ShowDeteInfo("检测出人头:" + message.ToString()); 
        }

        /// <summary>
        /// 操作完成、取消、异常时执行的操作
        /// </summary>
        /// <param name="sender">对象</param>
        /// <param name="e">事件</param>
        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            CloseVideo(video);
        }

        /// <summary>
        /// 调用RunWorkerAsync时发生
        /// </summary>
        /// <param name="sender">对象</param>
        /// <param name="e">事件</param>
        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                BackgroundWorker worker = sender as BackgroundWorker;
                string path = e.Argument.ToString();
                if (path.ToUpper().Contains("JPG") || path.ToUpper().Contains("JPEG"))
                {
                    this.DisplayPic(path);
                }
                else
                {
                    this.PlayVideo(path);
                }
            }
            catch (MyException ex)
            {
                backgroundWorker.CancelAsync();
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 数据绑定
        /// </summary>
        public void TvTestDataBind()
        {
            ztvTest.ItemsSourceData = OperaXml.GetXmlData();
        }

        /// <summary>
        /// 窗口关闭
        /// </summary>
        /// <param name="sender">对象</param>
        /// <param name="e">事件</param>
        private void Window_Closed(object sender, EventArgs e)
        {
            if (backgroundWorker.IsBusy)
            {
                backgroundWorker.CancelAsync();
            }
            if (video != null)
            {
                video.CloseVideo();
            }
        }

        /// <summary>
        /// 双击图像
        /// </summary>
        /// <param name="sender">事件</param>
        /// <param name="e">对象</param>
        private void BoxOneDoubleClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            MessageBox.Show("该功能有待实现");
        }

        /// <summary>
        /// 点击开启检测
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void OpenCheckedClick(object sender, RoutedEventArgs e)
        {
            if (_videoBox1.Image == null || !backgroundWorker.IsBusy)
            {
                MessageBox.Show("请先打开视频");
                return;
            }

            IsOpenChecked = !IsOpenChecked;
           
            // 如果已经开始进行检测了
            if (IsOpenChecked)
            {
                DMessagePie.OnDisplayEvent("打开视频检测");
                this.buttonChecked.Content = "关闭检测";
            }
            else
            {
                DMessagePie.OnDisplayEvent("关闭视频检测");
                this.buttonChecked.Content = "开启检测";
            }
        }

        /// <summary>
        /// 现实消息
        /// </summary>
        /// <param name="message"></param>
        private void DisplayMessage(string message)
        {
            string dateTime = DateTime.Now.ToString();
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("{0}", this.infoTxt.Text);

                if (sb.Length < 250)
                {
                    sb.AppendFormat("\r\n{0}", dateTime + "," + message);
                }
                else
                {
                    sb = new StringBuilder(dateTime + "," + message);
                }
                this.infoTxt.Text = sb.ToString();
            }
            catch (Exception ex)
            {
                this.infoTxt.Text = dateTime + " 显示出现异常:" + ex.Message;
            }
        }

        /// <summary>
        /// 检测信息显示
        /// </summary>
        /// <param name="info">要显示的信息</param>
        private void ShowDeteInfo(string info)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("{0}", this.deteInfo.Text);

                if (sb.Length < 80)
                {
                    sb.AppendFormat("\r\n{0}", info);
                }
                else
                {
                    sb = new StringBuilder(info);
                }
                this.deteInfo.Text = sb.ToString();
            }
            catch (Exception ex)
            {
                this.deteInfo.Text = " 显示出现异常:" + ex.Message;
            }
        }
      

        #endregion
    }
}
