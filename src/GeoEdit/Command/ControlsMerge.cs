using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.SystemUI;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.esriSystem;

namespace GeoEdit
{
    public class ControlsMerge : Plugin.Interface.CommandRefBase
    {
        private Plugin.Application.IAppGISRef myHook;
        private IFeatureLayer  m_MergeLayer;
        static int m_SavedOID = 0;

        ICommand _cmd = null;      //��̬���ص�����İ�ť

        public static int SavedOID
        {
            set { ControlsMerge.m_SavedOID = value; }
        }

        public ControlsMerge()
        {
            base._Name = "GeoEdit.ControlsMerge";
            base._Caption = "�ں�";
            base._Tooltip = "�ں�";
            base._Checked = false;
            base._Visible = true;
            base._Enabled = true;
            base._Message = "�ں�";
            
        }

        //===================================================================================================
        //��ʤ��  2009-08-19 ���
        /// <summary>
        /// �����ںϰ�ť�Ƿ�Ϊ���ã���Map�ϵĴ���ͼ�㣬����ѡ����Ҫ�أ����ұ༭״̬Ϊ��ʱ���ð�ť����
        /// </summary>
        public override bool Enabled
        {
            get
            {
                if (myHook == null) return false;
                try
                {

                    if (MoData.v_CurWorkspaceEdit == null) return false;

                    //�ж��Ƿ������ͼ��
                    if (myHook.MapControl.LayerCount == 0)
                    {
                        base._Enabled = false;
                        return false;
                    }
                    //�ж��Ƿ�ѡ���������������ϵ�Ҫ��Ҫ��
                    if (myHook.MapControl.Map.SelectionCount < 2)
                    {
                        base._Enabled = false;
                        return false;
                    }

                    //��ȡ��ѡ��Ҫ�ص����ݼ����ж��Ƿ�༭�Ѿ���
                    bool isEditing = GetDatasetEditState(myHook.MapControl.Map);
                    if (!isEditing)
                    {
                        base._Enabled = false;
                        return false;
                    }

                    //��������������������Ϊ����
                    base._Enabled = true;
                    return true;
                }
                catch
                {
                    base._Enabled = false;
                    return false;
                }
            }
        }
        //======================================================================================================

        #region �ж�Ҫ���Ƿ���ͬһͼ���Լ�ͼ��ı༭״̬
        //===================================================================================================
        //��ʤ��  2009-08-19 ���
        /// <summary>
        /// ��ͼ���ϻ�ȡѡ��Ҫ���������ݼ��Ĺ����ռ䣬�ж��Ƿ����༭
        /// </summary>
        /// <param name="pMap">��ǰ���������ݵĵ�ͼ����</param>
        /// <returns></returns>
        private bool GetDatasetEditState(ESRI.ArcGIS.Carto.IMap pMap)
        {

            int pSameLyr = 0;  //��¼Ҫ���Ƿ�Ϊͬ��
            ILayer pLayer = null;
            //�ж�ѡ���Ҫ���Ƿ���ͬһ��ͼ��
            for (int i = 0; i < pMap.LayerCount; i++)
            {
                IFeatureLayer pFeatLyr = null;
                IFeatureSelection pFeatSel = null;
                pLayer = pMap.get_Layer(i);
                if(pLayer is IGroupLayer )
                {
                    if (pLayer.Name == "ʾ��ͼ")
                    {
                        continue;
                    }
                    ICompositeLayer pComLayer = pLayer as ICompositeLayer;
                    for (int j = 0; j < pComLayer.Count; j++)
                    {
                        ILayer mLayer = pComLayer.get_Layer(j);
                        pFeatLyr = mLayer as IFeatureLayer;
                        if (pFeatLyr != null)
                        {
                            pFeatSel = pFeatLyr as IFeatureSelection;
                            if (pFeatSel.SelectionSet.Count > 0)
                            {
                                pSameLyr = pSameLyr + 1;
                                m_MergeLayer = pFeatLyr;//��ֻ��һ��ͼ�㱻ѡ��ʱ��pFeatLyr����Ҫ�����ںϵ�Ŀ��ͼ��
                            }
                        }
                    }
                }
                
                pFeatLyr = pLayer as IFeatureLayer;
                if (pFeatLyr != null)
                {
                    pFeatSel = pFeatLyr as IFeatureSelection;
                    if (pFeatSel.SelectionSet.Count > 0)
                    {
                        pSameLyr = pSameLyr + 1;
                        m_MergeLayer = pFeatLyr;//��ֻ��һ��ͼ�㱻ѡ��ʱ��pFeatLyr����Ҫ�����ںϵ�Ŀ��ͼ��
                    }
                }
            }
            //���ѡ���Ҫ�����ڵĲ�������1����Ҫ�ز���ͬһ����
            if (pSameLyr != 1)
            {
                return false;
            }
            
            //һϵ�е�QI����IWorkspaceEdit�ӿڶ���pWSE
            IFeatureClass pFeatCls = m_MergeLayer.FeatureClass as IFeatureClass;
            IDataset pDS = pFeatCls as IDataset;
            IWorkspace pWs = pDS.Workspace as IWorkspace;
            IWorkspaceEdit pWSE = pWs as IWorkspaceEdit;

            return pWSE.IsBeingEdited();  //���ر༭״̬
            
        }
        //=================================================================================================== 
        #endregion

