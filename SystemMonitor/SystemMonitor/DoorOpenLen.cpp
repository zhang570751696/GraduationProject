#include "stdafx.h"
#include "DoorOpenLen.h"

DoorOpenLen::DoorOpenLen(void) : 
THRESHOLD(0.8),
m_openLen(0)
{
#ifdef _INSIDE_
	m_roiRigion.x = 305;
	m_roiRigion.y = 349;
	m_roiRigion.width = 100;
	m_roiRigion.height = 48;
	templat = imread("C:\\data\\temp_in3.jpg");
#else
	m_roiRigion.x = 256;
	m_roiRigion.y = 336;
	m_roiRigion.width = 89;
	m_roiRigion.height = 30;
	templat = imread("C:\\data\\templ.jpg");

#endif
}

DoorOpenLen::~DoorOpenLen(void)
{
}

int DoorOpenLen::CalOpenLen(Mat &frame)
{
	Scalar score;
	m_roiImg = frame(m_roiRigion);

	bool beFound = false;

	Size templateSize;
	templateSize.width = templat.cols;
	templateSize.height = templat.rows;

	Mat result;

	for (int px = 0; px <= m_roiRigion.width - templateSize.width; px += 2)
	{
		for (int py = 0; py <= m_roiRigion.height - templateSize.height; py += 2)
		{
			Rect subRoiRigion(px, py, templateSize.width, templateSize.height);
			Mat subRoiImg = m_roiImg(subRoiRigion);
			matchTemplate(subRoiImg, templat, result, CV_TM_CCOEFF_NORMED);
			score = mean(result);

			if (JudgeMatch(frame, px, py, templateSize, score[0]))
			{
				beFound = true;
				#ifdef _INSIDE_
					m_openLen = 2 * (px * 82 / 92);
				#else
					m_openLen = 2 * (80 - px);
				#endif
				break;
			}
		}
	}
	//PutTextOnImg(frame, beFound, m_openLen);
	/*PutTextOnImg(frame, true, openLen);*/
	beFound = false;
	return m_openLen;
}

bool DoorOpenLen::JudgeMatch(Mat &srcImg, int xPos, int yPos, Size rectSize, float tempScore)
{
	if (tempScore >= THRESHOLD)
	{
		Rect toBeDraw;
		toBeDraw.x = m_roiRigion.x + xPos;
		toBeDraw.y = m_roiRigion.y + yPos;
		toBeDraw.width = rectSize.width;
		toBeDraw.height = rectSize.height;
		rectangle(srcImg, toBeDraw, CV_RGB(0,255,0));
		return true;
	}
	return false;
}

void DoorOpenLen::PutTextOnImg(Mat &srcImg, bool flag, int length)
{
	stringstream ss;

	ss << "Door: " << length << "cm";
	putText(srcImg, ss.str(), Point(0, 460), 
			FONT_HERSHEY_SIMPLEX, 1, CV_RGB(0, 255, 0), 2);
}

int DoorOpenLen::GetOpenLen()
{
	return m_openLen;
}