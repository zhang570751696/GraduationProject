using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace MonitorSystemClient
{
    /// <summary>
    /// TreeView.xaml 的交互逻辑
    /// </summary>
    public partial class TreeView : UserControl
    {
        #region 私有变量属性
        /// <summary>
        /// 控件数据
        /// </summary>
        private IList<MonitorCameraTreeModel> _itemsSourceData;
        #endregion

        #region 构造
        /// <summary>
        /// 构造
        /// </summary>
        public TreeView()
        {
            InitializeComponent();
        }

        #endregion

        #region 实现方法
       
        /// <summary>
        /// 控件数据
        /// </summary>
        public IList<MonitorCameraTreeModel> ItemsSourceData
        {
            get { return _itemsSourceData; }
            set
            {
                _itemsSourceData = value;
                tvZsmTree.ItemsSource = _itemsSourceData;
            }
        }

        /// <summary>
        /// 设置对应Id的项为选中状态
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int SetCheckedById(string id, IList<MonitorCameraTreeModel> treeList)
        {
            foreach (var tree in treeList)
            {
                if (tree.Id.Equals(id))
                {
                    tree.IsChecked = true;
                    return 1;
                }
                if (SetCheckedById(id, tree.Children) == 1)
                {
                    return 1;
                }
            }

            return 0;
        }
        /// <summary>
        /// 设置对应Id的项为选中状态
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int SetCheckedById(string id)
        {
            foreach (var tree in ItemsSourceData)
            {
                if (tree.Id.Equals(id))
                {
                    tree.IsChecked = true;
                    return 1;
                }
                if (SetCheckedById(id, tree.Children) == 1)
                {
                    return 1;
                }
            }

            return 0;
        }

        /// <summary>
        /// 获取选中项
        /// </summary>
        /// <returns></returns>
        public IList<MonitorCameraTreeModel> CheckedItemsIgnoreRelation()
        {
            return GetCheckedItemsIgnoreRelation(_itemsSourceData);
        }

        /// <summary>
        /// 私有方法，忽略层次关系的情况下，获取选中项
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private IList<MonitorCameraTreeModel> GetCheckedItemsIgnoreRelation(IList<MonitorCameraTreeModel> list)
        {
            IList<MonitorCameraTreeModel> treeList = new List<MonitorCameraTreeModel>();
            foreach (var tree in list)
            {
                if (tree.IsChecked)
                {
                    treeList.Add(tree);
                }
                foreach (var child in GetCheckedItemsIgnoreRelation(tree.Children))
                {
                    treeList.Add(child);
                }
            }
            return treeList;
        }

        /// <summary>
        /// 选中所有子项菜单事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuSelectAllChild_Click(object sender, RoutedEventArgs e)
        {
            if (tvZsmTree.SelectedItem != null)
            {
                MonitorCameraTreeModel tree = (MonitorCameraTreeModel)tvZsmTree.SelectedItem;
                tree.IsChecked = true;
                tree.SetChildrenChecked(true);
            }
        }

        /// <summary>
        /// 全部展开菜单事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuExpandAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (MonitorCameraTreeModel tree in tvZsmTree.ItemsSource)
            {
                tree.IsExpanded = true;
                tree.SetChildrenExpanded(true);
            }
        }

        /// <summary>
        /// 全部折叠菜单事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuUnExpandAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (MonitorCameraTreeModel tree in tvZsmTree.ItemsSource)
            {
                tree.IsExpanded = false;
                tree.SetChildrenExpanded(false);
            }
        }

        /// <summary>
        /// 全部选中事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuSelectAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (MonitorCameraTreeModel tree in tvZsmTree.ItemsSource)
            {
                tree.IsChecked = true;
                tree.SetChildrenChecked(true);
            }
        }

        /// <summary>
        /// 全部取消选中
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuUnSelectAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (MonitorCameraTreeModel tree in tvZsmTree.ItemsSource)
            {
                tree.IsChecked = false;
                tree.SetChildrenChecked(false);
            }
        }

        /// <summary>
        /// 鼠标右键事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeViewItem_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            TreeViewItem item = VisualUpwardSearch<TreeViewItem>(e.OriginalSource as DependencyObject) as TreeViewItem;
            if (item != null)
            {
                item.Focus();
                e.Handled = true;
            }
        }

        /// <summary>
        /// 鼠标左键事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeViewItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (e.ButtonState == MouseButtonState.Pressed)
                {
                    TreeViewItem item = VisualUpwardSearch<TreeViewItem>(e.OriginalSource as DependencyObject) as TreeViewItem;
                    if (item != null)
                    {
                        MonitorCameraTreeModel selectModel = (MonitorCameraTreeModel)item.Header;
                        if (!selectModel.IsChecked)
                        {
                            selectModel.IsChecked = true;
                            if (selectModel.Parent != null)
                            {
                                selectModel.Parent.IsChecked = true;
                            }
                            if (selectModel.Children != null && selectModel.Children.Count < 2)
                            {
                                selectModel.SetChildrenChecked(true);
                            }
                            else
                            {
                                MessageBox.Show("最多播放一个视频");
                                selectModel.IsChecked = false;
                                return;
                            }
                            if (selectModel.Children == null || selectModel.Children.Count == 0)
                            {
                                ((MainWindow)Application.Current.MainWindow).PlayVideoInvoke(selectModel);
                            }
                            
                        }
                        else
                        {
                            selectModel.IsChecked = false;
                            if (selectModel.Children != null && selectModel.Children.Count != 0)
                            {
                                foreach (var model in selectModel.Children)
                                {
                                    if (model.IsChecked)
                                    {
                                        ((MainWindow)Application.Current.MainWindow).CloseVideoInvoke(model);
                                    }
                                }
                                selectModel.SetChildrenChecked(false);
                            }
                            if (selectModel.Parent != null)
                            {
                                if (this.ChildrenSelectNum(selectModel.Parent) == 0)
                                {
                                    selectModel.Parent.IsChecked = false;
                                }
                            }
                            if (selectModel.Children == null || selectModel.Children.Count == 0)
                            {
                                ((MainWindow)Application.Current.MainWindow).CloseVideoInvoke(selectModel);
                            }
                        }
                        e.Handled = true;
                    }
                }
            }
            catch (MyException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 获取子项选中的数量
        /// </summary>
        /// <param name="model">父项</param>
        /// <returns>选中数量</returns>
        private int ChildrenSelectNum(MonitorCameraTreeModel model)
        {
            int count = 0;
            foreach (var item in model.Children)
            {
                if (item.IsChecked)
                {
                    count++;
                }
            }

            return count;
        }

        static DependencyObject VisualUpwardSearch<T>(DependencyObject source)
        {
            while (source != null && source.GetType() != typeof(T))
                source = VisualTreeHelper.GetParent(source);

            return source;
        }

       

        /// <summary>
        /// 点击添加
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuAddItem_Click(object sender, RoutedEventArgs e)
        {
            VideoView videoview = new VideoView();
            videoview.ShowDialog();
            if (videoview.Xmlmodel != null)
            {
                bool flag = false;
                IList<MonitorCameraTreeModel> model = ((MainWindow)Application.Current.MainWindow).ztvTest._itemsSourceData;
                switch (videoview.Xmlmodel.ParentName)
                {
                    case XmlType.AddinterData:
                        {
                            foreach (var item in model[1].Children)
                            {
                                if (item.Name == videoview.Xmlmodel.ChildName)
                                {
                                    flag = true;
                                    break;
                                }
                            }
                            int count = model[1].Children.Count;
                            videoview.Xmlmodel.ChildId = "1-" + (count + 1).ToString();
                            break; 
                        }
                    case XmlType.AddLocalData:
                        {
                            foreach (var item in model[0].Children)
                            {
                                if (item.Name == videoview.Xmlmodel.ChildName)
                                {
                                    flag = true;
                                    break;
                                }
                            }
                            int count = model[0].Children.Count;
                            videoview.Xmlmodel.ChildId = "0-" + (count + 1).ToString();
                            break;
                        }
                }
                if (!flag)
                {
                    OperaXml.AddDataToXml(videoview.Xmlmodel, videoview.Xmlmodel.ParentName);
                    ((MainWindow)Application.Current.MainWindow).TvTestDataBind();
                }
                else
                {
                    MessageBox.Show("该视频名已经存在，不允许重复");
                }
            }
        }

        /// <summary>
        /// 点击删除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuRemoveItem_Click(object sender, RoutedEventArgs e)
        {

            DelData delData = new DelData();
            delData.ShowDialog();

            if (delData.model != null)
            {
                OperaXml.RemoveXmlData(delData.model, delData.model.ParentName);
                ((MainWindow)Application.Current.MainWindow).TvTestDataBind();
            }
        }
        #endregion
    }
}
