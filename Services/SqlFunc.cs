using BlazorApp1.Models;
using Npgsql;
using BlazorApp1.Services;
using Microsoft.AspNetCore.Identity;
using BlazorApp1.Models;
using System;
using System.Reflection.PortableExecutable;

namespace BlazorApp1.SQLFunc
{
    
    public class sqlFunc
    {
        //IncludedData InstanceOfсonnStr = new IncludedData
        //{
        //    connectionString = ""
        //};
        public struct IncludedData
        {
            public string connectionString;
            public string returnValue;
            public int userId = 1;
            public int serverID;


            public IncludedData()
            {
                connectionString = "Host=127.0.0.1;Port=8080;Username=postgres;Password=1234;Database=postgres";
                serverID = 0;
                returnValue = "";
            } 
        }
        
        IncludedData AttrInclData = new IncludedData();


        public int GetSelectedServerID()
        {
            return 1;
        }

        
        // Функция добавления нового пользователя если тот появляется впервые
        public int AddNewUser(string username)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(AttrInclData.connectionString))
            {
                
                try
                {
                    connection.Open();

                    //string QueryStr = "insert into servers ()";
                    //Console.WriteLine(QueryStr);
                    // select EXISTS(select user_login from te_users where user_login = 'Anton') as consist
                    using (var command = new NpgsqlCommand("select EXISTS(select user_login from te_users where user_login = @username);", connection))
                    {
                        command.Parameters.AddWithValue("username", $"\"{username}\"");

                        var retVal = command.ExecuteScalar();
                        Console.WriteLine(Convert.ToString(retVal));
                        if (Convert.ToString(retVal) == "False")
                        {
                            using (var comm = new NpgsqlCommand("INSERT INTO te_users (user_login) VALUES (@n1) RETURNING key_id", connection))
                            {

                                // Here need determine args form
                                comm.Parameters.AddWithValue("n1", $"\"{username}\"");

                                var uID = comm.ExecuteScalar();
                                
                                if (uID != null)
                                {
                                    AttrInclData.userId = Convert.ToInt32(uID);
                                }
                            }
                        }else
                        {
                            using (var comm = new NpgsqlCommand("select key_id from te_users where user_login = @n1", connection))
                            {

                                // Here need determine args form
                                comm.Parameters.AddWithValue("n1", $"\"{username}\"");

                                var uID = comm.ExecuteScalar();

                                if (uID != null)
                                {
                                    AttrInclData.userId = Convert.ToInt32(uID);
                                    Console.WriteLine(AttrInclData.userId);
                                }
                            }
                        }
                        return AttrInclData.userId;
                    }

                }
                catch (Exception ex)
                {
                    // Обраюотка ошибок
                    Console.WriteLine($"Произошла ошибка: {ex.Message}");
                    Console.WriteLine("AddServers");
                }
                return AttrInclData.userId;
            }
        }




        public void LoadFromPsql(List<Server> listserver)
        {
            
            using (NpgsqlConnection connection = new NpgsqlConnection(AttrInclData.connectionString))
            {
                try
                {
                    connection.Open();

                    string QueryStr = "Select key_id, host, port, user_name, passwrd, db_name from te_servers WHERE is_deleted = false;";

                    using (var command = new NpgsqlCommand(QueryStr, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                listserver.Add(new Server
                                {
                                    //Login = "anton"/*reader.GetString(0)*/,
                                    KeyID = reader.GetInt32(0),
                                    Host = reader.GetString(1),
                                    Port = reader.GetString(2),
                                    UserName = reader.GetString(3),                                   //переписать как бд
                                    Password = reader.GetString(4),
                                    Database = reader.GetString(5)
                                });
                                
                            }
                            reader.Close();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Произошла ошибка: {ex.Message}");
                }
            }
            //LoadAction(AttrInclData.userId, 4, AttrInclData.serverID); // 4-ка пока чо отвечает за log "таблица добавлена"

        }


        public int AddDataToSql(Server newServer)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(AttrInclData.connectionString))
            {
                try
                {
                    connection.Open();

                    string QueryStr = "insert into servers ()";
                    Console.WriteLine(QueryStr);

                    using (var command = new NpgsqlCommand("INSERT INTO te_servers (host, port, user_name, passwrd, db_name) VALUES (@n2, @n3, @n4, @n5, @n6) RETURNING key_id", connection))
                    {

                        // Here need determine args form
                        //command.Parameters.AddWithValue("n1", $"\"{newServer.KeyID}\"");
                        command.Parameters.AddWithValue("n2", $"\"{newServer.Host}\"");
                        command.Parameters.AddWithValue("n3", $"\"{newServer.Port}\"");
                        command.Parameters.AddWithValue("n4", $"\"{newServer.UserName}\"");
                        command.Parameters.AddWithValue("n5", $"\"{newServer.Password}\"");
                        command.Parameters.AddWithValue("n6", $"\"{newServer.Database}\"");

                        var keyID = command.ExecuteScalar();
                        if (keyID != null)
                        {
                            AttrInclData.serverID = Convert.ToInt32(keyID);
                        }
                        
                    }
                }
                catch (Exception ex)
                {
                    // Обраюотка ошибок
                    Console.WriteLine($"Произошла ошибка: {ex.Message}");
                }
            }
            return AttrInclData.serverID;
        }

        public void UpdateServer(Server selectedServer)
        {
            try
            {
                using (var connection = new NpgsqlConnection(AttrInclData.connectionString))
                {
                    connection.Open();

                    // "UPDATE te_servers SET host = @n1, port = @n2, user_name = @n3, passwrd = @n4, db_name = @n5, WHERE key_id = @q1 ;"
                    using (var command = new NpgsqlCommand("UPDATE te_servers SET host = @n1, port = @n2, user_name = @n3, passwrd = @n4, db_name = @n5 WHERE key_id = @q1;", connection))
                    {

                        //Console.WriteLine(selectedServer.KeyID);
                        //Console.WriteLine(selectedServer.UserName);
                        //Console.WriteLine(selectedServer.Port);
                        //Console.WriteLine(selectedServer.Login);
                        //Console.WriteLine(selectedServer.Database);
                        //Console.WriteLine(selectedServer.Host);
                        command.Parameters.AddWithValue("n1", selectedServer.Host);
                        command.Parameters.AddWithValue("n2", selectedServer.Port);
                        command.Parameters.AddWithValue("n3", selectedServer.UserName);
                        command.Parameters.AddWithValue("n4", selectedServer.Password);
                        command.Parameters.AddWithValue("n5", selectedServer.Database);
                        command.Parameters.AddWithValue("q1", selectedServer.KeyID);

                        //Console.WriteLine("Проверочный тег");
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                // Обраюотка ошибок
                Console.WriteLine($"Произошла ошибка: {ex.Message}");
            }
        }


        public void DeleteServer(Server selectedServer)
        {
            using (var connection = new NpgsqlConnection(AttrInclData.connectionString))
            {
                connection.Open();
                try
                {
                    using (var command = new NpgsqlCommand("UPDATE te_servers SET is_deleted = true WHERE key_id = @n1", connection))
                    {
                        Console.WriteLine(selectedServer.KeyID);
                        command.Parameters.AddWithValue("n1", selectedServer.KeyID);

                        command.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Произошла ошибка: {ex.Message}");
                }
            }
        }

        public void WriteAction(int uID, int aID, int sID)
        {
            using (var connection = new NpgsqlConnection(AttrInclData.connectionString))
            {
                connection.Open();
                try
                {
                    using (var command = new NpgsqlCommand("INSERT INTO te_actionlogs (user_id, action_id, server_id) VALUES (@n1, @n2, @n3);", connection))
                    {
                        Console.WriteLine(uID);
                        Console.WriteLine(aID);
                        Console.WriteLine(sID);
                        command.Parameters.AddWithValue("n1", uID);
                        command.Parameters.AddWithValue("n2", aID);
                        command.Parameters.AddWithValue("n3", sID);
                        
                        

                        command.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Произошла ошибка: {ex.Message}");
                }
            }
        }

    }
}

