# .netcore DBHelper
---

## 简介
netcoreDBHelper不是ORM框架，只是对.netcore中数据库操作的封装。封装的意义在于使用DBHelper进行数据库操作时，将操作代码和具体的数据库隔离开，在未来如果需要更换数据库便不需要修改操作代码，只需简单的替换数据提供者和数据源配置信息。

目前支持Sqlite、Sqlserver、Mysql、Oracle、PostgreSQL、达梦数据库，理论上只要实现了微软数据提供者接口的关系型数据库都能得到支持，各位可以自行尝试。



## 版本

**.NetCore3.1**：2.0.0之前的版本全部基于.netcore3.1开发；后续所有变更在netcore3.1分支进行。
<br>
**.Net8.0**：从2.0.0开始之后的版本，全部基于.net8开发；后续所有变更在master分支进行。



## Install

### 依赖安装
#### Package Manager
```shell
Install-Package CodeM.Common.DbHelper -Version 2.0.0
```

#### .NET CLI
```shell
dotnet add package CodeM.Common.DbHelper --version 2.0.0
```

#### PackageReference
```xml
<PackageReference Include="CodeM.Common.DbHelper" Version="2.0.0" />
```

#### Paket CLI
```shell
paket add CodeM.Common.DbHelper --version 2.0.0
```



## 代码示例

### Sqlite示例（需安装System.Data.SQLite程序包）
```c#
// 同一类型数据库只需注册一次
DbUtils.RegisterDbProvider("sqlite", "System.Data.SQLite.SQLiteFactory, System.Data.SQLite");
DbUtils.AddDataSource("sqlite_test", "sqlite", "DataSource=test.db;Version=3;");

DbUtils.ExecuteNonQuery("sqlite_test", "Insert Into User (id, name) Values(1, 'wangxm')");

string sql = "Select * From test Where id=1";
DbDataReader dr = DbUtils.ExecuteDataReader("sqlite_test", sql);
```



### Mysql示例（需安装MySql.Data程序包）

```c#
// 同一类型数据库只需注册一次
DbUtils.RegisterDbProvider("mysql", "MySql.Data.MySqlClient.MySqlClientFactory, MySql.Data");
DbUtils.AddDataSource("mysql_test", "mysql", "Server=localhost;Database=test; User=root;Password=root;");

DbUtils.ExecuteNonQuery("mysql_test", "Insert Into User (id, name) Values(1, 'wangxm')");

string sql = "Select * From test Where id=1";
DbDataReader dr = DbUtils.ExecuteDataReader("mysql_test", sql);
```



### Sqlserver示例（需安装Microsoft.Data.SqlClient程序包）

```c#
// 同一类型数据库只需注册一次
DbUtils.RegisterDbProvider("sqlserver", "Microsoft.Data.SqlClient.SqlClientFactory, Microsoft.Data.SqlClient");
DbUtils.AddDataSource("sqlserver_test", "sqlserver", "Data Source=localhost;Database=test;User Id=sa;Password=sa123;Encrypt=no;");

DbUtils.ExecuteNonQuery("sqlserver_test", "Insert Into User (id, name) Values(1, 'wangxm')");

string sql = "Select * From test Where id=1";
DbDataReader dr = DbUtils.ExecuteDataReader("sqlserver_test", sql);
```



### Oracle示例（需安装Oracle.ManagedDataAccess.Core程序包）

```c#
// 同一类型数据库只需注册一次
DbUtils.RegisterDbProvider("oracle", "Oracle.ManagedDataAccess.Client.OracleClientFactory, Oracle.ManagedDataAccess");
string connString = "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST = 127.0.0.1)(PORT = 1521)))(CONNECT_DATA = (SERVICE_NAME = orcl)));User Id = system; Password = orcl123";
DbUtils.AddDataSource("oracle_test", "oracle", connString);

DbUtils.ExecuteNonQuery("oracle_test", "Insert Into User (id, name) Values(1, 'wangxm')");

string sql = "Select * From test Where id=1";
DbDataReader dr = DbUtils.ExecuteDataReader("oracle_test", sql);
```



