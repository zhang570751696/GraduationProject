// Mointor.h

#pragma once
#include "Video.h"

using namespace System;

namespace Mointor {

	public ref class CMonitor
	{
	public:
		CMonitor();
		~CMonitor();
		// TODO:  �ڴ˴���Ӵ���ķ�����
		void InitVideo(wchar_t *filename);

		void* GetCurrentFrame();

		void* GetDetectFeame();
	private:
		CVideo *video;
	};
}
