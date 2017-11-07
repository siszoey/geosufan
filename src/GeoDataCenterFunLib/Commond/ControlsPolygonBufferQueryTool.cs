using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ADF.CATIDs;

using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.SystemUI;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.esriSystem;

namespace GeoDataCenterFunLib
{
    public class ControlsPolygonBufferQueryTool : Plugin.Interface.ToolRefBase
    {
        private Plugin.Application.IAppArcGISRef _AppHk;

        private ITool _tool = null;
        private ICommand _cmd = null;

        public ControlsPolygonBufferQueryTool()
        {
            base._Name = "GeoDataCenterFunLib.ControlsPolygonBufferQueryTool";
            base._Caption = "�滺���ѯ";
            base._Tooltip = "�滺���ѯ";
            base._Checked = false;
            base._Visible = true;
            base._Enabled = true;
            base._Deactivate = false;
            base._Message = "�滺���ѯ";
            //base._Cursor = (int)esriControlsMousePointer.esriPointerIdentify;
            //base._Image = "";
            //base._Category = "";
        }

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
            Plugin.Application.IAppGisUpdateRef phook = _AppHk as Plugin.Application.IAppGisUpdateRef;
            SysCommon.BottomQueryBar pBar = phook.QueryBar;
            if (pBar.m_WorkSpace == null)
            {
                pBar.m_WorkSpace = Plugin.ModuleCommon.TmpWorkSpace;                
            }
            if (pBar.ListDataNodeKeys == null)
            {
                pBar.ListDataNodeKeys = Plugin.ModuleCommon.ListUserdataPriID;
            }
            PolygonBufferQueryToolClass pTool = _cmd as PolygonBufferQueryToolClass;
            pTool.WriteLog = WriteLog;//ygc 2012-9-12 �Ƿ�д��־
            pTool.GetQueryBar(pBar);
            _cmd.OnClick();
            if (_AppHk.CurrentControl is IMapControl2)
            {
                _AppHk.MapControl.CurrentTool = _tool;
            }
            else
            {
                _AppHk.PageLayoutControl.CurrentTool = _tool;
            }

            _AppHk.CurrentTool = this.Name;
        }

