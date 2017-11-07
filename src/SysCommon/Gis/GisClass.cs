using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using System.Xml;
using System.Windows.Forms;

using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.ConversionTools;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.DataSourcesFile;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Display;

using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.DataManagementTools;
using ESRI.ArcGIS.Geoprocessing;
using ESRI.ArcGIS.Geoprocessor;
using SCHEMEMANAGERCLASSESLib;
using System.Data.OleDb;
using ESRI.ArcGIS.DataSourcesOleDB;
using ADODB;
using System.Data;

namespace SysCommon.Gis
{

    public class SysGisDB : IGisDB, IDisposable
    {
        private IWorkspace _Workspace;
        public IWorkspace WorkSpace
        {
            get { return _Workspace; }
            set { _Workspace = value; }
        }

        private string _WorkspaceServer;
        public string WorkspaceServer
        {
            get
            {
                return _WorkspaceServer;
            }

        }

        /// <summary>
        /// ����SDE������
        /// </summary>
        /// <param name="sServer">��������</param>
        /// <param name="sService">������</param>
        /// <param name="sDatabase">���ݿ���(SQLServer)</param>
        /// <param name="sUser">�û���</param>
        /// <param name="sPassword">����</param>
        /// <param name="strVersion">SDE�汾</param>
        /// <returns>�������Exception</returns>
        public bool SetWorkspace(string sServer, string sService, string sDatabase, string sUser, string sPassword, string strVersion, out Exception eError)
        {
            eError = null;
            IPropertySet pPropSet = new PropertySetClass();
            IWorkspaceFactory pSdeFact = new SdeWorkspaceFactoryClass();
            pPropSet.SetProperty("SERVER", sServer);
            pPropSet.SetProperty("INSTANCE", sService);
            pPropSet.SetProperty("DATABASE", sDatabase);
            pPropSet.SetProperty("USER", sUser);
            pPropSet.SetProperty("PASSWORD", sPassword);
            pPropSet.SetProperty("VERSION", strVersion);

            try
            {
                _Workspace = pSdeFact.Open(pPropSet, 0);
                _WorkspaceServer = sServer;
                pPropSet = null;
                pSdeFact = null;
                return true;
            }
            catch (Exception eX)
            {
                //********************************
                //guozheng added  system exception log
                if (SysCommon.Log.Module.SysLog == null)
                    SysCommon.Log.Module.SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
                SysCommon.Log.Module.SysLog.Write(eX);
                //********************************
                eError = eX;
                return false;
            }
        }

        /// <summary>
        /// ����PDB��GDB������
        /// </summary>
        /// <param name="sFilePath">�ļ�·��</param>
        /// <param name="wstype">����������</param>
        /// <returns>�������Exception</returns>
        public bool SetWorkspace(string sFilePath, enumWSType wstype, out Exception eError)
        {
            eError = null;

            try
            {
                IPropertySet pPropSet = new PropertySetClass();
                switch (wstype)
                {
                    case enumWSType.PDB:
                        AccessWorkspaceFactory pAccessFact = new AccessWorkspaceFactoryClass();
                        pPropSet.SetProperty("DATABASE", sFilePath);
                        _Workspace = pAccessFact.Open(pPropSet, 0);
                        pAccessFact = null;
                        break;
                    case enumWSType.GDB:
                        FileGDBWorkspaceFactoryClass pFileGDBFact = new FileGDBWorkspaceFactoryClass();
                        pPropSet.SetProperty("DATABASE", sFilePath);
                        _Workspace = pFileGDBFact.Open(pPropSet, 0);
                        pFileGDBFact = null;
                        break;
                    case enumWSType.SHP:
                        ShapefileWorkspaceFactory pFileSHPFact = new ShapefileWorkspaceFactory();
                        pPropSet.SetProperty("DATABASE", sFilePath);
                        _Workspace = pFileSHPFact.Open(pPropSet, 0);
                        pFileSHPFact = null;
                        break;
                }
                _WorkspaceServer = sFilePath;
                pPropSet = null;
                return true;
            }
            catch (Exception eX)
            {
                //********************************
                //guozheng added  system exception log
                if (SysCommon.Log.Module.SysLog == null)
                    SysCommon.Log.Module.SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
                SysCommon.Log.Module.SysLog.Write(eX);
                //********************************
                eError = eX;
                return false;
            }
        }

        /// <summary>
        /// ����WS�ı༭����
        /// </summary>
        /// <param name="withunodredo">�Ƿ�֧��UndoRedo����</param>
        /// <returns>�������Exception</returns>
        public bool StartWorkspaceEdit(bool withunodredo, out Exception eError)
        {
            IWorkspaceEdit pFeaWS = (IWorkspaceEdit)_Workspace;
            eError = null;
            try
            {
                if (pFeaWS.IsBeingEdited()) return true;
                pFeaWS.StartEditing(withunodredo);
                pFeaWS.StartEditOperation();
                return true;
            }
            catch (Exception eX)
            {
                //********************************
                //guozheng added  system exception log
                if (SysCommon.Log.Module.SysLog == null)
                    SysCommon.Log.Module.SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
                SysCommon.Log.Module.SysLog.Write(eX);
                //********************************
                eError = eX;
                return false;
            }
        }

        /// <summary>
        /// �ر�WS�ı༭����
        /// </summary>
        /// <param name="saveedits">�Ƿ񱣴�༭����</param>
        /// <returns>�������Exception</returns>
        public bool EndWorkspaceEdit(bool saveedits, out Exception eError)
        {
            eError = null;
            try
            {
                IWorkspaceEdit pFeaWS = (IWorkspaceEdit)_Workspace;
                if (!pFeaWS.IsBeingEdited()) return true;

                pFeaWS.StopEditOperation();
                pFeaWS.StopEditing(saveedits);
                return true;
            }
            catch (Exception eX)
            {
                //********************************
                //guozheng added  system exception log
                if (SysCommon.Log.Module.SysLog == null)
                    SysCommon.Log.Module.SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
                SysCommon.Log.Module.SysLog.Write(eX);
                //********************************
                eError = eX;
                return false;
            }
        }

        /// <summary>
        /// ��������
        /// </summary>
        /// <param name="eError">�������Exception</param>
        /// <returns></returns>
        public bool StartTransaction(out Exception eError)
        {
            eError = null;
            try
            {
                ITransactions pTransactions = (ITransactions)_Workspace;
                if (!pTransactions.InTransaction) pTransactions.StartTransaction();
                return true;
            }
            catch (Exception eX)
            {
                //********************************
                //guozheng added  system exception log
                if (SysCommon.Log.Module.SysLog == null)
                    SysCommon.Log.Module.SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
                SysCommon.Log.Module.SysLog.Write(eX);
                //********************************
                eError = eX;
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="saveedits">�Ƿ��ύ����</param>
        /// <param name="eError">�������Exception</param>
        /// <returns></returns>
        public bool EndTransaction(bool saveedits, out Exception eError)
        {
            eError = null;
            try
            {
                ITransactions pTransactions = (ITransactions)_Workspace;
                if (saveedits)
                {
                    if (pTransactions.InTransaction) pTransactions.CommitTransaction();
                }
                else
                {
                    if (pTransactions.InTransaction) pTransactions.AbortTransaction();
                }
                return true;
            }
            catch (Exception eX)
            {
                //********************************
                //guozheng added  system exception log
                if (SysCommon.Log.Module.SysLog == null)
                    SysCommon.Log.Module.SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
                SysCommon.Log.Module.SysLog.Write(eX);
                //********************************
                eError = eX;
                return false;
            }
        }

        /// <summary>
        /// �رչ�����
        /// </summary>
        /// /// <returns>�������</returns>
        public bool CloseWorkspace()
        {
            if (_Workspace == null) return true;
            ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(_Workspace.WorkspaceFactory);
            ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(_Workspace);
            _Workspace = null;
            return true;
        }

        /// <summary>
        /// �رչ�����
        /// </summary>
        /// <returns>�������</returns>
        public bool CloseWorkspace(bool bRemove)
        {
            if (_Workspace == null) return true;
            if (bRemove == true)
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(_Workspace.WorkspaceFactory);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(_Workspace);
            }
            _Workspace = null;
            return true;
        }

        #region IDisposable ��Ա

        public void Dispose()
        {
            ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(_Workspace);
            Marshal.ReleaseComObject(_Workspace);
            _Workspace = null;
        }

        #endregion
    }

    public class SysGisDataSet : SysGisDB, IGisDataSet
    {
        public SysGisDataSet()
        {

        }

        public SysGisDataSet(IWorkspace pWorkspace)
        {
            this.WorkSpace = pWorkspace;
        }

        /// <summary>
        /// ����DEM���ݼ�
        /// </summary>
        /// <param name="name">���ݼ�����</param>
        /// <param name="vCompressionType">ѹ����ʽ</param>
        /// <param name="CompressionQuality">ѹ���ȣ���JPEG��ʽ��Ч</param>
        /// <param name="vPyramidLevel">����������</param>
        /// <param name="vResamplingTypes">������������ʽ</param>
        /// <param name="vTileHeight">�����߶�</param>
        /// <param name="vTileWidth">�������</param>
        /// <returns></returns>
        public bool CreateDEMDataSet(string name, int vCompressionType, int CompressionQuality, int vResamplingTypes, int vTileHeight, int vTileWidth, out Exception eError)
        {
            eError = null;
            IRasterWorkspaceEx pRasterWSEx = (IRasterWorkspaceEx)this.WorkSpace;
            ISpatialReference pSpatialRef = new UnknownCoordinateSystemClass();

            // ����IRasterStorageDef
            IRasterStorageDef pRasterStorageDef = new RasterStorageDefClass();
            pRasterStorageDef.CompressionType = (esriRasterCompressionType)vCompressionType;
            pRasterStorageDef.CompressionQuality = CompressionQuality;
            pRasterStorageDef.PyramidLevel = 7;
            pRasterStorageDef.PyramidResampleType = (rstResamplingTypes)vResamplingTypes;
            pRasterStorageDef.TileHeight = vTileHeight;
            pRasterStorageDef.TileWidth = vTileWidth;

            // ����IRasterDef
            IRasterDef pRasterDef = new RasterDefClass();
            pRasterDef.Description = "RasterDataSet";
            pRasterDef.SpatialReference = pSpatialRef;

            // ����IGeometryDef
            IGeometryDef pGeometryDef = new GeometryDefClass();
            IGeometryDefEdit pGeometryDefEdit = (IGeometryDefEdit)pGeometryDef;

            pGeometryDefEdit.GeometryType_2 = esriGeometryType.esriGeometryPolygon;
            pGeometryDefEdit.AvgNumPoints_2 = 4;
            pGeometryDefEdit.GridCount_2 = 1;
            pGeometryDefEdit.set_GridSize(0, 1000);
            pGeometryDefEdit.SpatialReference_2 = new UnknownCoordinateSystemClass();
            try
            {
                pRasterWSEx.CreateRasterDataset(name, 3, rstPixelType.PT_LONG, pRasterStorageDef, "DEFAULTS", pRasterDef, pGeometryDef);
                return true;
            }
            catch (Exception eX)
            {
                //********************************
                //guozheng added  system exception log
                if (SysCommon.Log.Module.SysLog == null)
                    SysCommon.Log.Module.SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
                SysCommon.Log.Module.SysLog.Write(eX);
                //********************************
                eError = eX;
                return false;
            }
        }

        /// <summary>
        /// ����DOM���ݼ�
        /// </summary>
        /// <param name="name">���ݼ�����</param>
        /// <returns></returns>
        public bool CreateDOMDataSet(string name, out Exception eError)
        {
            eError = null;
            IRasterWorkspaceEx pRasterWSEx = (IRasterWorkspaceEx)this.WorkSpace;
            ISpatialReference pSpatialRef = new UnknownCoordinateSystemClass();

            //������Ӧ��Fields
            IField pField;
            IFieldEdit pFieldEdit;

            IFields pFields = new FieldsClass();
            IFieldsEdit pFieldsEdit = (IFieldsEdit)pFields;

            //����OID�ֶ�
            pField = new FieldClass();
            pFieldEdit = (IFieldEdit)pField;
            pFieldEdit.Name_2 = "ObjectID";
            pFieldEdit.Type_2 = esriFieldType.esriFieldTypeOID;

            pFieldsEdit.AddField(pField);

            //����Name�ֶ�
            pField = new FieldClass();
            pFieldEdit = (IFieldEdit)pField;
            pFieldEdit.Name_2 = "Name";
            pFieldEdit.Type_2 = esriFieldType.esriFieldTypeString;

            pFieldsEdit.AddField(pField);

            //����Shape�ֶ�
            pField = new FieldClass();
            pFieldEdit = (IFieldEdit)pField;
            pFieldEdit.Name_2 = "Shape";
            pFieldEdit.Type_2 = esriFieldType.esriFieldTypeGeometry;

            // ����IGeometryDef
            IGeometryDef pGeometryDef = new GeometryDefClass();
            IGeometryDefEdit pGeometryDefEdit = (IGeometryDefEdit)pGeometryDef;
            pGeometryDefEdit.GeometryType_2 = esriGeometryType.esriGeometryPolygon;
            pGeometryDefEdit.AvgNumPoints_2 = 4;
            pGeometryDefEdit.GridCount_2 = 1;
            pGeometryDefEdit.set_GridSize(0, 1000);
            pGeometryDefEdit.SpatialReference_2 = pSpatialRef;

            pFieldEdit.GeometryDef_2 = pGeometryDef;
            pFieldsEdit.AddField(pField);

            //����Raster�ֶ�
            IField2 pField2 = new FieldClass();
            IFieldEdit2 pFieldEdit2 = (IFieldEdit2)pField2;
            pFieldEdit2.Name_2 = "Raster";
            pFieldEdit2.Type_2 = esriFieldType.esriFieldTypeRaster;

            // ����IRasterDef
            IRasterDef pRasterDef = new RasterDefClass();
            pRasterDef.Description = "it is a raster catalog";
            pRasterDef.SpatialReference = pSpatialRef;

            pFieldEdit2.RasterDef = pRasterDef;
            pFieldsEdit.AddField(pField2);

            try
            {
                pRasterWSEx.CreateRasterCatalog(name, pFields, "Shape", "Raster", "defaults");
                return true;
            }
            catch (Exception eX)
            {
                //********************************
                //guozheng added  system exception log
                if (SysCommon.Log.Module.SysLog == null)
                    SysCommon.Log.Module.SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
                SysCommon.Log.Module.SysLog.Write(eX);
                //********************************
                eError = eX;
                return false;
            }
        }

        /// <summary>
        /// ��ȡҪ�ؼ�FeatureClass
        /// </summary>
        /// <param name="feaclassname">Ҫ�ؼ���</param>
        /// <returns></returns>
        public IFeatureClass GetFeatureClass(string feaclassname, out Exception eError)
        {
            eError = null;
            //�õ�FeatrueWS
            IFeatureWorkspace pFeaWS = (IFeatureWorkspace)this.WorkSpace;
            //��FeaClass
            try
            {   //Ҫ�ؼ����ܲ����ڣ���һ�α���
                return pFeaWS.OpenFeatureClass(feaclassname);
            }
            catch (Exception eX)
            {
                //********************************
                //guozheng added  system exception log
                if (SysCommon.Log.Module.SysLog == null)
                    SysCommon.Log.Module.SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
                SysCommon.Log.Module.SysLog.Write(eX);
                //********************************
                eError = eX;
                return null;
            }
        }

        /// <summary>
        /// ��ȡҪ�ؼ�FeatureDataset
        /// </summary>
        /// <param name="featuredsname">Ҫ�ؼ���</param>
        /// <returns></returns>
        public IFeatureDataset GetFeatureDataset(string featuredsname, out Exception eError)
        {
            eError = null;
            //�õ�FeatrueWS
            IFeatureWorkspace pFeaWS = (IFeatureWorkspace)this.WorkSpace;
            //��FeatureDataset
            try
            {   //Ҫ�ؼ����ܲ����ڣ���һ�α���
                return pFeaWS.OpenFeatureDataset(featuredsname);
            }
            catch (Exception eX)
            {
                //********************************
                //guozheng added  system exception log
                if (SysCommon.Log.Module.SysLog == null)
                    SysCommon.Log.Module.SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
                SysCommon.Log.Module.SysLog.Write(eX);
                //********************************
                eError = eX;
                return null;
            }
        }

        public IRasterDataset GetRasterDataset(string name, out Exception eError)
        {
            eError = null;

            IRasterWorkspaceEx pRasterWSEx = (IRasterWorkspaceEx)this.WorkSpace;
            try
            {   //Ҫ�ؼ����ܲ����ڣ���һ�α���
                return pRasterWSEx.OpenRasterDataset(name);
            }
            catch (Exception eX)
            {
                //********************************
                //guozheng added  system exception log
                if (SysCommon.Log.Module.SysLog == null)
                    SysCommon.Log.Module.SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
                SysCommon.Log.Module.SysLog.Write(eX);
                //********************************
                eError = eX;
                return null;
            }
        }

        /// <summary>
        /// ��ȡ�����ռ���ָ���������ݼ�����
        /// </summary>
        /// <param name="pWorkspace">�����ռ�</param>
        /// <param name="aDatasetTyp">���ݼ�����</param>
        /// <returns></returns>
        public List<string> GetDatasetNames(IWorkspace pWorkspace, esriDatasetType aDatasetTyp)
        {
            try
            {
                List<string> DatasetNames = new List<string>();
                IEnumDatasetName pEnumDatasetName = pWorkspace.get_DatasetNames(aDatasetTyp);
                IDatasetName pDatasetName = pEnumDatasetName.Next();
                while (pDatasetName != null)
                {
                    //deleted by chulili 20110915 ��ʱע�͵�  �����ڱ��û������ݼ�����ӡ��������޸ĳɸ��������ļ��ж�
                    //if (pWorkspace.WorkspaceFactory is SdeWorkspaceFactoryClass)//add by xisheng 20110906
                    //{
                    //    object name = pWorkspace.ConnectionProperties.GetProperty("USER");

                    //    string[] strTemp = pDatasetName.Name.Split('.');
                    //    if (strTemp[0].Trim().ToUpper() != name.ToString().ToUpper())
                    //    {
                    //        pDatasetName = pEnumDatasetName.Next();
                    //        continue;
                    //    }
                    //}
                    //end deleted by chulili 20110915
                    DatasetNames.Add(pDatasetName.Name);
                    pDatasetName = pEnumDatasetName.Next();

                }
                return DatasetNames;
            }
            catch (Exception ex)
            {
                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", ex.Message);
                return null;
            }
        }

        /// <summary>
        /// ��ȡ�����ռ���ָ���������ݼ�����
        /// </summary>
        /// <param name="pWorkspace">�����ռ�</param>
        /// <param name="aDatasetTyp">���ݼ�����</param>
        /// <param name="bHasUserName">���ΪSDE,�Ƿ�����û���</param>
        /// <returns></returns>
        public List<string> GetDatasetNames(IWorkspace pWorkspace, esriDatasetType aDatasetTyp, bool bHasUserName)
        {
            List<string> DatasetNames = new List<string>();
            IEnumDatasetName pEnumDatasetName = pWorkspace.get_DatasetNames(aDatasetTyp);
            IDatasetName pDatasetName = pEnumDatasetName.Next();
            object name = pWorkspace.ConnectionProperties.GetProperty("USER");
            
            while (pDatasetName != null)
            {
                string[] strTemp = pDatasetName.Name.Split('.');
                if (strTemp[0].Trim().ToUpper() != name.ToString().ToUpper())
                {
                    pDatasetName = pEnumDatasetName.Next();
                    continue;
                }
                if (bHasUserName == true)
                {
                    DatasetNames.Add(pDatasetName.Name);
                }
                else
                {
                   
                    DatasetNames.Add(strTemp[1]);
                }
                pDatasetName = pEnumDatasetName.Next();
            }
            return DatasetNames;
        }

        /// <summary>
        /// ��ȡ���ݿ���ȫ����RD����
        /// </summary>
        /// <returns></returns>
        public List<string> GetAllRDNames()
        {
            return GetDatasetNames(this.WorkSpace, esriDatasetType.esriDTRasterDataset);

        }

        /// <summary>
        /// ��ȡ���ݿ���ȫ����RC����
        /// </summary>
        /// <returns></returns>
        public List<string> GetAllRCNames()
        {
            return GetDatasetNames(this.WorkSpace, esriDatasetType.esriDTRasterCatalog);
        }

        /// <summary>
        /// ��ȡ���ݿ���ȫ����FD����
        /// </summary>
        /// <returns></returns>
        public List<string> GetAllFeatureDatasetNames()
        {
            return GetDatasetNames(this.WorkSpace, esriDatasetType.esriDTFeatureDataset);
        }

        /// <summary>
        /// ��ȡ���ݿ���ȫ����FC����
        /// </summary>
        /// <param name="bFullName">FC�����Ƿ�ΪFD.Name + "\" +FC.Name</param>
        /// <returns></returns>
        public List<string> GetAllFeatureClassNames(bool bFullName)
        {
            List<string> FCNames = new List<string>();

            //�õ�ȫ�������FC����
            List<string> LsFCNames = GetFeatureClassNames();
            if (LsFCNames != null)
            {
                if (LsFCNames.Count > 0)
                {
                    FCNames.AddRange(LsFCNames);
                }
            }

            //�õ�Ҫ�ؼ�����ȫ��FC����
            IEnumDatasetName pEnumDsName = this.WorkSpace.get_DatasetNames(esriDatasetType.esriDTFeatureDataset);
            IDatasetName pDsName = pEnumDsName.Next();
            while (pDsName != null)
            {
                IFeatureDatasetName pFeatureDsName = (IFeatureDatasetName)pDsName;
                List<string> FdFCNames = GetFeatureClassNames(pFeatureDsName, bFullName);
                if (FdFCNames != null)
                {
                    if (FdFCNames.Count > 0)
                    {
                        FCNames.AddRange(FdFCNames);
                    }
                }
                pDsName = pEnumDsName.Next();
            }

            return FCNames;
        }

        /// <summary>
        /// ��ȡ���ݿ���ȫ����FC����
        /// </summary>
        /// <param name="bFullName">FC�����Ƿ�ΪFD.Name + "\" +FC.Name</param>
        /// <param name="bHasUserName">���ΪSDE,���Ƿ���û���</param>
        /// <returns></returns>
        public List<string> GetAllFeatureClassNames(bool bFullName, bool bHasUserName)
        {
            List<string> FCNames = new List<string>();

            //�õ�ȫ�������FC����
            List<string> LsFCNames = GetFeatureClassNames(bHasUserName);
            if (LsFCNames != null)
            {
                if (LsFCNames.Count > 0)
                {
                    FCNames.AddRange(LsFCNames);
                }
            }

            //�õ�Ҫ�ؼ�����ȫ��FC����
            IEnumDatasetName pEnumDsName = this.WorkSpace.get_DatasetNames(esriDatasetType.esriDTFeatureDataset);
            IDatasetName pDsName = pEnumDsName.Next();
            object name = this.WorkSpace.ConnectionProperties.GetProperty("USER");
            while (pDsName != null)
            {
                string[] strTemp = pDsName.Name.Split('.');
                if (strTemp[0].Trim().ToUpper() != name.ToString().ToUpper())
                {
                    pDsName = pEnumDsName.Next();
                    continue;
                }
                IFeatureDatasetName pFeatureDsName = (IFeatureDatasetName)pDsName;
                List<string> FdFCNames = GetFeatureClassNames(pFeatureDsName, bFullName, bHasUserName);
                if (FdFCNames != null)
                {
                    if (FdFCNames.Count > 0)
                    {
                        FCNames.AddRange(FdFCNames);
                    }
                }
                pDsName = pEnumDsName.Next();
            }

            return FCNames;
        }

        /// <summary>
        /// ��ȡ�����ռ���ָ���������ݼ�
        /// </summary>
        /// <param name="pWorkspace">�����ռ�</param>
        /// <param name="aDatasetTyp">���ݼ�����</param>
        /// <returns></returns>
        public List<IDataset> GetDatasets(IWorkspace pWorkspace, esriDatasetType aDatasetTyp)
        {
            List<IDataset> Datasets = new List<IDataset>();
            IEnumDataset pEnumDataset = pWorkspace.get_Datasets(aDatasetTyp);
            IDataset pDataset = pEnumDataset.Next();
            while (pDataset != null)
            {
                Datasets.Add(pDataset);
                pDataset = pEnumDataset.Next();
            }
            return Datasets;
        }

        /// <summary>
        /// ��ȡ�����ռ���ָ���������ݼ�
        /// </summary>
        /// <param name="pWorkspace">�����ռ�</param>
        /// <param name="aDatasetTyp">���ݼ�����</param>
        /// <returns></returns>
        public List<IDataset> GetDatasets1(IWorkspace pWorkspace, esriDatasetType aDatasetTyp)
        {
            List<IDataset> Datasets = new List<IDataset>();
            IEnumDataset pEnumDataset = pWorkspace.get_Datasets(aDatasetTyp);
            if (pWorkspace.WorkspaceFactory is SdeWorkspaceFactoryClass)//add by xisheng 20110906
            {
                object name = pWorkspace.ConnectionProperties.GetProperty("USER");

              
                IDataset pDataset = pEnumDataset.Next();
                while (pDataset != null)
                {
                    string[] strTemp = pDataset.Name.Split('.');
                    if (strTemp[0].Trim().ToUpper() != name.ToString().ToUpper())
                    {
                        pDataset = pEnumDataset.Next();
                        continue;
                    }
                    Datasets.Add(pDataset);
                    pDataset = pEnumDataset.Next();
                }
            }
            else
            {
                IDataset pDataset = pEnumDataset.Next();
                while (pDataset != null)
                {
                    Datasets.Add(pDataset);
                    pDataset = pEnumDataset.Next();
                }
            }
            return Datasets;
        }

