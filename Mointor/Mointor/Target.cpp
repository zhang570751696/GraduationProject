#include "Stdafx.h"
#include "Target.h"


Target::Target():
	m_time(0),
	m_learningRate(0.2),
	m_kalman(4, 2, 0),
	m_isCount(false)
{
}

Target::~Target()
{
}

void Target::addTrajectory(const Point& points)
{
	if (m_trajectories.empty())
	{
		m_trajectories.push_back(points);
		m_kalmanPos.push_back(points);

 		m_kalman.statePost = 
 			*(Mat_<float>(4, 1) << points.x, points.y, 0, 0);

		m_kalman.transitionMatrix = 
			*(Mat_<float>(4, 4) << 1, 0, 1, 0,
								   0, 1, 0, 1,
								   0, 0, 1, 0,
								   0, 0, 0, 1);

		setIdentity(m_kalman.measurementMatrix, Scalar::all(1));
		setIdentity(m_kalman.processNoiseCov, Scalar::all(1));
		setIdentity(m_kalman.measurementNoiseCov, Scalar::all(1));

		m_kalman.predict();
	}
	else
	{
		Mat currPos(2, 1, CV_32F);

		currPos.at<float>(0, 0) = points.x;
		currPos.at<float>(1, 0) = points.y;

		Mat correctPos = m_kalman.correct(currPos);

		Point temp;
		temp.x = correctPos.at<float>(0, 0);
		temp.y = correctPos.at<float>(1, 0);
		m_trajectories.push_back(points);
		m_kalmanPos.push_back(temp);
	}
}

void Target::addTemplate(const Mat& img)
{
	if (!m_templates.empty())
	{
		Mat temp;
		addWeighted(m_templates[0], 1 - m_learningRate,
			img, m_learningRate, 0, temp);

		m_templates.push_back(temp);

		if (m_templates.size() > gMaxTemplates)
		{
			std::swap(m_templates[0],
				m_templates[m_templates.size() - 1]);
			m_templates.pop_back();
		} 
	}
	else
		m_templates.push_back(img);
}

void Target::addHist(const MatND& hist)
{
	m_hists.push_back(hist);

	if (m_hists.size() > gMaxTemplates)
	{
		std::swap(m_hists[0],
			m_hists[m_templates.size() - 1]);
		m_hists.pop_back();
	}
}

// int Target::matchTemplates(const vector<Mat>& imgs)
// {
// 	Mat result;
// 	Scalar score;
// 	Scalar maxScore;
// 	maxScore[0] = numeric_limits<double>::min();
// 	int counts = 0;
// 	int maxCount = 0;
// 	vector<Mat>::const_iterator beginIter = imgs.begin();
// 	vector<Mat>::const_iterator endIter = imgs.end();
// 
// 	while (endIter != beginIter)
// 	{
// 
// 		matchTemplate(*beginIter, m_templates[0], result, CV_TM_CCOEFF_NORMED);
// 		score = mean(result);
// 
//  		if (score[0] > maxScore[0] && score[0] > gThreshold) 
//  		{
// 			maxCount = counts;
// 			maxScore = score;
//  		}
// 
// 		++counts;
// 		++beginIter;
// 	}
// 
// 	addTemplate(imgs[maxCount]);
// 
// 	return maxCount;
// }

int Target::matchTemplates(const vector<Mat>& imgs, 
					const vector<Rect>& heads)
{
	assert(imgs.size() == heads.size());

	Mat result;
	Scalar score;
	Scalar minScore;
	minScore[0] = numeric_limits<double>::min();
	int counts = 0;
	int minCount = 0;
	vector<Mat>::const_iterator beginIter = imgs.begin();
	vector<Mat>::const_iterator endIter = imgs.end();

	while (endIter != beginIter)
	{
		matchTemplate(*beginIter, m_templates[0], result, CV_TM_SQDIFF);
		score = mean(result);

		if (score[0] > minScore[0])
		{
			Point prePos;
			prePos.x = m_trajectories[m_trajectories.size() - 1].x -
				m_templates[m_templates.size() - 1].cols / 2;
			prePos.y = m_trajectories[m_trajectories.size() - 1].y -
				m_templates[m_templates.size() - 1].rows;

			Point distance;
			distance.x = abs(heads[counts].x - prePos.x);
			distance.y = abs(heads[counts].y - prePos.y);

			if (distance.x < gMovePixels && distance.y < gMovePixels)
			{
				minCount = counts;
				minScore = score;
			}
		}
		++counts;
		++beginIter;
	}

	if (minScore[0] != numeric_limits<double>::min())
	{
		addTemplate(imgs[minCount]);

		Point pos;
		pos.x = heads[minCount].x + heads[minCount].width / 2;
		pos.y = heads[minCount].y + heads[minCount].height;
		addTrajectory(pos);
		return minCount;
	}
	else
		return -1;
}

int Target::matchHists(const vector<MatND>& hists, 
					const vector<Rect>& heads)
{
	assert(hists.size() == heads.size());

	int counts = 0;
	int minCount = 0;
	double score = 0.0;
	double minScore = numeric_limits<double>::max();

	vector<MatND>::const_iterator beginIter = hists.begin();
	vector<MatND>::const_iterator endIter = hists.end();

	while (endIter != beginIter)
	{
		score = compareHist(*beginIter, 
						m_hists[0], CV_COMP_BHATTACHARYYA);

		if (score < minScore)
		{
			Point prePos;
			prePos.x = m_trajectories[m_trajectories.size() - 1].x -
				m_templates[m_templates.size() - 1].cols / 2;
			prePos.y = m_trajectories[m_trajectories.size() - 1].y -
				m_templates[m_templates.size() - 1].rows;

			Point distance;
			distance.x = abs(heads[counts].x - prePos.x);
			distance.y = abs(heads[counts].y - prePos.y);

			if (distance.x < gMovePixels && distance.y < gMovePixels)
			{
				minCount = counts;
				minScore = score;
			}
		}

		++counts;
		++beginIter;
	}

	if ( minScore != numeric_limits<double>::max())
	{
		addHist(hists[minCount]);

		Point pos;
		pos.x = heads[minCount].x + heads[minCount].width / 2;
		pos.y = heads[minCount].y + heads[minCount].height;
		addTrajectory(pos);
		return minCount;
	}
	else
		return -1;
}

