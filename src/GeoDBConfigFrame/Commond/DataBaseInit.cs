using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using GeoDataCenterFunLib;
//���ݿ��ʼ��
namespace GeoDBConfigFrame
{
    public class DataBaseInit : Plugin.Interface.CommandRefBase
    {
        private Plugin.Application.IAppPrivilegesRef m_Hook;
        public DataBaseInit()
        {
            base._Name = "GeoDBConfigFrame.DataBaseInit";
            base._Caption = "���ݿ��ʼ��";
            base._Tooltip = "���ݿ��ʼ��";
            base._Checked = false;
            base._Visible = true;
            base._Enabled = true;
            base._Message = "���ݿ��ʼ��";
        }

        public override void OnClick()
        {
            if (m_Hook != null)
            {
                LogFile log = new LogFile(m_Hook.tipRichBox, m_Hook.strLogFilePath);
                if (log != null)
                {
                    log.Writelog("���ݿ��ʼ��");

                }
            }
            /*       Exception eError;
                   AddGroup frmGroup = new AddGroup();
                   if (frmGroup.ShowDialog() == DialogResult.OK)
                   {
                       ModuleOperator.DisplayRoleTree("", m_Hook.RoleTree, ref ModData.gisDb, out eError);
                       if (eError != null)
                       {
                           ErrorHandle.ShowFrmError("��ʾ", eError.Message);
                           return;
                       }
                   }
             */
        }

        public override void OnCreate(Plugin.Application.IApplicationRef hook)
        {
            if (hook == null)
                return;
            m_Hook = hook as Plugin.Application.IAppPrivilegesRef;
        }
    }
}
