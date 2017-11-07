using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Data;
using System.Drawing;
using System.Data.Common;

using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;

namespace GeoDataChecker
{
    public class ClsCodeCheck : GOGISErrorChecker
    {
        private Plugin.Application.IAppGISRef _AppHk;
        string m_TempletePath = "";                         //���ݿ��׼ģ���ļ�·��
        List<IFeatureLayer> m_LstFeaLayer = null;           //Դ��������ͼ��

        private DbConnection _ErrDbCon = null;                       //������־������
        public DbConnection ErrDbCon
        {
            get
            {
                return _ErrDbCon;
            }
            set
            {
                _ErrDbCon = value;
            }
        }
        private string _ErrTableName = "";                            //������־����
        public string ErrTableName
        {
            get
            {
                return _ErrTableName;
            }
            set
            {
                _ErrTableName = value;
            }
        }

        public event DataErrTreatHandle DataErrTreat;

        private DataTable _ResultTable = new DataTable();

        public ClsCodeCheck(Plugin.Application.IAppGISRef pAppHk, string path, List<IFeatureLayer> LstFeaLayer)
        {
            CreateTable();
            m_TempletePath = path;
            _AppHk = pAppHk;
            m_LstFeaLayer = LstFeaLayer;

            if (_AppHk.DataCheckGrid.RowCount > 0)
            {
                _AppHk.DataCheckGrid.DataSource = null;
            }
            _AppHk.DataCheckGrid.DataSource = _ResultTable;
            _AppHk.DataCheckGrid.SelectionMode=DataGridViewSelectionMode.FullRowSelect;
            DataErrTreat += new DataErrTreatHandle(GeoDataChecker_DataErrTreat);
        }

         /// <summary>
        /// ���������---���������
        /// </summary>
        public void ExcuteCheck()
        {
            Exception eError = null;

            string pClassifyName = GetClassifyName1(out eError);
            if (eError != null || pClassifyName == "")
            {
                return;
            }

            //�����洢MapControl�ϵ�ͼ��ķ������������Ϣ
            Dictionary<string, Dictionary<string, List<long>>> DicFea = new Dictionary<string, Dictionary<string, List<long>>>();
            foreach (IFeatureLayer pFeaLayer in m_LstFeaLayer)
            {
                
                IFeatureClass pFeatureClass = pFeaLayer.FeatureClass;
                IDataset pDT = pFeatureClass as IDataset;
                if (pDT == null) return;
                string pFeaClsName = pDT.Name.Trim();
              

                #region ���ȼ��Mapcontrol�ϵ�Ҫ�����Ƿ���з����������ֶ�
                int index = -1;   //�����������
                index = pFeatureClass.Fields.FindField(pClassifyName);
                if (index == -1)
                {
                    SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "�����ڷ�������ֶΣ�");
                    return;
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

                    if (!DicFea.ContainsKey(pFeaClsName))
                    {
                        //��������GISID�Ͷ�Ӧ��OID
                        Dictionary<string, List<long>> DicCode = new Dictionary<string, List<long>>();
                        List<long> LstOID = new List<long>();
                        LstOID.Add(pOID);
                        DicCode.Add(pGISID, LstOID);
                        DicFea.Add(pFeaClsName, DicCode);
                    }
                    else
                    {
                        if (!DicFea[pFeaClsName].ContainsKey(pGISID))
                        {
                            List<long> LstOID = new List<long>();
                            LstOID.Add(pOID);
                            DicFea[pFeaClsName].Add(pGISID, LstOID);
                        }
                        else
                        {
                            DicFea[pFeaClsName][pGISID].Add(pOID);
                        }
                    }
                    pFeature = pFeaCursor.NextFeature();
                }

                //�ͷ�cursor
                System.Runtime.InteropServices.Marshal.ReleaseComObject(pFeaCursor);

                #endregion

            }
            #region ���з��������
            //ErrorEventArgs ErrEvent = new ErrorEventArgs();

