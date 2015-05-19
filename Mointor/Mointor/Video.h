#pragma once
#include <iostream>
#include <string>
#include <sstream>
#include <map>
#include <windows.h>
#include <time.h>
#include <opencv2\opencv.hpp>

#include "Tracker.h"
#include "HeadDetection.h"
#include "DetectRegion.h"
#include "IsInRegion.h"
#include "DoorOpenLen.h"

using namespace std;
using namespace cv;


class CVideo
{
public:
	CVideo();
	~CVideo();
	void InitVideo(const char* filename);
	void InitVideo(wchar_t* filename);
	IplImage* GetCurrentFrame();
	IplImage* GetDetectFrame();

private:// ¿ªÆô¼ì²â
	Tracker *head;
	DetectRegion *region;
	HeadDetection *headDetects;
	DoorOpenLen *door;
	bool isInit;
	int regions;
	int countOut;
	unsigned int frames;
	int detectionCounts;
	stringstream text;
	vector<Rect> heads;

private :
	CvCapture *capture;
	IplImage* srcImage;
	IplImage* deteImage;
	IplImage img;
	int currentCount;
	int totalCount;

	char* UnicodeToAnsi(wchar_t *szStr);
	void InitDeteInfo();
};

