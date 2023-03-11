using DemoSqlClient;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using System.Data;

namespace IntegrationTest
{
    public class SqlIntegrationTest
    {
#pragma warning disable MEN002 // Line is too long
        private const string ConnectionStringToTestDB = "Server=tcp:localhost;Authentication=Sql Password;Database=MySchool;User=sa;Password=Admin123;TrustServerCertificate=true";
#pragma warning restore MEN002 // Line is too long

        [Fact]
        public void ReadViaTextCommand()
        {
            // Act
            var minHitParam = new SqlParameter("@MinCredits", SqlDbType.Int, 100);
            minHitParam.Value = 3;
            SqlDataReader sqlDataReader = DemoSqlUtility.ExecuteReader(
                ConnectionStringToTestDB,
                @"SELECT TOP (1000) [CourseID],[Title]
                  FROM [MySchool].[dbo].[Course]
                  WHERE [Credits] > @MinCredits",
                CommandType.Text,
                minHitParam);

            var result = new List<string>();
            using (SqlDataReader reader = sqlDataReader)
            {
                while (reader.Read())
                {
                    result.Add(reader.GetString(0) + "," + reader.GetString(1));
                }
            }

            // Assert
            result.Should().HaveCount(3);
            result.Should().BeEquivalentTo(new[]
                {
                "C1045,Calculus",
                "C1061,Physics",
                "C2042,Literature"
            });
        }

        [Fact]
        public void ReadViaStoredProcedure()
        {
            // Act
            var courseIdParam = new SqlParameter("@CourseId", SqlDbType.NVarChar, 10);
            courseIdParam.Value = "C1045";

            SqlDataReader sqlDataReader = DemoSqlUtility.ExecuteReader(
                ConnectionStringToTestDB,
                "[dbo].[CourseExtInfo]",
                CommandType.StoredProcedure,
                courseIdParam);

            var result = new List<string>();
            using (SqlDataReader reader = sqlDataReader)
            {
                while (reader.Read())
                {
                    result.Add(reader.GetString(0) + "," + reader.GetString(1) + "," + reader.GetInt32(2) + "," + reader.GetString(3));
                }
            }

            // Assert
            result.Should().HaveCount(1);
            result.Should().BeEquivalentTo(new[] { "C1045,Calculus,4,Mathematics" });
        }

        [Fact]
        public void InsertData()
        {
            // Act
            var courseIdParam = new SqlParameter("@CourseId", SqlDbType.NVarChar, 10);
            courseIdParam.Value = "C1045";

            var affectedRows = DemoSqlUtility.ExecuteNonQuery(
                ConnectionStringToTestDB,
                @"INSERT INTO [MySchool].[dbo].[Person] ([LastName]
                          ,[FirstName]
                          ,[HireDate]
                          ,[EnrollmentDate]) VALUES('Li', 'Si', '2023-01-01 00:00:00.000', '2023-01-01 00:00:00.000')",
                CommandType.Text,
                courseIdParam);

            // Assert
            affectedRows.Should().Be(1);
        }
    }
}