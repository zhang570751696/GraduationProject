using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using Emgu.Util;
using System;
using System.Collections.Generic;
using System.Text;
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
        /// <summary>
        /// 连接服务器
        /// </summary>
        private InitInternet server;

        /// <summary>
        /// videoBox
        /// </summary>
        private ImageBox _videoBox = null;

        /// <summary>
        /// 显示图像委托
        /// </summary>
        private delegate void DelegateShowVideo();

        /// <summary>
        /// MainWidow
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 播放视频
        /// </summary>
        /// <param name="videoPath"></param>
        public void PlayVideo(string videoPath)
        {
            try
            {
                Video.OpenCapture(videoPath);
                _videoBox.Image = Video.PlayVideo(server);
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
        public void CloseVideo(string videoPath)
        {
            try
            {
                Video.CloseVideo(videoPath);
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
                //_pictureBox = picHost.Child as System.Windows.Forms.PictureBox;
                //_pictureBox.ImageLocation = "D:\\1.jpeg";
                _videoBox = picHost.Child as Emgu.CV.UI.ImageBox;
                TvTestDataBind();
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

        ///// <summary>
        ///// 获取treeview名称
        ///// </summary>
        ///// <param name="treeList"></param>
        ///// <returns></returns>
        //private List<string> GetIds(IList<MonitorCameraTreeModel> treeList)
        //{
        //    StringBuilder ids = new StringBuilder();
        //    List<string> idNames = new List<string>();

        //    foreach (MonitorCameraTreeModel tree in treeList)
        //    {
        //        //ids.Append(tree.Id).Append(",");
        //        if(tree.Children.Count == 0)
        //        {
        //            ids.Append(tree.VideoPath);
        //            idNames.Add(ids.ToString());
        //        }
        //    }

        //   // return ids.ToString();
        //    return idNames;
        //}

        ///// <summary>
        ///// 鼠标双击treeview
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void ztvTest_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        //{
        //    try
        //    {
        //        IList<MonitorCameraTreeModel> treeList = ztvTest.CheckedItemsIgnoreRelation();
        //        List<string> nameList = GetIds(treeList);

        //        if (nameList.Count > 4)
        //        {
        //            MessageBox.Show("最多可以播放四个视频！");
        //        }
        //        else if (nameList.Count == 0)
        //        {
        //            MessageBox.Show("未选择视频");
        //        }
        //        else
        //        {
        //            try
        //            {
        //                // 获取视频路径
        //                capture = new Capture(nameList[0]);
        //                VideoFps = (int)CvInvoke.cvGetCaptureProperty(capture, Emgu.CV.CvEnum.CAP_PROP.CV_CAP_PROP_FPS);
        //                _videoBox.Image = null;
        //                DelegateShowVideo d = ProcessFrame;
        //                this.Dispatcher.Invoke(d);
        //            }
        //            catch (NullReferenceException)
        //            {
        //                MessageBox.Show("不能打开" + nameList[0]);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("出现错误，请重试!" + ex.Message);
        //    }
        //}

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
    }
}
