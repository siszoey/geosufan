using System;
using System.Collections.Generic;
using System.Text;

using ESRI.ArcGIS.Controls;
using System.Windows.Forms;
using ESRI.ArcGIS.ADF;
using ESRI.ArcGIS.SystemUI;

namespace GeoPageLayout
{
    /// <summary>
    /// ���ߣ�yjl
    /// ���ڣ�20110928
    /// ˵���������ڵ�ͼ
    /// </summary>
   public class ControlsSelOutmapZD : Plugin.Interface.CommandRefBase
    {
        private Plugin.Application.IAppGisUpdateRef m_Hook;
       public Plugin.Application.IAppFormRef m_frmhook;
       private ICommand _cmd = null;
       public ControlsSelOutmapZD()
        {
            base._Name = "GeoPageLayout.ControlsSelOutmapZD";
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
            //LogFile log = new LogFile(m_Hook.tipRichBox, m_Hook.strLogFilePath);

            //if (log != null)
            //{
            //    log.Writelog("ѡ��Ҫ�ط�Χ��ͼ");
            //}
            if (m_Hook.ArcGisMapControl.Map.LayerCount == 0)
            {
                MessageBox.Show("��ǰû�е������ݣ�", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                if (this.WriteLog)
                {
                    Plugin.LogTable.Writelog("�ڵ�ͼ ��ʾ'��ǰû�е�������!'", m_Hook.tipRichBox);
                }
                return;
            }
            try
            {
                if (this.WriteLog)
                {
                    Plugin.LogTable.Writelog("�ڵ�ͼ", m_Hook.tipRichBox);
                }
                _cmd = new CommandSelOutmapZD();
                CommandSelOutmapZD TempCommand = _cmd as CommandSelOutmapZD;
                TempCommand.WriteLog = this.WriteLog;//ygc 2012-9-12 �Ƿ�д��־
                TempCommand.OnCreate(m_Hook.MapControl);
                TempCommand.OnClick();
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
            m_Hook = hook as Plugin.Application.IAppGisUpdateRef;
            m_frmhook = hook as Plugin.Application.IAppFormRef;
            
        }
    }
}
