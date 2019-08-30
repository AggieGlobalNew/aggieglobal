/* ========================================================================
 * Includes    : FormatterConfig.cs
 * Website     : http://www.atlassoft.com
 * Create Date : May, May 08, 2019 by Sudipta Sarkar
 * Details     : Implements various data type formatter registration logics for AggieGlobal.WebApi.
 * Copyright © 2019 Atlas Software Technologies All rights reserved
 * ========================================================================
 * Release History
 * ---------------
 * DESCRIPTION					CREATED / MODIFIED BY
 * -----------                  ---------------------
 * Initial Implementation		Sudipta Sarkar
 * ========================================================================
 */

using System;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using AggieGlobal.WebApi.Infrastructure;
using System.Text.RegularExpressions;
using AggieGlobal.WebApi.Common;
using AggieGlobal.WebApi.DataAccess.Interface;

namespace AggieGlobal.WebApi.DataAccess.Common
{
    public class CommandParser : ICommandParser
    {
        private static ICommandParser singletonObject = new CommandParser();
        private XDocument xdoc;

        private CommandParser()
        {
            string filePath = Path.Combine(Utility.CurrentDirectory, ConfigurationManager.AppSettings["DataAccessConfigPath"]);
            if (!File.Exists(filePath))
            {
                string s = string.Format("FATAL ERROR: Configuration file for template:{0} is not present from 'CommandParser'! Application can't be run correctly!, Check the App-key \"DataAccessConfigPath\" to fix configuration issues!", filePath);
                AggieGlobalLogManager.Fatal(s);
                //AggieGlobalLogManager.Trace(, Severity.Error);
                return;
            }
            xdoc = XDocument.Load(filePath);
        }

        public static ICommandParser Current
        {
            get
            {
                return singletonObject;
            }
        }

        public DbCommand GetCommand<TItem>(DbConnection connection, Method method, params object[] values)
        {
            var ns = typeof(TItem).ToString().Split('.');
            var type = ns[ns.Length - 1];

            return GetCommand(type, connection, method, values);
        }

        public DbCommand GetCommand(string key, DbConnection connection, Method method, params object[] values)
        {
            var objectElement = xdoc.Descendants("Object").Where(o => o.Attribute("Key").Value.Equals(key));
            var commandElement = objectElement.Descendants("DbCommand").Where(d => d.Attribute("Method").Value.Equals(method.ToString()));
            var found = commandElement.Count() == 1;

            if (!found)
            {
                for (int index = 0; index < commandElement.Count(); index++)
                {
                    int count = 0;
                    var cmdElement = commandElement.ElementAt(index);
                    if (cmdElement.Attribute("Caption") == null)
                    {
                        foreach (var elem in cmdElement.Descendants("Parameter"))
                        {
                            var att = elem.Attribute("Value");

                            if (att != null && att.Value.StartsWith("%"))
                                count++;
                        }
                    }

                    if (count == values.Length)
                    {
                        commandElement = cmdElement.DescendantsAndSelf();
                        found = true;
                        break;
                    }
                }
            }

            if (!found)
                throw new InvalidOperationException("DbCommand could not be resolved");

            var dbCommand = connection.CreateCommand();

            var commandType = commandElement.Attributes("CommandType").FirstOrDefault();
            if (commandType != null)
            {
                dbCommand.CommandType = commandType.Value.Equals("StoredProcedure") ?
                CommandType.StoredProcedure : CommandType.Text;
            }

            dbCommand.CommandText = commandElement.Attributes("CommandText").First().Value;

            if (dbCommand.CommandType == CommandType.StoredProcedure)
            {
                foreach (var parameterNode in commandElement.Descendants("Parameter"))
                {
                    var parameter = dbCommand.CreateParameter();
                    var paramName = parameterNode.Attribute("Name");
                    if (paramName != null && !string.IsNullOrWhiteSpace(paramName.Value))
                    {
                        parameter.ParameterName = "@" + paramName.Value;

                        var val = parameterNode.Attribute("Value");
                        if (val != null)
                        {
                            if (val.Value.StartsWith("%"))
                            {
                                var pos = int.Parse(val.Value.TrimStart('%'));
                                parameter.Value = pos < values.Length ? values[pos] : null;
                            }
                            else
                            {
                                parameter.Value = val.Value;
                            }
                        }

                        var direction = parameterNode.Attribute("Direction");
                        if (direction != null)
                        {
                            parameter.Direction = (ParameterDirection)Enum.Parse(typeof(ParameterDirection), direction.Value);
                        }

                        var size = parameterNode.Attribute("Size");
                        if (size != null)
                        {
                            parameter.Size = int.Parse(size.Value);
                        }

                        var dbType = parameterNode.Attribute("DbType");
                        if (dbType != null)
                        {
                            parameter.DbType = (DbType)Enum.Parse(typeof(DbType), dbType.Value);
                        }

                        var allowNull = false;
                        if (parameterNode.Attribute("AllowNull") != null)
                        {
                            allowNull = parameterNode.Attribute("AllowNull").Value.Equals("true");
                        }

                        if (!(parameter.Value == null && parameter.Direction == ParameterDirection.Input) || allowNull)
                        {
                            AddParameter(dbCommand, parameter);
                        }
                    }
                }
            }
            else
            {
                dbCommand.CommandText = string.Format(dbCommand.CommandText, values);
            }

            return dbCommand;
        }

