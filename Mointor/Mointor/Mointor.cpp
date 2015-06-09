// ������ DLL �ļ���

#include "stdafx.h"

#include "Mointor.h"

namespace Mointor
{
	CMonitor::CMonitor()
	{
		video = new CVideo();
	}

	CMonitor::~CMonitor()
	{
		delete video;
	}

	void CMonitor::InitVideo(wchar_t *filename)
	{
		video->InitVideo(filename);
	}

	void* CMonitor::GetCurrentFrame()
	{
		return video->GetCurrentFrame();
	}

	void* CMonitor::GetDetectFeame()
	{
		return video->GetDetectFrame();
	}
}