        public override string Message
        {
            get
            {
                Plugin.Application.IAppFormRef pAppFormRef = myHook as Plugin.Application.IAppFormRef;
                if (pAppFormRef != null)
                {
                    pAppFormRef.OperatorTips = base._Message;
                }
                return base._Message;
            }
        }

        public override void ClearMessage()
        {
            Plugin.Application.IAppFormRef pAppFormRef = myHook as Plugin.Application.IAppFormRef;
            if (pAppFormRef != null)
            {
                pAppFormRef.OperatorTips = string.Empty;
            }
        }
        /// <summary>
        /// ִ���ںϹ���
        /// </summary>
        public override void OnClick()
        {
            if (m_MergeLayer!=null)
            {
                IFeatureSelection pFeatSel = m_MergeLayer as IFeatureSelection;
                ISelectionSet pSelectionSet = pFeatSel.SelectionSet;
      
                //���ѡ���Ҫ�ض���һ��������Կ�ʼ�ں�
                if (pSelectionSet.Count>1)
                {
                    //��������
                    MoData.v_CurWorkspaceEdit.StartEditOperation();

                    #region ��������ѡ�����
                    Plugin.Application.IAppFormRef pAppform = myHook as Plugin.Application.IAppFormRef;
                    AttributeShow_state.Temp_frm4Merge = new FrmAttribute4Merge(GetData_tree(), GetData_View(), myHook);//��һ��ֵ�����嵱��
                    AttributeShow_state.Temp_frm4Merge.Owner = pAppform.MainForm;
                    AttributeShow_state.Temp_frm4Merge.ShowInTaskbar = false;
                    AttributeShow_state.Temp_frm4Merge.ShowDialog(); 
                    #endregion

                    //ȷ���˱����ĸ�Ҫ�ز��ܽ����ں�
                    if (m_SavedOID != 0)
                    {
                        MergeSelectedFeature(pSelectionSet, m_MergeLayer, m_SavedOID);
                        m_SavedOID = 0;   //��Ϊ��
                    }

                    //��������
                    MoData.v_CurWorkspaceEdit.StopEditOperation();
                }

                _cmd.OnClick();  //ִ�����ѡ��

                //ˢ��ͼ��
                IActiveView pActiveView = myHook.MapControl.Map as IActiveView;
                pActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeography, null, null);  
            }
        }

