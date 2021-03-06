//*********************************************************************************
//** 文件名：ModTableFun.cs
//** CopyRight (c) 2000-2007 武汉吉奥信息工程技术有限公司工程部
//** 创建人：chulili
//** 日  期：2011-03
//** 修改人：
//** 日  期：
//** 描  述：与表相关的一些公共函数 
//**
//** 版  本：1.0
//*********************************************************************************
using System;
using System.Collections.Generic;
using System.Text;

using System.Data.OleDb;
namespace GeoHistory
{
    class ModTableFun
    {
        //函数功能：获取游标
        //输入参数：数据库连接  sql语句 //输出参数：根据sql语句打开的表的游标
        public static OleDbDataReader GetReader(OleDbConnection conn, string sqlstr)
        {
            OleDbCommand comm = conn.CreateCommand();
            comm.CommandText = sqlstr;
            OleDbDataReader myreader;
            try
            {
                myreader = comm.ExecuteReader();
                return myreader;
            }
            catch (System.Exception e)
            {
                e.Data.Clear();
                return null;
            }
        }
        //函数功能：删除表   输入参数：数据库连接  表名  输出参数：无
        public static void DropTable(OleDbConnection conn,string TableName)
        {
            if (isExist(conn, TableName))
            {
                try
                {
                    OleDbCommand mycomm = conn.CreateCommand();
                    mycomm.CommandText = "drop table " + TableName;
                    mycomm.ExecuteNonQuery();
                }
                catch
                { }
            }
        }
        //函数功能：判断表是否存在  
        //输入参数：数据库连接 表名  输出参数：布尔型
        public static bool isExist(OleDbConnection conn,string TableName)
        {
            OleDbCommand comm = conn.CreateCommand();
            comm.CommandText     = "select * from " + TableName + " where 1=0";
            OleDbDataReader myreader;
            //根据错误保护判断表是否存在
            try
            {
                myreader = comm.ExecuteReader();
                myreader.Close();
                return true;
            }
            //报错则表示不存在
            catch(System.Exception e)
            {
                e.Data.Clear();
                return false;
            }

        }
        //判断表中是否存在某个字段】
        //输入参数：数据库连接  表名  字段名  输出参数：布尔型

        public static bool isFieldExist(OleDbConnection conn, string TableName, string FieldName)
        {
            OleDbCommand comm = conn.CreateCommand();
            comm.CommandText = "select " +FieldName+" from " + TableName + " where 1=0";
            OleDbDataReader myreader;
            //根据错误保护判断某个字段是否存在
            try
            {
                myreader = comm.ExecuteReader();
                myreader.Close();
                return true;
            }
            //报错则表示这个字段不存在
            catch (System.Exception e)
            {
                e.Data.Clear();
                return false;
            }
        }
    }
}
