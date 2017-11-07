using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ADF.CATIDs;

using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.SystemUI;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.esriSystem;

namespace GeoEdit
{
    public class ControlsEditTool : Plugin.Interface.ToolRefBase
    {
        private Plugin.Application.IAppArcGISRef m_Hook;
        private ITool _tool = null;
        private ICommand _cmd = null;

        public ControlsEditTool()
        {

            base._Name = "GeoEdit.ControlsEditTool";
            base._Caption = "Ҫ�ر༭";
            base._Tooltip = "Ҫ�ر༭";
            base._Checked = false;
            base._Visible = true;
            base._Enabled = true;
            base._Message = "Ҫ�ر༭";
        }
        public override bool Enabled
        {
            get
            {
                if (m_Hook == null) return false;
                if (m_Hook.MapControl == null) return false;
                if (MoData.v_CurWorkspaceEdit == null) return false;
                return true;
            }
        }

        public override bool Checked
        {
            get
            {
                if (m_Hook == null) return false;
                if (m_Hook.CurrentTool != this.Name) return false;
                return true;
            }
        }

        public override string Message
        {
            get
            {
                Plugin.Application.IAppFormRef pAppFormRef = m_Hook as Plugin.Application.IAppFormRef;
                if (pAppFormRef != null)
                {
                    pAppFormRef.OperatorTips = base._Message;
                }
                return base._Message;
            }
        }

        public override void ClearMessage()
        {
            Plugin.Application.IAppFormRef pAppFormRef = m_Hook as Plugin.Application.IAppFormRef;
            if (pAppFormRef != null)
            {
                pAppFormRef.OperatorTips = string.Empty;
            }
        }
        public override void OnCreate(Plugin.Application.IApplicationRef hook)
        {
            if (hook == null) return;
            m_Hook = hook as Plugin.Application.IAppArcGISRef;
            if (m_Hook.MapControl == null) return;

            _tool = new ControlsEditSelFeature();
            _cmd = _tool as ICommand;
            _cmd.OnCreate(m_Hook.MapControl);
        }

        public override void OnClick()
        {
            if (_tool == null || _cmd == null || m_Hook == null) return;
            if (m_Hook.MapControl == null) return;
            m_Hook.ArcGisMapControl.OnSelectionChanged+=new EventHandler(ArcGisMapControl_OnSelectionChanged);
            m_Hook.ArcGisMapControl.OnAfterScreenDraw+=new ESRI.ArcGIS.Controls.IMapControlEvents2_Ax_OnAfterScreenDrawEventHandler(ArcGisMapControl_OnAfterScreenDraw);
            m_Hook.MapControl.Map.ClearSelection();

            m_Hook.MapControl.CurrentTool = _tool;
            m_Hook.CurrentTool = this.Name;
        }

        private void ArcGisMapControl_OnSelectionChanged(object sender, EventArgs e)
        {
            if (m_Hook.CurrentTool != this.Name)
            {
                MoData.v_bVertexSelectionTracker = false;
            }
        }

        private void ArcGisMapControl_OnAfterScreenDraw(object sender, IMapControlEvents2_OnAfterScreenDrawEvent e)
        {
            //�жϽڵ�༭�е�ѡ��ڵ㣬����ˢ�»��ڵ�
            if (MoData.v_bVertexSelectionTracker == false) return;

            IEnumFeature pEnumFeature = m_Hook.MapControl.Map.FeatureSelection as IEnumFeature;
            if (pEnumFeature == null || m_Hook.MapControl.Map.SelectionCount!=1) return;
            pEnumFeature.Reset();
            IFeature pFeature = pEnumFeature.Next();
            if (pFeature == null) return;

            ModPublic.DrawEditSymbol(pFeature.Shape, m_Hook.MapControl.ActiveView);
        }
    }


//    ����: ѡ����󣬲������ͣ����ѡ��Ķ����ϣ������ƶ����󣻵�ֻѡ��һ������ʱ��
//          ˫��������ʾ�ڵ㣬���ҿ����ƶ��ڵ�;��ֻͨ����������ѡ�������ֻѡ��һ������
//          ������ѡ�����ڿ�Χ�ڵĶ��󶼱�ѡ��
//          �ȼ���Ctrl+C ���ơ�Ctrl+X ���С�Ctrl+V ճ����Esc ȡ����ջ�ճ�������ݡ�
//          Ctrl+R ����
    public class ControlsEditSelFeature : BaseTool
    {
        private IHookHelper m_hookHelper;
        private IMapControlDefault m_MapControl;

