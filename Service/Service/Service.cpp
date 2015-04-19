#include <winsock2.h>
#include <stdio.h>
#include <windows.h>

#pragma comment (lib,"WS2_32.lib")
int total = 0;

DWORD WINAPI ClientThread(LPVOID lparam)
{
	SOCKET s = (SOCKET)lparam;
	char buf[204800];
	while (1)
	{
		Sleep(100);
		ZeroMemory(buf, 204800);
		int retval = ::recv(s, buf, 204800, 0);
		if (retval > 0)
		{
			char buffer[204800];
			int count = 0;
			for (int i = 0; i < retval; i++)
			{
				if (buf[i] != '\0')
				{
					buffer[count++] = buf[i];
				}
			}

			//buf[retval] = '\0';
			buffer[count] = '\0';
			printf("%s\n", buffer);
			::send(s, buffer, strlen(buffer), 0);
		}
		else
		{
			total--;
			break;
		}
	}

	return 0;
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