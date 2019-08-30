using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using AggieGlobal.WebApi.DataAccess.Common;
using AggieGlobal.WebApi.Infrastructure;
using System.Collections.Specialized;

namespace AggieGlobal.WebApi.DataAccess
{
    internal abstract class SqlDataAccessRepository<TItem> : BaseDataAccessRepository where TItem : class, new() 
    {
        protected readonly DbTransaction _transaction = null;
        private const string _defaultConnectionStringName = "AggieGlobalDB";

        private const int defaultBatchCount = 25;
        protected int readerIndex = 0; /*used for identifying how many record set need to skip from the reader object*/
        protected int outParams = 0; /*used for identifying how many out parameter used in sp*/
        protected Dictionary<string, string> outContext; /*used for containing out parameter values*/

        internal SqlDataAccessRepository()
            : this(_defaultConnectionStringName, null)
        {
        }

        internal SqlDataAccessRepository(string connectionStringName)
            : this(connectionStringName, null)
        {
        }

        internal SqlDataAccessRepository(DbTransaction transaction)
            : this(_defaultConnectionStringName, transaction)
        {
        }

        internal SqlDataAccessRepository(string connectionStringName, DbTransaction transaction)
            : base(connectionStringName)
        {
            _transaction = transaction;
        }

        #region ExecuteNonQuery

        protected int Create(params object[] values)
        {
            using (var connection = GetConnection())
            {
                connection.Open();

                int id;
                using (var command = CommandParser.Current.GetCommand<TItem>(connection, Method.CREATE, values))
                {
                    command.ExecuteNonQuery();

                    if (!int.TryParse(command.Parameters[command.Parameters.Count - 1].Value.ToString(), out id) || id < 1)
                    {
                        throw new CannotCreateDataException(typeof(TItem).ToString() + "cannot be created");
                    }
                }

                return id;
            }
        }

        protected int CreateRecord(string caption, params object[] values)
        {
            using (var connection = GetConnection())
            {
                connection.Open();

                int id;
                using (var command = CommandParser.Current.PrepareCommand<TItem>(connection, Method.CREATE, caption, values))
                {
                    command.ExecuteNonQuery();

                    if (!int.TryParse(command.Parameters[command.Parameters.Count - 1].Value.ToString(), out id) || id < 1)
                        throw new CannotCreateDataException(typeof(TItem).ToString() + "cannot be created");
                }

                return id;
            }
        }

