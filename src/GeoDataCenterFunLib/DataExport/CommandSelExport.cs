using System;
using System.Collections.Generic;
using System.Text;
using ESRI.ArcGIS.Carto;
using System.Windows.Forms;
using ESRI.ArcGIS.Controls;

namespace GeoDataCenterFunLib
{
    public class CommandSelExport : Plugin.Interface.CommandRefBase
    {
        private Plugin.Application.IAppGisUpdateRef _AppHk;
        private Plugin.Application.IAppFormRef m_pAppForm;
        private ESRI.ArcGIS.SystemUI.ICommand m_pCommand;

        public CommandSelExport()
        {
            base._Name = "GeoDataCenterFunLib.CommandSelExport";
            base._Caption = "ѡ��Ҫ�ط�Χ��ȡ";
            base._Tooltip = "ѡ��Ҫ�ط�Χ��ȡ";
            base._Checked = false;
            base._Visible = true;
            base._Enabled =false;
            base._Message = "ѡ��Ҫ�ط�Χ��ȡ";
            m_pCommand = new GeoDataExport.Commands.CommandSelExport();
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
            if (_AppHk == null) 
                return;
            //IFeatureLayer tmpFeatureLayer = layerCurSeleted();
            //if (tmpFeatureLayer == null )
            //{
            //    MessageBox.Show("���ڵ�ͼĿ¼���õ�ǰѡ��ͼ�㣡", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //    return;
            //}
            if (this.WriteLog)
            {
                Plugin.LogTable.Writelog(base._Caption);//xisheng ��־��¼
            }
            ///ZQ  20111027 add  �ж������ֵ��Ƿ��ʼ��
            if (SysCommon.ModField._DicFieldName.Count == 0)
            {
                SysCommon.ModField.InitNameDic(Plugin.ModuleCommon.TmpWorkSpace, SysCommon.ModField._DicFieldName, "���Զ��ձ�");
            }
            m_pCommand.OnClick();
        }

        public override void OnCreate(Plugin.Application.IApplicationRef hook)
        {
            if (hook == null) return;
            _AppHk = hook as Plugin.Application.IAppGisUpdateRef;
            if (_AppHk.MapControl == null) return;
            m_pCommand.OnCreate(_AppHk);
            m_pAppForm = _AppHk as Plugin.Application.IAppFormRef;
        }
        private IFeatureLayer layerCurSeleted()
        {
            IMap pMap = _AppHk.MapControl.Map;
            int iLayerCount = pMap.LayerCount;
            IFeatureLayer pFeatureLayer;
            IFeatureLayer pCurSeLayer = null;

            int countSelectable = 0;
            for (int n = 0; n < iLayerCount; n++)
            {
                if (countSelectable >= 2) break;
                ILayer layer = pMap.get_Layer(n);
                if (layer is IGroupLayer)
                {
                    ICompositeLayer Comlayer = layer as ICompositeLayer;//��һ��������Ĳ����ת����һ����ϲ㣬ʹ�����Ա���
                    for (int c = 0; c < Comlayer.Count; c++)
                    {
                        pFeatureLayer = Comlayer.get_Layer(c) as IFeatureLayer;
                        if (pFeatureLayer != null && pFeatureLayer.Selectable)
                        {
                            countSelectable++;
                            pCurSeLayer = pFeatureLayer;

                        }

                    }
                }
            }//for
            if (countSelectable != 1)
                return null;
            else
                return pCurSeLayer;




        }



    }
}
