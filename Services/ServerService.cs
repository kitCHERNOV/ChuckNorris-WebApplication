using System.Collections.Generic;
using System.Text.Json;
using BlazorApp1.Models;
using BlazorApp1.SQLFunc;

namespace BlazorApp1.Services
{
    //public static class LocalVaiables
    //{
    //    public static int usID { get; set; }
    //}

    
    
    public class ServerService
    {
        private List<Server> _servers;
        private Server _selectedServer;
        private Server _newServer;
        private static int uID; // id пользователя
        private int sID = 0; // id сервера; 0 - по умолчанию
        private int aID;
        
        // Список действий
        //"server has been added"    = 1
        //"server has been changed"  = 2
        //"server has been deleted"  = 3
        //"user is logged in"        = 4
        public ServerService()
        {

        }
        public ServerService(List<Server> servers, Server newServer)
        {
            _servers = servers;
            _newServer = new Server();
            LoadServersFromFile();
        }
        
        sqlFunc sqlFunc = new sqlFunc();

        public void CheckUsers(string username)
        {
            // returning func is located here
            uID = sqlFunc.AddNewUser(username);
            aID = 4; // 
            sqlFunc.WriteAction(uID,aID,sID);

            Console.WriteLine($"uuuID : {uID}");
            //sqlFunc.Test();
        }

        private void LoadServersFromFile()
        {
            //if (File.Exists("servers.txt"))
            //{
            //    var serversJson = File.ReadAllText("servers.txt");
            //    _servers.AddRange(JsonSerializer.Deserialize<List<Server>>(serversJson));
            //}
            sqlFunc.LoadFromPsql(_servers);
            
        }
        // "Host=127.0.0.1;Port=8080;Username=postgres;Password=1234;Database=postgres"
        public void CheckConnected(Server server) // Функция вызова проверки подключения
        {
            var ConnPath = string.Format(
                "Host=127.0.0.1;Port={0};Username={1};Password={2};Database={3}",
                server.Port,
                server.UserName,
                server.Password,
                server.Database
            );
            Console.WriteLine(ConnPath); // Check correct compose connection string
            sqlFunc.CheckConnection(ConnPath);
        }

        private void SaveServersToFile()
        {
            var serversJson = JsonSerializer.Serialize(_servers);
            File.WriteAllText("servers.txt", serversJson);
        }

        public void SetSelectedServer(Server server)
        {
            _selectedServer = server;
        }

        public Server GetSelectedServer()
        {
            return _selectedServer;
        }

        public Server GetNewServer()
        {
            return _newServer;
        }

        public List<Server> GetServers()
        {
            return _servers;
        }

        public void AddServer(Server server)
        {
            _servers.Add(server);
            sID = sqlFunc.AddDataToSql(server);
            //uID = sqlFunc.GetSelectedServerID(server);

            _newServer = new Server();
            //SaveServersToFile();
            
            aID = 1;
            Console.WriteLine($"{uID} : uuuu");
            sqlFunc.WriteAction(uID, aID, sID); // Correct
        }

        public void UpdateServer(Server server)
        {
            

            // Работа с данными бд;
            sqlFunc.UpdateServer(server);
            aID = 2;

            sqlFunc.WriteAction(uID, aID, server.KeyID);

        }

        public void DeleteServer(Server server)
        {
            //LogAction($"Сервер '{_selectedServer.UserName}' удален");
            //SaveServersToFile();

            _servers.Remove(server);
            sqlFunc.DeleteServer(server);
            aID = 3;
            sqlFunc.WriteAction(uID, aID, server.KeyID);

        }

        

        public void SaveServers(List<Server> serverList)
        {
            SaveServersToFile();
        }

        
    }
}
