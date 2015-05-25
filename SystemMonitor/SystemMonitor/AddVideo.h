#pragma once
#include "afxwin.h"
#include "MyEditBrowseCtrl.h"


// CAddVideo �Ի���

class CAddVideo : public CDialogEx
{
	DECLARE_DYNAMIC(CAddVideo)

public:
	CAddVideo(CWnd* pParent = NULL);   // ��׼���캯��
	virtual ~CAddVideo();

// �Ի�������
	enum { IDD = IDD_DIALOG_ADD };

protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV ֧��

	DECLARE_MESSAGE_MAP()
	
protected:
	// ��Ƶ����
	CComboBox m_source;
	// ��Ƶ����
	CString m_name;
	// ��Ƶ·��
//	CString m_path;
	CMyEditBrowseCtrl  m_wndFileEdit;

	virtual BOOL OnInitDialog();
	afx_msg void OnBnClickedButtonSure();
	afx_msg void OnBnClickedButtonCancle();
	// ��֤��Ƶ�����Ƿ��ظ�
	bool CheckVideoName(int videoType);
	
};
