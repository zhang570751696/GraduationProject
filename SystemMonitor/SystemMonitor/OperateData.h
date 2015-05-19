#pragma once
#include "stdafx.h"
#include "SourceModel.h"

class COperateData
{
public:
	COperateData();
	~COperateData();

	vector<CSourceModel> GetData();

protected:
	vector<CSourceModel> m_data;

};