        private ControlsMoveSelFeature m_pControlsMoveSelFeature;

        //��ķ���
        public ControlsEditSelFeature()
        {
            base.m_category = "GeoCommon";
            base.m_caption = "EditSelFeature";
            base.m_message = "ѡ��༭";
            base.m_toolTip = "ѡ��༭";
            base.m_name = base.m_category + "_" + base.m_caption;
            try
            {
                base.m_cursor = new System.Windows.Forms.Cursor(GetType(), "Resources.EditSelect.cur");
            }
            catch (Exception eError)
            {
                //******************************************
                //guozheng added System Exception log
                if (SysCommon.Log.Module.SysLog == null)
                    SysCommon.Log.Module.SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
                SysCommon.Log.Module.SysLog.Write(eError);
                //******************************************
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
            //֧�ֿ�ݼ�
            m_MapControl.KeyIntercept = 1;  //esriKeyInterceptArrowKeys
        }

        public override void OnMouseDown(int Button, int Shift, int X, int Y)
        {
            if (Button != 1) return;
            MoData.v_bVertexSelectionTracker = false;

            //���õ�ѡ���ݲ�
            ISelectionEnvironment pSelectEnv = new SelectionEnvironmentClass();
            double Length = ModPublic.ConvertPixelsToMapUnits(m_hookHelper.ActiveView, pSelectEnv.SearchTolerance);

            IPoint pPoint = m_hookHelper.ActiveView.ScreenDisplay.DisplayTransformation.ToMapPoint(X, Y);
            IGeometry pGeometry = pPoint as IGeometry;
            ITopologicalOperator pTopo = pGeometry as ITopologicalOperator;
            IGeometry pBuffer = pTopo.Buffer(Length);

            //�����ܱ���ཻ����ᱻѡȡ
            pGeometry = m_MapControl.TrackRectangle() as IGeometry;
            bool bjustone = true;
            if (pGeometry != null)
            {
                if (pGeometry.IsEmpty)
                {
                    pGeometry = pBuffer;
                }
                else
                {
                    bjustone = false;
                }
            }
            else
            {
                pGeometry = pBuffer;
            }

            UID pUID = new UIDClass();
            pUID.Value = "{40A9E885-5533-11d0-98BE-00805F7CED21}";   //UID for IFeatureLayer
            IEnumLayer pEnumLayer=m_MapControl.Map.get_Layers(pUID, true);
            pEnumLayer.Reset();
            ILayer pLayer = pEnumLayer.Next();
            while(pLayer!=null)
            {
                if (pLayer.Visible == false)
                {
                    pLayer = pEnumLayer.Next();
                    continue;
                }
                IFeatureLayer pFeatureLayer = pLayer as IFeatureLayer;
                if (pFeatureLayer.Selectable == false)
                {
                    pLayer = pEnumLayer.Next();
                    continue;
                }

                GetSelctionSet(pFeatureLayer, pGeometry, bjustone, Shift);

                pLayer = pEnumLayer.Next();
            }

            //����Mapѡ�����仯�¼�
            ISelectionEvents pSelectionEvents = m_hookHelper.FocusMap as ISelectionEvents;
            pSelectionEvents.SelectionChanged();
            //ˢ��
            m_hookHelper.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeoSelection, null, m_hookHelper.ActiveView.Extent);

            //�����ѡ�񼯵ķ�Χ����TOOL��Ϊ�ƶ�����
            if (ModPublic.MouseOnSelection(pPoint, m_hookHelper.ActiveView) == true)
            {
                m_pControlsMoveSelFeature = new ControlsMoveSelFeature();
                m_pControlsMoveSelFeature.OnCreate(m_hookHelper.Hook);
                m_MapControl.CurrentTool = m_pControlsMoveSelFeature as ITool;
            }
        }
        private void GetSelctionSet(IFeatureLayer pFeatureLayer, IGeometry pGeometry, bool bjustone, int Shift)
        {
            IFeatureClass pFeatureClass = pFeatureLayer.FeatureClass;
            //û�����༭�Ĳ���ѡ��
            IDataset pDataset = pFeatureClass as IDataset;
            IWorkspaceEdit pWorkspaceEdit = pDataset.Workspace as IWorkspaceEdit;
            if (!pWorkspaceEdit.IsBeingEdited()) return;
            switch (Shift)
            {
                case 1:   //����ѡ������
                    ModPublic.GetSelctionSet(pFeatureLayer, pGeometry, pFeatureClass, esriSelectionResultEnum.esriSelectionResultAdd, bjustone);
                    break;
                case 4:   //����ѡ������
                    ModPublic.GetSelctionSet(pFeatureLayer, pGeometry, pFeatureClass, esriSelectionResultEnum.esriSelectionResultSubtract, bjustone);
                    break;
                case 2:
                    ModPublic.GetSelctionSet(pFeatureLayer, pGeometry, pFeatureClass, esriSelectionResultEnum.esriSelectionResultXOR, bjustone);
                    break;
                default:   //�½�ѡ������
                    ModPublic.GetSelctionSet(pFeatureLayer, pGeometry, pFeatureClass, esriSelectionResultEnum.esriSelectionResultNew, bjustone);
                    break;
            }
        }


