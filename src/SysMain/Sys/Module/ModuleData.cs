using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Data.OracleClient;
using System.Data.SqlClient;
using System.Xml;
using Fan.DataBase;
using Fan.Common;

namespace GDBM
{
    public static class Mod
    {
        public static string m_SysXmlPath = Application.StartupPath + "\\..\\Res\\Xml\\SysXml.Xml";
        public static SystemTypeEnum m_SystemType = SystemTypeEnum.ManagerSystem;
        public readonly static string m_ResPath = Application.StartupPath + "\\..\\Res\\Pic";
        public readonly static string m_PluginFolderPath = Application.StartupPath + "\\..\\Plugins";
        public readonly static string m_LogPath = Application.StartupPath + "\\..\\Log\\ϵͳ��ʼ��";
        public static string filestr = Application.StartupPath + "\\dbSet.txt";
        /// <summary>
        /// ҵ���������
        /// </summary>
        public static IDBOperate m_SysDbOperate = null;
        public static User m_LoginUser = null;
        public static bool LoginState = false;//�ؼ���½�������Ƿ���ʾ������

        public static Fan.Plugin.Application.AppForm v_AppForm;

        public readonly static string v_AppDBConectXml = Application.StartupPath + "\\AppDBConectInfo.xml";////////////ϵͳά���������ַ��� 
        /////guozheng 2011-2-14 added ϵͳά������嶨��ģ���ļ�
        public static string v_SystemFunctionDBSchema = Application.StartupPath + "\\..\\Res\\Schema\\SystemFunctionDBConfiguration.sql";
        //������Ϣ
        //cyf 20110602 add:�����û���Ӧ�Ľ�ɫ��Ϣ�б�
        //end
        public static XmlDocument v_SystemXml;

        public static Fan.Plugin.Parse.PluginCollection m_PluginCol;      //����ڴ�

        ///////////////////////////////////////ϵͳ������־
        public static Fan.Common.Log.clsWriteSystemFunctionLog v_SysLog;

        public static string v_UserInfoPath = Application.StartupPath + "\\user.dat";//added by chulili 20110707 ��ס�����ļ�

        //��ʱ��������Ϣ
        public static ESRI.ArcGIS.Geodatabase.IWorkspace TempWks = null;
        public static string v_ConfigPath = Application.StartupPath + "\\conn.dat";//�Ͼ���Ŀϵͳά�������������ļ�
        public static string Server = "";
        public static string Instance = "";
        public static string Database = "";
        public static string User = "";
        public static string Password = "";
        public static string Version = "";
        public static string dbType = "";

        //��ʽ��������Ϣ
        public static ESRI.ArcGIS.Geodatabase.IWorkspace CurWks = null;
        public static string CurServer = "";
        public static string CurInstance = "";
        public static string CurDatabase = "";
        public static string CurUser = "";
        public static string CurPassword = "";
        public static string CurVersion = "";
        public static string CurdbType = "";

        public static List<string> v_ListUserPrivilegeID;
        public static List<string> v_ListUserdataPriID;
    }

    // *=======================================================
    // *�����ߣ����Ƿ�
    // *ʱ  �䣺20110520
    // *��  �ܣ������û�ö������
    public enum UserTypeEnum
    {
        SuperAdmin=0,          //��������Ա�����������û�Ȩ�ޣ������û���
        Admin=1,               //����Ա�����Ƚ��뼯�ɹ���ϵͳ
        CommonUser=2           //��ͨ�û�
    }
    //===================================================
    // *=======================================================
    // *�����ߣ�������
    // *ʱ  �䣺20120809
    // *��  �ܣ�������ϵͳö������
    public enum SystemTypeEnum
    {
        ManagerSystem = 0,          //����չʾ��ϵͳ
        ConfigSystem = 1,               //������ϵͳ
        UpdateSystem = 2           //��������ϵͳ
    }
    //===================================================
}