### PostgreSQL示例（需安装Npgsql程序包）

```c#
// 同一类型数据库只需注册一次
DbUtils.RegisterDbProvider("postgres", "Npgsql.NpgsqlFactory, Npgsql");
DbUtils.AddDataSource("postgres_test", "postgres", "Host=localhost;Database=test;User Id=postgres;Password=postgres;");

DbUtils.ExecuteNonQuery("postgres_test", "Insert Into User (id, name) Values(1, 'wangxm')");

string sql = "Select * From test Where id=1";
DbDataReader dr = DbUtils.ExecuteDataReader("postgres_test", sql);
```



### 达梦数据库示例（需安装dmdbms.DmProvider程序包）

```c#
// 同一类型数据库只需注册一次
DbUtils.RegisterDbProvider("dm", "Dm.DmClientFactory, DmProvider");
DbUtils.AddDataSource("dm_test", "dm", "server=localhost;port=5236;schema=test;user=TEST;password=TEST123456");

DbUtils.ExecuteNonQuery("dm_test", "Insert Into User (id, name) Values(1, 'wangxm')");

string sql = "Select * From test Where id=1";
DbDataReader dr = DbUtils.ExecuteDataReader("dm_test", sql);
```



## 方法

### 一、注册数据库提供者（只要是实现了.netcore数据库提供者的数据库都可以）
#### 定义：
public static void RegisterDbProvider(string providerName, string providerFactoryClassName)
#### 参数：
providerName: 提供者名字，用户自定义，唯一性。
<br>
providerFactoryClassName: 数据库提供者工厂类全名称，如: MySql.Data.MySqlClient.MySqlClientFactory, MySql.Data
#### 返回：
无

### 二、移出注册的数据库提供者
#### 定义：
public static bool UnregisterDbProvider(string providerName)
#### 参数：
providerName: 提供者名字
#### 返回：
返回执行结果，成功返回True，失败返回False。

### 三、增加数据源
#### 定义： 
public static bool AddDataSource(string name, string providerName, string connectionString)
#### 参数：
name: 数据源名字，用户自定义，唯一性。
<br>
providerName: 提供者名字。
<br>
connectionString: 数据库连接字符串，因提供者不同而不同；如MySql: Server=localhost;Database=test; User=root;Password=root;
#### 返回：
返回执行结果，成功返回True，失败返回False。

### 四、删除数据源
#### 定义：
public static bool RemoveDataSource(string name)
#### 参数：
name: 数据源名字。
#### 返回：
返回执行结果，成功返回True，失败返回False。

### 五、判断指定名字的数据源是否存在
#### 定义：
public static bool HasDataSource(string name)
#### 参数：
naem: 数据源名字。
#### 返回：
指定数据源存在返回True，否则返回False。

### 六、创建并返回一个指定数据源的连接对象
#### 定义：
public static DbConnection GetConnection(string datasourceName)
#### 参数：
datasourceName: 数据源名字。
#### 返回：
数据库连接对象的实例。

### 七、创建并返回一个数据库命令执行对象
#### 定义：
public static DbCommand CreateCommand(string datasourceName)
#### 参数：
datasourceName: 数据源名称。
#### 返回：
数据库命令执行对象的实例。

### 八、创建并返回一个数据库命令执行对象
#### 定义：
public static DbCommand CreateCommand(string datasourceName, int timeout)
#### 参数：
datasourceName: 数据源名称。
timeout: 命令执行的超时时间。
#### 返回：
数据库命令执行对象的实例。

### 九、创建并返回一个数据库命令执行对象
#### 定义：
public static DbCommand CreateCommand(DbConnection connection)
#### 参数：
connection: 数据库连接对象实例。
#### 返回：
数据库命令执行对象的实例。

### 十、创建并返回一个数据库命令执行对象
#### 定义：
public static DbCommand CreateCommand(DbConnection connection, int timeout)
#### 参数：
connection: 数据库连接对象实例。
<br>
timeout: 命令执行的超时时间。
#### 返回：
数据库命令执行对象的实例。