        /// <summary>
        /// ��ȡ���ݿ���ȫ����FC
        /// </summary>
        /// <returns></returns>
        public List<IDataset> GetAllFeatureClass()
        {
            List<IDataset> listFC = new List<IDataset>();

            //�õ�ȫ�������FC����
            List<IDataset> LsFC = GetDatasets(this.WorkSpace, esriDatasetType.esriDTFeatureClass);
            if (LsFC != null)
            {
                if (LsFC.Count > 0)
                {
                    listFC.AddRange(LsFC);
                }
            }

            //�õ�Ҫ�ؼ�����ȫ��FC����
            IEnumDataset pEnumDs = this.WorkSpace.get_Datasets(esriDatasetType.esriDTFeatureDataset);
            IDataset pDs = pEnumDs.Next();
            while (pDs != null)
            {
                IFeatureDataset pFeatureDs = (IFeatureDataset)pDs;
                List<IDataset> FdFCs = GetFeatureClass(pFeatureDs);
                if (FdFCs != null)
                {
                    if (FdFCs.Count > 0)
                    {
                        listFC.AddRange(FdFCs);
                    }
                }
                pDs = pEnumDs.Next();
            }

            return listFC;
        }
        /// <summary>
        /// ��ȡĳһҪ�ؼ�����FC
        /// </summary>
        /// <param name="pFeaDsName">Ҫ�ؼ�IFeatureDataset</param>
        /// <returns></returns>
        public List<IDataset> GetFeatureClass(IFeatureDataset pFeaDs)
        {
            List<IDataset> FCs = new List<IDataset>();

            IEnumDataset pEnumDs = pFeaDs.Subsets;
            IDataset pDs = pEnumDs.Next();
            while (pDs != null)
            {
                FCs.Add(pDs);
                pDs = pEnumDs.Next();
            }
            return FCs;
        }

        /// <summary>
        /// ��ȡ���ݿ���ȫ������ɢFC����
        /// </summary>
        /// <returns></returns>
        public List<string> GetFeatureClassNames()
        {
            return GetDatasetNames(this.WorkSpace, esriDatasetType.esriDTFeatureClass);
        }

        /// <summary>
        /// ��ȡ���ݿ���ȫ������ɢFC����
        /// </summary>
        /// <param name="bHasUserName">���ΪSDE,�Ƿ�����û���</param>
        /// <returns></returns>
        public List<string> GetFeatureClassNames(bool bHasUserName)
        {
            return GetDatasetNames(this.WorkSpace, esriDatasetType.esriDTFeatureClass, bHasUserName);
        }

        /// <summary>
        /// ��ȡĳһҪ�ؼ�����FC����
        /// </summary>
        /// <param name="fdname">Ҫ�ؼ�FeatureDataset����</param>
        /// <param name="bFullName">FC�����Ƿ�ΪFD.Name + "\" +FC.Name</param>
        /// <returns></returns>
        public List<string> GetFeatureClassNames(string fdname, bool bFullName, out Exception eError)
        {
            eError = null;
            List<string> FCNames = new List<string>();

            // �õ���Ӧ��FeatureDs
            IFeatureDataset pFeatureDs = GetFeatureDataset(fdname, out eError);
            if (pFeatureDs == null) return null;

            IFeatureDatasetName pFeaDsName = (IFeatureDatasetName)(pFeatureDs.FullName);
            IEnumDatasetName pEnumDsName = pFeaDsName.FeatureClassNames;
            IDatasetName pDsName = pEnumDsName.Next();
            while (pDsName != null)
            {
                string strDSName = pDsName.Name;
                //shduan 20110718 delete**************************************************
                //if (strDSName.Contains("."))
                //{
                //    strDSName = strDSName.Substring(strDSName.IndexOf(".") + 1);
                //}
                //shduan 20110718 delete**************************************************
                if (bFullName == true)
                {
                    FCNames.Add(pFeatureDs.Name + "\\" + strDSName);
                }
                else
                {
                    FCNames.Add(strDSName);
                }
                pDsName = pEnumDsName.Next();
            }
            return FCNames;
        }

        /// <summary>
        /// ��ȡĳһҪ�ؼ�����FC����
        /// </summary>
        /// <param name="pFeaDsName">Ҫ�ؼ�IFeatureDatasetName</param>
        /// <param name="bFullName">FC�����Ƿ�ΪFD.Name + "\" +FC.Name</param>
        /// <returns></returns>
        public List<string> GetFeatureClassNames(IFeatureDatasetName pFeaDsName, bool bFullName)
        {
            List<string> FCNames = new List<string>();

            IEnumDatasetName pEnumDsName = pFeaDsName.FeatureClassNames;
            IDatasetName pDsName = pEnumDsName.Next();
            while (pDsName != null)
            {
                if (bFullName == true)
                {
                    IDatasetName pName = (IDatasetName)pFeaDsName;
                    FCNames.Add(pName.Name + "\\" + pDsName.Name);
                }
                else
                {
                    FCNames.Add(pDsName.Name);
                }
                pDsName = pEnumDsName.Next();
            }
            return FCNames;
        }

        /// <summary>
        /// ��ȡĳһҪ�ؼ�����FC����
        /// </summary>
        /// <param name="pFeaDsName">Ҫ�ؼ�IFeatureDatasetName</param>
        /// <param name="bFullName">FC�����Ƿ�ΪFD.Name + "\" +FC.Name</param>
        /// <param name="bHasUserName">���ΪSDE,�Ƿ�����û���</param>
        /// <returns></returns>
        public List<string> GetFeatureClassNames(IFeatureDatasetName pFeaDsName, bool bFullName, bool bHasUserName)
        {
            List<string> FCNames = new List<string>();
            IEnumDatasetName pEnumDsName = null;
            try
            {
                pEnumDsName = pFeaDsName.FeatureClassNames;
            }
            catch { return FCNames; }
            IDatasetName pDsName = pEnumDsName.Next();
            object name = this.WorkSpace.ConnectionProperties.GetProperty("USER");
            while (pDsName != null)
            {
                string[] strTemp = pDsName.Name.Split('.');
                if (strTemp[0].Trim().ToUpper() != name.ToString().ToUpper())
                {
                    pDsName = pEnumDsName.Next();
                    continue;
                }
                if (bFullName == true)
                {
                    IDatasetName pName = (IDatasetName)pFeaDsName;
                    if (bHasUserName == true)
                    {
                        FCNames.Add(pName.Name + "\\" + pDsName.Name);
                    }
                    else
                    {
                        string[] strTemp1 = pName.Name.Split('.');
                        FCNames.Add(strTemp1[1] + "\\" + strTemp[1]);
                    }
                }
                else
                {
                    if (bHasUserName == true)
                    {
                        FCNames.Add(pDsName.Name);
                    }
                    else
                    {
                        FCNames.Add(strTemp[1]);
                    }
                }
                pDsName = pEnumDsName.Next();
            }
            return FCNames;
        }

        /// <summary>
        /// ����������ȡҪ�ؼ���FeatureCursor
        /// </summary>
        /// <param name="featureclassname">FC����</param>
        /// <param name="condition">����</param>
        /// <returns></returns>
        public IFeatureCursor GetFeatureCursor(string featureclassname, string condition, out Exception eError)
        {
            eError = null;
            IFeatureClass pFeaClass = GetFeatureClass(featureclassname, out eError);
            if (pFeaClass == null) return null;

            IQueryFilter pQueryFilter = new QueryFilterClass();
            pQueryFilter.WhereClause = condition;

            try
            {
                IFeatureCursor pFeaCursor = pFeaClass.Search(pQueryFilter, false);
                return pFeaCursor;
            }
            catch (Exception eX)
            {
                //********************************
                //guozheng added  system exception log
                if (SysCommon.Log.Module.SysLog == null)
                    SysCommon.Log.Module.SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
                SysCommon.Log.Module.SysLog.Write(eX);
                //********************************
                eError = eX;
                return null;
            }
        }
        /// <summary>
        /// ����������ȡҪ�ؼ���FeatureCursor
        /// </summary>
        /// <param name="featureclassname">FC����</param>
        /// <param name="condition">����</param>
        /// <returns></returns>
        public IFeatureCursor GetFeatureCursor(string featureclassname, string condition, IGeometry pGeometry, esriSpatialRelEnum pSpatialRel, out Exception eError)
        {
            eError = null;
            IFeatureClass pFeaClass = GetFeatureClass(featureclassname, out eError);
            if (pFeaClass == null) return null;

            ISpatialFilter pSpatialFilter = new SpatialFilterClass();
            pSpatialFilter.WhereClause = condition;
            if (pGeometry != null)
            {
                pSpatialFilter.Geometry = pGeometry;
                pSpatialFilter.GeometryField = "SHAPE";
                pSpatialFilter.SpatialRel = pSpatialRel;
            }

            try
            {
                IFeatureCursor pFeaCursor = pFeaClass.Search(pSpatialFilter, false);
                return pFeaCursor;
            }
            catch (Exception eX)
            {
                //********************************
                //guozheng added  system exception log
                if (SysCommon.Log.Module.SysLog == null)
                    SysCommon.Log.Module.SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
                SysCommon.Log.Module.SysLog.Write(eX);
                //********************************
                eError = eX;
                return null;
            }
        }

        /// <summary>
        /// ����������ȡ,�����ȡҪ������
        /// </summary>
        /// <param name="featureclassname">FC����</param>
        /// <param name="condition">����</param>
        /// <param name="count">��ȡҪ������</param>
        /// <returns></returns>
        public IFeatureCursor GetFeatureCursor(string featureclassname, string condition, IGeometry pGeometry, esriSpatialRelEnum pSpatialRel, out Exception eError, out int count)
        {
            count = -1;
            eError = null;
            IFeatureClass pFeaClass = GetFeatureClass(featureclassname, out eError);
            if (pFeaClass == null) return null;
            ISpatialFilter pSpatialFilter = new SpatialFilterClass();
            pSpatialFilter.WhereClause = condition;
            if (pGeometry != null)
            {
                pSpatialFilter.Geometry = pGeometry;
                pSpatialFilter.GeometryField = "SHAPE";
                pSpatialFilter.SpatialRel = pSpatialRel;
            }
            try
            {
                count = pFeaClass.FeatureCount(pSpatialFilter);
                IFeatureCursor pFeaCursor = pFeaClass.Search(pSpatialFilter, false);
                return pFeaCursor;
            }
            catch (Exception eX)
            {
                //********************************
                //guozheng added  system exception log
                if (SysCommon.Log.Module.SysLog == null)
                    SysCommon.Log.Module.SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
                SysCommon.Log.Module.SysLog.Write(eX);
                //********************************
                eError = eX;
                count = -1;
                return null;
            }
        }

        /// <summary>
        /// ����������ȡ,�����ȡҪ��������FC��Ҫ������
        /// </summary>
        /// <param name="featureclassname">FC����</param>
        /// <param name="condition">����</param>
        /// <param name="count">��ȡҪ������</param>
        /// <param name="total">FC��Ҫ������</param>
        /// <returns></returns>
        public IFeatureCursor GetFeatureCursor(string featureclassname, string condition, IGeometry pGeometry, esriSpatialRelEnum pSpatialRel, out Exception eError, out int count, out int total)
        {
            count = -1;
            total = -1;
            eError = null;

            IFeatureClass pFeaClass = GetFeatureClass(featureclassname, out eError);
            if (pFeaClass == null) return null;
            ISpatialFilter pSpatialFilter = new SpatialFilterClass();
            pSpatialFilter.WhereClause = condition;
            if (pGeometry != null)
            {
                pSpatialFilter.Geometry = pGeometry;
                pSpatialFilter.GeometryField = "SHAPE";
                pSpatialFilter.SpatialRel = pSpatialRel;
            }

            try
            {
                count = pFeaClass.FeatureCount(pSpatialFilter);
                total = ModGisPub.GetFeatureCount(pFeaClass,null);
                IFeatureCursor pFeaCursor = pFeaClass.Search(pSpatialFilter, false);
                return pFeaCursor;
            }
            catch (Exception eX)
            {
                //********************************
                //guozheng added  system exception log
                if (SysCommon.Log.Module.SysLog == null)
                    SysCommon.Log.Module.SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
                SysCommon.Log.Module.SysLog.Write(eX);
                //********************************
                eError = eX;
                count = -1;
                total = -1;
                return null;
            }
        }

        /// <summary>
        /// ����������ȡ,�����ȡҪ��������FC��Ҫ������
        /// </summary>
        /// <param name="featureclassname">FC����</param>
        /// <param name="condition">����</param>
        /// <param name="count">��ȡҪ������</param>
        /// <param name="total">FC��Ҫ������</param>
        /// <returns></returns>
        public IFeatureCursor GetFeatureCursor(string featureclassname, string condition, IGeometry pGeometry, string strSpatialRel, out Exception eError, out int count, out int total)
        {
            count = -1;
            total = -1;
            eError = null;

            IFeatureClass pFeaClass = GetFeatureClass(featureclassname, out eError);
            if (pFeaClass == null) return null;
            ISpatialFilter pSpatialFilter = new SpatialFilterClass();
            pSpatialFilter.WhereClause = condition;
            if (pGeometry != null)
            {
                pSpatialFilter.Geometry = pGeometry;
                pSpatialFilter.GeometryField = "SHAPE";
                pSpatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelRelation;
                pSpatialFilter.SpatialRelDescription = strSpatialRel;
            }

            try
            {
                count = pFeaClass.FeatureCount(pSpatialFilter);
                total = ModGisPub.GetFeatureCount(pFeaClass, null);
                IFeatureCursor pFeaCursor = pFeaClass.Search(pSpatialFilter, false);
                return pFeaCursor;
            }
            catch (Exception eX)
            {
                //********************************
                //guozheng added  system exception log
                if (SysCommon.Log.Module.SysLog == null)
                    SysCommon.Log.Module.SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
                SysCommon.Log.Module.SysLog.Write(eX);
                //********************************
                eError = eX;
                count = -1;
                total = -1;
                return null;
            }
        }

        /// <summary>
        /// ����������ȡָ��Ҫ��
        /// </summary>
        /// <param name="featureclassname">FC����</param>
        /// <param name="condition">��������</param>
        /// <param name="pGeometry">�ռ�ͼ������</param>
        /// <param name="pSpatialRel">�ռ��ϵ</param>
        /// <param name="eError"></param>
        /// <returns></returns>
        public IFeature GetFeature(string featureclassname, string condition, IGeometry pGeometry, esriSpatialRelEnum pSpatialRel, out Exception eError)
        {
            eError = null;
            IFeatureClass pFeaClass = GetFeatureClass(featureclassname, out eError);
            if (pFeaClass == null) return null;
            ISpatialFilter pSpatialFilter = new SpatialFilterClass();
            pSpatialFilter.WhereClause = condition;
            if (pGeometry != null)
            {
                pSpatialFilter.Geometry = pGeometry;
                pSpatialFilter.GeometryField = "SHAPE";
                pSpatialFilter.SpatialRel = pSpatialRel;
            }
            try
            {
                IFeatureCursor pFeaCursor = pFeaClass.Search(pSpatialFilter, false);
                IFeature pFeat = pFeaCursor.NextFeature();
                Marshal.ReleaseComObject(pFeaCursor);
                return pFeat;
            }
            catch (Exception eX)
            {
                //********************************
                //guozheng added  system exception log
                if (SysCommon.Log.Module.SysLog == null)
                    SysCommon.Log.Module.SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
                SysCommon.Log.Module.SysLog.Write(eX);
                //********************************
                eError = eX;
                return null;
            }
        }

        /// <summary>
        /// ����������ȡָ��Ҫ��
        /// </summary>
        /// <param name="featureclassname">FC����</param>
        /// <param name="condition">��������</param>
        /// <param name="pGeometry">�ռ�ͼ������</param>
        /// <param name="strSpatialRel">�ռ��ϵ</param>
        /// <param name="eError"></param>
        /// <returns></returns>
        public IFeature GetFeature(string featureclassname, string condition, IGeometry pGeometry, string strSpatialRel, out Exception eError)
        {
            eError = null;
            IFeatureClass pFeaClass = GetFeatureClass(featureclassname, out eError);
            if (pFeaClass == null) return null;
            ISpatialFilter pSpatialFilter = new SpatialFilterClass();
            pSpatialFilter.WhereClause = condition;
            if (pGeometry != null)
            {
                pSpatialFilter.Geometry = pGeometry;
                pSpatialFilter.GeometryField = "SHAPE";
                pSpatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelRelation;
                pSpatialFilter.SpatialRelDescription = strSpatialRel;
            }
            try
            {
                IFeatureCursor pFeaCursor = pFeaClass.Search(pSpatialFilter, false);
                IFeature pFeat = pFeaCursor.NextFeature();
                Marshal.ReleaseComObject(pFeaCursor);
                return pFeat;
            }
            catch (Exception eX)
            {
                //********************************
                //guozheng added  system exception log
                if (SysCommon.Log.Module.SysLog == null)
                    SysCommon.Log.Module.SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
                SysCommon.Log.Module.SysLog.Write(eX);
                //********************************
                eError = eX;
                return null;
            }
        }

        /// <summary>
        /// ����������ȡָ��Ҫ��
        /// </summary>
        /// <param name="pFeatCls">FC</param>
        /// <param name="condition">��������</param>
        /// <param name="pGeometry">�ռ�ͼ������</param>
        /// <param name="pSpatialRel">�ռ��ϵ</param>
        /// <param name="eError"></param>
        /// <returns></returns>
        public IFeature GetFeature(IFeatureClass pFeaClass, string condition, IGeometry pGeometry, esriSpatialRelEnum pSpatialRel, out Exception eError)
        {
            eError = null;
            if (pFeaClass == null) return null;
            ISpatialFilter pSpatialFilter = new SpatialFilterClass();
            pSpatialFilter.WhereClause = condition;
            if (pGeometry != null)
            {
                pSpatialFilter.Geometry = pGeometry;
                pSpatialFilter.GeometryField = "SHAPE";
                pSpatialFilter.SpatialRel = pSpatialRel;
            }
            try
            {
                IFeatureCursor pFeaCursor = pFeaClass.Search(pSpatialFilter, false);
                IFeature pFeat = pFeaCursor.NextFeature();
                Marshal.ReleaseComObject(pFeaCursor);
                return pFeat;
            }
            catch (Exception eX)
            {
                //********************************
                //guozheng added  system exception log
                if (SysCommon.Log.Module.SysLog == null)
                    SysCommon.Log.Module.SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
                SysCommon.Log.Module.SysLog.Write(eX);
                //********************************
                eError = eX;
                return null;
            }
        }

        /// <summary>
        /// ����������ȡָ��Ҫ��
        /// </summary>
        /// <param name="pFeatCls">FC</param>
        /// <param name="condition">��������</param>
        /// <param name="pGeometry">�ռ�ͼ������</param>
        /// <param name="strSpatialRel">�ռ��ϵ</param>
        /// <param name="eError"></param>
        /// <returns></returns>
        public IFeature GetFeature(IFeatureClass pFeaClass, string condition, IGeometry pGeometry, string strSpatialRel, out Exception eError)
        {
            eError = null;
            if (pFeaClass == null) return null;
            ISpatialFilter pSpatialFilter = new SpatialFilterClass();
            pSpatialFilter.WhereClause = condition;
            if (pGeometry != null)
            {
                pSpatialFilter.Geometry = pGeometry;
                pSpatialFilter.GeometryField = "SHAPE";
                pSpatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelRelation;
                pSpatialFilter.SpatialRelDescription = strSpatialRel;
            }
            try
            {
                IFeatureCursor pFeaCursor = pFeaClass.Search(pSpatialFilter, false);
                IFeature pFeat = pFeaCursor.NextFeature();
                Marshal.ReleaseComObject(pFeaCursor);
                return pFeat;
            }
            catch (Exception eX)
            {
                //********************************
                //guozheng added  system exception log
                if (SysCommon.Log.Module.SysLog == null)
                    SysCommon.Log.Module.SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
                SysCommon.Log.Module.SysLog.Write(eX);
                //********************************
                eError = eX;
                return null;
            }
        }

        /// <summary>
        /// ������ݼ�FeatureClass�Ƿ����
        /// </summary>
        /// <param name="feaclassname">FC����</param>
        /// <param name="FeatureType">FC����</param>
        /// <returns></returns>
        public bool CheckFeatureClassExist(string feaclassname, out string FeatureType, out Exception eError)
        {
            FeatureType = string.Empty;
            eError = null;
            IFeatureClass pFeatureClass = GetFeatureClass(feaclassname, out eError);
            if (pFeatureClass == null) return false;

            switch (pFeatureClass.FeatureType)
            {
                case esriFeatureType.esriFTSimple:
                    switch (pFeatureClass.ShapeType)
                    {
                        case esriGeometryType.esriGeometryPoint:
                            FeatureType = "��";
                            break;
                        case esriGeometryType.esriGeometryPolyline:
                            FeatureType = "��"; ;
                            break;
                        case esriGeometryType.esriGeometryPolygon:
                            FeatureType = "��";
                            break;
                    }

                    break;
                case esriFeatureType.esriFTAnnotation:
                case esriFeatureType.esriFTDimension:
                    FeatureType = "ע��";
                    break;
            }

            return true;
        }

        /// <summary>
        /// ������ݼ�Dataset�Ƿ����
        /// </summary>
        /// <param name="featuredsname">Dataset����</param>
        /// <param name="aDatasetTyp">Dataset����</param>
        /// <returns></returns>
        public bool CheckDatasetExist(string Datasetname, esriDatasetType aDatasetTyp)
        {
            IWorkspace2 pWorkspace2 = (IWorkspace2)this.WorkSpace;
            if (pWorkspace2 == null) return false;
            bool bRes = pWorkspace2.get_NameExists(aDatasetTyp, Datasetname);
            if (bRes == false) return false;
            return true;
        }

        /// <summary>
        /// ����DOM����(RC�ļ�)����
        /// </summary>
        /// <param name="RCDatasetName">DOM���ݼ�(RC)</param>
        /// <param name="filepaths">DOM����(RC�ļ�)·������</param>
        /// <returns></returns>
        public bool InputDOMData(string RCDatasetName, List<string> filepaths, out Exception eError)
        {
            eError = null;
            string NameList = String.Empty;
            foreach (string filepath in filepaths)
            {
                if (NameList.Length == 0)
                {
                    NameList = filepath;
                }
                else
                {
                    NameList = NameList + " ; " + filepath;
                }
            }
            return InputDOMData(RCDatasetName, NameList, out eError);
        }

        /// <summary>
        /// ����DOM����(RC�ļ�)
        /// </summary>
        /// <param name="RCDatasetName">DOM���ݼ�(RC)</param>
        /// <param name="filepaths">DOM����(RC�ļ�)·��</param>
        /// <returns></returns>
        public bool InputDOMData(string RCDatasetName, string filepaths, out Exception eError)
        {
            eError = null;

            try
            {
                IRasterCatalogLoader pRCLoader = new RasterCatalogLoaderClass();
                pRCLoader.Workspace = this.WorkSpace;

                pRCLoader.LoadDatasets(RCDatasetName, filepaths, null);
                return true;
            }
            catch (Exception eX)
            {
                //********************************
                //guozheng added  system exception log
                if (SysCommon.Log.Module.SysLog == null)
                    SysCommon.Log.Module.SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
                SysCommon.Log.Module.SysLog.Write(eX);
                //********************************
                eError = eX;
                return false;
            }
        }

        /// <summary>
        /// ����DEM����(RD�ļ�)����
        /// </summary>
        /// <param name="RDDatasetName">DEM���ݼ�(RD)</param>
        /// <param name="filepaths">DEM����(RD�ļ�)·������</param>
        /// <returns></returns>
        public bool InputDEMData(string RDDatasetName, List<string> filepaths, out Exception eError)
        {
            eError = null;
            RasterToGeodatabase pRasterToGDB = new RasterToGeodatabase();

            string NameList = String.Empty;
            foreach (string filepath in filepaths)
            {
                if (NameList.Length == 0)
                {
                    NameList = filepath;
                }
                else
                {
                    NameList = NameList + " ; " + filepath;
                }
            }
            return InputDEMData(RDDatasetName, NameList, out eError);
        }

        /// <summary>
        /// ����DEM����(RD�ļ�)
        /// </summary>
        /// <param name="RDDatasetName">DEM���ݼ�(RD)<</param>
        /// <param name="filepaths">DEM����(RD�ļ�)·��</param>
        /// <returns></returns>
        public bool InputDEMData(string RDDatasetName, string filepaths, out Exception eError)
        {
            eError = null;
            IRasterDataset pRasterDataset = GetRasterDataset(RDDatasetName, out eError);
            if (pRasterDataset == null) return false;

            RasterToGeodatabase pRasterToGDB = new RasterToGeodatabase();
            pRasterToGDB.Input_Rasters = filepaths;
            pRasterToGDB.Output_Geodatabase = pRasterDataset;

            Geoprocessor pGeoProcessor = new Geoprocessor();
            try
            {
                pGeoProcessor.Execute(pRasterToGDB, null);
            }
            catch (Exception eX)
            {
                //********************************
                //guozheng added  system exception log
                if (SysCommon.Log.Module.SysLog == null)
                    SysCommon.Log.Module.SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
                SysCommon.Log.Module.SysLog.Write(eX);
                //********************************
                eError = eX;
                return false;
            }
            return true;
        }

        /// <summary>
        /// �½�Feature,�����ֶ�ΪDictionary,��������ΪIGeometry
        /// </summary>
        /// <param name="objfcname">FC����</param>
        /// <param name="values">�����ֶ���ֵ</param>
        /// <param name="geomtry">��������</param>
        /// <param name="Edit">�Ƿ����༭</param>
        /// <returns></returns>
        public bool NewFeature(string objfcname, Dictionary<string, object> values, IGeometry geomtry, bool Edit, out Exception eError)
        {
            eError = null;
            if (Edit == true)
            {
                if (this.StartWorkspaceEdit(false, out eError) == false)
                {
                    return false;
                }
            }

            IFeatureClass ObjFeatureCls = GetFeatureClass(objfcname, out eError);
            if (ObjFeatureCls == null) return false;

            try
            {
                IFeature pFeature = ObjFeatureCls.CreateFeature();
                foreach (KeyValuePair<string, object> keyvalue in values)
                {
                    int index = pFeature.Fields.FindField(keyvalue.Key);
                    if (index == -1) continue;

                    pFeature.set_Value(index, keyvalue.Value);
                }

                if (geomtry != null) pFeature.Shape = geomtry;
                pFeature.Store();

                if (Edit == true)
                {
                    if (this.EndWorkspaceEdit(true, out eError) == false)
                    {
                        return false;
                    }
                }
                return true;
            }
            catch (Exception eX)
            {
                //********************************
                //guozheng added  system exception log
                if (SysCommon.Log.Module.SysLog == null)
                    SysCommon.Log.Module.SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
                SysCommon.Log.Module.SysLog.Write(eX);
                //********************************
                if (Edit == true)
                {
                    if (this.EndWorkspaceEdit(false, out eError) == false)
                    {
                        return false;
                    }
                }
                eError = eX;
                return false;
            }
        }

