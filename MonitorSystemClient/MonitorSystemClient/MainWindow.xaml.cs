using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
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

        public MainWindow()
        {
            InitializeComponent();
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
            //for (int i = 0; i < 5; i++)
            //{
            //    MonitorCameraTreeModel tree = new MonitorCameraTreeModel();
            //    tree.Id = i.ToString();
            //    tree.Name = "Test" + i;
            //    tree.IsExpanded = true;
            //    for (int j = 0; j < 5; j++)
            //    {
            //        MonitorCameraTreeModel child = new MonitorCameraTreeModel();
            //        child.Id = i + "-" + j;
            //        child.Name = "Test" + child.Id;
            //        child.Parent = tree;
            //        tree.Children.Add(child);
            //    }
            //    treeList.Add(tree);
            //}
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

        }

        /// <summary>
        /// 获取treeview名称
        /// </summary>
        /// <param name="treeList"></param>
        /// <returns></returns>
        private List<string> GetIds(IList<MonitorCameraTreeModel> treeList)
        {
            StringBuilder ids = new StringBuilder();
            List<string> idNames = new List<string>();

            foreach (MonitorCameraTreeModel tree in treeList)
            {
                //ids.Append(tree.Id).Append(",");
                if(tree.Children.Count == 0)
                {
                    ids.Append(tree.VideoPath);
                    idNames.Add(ids.ToString());
                }
            }

           // return ids.ToString();
            return idNames;
        }

        /// <summary>
        /// 鼠标双击treeview
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ztvTest_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                IList<MonitorCameraTreeModel> treeList = ztvTest.CheckedItemsIgnoreRelation();
                List<string> nameList = GetIds(treeList);

                if (nameList.Count > 4 || nameList.Count == 0)
                {
                    MessageBox.Show("最多可以播放四个视频！");
                }
                else
                {
                    // 获取视频路径
                    //判断该视频是否在播放
                    if (mediaOne.Source == null)
                    {
                        mediaOne.Source = new Uri(nameList[0]);
                    }
                    else
                    {
                        if (!nameList.Exists(sa => sa == mediaOne.Source.ToString()))
                        {
                            mediaOne.Source = new Uri(nameList[0]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("出现错误，请重试!");
            }
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
    }
}
