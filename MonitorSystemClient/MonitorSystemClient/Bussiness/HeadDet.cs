using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Drawing;

namespace MonitorSystemClient
{
    /// <summary>
    /// 检测类
    /// </summary>
    public class HeadDet
    {
        #region 实现方法
        /// <summary>
        /// 样本数据
        /// </summary>
        private static HaarCascade cascade = new HaarCascade(@"../../data/cascades.xml");

        /// <summary>
        /// 进行视频检测
        /// </summary>
        /// <param name="image">待检测图像</param>
        /// <returns>检测后的图像</returns>
        public static MDeteInfo GetHead(Image<Bgr, Byte> image)
        {
            MDeteInfo model = new MDeteInfo();
            try
            {
                double scale = 1.3;
                Image<Bgr, Byte> smallframe = image.Resize(1 / scale, Emgu.CV.CvEnum.INTER.CV_INTER_LINEAR);
                Image<Gray, Byte> gray = smallframe.Convert<Gray, Byte>();
                gray._EqualizeHist(); //均衡化

                MCvAvgComp[][] faceDetected = gray.DetectHaarCascade(
                    cascade,
                    1.1,
                    2,
                    Emgu.CV.CvEnum.HAAR_DETECTION_TYPE.DO_CANNY_PRUNING,
                    new Size(30, 30));

                int count = 0;
                foreach (var item in faceDetected[0])
                {
                    count++;
                    image.Draw(item.rect, new Bgr(Color.Red), 3);
                }
                model.Frame = image;
                model.HeadCount = count;

            }
            catch (Exception ex)
            {
                // 若有异常，吃掉异常
            }
            return model;
        }
        #endregion
    }
}