        #region Ҫ���ںϵ�������   
        //========================================================================================
        //��ʤ��  2009-08-19  ���
        /// <summary>
        /// �ں�ѡ���е�Ҫ��
        /// </summary>
        /// <param name="pSelectionSet">ѡ�񼯶��󣬴���ȡ����Ҫ�ںϵ�Ҫ��</param>
        /// <param name="pMergeLayer">�����ںϵ�Ҫ������ͼ��</param>
        /// <param name="OID">��������Ҫ��OID</param>
        private void MergeSelectedFeature(ISelectionSet pSelectionSet, IFeatureLayer pMergeLayer, int OID)
        {
            //�ж��Ƿ������ں�����
            bool CanMerge = JudgeCondition();
            //OID = 0;
            //��������ںϵ��ж��������ܿ�ʼ�����ںϣ�������Ҫ�ּ�������������������
            if (CanMerge)
            {
                IEnumIDs pEnumIDs;     //ö��Ҫ�ر���
                Dictionary<int, IFeature> pDicFeature =new Dictionary<int,IFeature>();   //�洢�����Ҫ�ںϵ�Ҫ�ص��ֵ�
                int FtID = 0;         //Ҫ��OID
                IGeometry pGeoTemp=null;    //�洢�ںϺ��Ҫ��ͼ��
                ITopologicalOperator pTopoOperator;
                IFeature pFt = null;
                int pTempFeatID = 0;

                IFeatureClass pFeatCls = pMergeLayer.FeatureClass;

                pEnumIDs = pSelectionSet.IDs;
                FtID = pEnumIDs.Next();

                //��ʼ����ʱҪ��ͼ��
                pTempFeatID = FtID;
                pGeoTemp = pFeatCls.GetFeature(pTempFeatID).Shape;
                pDicFeature.Add(FtID, pFeatCls.GetFeature(pTempFeatID));  //��¼��������Ҫ��ID

                FtID = pEnumIDs.Next();
                while (FtID!=-1)
                {
                    //�����Ҫ�ںϵĵڶ���Ҫ��
                    pFt = pFeatCls.GetFeature(FtID);
                    pDicFeature.Add(FtID, pFt);  //��¼��������Ҫ��ID

                    //�ں�Ҫ�ص�ͼ��
                    pTopoOperator = pGeoTemp as ITopologicalOperator;
                    pGeoTemp = pTopoOperator.Union(pFt.ShapeCopy);

                    //��ʱͼ�ν��м򵥻�
                    pTopoOperator = pGeoTemp as ITopologicalOperator;
                    pTopoOperator.Simplify();

                    //��һ��Ҫ��
                    FtID = pEnumIDs.Next();
                }

                //��ȡ��һ��������Ҫ�أ��������ںϺ��ͼ�Σ�������
                pFt = pFeatCls.GetFeature(OID);
                pFt.Shape = pGeoTemp;

                //��������FID��state��һ������ wjj 20090903
                IDisplayRelationshipClass pDisResCls = pMergeLayer as IDisplayRelationshipClass;
                if (pDisResCls.RelationshipClass != null)     //�Ƿ�����־��¼�����������ѯ
                {
                    //��ȡͼ����ϱ�
                    IFeatureLayer pFeatLay = ModPublic.GetLayerOfGroupLayer(myHook.MapControl, "��Χ", "ͼ����Χ") as IFeatureLayer;
                    if (pFeatLay != null)
                    {
                        ChangeFeatureState(pDisResCls.RelationshipClass, pFt, pDicFeature, pFeatLay.FeatureClass);
                    }
                }
                
                pFt.Store();

                //ɾ�����ںϵ�Ҫ��
                foreach (KeyValuePair<int, IFeature> var in pDicFeature)
                {
                    if (var.Key != OID)
                    {
                        pFt = pFeatCls.GetFeature(var.Key);
                        pFt.Delete();
                    }

                }

                pDicFeature = null;
            }
        }