        /// <summary>
        /// �½�Feature,�����ֶ�ΪDictionary,��������ΪIGeometry,�����½�Ҫ�ص�OID
        /// </summary>
        /// <param name="objfcname">Ҫ��������</param>
        /// <param name="values">�����ֶ�</param>
        /// <param name="geomtry">ͼ���ֶ�</param>
        /// <param name="NewOID">��Ҫ��OID</param>
        /// <param name="Edit">�Ƿ����༭</param>
        /// <param name="eError">���ش�����Ϣ</param>
        /// <returns></returns>
        public bool NewFeature(string objfcname, Dictionary<string, object> values, IGeometry geomtry, out int NewOID, bool Edit, out Exception eError)
        {
            eError = null;
            NewOID = -1;
            if (Edit == true)
            {
                if (this.StartWorkspaceEdit(false, out eError) == false)
                {
                    return false;
                }
            }

            IFeatureClass ObjFeatureCls = GetFeatureClass(objfcname, out eError);
            if (ObjFeatureCls == null) return false;

            try
            {
                IFeature pFeature = ObjFeatureCls.CreateFeature();
                foreach (KeyValuePair<string, object> keyvalue in values)
                {
                    int index = pFeature.Fields.FindField(keyvalue.Key);
                    if (index == -1) continue;

                    pFeature.set_Value(index, keyvalue.Value);
                }

                if (geomtry != null) pFeature.Shape = geomtry;
                pFeature.Store();

                NewOID = pFeature.OID;

                if (Edit == true)
                {
                    if (this.EndWorkspaceEdit(true, out eError) == false)
                    {
                        return false;
                    }
                }
                return true;
            }
            catch (Exception eX)
            {
                //********************************
                //guozheng added  system exception log
                if (SysCommon.Log.Module.SysLog == null)
                    SysCommon.Log.Module.SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
                SysCommon.Log.Module.SysLog.Write(eX);
                //********************************
                if (Edit == true)
                {
                    if (this.EndWorkspaceEdit(false, out eError) == false)
                    {
                        return false;
                    }
                }
                eError = eX;
                return false;
            }
        }

        /// <summary>
        /// �½�Feature,�����ֶ�ΪDictionary,��������ΪDictionary
        /// </summary>
        /// <param name="objfcname">FC����</param>
        /// <param name="values">�����ֶ���ֵ</param>
        /// <param name="dicCoor">��������</param>
        /// <param name="Edit">�Ƿ����༭</param>
        /// <returns></returns>
        public bool NewFeature(string objfcname, Dictionary<string, object> values, Dictionary<int, string> dicCoor, bool Edit, out Exception eError)
        {
            eError = null;
            IGeometry geomtry = null;
            IPolygon pPolygon;
            // �������ֵ�õ���Χgeomtry
            if (ModGisPub.GetPolygonByCol(dicCoor, out pPolygon, out eError))
            {
                geomtry = (IGeometry)pPolygon;
            }
            else
                return false;

            return NewFeature(objfcname, values, geomtry, Edit, out eError);
        }

        /// <summary>
        /// �½�Feature,�����ֶ�ΪDictionary,��������ΪIGeometry
        /// </summary>
        /// <param name="objfcname">FC����</param>
        /// <param name="values">�����ֶ���ֵ</param>
        /// <param name="geomtry">��������</param>
        /// <param name="Edit">�Ƿ����༭</param>
        /// <returns></returns>
        public bool NewFeature(IFeatureClass ObjFeatureCls, Dictionary<string, object> values, IGeometry geomtry, bool Edit, out Exception eError)
        {
            eError = null;
            if (ObjFeatureCls == null) return false;

            if (Edit == true)
            {
                if (this.StartWorkspaceEdit(false, out eError) == false)
                {
                    return false;
                }
            }

            try
            {
                IFeature pFeature = ObjFeatureCls.CreateFeature();
                foreach (KeyValuePair<string, object> keyvalue in values)
                {
                    int index = pFeature.Fields.FindField(keyvalue.Key);
                    if (index == -1) continue;

                    pFeature.set_Value(index, keyvalue.Value);
                }

                if (geomtry != null) pFeature.Shape = geomtry;
                pFeature.Store();

                if (Edit == true)
                {
                    if (this.EndWorkspaceEdit(true, out eError) == false)
                    {
                        return false;
                    }
                }
                return true;
            }
            catch (Exception eX)
            {
                //********************************
                //guozheng added  system exception log
                if (SysCommon.Log.Module.SysLog == null)
                    SysCommon.Log.Module.SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
                SysCommon.Log.Module.SysLog.Write(eX);
                //********************************
                if (Edit == true)
                {
                    if (this.EndWorkspaceEdit(false, out eError) == false)
                    {
                        return false;
                    }
                }
                eError = eX;
                return false;
            }
        }

        /// <summary>
        /// �½�Feature,�����ֶ�ΪDictionary,��������ΪIGeometry,�����½�Ҫ�ص�OID
        /// </summary>
        /// <param name="objfcname">Ҫ��������</param>
        /// <param name="values">�����ֶ�</param>
        /// <param name="geomtry">ͼ���ֶ�</param>
        /// <param name="NewOID">��Ҫ��OID</param>
        /// <param name="Edit">�Ƿ����༭</param>
        /// <param name="eError">���ش�����Ϣ</param>
        /// <returns></returns>
        public bool NewFeature(IFeatureClass ObjFeatureCls, Dictionary<string, object> values, IGeometry geomtry, out int NewOID, bool Edit, out Exception eError)
        {
            eError = null;
            NewOID = -1;
            if (ObjFeatureCls == null) return false;

            if (Edit == true)
            {
                if (this.StartWorkspaceEdit(false, out eError) == false)
                {
                    return false;
                }
            }

            try
            {
                IFeature pFeature = ObjFeatureCls.CreateFeature();
                foreach (KeyValuePair<string, object> keyvalue in values)
                {
                    int index = pFeature.Fields.FindField(keyvalue.Key);
                    if (index == -1) continue;

                    pFeature.set_Value(index, keyvalue.Value);
                }

                if (geomtry != null) pFeature.Shape = geomtry;
                pFeature.Store();

                NewOID = pFeature.OID;

                if (Edit == true)
                {
                    if (this.EndWorkspaceEdit(true, out eError) == false)
                    {
                        return false;
                    }
                }
                return true;
            }
            catch (Exception eX)
            {
                //********************************
                //guozheng added  system exception log
                if (SysCommon.Log.Module.SysLog == null)
                    SysCommon.Log.Module.SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
                SysCommon.Log.Module.SysLog.Write(eX);
                //********************************
                if (Edit == true)
                {
                    if (this.EndWorkspaceEdit(false, out eError) == false)
                    {
                        return false;
                    }
                }
                eError = eX;
                return false;
            }
        }

        /// <summary>
        /// �½�Feature,�����ֶ�ΪDictionary,��������ΪDictionary
        /// </summary>
        /// <param name="objfcname">FC����</param>
        /// <param name="values">�����ֶ���ֵ</param>
        /// <param name="dicCoor">��������</param>
        /// <param name="Edit">�Ƿ����༭</param>
        /// <returns></returns>
        public bool NewFeature(IFeatureClass ObjFeatureCls, Dictionary<string, object> values, Dictionary<int, string> dicCoor, bool Edit, out Exception eError)
        {
            eError = null;
            IGeometry geomtry = null;
            IPolygon pPolygon;
            // �������ֵ�õ���Χgeomtry
            if (ModGisPub.GetPolygonByCol(dicCoor, out pPolygon, out eError))
            {
                geomtry = (IGeometry)pPolygon;
            }
            else
                return false;

            return NewFeature(ObjFeatureCls, values, geomtry, Edit, out eError);
        }

