using System.Text;

namespace ConsoleApp1.Models;

public class MessageRecords
{
    public int SenderId;
    public int ReceiverId;
    public string Content;
    public string SentTime;
    public int Status;


    public MessageRecords()
    {
    }

    /// <summary>
    /// 根据字节数组反序列化 MessageRecords 对象
    /// </summary>
    public MessageRecords(byte[] bytes)
    {
        int index = 0;

        // 读取 SenderId
        SenderId = BitConverter.ToInt32(bytes, index);
        index += sizeof(int);

        // 读取 ReceiverId
        ReceiverId = BitConverter.ToInt32(bytes, index);
        index += sizeof(int);

        // 读取 Content
        int contentLength = BitConverter.ToInt32(bytes, index);
        index += sizeof(int);
        Content = Encoding.UTF8.GetString(bytes, index, contentLength);
        // 可在此对空字符串进行默认值赋值，例如：
        // Content = string.IsNullOrEmpty(Content) ? "默认内容" : Content;
        index += contentLength;

        // 读取 SentTime
        int sentTimeLength = BitConverter.ToInt32(bytes, index);
        index += sizeof(int);
        SentTime = Encoding.UTF8.GetString(bytes, index, sentTimeLength);
        // SentTime = string.IsNullOrEmpty(SentTime) ? "默认发送时间" : SentTime;
        index += sentTimeLength;

        // 读取 Status
        Status = BitConverter.ToInt32(bytes, index);
        index += sizeof(int);
    }

    /// <summary>
    /// 将 MessageRecords 对象序列化为字节数组
    /// </summary>
    public byte[] MessageGetBytes()
    {
        // 对可能为 null 的字符串字段赋空字符串保护
        string content = Content ?? "";
        string sentTime = SentTime ?? "";

        // 计算字节数组总长度：
        // SenderId 和 ReceiverId 和 Status 各占 sizeof(int)
        // 字符串字段均先存储长度信息（sizeof(int)），再存储字符串内容字节数组
        int totalLength = sizeof(int) * 3 + // SenderId, ReceiverId, Status
                          (sizeof(int) + Encoding.UTF8.GetBytes(content).Length) + // Content: 长度信息 + 内容字节
                          (sizeof(int) + Encoding.UTF8.GetBytes(sentTime).Length); // SentTime: 长度信息 + 内容字节

        byte[] bytes = new byte[totalLength];
        int index = 0;

        // 写入 SenderId
        BitConverter.GetBytes(SenderId).CopyTo(bytes, index);
        index += sizeof(int);

        // 写入 ReceiverId
        BitConverter.GetBytes(ReceiverId).CopyTo(bytes, index);
        index += sizeof(int);

        // 写入 Content
        byte[] contentBytes = Encoding.UTF8.GetBytes(content);
        int contentLength = contentBytes.Length;
        BitConverter.GetBytes(contentLength).CopyTo(bytes, index);
        index += sizeof(int);
        contentBytes.CopyTo(bytes, index);
        index += contentLength;

        // 写入 SentTime
        byte[] sentTimeBytes = Encoding.UTF8.GetBytes(sentTime);
        int sentTimeLength = sentTimeBytes.Length;
        BitConverter.GetBytes(sentTimeLength).CopyTo(bytes, index);
        index += sizeof(int);
        sentTimeBytes.CopyTo(bytes, index);
        index += sentTimeLength;

        // 写入 Status
        BitConverter.GetBytes(Status).CopyTo(bytes, index);
        index += sizeof(int);

        return bytes;
    }
}