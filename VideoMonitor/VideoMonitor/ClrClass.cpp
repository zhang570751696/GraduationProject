#include "ClrClass.h"


CClrClass::CClrClass()
{
	video = new CVideo();
}

void CClrClass::InitVideo(wchar_t* filename)
{
	video->InitVideo(filename);
}

void* CClrClass::GetCurrentFrame()
{
	return video->GetCurrentFrame();
}

void* CClrClass::GetDetectFrame()
{
	return video->GetDetectFrame();
}