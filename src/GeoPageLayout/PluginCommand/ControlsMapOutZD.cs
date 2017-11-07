using System;
using System.Collections.Generic;
using System.Text;

using ESRI.ArcGIS.Controls;
using System.Windows.Forms;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.SystemUI;

namespace GeoPageLayout
{
    /// <summary>
    /// ���ߣ�yjl
    /// ���ڣ�20110928
    /// ˵���������ڵ�ͼ
    /// </summary>
   public class ControlsMapOutZD : Plugin.Interface.CommandRefBase
    {
        private Plugin.Application.IAppGisUpdateRef m_Hook;
       private Plugin.Application.IAppFormRef m_frmhook;
       Plugin.Application.IApplicationRef pHook;
       private ICommand _cmd = null;
       public ControlsMapOutZD()
        {
            base._Name = "GeoPageLayout.ControlsMapOutCZDJ";
            base._Caption = "�ڵ�ͼ";
            base._Tooltip = "�ڵ�ͼ";
            base._Checked = false;
            base._Visible = true;
            base._Enabled = true;
            base._Message = "�ڵ�ͼ";
        }
       public override bool Enabled
       {
           get
           {
               if (m_Hook == null) return false;
               if (m_Hook.CurrentControl == null) return false;
               if (m_Hook.CurrentControl is ISceneControl) return false;
               return true;
           }
       }
        public override void OnClick()
        {
            if (m_Hook == null)
                return;

            if (m_Hook.ArcGisMapControl.Map.LayerCount == 0)
            {
                MessageBox.Show("��ǰû�е������ݣ�", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
               Plugin.LogTable.Writelog("��׼�ַ���ͼ ��ʾ����ǰû�е������ݣ���", m_Hook.tipRichBox);
                return;
            }
            ISpatialReference pSpatialRefrence = m_Hook.ArcGisMapControl.SpatialReference;
            if (!(pSpatialRefrence is IProjectedCoordinateSystem))
            {
                //MessageBox.Show("�����õ�ͼ��ͶӰ���꣡", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
               //Plugin.LogTable.Writelog("��׼�ַ���ͼ ��ʾ�������õ�ͼ��ͶӰ���꣡��", m_Hook.tipRichBox);
                //return;
            }
            Plugin.LogTable.Writelog("�ڵ�ͼ", m_Hook.tipRichBox);
            _cmd = new CommandSelOutmapZD();
            _cmd.OnCreate(m_Hook.MapControl);
            _cmd.OnClick();
                

           
            

        }

        public override void OnCreate(Plugin.Application.IApplicationRef hook)
        {
            if (hook == null)
                return;
            pHook = hook;
            m_Hook = hook as Plugin.Application.IAppGisUpdateRef;
            m_frmhook = hook as Plugin.Application.IAppFormRef;
        }
    }
}
