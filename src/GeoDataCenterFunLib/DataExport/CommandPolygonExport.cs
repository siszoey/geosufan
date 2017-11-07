using System;
using System.Collections.Generic;
using System.Text;

namespace GeoDataCenterFunLib
{
    public class CommandPolygonExport : Plugin.Interface.CommandRefBase
    {
        private Plugin.Application.IAppGisUpdateRef _AppHk;
        private Plugin.Application.IAppFormRef m_pAppForm;
        private ESRI.ArcGIS.SystemUI.ICommand m_pCommand;

        public CommandPolygonExport()
        {
            base._Name = "GeoDataCenterFunLib.CommandPolygonExport";
            base._Caption = "����η�Χ��ȡ";
            base._Tooltip = "����η�Χ��ȡ";
            base._Checked = false;
            base._Visible = true;
            base._Enabled =false;
            base._Message = "����η�Χ��ȡ";
            m_pCommand = new GeoDataExport.Commands.ToolPolygonExport();
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
                    if (_AppHk.CurrentControl is ESRI.ArcGIS.Controls.ISceneControl) return false;
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
            if (this.WriteLog)
            {
                Plugin.LogTable.Writelog(base._Caption);//xisheng ��־��¼
            }
            ///ZQ  20111027 add  �ж������ֵ��Ƿ��ʼ��
            if (SysCommon.ModField._DicFieldName.Count == 0)
            {
                SysCommon.ModField.InitNameDic(Plugin.ModuleCommon.TmpWorkSpace, SysCommon.ModField._DicFieldName, "���Զ��ձ�");
            }
            _AppHk.MapControl.CurrentTool = m_pCommand as ESRI.ArcGIS.SystemUI.ITool;
        }

        public override void OnCreate(Plugin.Application.IApplicationRef hook)
        {
            if (hook == null) return;
            _AppHk = hook as Plugin.Application.IAppGisUpdateRef;
            if (_AppHk.MapControl == null) return;
            m_pCommand.OnCreate(_AppHk);
            m_pAppForm = _AppHk as Plugin.Application.IAppFormRef;
        }
    }
}
