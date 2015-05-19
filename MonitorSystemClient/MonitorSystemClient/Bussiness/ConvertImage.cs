using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System;
using System.Runtime.InteropServices;

namespace MonitorSystemClient
{
    /// <summary>
    /// c# 和c++图像类型的转化
    /// </summary>
    public class ConvertImage
    {
        /// <summary>
        /// 将MIplImage结构转换到IplImage指针；
        /// 注意：指针在使用完之后必须用Marshal.FreeHGlobal方法释放。
        /// </summary>
        /// <param name="mi">MIplImage对象</param>
        /// <returns>返回IplImage指针</returns>
        public static IntPtr MIpImageToUplImagePointer(MIplImage mi)
        {
            IntPtr ptr = Marshal.AllocHGlobal(mi.nSize);
            Marshal.StructureToPtr(mi, ptr, false);
            return ptr;
        }

        /// <summary>
        /// 将IplImage指针转换成MIplImage结构
        /// </summary>
        /// <param name="ptr">IplImage指针</param>
        /// <returns>返回MIplImage结构</returns>
        public static MIplImage IplImagePointerToMIplImage(IntPtr ptr)
        {
            return (MIplImage)Marshal.PtrToStructure(ptr, typeof(MIplImage));
        }

        /// <summary>
        /// 将IplImage指针转换成Emgucv中的Image对象；
        /// 注意：这里需要您自己根据IplImage中的depth和nChannels来决定
        /// </summary>
        /// <typeparam name="TColor">Color type of this image (either Gray, Bgr, Bgra, Hsv, Hls, Lab, Luv, Xyz or Ycc)</typeparam>
        /// <typeparam name="TDepth">Depth of this image (either Byte, SByte, Single, double, UInt16, Int16 or Int32)</typeparam>
        /// <param name="ptr">IplImage指针</param>
        /// <returns>返回Image对象</returns>
        public static Image<TColor, TDepth> IplImagePointerToEmgucvImage<TColor, TDepth>(IntPtr ptr)
            where TColor : struct, IColor
            where TDepth : new()
        {
            MIplImage mi = IplImagePointerToMIplImage(ptr);
            return new Image<TColor, TDepth>(mi.width, mi.height, mi.widthStep, mi.imageData);
        }

