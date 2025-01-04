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

        public Redirector(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public void Command(string[] arg)
        {
            Init init = new(configuration);
            string? dbName = null;

            switch (arg[0])
            {
                case "--sql":
                    dbName = "sqlserver";
                    break;

                case "--oracle":
                    dbName = "oracle";
                    break;

                case "--postgre":
                    dbName = "postgre";
                    break;

                case "--mysql":
                    dbName = "mysql";
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

            if (dbName != null)
            {
                if (arg.Length > 1)
                {
                    Command(arg, dbName);
                }

                else
                {
                    ServiceResponse<bool> serviceResponse = init.validateDatabase(dbName);

                    init.validationResult(serviceResponse, dbName);

                }
            }
        }

        public void Command(string[] arg, string dbName)
        {
            Init init = new(configuration);

            string? type = null;

            switch (arg[1])
            {
                case "--backup":
                    type = "backup";
                    break;

                case "--restore":
                    type = "restore";
                    break;

                case "--help":
                    Help(dbName);
                    break;

                default:
                    Help(dbName);
                    break;
            }

            if (type != null)
            {
                if (arg.Length > 2)
                {
                    Command(arg, dbName, type);
                }

                else
                {
                    init.Redirector(dbName, type);
                }
            }

        }

        public void Command(string[] arg, string dbName, string type)
        {

            if (arg[2].Equals("backup all", StringComparison.OrdinalIgnoreCase) || arg[2].Equals("restore all", StringComparison.OrdinalIgnoreCase))
            {
                SetClassMethods(dbName, type, arg[2].ToLower());
            }

            else if (arg[2].Equals("--help", StringComparison.OrdinalIgnoreCase))
            {
                Help(dbName, type);
            }

            else if (arg[2].Contains('-') || arg[2].ToLower().IsNullOrEmpty() || arg[2].Equals(" ", StringComparison.OrdinalIgnoreCase))
            {
                Help(dbName, type);
            }

            else
            {
                BeginSearch(arg, dbName, type);
            }
        }

        private void SetClassMethods(string dbName, string type, string command)
        {
            dynamic? classHolder = null;

            switch (dbName)
            {
                case "sqlserver":
                    classHolder = new MssqlService(configuration);
                    StartProcess(classHolder, type, command);
                    break;

                case "oracle":


                    break;

                case "postgre":


                    break;

                case "mysql":


                    break;
            }


        }

        private void StartProcess(dynamic classHolder, string type, string command)
        {
            if (command == "backup all")
            {
                ServiceResponse<DataTable> backupService = classHolder.DatabaseNames();

                if (backupService.isSuccess)
                {
                    classHolder.BackupOrRestore(backupService.data, type);
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
                    classHolder.BackupOrRestore(restoreService.data, type);
                }

                else
                {
                    Console.WriteLine(restoreService.message);
                }
            }
        }

        private void BeginSearch(string[] arg, string dbName, string type)
        {
            ServiceResponse<List<string>> service = new();

            switch (arg[0])
            {
                case "--sql":
                    MssqlService mssqlService = new(configuration);
                    service = mssqlService.searchDatabase(dbName);
                    break;

                case "--oracle":
                    dbName = "oracle";
                    break;

                case "--postgre":
                    dbName = "postgre";
                    break;

                case "--mysql":
                    dbName = "mysql";
                    break;
            }

            if (service.data.Count > 1)
            {

            }
        }
        //displaying "help" commands
        private void Help()
        {
            Console.WriteLine();
            Console.WriteLine("  database [option] \n");
            Console.WriteLine("  --sql : Connect to Microsoft Sql Server and make use of its methods or initiates.\n");
            Console.WriteLine("  --oracle : Connect to Oracle and make use of its methods or initiates.\n");
            Console.WriteLine("  --mysql : Connect to MySql and make use of its methods or initiates.\n");
            Console.WriteLine("  --postgre : Connect to Postgre and make use of its methods or initiates.\n");
        }

        //overloading : displaying "help" commands of selected database name
        private void Help(string dbName)
        {
            Console.WriteLine();
            Console.WriteLine($"database --{dbName} [option] \n");
            Console.WriteLine("  --backup : a list of databases will be displayed to choose from, in order to backup.\n");
            Console.WriteLine("  --restore : a list of databases will be displayed to choose from, in order to restore the file.\n");
        }

        //overloading : displaying "help" commands of selected database name and type
        private void Help(string dbName, string type)
        {
            Console.WriteLine();
            Console.WriteLine($"database --{dbName} --{type} [option] \n");
            if (type == "backup")
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
