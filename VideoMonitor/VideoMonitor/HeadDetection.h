/*
 * 文件名称: HeadDetection.h
 * 文件标识: 人头检测
 * 摘要: 根据以训练的结果在图像中检测人头
 * 作者: 杨伟清
 * 完成日期: 2014年12月22日
*/
#ifndef _HEADDETECTION_H_
#define _HEADDETECTION_H_

#include <opencv2\opencv.hpp>
#include <opencv\highgui.h>
#include <opencv\cv.h>
using namespace cv;
#include <vector>

class HeadDetection
{
public:
	HeadDetection(void);
	~HeadDetection(void);
	void init(void);
	//检测单尺度下的人头
	//bool detectSingle(const char* detectorSavePath, const char* testPath);
	//检测多尺度下的人头
	//bool detectMulti(const char* detectorSavePath, const char* testPath);

	void detectSingle(cv::Mat srcFrame, std::vector<cv::Rect> &vecRects);

	void draw(Mat& frame);
private:
	float Intersection(CvRect rect1, CvRect rect2);
	//训练样本图像大小
	CvSize* m_winSize;
	//block size; block stride; window stride; cell size
	CvSize* m_blockSize;
	CvSize* m_blockStride;
	CvSize* m_winStride;
	CvSize* m_cellSize;
	//9个梯度方向
	int m_nbins;

	std::vector<float> detector;
	cv::HOGDescriptor *hog;
	std::vector<cv::Point > found;

	cv::Rect* m_roiRect;
	vector<Rect> m_heads;
};

#endif

