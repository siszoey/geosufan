using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.Collections;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.SystemUI;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;

namespace GeoEdit
{
    /// <summary>
    /// �༭ʱ��״̬ ���༭�ռ�Ķ���
    /// </summary>
    public static class MoData
    {
        public static IWorkspaceEdit v_CurWorkspaceEdit;//����һ��ȫ�ֵı༭�ռ�

        public static SysCommon.DataBase.SysTable v_LogTable;//��־��¼��

        public static ILayer v_CurLayer;               //��ǰ����ͼ��

        public static bool v_bVertexSelectionTracker;  //�жϽڵ�༭�е�ѡ��ڵ㣬����ˢ�»��ڵ�

        public static bool v_bSnapStart;               //�Ƿ�����׽
        public static double v_SearchDist;             //��׽�뾶
        public static double v_CacheRadius;            //����뾶
        public static Dictionary<ILayer, ArrayList> v_dicSnapLayers; //�洢��׽��ͼ�㼰����(�ڵ㲶׽���˵㲶׽���ཻ�㲶׽���е㲶׽������㲶׽)

        public static Hashtable GetOnlyReadAtt = new Hashtable();//�����޸�����
        public static int DBVersion = 0;
    }


    /// <summary>
    /// �����������Ա༭��ʾʱ��״̬
    /// </summary>
    public static class AttributeShow_state
    {
        public static Hashtable hs_Feature;//�����ǵ�ԴҪ�����Դ�����
        public static string OID = "";//��ԴOID������
        public static int feature_count = 0;//Ҫ���ֶθ�����ȷ��һ��Դ������ĸ�������һ���ġ�
        public static bool state_brush = false;//����һ��״̬������ˢʹ��
        public static bool show_state = false;//һ��ʼ�ǲ���ʾ��
        public static bool state_value = false;//ȷ����ͼ���Ƿ���ѡ�е�ֵ
        public static bool doubleclick = false;//��ͼѡ�񼯵�״̬���Ƿ��иı�
        public static FrmAttribute Temp_frm;//һ����ʱ����������ԵĴ���
        public static FrmAttribute4Merge Temp_frm4Merge;//һ����ʱ����������ԵĴ���

    }
}
