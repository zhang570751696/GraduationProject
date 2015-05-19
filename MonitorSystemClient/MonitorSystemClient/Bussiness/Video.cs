using Emgu.CV;
using System;

namespace MonitorSystemClient
{
    /// <summary>
    /// 视频类
    /// </summary>
    public class Video
    {
        #region 私有变量
        /// <summary>
        /// capture
        /// </summary>
        private Capture _capture;

        /// <summary>
        /// 视频帧率
        /// </summary>
        private int _videoFps;

        /// <summary>
        /// 视频路径
        /// </summary>
        private string videoPath;

        /// <summary>
        /// 是否关闭
        /// </summary>
        private bool isClose;

        /// <summary>
        /// 视频名称
        /// </summary>
        private string videoName;
        #endregion

        #region 公共变量
        public int VideoFps
        {
            get { return this._videoFps; }
            private set { }
        }

        /// <summary>
        /// 是否关闭
        /// </summary>
        public bool IsClose
        {
            get { return this.isClose; }
            private set { }
        }

        public string VideoPath
        {
            get { return this.videoPath; }
            private set { }
        }

        public Capture Cap
        {
            get { return this._capture; }
            set { this._capture = value; }
        }

        public string VideoName
        {
            get { return this.videoName; }
            set { this.videoName = value; }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 构造方法
        /// </summary>
        public Video()
        {
            videoPath = string.Empty;
        }
        #endregion 

        #region 实现方法

        /// <summary>
        /// 获取capture
        /// </summary>
        /// <param name="videoPath">视频路径</param>
        /// <returns>capture</returns>
        public Capture GetCapture(MonitorCameraTreeModel model)
        {
            try
            {
                _capture = new Capture(model.VideoPath);
                _videoFps = (int)CvInvoke.cvGetCaptureProperty(_capture, Emgu.CV.CvEnum.CAP_PROP.CV_CAP_PROP_FPS);
                this.videoPath = model.VideoPath;
                this.videoName = model.Name;
                return _capture;
            }
            catch (Exception ex)
            {
                throw new MyException("打开视频出错");
            }
        }

        /// <summary>
        /// 关闭视频
        /// </summary>
        /// <exception cref="CloseFailed">关闭视频失败</exception>
        public void CloseVideo()
        {
            try
            {
                if (_capture != null)
                {
                    _capture.Dispose();
                    _capture = null;
                    this.videoPath = null;
                }

                isClose = true; 
            }
            catch (Exception)
            {
                throw new MyException("关闭视频失败");
            }
        }

        /// <summary>
        /// 播放视频
        /// </summary>
        /// <param name="filename"></param>
        public void PlayVideo(string filename)
        {
            try
            {
               
            }
            catch (Exception ex)
            {
                string str = ex.Message;
            }
        }

        #endregion
    }
}
