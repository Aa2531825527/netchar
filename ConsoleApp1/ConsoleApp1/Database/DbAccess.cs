using ConsoleApp1.Models;
using Microsoft.Data.Sqlite;

namespace ConsoleApp1.Database;

public class DbAccess
{
    // 数据库连接字符串
    private readonly string connectionString = "Data Source=UserDatabase.db;Foreign Keys=True;";

    /// <summary>
    /// 添加一条用户记录
    /// </summary>
    public void AddUser(string name, string phone, string email, int gender, string loginPassword)
    {
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            // 插入语句，注意 Phone 字段要求唯一，Email格式受限等
            string sql = @"INSERT INTO UserInfo (Name, Phone, Email, Gender, LoginPassword)
                               VALUES (@Name, @Phone, @Email, @Gender, @LoginPassword);";
            using (var command = new SqliteCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@Name", name);
                command.Parameters.AddWithValue("@Phone", phone);
                command.Parameters.AddWithValue("@Email", email);
                command.Parameters.AddWithValue("@Gender", gender);
                command.Parameters.AddWithValue("@LoginPassword", loginPassword);
                command.ExecuteNonQuery();
            }
        }
    }


    /// <summary>
    /// 根据用户ID删除用户记录
    /// </summary>
    public void DeleteUser(int id)
    {
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            string sql = @"DELETE FROM UserInfo WHERE Id = @Id;";
            using (var command = new SqliteCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@Id", id);
                command.ExecuteNonQuery();
            }
        }
    }




    /// <summary>
    /// 根据用户ID查询单个用户记录
    /// </summary>
    public Userinfo GetUserById(int id)
    {
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            string sql = @"SELECT Id, Name, Phone, Email, Gender, LoginPassword 
                               FROM UserInfo WHERE Id = @Id;";
            using (var command = new SqliteCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@Id", id);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        // 构造 UserInfo 对象（注意根据你定义的模型类进行调整）
                        return new Userinfo
                        {
                            UserId = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Password = reader.GetString(2),
                            Email = reader.IsDBNull(3) ? null : reader.GetString(3),
                            Gender = reader.GetInt32(4),
                            PhoneNumber = reader.GetString(5)
                        };
                    }
                }
            }
        }

        return null;
    }

    /// <summary>
    /// 查询所有用户记录
    /// </summary>
    public List<Userinfo> GetAllUsers()
    {
        var users = new List<Userinfo>();
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            string sql = @"SELECT Id, Name, Phone, Email, Gender, LoginPassword FROM UserInfo;";
            using (var command = new SqliteCommand(sql, connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var user = new Userinfo
                        {
                            UserId = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            PhoneNumber = reader.GetString(2),
                            Email = reader.IsDBNull(3) ? null : reader.GetString(3),
                            Gender = reader.GetInt32(4),
                            Password = reader.GetString(5)
                        };
                        users.Add(user);
                    }
                }
            }
        }

        return users;
    }





    /// <summary>
    /// 封装对 FriendRelations 表的增、删、查操作
    /// </summary>
    public class FriendRelationsRepository
    {
        private readonly string connectionString = "Data Source=UserDatabase.db;Foreign Keys=True;";

        /// <summary>
        /// 添加好友关系记录
        /// </summary>
        public void AddFriendRelation(int userId, int friendId)
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                string sql = @"INSERT INTO FriendRelations (Userid, Friendid)
                               VALUES (@Userid, @Friendid);";
                using (var command = new SqliteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Userid", userId);
                    command.Parameters.AddWithValue("@Friendid", friendId);
                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// 删除好友关系记录
        /// </summary>
        public void DeleteFriendRelation(int userId, int friendId)
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                string sql = @"DELETE FROM FriendRelations 
                               WHERE Userid = @Userid AND Friendid = @Friendid;";
                using (var command = new SqliteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Userid", userId);
                    command.Parameters.AddWithValue("@Friendid", friendId);
                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// 查询某个用户的所有好友ID列表
        /// </summary>
        public List<int> GetFriendsByUserId(int userId)
        {
            var friendIds = new List<int>();
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                string sql = @"SELECT Friendid FROM FriendRelations WHERE Userid = @Userid;";
                using (var command = new SqliteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Userid", userId);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            friendIds.Add(reader.GetInt32(0));
                        }
                    }
                }
            }

            return friendIds;
        }
    }

    /// <summary>
    /// 封装对 MessageRecords 表的增、查操作
    /// </summary>
    public class MessageRecordsRepository
    {
        private readonly string connectionString = "Data Source=UserDatabase.db;Foreign Keys=True;";

        /// <summary>
        /// 添加一条消息记录
        /// </summary>
        public void AddMessage(int senderId, int receiverId, string content, int status)
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                // 注意：SentTime 字段在表中有默认值，这里无需指定
                string sql = @"INSERT INTO MessageRecords (Senderid, Receiverid, Content, Status)
                               VALUES (@Senderid, @Receiverid, @Content, @Status);";
                using (var command = new SqliteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Senderid", senderId);
                    command.Parameters.AddWithValue("@Receiverid", receiverId);
                    command.Parameters.AddWithValue("@Content", content);
                    command.Parameters.AddWithValue("@Status", status);
                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// 根据发送者和接收者ID查询消息记录，按发送时间升序排列
        /// </summary>
        public List<MessageRecords> GetMessagesBySenderAndReceiver(int senderId, int receiverId)
        {
            var messages = new List<MessageRecords>();
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                string sql = @"SELECT Id, Senderid, Receiverid, Content, SentTime, Status
                               FROM MessageRecords
                               WHERE Senderid = @Senderid AND Receiverid = @Receiverid
                               ORDER BY SentTime ASC;";
                using (var command = new SqliteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Senderid", senderId);
                    command.Parameters.AddWithValue("@Receiverid", receiverId);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var message = new MessageRecords
                            {

                                SenderId = reader.GetInt32(1),
                                ReceiverId = reader.GetInt32(2),
                                Content = reader.GetString(3),
                                SentTime = reader.GetString(4),
                                Status = reader.GetInt32(5)
                            };
                            messages.Add(message);
                        }
                    }
                }
            }

            return messages;
        }

        /// <summary>
        /// 根据用户ID查询该用户参与的所有消息记录（既可能为发送者也可能为接收者），按发送时间升序排列
        /// </summary>
        public List<MessageRecords> GetMessagesByUserId(int userId)
        {
            var messages = new List<MessageRecords>();
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                string sql = @"SELECT Id, Senderid, Receiverid, Content, SentTime, Status
                               FROM MessageRecords
                               WHERE Senderid = @UserId OR Receiverid = @UserId
                               ORDER BY SentTime ASC;";
                using (var command = new SqliteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var message = new MessageRecords
                            {

                                SenderId = reader.GetInt32(1),
                                ReceiverId = reader.GetInt32(2),
                                Content = reader.GetString(3),
                                SentTime = reader.GetString(4),
                                Status = reader.GetInt32(5)
                            };
                            messages.Add(message);
                        }
                    }
                }
            }

            return messages;
        }
    }
}