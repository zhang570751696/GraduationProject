#pragma once
#include "Video.h"

public ref class CClrClass
{
public:
	CClrClass();

	void InitVideo(wchar_t* filename);
	void* GetCurrentFrame();
	void* GetDetectFrame();

private :
	CVideo *video;
};

