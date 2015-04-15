using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitorSystemClient
{
    class MyException:Exception
    {
        /// <summary>
        /// 自定义异常
        /// </summary>
        /// <param name="message">异常信息</param>
        public MyException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public MyException(string message, Exception innerException)
            : base(message, innerException)
        { 
        }
    }
}
