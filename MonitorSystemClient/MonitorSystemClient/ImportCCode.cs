using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MonitorSystemClient
{
    /// <summary>
    /// 导入c++DLL
    /// </summary>
    public class ImportCCode
    {
        /// <summary>
        /// 播放视频
        /// </summary>
        /// <param name="filename"></param>
        [DllImport("CardAlaarm.dll", EntryPoint = "?PlayVideo@CCardAlaarm@@QAEXPA_W@Z", CharSet = CharSet.Auto)]
        public unsafe static extern void PlayVideo(char* filename);

        /// <summary>
        /// 关闭视频
        /// </summary>
        [DllImport("CardAlaarm.dll", EntryPoint = "?CloseVideo@CCardAlaarm@@QAEXXZ", CharSet = CharSet.Auto)]
        public static extern void CloseVideo();
    }
}
