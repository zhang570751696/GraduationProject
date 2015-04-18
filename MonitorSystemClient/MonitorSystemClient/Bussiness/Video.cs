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
        
        /// <summary>
        /// 打开摄像头
        /// </summary>
        public void OpenCapture(string videoPath,ImageBox imageBox)
        {
            try
            {
                _capture = new Capture(videoPath);
                _videoFps = (int)CvInvoke.cvGetCaptureProperty(_capture, Emgu.CV.CvEnum.CAP_PROP.CV_CAP_PROP_FPS);
                _imageBox = imageBox;
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
        public void PlayVideo(InitInternet server)
        {
            while (true)
            {
                Image<Bgr, Byte> _frame = _capture.QueryFrame();
                if (_frame != null)
                {
                    //为使播放顺畅，添加以下延时
                    System.Threading.Thread.Sleep((int)(1000.0 / _videoFps - 5));
                    if (server.IsConnect)
                    {
                        server.SendMessage(_frame);
                        _frame = server.GetMessage();
                        this.RefreshPictureBox(_frame);
                    }
                    else
                    {
                        try
                        {
                            this.RefreshPictureBox(_frame);
                        }
                        catch (ObjectDisposedException ex)
                        {
                            Thread.CurrentThread.Abort();
                        }
                    }
                }
                else
                {
                  //  image = null;
                    break;
                }
            }
        }

        /// <summary>
        /// 关闭视频
        /// </summary>
        /// <param name="videoPath"></param>
        /// <exception cref="CloseFailed">关闭视频失败</exception>
        public void CloseVideo(string videoPath)
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
