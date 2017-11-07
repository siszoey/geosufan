using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Xml;

using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.SystemUI;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.esriSystem;
using System.Windows.Forms;

namespace GeoEdit
{
    /// <summary>
    /// �����༭
    /// </summary>
    public class ControlsStartEdit : Plugin.Interface.CommandRefBase
    {
        private Plugin.Application.IAppGISRef m_Hook;
        //private IWorkspace m_CurWorkspace;                                     //���¿������ݹ����ռ�
        private Dictionary<IWorkspace, List<IFeatureLayer>> m_AllLayers;     //mapcontrol������ͼ��
        public ControlsStartEdit()
        {

            base._Name = "GeoEdit.ControlsStartEdit";
            base._Caption = "�����༭";
            base._Tooltip = "�����༭";
            base._Checked = false;
            base._Visible = true;
            base._Enabled = true;
            base._Message = "�����༭";
        }

        /// <summary>
        /// ȷ���Ƿ������ݴ򿪣���ȷ������Ť����ʾ
        /// </summary>
        public override bool Enabled
        {
            get
            {
                try
                {
                    if (m_Hook == null) return false;
                    if (m_Hook.MapControl == null) return false;
                    if (m_Hook.CurrentThread != null) return false;
                    //��ȡmapcontrol������ͼ��
                    m_AllLayers = ModPublic.GetAllLayersFromMap(m_Hook.MapControl);
                    if (m_AllLayers == null) return false;

                    bool bres = true;
                    //for (int i = 0; i < m_Hook.MapControl.Map.LayerCount; i++)
                    //{
                    //    ILayer pLay = m_Hook.MapControl.Map.get_Layer(i);
                    //    if (pLay is IGroupLayer && pLay.Name == "�����ޱ�����")
                    //    {
                    //        bres = true;
                    //        ICompositeLayer pCompositeLayer = pLay as ICompositeLayer;
                    //        if (pCompositeLayer.Count > 0)
                    //        {
                    //            IFeatureLayer pFeatLay = pCompositeLayer.get_Layer(0) as IFeatureLayer;
                    //            if (pFeatLay != null)
                    //            {
                    //                IDataset pDataset = pFeatLay.FeatureClass as IDataset;
                    //                m_CurWorkspace = pDataset.Workspace;
                    //                IWorkspaceEdit pWorkspaceEdit = pDataset.Workspace as IWorkspaceEdit;
                    //                if (pWorkspaceEdit.IsBeingEdited())
                    //                {
                    //                    bres = false;
                    //                }
                    //            }
                    //        }
                    //        break;
                    //    }
                    //}

                    foreach (KeyValuePair<IWorkspace, List<IFeatureLayer>> keyValue in m_AllLayers)
                    {
                        IWorkspaceEdit pWorkspaceEdit = keyValue.Key as IWorkspaceEdit;
                        if (pWorkspaceEdit.IsBeingEdited() == true)
                        {
                            bres = false;
                            break;
                        }
                    }

                    return bres;
                }
                catch
                {
                    return false;
                }
            }
        }

        public override string Message
        {
            get
            {
                Plugin.Application.IAppFormRef pAppFormRef = m_Hook as Plugin.Application.IAppFormRef;
                if (pAppFormRef != null)
                {
                    pAppFormRef.OperatorTips = base._Message;
                }
                return base._Message;
            }
        }

        public override void ClearMessage()
        {
            Plugin.Application.IAppFormRef pAppFormRef = m_Hook as Plugin.Application.IAppFormRef;
            if (pAppFormRef != null)
            {
                pAppFormRef.OperatorTips = string.Empty;
            }
        }

        public override void OnClick()
        {
            IWorkspace pCurWorkspace = null;// m_CurWorkspace;
            //�������ռ�Ϊ���ʱ���ṩѡ��Ի���
            if (m_AllLayers == null || m_AllLayers.Count == 0) return;
            if (m_AllLayers.Count > 1)
            {
                FrmStartEdit frmStarEdit = new FrmStartEdit();
                frmStarEdit.AllEditInfo = m_AllLayers;
                frmStarEdit.ShowDialog();
                if (frmStarEdit.DialogResult == DialogResult.OK)
                {
                    pCurWorkspace = frmStarEdit.SelectWorkspace;
                }
                else
                {
                    return;
                }
            }
            else
            {
                foreach (IWorkspace pWorkspace in m_AllLayers.Keys)
                {
                    pCurWorkspace = pWorkspace;
                }
            }

            if (pCurWorkspace == null) return;

            //================================================================
            //chenyafei  20110105  add���ڱ༭ǰע��汾
            //ֻ���SDE����
            if (pCurWorkspace.WorkspaceFactory.WorkspaceType != esriWorkspaceType.esriRemoteDatabaseWorkspace)
            {
                //������SDE���ݣ�������༭
                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "�����SDE���ݽ��б༭");
                return;
            }
            //ֻ���ע��汾������
            IEnumDataset pEnumDataset = pCurWorkspace.get_Datasets(esriDatasetType.esriDTFeatureDataset);
            if (pEnumDataset == null) return;

