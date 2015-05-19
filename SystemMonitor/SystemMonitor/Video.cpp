#include "stdafx.h"
#include "Video.h"


CVideo::CVideo()
{
	capture = NULL;
	srcImage = NULL;
	deteImage = NULL;
	currentCount = -1;
	totalCount = -1;
	isInit = false;
	videostate = false;
}


CVideo::~CVideo()
{
	cvReleaseCapture(&capture);
	cvReleaseImage(&srcImage);
	cvReleaseImage(&deteImage);
	currentCount = 0;
	totalCount = 0;
	isInit = false;
	videostate = false;
}

void CVideo::InitVideo(wchar_t* filename)
{
	char* name = UnicodeToAnsi(filename);
	InitVideo(name);
}

void CVideo::InitVideo(const char* filename)
{
	capture = cvCaptureFromFile(filename);
	if (capture)
	{
		totalCount = (int)cvGetCaptureProperty(capture, CV_CAP_PROP_FRAME_COUNT);
	}
	videostate = true;
}

// 字符转化
char* CVideo::UnicodeToAnsi(wchar_t *szStr)
{
	int nLen = WideCharToMultiByte(CP_ACP, 0, szStr, -1, NULL, 0, NULL, NULL);
	if (nLen == 0)
	{
		return NULL;
	}
	char* pResult = new char[nLen];
	WideCharToMultiByte(CP_ACP, 0, szStr, -1, pResult, nLen, NULL, NULL);
	return pResult;
}

IplImage* CVideo::GetCurrentFrame()
{
	if (videostate)
	{

		if (currentCount < totalCount)
		{
			srcImage = cvQueryFrame(capture);
			currentCount++;
		}
		else
		{
			srcImage = NULL;
		}
	}
	return srcImage;
}

IplImage* CVideo::GetDetectFrame()
{
	if (videostate)
	{

		Mat currFrame(srcImage, 1);
		if (!isInit)
		{
			InitDeteInfo();
		}

		//检测
		headDetects->detectSingle(currFrame, heads);
		headDetects->draw(currFrame);

		//跟踪
		head->track(currFrame, heads);
		head->draw(currFrame);

		//是否出（进）门
		regions = region->detect(currFrame);

		if (regions)
			countOut += head->isInRegion(currFrame);

		/*text << "Out: " << countOut;
		putText(currFrame, text.str(), Point(0, 490),
			FONT_HERSHEY_SIMPLEX, 1, CV_RGB(0, 255, 0), 2);
		text.str("");*/

		//大门开启宽度
		door->CalOpenLen(currFrame);

		//显示目标数目
	/*	text << "Target: " << head->getTargets();
		putText(currFrame, text.str(), Point(0, 550),
			FONT_HERSHEY_SIMPLEX, 1, CV_RGB(0, 255, 0), 2);
		text.str("");*/

		++detectionCounts;
		heads.clear();
		++frames;

		img = IplImage(currFrame); // 添加数据头
		deteImage = &img;
	}
	//deteImage = &IplImage(currFrame);
	return deteImage;
}

void CVideo::InitDeteInfo()
{
	head = new Tracker();
	region = new DetectRegion();
	headDetects = new HeadDetection();
	door = new DoorOpenLen();
	headDetects->init();
	region->init();
	regions = 0;
	countOut = 0;
	frames = 1;
	detectionCounts = 0;
	isInit = true;
}

void CVideo::CloseVideo()
{
	cvReleaseCapture(&capture);
	videostate = false;
}

int CVideo::GetOutCount()
{
	return countOut;
}

int CVideo::GetTargetCount()
{
	return head->getTargets();
}

int CVideo::GetDoorLen()
{
	return door->GetOpenLen();
}