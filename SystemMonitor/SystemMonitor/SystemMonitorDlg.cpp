
// SystemMonitorDlg.cpp : ʵ���ļ�
//

#include "stdafx.h"
#include "SystemMonitor.h"
#include "SystemMonitorDlg.h"
#include "afxdialogex.h"
#include "AddVideo.h"
#include "DeleteVideo.h"


#ifdef _DEBUG
#define new DEBUG_NEW
#endif


// ����Ӧ�ó��򡰹��ڡ��˵���� CAboutDlg �Ի���

class CAboutDlg : public CDialogEx
{
public:
	CAboutDlg();

// �Ի�������
	enum { IDD = IDD_ABOUTBOX };

	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV ֧��

// ʵ��
protected:
	DECLARE_MESSAGE_MAP()
};

CAboutDlg::CAboutDlg() : CDialogEx(CAboutDlg::IDD)
{
}

void CAboutDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialogEx::DoDataExchange(pDX);
}

BEGIN_MESSAGE_MAP(CAboutDlg, CDialogEx)
END_MESSAGE_MAP()


// CSystemMonitorDlg �Ի���



CSystemMonitorDlg::CSystemMonitorDlg(CWnd* pParent /*=NULL*/)
	: CDialogEx(CSystemMonitorDlg::IDD, pParent)
{
	m_hIcon = AfxGetApp()->LoadIcon(IDR_MAINFRAME);
}

void CSystemMonitorDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialogEx::DoDataExchange(pDX);
	DDX_Control(pDX, IDC_TREE_SOURCE, m_treeContrl);
}

BEGIN_MESSAGE_MAP(CSystemMonitorDlg, CDialogEx)
	ON_WM_SYSCOMMAND()
	ON_WM_PAINT()
	ON_WM_QUERYDRAGICON()
	ON_WM_CLOSE()
	ON_NOTIFY(NM_CLICK, IDC_TREE_SOURCE, &CSystemMonitorDlg::OnNMClickTreeSource)
	ON_WM_TIMER()
	ON_BN_CLICKED(IDC_BUTTON_OPEN, &CSystemMonitorDlg::OnBnClickedButton1)
	ON_BN_CLICKED(IDC_BUTTON_ADD, &CSystemMonitorDlg::OnBnClickedButtonAdd)
	ON_BN_CLICKED(IDC_BUTTON_DELETE, &CSystemMonitorDlg::OnBnClickedButtonDelete)
	ON_WM_ERASEBKGND()
END_MESSAGE_MAP()


// CSystemMonitorDlg ��Ϣ�������

BOOL CSystemMonitorDlg::OnInitDialog()
{
	CDialogEx::OnInitDialog();

	// ��������...���˵�����ӵ�ϵͳ�˵��С�

	// IDM_ABOUTBOX ������ϵͳ���Χ�ڡ�
	ASSERT((IDM_ABOUTBOX & 0xFFF0) == IDM_ABOUTBOX);
	ASSERT(IDM_ABOUTBOX < 0xF000);

	CMenu* pSysMenu = GetSystemMenu(FALSE);
	if (pSysMenu != NULL)
	{
		BOOL bNameValid;
		CString strAboutMenu;
		bNameValid = strAboutMenu.LoadString(IDS_ABOUTBOX);
		ASSERT(bNameValid);
		if (!strAboutMenu.IsEmpty())
		{
			pSysMenu->AppendMenu(MF_SEPARATOR);
			pSysMenu->AppendMenu(MF_STRING, IDM_ABOUTBOX, strAboutMenu);
		}
	}

	// ���ô˶Ի����ͼ�ꡣ  ��Ӧ�ó��������ڲ��ǶԻ���ʱ����ܽ��Զ�
	//  ִ�д˲���
	SetIcon(m_hIcon, TRUE);			// ���ô�ͼ��
	SetIcon(m_hIcon, FALSE);		// ����Сͼ��

	// TODO:  �ڴ���Ӷ���ĳ�ʼ������
	m_img = NULL;
	video = new CVideo();
	InitDataSource();

	// ��Ƶ����׼�� ����Ҫɾ��
	startImagePath = "C:\\data\\display.png";
	m_img = cvLoadImage(startImagePath);

	isOpenDetect = false;

	return TRUE;  // ���ǽ��������õ��ؼ������򷵻� TRUE
}

