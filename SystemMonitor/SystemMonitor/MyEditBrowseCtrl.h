#pragma once


// CMyEditBrowseCtrl

class CMyEditBrowseCtrl : public CMFCEditBrowseCtrl
{
	//DECLARE_DYNAMIC(CMyEditBrowseCtrl)

public:
	CMyEditBrowseCtrl();
	virtual ~CMyEditBrowseCtrl();

public:
	CString m_strFileType;//�Զ��忪�ļ�����  

private:
	virtual void OnBrowse();//��дCMFCEditBrowseCtrl�Ĵ򿪺���

protected:
	DECLARE_MESSAGE_MAP()
};


