using Microsoft.Data.Sqlite;
using SinkingFunds.Domain.Entities;
using SinkingFunds.Infrastructure.Adapters;
using SinkingFunds.Infrastructure.Persistence;
using System.IO;
using System.Threading.Tasks.Dataflow;
using Xunit;

namespace SinkingFunds.Tests.Infrastructure
{
    public class SqlLiteAdapter
    {

        [Fact]
        public void Deposit_ThenCheckOtherDb()
        {
            //Arrange
            string dbPath = "test-sinkingfunds.db";
            if (File.Exists(dbPath))
            {
                File.Delete(dbPath);
            }
            string connString = $"Data Source={dbPath}";
            Envelope fakeEnvelope = new Envelope("test-sql-envelope");
            SqliteSchemaInitializer testSqlInitializer = new SqliteSchemaInitializer(connString);
            testSqlInitializer.SchemaVerification(); 
            SqliteEnvelopeRepository fakeSqlAdapter1 = new SqliteEnvelopeRepository(connString);
            fakeEnvelope.Deposit("imaginary house fund", 250, DateTime.Today);

            //Act
            fakeSqlAdapter1.Add(fakeEnvelope);

            //Assert
            using SqliteConnection currConnection = new SqliteConnection(connString);
            currConnection.Open();
            string rowCountCheck = "SELECT COUNT(*) FROM Envelopes WHERE Id = @Id";
            using SqliteCommand commandObj = currConnection.CreateCommand();
            commandObj.Parameters.AddWithValue("@Id", fakeEnvelope.Id.ToString());
            commandObj.CommandText = rowCountCheck;
            long envelopeCount = (long)commandObj.ExecuteScalar();
            Assert.Equal(1L, envelopeCount);
        }

        [Fact]
        public void Deposit_ThenCheckTransactions()
        {
            //Arrange
            string dbPath = "test-sinkingfunds-transactions.db";
            if(File.Exists(dbPath))
            {
                File.Delete(dbPath);
            }
            string connString = $"Data Source={dbPath}";
            Envelope fakeTransactionsEnvelope = new Envelope("test-transactions-envelope");
            SqliteSchemaInitializer testTransactionsSqlInitializer = new SqliteSchemaInitializer(connString);
            testTransactionsSqlInitializer.SchemaVerification();
            SqliteEnvelopeRepository testTransactionsRepository = new SqliteEnvelopeRepository(connString);
            fakeTransactionsEnvelope.Deposit("beater car fund", 100, DateTime.Today);
            fakeTransactionsEnvelope.Deposit("clothes fund", 50, DateTime.Today);

            //Act
            testTransactionsRepository.Add(fakeTransactionsEnvelope);

            //Assert
            using SqliteConnection newConn = new SqliteConnection(connString);
            newConn.Open();
            string transactionsCountCheck = "SELECT COUNT(*) FROM Transactions WHERE EnvelopeId = @EnvelopeId";
            string transactionsFirstTransactionCountCheck = "SELECT COUNT(*) FROM Transactions WHERE EnvelopeId = @EnvelopeId AND Description = 'beater car fund' AND Amount = 100";
            string transactionsSecondTransactionCountCheck = "SELECT COUNT(*) FROM Transactions WHERE EnvelopeId = @EnvelopeId AND Description = 'clothes fund' AND Amount = 50";
            using SqliteCommand testCmd = newConn.CreateCommand();
            using SqliteCommand testFirstTransactionCmd = newConn.CreateCommand();
            using SqliteCommand testSecondTransactionCmd = newConn.CreateCommand();
            testCmd.Parameters.AddWithValue("@EnvelopeId", fakeTransactionsEnvelope.Id.ToString());
            testCmd.CommandText = transactionsCountCheck;
            long transactionCount = (long)testCmd.ExecuteScalar();
            Assert.Equal(2L, transactionCount);

            testFirstTransactionCmd.Parameters.AddWithValue("@EnvelopeId", fakeTransactionsEnvelope.Id.ToString());
            testFirstTransactionCmd.CommandText = transactionsFirstTransactionCountCheck;
            long firstTransactionCount = (long)testFirstTransactionCmd.ExecuteScalar();
            Assert.Equal(1L, firstTransactionCount);

            testSecondTransactionCmd.Parameters.AddWithValue("@EnvelopeId", fakeTransactionsEnvelope.Id.ToString());
            testSecondTransactionCmd.CommandText = transactionsSecondTransactionCountCheck;
            long secondTransactionCount = (long)testSecondTransactionCmd.ExecuteScalar();
            Assert.Equal(1L, secondTransactionCount);


        }

    }
}
