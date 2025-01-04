﻿using Microsoft.Extensions.Configuration;
using Xunit;
using System.IO;
using database.Databases.MSSQL;
using System.Data;

namespace TestBackup
{
    public class MssqlTest
    {
        private IConfiguration? configuration;

        MssqlRepository? repository;

        private void setConfiguration()
        {
            var builder = new ConfigurationBuilder();

            //sets the path
            var jsonpath = "C:\\database\\database";

            //reads from the appsetting.json
            builder.SetBasePath(jsonpath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();

            configuration = builder.Build();

        }

        [Fact]
        public void testConnection()
        {
            setConfiguration();

            if (configuration != null)
            {
                repository = new MssqlRepository(configuration);
            }

            else
            {
                throw new Exception("configuration variable was nulll");
            }

            bool isConnected = repository.testConnection();
            Assert.True(isConnected);
        }

        [Fact]
        public void retrieveDbName()
        {
            setConfiguration();

            if (configuration != null)
            {
                repository = new MssqlRepository(configuration);
            }

            else
            {
                throw new Exception("configuration variable was nulll");
            }

            DataTable dt = new();

            dt = repository.getDbnames();

            if (dt == null)
            {
                Assert.True(false);
            }

            else
            {
                foreach (DataRow dr in dt.Rows)
                {
                    Console.WriteLine(dr.ToString());
                }
                Assert.True(true);
            }
        }

        [Fact]
        public void testBackup()
        {
            setConfiguration();

            if (configuration != null)
            {
                repository = new MssqlRepository(configuration);
            }

            else
            {
                throw new Exception("configuration variable was nulll");
            }

            DataTable dt = new();

            dt = repository.getDbnames();

            var dbName = dt.Rows[0]["name"].ToString();

            if (dbName != null)
            {

                bool doesExist = repository.doesBackupExist(dbName);

                if (repository.Backup(dbName, doesExist))
                {
                    Assert.True(true);
                }

                else
                {
                    Assert.True(false);
                }

            }

            else
            {
                Assert.True(false);
                throw new Exception("DataTable was null");

            }

        }

        [Fact]
        public void testRestore()
        {
            setConfiguration();

            if (configuration != null)
            {
                repository = new MssqlRepository(configuration);
            }

            else
            {
                throw new Exception("configuration variable was nulll");
            }

            DataTable dt = new();

            dt = repository.getDbnames();

            var dbName = dt.Rows[0]["name"].ToString();

            if (dbName != null)
            {
                bool isRestored = repository.restoreDatabase(dbName);
                Assert.True(isRestored);
            }

            else
            {
                Assert.True(false);
            }
        }

        [Fact]
        public void testGetFileNames()
        {
            setConfiguration();

            if (configuration != null)
            {
                repository = new MssqlRepository(configuration);
            }

            else
            {
                throw new Exception("configuration variable was nulll");
            }

            List<string>? test = repository.getBackupFileName();

            if (test != null)
            {
                Assert.True(true, "file names found");
            }

            else
            {
                Assert.Fail("no fles came back");
            }
        }
    }
}
