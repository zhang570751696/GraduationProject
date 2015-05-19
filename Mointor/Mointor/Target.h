#ifndef _TARGET_H_
#define _TRACKER_H_

#include <vector>
#include <limits>
#include <cassert>
using namespace std;

#include <opencv2\opencv.hpp>
//#include <opencv.hpp>
using namespace cv;

const int gMaxTemplates = 1;
const double gThreshold = 0.5;
const int gOffSetPixels = 5;
const int gStepLen = 5;
const int gMovePixels = 15;

class Target
{
public:
	Target();
	~Target();
	
	void addTrajectory(const Point& points);
	
	void addTemplate(const Mat& img);

	void addHist(const MatND& hist);
	
	/*int matchTemplates(const vector<Mat>& imgs);*/
	
	int matchTemplates(const vector<Mat>& imgs, 
		const vector<Rect>& heads);

 	int matchHists(const vector<MatND>& hists,
 		const vector<Rect>& heads);
	
	void searchTemplates(const Mat& frame);
	
	void searchTemplates(const Mat& frame, int num);

	void searchHists(const Mat& frame);
	
	void draw(Mat& frame);

	int getSearchTime() 
	{
		return m_time;
	}

	Point getPos()
	{
		return m_trajectories[m_trajectories.size() - 1];
	}

	int getTime()
	{
		return m_time;
	}

	void isCounted()
	{
		m_isCount = true;
	}

	bool isCount()
	{
		return m_isCount;
	}

private:
	bool avgDistance(int preFrameNum, int& xAvgOff, int& yAvgOff);

	void genSearchSpace(Point pos, Size frameSize, 
					Size targetSize, vector<Rect>& space);

private:
	vector<Point> m_trajectories;
	vector<Point> m_kalmanPos;
	vector<Mat> m_templates;
	vector<MatND> m_hists;
	KalmanFilter m_kalman;
	//Mat m_state;
	//Mat m_transition;
	int m_time;
	double m_learningRate;
	bool m_isCount;
};

#endif