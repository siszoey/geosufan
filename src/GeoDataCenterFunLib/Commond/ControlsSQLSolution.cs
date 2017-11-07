using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using ESRI.ArcGIS.Carto;
using System.Windows.Forms;
using ESRI.ArcGIS.Controls;

namespace GeoDataCenterFunLib
{
    public class ControlsSQLSolution : Plugin.Interface.CommandRefBase
    {
        private Plugin.Application.IAppArcGISRef _AppHk;
        FrmSQLSolution  frmSQL = null;
        public ControlsSQLSolution()
        {
            base._Name = "GeoDataCenterFunLib.ControlsSQLSolution";
            base._Caption = "��ѯ����";
            base._Tooltip = "��ѯ����";
            base._Checked = false;
            base._Visible = true;
            base._Enabled = true;
            base._Message = "��ѯ����";
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
          /*  get
            {
                //������ͼ����ϱ�ͼ�㡢���ݲ����������ڽ���ʱ������
                if (_AppHk.MapControl == null) return false;
                if (_AppHk.MapControl.Map.LayerCount == 0) return false;
                return true;
            }*/
            get
            {
                try
                {
                    if (_AppHk.CurrentControl is ISceneControl) return false;
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
        public override void OnClick()
        {
            Plugin.Application.IAppGisUpdateRef phook = _AppHk as Plugin.Application.IAppGisUpdateRef;
            SysCommon.BottomQueryBar pBar = phook.QueryBar;
            if (pBar.m_WorkSpace == null)
            {
                pBar.m_WorkSpace = Plugin.ModuleCommon.TmpWorkSpace;
            }
            if (pBar.ListDataNodeKeys == null)
            {
                pBar.ListDataNodeKeys = Plugin.ModuleCommon.ListUserdataPriID;
            }
            if (_AppHk == null) return;
            if (_AppHk.MapControl == null) return;
            IMap pMap = _AppHk.MapControl.Map;
            if(frmSQL!=null)
            {
                frmSQL.Close();
                frmSQL = null;
            }
            
            frmSQL = new FrmSQLSolution(_AppHk.MapControl,Plugin.ModuleCommon.TmpWorkSpace);
            frmSQL._QueryBar = pBar;
            frmSQL.Show((_AppHk as Plugin.Application.IAppFormRef).MainForm);
            if (WriteLog)
            {
                Plugin.LogTable.Writelog("�Զ����ѯ"); //ygc 2012-9-14 д��־
            }
        }

        public override void OnCreate(Plugin.Application.IApplicationRef hook)
        {
            if (hook == null) return;
            _AppHk = hook as Plugin.Application.IAppArcGISRef;
            if (_AppHk.MapControl == null) return;
        }
    }
}
