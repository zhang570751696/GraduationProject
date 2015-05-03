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
                using (HaarCascade cascade = new HaarCascade(@"cascades.xml"))
                {
                    using (Image<Gray, Byte> gray = image.Convert<Gray, Byte>())
                    {
                        gray._EqualizeHist();
                        MCvAvgComp[] facesDetected = cascade.Detect(gray, 1.1, 10, Emgu.CV.CvEnum.HAAR_DETECTION_TYPE.DO_CANNY_PRUNING, new Size(20, 20));

                        foreach (var det in facesDetected)
                        {
                            image.Draw(det.rect, new Bgr(Color.Blue), 2);
                        }
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
