#pragma once
#include "afxwin.h"


// CDeleteVideo �Ի���

class CDeleteVideo : public CDialogEx
{
	DECLARE_DYNAMIC(CDeleteVideo)

public:
	CDeleteVideo(CWnd* pParent = NULL);   // ��׼���캯��
	virtual ~CDeleteVideo();

// �Ի�������
	enum { IDD = IDD_DIALOG_DELETE };

protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV ֧��

	DECLARE_MESSAGE_MAP()
	afx_msg void OnBnClickedButtonOk();
	afx_msg void OnBnClickedButtonCancle();
	afx_msg void OnCbnDropdownComboName();
	// ��Ƶ����
	CComboBox m_name;
	// ��Ƶ�����Ľڵ�
	CComboBox m_parent;
	virtual BOOL OnInitDialog();
};
