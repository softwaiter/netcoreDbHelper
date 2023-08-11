using System;
using System.Collections.Concurrent;
using System.Data;
using System.Data.Common;

namespace CodeM.Common.DbHelper
{
    public class DbUtils
    {

        #region 设置数据提供者、数据源等配置
        private static ConcurrentDictionary<string, DataSource> sDataSources = new ConcurrentDictionary<string, DataSource>();

        /// <summary>
        /// 注册数据库工厂，用以支持对应的数据库类型
        /// </summary>
        /// <param name="providerName">名称，唯一</param>
        /// <param name="providerFactoryClassName">数据库工厂对应的类全名称，如：MySql.Data.MySqlClient.MySqlClientFactory</param>
        public static void RegisterDbProvider(string providerName, string providerFactoryClassName)
        {
            DbProviderFactories.RegisterFactory(providerName, providerFactoryClassName);
        }

        /// <summary>
        /// 取消注册的数据库工厂
        /// </summary>
        /// <param name="providerName">名称，唯一</param>
        public static bool UnregisterDbProvider(string providerName)
        {
            return DbProviderFactories.UnregisterFactory(providerName);
        }

        /// <summary>
        /// 添加数据源
        /// </summary>
        /// <param name="name">数据源名称，唯一</param>
        /// <param name="providerName">数据源提供者，对应RegisterDbProvider注册的数据提供者名称</param>
        /// <param name="connectionString">数据源连接字符串，根据提供者不同格式不同</param>
        /// <returns></returns>
        public static bool AddDataSource(string name, string providerName, string connectionString)
        {
            return sDataSources.TryAdd(name, new DataSource(name, providerName, connectionString));
        }

        /// <summary>
        /// 移除数据源
        /// </summary>
        /// <param name="name">数据源名称，唯一</param>
        /// <returns></returns>
        public static bool RemoveDataSource(string name)
        {
            DataSource result;
            return sDataSources.TryRemove(name, out result);
        }

        /// <summary>
        /// 判断数据源是否存在
        /// </summary>
        /// <param name="name">数据源名称，唯一</param>
        /// <returns></returns>
        public static bool HasDataSource(string name)
        {
            return sDataSources.ContainsKey(name);
        }

        /// <summary>
        /// 根据数据源名称返回数据源对象
        /// </summary>
        /// <param name="name">数据源名称，唯一</param>
        /// <returns></returns>
        private static DataSource GetDataSourceByName(string name)
        {
            DataSource result;
            if (!sDataSources.TryGetValue(name, out result))
            {
                throw new Exception(string.Concat("未声明的的数据源：", name));
            }
            return result;
        }

        /// <summary>
        /// 根据数据源对象返回数据提供者工厂
        /// </summary>
        /// <param name="datasource">数据源对象</param>
        /// <returns></returns>
        private static DbProviderFactory GetDbProviderFactoryByDataSource(DataSource datasource)
        {
            return DbProviderFactories.GetFactory(datasource.ProviderName);
        }

        /// <summary>
        /// 根据数据源名称返回数据提供者工厂
        /// </summary>
        /// <param name="datasourceName">数据源名称</param>
        /// <returns></returns>
        public static DbProviderFactory GetDbProviderFactoryByDataSourceName(string datasourceName)
        {
            DataSource datasource = GetDataSourceByName(datasourceName);
            return GetDbProviderFactoryByDataSource(datasource);
        }

        #endregion

        #region 创建各种数据对象

        /// <summary>
        /// 默认执行命令超时时间，单位：秒
        /// </summary>
        public static int DefaultCommandTimeout { get; set; } = 30;

        /// <summary>
        /// 根据数据源名称创建并返回对应数据库连接对象
        /// </summary>
        /// <param name="datasourceName"></param>
        /// <returns></returns>
        public static DbConnection GetConnection(string datasourceName)
        {
            DataSource datasource = GetDataSourceByName(datasourceName);
            DbProviderFactory factory = GetDbProviderFactoryByDataSource(datasource);
            DbConnection connection = factory.CreateConnection();
            connection.ConnectionString = datasource.ConnectionString;
            return connection;
        }

        /// <summary>
        /// 根据数据源名称创建数据库命令对象
        /// </summary>
        /// <param name="datasourceName"></param>
        /// <returns></returns>
        public static DbCommand CreateCommand(string datasourceName)
        {
            return CreateCommand(datasourceName, DefaultCommandTimeout);
        }

        public static DbCommand CreateCommand(string datasourceName, int timeout)
        {
            DbProviderFactory factory = GetDbProviderFactoryByDataSourceName(datasourceName);
            DbCommand command = factory.CreateCommand();
            command.CommandTimeout = timeout;
            return command;
        }

