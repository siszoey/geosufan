using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geometry;

namespace GeoEdit
{
    /// <summary>
    /// ����༭
    /// </summary>
    public class ControlsSaveEdit : Plugin.Interface.CommandRefBase
    {
        private Plugin.Application.IAppGISRef m_Hook;
        public ControlsSaveEdit()
        {
            base._Name = "GeoEdit.ControlsSaveEdit";
            base._Caption = "����༭";
            base._Tooltip = "����༭";
            base._Checked = false;
            base._Visible = true;
            base._Enabled = false;
            base._Message = "����༭";

        }

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
            MoData.v_CurWorkspaceEdit.HasEdits(ref bHasEdits);

            DateTime SaveTime = DateTime.Now;//////////////����༭ʱ��

            if (bHasEdits == true)
            {
                Exception eError = null;
                //bool bLock = true;                      //true�����
                //bool bebortIfConflict = false;           //��⵽��ͻʱ���汾Э���Ƿ�ֹͣ��trueֹͣ��false��ֹͣ
                bool bChildWin = false;                 //true�滻��һ���汾����ͻ�汾��,false����һ���汾����ͻ�汾��
                //bool battributeCheck = false;          //false��Ϊtrue��ֻ���޸�ͬһ������ʱ�ż�����ͻ
                bool beMerge = true;                   //���ڲ�����ͻ��Ҫ���Ƿ��ں�
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

                MoData.v_CurWorkspaceEdit.StopEditing(true);
                //*******************************************************************************************
                //guozheng 2010-11-4 added
                //�༭��ɣ���ͻ������ɺ�д�������־��
                #region �༭��ɣ���ͻ������ɺ�д�������־��
                bool SaveLOG = true;
                if (feaChangeDic != null)
                {
                    //////////Զ�̸�����־�������
                    clsUpdataEnvironmentOper UpLogOper = new clsUpdataEnvironmentOper();
                    UpLogOper.HisWs = (IWorkspace)MoData.v_CurWorkspaceEdit;
                    foreach (KeyValuePair<string, Dictionary<int, List<IRow>>> item in feaChangeDic)
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
                                    UpLogOper.RecordLOG(getRow, iState, SaveTime, MoData.DBVersion, sLayerName, out eError);
                                    if (eError != null)
                                    {
                                        SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "���±༭��¼ʧ�ܡ�\nԭ��" + eError.Message);
                                        SaveLOG = false;
                                        break;
                                    }
                                }
                                if (SaveLOG == false) break;
                            }
                        }
                    }
                    UpLogOper.WriteDBVersion(MoData.DBVersion, SaveTime, out eError);
                    if (eError != null)
                    {
                        SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "���°汾��Ϣд��ʧ�ܡ�\nԭ��" + eError.Message);
                    }

                }
                #endregion
                MoData.v_CurWorkspaceEdit.StartEditing(true);
                MoData.v_CurWorkspaceEdit.EnableUndoRedo();
                if (bHasEdits == true && SaveLOG == true)
                {
                    //*******************************************************************************************
                    //guozheng 2010-11-4 added
                    //����ɹ���ˢ�°汾��Ϣ
                    Exception ex = null;
                    //////////Զ�̸�����־�������
                    clsUpdataEnvironmentOper UpLogOper = new clsUpdataEnvironmentOper();
                    UpLogOper.HisWs = (IWorkspace)MoData.v_CurWorkspaceEdit;
                    // UpLogOper.WriteDBVersion(MoData.DBVersion, SaveTime, out ex);
                    MoData.DBVersion = UpLogOper.GetVersion(out ex);
                    if (ex != null)
                    {
                        SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "��ȡ���ݿ�汾ʧ�ܡ�\nԭ��" + ex.Message);
                    }
                    //*******************************************************************************************
                }

                ////����༭�󣬷������±仯��Ҫ����Ϣ
                //if (bChildWin == false)
                //{
                //    feaChangeSaveDic = pClsVersionReconcile.GetPureModifySaveInfo(feaChangeDic, conflictFeaClsDic, out eError);
                //    if (eError != null)
                //    {
                //        SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", eError.Message);
                //        return;
                //    }
                //}
                //else
                //{
                //    feaChangeSaveDic = feaChangeDic;
                //}


            }

            if (MoData.v_LogTable != null)
            {
                //������־��¼����޸�
                MoData.v_LogTable.EndTransaction(true);
                MoData.v_LogTable.CloseDbConnection();
                MoData.v_LogTable = null;
            }

            m_Hook.MapControl.ActiveView.Refresh();
        }

        public override void OnCreate(Plugin.Application.IApplicationRef hook)
        {
            if (hook == null) return;
            m_Hook = hook as Plugin.Application.IAppGISRef;
        }
    }
}
