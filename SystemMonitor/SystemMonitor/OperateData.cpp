#include "stdafx.h"
#include <fstream>
#include "OperateData.h"

using namespace std;

COperateData::COperateData()
{
	m_data.clear();
}


COperateData::~COperateData()
{
	m_data.clear();
}

vector<CSourceModel> COperateData::GetData()
{
	const char* localpath = "C:\\data\\localdata.txt";

	ifstream fileIn(localpath, ios::in);
	string dataname;
	string datapath;

	while (!fileIn.eof())
	{
		CSourceModel model;
		getline(fileIn, dataname);
		getline(fileIn, datapath);
		model.name = CString(dataname.c_str());
		model.videopath = CString(datapath.c_str());
		m_data.push_back(model);
	}

	fileIn.clear();
	fileIn.close();

	return m_data;
}