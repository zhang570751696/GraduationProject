using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitorSystemClient
{
    public class XmlModel
    {
        /// <summary>
        /// 节点ID号
        /// </summary>
        private string childId;

        /// <summary>
        /// 名称
        /// </summary>
        private string childName;
        
        /// <summary>
        /// 路径
        /// </summary>
        private string videopath;

        /// <summary>
        /// 父节点
        /// </summary>
        private XmlType parentName;

        /// <summary>
        /// 默认构造方法
        /// </summary>
        public XmlModel()
        {
            videopath = string.Empty;
        }

        /// <summary>
        /// ID号
        /// </summary>
        public string ChildId
        {
            get { return childId; }
            set { childId = value; }
        }

        /// <summary>
        /// 姓名
        /// </summary>
        public string ChildName
        {
            get { return childName; }
            set { childName = value; }
        }

        /// <summary>
        /// 路径
        /// </summary>
        public string Videopath
        {
            get { return videopath; }
            set { videopath = value; }
        }

        /// <summary>
        /// 父节点
        /// </summary>
        public XmlType ParentName
        {
            get { return this.parentName; }
            set { this.parentName = value; }
        }
    }
}