        private static void AddParameter(DbCommand dbCommand, DbParameter parameter)
        {
            if (dbCommand.CommandType == CommandType.StoredProcedure && (dbCommand.CommandText.EndsWith("_q") || dbCommand.CommandText.Contains("_q_")))
            {
                //TODO: Presently parameters are handled with considersation (for having some immediate effects) that all sql injections having "'" (single quote)
                //      ,it may cause to miss some other combinations. 
                //      It is strongly recommended to use parameterized sp instead of having dynamic query.
                var value = parameter.Value.ToString();

                if ((value.Contains("'")) && (parameter.DbType == DbType.String || parameter.DbType == DbType.AnsiString))
                {
                    if (value.ToLower().Contains("and") || value.ToLower().Contains("or") || value.ToLower().Contains("like"))
                    {
                        var values = Regex.Split(value, @"(AND)|(and)|(OR)|(or)|(like)|(LIKE)|(,)|(=)|(>)|(<)|( )|(\()|(\))");
                        for (var index = 0; index < values.Length; index++)
                        {
                            if (values[index].Contains("'"))
                            {
                                var startsWithN = values[index].StartsWith("N'");
                                if (startsWithN)
                                    values[index] = values[index].TrimStart('N');

                                var startsWith = values[index].StartsWith("'");
                                if (startsWith)
                                    values[index] = values[index].Remove(0, 1);

                                var endsWith = values[index].EndsWith("'");
                                if (endsWith)
                                    values[index] = values[index].Remove(values[index].Length - 1, 1);

                                values[index] = (startsWith ? "'" : string.Empty) + values[index].Replace("''", "'").Replace("'", "''") + (endsWith ? "'" : string.Empty);
                                if (startsWithN)
                                    values[index] = "N" + values[index];

                                if (index == 0 && !startsWithN & startsWith)
                                    values[index] = values[index].Replace("''", "'").Replace("'", "''");
                            }
                        }

                        parameter.Value = string.Join(string.Empty, values);
                    }
                    else
                    {
                        parameter.Value = parameter.Value.ToString().Replace("''", "'").Replace("'", "''");
                    }
                }
            }

            dbCommand.Parameters.Add(parameter);
        }

        public DbCommand GetCommand(string query, DbConnection connection)
        {
            var dbCommand = connection.CreateCommand();

            dbCommand.CommandType = CommandType.Text;

            dbCommand.CommandText = query;

            return dbCommand;
        }

        public DbCommand PrepareCommand<TItem>(DbConnection connection, Method method, string caption, params object[] values)
        {
            var ns = typeof(TItem).ToString().Split('.');
            var type = ns[ns.Length - 1];

            return PrepareCommand(type, connection, method, caption, values);
        }

