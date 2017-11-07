using System;
using System.Collections.Generic;
using System.Text;
using GeoDataCenterFunLib;
using System.Windows.Forms;
using SysCommon.DataBase;

namespace GeoDataManagerFrame
{
   public class LogManager : Plugin.Interface.CommandRefBase
    {
        private Plugin.Application.IAppFormRef m_Hook;

       public LogManager()
        {
            base._Name = "GeoDataManagerFrame.LogManager";
            base._Caption = "��־����";
            base._Tooltip = "��־����";
            base._Checked = false;
            base._Visible = true;
            base._Enabled = true;
            base._Message = "��־����";
        }
       string  m_strLogFilePath=Application.StartupPath + "\\..\\Log\\DataCenterLog.txt";
        public override void OnClick()
        {
            if (m_Hook == null)
                return;
           
            frmDetailLog frm = new frmDetailLog(m_strLogFilePath);
            frm.Show(m_Hook.MainForm);
            if (this.WriteLog)
            {
                Plugin.LogTable.Writelog(Caption);//xisheng 2011.07.09 ������־
            }

        }

        public override void OnCreate(Plugin.Application.IApplicationRef hook)
        {
            if (hook == null)
                return;
            m_Hook = hook as Plugin.Application.IAppFormRef;
        }
    }
}
