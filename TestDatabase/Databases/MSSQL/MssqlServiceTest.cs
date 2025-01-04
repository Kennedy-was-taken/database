using System.Data;
using database.Databases.MSSQL;
using database.GlobalService;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace TestDatabase.MSSQL
{
    [TestCaseOrderer(
    ordererTypeName: "TestBackup.PriorityOrderer",
    ordererAssemblyName: "TestBackup")]
    public class MssqlServiceTest
    {
        private IConfiguration? configuration;

        private MssqlService? mssqlService;

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

        [Fact, TestPriority(1)]
        public void TestConnection()
        {

            try
            {
                setConfiguration();

                if (configuration != null)
                {
                    mssqlService = new(configuration);
                    ServiceResponse<bool> service = mssqlService.TestConnection();

                    if (service.isSuccess)
                    {
                        Assert.True(service.isSuccess, service.message);
                    }

                    else
                    {
                        Assert.Fail(service.message);
                    }


                }

                else
                {
                    Assert.Fail("appSetting did not have connectionString");
                }

            }

            catch (Exception ex)
            {
                Assert.Fail($"{ex.Message}");
            }
        }

        [Fact, TestPriority(2)]
        public void TestGetDatabaseName()
        {
            try
            {
                setConfiguration();

                if (configuration != null)
                {
                    mssqlService = new(configuration);
                    ServiceResponse<DataTable> service = mssqlService.DatabaseNames();

                    if (service.isSuccess)
                    {
                        Assert.True(service.isSuccess, service.message);
                    }

                    else
                    {
                        Assert.Fail(service.message);
                    }

                }

                else
                {
                    Assert.Fail("appSetting did not have connectionString");
                }
            }

            catch (Exception ex)
            {
                Assert.Fail($"{ex.Message}");
            }
        }

        [Fact, TestPriority(3)]
        public void TestCheckBackupExistsTrue()
        {
            try
            {
                setConfiguration();

                if (configuration != null)
                {
                    mssqlService = new(configuration);
                    ServiceResponse<bool> service = mssqlService.CheckBackupExists("test2");

                    if (service.isSuccess)
                    {
                        Assert.True(service.isSuccess, service.message);
                    }

                    else
                    {
                        Assert.Fail(service.message);
                    }

                }

                else
                {
                    Assert.Fail("appSetting did not have connectionString");
                }
            }

            catch (Exception ex)
            {
                Assert.Fail($"{ex.Message}");
            }
        }

        [Fact, TestPriority(4)]
        public void TestCheckBackupExistsFalse()
        {
            try
            {
                setConfiguration();

                if (configuration != null)
                {
                    mssqlService = new(configuration);
                    ServiceResponse<bool> service = mssqlService.CheckBackupExists("fakedatabase");

                    if (service.isSuccess)
                    {
                        Assert.Fail(service.message);
                    }

                    else
                    {
                        Assert.False(service.isSuccess, service.message);
                    }

                }

                else
                {
                    Assert.Fail("appSetting did not have connectionString");
                }
            }

            catch (Exception ex)
            {
                Assert.Fail($"{ex.Message}");
            }
        }


        [Fact, TestPriority(5)]
        public void TestBackupDatabaseTrue()
        {
            try
            {
                setConfiguration();

                if (configuration != null)
                {
                    mssqlService = new MssqlService(configuration);
                    ServiceResponse<bool> service = mssqlService.BackupDatabase("test2", true);

                    if (service.isSuccess)
                    {
                        Assert.True(service.isSuccess, service.message);
                    }

                    else
                    {
                        Assert.Fail(service.message);
                    }

                }

                else
                {
                    Assert.Fail("appSetting did not have connectionString");
                }
            }

            catch (Exception ex)
            {
                Assert.Fail($"{ex.Message}");
            }
        }

        [Fact, TestPriority(6)]
        public void TestBackupDatabaseFalse()
        {
            try
            {
                setConfiguration();

                if (configuration != null)
                {
                    mssqlService = new MssqlService(configuration);
                    ServiceResponse<bool> service = mssqlService.BackupDatabase("testDb3", false);

                    if (service.isSuccess)
                    {
                        Assert.True(service.isSuccess, service.message);
                    }

                    else
                    {
                        Assert.Fail(service.message);
                    }

                }

                else
                {
                    Assert.Fail("appSetting did not have connectionString");
                }
            }

            catch (Exception ex)
            {
                Assert.Fail($"{ex.Message}");
            }
        }

        [Fact, TestPriority(7)]
        public void TestBackupDatabaseFail()
        {
            try
            {
                setConfiguration();

                if (configuration != null)
                {
                    mssqlService = new MssqlService(configuration);
                    ServiceResponse<bool> service = mssqlService.BackupDatabase("fakedatabasename", false);

                    if (service.isSuccess)
                    {

                        Assert.Fail(service.message);
                    }

                    else
                    {
                        Assert.False(service.isSuccess, service.message);
                    }

                }

                else
                {
                    Assert.Fail("appSetting did not have connectionString");
                }
            }

            catch (Exception ex)
            {
                Assert.Fail($"{ex.Message}");
            }
        }

        [Fact, TestPriority(8)]
        public void TestRestoreDatabaseTrue()
        {
            try
            {
                setConfiguration();

                if (configuration != null)
                {
                    mssqlService = new MssqlService(configuration);
                    ServiceResponse<bool> service = mssqlService.RestoreDatabase("testDb3");

                    if (service.isSuccess)
                    {
                        Assert.True(service.isSuccess, service.message);
                    }

                    else
                    {
                        Assert.Fail(service.message);
                    }

                }

                else
                {
                    Assert.Fail("appSetting did not have connectionString");
                }
            }

            catch (SqlException ex)
            {
                Assert.Fail($"Failed to connect. : {ex.Message}");
            }

            catch (Exception ex)
            {
                Assert.Fail($"{ex.Message}");
            }
        }

        [Fact, TestPriority(9)]
        public void TestRestoreDatabaseFalse()
        {
            try
            {
                setConfiguration();

                if (configuration != null)
                {
                    mssqlService = new MssqlService(configuration);
                    ServiceResponse<bool> service = mssqlService.RestoreDatabase("fakedatabasename");

                    if (service.isSuccess)
                    {
                        Assert.Fail(service.message);
                    }

                    else
                    {
                        Assert.False(service.isSuccess, service.message);
                    }

                }

                else
                {
                    Assert.Fail("appSetting did not have connectionString");
                }
            }

            catch (SqlException ex)
            {
                Assert.Fail($"Failed to connect. : {ex.Message}");
            }

            catch (Exception ex)
            {
                Assert.Fail($"{ex.Message}");
            }
        }


    }
}
