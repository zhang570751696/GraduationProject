// 这是主 DLL 文件。

#include "stdafx.h"

#include "Mointor.h"

namespace Mointor
{
	CMonitor::CMonitor()
	{
	}

	CMonitor::~CMonitor()
	{
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

