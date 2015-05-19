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
		// TODO:  在此处添加此类的方法。
		void InitVideo(wchar_t *filename);

		void* GetCurrentFrame();

		void* GetDetectFeame();
	private:
		CVideo *video;
	};
}