        public DbCommand PrepareCommand(string key, DbConnection connection, Method method, string caption, params object[] values)
        {
            var objectElement = xdoc.Descendants("Object").Where(o => o.Attribute("Key").Value.Equals(key));
            var commandElement = objectElement.Descendants("DbCommand").Where(d => d.Attribute("Method").Value.Equals(method.ToString()) && (d.Attribute("Caption") != null && d.Attribute("Caption").Value.Equals(caption)));
            var found = commandElement.Count() == 1;

            if (!found)
                throw new InvalidOperationException("DbCommand could not be resolved");

            var dbCommand = connection.CreateCommand();

            var commandType = commandElement.Attributes("CommandType").FirstOrDefault();
            if (commandType != null)
            {
                dbCommand.CommandType = commandType.Value.Equals("StoredProcedure") ?
                CommandType.StoredProcedure : CommandType.Text;
            }

            dbCommand.CommandText = commandElement.Attributes("CommandText").First().Value;

            if (dbCommand.CommandType == CommandType.StoredProcedure)
            {
                foreach (var parameterNode in commandElement.Descendants("Parameter"))
                {
                    var parameter = dbCommand.CreateParameter();
                    var paramName = parameterNode.Attribute("Name");
                    if (paramName != null && !string.IsNullOrWhiteSpace(paramName.Value))
                    {
                        parameter.ParameterName = "@" + paramName.Value;

                        var val = parameterNode.Attribute("Value");
                        if (val != null)
                        {
                            if (val.Value.StartsWith("%"))
                            {
                                var pos = int.Parse(val.Value.TrimStart('%'));
                                parameter.Value = pos < values.Length ? values[pos] : null;
                            }
                            else
                            {
                                parameter.Value = val.Value;
                            }
                        }

                        var direction = parameterNode.Attribute("Direction");
                        if (direction != null)
                        {
                            parameter.Direction = (ParameterDirection)Enum.Parse(typeof(ParameterDirection), direction.Value);
                        }

                        var size = parameterNode.Attribute("Size");
                        if (size != null)
                        {
                            parameter.Size = int.Parse(size.Value);
                        }

                        var dbType = parameterNode.Attribute("DbType");
                        if (dbType != null)
                        {
                            parameter.DbType = (DbType)Enum.Parse(typeof(DbType), dbType.Value);
                        }

                        var allowNull = false;
                        if (parameterNode.Attribute("AllowNull") != null)
                        {
                            allowNull = parameterNode.Attribute("AllowNull").Value.Equals("true");
                        }

                        if (!(parameter.Value == null && parameter.Direction == ParameterDirection.Input) || allowNull)
                        {
                            AddParameter(dbCommand, parameter);
                        }
                    }
                }
            }
            else
            {
                dbCommand.CommandText = string.Format(dbCommand.CommandText, values);
            }

            return dbCommand;
        }

        public string PrepareCommand(string key, Method method, string caption, params object[] values)
        {
            var objectElement = xdoc.Descendants("Object").Where(o => o.Attribute("Key").Value.Equals(key));
            var commandElement = objectElement.Descendants("DbCommand").Where(d => d.Attribute("Method").Value.Equals(method.ToString()) && (d.Attribute("Caption") != null && d.Attribute("Caption").Value.Equals(caption)));
            var found = commandElement.Count() == 1;

            if (!found)
                throw new InvalidOperationException("DbCommand could not be resolved");

            var commantText = string.Empty;

            var commandType = commandElement.Attributes("CommandType").FirstOrDefault();
            var cType = CommandType.Text;

            if (commandType != null)
                cType = commandType.Value.Equals("StoredProcedure") ? CommandType.StoredProcedure : CommandType.Text;

            commantText = commandElement.Attributes("CommandText").First().Value;

            if (cType == CommandType.StoredProcedure)
                commantText = string.Empty;
            else
                commantText = string.Format(commantText, values);

            return commantText;
        }

        public string GetCommand(string key, Method method, params object[] values)
        {
            var objectElement = xdoc.Descendants("Object").Where(o => o.Attribute("Key").Value.Equals(key));
            var commandElement = objectElement.Descendants("DbCommand").Where(d => d.Attribute("Method").Value.Equals(method.ToString()));
            var found = commandElement.Count() == 1;

            if (!found)
            {
                for (int index = 0; index < commandElement.Count(); index++)
                {
                    int count = 0;
                    var cmdElement = commandElement.ElementAt(index);
                    foreach (var elem in cmdElement.Descendants("Parameter"))
                    {
                        var att = elem.Attribute("Value");

                        if (att != null && att.Value.StartsWith("%"))
                            count++;
                    }

                    if (count == values.Length)
                    {
                        commandElement = cmdElement.DescendantsAndSelf();
                        found = true;
                        break;
                    }
                }
            }

            if (!found)
                throw new InvalidOperationException("DbCommand could not be resolved");

            var commantText = string.Empty;

            var commandType = commandElement.Attributes("CommandType").FirstOrDefault();
            var cType = CommandType.Text;

            if (commandType != null)
                cType = commandType.Value.Equals("StoredProcedure") ? CommandType.StoredProcedure : CommandType.Text;

            commantText = commandElement.Attributes("CommandText").First().Value;

            if (cType == CommandType.StoredProcedure)
                commantText = string.Empty;
            else
                commantText = string.Format(commantText, values);

            return commantText;
        }

    }

    public enum Method
    {
        COUNT,
        ORDINAL,
        CREATE,
        READ,
        UPDATE,
        DELETE
    }
}