            IDataset pFeaDt = pEnumDataset.Next();
           
            while (pFeaDt != null)
            {
                if (pFeaDt.Name.ToUpper().EndsWith("_GOH"))
                {
                    pFeaDt = pEnumDataset.Next();
                    continue;
                }
                IVersionedObject pVerObj = pFeaDt as IVersionedObject;
                if (!pVerObj.IsRegisteredAsVersioned)
                {
                    //ע��汾
                    if (SysCommon.Error.ErrorHandle.ShowFrmInformation("��", "��", "�����ݻ�δע��汾���Ƿ�ע��汾�Ա���б༭��"))
                    {
                        pVerObj.RegisterAsVersioned(true);
                        break;
                    }
                    else
                    {
                        return;
                    }
                }
                //else
                //{
                //    pVerObj.RegisterAsVersioned(false);
                //}
                pFeaDt = pEnumDataset.Next();
            }

            //==================================================================

            IWorkspaceEdit pWorkspaceEdit = pCurWorkspace as IWorkspaceEdit;
            if (!pWorkspaceEdit.IsBeingEdited())
            {
                try
                {
                    pWorkspaceEdit.StartEditing(true);
                    pWorkspaceEdit.EnableUndoRedo();
                }
                catch (Exception eError)
                {
                    //******************************************
                    //guozheng added System Exception log
                    if (SysCommon.Log.Module.SysLog == null)
                        SysCommon.Log.Module.SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
                    SysCommon.Log.Module.SysLog.Write(eError);
                    //******************************************
                    SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "���ܱ༭�ù����ռ�,�����Ƿ�Ϊֻ���������û�ռ�ã�");
                    return;
                }
            }

            MoData.v_CurWorkspaceEdit = pWorkspaceEdit;

            //�������༭��ͼ��ӵ�ͼ����������
            if (Plugin.ModuleCommon.DicCommands.ContainsKey("GeoEdit.LayerControl"))
            {
                LayerControl pLayerControl = Plugin.ModuleCommon.DicCommands["GeoEdit.LayerControl"] as LayerControl;
                if (pLayerControl != null)
                {
                    pLayerControl.GetLayers();
                }
            }
            //******************************************************************
            //guozheng 2010-11-4 added �����༭�ɹ����ȡ��ǰ�����ݿ�汾
            Exception ex = null;
            clsUpdataEnvironmentOper UpOper = new clsUpdataEnvironmentOper();
            UpOper.HisWs = pCurWorkspace;
            MoData.DBVersion = UpOper.GetVersion(out ex);
            if (ex != null)
            {
                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "��ȡ���ݿ�汾ʧ�ܡ�\nԭ��" + ex.Message);
            }
            //******************************************************************
            ////����־��¼��,��������
            //Exception exError = null;
            //XmlNode DBNode = m_Hook.DBXmlDocument.SelectSingleNode(".//���ݿ⹤��");
            //XmlElement DBElement = DBNode as XmlElement;
            //string strLogMdbPath = DBElement.GetAttribute("·��") + "\\Log\\������־.mdb";
            //MoData.v_LogTable = new SysCommon.DataBase.SysTable();
            //MoData.v_LogTable.SetDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + strLogMdbPath+";Mode=Share Deny None;Persist Security Info=False", SysCommon.enumDBConType.OLEDB, SysCommon.enumDBType.ACCESS, out exError);
            //MoData.v_LogTable.StartTransaction();

            //ί��������ر��¼�
            Plugin.Application.IAppFormRef pAppFormRef = m_Hook as Plugin.Application.IAppFormRef;
            pAppFormRef.MainForm.FormClosing += new FormClosingEventHandler(MainForm_FormClosing);
        }

        //���˳�ϵͳǰ�����ڱ༭������Ӧ��ʾ
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MoData.v_CurWorkspaceEdit != null)
            {
                bool bHasEdits = false;
                bool bSave = false;
                MoData.v_CurWorkspaceEdit.HasEdits(ref bHasEdits);
                if (bHasEdits == true)
                {
                    bSave = SysCommon.Error.ErrorHandle.ShowFrmInformation("��", "��", "ͼ���ѽ��й��༭���Ƿ���Ҫ���棿");
                }

                MoData.v_CurWorkspaceEdit.StopEditing(bSave);
                MoData.v_CurWorkspaceEdit = null;
                ////������־��¼����޸�
                //MoData.v_LogTable.EndTransaction(bSave);
                //MoData.v_LogTable.CloseDbConnection();
                //MoData.v_LogTable = null;

                m_Hook.CurrentTool = "";
                m_Hook.MapControl.CurrentTool = null;
                m_Hook.MapControl.Map.ClearSelection();
                m_Hook.MapControl.ActiveView.Refresh();
            }
        }

        public override void OnCreate(Plugin.Application.IApplicationRef hook)
        {
            if (hook == null) return;
            m_Hook = hook as Plugin.Application.IAppGISRef;
        }
    }
}
