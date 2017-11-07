using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using SysCommon.Gis;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Carto;

namespace GeoSysUpdate
{
    public enum enumOperatorType
    {
        Null = -1,
        Stract = 1,
        Output = 2,
        Input = 3,
        Update = 4,
        Validate = 5,
        Submit = 6
    }

    public enum enumLayType
    {
        LEFT = 1,
        RIGHT = 2,
        TOP = 3,
        BOTTOM = 4,
        FILL = 5,
    }

    public enum enumTaskType
    {
        MANAGER=1,
        ZONE=2,
        INTERZONE=3,
        CHECK=4,
        SPOTCHECK=5
    }

    public enum enumProjectState
    {
        δ���=0,
        ���=1
    }

    public enum enumFeatureUpdate
    {
        ���� = 1,
        ɾ��= 2,
        �޸�=3
    }

    public enum enumZoneState
    {
        �� = 0,
        �����ѷ��� = 1,
        ���������� = 2,
        ���ύ�������� = 3,
        ���δͨ�� = 4,
        ���ͨ�� = 5,
        �Ѿ��ύ�����ƿ� = 6
    }

    public enum enumInterZoneState
    {
        �� = 0,
        �����ѷ��� = 1,
        �����ޱ� = 2,
        ���δͨ�� = 3,
        ����ޱ� = 4
    }

    public enum enumMapState
    {
        �� = 0,
        �����ޱ� = 1,
        ���δͨ�� = 2,
        ����ޱ� = 3
    }

    public enum enumMetaType
    {
        ProjectInfo,
        Zone,
        InterZone,
        Map,
        FeatureUpdate
    }

    public static class ModData
    {
        public static string _XZQpath = Application.StartupPath + "\\..\\Res\\Xml\\XZQ.xml";
        public readonly static string v_CachedDBPath = Application.StartupPath + "\\..\\CacheDB\\map.mdb";
        public readonly static string v_LogDBPath = Application.StartupPath + "\\..\\CacheDB\\Log.mdb";
        public readonly static string v_projectDetalXML = Application.StartupPath + "\\DatabaseDetalProject.xml";

        public static Plugin.Application.AppGidUpdate v_AppGisUpdate;
        public static IFeatureClass v_CurMapSheetFeatureClass;

        //�����λ��
        public static ESRI.ArcGIS.Geometry.IPoint v_CurPoint;
        //���ݲ�����(����)
        public static SysGisDataSet v_SysDataSet;

        //���ݲ�����(��)
        public static SysGisDataSet v_SysSet;

        //���ݲ�����(ͼ��Ԫ����)
        public static SysGisTable v_SysMapTable;

        //��¼���ݲ���ʱ��
        public static DateTime v_OperatorTime;

        //�Ƿ���Ҫ����ˢ�¸��¶Ա��б�
        public static bool v_RefreshUpdateDataInfo;

        //��ǰͼ��������
        public static string Scale;

        //��ǰ��������
        public static enumTaskType v_CurrentTaskType;


        //ͼ�������ͼ
        public static System.Xml.XmlDocument v_DataViewXml;

        //*****************************************************************************************
        // System Function Log
        public static SysCommon.Log.clsWriteSystemFunctionLog SysLog;
        //*****************************************************************************************
        //������Ϣ
        public static string v_ConfigPath = Application.StartupPath + "\\conn.dat";
        public static string dbType = "";
        public static string Server = "";
        public static string Instance = "";
        public static string Database="";
        public static string User = "";
        public static string Password = "";
        public static string Version = "";
     }
    public static class ModFrameData
    {
        public static Plugin.Application.AppGidUpdate v_AppGisUpdate;
        public static IFeatureWorkspace m_pFeatureWorkspace;
        //?  public static IFeatureClass v_CurMapSheetFeatureClass;
        //?  public static frmQueryOperationRecords v_QueryResult;

        //�����λ��
        public static ESRI.ArcGIS.Geometry.IPoint v_CurPoint;

        //������Ϣ
        public static string v_ConfigPath = Application.StartupPath + "\\conn.dat";
        public static string dbType = "";
        public static string Server = "";
        public static string Instance = "";
        public static string Database = "";
        public static string User = "";
        public static string Password = "";
        public static string Version = "DATACENTER.DEFAULT";

        //======================================================
        //ϵͳ������־  cyf 20110518
        public static SysCommon.Log.clsWriteSystemFunctionLog v_SysLog;
        public static string m_SysXmlPath = Application.StartupPath + "\\..\\Res\\Xml\\SysXml.Xml";                 //����ϵͳ����xml
        //ϵͳά����������Ϣ cyf 20110520
        public readonly static string v_AppDBConectXml = Application.StartupPath + "\\AppDBConectInfo.xml";////////////ϵͳά���������ַ��� 
        //end=======================================================
    }

}
