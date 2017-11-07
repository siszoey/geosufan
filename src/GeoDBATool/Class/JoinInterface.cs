using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Carto;

namespace GeoDBATool
{
    public interface IMapframe////ͼ����ϱ�
    {
        #region ����ͼ����ϱ�ͼ��
        IFeatureClass MapFrameFea { get; set; }
        #endregion

        #region ���ô��ӱ�Դͼ��
        IFeature OriMapFrame { get; set; }
        #endregion

        #region ��ȡԴͼ��
        IFeature GetOriFrame(string OriFrameNoValue, string OriFrameNoField);
        #endregion

        #region ��ȡԴͼ����Ŀ��ͼ���Ĵ��ӱ߻��巶Χ
        void GetBufferArea(double dBufferRadius, out IGeometry OriBufferArea, out IGeometry DesBufferArea);
        #endregion

        #region ��ȡԴͼ������ӱߵı߽�
        List<IPolyline> Getborderline();
        #endregion
    }


    public interface IDestinatDataset/////Ҫ���нӱߵ�ͼ�����
    {
        bool IsStandardMapFrame { get; set; }////�Ƿ��Ǳ�׼ͼ���ӱ�       

        bool IsRemoveRedundantPnt { get; set; }/////�Ƿ�ɾ��������϶����

        bool IsGeometrySimplify { get; set; }//////�Ƿ����Ҫ�صļ򵥻�����Ծ��ж��Geometry��Ҫ�أ�

        double Angle_to { get; set; }/////�Ƕ��ݲ�

        #region ���ò���ӱߵ�ͼ��
        List<IFeatureClass> JoinFeatureClass { get; set; }
        #endregion

        #region ������ȡͼ��
        IFeatureClass TargetFeatureClass(string FeatureClassName);////������ȡͼ��
        #endregion

        #region ��ȡ���巶Χ�ڵĽӱ�Ҫ��
        Dictionary<string, List<long>> GetFeaturesByGeometry(IGeometry BufferArea, bool isOriArea);
        #endregion


    }

    public interface ICheckOperation/////�ӱ߲���
    {
        #region ������Ҫ�ӱߵ�ͼ��
        IFeatureClass DestinatFeaCls { get; set; }
        #endregion

        bool CreatLog { get; set; }

        List<IPolyline> borderline { get; set; }/////�ӱ߽߱�

        List<long> OriFeaturesOID { get; set; }////���ӱ�ԴҪ�ؼ�

        List<long> DesFeaturesOID { get; set; }////���ӱ�Ŀ��Ҫ�ؼ�

        List<string> FieldsControlList { get; set; }////////���������ֶ�

        double Dis_Tolerance { get; set; }////�����ݲ�

        double Angel_Tolerrance { get; set; }////�Ƕ��ݲ�

        double Length_Tolerrance { get; set; }////�����ݲ�

        double Search_Tolerrance { get; set; }////�����ݲ�

        IGeometry OriBufferArea { get; set; }////Դͼ���Ļ��巶Χ

        IGeometry DesBufferArea { get; set; }////Ŀ��ͼ���Ļ��巶Χ

        esriGeometryType GetDatasetGeometryType();////��ȡ��Ҫ�ӱߵ�ͼ��ļ�������

        DataTable GetPolylineDesFeatureOIDByOriFeature();////��ȡ���ӱ�Ŀ��Ҫ�ؼ���Ҫ��ԴҪ�ؽӱߵ�Ҫ��

        DataTable GetPolygonDesFeatureOIDByOriFeature();



        //bool PolylineFilter(IPolyline polyline, bool IsOri);////�ж�һ�����Ƿ�����ӱ�����������˵㲻�ڽӱ߻������������յ������ȣ�




    }

    public interface IJoinOperation
    {
        bool CreatLog { get; set; }
        List<IFeatureClass> JoinFeaClss { get; set; }
        List<IPolyline> borderline { get; set; }/////�ӱ߽߱�
        DataTable MovePolylinePnt(DataTable OIDInfo);/////�ƶ����ϵĵ���нӱ�
        DataTable MovePolygonPnt(DataTable OIDInfo);//////�ƶ�������ϵĵ���нӱ�
    }

    public interface IMergeOperation/////�ںϲ���
    {
        bool CreatLog { get; set; }
        #region ������Ҫ�ںϵ�ͼ��
        List<IFeatureClass> JoinFeaClss { get; set; }
        #endregion

        #region �Ƿ񽫴��ӱ�Ŀ��Ҫ�ص�������Ϣ����ԴҪ�ص�������Ϣ
        bool SetDesValueToOri { get; set; }
        #endregion

        bool MergePolyline(string DataSetName, long OriOID, long DesOID);
        bool MergePolygon(string DataSetName, long OriOID, long DesOID);
    }



    public interface IJoinLOG/////�¼�
    {

        void InitialLog(out Exception ex);

        void onDataJoin_JoinDataSet(int state, List<IFeatureClass> DataSetList, out Exception ex);

        void onDataJoin_Start(int state, out Exception ex);

        void onDataJoin_Terminate(int state, out Exception ex);

        void OnDataJoin_OnCheck(DataRow in_DataRow, double x, double y, out Exception ex);

        void OnDataJoin_OnJoin(DataRow in_DataRow, double x, double y, out Exception ex);

        void OnDataJoin_OnMerge(DataRow in_DataRow, double x, double y, out Exception ex);

    }


}