        #region wjj 20090903 ���������ں�ʱFID��state��һ�����Ⲣ�޸ĺ�����־��¼��
        /// <summary> 
        /// ���������ں�ʱFID��state��һ�����Ⲣ�޸ĺ�����־��¼��
        /// </summary>
        /// <param name="pRelationshipClass">�ں�Ҫ�ع�ϵ��</param>
        /// <param name="pTargetFeat">�������Ե�Ҫ��</param>
        /// <param name="pDicFeature">�ں�Ҫ�ؼ���</param>
        /// <param name="pRangeClss">ͼ����ϱ�</param>
        private void ChangeFeatureState(IRelationshipClass pRelationshipClass, IFeature pTargetFeat, Dictionary<int, IFeature> pDicFeature,IFeatureClass pRangeClss)
        {
            if (MoData.v_LogTable == null) return;
            if (MoData.v_LogTable.DbConn == null) return;  //��־��¼���Ƿ�����

            if (pDicFeature == null || pRelationshipClass == null || pRangeClss==null) return;
            Exception exError = null;
            IRelQueryTable pJionRelQueryTable = SysCommon.Gis.ModGisPub.GetRelQueryTable(pRelationshipClass, true, null, "STATE", false, true, out exError);
            IFeatureClass pFeatCls=pJionRelQueryTable as IFeatureClass;
            if (pFeatCls == null) return;

            IDataset pOriginIDataset = pRelationshipClass.OriginClass as IDataset;    //Ҫ����
            if (pOriginIDataset == null) return;

            int indexOID = pFeatCls.Fields.FindField(pOriginIDataset.Name+".OBJECTID");
            int indexFID = pFeatCls.Fields.FindField(pOriginIDataset.Name + ".GOFID");
            int indexState = pFeatCls.Fields.FindField( pRelationshipClass.DestinationClass.AliasName+ ".STATE");
            if (indexOID == -1 || indexFID == -1 || indexState == -1) return;

            object temp = pFeatCls.GetFeature(pTargetFeat.OID).get_Value(indexState);
            if (temp is DBNull) temp = -1;          //��ʾδ�仯
            int valueTargetState = Convert.ToInt32(temp);
            int valueTargetFID = Convert.ToInt32(pFeatCls.GetFeature(pTargetFeat.OID).get_Value(indexFID));
            if (valueTargetState == 1 || valueTargetState==2)    //Ϊ�����仯��Ҫ��      
            {
                foreach (KeyValuePair<int, IFeature> keyValue in pDicFeature)
                {
                    if (keyValue.Key == pTargetFeat.OID) continue;   //Ϊ�������Ե�Ҫ���������¸�Ҫ��
                    IFeature pTempFeature=pFeatCls.GetFeature(keyValue.Key);
                    temp = pTempFeature.get_Value(indexState);
                    if (temp is DBNull) temp = -1;          //��ʾδ�仯
                    int valueState = Convert.ToInt32(temp);
                    int valueFID = Convert.ToInt32(pTempFeature.get_Value(indexFID));

                    if (valueState == 1)
                    {//��Ҫ�ص�StateΪ�½�,��ɾ����Ҫ����־��¼���ж�Ӧ����
                        string strCon = "LAYERNAME='" + pOriginIDataset.Name + "'and OID=" + pTempFeature.OID + " and STATE=1 and SAVE=1";
                        MoData.v_LogTable.UpdateTable("delete * from UpdateLog where " + strCon, out exError);
                    }
                    else if (valueState == 2)
                    {//��Ҫ�ص�StateΪ�޸�
                        string strCon = "GOFID=" + valueFID + " and SAVE=1";
                        if (valueFID != valueTargetFID)
                        {//��Ҫ�ص�FID�뱣������Ҫ��FID��ͬ,�򽫸�Ҫ����־��¼���ж�Ӧ����״̬�޸�Ϊɾ��
                            MoData.v_LogTable.UpdateTable("update UpdateLog set STATE=3 where " + strCon, out exError);
                        }
                        else
                        {//��ɾ����Ҫ����־��¼���ж�Ӧ����
                            MoData.v_LogTable.UpdateTable("delete * from UpdateLog where " + strCon, out exError);
                        }
                    }
                    else
                    {//��Ҫ�ص�StateΪδ�仯
                        if (valueFID != valueTargetFID)
                        {//��Ҫ�ص�FID�뱣������Ҫ��FID��ͬ,������־��¼�������Ӹ�Ҫ�ض�Ӧ����״̬Ϊɾ��
                            string strSQL = "insert into UpdateLog(GOFID,STATE,SAVE) values(" + valueFID.ToString() + ",3,1)";
                            MoData.v_LogTable.UpdateTable( strSQL, out exError);
                        }
                    }
                }
            }
            else    //Ϊδ�����仯��Ҫ��   
            {
                bool bAddLog = false;
                foreach (KeyValuePair<int, IFeature> keyValue in pDicFeature)
                {
                    if (keyValue.Key == pTargetFeat.OID) continue;   //Ϊ�������Ե�Ҫ���������¸�Ҫ��
                    IFeature pTempFeature = pFeatCls.GetFeature(keyValue.Key);
                    temp = pTempFeature.get_Value(indexState);
                    if (temp is DBNull) temp = -1;          //��ʾδ�仯
                    int valueState = Convert.ToInt32(temp);
                    int valueFID = Convert.ToInt32(pTempFeature.get_Value(indexFID));

                    if (valueState == 1)
                    {//��Ҫ�ص�StateΪ�½�,��ɾ����Ҫ����־��¼���ж�Ӧ����,������־��¼�������ӱ�������Ҫ�ض�Ӧ����״̬Ϊ�޸�
                        string strCon = "LAYERNAME='" + pOriginIDataset.Name + "'and OID=" + pTempFeature.OID + " and STATE=1 and SAVE=1";
                        MoData.v_LogTable.UpdateTable("delete * from UpdateLog where " + strCon, out exError);

                        if (bAddLog == false) //(ֻ����һ��)����־��¼�������ӱ�������Ҫ�ض�Ӧ����״̬Ϊ�޸�
                        {
                            string strSQL = "insert into UpdateLog(GOFID,LAYERNAME,OID,STATE,SAVE) values(" + valueTargetFID.ToString() + ",'" + pOriginIDataset.Name + "'," + pTargetFeat.OID + ",2,1)";
                            MoData.v_LogTable.UpdateTable(strSQL, out exError);
                            bAddLog = true;
                        }
                    }
                    else if (valueState == 2)
                    {//��Ҫ�ص�StateΪ�޸�
                        string strCon = "GOFID=" + valueFID + " and SAVE=1";
                        if (valueFID != valueTargetFID)
                        {//��Ҫ�ص�FID�뱣������Ҫ��FID��ͬ,�򽫸�Ҫ����־��¼���ж�Ӧ����״̬�޸�Ϊɾ��
                            MoData.v_LogTable.UpdateTable("update UpdateLog set STATE=3 where " + strCon, out exError);
                        }
                        else
                        {//��ɾ����Ҫ����־��¼���ж�Ӧ����
                            MoData.v_LogTable.UpdateTable("delete * from UpdateLog where " + strCon, out exError);
                        }

                        if (bAddLog == false) //(ֻ����һ��)����־��¼�������ӱ�������Ҫ�ض�Ӧ����״̬Ϊ�޸�
                        {
                            string strSQL = "insert into UpdateLog(GOFID,LAYERNAME,OID,STATE,SAVE) values(" + valueTargetFID.ToString() + ",'" + pOriginIDataset.Name + "'," + pTargetFeat.OID + ",2,1)";
                            MoData.v_LogTable.UpdateTable(strSQL, out exError);
                            bAddLog = true;
                        }
                    }
                    else
                    {//��Ҫ�ص�StateΪδ�仯,��������־��¼���ж�Ӧ����״̬Ϊɾ��
                        if (valueFID != valueTargetFID)
                        {//��Ҫ�ص�FID�뱣������Ҫ��FID��ͬ,������־��¼�������Ӹ�Ҫ��Ӧ����״̬Ϊɾ��
                            string strSQL = "insert into UpdateLog(GOFID,STATE,SAVE) values(" + valueFID.ToString() + ",3,1)";
                            MoData.v_LogTable.UpdateTable(strSQL, out exError);
                        }

                        if (bAddLog == false) //(ֻ����һ��)����־��¼�������ӱ�������Ҫ�ض�Ӧ����״̬Ϊ�޸�
                        {
                            string strSQL = "insert into UpdateLog(GOFID,LAYERNAME,OID,STATE,SAVE) values(" + valueTargetFID.ToString() + ",'" + pOriginIDataset.Name + "'," + pTargetFeat.OID + ",2,1)";
                            MoData.v_LogTable.UpdateTable(strSQL, out exError);
                            bAddLog = true;
                        }
                    }
                }
            }
        }
        #endregion