        protected void CreateNoResult(params object[] values)
        {
            using (var connection = GetConnection())
            {
                connection.Open();

                using (var command = CommandParser.Current.GetCommand<TItem>(connection, Method.CREATE, values))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        protected void CreateRecordNoResult(string caption, params object[] values)
        {
            using (var connection = GetConnection())
            {
                connection.Open();

                using (var command = CommandParser.Current.PrepareCommand<TItem>(connection, Method.CREATE, caption, values))
                    command.ExecuteNonQuery();
            }
        }

        protected void CreateRecordNoResultWithinTransaction(string caption, DbTransaction transaction, params object[] values)
        {
            using (var command = CommandParser.Current.PrepareCommand<TItem>(transaction.Connection, Method.CREATE, caption, values))
            {
                command.Transaction = transaction;
                command.ExecuteNonQuery();
            }
        }
        protected int CreateRecordWithinTransaction(string caption, DbTransaction transaction, params object[] values)
        {
            int id;
            using (var command = CommandParser.Current.PrepareCommand<TItem>(transaction.Connection, Method.CREATE, caption, values))
            {
                command.Transaction = transaction;
                command.ExecuteNonQuery();
                id = Convert.ToInt32(command.Parameters[command.Parameters.Count - 1].Value.ToString());
                if (id > 0) return id;
                else throw new CannotCreateDataException(typeof(TItem).ToString() + "cannot be created");
            }
            return id;
        }

        protected bool CreateRecordBoolResultWithinTransaction(string caption, DbTransaction transaction, params object[] values)
        {
            bool bReturn = false;
            using (var command = CommandParser.Current.PrepareCommand<TItem>(transaction.Connection, Method.CREATE, caption, values))
            {
                command.Transaction = transaction;
                command.ExecuteNonQuery();
                if (outParams > 0)
                    outContext = new Dictionary<string, string>();

                while (outParams > 0)
                {
                    outContext.Add(command.Parameters[command.Parameters.Count - outParams].ParameterName
                        , command.Parameters[command.Parameters.Count - outParams].Value.ToString());
                    outParams--;
                }
                bReturn = true;
            }
            return bReturn;
        }

        protected int DeleteRecord(string caption, params object[] values)
        {
            using (var connection = GetConnection())
            {
                try
                {
                    connection.Open();

                    using (var command = CommandParser.Current.PrepareCommand<TItem>(connection, Method.DELETE, caption, values))
                    {
                        command.ExecuteNonQuery();
                    }
                    return 1;
                }
                catch
                {
                    return -1;
                }
            }
        }

        protected int DeleteRecordWithinTransaction(string caption, DbTransaction transaction, params object[] values)
        {
            try
            {
                using (var command = CommandParser.Current.PrepareCommand<TItem>(transaction.Connection, Method.DELETE, caption, values))
                {
                    command.Transaction = transaction;
                    command.ExecuteNonQuery();
                }
                return 1;
            }
            catch
            {
                // TODO: Catch custom exceptions
                return -1;
            }
        }
        protected int UpdateRecord(string caption, params object[] values)
        {
            using (var connection = GetConnection())
            {
                try
                {
                    connection.Open();

                    using (var command = CommandParser.Current.PrepareCommand<TItem>(connection, Method.UPDATE, caption, values))
                    {
                        command.ExecuteNonQuery();

                        if (outParams > 0)
                            outContext = new Dictionary<string, string>();

                        while (outParams > 0)
                        {
                            outContext.Add(command.Parameters[command.Parameters.Count - outParams].ParameterName
                                , command.Parameters[command.Parameters.Count - outParams].Value.ToString());
                            outParams--;
                        }
                    }
                    return 1;
                }
                catch (Exception ex)
                {
                    // TODO: Catch custom exceptions
                    return -1;
                }
            }
        }

        protected int UpdateRecordWithinTransaction(string caption, DbTransaction transaction, params object[] values)
        {
            try
            {
                using (var command = CommandParser.Current.PrepareCommand<TItem>(transaction.Connection, Method.UPDATE, caption, values))
                {
                    command.Transaction = transaction;
                    command.ExecuteNonQuery();

                    if (outParams > 0)
                        outContext = new Dictionary<string, string>();

                    while (outParams > 0)
                    {
                        outContext.Add(command.Parameters[command.Parameters.Count - outParams].ParameterName
                            , command.Parameters[command.Parameters.Count - outParams].Value.ToString());
                        outParams--;
                    }
                }
                return 1;
            }
            catch (Exception ex)
            {
                // TODO: Catch custom exceptions
                return -1;
            }
        }


        //F&A viewer
        protected int Create<T>(string caption, params object[] values)
        {
            using (var connection = GetConnection())
            {
                connection.Open();

                int id;
                using (var command = CommandParser.Current.GetCommand<T>(connection, Method.CREATE, caption, values))
                {
                    command.ExecuteNonQuery();

                    if (!int.TryParse(command.Parameters[command.Parameters.Count - 1].Value.ToString(), out id) || id < 1)
                        throw new Exception(typeof(T).ToString() + "cannot be created");
                }

                return id;
            }
        }

        protected int Update<T>(string caption, params object[] values)
        {
            using (var connection = GetConnection())
            {
                try
                {
                    connection.Open();

                    using (var command = CommandParser.Current.GetCommand<T>(connection, Method.UPDATE, caption, values))
                    {
                        command.ExecuteNonQuery();

                        if (outParams > 0)
                            outContext = new Dictionary<string, string>();

                        while (outParams > 0)
                        {
                            outContext.Add(command.Parameters[command.Parameters.Count - outParams].ParameterName
                                , command.Parameters[command.Parameters.Count - outParams].Value.ToString());
                            outParams--;
                        }
                    }
                    return 1;
                }
                catch (Exception ex)
                {
                    // TODO: Catch custom exceptions
                    return -1;
                }
            }
        }

        protected int Delete<T>(string caption, params object[] values)
        {
            using (var connection = GetConnection())
            {
                try
                {
                    connection.Open();

                    using (var command = CommandParser.Current.GetCommand<T>(connection, Method.DELETE, caption, values))
                    {
                        command.ExecuteNonQuery();
                    }
                    return 1;
                }
                catch (Exception ex)
                {
                    return -1;
                }
            }
        }

        #endregion

        #region ExecuteReader

        protected IEnumerable<TItem> GetRecord(string caption, params object[] values)
        {

            using (DbConnection connection = GetConnection())
            {
                connection.Open();
                 
                using (var dbCommand = CommandParser.Current.PrepareCommand<TItem>(connection, Method.READ, caption, values))
                {
                    using (var reader = dbCommand.ExecuteReader())
                    {
                        while (readerIndex > 0)
                        {
                            reader.NextResult();
                            readerIndex--;
                        }

                        IList<string> rows = null;
                        using (var datatable = reader.GetSchemaTable())
                        {
                            rows = GetDataRows(datatable);
                        }

                        var properties = GetObjectProperties(typeof(TItem));
                        List<TItem> collectionToReturn = new List<TItem>();
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                var element = new TItem();

                                foreach (var property in properties)
                                {
                                    if (rows.Contains(property.Name))
                                    {
                                        var field = reader[property.Name];
                                        if (field.GetType() != typeof(DBNull))
                                            property.SetValue(element, field, null);
                                    }
                                }
                                collectionToReturn.Add(element);
                            }
                        }
                        reader.Close();
                        if (connection.State == ConnectionState.Open)
                        {
                            connection.Close();
                            connection.Dispose();
                        }
                        return collectionToReturn;
                    }
                }
            }
        }

