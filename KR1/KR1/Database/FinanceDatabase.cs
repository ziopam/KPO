using KR1.DomainObjects;
using Microsoft.Data.Sqlite;

namespace KR1.Database
{
    internal class FinanceDatabase : IFinanceDatabase
    {
        private SqliteConnection connection;
        private readonly static String connectionString = "Data Source=FinanceDatabase.db";
        private readonly static String dateFormat = "MM.dd.yyyy";

        public FinanceDatabase()
        {
            connection = new SqliteConnection(connectionString);

            try
            {
                connection.Open();
                CreateTables();
            }
            catch (SqliteException e)
            {
                Console.WriteLine($"Ошибка при открытии БД {e.Message}");
                Environment.Exit(1);
            }
        }

        private void CreateTables()
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "CREATE TABLE IF NOT EXISTS BankAccounts (Id INTEGER PRIMARY KEY AUTOINCREMENT, Name TEXT, Balance DECIMAL)";
                command.ExecuteNonQuery();
            }
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "CREATE TABLE IF NOT EXISTS Categories (Id INTEGER PRIMARY KEY AUTOINCREMENT, Name TEXT, IsIncome BOOLEAN)";
                command.ExecuteNonQuery();
            }
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "CREATE TABLE IF NOT EXISTS Operations (Id INTEGER PRIMARY KEY AUTOINCREMENT, IsIncome BOOLEAN NOT NULL, BankAccountId INTEGER NOT NULL, Amount DECIMAL NOT NULL, Date TEXT NOT NULL, CategoryId INTEGER NOT NULL, Description TEXT, FOREIGN KEY (BankAccountId) REFERENCES BankAccounts(Id) ON DELETE CASCADE, FOREIGN KEY (CategoryId) REFERENCES Categories(Id) ON DELETE CASCADE)";
                command.ExecuteNonQuery();
            }
        }

        public Int64 GetNextId(String table_name)
        {
            try
            {
                using var command = connection.CreateCommand();
                command.CommandText = $"SELECT seq FROM sqlite_sequence WHERE name = '{table_name}'";
                var result = command.ExecuteScalar();
                return result == null ? 1 : (Int64)result! + 1;
            }
            catch (SqliteException e)
            {
                Console.WriteLine($"Ошибка при получении следующего Id счета {e.Message}");
                Environment.Exit(-1);
                return -1;
            }
        }

        public void UpdateBalance(long accountId, decimal amount)
        {
            try
            {
                using var command = connection.CreateCommand();
                command.CommandText = "UPDATE BankAccounts SET Balance = Balance + @Amount WHERE Id = @Id";
                command.Parameters.AddWithValue("@Amount", amount);
                command.Parameters.AddWithValue("@Id", accountId);
                command.ExecuteNonQuery();
            }
            catch (SqliteException e)
            {
                Console.WriteLine($"Ошибка при обновлении баланса счета {e.Message}");
            }
        }

        public void AddAccount(string name, decimal balance)
        {
            ArgumentNullException.ThrowIfNull(name);

            try
            {
                using var command = connection.CreateCommand();
                command.CommandText = "INSERT INTO BankAccounts (Name, Balance) VALUES (@Name, @Balance)";
                command.Parameters.AddWithValue("@Name", name);
                command.Parameters.AddWithValue("@Balance", balance);
                command.ExecuteNonQuery();
            }
            catch (SqliteException e)
            {
                Console.WriteLine($"Ошибка при добавлении счета {e.Message}");
            }
        }

        public void AddCategory(string name, bool isIncome)
        {
            ArgumentNullException.ThrowIfNull(name);

            try
            {
                using var command = connection.CreateCommand();
                command.CommandText = "INSERT INTO Categories (Name, IsIncome) VALUES (@Name, @IsIncome)";
                command.Parameters.AddWithValue("@Name", name);
                command.Parameters.AddWithValue("@IsIncome", isIncome);
                command.ExecuteNonQuery();
            }
            catch (SqliteException e)
            {
                Console.WriteLine($"Ошибка при добавлении категории {e.Message}");
            }
        }

        public void AddOperation(bool isIncome, long bankAccountId, decimal amount, DateOnly date, long categoryId, string description = "")
        {
            try
            {
                using var command = connection.CreateCommand();
                command.CommandText = "INSERT INTO Operations (IsIncome, BankAccountId, Amount, Date, CategoryId, Description) VALUES (@IsIncome, @BankAccountId, @Amount, @Date, @CategoryId, @Description)";
                command.Parameters.AddWithValue("@IsIncome", isIncome);
                command.Parameters.AddWithValue("@BankAccountId", bankAccountId);
                command.Parameters.AddWithValue("@Amount", amount);
                command.Parameters.AddWithValue("@Date", date.ToString(dateFormat));
                command.Parameters.AddWithValue("@CategoryId", categoryId);
                command.Parameters.AddWithValue("@Description", description);
                command.ExecuteNonQuery();

                UpdateBalance(bankAccountId, isIncome ? amount : -amount);
            }
            catch (SqliteException e)
            {
                Console.WriteLine($"Ошибка при добавлении операции {e.Message}");
            }
        }

        public IEnumerable<BankAccount> GetAccounts()
        {
            try
            {
                var bankAccounts = new List<BankAccount>();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT * FROM BankAccounts";

                    using var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        int id = reader.GetInt32(0);
                        string name = reader.GetString(1);
                        decimal balance = reader.GetDecimal(2);

                        var bankAccount = new BankAccount(id, name, balance);
                        bankAccounts.Add(bankAccount);
                    }
                }

                return bankAccounts;
            }
            catch (SqliteException e)
            {
                Console.WriteLine($"Ошибка при получении счетов {e.Message}");
                Environment.Exit(-1);
                return [];
            }
        }

        public IEnumerable<Category> GetCategories()
        {
            try
            {
                var categories = new List<Category>();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT * FROM Categories";
                    using var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        int id = reader.GetInt32(0);
                        string name = reader.GetString(1);
                        bool isIncome = reader.GetBoolean(2);
                        var category = new Category(id, name, isIncome);
                        categories.Add(category);
                    }
                }
                return categories;
            }
            catch (SqliteException e)
            {
                Console.WriteLine($"Ошибка при получении категорий {e.Message}");
                Environment.Exit(-1);
                return [];
            }
        }

        public IEnumerable<Operation> GetOperations()
        {
            try
            {
                var operations = new List<Operation>();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT * FROM Operations";
                    using var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        int id = reader.GetInt32(0);
                        bool isIncome = reader.GetBoolean(1);
                        int bankAccountId = reader.GetInt32(2);
                        decimal amount = reader.GetDecimal(3);
                        DateOnly date = DateOnly.ParseExact(reader.GetString(4), "MM.dd.yyyy");
                        int categoryId = reader.GetInt32(5);
                        string description = reader.GetString(6);
                        var operation = new Operation(id, isIncome, bankAccountId, amount, date, categoryId, description);
                        operations.Add(operation);
                    }
                }
                return operations;
            }
            catch (SqliteException e)
            {
                Console.WriteLine($"Ошибка при получении операций {e.Message}");
                Environment.Exit(-1);
                return [];
            }
        }

        public void RemoveAccount(long accountId)
        {
            try
            {
                using var command = connection.CreateCommand();
                command.CommandText = "DELETE FROM BankAccounts WHERE Id = @Id";
                command.Parameters.AddWithValue("@Id", accountId);
                command.ExecuteNonQuery();
            }
            catch (SqliteException e)
            {
                Console.WriteLine($"Ошибка при удалении счета {e.Message}");
            }
        }

        public void RemoveCategory(long categoryId)
        {
            try
            {
                using var getAccountCommand = connection.CreateCommand();
                getAccountCommand.CommandText = "SELECT BankAccountId, IsIncome, Amount  FROM Operations WHERE CategoryId = @CategoryId";
                getAccountCommand.Parameters.AddWithValue("@CategoryId", categoryId);
                var reader = getAccountCommand.ExecuteReader();
                while (reader.Read())
                {
                    long bankAccountId = reader.GetInt64(0);
                    bool isIncome = reader.GetBoolean(1);
                    decimal amount = reader.GetDecimal(2);
                    UpdateBalance(bankAccountId, isIncome ? -amount : amount);
                }

                using var command = connection.CreateCommand();
                command.CommandText = "DELETE FROM Categories WHERE Id = @Id";
                command.Parameters.AddWithValue("@Id", categoryId);
                command.ExecuteNonQuery();
            }
            catch (SqliteException e)
            {
                Console.WriteLine($"Ошибка при удалении категории {e.Message}");
            }
        }

        public void RemoveOperation(long operationId)
        {
            try
            {
                using var command = connection.CreateCommand();
                command.CommandText = "SELECT BankAccountId, IsIncome, Amount FROM Operations WHERE Id = @Id";
                command.Parameters.AddWithValue("@Id", operationId);
                var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    long bankAccountId = reader.GetInt64(0);
                    bool isIncome = reader.GetBoolean(1);
                    decimal amount = reader.GetDecimal(2);
                    UpdateBalance(bankAccountId, isIncome ? -amount : amount);
                }

                using var deleteCommand = connection.CreateCommand();
                deleteCommand.CommandText = "DELETE FROM Operations WHERE Id = @Id";
                deleteCommand.Parameters.AddWithValue("@Id", operationId);
                deleteCommand.ExecuteNonQuery();
            }
            catch (SqliteException e)
            {
                Console.WriteLine($"Ошибка при удалении операции {e.Message}");
            }
        }

        public void UpdateAccountName(long accountId, string newName)
        {
            try
            {
                using var command = connection.CreateCommand();
                command.CommandText = "UPDATE BankAccounts SET Name = @Name WHERE Id = @Id";
                command.Parameters.AddWithValue("@Name", newName);
                command.Parameters.AddWithValue("@Id", accountId);
                command.ExecuteNonQuery();
            }
            catch (SqliteException e)
            {
                Console.WriteLine($"Ошибка при обновлении имени счета {e.Message}");
            }
        }

        public void UpdateCategoryIsIncome(long categoryId, bool isIncome)
        {
            try
            {
                using var getAccountCommand = connection.CreateCommand();
                getAccountCommand.CommandText = "SELECT BankAccountId, IsIncome, Amount  FROM Operations WHERE CategoryId = @CategoryId";
                getAccountCommand.Parameters.AddWithValue("@CategoryId", categoryId);
                var reader = getAccountCommand.ExecuteReader();
                while (reader.Read())
                {
                    long bankAccountId = reader.GetInt64(0);
                    bool oldIsIncome = reader.GetBoolean(1);
                    decimal amount = reader.GetDecimal(2);

                    if (oldIsIncome == isIncome)
                    {
                        continue;
                    }

                    if (isIncome)
                    {
                        UpdateBalance(bankAccountId, 2 * amount);
                    }
                    else
                    {
                        UpdateBalance(bankAccountId, 2 * (-amount));
                    }
                }

                using var updateCategoryCommand = connection.CreateCommand();
                updateCategoryCommand.CommandText = "UPDATE Categories SET IsIncome = @IsIncome WHERE Id = @Id";
                updateCategoryCommand.Parameters.AddWithValue("@IsIncome", isIncome);
                updateCategoryCommand.Parameters.AddWithValue("@Id", categoryId);
                updateCategoryCommand.ExecuteNonQuery();

                using var updateOperationCommand = connection.CreateCommand();
                updateOperationCommand.CommandText = "UPDATE Operations SET IsIncome = @IsIncome WHERE CategoryId = @CategoryId";
                updateOperationCommand.Parameters.AddWithValue("@IsIncome", isIncome);
                updateOperationCommand.Parameters.AddWithValue("@CategoryId", categoryId);
                updateOperationCommand.ExecuteNonQuery();
            }
            catch (SqliteException e)
            {
                Console.WriteLine($"Ошибка при обновлении типа категории {e.Message}");

            }
        }

        public void UpdateCategoryName(long categoryId, string newName)
        {
            try
            {
                using var command = connection.CreateCommand();
                command.CommandText = "UPDATE Categories SET Name = @Name WHERE Id = @Id";
                command.Parameters.AddWithValue("@Name", newName);
                command.Parameters.AddWithValue("@Id", categoryId);
                command.ExecuteNonQuery();
            }
            catch (SqliteException e)
            {
                Console.WriteLine($"Ошибка при обновлении имени категории {e.Message}");
            }
        }

        public void UpdateOperationAmount(long operationId, decimal newAmount)
        {
            try
            {
                using var command = connection.CreateCommand();
                command.CommandText = "SELECT BankAccountId, IsIncome, Amount FROM Operations WHERE Id = @Id";
                command.Parameters.AddWithValue("@Id", operationId);
                var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    long bankAccountId = reader.GetInt64(0);
                    bool isIncome = reader.GetBoolean(1);
                    decimal oldAmount = reader.GetDecimal(2);
                    UpdateBalance(bankAccountId, isIncome ? newAmount - oldAmount : oldAmount - newAmount);
                }

                using var updateCommand = connection.CreateCommand();
                updateCommand.CommandText = "UPDATE Operations SET Amount = @Amount WHERE Id = @Id";
                updateCommand.Parameters.AddWithValue("@Amount", newAmount);
                updateCommand.Parameters.AddWithValue("@Id", operationId);
                updateCommand.ExecuteNonQuery();
            }
            catch (SqliteException e)
            {
                Console.WriteLine($"Ошибка при обновлении суммы операции {e.Message}");
            }
        }

        public void UpdateOperationBankAccountId(long operationId, long newBankAccountId)
        {
            try
            {
                using var command = connection.CreateCommand();
                command.CommandText = "SELECT BankAccountId, IsIncome, Amount FROM Operations WHERE Id = @Id";
                command.Parameters.AddWithValue("@Id", operationId);
                var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    long oldBankAccountId = reader.GetInt64(0);
                    bool isIncome = reader.GetBoolean(1);
                    decimal amount = reader.GetDecimal(2);
                    UpdateBalance(oldBankAccountId, isIncome ? -amount : amount);
                    UpdateBalance(newBankAccountId, isIncome ? amount : -amount);
                }

                using var updateCommand = connection.CreateCommand();
                updateCommand.CommandText = "UPDATE Operations SET BankAccountId = @BankAccountId WHERE Id = @Id";
                updateCommand.Parameters.AddWithValue("@BankAccountId", newBankAccountId);
                updateCommand.Parameters.AddWithValue("@Id", operationId);
                updateCommand.ExecuteNonQuery();
            }
            catch (SqliteException e)
            {
                Console.WriteLine($"Ошибка при обновлении счета операции {e.Message}");
            }
        }

        public void UpdateOperationCategoryId(long operationId, long newCategoryId)
        {
            try
            {
                using var getNewCategoryCommand = connection.CreateCommand();
                getNewCategoryCommand.CommandText = "SELECT IsIncome FROM Categories WHERE Id = @Id";
                getNewCategoryCommand.Parameters.AddWithValue("@Id", newCategoryId);
                var newCategoryReader = getNewCategoryCommand.ExecuteReader();
                bool newIsIncome = false;
                if (newCategoryReader.Read())
                {
                    newIsIncome = newCategoryReader.GetBoolean(0);
                }


                using var getAccountCommand = connection.CreateCommand();
                getAccountCommand.CommandText = "SELECT BankAccountId, IsIncome, Amount  FROM Operations WHERE Id = @Id";
                getAccountCommand.Parameters.AddWithValue("@Id", operationId);
                var reader = getAccountCommand.ExecuteReader();
                if (reader.Read())
                {
                    long bankAccountId = reader.GetInt64(0);
                    bool isIncome = reader.GetBoolean(1);
                    decimal amount = reader.GetDecimal(2);
                    UpdateBalance(bankAccountId, isIncome ? -amount : amount);
                    UpdateBalance(bankAccountId, newIsIncome ? amount : -amount);
                }

                using var command = connection.CreateCommand();
                command.CommandText = "UPDATE Operations SET CategoryId = @CategoryId WHERE Id = @Id";
                command.Parameters.AddWithValue("@CategoryId", newCategoryId);
                command.Parameters.AddWithValue("@Id", operationId);
                command.ExecuteNonQuery();

                using var updateIsIncomeCommand = connection.CreateCommand();
                updateIsIncomeCommand.CommandText = "UPDATE Operations SET IsIncome = @IsIncome WHERE Id = @Id";
                updateIsIncomeCommand.Parameters.AddWithValue("@IsIncome", newIsIncome);
                updateIsIncomeCommand.Parameters.AddWithValue("@Id", operationId);
                updateIsIncomeCommand.ExecuteNonQuery();
            }
            catch (SqliteException e)
            {
                Console.WriteLine($"Ошибка при обновлении категории операции {e.Message}");
            }
        }

        public void UpdateOperationDate(long operationId, DateOnly newDate)
        {
            try
            {
                using var command = connection.CreateCommand();
                command.CommandText = "UPDATE Operations SET Date = @Date WHERE Id = @Id";
                command.Parameters.AddWithValue("@Date", newDate.ToString(dateFormat));
                command.Parameters.AddWithValue("@Id", operationId);
                command.ExecuteNonQuery();
            }
            catch (SqliteException e)
            {
                Console.WriteLine($"Ошибка при обновлении даты операции {e.Message}");
            }
        }

        public void UpdateOperationDescription(long operationId, string newDescription)
        {
            try
            {
                using var command = connection.CreateCommand();
                command.CommandText = "UPDATE Operations SET Description = @Description WHERE Id = @Id";
                command.Parameters.AddWithValue("@Description", newDescription);
                command.Parameters.AddWithValue("@Id", operationId);
                command.ExecuteNonQuery();
            }
            catch (SqliteException e)
            {
                Console.WriteLine($"Ошибка при обновлении описания операции {e.Message}");
            }
        }
    }
}