void CSystemMonitorDlg::OnSysCommand(UINT nID, LPARAM lParam)
{
	if ((nID & 0xFFF0) == IDM_ABOUTBOX)
	{
		CAboutDlg dlgAbout;
		dlgAbout.DoModal();
	}
	else
	{
		CDialogEx::OnSysCommand(nID, lParam);
	}
}

// �����Ի��������С����ť������Ҫ����Ĵ���
//  �����Ƹ�ͼ�ꡣ  ����ʹ���ĵ�/��ͼģ�͵� MFC Ӧ�ó���
//  �⽫�ɿ���Զ���ɡ�

void CSystemMonitorDlg::OnPaint()
{
	if (IsIconic())
	{
		CPaintDC dc(this); // ���ڻ��Ƶ��豸������

		SendMessage(WM_ICONERASEBKGND, reinterpret_cast<WPARAM>(dc.GetSafeHdc()), 0);

		// ʹͼ���ڹ����������о���
		int cxIcon = GetSystemMetrics(SM_CXICON);
		int cyIcon = GetSystemMetrics(SM_CYICON);
		CRect rect;
		GetClientRect(&rect);
		int x = (rect.Width() - cxIcon + 1) / 2;
		int y = (rect.Height() - cyIcon + 1) / 2;

		// ����ͼ��
		dc.DrawIcon(x, y, m_hIcon);
	}
	else
	{
		// ��ȡ�����豸
		CRect rect;
		CDC *pDC = GetDlgItem(IDC_STATIC_PIC)->GetDC();
		HDC hDC = pDC->GetSafeHdc();
		GetDlgItem(IDC_STATIC_PIC)->GetClientRect(&rect);

		// ����˫������ͼ
		CDC dcMem;  // ���ڻ�����ͼ���ڴ�DC
		CBitmap bmp; //�ڴ��г�����ʱͼ���λͼ
		dcMem.CreateCompatibleDC(NULL);  //��������DC���������ڴ�DC
		bmp.CreateCompatibleBitmap(pDC, rect.Width(), rect.Height());//��������λͼ
		dcMem.SelectObject(&bmp);//��λͼѡ����ڴ�DC
		dcMem.FillSolidRect(rect, pDC->GetBkColor());//��ԭ���������ͻ�������Ȼ���Ǻ�ɫ
		dcMem.SelectStockObject(NULL_BRUSH);

		CvvImage cimg;
		cimg.CopyOf(m_img);
		cimg.DrawToHDC(dcMem, &rect);

		pDC->BitBlt(0, 0, rect.Width(), rect.Height(), &dcMem, 0, 0, SRCCOPY);//���ڴ�DC�ϵ�ͼ�󿽱���ǰ̨
		dcMem.DeleteDC();                                       //ɾ��DC
		bmp.DeleteObject();                                        //ɾ��λͼ
		ReleaseDC(pDC);

		CDialogEx::OnPaint();
	}
}

//���û��϶���С������ʱϵͳ���ô˺���ȡ�ù��
//��ʾ��
HCURSOR CSystemMonitorDlg::OnQueryDragIcon()
{
	return static_cast<HCURSOR>(m_hIcon);
}

void CSystemMonitorDlg::OnClose()
{
	if (video->GetVideoState())
	{
		video->CloseVideo();
	}

	//delete video;
	//cvReleaseImage(&m_img);

	CDialogEx::OnClose();
}


// ��ʼ�����οؼ�
void CSystemMonitorDlg::InitDataSource()
{
	//���Ƚ������
	m_treeContrl.DeleteAllItems();
	model.clear();

	vector<CSourceModel> loaclmodel;
	vector<CSourceModel> intermodel;

	HTREEITEM hImgNodeGlobal = m_treeContrl.InsertItem(_T("��ƵԴ"), 0, 0);

	HTREEITEM hImgNodeDom = m_treeContrl.InsertItem(_T("������Ƶ"), 0, 0, hImgNodeGlobal, TVI_LAST);

	COperateData data;
	//loaclmodel = data.GetData();
	loaclmodel = data.GetData(0);
	if ((int)loaclmodel.size() > 0)
	{
		for (int i = 0; i < (int)loaclmodel.size(); i++)
		{
			if (loaclmodel[i].name != _T(""))
			{
				HTREEITEM hImgNodeData = m_treeContrl.InsertItem(loaclmodel[i].name, 0, 0, hImgNodeDom, TVI_LAST);
				model.push_back(loaclmodel[i]);
			}
		}
	}

	HTREEITEM hImgNodeDem = m_treeContrl.InsertItem(_T("������Ƶ"), 0, 0, hImgNodeGlobal, TVI_LAST);
	intermodel = data.GetData(1);
	if ((int)intermodel.size()>0)
	{
		for (int i = 0; i < (int)intermodel.size(); i++)
		{
			if (intermodel[i].name != _T(""))
			{
				HTREEITEM hImgNodeData = m_treeContrl.InsertItem(intermodel[i].name, 0, 0, hImgNodeDem, TVI_LAST);
				model.push_back(intermodel[i]);
			}
		}
	}
}