        /// <summary>
        /// �ж��ںϺ�����ĸ�Ҫ�ص�����
        /// </summary>
        /// <returns></returns>
        private bool JudgeCondition()
        {

            return true;
        }
        //======================================================================================== 
        #endregion

        public override void OnCreate(Plugin.Application.IApplicationRef hook)
        {
            if (hook == null) return;
            myHook = hook as Plugin.Application.IAppGISRef;
            if (myHook.MapControl == null) return;

            //�������ѡ��ť
            _cmd = new ControlsClearSelectionCommandClass();
            _cmd.OnCreate(myHook.MapControl);

            myHook.ArcGisMapControl.OnKeyDown += new IMapControlEvents2_Ax_OnKeyDownEventHandler(ArcGisMapControl_OnKeyDown);
        }

        private void ArcGisMapControl_OnKeyDown(object sender, IMapControlEvents2_OnKeyDownEvent e)
        {
            if (this.Enabled && e.shift == 2 && e.keyCode == 77)
            {
                this.OnClick();
            }
        }


        //==========================================================================================================
        //��ʤ��   2009-08-20   ���
        /// <summary>
        /// ��MAP�ϵõ�ѡ���Ҫ������
        /// <para>��ʽ�磺name oid������NAME��ָ��Ҫ��������֣�OIDָ����Ҫ�ص�ID</para>
        /// <remarks>��Ҫ������������ϵ���</remarks>
        /// </summary>
        private Hashtable GetData_tree()
        {

            Hashtable table = new Hashtable();//����һ��KEY VALUE
            IEnumFeature f = myHook.MapControl.Map.FeatureSelection as IEnumFeature;//�е���ѡ���Ҫ�ص����ݼ�
            f.Reset();
            IFeature feature = f.Next();
            string name = "";
            string oid = "";
            #region ͨ���������KEYΪ��Ҫ��������VALUEΪ��OID�����Ҫ��������KEY�У���ô���ۼӵ�֮ǰ��ֵ����
            while (feature != null)
            {
                name = feature.Class.AliasName;
                oid = feature.OID.ToString();
                if (table.Count == 0)
                {
                    table.Add(name, oid);
                }
                else
                {
                    if (table.ContainsKey(name))
                    {
                        string temp = table[name].ToString() + " " + oid;
                        table[name] = temp;
                    }
                    else
                    {
                        table.Add(name, oid);
                    }
                }
                feature = f.Next();

            }
            #endregion

            int K_count = table.Count;//�ж������ֵ��ǲ��ǿյ�
            if (K_count == 0)
            {
                table = null;
            }
            return table;
        }
        //=================================================================================================================


