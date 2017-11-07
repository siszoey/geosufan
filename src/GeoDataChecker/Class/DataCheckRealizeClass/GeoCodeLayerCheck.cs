using System;
using System.Collections.Generic;
using System.Text;
using ESRI.ArcGIS.Carto;
using System.Data;
using System.Windows.Forms;

using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;

namespace GeoDataChecker
{
    /// <summary>
    /// ����ͼ���顣�������
    /// </summary>
    public class GeoCodeLayerCheck: IDataCheckRealize
    {
        public event DataErrTreatHandle DataErrTreat;
        public event ProgressChangeHandle ProgressShow;

        private IArcgisDataCheckHook Hook;
        public GeoCodeLayerCheck()
        {
        }

        #region IDataCheck ��Ա

        public void OnCreate(IDataCheckHook hook)
        {
            Hook = hook as IArcgisDataCheckHook;
        }

        public void OnDataCheck()
        {
            if (Hook == null) return;
            IArcgisDataCheckParaSet dataCheckParaSet = Hook.DataCheckParaSet as IArcgisDataCheckParaSet;
            if (dataCheckParaSet == null) return;
            if (dataCheckParaSet.Workspace == null) return;

            //��ȡ�������ݼ�
            SysCommon.Gis.SysGisDataSet sysGisDataSet = new SysCommon.Gis.SysGisDataSet(dataCheckParaSet.Workspace);
            List<IDataset> lstDT = sysGisDataSet.GetAllFeatureClass();
            string path = dataCheckParaSet.Workspace.PathName;
            ExcuteLayerCheck(lstDT, path);
        }

        /// <summary>
        /// ��÷�������ֶ���
        /// </summary>
        /// <param name="outErr"></param>
        /// <returns></returns>
        private string GetClassifyName1(out Exception outErr)
        {
            outErr = null;
            Exception eError = null;

            SysCommon.DataBase.SysTable pSysTable = new SysCommon.DataBase.SysTable();
            pSysTable.SetDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + TopologyCheckClass.GeoDataCheckParaPath, SysCommon.enumDBConType.OLEDB, SysCommon.enumDBType.ACCESS, out eError);
            if (eError != null)
            {
                outErr = new Exception("���ӿ������·��Ϊ��" + TopologyCheckClass.GeoDataCheckParaPath);
                pSysTable.CloseDbConnection();
                return "";
            }
            string str = "select * from GeoCheckPara where ����ID=1";//���������Ϣ
            DataTable tempDt = pSysTable.GetSQLTable(str, out eError);
            if (eError != null || tempDt.Rows.Count == 0)
            {
                outErr = new Exception("��ȡ�������������Ϣ����");
                pSysTable.CloseDbConnection();
                return "";
            }
            pSysTable.CloseDbConnection();
            string pClassifyName = tempDt.Rows[0]["����ֵ"].ToString();//��������ֶ���
            return pClassifyName;

        }



