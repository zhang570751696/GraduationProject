// AddVideo.cpp : 实现文件
//

#include "stdafx.h"
#include "SystemMonitor.h"
#include "AddVideo.h"
#include "afxdialogex.h"
#include "OperateData.h"


// CAddVideo 对话框

IMPLEMENT_DYNAMIC(CAddVideo, CDialogEx)

CAddVideo::CAddVideo(CWnd* pParent /*=NULL*/)
	: CDialogEx(CAddVideo::IDD, pParent)
	, m_name(_T(""))
{

}

CAddVideo::~CAddVideo()
{
}

void CAddVideo::DoDataExchange(CDataExchange* pDX)
{
	CDialogEx::DoDataExchange(pDX);
	DDX_Control(pDX, IDC_COMBO_SOURCE, m_source);
	DDX_Text(pDX, IDC_EDIT_NAME, m_name);
	DDX_Control(pDX, IDC_MFCEDITBROWSE_FILE, m_wndFileEdit);
}


BEGIN_MESSAGE_MAP(CAddVideo, CDialogEx)
	ON_BN_CLICKED(IDC_BUTTON_SURE, &CAddVideo::OnBnClickedButtonSure)
	ON_BN_CLICKED(IDC_BUTTON_CANCLE, &CAddVideo::OnBnClickedButtonCancle)
END_MESSAGE_MAP()


// CAddVideo 消息处理程序


BOOL CAddVideo::OnInitDialog()
{
	CDialogEx::OnInitDialog();

	// TODO:  在此添加额外的初始化
	m_source.AddString(_T("本地视频"));
	m_source.AddString(_T("网络视频"));

	m_wndFileEdit.EnableFileBrowseButton();
	m_wndFileEdit.m_strFileType = _T("AVI Files(*.avi)|*.avi|WMV Files(*.wmv)|*.wmv |All Files (*.*)|*.*||");

	return TRUE;  // return TRUE unless you set the focus to a control
	// 异常:  OCX 属性页应返回 FALSE
}


void CAddVideo::OnBnClickedButtonSure()
{
	int nIndex = m_source.GetCurSel();

	CString path;
	GetDlgItemText(IDC_MFCEDITBROWSE_FILE, path);
	
	// 进行简单验证
	if (nIndex == -1)
	{
		AfxMessageBox(_T("未选择视频所在的节点"));
		return;
	}
	GetDlgItemText(IDC_EDIT_NAME, m_name);
	if (m_name == _T(""))
	{
		AfxMessageBox(_T("请输入视频名称"));
		return;
	}
	if (path == _T(""))
	{
		AfxMessageBox(_T("请选择视频路径"));
		return;
	}

	// 视频名称重复验证
	bool isRepeat = CheckVideoName(nIndex);
	if (isRepeat)
	{
		AfxMessageBox(_T("该视频名称已经存在，请更换"));
		return;
	}

	//进行存储
	CSourceModel model;
	model.name = m_name;
	model.videopath = path;
	COperateData operadata;
	bool flag = operadata.StroageDataInfo(model, nIndex);
	if (!flag)
	{
		CString mess = operadata.GetErrorMessage();
		AfxMessageBox(mess);
		return;
	}
	else
	{
		AfxMessageBox(L"添加成功");
	}


	CDialogEx::OnOK();
}


void CAddVideo::OnBnClickedButtonCancle()
{
	CDialogEx::OnCancel();
}


// 验证视频名称是否重复
bool CAddVideo::CheckVideoName(int videotype)
{
	COperateData operadata;
	return operadata.IsRepeat(m_name, videotype);
}
