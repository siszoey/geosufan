using System;
using System.Collections.Generic;
using System.Text;
using ESRI.ArcGIS.Carto;
using System.Windows.Forms;
using ESRI.ArcGIS.Controls;

/*-------------------------------------------------------------------
 added by xisheng 20110730 ��Χ��λ�˵��ļ� ControlsExtentLocation.cs
 -------------------------------------------------------------------*/
namespace GeoDataCenterFunLib
{
    public class ControlsExtentLocation : Plugin.Interface.CommandRefBase
    {
        private Plugin.Application.IAppGisUpdateRef _AppHk;
        private Plugin.Application.IAppFormRef m_pAppForm;

        public ControlsExtentLocation()
        {
            base._Name = "GeoDataCenterFunLib.ControlsExtentLocation";
            base._Caption = "��Χ��λ";
            base._Tooltip = "��Χ��λ";
            base._Checked = false;
            base._Visible = true;
            base._Enabled =false;
            base._Message = "��Χ��λ";
        }

        /// <summary>
        /// ͼ���д�������ʱ����״̬Ϊ����ʱ�ſ���
        /// </summary>
        public override bool Enabled
        {
            get
            {
                try
                {
                    if (_AppHk.MapControl.LayerCount == 0)
                    {
                        base._Enabled = false;
                        return false;
                    }

                    base._Enabled = true;
                    return true;
                }
                catch
                {
                    base._Enabled = false;
                    return false;
                }
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
            if (_AppHk == null) 
                return;
            frmRangeLocation frm = new frmRangeLocation(_AppHk.MapControl);
            frm.WriteLog = this.WriteLog;
            frm.Show();
            
        }

        public override void OnCreate(Plugin.Application.IApplicationRef hook)
        {
            if (hook == null) return;
            _AppHk = hook as Plugin.Application.IAppGisUpdateRef;
            if (_AppHk.MapControl == null) return;
            m_pAppForm = _AppHk as Plugin.Application.IAppFormRef;
        }



    }
}
