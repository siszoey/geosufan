using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.OracleClient;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;

namespace GeoEdit
{
    class clsUpdataEnvironmentOper
    {
        /*
         * guozheng 2010-11-4 added 
         * ���»��������࣬��������Զ����־��GO_DATABASE_UPDATELOG������
         * ���ݿ�汾��GO_DATABASE_VERSION������
         * ���̿⣨XXX_goh���Ĳ���
         */
        private string m_sUpDataLOGTable = "GO_DATABASE_UPDATELOG";///////Զ�̸�����־��
        private string m_sDBVersionTable = "GO_DATABASE_VERSION";/////////���ݿ�汾��

        private IWorkspace m_HisWS;
        /// <summary>
        /// ���̿�WorkSpace
        /// </summary>
        public IWorkspace HisWs
        {
            get { return this.m_HisWS;}
            set { this.m_HisWS = value; }
        }
        #region ���캯��
        public clsUpdataEnvironmentOper()
        {

        }
        public clsUpdataEnvironmentOper(IWorkspace WS)
        {
            this.HisWs = WS;
        }
        #endregion
        /// <summary>
        /// ��¼һ��Ҫ�صı༭��Ϣ
        /// </summary>
        /// <param name="in_Row">�༭Ҫ�ص�IRow</param>
        /// <param name="in_iState">�༭״̬��1��������2���޸ģ�3��ɾ��</param>
        /// <param name="in_DateTime">�༭��ʱ��</param>
        /// <param name="in_iVersion">�汾</param>
        /// <param name="in_sLayerName">Ҫ�����ڵ�ͼ����</param>
        /// <param name="ex"></param>
        public void RecordLOG(IRow in_Row, int in_iState, DateTime in_DateTime,int in_iVersion,string in_sLayerName, out Exception ex)
        {
            ex = null;
            if (in_Row == null) { ex = new Exception("����Ҫ��Ϊ��"); return; }
            if (in_DateTime == null) { ex = new Exception("����ı༭ʱ��Ϊ��"); return; }
            IFeature getFea = in_Row as IFeature;
            if (getFea == null) { ex = new Exception("��ȡҪ��ʧ��"); return; }
            ///////////////��ȡ��Ҫ��Ϣ/////////////////
            //////ȥ��sdeͼ������û���
            if (in_sLayerName.Contains("."))
            {
                in_sLayerName = in_sLayerName.Substring(in_sLayerName.LastIndexOf('.') + 1);
            }
            int iOID = -1;/////////////////////////////Ҫ��OID
            string sLayerName = string.Empty;//////////Ҫ��ͼ����
            int iVersion = -1;/////////////////////////�汾��Ϣ 
            if (getFea.HasOID) iOID = getFea.OID;
            sLayerName = in_sLayerName;   
            iVersion = in_iVersion;/////////////��ȡ�汾
            if (ex != null) return;
            //////////////////д����־////////////////////////////
            try
            {
                ITransactions LOGTran = this.m_HisWS as ITransactions;
                LOGTran.StartTransaction();
                ////////д�������־��///////
                WriteLog(iOID, sLayerName, iVersion, in_DateTime, in_iState, getFea.Shape.Envelope, out ex);
                if (ex != null) { LOGTran.AbortTransaction(); return; }
                ////////д�����ݿ�汾��/////
                //WriteDBVersion(iVersion, in_DateTime, this.m_Con, out ex);
                //if (ex != null) { myTrans.Rollback(); }
                ///////�޸Ĺ��̿�////////////
                WriteHisDB(sLayerName, this.m_HisWS, getFea, in_DateTime, in_iState, iVersion, out ex);
                if (ex != null) { LOGTran.AbortTransaction(); return; }
                LOGTran.CommitTransaction();
            }
            catch (Exception eError)
            {
                //******************************************
                //guozheng added System Exception log
                if (SysCommon.Log.Module.SysLog == null)
                    SysCommon.Log.Module.SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
                SysCommon.Log.Module.SysLog.Write(eError);
                //******************************************
                ex =  eError;
                return;
            }           
        }
        /// <summary>
        /// ��ȡ��ǰ�汾��Ϣ����ȡ���������ݿ�汾����VERSION�����ֵ��1��Ϊ��ǰ�༭���ɰ汾��
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public int GetVersion(out Exception ex)
        {
            ex = null;
            int iVersion=-1;
            if (this.HisWs == null) { ex = new Exception("���»�������δ��ʼ��"); return -1; }
            try
            {
                ITable getTable = (this.HisWs as IFeatureWorkspace).OpenTable(this.m_sDBVersionTable);
                if (getTable.RowCount(null) == 0) return 1;
                else
                {
                    int index = getTable.FindField("VERSION");
                    if (index < 0) { ex = new Exception("���ݿ�汾����δ���ҵ�VERSION�ֶ�"); return -1; }
                    ICursor TableCursor = getTable.Search(null, false);
                    //IRow getRow = TableCursor.NextRow();
                    //while (getRow != null)
                    //{
                    //    int getValue =Convert.ToInt32(getRow.get_Value(index));
                    //    if (getValue > iVersion) iVersion = getValue;
                    //    getRow = TableCursor.NextRow();
                    //}
                    IDataStatistics dataStatistics = new DataStatisticsClass();
                    dataStatistics.Field = "VERSION";
                    dataStatistics.Cursor = TableCursor;
                    ESRI.ArcGIS.esriSystem.IStatisticsResults statisticsResults = dataStatistics.Statistics;
                    double getMaxVersion = statisticsResults.Maximum;
                    iVersion= Convert.ToInt32(getMaxVersion);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(TableCursor);
                }
                return iVersion+1;
            }
            catch(Exception eError)
            {
                //******************************************
                //guozheng added System Exception log
                if (SysCommon.Log.Module.SysLog == null)
                    SysCommon.Log.Module.SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
                SysCommon.Log.Module.SysLog.Write(eError);
                //******************************************
                ex = new Exception("��ȡ�����ݿ�汾��Ϣʧ�ܡ�\nԭ��" + eError.Message);
                return -1;
            }
        }
        /// <summary>
        /// д��Զ�̸�����־
        /// </summary>
        /// <param name="in_iOID">����Ҫ��OID</param>
        /// <param name="in_sLayerName">����Ҫ������ͼ����</param>
        /// <param name="in_iVersion">�汾</param>
        /// <param name="in_DateTime">����ʱ��</param>
        /// <param name="in_iState">����״̬��1��������2���޸ģ�3��ɾ��</param>
        /// <param name="in_Envelope">����Ҫ�ص���С�������</param>
        /// <param name="ex"></param>
        private void WriteLog(int in_iOID,string in_sLayerName, int in_iVersion, DateTime in_DateTime, int in_iState,IEnvelope in_Envelope, out Exception ex)
        {
            ex = null;
            //////ȥ��sdeͼ������û���
            if (in_sLayerName.Contains("."))
            {
                in_sLayerName = in_sLayerName.Substring(in_sLayerName.LastIndexOf('.') + 1);
            }
            if (this.HisWs == null) { ex = new Exception("���»�����������Ϣδ��ʼ��"); return; };
            string sql = "INSERT INTO " + this.m_sUpDataLOGTable + "(OID,STATE,LAYERNAME,LASTUPDATE,VERSION,XMIN,XMAX,YMIN,YMAX) values(";
            sql += in_iOID.ToString() + "," + in_iState.ToString() + ",'" + in_sLayerName + "'," + "to_date('" +in_DateTime.ToString("G")+"','yyyy-mm-dd hh24:mi:ss')"+ "," + in_iVersion.ToString() + ",";
            if (in_Envelope != null)
                sql += in_Envelope.XMin.ToString() + "," + in_Envelope.XMax.ToString() + "," + in_Envelope.YMin.ToString() + "," + in_Envelope.YMax.ToString() + ")";
            else
                sql += "NULL,NULL,NULL,NULL)";
            try
            {
                this.HisWs.ExecuteSQL(sql);
            }
            catch(Exception eError)
            {
                //******************************************
                //guozheng added System Exception log
                if (SysCommon.Log.Module.SysLog == null)
                    SysCommon.Log.Module.SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
                SysCommon.Log.Module.SysLog.Write(eError);
                //******************************************
                ex = new Exception("д����־��ʧ�ܡ�\nԭ��" + eError.Message);
                return;
            }
        }
        /// <summary>
        ///  д�����ݿ�汾��
        /// </summary>
        /// <param name="in_iVersion">Ҫд��İ汾��</param>
        /// <param name="in_DateTime">�汾����ʱ��</param>
        /// <param name="ex"></param>
        public void WriteDBVersion(int in_iVersion,DateTime in_DateTime, out Exception ex)
        {
            ex = null;
            if (null == this.HisWs) { ex = new Exception("���»�����������Ϣδ��ʼ����"); return; }
            if (null == in_DateTime) { ex = new Exception("����ʱ�䲻��Ϊ��"); return; }
            string sql = "INSERT INTO " + this.m_sDBVersionTable + "(VERSION,USERNAME,VERSIONTIME,DES) values(";
            sql += in_iVersion.ToString() + "," + "null," + "to_date('" + in_DateTime.ToString("G") + "','yyyy-mm-dd hh24:mi:ss')" + ",null)";
            try
            {
                this.HisWs.ExecuteSQL(sql);
            }
            catch (Exception eError)
            {
                //******************************************
                //guozheng added System Exception log
                if (SysCommon.Log.Module.SysLog == null)
                    SysCommon.Log.Module.SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
                SysCommon.Log.Module.SysLog.Write(eError);
                //******************************************
                ex = new Exception("д�����ݿ�汾��ʧ�ܡ�\nԭ��" + eError.Message);
                return;
            }

        }
        /// <summary>
        /// �޸Ĺ��̿⣨��ʷ�⣬���ɾ����
        /// </summary>
        /// <param name="in_sLayerName">ͼ����(Ҫ�ؼ���)</param>
        /// <param name="in_WS">���̿��Workspace</param>
        /// <param name="in_fea">���µ�Feature</param>
        /// <param name="in_FromDateTime">�༭ʱ��</param>
        /// <param name="in_iState">�༭״̬��1��������2���޸ģ�3��ɾ��</param>
        /// <param name="in_iVersion">�汾</param>
        /// <param name="ex"></param>
        private void WriteHisDB(string in_sLayerName, IWorkspace in_WS, int OID, DateTime in_FromDateTime, int in_iVersion, out Exception ex)
        {
            ex = null;
            //////ȥ��sdeͼ������û���
            if (in_sLayerName.Contains("."))
            {
                in_sLayerName = in_sLayerName.Substring(in_sLayerName.LastIndexOf('.') + 1);
            }
            IFeatureClass getFeaCls = (in_WS as IFeatureWorkspace).OpenFeatureClass(in_sLayerName + "_goh");
            if (getFeaCls == null) { ex = new Exception("�Ҳ�����Ϊ��" + in_sLayerName + "����ͼ��"); return; }
            //ɾ��Ҫ��Ҫ��Ҫ�ؼ����ҵ���Ҫ�ص���Ч�汾��ʹ���ΪʧЧ״̬
            try
            {
                #region ɾ����Ҫ��
                IQueryFilter queryFilter = new QueryFilterClass();
                string sValue = DateTime.MaxValue.ToString("u");
                //queryFilter.WhereClause = "ToDate='" + sValue + "' AND SourceOID=" + in_fea.OID.ToString();
                queryFilter.WhereClause = "ToDate='" + sValue + "' AND SourceOID=" + OID.ToString();
                IFeatureCursor FesCur = getFeaCls.Search(queryFilter, false);
                IFeature CurFea = FesCur.NextFeature();
                while (CurFea != null)
                {
                    //**************************************************************************************
                    //GUOZHENG ADDED Ӧ�����жϣ�����ɾ������ʷ��¼����Ϊ��Ч�İ汾�ȵ�ǰ�༭���ɰ汾��һҪС
                    //               ����Ҫ��ǰһ�汾����ʷ��¼��ΪʧЧ���ٽ���һ��ʧЧ�İ汾״̬Ϊɾ��
                    //               ����鿴��ʷ����ʱ���Ͱ汾��״̬��Ϊɾ�����ⲻ���߼�
                    bool IsLowVersion = false;////////��ǰ��Ч����ʷ��¼�Ƿ�ȵ�ǰ�༭���ɰ汾��һҪС
                    int index = CurFea.Fields.FindField("VERSION");
                    if (index > -1)
                    {
                        // CurFea.set_Value(index, in_iVersion.ToString());
                        int igetVersion = -1;
                        try
                        {
                            igetVersion = Convert.ToInt32(CurFea.get_Value(index).ToString());
                            if (igetVersion < in_iVersion - 1)
                            {
                                IsLowVersion = true;
                            }
                            else
                            {
                                IsLowVersion = false;
                            }
                        }
                        catch (Exception e)
                        {
                            //******************************************
                            //guozheng added System Exception log
                            if (SysCommon.Log.Module.SysLog == null)
                                SysCommon.Log.Module.SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
                            SysCommon.Log.Module.SysLog.Write(e);
                            //******************************************
                        }
                    }
                    //***************************************************************************************
                    if (IsLowVersion)
                    {
                        string sLastVersionTime = Convert.ToDateTime(GetVersionEstablishTime(in_WS, in_iVersion - 1, out ex)).ToString("u"); ;
                        //index = CurFea.Fields.FindField("VERSION");
                        //if (index > -1)
                        //{
                        //    CurFea.set_Value(index, in_iVersion - 1);
                        //}
                        index = CurFea.Fields.FindField("LASTUPDATE");
                        if (index > -1)
                        {
                            CurFea.set_Value(index, sLastVersionTime);
                        }
                        index = CurFea.Fields.FindField("ToDate");
                        if (index > -1)
                        {
                            CurFea.set_Value(index, sLastVersionTime);

                        }
                        ////////////����һ���µİ汾��״̬Ϊɾ��////////
                        #region ����һ���µİ汾��״̬Ϊɾ��/
                        IFeature newFea = getFeaCls.CreateFeature();
                        newFea.Shape = CurFea.Shape;
                        if (SetFieldsValue(ref newFea, ref CurFea))
                        {
                            index = -1;
                            index = newFea.Fields.FindField("FromDate");
                            if (index > -1)
                            {
                                newFea.set_Value(index, sLastVersionTime);
                            }
                            index = newFea.Fields.FindField("ToDate");
                            if (index > -1)
                            {
                                newFea.set_Value(index, in_FromDateTime.ToString("u"));
                            }
                            index = newFea.Fields.FindField("SourceOID");
                            if (index > -1)
                            {
                                newFea.set_Value(index, OID);
                            }
                            index = newFea.Fields.FindField("State");
                            if (index > -1)
                            {
                                newFea.set_Value(index, 3);
                            }
                            index = newFea.Fields.FindField("VERSION");
                            if (index > -1)
                            {
                                newFea.set_Value(index, in_iVersion.ToString());
                            }
                            newFea.Store();
                        }
                        #endregion
                    }
                    else
                    {
                        index = CurFea.Fields.FindField("State");
                        if (index > -1)
                        {
                            CurFea.set_Value(index, 3);
                        }
                        index = CurFea.Fields.FindField("VERSION");
                        if (index > -1)
                        {
                            CurFea.set_Value(index, in_iVersion);
                        }
                        index = CurFea.Fields.FindField("ToDate");
                        if (index > -1)
                        {
                            CurFea.set_Value(index, in_FromDateTime.ToString("u"));

                        }
                    }
                    CurFea.Store();
                    CurFea = FesCur.NextFeature();
                }
                System.Runtime.InteropServices.Marshal.ReleaseComObject(FesCur);
                #endregion
            }
            catch (Exception eError)
            {
                //******************************************
                //guozheng added System Exception log
                if (SysCommon.Log.Module.SysLog == null)
                    SysCommon.Log.Module.SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
                SysCommon.Log.Module.SysLog.Write(eError);
                //******************************************
            }
        }

