using System;
using System.Collections.Generic;
using System.Text;

using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.SystemUI;

namespace GeoStatistics
{
    public class CommandCustomizeStatistics : Plugin.Interface.CommandRefBase
    {
        private Plugin.Application.IAppGisUpdateRef _AppHk;
        private Plugin.Application.IAppFormRef m_pAppForm;

        public CommandCustomizeStatistics()
        {
            base._Name = "GeoStatistics.CommandCustomizeStatistics";
            base._Caption = "����ͳ��";
            base._Tooltip = "����ͳ��";
            base._Checked = false;
            base._Visible = true;
            base._Enabled =false;
            base._Message = "����ͳ��";

        }

        /// <summary>
        /// ͼ���д�������ʱ����״̬Ϊ����ʱ�ſ���
        /// </summary>
        public override bool Enabled
        {
            get
            {
                if (_AppHk != null)
                {
                    //if (_AppHk.MapControl != null)
                    //{
                        //if (_AppHk.MapControl.Map.LayerCount > 0)
                        //{
                    return true;
                        //}
                    //}
                }
                return false ;
            }
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

        public override void OnClick()
        {
            if (_AppHk == null) return;
            ///ZQ  20111017 add  �ж������ֵ��Ƿ��ʼ��
            if (SysCommon.ModField._DicFieldName.Count == 0)
            {
                SysCommon.ModField.InitNameDic(Plugin.ModuleCommon.TmpWorkSpace, SysCommon.ModField._DicFieldName, "���Զ��ձ�");
            }
            FrmCustomizeStatistic  frm = null;
            frm = new FrmCustomizeStatistic();
            frm.ShowDialog();
            
        }

        public override void OnCreate(Plugin.Application.IApplicationRef hook)
        {
            if (hook == null) return;
            _AppHk = hook as Plugin.Application.IAppGisUpdateRef;
            if (_AppHk.MapControl == null) return;

            m_pAppForm = _AppHk as Plugin.Application.IAppFormRef;
        }
    }
}
