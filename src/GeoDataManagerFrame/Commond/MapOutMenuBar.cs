using System;
using System.Collections.Generic;
using System.Text;

using ESRI.ArcGIS.Controls;
using System.Windows.Forms;

using GeoDataCenterFunLib;
using ESRI.ArcGIS.Geometry;

namespace GeoDataManagerFrame
{
   public class MapOutMenuBar : Plugin.Interface.CommandRefBase
    {
        private Plugin.Application.IAppGisUpdateRef m_Hook;
       private Plugin.Application.IAppFormRef m_frmhook;
       Plugin.Application.IApplicationRef pHook;
       public MapOutMenuBar()
        {
            base._Name = "GeoDataManagerFrame.MapOutMenuBar";
            base._Caption = "��׼ͼ��";
            base._Tooltip = "��׼ͼ��";
            base._Checked = false;
            base._Visible = true;
            base._Enabled = true;
            base._Message = "��׼ͼ��";
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
            //LogFile log = new LogFile(m_Hook.tipRichBox, m_Hook.strLogFilePath);

            //if (log != null)
            //{
            //    log.Writelog("��׼ͼ��");
            //}
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
           Plugin.LogTable.Writelog("��׼�ַ���ͼ", m_Hook.tipRichBox);
            m_Hook.ArcGisMapControl.CurrentTool = null;
            GeoPageLayout.FrmSheetMapUserSet fmSMUS = new
                 GeoPageLayout.FrmSheetMapUserSet(m_Hook.ArcGisMapControl, m_frmhook.MainForm,pHook);
            fmSMUS.Show(m_frmhook.MainForm);
                

           
            

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
