using SelectFile;
using System;
using System.Windows;

namespace MonitorSystemClient
{
    /// <summary>
    /// VideoView.xaml 的交互逻辑
    /// </summary>
    public partial class VideoView : Window
    {
        /// <summary>
        /// xmlModel
        /// </summary>
        public XmlModel Xmlmodel;

        /// <summary>
        /// videoView
        /// </summary>
        private SelectPathControl selectFile;

        public VideoView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 确定点击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnOKClick(object sender, RoutedEventArgs e)
        {

            int videoParent = -1;
            try
            {
                videoParent = this.combox.SelectedIndex;
            }
            catch (Exception ex)
            {
                // 这里是未点击使用默认值
            }

            if(string.IsNullOrEmpty(videoname.Text) )
            {
                MessageBox.Show("请填写视频名称!");
                return;
            }
            if(string.IsNullOrEmpty(selectFileName.Path))
            {
                MessageBox.Show("请选择视频路径");
                return;
            }

            if (Xmlmodel == null)
            {
                Xmlmodel = new XmlModel();
            }
            if (videoParent == 0 || videoParent == -1)
            {
                Xmlmodel.ParentName = XmlType.AddLocalData;
            }
            else
            {
                Xmlmodel.ParentName = XmlType.AddinterData;
            }

            Xmlmodel.ChildName = videoname.Text;
            Xmlmodel.Videopath = selectFile.Path;
            this.Close();
        }

        /// <summary>
        /// 点击取消
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnCancleClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 添加窗口加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddWindow_Load(object sender, RoutedEventArgs e)
        {
            selectFile = selectFileName;
        }
    }
}