        public override void OnMouseUp(int Button, int Shift, int X, int Y)
        {

        }

        public override void OnMouseMove(int Button, int Shift, int X, int Y)
        {
            //�����ѡ�񼯵ķ�Χ����TOOL��Ϊ�ƶ�����
            IPoint pPnt = m_hookHelper.ActiveView.ScreenDisplay.DisplayTransformation.ToMapPoint(X, Y);
            if (ModPublic.MouseOnSelection(pPnt, m_hookHelper.ActiveView) == true)
            {
                if (m_pControlsMoveSelFeature == null)
                {
                    m_pControlsMoveSelFeature = new ControlsMoveSelFeature();
                    m_pControlsMoveSelFeature.OnCreate(m_hookHelper.Hook);
                    m_MapControl.CurrentTool = m_pControlsMoveSelFeature as ITool;
                }
                else
                {
                    m_MapControl.CurrentTool = m_pControlsMoveSelFeature as ITool;
                }
            }
            else
            {
                m_MapControl.CurrentTool = this as ITool;
            }
        }

        public override void OnDblClick()
        {
           
        }

        //���߲�����ʱ�ͷŴ���ȱ���
        public override bool Deactivate()
        {
            return true;
        }
        #endregion
    }

//    ����: �ƶ����󣬵����ͣ����ѡ��Ķ���֮�⣬����ѡ����󣻵�ֻ��һ������ʱ��˫������
//          ��ʾ�ڵ㣬���ҿ����ƶ��ڵ㣬�ȼ���Ctrl+C ���ơ�Ctrl+X ���С�Ctrl+V ճ����Esc
//          ȡ����ջ�ճ�������ݡ�Ctrl+R ����
    public class ControlsMoveSelFeature : BaseTool
    {
        private IHookHelper m_hookHelper;
        private IMapControlDefault m_MapControl;

        private bool m_bMouseDown;
        private IPoint m_pPtStart;     //�ƶ�ǰ�������
        private ISet m_pMoveSet;
        private INewLineFeedback m_pNewLineFeedback;
        private IMoveGeometryFeedback m_pMoveGeometryFeedback;

        public ControlsMoveSelFeature()
        {
            base.m_category = "GeoCommon";
            base.m_caption = "MoveSelFeature";
            base.m_message = "�ƶ�ѡ��Ҫ��";
            base.m_toolTip = "�ƶ�ѡ��Ҫ��";
            base.m_name = base.m_category + "_" + base.m_caption;
            try
            {
                base.m_cursor = new System.Windows.Forms.Cursor(GetType(), "Resources.EditMove.cur");
            }
            catch
            {

            }
        }

        #region Overriden Class Methods

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
        }