        protected int GetOrdinalRecordData(string caption, params object[] values)
        {
            using (DbConnection connection = GetConnection())
            {
                connection.Open();

                using (var dbCommand = CommandParser.Current.PrepareCommand<TItem>(connection, Method.ORDINAL, caption, values))
                {
                    using (var reader = dbCommand.ExecuteReader())
                    {
                        var rowNumber = -1;
                        while (reader.Read())
                            int.TryParse(reader["RowNumber"].ToString(), out rowNumber);

                        reader.Close();
                        if (connection.State == ConnectionState.Open)
                        {
                            connection.Close();
                            connection.Dispose();
                        }
                        return rowNumber;

                    }
                }
            }
        }

        protected int GetOrdinalRecordEx(string caption, params object[] values)
        {
            using (DbConnection connection = GetConnection())
            {
                connection.Open();

                using (var dbCommand = CommandParser.Current.PrepareCommand<TItem>(connection, Method.ORDINAL, caption, values))
                {
                    using (var reader = dbCommand.ExecuteReader())
                    {
                        var rowNumber = -1;
                        while (reader.Read())
                            int.TryParse(reader["IsPermitted"].ToString(), out rowNumber);

                        reader.Close();
                        if (connection.State == ConnectionState.Open)
                        {
                            connection.Close();
                            connection.Dispose();
                        }
                        return rowNumber;

                    }
                }
            }
        }