        /// <summary>
        /// 将IplImage指针转换成Emgucv中的IImage接口；
        /// 1通道对应灰度图像，3通道对应BGR图像，4通道对应BGRA图像。
        /// 注意：3通道可能并非BGR图像，而是HLS,HSV等图像
        /// </summary>
        /// <param name="ptr">IplImage指针</param>
        /// <returns>返回IImage接口</returns>
        public static IImage IplImagePointToEmgucvIImage(IntPtr ptr)
        {
            MIplImage mi = IplImagePointerToMIplImage(ptr);
            Type tColor;
            Type tDepth;
            string unsupportedDepth = "不支持的像素位深度IPL_DEPTH";
            string unsupportedChannels = "不支持的通道数（仅支持1，2，4通道）";
            switch (mi.nChannels)
            {
                case 1:
                    tColor = typeof(Gray);
                    switch (mi.depth)
                    {
                        case IPL_DEPTH.IPL_DEPTH_8U:
                            tDepth = typeof(Byte);
                            return new Image<Gray, Byte>(mi.width, mi.height, mi.widthStep, mi.imageData);
                        case IPL_DEPTH.IPL_DEPTH_16U:
                            tDepth = typeof(UInt16);
                            return new Image<Gray, UInt16>(mi.width, mi.height, mi.widthStep, mi.imageData);
                        case IPL_DEPTH.IPL_DEPTH_16S:
                            tDepth = typeof(Int16);
                            return new Image<Gray, Int16>(mi.width, mi.height, mi.widthStep, mi.imageData);
                        case IPL_DEPTH.IPL_DEPTH_32S:
                            tDepth = typeof(Int32);
                            return new Image<Gray, Int32>(mi.width, mi.height, mi.widthStep, mi.imageData);
                        case IPL_DEPTH.IPL_DEPTH_32F:
                            tDepth = typeof(Single);
                            return new Image<Gray, Single>(mi.width, mi.height, mi.widthStep, mi.imageData);
                        case IPL_DEPTH.IPL_DEPTH_64F:
                            tDepth = typeof(Double);
                            return new Image<Gray, Double>(mi.width, mi.height, mi.widthStep, mi.imageData);
                        default:
                            throw new NotImplementedException(unsupportedDepth);
                    }
                case 3:
                    tColor = typeof(Bgr);
                    switch (mi.depth)
                    {
                        case IPL_DEPTH.IPL_DEPTH_8U:
                            tDepth = typeof(Byte);
                            return new Image<Bgr, Byte>(mi.width, mi.height, mi.widthStep, mi.imageData);
                        case IPL_DEPTH.IPL_DEPTH_16U:
                            tDepth = typeof(UInt16);
                            return new Image<Bgr, UInt16>(mi.width, mi.height, mi.widthStep, mi.imageData);
                        case IPL_DEPTH.IPL_DEPTH_16S:
                            tDepth = typeof(Int16);
                            return new Image<Bgr, Int16>(mi.width, mi.height, mi.widthStep, mi.imageData);
                        case IPL_DEPTH.IPL_DEPTH_32S:
                            tDepth = typeof(Int32);
                            return new Image<Bgr, Int32>(mi.width, mi.height, mi.widthStep, mi.imageData);
                        case IPL_DEPTH.IPL_DEPTH_32F:
                            tDepth = typeof(Single);
                            return new Image<Bgr, Single>(mi.width, mi.height, mi.widthStep, mi.imageData);
                        case IPL_DEPTH.IPL_DEPTH_64F:
                            tDepth = typeof(Double);
                            return new Image<Bgr, Double>(mi.width, mi.height, mi.widthStep, mi.imageData);
                        default:
                            throw new NotImplementedException(unsupportedDepth);
                    }
                case 4:
                    tColor = typeof(Bgra);
                    switch (mi.depth)
                    {
                        case IPL_DEPTH.IPL_DEPTH_8U:
                            tDepth = typeof(Byte);
                            return new Image<Bgra, Byte>(mi.width, mi.height, mi.widthStep, mi.imageData);
                        case IPL_DEPTH.IPL_DEPTH_16U:
                            tDepth = typeof(UInt16);
                            return new Image<Bgra, UInt16>(mi.width, mi.height, mi.widthStep, mi.imageData);
                        case IPL_DEPTH.IPL_DEPTH_16S:
                            tDepth = typeof(Int16);
                            return new Image<Bgra, Int16>(mi.width, mi.height, mi.widthStep, mi.imageData);
                        case IPL_DEPTH.IPL_DEPTH_32S:
                            tDepth = typeof(Int32);
                            return new Image<Bgra, Int32>(mi.width, mi.height, mi.widthStep, mi.imageData);
                        case IPL_DEPTH.IPL_DEPTH_32F:
                            tDepth = typeof(Single);
                            return new Image<Bgra, Single>(mi.width, mi.height, mi.widthStep, mi.imageData);
                        case IPL_DEPTH.IPL_DEPTH_64F:
                            tDepth = typeof(Double);
                            return new Image<Bgra, Double>(mi.width, mi.height, mi.widthStep, mi.imageData);
                        default:
                            throw new NotImplementedException(unsupportedDepth);
                    }
                default:
                    throw new NotImplementedException(unsupportedChannels);
            }
        }

        /// <summary>
        /// 将Emgucv中的Image对象转换成IplImage指针；
        /// </summary>
        /// <typeparam name="TColor">Color type of this image (either Gray, Bgr, Bgra, Hsv, Hls, Lab, Luv, Xyz or Ycc)</typeparam>
        /// <typeparam name="TDepth">Depth of this image (either Byte, SByte, Single, double, UInt16, Int16 or Int32)</typeparam>
        /// <param name="image">Image对象</param>
        /// <returns>返回IplImage指针</returns>
        public static IntPtr EmgucvImageToIplImagePointer<TColor, TDepth>(Image<TColor, TDepth> image)
            where TColor : struct, IColor
            where TDepth : new()
        {
            return image.Ptr;
        }
    }
}
