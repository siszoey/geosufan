using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.SystemUI;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Geodatabase;
using DevComponents.DotNetBar;
namespace GeoDataChecker
{
    /// <summary>
    /// ��ʼ��Ҫ�����ݼ�����MAP�ϲ�������������ɢ����������������Ҫ�����ݼ��£��ͽ��д���
    /// </summary>
    public class InitializationFeatureDataset : Plugin.Interface.CommandRefBase
    {
        private Plugin.Application.IAppGISRef _AppHk;


        public InitializationFeatureDataset()
        {
            base._Name = "GeoDataChecker.InitializationFeatureDataset";
            base._Caption = "Ԥ����";
            base._Tooltip = "���ü�������Ҫ�����ݼ���";
            base._Checked = false;
            base._Visible = false;
            base._Enabled = true;
            base._Message = "Ԥ��������";
            
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
                    else
                    {
                        if (SetCheckState.CheckState)
                        {
                            base._Enabled = true;
                            return true;
                        }
                        else
                        {
                            base._Enabled = false;
                            return false;
                        }
                    }
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

            if (_AppHk == null) return;
            if (_AppHk.MapControl == null) return;

            Plugin.Application.IAppFormRef hook = _AppHk as Plugin.Application.IAppFormRef;
            FrmInitiFeatureDataset frm_dataset = new FrmInitiFeatureDataset(hook);
            frm_dataset.ShowInTaskbar = false;
            frm_dataset.ShowDialog();
        }

        public override void OnCreate(Plugin.Application.IApplicationRef hook)
        {
            if (hook == null) return;
            _AppHk = hook as Plugin.Application.IAppGISRef;
            if (_AppHk.MapControl == null) return;

        }
    }
}
