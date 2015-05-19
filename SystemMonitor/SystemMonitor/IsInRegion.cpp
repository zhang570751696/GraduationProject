#include "stdafx.h"
#include "IsInRegion.h"

IsInRegion::IsInRegion(void)
{
#ifdef _INSIDE_
	ptUpLeft.x = 212;
	ptUpLeft.y = 200;
	ptUpRight.x = 404;
	ptUpRight.y = 242;
	ptDownLeft.x = 216;
	ptDownLeft.y = 252;
	ptDownRight.x = 404;
	ptDownRight.y = 302;
#else
	ptUpLeft.x = 250;
	ptUpLeft.y = 230;
	ptUpRight.x = 438;
	ptUpRight.y = 238;
	ptDownLeft.x = 250;
	ptDownLeft.y = 270;
	ptDownRight.x = 438;
	ptDownRight.y = 278;
#endif
}

IsInRegion::~IsInRegion(void)
{
}

bool IsInRegion::RegionJudge(const Rect rect)
{
	Point2f ptCenter;
	ptCenter.x = rect.x + rect.width / 2;
	ptCenter.y = rect.y + rect.height / 2;
	if (ptCenter.x < min(ptUpLeft.x, ptDownLeft.x) || 
		ptCenter.x > max(ptUpRight.x, ptDownRight.x))
	{
		return false;
	}
	if (LowerThanUp(ptCenter) && UpperThanDown(ptCenter))
	{
		return true;
	}
	return false;
}

bool IsInRegion::LowerThanUp(Point2f center)
{
	double lineRate, lineDist;
	double xShift = ptUpLeft.x - ptUpRight.x;
	double yShift = ptUpLeft.y - ptUpRight.y;
	//计算标定直线的斜率和截距
	if (xShift == 0)
	{
		//std::cout << "标定错误..." << std::endl;
	}
	lineRate = yShift / xShift;
	lineDist = ptUpLeft.y - ptUpLeft.x * 
		(ptUpRight.y - ptUpLeft.y) / (ptUpRight.x - ptUpLeft.x);

	return (center.y > center.x * lineRate + lineDist) ? true : false;
}

bool IsInRegion::UpperThanDown(Point2f center)
{
	double lineRate, lineDist;
	double xShift = ptDownLeft.x - ptDownRight.x;
	double yShift = ptDownLeft.y - ptDownRight.y;
	if (xShift == 0)
	{
		//std::cout << "标定错误..." << std::endl;
	}
	lineRate = yShift / xShift;
	lineDist = ptDownLeft.y - ptDownLeft.x * 
		(ptDownRight.y - ptDownLeft.y) / (ptDownRight.x - ptDownLeft.x);

	return (center.y < center.x * lineRate + lineDist) ? true : false;
}