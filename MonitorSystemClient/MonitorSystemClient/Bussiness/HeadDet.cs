using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

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

        private static HOGDescriptor hog;

        private static HOGDescriptor Hog
        {
            get 
            {
                if (hog == null)
                {
                    Size winSize = new Size(64,64);
                    Size blockSize = new Size(16, 16);
                    Size blockStride = new Size(8, 8);
                    Size winStride = new Size(8, 8);
                    Size cellSize = new Size(8, 8);
                    int nbins = 9;
                    hog = new HOGDescriptor(winSize, blockSize, blockStride, cellSize, nbins, 1, 1, 0.2, true);
                    float[] data = GetData();
                    hog.SetSVMDetector(data);

                }
                return hog;
            }
            set
            {
                hog = value;
            }
        }

        /// <summary>
        /// 进行视频检测
        /// </summary>
        /// <param name="image">待检测图像</param>
        /// <returns>检测后的图像</returns>
        public static MDeteInfo GetHead(Image<Bgr, Byte> image,int type)
        {
            if (type == 0)
            {
                return GetAdaBoostHead(image);
            }
            else
            {
                return GetSVMHead(image);
            }
        }

        private static float Intersection(Rectangle rect1, Rectangle rect2)
        {
            if (rect1.X > rect2.X + rect2.Width)
            {
                return 0.0f;
            }
            if (rect1.Y > rect2.Y + rect2.Height)
            {
                return 0.0f;
            }
            if (rect1.X + rect1.Width < rect2.X)
            {
                return 0.0f;
            }
            if (rect1.Y + rect1.Height < rect2.Y)
            {
                return 0.0f;
            }
            //交集矩形的坐标右上角点的x和左下角点的y
            float right_up_x, left_down_y;
            //交集矩形的左侧x的坐标和上侧的y坐标
            float left_x, up_y;
            //交集矩形的面积,两个矩形的面积
            float area_Intersection, area1, area2;
            //对交集区域右上角的x坐标赋值
            if (rect1.X + rect1.Width > rect2.X + rect2.Width)
            {
                right_up_x = rect2.X + rect2.Width;
            }
            else
            {
                right_up_x = rect1.X + rect1.Width;
            }
            //对交集区域左下角的y坐标赋值
            if (rect1.Y + rect1.Height > rect2.Y + rect2.Height)
            {
                left_down_y = rect2.Y + rect2.Height;
            }
            else
            {
                left_down_y = rect1.Y + rect1.Height;
            }
            //对交集矩形的左侧x的坐标赋值
            if (rect1.X > rect2.X)
            {
                left_x = rect1.X;
            }
            else
            {
                left_x = rect2.X;
            }
            //对交集矩形的上侧y的坐标赋值
            if (rect1.Y > rect2.Y)
            {
                up_y = rect1.Y;
            }
            else
            {
                up_y = rect2.Y;
            }
            //计算交集的面积,及两个矩形的面积
            area_Intersection = (right_up_x - left_x) * (left_down_y - up_y);
            area1 = rect1.Width * rect1.Height;
            area2 = rect2.Width * rect2.Height;
            //返回覆盖比例
            float coverRate;
            coverRate = area_Intersection / (area1 + area2 - area_Intersection);
            return coverRate;
        }

        private static float[] GetData()
        {
            List<float> data = new List<float>();
            using (StreamReader reader = new StreamReader("../../data/HogDetector.txt"))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    var arr = new float[line.Length];
                    for (var i = 0; i < line.Length; i++)
                    {
                        arr[i] = (float)Convert.ToDouble(line[i]);
                        data.Add(arr[i]);
                    }
                }
            }

            var array = data.ToArray();
            return array;
        }


        /// <summary>
        /// AdaBoost算法检测
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        private static MDeteInfo GetAdaBoostHead(Image<Bgr, Byte> image)
        {
            MDeteInfo model = new MDeteInfo();
            try
            {
                double scale = 1.3;
                Image<Bgr, Byte> smallframe = image.Resize(1 / scale, Emgu.CV.CvEnum.INTER.CV_INTER_LINEAR);
                Image<Gray, Byte> gray = smallframe.Convert<Gray, Byte>();
                gray._EqualizeHist(); //均衡化

               // MCvAvgComp[] faceDetected= cascade.Detect(gray,1.1,2,Emgu.CV.CvEnum.HAAR_DETECTION_TYPE.DO_CANNY_PRUNING,new Size(20,20),new Size(30,30));

                MCvAvgComp[][] faceDetected = gray.DetectHaarCascade(
                    cascade,
                    1.1,
                    1,
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
                //若有异常，吃掉异常
            }
            return model;
        }


        /// <summary>
        /// SVM算法检测
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        private static MDeteInfo GetSVMHead(Image<Bgr, Byte> image)
        {
            List<Rectangle> rects = new List<Rectangle>();
            Size winStride = new Size(8, 8);
            Size winSize = new Size(64, 64);
            Rectangle roiRect = new Rectangle(144, 206, 406, 360);
            int count = 0;
            Rectangle[] rectangle = Hog.DetectMultiScale(image);
            if (rectangle.Length > 0)
            {
                //Rectangle temp = new Rectangle();
                //bool pushFlag;
                foreach (var item in rectangle)
                {
                //    pushFlag = true;

                //    temp.X = item.X + roiRect.X;
                //    temp.Y = item.Y + roiRect.Y;
                //    temp.Width = winSize.Width;
                //    temp.Height = winSize.Height;

                //    foreach (var rectNum in rects)
                //    {
                //        if (Intersection(rectNum, temp) > 0.2)
                //        {
                //            pushFlag = false;
                //            break;
                //        }
                //    }

                //    if (pushFlag)
                //    {
                //        rects.Add(temp);
                //    }

                //    count++;
                //}

                //foreach (var r in rects)
                //{
                    image.Draw(item, new Bgr(Color.Red), 3);
                }

            }
            MDeteInfo model = new MDeteInfo();
            model.Frame = image;
            model.HeadCount = count;
            return model;
        }

        public static bool StroageImage(Image<Bgr,Byte> image)
        {
            bool reslut = false;
            try
            {
                string picname ="C:\\data\\pic\\" +DateTime.Now.ToShortTimeString() + ".png";
                IntPtr ptr = ConvertImage.EmgucvImageToIplImagePointer<Bgr,Byte>(image);
                CvInvoke.cvSaveImage(picname, ptr,new IntPtr());
                //image.Save(picname);

                reslut = true;
            }
            catch (Exception)
            {
                reslut = false;
            }
            return reslut;
        }

        #endregion
    }
}
