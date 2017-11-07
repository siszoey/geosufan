using System;
using System.Collections.Generic;
using System.Text; 
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.esriSystem;

namespace GeoEdit
{
    /// <summary>
    /// ��ͻ�汾Э��  ���Ƿ�20101104���
    /// </summary>
    public class ClsVersionReconcile
    {
        private IFeatureWorkspace featureWorkspace = null;  //

        private IWorkspaceEdit v_WSEdit = null;//��ǰ�༭�����ռ�

        public ClsVersionReconcile(IWorkspaceEdit pWSEdit)//(IVersion version)
        {
            if (pWSEdit == null) return;
            v_WSEdit = pWSEdit;
            //// Save the version as a member variable.
            //featureWorkspace = (IFeatureWorkspace)version;
            //// Subscribe to the OnReconcile event.
            //IVersionEvents_Event versionEvent = (IVersionEvents_Event)version;
            //versionEvent.OnConflictsDetected += new
            //  IVersionEvents_OnConflictsDetectedEventHandler(OnConflictsDetected);
        }

        /// <summary>
        /// ��ȡ�����ռ��Ĭ�ϰ汾
        /// </summary>
        /// <param name="eError"></param>
        /// <returns></returns>
        public string GetDefautVersionName(out Exception eError)
        {
            string verName = "";//Ĭ�ϰ汾����
            eError = null;
            IVersionedWorkspace pVersionedWS = v_WSEdit as IVersionedWorkspace;
            if (pVersionedWS == null)
            {
                eError=new Exception("�����ݻ�û��ע��汾!");
                return "";
            }

            IVersion pDefVersion = pVersionedWS.DefaultVersion;  //Ĭ�ϰ汾
            if (pDefVersion == null)
            {
                eError = new Exception("��ȡĬ�ϰ汾����");
                return "";
            }
            if (pDefVersion.VersionInfo == null)
            {
                eError = new Exception("��ȡĬ�ϰ汾����");
                return "";
            }
            verName = pDefVersion.VersionInfo.VersionName;  

            if (verName == "")
            {
                eError=new Exception("��ȡĬ�ϰ汾���Ƴ���!");
                return "";
            }
            return verName;
        }

        //******************************************************************************************************************************************* 
        //��ʹ�����ݿ�汾������û��ѡ���ںϣ�˵����ͻҪ��û�з����仯������Ҫ����ͻҪ�ر�����������д�������־ʱ��Ҫ�ų���Щ��¼
        //ͬ����ʹ�����ݿ�汾��ѡ���ںϣ������ںϲ��ɹ���˵��Ҫ�ػ���û�з����仯��Ҳ��Ҫ����һ��Ҫ�ش洢��������д�������־ʱ���ų���Щ��¼

