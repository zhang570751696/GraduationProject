using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitorSystemClient
{
    class Video
    {
        /// <summary>
        /// capture
        /// </summary>
        private static Capture _capture;

        /// <summary>
        /// 每帧图像
        /// </summary>
        private static Image<Bgr, Byte> _frame;

        /// <summary>
        /// 视频帧率
        /// </summary>
        private static int _videoFps;

        /// <summary>
        /// 
        /// </summary>
        public static void OpenCapture(string videoPath)
        {
            try
            {
                _capture = new Capture(videoPath);
                _videoFps = (int)CvInvoke.cvGetCaptureProperty(_capture, Emgu.CV.CvEnum.CAP_PROP.CV_CAP_PROP_FPS);
            }
            catch (Exception ex)
            {
                throw new MyException("打开视频失败");
            }
        }

        /// <summary>
        /// 播放视频
        /// </summary>
        /// <param name="server"></param>
        /// <returns></returns>
        public static Image<Bgr, Byte> PlayVideo(InitInternet server)
        {
            try
            {
                _frame = _capture.QueryFrame();
                if (_frame != null)
                {
                    //为使播放顺畅，添加以下延时
                    System.Threading.Thread.Sleep((int)(1000.0 / _videoFps - 5));
                    if (server.IsConnect)
                    {
                        server.SendMessage(_frame);
                        return server.GetMessage();
                    }
                    else
                    {
                        return _frame;
                    }
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                throw new MyException("解析视频失败");
            }
        }

        /// <summary>
        /// 关闭视频
        /// </summary>
        /// <param name="videoPath"></param>
        /// <exception cref="CloseFailed">关闭视频失败</exception>
        public static void CloseVideo(string videoPath)
        {
            try
            {
                if (_capture != null)
                {
                    _capture.Dispose();
                }
            }
            catch (Exception)
            {
                throw new MyException("关闭视频失败");
            }
        }
    }
}
