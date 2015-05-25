#pragma once
#include "afxwin.h"
#include "MyEditBrowseCtrl.h"


// CAddVideo 对话框

class CAddVideo : public CDialogEx
{
	DECLARE_DYNAMIC(CAddVideo)

public:
	CAddVideo(CWnd* pParent = NULL);   // 标准构造函数
	virtual ~CAddVideo();

// 对话框数据
	enum { IDD = IDD_DIALOG_ADD };

protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV 支持

	DECLARE_MESSAGE_MAP()
	
protected:
	// 视频类型
	CComboBox m_source;
	// 视频名称
	CString m_name;
	// 视频路径
//	CString m_path;
	CMyEditBrowseCtrl  m_wndFileEdit;

	virtual BOOL OnInitDialog();
	afx_msg void OnBnClickedButtonSure();
	afx_msg void OnBnClickedButtonCancle();
	// 验证视频名称是否重复
	bool CheckVideoName(int videoType);
	
};
