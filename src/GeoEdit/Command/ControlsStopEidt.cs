using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
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
    /// 
    /// </summary>
    public class ControlsStopEidt : Plugin.Interface.CommandRefBase
    {
        private Plugin.Application.IAppGISRef m_Hook;
        public ControlsStopEidt()
        {
            base._Name = "GeoEdit.ControlsStopEidt";
            base._Caption = "�����༭";
            base._Tooltip = "�����༭";
            base._Checked = false;
            base._Enabled = false;
            base._Visible = true;

            base._Message = "�����༭";
        }

        /// <summary>
        /// ȷ���Ƿ��д򿪵����ݣ����ؼ���ʾ
        /// </summary>
        public override bool Enabled
        {
            get
            {
                if (m_Hook == null) return false;
                if (m_Hook.MapControl == null) return false;
                if (MoData.v_CurWorkspaceEdit == null) return false;
                return true;
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
            if (!MoData.v_CurWorkspaceEdit.IsBeingEdited()) return;
            bool bHasEdits = false;
            DateTime SaveTime = DateTime.Now;////////////////////////////////////�����༭��ʱ��
            Dictionary<string, Dictionary<int, List<IRow>>> ChangeFea = null;///���±仯��Ҫ��
            MoData.v_CurWorkspaceEdit.HasEdits(ref bHasEdits);
            bool bSave = false;
            if (bHasEdits == true)   //��������˱༭����ʾ�Ƿ񱣴�
            {
                if (SysCommon.Error.ErrorHandle.ShowFrmInformation("��", "��", "�Ƿ�Ա༭���б���") == true)
                {
                    bSave = true;
                }
            }

            if (bSave)
            {
                //bool bLock = true;                      //true�����
                //bool bebortIfConflict = false;           //��⵽��ͻʱ���汾Э���Ƿ�ֹͣ��trueֹͣ��false��ֹͣ
                bool bChildWin = false;                 //true�滻��һ���汾����ͻ�汾��,false����һ���汾����ͻ�汾��
                //bool battributeCheck = false;          //false��Ϊtrue��ֻ���޸�ͬһ������ʱ�ż�����ͻ
                bool beMerge = true;                   //���ڲ�����ͻ��Ҫ���Ƿ��ں�

                Exception eError = null;

                frmConflictSet pfrmConflictSet = new frmConflictSet();
                if (pfrmConflictSet.ShowDialog() == DialogResult.OK)
                {
                    bChildWin = pfrmConflictSet.BECHILDWIM;
                    beMerge = pfrmConflictSet.BEMERGE;
                }

                string pDefVerName = "";              //Ĭ�ϰ汾����
                Dictionary<string, Dictionary<int, List<int>>> conflictFeaClsDic = null;  //������ͻ��Ҫ������Ϣ
                Dictionary<string, Dictionary<int, List<IRow>>> feaChangeDic = null;       //���±仯��Ҫ��
                //Dictionary<string, Dictionary<int, List<IRow>>> feaChangeSaveDic = null;   //����༭����±仯��Ҫ�� 

                ClsVersionReconcile pClsVersionReconcile = new ClsVersionReconcile(MoData.v_CurWorkspaceEdit);
                //���Ĭ�ϰ汾����
                pDefVerName = pClsVersionReconcile.GetDefautVersionName(out eError);
                if (eError != null || pDefVerName == "")
                {
                    SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", eError.Message);
                    return;
                }

                //���а汾Э��ʱ������ͻ��Ҫ������Ϣ
                conflictFeaClsDic = pClsVersionReconcile.ReconcileVersion(pDefVerName, bChildWin, beMerge, out eError);
                if (eError != null)
                {
                    SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", eError.Message);
                    return;
                }

                //��÷������±仯��Ҫ����Ϣ
                feaChangeDic = pClsVersionReconcile.GetModifyClsInfo(out eError);
                if (eError != null)
                {
                    SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", eError.Message);
                    return;
                }
                ChangeFea = feaChangeDic;

            }

            MoData.v_CurWorkspaceEdit.StopEditing(bSave);
            if (bSave)
            {
                //������־��¼���
                //*******************************************************************************************
                //guozheng 2010-11-4 added
                //�༭��ɣ���ͻ������ɺ�д�������־��
                #region �༭��ɣ���ͻ������ɺ�д�������־��
                Exception ex = null;
                if (ChangeFea != null)
                {
                    //////////Զ�̸�����־�������
                    clsUpdataEnvironmentOper UpLogOper = new clsUpdataEnvironmentOper();
                    bool SaveLOG = true;
                    UpLogOper.HisWs = (IWorkspace)MoData.v_CurWorkspaceEdit;
                    foreach (KeyValuePair<string, Dictionary<int, List<IRow>>> item in ChangeFea)
                    {
                        string sLayerName = item.Key;
                        Dictionary<int, List<IRow>> FeatureDic = item.Value;
                        if (FeatureDic != null)
                        {
                            foreach (KeyValuePair<int, List<IRow>> item2 in FeatureDic)
                            {
                                int iState = item2.Key;
                                List<IRow> ListFea = item2.Value;
                                for (int count = 0; count < ListFea.Count; count++)
                                {
                                    IRow getRow = ListFea[count];
                                    UpLogOper.RecordLOG(getRow, iState, SaveTime, iState, sLayerName, out ex);
                                    if (ex != null)
                                    {
                                        SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "���±༭��¼ʧ�ܡ�\nԭ��" + ex.Message);
                                        SaveLOG = false;
                                        break;
                                    }
                                }
                                if (SaveLOG == false) break;
                            }
                        }
                    }
                    if (SaveLOG)
                    {
                        UpLogOper.WriteDBVersion(MoData.DBVersion, SaveTime, out ex);
                        if (ex != null)
                        {
                            SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "�汾��Ϣд��ʧ�ܣ�\nԭ��" + ex.Message);
                            // return;
                        }
                    }
                }
                #endregion
                //*******************************************************************************************
                //д��汾��Ϣ
                //*******************************************************************************************
            }

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

        public override void OnCreate(Plugin.Application.IApplicationRef hook)
        {
            if (hook == null) return;
            m_Hook = hook as Plugin.Application.IAppGISRef;
        }
    }
}
