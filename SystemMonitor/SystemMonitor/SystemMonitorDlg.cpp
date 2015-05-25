
// SystemMonitorDlg.cpp : 实现文件
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


// 用于应用程序“关于”菜单项的 CAboutDlg 对话框

class CAboutDlg : public CDialogEx
{
public:
	CAboutDlg();

// 对话框数据
	enum { IDD = IDD_ABOUTBOX };

	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV 支持

// 实现
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


// CSystemMonitorDlg 对话框



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


// CSystemMonitorDlg 消息处理程序

BOOL CSystemMonitorDlg::OnInitDialog()
{
	CDialogEx::OnInitDialog();

	// 将“关于...”菜单项添加到系统菜单中。

	// IDM_ABOUTBOX 必须在系统命令范围内。
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

	// 设置此对话框的图标。  当应用程序主窗口不是对话框时，框架将自动
	//  执行此操作
	SetIcon(m_hIcon, TRUE);			// 设置大图标
	SetIcon(m_hIcon, FALSE);		// 设置小图标

	// TODO:  在此添加额外的初始化代码
	m_img = NULL;
	video = new CVideo();
	InitDataSource();

	// 视频播放准备 后面要删除
	startImagePath = "C:\\data\\display.png";
	m_img = cvLoadImage(startImagePath);

	isOpenDetect = false;

	return TRUE;  // 除非将焦点设置到控件，否则返回 TRUE
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

// 如果向对话框添加最小化按钮，则需要下面的代码
//  来绘制该图标。  对于使用文档/视图模型的 MFC 应用程序，
//  这将由框架自动完成。

void CSystemMonitorDlg::OnPaint()
{
	if (IsIconic())
	{
		CPaintDC dc(this); // 用于绘制的设备上下文

		SendMessage(WM_ICONERASEBKGND, reinterpret_cast<WPARAM>(dc.GetSafeHdc()), 0);

		// 使图标在工作区矩形中居中
		int cxIcon = GetSystemMetrics(SM_CXICON);
		int cyIcon = GetSystemMetrics(SM_CYICON);
		CRect rect;
		GetClientRect(&rect);
		int x = (rect.Width() - cxIcon + 1) / 2;
		int y = (rect.Height() - cyIcon + 1) / 2;

		// 绘制图标
		dc.DrawIcon(x, y, m_hIcon);
	}
	else
	{
		// 获取绘制设备
		CRect rect;
		CDC *pDC = GetDlgItem(IDC_STATIC_PIC)->GetDC();
		HDC hDC = pDC->GetSafeHdc();
		GetDlgItem(IDC_STATIC_PIC)->GetClientRect(&rect);

		// 采用双缓冲作图
		CDC dcMem;  // 用于缓冲作图的内存DC
		CBitmap bmp; //内存中承载临时图象的位图
		dcMem.CreateCompatibleDC(NULL);  //依附窗口DC创建兼容内存DC
		bmp.CreateCompatibleBitmap(pDC, rect.Width(), rect.Height());//创建兼容位图
		dcMem.SelectObject(&bmp);//将位图选择进内存DC
		dcMem.FillSolidRect(rect, pDC->GetBkColor());//按原来背景填充客户区，不然会是黑色
		dcMem.SelectStockObject(NULL_BRUSH);

		CvvImage cimg;
		cimg.CopyOf(m_img);
		cimg.DrawToHDC(dcMem, &rect);

		pDC->BitBlt(0, 0, rect.Width(), rect.Height(), &dcMem, 0, 0, SRCCOPY);//将内存DC上的图象拷贝到前台
		dcMem.DeleteDC();                                       //删除DC
		bmp.DeleteObject();                                        //删除位图
		ReleaseDC(pDC);

		CDialogEx::OnPaint();
	}
}

//当用户拖动最小化窗口时系统调用此函数取得光标
//显示。
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


// 初始化树形控件
void CSystemMonitorDlg::InitDataSource()
{
	//　先进行清空
	m_treeContrl.DeleteAllItems();
	model.clear();

	vector<CSourceModel> loaclmodel;
	vector<CSourceModel> intermodel;

	HTREEITEM hImgNodeGlobal = m_treeContrl.InsertItem(_T("视频源"), 0, 0);

	HTREEITEM hImgNodeDom = m_treeContrl.InsertItem(_T("本地视频"), 0, 0, hImgNodeGlobal, TVI_LAST);

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

	HTREEITEM hImgNodeDem = m_treeContrl.InsertItem(_T("网络视频"), 0, 0, hImgNodeGlobal, TVI_LAST);
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

		// 为了保持一致化属性控件的选中状态，需设置当前选中项的复选框为改变后的状态
		m_treeContrl.SetCheck(oSelectItem, bCheck);

		// 一致化树形控件复选框状态
		ConsistentChildCheck(oSelectItem);
		ConsistentParentCheck(oSelectItem);

		// 将当前选中项的复选框状态复原，MFC会自动响应复选框状态的改变绘制
		m_treeContrl.SetCheck(oSelectItem, !bCheck);
	}
	*pResult = 0;
}

void CSystemMonitorDlg::ConsistentParentCheck(HTREEITEM hTreeItem)
{
	// 获取当前选中项的父节点，如果父节点为空则返回，否则处理父节点状态
	HTREEITEM hParentItem = m_treeContrl.GetParentItem(hTreeItem);
	if (hParentItem != NULL)
	{
		// 一次判断当前选中项的父节点的各个字节点的状态
		HTREEITEM hChildItem = m_treeContrl.GetChildItem(hParentItem);
		while (hChildItem != NULL)
		{
			// 如果父节点有一个子节点未被选中，则父节点设置为未选中状态
			// 同时递归处理父节点的父节点
			if (m_treeContrl.GetCheck(hChildItem) == FALSE)
			{
				m_treeContrl.SetCheck(hParentItem, FALSE);
				return ConsistentParentCheck(hParentItem);
			}

			// 获取父节点的下一个子节点
			hChildItem = m_treeContrl.GetNextItem(hChildItem, TVGN_NEXT);
		}

		m_treeContrl.SetCheck(hParentItem, TRUE);
		return ConsistentParentCheck(hParentItem);
	}
}

void CSystemMonitorDlg::ConsistentChildCheck(HTREEITEM hTreeItem)
{
	// 获取当前选中项的选择复选框状态
	BOOL bCheck = m_treeContrl.GetCheck(hTreeItem);

	// 如果当前节点存在子节点，则一致化子节点状态
	HTREEITEM hChildItem = m_treeContrl.GetChildItem(hTreeItem);
	while (hChildItem != NULL)
	{
		m_treeContrl.SetCheck(hChildItem, bCheck);
		ConsistentChildCheck(hChildItem);

		hChildItem = m_treeContrl.GetNextItem(hChildItem, TVGN_NEXT);
	}
}

// 定时器启动
void CSystemMonitorDlg::OnTimer(UINT_PTR nIDEvent)
{
	// TODO:  在此添加消息处理程序代码和/或调用默认值
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

// 播放视频
void CSystemMonitorDlg::PlayVideo(CString filename)
{
	for (int i = 0; i < (int)model.size(); i++)
	{
		if (model[i].name == filename)
		{
			DWORD dwMinSize;
			dwMinSize = WideCharToMultiByte(CP_ACP, NULL, model[i].videopath, -1, NULL, 0, NULL, FALSE); // 计算长度
			char * tempchar = new char[dwMinSize];
			WideCharToMultiByte(CP_OEMCP, NULL, model[i].videopath, -1, tempchar, dwMinSize, NULL, FALSE);
			video->InitVideo(tempchar);
			DisplayVideoInfo(model[i].name, model[i].videopath);
			DisplayOperateInfo(_T("你打开了") + model[i].name);
			SetTimer(1, 150, NULL);
			break;
		}
	}
}

// 关闭视频
void CSystemMonitorDlg::CloseVideo()
{
	video->CloseVideo();
	KillTimer(1);
	m_img = cvLoadImage(startImagePath);
	DisplayVideoInfo(_T("暂无选择视频"), _T("未获取到视频路径"));
	DisplayOperateInfo(_T("你关闭了视频"));
	Invalidate(FALSE);
}

// 显示视频信息
void CSystemMonitorDlg::DisplayVideoInfo(CString videoname, CString videopath)
{
	SetDlgItemText(IDC_STATIC_NAME, videoname);
	SetDlgItemText(IDC_STATIC_PATH, videopath);
}

// 显示操作信息
void CSystemMonitorDlg::DisplayOperateInfo(CString message)
{
	CString temp;
	GetDlgItemText(IDC_STATIC_OPERATE, temp);
	temp = temp + "\n" + message;
	SetDlgItemText(IDC_STATIC_OPERATE, temp);
}

// 开启检测
void CSystemMonitorDlg::OnBnClickedButton1()
{
	bool videostate = video->GetVideoState();
	if (videostate)
	{
		isOpenDetect = !isOpenDetect;
		if (isOpenDetect)
		{
			SetDlgItemText(IDC_BUTTON_OPEN, _T("关闭检测"));
			DisplayOperateInfo(_T("你开启了检测功能"));
		}
		else
		{
			SetDlgItemText(IDC_BUTTON_OPEN, _T("开启检测"));
			DisplayOperateInfo(_T("你关闭了检测功能"));
		}
	}
	else
	{
		AfxMessageBox(_T("请先播放视频"));
	}
}

// 显示检测信息
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

// 添加视频源
void CSystemMonitorDlg::OnBnClickedButtonAdd()
{
	//AfxMessageBox(_T("等待实现"));
	CAddVideo addvideo;
	addvideo.DoModal();
	InitDataSource();
}

// 删除视频源
void CSystemMonitorDlg::OnBnClickedButtonDelete()
{
	//AfxMessageBox(_T("等待实现"));
	CDeleteVideo deleteVideo;
	deleteVideo.DoModal();
	InitDataSource();
}


BOOL CSystemMonitorDlg::OnEraseBkgnd(CDC* pDC)
{
	return CDialogEx::OnEraseBkgnd(pDC);
}
