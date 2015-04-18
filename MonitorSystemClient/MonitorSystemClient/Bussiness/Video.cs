using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using System;
using System.Threading;

namespace MonitorSystemClient
{
    class Video
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
        /// ImageBox
        /// </summary>
        private ImageBox _imageBox;

        /// <summary>
        /// 是否关闭
        /// </summary>
        private bool isClose;
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
        #endregion

        #region 构造方法
        /// <summary>
        /// 构造方法
        /// </summary>
        public Video()
        {
        }
        #endregion 

        #region 实现方法

        public Capture GetCapture(string videoPath)
        {
            try
            {
                _capture = new Capture(videoPath);
                _videoFps = (int)CvInvoke.cvGetCaptureProperty(_capture, Emgu.CV.CvEnum.CAP_PROP.CV_CAP_PROP_FPS);
                return _capture;
            }
            catch (Exception ex)
            {
                throw new MyException("获取Capture失败！");
            }
        }

        /// <summary>
        /// 关闭视频
        /// </summary>
        /// <param name="videoPath"></param>
        /// <exception cref="CloseFailed">关闭视频失败</exception>
        public void CloseVideo()
        {
            try
            {
                if (_capture != null)
                {
                    _capture.Dispose();
                }

                isClose = true; 
            }
            catch (Exception)
            {
                throw new MyException("关闭视频失败");
            }
        }

        /// <summary>
        /// 显示图像
        /// </summary>
        /// <param name="image"></param>
        private void RefreshPictureBox(Image<Bgr, Byte> image)
        {
            if (!this._imageBox.InvokeRequired)
            {
                _imageBox.Image = image;
            }
        }

        #endregion
    }
}
