﻿using database;
using database.GlobalService;
using Microsoft.Extensions.Configuration;
using Microsoft.SqlServer.Management.Smo.Wmi;
using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace database.Databases.MSSQL
{
    public class MssqlService
    {
        //global variables
        private readonly MssqlRepository repository;

        public MssqlService(IConfiguration configuration)
        {
            this.repository = new MssqlRepository(configuration);
        }

        [ExcludeFromCodeCoverage]
        public void startProcess(string option)
        {

            switch (option)
            {
                case "backup":
                    ServiceResponse<DataTable> backupService = DatabaseNames();

                    if (backupService.isSuccess)
                    {
                        display(backupService, option);
                    }

                    else
                    {
                        Console.WriteLine(backupService.message);
                    }
                    break;

                case "restore":
                    ServiceResponse<List<string>> restoreService = getDatabaseBackupFiles();

                    if (restoreService.isSuccess)
                    {
                        display(restoreService, option);
                    }

                    else
                    {
                        Console.WriteLine(restoreService.message);
                    }

                    break;

            }
        }

        [ExcludeFromCodeCoverage]
        public void display(ServiceResponse<DataTable> dbName, string option)
        {

            int response = 0;

            DataTable? data = dbName.data;

            if (data != null)
            {

                while (true)
                {
                    try
                    {

                        int count = 0;

                        Console.WriteLine();
                        Console.WriteLine("What would you like to backup : ");

                        foreach (DataRow row in data.Rows)
                        {
                            Console.WriteLine($"{count + 1}. {row["name"].ToString()}");
                            count++;
                        }

                        Console.Write("Select you option (e.g. 1) : ");

                        var userInput = Console.ReadLine();

                        if (userInput != null)
                        {
                            response = int.Parse(userInput.ToString());

                        }

                        if (response <= data?.Rows.Count && response != 0)
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


                if (data.Rows[response - 1]["name"].ToString() == "Backup All")
                {
                    BackupOrRestore(data, option);
                }

                else
                {
                    BackupOrRestore(data.Rows[response - 1]["name"].ToString(), option);
                }

            }
        }

        [ExcludeFromCodeCoverage]
        public void display(ServiceResponse<List<string>> dbName, string option)
        {
            int response = 0;

            List<string>? data = dbName.data;

            while (true)
            {
                try
                {

                    for (int i = 0; i < data?.Count; i++)
                    {
                        Console.WriteLine($"{i + 1}. {data[i]}");
                    }

                    Console.Write("Select you option (e.g. 1) : ");

                    var userInput = Console.ReadLine();

                    if (userInput != null)
                    {
                        response = int.Parse(userInput.ToString());
                    }

                    if (response <= data?.Count && response != 0)
                    {
                        break;
                    }

                    else
                    {
                        Console.WriteLine("please enter a numerical value from the list");
                    }
                }

                catch (InvalidCastException)
                {
                    Console.WriteLine("please enter a numerical value from the list");

                }

                catch (Exception)
                {
                    Console.WriteLine("please enter a numerical value from the list");
                }
            }

            if (data[response - 1].ToString() == "Restore all")
            {

                BackupOrRestore(data, option);
            }

            else
            {
                BackupOrRestore(data[response - 1].ToString(), option);
            }
        }

        // backs up a single data
        [ExcludeFromCodeCoverage]
        public void BackupOrRestore(string dbName, string option)
        {

            Console.WriteLine("");
            switch (option)
            {
                case "backup":

                    var result = CheckBackupExists(dbName);
                    Console.WriteLine($"Attempting to Backup {dbName}");

                    var backupResult = BackupDatabase(dbName, result.data);

                    Console.WriteLine(backupResult.message);

                    break;

                case "restore":
                    Console.WriteLine($"Attempting to Restore {dbName}");

                    var restoreResult = RestoreDatabase(dbName);

                    Console.WriteLine(restoreResult.message);

                    break;

            }
        }

        //overloading method
        ///backing up multiple data
        [ExcludeFromCodeCoverage]
        public void BackupOrRestore<T>(T dbName, string option)
        {
            //inserting dbName into a dynamic type
            dynamic? obj = dbName;

            if (obj is not null)
            {
                switch (option)
                {
                    case "backup":

                        foreach (DataRow row in obj.Rows)
                        {
                            Console.WriteLine("");
                            if (row["name"].ToString() != "backup All")
                            {
                                var result = CheckBackupExists(row["name"].ToString());

                                Console.WriteLine($"Attempting to Backup {row["name"].ToString()}");

                                var restoreResult = BackupDatabase(row["name"].ToString(), result.data);

                                Console.WriteLine(restoreResult.message);
                            }

                        }

                        break;

                    case "restore":
                        for (int i = 0; i < obj.Count; i++)
                        {
                            Console.WriteLine("");
                            if (obj[i].ToString() != "restore all")
                            {
                                Console.WriteLine($"Attempting to Restore {obj[i].ToString()}");

                                var backupResult = RestoreDatabase(obj[i].ToString());

                                Console.WriteLine(backupResult.message);
                            }


                        }
                        break;

                }
            }
            
        }

        public ServiceResponse<List<string>> getDatabaseBackupFiles()
        {
            ServiceResponse<List<string>> service = new();
            var results = repository.getBackupFileName();

            if (results != null)
            {
                if (results.Count >= 2)
                {
                    results.Add("restore all");
                    service.data = results;
                }

                else
                {
                    service.data = results.ToList();
                }

                service.message = "Files found";
                service.isSuccess = true;

            }

            else
            {
                service.data = null;
                service.message = "Files not found";
                service.isSuccess = false;
            }

            return service;
        }

        [ExcludeFromCodeCoverage]
        public ServiceResponse<bool> TestConnection()
        {
            ServiceResponse<bool> service = new();

            var isConnected = repository.testConnection();

            if (isConnected)
            {
                service.data = isConnected;
                service.message = "Successfully connected to Sql Server";
                service.isSuccess = isConnected;
            }

            else
            {
                service.data = isConnected;
                service.message = "Failed to connect to Sql Server";
                service.isSuccess = isConnected;
            }

            return service;
        }

        // Gets Database names
        [ExcludeFromCodeCoverage]
        public ServiceResponse<DataTable> DatabaseNames()
        {
            ServiceResponse<DataTable> service = new();

            var dbNames = repository.getDbnames();

            if (dbNames != null)
            {
                // checks it contains more than two items
                if (dbNames.Rows.Count >= 2)
                {
                    DataRow newRow = dbNames.NewRow();
                    newRow["name"] = "backup all";
                    dbNames.Rows.Add(newRow);
                }

                service.data = dbNames;
                service.message = "Databases were found";
                service.isSuccess = true;
            }

            else
            {
                service.data = null;
                service.message = "No databases were found";
                service.isSuccess = false;
            }

            return service;
        }

        // Gets if a backup database exists
        public ServiceResponse<bool> CheckBackupExists(string dbName)
        {
            ServiceResponse<bool> service = new();
            if (dbName is not null) 
            {

                bool doesExist = repository.doesBackupExist(dbName);

                if (doesExist)
                {
                    service.data = doesExist;
                    service.message = $"Backup {dbName} database was found";
                    service.isSuccess = doesExist;
                }

                else
                {
                    service.data = doesExist;
                    service.message = $"There isn't a Backup {dbName} database";
                    service.isSuccess = doesExist;
                }
            }

            else
            {
                service.data = false;
                service.message = $"There isn't a Backup {dbName} database";
                service.isSuccess = false;
            }
            

            return service;
        }

        public ServiceResponse<bool> BackupDatabase(string dbName, bool result)
        {
            ServiceResponse<bool> service = new();

            bool results = repository.Backup(dbName, result);

            if (results)
            {
                service.data = results;
                service.message = $"{dbName} database has been backed up";
                
                service.isSuccess = results;
            }

            else
            {
                service.data = results;
                service.message = $"{dbName} database could not be backed up";
                service.isSuccess = results;
            }

            return service;
        }

        public ServiceResponse<bool> RestoreDatabase(string dbName)
        {
            ServiceResponse<bool> service = new();

            bool results = repository.restoreDatabase(dbName);

            if (results)
            {
                service.data = results;
                service.message = $"{dbName} database has been restored";
                service.isSuccess = results;
            }

            else
            {
                service.data = results;
                service.message = $"{dbName} database failed to be restored";
                service.isSuccess = results;
            }

            return service;
        }

        public ServiceResponse<List<string>> searchDatabase(string dbName)
        {
            ServiceResponse<List<string>> service = new();

            var results = repository.searchDatabase(dbName);

            if (results != null)
            {
                if (results.Count > 1)
                {

                    service.data = results;
                    service.message = $" More than one database was found with this combination : {dbName}";
                    service.isSuccess = true;
                }

                else
                {
                    service.data = results;
                    service.message = $"{dbName} database was found";
                    service.isSuccess = true;
                }
            }

            else
            {
                service.data = null;
                service.message = $"There is no database with the name {dbName}";
                service.isSuccess = false;
            }

            return service;
        }

    }

}