        /// <summary>
        /// 根据数据连接对象创建数据库命令对象
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        public static DbCommand CreateCommand(DbConnection connection)
        {
            return CreateCommand(connection, DefaultCommandTimeout);
        }

        public static DbCommand CreateCommand(DbConnection connection, int timeout)
        {
            DbCommand command = connection.CreateCommand();
            command.CommandTimeout = timeout;
            return command;
        }

        /// <summary>
        /// 创建数据源对应的命令变量
        /// </summary>
        /// <param name="datasourceName">数据源名称</param>
        /// <param name="name">变量名</param>
        /// <param name="value">变量值</param>
        /// <returns></returns>
        public static DbParameter CreateParam(string datasourceName, string name, object value)
        {
            DbProviderFactory factory = GetDbProviderFactoryByDataSourceName(datasourceName);
            DbParameter parameter = factory.CreateParameter();
            parameter.ParameterName = name;
            parameter.Value = value;
            return parameter;
        }

        public static DbParameter CreateParam(string datasourceName, string name, object value, 
            DbType type, ParameterDirection dir)
        {
            DbParameter parameter = CreateParam(datasourceName, name, value);
            parameter.DbType = type;
            parameter.Direction = dir;
            return parameter;
        }
        #endregion

        #region 事务相关
        public static DbTransaction GetTransaction(string datasourceName)
        {
            return GetTransaction(datasourceName, IsolationLevel.Unspecified);
        }

        public static DbTransaction GetTransaction(string datasourceName, IsolationLevel level)
        {
            DbConnection connection = GetConnection(datasourceName);
            connection.Open();
            return connection.BeginTransaction(level);
        }

        public static void CommitTransaction(DbTransaction transaction)
        {
            DbConnection connection = transaction.Connection;
            transaction.Commit();
            connection.Close();
        }

        public static void RollbackTransaction(DbTransaction transaction)
        {
            DbConnection connection = transaction.Connection;
            transaction.Rollback();
            connection.Close();
        }
        #endregion

        #region 参数准备
        /// <summary>
        /// 为DbCommand对象设置参数信息
        /// </summary>
        /// <param name="command">DbCommand对象</param>
        /// <param name="transaction">事务对象</param>
        /// <param name="commandType">Command类型</param>
        /// <param name="commandText">Command命令字符串</param>
        /// <param name="commandParams">参数数组</param>
        /// <param name="needCloseConnection">是否需要关闭DbConnection，用于返回</param>
        private static void SetCommandParams(DbCommand command, DbTransaction transaction, CommandType commandType, 
            string commandText, DbParameter[] commandParams, out bool needCloseConnection)
        {
            if (command == null)
            {
                throw new Exception("无效的command");
            }
            if (command.Connection.State != ConnectionState.Open)
            {
                command.Connection.Open();
                needCloseConnection = true;
            }
            else
            {
                needCloseConnection = false;
            }
            if (transaction != null)
            {
                command.Transaction = transaction;
            }
            command.CommandType = commandType;
            command.CommandText = commandText;
            if (commandParams != null)
            {
                AttachParameters(command, commandParams);
            }
        }

        /// <summary>
        /// 为command对象添加参数
        /// </summary>
        /// <param name="command"></param>
        /// <param name="commandParams"></param>
        private static void AttachParameters(DbCommand command, DbParameter[] commandParams)
        {
            if (commandParams != null)
            {
                foreach (DbParameter param in commandParams)
                {
                    if (param != null)
                    {
                        if ((param.Direction == ParameterDirection.Input ||
                            param.Direction == ParameterDirection.InputOutput) &&
                            (param.Value == null))
                        {
                            param.Value = DBNull.Value;
                        }
                        command.Parameters.Add(param);
                    }
                }
            }
        }
        #endregion

        #region ExecuteNonQuery
        public static int ExecuteNonQuery(string datasourceName, string commandText)
        {
            return ExecuteNonQuery(datasourceName, commandText, null);
        }

        public static int ExecuteNonQuery(string datasourceName, string commandText, params DbParameter[] commandParams)
        {
            return ExecuteNonQuery(datasourceName, CommandType.Text, commandText, commandParams);
        }

        public static int ExecuteNonQuery(string datasourceName, CommandType commandType, string commandText, params DbParameter[] commandParams)
        {
            using (DbConnection connection = GetConnection(datasourceName))
            {
                return ExecuteNonQuery(connection, commandType, commandText, commandParams);
            }
        }

        public static int ExecuteNonQuery(DbConnection connection, string commandText)
        {
            return ExecuteNonQuery(connection, commandText, null);
        }

        public static int ExecuteNonQuery(DbConnection connection, string commandText, params DbParameter[] commandParams)
        {
            return ExecuteNonQuery(connection, CommandType.Text, commandText, commandParams);
        }

