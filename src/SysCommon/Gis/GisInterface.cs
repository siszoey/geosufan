using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Collections;

using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;

namespace Fan.Common.Gis
{
    public interface IGisDB
    {
        #region ��ȡ�����ռ�
        IWorkspace WorkSpace { get;set;}
        #endregion

        #region ���ù����ռ�

            #region �칹 �������ӹ����ռ�
            //SDE����
            bool SetWorkspace(String sServer, String sService, String sDatabase, String sUser, String sPassword,string strVersion,out Exception eError);
            //FDB��PDB����
            bool SetWorkspace(string sFilePath, Fan.Common.enumWSType wstype, out Exception eError);
            #endregion
        #endregion

        #region �������رձ༭
        bool StartWorkspaceEdit(bool withunodredo, out Exception eError);
        bool EndWorkspaceEdit(bool saveedits, out Exception eError);
        #endregion
    }

    public interface IGisDataSet
    {
        #region ����
        //����DEM���ݼ�
        bool CreateDEMDataSet(string name, int vCompressionType, int CompressionQuality, int vResamplingTypes, int vTileHeight, int vTileWidth, out Exception eError);
        //����DOM���ݼ�
        bool CreateDOMDataSet(string name, out Exception eError);
        #endregion

        #region ������
        //��ȡҪ�ؼ�FeatureClass
        IFeatureClass GetFeatureClass(string feaclassname, out Exception eError);
        //��ȡҪ�ؼ�FeatureDataset
        IFeatureDataset GetFeatureDataset(string featuredsname, out Exception eError);
        //��ȡ�����ռ���ָ���������ݼ�����
        List<string> GetDatasetNames(IWorkspace pWorkspace, esriDatasetType aDatasetTyp);
        //��ȡ���ݿ���ȫ����RD����
        List<string> GetAllRDNames();
        //��ȡ���ݿ���ȫ����RC����
        List<string> GetAllRCNames();
        //��ȡ���ݿ���ȫ����FD����
        List<string> GetAllFeatureDatasetNames();
        //��ȡ���ݿ���ȫ����FC����
        List<string> GetAllFeatureClassNames(bool bFullName);

        #region �칹 ��ȡFC����
        //��ȡ���ݿ���ȫ������ɢFC����
        List<string> GetFeatureClassNames();
        //��ȡĳһҪ�ؼ�����FC����
        List<string> GetFeatureClassNames(string fdname, bool bFullName, out Exception eError);
        List<string> GetFeatureClassNames(IFeatureDatasetName pFeaDsName, bool bFullName);
        #endregion

        #region �칹 ��ȡҪ�ؼ���FeatureCursor
        //����������ȡ
        IFeatureCursor GetFeatureCursor(string featureclassname, string condition, IGeometry pGeometry, esriSpatialRelEnum pSpatialRel, out Exception eError);
        //����������ȡ,�����ȡҪ������
        IFeatureCursor GetFeatureCursor(string featureclassname, string condition, IGeometry pGeometry, esriSpatialRelEnum pSpatialRel, out Exception eError, out int count);
        //����������ȡ,�����ȡҪ��������FC��Ҫ������
        IFeatureCursor GetFeatureCursor(string featureclassname, string condition, IGeometry pGeometry, esriSpatialRelEnum pSpatialRel, out Exception eError, out int count, out int total);
        IFeatureCursor GetFeatureCursor(string featureclassname, string condition, IGeometry pGeometry, string strSpatialRel, out Exception eError, out int count, out int total);

        #endregion

        #region �칹 ��ȡָ��Ҫ��

        IFeature GetFeature(string featureclassname, string condition, IGeometry pGeometry, esriSpatialRelEnum pSpatialRel, out Exception eError);
        IFeature GetFeature(string featureclassname, string condition, IGeometry pGeometry, string strSpatialRel, out Exception eError);
        IFeature GetFeature(IFeatureClass pFeatCls, string condition, IGeometry pGeometry, esriSpatialRelEnum pSpatialRel, out Exception eError);
        IFeature GetFeature(IFeatureClass pFeatCls, string condition, IGeometry pGeometry, string strSpatialRel, out Exception eError);

        #endregion

        //������ݼ�FeatureClass�Ƿ����
        bool CheckFeatureClassExist(string feaclassname, out string FeatureType, out Exception eError);
        //������ݼ�Dataset�Ƿ����
        bool CheckDatasetExist(string featuredsname, esriDatasetType aDatasetTyp);
        #endregion