        public override void OnMouseDown(int Button, int Shift, int X, int Y)
        {
            if (Button != 1 || m_MapControl.Map.SelectionCount==0) return;
            MoData.v_bVertexSelectionTracker = false;

            m_pNewLineFeedback = new NewLineFeedbackClass();
            m_pMoveGeometryFeedback = new MoveGeometryFeedbackClass();
            IDisplayFeedback pDisplayFeedback = m_pMoveGeometryFeedback as IDisplayFeedback;
            pDisplayFeedback.Display = m_hookHelper.ActiveView.ScreenDisplay;

            m_pPtStart = m_hookHelper.ActiveView.ScreenDisplay.DisplayTransformation.ToMapPoint(X, Y);
            m_pNewLineFeedback.Display = m_hookHelper.ActiveView.ScreenDisplay;
            m_pNewLineFeedback.Start(m_pPtStart);

            //ֻˢ��ѡ�еĵ���
            IEnumFeature pEnumFeature = m_MapControl.Map.FeatureSelection as IEnumFeature;
            IInvalidArea pInvalidArea = new InvalidAreaClass();
            pInvalidArea.Display = m_hookHelper.ActiveView.ScreenDisplay;
            pInvalidArea.Add(pEnumFeature);

            pEnumFeature.Reset();
            IFeature pFeature = pEnumFeature.Next();
            m_pMoveSet = new SetClass();
            while (pFeature != null)
            {
                //'�ж�ѡ�е�Ҫ���Ƿ������˱༭
                IFeatureClass pFeatureClass = pFeature.Class as IFeatureClass;
                IDataset pDataset = pFeatureClass as IDataset;
                IWorkspaceEdit pWSEdit = pDataset.Workspace as IWorkspaceEdit;
                if (pWSEdit != null)
                {
                    if (pWSEdit.IsBeingEdited())
                    {
                        m_pMoveGeometryFeedback.AddGeometry(pFeature.Shape);
                        m_pMoveSet.Add(pFeature);
                    }
                }
                pFeature = pEnumFeature.Next();
            }

            m_pMoveGeometryFeedback.Start(m_pPtStart);

            m_bMouseDown = true;
        }

        public override void OnMouseUp(int Button, int Shift, int X, int Y)
        {
            if (Button != 1) return;
            if (m_MapControl.Map.SelectionCount == 0 || m_pMoveGeometryFeedback == null || m_pNewLineFeedback == null || m_pPtStart == null) return;
            IPoint pPtFinal=m_hookHelper.ActiveView.ScreenDisplay.DisplayTransformation.ToMapPoint(X, Y);

            m_pNewLineFeedback.Stop();
            m_pMoveGeometryFeedback.MoveTo(pPtFinal);

            //�����ƶ����ƶ�����Ҫ��
            if (m_pPtStart.X != pPtFinal.X || m_pPtStart.Y != pPtFinal.Y)
            {
                MoveAllFeature(m_pMoveSet, m_pPtStart, pPtFinal, MoData.v_CurWorkspaceEdit, m_MapControl.Map);
                m_MapControl.ActiveView.Refresh();
            }

            m_bMouseDown = false;
        }

        public override void OnMouseMove(int Button, int Shift, int X, int Y)
        {
            //�����ѡ�񼯵ķ�Χ��Ϊѡ����
            IPoint pPnt = m_hookHelper.ActiveView.ScreenDisplay.DisplayTransformation.ToMapPoint(X, Y);
            if (ModPublic.MouseOnSelection(pPnt, m_hookHelper.ActiveView) == false && m_bMouseDown == false)
            {//����겻��ѡ��Ķ����ϣ�Ϊѡ����
                ControlsEditSelFeature clsSelectFeature = new ControlsEditSelFeature();
                clsSelectFeature.OnCreate(m_hookHelper.Hook);
                clsSelectFeature.OnClick();
                m_MapControl.CurrentTool = clsSelectFeature as ITool;
                return;
            }

            //�����ѡ��Ҫ�ؽڵ���ʱΪ�ڵ��ƶ�����
            if(m_MapControl.Map.SelectionCount==1)
            {
                IEnumFeature pEnumFeature=m_MapControl.Map.FeatureSelection as IEnumFeature;
                pEnumFeature.Reset();
                IFeature pFeature=pEnumFeature.Next();
                if (ModPublic.MouseOnFeatureVertex(pPnt, pFeature, m_hookHelper.ActiveView) == true && MoData.v_bVertexSelectionTracker ==true)
                {
                    ControlsMoveVertex pControlsMoveVertex = new ControlsMoveVertex();
                    pControlsMoveVertex.OnCreate(m_hookHelper.Hook);
                    pControlsMoveVertex.OnClick();
                    m_MapControl.CurrentTool = pControlsMoveVertex as ITool;
                    return;
                }
            }

            //��׽�ڵ�
            if (MoData.v_bSnapStart)
            {
                ModPublic.SnapPoint(pPnt, m_hookHelper.ActiveView);
            }

            if (Button != 1) return;
            //�����ƶ�
            if (m_MapControl.Map.SelectionCount > 0 && m_pMoveGeometryFeedback != null && m_pNewLineFeedback!=null)
            {
                m_pMoveGeometryFeedback.MoveTo(pPnt);
                m_pNewLineFeedback.MoveTo(pPnt);
            }
        }

