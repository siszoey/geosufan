using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using GeoDataCenterFunLib;

using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.DataSourcesFile;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.SystemUI;
using ESRI.ArcGIS.Output;
using ESRI.ArcGIS.Display;

//�˳���ͼ�ĵ�
namespace GeoDataManagerFrame
{
    public class ExitMapDoc : Plugin.Interface.CommandRefBase
    {
        private Plugin.Application.IAppGisUpdateRef m_Hook;
        private Plugin.Application.IAppFormRef m_frmhook;
        public ExitMapDoc()
        {
            base._Name = "GeoDataManagerFrame.ExitMapDoc";
            base._Caption = "�˳���ͼ�ĵ�";
            base._Tooltip = "�˳���ͼ�ĵ�";
            base._Checked = false;
            base._Visible = true;
            base._Enabled = true;
            base._Message = "�˳���ͼ�ĵ�";
        }

        public override void OnClick()
        {
            if (m_Hook != null)
            {
                LogFile log = new LogFile(m_Hook.tipRichBox, m_Hook.strLogFilePath);
                if (log != null)
                {
                    log.Writelog("�˳���ͼ�ĵ�");
                }
            }
            //���ͼ��
            m_Hook.MapControl.ActiveView.Clear();
            m_Hook.MapControl.ActiveView.Refresh();
            
            //��յ�ͼ�ĵ���
            m_Hook.MapDocTree.Nodes.Clear();
            
            m_Hook.TextDocTree.Nodes.Clear();
            
            m_Hook.DocControl.Clear();

            SetControl.m_tparent = null;
         /*   m_Hook.ArcGisMapControl.Map.Name = "����ͼ��";
            m_Hook.TOCControl.SetBuddyControl(m_Hook.ArcGisMapControl.Object);

            //
            m_Hook.ArcGisMapControl.LoadMxFile("D:\\Untitled.mxd");
            m_Hook.TOCControl.Update();*/
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
