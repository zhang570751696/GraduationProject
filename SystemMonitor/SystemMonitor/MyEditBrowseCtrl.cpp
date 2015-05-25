// MyEditBrowseCtrl.cpp : 实现文件
//

#include "stdafx.h"
#include "SystemMonitor.h"
#include "MyEditBrowseCtrl.h"


// CMyEditBrowseCtrl

//IMPLEMENT_DYNAMIC(CMyEditBrowseCtrl, CMFCEditBrowseCtrl)

CMyEditBrowseCtrl::CMyEditBrowseCtrl()
{
	m_strFileType = _T("All Files (*.*)|*.*||");
}

CMyEditBrowseCtrl::~CMyEditBrowseCtrl()
{
}


BEGIN_MESSAGE_MAP(CMyEditBrowseCtrl, CMFCEditBrowseCtrl)
END_MESSAGE_MAP()



// CMyEditBrowseCtrl 消息处理程序
void CMyEditBrowseCtrl::OnBrowse()
{
	CString TempPathName;

	CFileDialog dlg(TRUE, NULL, NULL, NULL, m_strFileType, NULL, 0, TRUE);
	(dlg.m_ofn).lpstrTitle = _T("打开文件");

	if (dlg.DoModal() == IDOK)
	{
		TempPathName = dlg.GetPathName();
		SetWindowText(TempPathName);
	}
	else
		return;
}