        /// <summary>
        /// �½�Features,�����ֶ�ΪDictionary,��������ΪԴ���ݵ�Geometry
        /// </summary>
        /// <param name="objfcname">FC����</param>
        /// <param name="pfeacursor">Դ����</param>
        /// <param name="values">�ֶμ���</param>
        /// <param name="Edit">�Ƿ����༭</param>
        /// <param name="bIngore">�Ƿ���Դ��������һѭ��</param>
        /// <returns></returns>
        public bool NewFeatures(IFeatureClass ObjFeatureCls, IFeatureCursor pfeacursor, Dictionary<string, object> values, bool Edit, bool bIngore, out Exception eError)
        {
            eError = null;
            if (ObjFeatureCls == null) return false;

            if (Edit == true)
            {
                if (this.StartWorkspaceEdit(false, out eError) == false)
                {
                    return false;
                }
            }

            IFeatureBuffer pFeatureBuffer = ObjFeatureCls.CreateFeatureBuffer();
            IFeatureCursor pObjFeaCursor = ObjFeatureCls.Insert(true);

            IFeature pFeature = pfeacursor.NextFeature();
            while (pFeature != null)
            {
                try
                {
                    if (values != null)
                    {
                        foreach (KeyValuePair<string, object> keyvalue in values)
                        {
                            int index = pFeatureBuffer.Fields.FindField(keyvalue.Key);
                            if (index == -1) continue;

                            pFeatureBuffer.set_Value(index, keyvalue.Value);
                        }
                    }

                    pFeatureBuffer.Shape = pFeature.ShapeCopy;
                    pObjFeaCursor.InsertFeature(pFeatureBuffer);

                    pFeature = pfeacursor.NextFeature();
                }
                catch (Exception eX)
                {
                    //********************************
                    //guozheng added  system exception log
                    if (SysCommon.Log.Module.SysLog == null)
                        SysCommon.Log.Module.SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
                    SysCommon.Log.Module.SysLog.Write(eX);
                    //********************************
                    eError = eX;
                    if (bIngore == false)
                        break;
                    else
                        pFeature = pfeacursor.NextFeature();
                    continue;
                }
            }

            pObjFeaCursor.Flush();
            Marshal.ReleaseComObject(pObjFeaCursor);

            if (Edit == true)
            {
                if (eError != null)
                {
                    if (this.EndWorkspaceEdit(bIngore, out eError) == false)
                    {
                        return false;
                    }
                    return false;
                }
                else
                {
                    if (this.EndWorkspaceEdit(true, out eError) == false)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// �½�Features,�����ֶβ��ֲ���Dictionary��������Դ����(��ȡֵ������ͻ��Դ����Ϊ׼),��������ΪԴ���ݵ�Geometry
        /// </summary>
        /// <param name="objfcname">FC����</param>
        /// <param name="pfeacursor">Դ����</param>
        /// <param name="FieldNames">����Դ�����ֶ�ֵ���ֶ����б�</param>
        /// <param name="values">�ֶμ���</param>
        /// <param name="Edit">�Ƿ����༭</param>
        /// <returns></returns>
        public bool NewFeatures(IFeatureClass ObjFeatureCls, IFeatureCursor pfeacursor, List<string> FieldNames, Dictionary<string, object> values, bool Edit, bool bIngore, out Exception eError)
        {
            eError = null;
            if (ObjFeatureCls == null) return false;

            if (Edit == true)
            {
                if (this.StartWorkspaceEdit(false, out eError) == false)
                {
                    return false;
                }
            }

            IFeatureBuffer pFeatureBuffer = ObjFeatureCls.CreateFeatureBuffer();
            IFeatureCursor pObjFeaCursor = ObjFeatureCls.Insert(true);

            IFeature pFeature = pfeacursor.NextFeature();
            while (pFeature != null)
            {
                try
                {
                    if (values != null)
                    {
                        foreach (KeyValuePair<string, object> keyvalue in values)
                        {
                            int index = pFeatureBuffer.Fields.FindField(keyvalue.Key);
                            if (index == -1) continue;

                            pFeatureBuffer.set_Value(index, keyvalue.Value);
                        }
                    }

                    if (FieldNames != null)
                    {
                        foreach (string fieldname in FieldNames)
                        {
                            int index = pFeature.Fields.FindField(fieldname);
                            int ObjIndex = pFeatureBuffer.Fields.FindField(fieldname);
                            if (index == -1 || ObjIndex == -1) continue;

                            pFeatureBuffer.set_Value(ObjIndex, pFeature.get_Value(index));
                        }
                    }

                    pFeatureBuffer.Shape = pFeature.ShapeCopy;
                    pObjFeaCursor.InsertFeature(pFeatureBuffer);

                    pFeature = pfeacursor.NextFeature();
                }
                catch (Exception eX)
                {
                    //********************************
                    //guozheng added  system exception log
                    if (SysCommon.Log.Module.SysLog == null)
                        SysCommon.Log.Module.SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
                    SysCommon.Log.Module.SysLog.Write(eX);
                    //********************************
                    eError = eX;
                    if (bIngore == false)
                        break;
                    else
                        pFeature = pfeacursor.NextFeature();
                    continue;
                }
            }

            pObjFeaCursor.Flush();
            Marshal.ReleaseComObject(pObjFeaCursor);

            if (Edit == true)
            {
                if (eError != null)
                {
                    if (this.EndWorkspaceEdit(bIngore, out eError) == false)
                    {
                        return false;
                    }
                    return false;
                }
                else
                {
                    if (this.EndWorkspaceEdit(true, out eError) == false)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// �½�Features,�����ֶβ��ֲ���Dictionary��������Դ����(��ȡֵ������ͻ��Դ����Ϊ׼),��������ΪԴ���ݵ�Geometry
        /// </summary>
        /// <param name="objfcname">FC����</param>
        /// <param name="pfeacursor">Դ����</param>
        /// <param name="dicFieldsPair">����Դ�����ֶ�ֵ����Ŀ���ֶζ�Ӧ��Dictionary,keyΪԴ�����ֶ���,valueΪĿ���ֶ���</param>
        /// <param name="values">�ֶμ���</param>
        /// <param name="Edit">�Ƿ����༭</param>
        /// <returns></returns>
        public bool NewFeatures(IFeatureClass ObjFeatureCls, IFeatureCursor pfeacursor, Dictionary<string, string> dicFieldsPair, Dictionary<string, object> values, bool Edit, bool bIngore, out Exception eError)
        {
            eError = null;
            if (ObjFeatureCls == null) return false;

            if (Edit == true)
            {
                if (this.StartWorkspaceEdit(false, out eError) == false)
                {
                    return false;
                }
            }

            IFeatureBuffer pFeatureBuffer = ObjFeatureCls.CreateFeatureBuffer();
            IFeatureCursor pObjFeaCursor = ObjFeatureCls.Insert(true);

            IFeature pFeature = pfeacursor.NextFeature();
            while (pFeature != null)
            {
                try
                {
                    if (values != null)
                    {
                        foreach (KeyValuePair<string, object> keyvalue in values)
                        {
                            int index = pFeatureBuffer.Fields.FindField(keyvalue.Key);
                            if (index == -1) continue;

                            pFeatureBuffer.set_Value(index, keyvalue.Value);
                        }
                    }

                    if (dicFieldsPair != null)
                    {
                        foreach (KeyValuePair<string, string> keyvalue in dicFieldsPair)
                        {
                            int index = pFeature.Fields.FindField(keyvalue.Key);
                            int ObjIndex = pFeatureBuffer.Fields.FindField(keyvalue.Value);
                            if (index == -1 || ObjIndex == -1) continue;

                            pFeatureBuffer.set_Value(ObjIndex, pFeature.get_Value(index));
                        }
                    }

                    pFeatureBuffer.Shape = pFeature.ShapeCopy;
                    pObjFeaCursor.InsertFeature(pFeatureBuffer);

                    pFeature = pfeacursor.NextFeature();
                }
                catch (Exception eX)
                {
                    //********************************
                    //guozheng added  system exception log
                    if (SysCommon.Log.Module.SysLog == null)
                        SysCommon.Log.Module.SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
                    SysCommon.Log.Module.SysLog.Write(eX);
                    //********************************
                    eError = eX;
                    if (bIngore == false)
                        break;
                    else
                        pFeature = pfeacursor.NextFeature();
                    continue;
                }
            }

            pObjFeaCursor.Flush();
            Marshal.ReleaseComObject(pObjFeaCursor);

            if (Edit == true)
            {
                if (eError != null)
                {
                    if (this.EndWorkspaceEdit(bIngore, out eError) == false)
                    {
                        return false;
                    }
                    return false;
                }
                else
                {
                    if (this.EndWorkspaceEdit(true, out eError) == false)
                    {
                        return false;
                    }
                }
            }

            return true;
        }


        /// <summary>
        /// �½�Features,�����ֶ�ΪDictionary,��������ΪԴ���ݵ�Geometry
        /// </summary>
        /// <param name="objfcname">FC����</param>
        /// <param name="pfeacursor">Դ����</param>
        /// <param name="values">�ֶμ���</param>
        /// <param name="Edit">�Ƿ����༭</param>
        /// <param name="bIngore">�Ƿ���Դ��������һѭ��</param>
        /// <returns></returns>
        public bool NewFeatures(string objfcname, IFeatureCursor pfeacursor, Dictionary<string, object> values, bool useOrgFdVal, bool Edit, bool bIngore, out Exception eError)
        {
            eError = null;

            if (Edit == true)
            {
                if (this.StartWorkspaceEdit(false, out eError) == false)
                {
                    return false;
                }
            }

            IFeatureClass ObjFeatureCls = GetFeatureClass(objfcname, out eError);
            if (ObjFeatureCls == null) return false;
            IFeatureBuffer pFeatureBuffer = ObjFeatureCls.CreateFeatureBuffer();
            IFeatureCursor pObjFeaCursor = ObjFeatureCls.Insert(true);

            IFeature pFeature = pfeacursor.NextFeature();
            while (pFeature != null)
            {
                try
                {
                    if (values != null)
                    {
                        foreach (KeyValuePair<string, object> keyvalue in values)
                        {
                            int index = pFeatureBuffer.Fields.FindField(keyvalue.Key);
                            if (index == -1) continue;

                            pFeatureBuffer.set_Value(index, keyvalue.Value);
                        }
                    }

                    if (useOrgFdVal)
                    {
                        for (int i = 0; i < pFeature.Fields.FieldCount; i++)
                        {
                            IField aField = pFeature.Fields.get_Field(i);
                            if (aField.Type != esriFieldType.esriFieldTypeGeometry && aField.Type != esriFieldType.esriFieldTypeOID && aField.Editable)
                            {
                                int ObjIndex = pFeatureBuffer.Fields.FindField(aField.Name);
                                if (ObjIndex == -1) continue;

                                pFeatureBuffer.set_Value(ObjIndex, pFeature.get_Value(i));
                            }
                        }
                    }

                    pFeatureBuffer.Shape = pFeature.ShapeCopy;
                    pObjFeaCursor.InsertFeature(pFeatureBuffer);

                    pFeature = pfeacursor.NextFeature();
                }
                catch (Exception eX)
                {
                    //********************************
                    //guozheng added  system exception log
                    if (SysCommon.Log.Module.SysLog == null)
                        SysCommon.Log.Module.SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
                    SysCommon.Log.Module.SysLog.Write(eX);
                    //********************************
                    eError = eX;
                    if (bIngore == false)
                        break;
                    else
                        pFeature = pfeacursor.NextFeature();
                    continue;
                }
            }

            pObjFeaCursor.Flush();
            Marshal.ReleaseComObject(pObjFeaCursor);

            if (Edit == true)
            {
                if (eError != null)
                {
                    if (this.EndWorkspaceEdit(bIngore, out eError) == false)
                    {
                        return false;
                    }
                    return false;
                }
                else
                {
                    if (this.EndWorkspaceEdit(true, out eError) == false)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// �½�Features,�����ֶβ��ֲ���Dictionary��������Դ����(��ȡֵ������ͻ��Դ����Ϊ׼),��������ΪԴ���ݵ�Geometry
        /// </summary>
        /// <param name="objfcname">FC����</param>
        /// <param name="pfeacursor">Դ����</param>
        /// <param name="FieldNames">����Դ�����ֶ�ֵ���ֶ����б�</param>
        /// <param name="values">�ֶμ���</param>
        /// <param name="Edit">�Ƿ����༭</param>
        /// <returns></returns>
        public bool NewFeatures(string objfcname, IFeatureCursor pfeacursor, List<string> FieldNames, Dictionary<string, object> values, bool Edit, bool bIngore, out Exception eError)
        {
            eError = null;

            if (Edit == true)
            {
                if (this.StartWorkspaceEdit(false, out eError) == false)
                {
                    return false;
                }
            }

            IFeatureClass ObjFeatureCls = GetFeatureClass(objfcname, out eError);
            if (ObjFeatureCls == null) return false;
            IFeatureBuffer pFeatureBuffer = ObjFeatureCls.CreateFeatureBuffer();
            IFeatureCursor pObjFeaCursor = ObjFeatureCls.Insert(true);

            IFeature pFeature = pfeacursor.NextFeature();
            while (pFeature != null)
            {
                try
                {
                    if (values != null)
                    {
                        foreach (KeyValuePair<string, object> keyvalue in values)
                        {
                            int index = pFeatureBuffer.Fields.FindField(keyvalue.Key);
                            if (index == -1) continue;

                            pFeatureBuffer.set_Value(index, keyvalue.Value);
                        }
                    }

                    if (FieldNames != null)
                    {
                        foreach (string fieldname in FieldNames)
                        {
                            int index = pFeature.Fields.FindField(fieldname);
                            int ObjIndex = pFeatureBuffer.Fields.FindField(fieldname);
                            if (index == -1 || ObjIndex == -1) continue;

                            pFeatureBuffer.set_Value(ObjIndex, pFeature.get_Value(index));
                        }
                    }

                    pFeatureBuffer.Shape = pFeature.ShapeCopy;
                    pObjFeaCursor.InsertFeature(pFeatureBuffer);

                    pFeature = pfeacursor.NextFeature();
                }
                catch (Exception eX)
                {
                    //********************************
                    //guozheng added  system exception log
                    if (SysCommon.Log.Module.SysLog == null)
                        SysCommon.Log.Module.SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
                    SysCommon.Log.Module.SysLog.Write(eX);
                    //********************************
                    eError = eX;
                    if (bIngore == false)
                        break;
                    else
                        pFeature = pfeacursor.NextFeature();
                    continue;
                }
            }

            pObjFeaCursor.Flush();
            Marshal.ReleaseComObject(pObjFeaCursor);

            if (Edit == true)
            {
                if (eError != null)
                {
                    if (this.EndWorkspaceEdit(bIngore, out eError) == false)
                    {
                        return false;
                    }
                    return false;
                }
                else
                {
                    if (this.EndWorkspaceEdit(true, out eError) == false)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// �½�Features,�����ֶβ��ֲ���Dictionary��������Դ����(��ȡֵ������ͻ��Դ����Ϊ׼),��������ΪԴ���ݵ�Geometry
        /// </summary>
        /// <param name="objfcname">FC����</param>
        /// <param name="pfeacursor">Դ����</param>
        /// <param name="dicFieldsPair">����Դ�����ֶ�ֵ����Ŀ���ֶζ�Ӧ��Dictionary,keyΪԴ�����ֶ���,valueΪĿ���ֶ���</param>
        /// <param name="values">�ֶμ���</param>
        /// <param name="Edit">�Ƿ����༭</param>
        /// <returns></returns>
        public bool NewFeatures(string objfcname, IFeatureCursor pfeacursor, Dictionary<string, string> dicFieldsPair, Dictionary<string, object> values, bool Edit, bool bIngore, out Exception eError)
        {
            eError = null;

            if (Edit == true)
            {
                if (this.StartWorkspaceEdit(false, out eError) == false)
                {
                    return false;
                }
            }

            IFeatureClass ObjFeatureCls = GetFeatureClass(objfcname, out eError);
            if (ObjFeatureCls == null) return false;
            IFeatureBuffer pFeatureBuffer = ObjFeatureCls.CreateFeatureBuffer();
            IFeatureCursor pObjFeaCursor = ObjFeatureCls.Insert(true);

            IFeature pFeature = pfeacursor.NextFeature();
            while (pFeature != null)
            {
                try
                {
                    if (values != null)
                    {
                        foreach (KeyValuePair<string, object> keyvalue in values)
                        {
                            int index = pFeatureBuffer.Fields.FindField(keyvalue.Key);
                            if (index == -1) continue;

                            pFeatureBuffer.set_Value(index, keyvalue.Value);
                        }
                    }

                    if (dicFieldsPair != null)
                    {
                        foreach (KeyValuePair<string, string> keyvalue in dicFieldsPair)
                        {
                            int index = pFeature.Fields.FindField(keyvalue.Key);
                            int ObjIndex = pFeatureBuffer.Fields.FindField(keyvalue.Value);
                            if (index == -1 || ObjIndex == -1) continue;

                            pFeatureBuffer.set_Value(ObjIndex, pFeature.get_Value(index));
                        }
                    }

                    pFeatureBuffer.Shape = pFeature.ShapeCopy;
                    pObjFeaCursor.InsertFeature(pFeatureBuffer);

                    pFeature = pfeacursor.NextFeature();
                }
                catch (Exception eX)
                {
                    //********************************
                    //guozheng added  system exception log
                    if (SysCommon.Log.Module.SysLog == null)
                        SysCommon.Log.Module.SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
                    SysCommon.Log.Module.SysLog.Write(eX);
                    //********************************
                    eError = eX;
                    if (bIngore == false)
                        break;
                    else
                        pFeature = pfeacursor.NextFeature();
                    continue;
                }
            }

            pObjFeaCursor.Flush();
            Marshal.ReleaseComObject(pObjFeaCursor);

            if (Edit == true)
            {
                if (eError != null)
                {
                    if (this.EndWorkspaceEdit(bIngore, out eError) == false)
                    {
                        return false;
                    }
                    return false;
                }
                else
                {
                    if (this.EndWorkspaceEdit(true, out eError) == false)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// �༭Feature
        /// </summary>
        /// <param name="objfcname">FC����</param>
        /// <param name="condition">����</param>
        /// <param name="values">�ֶμ���</param>
        /// <param name="dicCoor">��������</param>
        /// <param name="Edit">�Ƿ����༭</param>
        /// <returns></returns>
        public bool EditFeature(string objfcname, string condition, Dictionary<string, object> values, Dictionary<int, string> dicCoor, bool Edit, out Exception eError)
        {
            IGeometry geomtry = null;
            IPolygon pPolygon;
            // �������ֵ�õ���Χgeomtry
            if (ModGisPub.GetPolygonByCol(dicCoor, out pPolygon, out eError))
            {
                geomtry = (IGeometry)pPolygon;
            }
            else
                return false;

            return EditFeature(objfcname, condition, values, geomtry, Edit, out eError);
        }

        /// <summary>
        /// �༭Feature
        /// </summary>
        /// <param name="objfcname">FC����</param>
        /// <param name="condition">����</param>
        /// <param name="values">�ֶμ���</param>
        /// <param name="geomtry">��������</param>
        /// <param name="Edit">�Ƿ����༭</param>
        /// <returns></returns>
        public bool EditFeature(string objfcname, string condition, Dictionary<string, object> values, IGeometry geomtry, bool Edit, out Exception eError)
        {
            eError = null;

            if (Edit == true)
            {
                if (this.StartWorkspaceEdit(false, out eError) == false)
                {
                    return false;
                }
            }

            IFeatureCursor pFeatureCursor = GetFeatureCursor(objfcname, condition, out eError);
            if (pFeatureCursor == null) return false;
            IFeature pFeature = pFeatureCursor.NextFeature();
            if (pFeature == null)
            {
                eError = new Exception("Ҫ�ز�����");
                return false;
            }

            try
            {
                foreach (KeyValuePair<string, object> keyvalue in values)
                {
                    int index = pFeature.Fields.FindField(keyvalue.Key);
                    if (index == -1) continue;

                    pFeature.set_Value(index, keyvalue.Value);
                }

                if (geomtry != null) pFeature.Shape = geomtry;
                pFeature.Store();

                Marshal.ReleaseComObject(pFeatureCursor);
                if (Edit == true)
                {
                    if (this.EndWorkspaceEdit(true, out eError) == false)
                    {
                        return false;
                    }
                }
                return true;
            }
            catch (Exception eX)
            {
                //********************************
                //guozheng added  system exception log
                if (SysCommon.Log.Module.SysLog == null)
                    SysCommon.Log.Module.SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
                SysCommon.Log.Module.SysLog.Write(eX);
                //********************************
                if (Edit == true)
                {
                    if (this.EndWorkspaceEdit(false, out eError) == false)
                    {
                        return false;
                    }
                }
                eError = eX;
                return false;
            }
        }

        /// <summary>
        /// �༭Feature
        /// </summary>
        /// <param name="objfcname">FC����</param>
        /// <param name="condition">����</param>
        /// <param name="values">�ֶμ���</param>
        /// <param name="geomtry">��������</param>
        /// <param name="Edit">�Ƿ����༭</param>
        /// <returns></returns>
        public IFeature GetEditFeature(string objfcname, string condition, Dictionary<string, object> values, IGeometry geomtry, bool Edit, out Exception eError)
        {
            eError = null;

            if (Edit == true)
            {
                if (this.StartWorkspaceEdit(false, out eError) == false)
                {
                    return null;
                }
            }

            IFeatureCursor pFeatureCursor = GetFeatureCursor(objfcname, condition, out eError);
            if (pFeatureCursor == null) return null;
            IFeature pFeature = pFeatureCursor.NextFeature();
            if (pFeature == null)
            {
                eError = new Exception("Ҫ�ز�����");
                return null;
            }

            try
            {
                foreach (KeyValuePair<string, object> keyvalue in values)
                {
                    int index = pFeature.Fields.FindField(keyvalue.Key);
                    if (index == -1) continue;

                    pFeature.set_Value(index, keyvalue.Value);
                }

                if (geomtry != null) pFeature.Shape = geomtry;
                pFeature.Store();

                Marshal.ReleaseComObject(pFeatureCursor);
                if (Edit == true)
                {
                    if (this.EndWorkspaceEdit(true, out eError) == false)
                    {
                        return null;
                    }
                }
                return pFeature;
            }
            catch (Exception eX)
            {
                //********************************
                //guozheng added  system exception log
                if (SysCommon.Log.Module.SysLog == null)
                    SysCommon.Log.Module.SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
                SysCommon.Log.Module.SysLog.Write(eX);
                //********************************
                if (Edit == true)
                {
                    if (this.EndWorkspaceEdit(false, out eError) == false)
                    {
                        return null;
                    }
                }
                eError = eX;
                return null;
            }
        }

        /// <summary>
        /// �༭Features
        /// </summary>
        /// <param name="objfcname">FC����</param>
        /// <param name="condition">����</param>
        /// <param name="values">�ֶμ���</param>
        /// <param name="Edit">�Ƿ����༭</param>
        /// <returns></returns>
        public bool EditFeatures(string objfcname, string condition, Dictionary<string, object> values, bool bEdit, out Exception eError)
        {
            eError = null;

            if (bEdit == true)
            {
                if (this.StartWorkspaceEdit(false, out eError) == false) return false;
            }

            IFeatureClass pFeaClass = GetFeatureClass(objfcname, out eError);
            if (pFeaClass == null) return false;
            IQueryFilter pQueryFilter = new QueryFilterClass();
            pQueryFilter.WhereClause = condition;
            IFeatureCursor pFeatureCursor = pFeaClass.Update(pQueryFilter, false);
            if (pFeatureCursor == null) return false;
            IFeature pFeature = pFeatureCursor.NextFeature();

            try
            {
                while (pFeature != null)
                {
                    foreach (KeyValuePair<string, object> keyvalue in values)
                    {
                        int index = pFeature.Fields.FindField(keyvalue.Key);
                        if (index == -1) continue;
                        pFeature.set_Value(index, keyvalue.Value);
                    }

                    pFeatureCursor.UpdateFeature(pFeature);
                    pFeature = pFeatureCursor.NextFeature();
                }

                Marshal.ReleaseComObject(pFeatureCursor);
                if (bEdit == true)
                {
                    if (this.EndWorkspaceEdit(true, out eError) == false) return false;
                }

                return true;
            }
            catch (Exception eX)
            {
                //********************************
                //guozheng added  system exception log
                if (SysCommon.Log.Module.SysLog == null)
                    SysCommon.Log.Module.SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
                SysCommon.Log.Module.SysLog.Write(eX);
                //********************************
                if (bEdit == true)
                {
                    if (this.EndWorkspaceEdit(false, out eError) == false)
                    {
                        return false;
                    }
                }
                eError = eX;
                return false;
            }
        }

        /// <summary>
        /// �õ�Ҫ����������������ֶ���������������ֵ�
        /// </summary>
        /// <param name="fcname"> Ҫ��������</param>
        /// <param name="bEdit"> �Ƿ���Ӳ��ɱ༭�ֶ�</param>
        /// <param name="wstype"> ����������</param>
        /// <returns> �ֶ���������������ֵ�</returns>
        public Dictionary<string, int> GetFieldIndexs(IFeatureClass pFeaClass, bool bEdit)
        {
            if (pFeaClass == null) return null;
            Dictionary<string, int> FieldIndexs = new Dictionary<string, int>();
            // ��ȡҪ�������ֶμ��ϲ�ѭ��
            IFields pFields = pFeaClass.Fields;
            for (int index = 0; index < pFields.FieldCount; index++)
            {
                IField pField = pFields.get_Field(index);

                if (!bEdit)
                {
                    if (!pField.Editable) continue;

                }

                if (pField.Type == esriFieldType.esriFieldTypeGeometry || pField.Type == esriFieldType.esriFieldTypeOID) continue;
                FieldIndexs.Add(pField.Name.ToUpper(), index);
            }
            return FieldIndexs;
        }

        /// <summary>
        /// �õ�Ҫ����������������ֶ���������������ֵ�
        /// </summary>
        /// <param name="fcname"> Ҫ��������</param>
        /// <param name="bEdit"> �Ƿ���Ӳ��ɱ༭�ֶ�</param>
        /// <param name="wstype"> ����������</param>
        /// <returns> �ֶ���������������ֵ�</returns>
        public Dictionary<string, int> GetFieldIndexs(string fcname, bool bEdit)
        {
            // ��Ҫ����
            int pos = fcname.IndexOf("\\");
            if (pos >= 1) fcname = fcname.Substring(pos + 1);

            Exception errEx = null;
            IFeatureClass pFeaClass = GetFeatureClass(fcname, out errEx);
            if (pFeaClass == null) return null;

            Dictionary<string, int> FieldIndexs = new Dictionary<string, int>();
            // ��ȡҪ�������ֶμ��ϲ�ѭ��
            IFields pFields = pFeaClass.Fields;
            for (int index = 0; index < pFields.FieldCount; index++)
            {
                IField pField = pFields.get_Field(index);

                if (!bEdit)
                {
                    if (!pField.Editable) continue;

                }
                if (pField.Type == esriFieldType.esriFieldTypeGeometry || pField.Type == esriFieldType.esriFieldTypeOID) continue;
                FieldIndexs.Add(pField.Name.ToUpper(), index);
            }
            return FieldIndexs;
        }

        public bool DeleteRows(string tablename, string condition, out Exception eError)
        {
            eError = null;
            //�õ�FeatrueWS
            IFeatureWorkspace pFeaWS = (IFeatureWorkspace)this.WorkSpace;

            try
            {
                ITable pTable = pFeaWS.OpenTable(tablename);
                IQueryFilter pQueryFilter = new QueryFilterClass();
                pQueryFilter.WhereClause = condition;
                pTable.DeleteSearchedRows(pQueryFilter);

                return true;
            }
            catch (Exception eX)
            {
                //********************************
                //guozheng added  system exception log
                if (SysCommon.Log.Module.SysLog == null)
                    SysCommon.Log.Module.SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
                SysCommon.Log.Module.SysLog.Write(eX);
                //********************************
                eError = eX;
                return false;
            }
        }

        public bool DeleteRows(string tablename, IQueryFilter pQueryFilter, out Exception eError)
        {
            eError = null;
            //�õ�FeatrueWS
            IFeatureWorkspace pFeaWS = (IFeatureWorkspace)this.WorkSpace;

            try
            {
                ITable pTable = pFeaWS.OpenTable(tablename);
                pTable.DeleteSearchedRows(pQueryFilter);

                return true;
            }
            catch (Exception eX)
            {
                //********************************
                //guozheng added  system exception log
                if (SysCommon.Log.Module.SysLog == null)
                    SysCommon.Log.Module.SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
                SysCommon.Log.Module.SysLog.Write(eX);
                //********************************
                eError = eX;
                return false;
            }
        }
    }

    public class SysGisTable : SysGisDB, IGisTable
    {
        public SysGisTable()
        {

        }

        public SysGisTable(IWorkspace pWorkspace)
        {
            this.WorkSpace = pWorkspace;
        }

        public SysGisTable(SysGisDB gisDb)
        {
            this.WorkSpace = gisDb.WorkSpace;
        }

        #region IGisTable ��Ա

        public bool CreateTable(string sTableName, IFields pFields, out Exception eError)
        {
            eError = null;
            //�õ�FeatrueWS
            IFeatureWorkspace pFeaWS = (IFeatureWorkspace)this.WorkSpace;
            IObjectClassDescription pOCD=new ObjectClassDescriptionClass();
            try
            {
               ITable pTable=pFeaWS.CreateTable(sTableName, pFields,pOCD.InstanceCLSID,null,"");
               if (pTable == null)
                   return false;
               return true;
            }
            catch (Exception eX)
            {
                //********************************
                //guozheng added  system exception log
                if (SysCommon.Log.Module.SysLog == null)
                    SysCommon.Log.Module.SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
                SysCommon.Log.Module.SysLog.Write(eX);
                //********************************
                eError = eX;
                return false;
            }
        }

        public ITable OpenTable(string tablename, out Exception eError)
        {
            eError = null;
     
            try
            {
                //�õ�FeatrueWS
                IFeatureWorkspace pFeaWS = (IFeatureWorkspace)this.WorkSpace;
                return pFeaWS.OpenTable(tablename);
            }
            catch (Exception eX)
            {
                //********************************
                //guozheng added  system exception log
                if (SysCommon.Log.Module.SysLog == null)
                    SysCommon.Log.Module.SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
                SysCommon.Log.Module.SysLog.Write(eX);
                //********************************
                eError = eX;
                return null;
            }
        }

        public System.Collections.ArrayList GetUniqueValue(string tablename, string fieldname, string condition, out Exception eError)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public Dictionary<string, Type> GetFieldsType(string tablename, out Exception eError)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public Dictionary<string, object> GetRow(string tablename, string condition, out Exception eError)
        {
            eError = null;
            //�õ�FeatrueWS
            try
            {
                Dictionary<string, object> dicValue = new Dictionary<string, object>();
                ITable pTable = OpenTable(tablename, out eError);
                if (eError != null) return null;
                IQueryFilter pQueryFilter = new QueryFilterClass();
                pQueryFilter.WhereClause = condition;
                ICursor pCursor = pTable.Search(pQueryFilter, true);
                IRow pRow = pCursor.NextRow();
                if (pRow == null)
                {
                    Marshal.ReleaseComObject(pCursor);
                    return null;
                }

                for (int i = 0; i < pRow.Fields.FieldCount; i++)
                {
                    dicValue.Add(pRow.Fields.get_Field(i).Name, pRow.get_Value(i));
                }

                Marshal.ReleaseComObject(pCursor);
                return dicValue;
            }
            catch (Exception eX)
            {
                //********************************
                //guozheng added  system exception log
                if (SysCommon.Log.Module.SysLog == null)
                    SysCommon.Log.Module.SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
                SysCommon.Log.Module.SysLog.Write(eX);
                //********************************
                eError = eX;
                return null;
            }
        }

        /// <summary>
        /// ��ȡ���Ա��ж��ֶζ�����¼
        /// </summary>
        /// <param name="tablename"></param>
        /// <param name="condition"></param>
        /// <param name="eError"></param>
        /// <returns></returns>
        public List<Dictionary<string, object>> GetRows(string tablename, string condition, out Exception eError)
        {
            try
            {
                ITable table = OpenTable(tablename, out eError);
                List<Dictionary<string, object>> lstDicData = null;

                if (table != null)
                {
                    lstDicData = new List<Dictionary<string, object>>();
                    IQueryFilter pQueryFilter = new QueryFilterClass();
                    pQueryFilter.WhereClause = condition;

                    ICursor pCursor = table.Search(pQueryFilter, true);
                    if (pCursor != null)
                    {
                        IRow row = pCursor.NextRow();
                        while (row != null)
                        {
                            Dictionary<string, object> dicData = new Dictionary<string, object>();
                            for (int i = 0; i < table.Fields.FieldCount; i++)
                            {
                                object obj = row.get_Value(i);
                                //blob����ת��(��ת�������ֵ�滻�Ĺ�����)
                                if (obj is IMemoryBlobStreamVariant)
                                {
                                    try
                                    {
                                        IMemoryBlobStreamVariant var = obj as IMemoryBlobStreamVariant;
                                        object tempObj = null;
                                        if (var == null) continue;
                                        var.ExportToVariant(out tempObj);
                                        XmlDocument doc = new XmlDocument();
                                        byte[] btyes = (byte[])tempObj;
                                        string xml = Encoding.Default.GetString(btyes);
                                        doc.LoadXml(xml);
                                        dicData.Add(table.Fields.get_Field(i).Name, doc);
                                        continue;
                                    }
                                    catch
                                    {}
                                }
                                dicData.Add(table.Fields.get_Field(i).Name, obj);
                            }
                            lstDicData.Add(dicData);
                            row = pCursor.NextRow();
                        }
                    }
                }
                return lstDicData;
            }
            catch (Exception ex)
            {
                //********************************
                //guozheng added  system exception log
                if (SysCommon.Log.Module.SysLog == null)
                    SysCommon.Log.Module.SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
                SysCommon.Log.Module.SysLog.Write(ex);
                //********************************
                eError = ex;
                return null;
            }
        }
        public List<object> GetFieldValues(string tablename, string keyfieldname, string condition, out Exception eError)
        {
            try
            {
                ITable table = OpenTable(tablename, out eError);
                if (table != null)
                {
                    IQueryFilter pQueryFilter = new QueryFilterClass();
                    pQueryFilter.WhereClause = condition;

                    ICursor pCursor = table.Search(pQueryFilter, true);
                    int index = table.Fields.FindField(keyfieldname);
                    if (pCursor != null)
                    {
                        List<object> zonesetids = new List<object>();
                        IRow row = pCursor.NextRow();
                        while (row != null)
                        {
                            if (index != -1)
                            {
                                if (row != null)
                                {
                                    object obj = row.get_Value(index);
                                    if (obj is IMemoryBlobStreamVariant)
                                    {
                                        IMemoryBlobStreamVariant var = obj as IMemoryBlobStreamVariant;
                                        object tempObj = null;
                                        if (var == null) continue;
                                        var.ExportToVariant(out tempObj);
                                        XmlDocument doc = new XmlDocument();
                                        byte[] btyes = (byte[])tempObj;
                                        string xml = Encoding.Default.GetString(btyes);
                                        doc.LoadXml(xml);
                                        obj = doc;
                                    }
                                    if (!zonesetids.Contains(obj))
                                    {
                                        zonesetids.Add(obj);
                                    }
                                }
                            }
                            row = pCursor.NextRow();
                        }
                        return zonesetids;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                eError = ex;
                return null;
            }
        }
        public object GetFieldValue(string tablename, string keyfieldname, string condition, out Exception eError)
        {
            eError = null;
            //�õ�FeatrueWS
            try
            {
                Dictionary<string, object> dicValue = new Dictionary<string, object>();
                ITable pTable = OpenTable(tablename, out eError);
                if (eError != null) return null;
                IQueryFilter pQueryFilter = new QueryFilterClass();
                pQueryFilter.WhereClause = condition;
                ICursor pCursor = pTable.Search(pQueryFilter, true);
                IRow pRow = pCursor.NextRow();
                if (pRow == null)
                {
                    Marshal.ReleaseComObject(pCursor);
                    return null;
                }

                int indexF = pRow.Fields.FindField(keyfieldname);
                if (indexF == -1)
                {
                    Marshal.ReleaseComObject(pCursor);
                    return null;
                }

                object val = pRow.get_Value(indexF);
                //Marshal.ReleaseComObject(pCursor);
                if (val is IMemoryBlobStreamVariant)
                {
                    IMemoryBlobStreamVariant var = val as IMemoryBlobStreamVariant;
                    object tempObj = null;
                    if (var == null) return null;
                    var.ExportToVariant(out tempObj);
                    XmlDocument doc = new XmlDocument();
                    byte[] btyes = (byte[])tempObj;
                    try
                    {
                        string xml = Encoding.Default.GetString(btyes);
                        doc.LoadXml(xml);
                        Marshal.ReleaseComObject(pCursor);
                        return doc;
                    }
                    catch
                    {
                        Marshal.ReleaseComObject(pCursor);
                        return btyes;
                    }
                }
                Marshal.ReleaseComObject(pCursor);
                return val;
            }
            catch (Exception eX)
            {
                //********************************
                //guozheng added  system exception log
                if (SysCommon.Log.Module.SysLog == null)
                    SysCommon.Log.Module.SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
                SysCommon.Log.Module.SysLog.Write(eX);
                //********************************
                eError = eX;
                return null;
            }
        }

        public long GetRowCount(string tablename, string condition, out Exception eError)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public Dictionary<int, object> GetRows(string tablename, string field, string condition, out Exception eError)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public Dictionary<int, System.Collections.ArrayList> GetRows(string tablename, List<string> fields, string condition, out Exception eError)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public Dictionary<int, System.Collections.ArrayList> GetRows(string tablename, List<string> fields, string condition, string postfixClause, out Exception eError)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public Dictionary<object, object> GetRows(string tablename, string keyField, string field, string condition, out Exception eError)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public Dictionary<object, System.Collections.ArrayList> GetRows(string tablename, string keyField, List<string> fields, string condition, out Exception eError)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public Dictionary<object, System.Collections.ArrayList> GetRows(string tablename, string keyField, List<string> fields, string condition, string postfixClause, out Exception eError)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool CheckFieldValue(string tablename, string fieldname, object value, out Exception eError)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool NewRow(string tablename, Dictionary<string, object> dicvalues, bool bEdit, out Exception eError)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool NewRow(string tablename, Dictionary<string, object> dicvalues, bool bEdit, out int objectid, out Exception eError)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool NewRow(string tablename, Dictionary<string, object> dicvalues, bool bEdit, string strOIDField, out Exception eError)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool NewRow(string tablename, Dictionary<string, object> dicvalues, bool bEdit, string strOIDField, out int objectid, out Exception eError)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool NewRow(string tablename, Dictionary<string, object> dicvalues, bool bEdit, string strField, esriFieldType FieldType, out Exception eError)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool NewRow(string tablename, Dictionary<string, object> dicvalues, bool bEdit, string strField, esriFieldType FieldType, out int objectid, out Exception eError)
        {
            throw new Exception("The method or operation is not implemented.");
        }
        public bool NewRowByAliasName(string tablename, Dictionary<string, object> dicValues, out Exception eError)
        {
            try
            {
                ITable table = OpenTable(tablename, out eError);
                if (table != null)
                {
                    return ModGisPub.NewRowByAliasName(table, dicValues, out eError);
                }
                return false;
            }
            catch (Exception ex)
            {
                //********************************
                //guozheng added  system exception log
                if (SysCommon.Log.Module.SysLog == null)
                    SysCommon.Log.Module.SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
                SysCommon.Log.Module.SysLog.Write(ex);
                //********************************
                eError = ex;
                return false;
            }
        }
        public bool NewRow(string tablename, Dictionary<string, object> dicvalues, out Exception eError)
        {
            try
            {
                ITable table = OpenTable(tablename, out eError);
                if (table != null)
                {
                    return ModGisPub.NewRow(table, dicvalues, out eError);
                }
                return false;
            }
            catch (Exception ex)
            {
                //********************************
                //guozheng added  system exception log
                if (SysCommon.Log.Module.SysLog == null)
                    SysCommon.Log.Module.SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
                SysCommon.Log.Module.SysLog.Write(ex);
                //********************************
                eError = ex;
                return false;
            }
        }

        public bool ExistData(string tablename, string condition)
        {
            Exception eError = null;
            try
            {
                ITable table = OpenTable(tablename, out eError);
                return ExistData(table, condition);
            }
            catch (Exception ex)
            {
                //********************************
                //guozheng added  system exception log
                if (SysCommon.Log.Module.SysLog == null)
                    SysCommon.Log.Module.SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
                SysCommon.Log.Module.SysLog.Write(ex);
                //********************************
                eError = ex;
                return false;
            }
        }

        private bool ExistData(ITable table, string condition)
        {
            try
            {
                IQueryFilter pQueryFilter = new QueryFilterClass();
                pQueryFilter.WhereClause = condition;
                ICursor pCursor = table.Search(pQueryFilter, false);
                if (pCursor != null)
                {
                    IRow pRow = pCursor.NextRow();
                    if (pRow != null)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                //********************************
                //guozheng added  system exception log
                if (SysCommon.Log.Module.SysLog == null)
                    SysCommon.Log.Module.SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
                SysCommon.Log.Module.SysLog.Write(ex);
                //********************************
                return false;
            }
        }

        public bool EditRows(string tablename, string condition, Dictionary<string, object> dicValues, bool bEdit, out Exception eError)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool EditRows(string tablename, string condition, Dictionary<string, object> dicValues, bool bEdit, string fieldName, esriFieldType FieldType, out Exception eError)
        {
            throw new Exception("The method or operation is not implemented.");
        }
        public bool UpdateRowByAliasName(string tablename, string condition, Dictionary<string, object> dicValues, out Exception eError)
        {
            try
            {
                ITable table = OpenTable(tablename, out eError);
                if (table != null)
                {
                    return ModGisPub.UpdateRowByAliasName(table, condition, dicValues, out eError);
                }
                return false;
            }
            catch (Exception ex)
            {
                //********************************
                //guozheng added  system exception log
                if (SysCommon.Log.Module.SysLog == null)
                    SysCommon.Log.Module.SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
                SysCommon.Log.Module.SysLog.Write(ex);
                //********************************
                eError = ex;
                return false;
            }
        }

        public bool UpdateRow(string tablename, string condition, Dictionary<string, object> dicValues, out Exception eError)
        {
            try
            {
                ITable table = OpenTable(tablename, out eError);
                if (table != null)
                {
                    return ModGisPub.UpdateRow(table, condition, dicValues, out eError);
                }
                return false;
            }
            catch (Exception ex)
            {
                //********************************
                //guozheng added  system exception log
                if (SysCommon.Log.Module.SysLog == null)
                    SysCommon.Log.Module.SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
                SysCommon.Log.Module.SysLog.Write(ex);
                //********************************
                eError = ex;
                return false;
            }
        }

        public bool DeleteRows(string tablename, string condition, out Exception eError)
        {
            eError = null;
            try
            {
                ITable pTable = OpenTable(tablename, out eError);
                if (pTable == null) return false;

                IQueryFilter pQueryFilter = new QueryFilterClass();
                pQueryFilter.WhereClause = condition;
                pTable.DeleteSearchedRows(pQueryFilter);

                return true;
            }
            catch (Exception eX)
            {
                //********************************
                //guozheng added  system exception log
                if (SysCommon.Log.Module.SysLog == null)
                    SysCommon.Log.Module.SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
                SysCommon.Log.Module.SysLog.Write(eX);
                //********************************
                eError = eX;
                return false;
            }
        }

        #endregion
    }

    public class CreateArcGISGeoDatabase : ICreateGeoDatabase
    {

        private ISchemeProject m_pProject = null;                                           //���ݿ�ṹ�ļ�����
        private int m_DBScale = 0;                                                          //Ĭ�ϱ�����
        private int m_DSScale = 0;                                                          //���ݼ�������
        private ISpatialReference m_SpatialReference = null;                                //�ռ�ο�����

        private IWorkspace m_WorkSpace = null;                                              //�������Ժ��õĹ����ռ�

        string netLogPath = Application.StartupPath + @"\..\Template\Network_Log.mdb";       //Զ����־ģ��·��

        #region ICreateGeoDatabase ��Ա
        /// <summary>
        /// �������ݿ�ṹ�ļ�����
        /// </summary>
        /// <param name="LoadPath">�ļ�·��</param>
        /// <returns></returns>
        public bool LoadDBShecmaDocument(string LoadPath)
        {
            try
            {
                m_pProject = new SchemeProjectClass();     //����ʵ��
                int index = LoadPath.LastIndexOf('.');
                if (index == -1) return false;
                string lastName = LoadPath.Substring(index + 1);
                if (lastName == "mdb")
                {
                    m_pProject.Load(LoadPath, e_FileType.GO_SCHEMEFILETYPE_MDB);    //����schema�ļ�
                }
                else if (lastName == "gosch")
                {
                    m_pProject.Load(LoadPath, e_FileType.GO_SCHEMEFILETYPE_GOSCH);    //����schema�ļ�
                }


                ///������سɹ����ȡ�����߷���true�����򷵻�false
                if (m_pProject != null)
                {
                    string DBScale = m_pProject.get_MetaDataValue("Scale") as string;   //��ȡ��������Ϣ���ܹ����е�Ĭ�ϱ����ߣ�
                    string[] DBScaleArayy = DBScale.Split(':');
                    m_DBScale = Convert.ToInt32(DBScaleArayy[1]);

                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {

                return false;
            }
        }

        /// <summary>
        /// ���ؿռ�ο��ļ���ȡ�ռ�ο�
        /// </summary>
        /// <param name="LoadPath">�ռ�ο��ļ�·��</param>
        /// <returns></returns>
        public bool LoadSpatialReference(string LoadPath)
        {
            try
            {
                ISpatialReference pSR = null;
                ISpatialReferenceFactory pSpatialRefFac = new SpatialReferenceEnvironmentClass();

                if (!File.Exists(LoadPath))
                {
                    m_SpatialReference = null;
                    return false;
                }
                pSR = pSpatialRefFac.CreateESRISpatialReferenceFromPRJFile(LoadPath);

                ISpatialReferenceResolution pSRR = pSR as ISpatialReferenceResolution;
                ISpatialReferenceTolerance pSRT = (ISpatialReferenceTolerance)pSR;
                IControlPrecision2 pSpatialPrecision = (IControlPrecision2)pSR;

                pSRR.ConstructFromHorizon();//Defines the XY resolution and domain extent of this spatial reference based on the extent of its horizon
                pSRR.SetDefaultXYResolution();
                pSRT.SetDefaultXYTolerance();

                m_SpatialReference = pSR;                //��ֵ�ռ�ο�

                return true;
            }
            catch
            {
                m_SpatialReference = null;
                return false;
            }
        }

        /// <summary>
        /// ������������
        /// </summary>
        /// <param name="Ҫ������">���ݿ�����</param>
        /// <param name="IPoPath">���ݿ����·���������IP</param>
        /// <param name="Intance">sde����ʵ��</param>
        /// <param name="User">�û���</param>
        /// <param name="PassWord">����</param>
        /// <param name="Version">sde�汾</param>
        /// <returns></returns>
        public bool SetDestinationProp(string Type, string IPoPath, string Intance, string User, string PassWord, string Version)
        {
            IWorkspace TempWorkSpace = null;                                 //�����ռ�
            IWorkspaceFactory pWorkspaceFactory = null;                      //�����ռ乤��

            try
            {
                //��ʼ�������ռ乤��
                if (Type == "PDB")
                {
                    pWorkspaceFactory = new AccessWorkspaceFactoryClass();
                }
                else if (Type == "GDB")
                {
                    pWorkspaceFactory = new FileGDBWorkspaceFactoryClass();
                }
                else if (Type == "SDE")
                {
                    pWorkspaceFactory = new SdeWorkspaceFactoryClass();
                }
                //cyf  20110622 delete:��ɾ��ԭ�������ݿ⣬��ԭ�еĻ�������׷��
                ///����������Ǳ��ؿ��壬�������жϿ����Ƿ����
                ///���������ڣ�����ɾ��ԭ�п���
                //if (File.Exists(IPoPath))
                //{
                    //if (!SysCommon.Error.ErrorHandle.ShowFrmInformation("��", "��", "�Ƿ�����д������ݼ�"))
                    //{
                    //    File.Delete(IPoPath);
                    //}
                //}
                //end

                if (Type == "SDE")  //�����SDE������sde�����ռ�������Ϣ
                {
                    IPropertySet propertySet = new PropertySetClass();
                    propertySet.SetProperty("SERVER", IPoPath);
                    propertySet.SetProperty("INSTANCE", Intance);
                    //propertySet.SetProperty("DATABASE", ""); 
                    propertySet.SetProperty("USER", User);
                    propertySet.SetProperty("PASSWORD", PassWord);
                    propertySet.SetProperty("VERSION", Version);
                    TempWorkSpace = pWorkspaceFactory.Open(propertySet, 0);
                }
                else  //�������sde�򴴽������ռ�
                {
                    FileInfo finfo = new FileInfo(IPoPath);
                    string outputDBPath = finfo.DirectoryName;
                    string outputDBName = finfo.Name;
                    if (outputDBName.EndsWith(".gdb"))
                    {
                        outputDBName = outputDBName.Substring(0, outputDBName.Length - 4);
                    }
                    //cyf 20110622 add:�����еĹ����ռ�
                    try { TempWorkSpace = pWorkspaceFactory.OpenFromFile(IPoPath, 0); }
                    catch { }
                    //end
                    if (TempWorkSpace == null)
                    {
                        IWorkspaceName pWorkspaceName = pWorkspaceFactory.Create(outputDBPath, outputDBName, null, 0);
                        ESRI.ArcGIS.esriSystem.IName pName = (ESRI.ArcGIS.esriSystem.IName)pWorkspaceName;
                        TempWorkSpace = (IWorkspace)pName.Open();
                    }
                }

                //�жϻ�ȡ�����ռ��Ƿ�ɹ�
                if (TempWorkSpace != null)
                {
                    m_WorkSpace = TempWorkSpace;                //�����ռ丳ֵ
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                //***********************
                //guozheng 2010-12-17 added
                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "�������ݿ�ʧ�ܣ�\nԭ��" + ex.Message);
                //***********************
                return false;
            }

        }

        /// <summary>
        /// ��������������
        /// </summary>
        /// <returns></returns>
        public bool CreateDBStruct()
        {
            IFeatureWorkspace pFeatureWorkSpace = null;

            try
            {
                //��������ռ��ȡ�ɹ���ֵҪ�ع����ռ�
                if (m_WorkSpace != null)
                {
                    pFeatureWorkSpace = m_WorkSpace as IFeatureWorkspace;
                }
                else
                {
                    return false;
                }

                //�����ȡ����schema�ɹ���ʼ��������
                if (m_pProject != null)
                {
                    IChildItemList pProjects = m_pProject as IChildItemList;
                    //��ȡ���Կ⼯����Ϣ
                    ISchemeItem pDBList = pProjects.get_ItemByName("ATTRDB");
                    IChildItemList pDBLists = pDBList as IChildItemList;
                    //�������Կ⼯��
                    long DBNum = pDBLists.GetCount();
                    for (int i = 0; i < DBNum; i++)
                    {
                        m_DSScale = 0;    //��������Ϣ

                        //ȡ�����Կ���Ϣ
                        ISchemeItem pDB = pDBLists.get_ItemByIndex(i);
                        ///��ȡ���ݼ��ı�������Ϣ�������ȡʧ����ȡĬ�ϱ�������Ϣ
                        IAttribute pa = pDB.AttributeList.get_AttributeByName("Scale") as IAttribute;
                        if (pa == null)
                        {
                            m_DSScale = m_DBScale;
                        }
                        else
                        {
                            string[] DBScaleArayy = pa.Value.ToString().Split(':');
                            m_DSScale = Convert.ToInt32(DBScaleArayy[1]);
                        }

                        IChildItemList pDBs = pDB as IChildItemList;
                        string pDatasetName = pDB.Name;

                        #region �ڹ����ռ��д������ݼ����������ݼ�����
                        //�������ݼ���Ϣ����������ݼ�����
                        IFeatureDataset pFeatureDataset = null;  //�������ݼ�����װ��Ҫ����
                        if (createFeatureDataset(pFeatureWorkSpace, pDatasetName, out pFeatureDataset, m_SpatialReference) == false)
                        {
                            return false;
                        }
                        #endregion

                        //�������Ա�
                        long TabNum = pDBs.GetCount();
                        for (int j = 0; j < TabNum; j++)
                        {
                            //��ȡ���Ա���Ϣ
                            ISchemeItem pTable = pDBs.get_ItemByIndex(j);  //��ȡ���Ա����

                            string pFeatureClassName = pTable.Name;     //Ҫ��������
                            string pFeatureClassType = pTable.Value as string;   //Ҫ��������

                            //�����ֶ�
                            IAttributeList pAttrs = pTable.AttributeList;
                            long FNum = pAttrs.GetCount();

                            //�����û��Զ�����ֶ�

                            IFields fields = new FieldsClass();
                            IFieldsEdit fsEdit = fields as IFieldsEdit;

                            //ѭ�����Ա��е��ֶΣ���ӵ�arcgis���ֶζ�����
                            for (int k = 0; k < FNum; k++)
                            {
                                //����Զ��������ֶ�
                                AddCustomusFields(pAttrs, k, fsEdit);
                            }

                            if (pFeatureClassType == "NONE")
                            {
                                //�����ǿռ��
                            }
                            //����Ҫ�������ע��
                            else if (pFeatureClassType == "ANNO")  //�����ע��ͼ��
                            {
                                //����ע�ǲ�
                                createAnnoFeatureClass(pFeatureClassName, pFeatureWorkSpace, fsEdit, m_DSScale, "ע��", pFeatureDataset);
                            }
                            else  //�������ͨҪ����ͼ��
                            {
                                //������ͨҪ����
                                createCommomFeatureClass(pFeatureClassName, pFeatureClassType, fsEdit, fields, pFeatureDataset);
                            }
                        }
                    }
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(pFeatureWorkSpace);
                    return true;
                }
                else
                {
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(pFeatureWorkSpace);
                    return false;
                }
            }
            catch (Exception e)
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(pFeatureWorkSpace);
                return false;
            }

        }


        /// <summary>
        /// ��������������
        /// </summary>
        /// <returns></returns>
        public bool CreateDBStruct(List<string> DSName)
        {
            IFeatureWorkspace pFeatureWorkSpace = null;

            try
            {
                //��������ռ��ȡ�ɹ���ֵҪ�ع����ռ�
                if (m_WorkSpace != null)
                {
                    pFeatureWorkSpace = m_WorkSpace as IFeatureWorkspace;
                }
                else
                {
                    return false;
                }

                //�����ȡ����schema�ɹ���ʼ��������
                if (m_pProject != null)
                {
                    IChildItemList pProjects = m_pProject as IChildItemList;
                    //��ȡ���Կ⼯����Ϣ
                    ISchemeItem pDBList = pProjects.get_ItemByName("ATTRDB");
                    IChildItemList pDBLists = pDBList as IChildItemList;
                    //�������Կ⼯��
                    long DBNum = pDBLists.GetCount();
                    for (int i = 0; i < DBNum; i++)
                    {
                        m_DSScale = 0;    //��������Ϣ

                        //ȡ�����Կ���Ϣ
                        ISchemeItem pDB = pDBLists.get_ItemByIndex(i);
                        ///��ȡ���ݼ��ı�������Ϣ�������ȡʧ����ȡĬ�ϱ�������Ϣ
                        IAttribute pa = pDB.AttributeList.get_AttributeByName("Scale") as IAttribute;
                        if (pa == null)
                        {
                            m_DSScale = m_DBScale;
                        }
                        else
                        {
                            string[] DBScaleArayy = pa.Value.ToString().Split(':');
                            m_DSScale = Convert.ToInt32(DBScaleArayy[1]);
                        }

                        IChildItemList pDBs = pDB as IChildItemList;
                        string pDatasetName = pDB.Name;
                        DSName.Add(pDatasetName);

                        #region �ڹ����ռ��д������ݼ����������ݼ�����
                        //�������ݼ���Ϣ����������ݼ�����
                        IFeatureDataset pFeatureDataset = null;  //�������ݼ�����װ��Ҫ����
                        if (createFeatureDataset(pFeatureWorkSpace, pDatasetName, out pFeatureDataset, m_SpatialReference) == false)
                        {
                            return false;
                        }
                        #endregion

                        //�������Ա�
                        long TabNum = pDBs.GetCount();
                        for (int j = 0; j < TabNum; j++)
                        {
                            //��ȡ���Ա���Ϣ
                            ISchemeItem pTable = pDBs.get_ItemByIndex(j);  //��ȡ���Ա����

                            string pFeatureClassName = pTable.Name;     //Ҫ��������
                            string pFeatureClassType = pTable.Value as string;   //Ҫ��������

                            //�����ֶ�
                            IAttributeList pAttrs = pTable.AttributeList;
                            long FNum = pAttrs.GetCount();

                            //�����û��Զ�����ֶ�

                            IFields fields = new FieldsClass();
                            IFieldsEdit fsEdit = fields as IFieldsEdit;

                            //ѭ�����Ա��е��ֶΣ���ӵ�arcgis���ֶζ�����
                            for (int k = 0; k < FNum; k++)
                            {
                                //����Զ��������ֶ�
                                AddCustomusFields(pAttrs, k, fsEdit);
                            }

                            /////��Ӱ汾�ֶΣ���Эͬ���°汾��ʹ�ã���ʤ��  2010.3.26���
                            //AddVersionField(fsEdit);

                            if (pFeatureClassType == "NONE")
                            {
                                //�����ǿռ��
                                //createNonSpatialTable(pFeatureClassName, m_WorkSpace, fsEdit, pFeatureDataset);
                            }
                            //����Ҫ�������ע��
                            else if (pFeatureClassType == "ANNO")  //�����ע��ͼ��
                            {
                                //����ע�ǲ�
                                createAnnoFeatureClass(pFeatureClassName, pFeatureWorkSpace, fsEdit, m_DSScale, "ע��", pFeatureDataset);
                            }
                            else  //�������ͨҪ����ͼ��
                            {
                                //������ͨҪ����
                                createCommomFeatureClass(pFeatureClassName, pFeatureClassType, fsEdit, fields, pFeatureDataset);
                            }
                        }
                    }
                    //System.Runtime.InteropServices.Marshal.ReleaseComObject(pFeatureWorkSpace);
                    return true;
                }
                else
                {
                    //System.Runtime.InteropServices.Marshal.ReleaseComObject(pFeatureWorkSpace);
                    return false;
                }
            }
            catch (Exception e)
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(pFeatureWorkSpace);
                //***********************
                //guozheng 2010-12-17 added
                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "�������ݿ�ʧ�ܣ�\nԭ��" + e.Message);
                //***********************
                return false;
            }

        }

        /// <summary>
        /// ��������������
        /// </summary>
        /// <returns></returns>
        public bool CreateDBStruct(List<string> DSName, out int iScale, out string sDBName)
        {
            iScale = -1;
            sDBName = string.Empty;
            IFeatureWorkspace pFeatureWorkSpace = null;

            try
            {
                //��������ռ��ȡ�ɹ���ֵҪ�ع����ռ�
                if (m_WorkSpace != null)
                {
                    pFeatureWorkSpace = m_WorkSpace as IFeatureWorkspace;
                }
                else
                {
                    return false;
                }

                //�����ȡ����schema�ɹ���ʼ��������
                if (m_pProject != null)
                {
                    IChildItemList pProjects = m_pProject as IChildItemList;
                    //��ȡ���Կ⼯����Ϣ
                    ISchemeItem pDBList = pProjects.get_ItemByName("ATTRDB");
                    IChildItemList pDBLists = pDBList as IChildItemList;
                    //�������Կ⼯��
                    long DBNum = pDBLists.GetCount();
                    for (int i = 0; i < DBNum; i++)
                    {
                        m_DSScale = 0;    //��������Ϣ

                        //ȡ�����Կ���Ϣ
                        ISchemeItem pDB = pDBLists.get_ItemByIndex(i);
                        ///��ȡ���ݼ��ı�������Ϣ�������ȡʧ����ȡĬ�ϱ�������Ϣ
                        IAttribute pa = pDB.AttributeList.get_AttributeByName("Scale") as IAttribute;
                        if (pa == null)
                        {
                            iScale = m_DBScale;
                        }
                        else
                        {
                            string[] DBScaleArayy = pa.Value.ToString().Split(':');
                            m_DSScale = Convert.ToInt32(DBScaleArayy[1]);
                            iScale = m_DSScale;
                        }

                        IChildItemList pDBs = pDB as IChildItemList;
                        string pDatasetName = pDB.Name;
                        sDBName = pDatasetName;
                        DSName.Add(pDatasetName);

                        #region �ڹ����ռ��д������ݼ����������ݼ�����
                        //�������ݼ���Ϣ����������ݼ�����
                        IFeatureDataset pFeatureDataset = null;  //�������ݼ�����װ��Ҫ����
                        if (createFeatureDataset(pFeatureWorkSpace, pDatasetName, out pFeatureDataset, m_SpatialReference) == false)
                        {
                            return false;
                        }
                        #endregion

                        //�������Ա�
                        long TabNum = pDBs.GetCount();
                        for (int j = 0; j < TabNum; j++)
                        {
                            //��ȡ���Ա���Ϣ
                            ISchemeItem pTable = pDBs.get_ItemByIndex(j);  //��ȡ���Ա����

                            string pFeatureClassName = pTable.Name;     //Ҫ��������
                            string pFeatureClassType = pTable.Value as string;   //Ҫ��������

                            //�����ֶ�
                            IAttributeList pAttrs = pTable.AttributeList;
                            long FNum = pAttrs.GetCount();

                            //�����û��Զ�����ֶ�

                            IFields fields = new FieldsClass();
                            IFieldsEdit fsEdit = fields as IFieldsEdit;

                            //ѭ�����Ա��е��ֶΣ���ӵ�arcgis���ֶζ�����
                            for (int k = 0; k < FNum; k++)
                            {
                                //����Զ��������ֶ�
                                AddCustomusFields(pAttrs, k, fsEdit);
                            }

                            /////��Ӱ汾�ֶΣ���Эͬ���°汾��ʹ�ã���ʤ��  2010.3.26���
                            //AddVersionField(fsEdit);

                            if (pFeatureClassType == "NONE")
                            {
                                //�����ǿռ��
                            }
                            //����Ҫ�������ע��
                            else if (pFeatureClassType == "ANNO")  //�����ע��ͼ��
                            {
                                //����ע�ǲ�
                                createAnnoFeatureClass(pFeatureClassName, pFeatureWorkSpace, fsEdit, m_DSScale, "ע��", pFeatureDataset);
                            }
                            else  //�������ͨҪ����ͼ��
                            {
                                //������ͨҪ����
                                createCommomFeatureClass(pFeatureClassName, pFeatureClassType, fsEdit, fields, pFeatureDataset);
                            }
                        }
                    }
                    //System.Runtime.InteropServices.Marshal.ReleaseComObject(pFeatureWorkSpace);
                    return true;
                }
                else
                {
                    //System.Runtime.InteropServices.Marshal.ReleaseComObject(pFeatureWorkSpace);
                    return false;
                }
            }
            catch (Exception e)
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(pFeatureWorkSpace);
                //***********************
                //guozheng 2010-12-17 added
                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "�������ݿ�ʧ�ܣ�\nԭ��" + e.Message);
                //***********************
                return false;
            }

        }

        /// <summary>
        /// ��������������(����������)  cyf 20110707 modify
        /// </summary>
        /// <returns></returns>
        public bool CreateDBStruct(out string iScale, out List<string> DSName, System.Windows.Forms.ProgressBar in_ProcBar)
        {
            //cyf 20110622 modify
            //iScale = -1;
            iScale = "";
            //end
            //sDBName = string.Empty;  //cyf 20110707 modify
            IFeatureWorkspace pFeatureWorkSpace = null;
            //cyf 20110707 modify
                DSName = new List<string>();
            //end
            try
            {
                //��������ռ��ȡ�ɹ���ֵҪ�ع����ռ�
                if (m_WorkSpace != null)
                {
                    pFeatureWorkSpace = m_WorkSpace as IFeatureWorkspace;
                }
                else
                {
                    return false;
                }

                //�����ȡ����schema�ɹ���ʼ��������
                if (m_pProject != null)
                {
                    IChildItemList pProjects = m_pProject as IChildItemList;
                    //��ȡ���Կ⼯����Ϣ
                    ISchemeItem pDBList = pProjects.get_ItemByName("ATTRDB");
                    IChildItemList pDBLists = pDBList as IChildItemList;
                    //�������Կ⼯��
                    long DBNum = pDBLists.GetCount();
                   
                    for (int i = 0; i < DBNum; i++)
                    {
                        m_DSScale = 0;    //��������Ϣ
                        //ȡ�����Կ���Ϣ
                        ISchemeItem pDB = pDBLists.get_ItemByIndex(i);
                        ///��ȡ���ݼ��ı�������Ϣ�������ȡʧ����ȡĬ�ϱ�������Ϣ
                        IAttribute pa = pDB.AttributeList.get_AttributeByName("Scale") as IAttribute;
                        if (pa == null)
                        {
                            //cyf 20110622 modify:�����������͸�Ϊ�ַ�����
                            iScale = m_DBScale.ToString();
                            //end
                        }
                        else
                        {
                            string[] DBScaleArayy = pa.Value.ToString().Split(':');
                            m_DSScale = Convert.ToInt32(DBScaleArayy[1]);
                            //cyf 20110622 modify:�����������͸�Ϊ�ַ�����
                            iScale = m_DSScale.ToString();
                            //end
                        }

                        IChildItemList pDBs = pDB as IChildItemList;
                        string pDatasetName = pDB.Name;
                        //cyf 20110706 modify
                        //sDBName = pDatasetName;
                        //DSName.Add(pDatasetName);
                        //end

                        #region �ڹ����ռ��д������ݼ����������ݼ�����
                        //�������ݼ���Ϣ����������ݼ�����
                        IFeatureDataset pFeatureDataset = null;  //�������ݼ�����װ��Ҫ����
                        if (createFeatureDataset(pFeatureWorkSpace, pDatasetName, out pFeatureDataset, m_SpatialReference) == false)
                        {
                            return false;
                        }
                        #endregion
                        //cyf 20110706 modify
                        pDatasetName = (pFeatureDataset as IDataset).Name;
                        //sDBName = pDatasetName;  //cyf 20110707 modify
                        DSName.Add(pDatasetName);
                        //end

                        //�������Ա�
                        long TabNum = pDBs.GetCount();
                        /////////////////////////////////////////������//////////////////////
                        if (in_ProcBar != null)
                        {
                            in_ProcBar.Maximum = (int)TabNum;
                            in_ProcBar.Value = 0;
                        }
                        for (int j = 0; j < TabNum; j++)
                        {
                            //��ȡ���Ա���Ϣ
                            if (in_ProcBar != null)
                            {
                                in_ProcBar.Value = j;
                                Application.DoEvents();
                            }
                            ISchemeItem pTable = pDBs.get_ItemByIndex(j);  //��ȡ���Ա����

                            string pFeatureClassName = pTable.Name;     //Ҫ��������
                            string pFeatureClassType = pTable.Value as string;   //Ҫ��������

                            //�����ֶ�
                            IAttributeList pAttrs = pTable.AttributeList;
                            long FNum = pAttrs.GetCount();

                            //�����û��Զ�����ֶ�

                            IFields fields = new FieldsClass();
                            IFieldsEdit fsEdit = fields as IFieldsEdit;

                            //ѭ�����Ա��е��ֶΣ���ӵ�arcgis���ֶζ�����
                            for (int k = 0; k < FNum; k++)
                            {
                                //����Զ��������ֶ�
                                AddCustomusFields(pAttrs, k, fsEdit);
                            }

                            /////��Ӱ汾�ֶΣ���Эͬ���°汾��ʹ�ã���ʤ��  2010.3.26���
                            //AddVersionField(fsEdit);

                            if (pFeatureClassType == "NONE")
                            {
                                //�����ǿռ��
                            }
                            //����Ҫ�������ע��
                            else if (pFeatureClassType == "ANNO")  //�����ע��ͼ��
                            {
                                //����ע�ǲ�
                                createAnnoFeatureClass(pFeatureClassName, pFeatureWorkSpace, fsEdit, m_DSScale, "ע��", pFeatureDataset);
                            }
                            else  //�������ͨҪ����ͼ��
                            {
                                //������ͨҪ����
                                createCommomFeatureClass(pFeatureClassName, pFeatureClassType, fsEdit, fields, pFeatureDataset);
                            }
                        }
                    }
                    //System.Runtime.InteropServices.Marshal.ReleaseComObject(pFeatureWorkSpace);
                    return true;
                }
                else
                {
                    //System.Runtime.InteropServices.Marshal.ReleaseComObject(pFeatureWorkSpace);
                    return false;
                }
            }
            catch (Exception e)
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(pFeatureWorkSpace);
                //***********************
                //guozheng 2010-12-17 added
                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "�������ݿ�ʧ�ܣ�\nԭ��" + e.Message);
                //***********************
                return false;
            }

        }
        /// <summary>
        /// ����Զ����־�� ���Ƿ����
        /// </summary>
        /// <param name="pDBType">�����ռ����ͣ�PDB��GDB��SDE</param>
        /// <param name="eError"></param>
        /// <returns></returns>
        public bool CreateSQLTable(string pDBType, out Exception eError)
        {
            //����Զ����־��
            eError = null;

            //���Զ����־���Ƿ���ڣ�ֻҪ��һ�ű����ڣ��ͷ���
            ITable pTable = null;
            ITable mTable = null;
            IFeatureWorkspace pFeaWS = m_WorkSpace as IFeatureWorkspace;
            if (pFeaWS == null) return false;
            //cyf 20110622
            try
            {
                pTable = pFeaWS.OpenTable("GO_DATABASE_UPDATELOG");
            }
            catch
            {
            }
            if (pTable == null)
            {
                //�����Ϊ�գ��򴴽����
                //�������
                try
                {
                    if (pDBType.ToUpper() == "PDB")
                    {
                        m_WorkSpace.ExecuteSQL("create table GO_DATABASE_UPDATELOG (OID  integer,STATE integer,LAYERNAME varchar(50),USERNAME varchar(255),LASTUPDATE date,VERSION integer,XMIN float,XMAX float,YMIN float,YMAX float)");
                        //m_WorkSpace.ExecuteSQL("create table go_database_version (VERSION  integer,USERNAME varchar(255),VERSIONTIME date,DES varchar(255))");

                    }
                    else if (pDBType.ToUpper() == "SDE")
                    {
                        m_WorkSpace.ExecuteSQL("create table GO_DATABASE_UPDATELOG (OID  INTEGER,STATE INTEGER,LAYERNAME NVARCHAR2(50),USERNAME NVARCHAR2(255),LASTUPDATE DATE,VERSION INTEGER,XMIN FLOAT,XMAX FLOAT,YMIN FLOAT,YMAX FLOAT)");
                        //m_WorkSpace.ExecuteSQL("create table go_database_version (VERSION  INTEGER,USERNAME NVARCHAR2(255),VERSIONTIME DATE,DES NVARCHAR2(255))");
                    }
                    else if (pDBType.ToUpper() == "GDB")
                    {
                        string tempFile = netLogPath;
                        FileInfo pFI = new FileInfo(tempFile);
                        string fName = pFI.Name;  //Զ����־����

                        //��־�洢·��
                        string dbPath = m_WorkSpace.PathName;
                        int index = dbPath.LastIndexOf('\\');
                        if (index == -1) return false;
                        string FileDic = dbPath.Substring(0, index);
                        string FileName = FileDic + "\\" + fName;

                        if (File.Exists(FileName))
                        {
                            File.Delete(FileName);
                        }
                        File.Copy(tempFile, FileName);
                    }
                } catch (System.Exception ex)
                {
                    eError = ex;
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(pFeaWS);
                    return false;
                }
            }
            try
            {
                mTable = pFeaWS.OpenTable("go_database_version");
            } catch
            {
            }
            if (mTable == null)
            {
                //�����Ϊ�գ��򴴽����
                try
                {
                    if (pDBType.ToUpper() == "PDB")
                    {
                        //m_WorkSpace.ExecuteSQL("create table GO_DATABASE_UPDATELOG (OID  integer,STATE integer,LAYERNAME varchar(50),USERNAME varchar(255),LASTUPDATE date,VERSION integer,XMIN float,XMAX float,YMIN float,YMAX float)");
                        m_WorkSpace.ExecuteSQL("create table go_database_version (VERSION  integer,USERNAME varchar(255),VERSIONTIME date,DES varchar(255))");

                    }
                    else if (pDBType.ToUpper() == "SDE")
                    {
                        //m_WorkSpace.ExecuteSQL("create table GO_DATABASE_UPDATELOG (OID  INTEGER,STATE INTEGER,LAYERNAME NVARCHAR2(50),USERNAME NVARCHAR2(255),LASTUPDATE DATE,VERSION INTEGER,XMIN FLOAT,XMAX FLOAT,YMIN FLOAT,YMAX FLOAT)");
                        m_WorkSpace.ExecuteSQL("create table go_database_version (VERSION  INTEGER,USERNAME NVARCHAR2(255),VERSIONTIME DATE,DES NVARCHAR2(255))");
                    }
                    else if (pDBType.ToUpper() == "GDB")
                    {
                        string tempFile = netLogPath;
                        FileInfo pFI = new FileInfo(tempFile);
                        string fName = pFI.Name;  //Զ����־����

                        //��־�洢·��
                        string dbPath = m_WorkSpace.PathName;
                        int index = dbPath.LastIndexOf('\\');
                        if (index == -1) return false;
                        string FileDic = dbPath.Substring(0, index);
                        string FileName = FileDic + "\\" + fName;

                        if (File.Exists(FileName))
                        {
                            File.Delete(FileName);
                        }
                        File.Copy(tempFile, FileName);
                    }
                } catch (System.Exception ex)
                {
                    eError = ex;
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(pFeaWS);
                    return false;
                }
            }
            System.Runtime.InteropServices.Marshal.ReleaseComObject(pFeaWS);
            return true;
            //�������
            //try
            //{
            //    if (pDBType.ToUpper() == "PDB")
            //    {
            //        m_WorkSpace.ExecuteSQL("create table GO_DATABASE_UPDATELOG (OID  integer,STATE integer,LAYERNAME varchar(50),USERNAME varchar(255),LASTUPDATE date,VERSION integer,XMIN float,XMAX float,YMIN float,YMAX float)");
            //        m_WorkSpace.ExecuteSQL("create table go_database_version (VERSION  integer,USERNAME varchar(255),VERSIONTIME date,DES varchar(255))");

            //    }
            //    else if (pDBType.ToUpper() == "SDE")
            //    {
            //        m_WorkSpace.ExecuteSQL("create table GO_DATABASE_UPDATELOG (OID  INTEGER,STATE INTEGER,LAYERNAME NVARCHAR2(50),USERNAME NVARCHAR2(255),LASTUPDATE DATE,VERSION INTEGER,XMIN FLOAT,XMAX FLOAT,YMIN FLOAT,YMAX FLOAT)");
            //        m_WorkSpace.ExecuteSQL("create table go_database_version (VERSION  INTEGER,USERNAME NVARCHAR2(255),VERSIONTIME DATE,DES NVARCHAR2(255))");
            //    }
            //    else if (pDBType.ToUpper() == "GDB")
            //    {
            //        string tempFile = netLogPath;
            //        FileInfo pFI = new FileInfo(tempFile);
            //        string fName = pFI.Name;  //Զ����־����

            //        //��־�洢·��
            //        string dbPath = m_WorkSpace.PathName;
            //        int index = dbPath.LastIndexOf('\\');
            //        if (index == -1) return false;
            //        string FileDic = dbPath.Substring(0, index);
            //        string FileName = FileDic + "\\" + fName;

            //        if (File.Exists(FileName))
            //        {
            //            if (!SysCommon.Error.ErrorHandle.ShowFrmInformation("��", "��", "��־�ļ�'" + fName + "'�Ѵ���,\n�Ƿ��滻��"))
            //            {
            //                return true;
            //            }
            //            else
            //            {
            //                File.Delete(FileName);
            //            }
            //        }
            //        File.Copy(tempFile, FileName);
            //    }
            //    System.Runtime.InteropServices.Marshal.ReleaseComObject(pFeaWS);
            //    return true;
            //}
            //catch (System.Exception ex)
            //{
            //    eError = ex;
            //    System.Runtime.InteropServices.Marshal.ReleaseComObject(pFeaWS);
            //    return false;
            //}
            //end
        }

        /// <summary>
        /// ��Ӱ汾�ֶΣ���ʤ��  2010.3.25���
        /// </summary>
        /// <param name="fsEdit"></param>
        private void AddVersionField(IFieldsEdit fsEdit)
        {
            try
            {
                IField newfield = new FieldClass();                //�ֶζ���
                IFieldEdit fieldEdit = newfield as IFieldEdit;     //�ֶα༭����

                //���±������������ֶε�����
                string fieldName = "VERSION";//��¼�ֶ�����
                string fieldType = "esriFieldTypeInteger";//��¼�ֶ�����
                int fieldLen;//��¼�ֶγ���
                bool isNullable = false;//��¼�ֶ��Ƿ������ֵ
                int precision = 0;//����

                bool required = false;
                bool editable = true;
                bool domainfixed = false;   //ֵ���Ƿ���Ըı�

                fieldEdit.Name_2 = fieldName;
                fieldEdit.AliasName_2 = fieldName;
                //�ֶ�����Ҫװ��Ϊö������
                fieldEdit.Type_2 = (esriFieldType)Enum.Parse(typeof(esriFieldType), fieldType, true);
                fieldEdit.IsNullable_2 = isNullable;
                fieldEdit.DefaultValue_2 = 0;  //Ĭ��ֵ

                fieldEdit.Required_2 = required;
                fieldEdit.Editable_2 = editable;
                fieldEdit.DomainFixed_2 = domainfixed;
                newfield = fieldEdit as IField;
                fsEdit.AddField(newfield);
                return;
            }
            catch
            {
                fsEdit = null;
                return;
            }
        }

        /// <summary>
        /// ��ָ�����ݼ��´�����ͨҪ����
        /// </summary>
        /// <param name="pFeatureClassName">Ҫ��������</param>
        /// <param name="pFeatureClassType">Ҫ��������</param>
        /// <param name="fsEdit">�༭�ֶζ���</param>
        /// <param name="fields">�ֶμ��϶���</param>
        /// <param name="pFeatureDataset">���ݼ�����</param>
        private void createCommomFeatureClass(string pFeatureClassName, string pFeatureClassType, IFieldsEdit fsEdit, IFields fields, IFeatureDataset pFeatureDataset)
        {
            try
            {
                string pCommonFeatureType = null;

                switch (pFeatureClassType)
                {
                    case "POINT":
                        pCommonFeatureType = "��";
                        break;
                    case "3DPOINT":
                        pCommonFeatureType = "3D��";
                        break;
                    case "LINE":
                        pCommonFeatureType = "��";
                        break;
                    case "3DLINE":
                        pCommonFeatureType = "3D��";
                        break;
                    case "AREA":
                        pCommonFeatureType = "��";
                        break;
                    case "3DAREA":
                        pCommonFeatureType = "3D��";
                        break;
                    default:
                        break;
                }
                //������ͨͼ��
                # region ������ͨfeatureClass�������ֶ�
                //���Object�ֶ�
                IField newfield2 = new FieldClass();
                IFieldEdit fieldEdit2 = newfield2 as IFieldEdit;
                fieldEdit2.Name_2 = "OBJECTID";
                fieldEdit2.Type_2 = esriFieldType.esriFieldTypeOID;
                fieldEdit2.AliasName_2 = "OBJECTID";
                fieldEdit2.IsNullable_2 = false;
                fieldEdit2.Required_2 = true;
                fieldEdit2.Editable_2 = false;
                newfield2 = fieldEdit2 as IField;
                fsEdit.AddField(newfield2);

                ISpatialReference pSR = null;
                IGeoDataset GeoDataset = pFeatureDataset as IGeoDataset;
                pSR = GeoDataset.SpatialReference;

                //���Geometry�ֶ�
                IField newfield1 = new FieldClass();
                newfield1 = GetGeometryField(newfield1, pCommonFeatureType, pSR);
                if (newfield1 == null) return;
                fsEdit.AddField(newfield1);
                fields = fsEdit as IFields;

                #endregion
                pFeatureDataset.CreateFeatureClass(pFeatureClassName, fields, null, null, esriFeatureType.esriFTSimple, "SHAPE", "");
            }
            catch
            {
                return;
            }
        }
        /// <summary>
        /// �����Զ�����ֶΣ���ָ�������ռ��ָ�����ݼ��´���ע��ͼ��
        /// �����òο�������
        /// </summary>
        /// <param name="feaName">������ע������</param>
        /// <param name="feaworkspace">��ҵ�����ռ�</param>
        /// <param name="fsEditAnno">ע���ֶζ���</param>
        /// <param name="intScale">ע�ǲο�������</param>
        /// <param name="shapeType">Ҫ�����ͣ��������롰ע�ǡ��Ի�ȡע�ǵ�Geometry�ֶ�</param>
        /// <param name="pFeatureDataset">��ע�ǲ����ɵ������ݼ���</param>
        private void createAnnoFeatureClass(string feaName, IFeatureWorkspace feaworkspace, IFieldsEdit fsEditAnno, int intScale, string shapeType, IFeatureDataset pFeatureDataset)
        {
            //����ע�ǵ������ֶ�
            try
            {
                //ע�ǵ�workSpace
                IFeatureWorkspaceAnno pFWSAnno = feaworkspace as IFeatureWorkspaceAnno;

                IGraphicsLayerScale pGLS = new GraphicsLayerScaleClass();
                pGLS.Units = esriUnits.esriMeters;
                pGLS.ReferenceScale = Convert.ToDouble(intScale);//����ע�Ǳ���Ҫ���ñ�����

                IFormattedTextSymbol myTextSymbol = new TextSymbolClass();
                ISymbol pSymbol = (ISymbol)myTextSymbol;
                //AnnoҪ��������е�ȱʡ����
                ISymbolCollection2 pSymbolColl = new SymbolCollectionClass();
                ISymbolIdentifier2 pSymID = new SymbolIdentifierClass();
                pSymbolColl.AddSymbol(pSymbol, "Default", out pSymID);

                //AnnoҪ����ı�Ҫ����
                IAnnotateLayerProperties pAnnoProps = new LabelEngineLayerPropertiesClass();
                pAnnoProps.CreateUnplacedElements = true;
                pAnnoProps.CreateUnplacedElements = true;
                pAnnoProps.DisplayAnnotation = true;
                pAnnoProps.UseOutput = true;

                ILabelEngineLayerProperties pLELayerProps = (ILabelEngineLayerProperties)pAnnoProps;
                pLELayerProps.Symbol = pSymbol as ITextSymbol;
                pLELayerProps.SymbolID = 0;
                pLELayerProps.IsExpressionSimple = true;
                pLELayerProps.Offset = 0;
                pLELayerProps.SymbolID = 0;

                IAnnotationExpressionEngine aAnnoVBScriptEngine = new AnnotationVBScriptEngineClass();
                pLELayerProps.ExpressionParser = aAnnoVBScriptEngine;
                pLELayerProps.Expression = "[DESCRIPTION]";
                IAnnotateLayerTransformationProperties pATP = (IAnnotateLayerTransformationProperties)pAnnoProps;
                pATP.ReferenceScale = pGLS.ReferenceScale;
                pATP.ScaleRatio = 1;

                IAnnotateLayerPropertiesCollection pAnnoPropsColl = new AnnotateLayerPropertiesCollectionClass();
                pAnnoPropsColl.Add(pAnnoProps);

                IObjectClassDescription pOCDesc = new AnnotationFeatureClassDescription();
                IFields fields = pOCDesc.RequiredFields;
                IFeatureClassDescription pFDesc = pOCDesc as IFeatureClassDescription;

                for (int j = 0; j < pOCDesc.RequiredFields.FieldCount; j++)
                {
                    IField tempField = pOCDesc.RequiredFields.get_Field(j);
                    if (tempField.Type == esriFieldType.esriFieldTypeGeometry)
                    {
                        continue;
                    }
                    fsEditAnno.AddField(tempField);
                }
                ISpatialReference pSR = null;
                IGeoDataset GeoDataset = pFeatureDataset as IGeoDataset;
                pSR = GeoDataset.SpatialReference;


                //����xml�ļ���Geometry�ֶο��ܴ��пռ�ο�����˵������Geometry�ֶ�
                //���Geometry�ֶ�
                IField newfield1 = new FieldClass();
                newfield1 = GetGeometryField(newfield1, shapeType, pSR);
                if (newfield1 == null) return;
                fsEditAnno.AddField(newfield1);

                fields = fsEditAnno as IFields;
                pFWSAnno.CreateAnnotationClass(feaName, fields, pOCDesc.InstanceCLSID, pOCDesc.ClassExtensionCLSID, pFDesc.ShapeFieldName, "", pFeatureDataset, null, pAnnoPropsColl, pGLS, pSymbolColl, true);
            }
            catch
            {
                return;
            }
        }

        private void createNonSpatialTable(string feaName, IWorkspace pWS, IFieldEdit fsEdit, IFeatureDataset pFeaDataset)
        {
            //string strSql = "create table " + feaName + " (";
            //if (pDBType.ToUpper() == "PDB")
            //{

            //    //pWS.ExecuteSQL("create table GO_DATABASE_UPDATELOG (OID  integer,STATE integer,LAYERNAME varchar(50),LASTUPDATE date,VERSION integer,XMIN float,XMAX float,YMIN float,YMAX float)");

            //}
            //else if (pDBType.ToUpper() == "SDE")
            //{
            //    //pWS.ExecuteSQL("create table GO_DATABASE_UPDATELOG (OID  INTEGER,STATE INTEGER,LAYERNAME NVARCHAR2(50),LASTUPDATE DATE,VERSION INTEGER,XMIN FLOAT,XMAX FLOAT,YMIN FLOAT,YMAX FLOAT)");
            //}
            //else if (pDBType.ToUpper() == "GDB")
            //{
            //    //GDB�洢���˷ǿռ�����
            //}
        }

        /// <summary>
        /// ���ͼ���ֶ�
        /// </summary>
        /// <param name="newfield1">�ֶζ���</param>
        /// <param name="shapeType">ͼ������</param>
        /// <param name="pSR">�ռ�ο�����</param>
        /// <returns>����ͼ���ֶ�</returns>
        private IField GetGeometryField(IField newfield1, string shapeType, ISpatialReference pSR)
        {
            IFieldEdit fieldEdit1 = newfield1 as IFieldEdit;
            fieldEdit1.Name_2 = "SHAPE";
            fieldEdit1.Type_2 = esriFieldType.esriFieldTypeGeometry;
            IGeometryDef geoDef = new GeometryDefClass();
            IGeometryDefEdit geoDefEdit = geoDef as IGeometryDefEdit;
            if (shapeType == "��")
            {
                geoDefEdit.GeometryType_2 = esriGeometryType.esriGeometryPoint;
            }
            else if (shapeType == "��")
            {
                geoDefEdit.GeometryType_2 = esriGeometryType.esriGeometryPolyline;
            }
            else if (shapeType == "��")
            {
                geoDefEdit.GeometryType_2 = esriGeometryType.esriGeometryPolygon;
            }
            else if (shapeType == "ע��")
            {
                geoDefEdit.GeometryType_2 = esriGeometryType.esriGeometryPolygon;
            }
            else if (shapeType == "3D��")
            {
                geoDefEdit.GeometryType_2 = esriGeometryType.esriGeometryPoint;
                geoDefEdit.HasM_2 = false;
                geoDefEdit.HasZ_2 = true;
            }
            else if (shapeType == "3D��")
            {
                geoDefEdit.GeometryType_2 = esriGeometryType.esriGeometryPolyline;
                geoDefEdit.HasM_2 = false;
                geoDefEdit.HasZ_2 = true;
            }
            else if (shapeType == "3D��")
            {
                geoDefEdit.GeometryType_2 = esriGeometryType.esriGeometryPolygon;
                geoDefEdit.HasM_2 = false;
                geoDefEdit.HasZ_2 = true;
            }


            try
            {
                geoDefEdit.SpatialReference_2 = pSR;
                fieldEdit1.GeometryDef_2 = geoDefEdit as GeometryDef;
                newfield1 = fieldEdit1 as IField;
                return newfield1;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// ����Զ����ֶκ���
        /// </summary>
        /// <param name="pAttrs">����ṹ�����Ա���</param>
        /// <param name="k">���Ա�������</param>
        /// <param name="fsEdit">������Զ����ֶζ���</param>
        private void AddCustomusFields(IAttributeList pAttrs, int k, IFieldsEdit fsEdit)
        {
            try
            {
                IField newfield = new FieldClass();                //�ֶζ���
                IFieldEdit fieldEdit = newfield as IFieldEdit;     //�ֶα༭����

                //��ȡ����������Ϣ
                IAttribute pAttr = pAttrs.get_AttributeByIndex(k);

                //��ȡ��չ������Ϣ
                IAttributeDes pAttrDes = pAttr.Description;

                //���±������������ֶε�����
                string fieldName = "";//��¼�ֶ�����
                esriFieldType fieldType = esriFieldType.esriFieldTypeString;//��¼�ֶ�����
                //string fieldType = "";
                int fieldLen;//��¼�ֶγ���
                bool isNullable = true;//��¼�ֶ��Ƿ������ֵ
                int precision = 0;//����

                bool required = false;
                bool editable = true;
                bool domainfixed = false;   //ֵ���Ƿ���Ըı�

                //����ֶε�����
                fieldName = pAttr.Name;

                //======chenayfei modify ����BLOB�ֶεĶ���============================================
                //fieldType = pAttr.Type.ToString();
                //�����ֶ����ͼ�¼arcgis�ж�Ӧ���ֶ�����
                switch (pAttr.Type)
                {
                    case GeoOneDataCheck.VALUETYPE.GO_VALUETYPE_STRING:
                        fieldType = esriFieldType.esriFieldTypeString;// "esriFieldTypeString";
                        try
                        {
                            fieldEdit.DefaultValue_2 = pAttr.Value.ToString();  //Ĭ��ֵ
                        }
                        catch
                        {
                        }
                        break;
                    case GeoOneDataCheck.VALUETYPE.GO_VALUETYPE_LONG:// "GO_VALUETYPE_LONG":
                        fieldType = esriFieldType.esriFieldTypeInteger;// "esriFieldTypeInteger";
                        try
                        {
                            fieldEdit.DefaultValue_2 = Convert.ToInt32(pAttr.Value.ToString());  //Ĭ��ֵ
                        }
                        catch
                        {
                        }
                        break;
                    case GeoOneDataCheck.VALUETYPE.GO_VALUETYPE_BOOL:// "GO_VALUETYPE_BOOL":
                        fieldType = esriFieldType.esriFieldTypeSmallInteger;// "esriFieldTypeSmallInteger";
                        try
                        {
                            fieldEdit.DefaultValue_2 = Convert.ToBoolean(pAttr.Value.ToString()); //Ĭ��ֵ
                        }
                        catch
                        {
                        }
                        break;
                    case GeoOneDataCheck.VALUETYPE.GO_VALUETYPE_DATE:// "GO_VALUETYPE_DATE":
                        fieldType = esriFieldType.esriFieldTypeDate;// "esriFieldTypeDate";
                        try
                        {
                            fieldEdit.DefaultValue_2 = Convert.ToDateTime(pAttr.Value.ToString());  //Ĭ��ֵ
                        }
                        catch
                        {
                            fieldEdit.DefaultValue_2 = "";
                        }

                        break;
                    case GeoOneDataCheck.VALUETYPE.GO_VALUETYPE_DATETIME:// "GO_VALUETYPE_DATE":
                        fieldType = esriFieldType.esriFieldTypeDate;// "esriFieldTypeDate";
                        try
                        {
                            fieldEdit.DefaultValue_2 = Convert.ToDateTime(pAttr.Value.ToString());  //Ĭ��ֵ
                        }
                        catch
                        {
                            fieldEdit.DefaultValue_2 = "";
                        }
                        break;
                    case GeoOneDataCheck.VALUETYPE.GO_VALUETYPE_FLOAT:// "GO_VALUETYPE_FLOAT":
                        fieldType = esriFieldType.esriFieldTypeSingle;// "esriFieldTypeSingle";
                        try
                        {
                            fieldEdit.DefaultValue_2 = Convert.ToSingle(pAttr.Value.ToString());//Ĭ��ֵ
                        }
                        catch
                        {
                        }
                        break;
                    case GeoOneDataCheck.VALUETYPE.GO_VALUETYPE_DOUBLE:// "GO_VALUETYPE_DOUBLE":
                        fieldType = esriFieldType.esriFieldTypeDouble;// "esriFieldTypeDouble";
                        try
                        {
                            fieldEdit.DefaultValue_2 = Convert.ToDouble(pAttr.Value.ToString());  //Ĭ��ֵ
                        }
                        catch
                        {
                        }
                        break;
                    case GeoOneDataCheck.VALUETYPE.GO_VALUETYPE_BYTE:// "GO_VALUETYPE_DOUBLE":
                        fieldType = esriFieldType.esriFieldTypeBlob;// "esriFieldTypeDouble";
                        try
                        {
                            fieldEdit.DefaultValue_2 = pAttr.Value.ToString();  //Ĭ��ֵ
                        }
                        catch
                        {
                            fieldEdit.DefaultValue_2 = null;
                        }
                        break;

                    default:
                        break;
                }

                isNullable = pAttrDes.AllowNull;
                fieldLen = Convert.ToInt32(pAttrDes.InputWidth);
                precision = Convert.ToInt32(pAttrDes.PrecisionEx);

                required = bool.Parse(pAttrDes.Necessary.ToString());

                fieldEdit.Name_2 = fieldName;
                fieldEdit.AliasName_2 = fieldName;
                //�ֶ�����Ҫװ��Ϊö������
                fieldEdit.Type_2 = fieldType;// (esriFieldType)Enum.Parse(typeof(esriFieldType), fieldType, true);
                fieldEdit.IsNullable_2 = isNullable;
                fieldEdit.Length_2 = fieldLen;
                //fieldEdit.DefaultValue_2 =  pAttr.Value;  //Ĭ��ֵ

                //˫�������Ͳ������þ��ȣ���PDB��GDB�в�����ִ��󣬵�����SDE�л��׳�����Ч���С�����
                if (fieldType != esriFieldType.esriFieldTypeDouble)// "esriFieldTypeDouble")
                {
                    fieldEdit.Precision_2 = precision;
                }

                //========================================================================

                fieldEdit.Required_2 = required;
                fieldEdit.Editable_2 = editable;
                fieldEdit.DomainFixed_2 = domainfixed;
                newfield = fieldEdit as IField;
                fsEdit.AddField(newfield);
                return;
            }
            catch
            {
                fsEdit = null;
                return;
            }
        }
        /// <summary>
        /// �������ݼ�
        /// </summary>
        /// <param name="pFeatureWorkSpace">�����ռ����</param>
        /// <param name="pDatasetName">���ݼ�����</param>
        /// <param name="pFeatureDataset">��������ݼ�����</param>
        /// <param name="ProjectFilePath">�ռ�ο��ļ�·��</param>
        /// <returns></returns>
        private bool createFeatureDataset(IFeatureWorkspace pFeatureWorkSpace, string pDatasetName, out IFeatureDataset pFeatureDataset, ISpatialReference pSR)
        {
            try
            {
                //ISpatialReference pSR = null;
                //ISpatialReferenceFactory pSpatialRefFac = new SpatialReferenceEnvironmentClass();

                //if (!File.Exists(ProjectFilePath))
                //{
                //    pFeatureDataset = null;
                //    return false;
                //}
                //pSR = pSpatialRefFac.CreateESRISpatialReferenceFromPRJFile(ProjectFilePath);

                //ISpatialReferenceResolution pSRR = pSR as ISpatialReferenceResolution;
                //ISpatialReferenceTolerance pSRT = (ISpatialReferenceTolerance)pSR;
                //IControlPrecision2 pSpatialPrecision = (IControlPrecision2)pSR;

                //pSRR.ConstructFromHorizon();//Defines the XY resolution and domain extent of this spatial reference based on the extent of its horizon
                //pSRR.SetDefaultXYResolution();
                //pSRT.SetDefaultXYTolerance();

                pFeatureDataset = pFeatureWorkSpace.CreateFeatureDataset(pDatasetName, pSR);
                if (pFeatureDataset == null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch
            {
                pFeatureDataset = null;
                return false;
            }
        }

        #endregion
    }

    public static class ModGisPub
    {
        /// <summary>
        /// �������ֵ�õ���ΧPolygon
        /// </summary>
        /// <param name="diccoor">�����ֵ�valueΪX@Y</param>
        /// <param name="polygon"></param>
        /// <returns></returns>
        public static bool GetPolygonByCol(Dictionary<int, string> diccoor, out IPolygon polygon, out Exception eError)
        {
            eError = null;
            object after = Type.Missing;
            object before = Type.Missing;
            polygon = new PolygonClass();
            IPointCollection pPointCol = (IPointCollection)polygon;

            try
            {
                for (int index = 0; index < diccoor.Count; index++)
                {
                    string CoorLine = diccoor[index];
                    string[] coors = CoorLine.Split('@');

                    double X = Convert.ToDouble(coors[0]);
                    double Y = Convert.ToDouble(coors[1]);

                    IPoint pPoint = new PointClass();
                    pPoint.PutCoords(X, Y);
                    pPointCol.AddPoint(pPoint, ref before, ref after);
                }

                polygon = (IPolygon)pPointCol;
                polygon.Close();

                if (!IsValidateGeometry(polygon))
                {
                    eError = new Exception("���β�����Ҫ��");
                    return false;
                }
            }
            catch (Exception eX)
            {
                //********************************
                //guozheng added  system exception log
                if (SysCommon.Log.Module.SysLog == null)
                    SysCommon.Log.Module.SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
                SysCommon.Log.Module.SysLog.Write(eX);
                //********************************
                eError = eX;
                return false;
            }

            return true;
        }
        /// <summary>
        ///  ����ָ��һ�ֶε�ICursor xisheng 20120612
        /// </summary>
        /// <param name="pWorkspace"></param>
        /// <param name="sTableName"></param>
        /// <param name="sQueryString"></param>
        /// <param name="sFieldName"></param>
        /// <returns></returns>
        public static ICursor GetQueryCursor(IWorkspace workspace, string tableName, string queryString, string[] fieldNames)
        {
            if (workspace == null) return null;

            try
            {
                IFeatureWorkspace pFeaWorkspace = workspace as IFeatureWorkspace;
                ITable pTable = pFeaWorkspace.OpenTable(tableName);

                IQueryFilter pQueryFilter = new QueryFilterClass();
                pQueryFilter.WhereClause = queryString;
                pQueryFilter.SubFields = fieldNames == null ? "*" : string.Join(",", fieldNames);

                return pTable.Search(pQueryFilter, false);
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {

            }
        }

        /// <summary>
        /// ���һ���������Ƿ�Ƿ�
        /// </summary>
        /// <param name="pgeometry">������</param>
        /// <returns></returns>
        public static bool IsValidateGeometry(IGeometry pgeometry)
        {
            // ��ȡ��Geometry��ԭʼ����
            IPointCollection pOrgPointCol = (IPointCollection)pgeometry;

            // ��ȡ��Geometry��ԭʼPart��
            IGeometryCollection pOrgGeometryCol = (IGeometryCollection)pgeometry;

            // ��Ŀ����п�¡�Ͷ�Ӧ�Ĵ���
            ESRI.ArcGIS.esriSystem.IClone pClone = (ESRI.ArcGIS.esriSystem.IClone)pgeometry;
            IGeometry pGeometryTemp = (IPolygon)pClone.Clone();
            ITopologicalOperator pTopo = (ITopologicalOperator)pGeometryTemp;
            pTopo.Simplify();

            // �õ��µ�Geometry
            pGeometryTemp = (IPolygon)pTopo;

            // ��ȡ�µ�Geometry�ĵ���
            IPointCollection pObjPointCol = (IPointCollection)pGeometryTemp;

            // ��ȡ�µ�Geometry��Part��
            IGeometryCollection pObjGeometryCol = (IGeometryCollection)pGeometryTemp;

            // ���бȽ�
            if (pOrgPointCol.PointCount != pObjPointCol.PointCount) return false;

            if (pOrgGeometryCol.GeometryCount != pObjGeometryCol.GeometryCount) return false;

            return true;
        }

        /// <summary>
        /// ���ŵ�Feature
        /// </summary>
        /// <param name="pMapControl"></param>
        /// <param name="pFeature"></param>
        public static void ZoomToFeature(IMapControlDefault pMapControl, IFeature pFeature)
        {
            if (pFeature == null) return;
            if (pFeature.Shape == null) return;
            IEnvelope pEnvelope = null;
			/*xisheng 20110802 changed*/
            if (pFeature.Shape.GeometryType == esriGeometryType.esriGeometryPoint)
            {
                //ITopologicalOperator pTop = pFeature.Shape as ITopologicalOperator;
                //IGeometry pGeometry = pTop.Buffer(50);
                //pEnvelope = pGeometry.Envelope;

                IActiveView pActiveView = pMapControl.Map as IActiveView;
                //if (!GetPointInEnvelope(pActiveView.Extent, pFeature.Shape as IPoint))
                //{//pActiveView.Extent = pEnvelope; 
                    pMapControl.CenterAt(pFeature.Shape as IPoint);
                //}
                pActiveView.Refresh();
            }
            else
            {
                pEnvelope = pFeature.Extent;
            

	            if (pEnvelope == null) return;
	            pEnvelope.Expand(1.5, 1.5, true);
	            IActiveView pActiveView = pMapControl.Map as IActiveView;
	            pActiveView.Extent = pEnvelope;
	            pActiveView.Refresh();
			}
            /*xisheng 20110802 changed end*/
        }

        /// <summary>
        /// �鿴���Ƿ�����ڷ�Χ����
        /// </summary>
        /// <param name="pEnvelop">���η�Χ</param>
        /// <param name="point">��</param>
        /// <returns></returns>
        private static bool GetPointInEnvelope(IEnvelope pEnvelop, IPoint point)
        {
            if (point.X > pEnvelop.XMax)
            {
                return false;
            }
            if(point.X < pEnvelop.XMin)
            {
                return false;
            }
            if(point.Y < pEnvelop.YMin)
            {
                return false;
            }
            if(point.Y > pEnvelop.YMax)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// ���ŵ�Feature  
        /// chenyafei  20110328  modify   :�޸Ķ�λ
        /// </summary>
        /// <param name="pMapControl"></param>
        /// <param name="pFeature"></param>
        public static void ZoomToFeature(IMapControlDefault pMapControl, IFeature pFeature,ISpatialReference pSpatialRef)
        {
            if (pFeature == null) return;
            if (pFeature.Shape == null) return;
            IEnvelope pEnvelope = null;
            double pDis = 50;
            IGeographicCoordinateSystem pGeoCoorSys = null;
            if (pSpatialRef != null)
            {
                pGeoCoorSys = pSpatialRef as IGeographicCoordinateSystem;
            }
            if (pGeoCoorSys != null)
            {
                //��������ϵ����������ת��
                IUnitConverter pUnitConverter = new UnitConverterClass();
                pDis = pUnitConverter.ConvertUnits(pDis, esriUnits.esriMeters, esriUnits.esriDecimalDegrees);
            }
            
            if (pFeature.Shape.GeometryType == esriGeometryType.esriGeometryPoint)
            {
                ITopologicalOperator pTop = pFeature.Shape as ITopologicalOperator;
                IGeometry pGeometry = pTop.Buffer(pDis);
                pEnvelope = pGeometry.Envelope;
            }
            else
            {
                pEnvelope = pFeature.Extent;
            }

            if (pEnvelope == null) return;
            pEnvelope.Expand(1.5, 1.5, true);
            IActiveView pActiveView = pMapControl.Map as IActiveView;
            pActiveView.Extent = pEnvelope;
            pActiveView.Refresh();
        }

        /// <summary>
        /// ����ͼ����ȾUniqueValueRenderer
        /// </summary>
        /// <param name="pFeatLay">��Ⱦͼ��</param>
        /// <param name="strFieldName">��Ⱦ�ֶ�</param>
        /// <param name="dicFieldValue">��Ⱦֵ��(�ֶ�ֵ,��Ⱦ����)</param>
        /// <param name="dicFieldSymbol">��ȾSymbol��(�ֶ�ֵ,Symbol)</param>
        public static void SetLayerUniqueValueRenderer(IFeatureLayer pFeatLay, string strFieldName, Dictionary<string, string> dicFieldValue, Dictionary<string, ISymbol> dicFieldSymbol, bool bUseDefaultSymbol)
        {
            if (pFeatLay == null || strFieldName == string.Empty || dicFieldValue == null || dicFieldSymbol == null) return;
            IFeatureClass pFeatCls = pFeatLay.FeatureClass;
            IUniqueValueRenderer pUniqueValueRenderer = new UniqueValueRendererClass();
            pUniqueValueRenderer.FieldCount = 1;
            pUniqueValueRenderer.set_Field(0, strFieldName);
            if (bUseDefaultSymbol == true)
            {
                pUniqueValueRenderer.UseDefaultSymbol = true;
            }
            else
            {
                pUniqueValueRenderer.UseDefaultSymbol = false;
            }
            foreach (KeyValuePair<string, string> keyValue in dicFieldValue)
            {
                if (dicFieldSymbol.ContainsKey(keyValue.Key))
                {
                    pUniqueValueRenderer.AddValue(keyValue.Key, "", dicFieldSymbol[keyValue.Key]);
                    pUniqueValueRenderer.set_Label(keyValue.Key, keyValue.Value);
                }
            }

            IGeoFeatureLayer pGeoFeatLay = pFeatLay as IGeoFeatureLayer;
            if (pGeoFeatLay != null) pGeoFeatLay.Renderer = pUniqueValueRenderer as IFeatureRenderer;
        }

        /// <summary>
        /// ��ȡRGB
        /// </summary>
        /// <param name="lngR"></param>
        /// <param name="lngG"></param>
        /// <param name="lngB"></param>
        /// <returns></returns>
        public static IRgbColor GetRGBColor(int lngR, int lngG, int lngB)
        {
            IRgbColor rgbColor = new RgbColorClass();
            rgbColor.Red = lngR;
            rgbColor.Green = lngG;
            rgbColor.Blue = lngB;
            rgbColor.UseWindowsDithering = false;

            return rgbColor;
        }

        /// <summary>
        /// ������ʾ����
        /// </summary>
        /// <param name="pGeometry">����ͼ��</param>
        /// <param name="pActiveView">��ʾ�ؼ�</param>
        public static void FlashGeometry(IGeometry pGeometry, IActiveView pActiveView)
        {
            if (pGeometry == null || pActiveView == null) return;
            //�����ǰͼ��Ŀռ�ο��뵱ǰ��ͼ�Ŀռ�ο���һ��,����ֲ�����˸��Ч��
            pGeometry.Project(pActiveView.FocusMap.SpatialReference);
            IRgbColor pRgbColor = GetRGBColor(255, 150, 150);
            ISymbol pSymbol = null;

            pActiveView.ScreenDisplay.StartDrawing(0, -1);
            switch (pGeometry.GeometryType)
            {
                case esriGeometryType.esriGeometryPoint:
                    ISimpleMarkerSymbol pSimpleMarkerSymbol = new SimpleMarkerSymbolClass();
                    pSimpleMarkerSymbol.Style = esriSimpleMarkerStyle.esriSMSCircle;
                    pSimpleMarkerSymbol.Color = pRgbColor;
                    pSymbol = pSimpleMarkerSymbol as ISymbol;
                    pSymbol.ROP2 = esriRasterOpCode.esriROPNotXOrPen;
                    pActiveView.ScreenDisplay.SetSymbol(pSymbol);
                    pActiveView.ScreenDisplay.DrawPoint(pGeometry);
                    TimeSpan pPointTimeSpan = new TimeSpan(150);
                    pActiveView.ScreenDisplay.DrawPoint(pGeometry);
                    break;
                case esriGeometryType.esriGeometryPolyline:
                    ISimpleLineSymbol pSimpleLineSymbol = new SimpleLineSymbolClass();
                    pSimpleLineSymbol.Color = pRgbColor;
                    pSimpleLineSymbol.Width = 4;
                    pSimpleLineSymbol.Style = esriSimpleLineStyle.esriSLSSolid;
                    pSymbol = pSimpleLineSymbol as ISymbol;
                    pSymbol.ROP2 = esriRasterOpCode.esriROPNotXOrPen;
                    pActiveView.ScreenDisplay.SetSymbol(pSymbol);
                    pActiveView.ScreenDisplay.DrawPolyline(pGeometry);
                    TimeSpan pPolylineTimeSpan = new TimeSpan(150);
                    pActiveView.ScreenDisplay.DrawPolyline(pGeometry);
                    break;
                case esriGeometryType.esriGeometryPolygon:
                    ISimpleFillSymbol pSimpleFillSymbol = new SimpleFillSymbolClass();
                    pSimpleFillSymbol.Outline = null;
                    pSimpleFillSymbol.Color = pRgbColor;
                    pSimpleFillSymbol.Style = esriSimpleFillStyle.esriSFSSolid;
                    pSymbol = pSimpleFillSymbol as ISymbol;
                    pSymbol.ROP2 = esriRasterOpCode.esriROPNotXOrPen;
                    pActiveView.ScreenDisplay.SetSymbol(pSymbol);
                    pActiveView.ScreenDisplay.DrawPolygon(pGeometry);
                    TimeSpan pPolygonTimeSpan = new TimeSpan(150);
                    pActiveView.ScreenDisplay.DrawPolygon(pGeometry);
                    break;
            }
            pActiveView.ScreenDisplay.FinishDrawing();
        }

        // ���ݼ����廭�����Ӧ�ķ�Χ
        public static bool DoDrawRange(IMapControlDefault pMapcontrol, IGeometry pgeometry, int intRed, int intGreen, int intBlue, bool bDel)
        {
            // ������Ҫ�ص�Element
            IGraphicsContainer pMapGraphics = (IGraphicsContainer)pMapcontrol.Map;

            IElement pElement = null;
            IRgbColor pRGBColor = new RgbColorClass();
            pRGBColor.Red = intRed;
            pRGBColor.Green = intGreen;
            pRGBColor.Blue = intBlue;
            switch (pgeometry.GeometryType)
            {
                case esriGeometryType.esriGeometryPolygon:
                    IPolygonElement pPolElemnt = new PolygonElementClass();
                    IFillShapeElement pFillShapeElement = (IFillShapeElement)pPolElemnt;
                    pFillShapeElement.Symbol = GetDrawSymbol(intRed, intGreen, intBlue);

                    pElement = pFillShapeElement as IElement;
                    break;
                case esriGeometryType.esriGeometryPolyline:
                    ILineElement pLineElement = new LineElementClass();
                    ISimpleLineSymbol pLineSymbol = new SimpleLineSymbolClass();
                    pLineSymbol.Color = pRGBColor;
                    pLineElement.Symbol = pLineSymbol;

                    pElement = pLineElement as IElement;
                    break;
                case esriGeometryType.esriGeometryPoint:
                    IMarkerElement pMarkerElemnt = new MarkerElementClass();
                    ISimpleMarkerSymbol pMarkerSymbol = new SimpleMarkerSymbolClass();
                    pMarkerSymbol.Color = pRGBColor;
                    pMarkerSymbol.Size = 2;
                    pMarkerElemnt.Symbol = pMarkerSymbol;

                    pElement = pMarkerElemnt as IElement;
                    break;
                default:
                    return false;
            }

            // ����Element
            pElement.Geometry = pgeometry;

            if (bDel == true)
            {
                pMapGraphics.DeleteAllElements();
            }

            // ���»���Element��ӵ�ͼ�ν���
            pMapGraphics.AddElement(pElement, 0);
            pMapcontrol.Refresh(esriViewDrawPhase.esriViewForeground, null, null);

            // ˢ�µ�ǰ����
            IActiveView pActiveView = (IActiveView)pMapcontrol.Map;
            pActiveView.Refresh();
            return true;
        }

        // ���ݼ����廭�����Ӧ�ķ�Χ
        public static IElement DoDrawGeometry(IMapControlDefault pMapcontrol, IGeometry pgeometry, int intRed, int intGreen, int intBlue, bool bDel)
        {
            // ������Ҫ�ص�Element
            IGraphicsContainer pMapGraphics = (IGraphicsContainer)pMapcontrol.Map;

            IElement pElement = null;
            IRgbColor pRGBColor = new RgbColorClass();
            pRGBColor.Red = intRed;
            pRGBColor.Green = intGreen;
            pRGBColor.Blue = intBlue;
            switch (pgeometry.GeometryType)
            {
                case esriGeometryType.esriGeometryPolygon:
                    IPolygonElement pPolElemnt = new PolygonElementClass();
                    IFillShapeElement pFillShapeElement = (IFillShapeElement)pPolElemnt;
                    pFillShapeElement.Symbol = GetDrawSymbol(intRed, intGreen, intBlue);

                    pElement = pFillShapeElement as IElement;
                    break;
                case esriGeometryType.esriGeometryPolyline:
                    ILineElement pLineElement = new LineElementClass();
                    ISimpleLineSymbol pLineSymbol = new SimpleLineSymbolClass();
                    pLineSymbol.Color = pRGBColor;
                    pLineElement.Symbol = pLineSymbol;

                    pElement = pLineElement as IElement;
                    break;
                case esriGeometryType.esriGeometryPoint:
                    IMarkerElement pMarkerElemnt = new MarkerElementClass();
                    ISimpleMarkerSymbol pMarkerSymbol = new SimpleMarkerSymbolClass();
                    pMarkerSymbol.Color = pRGBColor;
                    pMarkerSymbol.Size = 2;
                    pMarkerElemnt.Symbol = pMarkerSymbol;

                    pElement = pMarkerElemnt as IElement;
                    break;
                default:
                    return null;
            }

            // ����Element
            pElement.Geometry = pgeometry;

            //if (bDel == true)
            //{
            //    pMapGraphics.DeleteAllElements();
            //}
            // ���»���Element��ӵ�ͼ�ν���
            pMapGraphics.AddElement(pElement, 0);
            pMapcontrol.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewBackground, null, null);

            // ˢ�µ�ǰ����
            IActiveView pActiveView = (IActiveView)pMapcontrol.Map;
            pActiveView.PartialRefresh(esriViewDrawPhase.esriViewBackground, null, null);
            return pElement;
        }

        // �õ�����Χ��������
        public static ISimpleFillSymbol GetDrawSymbol(int intRed, int intGreen, int intBlue)
        {
            ISimpleFillSymbol pFillSymbol = new SimpleFillSymbolClass();
            ISimpleLineSymbol pLineSymbol = new SimpleLineSymbolClass();

            IRgbColor pRGBColor = new RgbColorClass();
            pRGBColor.UseWindowsDithering = false;

            ISymbol pSymbol = (ISymbol)pFillSymbol;
            pSymbol.ROP2 = esriRasterOpCode.esriROPNotXOrPen;

            pRGBColor.Red = intRed;
            pRGBColor.Green = intGreen;
            pRGBColor.Blue = intBlue;
            pLineSymbol.Color = pRGBColor;

            pLineSymbol.Width = 0.8;
            pLineSymbol.Style = esriSimpleLineStyle.esriSLSSolid;
            pFillSymbol.Outline = pLineSymbol;

            pFillSymbol.Color = pRGBColor;
            pFillSymbol.Style = esriSimpleFillStyle.esriSFSDiagonalCross;

            return pFillSymbol;
        }

        #region ʵ�ֽ�ͬһ�����ݿ��в�ͬ�ı��ͬ���ݿ��еı�Join����
        /// <summary>
        /// ��ȡ��ϵ��
        /// </summary>
        /// <param name="Name">��ʾ����</param>
        /// <param name="originPrimaryClass">��1</param>
        /// <param name="originPrimaryKeyField">��1�����ֶ�</param>
        /// <param name="originForeignClass">��2</param>
        /// <param name="originForeignKeyField">��2����ֶ�</param>
        /// <param name="ForwardPathLabel">��ע�ֶ�1</param>
        /// <param name="BackwardPathLabel">��ע�ֶ�2</param>
        /// <param name="Cardinality">������Ӧ��ϵ(OneToOne,OneToMany,ManyToMany)</param>
        /// <param name="eError">��������</param>
        /// <returns></returns>
        public static IRelationshipClass GetRelationShipClass(string Name, IObjectClass originPrimaryClass, string originPrimaryKeyField, IObjectClass originForeignClass, string originForeignKeyField, string ForwardPathLabel, string BackwardPathLabel, esriRelCardinality Cardinality, out Exception eError)
        {
            eError = null;
            try
            {
                IMemoryRelationshipClassFactory memoryRelateFactory = new MemoryRelationshipClassFactoryClass();
                IRelationshipClass relationShipClass = memoryRelateFactory.Open(Name, originPrimaryClass, originPrimaryKeyField, originForeignClass, originForeignKeyField, ForwardPathLabel, BackwardPathLabel, Cardinality);
                return relationShipClass;
            }
            catch (Exception ex)
            {
                //********************************
                //guozheng added  system exception log
                if (SysCommon.Log.Module.SysLog == null)
                    SysCommon.Log.Module.SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
                SysCommon.Log.Module.SysLog.Write(ex);
                //********************************
                eError = ex;
                return null;
            }
        }

        /// <summary>
        /// ������ϵ��
        /// </summary>
        /// <param name="relClass">��ϵ��</param>
        /// <param name="joinForward">?</param>
        /// <param name="queryFilter">��������</param>
        /// <param name="target_Columns">�����ֶ�����ַ�����"a,b"</param>
        /// <param name="DoNotPushJoinToDB">?</param>
        /// <param name="openAsLeftOuterJoin">���ݼ�¼�������ĸ�Ϊ׼</param>
        /// <param name="eError">��������</param>
        /// <returns></returns>
        public static IRelQueryTable GetRelQueryTable(IRelationshipClass relClass, bool joinForward, IQueryFilter queryFilter, string target_Columns, bool DoNotPushJoinToDB, bool openAsLeftOuterJoin, out Exception eError)
        {
            eError = null;
            try
            {
                IRelQueryTableFactory relQueryTableFactory = new RelQueryTableFactoryClass();
                IRelQueryTable relQueryTable = relQueryTableFactory.Open(relClass, joinForward, queryFilter, null, target_Columns, DoNotPushJoinToDB, openAsLeftOuterJoin);
                return relQueryTable;
            }
            catch (Exception ex)
            {
                //********************************
                //guozheng added  system exception log
                if (SysCommon.Log.Module.SysLog == null)
                    SysCommon.Log.Module.SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
                SysCommon.Log.Module.SysLog.Write(ex);
                //********************************
                eError = ex;
                return null;
            }
        }
        #endregion

        #region ͼ������
        /// <summary>
        /// ��mapcontrol�ϵ�ͼ���������
        /// </summary>
        /// <param name="vAxMapControl"></param>
        public static void LayersCompose(IMapControlDefault pMapcontrol)
        {
            IMap pMap = pMapcontrol.Map;
            int[] iLayerIndex = new int[2] { 0, 0 };
            int[] iFeatureLayerIndex = new int[3] { 0, 0, 0 };

            int iCount = pMapcontrol.LayerCount;
            for (int iIndex = 0; iIndex < iCount; iIndex++)
            {
                IFeatureLayer pFeatureLayer = pMap.get_Layer(iIndex) as IFeatureLayer;
                IGroupLayer groupTempLayer = pMap.get_Layer(iIndex) as IGroupLayer;
                if (groupTempLayer != null)
                {
                    LayersCompose(groupTempLayer);
                }

                if (pFeatureLayer != null)
                {
                    switch (pFeatureLayer.FeatureClass.FeatureType)
                    {
                        case esriFeatureType.esriFTDimension:
                            pMap.MoveLayer(pFeatureLayer, iLayerIndex[0]);
                            iLayerIndex[0] = iLayerIndex[0] + 1;
                            break;
                        case esriFeatureType.esriFTAnnotation:

                            pMap.MoveLayer(pFeatureLayer, iLayerIndex[0] + iLayerIndex[1]);
                            iLayerIndex[1] = iLayerIndex[1] + 1;
                            break;
                        case esriFeatureType.esriFTSimple:

                            switch (pFeatureLayer.FeatureClass.ShapeType)
                            {
                                case esriGeometryType.esriGeometryPoint:
                                    pMap.MoveLayer(pFeatureLayer, iLayerIndex[0] + iLayerIndex[1] + iFeatureLayerIndex[0]);
                                    iFeatureLayerIndex[0] = iFeatureLayerIndex[0] + 1;
                                    break;
                                case esriGeometryType.esriGeometryLine:
                                case esriGeometryType.esriGeometryPolyline:
                                    pMap.MoveLayer(pFeatureLayer, iLayerIndex[0] + iLayerIndex[1] + iFeatureLayerIndex[0] + iFeatureLayerIndex[1]);
                                    iFeatureLayerIndex[1] = iFeatureLayerIndex[1] + 1;
                                    break;
                                case esriGeometryType.esriGeometryPolygon:
                                    pMap.MoveLayer(pFeatureLayer, iLayerIndex[0] + iLayerIndex[1] + iFeatureLayerIndex[0] + iFeatureLayerIndex[1] + iFeatureLayerIndex[2]);
                                    iFeatureLayerIndex[2] = iFeatureLayerIndex[2] + 1;
                                    break;
                            }
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// ��mapcontrol��groupLayer�ڵ�ͼ���������
        /// </summary>
        /// <param name="groupLayer"></param>
        public static void LayersCompose(IGroupLayer groupLayer)
        {
            ICompositeLayer comLayer = groupLayer as ICompositeLayer;
            int iCount = comLayer.Count;

            List<ILayer> listLays = new List<ILayer>();
            //��Dimension���������
            for (int iIndex = 0; iIndex < iCount; iIndex++)
            {
                IFeatureLayer pFeatureLayer = comLayer.get_Layer(iIndex) as IFeatureLayer;
                IGroupLayer groupTempLayer = comLayer.get_Layer(iIndex) as IGroupLayer;
                if (groupTempLayer != null)
                {
                    LayersCompose(groupTempLayer);
                }

                if (pFeatureLayer == null) break;
                if (pFeatureLayer.FeatureClass.FeatureType == esriFeatureType.esriFTDimension)
                {
                    listLays.Add(pFeatureLayer as ILayer);
                }
            }
            foreach (ILayer pTempLay in listLays)
            {
                groupLayer.Delete(pTempLay);
                groupLayer.Add(pTempLay);
            }

            listLays = new List<ILayer>();
            //��Annotation���������
            for (int iIndex = 0; iIndex < iCount; iIndex++)
            {
                IFeatureLayer pFeatureLayer = comLayer.get_Layer(iIndex) as IFeatureLayer;
                IGroupLayer groupTempLayer = comLayer.get_Layer(iIndex) as IGroupLayer;
                if (groupTempLayer != null)
                {
                    LayersCompose(groupTempLayer);
                }
                if (pFeatureLayer == null) break;
                if (pFeatureLayer.FeatureClass.FeatureType == esriFeatureType.esriFTAnnotation)
                {
                    listLays.Add(pFeatureLayer as ILayer);
                }
            }
            foreach (ILayer pTempLay in listLays)
            {
                groupLayer.Delete(pTempLay);
                groupLayer.Add(pTempLay);
            }

            listLays = new List<ILayer>();
            //�Ե���������
            for (int iIndex = 0; iIndex < iCount; iIndex++)
            {
                IFeatureLayer pFeatureLayer = comLayer.get_Layer(iIndex) as IFeatureLayer;
                IGroupLayer groupTempLayer = comLayer.get_Layer(iIndex) as IGroupLayer;
                if (groupTempLayer != null)
                {
                    LayersCompose(groupTempLayer);
                }
                if (pFeatureLayer == null) break;
                if (pFeatureLayer.FeatureClass.FeatureType == esriFeatureType.esriFTSimple)
                {
                    if (pFeatureLayer.FeatureClass.ShapeType == esriGeometryType.esriGeometryPoint)
                    {
                        listLays.Add(pFeatureLayer as ILayer);
                    }
                }
            }
            foreach (ILayer pTempLay in listLays)
            {
                groupLayer.Delete(pTempLay);
                groupLayer.Add(pTempLay);
            }

            listLays = new List<ILayer>();
            //���߲��������
            for (int iIndex = 0; iIndex < iCount; iIndex++)
            {
                IFeatureLayer pFeatureLayer = comLayer.get_Layer(iIndex) as IFeatureLayer;
                IGroupLayer groupTempLayer = comLayer.get_Layer(iIndex) as IGroupLayer;
                if (groupTempLayer != null)
                {
                    LayersCompose(groupTempLayer);
                }
                if (pFeatureLayer == null) break;
                if (pFeatureLayer.FeatureClass.FeatureType == esriFeatureType.esriFTSimple)
                {
                    if (pFeatureLayer.FeatureClass.ShapeType == esriGeometryType.esriGeometryLine || pFeatureLayer.FeatureClass.ShapeType == esriGeometryType.esriGeometryPolyline)
                    {
                        listLays.Add(pFeatureLayer as ILayer);
                    }
                }
            }
            foreach (ILayer pTempLay in listLays)
            {
                groupLayer.Delete(pTempLay);
                groupLayer.Add(pTempLay);
            }

            listLays = new List<ILayer>();
            //���������
            for (int iIndex = 0; iIndex < iCount; iIndex++)
            {
                IFeatureLayer pFeatureLayer = comLayer.get_Layer(iIndex) as IFeatureLayer;
                IGroupLayer groupTempLayer = comLayer.get_Layer(iIndex) as IGroupLayer;
                if (groupTempLayer != null)
                {
                    LayersCompose(groupTempLayer);
                }
                if (pFeatureLayer == null) break;
                if (pFeatureLayer.FeatureClass.FeatureType == esriFeatureType.esriFTSimple)
                {
                    if (pFeatureLayer.FeatureClass.ShapeType == esriGeometryType.esriGeometryPolygon)
                    {
                        listLays.Add(pFeatureLayer as ILayer);
                    }
                }
            }
            foreach (ILayer pTempLay in listLays)
            {
                groupLayer.Delete(pTempLay);
                groupLayer.Add(pTempLay);
            }

            listLays = null;
        }
        #endregion

        #region Table��ز���ʵ��
        /// <summary>
        /// �½���
        /// </summary>
        /// <param name="pTable">����</param>
        /// <param name="dicvalues">�ֶ�,ֵ��Ӧ����</param>
        /// <param name="eError"></param>
        /// <returns></returns>
        public static bool NewRow(ITable pTable, Dictionary<string, object> dicvalues, out Exception eError)
        {
            eError = null;

            ICursor pCursor = pTable.Insert(false);
            IRowBuffer pRowBuffer = pTable.CreateRowBuffer();
            foreach (KeyValuePair<string, object> keyValue in dicvalues)
            {
                int index = pRowBuffer.Fields.FindField(keyValue.Key);
                if (index == -1)
                {
                    eError = new Exception("�ֶ�" + keyValue.Key + "������");
                    return false;
                }

                try
                {
                    if (pRowBuffer.Fields.get_Field(index).Editable)
                    {
                        pRowBuffer.set_Value(index, keyValue.Value);
                    }
                }
                catch (Exception eX)
                {
                    eError = new Exception("�ֶ�" + keyValue.Key + "������ֵ��ƥ��");
                    //********************************
                    //guozheng added  system exception log
                    if (SysCommon.Log.Module.SysLog == null)
                        SysCommon.Log.Module.SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
                    SysCommon.Log.Module.SysLog.Write(eX);
                    SysCommon.Log.Module.SysLog.Write(eError);
                    //********************************
                    return false;
                }
            }

            try
            {
                pCursor.InsertRow(pRowBuffer);
            }
            catch (Exception eR)
            {
                //********************************
                //guozheng added  system exception log
                if (SysCommon.Log.Module.SysLog == null)
                    SysCommon.Log.Module.SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
                SysCommon.Log.Module.SysLog.Write(eR);
                //********************************
                eError = eR;
                return false;
            }

            return true;
        }
        /// <summary>
        /// �½���
        /// </summary>
        /// <param name="pTable">����</param>
        /// <param name="dicvalues">�ֶ�,ֵ��Ӧ����</param>
        /// <param name="eError"></param>
        /// <returns></returns>
        public static bool NewRowByAliasName(ITable pTable, Dictionary<string, object> dicvalues, out Exception eError)
        {
            eError = null;

            ICursor pCursor = pTable.Insert(false);
            IRowBuffer pRowBuffer = pTable.CreateRowBuffer();
            foreach (KeyValuePair<string, object> keyValue in dicvalues)
            {
                int index = pRowBuffer.Fields.FindFieldByAliasName(keyValue.Key);
                if (index == -1)
                {
                    eError = new Exception("�ֶ�" + keyValue.Key + "������");
                    return false;
                }

                try
                {
                    if (pRowBuffer.Fields.get_Field(index).Editable)
                    {
                        pRowBuffer.set_Value(index, keyValue.Value);
                    }
                }
                catch (Exception eX)
                {
                    eError = new Exception("�ֶ�" + keyValue.Key + "������ֵ��ƥ��");
                    //********************************
                    //guozheng added  system exception log
                    if (SysCommon.Log.Module.SysLog == null)
                        SysCommon.Log.Module.SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
                    SysCommon.Log.Module.SysLog.Write(eX);
                    SysCommon.Log.Module.SysLog.Write(eError);
                    //********************************
                    return false;
                }
            }

            try
            {
                pCursor.InsertRow(pRowBuffer);
            }
            catch (Exception eR)
            {
                //********************************
                //guozheng added  system exception log
                if (SysCommon.Log.Module.SysLog == null)
                    SysCommon.Log.Module.SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
                SysCommon.Log.Module.SysLog.Write(eR);
                //********************************
                eError = eR;
                return false;
            }

            return true;
        }
        /// <summary>
        /// ɾ����
        /// </summary>
        /// <param name="pTable">����</param>
        /// <param name="strDelCon">ɾ������</param>
        /// <param name="eError"></param>
        /// <returns></returns>
        public static bool DelRow(ITable pTable, string strDelCon, out Exception eError)
        {
            eError = null;
            IQueryFilter pQueryFilter = new QueryFilterClass();
            pQueryFilter.WhereClause = strDelCon;
            try
            {
                pTable.DeleteSearchedRows(pQueryFilter);
            }
            catch (Exception eR)
            {
                //********************************
                //guozheng added  system exception log
                if (SysCommon.Log.Module.SysLog == null)
                    SysCommon.Log.Module.SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
                SysCommon.Log.Module.SysLog.Write(eR);
                //********************************
                eError = eR;
                return false;
            }

            return true;
        }
        /// <summary>
        /// �޸���
        /// </summary>
        /// <param name="pTable">����</param>
        /// <param name="strDelCon">�޸�����</param>
        /// <param name="dicvalues">�ֶ�,ֵ��Ӧ����</param>
        /// <param name="eError"></param>
        /// <returns></returns>
        public static bool UpdateRowByAliasName(ITable pTable, string strDelCon, Dictionary<string, object> dicvalues, out Exception eError)
        {
            eError = null;
            IQueryFilter pQueryFilter = new QueryFilterClass();
            pQueryFilter.WhereClause = strDelCon;

            ICursor pCursor = null;
            try
            {
                pCursor = pTable.Update(pQueryFilter, false);
            }
            catch (Exception eR)
            {
                //********************************
                //guozheng added  system exception log
                if (SysCommon.Log.Module.SysLog == null)
                    SysCommon.Log.Module.SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
                SysCommon.Log.Module.SysLog.Write(eR);
                //********************************
                eError = new Exception("���˲�ѯ�������ô���");
                return false;
            }

            IRow pRow = pCursor.NextRow();
            if (pRow == null) return false;

            foreach (KeyValuePair<string, object> keyValue in dicvalues)
            {
                int index = pRow.Fields.FindFieldByAliasName(keyValue.Key);//changed by xisheng 06.17
                if (index == -1)
                {
                    eError = new Exception("�ֶ�" + keyValue.Key + "������");
                    return false;
                }

                try
                {
                    if (keyValue.Value != pRow.get_Value(index))//������ȲŸ�ֵ
                        pRow.set_Value(index, keyValue.Value);
                }
                catch (Exception eR)
                {
                    eError = new Exception("�ֶ�" + keyValue.Key + "������ֵ��ƥ��");
                    //********************************
                    //guozheng added  system exception log
                    if (SysCommon.Log.Module.SysLog == null)
                        SysCommon.Log.Module.SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
                    SysCommon.Log.Module.SysLog.Write(eR);
                    SysCommon.Log.Module.SysLog.Write(eError);
                    //********************************
                    return false;
                }
            }

            try
            {
                pCursor.UpdateRow(pRow);
            }
            catch (Exception eR)
            {
                //********************************
                //guozheng added  system exception log
                if (SysCommon.Log.Module.SysLog == null)
                    SysCommon.Log.Module.SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
                SysCommon.Log.Module.SysLog.Write(eR);
                //********************************
                eError = eR;
                return false;
            }

            return true;
        }
        /// <summary>
        /// �޸���
        /// </summary>
        /// <param name="pTable">����</param>
        /// <param name="strDelCon">�޸�����</param>
        /// <param name="dicvalues">�ֶ�,ֵ��Ӧ����</param>
        /// <param name="eError"></param>
        /// <returns></returns>
        public static bool UpdateRow(ITable pTable, string strDelCon, Dictionary<string, object> dicvalues, out Exception eError)
        {
            eError = null;
            IQueryFilter pQueryFilter = new QueryFilterClass();
            pQueryFilter.WhereClause = strDelCon;

            ICursor pCursor = null;
            try
            {
                pCursor = pTable.Update(pQueryFilter, false);
            }
            catch (Exception eR)
            {
                //********************************
                //guozheng added  system exception log
                if (SysCommon.Log.Module.SysLog == null)
                    SysCommon.Log.Module.SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
                SysCommon.Log.Module.SysLog.Write(eR);
                //********************************
                eError = new Exception("���˲�ѯ�������ô���");
                return false;
            }

            IRow pRow = pCursor.NextRow();
            if (pRow == null) return false;

            foreach (KeyValuePair<string, object> keyValue in dicvalues)
            {
                int index = pRow.Fields.FindField(keyValue.Key);//changed by xisheng 06.17
                if (index == -1)
                {
                    eError = new Exception("�ֶ�" + keyValue.Key + "������");
                    return false;
                }

                try
                {
                    if(keyValue.Value!=pRow.get_Value(index))//������ȲŸ�ֵ
                        pRow.set_Value(index, keyValue.Value);
                }
                catch (Exception eR)
                {
                    eError = new Exception("�ֶ�" + keyValue.Key + "������ֵ��ƥ��");
                    //********************************
                    //guozheng added  system exception log
                    if (SysCommon.Log.Module.SysLog == null)
                        SysCommon.Log.Module.SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
                    SysCommon.Log.Module.SysLog.Write(eR);
                    SysCommon.Log.Module.SysLog.Write(eError);
                    //********************************
                    return false;
                }
            }

            try
            {
                pCursor.UpdateRow(pRow);
            }
            catch (Exception eR)
            {
                //********************************
                //guozheng added  system exception log
                if (SysCommon.Log.Module.SysLog == null)
                    SysCommon.Log.Module.SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
                SysCommon.Log.Module.SysLog.Write(eR);
                //********************************
                eError = eR;
                return false;
            }

            return true;
        }
        #endregion

        #region Featureclass��ز���ʵ��
        /// <summary>
        /// ɾ��ָ��Ҫ��
        /// </summary>
        /// <param name="pFeatCls">Ҫ����</param>
        /// <param name="strCon">ָ������</param>
        /// <param name="eError"></param>
        public static void DelFeature(IFeatureClass pFeatCls, string strCon, out Exception eError)
        {
            eError = null;
            try
            {
                IQueryFilter pQueryFilter = new QueryFilterClass();
                pQueryFilter.WhereClause = strCon;
                IFeatureCursor pFcursor = pFeatCls.Update(pQueryFilter, false);
                IFeature pFeatTemp = pFcursor.NextFeature();
                if (pFeatTemp != null)
                {
                    pFeatTemp.Delete();
                }

                System.Runtime.InteropServices.Marshal.ReleaseComObject(pFcursor);
            }
            catch (Exception exError)
            {
                //********************************
                //guozheng added  system exception log
                if (SysCommon.Log.Module.SysLog == null)
                    SysCommon.Log.Module.SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
                SysCommon.Log.Module.SysLog.Write(exError);
                //********************************
                eError = exError;
            }
        }
        #endregion

        //����MXD�ĵ�Ϊͼ�����÷���
        public static void RenderLayerByMxd(String MxdPath, IMapControlDefault pMapcontrol, out Exception errOut)
        {
            errOut = null;

            try
            {
                //����MXD�ĵ�
                IMapDocument pMapDocument = new MapDocumentClass();
                pMapDocument.Open(MxdPath, "");
                IMap pMap = pMapDocument.get_Map(0);
                if (pMap == null) return;

                //���ŷ����ֵ�
                Dictionary<string, IFeatureRenderer> dicValue = new Dictionary<string, IFeatureRenderer>();
                //�����С�ɼ��������ֵ�
                Dictionary<string, ILayer> dicScaleRange = new Dictionary<string, ILayer>();
                for (int i = 0; i < pMap.LayerCount; i++)
                {
                    ILayer pLayer = pMap.get_Layer(i);
                    IGeoFeatureLayer pGeoFeatureLayer = pLayer as IGeoFeatureLayer;

                    if (pGeoFeatureLayer == null) continue;

                    string name = string.Empty;
                    IDataset pDataset = (IDataset)pGeoFeatureLayer.FeatureClass;
                    if (pDataset != null)
                    {
                        name = pDataset.Name;
                    }
                    else
                    {
                        name = pLayer.Name;
                    }
                    if (name.Contains(".")) name = name.Substring(name.LastIndexOf('.') + 1);

                    if (!dicScaleRange.ContainsKey(name))
                    {
                        dicScaleRange.Add(name, pLayer);
                    }


                    IFeatureRenderer pFeatureRender = pGeoFeatureLayer.Renderer;
                    if (!dicValue.ContainsKey(name))
                    {
                        dicValue.Add(name, pFeatureRender);
                    }
                }

                if (dicValue.Count == 0) return;

                //���òο�������
                pMapcontrol.ReferenceScale = pMap.ReferenceScale;

                //���÷��ź���ʵ������
                for (int i = 0; i < pMapcontrol.LayerCount; i++)
                {
                    ILayer pLayer = pMapcontrol.get_Layer(i);
                    if (pLayer is IGroupLayer)
                    {
                        ScaleVisibleGroupLayer(pLayer as IGroupLayer, dicScaleRange);
                        RenderGroupLayer(pLayer as IGroupLayer, dicValue);
                    }
                    else if (pLayer is IGeoFeatureLayer)
                    {
                        string name = ((pLayer as IFeatureLayer).FeatureClass as IDataset).Name;
                        if (name.Contains(".")) name = name.Substring(name.LastIndexOf('.') + 1);
                        if (dicValue.ContainsKey(name))
                        {
                            (pLayer as IGeoFeatureLayer).Renderer = dicValue[name];

                            pLayer.MaximumScale = dicScaleRange[name].MaximumScale;
                            pLayer.MinimumScale = dicScaleRange[name].MinimumScale;

                        }
                    }
                }

            }
            catch (Exception err)
            {
                //********************************
                //guozheng added  system exception log
                if (SysCommon.Log.Module.SysLog == null)
                    SysCommon.Log.Module.SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
                SysCommon.Log.Module.SysLog.Write(err);
                //********************************
                errOut = err;
            }
        }

        private static void ScaleVisibleGroupLayer(IGroupLayer pLayer, Dictionary<string, ILayer> dicScaleRange)
        {
            ICompositeLayer pCompositeLayer = pLayer as ICompositeLayer;
            for (int i = 0; i < pCompositeLayer.Count; i++)
            {
                ILayer mLayer = pCompositeLayer.get_Layer(i);
                if (mLayer is IGroupLayer)
                {
                    ScaleVisibleGroupLayer(mLayer as IGroupLayer, dicScaleRange);
                }
                else if (mLayer is IGeoFeatureLayer)
                {
                    string name = ((mLayer as IFeatureLayer).FeatureClass as IDataset).Name;
                    if (name.Contains(".")) name = name.Substring(name.LastIndexOf('.') + 1);
                    if (dicScaleRange.ContainsKey(name))
                    {
                        mLayer.MaximumScale = dicScaleRange[name].MaximumScale;
                        mLayer.MinimumScale = dicScaleRange[name].MinimumScale;

                    }
                }
            }
        }

        private static void RenderGroupLayer(IGroupLayer pLayer, Dictionary<string, IFeatureRenderer> dicValue)
        {
            ICompositeLayer pCompositeLayer = pLayer as ICompositeLayer;
            for (int i = 0; i < pCompositeLayer.Count; i++)
            {
                ILayer mLayer = pCompositeLayer.get_Layer(i);
                if (mLayer is IGroupLayer)
                {
                    RenderGroupLayer(mLayer as IGroupLayer, dicValue);
                }
                else if (mLayer is IGeoFeatureLayer)
                {
                    string name = ((mLayer as IFeatureLayer).FeatureClass as IDataset).Name;
                    if (name.Contains(".")) name = name.Substring(name.LastIndexOf('.') + 1);
                    if (dicValue.ContainsKey(name))
                    {
                        (mLayer as IGeoFeatureLayer).Renderer = dicValue[name];
                    }
                }
            }
        }

        //chenxinwei Add 20110730
        /// <summary>
        /// ���Ҳ��� ����x���ͶӰ�ļ�
        /// </summary>
        /// <param name="dblX"></param>
        /// <returns></returns>
        public static ISpatialReference GetSpatialByX(double dblX)
        {
            string strPrjFileName = "";
            if (dblX < 112.5)
            {
                strPrjFileName = "Xian 1980 3 Degree GK CM 111E.prj";
            }
            else if (dblX < 115.5)
            {
                strPrjFileName = "Xian 1980 3 Degree GK CM 114E.prj";
            }
            else if (dblX < 118.5)
            {
                strPrjFileName = "Xian 1980 3 Degree GK CM 117E.prj";
            }
            else
            {
                strPrjFileName = "WGS 1984 PDC Mercator.prj";
            }
            

            //
            strPrjFileName = Application.StartupPath + "\\..\\Prj\\" + strPrjFileName;
            if (!System.IO.File.Exists(strPrjFileName)) return null;

            ISpatialReferenceFactory pSpaFac = new SpatialReferenceEnvironmentClass();
            return pSpaFac.CreateESRISpatialReferenceFromPRJFile(strPrjFileName);

        }

        /// <summary>
        /// ��õ�ǰmap�е�����ͼ��
        /// </summary>
        /// <param name="pMap"></param>
        /// <param name="lstLyrs"></param>
        public static void GetLayersByMap(ESRI.ArcGIS.Carto.IMap pMap, ref List<ILayer> lstLyrs)
        {
            lstLyrs = new List<ILayer>();
            for (int i = 0; i < pMap.LayerCount; i++)
            {
                GetLyrsByLyr(pMap.get_Layer(i), ref lstLyrs);
            }
        }

        public static void GetLyrsByLyr(ILayer pLyr, ref List<ILayer> lstLyrs)
        {
            if (pLyr != null)
            {
                if (pLyr is IGroupLayer)
                {
                    ICompositeLayer pComLyr = pLyr as ICompositeLayer;
                    for (int i = 0; i < pComLyr.Count; i++)
                    {
                        GetLyrsByLyr(pComLyr.get_Layer(i), ref lstLyrs);
                    }
                }
                else
                {
                    lstLyrs.Add(pLyr);
                }
            }
        }
        /// <summary>
        /// ZQ 2011 1203 ��ȡfeature�ĸ���
        /// </summary>
        /// <param name="pFeatureClass"></param>
        /// <param name="WhereClause"></param>
        /// <returns></returns>
        public static int  GetFeatureCount(IFeatureClass pFeatureClass,string WhereClause)
        {
            int iFeatureCount = 0;
            try
            {
                IDataset pDataset = pFeatureClass as IDataset;
                IFeatureWorkspace pFeatureWorkspace = pDataset.Workspace as IFeatureWorkspace;
                IQueryDef pQueryDef = pFeatureWorkspace.CreateQueryDef();
                pQueryDef.Tables = pDataset.Name.ToString();
                pQueryDef.SubFields = "count(*)";
                pQueryDef.WhereClause = WhereClause;
                ICursor pCursor = pQueryDef.Evaluate();
                IRow pRow = pCursor.NextRow();
                iFeatureCount =Convert.ToInt32(pRow.get_Value(0).ToString());
                return iFeatureCount;
            }
            catch { return iFeatureCount =0; }
        }
        /// <summary>
        /// ��ITableת��ΪDataTable
        /// </summary>
        /// <param name="pTable"></param>
        /// <param name="sTableName"></param>
        /// <returns></returns>
        public static DataTable ITableToDataTable(ITable pTable, string sTableName)
        {
            DataTable pDataTable = new DataTable(sTableName);
            try
            {
                IQueryFilter pQueryFilter = new QueryFilterClass();
                ICursor pCursor = pTable.Search(pQueryFilter, false);
                IRow pRow = pCursor.NextRow();
                if (pRow != null)
                {
                    for (int i = 0; i < pRow.Fields.FieldCount; i++)
                    {
                        pDataTable.Columns.Add(pRow.Fields.get_Field(i).Name);
                    }
                    while (pRow != null)
                    {
                        DataRow pDataRow = pDataTable.NewRow();
                        for (int j = 0; j < pCursor.Fields.FieldCount; j++)
                            pDataRow[j] = pRow.get_Value(j);
                        pDataTable.Rows.Add(pDataRow);
                        pRow = pCursor.NextRow();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return pDataTable;
        }



        /// <summary>
        /// IFDOToADOConnection��ȡ���Ա�
        /// </summary>
        /// <param name="pFeatureClass"></param>
        /// <returns></returns>
        public static DataTable GetFeaturClassTable(IFeatureClass pFeatureClass)
        {
            IDataset pDataset = pFeatureClass as IDataset;
            IFeatureWorkspace pFeawks = pDataset.Workspace as IFeatureWorkspace;
            ITable pTable = null;
            try
            {
                pTable = pFeawks.OpenTable(pDataset.Name);
            }
            catch
            { }
            DataTable dtTerritories = null;
            try
            {
                dtTerritories = ITableToDataTable(pTable, pDataset.Name);
            }
            catch
            { }
            return dtTerritories;
            //IDataset pDataset = pFeatureClass as IDataset;
            //IFDOToADOConnection pFDOToADOConnection = new FdoAdoConnectionClass();
            
            //ADODB.Connection pADODBConnection = new ADODB.Connection();
            //ADODB.Recordset pADODBRecordSet = new ADODB.Recordset();
            //DataTable dtTerritories = new DataTable();
            //try
            //{
            //    string strSQL = GetFiledSQL(pFeatureClass);
            //    pFDOToADOConnection.Connect(pDataset.Workspace, pADODBConnection);                
            //    pADODBRecordSet.Open("Select " + strSQL + " from " + pDataset.Name, pADODBConnection, ADODB.CursorTypeEnum.adOpenForwardOnly, ADODB.LockTypeEnum.adLockOptimistic, 0);
            //    OleDbDataAdapter custDA = new OleDbDataAdapter();
            //    custDA.Fill(dtTerritories, pADODBRecordSet);
            //    return dtTerritories;
            //}
            //catch(Exception err)
            //{
            //    return dtTerritories = null;
            //}
            //finally
            //{

            //    pADODBConnection.Close();
            //    pADODBConnection = null;
            //    pADODBRecordSet = null;
            //}

        }
        public static string GetFiledSQL(IFeatureClass pFeatureClass)
        {
            IFields pFields = pFeatureClass.Fields;
            string strSQL="";
            for (int i = 0; i < pFields.FieldCount;i++ )
            {
                string strField = pFields.get_Field(i).Name.ToString();
                if (strField == "SHAPE" || strField == "Shape" || pFields.get_Field(i).Type == esriFieldType.esriFieldTypeBlob)
                {
                    continue;
                }
                //if (!strField.Contains("SHAPE.") || !strField.Contains("Shape_"))
                //{
                //    strSQL = strSQL + strField + ",";
                //}
                if (i == pFields.FieldCount - 1)
                {
                    strSQL = strSQL + strField;
                }
                else { strSQL = strSQL + strField + ","; }
            }
            return strSQL;
        }
        public enum eumDataType
        {
            Oracle =0,
            PDB = 1,
            GDB= 2,
            Shp =3,
        }
        //��ȡ���ݿ���������ͣ�ORACLE MDB GDB��
        public static eumDataType GetDescriptionOfWorkspace(IWorkspace pWorkspace)
        {
            
            if (pWorkspace == null)
            {
                return eumDataType.PDB;
            }
            IWorkspaceFactory pWorkSpaceFac = pWorkspace.WorkspaceFactory;
            if (pWorkSpaceFac == null)
            {
                return eumDataType.PDB;
            }
            string strDescrip = pWorkSpaceFac.get_WorkspaceDescription(false);
            switch (strDescrip)
            {
                case "Personal Geodatabase"://mdb���ݿ� ʹ��*��ƥ���
                    return eumDataType.PDB;
                    break;
                case "File Geodatabase"://gdb���ݿ� ʹ��%��ƥ���
                    return eumDataType.GDB;
                    break;
                case "Spatial Database Connection"://sde(oracle���ݿ�) ʹ��%��ƥ���(sql server���ݿ⣬������δ����)
                    return eumDataType.Oracle;
                    break;
                case"Shapefiles":
                    return eumDataType.Shp;
                    break;
                default:
                    return eumDataType.PDB;
                    break;
            }
           return eumDataType.PDB;
        }

    }

}
