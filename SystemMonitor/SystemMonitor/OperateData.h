#pragma once
#include "stdafx.h"
#include "SourceModel.h"

///����Դ�Ĳ�����
class COperateData
{
public:
	COperateData();
	~COperateData();

	// �洢����
	bool StroageDataInfo(CSourceModel model, int type);
	// ɾ������
	bool DeleteDataInfo(CString name, int type);
	// ������ظ�
	bool IsRepeat(CString name, int type);

	///��ȡ����xml��Ƶ����Դ
	vector<CSourceModel> GetData(int type);

	CString GetErrorMessage();
protected:
	vector<CSourceModel> m_data;
	CString erroMessage;  // ��ȡ������Ϣ
	char* CstringToChar(CString str);
};

