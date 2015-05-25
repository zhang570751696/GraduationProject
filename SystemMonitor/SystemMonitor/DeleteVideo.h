#pragma once
#include "afxwin.h"


// CDeleteVideo 对话框

class CDeleteVideo : public CDialogEx
{
	DECLARE_DYNAMIC(CDeleteVideo)

public:
	CDeleteVideo(CWnd* pParent = NULL);   // 标准构造函数
	virtual ~CDeleteVideo();

// 对话框数据
	enum { IDD = IDD_DIALOG_DELETE };

protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV 支持

	DECLARE_MESSAGE_MAP()
	afx_msg void OnBnClickedButtonOk();
	afx_msg void OnBnClickedButtonCancle();
	afx_msg void OnCbnDropdownComboName();
	// 视频名称
	CComboBox m_name;
	// 视频所处的节点
	CComboBox m_parent;
	virtual BOOL OnInitDialog();
};