### 十一、创建并返回一个数据库命令参数对象
#### 定义：
public static DbParameter CreateParam(string datasourceName, string name, object value)
#### 参数：
datasourceName: 数据源名称。
<br>
name: 参数名称。
<br>
value: 参数值。
#### 返回：
数据库命令参数对象的实例。

### 十二、创建并返回一个数据库命令参数对象
#### 定义：
public static DbParameter CreateParam(string datasourceName, string name, object value, DbType type, ParameterDirection dir)
#### 参数：
datasourceName: 数据源名称。
<br>
name: 参数名称。
<br>
value: 参数值。
<br>
dir: 参数方向，输入参数还是输出参数。
#### 返回：
数据库命令参数对象的实例。

### 十三、创建并返回一个数据库事务对象
#### 定义：
public static DbTransaction GetTransaction(string datasourceName)
#### 参数：
datasourceName: 数据源名称。
#### 返回：
数据库事务对象的实例。

### 十四、创建并返回一个数据库事务对象
#### 定义：
public static DbTransaction GetTransaction(string datasourceName, IsolationLevel level)
#### 参数：
datasourceName: 数据源名称。
<br>
level: 事务隔离级别。
#### 返回：
数据库事务对象的实例。

### 十五、提交指定数据库事务
#### 定义：
public static void CommitTransaction(DbTransaction transaction)
#### 参数：
transaction: 数据库事务对象。
#### 返回：
无

### 十六、回滚指定数据库事务
#### 定义：
public static void RollbackTransaction(DbTransaction transaction)
#### 参数：
transaction: 数据库事务对象。
#### 返回：
无

### 十七、执行数据库命令，通常用于增、删、改操作
#### 定义：
public static int ExecuteNonQuery(string datasourceName, string commandText)
#### 参数：
datasourceName: 数据源名称。
<br>
commandText: 数据库命令。
#### 返回：
执行数据库命令影响的记录条数。

### 十八、执行数据库命令，通常用于增、删、改操作
#### 定义：
public static int ExecuteNonQuery(string datasourceName, string commandText, params DbParameter[] commandParams)
#### 参数：
datasourceName: 数据源名称。
<br>
commandText: 数据库命令
<br>
commandParams: 数据库命令参数
#### 返回：
执行数据库命令影响的记录条数。

### 十九、执行数据库命令，通常用于增、删、改操作
#### 定义：
public static int ExecuteNonQuery(string datasourceName, CommandType commandType, string commandText, params DbParameter[] commandParams)
#### 参数：
datasourceName: 数据源名称。
<br>
commandType: 执行命令的类型。
<br>
commandText: 数据库命令。
<br>
commandParams: 数据库命令参数。
#### 返回：
执行数据库命令影响的记录条数。

### 二十、执行数据库命令，通常用于增、删、改操作
#### 定义：
public static int ExecuteNonQuery(DbConnection connection, string commandText)
#### 参数：
connection: 数据库连接对象。
<br>
commandText: 数据库命令。
#### 返回：
执行数据库命令影响的记录条数。

### 二十一、执行数据库命令，通常用于增、删、改操作
#### 定义：
public static int ExecuteNonQuery(DbConnection connection, string commandText, params DbParameter[] commandParams)
#### 参数：
connection: 数据库连接对象。
<br>
commandText: 数据库命令。
<br>
commandParams: 数据库命令参数。
#### 返回：
执行数据库命令影响的记录条数。

### 二十二、执行数据库命令，通常用于增、删、改操作
#### 定义：
public static int ExecuteNonQuery(DbConnection connection, CommandType commandType, string commandText, params DbParameter[] commandParams)
#### 参数：
connection: 数据库连接对象。
<br>
commandType: 执行命令的类型。
<br>
commandText: 数据库命令。
<br>
commandParams: 数据库命令参数。
#### 返回：
执行数据库命令影响的记录条数。