        /// <summary>
        /// ����ͼ����---���������
        /// </summary>
        private void ExcuteLayerCheck(List<IDataset> LstDataset, string path)
        {
            Exception eError=null;

            string pClassifyName = GetClassifyName1(out eError);
            if (eError != null || pClassifyName == "")
            {
                return;
            }

            //�����洢MapControl�ϵ�ͼ��ķ������������Ϣ
            Dictionary<IFeatureClass, Dictionary<string, List<int>>> DicFea = new Dictionary<IFeatureClass, Dictionary<string, List<int>>>();

            foreach (IDataset pDT in LstDataset)
            {
                IFeatureClass pFeatureClass = pDT as IFeatureClass;
                if (pFeatureClass == null) return;
                
                #region ���ȼ��Mapcontrol�ϵ�Ҫ�����Ƿ���з����������ֶ�
                int index = -1;   //�����������
                index = pFeatureClass.Fields.FindField(pClassifyName);
                if (index == -1)
                {
                    //SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "�����ڷ�������ֶΣ�");
                    //return;
                    continue;
                }
                #endregion

                #region ��MapControl�ϵ�ͼ�������Ϣ���ֵ�洢����
                IFeatureCursor pFeaCursor = pFeatureClass.Search(null, false);
                if (pFeaCursor == null) return;
                IFeature pFeature = pFeaCursor.NextFeature();
                if (pFeature == null) continue;
                while (pFeature != null)
                {
                    string pGISID = pFeature.get_Value(index).ToString().Trim();  //�������
                    int pOID = pFeature.OID;                   //OID

                    if (!DicFea.ContainsKey(pFeatureClass))
                    {
                        //��������GISID�Ͷ�Ӧ��OID
                        Dictionary<string, List<int>> DicCode = new Dictionary<string, List<int>>();
                        List<int> LstOID = new List<int>();
                        LstOID.Add(pOID);
                        DicCode.Add(pGISID, LstOID);
                        DicFea.Add(pFeatureClass, DicCode);
                    }
                    else
                    {
                        if (!DicFea[pFeatureClass].ContainsKey(pGISID))
                        {
                            List<int> LstOID = new List<int>();
                            LstOID.Add(pOID);
                            DicFea[pFeatureClass].Add(pGISID, LstOID);
                        }
                        else
                        {
                            DicFea[pFeatureClass][pGISID].Add(pOID);
                        }
                    }
                    pFeature = pFeaCursor.NextFeature();
                }

                //�ͷ�CURSOR
                System.Runtime.InteropServices.Marshal.ReleaseComObject(pFeaCursor);

                #endregion

            }

            //���ý�����
            ProgressChangeEvent eInfo = new ProgressChangeEvent();
            eInfo.Max = DicFea.Count;
            int pValue = 0;

            //int pMax = DicFea.Count;
            //GeoDataChecker._CheckForm.Invoke(new GeoDataChecker.IntiProgressBar(GeoDataChecker.intiaProgress), new object[] { pMax });

            #region ���з��������
            //����ͼ��
            foreach (KeyValuePair<IFeatureClass, Dictionary<string, List<int>>> FeaCls in DicFea)
            {
                IFeatureClass pFeaCls = FeaCls.Key;
                IDataset pDT = pFeaCls as IDataset;
                if (pDT == null) return;
                string pFeaClsNameStr = pDT.Name.Trim();                 //Ҫ��������
                if(pFeaClsNameStr.Contains("."))
                {
                    pFeaClsNameStr = pFeaClsNameStr.Substring(pFeaClsNameStr.IndexOf('.') + 1);
                }

                #region �����������ͼ��Ķ�Ӧ��ϵ
                //�����������ֵ�����м��
                foreach (KeyValuePair<string, List<int>> pGISIDItem in FeaCls.Value)
                {
                    //��������ֵ
                    string ppGISID = pGISIDItem.Key;
                    string sqlStr = "select * from GeoCheckCode where ������� ='" + ppGISID + "'";
                    int pResult = CodeStandardizeCheck(sqlStr);                       //���÷�������Ƿ������ģ�����
                    if (pResult == -1)
                    {
                        return;
                    }
                    if (pResult == 1)
                    {
                        //����������
                        #region �����������ͼ�����Ķ�Ӧ��ϵ�Ƿ���ȷ

                        //���ܹ��ҵ��÷�����룬�����������Ӧ��ͼ�����Ƿ���ȷ
                        string sqlStr2 = "select * from GeoCheckCode where ������� ='" + ppGISID + "' and ͼ��='" + pFeaClsNameStr + "'";
                        int pResult2 = CodeStandardizeCheck(sqlStr2);                 //����������Ӧ��ͼ�����Ƿ���ȷ
                        if (pResult2 == -1)
                        {
                            return;
                        }
                        if (pResult2 == 1)
                        {
                            //��Ӧ��ϵ��ȷ
                            continue;

                        }
                        if (pResult2 == 0)
                        {
                            //��Ӧ��ϵ����ȷ

                            #region ���������
                            //�����÷�������Ӧ��Ҫ��OID����
                            for (int m = 0; m < pGISIDItem.Value.Count; m++)
                            {
                                int pOID = pGISIDItem.Value[m];

                                IFeature pFeature = pFeaCls.GetFeature(pOID);
                                IPoint pPoint = ModCommonFunction.GetPntOfFeature(pFeature);
                                double pMapx = 0;// pPoint.X;
                                double pMapy = 0;// pPoint.Y;
                                if(pPoint!=null)
                                {
                                    pMapx = pPoint.X;
                                    pMapy = pPoint.Y;
                                }

                                //�������������
                                List<object> ErrorLst = new List<object>();
                                ErrorLst.Add("�������");
                                ErrorLst.Add("����ͼ����");
                                ErrorLst.Add(path);
                                ErrorLst.Add(enumErrorType.���������ͼ������Ӧ��ϵ����ȷ.GetHashCode());
                                ErrorLst.Add("�������"+ppGISID+"��ͼ��"+pFeaClsNameStr+"����Ӧ��");
                                ErrorLst.Add(pMapx);    //...
                                ErrorLst.Add(pMapy);    //...
                                ErrorLst.Add(pFeaClsNameStr);
                                ErrorLst.Add(pOID);
                                ErrorLst.Add("");
                                ErrorLst.Add(-1);
                                ErrorLst.Add(false);
                                ErrorLst.Add(System.DateTime.Now.ToString());

                                //���ݴ�����־
                                IDataErrInfo dataErrInfo = new DataErrInfo(ErrorLst);
                                DataErrTreatEvent dataErrTreatEvent = new DataErrTreatEvent(dataErrInfo);
                                DataErrTreat(Hook.DataCheckParaSet as object, dataErrTreatEvent);
                            }
                            #endregion
                        }
                        #endregion
                    }
                    if (pResult == 0)
                    {
                        //������벻����
                        #region ���������
                        //�����÷�������Ӧ��Ҫ��OID����
                        for (int m = 0; m < pGISIDItem.Value.Count; m++)
                        {
                            int pOID = pGISIDItem.Value[m];

                            double pMapx = 0.0;
                            double pMapy = 0.0;

                            //�������������
                            List<object> ErrorLst = new List<object>();
                            ErrorLst.Add("�������");
                            ErrorLst.Add("����ͼ����");
                            ErrorLst.Add(path);
                            ErrorLst.Add(enumErrorType.������벻����.GetHashCode());
                            ErrorLst.Add("�������"+ppGISID+"�����ڣ�");
                            ErrorLst.Add(pMapx);    //...
                            ErrorLst.Add(pMapy);    //...
                            ErrorLst.Add(pFeaClsNameStr);
                            ErrorLst.Add(pOID);
                            ErrorLst.Add("");
                            ErrorLst.Add(-1);
                            ErrorLst.Add(false);
                            ErrorLst.Add(System.DateTime.Now.ToString());

                            //���ݴ�����־
                            IDataErrInfo dataErrInfo = new DataErrInfo(ErrorLst);
                            DataErrTreatEvent dataErrTreatEvent = new DataErrTreatEvent(dataErrInfo);
                            DataErrTreat(Hook.DataCheckParaSet as object, dataErrTreatEvent);
                        }
                        #endregion
                    }
                }
                #endregion

                //��������1
                pValue++;
                eInfo.Value = pValue;
                //GeoDataChecker._CheckForm.Invoke(new GeoDataChecker.GEODataCheckerProgressShow(GeoDataChecker.GeoDataChecker_ProgressShow), new object[] { (object)GeoDataChecker._ProgressBarInner, eInfo });
                GeoDataChecker.GeoDataChecker_ProgressShow((object)GeoDataChecker._ProgressBarInner, eInfo);
                //GeoDataChecker._CheckForm.Invoke(new GeoDataChecker.ChangeProgressBar(GeoDataChecker.changeProgress), new object[] {pValue});
            }
            #endregion
        }

