using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitorSystemClient
{
    public class MonitorCameraTreeModel : INotifyPropertyChanged
    {
        #region 私有变量

        /// <summary>
        /// id值
        /// </summary>
        private string id;

        /// <summary>
        /// 显示的名称
        /// </summary>
        private string name;

        /// <summary>
        /// 图标路径
        /// </summary>
        private string icon;

        /// <summary>
        /// 选中状态
        /// </summary>
        private bool isChecked;

        /// <summary>
        /// 折叠状态
        /// </summary>
        private bool isExpanded;

        /// <summary>
        /// 子项
        /// </summary>
        private IList<MonitorCameraTreeModel> children;

        /// <summary>
        /// 父项
        /// </summary>
        private MonitorCameraTreeModel parent;

        /// <summary>
        /// 视频路径
        /// </summary>
        private string videoPath;

        #endregion

        #region 构造

        public MonitorCameraTreeModel()
        {
            Children = new List<MonitorCameraTreeModel>();
            isChecked = false;
            isExpanded = false;
            icon = "/Images/16_16/folder_go.png";
            videoPath = string.Empty;
        }

        #endregion

        #region 成员变量

        /// <summary>
        /// 键值
        /// </summary>
        public string Id 
        {
            get { return this.id; }
            set {this.id = value;}
        }

        /// <summary>
        /// 显示的字符
        /// </summary>
        public string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }

        /// <summary>
        /// 图标
        /// </summary>
        public string Icon
        {
            get { return this.icon; }
            set { this.icon = value; }
        }

        /// <summary>
        /// 指针悬停的显示说明
        /// </summary>
        public string ToolTip
        {
            get
            {
                //return String.Format("{0}-{1}", Id, Name);
                return VideoPath;
            }
        }

        public bool IsChecked
        {
            get { return this.isChecked; }
            set 
            {
                if (value != isChecked)
                {
                    isChecked = value;
                    NotifyPropertyChanged("IsChecked");

                    if (isChecked)
                    {
                        // 如果选中父项也应该选中
                        if (Parent != null)
                        {
                            Parent.IsChecked = true;
                        }
                    }
                    else 
                    {
                        // 如果取消选中子项也应该取消选中
                        foreach (var child in Children)
                        {
                            child.IsChecked = false;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 是否展开
        /// </summary>
        public bool IsExpanded
        {
            get { return this.isExpanded; }
            set 
            {
                if (value != isExpanded)
                {
                    // 折叠状态改变
                    isExpanded = value;
                    NotifyPropertyChanged("IsExpanded");
                }
            }
        }

        /// <summary>
        /// 父项
        /// </summary>
        public MonitorCameraTreeModel Parent
        {
            get { return this.parent; }
            set { this.parent = value; }
        }

        /// <summary>
        /// 子项
        /// </summary>
        public IList<MonitorCameraTreeModel> Children
        {
            get { return this.children; }
            set { this.children = value; }
        }

        /// <summary>
        /// 视频路径
        /// </summary>
        public string VideoPath
        {
            get { return this.videoPath; }
            set { this.videoPath = value; }
        }

        /// <summary>
        /// 设置所有子项的选中状态
        /// </summary>
        /// <param name="isChecked"></param>
        public void SetChildrenChecked(bool _isChecked)
        {
            foreach (var child in Children)
            {
                child.IsChecked = _isChecked;
                child.SetChildrenChecked(_isChecked);
            }
        }

        /// <summary>
        /// 设置所有子项展开状态
        /// </summary>
        /// <param name="_isExpanded"></param>
        public void SetChildrenExpanded(bool _isExpanded)
        {
            foreach (var child in Children)
            {
                child.IsExpanded = _isExpanded;
                child.SetChildrenExpanded(_isExpanded);
            }
        }

        /// <summary>
        /// 属性改变事件
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
      
        /// <summary>
        /// 
        /// </summary>
        /// <param name="p"></param>
        private void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
        #endregion

    }
}
