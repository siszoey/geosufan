using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Carto;
using System.Collections;
using System.Xml;
using System.Drawing;
using System.Data;

using ESRI.ArcGIS.Controls;


namespace GeoDBATool
{
    public static class ModData
    {
        public static Plugin.Application.AppGIS v_AppGIS;
        public static int recNum = 0;//��һ��ҳ������һ����¼
        public static int CurrentPage = 1;//��ǰҳ��
        public static int TotalPageCount;//��ҳ��
        public static int pageSize = 50;//ÿҳ�����ļ�¼��
        public static DataTable TotalTable = null;//���¶Ա��б���ʾԴ��

        public readonly static string v_projectXMLTemp = Application.StartupPath + "\\..\\Res\\Schema\\DatabaseProject.xml";
        public readonly static string v_projectXML = Application.StartupPath + "\\DatabaseProject.xml";
        //cyf 20110624 20110624 add:��ʼ��������ͼ��xml
        public readonly static string v_projectDetalXMLTemp = Application.StartupPath + "\\..\\Res\\Schema\\DatabaseDetalProject.xml";
        public readonly static string v_projectDetalXML = Application.StartupPath + "\\DatabaseDetalProject.xml";
        //end
        public readonly static string v_DbInfopath = Application.StartupPath + "\\..\\Template\\DbInfoTemplate.mdb";

        public readonly static string v_JoinSettingXML = Application.StartupPath + "\\MapFrameJoinSetting.xml";
        public readonly static string v_JoinLOGXML = Application.StartupPath + "\\MapFrameJoinLOGTemplet.xml";

        public static string MapPath = Application.StartupPath + @"\..\CacheDB\Map.mdb";
        public static string countyPath = Application.StartupPath + @"\..\CacheDB\Range.mdb";

        //Эͬ����Զ����־��
        public static string netLogFile = Application.StartupPath + @"\..\Template\Network_Log.mdb";

        public static string DBImportPath = Application.StartupPath + @"\..\Res\rule\������ֲ\�����������.xml";  //��������������

        //ͼ���������ݴ����xmlӳ���ϵ
        public static string DBTufuInputXml = Application.StartupPath + @"\..\Res\rule\������ֲ\ͼ�������빤����.xml";  //ͼ�������빤�������
        public static string DBTufuStractXml = Application.StartupPath + @"\..\Res\rule\������ֲ\ͼ���ӱ�������ȡ.xml";   //��ȡͼ���ӱ�����
        public static string DBTufuSubmitXml = Application.StartupPath + @"\..\Res\rule\������ֲ\ͼ�����ݸ������.xml";   //��ȡͼ���ӱ�����

        public static string temporaryDBPath = Application.StartupPath + @"\..\CacheDB\tempRasterDtDB.gdb";            //��ʱդ���������ݿ�洢·��
        //cyf 20110609 add
        public static string RasterInDBTemp = Application.StartupPath + @"\..\Template\RasterInDBTemp.mdb";            //�洢����դ�����ݵ�ģ��
        public static string RasterInDBLog = Application.StartupPath + "\\RasterInDBLog.mdb";                        //�洢����դ������·��
        //end
        public static string v_DbInterConn = Application.StartupPath + "\\AppDBConectInfo.xml";    //ϵͳά���������ַ���

        public static Dictionary<IFeatureLayer, IFeatureRenderer> m_DicFeaLayerRender = new Dictionary<IFeatureLayer, IFeatureRenderer>();  //������Ⱦ�ķ���

        //������־��Ͱ汾��Ϣ��  һ���Ը���
        public static string m_sUpDataLOGTable = "GO_DATABASE_UPDATELOG";///////Զ�̸�����־��
        public static string m_sDBVersionTable = "GO_DATABASE_VERSION";/////////���ݿ�汾��
        public static int DBVersion = 0;          //�汾

        public static IWorkspace m_ObjWS;                           //Ŀ�깤���ռ�
        public static IFeature m_OrgFeature;                        //����Ҫ��
        public static IMap m_orgMap;                                //����ͼ��

        //public static ComboBox m_ComboBox;                                        //ͼ���б��

        public static ILayer m_CurLayer;                                          //��ǰͼ��

        public static EnumUpdateType m_CurOperType;                            //��ǰ��������

        public static AxMapControl m_Mapcontrol;                                //�Ӵ��ڿؼ�

        //*****************************************************************************************
        // System Function Log
        public static SysCommon.Log.clsWriteSystemFunctionLog SysLog;
        //*****************************************************************************************



        //��ʱ��������Ϣ
        public static ESRI.ArcGIS.Geodatabase.IWorkspace TempWks = null;
        public static string v_ConfigPath = Application.StartupPath + "\\conn.dat";//ϵͳά�������������ļ�
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
        //end   added by chulili 20110531


        ////////һ���Ը��¶Աȴ���
        public static frmCompareData UpDataCompareFrm;

        //guozheng 2011-5-6 ���·������
        //�������±��� Զ�̷���
        public static CSendUpdateMsg v_RemoteMsg = null;       //����ͨѶ
        public static GeoMsgCenterCOMLib.MessageSession v_RemoteSesion = null; //session
        public static CSendUpdateMsg v_Msg = null;       //����ͨѶ
        public static GeoMsgCenterCOMLib.MessageSession v_Sesion = null; //session


        //��ʱ��������Ϣ
        //public static ESRI.ArcGIS.Geodatabase.IWorkspace TempWks = null;
        //public static string v_ConfigPath = Application.StartupPath + "\\conn.dat";//ϵͳά�������������ļ�
        //public static string Server = "";
        //public static string Instance = "";
        //public static string Database = "";
        //public static string User = "";
        //public static string Password = "";
        //public static string Version = "";
        //public static string dbType = "";
  
    }
    public enum EnumOperateType
    {
        NULL=0,
        Input = 1,
        Stract = 2,
        Submit=3,
        OutputUpdateData=4,
        UserDBInput=5
    }
    public enum enumLayType
    {
        LEFT = 1,
        RIGHT = 2,
        TOP = 3,
        BOTTOM = 4,
        FILL = 5,
    }

    public enum enumInterDBFormat///////���ݿ�ƽ̨����
    {
        ARCGISPDB = 1,
        ARCGISGDB = 2,
        ARCGISSDE = 3,
        ORACLESPATIAL = 4,
        GEOSTARACCESS = 5,
        GEOSTARORACLE = 6,
        GEOSTARORSQLSERVER = 7,
        FTP = 8
    }

    public enum enumInterDBType///////���ݿ�����
    {
        �ɹ��ļ����ݿ� = 1,
        ���Ҫ�����ݿ� = 2,
        �߳����ݿ� = 3,
        Ӱ�����ݿ� = 4,
        �������ݿ� = 5,
        ����������ݿ� = 6,
        ���ӵ�ͼ���ݿ� = 7,
        ר��Ҫ�����ݿ� = 8
    }

    public enum EnumFeatureType
    {
        ����Ҫ�� = 1,  //����Ҫ��
        ����Ҫ�� = 2  //��Ҫ����Ҫ��
    }

    public enum EnumUpdateType
    {
        ���� = 1,           //����
        �޸� = 2,           //�޸�
        ɾ�� = 3            //ɾ��
    }

    //cyf 20110602 modify:�����ɫ����
    public enum EnumRoleType
    {
        ����Ա = 1,    //����Ա
        ��ҵԱ = 2,
        ��ͨ�û� = 3,
        �ʼ�Ա = 4,
    }

    public enum EnumRasterOperateType
    {
        Input=1,            //���
        Update=2             //����
    }
       
}
