using Microsoft.Data.Sqlite;
using SQLitePCL;
using System;
using System.Collections.Generic;
using System.Text;

namespace SinkingFunds.Infrastructure.Persistence
{
    public class SqliteSchemaInitializer
    {
        private readonly string _connectionString;
        public SqliteSchemaInitializer(string connectionString) 
        {
            _connectionString = connectionString;
        }

        private SqliteConnection CreateConnection()
        {
            SqliteConnection freshConnection = new SqliteConnection(_connectionString);
            return freshConnection;
        }

        private void EnsureEnvelopesTable(SqliteConnection currentConnection)
        {
            string createCommand = "CREATE TABLE IF NOT EXISTS Envelopes (Id TEXT PRIMARY KEY, Name TEXT NOT NULL, IsActive INTEGER NOT NULL)";
            SqliteCommand commandObj = currentConnection.CreateCommand();
            commandObj.CommandText = createCommand;
            commandObj.ExecuteNonQuery();
        }

        private void EnsureTransactionsTable(SqliteConnection currentConnection)
        {
            string createCommand = "CREATE TABLE IF NOT EXISTS Transactions (Id TEXT PRIMARY KEY, Description TEXT NOT NULL, Amount REAL NOT NULL, Direction TEXT NOT NULL, OccurredOn TEXT NOT NULL, EnvelopeId TEXT NOT NULL)";
            SqliteCommand commandObj = currentConnection.CreateCommand();
            commandObj.CommandText = createCommand;
            commandObj.ExecuteNonQuery();
        }

        private void EnsureRecurringRuleTable(SqliteConnection currentConnection)
        {
            throw new NotImplementedException();
        }
        public void SchemaVerification()
        {
            // initialize schema
            SqliteConnection currentConnection = CreateConnection();
            currentConnection.Open();

            // ensure schema
            EnsureEnvelopesTable(currentConnection);
            EnsureTransactionsTable(currentConnection);

            // create tables if missing
        }
    }
}
