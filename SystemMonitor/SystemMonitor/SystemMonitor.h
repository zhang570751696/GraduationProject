
// SystemMonitor.h : PROJECT_NAME Ӧ�ó������ͷ�ļ�
//

#pragma once

#ifndef __AFXWIN_H__
	#error "�ڰ������ļ�֮ǰ������stdafx.h�������� PCH �ļ�"
#endif

#include "resource.h"		// ������


// CSystemMonitorApp: 
// �йش����ʵ�֣������ SystemMonitor.cpp
//

class CSystemMonitorApp : public CWinApp
{
public:
	CSystemMonitorApp();

// ��д
public:
	virtual BOOL InitInstance();

// ʵ��

	DECLARE_MESSAGE_MAP()
};

extern CSystemMonitorApp theApp;