using ConsoleApp1.Database;
using ConsoleApp1.Server;

namespace ConsoleApp1;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");
        DbCreation.Creation();
        Serversocket server = new Serversocket();
        server.Start("127.0.0.1",8080 ,12000);
    }
}