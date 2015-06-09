#include "stdafx.h"
#include "DetectRegion.h"

DetectRegion::DetectRegion()
{
}


DetectRegion::~DetectRegion()
{
}

void DetectRegion::init(const Mat& frame, 
							const Rect& region)
{
	m_templates.push_back(frame(region));
}

void DetectRegion::init(void)
{
	ifstream templateTXT("C:\\data\\regions.txt");
	Rect pos;
	Mat frame = imread("C:\\data\\frame.jpg");

	while (!templateTXT.eof())
	{
		templateTXT >> pos.x;
		templateTXT >> pos.y;
		templateTXT >> pos.width;
		templateTXT >> pos.height;
		m_regions.push_back(pos);
		m_templates.push_back(frame(pos));
	}

	frame.release();
	templateTXT.close();
}

int DetectRegion::detect(const Mat& frame)
{
	assert(m_regions.size() == m_templates.size());
	Mat result;
	Scalar score;

	int sub = 0, count = 0;
	while (sub < m_regions.size())
	{
		matchTemplate(frame(m_regions[sub]), 
				m_templates[sub], result, CV_TM_CCOEFF_NORMED);
		score = mean(result);

		if (score[0] < 0.9)
			++count;

		++sub;
	}

	return count;
}
