using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using System.Runtime.InteropServices;

using System.Drawing;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.SystemUI;

namespace GeoDataEdit
{

    public class ClsEditorMain
    {

        private string m_CaseTableName;  ////�����汾������
        public string CaseTableName
        {
            get { return m_CaseTableName; }
            set { m_CaseTableName = value; }
        }

        private string m_CaseID;                             //����ID
        public string CaseID
        {
            get { return m_CaseID; }
            set { m_CaseID = value; }
        }

        private string m_VersionID;                             //����ID
        public string VersionID
        {
            get { return m_VersionID; }
            set { m_VersionID = value; }
        }

        /// <summary>
        /// �༭�����ͣ�����ע���Ǳ༭
        /// ��ע:CallOut
        /// �༭:Edit
        /// </summary>
        private string m_EditType;
        public string EditType
        {
            get { return m_EditType; }
            set { m_EditType = value; }
        }


        private string m_User;                               //�����û�
        public string User
        {
            get { return m_User; }
            set { m_User = value; }
        }


        private string m_CaseLayerName;                      //������Χ������
        public string CaseLayerName
        {
            get { return m_CaseLayerName; }
            set { m_CaseLayerName = value; }
        }


        private string m_SDEUser;                            //��ǰ�����ݵ�sde�û�
        public string SDEUser
        {
            get { return m_SDEUser; }
            set { m_SDEUser = value; }
        }


        private IVersion m_DefaultVersion;
        public IVersion DefaultVersion
        {
            get { return m_DefaultVersion; }
            set { m_DefaultVersion = value; }
        }


        private IWorkspace m_EditWorkspace;                   //��ǰ�༭�����ռ�
        public IWorkspace EditWorkspace
        {
            get { return m_EditWorkspace; }
            set { m_EditWorkspace = value; }
        }

        public IWorkspaceEdit GetWorkspaceEdit
        {
            get { return m_EditWorkspace as IWorkspaceEdit; }
        }

        private IFeatureLayer m_EditFeatureLayer;             //��ǰ�����ڱ༭��ͼ��
        public IFeatureLayer EditFeatureLayer
        {
            get { return m_EditFeatureLayer; }
            set { m_EditFeatureLayer = value; }
        }


        private List<IFeatureLayer> m_EditLayerlist;          //�ɱ༭ͼ���б�
        public List<IFeatureLayer> EditLayerlist
        {
            get { return m_EditLayerlist; }
            set { m_EditLayerlist = value; }
        }


        //private GeoControls.Editor.ClipBoard m_Clipboard;     //ȫ�ּ��а�
        //public GeoControls.Editor.ClipBoard Clipboard
        //{
        //    get { return m_Clipboard; }
        //    set { m_Clipboard = value; }
        //}


        private bool m_bIsPublicVersion;                      //�Ƿ�Ϊ���ð汾
        public bool BIsPublicVersion
        {
            get { return m_bIsPublicVersion; }
            set { m_bIsPublicVersion = value; }
        }


        private string m_NewVersiontext;                      //��ǰ�����İ汾��ע��Ϣ
        public string NewVersiontext
        {
            get { return m_NewVersiontext; }
            set { m_NewVersiontext = value; }
        }


        private Color m_InkColor;
        public Color InkColor
        {
            get { return m_InkColor; }
            set { m_InkColor = value; }
        }


        private int m_InkWidth;
        public int InkWidth
        {
            get { return m_InkWidth; }
            set { m_InkWidth = value; }
        }


        private IHookHelper m_HookHelper;
        public IHookHelper HookHelper
        {
            get { return m_HookHelper; }
            set { m_HookHelper = value; }
        }

        //���ںϲ�Ҫ��
        private int m_mOid;
        public int MOid
        {
            get { return m_mOid; }
            set { m_mOid = value; }
        }


        private bool m_bUnionCancel;
        public bool BUnionCancel
        {
            get { return m_bUnionCancel; }
            set { m_bUnionCancel = value; }
        }


        private bool m_bCheckDelete;
        public bool BCheckDelete
        {
            get { return m_bCheckDelete; }
            set { m_bCheckDelete = value; }
        }


        private DrawVertex m_DrawVertex;
        public DrawVertex DrawVertex
        {
            get { return m_DrawVertex; }
            set { m_DrawVertex = value; }
        }


        private string m_SubTypeValue;
        public string SubTypeValue
        {
            get { return m_SubTypeValue; }
            set { m_SubTypeValue = value; }
        }


        private string m_SubTypeFiledName;
        public string SubTypeFiledName
        {
            get { return m_SubTypeFiledName; }
            set { m_SubTypeFiledName = value; }
        }

        //��ǰ�༭����
        private object m_EditOpration;
        public object EditOpration
        {
            get { return m_EditOpration; }
            set { m_EditOpration = value; }
        }

        //ȫ�ֵı༭��������
        private IEngineSnapEnvironment m_SnapEnv;
        public IEngineSnapEnvironment SnapEnv
        {
            get { return m_SnapEnv; }
            set { m_SnapEnv = value; }
        }

