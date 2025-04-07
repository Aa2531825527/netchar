using System.Text;

namespace ConsoleApp1.Models;

public class Userinfo
{
    public int UserId;
    public string Name;
    public string PhoneNumber;
    public string Email;
    public int Gender;
    public string Password;

    public Userinfo() { }

    public Userinfo(byte[] bytes)
    {
        int index = 0;
        
        UserId = BitConverter.ToInt32(bytes, index);
        index += 4;
        
        int namenum = BitConverter.ToInt32(bytes, index);
        index += 4;
        Name = Encoding.UTF8.GetString(bytes, index, namenum);
        index += namenum;
        
        int phonenum = BitConverter.ToInt32(bytes, index);
        index += 4;
        PhoneNumber = Encoding.UTF8.GetString(bytes, index, phonenum);
        index += phonenum;
        
        int emailnum = BitConverter.ToInt32(bytes, index);
        index += 4;
        Email = Encoding.UTF8.GetString(bytes, index, emailnum);
        index += emailnum;
        
        Gender = BitConverter.ToInt32(bytes, index);
        index += 4;
        
        int  passwordnum = BitConverter.ToInt32(bytes, index);
        index += 4;
        Password = Encoding.UTF8.GetString(bytes, index, passwordnum);
    }
    
    
    
    /// <summary>
    /// User序列化方法
    /// </summary>
    public byte[] UserGetBytes()
    {
        #region 获取数组长度
        int indexNum = sizeof(int) + Encoding.UTF8.GetBytes(Name).Length +          //name加前置int的长度
                       sizeof(int) + Encoding.UTF8.GetBytes(PhoneNumber).Length +   //PhoneNumber加前置int的长度
                       sizeof(int) + Encoding.UTF8.GetBytes(Email).Length +         //Email加前置int的长度
                       sizeof(int) * 2 +                                                //Gender+id的长度
                       sizeof(int) + Encoding.UTF8.GetBytes(Password).Length;       //Password加前置int的长度

        byte[] Bytes = new byte[indexNum];                                    //需要返回的字节数组
        int index = 0;                                                              //数组索引，表示从 playerBytes数组中的第几个位置去存储数据
        #endregion

        #region id
        BitConverter.GetBytes(UserId).CopyTo(Bytes, index);
        index += 4;
        #endregion
        
        #region Name字段
        byte[] nameBytes = Encoding.UTF8.GetBytes(Name);                            //name字节数组
        int namenum = nameBytes.Length;                                             //name数组的长度
        
        BitConverter.GetBytes(namenum).CopyTo(Bytes, index);                  //存储 Name 的长度信息
        index += sizeof(int);                                                       //数组索引向后移动一个int长度
        nameBytes.CopyTo(Bytes, index);                                       //存储name字节数组
        index += namenum;                                                           //数组索引向后移动整个name字节数组的长度
        #endregion
        
        #region Phone字段
        byte[] phoneBytes = Encoding.UTF8.GetBytes(PhoneNumber);                    //Phone字节数组
        int phonenum = phoneBytes.Length;                                           //Phone数组的长度
        
        BitConverter.GetBytes(phonenum).CopyTo(Bytes, index);                 //存储 Phone 的长度信息
        index += sizeof(int);                                                       //数组索引向后移动一个int长度
        phoneBytes.CopyTo(Bytes, index);                                      //存储Phone字节数组
        index += phonenum;                                                          //数组索引向后移动整个Phone字节数组的长度
        #endregion
        
        #region Email字段
        byte[] emailBytes = Encoding.UTF8.GetBytes(Email);                          //Email字节数组
        int emailnum = emailBytes.Length;                                           //Email数组的长度
        
        BitConverter.GetBytes(emailnum).CopyTo(Bytes, index);                 //存储 Email 的长度信息
        index += sizeof(int);                                                       //数组索引向后移动一个int长度
        emailBytes.CopyTo(Bytes, index);                                      //存储Email字节数组
        index += emailnum;                                                          //数组索引向后移动整个Email字节数组的长度
        #endregion
        
        #region Gender字段
        BitConverter.GetBytes(Gender).CopyTo(Bytes, index);                   //将性别转为字节数组
        index += sizeof(int);                                                       //向后移动一个整型长度
        #endregion
        
        #region Passwoed字段
        byte[] passwordBytes = Encoding.UTF8.GetBytes(Password);                    //password字节数组
        int passwordnum = passwordBytes.Length;                                     //password数组的长度
        
        BitConverter.GetBytes(passwordnum).CopyTo(Bytes, index);              //存储 password 的长度信息
        index += sizeof(int);                                                       //数组索引向后移动一个int长度
        passwordBytes.CopyTo(Bytes, index);                                   //存储password字节数组
        #endregion
        
        return Bytes;
    }
}