        //=================================================================================================================
        //��ʤ��   2009-08-20  ���
        /// <summary>
        /// ��Ҫ��������ʾ��ӦOID������ֵ ��������ʾ�ؼ��ṩ����
        /// <para>key=name+oid value=field +value</para>
        /// <remarks>���������ֵ䣬��Ҫ��������ƺ͵�����OID��Ϊһ��KEY��Ȼ���OID����ֶκ�ֵȡ��������ֵ</remarks>
        /// </summary>
        /// <returns></returns>
        private Hashtable GetData_View()
        {

            Hashtable table = new Hashtable();
            IEnumFeature f_dataset = myHook.MapControl.Map.FeatureSelection as IEnumFeature;//ȡ�õ�ͼ�ؼ��ϱ�ѡ���Ҫ�ؼ�

            f_dataset.Reset();
            IFeature feature = f_dataset.Next();//ȡ����һ��Ҫ��
            string name = "";
            string oid = "";
            #region ������ӦҪ�����¶�ӦOIDҪ�صļ�¼ֵ
            while (feature != null)
            {
                name = feature.Class.AliasName;
                oid = feature.OID.ToString();

                string key = name + oid;//ʹ��KEYֵ,��Ҫ��������Ƽ��ϵ�����OID��ϳ�һ��KEY
                string Value = "";//ʹ��VALUE

                string shape = "";//ȷ��Ҫ�ص�SHAPE��ʲô
                #region �õ�Ҫ�ص�SHAPE���ͣ�ע�ǣ��棬�ߣ���
                if (feature.FeatureType == esriFeatureType.esriFTAnnotation)
                {
                    shape = "ע��";
                }
                else
                {
                    IGeometry geometry = feature.Shape;//�õ�Ҫ�صļ���ͼ��

                    switch (geometry.GeometryType.ToString())//ȷ�����ļ���Ҫ������
                    {
                        case "esriGeometryPolygon":
                            shape = "��";
                            break;
                        case "esriGeometryPolyline":
                            shape = "��";
                            break;
                        case "esriGeometryPoint":
                            shape = "��";
                            break;
                    }
                }
                #endregion

                IFields fields = feature.Fields;
                int k = fields.FieldCount;
                #region �˱�����Ҫ�������õ�һ����Ҫ�صļ�¼������������ϳ�һ��VALUE
                for (int s = 0; s < k; s++)
                {

                    IField field = fields.get_Field(s);
                    string str = "";
                    string f_value = feature.get_Value(s).ToString();//�õ���Ӧ��ֵ
                    if (field.Name.ToLower() == "shape")
                    {
                        str = "SHAPE " + shape;//�õ���Ӧ��SHAPE����
                    }
                    else if (field.Name.ToLower() == "element")
                    {
                        str = "Element blob";
                    }
                    else
                    {
                        if (f_value != string.Empty)
                        {
                            str = field.Name + " " + f_value;
                        }
                        else
                        {
                            str = field.Name + " " + "null";
                        }
                    }
                    Value += str + ",";
                }
                #endregion
                #region ��KEY��VALUE���뵽�����ֵ䵱�У��Ա�����ʹ��
                if (table.Count == 0)
                {
                    table.Add(key, Value);
                }
                else
                {
                    table.Add(key, Value);
                }
                #endregion
                feature = f_dataset.Next();

            }
            #endregion

            if (table.Count == 0)
            {
                table = null;
            }
            return table;
        }
        //=================================================================================================================
    }
}
