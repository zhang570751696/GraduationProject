#include <winsock2.h>
#include <stdio.h>
#include <windows.h>
#include <fstream>

using namespace std;

#pragma comment (lib,"WS2_32.lib")
int total = 0;



//DWORD WINAPI SendThread(LPVOID lparam)
//{
//
//	SOCKET s = (SOCKET)lparam;
//	while (true)
//	{
//		if (s == NULL)
//		{
//			break;
//		}
//
//		// 发送数据
//		char senlen[20];
//		ZeroMemory(senlen, 20);
//		sprintf(senlen, "%d", sendsize);
//		::send(s, senlen, strlen(senlen), 0);
//		int coun = sendsize / 1024;
//		int r = sendsize % 1024;
//
//		Sleep(100);
//		// 打开文件
//		fstream fcin;
//		fcin.open("data.bin", ios::in | ios::binary);
//		char sendMess[1025];
//		for (int j = 0; j < coun; j++)
//		{
//			ZeroMemory(sendMess, 1025);
//			fcin.get(sendMess, 1024);
//			fcin.seekg(1024, ios::cur);
//			::send(s, sendMess, strlen(sendMess), 0);
//		}
//
//		if (r != 0)
//		{
//			ZeroMemory(sendMess, 1025);
//			fcin.get(sendMess, r);
//			::send(s, sendMess, strlen(sendMess), 0);
//		}
//
//		fcin.close();
//	}
//	return 0;
//}

DWORD WINAPI ClientThread(LPVOID lparam)
{
	SOCKET s = (SOCKET)lparam;
	char buf[20];
	int sendsize = 0;
	while (1)
	{
		Sleep(100);
		ZeroMemory(buf, 20);
		int retval = ::recv(s, buf, 20, 0);
		if (retval > 0)
		{
			char buffer[20];
			ZeroMemory(buffer, 20);
			int count = 0;
			for (int i = 0; i < retval; i++)
			{
				if (buf[i] != '\0')
				{
					buffer[count++] = buf[i];
				}
			}
			buffer[count] = '\0';

			// 表示接收长度
			int reciveLen = atoi(buffer);

			printf("获取到图像大小%d\n", reciveLen);

			// 现在接收到的长度
			int size = 0;

			// 定义一个文件对象，来缓存图片信息
			ofstream ocout;
			char data[1025];
			ocout.open("data.bin", ios::out | ios::app | ios::binary );
			//CMemFile memFile;
			while (size < reciveLen)
			{
				ZeroMemory(data, 1025);
				int res = ::recv(s, data, 1024, 0);
				if (res > 0)
				{
					char buff[1025];
					int counter = 0;
					if (res > 1024)
					{
						int i = res;
					}
					for (int i = 0; i < res; i++)
					{
						if (data[i] != '\0')
						{
							buff[counter++] = data[i];
						}
					}

					buff[counter] = '\0';
					sendsize = counter + sendsize - 1;
					ocout << buff;
					//memFile.Write(buff, strlen(buff));
					size += res;
				}
				else
				{
					break;
				}
			}

			// 文件关闭
			ocout.close();
			printf("图像接收完毕\n");
			//CreateThread(NULL, NULL, SendThread, (LPVOID)s, 0, 0);
			// 发送数据
			char senlen[20];
			ZeroMemory(senlen, 20);
			printf("发送图像大小:%d\n", sendsize);
			sprintf(senlen, "%d", sendsize);
			::send(s, senlen, strlen(senlen), 0);
			int coun = sendsize / 1024;
			int r = sendsize % 1024;

			Sleep(100);
			// 打开文件
			fstream fcin;
			fcin.open("data.bin", ios::in | ios::binary);
			char sendMess[1025];
			for (int j = 0; j < coun; j++)
			{
				ZeroMemory(sendMess, 1025);
				fcin.get(sendMess, 1024);
				fcin.seekg(1024, ios::cur);
				::send(s, sendMess, strlen(sendMess), 0);
			}

			if (r != 0)
			{
				ZeroMemory(sendMess, 1025);
				fcin.get(sendMess, r);
				::send(s, sendMess, strlen(sendMess), 0);
			}

			fcin.close();
		}
		else
		{
			total--;
			break;
		}

		return 0;
	}
}




int main()
{
	WSADATA wsaData;
	if (WSAStartup(MAKEWORD(2, 2), &wsaData) != 0)
	{
		printf("WSAStartup Initlazetion errror!\n");
		return -1;
	}

	SOCKET sServer = socket(AF_INET, SOCK_STREAM, IPPROTO_TCP);
	if (INVALID_SOCKET == sServer)
	{
		printf("socket failed\n");
		WSACleanup();
		return -1;
	}


	SOCKADDR_IN addrServ;
	addrServ.sin_family = AF_INET;
	addrServ.sin_port = htons(8888);
	addrServ.sin_addr.S_un.S_addr = htonl(INADDR_ANY);

	int retval = bind(sServer, (const struct sockaddr*)&addrServ, sizeof(SOCKADDR_IN));
	if (SOCKET_ERROR == retval)
	{
		printf("bind failed\n");
		WSACleanup();
		return -1;
	}

	retval = listen(sServer, 10);
	if (SOCKET_ERROR == retval)
	{
		printf("listen failed\n");
		WSACleanup();
		return -1;
	}

	while (true)
	{
		SOCKET sClient;
		SOCKADDR_IN addrclient;
		int addrclientlen = sizeof(addrclient);
		sClient = accept(sServer, (sockaddr FAR*)&addrclient, &addrclientlen);
		if (INVALID_SOCKET == sServer)
		{
			printf("accept failed\n");
			closesocket(sServer);
			break;
		}
		total++;
		CreateThread(NULL, NULL, ClientThread, (LPVOID)sClient, 0, 0);
		printf("total = %d\n", total);
		printf("IP:%s\tPort%d\n", inet_ntoa(addrclient.sin_addr), ntohs(addrclient.sin_port));
		Sleep(200);
	}

	WSACleanup();

	return 0;
}