        #region д����
        #region �칹 ����DOM����(RC�ļ�)����
        bool InputDOMData(string RCDatasetName, List<string> filepaths, out Exception eError);
        bool InputDOMData(string RCDatasetName, string filepaths, out Exception eError);
        #endregion

        #region �칹 ����DEM����(RD�ļ�)����
        bool InputDEMData(string RDDatasetName, List<string> filepaths, out Exception eError);
        bool InputDEMData(string RDDatasetName, string filepaths, out Exception eError);
        #endregion

        #region �칹 �½�Feature
        bool NewFeature(string objfcname, Dictionary<string, object> values, IGeometry geomtry, bool Edit, out Exception eError);
        bool NewFeature(string objfcname, Dictionary<string, object> values, Dictionary<int, string> dicCoor, bool Edit, out Exception eError);
        bool NewFeature(IFeatureClass objfc, Dictionary<string, object> values, IGeometry geomtry, bool Edit, out Exception eError);
        bool NewFeature(IFeatureClass objfc, Dictionary<string, object> values, Dictionary<int, string> dicCoor, bool Edit, out Exception eError);
        bool NewFeatures(IFeatureClass objfc, IFeatureCursor pfeacursor, Dictionary<string, object> values, bool Edit, bool bIngore, out Exception eError);
        bool NewFeatures(IFeatureClass objfc, IFeatureCursor pfeacursor, List<string> FieldNames, Dictionary<string, object> values, bool Edit, bool bIngore, out Exception eError);
        bool NewFeatures(IFeatureClass objfc, IFeatureCursor pfeacursor, Dictionary<string, string> dicFieldsPair, Dictionary<string, object> values, bool Edit, bool bIngore, out Exception eError);
        bool NewFeatures(string objfcname, IFeatureCursor pfeacursor, Dictionary<string, object> values, bool useOrgFdVal, bool Edit, bool bIngore, out Exception eError);
        bool NewFeatures(string objfcname, IFeatureCursor pfeacursor, List<string> FieldNames, Dictionary<string, object> values, bool Edit, bool bIngore, out Exception eError);
        bool NewFeatures(string objfcname, IFeatureCursor pfeacursor, Dictionary<string, string> dicFieldsPair, Dictionary<string, object> values, bool Edit, bool bIngore, out Exception eError);
        #endregion

        #region �칹 �༭Feature
        bool EditFeature(string objfcname, string condition, Dictionary<string, object> values, Dictionary<int, string> dicCoor, bool Edit, out Exception eError);
        bool EditFeature(string objfcname, string condition, Dictionary<string, object> values, IGeometry geomtry, bool Edit, out Exception eError);
        bool EditFeatures(string objfcname, string condition, Dictionary<string, object> dicValues, bool bEdit, out Exception eError);
        #endregion
        #endregion
    }

    public interface IGisLayer
    {
    }

    public interface IGisTable
    {
        #region ����
        bool CreateTable(string sTableName, IFields pFields, out Exception eError);
        #endregion

        #region ������
        //�򿪱�
        ITable OpenTable(string tablename, out Exception eError);
        //���ر���ĳ���ֶε�Ψһֵ
        ArrayList GetUniqueValue(string tablename, string fieldname, string condition, out Exception eError);
        //�õ�ĳ������ֶ����ͼ���
        Dictionary<string, Type> GetFieldsType(string tablename, out Exception eError);
        //�õ����ݱ��ĳһ����¼
        Dictionary<string, object> GetRow(string tablename, string condition, out Exception eError);
        //�õ����ݱ��ĳһ����¼ĳһ�ֶ�ֵ
        object GetFieldValue(string tablename, string keyfieldname, string condition, out Exception eError);
        //�õ����ݱ��з���ĳһ�����ļ�¼����
        long GetRowCount(string tablename, string condition, out Exception eError);

            #region �칹 �õ�ĳ�����з��������Ķ�����¼
            //��ȡĳ�ֶ�ֵ,����Dictionary��KEYΪOID
        Dictionary<int, object> GetRows(string tablename, string field, string condition, out Exception eError);
            //��ȡָ������ֶ�ֵ,����Dictionary��KEYΪOID
        Dictionary<int, ArrayList> GetRows(string tablename, List<string> fields, string condition, out Exception eError);
            //��ȡʱ����ĳ�ֶν�������,����Dictionary��KEYΪOID
        Dictionary<int, ArrayList> GetRows(string tablename, List<string> fields, string condition, string postfixClause, out Exception eError);
            //��ȡĳ�ֶ�ֵ,����Dictionary��KEYΪָ���ֶ�ֵ
        Dictionary<object, object> GetRows(string tablename, string keyField, string field, string condition, out Exception eError);
            //��ȡָ������ֶ�ֵ,����Dictionary��KEYΪָ���ֶ�ֵ
        Dictionary<object, ArrayList> GetRows(string tablename, string keyField, List<string> fields, string condition, out Exception eError);
            //��ȡʱ����ĳ�ֶν�������,����Dictionary��KEYΪָ���ֶ�ֵ
        Dictionary<object, ArrayList> GetRows(string tablename, string keyField, List<string> fields, string condition, string postfixClause, out Exception eError);
            #endregion