            if (_AppHk.DataTree == null) return;
            _AppHk.DataTree.Nodes.Clear();
            //����������ͼ
            IntialTree(_AppHk.DataTree);
            //�������ڵ���ɫ
            setNodeColor(_AppHk.DataTree);
            _AppHk.DataTree.Tag = false;

            //����ͼ��
            foreach (KeyValuePair<string, Dictionary<string, List<long>>> FeaCls in DicFea)
            {
                string pFeaClsNameStr = FeaCls.Key;
                if(pFeaClsNameStr.Contains("."))
                {
                    pFeaClsNameStr = pFeaClsNameStr.Substring(pFeaClsNameStr.IndexOf('.') + 1);
                }
                //������ͼ�ڵ�(��ͼ������Ϊ�����)
                DevComponents.AdvTree.Node pNode = new DevComponents.AdvTree.Node();
                pNode = (DevComponents.AdvTree.Node)CreateAdvTreeNode(_AppHk.DataTree.Nodes, pFeaClsNameStr, pFeaClsNameStr, _AppHk.DataTree.ImageList.Images[6], true);//ͼ�����ڵ�
               
                ////�����ͼ�ڵ���
                //CreateAdvTreeCell(pNode, "", null);
                ////���ó�ʼ������
                int tempValue = 0;
                //ChangeProgressBar((_AppHk as Plugin.Application.IAppFormRef).ProgressBar, 0, FeaCls.Value.Count, tempValue);
                
 
                //�����������ֵ�����м��
                foreach (KeyValuePair<string, List<long>> pGISIDItem in FeaCls.Value)
                {
                    //��������ֵ
                    string ppGISID = pGISIDItem.Key;
                    string sqlStr = "select * from GeoCheckCode where ������� ='" + ppGISID + "'";
                    //ִ�м��
                    int pResult = CodeStandardizeCheck(sqlStr);
                    #region �Լ���������ж�
                    if (pResult == -1)
                    {
                        return;
                    }
                    if (pResult == 1)
                    {
                        //�÷��������ȷ
                        tempValue += 1;//��������ֵ��1
                        continue;
                    }
                    if (pResult == 0)
                    {
                        //�÷�����벻��ȷ
                        #region ���������
                        //�ֶ�����Ϊ�գ����׼��һ�£��������������

                        //�����÷�������Ӧ��Ҫ��OID����
                        long[] OIDs = new long[pGISIDItem.Value.Count];
                        for (int m = 0; m < pGISIDItem.Value.Count; m++)
                        {
                            OIDs[m] = pGISIDItem.Value[m];

                            //ErrEvent.FeatureClassName = pFeaClsNameStr;
                            //ErrEvent.OIDs = OIDs;
                            //ErrEvent.ErrorName = "������벻����";
                            //ErrEvent.ErrDescription = "��ģ������Ҳ���������룺" + ppGISID;
                            //ErrEvent.CheckTime = System.DateTime.Now.ToString();

                            //double pMapx = 0.0;
                            //double pMapy = 0.0;
                            //IPoint pPoint = new PointClass();
                            //if (pFeaCls.ShapeType != esriGeometryType.esriGeometryPoint)
                            //{
                            //    pPoint = TopologyCheckClass.GetPointofFeature(pFeature);
                            //}
                            //else
                            //{
                            //    pPoint = pFeature.Shape as IPoint;
                            //}
                            //pMapx = pPoint.X;
                            //pMapy = pPoint.Y;

                            List<object> ErrorLst = new List<object>();
                            ErrorLst.Add("Ҫ�����Լ��");//����������
                            ErrorLst.Add("���������");//��������
                            ErrorLst.Add("");  //�����ļ���
                            ErrorLst.Add(enumErrorType.������벻����.GetHashCode());//����id;
                            ErrorLst.Add("ģ����ͼ��" + pFeaClsNameStr + "�в����ڷ������" + ppGISID);//��������
                            ErrorLst.Add(0);    //...
                            ErrorLst.Add(0);           //...
                            ErrorLst.Add(pFeaClsNameStr);
                            ErrorLst.Add(OIDs[m]);
                            ErrorLst.Add("");
                            ErrorLst.Add(-1);
                            ErrorLst.Add(false);
                            ErrorLst.Add(System.DateTime.Now.ToString());

                            //���ݴ�����־
                            IDataErrInfo dataErrInfo = new DataErrInfo(ErrorLst);
                            DataErrTreatEvent dataErrTreatEvent = new DataErrTreatEvent(dataErrInfo);
                            DataErrTreat(_AppHk.DataCheckGrid as object, dataErrTreatEvent);
                        }
                        #endregion
                    }
                    #endregion
                    
                    //�����¼�
                     //this.OnErrorFind(_AppHk, ErrEvent);

                    tempValue += 1;//��������ֵ��1
                    //�����������¼�
                    this.OnProgressStep(_AppHk, tempValue, FeaCls.Value.Count);

                    //ChangeProgressBar((_AppHk as Plugin.Application.IAppFormRef).ProgressBar, -1, -1, tempValue);
                }
                //�ı���ͼ����״̬
                ChangeTreeSelectNode(pNode, "���"+FeaCls.Value.Count + "���������ļ��", false);
          
            }
           
            
            #endregion

            SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "������!");
        }


        /// <summary>
        /// ����ͼ����---���������
        /// </summary>
        public void ExcuteLayerCheck()
        {
            Exception eError = null;
            //�����洢MapControl�ϵ�ͼ��ķ������������Ϣ
            Dictionary<IFeatureClass, Dictionary<string, List<long>>> DicFea = new Dictionary<IFeatureClass, Dictionary<string, List<long>>>();
            ////�����洢ͼ�����������ֵ��
            //Dictionary<string, string> DicFeaType = new Dictionary<string, string>();


            string pClassifyName = GetClassifyName1(out eError);
            if (eError != null || pClassifyName == "")
            {
                return;
            }

            foreach (IFeatureLayer pFeaLayer in m_LstFeaLayer)
            {
                IFeatureClass pFeatureClass = pFeaLayer.FeatureClass;     
               

                #region ���ȼ��Mapcontrol�ϵ�Ҫ�����Ƿ���з����������ֶ�
                int index = -1;   //�����������
                index = pFeatureClass.Fields.FindField(pClassifyName);
                if (index == -1)
                {
                    SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "�����ڷ�������ֶΣ�");
                    return;
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
                        Dictionary<string, List<long>> DicCode = new Dictionary<string, List<long>>();
                        List<long> LstOID = new List<long>();
                        LstOID.Add(pOID);
                        DicCode.Add(pGISID, LstOID);
                        DicFea.Add(pFeatureClass, DicCode);
                    }
                    else
                    {
                        if (!DicFea[pFeatureClass].ContainsKey(pGISID))
                        {
                            List<long> LstOID = new List<long>();
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

                //�ͷ�cursor
                System.Runtime.InteropServices.Marshal.ReleaseComObject(pFeaCursor);
                #endregion

            }
            #region ���з��������
            //ErrorEventArgs ErrEvent = new ErrorEventArgs();
            //�������������������
            //DataTable pResultTable = CreateTable();

            if (_AppHk.DataTree == null) return;
            _AppHk.DataTree.Nodes.Clear();
            //����������ͼ
            IntialTree(_AppHk.DataTree);
            //�������ڵ���ɫ
            setNodeColor(_AppHk.DataTree);
            _AppHk.DataTree.Tag = false;


            //����ͼ��
            foreach (KeyValuePair<IFeatureClass, Dictionary<string, List<long>>> FeaCls in DicFea)
            {
                IFeatureClass pFeaCls = FeaCls.Key;
                IDataset pDT = pFeaCls as IDataset;
                if (pDT == null) return;
                //string pFeaClsType="";                                        //Ҫ���������ͣ��㡢�ߡ��桢ע��

                string pFeaClsNameStr = pDT.Name.Trim();                 //Ҫ��������
                if(pFeaClsNameStr.Contains("."))
                {
                    pFeaClsNameStr = pFeaClsNameStr.Substring(pFeaClsNameStr.IndexOf('.') + 1);
                }

                //������ͼ�ڵ�(��ͼ������Ϊ�����)
                DevComponents.AdvTree.Node pNode = new DevComponents.AdvTree.Node();
                pNode = (DevComponents.AdvTree.Node)CreateAdvTreeNode(_AppHk.DataTree.Nodes, pFeaClsNameStr, pFeaClsNameStr, _AppHk.DataTree.ImageList.Images[6], true);//ͼ�����ڵ�
                //��ʾ������
                ShowProgressBar(true);
                ////�����ͼ�ڵ���
                //CreateAdvTreeCell(pNode, "", null);
                //���ó�ʼ������
                int tempValue = 0;
                #region ���ͼ���������Ƿ���ȷ
                
                #region �Լ���������ж�

                ChangeProgressBar((_AppHk as Plugin.Application.IAppFormRef).ProgressBar, 0, FeaCls.Value.Count, tempValue);

                #region �����������ͼ��Ķ�Ӧ��ϵ
                //�����������ֵ�����м��
                foreach (KeyValuePair<string, List<long>> pGISIDItem in FeaCls.Value)
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
                            //��ģ�����ܹ��ҵ��÷�������Ӧ��ͼ����,˵����������Ӧ��ͼ������ȷ
                            tempValue += 1;//��������ֵ��1
                            ChangeProgressBar((_AppHk as Plugin.Application.IAppFormRef).ProgressBar, -1, -1, tempValue);
                            continue;

                        }
                        if (pResult2 == 0)
                        {
                            //��ģ�����Ҳ����÷�������Ӧ��ͼ����,˵����������Ӧ��ͼ��������ȷ

                            #region ���������
                            //�����÷�������Ӧ��Ҫ��OID����
                            long[] OIDs = new long[pGISIDItem.Value.Count];
                            for (int m = 0; m < pGISIDItem.Value.Count; m++)
                            {
                                OIDs[m] = pGISIDItem.Value[m];

                                List<object> ErrorLst = new List<object>();
                                ErrorLst.Add("Ҫ�����Լ��");//����������
                                ErrorLst.Add("����ͼ����");//��������
                                ErrorLst.Add("");  //�����ļ���
                                ErrorLst.Add(enumErrorType.���������ͼ������Ӧ��ϵ����ȷ.GetHashCode());//����id;
                                ErrorLst.Add("ģ����ͼ��" + pFeaClsNameStr + "����������" + ppGISID+"�Ķ�Ӧ��ϵ����ȷ��");//��������
                                ErrorLst.Add(0);    //...
                                ErrorLst.Add(0);           //...
                                ErrorLst.Add(pFeaClsNameStr);
                                ErrorLst.Add(OIDs[m]);
                                ErrorLst.Add("");
                                ErrorLst.Add(-1);
                                ErrorLst.Add(false);
                                ErrorLst.Add(System.DateTime.Now.ToString());

                                //���ݴ�����־
                                IDataErrInfo dataErrInfo = new DataErrInfo(ErrorLst);
                                DataErrTreatEvent dataErrTreatEvent = new DataErrTreatEvent(dataErrInfo);
                                DataErrTreat(_AppHk.DataCheckGrid as object, dataErrTreatEvent);
                            }
                            //ErrEvent.FeatureClassName = pFeaClsNameStr;
                            //ErrEvent.OIDs = OIDs;
                            //ErrEvent.ErrorName = "���������ͼ������Ӧ����ȷ";
                            //ErrEvent.ErrDescription = "��ģ���з������" + ppGISID + "��Ӧ��ͼ��������" + pFeaClsNameStr;
                            //ErrEvent.CheckTime = System.DateTime.Now.ToString();
                            #endregion
                        }
                        #endregion
                    }
                    if (pResult == 0)
                    {
                        //������벻����
                        #region ���������
                        //�����÷�������Ӧ��Ҫ��OID����
                        long[] OIDs = new long[pGISIDItem.Value.Count];
                        for (int m = 0; m < pGISIDItem.Value.Count; m++)
                        {
                            OIDs[m] = pGISIDItem.Value[m];

                            List<object> ErrorLst = new List<object>();
                            ErrorLst.Add("Ҫ�����Լ��");//����������
                            ErrorLst.Add("����ͼ����");//��������
                            ErrorLst.Add("");  //�����ļ���
                            ErrorLst.Add(enumErrorType.������벻����.GetHashCode());//����id;
                            ErrorLst.Add("ģ����ͼ��" + pFeaClsNameStr + "�в����ڷ������" + ppGISID);//��������
                            ErrorLst.Add(0);    //...
                            ErrorLst.Add(0);           //...
                            ErrorLst.Add(pFeaClsNameStr);
                            ErrorLst.Add(OIDs[m]);
                            ErrorLst.Add("");
                            ErrorLst.Add(-1);
                            ErrorLst.Add(false);
                            ErrorLst.Add(System.DateTime.Now.ToString());

                            //���ݴ�����־
                            IDataErrInfo dataErrInfo = new DataErrInfo(ErrorLst);
                            DataErrTreatEvent dataErrTreatEvent = new DataErrTreatEvent(dataErrInfo);
                            DataErrTreat(_AppHk.DataCheckGrid as object, dataErrTreatEvent);
                        }
                        //ErrEvent.FeatureClassName = pFeaClsNameStr;
                        //ErrEvent.OIDs = OIDs;
                        //ErrEvent.ErrorName = "������벻����";
                        //ErrEvent.ErrDescription = "��ģ������Ҳ���������룺" + ppGISID;
                        //ErrEvent.CheckTime = System.DateTime.Now.ToString();
                        #endregion
                    }

                    //�����¼�
                    //this.OnErrorFind(_AppHk, ErrEvent);

                    tempValue += 1;//��������ֵ��1
                    ChangeProgressBar((_AppHk as Plugin.Application.IAppFormRef).ProgressBar, -1, -1, tempValue);
                }
                #endregion

                #endregion

                #endregion
                //�ı���ͼ����״̬
                ChangeTreeSelectNode(pNode, "���" + FeaCls.Value.Count + "������ͼ��ļ��", false);
            }
            #endregion

            SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "������!");
            //���ؽ�����
            ShowProgressBar(false);
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
            pSysTable.SetDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + m_TempletePath, SysCommon.enumDBConType.OLEDB, SysCommon.enumDBType.ACCESS, out Error);
            if (Error != null)
            {
                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("����", "���ӿ������·��Ϊ��" + m_TempletePath);
                return -1;
            }
            DataTable dt = pSysTable.GetSQLTable(sqlStr, out Error);
            if (Error != null)
            {
                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("����", "�򿪱�����");
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
            pSysTable.SetDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" +TopologyCheckClass.GeoDataCheckParaPath, SysCommon.enumDBConType.OLEDB, SysCommon.enumDBType.ACCESS, out eError);
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

        private void CreateTable()
        {
            _ResultTable.Columns.Add("��鹦����", typeof(string));
            _ResultTable.Columns.Add("��������", typeof(string));
            _ResultTable.Columns.Add("��������", typeof(string));
            _ResultTable.Columns.Add("����ͼ��1", typeof(string));
            _ResultTable.Columns.Add("Ҫ��OID1", typeof(string));
            _ResultTable.Columns.Add("����ͼ��2", typeof(string));
            _ResultTable.Columns.Add("Ҫ��OID2", typeof(string));
            _ResultTable.Columns.Add("���ʱ��", typeof(string));
            _ResultTable.Columns.Add("��λ��X", typeof(string));
            _ResultTable.Columns.Add("��λ��Y", typeof(string));
            _ResultTable.Columns.Add("�����ļ���", typeof(string));
        }

        //��������
        public void GeoDataChecker_DataErrTreat(object sender, DataErrTreatEvent e)
        {
            //�û������ϱ��ֳ�������Ϣ
            DevComponents.DotNetBar.Controls.DataGridViewX pDataGrid = sender as DevComponents.DotNetBar.Controls.DataGridViewX;
            if (_ResultTable == null) return;
            DataRow newRow = _ResultTable.NewRow();
            newRow["��鹦����"] = e.ErrInfo.FunctionName;
            newRow["��������"] = GeoDataChecker.GetErrorTypeString(Enum.Parse(typeof(enumErrorType), e.ErrInfo.ErrID.ToString()).ToString());
            newRow["��������"] = e.ErrInfo.ErrDescription;
            newRow["����ͼ��1"] = e.ErrInfo.OriginClassName;
            newRow["Ҫ��OID1"] = e.ErrInfo.OriginFeatOID;
            newRow["����ͼ��2"] = e.ErrInfo.DestinationClassName;
            newRow["Ҫ��OID2"] = e.ErrInfo.DestinationFeatOID;
            newRow["���ʱ��"] = e.ErrInfo.OperatorTime;
            newRow["��λ��X"] = e.ErrInfo.MapX;
            newRow["��λ��Y"] = e.ErrInfo.MapY;
            newRow["�����ļ���"] = e.ErrInfo.DataFileName;
            _ResultTable.Rows.Add(newRow);

            pDataGrid.Update();

            //��������excle

            InsertRowToExcel(e);
        }

        //�����ݼ��������Excel�� 
        private void InsertRowToExcel(DataErrTreatEvent e)
        {
            if (_ErrDbCon != null && _ErrTableName != "")
            {
                SysCommon.DataBase.SysTable sysTable = new SysCommon.DataBase.SysTable();
                sysTable.DbConn = _ErrDbCon;
                sysTable.DBConType = SysCommon.enumDBConType.OLEDB;
                sysTable.DBType = SysCommon.enumDBType.ACCESS;
                string sql = "insert into " + _ErrTableName +
                    "(�����ļ�·��,��鹦����,��������,��������,����ͼ��1,����OID1,����ͼ��2,����OID2,��λ��X,��λ��Y,���ʱ��) values(" +
                    "'" + e.ErrInfo.DataFileName + "','" + e.ErrInfo.FunctionName + "','" + GeoDataChecker.GetErrorTypeString(Enum.Parse(typeof(enumErrorType), e.ErrInfo.ErrID.ToString()).ToString()) + "','" + e.ErrInfo.ErrDescription + "','" + e.ErrInfo.OriginClassName + "','" + e.ErrInfo.OriginFeatOID.ToString() + "','" +
                    e.ErrInfo.DestinationClassName + "','" + e.ErrInfo.DestinationFeatOID.ToString() + "'," + e.ErrInfo.MapX + "," + e.ErrInfo.MapY + ",'" + e.ErrInfo.OperatorTime + "')";

                Exception errEx = null;
                sysTable.UpdateTable(sql, out errEx);
                if (errEx != null)
                {
                    SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "д��Excel�ļ�����");
                    return;
                }
            }
        }


        #region ������ͼ��غ���
        //����������ͼ
        private void IntialTree(DevComponents.AdvTree.AdvTree aTree)
        {
            DevComponents.AdvTree.ColumnHeader aColumnHeader;
            aColumnHeader = new DevComponents.AdvTree.ColumnHeader();
            aColumnHeader.Name = "FCName";
            aColumnHeader.Text = "ͼ����";
            aColumnHeader.Width.Relative = 50;
            aTree.Columns.Add(aColumnHeader);

            aColumnHeader = new DevComponents.AdvTree.ColumnHeader();
            aColumnHeader.Name = "NodeRes";
            aColumnHeader.Text = "���";
            aColumnHeader.Width.Relative = 45;
            aTree.Columns.Add(aColumnHeader);
        }
        //����ѡ�����ڵ���ɫ
        private void setNodeColor(DevComponents.AdvTree.AdvTree aTree)
        {
            DevComponents.DotNetBar.ElementStyle elementStyle = new DevComponents.DotNetBar.ElementStyle();
            elementStyle.BackColor = Color.FromArgb(255, 244, 213);
            elementStyle.BackColor2 = Color.FromArgb(255, 216, 105);
            elementStyle.BackColorGradientAngle = 90;
            elementStyle.Border = DevComponents.DotNetBar.eStyleBorderType.Solid;
            elementStyle.BorderBottom = DevComponents.DotNetBar.eStyleBorderType.Solid;
            elementStyle.BorderBottomWidth = 1;
            elementStyle.BorderColor = Color.DarkGray;
            elementStyle.BorderLeft = DevComponents.DotNetBar.eStyleBorderType.Solid;
            elementStyle.BorderLeftWidth = 1;
            elementStyle.BorderRight = DevComponents.DotNetBar.eStyleBorderType.Solid;
            elementStyle.BorderRightWidth = 1;
            elementStyle.BorderTop = DevComponents.DotNetBar.eStyleBorderType.Solid;
            elementStyle.BorderTopWidth = 1;
            elementStyle.BorderWidth = 1;
            elementStyle.CornerDiameter = 4;
            elementStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            aTree.NodeStyleSelected = elementStyle;
            aTree.DragDropEnabled = false;
        }
        //������ͼ�ڵ�
        private DevComponents.AdvTree.Node CreateAdvTreeNode(DevComponents.AdvTree.NodeCollection nodeCol, string strText, string strName, Image pImage, bool bExpand)
        {

            DevComponents.AdvTree.Node node = new DevComponents.AdvTree.Node();
            node.Text = strText;
            node.Image = pImage;
            if (strName != null)
            {
                node.Name = strName;
            }

            if (bExpand == true)
            {
                node.Expand();
            }
            //�����ͼ�нڵ�
            DevComponents.AdvTree.Cell aCell = new DevComponents.AdvTree.Cell();
            aCell.Images.Image = null;
            node.Cells.Add(aCell);
            nodeCol.Add(node);
            return node;
        }

        //�����ͼ�ڵ���
        private DevComponents.AdvTree.Cell CreateAdvTreeCell(DevComponents.AdvTree.Node aNode, string strText, Image pImage)
        {
            DevComponents.AdvTree.Cell aCell = new DevComponents.AdvTree.Cell(strText);
            aCell.Images.Image = pImage;
            aNode.Cells.Add(aCell);

            return aCell;
        }

        //Ϊ���ݴ�����ͼ�ڵ���Ӵ�����״̬
        private void ChangeTreeSelectNode(DevComponents.AdvTree.Node aNode,string strRes, bool bClear)
        {
            if (aNode == null)
            {
                _AppHk.DataTree.SelectedNode = null;
                _AppHk.DataTree.Refresh();
                return;
            }

            _AppHk.DataTree.SelectedNode = aNode;
            if (bClear)
            {
                _AppHk.DataTree.SelectedNode.Nodes.Clear();
            }
            _AppHk.DataTree.SelectedNode.Cells[1].Text = strRes;
            _AppHk.DataTree.Refresh();
        }
        #endregion

        #region ��������ʾ
        //���ƽ�������ʾ
        private void ShowProgressBar(bool bVisible)
        {
            if (bVisible == true)
            {
               (_AppHk as Plugin.Application.IAppFormRef).ProgressBar.Visible = true;
            }
            else
            {
                (_AppHk as Plugin.Application.IAppFormRef).ProgressBar.Visible = false;
            }
        }
        //�޸Ľ�����
        private void ChangeProgressBar(DevComponents.DotNetBar.ProgressBarItem pProgressBar, int min, int max, int value)
        {
            if (min != -1)
            {
                pProgressBar.Minimum = min;
            }
            if (max != -1)
            {
                pProgressBar.Maximum = max;
            }
            pProgressBar.Value = value;
            pProgressBar.Refresh();
        }


        //�ı�״̬����ʾ����
        private void ShowStatusTips(string strText)
        {
            (_AppHk as Plugin.Application.IAppFormRef).OperatorTips = strText;
        }
        #endregion
    }
}