### 二十三、执行数据库命令，通常用于增、删、改操作
#### 定义：
public static int ExecuteNonQuery(DbTransaction transaction, string commandText)
#### 参数：
transaction: 数据库事务对象。
<br>
commandText: 数据库命令。
#### 返回：
执行数据库命令影响的记录条数。

### 二十四、执行数据库命令，通常用于增、删、改操作
#### 定义：
public static int ExecuteNonQuery(DbTransaction transaction, string commandText, params DbParameter[] commandParams)
#### 参数：
transaction: 数据库事务对象。
<br>
commandText: 数据库命令。
<br>
commandParams: 数据库命令参数。
#### 返回：
执行数据库命令影响的记录条数。

### 二十五、执行数据库命令，通常用于增、删、改操作
#### 定义：
public static int ExecuteNonQuery(DbTransaction transaction, CommandType commandType, string commandText, params DbParameter[] commandParams)
#### 参数：
transaction: 数据库事务对象。
<br>
commandType: 执行命令的类型。
<br>
commandText: 数据库命令。
<br>
commandParams: 数据库命令参数。
#### 返回：
执行数据库命令影响的记录条数。

### 二十六、执行数据库命令，通常用于聚合类查询，如Count、Max等
#### 定义：
public static object ExecuteScalar(string datasourceName, string commandText)
#### 参数：
datasourceName: 数据源名称。
<br>
commandText: 数据库命令。
#### 返回：
返回查询结果第一行第一列的值。

### 二十七、执行数据库命令，通常用于聚合类查询，如Count、Max等
#### 定义：
public static object ExecuteScalar(string datasourceName, string commandText, params DbParameter[] commandParams)
#### 参数：
datasourceName: 数据源名称。
<br>
commandText: 数据库命令。
<br>
commandParams: 数据库命令参数。
#### 返回：
返回查询结果第一行第一列的值。

### 二十八、执行数据库命令，通常用于聚合类查询，如Count、Max等
#### 定义：
public static object ExecuteScalar(string datasourceName, CommandType commandType, string commandText, params DbParameter[] commandParams)
#### 参数：
datasourceName: 数据源名称。
<br>
commandType: 执行命令的类型。
<br>
commandText: 数据库命令。
<br>
commandParams: 数据库命令参数。
#### 返回：
返回查询结果第一行第一列的值。

### 二十九、执行数据库命令，通常用于聚合类查询，如Count、Max等
#### 定义：
public static object ExecuteScalar(DbConnection connection, string commandText)
#### 参数：
connection: 数据库连接对象。
<br>
commandText: 数据库命令。
#### 返回：
返回查询结果第一行第一列的值。

### 三十、执行数据库命令，通常用于聚合类查询，如Count、Max等
#### 定义：
public static object ExecuteScalar(DbConnection connection, string commandText, params DbParameter[] commandParams)
#### 参数：
connection: 数据库连接对象。
<br>
commandText: 数据库命令。
<br>
commandParams: 数据库命令参数。
#### 返回：
返回查询结果第一行第一列的值。

### 三十一、执行数据库命令，通常用于聚合类查询，如Count、Max等
#### 定义：
public static object ExecuteScalar(DbConnection connection, CommandType commandType, string commandText, params DbParameter[] commandParams)
#### 参数：
connection: 数据库连接对象。
<br>
commandType: 执行命令的类型。
<br>
commandText: 数据库命令。
<br>
commandParams: 数据库命令参数。
#### 返回：
返回查询结果第一行第一列的值。

### 三十二、执行数据库命令，通常用于聚合类查询，如Count、Max等
#### 定义：
public static object ExecuteScalar(DbTransaction transaction, string commandText)
#### 参数：
transaction: 数据库事务对象。
<br>
commandText: 数据库命令。
#### 返回：
返回查询结果第一行第一列的值。

### 三十三、执行数据库命令，通常用于聚合类查询，如Count、Max等
#### 定义：
public static object ExecuteScalar(DbTransaction transaction, string commandText, params DbParameter[] commandParams)
#### 参数：
transaction: 数据库事务对象。
<br>
commandText: 数据库命令。
<br>
commandParams: 数据库命令参数。
#### 返回：
返回查询结果第一行第一列的值。

