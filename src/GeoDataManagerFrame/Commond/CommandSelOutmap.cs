using System;
using System.Collections.Generic;
using System.Text;

using ESRI.ArcGIS.Controls;
using System.Windows.Forms;
using ESRI.ArcGIS.ADF;
using GeoDataCenterFunLib;
using ESRI.ArcGIS.SystemUI;

namespace GeoDataManagerFrame
{
   public class CommandSelOutmap : Plugin.Interface.CommandRefBase
    {
        private Plugin.Application.IAppGisUpdateRef m_Hook;
       public Plugin.Application.IAppFormRef m_frmhook;
       private ICommand _cmd = null;
       public CommandSelOutmap()
        {
            base._Name = "GeoDataManagerFrame.CommandSelOutmap";
            base._Caption = "ѡ��Ҫ�ط�Χ��ͼ";
            base._Tooltip = "ѡ��Ҫ�ط�Χ��ͼ";
            base._Checked = false;
            base._Visible = true;
            base._Enabled = true;
            base._Message = "ѡ��Ҫ�ط�Χ��ͼ";
        }
       public override bool Enabled
       {
           get
           {
               try
               {
                   if (m_Hook.CurrentControl is ISceneControl) return false;
                   if (m_Hook.MapControl.LayerCount == 0)
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
                    Plugin.LogTable.Writelog("ѡ��Ҫ�ط�Χ��ͼ ��ʾ'��ǰû�е�������!'", m_Hook.tipRichBox);
                }
                return;
            }
            if (this.WriteLog)
            {
                Plugin.LogTable.Writelog("ѡ��Ҫ�ط�Χ��ͼ", m_Hook.tipRichBox);
            }
            _cmd = new GeoPageLayout.CommandSelOutmap();
            CommandSelOutmap TempCommand = _cmd as CommandSelOutmap;
            TempCommand.WriteLog = this.WriteLog;//ygc 2012-9-12 �Ƿ�д��־
            TempCommand.OnCreate(m_Hook);
            TempCommand.OnClick();
       
            SetControl pSetControl = m_Hook.MainUserControl as SetControl ;
            if (pSetControl != null)
            {
                pSetControl.InitOutPutResultTree();
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
