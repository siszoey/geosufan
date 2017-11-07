using System;
using System.Collections.Generic;
using System.Text;

using ESRI.ArcGIS.Controls;
using System.Windows.Forms;
using ESRI.ArcGIS.Geometry;

namespace GeoPageLayout
{
    /// <summary>
    /// ���ߣ�yjl
    /// ���ڣ�20110928
    /// ˵������������ؼ�ͼ
    /// </summary>
   public class ControlsMapOutCZDJ : Plugin.Interface.CommandRefBase
    {
        private Plugin.Application.IAppGisUpdateRef m_Hook;
       private Plugin.Application.IAppFormRef m_frmhook;
       Plugin.Application.IApplicationRef pHook;
       public ControlsMapOutCZDJ()
        {
            base._Name = "GeoPageLayout.ControlsMapOutCZDJ";
            base._Caption = "����ؼ�ͼ";
            base._Tooltip = "����ؼ�ͼ";
            base._Checked = false;
            base._Visible = true;
            base._Enabled = true;
            base._Message = "����ؼ�ͼ";
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
                if (this.WriteLog)
                {
                    Plugin.LogTable.Writelog("��׼�ַ���ͼ ��ʾ����ǰû�е������ݣ���", m_Hook.tipRichBox);
                }
                return;
            }
            ISpatialReference pSpatialRefrence = m_Hook.ArcGisMapControl.SpatialReference;
            if (!(pSpatialRefrence is IProjectedCoordinateSystem))
            {
                //MessageBox.Show("�����õ�ͼ��ͶӰ���꣡", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
               //Plugin.LogTable.Writelog("��׼�ַ���ͼ ��ʾ�������õ�ͼ��ͶӰ���꣡��", m_Hook.tipRichBox);
                //return;
            }
            try
            {
                if (this.WriteLog)
                {
                    Plugin.LogTable.Writelog("����ؼ�ͼ", m_Hook.tipRichBox);
                }
                m_Hook.ArcGisMapControl.CurrentTool = null;
                FrmSheetMapUserSet fmSMUS = new
                     FrmSheetMapUserSet(m_Hook.ArcGisMapControl, m_frmhook.MainForm, pHook, SheetType.urbanCadastre);
                fmSMUS.Show(m_frmhook.MainForm);
                fmSMUS.WriteLog = WriteLog;//ygc 2012-9-12 �Ƿ�д��־

            }
            catch (Exception ex)
            {
                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", ex.Message);
            }
           
            

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
