using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitorSystemClient
{
    /// <summary>
    /// 检测结果模型
    /// </summary>
    public class MDeteInfo
    {
        /// <summary>
        ///  检测后的图像
        /// </summary>
        public Image<Bgr, Byte> Frame;

        /// <summary>
        /// 人头数
        /// </summary>
        public int HeadCount;

        /// <summary>
        /// 默认构造方法
        /// </summary>
        public MDeteInfo()
        {
            Frame = null;
            HeadCount = 0;
        }
    }
}
