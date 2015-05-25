#pragma once


// CMyEditBrowseCtrl

class CMyEditBrowseCtrl : public CMFCEditBrowseCtrl
{
	//DECLARE_DYNAMIC(CMyEditBrowseCtrl)

public:
	CMyEditBrowseCtrl();
	virtual ~CMyEditBrowseCtrl();

public:
	CString m_strFileType;//自定义开文件类型  

private:
	virtual void OnBrowse();//重写CMFCEditBrowseCtrl的打开函数

protected:
	DECLARE_MESSAGE_MAP()
};


