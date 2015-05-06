using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitorSystemClient
{
    public class HeadDet
    {
        public static Image<Bgr, Byte> GetHead(Image<Bgr, Byte> image)
        {
            try
            {
                double scale = 1.5;
                Image<Bgr,Byte> smallframe = image.Resize(1/scale,Emgu.CV.CvEnum.INTER.CV_INTER_LINEAR);
                
                using (HaarCascade cascade = new HaarCascade(@"../../cascades.xml"))
                {
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
               
            }
            catch (Exception ex)
            {
 
            }
            return image;
        }
    }
}
