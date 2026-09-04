using Microsoft.Data.Sqlite;
using SinkingFunds.Application.Abstractions;
using SinkingFunds.Domain.Entities;
using SinkingFunds.Domain.Enums;
using System;
using System.Collections.Generic;

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

            // Lookup the target envelope based off its ID
            string lookupEnvelope = "SELECT * FROM Envelopes WHERE Id = @Id";

            using SqliteCommand lookupEnvelopeCmd = currConnection.CreateCommand();
            lookupEnvelopeCmd.CommandText = lookupEnvelope;
            lookupEnvelopeCmd.Parameters.AddWithValue("@Id", id.ToString());

            using SqliteDataReader reader = lookupEnvelopeCmd.ExecuteReader();

            if (!reader.Read())
            {
                throw new KeyNotFoundException(
                    $"Envelope with Id '{id}' was not found."
                );
            }

            // Read core Envelope fields from DB
            string name =
                reader.GetString(reader.GetOrdinal("Name"));

            bool isActive =
                reader.GetInt32(reader.GetOrdinal("IsActive")) == 1;

            int targetAmountOrdinal =
                reader.GetOrdinal("TargetAmount");

            decimal? targetAmount =
                reader.IsDBNull(targetAmountOrdinal)
                    ? null
                    : Convert.ToDecimal(
                        reader.GetDouble(targetAmountOrdinal)
                    );

            int ruleAmountOrdinal =
                reader.GetOrdinal("RuleAmount");

            decimal? ruleAmount =
                reader.IsDBNull(ruleAmountOrdinal)
                    ? null
                    : Convert.ToDecimal(
                        reader.GetDouble(ruleAmountOrdinal)
                    );

            // Rehydrate the Envelope
            Envelope returnedEnvelope =
                new Envelope(
                    id,
                    name,
                    isActive,
                    targetAmount
                );

            // MVP assumption:
            // Any stored RuleAmount represents a monthly contribution.
            if (ruleAmount.HasValue)
            {
                RecurringRule recurringRule =
                    new RecurringRule(
                        frequency: 1,
                        unit: FrequencyUnits.Months,
                        ruleAmount: ruleAmount.Value,
                        nextDueDate: DateTime.Today.AddMonths(1)
                    );

                returnedEnvelope.SetRecurringRule(recurringRule);
            }

            reader.Close();

            // Retrieve Transactions and rehydrate the Envelope's transaction collection
            string retrieveTransactionsDb =
                "SELECT * FROM Transactions WHERE EnvelopeId = @Id";

            using SqliteCommand retrieveTransactionsCommand =
                currConnection.CreateCommand();

            retrieveTransactionsCommand.CommandText =
                retrieveTransactionsDb;

            retrieveTransactionsCommand.Parameters.AddWithValue(
                "@Id",
                id.ToString()
            );

            using SqliteDataReader transactionReader =
                retrieveTransactionsCommand.ExecuteReader();

            while (transactionReader.Read())
            {
                Guid transactionId =
                    Guid.Parse(
                        transactionReader.GetString(
                            transactionReader.GetOrdinal("Id")
                        )
                    );

                string transactionDescription =
                    transactionReader.GetString(
                        transactionReader.GetOrdinal("Description")
                    );

                decimal transactionAmount =
                    Convert.ToDecimal(
                        transactionReader.GetDouble(
                            transactionReader.GetOrdinal("Amount")
                        )
                    );

                TransactionType transactionType =
                    Enum.Parse<TransactionType>(
                        transactionReader.GetString(
                            transactionReader.GetOrdinal("Direction")
                        )
                    );

                DateTime transactionDate =
                    DateTime.Parse(
                        transactionReader.GetString(
                            transactionReader.GetOrdinal("OccurredOn")
                        )
                    );

                Transaction newTransaction =
                    new Transaction(
                        transactionId,
                        transactionDescription,
                        transactionAmount,
                        transactionType,
                        transactionDate
                    );

                returnedEnvelope.AddExistingTransaction(
                    newTransaction
                );
            }

            return returnedEnvelope;
        }


        public void Save(Envelope envelope)
        {
            using SqliteConnection currConnection = CreateConnection();
            currConnection.Open();

            //Update the ENVELOPES core db fields to match in memory fields
            string updateEnvelopeCommand = "UPDATE Envelopes SET Name = @Name, IsActive = @IsActive, TargetAmount = @TargetAmount, RuleAmount = @RuleAmount WHERE Id = @Id";            

            using SqliteCommand envelopeCmd = currConnection.CreateCommand();
            envelopeCmd.CommandText = updateEnvelopeCommand;
            envelopeCmd.Parameters.AddWithValue("@Id", envelope.Id.ToString());
            envelopeCmd.Parameters.AddWithValue("@Name", envelope.Name);
            envelopeCmd.Parameters.AddWithValue("@IsActive", envelope.IsActive ? 1 : 0);
            envelopeCmd.Parameters.AddWithValue(
     "@TargetAmount",
     envelope.TargetAmount.HasValue
         ? envelope.TargetAmount.Value
         : DBNull.Value
 );
            var recurringRule = envelope.GetRecurringRule();

            envelopeCmd.Parameters.AddWithValue(
                "@RuleAmount",
                recurringRule != null
                    ? recurringRule.RuleAmount
                    : DBNull.Value
            );
            envelopeCmd.ExecuteNonQuery();

            //delete db's version of envelopes TRANSACTIONS
            string deleteCurrentTransactionsCommand = "DELETE FROM Transactions WHERE EnvelopeId = @EnvelopeId";
            using SqliteCommand deleteTransactionsCmd = currConnection.CreateCommand();
            deleteTransactionsCmd.CommandText = deleteCurrentTransactionsCommand;
            deleteTransactionsCmd.Parameters.AddWithValue("@EnvelopeId", envelope.Id.ToString());
            deleteTransactionsCmd.ExecuteNonQuery();

            // Re-persist the envelope's transactions to match the in-memory collection
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

            // Create a new DB entry with this envelope's persisted fields
            string insertCommand =
                "INSERT INTO Envelopes " +
                "(Id, Name, IsActive, TargetAmount, RuleAmount) " +
                "VALUES (@Id, @Name, @IsActive, @TargetAmount, @RuleAmount)";

            using SqliteCommand commandObj = currConnection.CreateCommand();
            commandObj.CommandText = insertCommand;

            commandObj.Parameters.AddWithValue("@Id", envelope.Id.ToString());
            commandObj.Parameters.AddWithValue("@Name", envelope.Name);
            commandObj.Parameters.AddWithValue("@IsActive", envelope.IsActive ? 1 : 0);

            commandObj.Parameters.AddWithValue(
                "@TargetAmount",
                envelope.TargetAmount.HasValue
                    ? envelope.TargetAmount.Value
                    : DBNull.Value
            );

            RecurringRule? recurringRule = envelope.GetRecurringRule();

            commandObj.Parameters.AddWithValue(
                "@RuleAmount",
                recurringRule != null
                    ? recurringRule.RuleAmount
                    : DBNull.Value
            );

            commandObj.ExecuteNonQuery();

            // Insert the envelope's transactions based on the in-memory version
            foreach (var transaction in envelope.GetTransactions())
            {
                string insertTransactions =
                    "INSERT INTO Transactions " +
                    "(Id, Description, Amount, Direction, OccurredOn, EnvelopeId) " +
                    "VALUES (@Id, @Description, @Amount, @Direction, @OccurredOn, @EnvelopeId)";

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

        public IEnumerable<Envelope> GetAll()
        {
            List<Guid> envelopeIds = new List<Guid>();

            using (SqliteConnection currConnection = CreateConnection())
            {
                currConnection.Open();

                string getAllCommand = "SELECT Id FROM Envelopes";
                using SqliteCommand commandObj = currConnection.CreateCommand();
                commandObj.CommandText = getAllCommand;

                using SqliteDataReader envelopesReader = commandObj.ExecuteReader();

                while (envelopesReader.Read())
                {
                    Guid envelopeId = Guid.Parse(
                        envelopesReader.GetString(envelopesReader.GetOrdinal("Id"))
                    );

                    envelopeIds.Add(envelopeId);
                }
            }

            List<Envelope> envelopesList = new List<Envelope>();

            foreach (Guid envelopeId in envelopeIds)
            {
                Envelope fullEnvelope = GetById(envelopeId);
                envelopesList.Add(fullEnvelope);
            }

            return envelopesList;
        }

        public void Delete(Guid envelopeId)
        {
            //throw new NotImplementedException();
            using SqliteConnection currConnection = CreateConnection();
            currConnection.Open();

            //Delete Transactions
            string deleteTransactionsCommand = "DELETE FROM Transactions WHERE EnvelopeId = @EnvelopeId";
            using SqliteCommand deleteTransactionsCommandObj = currConnection.CreateCommand();
            deleteTransactionsCommandObj.CommandText = deleteTransactionsCommand;
            deleteTransactionsCommandObj.Parameters.AddWithValue("@EnvelopeId", envelopeId.ToString());
            deleteTransactionsCommandObj.ExecuteNonQuery();

            //Delete Envelopes
            string deleteEnvelopesCommand = "DELETE FROM Envelopes WHERE Id = @Id";
            using SqliteCommand deleteEnvelopesCommandObj = currConnection.CreateCommand();
            deleteEnvelopesCommandObj.CommandText = deleteEnvelopesCommand;
            deleteEnvelopesCommandObj.Parameters.AddWithValue("@Id", envelopeId.ToString());
            deleteEnvelopesCommandObj.ExecuteNonQuery();

        }
    }
}
