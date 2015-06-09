/*
 * �ļ�����: HeadDetection.h
 * �ļ���ʶ: ��ͷ���
 * ժҪ: ������ѵ���Ľ����ͼ���м����ͷ
 * ����: ��ΰ��
 * �������: 2014��12��22��
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
	//��ⵥ�߶��µ���ͷ
	//bool detectSingle(const char* detectorSavePath, const char* testPath);
	//����߶��µ���ͷ
	//bool detectMulti(const char* detectorSavePath, const char* testPath);

	void detectSingle(cv::Mat srcFrame, std::vector<cv::Rect> &vecRects);

	void draw(Mat& frame);
private:
	float Intersection(CvRect rect1, CvRect rect2);
	//ѵ������ͼ���С
	CvSize* m_winSize;
	//block size; block stride; window stride; cell size
	CvSize* m_blockSize;
	CvSize* m_blockStride;
	CvSize* m_winStride;
	CvSize* m_cellSize;
	//9���ݶȷ���
	int m_nbins;

	std::vector<float> detector;
	cv::HOGDescriptor *hog;
	std::vector<cv::Point > found;

	cv::Rect* m_roiRect;
	vector<Rect> m_heads;
};

#endif