        //��ʹ�õ�ǰ�汾�������ݿ�汾���������������־��¼�������ͻҪ�ض�Ӧ�ĸ�����־��Ϣ������Ҫ�޸���ʷ����Ӧ��Ҫ�أ�����ǰ�ĸ���������Ϊ��Ч
        //*******************************************************************************************************************************************
        /// <summary>
        /// ������Ҫ�ų��ĳ�ͻҪ�ؼ�¼
        /// </summary>
        /// <param name="defVerName">Ĭ�ϰ汾����</param>
        /// <param name="bChildWin">true��ʾ�õ�ǰ�汾�滻ǰһ�汾��false��ʾʹ�����ݿ�汾</param>
        /// <param name="beMerge">true��ʾ�ںϳ�ͻҪ�ؼ�����״��false��ʾ���ں�</param>
        /// <param name="eError"></param>
        /// <returns></returns>
        public Dictionary<string, Dictionary<int, List<int>>> ReconcileVersion(string defVerName, bool bChildWin, bool beMerge, out Exception eError)
        {
            Dictionary<string, Dictionary<int, List<int>>> conflictFeaClsDic = null;  //������ͻ��Ҫ������Ϣ

            bool bLock = true;                      //true�����
            bool bebortIfConflict = false;           //��⵽��ͻʱ���汾Э���Ƿ�ֹͣ��trueֹͣ��false��ֹͣ
            //bool bChildWin = false;                 //true�滻��һ���汾����ͻ�汾��,false����һ���汾����ͻ�汾��
            bool battributeCheck = false;          //false��Ϊtrue��ֻ���޸�ͬһ������ʱ�ż�����ͻ
            //bool beMerge = true;                   //���ڲ�����ͻ��Ҫ���Ƿ��ں�
            eError = null;

            IVersionEdit4 pVersionEdit4 =v_WSEdit as IVersionEdit4; //�汾�༭
            if (pVersionEdit4 == null)
            {
                eError = new Exception("��ȡ��ǰ�汾�༭����");
                return null;
            }
            try
            {
                //Э���汾
                if (pVersionEdit4.Reconcile4(defVerName, bLock, bebortIfConflict, bChildWin, battributeCheck))
                {
                    //���ڳ�ͻ,��ͻ����ʽ��Ϊ3�֣� ʹ�õ�ǰ�汾��ʹ�����ݿ�汾��ʹ���Զ��崦��ʽ(δʵ��)
                    IFeatureWorkspace pOrgFWS = null;           //ԭʼ���ݹ����ռ�
                    IFeatureWorkspace pReconWS = null;          //��һ���༭��ͻ�汾�����ռ�
                    IFeatureWorkspace pPreReconWS = null;       //�ڶ����༭��ͻ�汾�����ռ䣨���ڱ༭�ģ�

                    pOrgFWS = pVersionEdit4.CommonAncestorVersion as IFeatureWorkspace;
                    if (pOrgFWS == null)
                    {
                        eError = new Exception("ԭʼ���ݿ⹤���ռ�Ϊ�գ�");
                        return null ;
                    }
                    pReconWS = pVersionEdit4.ReconcileVersion as IFeatureWorkspace;
                    if (pReconWS == null)
                    {
                        eError = new Exception("��һ���༭��ͻ�汾�����ռ�Ϊ�գ�");
                        return null;
                    }
                    pPreReconWS = pVersionEdit4.PreReconcileVersion as IFeatureWorkspace;
                    if (pPreReconWS == null)
                    {
                        eError = new Exception("�ڶ����༭��ͻ�汾�����ռ�Ϊ�գ�");
                        return null;
                    }

                    conflictFeaClsDic = new Dictionary<string, Dictionary<int, List<int>>>();
                    #region �Գ�ͻҪ������д���
                    //��ȡ������ͻ��Ҫ��
                    IEnumConflictClass pEnumConflictCls = pVersionEdit4.ConflictClasses;
                    if (pEnumConflictCls == null) return null ;
                    pEnumConflictCls.Reset();
                    IConflictClass pConflictCls = pEnumConflictCls.Next();
                    //������ͻҪ���࣬�Գ�ͻҪ�ؽ��д���
                    while (pConflictCls != null)
                    {
                        if (pConflictCls.HasConflicts)
                        {
                            //����Ҫ������ڳ�ͻҪ��
                            IDataset pdt = pConflictCls as IDataset;
                            string feaClsName = ""; //��ͻҪ��������
                            feaClsName = pdt.Name;  
                            if (feaClsName.Contains("."))
                            {
                                feaClsName = feaClsName.Substring(feaClsName.IndexOf('.') + 1);
                            }

                            IFeatureClass pFeaCls = null;               //��Ҫ�༭��featureclass
                            IFeatureClass pOrgFeaCls = null;                     //��ͻԭʼҪ����
                            IFeatureClass pReconFeaCls = null;                   //��һ���༭��ͻ�汾Ҫ����
                            IFeatureClass pPreReconFeaCls = null;                //�ڶ����༭�汾��ͻҪ����
                            IFeature pOrgFea = null;                             //ԭʼҪ�����в�����ͻ��Ӧ��Ҫ��
                            IFeature pReconFea = null;                           //��һ���༭�汾�в�����ͻ��Ҫ��
                            IFeature pPreReconFea = null;                        //�ڶ����༭�汾�в�����ͻ��Ҫ��
                            
                            Dictionary<int, List<int>> feaOIDDic = new Dictionary<int, List<int>>();//�������������ͻ��Ҫ������Ϣ
                            List<int> OidLst = null;                                                //�������������ͻ��Ҫ��oid����

                            try
                            {
                                pFeaCls = (v_WSEdit as IFeatureWorkspace).OpenFeatureClass(feaClsName);
                                //��ȡ��ͬ�汾��Ӧ�ĳ�ͻҪ����
                                pOrgFeaCls = pOrgFWS.OpenFeatureClass(feaClsName);
                                pReconFeaCls = pReconWS.OpenFeatureClass(feaClsName);
                                pPreReconFeaCls = pPreReconWS.OpenFeatureClass(feaClsName);
                            }
                            catch(Exception ex)
                            {
                                //******************************************
                                //guozheng added System Exception log
                                if (SysCommon.Log.Module.SysLog == null)
                                    SysCommon.Log.Module.SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
                                SysCommon.Log.Module.SysLog.Write(ex);
                                //******************************************
                                eError = new Exception("��ȡ��ͻҪ�������");
                                //******************************************
                                //guozheng added System Exception log
                                if (SysCommon.Log.Module.SysLog == null)
                                    SysCommon.Log.Module.SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
                                SysCommon.Log.Module.SysLog.Write(eError);
                                //******************************************
                                //if (!bChildWin &&beMerge)
                                //{
                                //    //ʹ�����ݿ�汾��δ�����ںϣ���Ҫ����Ϣ���ͻ
                                //    return;
                                //}
                                pConflictCls = pEnumConflictCls.Next();
                                continue;
                            }

                            #region �����ͻ�������ںϣ�,�������ͻҪ����Ϣ
                            //����һ���汾��Ŀ���ͻ�汾�����޸�,�޸���2��ʾ�������ڵ�ǰ�汾��ɾ����ѡ��
                            ISelectionSet deUpSelSet = pConflictCls.DeleteUpdates;
                            if (deUpSelSet != null)
                            {
                                #region ���г�ͻ���������������ͻ��Ҫ����Ϣ��������������ں�
                                IEnumIDs enumIDs = deUpSelSet.IDs;
                                int ppoid = -1;
                                ppoid = enumIDs.Next();

                                OidLst = new List<int>();
                                //������ͻҪ�����д��ڵ��޸�ɾ����ͻҪ��
                                while (ppoid != -1)
                                {
                                    if (beMerge)
                                    {
                                        // �ں�
                                        //eError = new Exception("�༭�����ص�,�����ں�\nҪ��OIDΪ:" + ppoid + ",Ҫ��������Ϊ��" + feaClsName + "��\n����Ϊ�õ�ǰ�༭����ԭ�б༭!");
                                    }
                                    if (bChildWin == false)
                                    {
                                        //ʹ�����ݿ�汾,���������ͻ��Ҫ����Ϣ
                                        if (!OidLst.Contains(ppoid))
                                        {
                                            OidLst.Add(ppoid);
                                        }
                                        if (!feaOIDDic.ContainsKey(2))
                                        {
                                            feaOIDDic.Add(2, OidLst);
                                        }
                                        else
                                        {
                                            if (!feaOIDDic[2].Contains(ppoid))
                                            {
                                                feaOIDDic[2].Add(ppoid);
                                            }
                                        }
                                    }
                                    ppoid = enumIDs.Next();
                                }
                                #endregion
                            }

                            //����һ���汾��Ŀ���ͻ�汾����ɾ��,ɾ����3��ʾ�������ڵ�ǰ�汾���޸ĵ�ѡ��
                            ISelectionSet upDeSelSet = pConflictCls.UpdateDeletes;
                            if (upDeSelSet != null)
                            {
                                #region ���г�ͻ���������������ͻ��Ҫ����Ϣ
                                IEnumIDs enumIDs = upDeSelSet.IDs;
                                int ppoid = -1;
                                ppoid = enumIDs.Next();
                                OidLst = new List<int>();
                                //����Ҫ�����д��ڵ�ɾ���޸ĳ�ͻ
                                while (ppoid != -1)
                                {
                                    if (beMerge)
                                    {
                                        // �ں�
                                        //SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "�༭�����ص�,�����ں�\nҪ��OIDΪ:" + ppoid + ",Ҫ��������Ϊ��" + feaClsName + "��");
                                    }
                                    if (bChildWin == false)
                                    {
                                        //ʹ�����ݿ�汾�����������ͻ���ںϲ��˵�Ҫ����Ϣ
                                        if (!OidLst.Contains(ppoid))
                                        {
                                            OidLst.Add(ppoid);
                                        }
                                        if (!feaOIDDic.ContainsKey(3))
                                        {
                                            feaOIDDic.Add(3, OidLst);
                                        }
                                        else
                                        {
                                            if (!feaOIDDic[3].Contains(ppoid))
                                            {
                                                feaOIDDic[3].Add(ppoid);
                                            }
                                        }
                                    }
                                    ppoid = enumIDs.Next();
                                }
                                #endregion
                            }

                            //����һ���汾��Ŀ���ͻ�汾�����޸�,�޸���2��ʾ�������ڵ�ǰ�汾���޸ĵ�ѡ��
                            ISelectionSet upUpSelSet = pConflictCls.UpdateUpdates;
                            if (upUpSelSet != null)
                            {
                                #region ���г�ͻ���������������ͻ��Ҫ����Ϣ
                                IEnumIDs enumIDs = upUpSelSet.IDs;
                                int ppoid = -1;
                                ppoid = enumIDs.Next();
                                OidLst = new List<int>();
                                //����Ҫ�����д��ڵ��޸��޸ĳ�ͻҪ��
                                while (ppoid != -1)
                                {
                                    if (pPreReconFeaCls == null || pReconFeaCls == null || pOrgFeaCls == null) break;
                                    pPreReconFea = pPreReconFeaCls.GetFeature(ppoid);
                                    pReconFea = pReconFeaCls.GetFeature(ppoid);
                                    pOrgFea = pOrgFeaCls.GetFeature(ppoid);
                                    if (pPreReconFea == null || pReconFea == null || pOrgFea == null) break;
                                    IConstructMerge constructMerge = new GeometryEnvironmentClass();  //�ںϱ���
                                    IGeometry newGeometry = null;                                     //�ںϺ�ļ�����״
                                    if (beMerge)
                                    {
                                        #region �ں�
                                        try
                                        {
                                            newGeometry = constructMerge.MergeGeometries(pOrgFea.ShapeCopy, pReconFea.ShapeCopy, pPreReconFea.ShapeCopy);
                                        }
                                        catch(Exception ex)
                                        {
                                            //SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "�༭�����ص�,�����ں�");
                                            //******************************************
                                            //guozheng added System Exception log
                                            if (SysCommon.Log.Module.SysLog == null)
                                                SysCommon.Log.Module.SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
                                            SysCommon.Log.Module.SysLog.Write(ex);
                                            //******************************************
                                            if (bChildWin == false)
                                            {
                                                //����ԭ���İ汾�滻���ڵİ汾���Ҳ����ںϣ�˵����Ҫ��û�з����仯��Ӧ������־��¼�������ų���
                                                //���������ͻ��Ҫ����Ϣ
                                                if (!OidLst.Contains(ppoid))
                                                {
                                                    OidLst.Add(ppoid);
                                                }
                                                if (!feaOIDDic.ContainsKey(2))
                                                {
                                                    feaOIDDic.Add(2, OidLst);
                                                }
                                                else
                                                {
                                                    if (!feaOIDDic[2].Contains(ppoid))
                                                    {
                                                        feaOIDDic[2].Add(ppoid);
                                                    }
                                                }
                                            }

                                            ppoid = enumIDs.Next();
                                            continue;
                                        }
                                        IFeature ppfea = pFeaCls.GetFeature(ppoid);
                                        ppfea.Shape = newGeometry;
                                        ppfea.Store();
                                        #endregion
                                    }
                                    else
                                    {
                                        #region ���ں�
                                        if (bChildWin == false)
                                        {
                                            //����ԭ���İ汾�滻���ڵİ汾���Ҳ����ںϣ�˵����Ҫ��û�з����仯��Ӧ������־��¼�������ų���
                                            //���������ͻ��Ҫ����Ϣ
                                            if (!OidLst.Contains(ppoid))
                                            {
                                                OidLst.Add(ppoid);
                                            }
                                            if (!feaOIDDic.ContainsKey(2))
                                            {
                                                feaOIDDic.Add(2, OidLst);
                                            }
                                            else
                                            {
                                                if (!feaOIDDic[2].Contains(ppoid))
                                                {
                                                    feaOIDDic[2].Add(ppoid);
                                                }
                                            }
                                        }
                                        #endregion
                                    }
                                    ppoid = enumIDs.Next();
                                }
                                #endregion
                            }

                            if (!conflictFeaClsDic.ContainsKey(feaClsName))
                            {
                                if (feaOIDDic != null)
                                {
                                    if (feaOIDDic.Count > 0)
                                    {
                                        conflictFeaClsDic.Add(feaClsName, feaOIDDic);
                                    }
                                }
                            }
                            #endregion
                        }
                        pConflictCls = pEnumConflictCls.Next();
                    }
                    #endregion
                }
                return conflictFeaClsDic;
            }
            catch(Exception ex)
            {
                //******************************************
                //guozheng added System Exception log
                if (SysCommon.Log.Module.SysLog == null)
                    SysCommon.Log.Module.SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
                SysCommon.Log.Module.SysLog.Write(ex);
                //******************************************
                eError=new Exception("�汾Э������!");
                return null;
            }
        }