        public override void OnCreate(Plugin.Application.IApplicationRef hook)
        {
            if (hook == null) return;
            _AppHk = hook as Plugin.Application.IAppArcGISRef;

            Plugin.Application.IAppFormRef pAppForm = hook as Plugin.Application.IAppFormRef;
            _tool = new PolygonBufferQueryToolClass(pAppForm.MainForm);
            _cmd = _tool as ICommand;
            _cmd.OnCreate(_AppHk.MapControl);
        }
    }

    public class PolygonBufferQueryToolClass : BaseTool
    {
        private IHookHelper m_hookHelper;
        private IMapControlDefault m_MapControl;

        private INewPolygonFeedback m_pNewPolygonFeedback;

        private Form m_mainFrm;
        private frmQuery m_frmQuery;
        private SysCommon.BottomQueryBar _QueryBar;
        private enumQueryMode m_enumQueryMode;
        private frmBufferSet m_frmBufferSet = null;
        private bool _Writelog = true;  //added by chulili 2012-09-07 �Ƿ�д��־
        public bool WriteLog
        {
            get
            {
                return _Writelog;
            }
            set
            {
                _Writelog = value;
            }
        }
        public void GetQueryBar(SysCommon.BottomQueryBar QueryBar)
        {
            _QueryBar = QueryBar;
        }
        //��ķ���
        public PolygonBufferQueryToolClass(Form mainFrm)
        {
            m_mainFrm = mainFrm;
            base.m_category = "GeoCommon";
            base.m_caption = "PolygonBufferQuery";
            base.m_message = "�滺���ѯ";
            base.m_toolTip = "�滺���ѯ";
            base.m_name = base.m_category + "_" + base.m_caption;
            try
            {
                base.m_cursor = new System.Windows.Forms.Cursor(GetType(), "Resources.MapIdentify.cur");
            }
            catch
            {

            }
        }

        #region Overriden Class Methods

        /// <summary>
        /// Occurs when this tool is created
        /// </summary>
        /// <param name="hook">Instance of the application</param>
        public override void OnCreate(object hook)
        {
            if (m_hookHelper == null)
                m_hookHelper = new HookHelperClass();

            m_hookHelper.Hook = hook;
            m_MapControl = hook as IMapControlDefault;

            m_enumQueryMode = enumQueryMode.Visiable ;    //changed by chulili 20110707�ֲ����е�ǰ�༭ͼ��ѡ��
            //m_enumQueryMode = enumQueryMode.CurEdit;  //changed by chulili 2011-04-15 Ĭ�ϵ�ǰ�༭ͼ��
        }

        /// <summary>
        /// Occurs when this tool is clicked
        /// </summary>
        public override void OnClick()
        {
            if (m_frmQuery == null)
            {
                m_frmQuery = new frmQuery(m_MapControl, m_enumQueryMode);
                m_frmQuery.Owner = m_mainFrm;
                m_frmQuery.FormClosed += new FormClosedEventHandler(frmQuery_FormClosed);
            }
            try
            {
                base.m_cursor = new System.Windows.Forms.Cursor(GetType(), "Resources.MapIdentify.cur");
            }
            catch
            {

            }
            //deleted by chulili 20110731
            //m_frmQuery.Show();
            //m_frmQuery.FillData(m_MapControl.ActiveView.FocusMap, null);
        }

        private void frmQuery_FormClosed(object sender, FormClosedEventArgs e)
        {
            m_enumQueryMode = m_frmQuery.QueryMode;
            m_frmQuery = null;
            if (m_frmBufferSet!=null) m_frmBufferSet.setBufferGeometry(null);//added by chulili 20110731
           // m_MapControl.ActiveView.Refresh();
            //deleted by chulili 20110731
            //base.m_cursor = Cursors.Default;//���ص�Ĭ��״̬ xisheng 20110722
            //m_MapControl.CurrentTool = null;
            //end deleted by chulili 
        }

        public override void OnMouseDown(int Button, int Shift, int X, int Y)
        {
            if (Button != 1) return;

            IPoint pPoint = m_MapControl.ActiveView.ScreenDisplay.DisplayTransformation.ToMapPoint(X, Y);
            if (m_pNewPolygonFeedback == null)  //��һ�ΰ���
            {
                IRgbColor pRGB = new RgbColorClass();
                ISimpleFillSymbol pSimpleFillSymbol = new SimpleFillSymbolClass();
                pRGB.Red = 15;
                pRGB.Blue = 15;
                pRGB.Green = 0;
                pSimpleFillSymbol.Color = pRGB;
                pSimpleFillSymbol.Style = esriSimpleFillStyle.esriSFSSolid;

                m_pNewPolygonFeedback = new NewPolygonFeedbackClass();
                m_pNewPolygonFeedback.Symbol = pSimpleFillSymbol as ISymbol;
                m_pNewPolygonFeedback.Display = m_MapControl.ActiveView.ScreenDisplay;
                
                m_pNewPolygonFeedback.Start(pPoint);
                if (m_frmBufferSet != null)
                {
                    SysCommon.ScreenDraw.list.Remove(m_frmBufferSet.BufferSetAfterDraw);
                    m_frmBufferSet.setBufferGeometry(null);
                    m_frmBufferSet = null;
                }
            }
            else                            //�������
            {
                m_pNewPolygonFeedback.AddPoint(pPoint);
            }

        }

        //����ƶ��� ������Ҳ�ƶ������λ��
        public override void OnMouseMove(int Button, int Shift, int X, int Y)
        {
            base.OnMouseMove(Button, Shift, X, Y);

            IPoint pPoint = m_MapControl.ActiveView.ScreenDisplay.DisplayTransformation.ToMapPoint(X, Y);
            if (m_pNewPolygonFeedback != null)
            {
                m_pNewPolygonFeedback.MoveTo(pPoint);
            }
        }

        //˫���򴴽����ߣ����������崰��
        public override void OnDblClick()
        {
            //��ȡ���� ����ȡ��ǰ��ͼ����Ļ��ʾ
            if (m_pNewPolygonFeedback == null) return;
            IPolygon pPolygon = m_pNewPolygonFeedback.Stop();
            m_pNewPolygonFeedback = null;

            //�����ڣ�Ϊ�ա��ߴ粻�����˳�
            if (pPolygon == null || pPolygon.IsEmpty) return;
           //xisheng �ж�����ĸ߶ȣ����ʱ���ж���ʲô��λ�����Ǿ�γ������Ҫת����20111220
            double w=pPolygon.Envelope.Width;
            double h=pPolygon.Envelope.Height;
            UnitConverter conver = new UnitConverterClass();
            if (m_MapControl.Map.MapUnits == esriUnits.esriDecimalDegrees)
            {
                w = conver.ConvertUnits(w, esriUnits.esriDecimalDegrees, esriUnits.esriMeters);
                h = conver.ConvertUnits(h, esriUnits.esriDecimalDegrees, esriUnits.esriMeters);
            }
            if (w < 0.01 || h < 0.01) return;
            //xisheng 20111220 end *********************************************************

            //����Topo���󣬼򻯺�ͳһ�ռ�ο�
            ITopologicalOperator pTopo = (ITopologicalOperator)pPolygon;
            pTopo.Simplify();
            pPolygon.Project(m_MapControl.Map.SpatialReference);

            if (m_frmQuery == null)
            {
                m_frmQuery = new frmQuery(m_MapControl, m_enumQueryMode);
                m_frmQuery.Owner = m_mainFrm;
                m_frmQuery.FormClosed += new FormClosedEventHandler(frmQuery_FormClosed);
            }
            if (this.WriteLog)
            {
                Plugin.LogTable.Writelog("�滺���ѯ");//xisheng ��־��¼ 0928;
            }
            //����ϴε�����Ԫ��
            (m_MapControl.Map as IGraphicsContainer).DeleteAllElements();
            m_frmBufferSet = new frmBufferSet(pPolygon as IGeometry, m_MapControl.Map, m_frmQuery);
            IGeometry pGeometry = m_frmBufferSet.GetBufferGeometry();
            if (pGeometry == null || m_frmBufferSet.Res == false) return;
           // m_frmQuery.Show();
            ///ZQ 20111119  modify
            //m_frmQuery.FillData(m_MapControl.ActiveView.FocusMap, pGeometry,m_frmBufferSet.pesriSpatialRelEnum);
            _QueryBar.m_pMapControl = m_MapControl;
            _QueryBar.EmergeQueryData(m_MapControl.ActiveView.FocusMap, pGeometry, m_frmBufferSet.pesriSpatialRelEnum);
            try
            {
                DevComponents.DotNetBar.Bar pBar = _QueryBar.Parent.Parent as DevComponents.DotNetBar.Bar;
                if (pBar != null)
                {
                    pBar.AutoHide = false;
                    //pBar.SelectedDockTab = 1;
                    int tmpindex = pBar.Items.IndexOf("dockItemDataCheck");
                    pBar.SelectedDockTab = tmpindex;
                }
            }
            catch
            { }
        }

        //���߲�����ʱ�ͷŴ���ȱ���
        public override bool Deactivate()
        {
            return true;
        }
        #endregion
    }
}
