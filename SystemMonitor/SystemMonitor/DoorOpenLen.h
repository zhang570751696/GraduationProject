/*
 *ժҪ: ����ģ��ƥ������ſ��ĳ���
 *����: ��ΰ��
 *����: 2014.12.30
*/
#ifndef _DOOROPENLEN_H_YWQ_
#define _DOOROPENLEN_H_YWQ_

#include <sstream>
#include <opencv2\opencv.hpp>
//#include <opencv.hpp>
using namespace cv;
using std::stringstream;

class DoorOpenLen
{
public:
	DoorOpenLen(void);
	~DoorOpenLen(void);
	int CalOpenLen(Mat &frame);
	int GetOpenLen();

private:
	bool JudgeMatch(Mat &srcImg, int xPos, int yPos, Size rectSize, float tempScore);
	void PutTextOnImg(Mat &srcImg, bool flag, int length);

private:
	const double THRESHOLD;
	Mat m_roiImg;
	Rect m_roiRigion;
	Mat templat;
	int m_openLen;
};
#endif