        /// <summary>
        /// �޸Ĺ��̿⣨��ʷ�⣩
        /// </summary>
        /// <param name="in_sLayerName">ͼ����(Ҫ�ؼ���)</param>
        /// <param name="in_WS">���̿��Workspace</param>
        /// <param name="in_fea">���µ�Feature</param>
        /// <param name="in_FromDateTime">�༭ʱ��</param>
        /// <param name="in_iState">�༭״̬��1��������2���޸ģ�3��ɾ��</param>
        /// <param name="in_iVersion">�汾</param>
        /// <param name="ex"></param>
        private void WriteHisDB(string in_sLayerName, IWorkspace in_WS, IFeature in_fea, DateTime in_FromDateTime, int in_iState, int in_iVersion, out Exception ex)
        {
            ex = null;
            //////ȥ��sdeͼ������û���
            if (in_sLayerName.Contains("."))
            {
                in_sLayerName = in_sLayerName.Substring(in_sLayerName.LastIndexOf('.') + 1);
            }
            IFeatureClass getFeaCls = (in_WS as IFeatureWorkspace).OpenFeatureClass(in_sLayerName + "_goh");
            if (getFeaCls == null) { ex = new Exception("�Ҳ�����Ϊ��" + in_sLayerName + "����ͼ��"); return; }
            try
            {
                if (in_iState == 1)/////����
                {
                    //�����ӵ�Ҫ����Ҫ�ؼ���CreateFeature����Ч����Ϊ��ǰ���ڣ�ʧЧ����Ϊmaxvalue
                    #region �����ӵ�Ҫ��
                    if (in_fea == null) { ex = new Exception("�����ӵ�Ҫ�ز���Ϊ��"); return; };
                    IFeature newFea = getFeaCls.CreateFeature();
                    newFea.Shape = in_fea.Shape;
                    if (SetFieldsValue(ref newFea, ref in_fea))
                    {
                        int index = -1;
                        index = newFea.Fields.FindField("FromDate");
                        if (index > -1)
                        {
                            newFea.set_Value(index, in_FromDateTime.ToString("u"));
                        }
                        index = newFea.Fields.FindField("ToDate");
                        if (index > -1)
                        {
                            newFea.set_Value(index, DateTime.MaxValue.ToString("u"));
                        }
                        index = newFea.Fields.FindField("SourceOID");
                        if (index > -1)
                        {
                            newFea.set_Value(index, in_fea.OID.ToString());
                        }
                        index = newFea.Fields.FindField("State");
                        if (index > -1)
                        {
                            newFea.set_Value(index, in_iState.ToString());
                        }
                        index = newFea.Fields.FindField("VERSION");
                        if (index > -1)
                        {
                            newFea.set_Value(index, in_iVersion.ToString());
                        }
                        newFea.Store();
                    }
                    #endregion
                }
                else if (in_iState == 2)/////�޸�
                {
                    if (in_fea == null) { ex = new Exception("�޸ĺ��Ҫ�ز���Ϊ��"); return; };
                    //�޸�Ҫ��Ҫ��Ҫ�ؼ����ҵ���Ҫ�ص���һ����Ч�汾����������ΪʧЧ״̬���ٽ���һ���µ���Ч�汾
                    #region �޸�Ҫ��
                    //////////ʹԭ�汾ʧЧ/////////////////////
                    IQueryFilter queryFilter = new QueryFilterClass();
                    string sValue = DateTime.MaxValue.ToString("u");
                    queryFilter.WhereClause = "ToDate='" + sValue + "' AND SourceOID=" + in_fea.OID.ToString();
                    IFeatureCursor FesCur = getFeaCls.Search(queryFilter, false);
                    IFeature CurFea = FesCur.NextFeature();
                    while (CurFea != null)
                    {
                        int index = -1;
                        index = CurFea.Fields.FindField("ToDate");
                        if (index > -1)
                        {
                            CurFea.set_Value(index, in_FromDateTime.ToString("u"));
                            CurFea.Store();
                        }
                        CurFea = FesCur.NextFeature();
                    }
                    ///////////�����µİ汾////////
                    IFeature newFea = getFeaCls.CreateFeature();
                    newFea.Shape = in_fea.Shape;
                    if (SetFieldsValue(ref newFea, ref in_fea))
                    {
                        int index = -1;
                        index = newFea.Fields.FindField("FromDate");
                        if (index > -1)
                        {
                            newFea.set_Value(index, in_FromDateTime.ToString("u"));
                        }
                        index = newFea.Fields.FindField("ToDate");
                        if (index > -1)
                        {
                            newFea.set_Value(index, DateTime.MaxValue.ToString("u"));
                        }
                        index = newFea.Fields.FindField("SourceOID");
                        if (index > -1)
                        {
                            newFea.set_Value(index, in_fea.OID.ToString());
                        }
                        index = newFea.Fields.FindField("State");
                        if (index > -1)
                        {
                            newFea.set_Value(index, in_iState.ToString());
                        }
                        index = newFea.Fields.FindField("VERSION");
                        if (index > -1)
                        {
                            newFea.set_Value(index, in_iVersion.ToString());
                        }
                        newFea.Store();
                    }
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(FesCur);
                    #endregion
                }
                else if (in_iState == 3)/////ɾ��
                {
                    //ɾ��Ҫ��Ҫ��Ҫ�ؼ����ҵ���Ҫ�ص���Ч�汾��ʹ���ΪʧЧ״̬
                    #region ɾ����Ҫ��
                    IQueryFilter queryFilter = new QueryFilterClass();
                    string sValue = DateTime.MaxValue.ToString("u");
                    //queryFilter.WhereClause = "ToDate='" + sValue + "' AND SourceOID=" + in_fea.OID.ToString();
                    queryFilter.WhereClause = "ToDate='" + sValue + "' AND SourceOID=" + in_fea.OID.ToString();
                    IFeatureCursor FesCur = getFeaCls.Search(queryFilter, false);
                    IFeature CurFea = FesCur.NextFeature();
                    while (CurFea != null)
                    {
                        //**************************************************************************************
                        //GUOZHENG ADDED Ӧ�����жϣ�����ɾ������ʷ��¼����Ϊ��Ч�İ汾�ȵ�ǰ�༭���ɰ汾��һҪС
                        //               ����Ҫ��ǰһ�汾����ʷ��¼��ΪʧЧ���ٽ���һ��ʧЧ�İ汾״̬Ϊɾ��
                        //               ����鿴��ʷ����ʱ���Ͱ汾��״̬��Ϊɾ�����ⲻ���߼�
                        bool IsLowVersion = false;////////��ǰ��Ч����ʷ��¼�Ƿ�ȵ�ǰ�༭���ɰ汾��һҪС
                        int index = CurFea.Fields.FindField("VERSION");
                        if (index > -1)
                        {
                            // CurFea.set_Value(index, in_iVersion.ToString());
                            int igetVersion = -1;
                            try
                            {
                                igetVersion = Convert.ToInt32(CurFea.get_Value(index).ToString());
                                if (igetVersion < in_iVersion - 1)
                                {
                                    IsLowVersion = true;
                                }
                                else
                                {
                                    IsLowVersion = false;
                                }
                            }
                            catch (Exception e)
                            {
                                //******************************************
                                //guozheng added System Exception log
                                if (SysCommon.Log.Module.SysLog == null)
                                    SysCommon.Log.Module.SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
                                SysCommon.Log.Module.SysLog.Write(e);
                                //******************************************
                            }
                        }
                        //***************************************************************************************
                        if (IsLowVersion)
                        {
                            string sLastVersionTime = Convert.ToDateTime(GetVersionEstablishTime(in_WS, in_iVersion - 1, out ex)).ToString("u");
                            //index = CurFea.Fields.FindField("VERSION");
                            //if (index > -1)
                            //{
                            //    CurFea.set_Value(index, in_iVersion - 1);
                            //}
                            index = CurFea.Fields.FindField("LASTUPDATE");
                            if (index > -1)
                            {
                                CurFea.set_Value(index, sLastVersionTime);
                            }
                            index = CurFea.Fields.FindField("ToDate");
                            if (index > -1)
                            {
                                CurFea.set_Value(index, sLastVersionTime);

                            }
                            ////////////����һ���µİ汾��״̬Ϊɾ��////////
                            #region ����һ���µİ汾��״̬Ϊɾ��/
                            IFeature newFea = getFeaCls.CreateFeature();
                            newFea.Shape = in_fea.Shape;
                            if (SetFieldsValue(ref newFea, ref in_fea))
                            {
                                index = -1;
                                index = newFea.Fields.FindField("FromDate");
                                if (index > -1)
                                {
                                    newFea.set_Value(index, sLastVersionTime);
                                }
                                index = newFea.Fields.FindField("ToDate");
                                if (index > -1)
                                {
                                    newFea.set_Value(index, in_FromDateTime.ToString("u"));
                                }
                                index = newFea.Fields.FindField("SourceOID");
                                if (index > -1)
                                {
                                    newFea.set_Value(index, in_fea.OID.ToString());
                                }
                                index = newFea.Fields.FindField("State");
                                if (index > -1)
                                {
                                    newFea.set_Value(index, in_iState.ToString());
                                }
                                index = newFea.Fields.FindField("VERSION");
                                if (index > -1)
                                {
                                    newFea.set_Value(index, in_iVersion.ToString());
                                }
                                newFea.Store();
                            }
                            #endregion
                        }
                        else
                        {
                            index = CurFea.Fields.FindField("State");
                            if (index > -1)
                            {
                                CurFea.set_Value(index, in_iState.ToString());
                            }
                            index = CurFea.Fields.FindField("VERSION");
                            if (index > -1)
                            {
                                CurFea.set_Value(index, in_iVersion);
                            }
                            index = CurFea.Fields.FindField("ToDate");
                            if (index > -1)
                            {
                                CurFea.set_Value(index, in_FromDateTime.ToString("u"));

                            }
                        }
                        CurFea.Store();
                        CurFea = FesCur.NextFeature();
                    }
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(FesCur);
                    #endregion
                }
                else
                {
                    ex = new Exception("��֧�ֵ�״̬");
                    return;
                }
            }
            catch (Exception eError)
            {
                //******************************************
                //guozheng added System Exception log
                if (SysCommon.Log.Module.SysLog == null)
                    SysCommon.Log.Module.SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
                SysCommon.Log.Module.SysLog.Write(eError);
                //******************************************
            }
        }
        /// <summary>
        /// ��pDesFeat���ֶθ���pOriFeat
        /// </summary>
        /// <param name="pOriFeat">���Խ���Ҫ��</param>
        /// <param name="pDesFeat">������ԴҪ��</param>
        /// <returns></returns>
        private bool SetFieldsValue(ref IFeature pOriFeat, ref IFeature pDesFeat)/////��pDesFeat���ֶθ���pOriFeat
        {

            int IdesFiledIndex = -1;
            string sFieldName = string.Empty;

            for (int i = 0; i < pOriFeat.Fields.FieldCount; i++)
            {
                if (pOriFeat.Fields.get_Field(i).Type == esriFieldType.esriFieldTypeOID || pOriFeat.Fields.get_Field(i).Type == esriFieldType.esriFieldTypeGeometry)
                    continue;
                if (pOriFeat.Fields.get_Field(i).Editable)
                {
                    sFieldName = pOriFeat.Fields.get_Field(i).Name;
                    IdesFiledIndex = pDesFeat.Fields.FindField(sFieldName);
                    if (IdesFiledIndex > -1)
                    {
                        if (pDesFeat.get_Value(IdesFiledIndex) != null)
                        {
                            pOriFeat.set_Value(i, pDesFeat.get_Value((int)IdesFiledIndex));

                        }
                        else
                        {
                            if (pDesFeat.Fields.get_Field(IdesFiledIndex).IsNullable)
                            {
                                pOriFeat.set_Value(i, null);
                            }
                            else
                            {
                                if (pDesFeat.Fields.get_Field(IdesFiledIndex).Type == esriFieldType.esriFieldTypeString)
                                {
                                    pOriFeat.set_Value(i, string.Empty);
                                }
                                else if (pDesFeat.Fields.get_Field(IdesFiledIndex).Type == esriFieldType.esriFieldTypeDouble || pDesFeat.Fields.get_Field(IdesFiledIndex).Type == esriFieldType.esriFieldTypeInteger || pDesFeat.Fields.get_Field(IdesFiledIndex).Type == esriFieldType.esriFieldTypeSingle)
                                {
                                    pOriFeat.set_Value(i, 0);
                                }
                            }

                        }
                    }
                }
            }
            return true;
        }

        //***********************************************************************************************************
        //guozheng added  ��ȡһ���汾������ʱ��
        private string GetVersionEstablishTime(IWorkspace in_WS, int in_iVersion, out Exception ex)
        {
            ex = null;
            if (in_WS == null) { ex = new Exception("���»�������δ��ʼ��"); return null; }
            try
            {
                ITable getTable = (in_WS as IFeatureWorkspace).OpenTable(this.m_sDBVersionTable);
                IQueryFilter Filter = new QueryFilter();
                Filter.WhereClause = "VERSION=" + in_iVersion.ToString();
                ICursor TableCur = getTable.Search(Filter, false);
                IRow getRow = TableCur.NextRow();
                while (getRow != null)
                {
                    int index = getRow.Fields.FindField("VERSIONTIME");
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(TableCur);
                    return getRow.get_Value(index).ToString();
                }
                ex = new Exception("���ݿ�汾��¼�����Ҳ����汾Ϊ��" + in_iVersion.ToString() + ",�Ľ���ʱ�䡣");
                System.Runtime.InteropServices.Marshal.ReleaseComObject(TableCur);
                return null;
            }
            catch
            {

                return null;
            }
        }
        //***********************************************************************************************************
    }
}
