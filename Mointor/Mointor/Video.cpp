#include "Stdafx.h"
#include "Video.h"


CVideo::CVideo()
{
	capture = NULL;
	srcImage = NULL;
	deteImage = NULL;
	currentCount = -1;
	totalCount = -1;
	isInit = false;
}


CVideo::~CVideo()
{
	cvReleaseCapture(&capture);
	cvReleaseImage(&srcImage);
	cvReleaseImage(&deteImage);
	currentCount = 0;
	totalCount = 0;
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
}

// �ַ�ת��
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
	if (currentCount<totalCount)
	{
		srcImage = cvQueryFrame(capture);
		currentCount++;
	}
	else
	{
		srcImage = NULL;
	}
	return srcImage;
}

IplImage* CVideo::GetDetectFrame()
{
	Mat currFrame(srcImage, 1);
	if (!isInit)
	{
		InitDeteInfo();
	}

	//���
	headDetects->detectSingle(currFrame, heads);
	headDetects->draw(currFrame);

	//����
	head->track(currFrame, heads);
	head->draw(currFrame);

	//�Ƿ����������
	regions = region->detect(currFrame);

	if (regions)
		countOut += head->isInRegion(currFrame);

	text << "Out: " << countOut;
	putText(currFrame, text.str(), Point(0, 490),
		FONT_HERSHEY_SIMPLEX, 1, CV_RGB(0, 255, 0), 2);
	text.str("");

	//���ſ������
	door->CalOpenLen(currFrame);

	//��ʾĿ����Ŀ
	text << "Target: " << head->getTargets();
	putText(currFrame, text.str(), Point(0, 550),
		FONT_HERSHEY_SIMPLEX, 1, CV_RGB(0, 255, 0), 2);
	text.str("");

	++detectionCounts;
	heads.clear();
	++frames;
	
	img = IplImage(currFrame); // �������ͷ
	deteImage = &img;
	
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