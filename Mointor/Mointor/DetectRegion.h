#ifndef _DETECT_REGION_
#define _DETECT_REGION_

#include <fstream>
#include <vector>
using namespace std;

#include <opencv2\opencv.hpp>
//#include "opencv.hpp"
using namespace cv;

class DetectRegion
{
public:
	DetectRegion();
	~DetectRegion();
	
	void init(void);

	void init(const Mat& frame, 
					const Rect& region);

	int detect(const Mat& frame);

private:
	vector<Mat> m_templates;
	vector<Rect> m_regions;
};

#endif