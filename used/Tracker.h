#ifndef _TRACKER_H_
#define _TRACKER_H_

#include <vector>
#include <map>
using namespace std;

#include <opencv2\opencv.hpp>
//#include <opencv.hpp>
using namespace cv;

#include "Target.h"
#include "IsInRegion.h"

const int gFrameThreshold = 10;
const int gTargetTime = 25;

class Tracker
{
public:
	Tracker();
	~Tracker();
	void track(const Mat& frame, vector<Rect>& heads);
	void draw(Mat& frame);
	int isInRegion(Mat& frame);

	vector<Target>& get() 
	{
		return m_targets;
	}

	int getTargets()
	{
		return m_targets.size();
	}

private:
	void deleted();
	void add();

private:
	vector<Target> m_targets;
	vector<Target> m_suspects;
	IsInRegion m_regionDetects;
	int m_counts;
};

#endif