        protected IEnumerable<T> GetData<T>(string caption, params object[] values) where T : new()
        {
            using (DbConnection connection = GetConnection())
            {
                connection.Open();

                using (var command = CommandParser.Current.GetCommand<T>(connection, Method.READ, caption, values))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (readerIndex > 0)
                        {
                            reader.NextResult();
                            readerIndex--;
                        }

                        IList<string> rows = null;
                        using (var datatable = reader.GetSchemaTable())
                        {
                            rows = GetDataRows(datatable);
                        }

                        var properties = GetObjectProperties(typeof(T));

                        while (reader.Read())
                        {
                            var element = new T();

                            foreach (var property in properties)
                            {
                                if (rows.Contains(property.Name))
                                {
                                    var field = reader[property.Name];
                                    if (field.GetType() != typeof(DBNull))
                                        property.SetValue(element, field, null);
                                }
                            }
                            yield return element;
                        }

                        reader.Close();
                    }
                }
            }
        }

        #endregion

        #region ExecuteScalar

        protected int GetRecordCount(string caption, params object[] values)
        {
            using (DbConnection connection = GetConnection())
            {
                connection.Open();

                using (var dbCommand = CommandParser.Current.PrepareCommand<TItem>(connection, Method.COUNT, caption, values))
                {
                    object o = dbCommand.ExecuteScalar();
                    if (o == null)
                        return 0;

                    int retval = -1;
                    if (!int.TryParse(o.ToString(), out retval))
                    {
                        return 0;
                    }
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                        connection.Dispose();
                    }
                    return retval;
                }
            }
        }

        protected object GetScalarRecord(string caption, params object[] values)
        {
            using (DbConnection connection = GetConnection())
            {
                connection.Open();

                using (var dbCommand = CommandParser.Current.PrepareCommand<TItem>(connection, Method.READ, caption, values))
                {
                   return dbCommand.ExecuteScalar();
                }
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
        }

        protected int GetCount<T>(string caption, params object[] values)
        {
            using (DbConnection connection = GetConnection())
            {
                connection.Open();

                using (var command = CommandParser.Current.GetCommand<T>(connection, Method.COUNT, caption, values))
                {
                    var retval = int.Parse(command.ExecuteScalar().ToString());
                    return retval;
                }
            }
        }

        protected object GetScalar<T>(string caption, params object[] values)
        {
            using (DbConnection connection = GetConnection())
            {
                connection.Open();

                using (var command = CommandParser.Current.GetCommand<T>(connection, Method.READ, caption, values))
                {
                    return command.ExecuteScalar();
                }
            }
        }

        #endregion

        #region ExexuteBatch

        protected string BatchCommand(string caption, Method method, params object[] values)
        {
            var ns = typeof(TItem).ToString().Split('.');
            var key = ns[ns.Length - 1];

            return CommandParser.Current.PrepareCommand(key, method, caption, values);
        }

        /* To execute string or varchar parameter value every time one single quote need to be added to execute the command eg: val=abc need to be ''abc'' */
        protected bool BatchExecution(DbTransaction transaction, StringCollection commands, int batchCount)
        {
            bool success = false;

            if (batchCount == 0)
                batchCount = defaultBatchCount;

            int commandCount = commands.Count;
            int totalBatches = commandCount / batchCount;

            if ((commandCount % batchCount) == 0)
                totalBatches = commandCount / batchCount;
            else
                totalBatches = (commandCount / batchCount) + 1;

            int currentBatch = default(int);
            int currentItemCount = default(int);
            string executeCommand = "sp_executesql  N' ";
            string[] finalCommands = new string[totalBatches];
            bool finalLeft = false;

            for (int loop = 0; loop < commandCount; loop++)
            {
                if ((commands[loop] != null) && (commands[loop] != string.Empty))
                {
                    if (currentItemCount == batchCount)
                    {
                        if (commands[loop] != string.Empty)
                        {
                            executeCommand += commands[loop];
                            executeCommand += ";";
                            executeCommand += "'";
                            currentItemCount = default(int);
                            finalCommands[currentBatch++] = executeCommand;
                            executeCommand = "sp_executesql  N' ";
                            finalLeft = false;
                        }
                    }
                    else
                    {
                        if (commands[loop] != string.Empty)
                        {
                            executeCommand += commands[loop];
                            executeCommand += ";";
                            currentItemCount++;
                            finalLeft = true;
                        }
                    }
                }
            }

            if (finalLeft)
            {
                executeCommand += "'";
                currentItemCount = default(int);
                finalCommands[currentBatch++] = executeCommand;
                finalLeft = false;
            }

            //Now Loop through the each item and execute
            commandCount = finalCommands.Length;
            if (commandCount > 0)
            {
                //objTrans = objConn.BeginTransaction();
                //bTransStarted = true;

                for (int loop = 0; loop < commandCount; loop++)
                {
                    executeCommand = finalCommands[loop];
                    if ((executeCommand != null) && (executeCommand != string.Empty))
                    {
                        if (transaction != null)
                        {
                            using (var command = transaction.Connection.CreateCommand())
                            {
                                command.Transaction = transaction;
                                command.CommandType = CommandType.Text;
                                command.CommandText = executeCommand;
                                command.ExecuteNonQuery();
                            }
                        }
                        else
                        {
                            using (var connection = GetConnection())
                            {
                                connection.Open();
                                using (var command = connection.CreateCommand())
                                {
                                    command.CommandType = CommandType.Text;
                                    command.CommandText = executeCommand;
                                    command.ExecuteNonQuery();
                                }
                                if (connection.State == ConnectionState.Open)
                                {
                                    connection.Close();
                                    connection.Dispose();
                                }
                            }
                        }
                    }
                }
            }
            success = true;

            return success;
        }

        #endregion ExexuteBatch

        private IEnumerable<PropertyInfo> GetObjectProperties(Type type)
        {
            // TODO - Consider property attributes and types
            return type.GetProperties();
        }

        private IDictionary<string, PropertyInfo> GetObjectPropertyInfo(Type type)
        {
            var retval = new Dictionary<string, PropertyInfo>();
            foreach (var property in type.GetProperties())
            {
                retval.Add(property.Name, property);
            }
            return retval;
        }

        private IList<string> GetDataRows(DataTable dataTable)
        {
            var retval = new List<string>();

            if (dataTable != null)
            {
                foreach (DataRow dr in dataTable.Rows)
                {
                    if (!retval.Contains(dr[0].ToString()))
                        retval.Add(dr[0].ToString());
                    dr.Delete();
                }

                dataTable.Rows.Clear();
            }
            return retval;
        }

        private dynamic DataReaderToExpando(IDataReader reader)
        {
            var expandoObject = new ExpandoObject() as IDictionary<string, object>;

            for (var i = 0; i < reader.FieldCount; i++)
            {
                if (!expandoObject.ContainsKey(reader.GetName(i)))
                {
                    expandoObject.Add(reader.GetName(i), reader[i]);
                }
            }
            return expandoObject;
        }
    }
}