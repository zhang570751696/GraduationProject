// AddVideo.cpp : ʵ���ļ�
//

#include "stdafx.h"
#include "SystemMonitor.h"
#include "AddVideo.h"
#include "afxdialogex.h"
#include "OperateData.h"


// CAddVideo �Ի���

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


// CAddVideo ��Ϣ�������


BOOL CAddVideo::OnInitDialog()
{
	CDialogEx::OnInitDialog();

	// TODO:  �ڴ���Ӷ���ĳ�ʼ��
	m_source.AddString(_T("������Ƶ"));
	m_source.AddString(_T("������Ƶ"));

	m_wndFileEdit.EnableFileBrowseButton();
	m_wndFileEdit.m_strFileType = _T("AVI Files(*.avi)|*.avi|WMV Files(*.wmv)|*.wmv |All Files (*.*)|*.*||");

	return TRUE;  // return TRUE unless you set the focus to a control
	// �쳣:  OCX ����ҳӦ���� FALSE
}


void CAddVideo::OnBnClickedButtonSure()
{
	int nIndex = m_source.GetCurSel();

	CString path;
	GetDlgItemText(IDC_MFCEDITBROWSE_FILE, path);
	
	// ���м���֤
	if (nIndex == -1)
	{
		AfxMessageBox(_T("δѡ����Ƶ���ڵĽڵ�"));
		return;
	}
	GetDlgItemText(IDC_EDIT_NAME, m_name);
	if (m_name == _T(""))
	{
		AfxMessageBox(_T("��������Ƶ����"));
		return;
	}
	if (path == _T(""))
	{
		AfxMessageBox(_T("��ѡ����Ƶ·��"));
		return;
	}

	// ��Ƶ�����ظ���֤
	bool isRepeat = CheckVideoName(nIndex);
	if (isRepeat)
	{
		AfxMessageBox(_T("����Ƶ�����Ѿ����ڣ������"));
		return;
	}

	//���д洢
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
		AfxMessageBox(L"��ӳɹ�");
	}


	CDialogEx::OnOK();
}


void CAddVideo::OnBnClickedButtonCancle()
{
	CDialogEx::OnCancel();
}


// ��֤��Ƶ�����Ƿ��ظ�
bool CAddVideo::CheckVideoName(int videotype)
{
	COperateData operadata;
	return operadata.IsRepeat(m_name, videotype);
}
