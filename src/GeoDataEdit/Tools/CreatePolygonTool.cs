using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Collections.Generic;

using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geometry;

namespace GeoDataEdit
{
    /// <summary>
    /// Summary description for CreatePolygon.
    /// </summary>
    [Guid ( "bffd8486-0fc3-46e8-a929-594221e0a063" )]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("GeoControls.CreatePolygonTool")]
    public sealed class CreatePolygonTool : BaseTool, ESRI.ArcGIS.SystemUI.IOperation, ICommandEx
    {
        #region COM Registration Function(s)
        [ComRegisterFunction()]
        [ComVisible(false)]
        static void RegisterFunction(Type registerType)
        {
            // Required for ArcGIS Component Category Registrar support
            ArcGISCategoryRegistration(registerType);

            //
            // TODO: Add any COM registration code here
            //
        }

        [ComUnregisterFunction()]
        [ComVisible(false)]
        static void UnregisterFunction(Type registerType)
        {
            // Required for ArcGIS Component Category Registrar support
            ArcGISCategoryUnregistration(registerType);

            //
            // TODO: Add any COM unregistration code here
            //
        }

        #region ArcGIS Component Category Registrar generated code
        /// <summary>
        /// Required method for ArcGIS Component Category registration -
        /// Do not modify the contents of this method with the code editor.
        /// </summary>
        private static void ArcGISCategoryRegistration(Type registerType)
        {
            string regKey = string.Format("HKEY_CLASSES_ROOT\\CLSID\\{{{0}}}", registerType.GUID);
            ControlsCommands.Register(regKey);

        }
        /// <summary>
        /// Required method for ArcGIS Component Category unregistration -
        /// Do not modify the contents of this method with the code editor.
        /// </summary>
        private static void ArcGISCategoryUnregistration(Type registerType)
        {
            string regKey = string.Format("HKEY_CLASSES_ROOT\\CLSID\\{{{0}}}", registerType.GUID);
            ControlsCommands.Unregister(regKey);

        }

        #endregion
        #endregion

        #region Variable

        private IHookHelper m_pHookHelper;                                         //����

        private ESRI.ArcGIS.Carto.IMap m_pMap;                                     //��ǰ��ͼ
        private ESRI.ArcGIS.Carto.IActiveView m_pActiveView;                       //������
        private ESRI.ArcGIS.Carto.IActiveViewEvents_Event m_pActiveViewEvent;      //ˢ���¼�

        private DrawLineTrack m_pDrawLineTrack;                 //׷����
        //private GeoControls.Editor.EditTools.OperateWorkSpace m_pOperateWorkSpace; //���ݿ������

        private Boolean m_blCanEdit;                                               //�Ƿ���Ա༭
        private Boolean m_blDrawOK;                                                //�Ƿ񻭳ɹ�

        private ClsEditorMain m_clsEditorMain;                  //ȫ�ֱ༭��������

        private bool m_blCanMove;                                                  //�Ƿ��ƶ�����

        private string m_sInputType;                                               //��ǰ���ܵ���������
        private string m_sTip;                                                     //������ʾ
        private string m_sKeyword;                                                 // �����������
        private bool isInSubCommand;

        #endregion

        public CreatePolygonTool()
        {
            base.m_category = "NJGIS";
            base.m_caption = "����";
            base.m_message = "����";
            base.m_toolTip = "����";
            base.m_name = base.m_category + "_" + base.m_caption;

            try
            {
                //string bitmapResourceName = "Resources." + GetType().Name + ".bmp";
                //base.m_bitmap = new Bitmap(GetType(), bitmapResourceName);
                string curResourceName = "Resources.CreatePolygon.cur";
                base.m_cursor = new System.Windows.Forms.Cursor(GetType(), curResourceName);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message, "Invalid Bitmap");
            }

            this.m_pHookHelper = new HookHelperClass();
            this.m_pDrawLineTrack = new DrawLineTrack();
            //this.m_pOperateWorkSpace = new GeoControls.Editor.EditTools.OperateWorkSpace();
            this.m_blCanEdit = false;
            this.m_blCanMove = true;

            this.m_sInputType = "";
            this.m_sTip = "";
            this.m_sKeyword = "";
        }

        ~CreatePolygonTool()
        {
            this.m_pHookHelper = null;
            //this.m_pDrawLineTrack = null;
            //this.m_pOperateWorkSpace = null;
        }

        public void setClsEditorMain ( ClsEditorMain clsEditorMain )
        {
            this.m_clsEditorMain = clsEditorMain;
        }

        #region Overriden Class Methods

        public override void OnCreate ( object hook )
        {
            this.m_pHookHelper.Hook = hook;

            if (this.m_clsEditorMain == null) return;

            this.m_clsEditorMain.HookHelper = this.m_pHookHelper;
        }

        public override bool Enabled
        {
            get
            {
                if ( m_clsEditorMain.EditFeatureLayer != null )
                {
                    if ( ( m_clsEditorMain.EditFeatureLayer as IFeatureLayer ).FeatureClass.FeatureType == esriFeatureType.esriFTSimple &&
                        ( m_clsEditorMain.EditFeatureLayer as IFeatureLayer ).FeatureClass.ShapeType == esriGeometryType.esriGeometryPolygon &&
                        ( m_clsEditorMain.EditWorkspace as IWorkspaceEdit ).IsBeingEdited () )
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public override void OnClick()
        {
            DrawTypeConstant nDrawType = DrawTypeConstant.CommonPolygon;

            this.m_blCanEdit = true;

            this.m_pMap = this.m_pHookHelper.FocusMap;
            if (this.m_pMap == null) return;

            this.m_pMap.ClearSelection();
            this.m_pActiveView = this.m_pMap as IActiveView;

            m_pActiveView.ScreenDisplay.Invalidate(m_pActiveView.Extent.Envelope, true, -1);
            m_pActiveView.ScreenDisplay.UpdateWindow();
            try
            {
                m_pActiveViewEvent = (IActiveViewEvents_Event) m_pMap;
                m_pActiveViewEvent.AfterDraw += new IActiveViewEvents_AfterDrawEventHandler ( m_pActiveViewEvent_AfterDraw );
            }
            catch ( Exception ex )
            {
            }
            if (this.m_clsEditorMain == null) return;

            //this.m_blCanEdit = m_pOperateWorkSpace.InitOperater(nDrawType, ref m_pMap, m_clsEditorMain);
            if (this.m_blCanEdit == false) return;

            this.m_blCanEdit = m_pDrawLineTrack.InitTrack(nDrawType, ref m_pMap, m_clsEditorMain);

            if (this.m_blCanEdit == false)
            {
                System.Windows.Forms.MessageBox.Show("����������ʼ��ʧ�ܣ�", "ϵͳ��ʾ", MessageBoxButtons.OK);
                return;
            }
            isInSubCommand = false;
            SetNextStep("0");           
        }
    
        public override bool Deactivate ()
        {
            if ( !isInSubCommand )
            {
                m_pDrawLineTrack.m_pDrawedPoints = null;
                m_pDrawLineTrack.m_pGeometryBag = null;
                m_pActiveView.Refresh ();
            }
            return true;
        }

        public override void OnMouseDown ( int Button , int Shift , int X , int Y )
        {
            //if (!this.m_blCanEdit) return;

            int nPointCount;
            nPointCount = this.m_pDrawLineTrack.GetPointCount();

            int nRing;
            nRing = this.m_pDrawLineTrack.GetPartCount();

            if (Button == 2)
            {

                int nMenuType;

                if (nPointCount >= 3)
                    nMenuType = 1;      //�ɷ��
                else if (nPointCount <= 0 && nRing <= 0)
                    nMenuType = 2;     //����
                else
                    nMenuType = 3;     //�ɻ���

                Dictionary<string, bool> ToolRightMenu = new Dictionary<string, bool>();

                if (nMenuType == 1)
                    ToolRightMenu.Add("���", true);
                else
                    ToolRightMenu.Add("���", false);

                if (nMenuType == 2)
                {
                    ToolRightMenu.Add("����", false);
                    ToolRightMenu.Add("����", false);
                }
                else
                {
                    ToolRightMenu.Add("����", true);
                    ToolRightMenu.Add("����", true);
                }

                ToolRightMenu.Add("ƽ��", true);
                ToolRightMenu.Add("�Ŵ�", true);
                ToolRightMenu.Add("��С", true);

                this.m_blCanMove = false;
                this.m_pDrawLineTrack.MouseMove(X, Y);
                //ModPublic.PublicClass.OnPopupMenuEventArgs(ToolRightMenu);
                m_clsEditorMain.MethodInfo_PopupMenuEventArgs.Invoke ( m_clsEditorMain.frmMain , new object[] { ToolRightMenu } );
                return;
            }

            if ( Button == 1 )
            {
                object obj = Type.Missing;
                try
                {
                    ESRI.ArcGIS.Geometry.IPoint pPoint = this.m_pDrawLineTrack.AddPoint(X, Y, false);
                    nPointCount = m_pDrawLineTrack.GetPointCount();
                    nRing = m_pDrawLineTrack.GetPartCount();

                    string sXY;
                    sXY = m_pDrawLineTrack.GetLastPointXYStr();
                    //ModPublic.PublicClass.OnGetCommandTipEventArgs(OperationParam.CheckUpOut, sXY, "");
                    m_clsEditorMain.MethodInfo_GetCommandTipEventArgs.Invoke ( m_clsEditorMain.frmMain , new object[] { OperationParam.CheckUpOut , sXY , "" } );

                    if (nPointCount <= 2)
                    {
                        if (nPointCount == 0)
                            if (nRing <= 0)
                                SetNextStep("0");
                            else
                               SetNextStep("4");
                        else if (nPointCount==1)
                            SetNextStep("1");
                        else
                            SetNextStep("2");
                    }
                    else
                        SetNextStep("3");
                }
                catch ( Exception ee )
                {
                    m_pActiveView.PartialRefresh ( esriViewDrawPhase.esriViewForeground , null , null );
                }
            }
        }

        public override void OnMouseMove ( int Button , int Shift , int X , int Y )
        {
            ////if (!this.m_blCanEdit) return;
            //if (!m_blCanMove) return;
            this.m_pDrawLineTrack.MouseMove(X, Y);
        }

        public override void OnDblClick ()
        {
            this.m_pDrawLineTrack.SetDo(true);
            ESRI.ArcGIS.Geometry.IGeometry pGeometry;
            pGeometry = this.m_pDrawLineTrack.GetDrawGeometry(0);
        }
        
        public override void OnKeyDown ( int keyCode , int Shift )
        {
            //ͨ���ո�ʹ�����л�ý���

                //ModPublic.PublicClass.OnSetFocusEventArgs();
                m_clsEditorMain.MethodInfo_SetFocusEventArgs.Invoke ( m_clsEditorMain.frmMain , new object[] { keyCode , Shift } );
        }

        #endregion

        private void m_pActiveViewEvent_AfterDraw ( IDisplay Display , esriViewDrawPhase phase )
        {
            //if (this.m_blCanEdit == false) return;
            if (this.m_pDrawLineTrack == null) return;
            this.m_pDrawLineTrack.ReDraw(false);
        }
 
        #region ICommandEx ��Ա
        public bool GetInputMsg(string sType, string sInfo)
        {
            if (this.m_blCanEdit == false) return false;

            string sXY;
            int nPointCount;
            int nRing;

            if (sType == OperationParam.CheckChar)
            {
                if (sInfo == OperationParam.KeyEsc)
                {
                    this.m_pDrawLineTrack.GiveUpDraw();
                    //ModPublic.PublicClass.OnGetCommandTipEventArgs(OperationParam.KeyEsc, "", "");  //�õ�ǰ��Ϊ��
                    m_clsEditorMain.MethodInfo_GetCommandTipEventArgs.Invoke ( m_clsEditorMain.frmMain , new object[] { OperationParam.KeyEsc , "" , "" } );
                    return true;
                }
                else if (sInfo == OperationParam.KeyUndo)
                {
                    if (this.m_pDrawLineTrack.GoBackaPoint() == true)
                    {
                        sXY = this.m_pDrawLineTrack.GetLastPointXYStr();
                        //ModPublic.PublicClass.OnGetCommandTipEventArgs(OperationParam.CheckUpdate, sXY, "");       //���õ�ǰ��
                        m_clsEditorMain.MethodInfo_GetCommandTipEventArgs.Invoke ( m_clsEditorMain.frmMain , new object[] { OperationParam.CheckUpdate , sXY , "" } );
                        //ModPublic.PublicClass.OnRefreshCommandArgs("#���˳ɹ���");
                        m_clsEditorMain.MethodInfo_RefreshCommandArgs.Invoke ( m_clsEditorMain.frmMain , new object[] { "#���˳ɹ���" } );

                        nPointCount = this.m_pDrawLineTrack.GetPointCount();
                        nRing = this.m_pDrawLineTrack.GetPartCount();

                        if (nPointCount <= 2)
                        {
                            if (nPointCount == 0)
                                if (nRing <= 0)
                                    SetNextStep("0");
                                else
                                    SetNextStep("4");
                            else if (nPointCount == 1)
                                SetNextStep("1");
                            else
                                SetNextStep("2");
                        }
                        else
                            SetNextStep("3");
                    }
                    else
                    {
                        //ModPublic.PublicClass.OnGetCommandTipEventArgs(OperationParam.CheckEmpty , "", "");//�õ�ǰ��Ϊ��
                        m_clsEditorMain.MethodInfo_GetCommandTipEventArgs.Invoke ( m_clsEditorMain.frmMain , new object[] { OperationParam.CheckEmpty , "" , "" } );
                        //ModPublic.PublicClass.OnRefreshCommandArgs("#������Ч��");
                        m_clsEditorMain.MethodInfo_RefreshCommandArgs.Invoke ( m_clsEditorMain.frmMain , new object[] { "#������Ч��" } );
                        SetNextStep("0");
                    }
                }
                else if (sInfo == OperationParam.KeyLength)
                    SetNextStep(OperationParam.KeyLength  );
                else if (sInfo == OperationParam.KeyNextPart)
                {
                    this.m_pDrawLineTrack.NewPartBegin();
                    SetNextStep("4");
                }
                else if (sInfo == OperationParam.KeyEnter || sInfo == OperationParam.KeyClose )
                {
                    this.m_pDrawLineTrack.SetDo(true);
                    this.m_pDrawLineTrack.GetOperationStack().Do(this);

                    if (this.m_blDrawOK == true)
                        m_clsEditorMain.MethodInfo_RefreshCommandArgs.Invoke ( m_clsEditorMain.frmMain , new object[] { "#�����Ҫ�ػ�����ɣ�" } );
                        //ModPublic.PublicClass.OnRefreshCommandArgs("#�����Ҫ�ػ�����ɣ�");
                    else
                        m_clsEditorMain.MethodInfo_RefreshCommandArgs.Invoke ( m_clsEditorMain.frmMain , new object[] { "#�����Ҫ�ػ���ʧ�ܣ�" } );
                        //ModPublic.PublicClass.OnRefreshCommandArgs("#�����Ҫ�ػ���ʧ�ܣ�");

                    //ModPublic.PublicClass.OnGetCommandTipEventArgs(OperationParam.CheckEmpty, "", "");  //�õ�ǰ��Ϊ��
                    m_clsEditorMain.MethodInfo_GetCommandTipEventArgs.Invoke ( m_clsEditorMain.frmMain , new object[] { OperationParam.CheckEmpty , "" , "" } );
                    SetNextStep("0");
                }
                else if (sType == OperationParam.CheckPoint )
                {
                    string[] sSplit = sInfo.Split(',');

                    double x;
                    x = Convert.ToDouble(sSplit[0].Trim());

                    double y;
                    y = Convert.ToDouble(sSplit[1].Trim());

                    this.m_pDrawLineTrack.AddPoint(x, y, true);

                    nPointCount = this.m_pDrawLineTrack.GetPointCount();
                    nRing = this.m_pDrawLineTrack.GetPartCount();

                    if (nPointCount <= 2)
                    {
                        if (nPointCount == 0)
                            if (nRing <= 0)
                                SetNextStep("0");
                            else
                                SetNextStep("4");
                        else if (nPointCount == 1)
                            SetNextStep("1");
                        else
                            SetNextStep("2");
                    }
                    else
                        SetNextStep("3");
                }
                else if (sType == OperationParam.CheckLength)
                {
                    nPointCount = this.m_pDrawLineTrack.GetPointCount();
                    if (nPointCount >= 2)
                    {
                        double fLen;
                        fLen = Convert.ToDouble(sInfo);

                        if (fLen > 0)
                        {
                            this.m_pDrawLineTrack.LengthChange(fLen);

                            sXY = this.m_pDrawLineTrack.GetLastPointXYStr();
                            //ModPublic.PublicClass.OnGetCommandTipEventArgs( OperationParam.CheckUpdate, sXY, "");       //���õ�ǰ��
                            m_clsEditorMain.MethodInfo_GetCommandTipEventArgs.Invoke ( m_clsEditorMain.frmMain , new object[] { OperationParam.CheckUpdate , sXY , "" } );
                            nRing = this.m_pDrawLineTrack.GetPartCount();
                            
                            if (nPointCount == 2)
                                SetNextStep("2");
                            else
                                SetNextStep("3");
                        }
                    }
                }
               
            }
            return true;
        }

        public void GetMenuKey(string sKey)
        {
            string sToolKey = GetType().Namespace + "." + GetType().Name;
            string sName = ModForEdit.GetShortNameFromKey(sToolKey, this.m_clsEditorMain);
            switch (sKey)
            {
                case "���":
                    GetInputMsg(OperationParam.CheckChar, OperationParam.KeyEnter);
                    break;

                case "����":
                    GetInputMsg(OperationParam.CheckChar, OperationParam.KeyEsc);
                    break;

                case "����":
                    GetInputMsg(OperationParam.CheckChar, OperationParam.KeyUndo);
                    break;

                case "ƽ��":
                    isInSubCommand = true;
                    //ModPublic.PublicClass.OnGetCommandTipEventArgs(OperationParam.CheckSubBegin, sName + "," + OperationParam.KeyPan, "");
                    m_clsEditorMain.MethodInfo_GetCommandTipEventArgs.Invoke ( m_clsEditorMain.frmMain , new object[] { OperationParam.CheckSubBegin , sName + "," + OperationParam.KeyPan , "" } );
                    break;

                case "�Ŵ�":
                    isInSubCommand = true;
                    //ModPublic.PublicClass.OnGetCommandTipEventArgs(OperationParam.CheckSubBegin, sName + "," + OperationParam.KeyZoomIn, "");
                    m_clsEditorMain.MethodInfo_GetCommandTipEventArgs.Invoke ( m_clsEditorMain.frmMain , new object[] { OperationParam.CheckSubBegin , sName + "," + OperationParam.KeyZoomIn , "" } );
                    break;

                case "��С":
                    isInSubCommand = true;
                    //ModPublic.PublicClass.OnGetCommandTipEventArgs(OperationParam.CheckSubBegin, sName + "," + OperationParam.KeyZoomOut, "");
                    m_clsEditorMain.MethodInfo_GetCommandTipEventArgs.Invoke ( m_clsEditorMain.frmMain , new object[] { OperationParam.CheckSubBegin , sName + "," + OperationParam.KeyZoomOut , "" } );
                    break;

                default:

                    break;
            }
            this.m_blCanMove = true;
        }

        public void ResetMoveEvent()
        {
            this.m_blCanMove = true;
        }

         #endregion

        #region IOperation ��Ա

        public bool CanRedo
        {
            get
            {
                if (!this.m_blCanEdit) return false;

                ESRI.ArcGIS.Geodatabase.IWorkspaceEdit pWorkspaceEdit;
                //pWorkspaceEdit = this.m_pOperateWorkSpace.GetWorkspaceEdit();

                Boolean blHasRedos = false;
                //pWorkspaceEdit.HasRedos(ref blHasRedos);

                return blHasRedos;
            }

        }

        public bool CanUndo
        {
            get
            {
                if (!this.m_blCanEdit) return false;

                ESRI.ArcGIS.Geodatabase.IWorkspaceEdit pWorkspaceEdit;
                //pWorkspaceEdit = this.m_pOperateWorkSpace.GetWorkspaceEdit();

                Boolean blHasUndos = false;
                //pWorkspaceEdit.HasUndos(ref blHasUndos);

                return blHasUndos;
            }
        }

        public void Do()
        {
            if (this.m_pDrawLineTrack.GetDo() == true)
            {
                ESRI.ArcGIS.Geometry.IGeometry pGeometry;
                pGeometry = this.m_pDrawLineTrack.GetDrawGeometry(0);

                //this.m_blDrawOK = ModPublic.PublicClass.CreateFeature(pGeometry, m_pMap, true, m_clsEditorMain);
                this.m_pDrawLineTrack.GiveUpDraw();
            }
            else
                Redo();
        }

        public string MenuString
        {
            get { return "CreatePolygonTool"; }
        }

        public void Redo()
        {
            if (this.m_blCanEdit == false) return;

            ESRI.ArcGIS.Geodatabase.IWorkspaceEdit pWorkspaceEdit;
            //pWorkspaceEdit = this.m_pOperateWorkSpace.GetWorkspaceEdit();

            Boolean blHasRedos = false;
            //pWorkspaceEdit.HasRedos(ref blHasRedos);

            if (blHasRedos == true)
            {
                //pWorkspaceEdit.RedoEditOperation();

                ESRI.ArcGIS.Carto.IActiveView pActiveView;
                pActiveView = this.m_pMap as ESRI.ArcGIS.Carto.IActiveView;
                pActiveView.Refresh();

            }
        }

        public void Undo()
        {
            if (this.m_blCanEdit == false) return;

            ESRI.ArcGIS.Geodatabase.IWorkspaceEdit pWorkspaceEdit;
            //pWorkspaceEdit = this.m_pOperateWorkSpace.GetWorkspaceEdit();

            Boolean blHasUndos = false;
            //pWorkspaceEdit.HasUndos(ref blHasUndos);
            if (blHasUndos == true)
            {
                //pWorkspaceEdit.UndoEditOperation();

                ESRI.ArcGIS.Carto.IActiveView pActiveView;
                pActiveView = this.m_pMap as ESRI.ArcGIS.Carto.IActiveView;
                pActiveView.Refresh();
            }
        }

        #endregion

        private void SetNextStep(string sCondition)
        {
            //bool blFindStepInfo;
            //blFindStepInfo = ModForEdit.GetToolStepInfo("Polygon", sCondition, ref this.m_sInputType, ref   this.m_sTip, ref   this.m_sKeyword, this.m_clsEditorMain);
            //if (blFindStepInfo)
            //    //ModPublic.PublicClass.OnGetCommandTipEventArgs(this.m_sInputType, this.m_sTip, this.m_sKeyword);   
            //    m_clsEditorMain.MethodInfo_GetCommandTipEventArgs.Invoke ( m_clsEditorMain.frmMain , new object[] { this.m_sInputType , this.m_sTip , this.m_sKeyword } );
        }


    }
}