        public override void OnDblClick()
        {
            IEnumFeature pEnumFeature = m_MapControl.Map.FeatureSelection as IEnumFeature;
            if (pEnumFeature == null) return;
            pEnumFeature.Reset();
            IFeature pFeature = pEnumFeature.Next();
            if (pFeature == null) return;

            ModPublic.DrawEditSymbol(pFeature.Shape, m_hookHelper.ActiveView);
            MoData.v_bVertexSelectionTracker = true;
        }

        public override bool Deactivate()
        {
            return true;
        }

        #endregion

        //�ƶ�����Ҫ��
        private void MoveAllFeature(ISet pALLMoveSet, IPoint pPtStart, IPoint pPtFinal, IWorkspaceEdit pCurWorkspaceEdit,IMap pMap)
        {
            pCurWorkspaceEdit.StartEditOperation();

            ILine pLine = new LineClass();
            pLine.PutCoords(pPtStart, pPtFinal);
            //���ռ�ο������ƶ�����
            pLine.SpatialReference = pMap.SpatialReference;

            pALLMoveSet.Reset();
            IFeatureEdit pFeatureEdit = pALLMoveSet.Next() as IFeatureEdit;
            while (pFeatureEdit != null)
            {
                pFeatureEdit.MoveSet(pALLMoveSet, pLine);
                pFeatureEdit = pALLMoveSet.Next() as IFeatureEdit;
            }

            pCurWorkspaceEdit.StopEditOperation();

        }
              
    }


    //����: �ƶ��ڵ�,ָ�벻�ڽڵ���˫�����ʱ���ص�ѡ��״̬
    public class ControlsMoveVertex : BaseTool
    {
        private IHookHelper m_hookHelper;
        private IMapControlDefault m_MapControl;

        private bool m_bMouseDown;

        private IFeature m_pFeature;
        private double m_dblTolearance;
        private int m_HitSegmentIndex;            //ѡ�еĽڵ�
        private IVertexFeedback m_pVertexFeed;    //�ڵ��ƶ���

        private IPoint m_pSnapPoint;              //������׽ʱ�ƶ��Բ�׽��Ϊ׼

        public ControlsMoveVertex()
        {
            base.m_category = "GeoCommon";
            base.m_caption = "MoveVertex";
            base.m_message = "�ƶ�Ҫ�ؽڵ�";
            base.m_toolTip = "�ƶ�Ҫ�ؽڵ�";
            base.m_name = base.m_category + "_" + base.m_caption;
            try
            {
                base.m_cursor = new System.Windows.Forms.Cursor(GetType(), "Resources.MoveVertex.cur");
            }
            catch
            {

            }
        }

        #region Overriden Class Methods

        public override bool Enabled
        {
            get
            {
                if (m_MapControl.Map.SelectionCount != 1) return false;
                IEnumFeature pSelected = m_MapControl.Map.FeatureSelection as IEnumFeature;
                IFeature pFeature = pSelected.Next();
                IFeatureClass pFeatClass=pFeature.Class as IFeatureClass;
                IDataset pDataset = pFeatClass as IDataset;
                IWorkspaceEdit pWSEdit = pDataset.Workspace as IWorkspaceEdit;
                //�����ǰѡ��Ҫ������ͼ�������˱༭���Ҳ��ǵ�״Ҫ�أ������ýڵ�༭����
                if (pFeatClass.FeatureType == esriFeatureType.esriFTSimple)
                {
                    if (pFeature.Shape.Dimension != esriGeometryDimension.esriGeometry0Dimension && pWSEdit.IsBeingEdited())
                    {
                        return true;
                    }
                }

                return false;
            }
        }

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

            IEnumFeature pEnumFeature = m_MapControl.Map.FeatureSelection as IEnumFeature;
            if (pEnumFeature == null || m_MapControl.Map.SelectionCount != 1) return;
            pEnumFeature.Reset();
            IFeature pFeature = pEnumFeature.Next();
            if (pFeature == null) return;

            //����ͼ�θ��ڵ�
            if (MoData.v_bVertexSelectionTracker == true)
            {
                ModPublic.DrawEditSymbol(pFeature.Shape, m_MapControl.ActiveView);
            }

            //���õ�ѡ�ݲ� 
            ISelectionEnvironment pSelectEnv = new SelectionEnvironmentClass();
            m_dblTolearance = ModPublic.ConvertPixelsToMapUnits(m_hookHelper.ActiveView, pSelectEnv.SearchTolerance);

            m_pFeature = pFeature;
        }

        public override void OnMouseDown(int Button, int Shift, int X, int Y)
        {
            if (Button != 1 || m_pFeature==null) return;

            IPoint pPnt = m_MapControl.ActiveView.ScreenDisplay.DisplayTransformation.ToMapPoint(X, Y);

            //��ȡѡ�еĽڵ�
            IHitTest pHitTest = m_pFeature.Shape as IHitTest;
            IPoint pHitPoint = new PointClass();
            double dblHitDistance=0; 
            int lPart=0;
            bool bRight=false;
            bool bHitTest=pHitTest.HitTest(pPnt,m_dblTolearance,esriGeometryHitPartType.esriGeometryPartVertex,pHitPoint,ref dblHitDistance,ref lPart,ref m_HitSegmentIndex,ref bRight);
            if (bHitTest == false) return;
            if (m_HitSegmentIndex == -1) return;   //δѡ�нڵ�

            ISegmentCollection pSegmentCollection = m_pFeature.Shape as ISegmentCollection;
            m_pVertexFeed = new VertexFeedbackClass();
            m_pVertexFeed.Display = m_MapControl.ActiveView.ScreenDisplay;

            //����Ƿ�����һ��
            if (m_HitSegmentIndex == 0)
            {
                //����ǵ�һ������Polygon�������һ�� (FromPoint as anchor)
                if (m_pFeature.Shape is IPolygon)
                {
                    m_pVertexFeed.AddSegment(pSegmentCollection.get_Segment(pSegmentCollection.SegmentCount - 1), true);
                }
            }
            else
            {
                //������ǵ�һ�㣬������һ��(FromPoint as anchor)
                m_pVertexFeed.AddSegment(pSegmentCollection.get_Segment(m_HitSegmentIndex - 1), true);
            }

            //����Ƿ������һ���ڵ�
            if (m_HitSegmentIndex == pSegmentCollection.SegmentCount)
            {
                //����ǵ�����һ���ڵ�,������polygon���ӵ�һ��
                if (m_pFeature.Shape is IPolygon)
                {
                    m_pVertexFeed.AddSegment(pSegmentCollection.get_Segment(0), false);
                }
            }
            else
            {
                //����������һ���ڵ㣬�ͼ���һ��(ToPoint as anchor)
                m_pVertexFeed.AddSegment(pSegmentCollection.get_Segment(m_HitSegmentIndex), false);
            }

            m_bMouseDown = true;
        }

        public override void OnMouseUp(int Button, int Shift, int X, int Y)
        {
            if (Button != 1 || m_pFeature == null || m_pVertexFeed == null || m_HitSegmentIndex == -1) return;

            IPoint pPnt = m_MapControl.ActiveView.ScreenDisplay.DisplayTransformation.ToMapPoint(X, Y);
            //���������׽���Բ�׽���ĵ�Ϊ׼
            if (MoData.v_bSnapStart && m_pSnapPoint!=null)
            {
                pPnt=m_pSnapPoint;
            }

            ISegmentCollection pSegmentCollection = m_pFeature.Shape as ISegmentCollection;
            

            //����Ƿ�����һ��
            if (m_HitSegmentIndex == 0)
            {
                //����ǵ�һ������Polygon���������һ��
                if (m_pFeature.Shape is IPolygon)
                {
                    pPnt.Z = pSegmentCollection.get_Segment(pSegmentCollection.SegmentCount - 1).ToPoint.Z;
                    pSegmentCollection.get_Segment(pSegmentCollection.SegmentCount - 1).ToPoint = pPnt;
                }
            }
            else
            {
                //������ǵ�һ�㣬������һ��(FromPoint as anchor)
                pPnt.Z = pSegmentCollection.get_Segment(m_HitSegmentIndex - 1).ToPoint.Z;
                pSegmentCollection.get_Segment(m_HitSegmentIndex - 1).ToPoint = pPnt;
            }

            //����Ƿ������һ���ڵ�
            if (m_HitSegmentIndex == pSegmentCollection.SegmentCount)
            {
                //����ǵ�����һ���ڵ�,������polygon�����µ�һ��
                if (m_pFeature.Shape is IPolygon)
                {
                    pPnt.Z = pSegmentCollection.get_Segment(0).FromPoint.Z;
                    pSegmentCollection.get_Segment(0).FromPoint = pPnt;
                }
            }
            else
            {
                //����������һ���ڵ㣬������һ��(ToPoint as anchor)
                pPnt.Z = pSegmentCollection.get_Segment(m_HitSegmentIndex).FromPoint.Z;
                pSegmentCollection.get_Segment(m_HitSegmentIndex).FromPoint = pPnt;
            }

            StoreFeature(m_pFeature, pSegmentCollection, MoData.v_CurWorkspaceEdit);

            m_MapControl.ActiveView.Refresh();
            m_pVertexFeed = null;
            m_bMouseDown = false;
            m_HitSegmentIndex = 0;
        }

        public override void OnMouseMove(int Button, int Shift, int X, int Y)
        {
            if (m_pFeature == null) return;
            IPoint pPnt = m_MapControl.ActiveView.ScreenDisplay.DisplayTransformation.ToMapPoint(X, Y);

            //�����ѡ�񼯵ķ�Χ��Ϊѡ����
            if (ModPublic.MouseOnSelection(pPnt, m_hookHelper.ActiveView) == false && m_bMouseDown==false)
            {//����겻��ѡ��Ķ����ϣ�Ϊѡ����
                ControlsEditSelFeature clsSelectFeature = new ControlsEditSelFeature();
                clsSelectFeature.OnCreate(m_hookHelper.Hook);
                clsSelectFeature.OnClick();
                m_MapControl.CurrentTool = clsSelectFeature as ITool;
                return;
            }
            //��Χ��Ϊ�ƶ�Ҫ�ع����Ҳ���Ҫ�ؽڵ���
            else if (ModPublic.MouseOnFeatureVertex(pPnt, m_pFeature, m_hookHelper.ActiveView) == false && m_bMouseDown == false)    
            {
                ControlsMoveSelFeature pControlsMoveSelFeature = new ControlsMoveSelFeature();
                pControlsMoveSelFeature.OnCreate(m_hookHelper.Hook);
                pControlsMoveSelFeature.OnClick();
                m_MapControl.CurrentTool = pControlsMoveSelFeature as ITool;
                return;
            }


            if (m_pVertexFeed==null) return;
            //��׽�ڵ�
            if (MoData.v_bSnapStart)
            {
                m_pSnapPoint=ModPublic.SnapPoint(pPnt, m_hookHelper.ActiveView);
            }

            IHitTest pHitTest = m_pFeature.Shape as IHitTest;
            IPoint pHitPoint = new PointClass();
            double dblHitDistance = 0;
            int lPart = 0;
            int intHitSegmentIndex = 0;
            bool bRight = false;
            bool bHitTest = pHitTest.HitTest(pPnt, m_dblTolearance, esriGeometryHitPartType.esriGeometryPartVertex, pHitPoint, ref dblHitDistance, ref lPart, ref intHitSegmentIndex, ref bRight);

            if (m_pSnapPoint != null && MoData.v_bSnapStart)
            {
                m_pVertexFeed.MoveTo(m_pSnapPoint);
            }
            else
            {
                m_pVertexFeed.MoveTo(pPnt);
            }
        }

        public override void OnDblClick()
        {

        }

        public override bool Deactivate()
        {
            return true;
        }

        #endregion

        //�洢Feature
        private void StoreFeature(IFeature pFeature, ISegmentCollection pSegcol, IWorkspaceEdit pCurWorkspaceEdit)
        {
            pCurWorkspaceEdit.StartEditOperation();

            pFeature.Shape = pSegcol as IGeometry;
            pFeature.Store();
            pCurWorkspaceEdit.StopEditOperation();
        }
    }

}
