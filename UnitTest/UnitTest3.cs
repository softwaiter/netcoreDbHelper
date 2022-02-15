using CodeM.Common.DbHelper;
using NUnit.Framework;
using System;
using System.Data.Common;

namespace UnitTest
{
    /// <summary>
    /// Mysql数据库测试
    /// </summary>
    public class UnitTest3
    {
        public UnitTest3()
        {
            DbUtils.RegisterDbProvider("mysql", "MySql.Data.MySqlClient.MySqlClientFactory, MySql.Data");
            DbUtils.AddDataSource("mysql_test", "mysql", "Data Source=localhost;Database=test;User Id=root;Password=root;");
        }

        [Test]
        public void Order1_CreateTable()
        {
            string sql = "Create Table test(id int not null primary key, name varchar(64), age int, address varchar(255))";
            int result = DbUtils.ExecuteNonQuery("mysql_test", sql);
            Assert.IsTrue(result == 0);
        }

        [Test]
        public void Order2_InsertRecord()
        {
            string sql = "Insert Into test (id, name, age, address) Values(1, 'wangxm', 18, '河北保定')";
            int result = DbUtils.ExecuteNonQuery("mysql_test", sql);
            Assert.IsTrue(result == 1);
        }

        [Test]
        public void Order3_InsertRecord2()
        {
            try
            {
                string sql = "Insert Into test (id, name, age, address) Values(1, 'wangxm', 18, '河北保定')";
                int result = DbUtils.ExecuteNonQuery("mysql_test", sql);
                Assert.Fail("Insert Error.");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.Message.Contains("Duplicate"));
            }
        }

        [Test]
        public void Order4_QueryRecord()
        {
            string sql = "Select * From test Where id=1";
            DbDataReader dr = DbUtils.ExecuteDataReader("mysql_test", sql);
            Assert.IsTrue(dr.HasRows);
            Assert.IsTrue(dr.Read());
            string name = dr.GetString(1);
            Assert.AreEqual(name, "wangxm");
            dr.Close();
        }

        [Test]
        public void Order5_DropTable()
        {
            string sql = "Drop Table test";
            int result = DbUtils.ExecuteNonQuery("mysql_test", sql);
            Assert.IsTrue(result == 0);
        }
    }
}
