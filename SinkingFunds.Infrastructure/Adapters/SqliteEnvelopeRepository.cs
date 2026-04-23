using Microsoft.Data.Sqlite;
using SinkingFunds.Application.Abstractions;
using SinkingFunds.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace SinkingFunds.Infrastructure.Adapters
{
    public class SqliteEnvelopeRepository : IEnvelopeRepository
    {
        private readonly string _connectionString;

        public SqliteEnvelopeRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        private SqliteConnection CreateConnection() 
        {
            SqliteConnection newConnection = new SqliteConnection(_connectionString);
            return newConnection;
        }

        public Envelope GetById(Guid id)
        {
            throw new NotImplementedException();
        }

        public void Add(Envelope envelope)
        {
            using SqliteConnection currConnection = CreateConnection();
            currConnection.Open();

            string insertCommand = "INSERT INTO Envelopes (Id, Name, IsActive) VALUES (@Id, @Name, @IsActive)";

            using SqliteCommand commandObj = currConnection.CreateCommand();
            commandObj.CommandText = insertCommand;
            commandObj.Parameters.AddWithValue("@Id", envelope.Id.ToString());
            commandObj.Parameters.AddWithValue("@Name", envelope.Name);
            commandObj.Parameters.AddWithValue("@IsActive", envelope.IsActive ? 1 : 0);
            commandObj.ExecuteNonQuery();

            foreach (var transaction in envelope.GetTransactions())
            {
                string insertTransactions = "INSERT INTO Transactions (Id, Description, Amount, Direction, OccurredOn, EnvelopeId) VALUES (@Id, @Description, @Amount, @Direction, @OccurredOn, @EnvelopeId)";
                using SqliteCommand transactionInserts = currConnection.CreateCommand();
                transactionInserts.CommandText = insertTransactions;
                transactionInserts.Parameters.AddWithValue("@Id", transaction.Id.ToString());
                transactionInserts.Parameters.AddWithValue("@Description", transaction.Description);
                transactionInserts.Parameters.AddWithValue("@Amount", transaction.Amount);
                transactionInserts.Parameters.AddWithValue("@Direction", transaction.Direction.ToString());
                transactionInserts.Parameters.AddWithValue("@OccurredOn", transaction.OccurredOn.ToString());
                transactionInserts.Parameters.AddWithValue("@EnvelopeId", envelope.Id.ToString());
                transactionInserts.ExecuteNonQuery();
            }

          
        }

    }
}
