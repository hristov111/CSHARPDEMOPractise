using System;
using System.Collections.Generic;
using System.Linq;
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
                var query2 = "SELECT v.Name as VillainName,STRING_AGG(m.Name, ','),STRING_AGG(m.Age, ',')  FROM Villains v JOIN MinionsVillains mv ON v.Id = mv.VillainId JOIN Minions m ON m.Id = mv.MinionId GROUP BY v.Name";
                using(var cmd = new SqlCommand(query2, connection))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Console.Write($"{reader[0]} =>");
                            var secondCol = ((string)reader[1]).Split(',');
                            var AgeCol = ((string)reader[2]).Split(',');
                            for (int i = 0; i < secondCol.Length; ++i)
                            {
                                Console.Write($"{AgeCol[i]}-{secondCol[i]},");
                            }
                            Console.WriteLine();
                        }
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