        //���ֶ�ֵ���ͼ��
        bool CheckFieldValue(string tablename, string fieldname, object value, out Exception eError);
        #endregion

        #region д����
            #region �칹 �½���
        bool NewRow(string tablename, Dictionary<string, object> dicvalues, bool bEdit, out Exception eError);
        bool NewRow(string tablename, Dictionary<string, object> dicvalues, bool bEdit, out int objectid, out Exception eError);
        bool NewRow(string tablename, Dictionary<string, object> dicvalues, bool bEdit, string strOIDField, out Exception eError);
        bool NewRow(string tablename, Dictionary<string, object> dicvalues, bool bEdit, string strOIDField, out int objectid, out Exception eError);
        bool NewRow(string tablename, Dictionary<string, object> dicvalues, bool bEdit, string strField, esriFieldType FieldType, out Exception eError);
        bool NewRow(string tablename, Dictionary<string, object> dicvalues, bool bEdit, string strField, esriFieldType FieldType, out int objectid, out Exception eError);
            #endregion

            #region �칹 �༭��
        bool EditRows(string tablename, string condition, Dictionary<string, object> dicValues, bool bEdit, out Exception eError);
        bool EditRows(string tablename, string condition, Dictionary<string, object> dicValues, bool bEdit, string fieldName, esriFieldType FieldType, out Exception eError);
            #endregion

        //ɾ����
        bool DeleteRows(string tablename, string condition, out Exception eError);
        #endregion
    }

    public interface ICreateGeoDatabase
    {
        /// <summary>
        /// �������ݿ�ṹ�ĵ�
        /// </summary>
        /// <param name="LoadPath">���ݿ�ṹ�ĵ�·��</param>
        /// <returns></returns>
        bool LoadDBShecmaDocument(string LoadPath);

        /// <summary>
        /// ���ؿռ�ο��ļ�
        /// </summary>
        /// <param name="LoadPath">�ռ�ο��ļ�·��</param>
        /// <returns></returns>
        bool LoadSpatialReference(string LoadPath);

        /// <summary>
        /// ����Ŀ�����ݿ���ʲ���
        /// </summary>
        /// <param name="Type">���ݿ����ͣ�PDB��GDB��SDE</param>
        /// <param name="IPoPath">�����PDB��GDB����д�ļ�·���������SDE����д������IP</param>
        /// <param name="Intance">�����SDE����дsdeʵ����</param>
        /// <param name="User">�����SDE����д�û���</param>
        /// <param name="PassWord">�����SDE����д����</param>
        /// <param name="Version">�����SDE����д�汾</param>
        /// <returns></returns>
        bool SetDestinationProp(string Type, string IPoPath, string Intance, string User, string PassWord, string Version);

        /// <summary>
        /// ��������
        /// </summary>
        /// 
        /// <returns></returns>
        bool CreateDBStruct();

        bool CreateDBStruct(List<string> DSName);

        bool CreateDBStruct(List<string> DSName,out int iScale, out string sDBName);

        //cyf 20110707 modify
        bool CreateDBStruct(out string iScale,out List<string> DSName,System.Windows.Forms.ProgressBar in_ProcBar);
        /// <summary>
        /// ����Զ����־��
        /// </summary>
        /// <param name="pDBType"></param>
        /// <param name="eError"></param>
        /// <returns></returns>
        bool CreateSQLTable(string pDBType, out Exception eError);
    }
        
    public interface IGisCommon
    {
        //���ù����ռ�
        bool SetWorkSpace(XmlElement aElemnet, Fan.Common.enumWSType wstype);

        #region �칹 �������ݱ�
        bool CreateTable(XmlElement aTableElement);
        bool CreateTable(XmlElement aOIDNode, XmlNodeList aNodeList, string sFeaClassName);
        #endregion

        //�������ݼ�
        bool CreateFeatureClass(XmlElement aOIDNode, XmlNodeList aNodeList, string sFeaClassName, string SpatialReference);
    }
}