void CSystemMonitorDlg::OnNMClickTreeSource(NMHDR *pNMHDR, LRESULT *pResult)
{
	CPoint oPoint;
	UINT nFlag;
	GetCursorPos(&oPoint);
	m_treeContrl.ScreenToClient(&oPoint);
	HTREEITEM oSelectItem = m_treeContrl.HitTest(oPoint, &nFlag);
	if (oSelectItem == NULL) 
	{
		return;
	}

	if (nFlag & TVHT_ONITEMSTATEICON)
	{
		BOOL bCheck = !m_treeContrl.GetCheck(oSelectItem);
		if (bCheck)
		{
			PlayVideo(m_treeContrl.GetItemText(oSelectItem));
			//AfxMessageBox(m_treeContrl.GetItemText(oSelectItem));
		}
		else
		{
			CloseVideo();
		}

		// Ϊ�˱���һ�»����Կؼ���ѡ��״̬�������õ�ǰѡ����ĸ�ѡ��Ϊ�ı���״̬
		m_treeContrl.SetCheck(oSelectItem, bCheck);

		// һ�»����οؼ���ѡ��״̬
		ConsistentChildCheck(oSelectItem);
		ConsistentParentCheck(oSelectItem);

		// ����ǰѡ����ĸ�ѡ��״̬��ԭ��MFC���Զ���Ӧ��ѡ��״̬�ĸı����
		m_treeContrl.SetCheck(oSelectItem, !bCheck);
	}
	*pResult = 0;
}

void CSystemMonitorDlg::ConsistentParentCheck(HTREEITEM hTreeItem)
{
	// ��ȡ��ǰѡ����ĸ��ڵ㣬������ڵ�Ϊ���򷵻أ��������ڵ�״̬
	HTREEITEM hParentItem = m_treeContrl.GetParentItem(hTreeItem);
	if (hParentItem != NULL)
	{
		// һ���жϵ�ǰѡ����ĸ��ڵ�ĸ����ֽڵ��״̬
		HTREEITEM hChildItem = m_treeContrl.GetChildItem(hParentItem);
		while (hChildItem != NULL)
		{
			// ������ڵ���һ���ӽڵ�δ��ѡ�У��򸸽ڵ�����Ϊδѡ��״̬
			// ͬʱ�ݹ鴦���ڵ�ĸ��ڵ�
			if (m_treeContrl.GetCheck(hChildItem) == FALSE)
			{
				m_treeContrl.SetCheck(hParentItem, FALSE);
				return ConsistentParentCheck(hParentItem);
			}

			// ��ȡ���ڵ����һ���ӽڵ�
			hChildItem = m_treeContrl.GetNextItem(hChildItem, TVGN_NEXT);
		}

		m_treeContrl.SetCheck(hParentItem, TRUE);
		return ConsistentParentCheck(hParentItem);
	}
}

void CSystemMonitorDlg::ConsistentChildCheck(HTREEITEM hTreeItem)
{
	// ��ȡ��ǰѡ�����ѡ��ѡ��״̬
	BOOL bCheck = m_treeContrl.GetCheck(hTreeItem);

	// �����ǰ�ڵ�����ӽڵ㣬��һ�»��ӽڵ�״̬
	HTREEITEM hChildItem = m_treeContrl.GetChildItem(hTreeItem);
	while (hChildItem != NULL)
	{
		m_treeContrl.SetCheck(hChildItem, bCheck);
		ConsistentChildCheck(hChildItem);

		hChildItem = m_treeContrl.GetNextItem(hChildItem, TVGN_NEXT);
	}
}