        /// <summary>
        /// 无返回查询
        /// </summary>
        /// <param name="connection">数据连接对象</param>
        /// <param name="commandType">命令类型</param>
        /// <param name="commandText">命令字符串</param>
        /// <param name="commandParams">命令参数</param>
        /// <returns></returns>
        public static int ExecuteNonQuery(DbConnection connection, CommandType commandType, string commandText, params DbParameter[] commandParams)
        {
            int result = 0;
            DbCommand command = CreateCommand(connection);
            bool needCloseConnection = false;
            SetCommandParams(command, null, commandType, commandText, commandParams, out needCloseConnection);
            try
            {
                result = command.ExecuteNonQuery();
            }
            finally
            {
                command.Parameters.Clear();
                if (needCloseConnection)
                {
                    connection.Close();
                }
            }
            return result;
        }

        public static int ExecuteNonQuery(DbTransaction transaction, string commandText)
        {
            return ExecuteNonQuery(transaction, commandText, null);
        }

        public static int ExecuteNonQuery(DbTransaction transaction, string commandText, params DbParameter[] commandParams)
        {
            return ExecuteNonQuery(transaction, CommandType.Text, commandText, commandParams);
        }

        /// <summary>
        /// 无返回查询
        /// </summary>
        /// <param name="transaction">数据库事务对象</param>
        /// <param name="commandType">命令类型</param>
        /// <param name="commandText">命令字符串</param>
        /// <param name="commandParams">命令参数</param>
        /// <returns></returns>
        public static int ExecuteNonQuery(DbTransaction transaction, CommandType commandType, string commandText, params DbParameter[] commandParams)
        {
            if (transaction == null)
            {
                throw new Exception("无效的Transaction");
            }
            if (transaction.Connection == null)
            {
                throw new Exception("当前Transaction已经回滚或提交，请提供一个有效的Transaction");
            }
            DbCommand command = CreateCommand(transaction.Connection);
            bool needCloseConnection = false;

            SetCommandParams(command, transaction, commandType, commandText, commandParams, out needCloseConnection);
            int result = 0;

            try
            {
                result = command.ExecuteNonQuery();
            }
            finally
            {
                command.Parameters.Clear();
                if (needCloseConnection)
                {
                    transaction.Connection.Close();
                }
            }

            return result;
        }
        #endregion

        #region ExecuteScalar
        public static object ExecuteScalar(string datasourceName, string commandText)
        {
            return ExecuteScalar(datasourceName, commandText, null);
        }

        public static object ExecuteScalar(string datasourceName, string commandText, params DbParameter[] commandParams)
        {
            return ExecuteScalar(datasourceName, CommandType.Text, commandText, commandParams);
        }

        public static object ExecuteScalar(string datasourceName, CommandType commandType, string commandText, params DbParameter[] commandParams)
        {
            using (DbConnection connection = GetConnection(datasourceName))
            {
                return ExecuteScalar(connection, commandType, commandText, commandParams);
            }
        }

        public static object ExecuteScalar(DbConnection connection, string commandText)
        {
            return ExecuteScalar(connection, commandText, null);
        }

        public static object ExecuteScalar(DbConnection connection, string commandText, params DbParameter[] commandParams)
        {
            return ExecuteScalar(connection, CommandType.TableDirect, commandText, commandParams);
        }

        /// <summary>
        /// 返回查询结果的第一行的第一列的内容
        /// </summary>
        /// <param name="connection">数据连接对象</param>
        /// <param name="commandType">命令类型</param>
        /// <param name="commandText">命令字符串</param>
        /// <param name="commandParams">命令参数</param>
        /// <returns></returns>
        public static object ExecuteScalar(DbConnection connection, CommandType commandType, string commandText, params DbParameter[] commandParams)
        {
            object result = null;
            DbCommand command = CreateCommand(connection);
            bool needCloseConnection = false;
            SetCommandParams(command, null, commandType, commandText, commandParams, out needCloseConnection);
            try
            {
                result = command.ExecuteScalar();
            }
            finally
            {
                command.Parameters.Clear();
                if (needCloseConnection)
                {
                    connection.Close();
                }
            }
            return result;
        }

        public static object ExecuteScalar(DbTransaction transaction, string commandText)
        {
            return ExecuteScalar(transaction, commandText, null);
        }

        public static object ExecuteScalar(DbTransaction transaction, string commandText, params DbParameter[] commandParams)
        {
            return ExecuteScalar(transaction, CommandType.Text, commandText, commandParams);
        }

