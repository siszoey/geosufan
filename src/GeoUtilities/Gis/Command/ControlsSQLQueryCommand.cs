using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using ESRI.ArcGIS.Carto;
using System.Windows.Forms;
using ESRI.ArcGIS.Controls;

namespace GeoUtilities
{
    public class ControlsSQLQueryCommand : Plugin.Interface.CommandRefBase
    {

        private Plugin.Application.IAppArcGISRef _AppHk;

        public ControlsSQLQueryCommand()
        {
            base._Name = "GeoUtilities.ControlsSQLQueryCommand";
            base._Caption = "SQL��ѯ";
            base._Tooltip = "SQL��ѯ";
            base._Checked = false;
            base._Visible = true;
            base._Enabled = true;
            base._Message = "SQL��ѯ";
            //base._Image = "";
            //base._Category = "";
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
        public override bool Enabled
        {
            get
            {
                //������ͼ����ϱ�ͼ�㡢���ݲ����������ڽ���ʱ������
                if (_AppHk == null) return false;
                if (_AppHk.CurrentControl is ISceneControl) return false;  //Ϊ��ֻ��Ч��2ά�ؼ�
                if (_AppHk.MapControl == null) return false;
                if (_AppHk.MapControl.Map.LayerCount == 0) return false;
                return true;
            }
        }
        public override void OnClick()
        {
            if (_AppHk == null) return;
            if (_AppHk.MapControl == null) return;
            IMap pMap = _AppHk.MapControl.Map;
            FrmSQLQuery frmSQL = new FrmSQLQuery(_AppHk.MapControl,false);
            frmSQL.ShowDialog();//.Show();
        }

        public override void OnCreate(Plugin.Application.IApplicationRef hook)
        {
            if (hook == null) return;
            _AppHk = hook as Plugin.Application.IAppArcGISRef;
            if (_AppHk.MapControl == null) return;
        }
    }
}
