//*********************************************************************************
//** �ļ�����ModTableFun.cs
//** CopyRight (c) 2000-2007 �人������Ϣ���̼������޹�˾���̲�
//** �����ˣ�chulili
//** ��  �ڣ�2011-03
//** �޸��ˣ�
//** ��  �ڣ�
//** ��  ���������ص�һЩ�������� 
//**
//** ��  ����1.0
//*********************************************************************************
using System;
using System.Collections.Generic;
using System.Text;

using System.Data.OleDb;
namespace GeoHistory
{
    class ModTableFun
    {
        //�������ܣ���ȡ�α�
        //������������ݿ�����  sql��� //�������������sql���򿪵ı���α�
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
        //�������ܣ�ɾ����   ������������ݿ�����  ����  �����������
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
        //�������ܣ��жϱ��Ƿ����  
        //������������ݿ����� ����  ���������������
        public static bool isExist(OleDbConnection conn,string TableName)
        {
            OleDbCommand comm = conn.CreateCommand();
            comm.CommandText     = "select * from " + TableName + " where 1=0";
            OleDbDataReader myreader;
            //���ݴ��󱣻��жϱ��Ƿ����
            try
            {
                myreader = comm.ExecuteReader();
                myreader.Close();
                return true;
            }
            //�������ʾ������
            catch(System.Exception e)
            {
                e.Data.Clear();
                return false;
            }

        }
        //�жϱ����Ƿ����ĳ���ֶΡ�
        //������������ݿ�����  ����  �ֶ���  ���������������

        public static bool isFieldExist(OleDbConnection conn, string TableName, string FieldName)
        {
            OleDbCommand comm = conn.CreateCommand();
            comm.CommandText = "select " +FieldName+" from " + TableName + " where 1=0";
            OleDbDataReader myreader;
            //���ݴ��󱣻��ж�ĳ���ֶ��Ƿ����
            try
            {
                myreader = comm.ExecuteReader();
                myreader.Close();
                return true;
            }
            //�������ʾ����ֶβ�����
            catch (System.Exception e)
            {
                e.Data.Clear();
                return false;
            }
        }
    }
}
