using Microsoft.Data.Sqlite;

namespace ConsoleApp1.Database;

public class DbCreation
{
    
    public static void Creation()
    {
        // 数据库文件名（自动创建）
        const string databaseFile = "UserDatabase.db";
        
        // 创建连接字符串
        string connectionString = $"Data Source={databaseFile};Foreign Keys=True;";

        // SQL建表语句
        string[] sqlStatements =
        {
            // UserInfo 表
            @"CREATE TABLE IF NOT EXISTS UserInfo (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                Phone  TEXT UNIQUE not null CHECK(length(Phone) >= 8),
                Email TEXT CHECK(Email LIKE '%@%'),
                Gender INT CHECK(Gender IN (0, 1, 2)),
                LoginPassword TEXT NOT NULL
            );",
            
            // MessageRecords 表及索引
            @"CREATE TABLE IF NOT EXISTS MessageRecords (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Senderid integer NOT NULL,
                Receiverid integer NOT NULL,
                Content TEXT NOT NULL CHECK(length(Content) <= 1000),
                SentTime TEXT NOT NULL DEFAULT (strftime('%Y-%m-%d %H:%M:%f', 'now', 'localtime')),
                Status INT NOT NULL CHECK(Status IN (0, 1, 2)), -- 0:发送中 1:成功 2:失败
                FOREIGN KEY (Senderid) REFERENCES UserInfo(id) ON DELETE CASCADE,
                FOREIGN KEY (Receiverid) REFERENCES UserInfo(id) ON DELETE CASCADE
            );",
            @"CREATE INDEX IF NOT EXISTS idx_message_sender ON MessageRecords(Senderid);",
            @"CREATE INDEX IF NOT EXISTS idx_message_receiver ON MessageRecords(Receiverid);",
            @"CREATE INDEX IF NOT EXISTS idx_message_time ON MessageRecords(SentTime);",
            
            // FriendRelations 表及索引
            @"CREATE TABLE IF NOT EXISTS FriendRelations (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Userid integer NOT NULL,
                Friendid integer NOT NULL,
                UNIQUE (Userid, Friendid),
                CHECK(Userid != Friendid),
                FOREIGN KEY (Userid) REFERENCES UserInfo(id) ON DELETE CASCADE,
                FOREIGN KEY (Friendid) REFERENCES UserInfo(id) ON DELETE CASCADE
            );",
            @"CREATE INDEX IF NOT EXISTS idx_friend_user ON FriendRelations(Userid);",
            @"CREATE INDEX IF NOT EXISTS idx_friend_pair ON FriendRelations(Userid, Friendid);"
        };

        
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            try
            {
                foreach (var sql in sqlStatements)
                {
                    using (var command = new SqliteCommand(sql, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
                Console.WriteLine("所有表创建成功！");
            }
            catch (SqliteException ex)
            {
                Console.WriteLine($"数据库创建错误：{ex.Message}");
            }
        }
    }
}