// ��ʱ������
void CSystemMonitorDlg::OnTimer(UINT_PTR nIDEvent)
{
	// TODO:  �ڴ������Ϣ�����������/�����Ĭ��ֵ
	IplImage* tep = video->GetCurrentFrame();
	if (tep)
	{
		if (isOpenDetect)
		{
			m_img = video->GetDetectFrame();
			DisplayDetectInfo(video->GetDoorLen(), video->GetTargetCount(), video->GetOutCount());
		}
		else
		{
			m_img = tep;
		}
		Invalidate(FALSE);
	}

	CDialogEx::OnTimer(nIDEvent);
}

// ������Ƶ
void CSystemMonitorDlg::PlayVideo(CString filename)
{
	for (int i = 0; i < (int)model.size(); i++)
	{
		if (model[i].name == filename)
		{
			DWORD dwMinSize;
			dwMinSize = WideCharToMultiByte(CP_ACP, NULL, model[i].videopath, -1, NULL, 0, NULL, FALSE); // ���㳤��
			char * tempchar = new char[dwMinSize];
			WideCharToMultiByte(CP_OEMCP, NULL, model[i].videopath, -1, tempchar, dwMinSize, NULL, FALSE);
			video->InitVideo(tempchar);
			DisplayVideoInfo(model[i].name, model[i].videopath);
			DisplayOperateInfo(_T("�����") + model[i].name);
			SetTimer(1, 150, NULL);
			break;
		}
	}
}

// �ر���Ƶ
void CSystemMonitorDlg::CloseVideo()
{
	video->CloseVideo();
	KillTimer(1);
	m_img = cvLoadImage(startImagePath);
	DisplayVideoInfo(_T("����ѡ����Ƶ"), _T("δ��ȡ����Ƶ·��"));
	DisplayOperateInfo(_T("��ر�����Ƶ"));
	Invalidate(FALSE);
}

// ��ʾ��Ƶ��Ϣ
void CSystemMonitorDlg::DisplayVideoInfo(CString videoname, CString videopath)
{
	SetDlgItemText(IDC_STATIC_NAME, videoname);
	SetDlgItemText(IDC_STATIC_PATH, videopath);
}

// ��ʾ������Ϣ
void CSystemMonitorDlg::DisplayOperateInfo(CString message)
{
	CString temp;
	GetDlgItemText(IDC_STATIC_OPERATE, temp);
	temp = temp + "\n" + message;
	SetDlgItemText(IDC_STATIC_OPERATE, temp);
}

// �������
void CSystemMonitorDlg::OnBnClickedButton1()
{
	bool videostate = video->GetVideoState();
	if (videostate)
	{
		isOpenDetect = !isOpenDetect;
		if (isOpenDetect)
		{
			SetDlgItemText(IDC_BUTTON_OPEN, _T("�رռ��"));
			DisplayOperateInfo(_T("�㿪���˼�⹦��"));
		}
		else
		{
			SetDlgItemText(IDC_BUTTON_OPEN, _T("�������"));
			DisplayOperateInfo(_T("��ر��˼�⹦��"));
		}
	}
	else
	{
		AfxMessageBox(_T("���Ȳ�����Ƶ"));
	}
}

// ��ʾ�����Ϣ
void CSystemMonitorDlg::DisplayDetectInfo(int doorlen, int targetcount, int outCount)
{
	CString temp;
	temp.Format(_T("%d"), outCount);
	SetDlgItemText(IDC_STATIC_OUT, temp);
	temp.Format(_T("%d"), doorlen);
	SetDlgItemText(IDC_STATIC_DOOR, temp);
	temp.Format(_T("%d"), targetcount);
	SetDlgItemText(IDC_STATIC_TARGET, temp);
}

// �����ƵԴ
void CSystemMonitorDlg::OnBnClickedButtonAdd()
{
	//AfxMessageBox(_T("�ȴ�ʵ��"));
	CAddVideo addvideo;
	addvideo.DoModal();
	InitDataSource();
}

// ɾ����ƵԴ
void CSystemMonitorDlg::OnBnClickedButtonDelete()
{
	//AfxMessageBox(_T("�ȴ�ʵ��"));
	CDeleteVideo deleteVideo;
	deleteVideo.DoModal();
	InitDataSource();
}


BOOL CSystemMonitorDlg::OnEraseBkgnd(CDC* pDC)
{
	return CDialogEx::OnEraseBkgnd(pDC);
}
