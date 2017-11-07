using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace GeoUserManager
{
    public class ControlUserManger : Plugin.Interface.CommandRefBase
    {
        private UserControl ucCtl = null;
        private Plugin.Application.IAppPrivilegesRef m_Hook;
        private Plugin.Application.IAppFormRef _hook;

        public ControlUserManger()
        {
            base._Name = "GeoUserManager.ControlUserManger";
            base._Caption = "Ȩ�޹���";
            base._Tooltip = "Ȩ�޹���";
            base._Checked = false;
            base._Visible = true;
            base._Enabled = true;
            base._Message = "Ȩ�޹���";
        }

        public override bool Enabled
        {
            get
            {
                if (ModData.v_AppPrivileges == null)
                    return false;
                if (_hook == null) return true;
                //����ʱ��ˢ��һ�¸Ŀռ��Ƿ���ʾ
                if (_hook.Visible == false && this.ucCtl != null)
                {
                    //this.ucCtl.Visible = false;
                }
                return true;
            }
        }

        public override string Message
        {
            get
            {
                Plugin.Application.IAppPrivilegesRef pAppFormRef = m_Hook as Plugin.Application.IAppPrivilegesRef;
                if (pAppFormRef != null)
                {
                    pAppFormRef.OperatorTips = base._Message;
                }
                return base._Message;
            }
        }

        public override void ClearMessage()
        {
            Plugin.Application.IAppPrivilegesRef pAppFormRef = m_Hook as Plugin.Application.IAppPrivilegesRef;
            if (pAppFormRef != null)
            {
                pAppFormRef.OperatorTips = string.Empty;
            }
        }

        public override void OnClick()
        {
            //changed by chulili20110718 �Ƚ�Ŀ¼������治�ɼ�������Ӧ�����л�
            for (int i = 0; i < _hook.MainForm.Controls.Count; i++)
            {
                if (_hook.MainForm.Controls[i].Name.Equals("UCDataSourceManger"))
                {
                    _hook.MainForm.Controls[i].Enabled = false;
                    break;
                }
            }
            //end added by chulili 
            if (ucCtl == null)
            {
                ucCtl = ModData.v_AppPrivileges.MainUserControl;
            }

            ucCtl.Enabled = true;//added by chulili 20110722 �л�������ʹ����
            ucCtl.Visible = true;
            UCRole pUCROLE = ucCtl as UCRole ;
            pUCROLE.RefreshDataList();
            //pUCROLE.RefreshDBsourcelist();//changed by chulili 20110705ɾ������ԴȨ��
            //_hook.MainForm.Controls.Add(ucCtl );
            _hook.MainForm.Controls.SetChildIndex(ucCtl, 0);
            //for (int i = 1; i < _hook.MainForm.Controls.Count; i++)
            //{
            //    if (_hook.MainForm.Controls[i].Name.Equals("UCDataSourceManger"))
            //    {
            //        _hook.MainForm.Controls[i].Visible = false;
            //        break;
            //    }
            //}
            if (this.WriteLog)
            {
                Plugin.LogTable.Writelog(Caption);//xisheng 2011.07.09 ������־
            }
        }

        public override void OnCreate(Plugin.Application.IApplicationRef hook)
        {
            if (hook == null) return;
            m_Hook = hook as Plugin.Application.IAppPrivilegesRef;
            _hook = hook as Plugin.Application.IAppFormRef;

        }

    }
}
