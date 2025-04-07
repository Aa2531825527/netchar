using System.Net;
using System.Net.Sockets;

namespace ConsoleApp1.Server;

public class Serversocket
{
    Socket ServerSocket;
    private Dictionary<int, ClientSocket> clientDic = new Dictionary<int, ClientSocket>();
    
    public void Start(string ip, int port, int num)
    {
        ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse(ip), port);
        try
        {
            ServerSocket.Bind(ipPoint);
            ServerSocket.Listen(num);
            //通过异步接受客户端连入
            ServerSocket.BeginAccept(AcceptCallBack, null);
        }
        catch (Exception e)
        {
            Console.WriteLine("启动服务器失败" + e.Message);
        }
    }
    
    //BeginAccept回调函数
    private void AcceptCallBack(IAsyncResult result)
    {
        try
        {
            //获取连入的客户端
            Socket clientSocket = ServerSocket.EndAccept(result);
            ClientSocket client = new ClientSocket(clientSocket);
            Console.WriteLine("客户端已连入，Id为{0}",client.clientID);
            
            //记录客户端对象
            clientDic.Add(client.clientID, client);

            //继续去让别的客户端可以连入
            ServerSocket.BeginAccept(AcceptCallBack, null);
        }
        catch (Exception e)
        {
            Console.WriteLine("客户端连入失败" + e.Message);
        }
    }
}