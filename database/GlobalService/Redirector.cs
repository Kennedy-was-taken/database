using Azure;
using database;
using database.Databases.MSSQL;
using database.GlobalService;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Data;

namespace database
{
    public class Redirector
    {
        private readonly IConfiguration configuration;
        private MssqlService? mssqlService;
        private readonly Init init;

        public Redirector(IConfiguration configuration)
        {
            this.configuration = configuration;
            this.init = new (configuration);
        }

        public void Command(string[] arg)
        {
            string? dbType = null;

            switch (arg[0])
            {
                case "--sql":
                    dbType = "sqlserver";
                    break;

                case "--oracle":
                    dbType = "oracle";
                    break;

                case "--postgre":
                    dbType = "postgre";
                    break;

                case "--mysql":
                    dbType = "mysql";
                    break;

                case " ":

                    init.begin();

                    break;

                case "--help":
                    Help();
                    break;

                default:
                    Help();
                    break;

            }

            if (dbType != null)
            {
                if (arg.Length >= 1)
                {
                    Command(arg, dbType);
                }

                else
                {
                    ServiceResponse<bool> serviceResponse = init.validateDatabase(dbType);

                    init.validationResult(serviceResponse, dbType);

                }
            }
        }

        public void Command(string[] arg, string dbType)
        {
            string? option = null;

            switch (arg[1])
            {
                case "--backup":
                    option = "backup";
                    break;

                case "--restore":
                    option = "restore";
                    break;

                case "--help":
                    Help(dbType);
                    break;

                default:
                    Help(dbType);
                    break;
            }

            if (option != null)
            {
                if (arg.Length >= 2)
                {
                    Command(arg, dbType, option);
                }

                else
                {
                    init.Redirector(dbType, option);
                }
            }

        }

        public void Command(string[] arg, string dbType, string option)
        {

            if (arg[2].Equals("backup all", StringComparison.OrdinalIgnoreCase) || arg[2].Equals("restore all", StringComparison.OrdinalIgnoreCase))
            {
                SetClassMethods(dbType, option, arg[2].ToLower());
            }

            else if (arg[2].Equals("--help", StringComparison.OrdinalIgnoreCase))
            {
                Help(dbType, option);
            }

            else if (arg[2].Contains('-') || arg[2].ToLower().IsNullOrEmpty() || arg[2].Equals(" ", StringComparison.OrdinalIgnoreCase))
            {
                Help(dbType, option);
            }

            else
            {
                BeginSearch(arg, dbType, option);
            }
        }

        private void SetClassMethods(string dbType, string option, string command)
        {
            dynamic? classHolder = null;

            switch (dbType)
            {
                case "sqlserver":
                    classHolder = new MssqlService(configuration);
                    StartProcess(classHolder, option, command);
                    break;

                case "oracle":


                    break;

                case "postgre":


                    break;

                case "mysql":


                    break;
            }


        }

        private void StartProcess(dynamic classHolder, string option, string command)
        {
            if (command == "backup all")
            {
                ServiceResponse<DataTable> backupService = classHolder.DatabaseNames();

                if (backupService.isSuccess)
                {
                    classHolder.BackupOrRestore(backupService.data, option);
                }

                else
                {
                    Console.WriteLine(backupService.message);
                }
            }

            else
            {
                ServiceResponse<List<string>> restoreService = classHolder.getDatabaseBackupFiles();

                if (restoreService.isSuccess)
                {
                    classHolder.BackupOrRestore(restoreService.data, option);
                }

                else
                {
                    Console.WriteLine(restoreService.message);
                }
            }
        }

        private void BeginSearch(string[] arg, string dbType, string option)
        {
            // initializing instance
            mssqlService = new MssqlService(configuration);

            // will be used to hold different types of instances
            dynamic? classHolder = null;

            ServiceResponse<List<string>> service = new();

            switch (dbType)
            {
                case "sqlserver":
                    classHolder = mssqlService;
                    service = classHolder.searchDatabase(arg[2].ToString());
                    break;

                case "oracle":
                    dbType = "oracle";
                    break;

                case "postgre":
                    dbType = "postgre";
                    break;

                case "mysql":
                    dbType = "mysql";
                    break;
            }

            if (service.isSuccess)
            {

                if (service.data?.Count == 1)
                {
                    classHolder?.BackupOrRestore(arg[2].ToString(), option);
                }

                else
                {
                    SelectDatabase(service, dbType, option);
                }
            }

            else
            {
                Console.WriteLine(service.message);
            }
        }

        private void SelectDatabase(ServiceResponse<List<string>> service, string dbType, string option)
        {
            int response = 0;
            Console.WriteLine();
            Console.WriteLine("Heres a list of known databases found : ");

            for (int i = 0; i < service.data?.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {service.data[i]}");
            }

            while (true)
            {
                try
                {
                    Console.Write($"Select you option (e.g. 1) for {dbType}: ");
                    var userInput = Console.ReadLine();

                    if (userInput != null)
                    {
                        response = int.Parse(userInput.ToString());
                    }

                    if (response <= service.data?.Count && response != 0)
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

            BeginBackupOrRestore(dbType, service.data[response - 1].ToString(), option);

        }

        public void BeginBackupOrRestore(string dbType, string dbName, string option)
        {
            switch (dbType)
            {
                case "sqlserver":
                    mssqlService?.BackupOrRestore(dbName, option);
                    break;

                case "oracle":
                    
                    break;

                case "postgre":
                    
                    break;

                case "mysql":
                    
                    break;
            }
        }

        //displaying "help" commands
        private static void Help()
        {
            Console.WriteLine();
            Console.WriteLine("  database [option] \n");
            Console.WriteLine("  --sql : Connect to Microsoft Sql Server and make use of its methods or initiates.\n");
            Console.WriteLine("  --oracle : Connect to Oracle and make use of its methods or initiates.\n");
            Console.WriteLine("  --mysql : Connect to MySql and make use of its methods or initiates.\n");
            Console.WriteLine("  --postgre : Connect to Postgre and make use of its methods or initiates.\n");
        }

        //overloading : displaying "help" commands of selected database name
        private static void Help(string dbType)
        {
            Console.WriteLine();
            Console.WriteLine($"database --{dbType} [option] \n");
            Console.WriteLine("  --backup : a list of databases will be displayed to choose from, in order to backup.\n");
            Console.WriteLine("  --restore : a list of databases will be displayed to choose from, in order to restore the file.\n");
        }

        //overloading : displaying "help" commands of selected database name and type
        private static void Help(string dbType, string option)
        {
            Console.WriteLine();
            Console.WriteLine($"database --{dbType} --{option} [option] \n");
            if (option == "backup")
            {
                Console.WriteLine("  enter \"backup all\" : The process will start to backup all the databases found on the chosen database.\n");
                Console.WriteLine("  enter database name : enter the name of the database you wish to backup, it will search for the database." +
                    "\n\t\t\t    if only one result comes back, the backup process will begin." +
                    "\n\t\t\t    if more than one result comes back, you can select one item from the list of results to backup");
            }

            else
            {
                Console.WriteLine("  enter \"restore all\" : The process will start to restore all the database .bak files found on the chosen database.\n");
                Console.WriteLine("  enter database file name : enter the name of the database you wish to restore, it will search for the database .bak file." +
                    "\n\t\t\t   if only one result comes back, the restore process will begin." +
                    "\n\t\t\t   if more than one result comes back, you can select one item from the list of results to restore");
            }

        }
    }
}
