using System;
using System.Collections.Generic;
using System.Text;

using ESRI.ArcGIS.Controls;
using System.Windows.Forms;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Geodatabase;


namespace GeoPageLayout
{
    /// <summary>
    /// ���ߣ�yjl
    /// ���ڣ�20110915
    /// ˵���������ͼɭ����Դ��״�ַ�ͼ
    /// </summary>
   public class ControlsMapOutTDLYFFT : Plugin.Interface.CommandRefBase
    {
       private Plugin.Application.IAppGisUpdateRef m_Hook;
       private Plugin.Application.IAppFormRef m_frmhook;
       public ControlsMapOutTDLYFFT()
        {
            base._Name = "GeoPageLayout.ControlsMapOutTDLYFFT";
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
                if (this.WriteLog)
                {
                    Plugin.LogTable.Writelog("��ɭ����Դ��״�ַ�ͼ ��ʾ����ǰû�е������ݣ���", m_Hook.tipRichBox);
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
                IFeatureClass xzqFC = ModGetData.GetXZQFC("//LayerConfig/County");
                if (xzqFC == null)
                {
                    SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "δ�ҵ��ؼ�������ͼ�㣡");
                    return;
                }
                string xzqdmFD = ModGetData.GetXZQFd("//LayerConfig/County");
                if (xzqdmFD == null)
                {
                    SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "δ�ҵ��ؽڵ�������������ֶ����ƣ�");
                    return;
                }
                if (this.WriteLog)
                {
                    Plugin.LogTable.Writelog("ɭ����Դ��״�ַ�ͼ", m_Hook.tipRichBox);
                }
                m_Hook.ArcGisMapControl.CurrentTool = null;

                FrmSheetMapUserSet_ZT fmSMUS = new
                     FrmSheetMapUserSet_ZT(m_Hook.ArcGisMapControl, m_frmhook.MainForm, "", xzqFC, xzqdmFD);
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
            m_Hook = hook as Plugin.Application.IAppGisUpdateRef;
            m_frmhook = hook as Plugin.Application.IAppFormRef;
        }
    }
}