        private bool m_ToolState;
        public bool ToolState
        {
            get { return m_ToolState; }
            set { m_ToolState = value; }
        }

        private bool m_bEndInkText;
        public bool BEndInkText
        {
            get { return m_bEndInkText; }
            set { m_bEndInkText = value; }
        }

        private SnapPointClass m_SnapPoint;
        public SnapPointClass SnapPoint
        {
            get { return m_SnapPoint; }
            set { m_SnapPoint = value; }
        }

        private IOperationStack m_pOperationStack;
        public IOperationStack OperationStack
        {
            get { return m_pOperationStack; }
            set { m_pOperationStack = value; }
        }

        //����ͼ�������Ľӿ�Xml
        private XmlDocument m_docEditCmdCollectionXml;
        public XmlDocument docEditCmdCollectionXml
        {
            get
            {
                return m_docEditCmdCollectionXml;
            }
            set
            {
                m_docEditCmdCollectionXml = value;
            }
        }

        //Ȩ��Xml
        private XmlDocument m_docRightXml;
        public XmlDocument docRightXml
        {
            get
            {
                return m_docRightXml;
            }
            set
            {
                if (m_docRightXml == null)
                {
                    m_docRightXml = new XmlDocument();
                }
                m_docRightXml = value;
            }
        }


        private object m_frmMain;
        public object frmMain
        {
            get { return m_frmMain; }
            set { m_frmMain = value; }
        }

        private  AxMapControl  m_pMapControl;
        public AxMapControl pMapControl
        {
            get { return m_pMapControl; }
            set { m_pMapControl = value; }
        }

        private IToolbarControl m_pToolbar;
        public IToolbarControl PToolbar
        {
            get { return m_pToolbar; }
            set { m_pToolbar = value; }
        }

        private bool m_bDo;
        public bool BDo
        {
            get { return m_bDo; }
            set { m_bDo = value; }
        }

        private string m_CurSolutionName;
        public string CurSolutionName
        {
            get { return m_CurSolutionName; }
            set { m_CurSolutionName = value; }
        }

        public System.Reflection.MethodInfo MethodInfo_RefreshCommandArgs;

        public System.Reflection.MethodInfo MethodInfo_GetCommandTipEventArgs;

        public System.Reflection.MethodInfo MethodInfo_SetFocusEventArgs;

        public System.Reflection.MethodInfo MethodInfo_PopupMenuEventArgs;

        public System.Reflection.MethodInfo MethodInfo_GetGeometryEventArgs;

        public System.Reflection.MethodInfo MethodInfo_SetBuddyTB;

        public System.Reflection.MethodInfo MethodInfo_GetCurrentVersionID;

        public System.Reflection.MethodInfo MethodInfo_CustomSetToolState;

        public System.Reflection.MethodInfo MethodInfo_GetToolbar;

        public System.Reflection.MethodInfo MethodInfo_CartoCommondTip;

        public System.Reflection.MethodInfo MethodInfo_PopupMenuEventArgsForCarto;

        public System.Reflection.MethodInfo MethodInfo_GetBufferSetForm;

        public IToolbarControl mToolbarControl;
        public ClsEditorMain(IToolbarControl pToolbarControl, AxMapControl pMapControl, object _frmMain)
        {
            m_EditType = "CallOut";
            frmMain = _frmMain;
            ToolState = false;
            mToolbarControl = pToolbarControl;
            m_pOperationStack = pToolbarControl.OperationStack;            

            PToolbar = pToolbarControl;
            m_pMapControl = pMapControl;
            DefaultVersion = null;
            SnapEnv = null;
            BEndInkText = false;

            //��ʼ���и�Ĭ�ϵĲ�׽�࣬���������ĳЩ�õ��ĵط����ִ���
            SnapPoint = new SnapPointClass ();

            Type type = frmMain.GetType ();
            MethodInfo_RefreshCommandArgs = type.GetMethod ( "RefreshCommandArgs" );
            MethodInfo_GetCommandTipEventArgs = type.GetMethod ( "GetCommandTipEventArgs" );
            MethodInfo_SetFocusEventArgs = type.GetMethod ( "SetFocusEventArgs" );
            MethodInfo_PopupMenuEventArgs = type.GetMethod ( "PopupMenuEventArgs" );
            MethodInfo_GetGeometryEventArgs = type.GetMethod ( "GetGeometry" );
            MethodInfo_SetBuddyTB = type.GetMethod ( "SetBuddyTool" );
            MethodInfo_CustomSetToolState = type.GetMethod ( "CustomSetToolState" );
            MethodInfo_GetCurrentVersionID = type.GetMethod("GetCurrentVersionID");
            MethodInfo_GetToolbar = type.GetMethod ( "GetToolbar" );
            MethodInfo_CartoCommondTip = type.GetMethod ( "CartoCommondTip" );
            MethodInfo_PopupMenuEventArgsForCarto = type.GetMethod ( "PopupMenuEventArgsForCarto" );
            MethodInfo_GetBufferSetForm = type.GetMethod("GetBufferSetForm");
        }      
      
    }
}
