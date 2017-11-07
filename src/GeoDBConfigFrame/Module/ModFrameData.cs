using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using SysCommon.Gis;

namespace GeoDBConfigFrame
{
    public static class ModFrameData
    {
        public static Plugin.Application.AppPrivileges v_AppPrivileges;
        public static string v_ConfigPath = Application.StartupPath + "\\conn.dat";

        //��¼���ݲ���ʱ��
        public static DateTime v_OperatorTime;
        //���ݿ������
        public static SysGisDB gisDb;

        //======================================================
        //ϵͳ������־  cyf 20110518
        public static SysCommon.Log.clsWriteSystemFunctionLog v_SysLog;
        public static string m_SysXmlPath = Application.StartupPath + "\\..\\Res\\Xml\\SysXml.Xml";                 //����ϵͳ����xml
        //ϵͳά����������Ϣ cyf 20110520
        public readonly static string v_AppDBConectXml = Application.StartupPath + "\\AppDBConectInfo.xml";////////////ϵͳά���������ַ��� 
        //end=======================================================
    }

    public enum enumLayType
    {
        LEFT = 1,
        RIGHT = 2,
        TOP = 3,
        BOTTOM = 4,
        FILL = 5,
    }

    public static class SdeConfig
    {
        public static string dbType = "";
        public static string Server="";
        public static string Instance="";
        public static string Database="";
        public static string User="";
        public static string Password="";
        public static string Version="SDE.DEFAULT";
    }    
}
