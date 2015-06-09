#include "HeadDetection.h"
#include <fstream>
#include <iostream>
using namespace std;

HeadDetection::HeadDetection(void)
{
	m_roiRect = new Rect();
#ifdef _INSIDE_
	m_roiRect->x = 58;
	m_roiRect->y = 134;
	m_roiRect->width = 534;
	m_roiRect->height = 430;
#else
	m_roiRect->x = 144;
	m_roiRect->y = 206;
	m_roiRect->width = 406;
	m_roiRect->height = 360;
#endif

	m_winSize = new CvSize(cvSize(64, 64));
	m_blockSize = new CvSize(cvSize(16, 16));
	m_blockStride = new CvSize(cvSize(8, 8));
	m_winStride = new CvSize(cvSize(8, 8));
	m_cellSize = new CvSize(cvSize(8, 8));
	m_nbins = 9;
}

HeadDetection::~HeadDetection(void)
{
}

float HeadDetection::Intersection(CvRect rect1, CvRect rect2)
{
	if (rect1.x > rect2.x + rect2.width)
	{
		return 0.0;
	}
	if (rect1.y > rect2.y + rect2.height)
	{
		return 0.0;
	}
	if (rect1.x + rect1.width < rect2.x)
	{
		return 0.0;
	}
	if (rect1.y + rect1.height < rect2.y)
	{
		return 0.0;
	}
	//交集矩形的坐标右上角点的x和左下角点的y
	float right_up_x, left_down_y;
	//交集矩形的左侧x的坐标和上侧的y坐标
	float left_x, up_y;
	//交集矩形的面积,两个矩形的面积
	float area_Intersection, area1, area2;
	//对交集区域右上角的x坐标赋值
	if (rect1.x + rect1.width > rect2.x + rect2.width)
	{
		right_up_x = rect2.x + rect2.width;
	}
	else
	{
		right_up_x = rect1.x + rect1.width;
	}
	//对交集区域左下角的y坐标赋值
	if (rect1.y + rect1.height > rect2.y + rect2.height)
	{
		left_down_y = rect2.y + rect2.height;
	}
	else
	{
		left_down_y = rect1.y + rect1.height;
	}
	//对交集矩形的左侧x的坐标赋值
	if (rect1.x > rect2.x)
	{
		left_x = rect1.x;
	}
	else
	{
		left_x = rect2.x;
	}
	//对交集矩形的上侧y的坐标赋值
	if (rect1.y > rect2.y)
	{
		up_y = rect1.y;
	}
	else
	{
		up_y = rect2.y;
	}
	//计算交集的面积,及两个矩形的面积
	area_Intersection = (right_up_x - left_x) * (left_down_y - up_y);
	area1 = rect1.width * rect1.height;
	area2 = rect2.width * rect2.height;
	//返回覆盖比例
	float coverRate;
	coverRate = area_Intersection / (area1 + area2 - area_Intersection);
	return coverRate;
}

//bool HeadDetection::detectSingle(const char* detectorSavePath, const char* testPath)
//{
//	cout << "\n******************** Detection Single********************" << endl;
//
//	CvCapture* cap = cvCreateFileCapture(testPath);
//	if (!cap)
//	{
//		cout << "avi file load error..." << endl;
//		return false;
//	}
//
//	vector<float> x;
//	ifstream fileIn(detectorSavePath, ios::in);
//	float val = 0.0f;
//	while(!fileIn.eof())
//	{
//		fileIn>>val;
//		x.push_back(val);
//	}
//	fileIn.close();
//
//	vector<cv::Point>  found;
//	cv::HOGDescriptor hog(m_winSize, m_blockSize, m_blockStride, m_cellSize, m_nbins);
//	hog.setSVMDetector(x);
//
//	IplImage* img = NULL;
//	cvNamedWindow("detection", CV_WINDOW_AUTOSIZE);
//	cvNamedWindow("video", CV_WINDOW_AUTOSIZE);
//
//	int frameCount = 0;
//	double timeSum = 0.0;
//	while(img=cvQueryFrame(cap))
//	{
//		cvShowImage("video", img);
//		frameCount++;
//
/*		double begin = clock();*/
		//检测：found为检测目标的左上角坐标点
//		hog.detect(img, found, 0, m_winStride, cvSize(0,0));
// 		double end = clock();
// 		double diff = (end-begin)/CLOCKS_PER_SEC*1000;
// 		timeSum += diff;
// 		cout<< "Detection time is: "<<diff<<"ms"<<endl;
//
//		if (found.size() > 0)
//		{
//			for (int i=0; i<found.size(); i++)
//			{
//				CvRect tempRect = cvRect(found[i].x, found[i].y, m_winSize.width, m_winSize.height);
//
//				cvRectangle(img, cvPoint(tempRect.x,tempRect.y),
//					cvPoint(tempRect.x+tempRect.width,tempRect.y+tempRect.height),CV_RGB(255,0,0), 2);
//			}
//		}
//
//		cvShowImage("detection", img);
//		if (cvWaitKey(1) == 27)
//		{
//			break;
//		}
//	}
//	cvReleaseCapture(&cap);
//
//	cout << "Average detection time is: " << timeSum / frameCount << "ms" << endl;
//	cvDestroyAllWindows();
//	return true;
//}

void HeadDetection::detectSingle(cv::Mat srcFrame, std::vector<cv::Rect> &vecRects)
{

	Mat *tempMat = new Mat(srcFrame, *m_roiRect);

	/*cv::Mat tempMat = srcFrame.clone();*/

	m_heads.clear();
	/*tempMat = tempMat(m_roiRect);*/

	found.clear();
	hog->detect(*tempMat, found, 0, *m_winStride, cvSize(0,0));
	if (found.size() > 0)
	{
		cv::Rect tempRect;
		//判断是否要将举行存储
		bool pushFlag;
		for (int i = 0; i < found.size(); i++)
		{
			pushFlag = true;
			tempRect.x = found[i].x + m_roiRect->x;
			tempRect.y = found[i].y + m_roiRect->y;
			tempRect.width = m_winSize->width;
			tempRect.height = m_winSize->height;

			for (int rectNum = 0; rectNum < vecRects.size(); rectNum++)
			{
				//交集的参数0.2可以调整1表示重合,0表示不相交
				if (Intersection(vecRects[rectNum], tempRect) > 0.2)
				{
					pushFlag = false;
					break;
				}
			}

			if (pushFlag)
			{
				vecRects.push_back(tempRect);
				m_heads.push_back(tempRect);
			}
		}
	}

}

void HeadDetection::init(void)
{
	const char* detectorSavePath = "C:\\data\\HogDetector.txt";
	
	ifstream fileIn(detectorSavePath, ios::in);
	float val = 0.0f;
	while(!fileIn.eof())
	{
		fileIn >> val;
		detector.push_back(val);
	}
	fileIn.close();

	hog->winSize = *m_winSize;
	hog->blockSize = *m_blockSize;
	hog->blockStride = *m_blockStride;
	hog->cellSize = *m_cellSize;
	hog->nbins = m_nbins;
	hog->setSVMDetector(detector);
}

void HeadDetection::draw(Mat& frame)
{
	vector<Rect>::iterator beginIter = m_heads.begin();
	const vector<Rect>::iterator endIter = m_heads.end();

	while (endIter != beginIter)
	{
		rectangle(frame, *beginIter, CV_RGB(0, 255, 0), 2);
		++beginIter;
	}
}
