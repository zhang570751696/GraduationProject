
// SystemMonitorDlg.h : ͷ�ļ�
//

#pragma once
#include "Video.h"
#include "afxcmn.h"
#include "SourceModel.h"
#include "OperateData.h"

// CSystemMonitorDlg �Ի���
class CSystemMonitorDlg : public CDialogEx
{
// ����
public:
	CSystemMonitorDlg(CWnd* pParent = NULL);	// ��׼���캯��

// �Ի�������
	enum { IDD = IDD_SYSTEMMONITOR_DIALOG };

	protected:
	virtual void DoDataExchange(CDataExchange* pDX);	// DDX/DDV ֧��

private:
	// ��ʾͼ��
	IplImage* m_img;

	// Video��
	CVideo *video;
protected:

	bool isOpenDetect;

	char* startImagePath;
	// ���οؼ�
	CTreeCtrl m_treeContrl;
	vector<CSourceModel> model;
	void ConsistentChildCheck(HTREEITEM hTreeItem);
	void ConsistentParentCheck(HTREEITEM hTreeItem);

// ʵ��
protected:
	HICON m_hIcon;
	// ������

	// ���ɵ���Ϣӳ�亯��
	virtual BOOL OnInitDialog();
	afx_msg void OnSysCommand(UINT nID, LPARAM lParam);
	afx_msg void OnPaint();
	afx_msg HCURSOR OnQueryDragIcon();
	DECLARE_MESSAGE_MAP()
	afx_msg void OnClose();
	afx_msg void OnNMClickTreeSource(NMHDR *pNMHDR, LRESULT *pResult);
	afx_msg void OnTimer(UINT_PTR nIDEvent);  //��ʱ��

	// ������Ƶ
	void PlayVideo(CString filename);
	
	// ��ʼ�����οؼ�
	void InitDataSource();
	
	// �ر���Ƶ
	void CloseVideo();

	// ��ʾ��Ƶ��Ϣ
	void DisplayVideoInfo(CString videoname, CString videopath);

	// ��ʾ������Ϣ
	void DisplayOperateInfo(CString message);

	// ��ʾ�����Ϣ
	void DisplayDetectInfo(int doorlen, int targetcount, int outCount);
public:
	afx_msg void OnBnClickedButton1();
};
