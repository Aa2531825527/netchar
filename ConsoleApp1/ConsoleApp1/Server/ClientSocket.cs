using System.Net.Sockets;
using System.Text;
using ConsoleApp1.Database;
using ConsoleApp1.Models;
namespace ConsoleApp1.Server;
/// <summary>
/// 客户端socket管理，包含有客户端id，异步接收发送消息方法
/// </summary>
public class ClientSocket
{
    private DbAccess dbAccess=new DbAccess();
    public Socket socket;
    public int clientID;
    private static int CLIENT_BEGIN_ID = 1;
    private byte[] receiveBuffer = new byte[1024 * 4]; // 用于异步接收的临时缓冲区
    private byte[] cacheBytes = new byte[1024*1024];
    private int cacheNum = 0;
    public ClientSocket(Socket socket)
    {
        clientID = CLIENT_BEGIN_ID++;
        this.socket = socket;
        //开始收消息
        this.socket.BeginReceive(receiveBuffer, 0, receiveBuffer.Length, SocketFlags.None, ReceiveCallback, null);
    }
    private void ReceiveCallback(IAsyncResult ar)
    {
        try
        {
            int bytesRead = socket.EndReceive(ar);
            if (bytesRead > 0)
            {
                // 将接收到的数据复制到缓存区中
                Buffer.BlockCopy(receiveBuffer, 0, cacheBytes, cacheNum, bytesRead);
                cacheNum += bytesRead;
                ProcessCache();
                // 继续接收数据
                socket.BeginReceive(receiveBuffer, 0, receiveBuffer.Length, SocketFlags.None, ReceiveCallback, null);
            }
            else
            {
                // bytesRead = 0时说明连接已关闭
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();                
            }
        }
        catch (SocketException ex)
        {
            #region 异常
            switch (ex.SocketErrorCode)
            {
                case SocketError.ConnectionReset:
                    Console.WriteLine("客户端强制关闭了连接。");
                    break;
                case SocketError.Interrupted:
                    Console.WriteLine("操作被中断，Socket 被关闭。");
                    break;
                default:
                    Console.WriteLine($"未知 Socket 错误：{ex.SocketErrorCode}");
                    break;
            }
            #endregion
        }
    }
    private void ProcessCache()
    {
        int nowIndex = 0;
        while (true)
        {
            int msgID = 0;
            int msgLength = -1;
            // 检查是否有足够的字节读取头部（ID + 长度，总共8字节）
            if (cacheNum - nowIndex >= 8)
            {
                msgID = BitConverter.ToInt32(cacheBytes, nowIndex);
                nowIndex += 4;
                msgLength = BitConverter.ToInt32(cacheBytes, nowIndex);
                nowIndex += 4;
            }
            else
            {
                break; // 不足8字节，等待下次数据
            }
            // 检查是否收到了完整的消息体
            if (cacheNum - nowIndex >= msgLength)
            {
                switch (msgID)
                {
                    case 1000: //心跳消息

                        break;
                    case 1001://注册
                        try
                        {
                            // 反序列化及数据库操作
                            byte[] userData = new byte[msgLength];
                            Buffer.BlockCopy(cacheBytes, nowIndex, userData, 0, userData.Length);
                            Userinfo userInfo = new Userinfo(userData);
                            dbAccess.AddUser(userInfo.Name,userInfo.PhoneNumber,userInfo.Email,userInfo.Gender,userInfo.Password);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"数据库错误: {ex.Message}");
                            socket.Close();
                        }
                        break;
                    
                }
                nowIndex += msgLength;
            }
            else
            {
                //数据不足，回退已读取的头部信息（如果头部已经读取过）
                nowIndex -= 8;
                break;
            }
        }
        // 将剩余的数据移到缓存区的起始位置
        if (nowIndex < cacheNum)
        {
            Array.Copy(cacheBytes, nowIndex, cacheBytes, 0, cacheNum - nowIndex);
            cacheNum = cacheNum - nowIndex;
        }
        else
        {
            cacheNum = 0;
        }
    }
}