        /// <summary>
        /// 返回查询结果的第一行的第一列的内容
        /// </summary>
        /// <param name="transaction">数据库事务对象</param>
        /// <param name="commandType">命令类型</param>
        /// <param name="commandText">命令字符串</param>
        /// <param name="commandParams">命令参数</param>
        /// <returns></returns>
        public static object ExecuteScalar(DbTransaction transaction, CommandType commandType, string commandText, params DbParameter[] commandParams)
        {
            if (transaction == null)
            {
                throw new Exception("无效的Transaction");
            }
            if (transaction.Connection == null)
            {
                throw new Exception("当前Transaction已经回滚或提交，请提供一个有效的Transaction");
            }
            DbCommand command = CreateCommand(transaction.Connection);
            bool needCloseConnection = false;

            SetCommandParams(command, transaction, commandType, commandText, commandParams, out needCloseConnection);

            object result = null;
            try
            {
                result = command.ExecuteScalar();
            }
            finally
            {
                command.Parameters.Clear();
                if (needCloseConnection)
                {
                    transaction.Connection.Close();
                }
            }
            return result;
        }
        #endregion

        #region ExecuteReader

        public static DbDataReader ExecuteDataReader(string datasourceName, string commandText)
        {
            return ExecuteDataReader(datasourceName, commandText, null);
        }

        public static DbDataReader ExecuteDataReader(string datasourceName, string commandText, params DbParameter[] commandParams)
        {
            return ExecuteDataReader(datasourceName, CommandType.Text, commandText, commandParams);
        }

        public static DbDataReader ExecuteDataReader(string datasourceName, CommandType commandType, string commandText, params DbParameter[] commandParams)
        {
            DbConnection connection = GetConnection(datasourceName);
            return ExecuteDataReader(connection, commandType, commandText, commandParams);
        }

        public static DbDataReader ExecuteDataReader(DbConnection connection, string commandText)
        {
            return ExecuteDataReader(connection, commandText, null);
        }

        public static DbDataReader ExecuteDataReader(DbConnection connection, string commandText, params DbParameter[] commandParams)
        {
            return ExecuteDataReader(connection, CommandType.Text, commandText, commandParams);
        }

        /// <summary>
        /// 返回查询结果集
        /// </summary>
        /// <param name="connection">数据连接对象</param>
        /// <param name="commandType">命令类型</param>
        /// <param name="commandText">命令字符串</param>
        /// <param name="commandParams">命令参数</param>
        /// <returns></returns>
        public static DbDataReader ExecuteDataReader(DbConnection connection, CommandType commandType, string commandText, params DbParameter[] commandParams)
        {
            DbCommand command = CreateCommand(connection);
            bool needCloseConnection = false;
            SetCommandParams(command, null, commandType, commandText, commandParams, out needCloseConnection);

            DbDataReader result = null;
            try
            {
                if (needCloseConnection)
                {
                    result = command.ExecuteReader(CommandBehavior.CloseConnection);
                }
                else
                {
                    result = command.ExecuteReader();
                }
            }
            catch
            {
                if (needCloseConnection)
                {
                    connection.Close();
                }
                throw;
            }
            finally
            {
                command.Parameters.Clear();
            }
            return result;
        }

        public static DbDataReader ExecuteDataReader(DbTransaction transaction, string commandText)
        {
            return ExecuteDataReader(transaction, commandText, null);
        }

        public static DbDataReader ExecuteDataReader(DbTransaction transaction, string commandText, params DbParameter[] commandParams)
        {
            return ExecuteDataReader(transaction, CommandType.Text, commandText, commandParams);
        }

        /// <summary>
        /// 返回查询结果集
        /// </summary>
        /// <param name="transaction">数据库事务对象</param>
        /// <param name="commandType">命令类型</param>
        /// <param name="commandText">命令字符串</param>
        /// <param name="commandParams">命令参数</param>
        /// <returns></returns>
        public static DbDataReader ExecuteDataReader(DbTransaction transaction, CommandType commandType, string commandText, params DbParameter[] commandParams)
        {
            if (transaction == null)
            {
                throw new Exception("无效的Transaction");
            }
            if (transaction.Connection == null)
            {
                throw new Exception("当前Transaction已经回滚或提交，请提供一个有效的Transaction");
            }
            DbCommand command = CreateCommand(transaction.Connection);
            bool needCloseConnection = false;

            SetCommandParams(command, transaction, commandType, commandText, commandParams, out needCloseConnection);

            DbDataReader result = null;
            try
            {
                if (needCloseConnection)
                {
                    result = command.ExecuteReader(CommandBehavior.CloseConnection);
                }
                else
                {
                    result = command.ExecuteReader();
                }
            }
            catch
            {
                if (needCloseConnection)
                {
                    transaction.Connection.Close();
                }
                throw;
            }
            finally
            {
                command.Parameters.Clear();
            }
            return result;
        }
        #endregion

    }
}
