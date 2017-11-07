using System;
using System.Windows.Forms;

namespace GeoDBIntegration
{
    /// <summary>
    /// yjl  20110716 add  content�����ݿⱸ��
    /// </summary>
    public class ControlsDBbackup : Plugin.Interface.CommandRefBase
    {
        private Plugin.Application.IAppDBIntegraRef m_Hook;
        private Plugin.Application.IAppFormRef _hook;
        public ControlsDBbackup()
        {
            base._Name = "GeoDBIntegration.ControlsDBbackup";
            base._Caption = "���ݿⱸ��";
            base._Tooltip = "���ݿⱸ��";
            base._Checked = false;
            base._Visible = true;
            base._Enabled = true;
            base._Message = "���ݿⱸ��";
        }

        public override bool Enabled
        {
            get 
            {
                ////added by chulili 20110624 ������������Դ�������  �˵�������
                //if (!(_hook.MainForm.Controls[0] is UserControlDBIntegra))
                //{
                //    return false;
                //}
                ////added by chulili 20110705���治�ɼ����˵�������
                //if (!_hook.MainForm.Controls[0].Visible)
                //{
                //    return false;
                //}

                //end add
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
            try
            {
                frmOraBk_Rc fmBak = new frmOraBk_Rc();
                if (this.WriteLog)
                {
                    Plugin.LogTable.Writelog(Caption); //ygc 2012-9-14 д��־
                }
                fmBak.ShowDialog();
                //frmAppendFeafrmAppendFea fmBak = new frmAppendFea();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "��ʾ", MessageBoxButtons.OK,
                         MessageBoxIcon.Information);
 
            }
        }

        public override void OnCreate(Plugin.Application.IApplicationRef hook)
        {
            if (hook == null) return;
            m_Hook = hook as Plugin.Application.IAppDBIntegraRef;
            _hook = hook as Plugin.Application.IAppFormRef;
        }

    }
}
