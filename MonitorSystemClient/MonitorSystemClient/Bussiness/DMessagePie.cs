using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitorSystemClient
{
    /// <summary>
    /// 消息现实委托类
    /// </summary>
    public class DMessagePie
    {
        /// <summary>
        /// 消息显示委托
        /// </summary>
        /// <param name="message"></param>
        public delegate void DisplayMessageDelegate(string message);

        /// <summary>
        /// 消息显示事件
        /// </summary>
        public static event DisplayMessageDelegate DisplayEvent;

        /// <summary>
        /// 消息注册
        /// </summary>
        /// <param name="message"></param>
        public static void OnDisplayEvent(string message)
        {
            if (DisplayEvent != null)
            {
                DisplayEvent(message);
            }
        }
    }
}