        /// <summary>
        /// ������---����������ģ��������м��
        /// </summary>
        /// <param name="sqlStr">��ѯ����</param>
        /// <param name="pGISID">�������</param>
        /// <returns></returns>
        private int CodeStandardizeCheck(string sqlStr)
        {
            Exception Error = null;
            SysCommon.DataBase.SysTable pSysTable = new SysCommon.DataBase.SysTable();
            pSysTable.SetDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" +TopologyCheckClass.GeoDataCheckParaPath, SysCommon.enumDBConType.OLEDB, SysCommon.enumDBType.ACCESS, out Error);
            if (Error != null)
            {
                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("����", "���ӿ������·��Ϊ��" + TopologyCheckClass.GeoDataCheckParaPath);
                return -1;
            }
            //pSysTable.DbConn = (Hook.DataCheckParaSet as IArcgisDataCheckParaSet).DbConnPara;
            //pSysTable.DBType = SysCommon.enumDBType.ACCESS;
            //pSysTable.DBConType = SysCommon.enumDBConType.OLEDB;

            DataTable dt = pSysTable.GetSQLTable(sqlStr, out Error);
            if (Error != null)
            {
                //SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("����", "�򿪱�����");
                pSysTable.CloseDbConnection();
                return -1;
            }
            if (dt.Rows.Count == 0)
            {
                //��ģ������û���ҵ��÷������
                pSysTable.CloseDbConnection();
                return 0;
            }
            else
            {
                pSysTable.CloseDbConnection();
                return 1;
            }
        }
        #endregion

        public void DataCheckRealize_DataErrTreat(object sender, DataErrTreatEvent e)
        {
        }
        /// <summary>
        /// ��ʾ������
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void DataCheck_ProgressShow(object sender, ProgressChangeEvent e)
        { }
    }
}

