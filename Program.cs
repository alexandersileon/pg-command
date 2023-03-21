using Npgsql;
using System.Text;

namespace pg.command
{
    public class Program
    {
        private static string _connectionString = "";

        public static void Main(string[] args)
        {
            _connectionString = args.Length > 0 && !string.IsNullOrWhiteSpace(args[0]) ?
                args[0] :
                Environment.GetEnvironmentVariable("ConnectionString_DefaultConnection") ?? "";

            var sqlCommand = args.Length > 1 ? args[1] : null;

            if (sqlCommand == null)
            {
                Console.WriteLine($"No command found. Exiting.");
            }
            else
            {
                Console.WriteLine($"Found connectionstring: {_connectionString}");
                Console.WriteLine($"Command to execute: {sqlCommand}");
                Console.WriteLine("Continue? (Y,N)");
                var continueResult = Console.ReadLine();

                if (continueResult?.ToLower() == "y")
                {
                    Console.WriteLine("Connecting to database.");
                    using (var con = new NpgsqlConnection(_connectionString))
                    {
                        Console.WriteLine("Connecting open.");
                        con.Open();

                        using (var cmd = new NpgsqlCommand(sqlCommand, con))
                        {

                            if (sqlCommand.Contains("UPDATE", StringComparison.InvariantCultureIgnoreCase) ||
                                sqlCommand.Contains("INSERT", StringComparison.InvariantCultureIgnoreCase))
                            {
                                Console.Write("sqlCommand contains UPDATE or INSERT");
                                Console.Write("ExecuteNonQuery");
                                Console.Write("");
                                Console.WriteLine(cmd.ExecuteNonQuery());
                                Console.Write("");
                                Console.WriteLine("End ExecuteNonQuery");
                                Console.WriteLine("All done.");
                            }
                            else
                            {
                                Console.Write("sqlCommand SELECT");
                                Console.Write("ExecuteReader");
                                Console.Write("");
                                var rdr = cmd.ExecuteReader();

                                while (rdr.Read())
                                {
                                    var sb = new StringBuilder();

                                    for (int i = 0; i < rdr.FieldCount; i++)
                                    {
                                        sb.Append(rdr.GetValue(i).ToString());
                                        if((i + 1) < rdr.FieldCount)
                                            sb.Append("[]");
                                    }

                                    Console.WriteLine(sb.ToString());

                                    sb.Clear();
                                }
                                Console.Write("");
                                Console.WriteLine("All done..");
                            }
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Exiting.");
                }
            }
        }
    }
}