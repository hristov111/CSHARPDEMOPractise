using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Identity.Client.Kerberos;

namespace CSHARPDEMOPractise
{
    internal class Program
    {
        const string SQLConnectionstring = "Data Source=DESKTOP-BB2FCD3;Database=MinionsDb;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False";
        static void Main(string[] args)
        {
            using(var connection = new SqlConnection(SQLConnectionstring))
            {
                connection.Open();


                //var createTableStatements = InsertDataStatements();
                //foreach (var statement in createTableStatements) 
                //{
                //    ExecuteNonQuery(connection,statement);
                //}
                var query = "SELECT Name, COUNT(mv.MinionId) haha FROM Villains as v JOIN MinionsVillains as mv ON mv.VillainId = v.Id GROUP BY v.Id, v.Name ORDER BY haha DESC ;";
                using (var cmd = new SqlCommand(query, connection)) 
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Console.WriteLine($"{reader["Name"]} => {reader["haha"]}");
                        }
                    }
                }
                Console.WriteLine("--------------------------------------------------------");
                //GetMinionNamesByBillain(connection);
                AddMinion(connection);


                
            }

        }
        private static void AddMinion(SqlConnection connection)
        {
            string minionInfo = Console.ReadLine();
            string[] minionInfoArr = minionInfo.Substring(minionInfo.IndexOf(':') + 2).Split(' ');
            string villainInfo = Console.ReadLine();
            villainInfo = villainInfo.Substring(villainInfo.IndexOf(':') + 2);

            string nameExists = "SELECT Name FROM Villains WHERE Name = @Name";
            string TownExists = "SELECT Name FROM Towns WHERE NAME = @Town";
            using (var cmd = new SqlCommand(nameExists, connection))
            {
                cmd.Parameters.AddWithValue("@Name", villainInfo);
                var name = (string)cmd.ExecuteScalar();
                if (name == null)
                {
                    string query = "INSERT INTO Villains (Name,EvilnessFactorId) VALUES (@Name,4)";
                    using(var cmd2 = new SqlCommand(query, connection))
                    {
                        cmd2.Parameters.AddWithValue("@Name",villainInfo);
                        cmd2.ExecuteNonQuery();

                    }
                }
                

            }
            using (var cmd3 = new SqlCommand(TownExists, connection))
            {
                cmd3.Parameters.AddWithValue("@Town", minionInfoArr[2]);
                var townName = (string)cmd3.ExecuteScalar();
                if(townName == null)
                {
                    string query = "INSERT INTO Towns (Name,CountryCode) VALUES (@Name,2)";
                    using( var cmd4 = new SqlCommand(query, connection))
                    {
                        cmd4.Parameters.AddWithValue("@Name", minionInfoArr[2]);
                        cmd4.ExecuteNonQuery();
                    }
                }
            }
            var minionQuery = "INSERT INTO Minions (Name,Age,TownId) VALUES (@Name,@Age,@TownId)";
            var townIDQuery = "SELECT Id FROM Towns WHERE Town = @Town";
            using(var cmd5 =  new SqlCommand(minionQuery, connection))
            {
                using (var cmd6 = new SqlCommand(townIDQuery, connection))
                {
                    cmd6.Parameters.AddWithValue("@Town", minionInfoArr[2]);
                    var id = (int)cmd5.ExecuteScalar();
                    cmd5.Parameters.AddWithValue("@Name", minionInfoArr[0]);
                    cmd5.Parameters.AddWithValue("@Age", minionInfoArr[1]);
                    cmd5.Parameters.AddWithValue("@TownId", id);

                }
                
            }
            


        }
        private static void GetMinionNamesByBillain(SqlConnection connection)
        {
            int id = int.Parse(Console.ReadLine());
            string villainQuery = "SELECT Name FROM Villains WHERE id = @Id";

            using (var command = new SqlCommand(villainQuery, connection))
            {
                command.Parameters.AddWithValue("@Id", id);
                var villainName = command.ExecuteScalar();
                Console.WriteLine($"Villain-{villainName}");
                string minionquery = "SELECT m.Name,Age FROM Minions m JOIN MinionsVillains mv ON m.Id = mv.MinionId JOIN Villains v ON mv.VillainId = v.Id WHERE v.Name = @Name";
                using (var cmd = new SqlCommand(minionquery, connection))
                {
                    cmd.Parameters.AddWithValue("@Name", villainName);
                    using (var reader = cmd.ExecuteReader())
                    {
                        int count = 1;
                        var read = reader.Read();
                        if (!read)
                        {
                            Console.WriteLine("No minions");
                        }
                        while (read)
                        {
                            Console.WriteLine($"{count}:Name:{reader["Name"]}-Age:{reader["Age"]}");
                            ++count;
                            read = reader.Read();
                        }

                    }


                }

            }

        }
        private static void GetVillainName(SqlConnection connection)
        {
            string query = "SELECT Name, COUNT(mv.MinionId) FROM Villains AS v JOIN MinionsVillains AS mv ON mv.VillainId = v.Id GROUP BY v.Id,v.Name";
            using(var command = new SqlCommand(query, connection))
            {
                using(var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var name = reader[0];
                        var count = reader[1];
                        Console.WriteLine($"{name} - {count}");
                    }
                }
            }
        }
        private static void ExecuteNonQuery(SqlConnection connection ,string query)
        {
            using (var command = new SqlCommand(query, connection))
            {
                command.ExecuteNonQuery();
            }
        }
        private static string[] InsertDataStatements()
        {
            var result = new string[]
            {
                "INSERT INTO Towns (Id,Name, CountryCode) VALUES (1,'Plovdiv',1),(2, 'Oslo',2), (3, 'Kiper',2),(4, 'Larnaca',3),(5,'Athens',4 ),(6, 'London',5);",
                "INSERT INTO Minions VALUES (1, 'Stoyan',12,1), (2, 'George',22,2), (3, 'Ivan',25,3), (4, 'Kiro',35,4), (5, 'Niki',25,5);",
                "INSERT INTO EvilnessFactors VALUES (1,'super good'),(2,'good'),(3,'bad'),(4,'evil'),(5,'super evil');",
                "INSERT INTO Villains VALUES (1, 'Gru',1),(2, 'Ivo',2),(3, 'Teo',3),(4, 'Sto',4),(5, 'Pro',5);",
                "INSERT INTO MinionsVillains VALUES (1,2), (2,2), (3,3), (4,4), (5,5);"

            };
            return result;
        }
        private static string[] GetCreateTableStatements()
        {
            var result = new string[]
            {
                "CREATE TABLE Countries(Id INT PRIMARY KEY,Name VARCHAR(50))",
                "CREATE TABLE Towns(Id INT PRIMARY KEY,Name VARCHAR(50)," +
                "CountryCode INT FOREIGN KEY REFERENCES Countries(Id))",
                "CREATE TABLE Minions(Id INT PRIMARY KEY,Name VARCHAR(50)," +
                "Age INT,TownId INT FOREIGN KEY REFERENCES Towns(Id))",
                "CREATE TABLE EvilnessFactors(Id INT PRIMARY KEY,Name VARCHAR(50))",
                "CREATE TABLE Villains(Id INT PRIMARY KEY,Name VARCHAR(50)," +
                "EvilnessFactorId INT FOREIGN KEY REFERENCES EvilnessFactors(Id))",
                "CREATE TABLE MinionsVillains(MinionId INT FOREIGN KEY REFERENCES Minions(Id),VillainId INT FOREIGN KEY REFERENCES Villains(Id)CONSTRAINT PK_MinionsVillains PRIMARY KEY(MinionId, VillainId))",


            };
            return result;
        }
    }
}
