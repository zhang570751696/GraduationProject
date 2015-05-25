#pragma once
#include "stdafx.h"
#include "SourceModel.h"

///数据源的操作类
class COperateData
{
public:
	COperateData();
	~COperateData();

	// 存储数据
	bool StroageDataInfo(CSourceModel model, int type);
	// 删除数据
	bool DeleteDataInfo(CString name, int type);
	// 检测是重复
	bool IsRepeat(CString name, int type);

	///获取本地xml视频数据源
	vector<CSourceModel> GetData(int type);

	CString GetErrorMessage();
protected:
	vector<CSourceModel> m_data;
	CString erroMessage;  // 获取错误信息
	char* CstringToChar(CString str);
};