### 三十四、执行数据库命令，通常用于聚合类查询，如Count、Max等
#### 定义：
public static object ExecuteScalar(DbTransaction transaction, CommandType commandType, string commandText, params DbParameter[] commandParams)
#### 参数：
transaction: 数据库事务对象。
<br>
commandType: 执行命令的类型。
<br>
commandText: 数据库命令。
<br>
commandParams: 数据库命令参数。
#### 返回：
返回查询结果第一行第一列的值。

### 三十五、执行数据库命令，通常用于记录查询
#### 定义：
public static DbDataReader ExecuteDataReader(string datasourceName, string commandText)
#### 参数：
datasourceName: 数据源名称。
<br>
commandText: 数据库命令。
#### 返回：
数据读取器，DbDataReader对象

### 三十六、执行数据库命令，通常用于记录查询
#### 定义：
public static DbDataReader ExecuteDataReader(string datasourceName, string commandText, params DbParameter[] commandParams)
#### 参数：
datasourceName: 数据源名称。
<br>
commandText: 数据库命令。
<br>
commandParams: 数据库命令参数。
#### 返回：
数据读取器，DbDataReader对象

### 三十七、执行数据库命令，通常用于记录查询
#### 定义：
public static DbDataReader ExecuteDataReader(string datasourceName, CommandType commandType, string commandText, params DbParameter[] commandParams)
#### 参数：
datasourceName: 数据源名称。
<br>
commandType: 执行命令的类型。
<br>
commandText: 数据库命令。
<br>
commandParams: 数据库命令参数。
#### 返回：
数据读取器，DbDataReader对象

### 三十八、执行数据库命令，通常用于记录查询
#### 定义：
public static DbDataReader ExecuteDataReader(DbConnection connection, string commandText)
#### 参数：
connection: 数据库连接对象。
<br>
commandText: 数据库命令。
#### 返回：
数据读取器，DbDataReader对象

### 三十九、执行数据库命令，通常用于记录查询
#### 定义：
public static DbDataReader ExecuteDataReader(DbConnection connection, string commandText, params DbParameter[] commandParams)
#### 参数：
connection: 数据库连接对象。
<br>
commandText: 数据库命令。
<br>
commandParams: 数据库命令参数。
#### 返回：
数据读取器，DbDataReader对象

### 四十、执行数据库命令，通常用于记录查询
#### 定义：
public static DbDataReader ExecuteDataReader(DbConnection connection, CommandType commandType, string commandText, params DbParameter[] commandParams)
#### 参数：
connection: 数据库连接对象。
<br>
commandType: 执行命令的类型。
<br>
commandText: 数据库命令。
<br>
commandParams: 数据库命令参数。
#### 返回：
数据读取器，DbDataReader对象

### 四十一、执行数据库命令，通常用于记录查询
#### 定义：
public static DbDataReader ExecuteDataReader(DbTransaction transaction, string commandText)
#### 参数：
transaction: 数据库事务对象。
<br>
commandText: 数据库命令。
#### 返回：
数据读取器，DbDataReader对象

### 四十二、执行数据库命令，通常用于记录查询
#### 定义：
public static DbDataReader ExecuteDataReader(DbTransaction transaction, string commandText, params DbParameter[] commandParams)
#### 参数：
transaction: 数据库事务对象。
<br>
commandText: 数据库命令。
<br>
commandParams: 数据库命令参数。
#### 返回：
数据读取器，DbDataReader对象

### 四十三、执行数据库命令，通常用于记录查询
#### 定义：
public static DbDataReader ExecuteDataReader(DbTransaction transaction, CommandType commandType, string commandText, params DbParameter[] commandParams)
#### 参数：
transaction: 数据库事务对象。
<br>
commandType: 执行命令的类型。
<br>
commandText: 数据库命令。
<br>
commandParams: 数据库命令参数。
#### 返回：
数据读取器，DbDataReader对象
