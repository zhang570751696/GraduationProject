/*
 * 文件名称: IsInRegion.h
 * 摘要: 判断人头是否在指定区域
 * 作者: 杨伟清
 * 完成日期: 2014年12月29日
*/
#ifndef _ISINREGION_H_YWQ_
#define _ISINREGION_H_YWQ_

#include <opencv2\opencv.hpp>
//#include <opencv.hpp>
using namespace cv;

class IsInRegion
{
public:
	IsInRegion(void);
	~IsInRegion(void);
	bool RegionJudge(const Rect rect);

private:
	bool LowerThanUp(Point2f center);
	bool UpperThanDown(Point2f center);

private:
	Point2f ptUpLeft;
	Point2f ptUpRight;
	Point2f ptDownLeft;
	Point2f ptDownRight;
};
#endif
