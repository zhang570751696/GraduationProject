#include "stdafx.h"
#include "Tracker.h"

Tracker::Tracker()
{
}


Tracker::~Tracker()
{
}

void Tracker::track(const Mat& frame, vector<Rect>& heads)
{
	vector<Mat> imgs;
	Mat gray;
	vector<Rect>::iterator begin = heads.begin();
	vector<Rect>::iterator end = heads.end();
	
	while (end != begin)
	{
		cvtColor(frame(*begin), gray, CV_RGB2GRAY);
		imgs.push_back(gray);
		++begin;
	}

	vector<Target>::iterator beginIter = m_targets.begin();
	vector<Target>::iterator endIter = m_targets.end();
	
	for (NULL; 
		endIter != beginIter && !imgs.empty(); 
		++beginIter)
	{
		int match = beginIter->matchTemplates(imgs, heads);

		if (match != -1)
		{
			swap(imgs[match], imgs[imgs.size() - 1]);
			imgs.pop_back();

			swap(heads[match], heads[heads.size() - 1]);
			heads.pop_back();
		}
	}

	//Ä¿±êÏûÊ§
	if (imgs.empty())
	{
		while (beginIter != endIter)
		{
 			beginIter->searchTemplates(frame, 3);
 			beginIter++;
		}
	} 
	else
	{
		begin = heads.begin();
		end = heads.end();
		Mat gray;
		while (begin != end)
		{
			Target newTarget;
			
			cvtColor(frame(*begin), gray, CV_RGB2GRAY);
			newTarget.addTemplate(gray);
			Point pos;
			pos.x = begin->x + begin->width / 2;
			pos.y = begin->y + begin->height;
			newTarget.addTrajectory(pos);

			//m_targets.push_back(newTarget);
			m_targets.push_back(newTarget);
			++begin;
		}
	}

	deleted();
//	add();
	return;
}

void Tracker::draw(Mat& frame)
{
	vector<Target>::iterator beginIter = m_targets.begin();
	const vector<Target>::iterator endIter = m_targets.end();

	while (endIter != beginIter)
	{
		beginIter->draw(frame);
		++beginIter;
	}
}

void Tracker::deleted()
{
	vector<Target>::iterator beginIter = m_targets.begin();
	vector<Target>::iterator endIter = m_targets.end();

	while (endIter != beginIter)
	{
		if ( beginIter->getSearchTime() > gFrameThreshold)
		{
			beginIter = m_targets.erase(beginIter);
			endIter = m_targets.end();
		}
		else
		{
			++beginIter;
		}
	}
}

int Tracker::isInRegion(Mat& frame)
{
	vector<Target>::iterator beginIter = m_targets.begin();
	const vector<Target>::iterator endIter = m_targets.end();
	m_counts = 0;

	while (endIter != beginIter)
	{
		Point pos = beginIter->getPos();
		Rect target;
		target.x = pos.x - 64;
		target.y = pos.y - 32;
		target.width = 64;
		target.height = 64;

		if (m_regionDetects.RegionJudge(target))
		{
			putText(frame, "out", Point(target.x, target.y - 20),
				FONT_HERSHEY_SIMPLEX, 1, CV_RGB(0, 0, 255), 2);

			if (!beginIter->isCount())
			{
				++m_counts;
				beginIter->isCounted();
			}
		}
		++beginIter;
	}

	return m_counts;
}

void Tracker::add()
{
	vector<Target>::iterator begin = m_suspects.begin();
	vector<Target>::iterator end = m_suspects.end();

	while (end != begin)
	{
		if (begin->getTime() > gTargetTime)
		{
			m_targets.push_back(*begin);
			begin = m_suspects.erase(begin);
			end = m_suspects.end();
		}
		else
			++begin;
	}
}
