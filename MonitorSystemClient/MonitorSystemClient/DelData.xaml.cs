﻿using System.Collections.Generic;
using System.Windows;
using System.Windows.Documents;

namespace MonitorSystemClient
{
    /// <summary>
    /// DelData.xaml 的交互逻辑
    /// </summary>
    public partial class DelData : Window
    {
        #region 变量
        /// <summary>
        /// XmlModel
        /// </summary>
        public XmlModel model;

        /// <summary>
        /// 父节点索引
        /// </summary>
        private int index_parent;

        #endregion

        #region 构造方法
        /// <summary>
        /// 默认构造方法
        /// </summary>
        public DelData()
        {
            index_parent = -1;
            InitializeComponent();
        }

        #endregion

        #region 方法实现

        /// <summary>
        /// 点击确认
        /// </summary>
        /// <param name="sender">对象</param>
        /// <param name="e">事件</param>
        private void ButtonOK_Click(object sender, RoutedEventArgs e)
        {
            int index = this.selectParent.SelectedIndex;
            int index_name = this.DelName.SelectedIndex;

            if(index_name == -1)
            {
                MessageBox.Show("请选择要删除的视频名");
                return;
            }

            if (model == null)
            {
                model = new XmlModel();
            }

            model.ChildName = this.DelName.SelectedItem.ToString();
            switch (index)
            {
                case 0: 
                    model.ParentName = XmlType.RemovrLocalData;
                    break;
                case 1:
                    model.ParentName = XmlType.RemoveinterData;
                    break;
            }

            this.Close();
        }

        /// <summary>
        /// 点击取消
        /// </summary>
        /// <param name="sender">对象</param>
        /// <param name="e">事件</param>
        private void ButtonCancle_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 点击视频名时发生
        /// </summary>
        /// <param name="sender">对象</param>
        /// <param name="e">事件</param>
        private void DelName_GotFocus(object sender, RoutedEventArgs e)
        {
            int index = this.selectParent.SelectedIndex;
            if (index == -1)
            {
                MessageBox.Show("请选择要删除的视频的位置");
            }
            else
            {
                if (index_parent != index)
                {
                    IList<MonitorCameraTreeModel> model = OperaXml.GetXmlData();
                    List<string> namelist = this.GetName(model[index].Children);
                    this.DelName.ItemsSource = namelist;
                    index_parent = index;
                }
            }

            e.Handled = true;
        }

        /// <summary>
        /// 获取名字
        /// </summary>
        /// <param name="model">数据对象</param>
        /// <returns>名称集合</returns>
        private List<string> GetName(IList<MonitorCameraTreeModel> model)
        {
            List<string> namelist = new List<string>();
            foreach (var item in model)
            {
                namelist.Add(item.Name);
            }

            return namelist;
        }

        #endregion
    }
}