        /// <summary>
        /// ���淢�����±仯�����е�Ҫ����Ϣ
        /// </summary>
        /// <param name="eError"></param>
        /// <returns></returns>
        public Dictionary<string, Dictionary<int, List<IRow>>> GetModifyClsInfo(out Exception eError)
        {
            eError = null; 
            Dictionary<string, Dictionary<int, List<IRow>>> feaChangeDic = null;

            //��������ռ��·����仯����Ϣ
            IWorkspaceEdit2 pWSEdit2 =v_WSEdit as IWorkspaceEdit2;
            if (pWSEdit2 == null)
            {
                eError = new Exception("�༭�����ռ�Ϊ�գ�");
                return null;
            }

            //��ȡ��һ���༭�Ự�з������±仯����
            IDataChangesEx pDataChangeEx = pWSEdit2.get_EditDataChanges(esriEditDataChangesType.esriEditDataChangesWithinSession);
            if (pDataChangeEx == null)
            {
                eError = new Exception("δ���ָ��±仯����!");
                return null;
            }

            feaChangeDic = new Dictionary<string, Dictionary<int, List<IRow>>>();

            //��ȡ�������±仯��Ҫ���༯��
            IEnumBSTR pEnumString = pDataChangeEx.ModifiedClasses;  //�����仯��Ҫ����
            pEnumString.Reset();
            string pModifyClsName = "";//�������±仯��Ҫ��������
            pModifyClsName = pEnumString.Next();
            //�����������±仯��Ҫ���࣬���������Ϣ��������
            while (pModifyClsName != null)
            {
                IDifferenceCursorEx pDifCusorEx = null;  //�α�
                int pFeaOid = -1;                       //Ҫ��OID
                IRow pSourceRow = null;                 //���ڱ༭�ĳ�ͻҪ����
                IRow pDifRow = null;                    //��һ�༭�汾�ĳ�ͻҪ����
                ILongArray pDifIndices = null;          //�ֶ�����

                if (pModifyClsName.Contains("."))
                {
                    pModifyClsName = pModifyClsName.Substring(pModifyClsName.IndexOf('.') + 1);
                }
                Dictionary<int, List<IRow>> stateRowDic = new Dictionary<int, List<IRow>>();
                List<IRow> rowLst = null;

                #region ���������ӵ�Ҫ����Ϣ
                rowLst = new List<IRow>();
                //���������α�
                pDifCusorEx = pDataChangeEx.ExtractEx(pModifyClsName, esriDifferenceType.esriDifferenceTypeInsert);
                if (pDifCusorEx == null)
                {
                    eError = new Exception("��ȡ�������ݵ��α귢������");
                    return null;
                }
                pDifCusorEx.Next(out pFeaOid, out pSourceRow, out pDifRow, out pDifIndices);//pDifRow=null
                //���������ӵ�Ҫ��oid,�����䱣������
                while (pFeaOid != -1)
                {
                    //���������ݱ�������
                    if (!rowLst.Contains(pSourceRow))
                    {
                        rowLst.Add(pSourceRow);
                    }
                    pDifCusorEx.Next(out pFeaOid, out pSourceRow, out pDifRow, out pDifIndices);
                }
                if (rowLst != null)
                {
                    if (rowLst.Count > 0)
                    {
                        stateRowDic.Add(1, rowLst);
                    }
                }
                #endregion

                #region �����޸ĺ��Ҫ����Ϣ
                rowLst = new List<IRow>();
                //�޸������α�
                pDifCusorEx = pDataChangeEx.ExtractEx(pModifyClsName, esriDifferenceType.esriDifferenceTypeUpdateNoChange);
                if (pDifCusorEx == null)
                {
                    eError = new Exception("��ȡ�޸����ݵ��α귢������");
                    return null;
                }
                pDifCusorEx.Next(out pFeaOid, out pSourceRow, out pDifRow, out pDifIndices);
                //�����޸ĺ��Ҫ��OID�������䱣������
                while (pFeaOid != -1)
                {
                    //���޸ĺ�����ݱ�������
                    if (!rowLst.Contains(pSourceRow))
                    {
                        rowLst.Add(pSourceRow);
                    }

                    pDifCusorEx.Next(out pFeaOid, out pSourceRow, out pDifRow, out pDifIndices);
                }
                if (rowLst != null)
                {
                    if (rowLst.Count > 0)
                    {
                        stateRowDic.Add(2, rowLst);
                    }
                }
                #endregion

                #region ����ɾ����Ҫ����Ϣ
                rowLst = new List<IRow>();
                //ɾ�������α�
                pDifCusorEx = pDataChangeEx.ExtractEx(pModifyClsName, esriDifferenceType.esriDifferenceTypeDeleteNoChange);
                if (pDifCusorEx == null)
                {
                    eError = new Exception("��ȡɾ�����ݵ��α귢������");
                    return null;
                }
                pDifCusorEx.Next(out pFeaOid, out pSourceRow, out pDifRow, out pDifIndices);//pSourceRow
                //����ɾ����Ҫ��OID�������䱣������
                while (pFeaOid != -1)
                {
                    //��ɾ�������ݱ������������ݿ�汾���ݣ�
                    if (!rowLst.Contains(pDifRow))
                    {
                        rowLst.Add(pDifRow);
                    }
                    pDifCusorEx.Next(out pFeaOid, out pSourceRow, out pDifRow, out pDifIndices);
                }
                if (rowLst != null)
                {
                    if (rowLst.Count > 0)
                    {
                        stateRowDic.Add(3, rowLst);
                    }
                }
                #endregion

                if (!feaChangeDic.ContainsKey(pModifyClsName))
                {
                    feaChangeDic.Add(pModifyClsName, stateRowDic);
                }
                pModifyClsName = pEnumString.Next();
            }
            
            return feaChangeDic;
        }


