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

namespace FileDBTool
{
    public class ControlsPolygonBufferQueryTool : Plugin.Interface.ToolRefBase
    {
        private Plugin.Application.IAppFileRef _AppHk;

        private ITool _tool = null;
        private ICommand _cmd = null;

        public ControlsPolygonBufferQueryTool()
        {
            base._Name = "GeoUtilities.ControlsPolygonBufferQueryTool";
            base._Caption = "�滺���ѯ";
            base._Tooltip = "�滺���ѯ";
            base._Checked = false;
            base._Visible = true;
            base._Enabled = true;
            base._Deactivate = false;
            base._Message = "�滺���ѯ";
        }
        public override bool Checked
        {
            get
            {
                if (_AppHk == null) return false;
                if (_AppHk.CurrentTool != this.Name) return false;
                return true;
            }
        }
        public override bool Enabled
        {
            get
            {
                if (_tool == null || _cmd == null || _AppHk == null) return false;
                if (_AppHk.MapControl == null) return false;
                if (_AppHk.MapControl.LayerCount == 0) return false;
                if (_AppHk.ProjectTree == null) return false;
                if (_AppHk.ProjectTree.SelectedNode == null) return false;
                if (_AppHk.ProjectTree.SelectedNode.DataKey == null) return false;
                if (_AppHk.ProjectTree.SelectedNode.DataKey.ToString() == EnumTreeNodeType.DATACONNECT.ToString()) return false;
                if (_AppHk.ProjectTree.SelectedNode.ImageIndex == 1) return false;
                if (_AppHk.ProjectTree.SelectedNode.DataKey.ToString() == EnumTreeNodeType.DATABASE.ToString()) return false;
                return true;
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
            _AppHk.MapControl.CurrentTool = _tool;
            _AppHk.CurrentTool = this.Name;
        }

        public override void OnCreate(Plugin.Application.IApplicationRef hook)
        {
            if (hook == null) return;
            _AppHk = hook as Plugin.Application.IAppFileRef;

            Plugin.Application.IAppFormRef pAppForm = hook as Plugin.Application.IAppFormRef;
            _tool = new PolygonBufferQueryToolClass(pAppForm.MainForm,_AppHk);
            _cmd = _tool as ICommand;
            _cmd.OnCreate(_AppHk.MapControl);
        }
    }

    public class PolygonBufferQueryToolClass : BaseTool
    {
        private IHookHelper m_hookHelper;
        private IMapControlDefault m_MapControl;
        private Plugin.Application.IAppFileRef m_hook;
        private INewPolygonFeedback m_pNewPolygonFeedback;

        private Form m_mainFrm;

        //��ķ���
        public PolygonBufferQueryToolClass(Form mainFrm, Plugin.Application.IAppFileRef hook)
        {
            m_mainFrm = mainFrm;
            m_hook = hook;
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

        }

        /// <summary>
        /// Occurs when this tool is clicked
        /// </summary>
        public override void OnClick()
        {
            m_MapControl.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewBackground, null, m_MapControl.ActiveView.Extent);
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

            m_pNewPolygonFeedback.MoveTo(pPoint);
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
            if (pPolygon.Envelope.Width < 0.01 || pPolygon.Envelope.Height < 0.01) return;

            //����Topo���󣬼򻯺�ͳһ�ռ�ο�
            ITopologicalOperator pTopo = (ITopologicalOperator)pPolygon;
            pTopo.Simplify();
            pPolygon.Project(m_MapControl.Map.SpatialReference);

            frmBufferSet pFrmBufSet = new frmBufferSet(pPolygon as IGeometry, m_MapControl.Map);
            IGeometry pGeometry = pFrmBufSet.GetBufferGeometry();
            if (pGeometry == null || pFrmBufSet.Res == false) return;

            //==================================================================================================
            //ִ�в�ѯ���ݲ���
            ModDBOperator.QueryDataByGeometry(pGeometry, m_hook);
        }

        public override void OnKeyUp(int keyCode, int Shift)
        {
            if (Shift != 2 && keyCode != 90) return;
            if (m_pNewPolygonFeedback == null) return;
            IPolygon pPolygon = m_pNewPolygonFeedback.Stop();

            m_pNewPolygonFeedback = null;
            if (pPolygon == null) return;
            IPointCollection pntCol = pPolygon as IPointCollection;
 
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

            m_pNewPolygonFeedback.Start(pntCol.get_Point(0));
            for (int i = 1; i < pntCol.PointCount - 2; i++)
            {
                m_pNewPolygonFeedback.AddPoint(pntCol.get_Point(i));
            }

            m_pNewPolygonFeedback.MoveTo(pntCol.get_Point(pntCol.PointCount - 1));
        }

        public override void Refresh(int hDC)
        {
            if (m_pNewPolygonFeedback != null)
            {
                m_pNewPolygonFeedback.Refresh(hDC);
            }
        }

        //���߲�����ʱ�ͷŴ���ȱ���
        public override bool Deactivate()
        {
            return true;
        }
        #endregion
    }
}
