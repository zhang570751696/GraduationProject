#include "stdafx.h"
#include <fstream>
#include "tinyxml.h"
#include "tinystr.h"
#include <string>
#include <Windows.h>
#include <atlstr.h>
#include "OperateData.h"

using namespace std;

COperateData::COperateData()
{
	m_data.clear();
	erroMessage = _T("");
}


COperateData::~COperateData()
{
	m_data.clear();
	erroMessage = _T("");
}

// �洢����
bool COperateData::StroageDataInfo(CSourceModel model, int type)
{
	erroMessage = _T("");
	try
	{
		char* filepath;
		if (type == 0)
		{
			filepath = "C:\\data\\LocalData.xml";
		}
		else
		{
			filepath = "C:\\data\\InterData.xml";
		}

		// ����һ��XML�ĵ�����
		TiXmlDocument *myDocument = new TiXmlDocument(filepath);
		myDocument->LoadFile(TIXML_ENCODING_UTF8);

		TiXmlElement *writerRoot = myDocument->RootElement();
		TiXmlNode* newNode = new TiXmlElement("video");

		const TiXmlNode* nameNewNode = new TiXmlElement("name");
		char* name = CstringToChar(model.name);
		newNode->InsertEndChild(*nameNewNode)->InsertEndChild(TiXmlText(name));

		const TiXmlNode* pathNewNode = new TiXmlElement("path");
		char* path = CstringToChar(model.videopath);
		newNode->InsertEndChild(*pathNewNode)->InsertEndChild(TiXmlText(path));

		writerRoot->InsertEndChild(*newNode);
		myDocument->SaveFile();

		TiXmlDocument doc;
		return true;
	}
	catch (exception ex)
	{
		erroMessage = _T("�洢�����쳣");
		return false;
	}
}

// ɾ������
bool COperateData::DeleteDataInfo(CString name,int type)
{
	erroMessage = _T("");
	bool flag = false;
	try
	{
		char* filepath;
		if (type == 0)
		{
			filepath = "C:\\data\\LocalData.xml";
		}
		else
		{
			filepath = "C:\\data\\InterData.xml";
		}

		char* tempname = CstringToChar(name);

		TiXmlDocument *myDocument = new TiXmlDocument(filepath);
		myDocument->LoadFile(TIXML_ENCODING_UTF8);

		// ��ø�Ԫ�ؼ�localvideo
		TiXmlElement *RootElement = myDocument->RootElement();

		//��õ�һ��Video�ڵ�
		TiXmlElement *firstvideo = RootElement->FirstChildElement();

		while (firstvideo)
		{
			//��õ�һ���ڵ�����
			TiXmlElement *nameElement = firstvideo->FirstChildElement();
			TiXmlElement *pathElement = nameElement->NextSiblingElement();

			const char* temp = nameElement->FirstChild()->Value();
			if (strcmp(temp, tempname) == 0)
			{
				RootElement->RemoveChild(firstvideo);
				myDocument->SaveFile(filepath);
				flag = true;
				break;
			}

			firstvideo = firstvideo->NextSiblingElement();
		}
	}
	catch (exception ex)
	{
		erroMessage = _T("ɾ���쳣");
		flag = false;
	}

	return flag;
}

// �Ƿ��ظ�
bool COperateData::IsRepeat(CString name, int type)
{
	erroMessage = _T("");
	bool flag = false;
	GetData(type);

	for (int i = 0; i <(int)m_data.size(); i++)
	{
		if (m_data[i].name == name)
		{
			flag = true;
			break;
		}
	}

	return flag;
}

///��ȡ����xml��Ƶ����Դ
vector<CSourceModel> COperateData::GetData(int type)
{
	erroMessage = _T("");
	m_data.clear();
	try
	{
		// ����һ��XML�ĵ����� 
		char* filepath;
		if (type == 0)
		{
			filepath = "C:\\data\\LocalData.xml";
		}
		else
		{
			filepath = "C:\\data\\InterData.xml";
		}
		
		TiXmlDocument *myDocument = new TiXmlDocument(filepath);
		myDocument->LoadFile(TIXML_ENCODING_UTF8);

		// ��ø�Ԫ�ؼ�localvideo
		TiXmlElement *RootElement = myDocument->RootElement();

		//��õ�һ��Video�ڵ�
		TiXmlElement *firstvideo = RootElement->FirstChildElement();
		while (firstvideo)
		{
			//��õ�һ���ڵ�����
			TiXmlElement *nameElement = firstvideo->FirstChildElement();
			TiXmlElement *pathElement = nameElement->NextSiblingElement();

			CSourceModel model;
			model.name = nameElement->FirstChild()->Value();
			model.videopath = pathElement->FirstChild()->Value();

			m_data.push_back(model);
			firstvideo = firstvideo->NextSiblingElement();
		}
	}
	catch (exception ex)
	{
		m_data.clear();
		erroMessage = _T("����xml�ļ�����");
	}
	return m_data;
}

// ��ȡ������Ϣ
CString COperateData::GetErrorMessage()
{
	return erroMessage;
}

char* COperateData::CstringToChar(CString str)
{
	int nLen = WideCharToMultiByte(CP_ACP, 0, str, -1, NULL, 0, NULL, NULL);
	if (nLen == 0)
	{
		return NULL;
	}
	char* pResult = new char[nLen];
	WideCharToMultiByte(CP_ACP, 0, str, -1, pResult, nLen, NULL, NULL);
	return pResult;
}