        /// <summary>
        /// ������º�ĸ��±仯��Ҫ����Ϣ���ų�����ͻҪ��
        /// </summary>
        /// <param name="feaChangeDic">���±仯Ҫ�أ���ͻҪ��</param>
        /// <param name="conflictFeaClsDic"></param>
        /// <param name="eError"></param>
        public Dictionary<string, Dictionary<int, List<IRow>>> GetPureModifySaveInfo(Dictionary<string, Dictionary<int, List<IRow>>> feaChangeDic, Dictionary<string, Dictionary<int, List<int>>> conflictFeaClsDic, out Exception eError)
        {
            eError = null;
            if (conflictFeaClsDic == null) return feaChangeDic;
            if (conflictFeaClsDic.Count == 0) return feaChangeDic;
            //����������ͻ��Ҫ����
            foreach (KeyValuePair<string, Dictionary<int, List<int>>> feaConflicItem in conflictFeaClsDic)
            {
                string pFeaCLsName= "";                                                         //���±仯��Ҫ��������
                Dictionary<int, List<IRow>> feaSafeDic = new Dictionary<int, List<IRow>>();    //����༭���״̬��Ҫ�ؼ���
                pFeaCLsName = feaConflicItem.Key;
                if (feaChangeDic.ContainsKey(pFeaCLsName))
                {
                    feaSafeDic = feaChangeDic[pFeaCLsName];
                }
                else
                {
                    continue;
                }

                //�������仯״̬��1,2,3
                foreach (KeyValuePair<int, List<int>> stateOIdItem in feaConflicItem.Value)
                {
                    int pState;                          //���±仯״̬
                    List<IRow> rowLst = new List<IRow>();  //������ͻ��Ҫ�ؼ���
                    pState = stateOIdItem.Key;

                    if (feaSafeDic.ContainsKey(pState))
                    {
                        rowLst = feaSafeDic[pState];
                    }
                    else
                    {
                        continue;
                    }
                    //����ĳһ����״̬�µĸ����м���
                    foreach (int oidItem in stateOIdItem.Value)
                    {
                        foreach(IRow rowItem in rowLst)
                        {
                            int pOID = rowItem.OID;
                            if (pOID == oidItem)
                            {
                                //ɾ��������¼
                                if (!rowLst.Remove(rowItem))
                                {
                                    eError = new Exception("ɾ����ͻ�����");
                                    return null;
                                }
                            }
                        }
                    }
                }
            }

            return feaChangeDic;
        }






