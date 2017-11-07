using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace GeoDBIntegration
{
    public class ControlDBIntergration : Plugin.Interface.CommandRefBase
    {
        private UserControl ucCtl = null;
        private Plugin.Application.IAppDBIntegraRef m_Hook;
        private Plugin.Application.IAppFormRef _hook;

        public ControlDBIntergration()
        {
            base._Name = "GeoDBIntegration.ControlDBIntergration";
            base._Caption = "����Դ����";
            base._Tooltip = "����Դ����";
            base._Checked = false;
            base._Visible = true;
            base._Enabled = true;
            base._Message = "����Դ����";
        }

        public override bool Enabled
        {
            get
            {
                if (ModuleData.v_AppDBIntegra == null)
                    return false;
                if (_hook == null) return true;
                //����ʱ��ˢ��һ�¸Ŀռ��Ƿ���ʾ
                if (_hook.Visible == false && this.ucCtl != null)
                {
                    this.ucCtl.Visible = false;
                }
                return true;
            }
        }

        public override string Message
        {
            get
            {
                Plugin.Application.IAppDBIntegraRef pAppFormRef = m_Hook as Plugin.Application.IAppDBIntegraRef;
                if (pAppFormRef != null)
                {
                  //  pAppFormRef.OperatorTips = base._Message;
                }
                return base._Message;
            }
        }

        public override void ClearMessage()
        {
            Plugin.Application.IAppDBIntegraRef pAppFormRef = m_Hook as Plugin.Application.IAppDBIntegraRef;
            if (pAppFormRef != null)
            {
               // pAppFormRef.OperatorTips = string.Empty;
            }
        }

        public override void OnClick()
        {
            //changed by chulili20110718 �Ƚ�Ŀ¼������治�ɼ�������Ӧ�����л�
            ModDBOperate.WriteLog("ControlDBIntergration_OnClick");
            for (int i = 0; i < _hook.MainForm.Controls.Count; i++)
            {
                if (_hook.MainForm.Controls[i].Name.Equals("UCDataSourceManger"))
                {
                    if (_hook.MainForm.Controls[i].Enabled)
                    {
                        _hook.MainForm.Controls[i].Enabled = false;
                    }
                    break;
                }
            }
            //end added by chulili
            
            if (ModuleData.v_AppDBIntegra == null)
                return;
            if (ucCtl == null)
            {
                ucCtl = ModuleData.v_AppDBIntegra.MainUserControl;
            }

            ucCtl.Visible = true;
            ucCtl.Enabled = true;
            _hook.MainForm.Controls.SetChildIndex(ucCtl, 0);
            if (this.WriteLog)
            {
                Plugin.LogTable.Writelog(Caption);//xisheng 2011.07.09 ������־
            }
            ModDBOperate.WriteLog("ControlDBIntergration_OnClick");
        }

        public override void OnCreate(Plugin.Application.IApplicationRef hook)
        {
            if (hook == null) return;
            m_Hook = hook as Plugin.Application.IAppDBIntegraRef;
            _hook = hook as Plugin.Application.IAppFormRef;


        }

    }
}
