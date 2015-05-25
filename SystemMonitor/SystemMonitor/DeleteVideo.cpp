// DeleteVideo.cpp : 实现文件
//

#include "stdafx.h"
#include "SystemMonitor.h"
#include "DeleteVideo.h"
#include "afxdialogex.h"
#include "SourceModel.h"
#include "OperateData.h"

// CDeleteVideo 对话框

IMPLEMENT_DYNAMIC(CDeleteVideo, CDialogEx)

CDeleteVideo::CDeleteVideo(CWnd* pParent /*=NULL*/)
	: CDialogEx(CDeleteVideo::IDD, pParent)
{

}

CDeleteVideo::~CDeleteVideo()
{
}

void CDeleteVideo::DoDataExchange(CDataExchange* pDX)
{
	CDialogEx::DoDataExchange(pDX);
	DDX_Control(pDX, IDC_COMBO_NAME, m_name);
	DDX_Control(pDX, IDC_COMBO_SOUR, m_parent);
}


BEGIN_MESSAGE_MAP(CDeleteVideo, CDialogEx)
	ON_BN_CLICKED(IDC_BUTTON_OK, &CDeleteVideo::OnBnClickedButtonOk)
	ON_BN_CLICKED(IDC_BUTTON_CANCLE, &CDeleteVideo::OnBnClickedButtonCancle)
	ON_CBN_DROPDOWN(IDC_COMBO_NAME, &CDeleteVideo::OnCbnDropdownComboName)
END_MESSAGE_MAP()


// CDeleteVideo 消息处理程序


void CDeleteVideo::OnBnClickedButtonOk()
{
	int nIndex = m_name.GetCurSel();
	int index = m_parent.GetCurSel(); // 父节点所在的位置

	CString constr;
	if (nIndex == -1)
	{
		AfxMessageBox(_T("请选择要删除的视频名称"));
		return;
	}
	m_name.GetLBText(nIndex, constr);
	COperateData *data = new COperateData();
	bool flag = data->DeleteDataInfo(constr,index);
	if (!flag)
	{
		CString mess = data->GetErrorMessage();
		AfxMessageBox(mess); 
		return;
	}
	else
	{
		AfxMessageBox(_T("删除成功"));
	}

	CDialog::OnOK();
}


void CDeleteVideo::OnBnClickedButtonCancle()
{
	CDialog::OnCancel();
}


void CDeleteVideo::OnCbnDropdownComboName()
{
	int nIndex = m_parent.GetCurSel();
	vector<CSourceModel> model;
	COperateData *data = new COperateData();

	model.clear();
	m_name.ResetContent();

	if (nIndex == -1)
	{
		AfxMessageBox(_T("请先选择所处的节点"));
		return;
	}
	else if (nIndex ==0) // 本地视频
	{
		model = data->GetData(0);
	}
	else  // 网络视频
	{
		model = data->GetData(1);
	}

	for (int i = 0; i < (int)model.size(); i++)
	{
		m_name.AddString(model[i].name);
	}

	delete data;
}


BOOL CDeleteVideo::OnInitDialog()
{
	CDialogEx::OnInitDialog();

	// TODO:  在此添加额外的初始化
	m_parent.AddString(_T("本地视频"));
	m_parent.AddString(_T("网络视频"));

	return TRUE;  // return TRUE unless you set the focus to a control
	// 异常:  OCX 属性页应返回 FALSE
}