void Target::searchTemplates(const Mat& frame)
{
	Point prePos = m_trajectories[m_trajectories.size() - 1];
	Size preSize = m_templates[m_templates.size() - 1].size();
	
	Point leftCorner;
	leftCorner.x = prePos.x - preSize.width / 2;
	leftCorner.y = prePos.y - preSize.height;
	
	vector<Rect> matchSpace;

	genSearchSpace(leftCorner, Size(frame.cols, frame.rows), 
			preSize, matchSpace);

	vector<Mat> imgs;
	Mat gray;
	vector<Rect>::iterator beginIter = matchSpace.begin();
	vector<Rect>::iterator endIter = matchSpace.end();

	while (endIter != beginIter)
	{
		cvtColor(frame(*beginIter), gray, CV_RGB2GRAY);
		imgs.push_back(gray);
		++beginIter;
	}

	matchTemplates(imgs, matchSpace);
	++m_time;
}

void Target::searchTemplates(const Mat& frame, int num)
{
	int xAvgOff, yAvgOff;
	if (!avgDistance(num, xAvgOff, yAvgOff))
	{
		searchTemplates(frame);
	}
	else
	{
		Point prePos = m_trajectories[m_trajectories.size() - 1];
		Size preSize = m_templates[m_templates.size() - 1].size();

		Point leftCorner;
		leftCorner.x = prePos.x - preSize.width / 2;
		leftCorner.y = prePos.y - preSize.height;
		leftCorner.x += xAvgOff;
		leftCorner.y += yAvgOff;

		vector<Rect> matchSpace;
		Mat gray;

		genSearchSpace(leftCorner, Size(frame.cols, frame.rows),
			preSize, matchSpace);

		vector<Mat> imgs;
		vector<Rect>::iterator beginIter = matchSpace.begin();
		vector<Rect>::iterator endIter = matchSpace.end();

		while (endIter != beginIter)
		{
			cvtColor(frame(*beginIter), gray, CV_RGB2GRAY);
			imgs.push_back(gray);
			++beginIter;
		}

		matchTemplates(imgs, matchSpace);
		++m_time;
	}
}

void Target::draw(Mat& frame)
{
	assert(m_trajectories.size() == m_kalmanPos.size());

	if (m_trajectories.empty())
		return ;

	Point prePos, preKalman;
	vector<Point>::iterator beginIter = m_trajectories.begin();
	const vector<Point>::iterator endIter = m_trajectories.end();

	prePos = *beginIter;
	preKalman = m_kalmanPos[0];
	int count = 1;
	++beginIter;
	while (endIter != beginIter)
	{
		line(frame, prePos, *beginIter, CV_RGB(0, 255, 0), 2);
		line(frame, preKalman, m_kalmanPos[count], CV_RGB(255, 0, 0), 2);
		prePos = *beginIter;
		preKalman = m_kalmanPos[count];
		++beginIter;
		++count;
	}
}

bool Target::avgDistance(int preFrameNum, int& xAvgOff, int& yAvgOff)
{
	if (preFrameNum > m_trajectories.size())
		return false;

	double sumX = 0.0;
	double sumY = 0.0;
	double x = 0.0;
	double y = 0.0;
	for (int subs = 0; subs < preFrameNum - 1; ++subs)
	{
		x = m_trajectories[subs + 1].x - m_trajectories[subs].x;
		y = m_trajectories[subs + 1].y - m_trajectories[subs].y;
		sumX += x;
		sumY += y;
	}

	xAvgOff = sumX / preFrameNum;
	xAvgOff = sumY / preFrameNum;

	return true;
}

void Target::genSearchSpace(Point pos, 
		Size frameSize, Size targetSize, vector<Rect>& space)
{
	Size bound;
	bound.width = frameSize.width - targetSize.width;
	bound.height = frameSize.height - targetSize.height;


	pos.x = pos.x - gOffSetPixels;
	pos.y = pos.y - gOffSetPixels;

	for (int step = 0; step < targetSize.width; step += gStepLen)
	{
		if (pos.x >= 0 && pos.x <= bound.width &&
			pos.y >= 0 && pos.y <= bound.height)
		{
			space.push_back(Rect(pos, targetSize));
		}

		pos.x += gStepLen;
	}

	for (int step = 0; step < targetSize.height; step += gStepLen)
	{
		pos.y += gStepLen;
		if (pos.x >= 0 && pos.x <= bound.width &&
			pos.y >= 0 && pos.y <= bound.height)
		{
			space.push_back(Rect(pos, targetSize));
		}
	}

	for (int step = 0; step < targetSize.width; step += gStepLen)
	{
		pos.x -= gStepLen;
		if (pos.x >= 0 && pos.x <= bound.width &&
			pos.y >= 0 && pos.y <= bound.height)
		{
			space.push_back(Rect(pos, targetSize));
		}
	}

	for (int step = 0; step < targetSize.width; step += gStepLen)
	{
		pos.y -= gStepLen;
		if (pos.x >= 0 && pos.x <= bound.width &&
			pos.y >= 0 && pos.y <= bound.height)
		{
			space.push_back(Rect(pos, targetSize));
		}
	}
}
