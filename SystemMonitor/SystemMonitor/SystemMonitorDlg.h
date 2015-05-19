
// SystemMonitorDlg.h : 头文件
//

#pragma once
#include "Video.h"
#include "afxcmn.h"
#include "SourceModel.h"
#include "OperateData.h"

// CSystemMonitorDlg 对话框
class CSystemMonitorDlg : public CDialogEx
{
// 构造
public:
	CSystemMonitorDlg(CWnd* pParent = NULL);	// 标准构造函数

// 对话框数据
	enum { IDD = IDD_SYSTEMMONITOR_DIALOG };

	protected:
	virtual void DoDataExchange(CDataExchange* pDX);	// DDX/DDV 支持

private:
	// 显示图像
	IplImage* m_img;

	// Video类
	CVideo *video;
protected:

	bool isOpenDetect;

	char* startImagePath;
	// 树形控件
	CTreeCtrl m_treeContrl;
	vector<CSourceModel> model;
	void ConsistentChildCheck(HTREEITEM hTreeItem);
	void ConsistentParentCheck(HTREEITEM hTreeItem);

// 实现
protected:
	HICON m_hIcon;
	// 绘制设

	// 生成的消息映射函数
	virtual BOOL OnInitDialog();
	afx_msg void OnSysCommand(UINT nID, LPARAM lParam);
	afx_msg void OnPaint();
	afx_msg HCURSOR OnQueryDragIcon();
	DECLARE_MESSAGE_MAP()
	afx_msg void OnClose();
	afx_msg void OnNMClickTreeSource(NMHDR *pNMHDR, LRESULT *pResult);
	afx_msg void OnTimer(UINT_PTR nIDEvent);  //定时器

	// 播放视频
	void PlayVideo(CString filename);
	
	// 初始化树形控件
	void InitDataSource();
	
	// 关闭视频
	void CloseVideo();

	// 显示视频信息
	void DisplayVideoInfo(CString videoname, CString videopath);

	// 显示操作信息
	void DisplayOperateInfo(CString message);

	// 显示检测信息
	void DisplayDetectInfo(int doorlen, int targetcount, int outCount);
public:
	afx_msg void OnBnClickedButton1();
};
