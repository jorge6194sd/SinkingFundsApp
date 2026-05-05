using Microsoft.Data.Sqlite;
using SinkingFunds.Application.Abstractions;
using SinkingFunds.Domain.Entities;
using SinkingFunds.Domain.Enums;
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
            using SqliteConnection currConnection = CreateConnection();
            currConnection.Open();

            string lookupEnvelope = "SELECT * FROM Envelopes WHERE Id = @Id";
            using SqliteCommand lookupEnvelopeCmd = currConnection.CreateCommand();
            lookupEnvelopeCmd.CommandText = lookupEnvelope;
            lookupEnvelopeCmd.Parameters.AddWithValue("@Id", id.ToString());
            using SqliteDataReader reader = lookupEnvelopeCmd.ExecuteReader();
            if (!reader.Read())
            {
                throw new KeyNotFoundException($"Envelope with Id '{id}' was not found.");
            }
            string name = reader.GetString(reader.GetOrdinal("Name"));
            bool isActive = reader.GetInt32(reader.GetOrdinal("IsActive")) == 1;
            Envelope returnedEnvelope = new Envelope(id, name, isActive);
            reader.Close();

            //Need to retrieve Transactions now from database and rehydrate the envelope
            string retrieveTransactionsDb = "SELECT * FROM Transactions WHERE EnvelopeId = @Id";
            using SqliteCommand retrieveTransactionsCommand = currConnection.CreateCommand();
            retrieveTransactionsCommand.CommandText = retrieveTransactionsDb;
            retrieveTransactionsCommand.Parameters.AddWithValue("@Id", id.ToString());
            using SqliteDataReader transactionReader = retrieveTransactionsCommand.ExecuteReader();

            while (transactionReader.Read())
            {

                Guid transactionId = Guid.Parse(transactionReader.GetString(transactionReader.GetOrdinal("Id")));
                string transactionDescription = transactionReader.GetString(transactionReader.GetOrdinal("Description"));
                decimal transactionAmount = Convert.ToDecimal(transactionReader.GetDouble(transactionReader.GetOrdinal("Amount")));
                TransactionType transactionType = Enum.Parse<TransactionType>(
                       transactionReader.GetString(transactionReader.GetOrdinal("Direction"))
                   );
                DateTime transactionDate = DateTime.Parse(
                        transactionReader.GetString(transactionReader.GetOrdinal("OccurredOn"))
                    );
                Transaction newTransaction = new Transaction(
                    transactionId,
                    transactionDescription,
                    transactionAmount,
                    transactionType,
                    transactionDate
                );
                returnedEnvelope.AddExistingTransaction( newTransaction );
            }


            return returnedEnvelope;

        }
        

        public void Save(Envelope envelope)
        {
            using SqliteConnection currConnection = CreateConnection();
            currConnection.Open();

            string updateEnvelopeCommand = "UPDATE Envelopes SET Name = @Name, IsActive = @IsActive WHERE Id = @Id";            
            using SqliteCommand envelopeCmd = currConnection.CreateCommand();
            envelopeCmd.CommandText = updateEnvelopeCommand;
            envelopeCmd.Parameters.AddWithValue("@Id", envelope.Id.ToString());
            envelopeCmd.Parameters.AddWithValue("@Name", envelope.Name);
            envelopeCmd.Parameters.AddWithValue("@IsActive", envelope.IsActive ? 1 : 0);
            envelopeCmd.ExecuteNonQuery();

            string deleteCurrentTransactionsCommand = "DELETE FROM Transactions WHERE EnvelopeId = @EnvelopeId";
            using SqliteCommand deleteTransactionsCmd = currConnection.CreateCommand();
            deleteTransactionsCmd.CommandText = deleteCurrentTransactionsCommand;
            deleteTransactionsCmd.Parameters.AddWithValue("@EnvelopeId", envelope.Id.ToString());
            deleteTransactionsCmd.ExecuteNonQuery();

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
