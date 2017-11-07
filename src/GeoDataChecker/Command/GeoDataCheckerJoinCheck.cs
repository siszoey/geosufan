using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Threading;
using System.Xml;
using System.IO;
using System.Windows.Forms;
using System.Data;

using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.Display;

namespace GeoDataChecker
{
    /// <summary>
    /// ���ܣ��ӱ߼�� �����һ����Χ��Ӧ��Ҫ�ӱߵ�����Ҫ�ض�û����
    /// ��д�ˣ�����
    /// �������������� 
    /// �ٴ�����:����
    /// 
    /// �ӱ߼���˼�룺
    /// ����˼�룺Ӧ���Ӧ���ӱ߶�û�н��ϵ�����Ҫ�أ������ѽ��϶�û���ںϵģ�����Ϊ��������
    /// �ӱߵĿ���˼�룺����ͼ��Ϊ���գ�
    /// 1������ȡ�õ���ͼ������߿�
    /// 2����Ա߿���л���
    /// 3�����õ���������巶Χ��ԭ����ͼ���߿�����󽻣����յõ�������ͼ���ڵ�һ���ֻ�����
    /// 4��ȡ����������ֻ������ڵ�����Ҫ��
    /// 5�����ÿһ��Ҫ�ؽ���ȡ�㼯�ϣ�ȷ���ڲ��ֻ������ڵĵ㣬����Щ����֯��һ����Ϊ��ѯ�Ķ���
    /// 6����X��YΪ���գ�ȷ���������ǲ��ǽ��ϣ�����ȡ�����Ĳ������Ա߿�Ϊ���յ������
    /// 7���õ㼯�����ÿһ����ͱ߿�������õ��Ǳ߿��ڵ�Ҫ�أ�Ȼ�����������ѡ������ԣ�ȷ��Ӧ��Ҫ�ӱߵ�Ҫ�ض�û�н��ϣ���ʾ��ЩҪ��
    /// 
    /// </summary>

    public class JoinCheck : Plugin.Interface.CommandRefBase
    {
        
        private Plugin.Application.IAppGISRef _AppHk;
        public JoinCheck()
        {
            base._Name = "GeoDataChecker.JoinCheck";
            base._Caption = "�ӱ߼��";
            base._Tooltip = "�ӱ߼��";
            base._Checked = false;
            base._Visible = true;
            base._Enabled = true;
            base._Message = "�ӱ߼��";

        }
       
        public override bool Enabled
        {
            get
            {
                try
                {
                    //if (_AppHk.MapControl.LayerCount == 0)
                    //{
                    //    base._Enabled = false;
                    //    return false;
                    //}
                    //else
                    //{
                    //    if (SetCheckState.CheckState)
                    //    {
                    //        base._Enabled = true;
                    //        return true;
                    //    }
                    //    else
                    //    {
                    //        base._Enabled = false;
                    //        return false;
                    //    }
                    //}
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
            if (_AppHk.MapControl == null) return;
            //ִ�еȸ���ע�Ǽ��
            //FrmMathematicsCheck mFrmMathematicsCheck = new FrmMathematicsCheck(_AppHk, enumErrorType.�ӱ߼��);
            SetJoinChecks frmSetJoinCheck = new SetJoinChecks(_AppHk);
            frmSetJoinCheck.ShowDialog();

            //FromJoinCheck = new SetJoinChecks(_AppHk);//ʵ�����ýӱ����ԵĴ���
            //FromJoinCheck.Show();
            //Overridfunction.name = "�ӱ߼��";
            ////������ʾ��������
            //DevComponents.DotNetBar.PanelDockContainer PanelTip = _AppHk.DataTree.Parent as DevComponents.DotNetBar.PanelDockContainer;
            //if (PanelTip != null)
            //{
            //    PanelTip.DockContainerItem.Selected = true;
            //}
            

        }
        public override void OnCreate(Plugin.Application.IApplicationRef hook)
        {
            if (hook == null) return;
            _AppHk = hook as Plugin.Application.IAppGISRef;
            if (_AppHk.MapControl == null) return;

        }


    }
}
