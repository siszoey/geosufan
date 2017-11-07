using System;
using System.Collections.Generic;
using System.Text;
using GeoDataCenterFunLib;

namespace GeoDBConfigFrame
{
    class BindDocSource : Plugin.Interface.CommandRefBase
    {
        private Plugin.Application.IAppPrivilegesRef m_Hook;
         public BindDocSource()
        {
            base._Name = "GeoDBConfigFrame.BindDocSource";
            base._Caption = "�ĵ����ݹҽ�";
            base._Tooltip = "�ĵ����ݹҽ�";
            base._Checked = false;
            base._Visible = true;
            base._Enabled = true;
            base._Message = "�ĵ����ݹҽ�";
        }

        public override void OnClick()
        {

            if (m_Hook != null)
            {
                LogFile log = new LogFile(m_Hook.tipRichBox, m_Hook.strLogFilePath);
                if (log != null)
                {
                    log.Writelog("�ĵ���ҽ�");

                }
            }
            frmBindSource frm = new frmBindSource(1);
            frm.Show();
        }

        public override void OnCreate(Plugin.Application.IApplicationRef hook)
        {
            if (hook == null)
                return;
            m_Hook = hook as Plugin.Application.IAppPrivilegesRef;
        }
    }
    }
