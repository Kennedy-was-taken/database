using database.Databases.MSSQL;
using database.GlobalService;
using Microsoft.Extensions.Configuration;
using System.Diagnostics.CodeAnalysis;

namespace database
{
    public class Init
    {

        private List<string>? dbNames;

        private readonly IConfiguration configuration;

        public Init(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        [ExcludeFromCodeCoverage]
        public void begin()
        {
            if (PopulateDatabaseList())
            {
                DisplayDatabases();
            }

            else
            {
                Console.WriteLine("No ConnectionString was found on the appsetting.json");
                Console.WriteLine("Replace this appsetting.json with the repository you downloaded it from");
            }
            
        }

        public bool PopulateDatabaseList()
        {
            dbNames = new List<string>();
            var Databases = configuration.GetSection("ConnectionStrings");

            if (Databases is null)
            {
                return false;
            }

            foreach (var item in Databases.GetChildren())
            {
                dbNames.Add(item.Key);
            }

            return true;
        }

        [ExcludeFromCodeCoverage]
        private void DisplayDatabases()
        {
            int response = 0;

            if (dbNames == null)
            {
                Console.WriteLine("No Databases found : ");
            }

            else
            {

                Console.WriteLine("Current Databases Available : ");
                for (int i = 0; i < dbNames?.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {dbNames[i]}");
                }

                while (true)
                {
                    try
                    {
                        Console.Write("Select you option (e.g. 1) : ");

                        var userInput = Console.ReadLine();

                        if (userInput != null)
                        {
                            response = int.Parse(userInput.ToString());

                        }

                        if (response <= dbNames?.Count && response != 0)
                        {
                            break;
                        }

                        else
                        {
                            Console.WriteLine("please enter a numerical value from the list");
                        }
                    }

                    catch (Exception)
                    {
                        Console.WriteLine("please enter a numerical value from the list");
                    }
                }

                ServiceResponse<bool> serviceResponse = new ServiceResponse<bool>();

                switch (dbNames[response - 1])
                {
                    case "mysql":

                        break;

                    case "postgre":

                        break;

                    case "sqlserver":

                        serviceResponse = validateDatabase("sqlserver");

                        break;
                }

                validationResult(serviceResponse, dbNames[response - 1]);

            }
        }

        public void validationResult(ServiceResponse<bool> serviceResponse, string dbName)
        {
            Console.WriteLine("");

            if (serviceResponse.isSuccess)
            {
                Console.WriteLine(serviceResponse.message);
                DatabaseOption(dbName);

            }

            else
            {
                Console.WriteLine(serviceResponse.message);
            }
        }

        public ServiceResponse<bool> validateDatabase(string dbName)
        {
            ServiceResponse<bool> serviceResponse = new ServiceResponse<bool>();

            switch (dbName)
            {
                case "sqlserver":
                    serviceResponse = Validate.isSqlServerInstalled();
                    break;

                case "oracle":

                    break;

                case "postgre":

                    break;

                case "mysql":

                    break;
            }

            return serviceResponse;
        }

        [ExcludeFromCodeCoverage]
        public void DatabaseOption(string dbName)
        {
            int response = 0;
            while (true)
            {
                try
                {
                    Console.WriteLine($"Would you like to backup or restore : ");
                    Console.WriteLine("1. Backup Database");
                    Console.WriteLine("2. Restore Database");
                    Console.Write($"Select you option (e.g. 1) for {dbName}: ");

                    var userInput = Console.ReadLine();

                    if (userInput != null)
                    {
                        response = int.Parse(userInput.ToString());
                    }

                    if (response <= dbNames?.Count && response != 0)
                    {
                        break;
                    }

                    else
                    {
                        Console.WriteLine("please enter a numerical value from the list");
                    }
                }

                catch (Exception)
                {
                    Console.WriteLine("please enter a numerical value from the list");
                }
            }

            if (response == 1)
            {
                Redirector(dbName, "backup");
            }

            else
            {
                Redirector(dbName, "restore");
            }

        }

        [ExcludeFromCodeCoverage]
        public void Redirector(string dbName, string option)
        {
            switch (dbName)
            {
                case "mysql":

                    break;

                case "postgre":

                    break;

                case "sqlserver":

                    beginMssqlProcess(dbName, option);

                    break;

                default:
                    Console.WriteLine("No database with that name exists");
                    break;
            }
        }

        public void beginMssqlProcess(string dbName, string option)
        {
            MssqlService mssqlservice = new(configuration);

            Console.WriteLine("");
            Console.WriteLine($"Testing Connnection to Sql Server");
            var isConnected = mssqlservice.TestConnection();

            if (isConnected.isSuccess)
            {
                Console.WriteLine(isConnected.message);
                mssqlservice.startProcess(option);
            }

            else
            {
                Console.WriteLine(isConnected.message);
            }
        }

    }
}
