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
        public static Image<Bgr, Byte> GetHead(Image<Bgr, Byte> image)
        {
            try
            {
                double scale = 1.5;
                Image<Bgr, Byte> smallframe = image.Resize(1 / scale, Emgu.CV.CvEnum.INTER.CV_INTER_LINEAR);

                MCvAvgComp[][] faceDetected = smallframe.DetectHaarCascade(
                    cascade,
                    1.1,
                    2,
                    Emgu.CV.CvEnum.HAAR_DETECTION_TYPE.DO_CANNY_PRUNING,
                    new Size(30, 30));

                foreach (var item in faceDetected[0])
                {
                    image.Draw(item.rect, new Bgr(Color.Red), 3);
                }

            }
            catch (Exception ex)
            {
                // 若有异常，吃掉异常
            }
            return image;
        }
        #endregion
    }
}