        /// <summary>
        /// Pseudocode:
        /// - Loop through all conflicts classes after the reconcile.
        /// - Loop through every UpdateUpdate conflict on the class.
        /// - Determine if geometry is in conflict on the feature.
        /// - If so, merge geometries together (handling errors) and store the feature.
        /// </summary>
        public void OnConflictsDetected(ref bool conflictsRemoved, ref bool errorOccurred, ref string errorString)
        {
            try
            {
                IVersionEdit4 versionEdit4 = (IVersionEdit4)featureWorkspace;
                // Get the various versions on which to output information.
                IFeatureWorkspace commonAncestorFWorkspace = (IFeatureWorkspace)
                  versionEdit4.CommonAncestorVersion;
                IFeatureWorkspace preReconcileFWorkspace = (IFeatureWorkspace)
                  versionEdit4.PreReconcileVersion;
                IFeatureWorkspace reconcileFWorkspace = (IFeatureWorkspace)
                  versionEdit4.ReconcileVersion;
                IEnumConflictClass enumConflictClass = versionEdit4.ConflictClasses;
                IConflictClass conflictClass = null;
                while ((conflictClass = enumConflictClass.Next()) != null)
                {
                    IDataset dataset = (IDataset)conflictClass;
                    // Make sure class is a feature class.
                    if (dataset.Type == esriDatasetType.esriDTFeatureClass)
                    {
                        String datasetName = dataset.Name;
                        IFeatureClass featureClass = featureWorkspace.OpenFeatureClass
                          (datasetName);
                        Console.WriteLine("Conflicts on feature class {0}", datasetName);
                        // Get all UpdateUpdate conflicts.
                        ISelectionSet updateUpdates = conflictClass.UpdateUpdates;
                        if (updateUpdates.Count > 0)
                        {
                            // Get conflict feature classes on the three reconcile versions.
                            IFeatureClass featureClassPreReconcile =
                              preReconcileFWorkspace.OpenFeatureClass(datasetName);
                            IFeatureClass featureClassReconcile =
                              reconcileFWorkspace.OpenFeatureClass(datasetName);
                            IFeatureClass featureClassCommonAncestor =
                              commonAncestorFWorkspace.OpenFeatureClass(datasetName);
                            // Iterate through each OID, outputting information.
                            IEnumIDs enumIDs = updateUpdates.IDs;
                            int oid = -1;
                            while ((oid = enumIDs.Next()) != -1)
                            //loop through all conflicting features 
                            {
                                Console.WriteLine("UpdateUpdate conflicts on feature {0}", oid);
                                // Get conflict feature on the three reconcile versions.
                                IFeature featurePreReconcile =
                                  featureClassPreReconcile.GetFeature(oid);
                                IFeature featureReconcile = featureClassReconcile.GetFeature(oid);
                                IFeature featureCommonAncestor =
                                  featureClassCommonAncestor.GetFeature(oid);
                                // Check to make sure each shape is different than the common ancestor (conflict is on shape field).
                                if (IsShapeInConflict(featureCommonAncestor, featurePreReconcile,
                                  featureReconcile))
                                {
                                    Console.WriteLine(
                                      " Shape attribute has changed on both versions...");
                                    // Geometries are in conflict ... merge geometries.
                                    try
                                    {
                                        IConstructMerge constructMerge = new GeometryEnvironmentClass
                                          ();
                                        IGeometry newGeometry = constructMerge.MergeGeometries
                                          (featureCommonAncestor.ShapeCopy,
                                          featureReconcile.ShapeCopy, featurePreReconcile.ShapeCopy);
                                        // Setting new geometry as a merge between the two versions.
                                        IFeature feature = featureClass.GetFeature(oid);
                                        feature.Shape = newGeometry;
                                        feature.Store();
                                        updateUpdates.RemoveList(1, ref oid);
                                        conflictsRemoved = true;
                                    }
                                    catch(Exception eError) //COMException comExc
                                    {
                                        //******************************************
                                        //guozheng added System Exception log
                                        if (SysCommon.Log.Module.SysLog == null)
                                            SysCommon.Log.Module.SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
                                        SysCommon.Log.Module.SysLog.Write(eError);
                                        //******************************************
                                        //// Check if the error is from overlapping edits.
                                        //if (comExc.ErrorCode == (int)
                                        //  fdoError.FDO_E_WORKSPACE_EXTENSION_DATASET_CREATE_FAILED ||
                                        //  comExc.ErrorCode == (int)
                                        //  fdoError.FDO_E_WORKSPACE_EXTENSION_DATASET_DELETE_FAILED)
                                        //{
                                        //    // Edited areas overlap.
                                        //    Console.WriteLine(
                                        //      "Error from overlapping edits on feature {0}", oid);
                                        //    Console.WriteLine(" Error Message: {0}", comExc.Message);
                                        //    Console.WriteLine(
                                        //      " Can't merge overlapping edits to same feature.");
                                        //}
                                        //else
                                        //{
                                        //    // Unexpected COM exception throw this to exception handler.
                                        //    throw comExc;
                                        //}
                                    }
                                }
                                else
                                {
                                    Console.WriteLine(
                                      " Shape field not in conflict: merge not necessary ... ");
                                }
                            }
                        }
                    }
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
                //Console.WriteLine("Error Message: {0}, Error Code: {1}", comExc.Message,
                //  comExc.ErrorCode);
            }
            //catch (Exception exc)
            //{
            //    Console.WriteLine("Unhandled Exception: {0}", exc.Message);
            //}
        }


        //�ж�ԭʼ�汾Ҫ�أ���ͻҪ�غ͵�ǰ�汾Ҫ���Ƿ���ڼ��γ�ͻ
        // Method to determine if shape field is in conflict.
        private bool IsShapeInConflict(IFeature commonAncestorFeature, IFeature preReconcileFeature, IFeature reconcileFeature)
        {
            // 1st check: Common Ancestor with PreReconcile.
            // 2nd check: Common Ancestor with Reconcile. 
            // 3rd check: Reconcile with PreReconcile (case of same change on both versions)
            if (IsGeometryEqual(commonAncestorFeature.ShapeCopy,preReconcileFeature.ShapeCopy) || 
                IsGeometryEqual(commonAncestorFeature.ShapeCopy, reconcileFeature.ShapeCopy) ||
              IsGeometryEqual(reconcileFeature.ShapeCopy, preReconcileFeature.ShapeCopy)
              )
            {
                return false;
            }
            else
            {
                return true;
            }
        }



        //�ж�����������״�Ƿ����
        // Method returning if two shapes are equal to one another.
        private bool IsGeometryEqual(IGeometry shape1, IGeometry shape2)
        {
            if (shape1 == null && shape2 == null)
            {
                return true;
            }
            else if (shape1 == null|| shape2 == null)
            {
                return false;
            }
            else
            {
                IClone clone1 = (IClone)shape1;
                IClone clone2 = (IClone)shape2;
                return clone1.IsEqual(clone2);
            }
        }
    }
}
