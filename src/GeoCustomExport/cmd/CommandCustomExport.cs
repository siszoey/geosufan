using System;
using System.Collections.Generic;
using System.Text;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Geodatabase;

namespace GeoCustomExport
{
    public class CommandCustomExport : Plugin.Interface.CommandRefBase
    {
        private Plugin.Application.IAppGISRef _AppHk;
        private Plugin.Application.IAppFormRef m_pAppForm;
        public CommandCustomExport()
        {
            base._Name = "GeoCustomExport.CommandCustomExport";
            base._Caption = "�Զ��嵼��";
            base._Tooltip = "�Զ��嵼��";
            base._Checked = false;
            base._Visible = true;
            base._Enabled = false;
            base._Message = "�Զ��嵼��";

        }

        /// <summary>
        /// ͼ���д�������ʱ����״̬Ϊ����ʱ�ſ���
        /// </summary>
        public override bool Enabled
        {
            get
            {
                return true;
            }
        }
        public override string Message
        {
            get
            {
                Plugin.Application.IAppFormRef pAppFormRef = _AppHk as Plugin.Application.IAppFormRef;
                if (pAppFormRef != null)
                {
                    pAppFormRef.OperatorTips = base._Message;
                }
                return base._Message;
            }
        }

        public override void ClearMessage()
        {
            Plugin.Application.IAppFormRef pAppFormRef = _AppHk as Plugin.Application.IAppFormRef;
            if (pAppFormRef != null)
            {
                pAppFormRef.OperatorTips = string.Empty;
            }
        }

        public override void OnClick()
        {
            if (_AppHk == null) return;
            if (this.WriteLog)
            {
                Plugin.LogTable.Writelog(Caption);//ygc 2012-9-14 д��־
            }
            showCustomExport();
        }

        public override void OnCreate(Plugin.Application.IApplicationRef hook)
        {
            if (hook == null) return;
            _AppHk = hook as Plugin.Application.IAppGISRef ;
            if (_AppHk.MapControl == null) return;


            m_pAppForm = _AppHk as Plugin.Application.IAppFormRef;
        }
        private void showCustomExport()
        {
            frmCustomExport fmCE = new frmCustomExport();
            fmCE.ShowDialog();
        }

    }
}
