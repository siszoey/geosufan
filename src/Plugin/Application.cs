using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Windows.Forms;
using System.Data;
using System.Xml;
using System.Threading;
using SysCommon.Authorize;
using System.Drawing.Printing;

using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;

using SysCommon.Gis;
using ESRI.ArcGIS.Geometry;

namespace Plugin.Application
{
    #region �ӿ�����
    public interface IApplicationRef
    {
    }
    /// <summary>
    /// ��ܹ������Խӿ�
    /// </summary>
    public interface IAppFormRef : IApplicationRef
    {
        /// <summary>
        /// ����
        /// </summary>
        Form MainForm { get; set; }
        /// <summary>
        /// ��������
        /// </summary>
        string Name { get; set; }
        /// <summary>
        /// �������
        /// </summary>
        string Caption { get; set; }
        /// <summary>
        /// �Ƿ���ʾ
        /// </summary>
        bool Visible { get; set; }
        /// <summary>
        /// �Ƿ����
        /// </summary>
        bool Enabled { get; set; }
        /// <summary>
        /// �������ؼ�����
        /// </summary>
        Control ControlContainer { get; set; }
        #region ״̬������
        /// <summary>
        /// ״̬��
        /// </summary>
        Control StatusBar { get; set; }
        /// <summary>
        /// ������ʾ����
        /// </summary>
        string OperatorTips { get; set; }
        /// <summary>
        /// ������
        /// </summary>
        DevExpress.XtraEditors.Repository.RepositoryItemProgressBar ProgressBar { get; set; }
        //�ο�������cmb
        DevExpress.XtraEditors.Repository.RepositoryItemComboBox RefScaleCmb { get; set; }
        //��ǰ������cmb
        DevExpress.XtraEditors.Repository.RepositoryItemComboBox CurScaleCmb { get; set; }
        //������ʾ�ı���
        DevExpress.XtraEditors.Repository.RepositoryItemTextEdit CoorTxt { get; set; }
        /// <summary>
        /// ͼ�ϵ���������
        /// </summary>
        string CoorXY { get; set; }
        /// <summary>
        /// �û���Ϣ����
        /// </summary>
        string UserInfo { get; set; }
        #endregion
        /// <summary>
        /// ��ǰʹ�õ�ϵͳ����
        /// </summary>
        string CurrentSysName { get; set; }
        /// <summary>
        /// ϵͳ������ͼXml�ڵ�
        /// </summary>
        XmlDocument SystemXml { get; set; }
        /// <summary>
        /// ������ͼXML�ڵ�
        /// </summary>
        XmlDocument DataTreeXml { get; set; }
        /// <summary>
        /// ������ϢXML�ڵ�
        /// </summary>
        XmlDocument DatabaseInfoXml { get; set; }
        /// <summary>
        /// �Ҽ��˵�����
        /// </summary>
        Dictionary<string, System.Windows.Forms.ContextMenuStrip> DicContextMenu { get; set; }
        /// <summary>
        /// ϵͳ�������
        /// </summary>
        Plugin.Parse.PluginCollection ColParsePlugin { get; set; }
        /// <summary>
        /// ͼƬ��Դ·��
        /// </summary>
        string ImageResPath { get; set; }
        /// <summary>
        /// ��½���û�
        /// </summary>
        User ConnUser { get; set; }
        //cyf 20110602 add ��¼�û���Ӧ�Ľ�ɫ��Ϣ
        List<Role> LstRoleInfo { get; set; }
        //end
        /// <summary>
        /// �û�Ȩ�ޱ��
        /// </summary>
        List<string> ListUserPrivilegeID { get; set; }
        //������Ϣ
        /// <summary>
        /// ��ʽ�����ݿ�ӿ���Ϣ
        /// </summary>
        ICustomWks CurWksInfo { get; set; }
        /// <summary>
        /// ��ʱ�����ݿ�ṹ��Ϣ
        /// </summary>
        ICustomWks TempWksInfo { get; set; }

        //added by chulili 20111120 
        IMapControlDefault MapControl { get; set; }
        ITOCControlDefault TOCControl { get; set; }
        //end added by chulili 
        object LayerTree { get; set; }
        object LayerAdvTree { get; set; }
        string LayerTreeXmlPath { get; set; }
    }

    //�Զ���ռ����ݿ⹤���ռ�ӿ�
    public struct ICustomWks
    {
        public ESRI.ArcGIS.Geodatabase.IWorkspace Wks;
        public string Server;
        public string Service;
        public string Database;
        public string User;
        public string PassWord;
        public string Version;
        public string DataBase;
        public string DBType;
    }
    /// <summary>
    /// ArcGIS�ؼ����Խӿ�
    /// </summary>
    public interface IAppArcGISRef : IApplicationRef
    {
        /// <summary>
        /// ��ǰʹ�õ�GIS����
        /// </summary>
        string CurrentTool { get; set; }
        /// <summary>
        /// �洢���ĵ�����
        /// </summary>
        IMapDocument MapDocument { get; set; }
        /// <summary>
        /// ���ؼ��е�MapControl�ؼ�
        /// </summary>
        IMapControlDefault MapControl { get; set; }
        AxMapControl ArcGisMapControl { get; set; }
        /// <summary>
        /// ���ؼ��е�SceneControl�ؼ�
        /// </summary>
        ISceneControlDefault SceneControlDefault { get; set; }
        AxSceneControl SceneControl { get; set; }
        /// <summary>
        /// ���ؼ��е�PageLayoutControl�ؼ�
        /// </summary>
        IPageLayoutControlDefault PageLayoutControl { get; set; }
        /// <summary>
        /// ���ؼ��е�TOCControl�ؼ�
        /// </summary>
        ITOCControlDefault TOCControl { get; set; }
        /// <summary>
        /// ʮ��˿ xisheng 20110805
        /// </summary>
        PictureBox p1 { get; set; }
        PictureBox p2 { get; set; }
        PictureBox p3{ get; set; }
        PictureBox p4 { get; set; }
        //cyf  20110518 
        /// <summary>
        /// ��ǰͼ����ʾ�ؼ�
        /// </summary>
        object CurrentControl { get; set; }
        /// <summary>
        /// ��tab����ϵͳ����
        /// </summary>
        Dictionary<DevExpress.XtraBars.Ribbon.RibbonPage, string> DicTabs { get; set; }
        //end
    }
    /// <summary>
    /// GISϵͳ�������Խӿ�
    /// </summary>
    public interface IAppGISRef : IAppArcGISRef
    {
        /// <summary>
        /// �û��ؼ�
        /// </summary>
        UserControl MainUserControl { get; set; }
        /// <summary>
        /// ���ݴ�����ͼ
        /// </summary>
        DevExpress.XtraTreeList.TreeList DataTree { get; set; }
        /// <summary>
        /// ������Ϣ��ͼ
        /// </summary>
        DevExpress.XtraTreeList.TreeList ProjectTree { get; set; }
        /// <summary>
        /// ���ݹ�������XML�ļ�
        /// </summary>
        XmlDocument DBXmlDocument { get; set; }
        /// <summary>
        /// ���¶Ա��б�
        /// </summary>
        DevExpress.XtraGrid.Views.Grid.GridView UpdateGrid { get; set; }
        /// <summary>
        /// ���¶Ա��б��ҳ��Ϣ��ʾ�ı���
        /// </summary>
        DevExpress.XtraEditors.Repository.RepositoryItemTextEdit TxtDisplayPage { get; set; }
        /// <summary>
        /// ���¶Ա��б�ť
        /// </summary>
        DevExpress.XtraBars.BarButtonItem BtnFirst { get; set; }
        DevExpress.XtraBars.BarButtonItem BtnLast { get; set; }
        DevExpress.XtraBars.BarButtonItem BtnPre { get; set; }
        DevExpress.XtraBars.BarButtonItem BtnNext { get; set; }
        /// <summary>
        /// ����������б�
        /// </summary>
        DevExpress.XtraGrid.Views.Grid.GridView PolylineSearchGrid { get; set; }
        /// <summary>
        /// ������������
        /// </summary>
        DevExpress.XtraGrid.Views.Grid.GridView PolygonSearchGrid { get; set; }
        /// <summary>
        /// �ӱ��ںϼ�¼���
        /// </summary>
        DevExpress.XtraGrid.Views.Grid.GridView JoinMergeResultGrid { get; set; }
        /// <summary>
        /// ���ݼ���б�
        /// </summary>
        DevExpress.XtraGrid.Views.Grid.GridView DataCheckGrid { get; set; }
        /// <summary>
        /// �������ݵĽ���(Ψһ)
        /// </summary>
        Thread CurrentThread { get; set; }
    }
    /// <summary>
    /// ���ݸ���ϵͳ�������Խӿ�
    /// </summary>
    public interface IAppSMPDRef : IAppArcGISRef
    {
        /// <summary>
        /// �û��ؼ�
        /// </summary>
        UserControl MainUserControl { get; set; }
        /// <summary>
        /// ���¶Ա��б��ҳ��Ϣ��ʾ�ı���
        /// </summary>
        DevExpress.XtraEditors.Repository.RepositoryItemTextEdit TxtDisplayPage { get; set; }
        /// <summary>
        /// ���¶Ա��б�ť
        /// </summary>
        DevExpress.XtraBars.BarButtonItem BtnFirst { get; set; }
        DevExpress.XtraBars.BarButtonItem BtnLast { get; set; }
        DevExpress.XtraBars.BarButtonItem BtnPre { get; set; }
        DevExpress.XtraBars.BarButtonItem BtnNext { get; set; }
        /// <summary>
        /// ������ͼ
        /// </summary>
        DevExpress.XtraTreeList.TreeList ErrTree { get; set; }
        /// <summary>
        /// ������Ϣ��ͼ
        /// </summary>
        DevExpress.XtraTreeList.TreeList ProjectTree { get; set; }
        /// <summary>
        /// ���¶Աȷ����б�
        /// </summary>
        DevExpress.XtraGrid.Views.Grid.GridView UpdateDataGrid { get; set; }
        /// <summary>
        /// ���ݼ���б�
        /// </summary>
        DevExpress.XtraGrid.Views.Grid.GridView DataCheckGrid { get; set; }
        /// <summary>
        /// ���ݹ�������XML�ļ�
        /// </summary>
        XmlDocument DBXmlDocument { get; set; }
        /// <summary>
        /// �������ݵĽ���(Ψһ)
        /// </summary>
        Thread CurrentThread { get; set; }
    }
    /// <summary>
    /// ����ϵͳ�������Խӿ�
    /// </summary>
    public interface IAppPrivilegesRef : IAppFormRef
    {
        //wgf 20110518
        //---------------------------------------------------------
        System.Windows.Forms.TreeView DataTabIndexTree { get; set; }

        System.Windows.Forms.DataGridView GridCtrl { get; set; }

        System.Windows.Forms.RichTextBox tipRichBox { get; set; }
        string strLogFilePath { get; set; }   //��־�ļ�
        //---------------------------------------------------------
        /// <summary>
        /// �û��ؼ�
        /// </summary>
        UserControl MainUserControl { get; set; }
        /// <summary>
        /// ���ù�������ͼ
        /// </summary>
        DevExpress.XtraTreeList.TreeList MainTree { get; set; }
        /// <summary>
        /// �û�����ͼ
        /// </summary>
        DevExpress.XtraTreeList.TreeList RoleTree { get; set; }

        /// �û���ͼ
        /// </summary>
        DevExpress.XtraTreeList.TreeList UserTree { get; set; }
        /// <summary>
        /// Ȩ����ͼ
        /// </summary>
        DevExpress.XtraTreeList.TreeList PrivilegeTree { get; set; }
        /// <summary>
        /// �������ݵĽ���(Ψһ)
        /// </summary>
        Thread CurrentThread { get; set; }
        //������Ϣ
        /// <summary>
        /// ��ʽ�����ݿ�ӿ���Ϣ
        /// </summary>
        ICustomWks CurWksInfo { get; set; }
        /// <summary>
        /// ��ʱ�����ݿ�ṹ��Ϣ
        /// </summary>
        ICustomWks TempWksInfo { get; set; }
        bool CurScaleVisible { get; set; }
        bool RefScaleVisible { get; set; }
    }

    /// <summary>
    /// ���ݸ���ϵͳ�������Խӿ�
    /// </summary>
    public interface IAppGisUpdateRef : IAppArcGISRef
    {
        //wgf 2011-5-18
        //-------------------------------------------------------------------
        //�ĵ�����
        System.Windows.Forms.RichTextBox DocControl { get; set; }
        /// ���ݵ�Ԫ��ͼ
        /// </summary>
        System.Windows.Forms.TreeView DataUnitTree { get; set; }

        /// ��ԴĿ¼��ͼ
        /// </summary>
        System.Windows.Forms.TreeView DataIndexTree { get; set; }

        /// ��ͼ�ĵ���ͼ
        /// </summary>
        System.Windows.Forms.TreeView MapDocTree { get; set; }

        //�ĵ���
        System.Windows.Forms.TreeView TextDocTree { get; set; }

        //�û��ɹ���
        System.Windows.Forms.TreeView UserResultTree { get; set; }

        DevExpress.XtraBars.Ribbon.RibbonPage IndextabControl { get; set; }

        System.Windows.Forms.RichTextBox tipRichBox { get; set; }
        SysCommon.BottomQueryBar QueryBar { get; set; }

        SysGisDataSet gisDataSet { get; set; }         //���ݿ������ wgf

        string strLogFilePath { get; set; }   //��־�ļ�

        //---------------------------------------------------------------------

        /// <summary>
        /// �û��ؼ�
        /// </summary>
        UserControl MainUserControl { get; set; }

        /// <summary>
        /// ���ݴ�����ͼ
        /// </summary>
        DevExpress.XtraTreeList.TreeList DataTree { get; set; }

        /// <summary>
        /// ������ͼ
        /// </summary>
        DevExpress.XtraTreeList.TreeList ErrTree { get; set; }

        /// <summary>
        /// ������Ϣ��ͼ
        /// </summary>
        DevExpress.XtraTreeList.TreeList ProjectTree { get; set; }

        /// <summary>
        /// ���¶Աȷ����б�
        /// </summary>
        DevExpress.XtraGrid.Views.Grid.GridView UpdateDataGrid { get; set; }
        /// <summary>
        /// ���ݹ�������XML�ļ�
        /// </summary>
        XmlDocument DBXmlDocument { get; set; }
        /// <summary>
        /// �������ݵĽ���(Ψһ)
        /// </summary>
        Thread CurrentThread { get; set; }
    }

    /// <summary>
    /// �ļ���ϵͳ�������Խӿ�
    /// </summary>
    public interface IAppFileRef : IAppArcGISRef
    {
        //wgf 2011-5-18
        //---------------------------------------------------------
        System.Windows.Forms.TreeView DataTabIndexTree { get; set; }
        System.Windows.Forms.DataGridView GridCtrl { get; set; }
        System.Windows.Forms.RichTextBox tipRichBox { get; set; }
        string strLogFilePath { get; set; }   //��־�ļ�
        //---------------------------------------------------------
        /// <summary>
        /// �û��ؼ�
        /// </summary>
        UserControl MainUserControl { get; set; }
        /// <summary>
        /// ������Ϣ��ͼ
        /// </summary>
        DevExpress.XtraTreeList.TreeList ProjectTree { get; set; }
        /// <summary>
        /// ������Ϣ�б�
        /// </summary>
        DevExpress.XtraGrid.Views.Grid.GridView DataInfoGrid { get; set; }
        /// <summary>
        /// Ԫ�����б�
        /// </summary>
        DevExpress.XtraGrid.Views.Grid.GridView MetaDataGrid { get; set; }
        /// <summary>
        /// ϵͳ�����б�
        /// </summary>
        DevExpress.XtraGrid.Views.Grid.GridView SysSettingGrid { get; set; }
        /// <summary>
        /// ϵͳ������ͼ
        /// </summary>
        DevExpress.XtraTreeList.TreeList SysSettingTree { get; set; }
        /// <summary>
        /// ���ݹ�������XML�ļ�
        /// </summary>
        XmlDocument DBXmlDocument { get; set; }
        /// <summary>
        /// ������Ϣ�б��ҳ��Ϣ��ʾ�ı���
        /// </summary>
        DevExpress.XtraEditors.Repository.RepositoryItemTextEdit TxtDisplayPage { get; set; }
        /// <summary>
        /// ������Ϣ�б�ť
        /// </summary>
        DevExpress.XtraBars.BarButtonItem BtnFirst { get; set; }
        DevExpress.XtraBars.BarButtonItem BtnLast { get; set; }
        DevExpress.XtraBars.BarButtonItem BtnPre { get; set; }
        DevExpress.XtraBars.BarButtonItem BtnNext { get; set; }
        /// <summary>
        /// �������ݵĽ���(Ψһ)
        /// </summary>
        Thread CurrentThread { get; set; }
    }
    /// <summary>
    /// ���ݿ⼯�ɹ�����ϵͳ����ӿڶ���  ���Ƿ����  20100927
    /// </summary>
    public interface IAppDBIntegraRef : IAppArcGISRef
    {
        /// <summary>
        /// �û��ؼ�
        /// </summary>
        UserControl MainUserControl { get; set; }
        /// <summary>
        /// ������Ϣ��ͼ
        /// </summary>
        DevExpress.XtraTreeList.TreeList ProjectTree { get; set; }
        /// <summary>
        /// ���ݹ�������XML�ļ�
        /// </summary>
        XmlDocument DBXmlDocument { get; set; }
        /// <summary>
        /// �������ݵĽ���(Ψһ)
        /// </summary>
        Thread CurrentThread { get; set; }
        //added by chulili 20110722 ��Ӻ��� ֧��״̬�������߿ɼ�״̬����
        bool CurScaleVisible { get; set; }
        bool RefScaleVisible { get; set; }
        //end added by chulili
    }
    #endregion
    #region �ӿ�ʵ����
    public class AppForm : IAppFormRef
    {
        //added by chulili 20111120
        private IMapControlDefault _MapControl;                      //MapControl�ؼ�
        private ITOCControlDefault _TOCControl;                     //TOCControl�ؼ�
        //end added by chulili
        private object _LayerAdvTree;  //Ŀ¼��ͼ �������advTree
        private object _LayerTree;                                      //added by chulili 20111119
        private string _LayerTreeXmlPath;                               //added by chulili 20111215
        private Form _MainForm;                                        // ����
        private Control _StatusStrip;                                  // ״̬��
        private Control _ControlContainer;                             // �ؼ�����
        private string _CurrentTab;                                   // ��ǰʹ�õ�Tab
        private XmlDocument _SystemXml;                                // ϵͳ������ͼXml�ڵ�
        private XmlDocument _DataTreeXml;                              // ������ͼXML�ڵ�
        private XmlDocument _DatabaseInfoXml;                          // ������ϢXML�ڵ�
        private Plugin.Parse.PluginCollection _ColParsePlugin;         // ϵͳ�������
        private Dictionary<string, System.Windows.Forms.ContextMenuStrip> _DicContextMenu;           //�Ҽ��˵�����
        private string _ImageResPath;                                  // ͼƬ��Դ·��
        private User _user;                                          //���ӵ��û���Ϣ 
        //cyf 20110602 add
        private List<Role> _LstRoleInfo;                             //���ӵ��û���Ӧ�Ľ�ɫ��Ϣ
        //end
        private List<string> _ListUserPrivilegeID;                   //�û�Ȩ�ޱ��
        //��ʽ��
        private ICustomWks _CurWks;
        //��ʱ��
        private ICustomWks _TempWks;

        //���캯��
        public AppForm()
        {
        }

        public AppForm(Form MainForm, Control StatusStrip, XmlDocument SystemXml, XmlDocument DataTreeXml, XmlDocument DatabaseInfoXml, Plugin.Parse.PluginCollection ColParsePlugin, string ImageResPath)
        {
            _MainForm = MainForm;
            _StatusStrip = StatusStrip;
            _SystemXml = SystemXml;
            _DataTreeXml = DataTreeXml;
            _DatabaseInfoXml = DatabaseInfoXml;
            _ColParsePlugin = ColParsePlugin;
            _ImageResPath = ImageResPath;
        }

        #region IDefAppForm ��Ա

        public Form MainForm
        {
            get
            {
                return _MainForm;
            }
            set
            {
                _MainForm = value;
            }
        }
        public object LayerTree
        {
            get
            {
                return _LayerTree;
            }
            set
            {
                _LayerTree = value;
            }
        }
        //added by chulili 2011-11-29 ͼ��Ŀ¼��AdvTree
        public object LayerAdvTree
        {
            get
            {
                return _LayerAdvTree; ;
            }
            set
            {
                _LayerAdvTree = value;
            }
        }
        //added by chulili 2011-12-15 ͼ��Ŀ¼�����ļ�·��
        public string LayerTreeXmlPath
        {
            get
            {
                return _LayerTreeXmlPath; ;
            }
            set
            {
                _LayerTreeXmlPath = value;
            }
        }
        //end added by chulili
        public IMapControlDefault MapControl
        {
            get
            {
                return _MapControl;
            }
            set
            {
                _MapControl = value;
            }
        }
        public ITOCControlDefault TOCControl
        {
            get
            {
                return _TOCControl;
            }
            set
            {
                _TOCControl = value;
            }
        }
        public string Name
        {
            get
            {
                return _MainForm.Name;
            }
            set
            {
                _MainForm.Name = value;
            }
        }

        public string Caption
        {
            get
            {
                return _MainForm.Text;
            }
            set
            {
                _MainForm.Text = value;
            }
        }

        public bool Visible
        {
            get
            {
                return _MainForm.Visible;
            }
            set
            {
                _MainForm.Visible = value;
            }
        }

        public bool Enabled
        {
            get
            {
                return _MainForm.Enabled;
            }
            set
            {
                _MainForm.Enabled = value;
            }
        }

        public Control ControlContainer
        {
            get
            {
                return _ControlContainer;
            }
            set
            {
                _ControlContainer = value;
            }
        }

        public Control StatusBar
        {
            get
            {
                return _StatusStrip;
            }
            set
            {
                _StatusStrip = value;
            }
        }

        public string CurrentSysName
        {
            get
            {
                return _CurrentTab;
            }
            set
            {
                _CurrentTab = value;
            }
        }

        public XmlDocument SystemXml
        {
            get
            {
                return _SystemXml;
            }
            set
            {
                _SystemXml = value;
            }
        }

        public XmlDocument DataTreeXml
        {
            get
            {
                return _DataTreeXml;
            }
            set
            {
                _DataTreeXml = value;
            }
        }

        public XmlDocument DatabaseInfoXml
        {
            get
            {
                return _DatabaseInfoXml;
            }
            set
            {
                _DatabaseInfoXml = value;
            }
        }

        public Dictionary<string, System.Windows.Forms.ContextMenuStrip> DicContextMenu
        {
            get
            {
                return _DicContextMenu;
            }
            set
            {
                _DicContextMenu = value;
            }
        }

        public Plugin.Parse.PluginCollection ColParsePlugin
        {
            get
            {
                return _ColParsePlugin;
            }
            set
            {
                _ColParsePlugin = value;
            }
        }

        public string ImageResPath
        {
            get
            {
                return _ImageResPath;
            }
            set
            {
                _ImageResPath = value;
            }
        }

        public User ConnUser
        {
            get { return _user; }
            set { _user = value; }
        }
        //cyf 20110602 add:�����û���Ӧ�Ľ�ɫ������
        public List<Role> LstRoleInfo
        {
            get { return _LstRoleInfo; }
            set { _LstRoleInfo = value; }
        }
        //end

        public List<string> ListUserPrivilegeID
        {
            get
            {
                return _ListUserPrivilegeID;
            }
            set
            {
                _ListUserPrivilegeID = value;
            }
        }

        //����������Ϣ
        public ICustomWks CurWksInfo
        {
            get
            {
                return _CurWks;
            }
            set
            {
                _CurWks = value;
            }
        }
        public ICustomWks TempWksInfo
        {
            get
            {
                return _TempWks;
            }
            set
            {
                _TempWks = value;
            }
        }

        #endregion

        #region ״̬������
        private String _OperatorTips;
        public string OperatorTips
        {
            get
            {
                return _OperatorTips;
            }
            set
            {
                _OperatorTips = value;
            }
        }

        private DevExpress.XtraEditors.Repository.RepositoryItemProgressBar _ProgressBar;
        public DevExpress.XtraEditors.Repository.RepositoryItemProgressBar ProgressBar
        {
            get
            {
                return _ProgressBar;
            }
            set
            {
                _ProgressBar = value;
            }
        }
        //�ο�������cmb
        private DevExpress.XtraEditors.Repository.RepositoryItemComboBox _RefScaleCmb;
        public DevExpress.XtraEditors.Repository.RepositoryItemComboBox RefScaleCmb
        {
            get
            {
                return _RefScaleCmb;
            }
            set
            {
                _RefScaleCmb = value;
            }
        }
        //��ǰ������cmb
        private DevExpress.XtraEditors.Repository.RepositoryItemComboBox _CurScaleCmb;
        public DevExpress.XtraEditors.Repository.RepositoryItemComboBox CurScaleCmb
        {
            get
            {
                return _CurScaleCmb;
            }
            set
            {
                _CurScaleCmb = value;
            }
        }
        //������ʾ�ı���
        private DevExpress.XtraEditors.Repository.RepositoryItemTextEdit _CoorTxt;
        public DevExpress.XtraEditors.Repository.RepositoryItemTextEdit CoorTxt
        {
            get
            {
                return _CoorTxt;
            }
            set
            {
                _CoorTxt = value;
            }
        }

        private String _UserInfo;
        public string UserInfo
        {
            get
            {
                return _UserInfo;
            }
            set
            {
                _UserInfo = value;
            }
        }

        private String _CoorXY;
        public string CoorXY
        {
            get
            {
                return _CoorXY;
            }
            set
            {
                _CoorXY = value;
            }
        }
        #endregion


    }

    public class AppGIS : IAppGISRef, IAppFormRef
    {
        private object _LayerTree;                                      //added by chulili 20111119
        private object _LayerAdvTree;  //Ŀ¼��ͼ �������advTree
        private string _LayerTreeXmlPath;                               //added by chulili 20111215
        private Form _MainForm;                                        // ����
        private UserControl _MainUserControl;                         //�ؼ�UserControl
        private Control _StatusStrip;                                 // ״̬��
        private Control _ControlContainer;                             // �ؼ�����
        private XmlDocument _SystemXml;                                // ϵͳ������ͼXml�ڵ�
        private XmlDocument _DataTreeXml;                              // ������ͼXML�ڵ�
        private XmlDocument _DatabaseInfoXml;                          // ������ϢXML�ڵ�
        private Plugin.Parse.PluginCollection _ColParsePlugin;         // ϵͳ�������
        private Dictionary<string, System.Windows.Forms.ContextMenuStrip> _DicContextMenu;           //�Ҽ��˵�����
        private string _ImageResPath;                                  // ͼƬ��Դ·��
        private IMapDocument _MapDocument;                            //�洢���ĵ�����
        private IMapControlDefault _MapControl;                      //MapControl�ؼ�
        private AxMapControl _AxMapControl;
        private ISceneControlDefault _SceneControlDefault;
        private AxSceneControl _SceneControl;

        private IPageLayoutControlDefault _PageLayoutControl;        //PageLayoutControl�ؼ�
        private ITOCControlDefault _TOCControl;                     //TOCControl�ؼ�

        PictureBox _p1;                                               //��ʮ��˿
        PictureBox _p2; 
        PictureBox _p3; 
        PictureBox _p4;

        private string _CurrentTool;                                   // ��ǰʹ�õ�TOOL����
        //20110518
        private object _CurrentControl;                             //��ǰͼ����ʾ�ؼ�    
        private List<DevExpress.XtraEditors.Repository.RepositoryItemComboBox> _scaleBoxList;                        //��������ʾ        
        private Dictionary<DevExpress.XtraBars.Ribbon.RibbonPage, string> _dicTabs;          //��tab����ϵͳ����
        //end

        private DevExpress.XtraTreeList.TreeList _ProjectTree;       // ������Ϣ��ͼ
        private DevExpress.XtraTreeList.TreeList _DataTree;          // ���ݴ�����ͼ
        private DevExpress.XtraGrid.Views.Grid.GridView _UpdateGrid;  //���¶Ա��б�
        private DevExpress.XtraEditors.Repository.RepositoryItemTextEdit _txtDisplayPage;//���¶Ա��б��ҳ��ʾ�ı���
        private DevExpress.XtraBars.BarButtonItem _btnFirst;//���¶Ա��б��ҳ��ʾ��ť
        private DevExpress.XtraBars.BarButtonItem _btnLast;//���¶Ա��б��ҳ��ʾ��ť
        private DevExpress.XtraBars.BarButtonItem _btnPre;//���¶Ա��б��ҳ��ʾ��ť
        private DevExpress.XtraBars.BarButtonItem _btnNext;//���¶Ա��б��ҳ��ʾ��ť
        private DevExpress.XtraGrid.Views.Grid.GridView _PolylineSearchGrid;  //�ӱ����ͼ�¼��
        private DevExpress.XtraGrid.Views.Grid.GridView _PolygonSearchGrid;  //�ӱ߶���μ�¼��
        private DevExpress.XtraGrid.Views.Grid.GridView _JoinMergeResultGrid;  //�ӱ��ںϽ����¼��

        private DevExpress.XtraGrid.Views.Grid.GridView _DataCheckGrid;   //���ݼ����

        private Thread _CurrentThread;                           //�������ݵĽ���(Ψһ)
        private XmlDocument _DBXmlDocument;                       //���ݹ�������XML�ļ�
        private User _user;                                          //���ӵ��û���Ϣ
        //cyf 20110602 add
        private List<Role> _LstRoleInfo;                             //���ӵ��û���Ӧ�Ľ�ɫ��Ϣ
        //end
        private List<string> _ListUserPrivilegeID;                   //�û�Ȩ�ޱ��

        //��ʽ��
        private ICustomWks _CurWks;
        //��ʱ��
        private ICustomWks _TempWks;

        private SysCommon.BottomQueryBar _QueryBar; //added by chulili 2012-10-16 ��������ϵͳ��Ӳ�ѯ���״̬��

        //���캯��
        public AppGIS()
        {
        }

        public AppGIS(Form pForm, Control ControlContainer, List<string> ListUserPrivilegeID, XmlDocument SystemXml, XmlDocument DataTreeXml, XmlDocument DatabaseInfoXml, Plugin.Parse.PluginCollection ColParsePlugin, string ImageResPath, User V_user)
        {
            //��ϮAppFormRef ����
            _MainForm = pForm;
            _ControlContainer = ControlContainer;
            _SystemXml = SystemXml;
            _DataTreeXml = DataTreeXml;
            _DatabaseInfoXml = DatabaseInfoXml;
            _ColParsePlugin = ColParsePlugin;
            _ImageResPath = ImageResPath;
            //added by chulili 20110711 �����������
            _ListUserPrivilegeID = ListUserPrivilegeID;
            _user = V_user;
        }


        #region IAppGISDef ��Ա

        public UserControl MainUserControl
        {
            get
            {
                return _MainUserControl;
            }
            set
            {
                _MainUserControl = value;
            }
        }

        public DevExpress.XtraTreeList.TreeList DataTree
        {
            get
            {
                return _DataTree;
            }
            set
            {
                _DataTree = value;
            }
        }

        public DevExpress.XtraTreeList.TreeList ProjectTree
        {
            get
            {
                return _ProjectTree;
            }
            set
            {
                _ProjectTree = value;
            }
        }

        public XmlDocument DBXmlDocument
        {
            get
            {
                return _DBXmlDocument;
            }
            set
            {
                _DBXmlDocument = value;
            }
        }

        public DevExpress.XtraGrid.Views.Grid.GridView UpdateGrid
        {
            get
            {
                return _UpdateGrid;
            }
            set
            {
                _UpdateGrid = value;
            }
        }
        public DevExpress.XtraGrid.Views.Grid.GridView PolylineSearchGrid
        {
            get
            {
                return _PolylineSearchGrid;
            }
            set
            {
                _PolylineSearchGrid = value;
            }
        }
        public DevExpress.XtraGrid.Views.Grid.GridView PolygonSearchGrid
        {
            get
            {
                return _PolygonSearchGrid;
            }
            set
            {
                _PolygonSearchGrid = value;
            }
        }
        public DevExpress.XtraGrid.Views.Grid.GridView JoinMergeResultGrid
        {
            get
            {
                return _JoinMergeResultGrid;
            }
            set
            {
                _JoinMergeResultGrid = value;
            }
        }




        public DevExpress.XtraEditors.Repository.RepositoryItemTextEdit TxtDisplayPage
        {
            get
            {
                return _txtDisplayPage;
            }
            set
            {
                _txtDisplayPage = value;
            }
        }

        public DevExpress.XtraBars.BarButtonItem BtnFirst
        {
            get
            {
                return _btnFirst;
            }
            set
            {
                _btnFirst = value;
            }
        }
        public DevExpress.XtraBars.BarButtonItem BtnLast
        {
            get
            {
                return _btnLast;
            }
            set
            {
                _btnLast = value;
            }
        }
        public DevExpress.XtraBars.BarButtonItem BtnPre
        {
            get
            {
                return _btnPre;
            }
            set
            {
                _btnPre = value;
            }
        }
        public DevExpress.XtraBars.BarButtonItem BtnNext
        {
            get
            {
                return _btnNext;
            }
            set
            {
                _btnNext = value;
            }
        }

        public DevExpress.XtraGrid.Views.Grid.GridView DataCheckGrid
        {
            get
            {
                return _DataCheckGrid;
            }
            set
            {
                _DataCheckGrid = value;
            }
        }
        public SysCommon.BottomQueryBar QueryBar
        {
            get
            {
                return _QueryBar;
            }
            set
            {
                _QueryBar = value;
            }
        }

        public string Name
        {
            get
            {
                return _MainUserControl.Name;
            }
            set
            {
                _MainUserControl.Name = value;
            }
        }

        public string Caption
        {
            get
            {
                return _MainUserControl.Tag as string;
            }
            set
            {
                _MainUserControl.Tag = value;
            }
        }

        public bool Visible
        {
            get
            {
                return _MainUserControl.Visible;
            }
            set
            {
                _MainUserControl.Visible = value;
            }
        }

        public bool Enabled
        {
            get
            {
                return _MainUserControl.Enabled;
            }
            set
            {
                _MainUserControl.Enabled = value;
            }
        }

        public Dictionary<string, System.Windows.Forms.ContextMenuStrip> DicContextMenu
        {
            get
            {
                return _DicContextMenu;
            }
            set
            {
                _DicContextMenu = value;
            }
        }

        public Control StatusBar
        {
            get
            {
                return _StatusStrip;
            }
            set
            {
                _StatusStrip = value;
            }
        }

        public Thread CurrentThread
        {
            get
            {
                return _CurrentThread;
            }
            set
            {
                _CurrentThread = value;
            }
        }


        #endregion

        #region ״̬������
        public string OperatorTips
        {
            get; set;
        }

        public DevExpress.XtraEditors.Repository.RepositoryItemProgressBar ProgressBar
        {
            get; set;
        }

        //�ο�������cmb
        public DevExpress.XtraEditors.Repository.RepositoryItemComboBox RefScaleCmb
        {
            get; set;
        }
        //��ǰ������cmb
        public DevExpress.XtraEditors.Repository.RepositoryItemComboBox CurScaleCmb
        {
            get;set;
        }
        //������ʾ�ı���
        public DevExpress.XtraEditors.Repository.RepositoryItemTextEdit CoorTxt
        {
            get; set;
        }


        public string CoorXY
        {
            get; set;
        }

        public string UserInfo
        {
            get; set;
        }

        #endregion

        #region IAppArcGISRef ��Ա
        public string CurrentTool
        {
            get
            {
                return _CurrentTool;
            }
            set
            {
                _CurrentTool = value;
            }
        }

        public IMapDocument MapDocument
        {
            get
            {
                return _MapDocument;
            }
            set
            {
                _MapDocument = value;
            }
        }

        public PictureBox p1
        {
            get
            { return _p1; }
            set
            { _p1 = value; }
        }

        public PictureBox p2
        {
            get
            { return _p2; }
            set
            { _p2 = value; }
        }

        public PictureBox p3
        {
            get
            { return _p3; }
            set
            { _p3 = value; }
        }

        public PictureBox p4
        {
            get
            { return _p4; }
            set
            { _p4 = value; }
        }


        public IMapControlDefault MapControl
        {
            get
            {
                return _MapControl;
            }
            set
            {
                _MapControl = value;
            }
        }

        public AxMapControl ArcGisMapControl
        {
            get
            {
                return _AxMapControl;
            }
            set
            {
                _AxMapControl = value;
            }
        }

        public ISceneControlDefault SceneControlDefault
        {
            get { return _SceneControlDefault; }
            set { _SceneControlDefault = value; }
        }

        public AxSceneControl SceneControl
        {
            get { return _SceneControl; }
            set { _SceneControl = value; }
        }

        public IPageLayoutControlDefault PageLayoutControl
        {
            get
            {
                return _PageLayoutControl;
            }
            set
            {
                _PageLayoutControl = value;
            }
        }

        public ITOCControlDefault TOCControl
        {
            get
            {
                return _TOCControl;
            }
            set
            {
                _TOCControl = value;
            }
        }
        public object CurrentControl
        {
            get
            {
                return _CurrentControl;
            }
            set
            {
                _CurrentControl = value;
            }
        }
        public List<DevExpress.XtraEditors.Repository.RepositoryItemComboBox> ScaleBoxList
        {
            get
            {
                return _scaleBoxList;
            }
            set
            {
                _scaleBoxList = value;
            }
        }

        public Dictionary<DevExpress.XtraBars.Ribbon.RibbonPage, string> DicTabs
        {
            get
            {
                return _dicTabs;
            }
            set
            {
                _dicTabs = value;
            }
        }
        #endregion

        #region IAppFormDef ��Ա

        Form IAppFormRef.MainForm
        {
            get
            {
                return _MainForm;
            }
            set
            {
                _MainForm = value;
            }
        }

        public Control ControlContainer
        {
            get
            {
                return _ControlContainer;
            }
            set
            {
                _ControlContainer = value;
            }
        }

        string IAppFormRef.CurrentSysName
        {
            get
            {
                return null;
            }
            set
            {
            }
        }

        public XmlDocument SystemXml
        {
            get
            {
                return _SystemXml;
            }
            set
            {
                _SystemXml = value;
            }
        }

        public XmlDocument DataTreeXml
        {
            get
            {
                return _DataTreeXml;
            }
            set
            {
                _DataTreeXml = value;
            }
        }

        public XmlDocument DatabaseInfoXml
        {
            get
            {
                return _DatabaseInfoXml;
            }
            set
            {
                _DatabaseInfoXml = value;
            }
        }

        public Plugin.Parse.PluginCollection ColParsePlugin
        {
            get
            {
                return _ColParsePlugin;
            }
            set
            {
                _ColParsePlugin = value;
            }
        }

        public string ImageResPath
        {
            get
            {
                return _ImageResPath;
            }
            set
            {
                _ImageResPath = value;
            }
        }

        public User ConnUser
        {
            get { return _user; }
            set { _user = value; }
        }

        //cyf 20110602 add:�����û���Ӧ�Ľ�ɫ������
        public List<Role> LstRoleInfo
        {
            get { return _LstRoleInfo; }
            set { _LstRoleInfo = value; }
        }
        //end

        public object LayerTree
        {
            get
            {
                return _LayerTree;
            }
            set
            {
                _LayerTree = value;
            }
        }
        //added by chulili 2011-11-29 ͼ��Ŀ¼��AdvTree
        public object LayerAdvTree
        {
            get
            {
                return _LayerAdvTree; ;
            }
            set
            {
                _LayerAdvTree = value;
            }
        }
        //added by chulili 2011-12-15 ͼ��Ŀ¼�����ļ�·��
        public string LayerTreeXmlPath
        {
            get
            {
                return _LayerTreeXmlPath; ;
            }
            set
            {
                _LayerTreeXmlPath = value;
            }
        }
        //end added by chulili
        public List<string> ListUserPrivilegeID
        {
            get
            {
                return _ListUserPrivilegeID;
            }
            set
            {
                _ListUserPrivilegeID = value;
            }
        }

        //����������Ϣ
        public ICustomWks CurWksInfo
        {
            get
            {
                return _CurWks;
            }
            set
            {
                _CurWks = value;
            }
        }
        public ICustomWks TempWksInfo
        {
            get
            {
                return _TempWks;
            }
            set
            {
                _TempWks = value;
            }
        }
        #endregion
    }
     
    public class AppSMPD : IAppSMPDRef, IAppFormRef
    {
        private object _LayerTree;                                      //added by chulili 20111119
        private object _LayerAdvTree;  //Ŀ¼��ͼ �������advTree
        private string _LayerTreeXmlPath;                               //added by chulili 20111215
        private Form _MainForm;                                        // ����
        private UserControl _MainUserControl;                         //�ؼ�UserControl
        private Control _StatusStrip;                                 // ״̬��
        private Control _ControlContainer;                             // �ؼ�����
        private XmlDocument _SystemXml;                                // ϵͳ������ͼXml�ڵ�
        private XmlDocument _DataTreeXml;                              // ������ͼXML�ڵ�
        private XmlDocument _DatabaseInfoXml;                          // ������ϢXML�ڵ�
        private Plugin.Parse.PluginCollection _ColParsePlugin;         // ϵͳ�������
        private Dictionary<string, System.Windows.Forms.ContextMenuStrip> _DicContextMenu;           //�Ҽ��˵�����
        private string _ImageResPath;                                  // ͼƬ��Դ·��
        private IMapDocument _MapDocument;                            //�洢���ĵ�����
        private IMapControlDefault _MapControl;                      //MapControl�ؼ�
        private AxMapControl _AxMapControl;

        private ISceneControlDefault _SceneControlDefault;
        private AxSceneControl _SceneControl;
        private IPageLayoutControlDefault _PageLayoutControl;        //PageLayoutControl�ؼ�
        private ITOCControlDefault _TOCControl;                     //TOCControl�ؼ�
        private string _CurrentTool;                                   // ��ǰʹ�õ�TOOL����
        //20110518
        private object _CurrentControl;                             //��ǰͼ����ʾ�ؼ� 
        private List<DevExpress.XtraEditors.Repository.RepositoryItemComboBox> _scaleBoxList;                        //��������ʾ        
        private Dictionary<DevExpress.XtraBars.Ribbon.RibbonPage, string> _dicTabs;          //��tab����ϵͳ����
        //end


        private DevExpress.XtraTreeList.TreeList _DataTree;          // ���ݴ�����ͼ
        private DevExpress.XtraTreeList.TreeList _ErrTree;           // ������ͼ
        private DevExpress.XtraTreeList.TreeList _ProjectTree;       // ������Ϣ��ͼ
        private DevExpress.XtraGrid.Views.Grid.GridView _UpdateDataGrid;   //���¶Աȱ��

        private DevExpress.XtraEditors.Repository.RepositoryItemTextEdit _txtDisplayPage;//���¶Ա��б��ҳ��ʾ�ı���
        private DevExpress.XtraBars.BarButtonItem _btnFirst;//���¶Ա��б��ҳ��ʾ��ť
        private DevExpress.XtraBars.BarButtonItem _btnLast;//���¶Ա��б��ҳ��ʾ��ť
        private DevExpress.XtraBars.BarButtonItem _btnPre;//���¶Ա��б��ҳ��ʾ��ť
        private DevExpress.XtraBars.BarButtonItem _btnNext;//���¶Ա��б��ҳ��ʾ��ť

        private DevExpress.XtraGrid.Views.Grid.GridView _DataCheckGrid;   //���ݼ����
        private XmlDocument _DBXmlDocument;                       //���ݹ�������XML�ļ�

        private Thread _CurrentThread;                           //�������ݵĽ���(Ψһ)

        private User _user;                                          //���ӵ��û���Ϣ
        //cyf 20110602 add
        private List<Role> _LstRoleInfo;                             //���ӵ��û���Ӧ�Ľ�ɫ��Ϣ
        //end
        private List<string> _ListUserPrivilegeID;                   //�û�Ȩ�ޱ��

        //��ʽ��
        private ICustomWks _CurWks;
        //��ʱ��
        private ICustomWks _TempWks;

        //���캯��
        public AppSMPD()
        {
        }

        public AppSMPD(Form pForm, Control ControlContainer, XmlDocument SystemXml, XmlDocument DataTreeXml, XmlDocument DatabaseInfoXml, Plugin.Parse.PluginCollection ColParsePlugin, string ImageResPath)
        {
            //��ϮAppFormRef ����
            _MainForm = pForm;
            _ControlContainer = ControlContainer;
            _SystemXml = SystemXml;
            _DataTreeXml = DataTreeXml;
            _DatabaseInfoXml = DatabaseInfoXml;
            _ColParsePlugin = ColParsePlugin;
            _ImageResPath = ImageResPath;

        }

        #region IAppSMPDRef ��Ա
        public UserControl MainUserControl
        {
            get
            {
                return _MainUserControl;
            }
            set
            {
                _MainUserControl = value;
            }
        }

        public string Name
        {
            get
            {
                return _MainUserControl.Name;
            }
            set
            {
                _MainUserControl.Name = value;
            }
        }

        public string Caption
        {
            get
            {
                return _MainUserControl.Tag as string;
            }
            set
            {
                _MainUserControl.Tag = value;
            }
        }

        public bool Visible
        {
            get
            {
                return _MainUserControl.Visible;
            }
            set
            {
                _MainUserControl.Visible = value;
            }
        }

        public bool Enabled
        {
            get
            {
                return _MainUserControl.Enabled;
            }
            set
            {
                _MainUserControl.Enabled = value;
            }
        }

        public Dictionary<string, System.Windows.Forms.ContextMenuStrip> DicContextMenu
        {
            get
            {
                return _DicContextMenu;
            }
            set
            {
                _DicContextMenu = value;
            }
        }

        public DevExpress.XtraTreeList.TreeList DataTree
        {
            get
            {
                return _DataTree;
            }
            set
            {
                _DataTree = value;
            }
        }

        public DevExpress.XtraEditors.Repository.RepositoryItemTextEdit TxtDisplayPage
        {
            get
            {
                return _txtDisplayPage;
            }
            set
            {
                _txtDisplayPage = value;
            }
        }

        public DevExpress.XtraBars.BarButtonItem BtnFirst
        {
            get
            {
                return _btnFirst;
            }
            set
            {
                _btnFirst = value;
            }
        }
        public DevExpress.XtraBars.BarButtonItem BtnLast
        {
            get
            {
                return _btnLast;
            }
            set
            {
                _btnLast = value;
            }
        }
        public DevExpress.XtraBars.BarButtonItem BtnPre
        {
            get
            {
                return _btnPre;
            }
            set
            {
                _btnPre = value;
            }
        }
        public DevExpress.XtraBars.BarButtonItem BtnNext
        {
            get
            {
                return _btnNext;
            }
            set
            {
                _btnNext = value;
            }
        }

        public DevExpress.XtraTreeList.TreeList ErrTree
        {
            get
            {
                return _ErrTree;
            }
            set
            {
                _ErrTree = value;
            }
        }

        public DevExpress.XtraTreeList.TreeList ProjectTree
        {
            get
            {
                return _ProjectTree;
            }
            set
            {
                _ProjectTree = value;
            }
        }

        public DevExpress.XtraGrid.Views.Grid.GridView UpdateDataGrid
        {
            get
            {
                return _UpdateDataGrid;
            }
            set
            {
                _UpdateDataGrid = value;
            }
        }

        public DevExpress.XtraGrid.Views.Grid.GridView DataCheckGrid
        {
            get
            {
                return _DataCheckGrid;
            }
            set
            {
                _DataCheckGrid = value;
            }
        }

        public XmlDocument DBXmlDocument
        {
            get
            {
                return _DBXmlDocument;
            }
            set
            {
                _DBXmlDocument = value;
            }
        }

        public Control StatusBar
        {
            get
            {
                return _StatusStrip;
            }
            set
            {
                _StatusStrip = value;
            }
        }

        public Thread CurrentThread
        {
            get
            {
                return _CurrentThread;
            }
            set
            {
                _CurrentThread = value;
            }
        }
        #endregion

        #region ״̬������
        public string OperatorTips
        {
            get;set;
        }

        public DevExpress.XtraEditors.Repository.RepositoryItemProgressBar ProgressBar
        {
            get; set;
        }

        //�ο�������cmb
        public DevExpress.XtraEditors.Repository.RepositoryItemComboBox RefScaleCmb
        {
            get; set;
        }
        //��ǰ������cmb
        public DevExpress.XtraEditors.Repository.RepositoryItemComboBox CurScaleCmb
        {
            get; set;
        }
        //������ʾ�ı���
        public DevExpress.XtraEditors.Repository.RepositoryItemTextEdit CoorTxt
        {
            get; set;
        }

        public string UserInfo
        {
            get; set;
        }

        public string CoorXY
        {
            get
            {
                return string.Empty;
            }
            set
            {
            }
        }
        #endregion

        #region IAppArcGISRef ��Ա
        public string CurrentTool
        {
            get
            {
                return _CurrentTool;
            }
            set
            {
                _CurrentTool = value;
            }
        }

        public IMapDocument MapDocument
        {
            get
            {
                return _MapDocument;
            }
            set
            {
                _MapDocument = value;
            }
        }

        public IMapControlDefault MapControl
        {
            get
            {
                return _MapControl;
            }
            set
            {
                _MapControl = value;
            }
        }

        public PictureBox p1 { get; set; }
        public PictureBox p2 { get; set; }
        public PictureBox p3 { get; set; }
        public PictureBox p4 { get; set; }
        public AxMapControl ArcGisMapControl
        {
            get
            {
                return _AxMapControl;
            }
            set
            {
                _AxMapControl = value;
            }
        }

        public ISceneControlDefault SceneControlDefault
        {
            get { return _SceneControlDefault; }
            set { _SceneControlDefault = value; }
        }

        public AxSceneControl SceneControl
        {
            get { return _SceneControl; }
            set { _SceneControl = value; }
        }

        public IPageLayoutControlDefault PageLayoutControl
        {
            get
            {
                return _PageLayoutControl;
            }
            set
            {
                _PageLayoutControl = value;
            }
        }

        public ITOCControlDefault TOCControl
        {
            get
            {
                return _TOCControl;
            }
            set
            {
                _TOCControl = value;
            }
        }

        //cyf 20110518  
        public object CurrentControl
        {
            get
            {
                return _CurrentControl;
            }
            set
            {
                _CurrentControl = value;
            }
        }
        public List<DevExpress.XtraEditors.Repository.RepositoryItemComboBox> ScaleBoxList
        {
            get
            {
                return _scaleBoxList;
            }
            set
            {
                _scaleBoxList = value;
            }
        }

        public Dictionary<DevExpress.XtraBars.Ribbon.RibbonPage, string> DicTabs
        {
            get
            {
                return _dicTabs;
            }
            set
            {
                _dicTabs = value;
            }
        }
        //end

        #endregion

        #region IAppFormDef ��Ա

        Form IAppFormRef.MainForm
        {
            get
            {
                return _MainForm;
            }
            set
            {
                _MainForm = value;
            }
        }

        public Control ControlContainer
        {
            get
            {
                return _ControlContainer;
            }
            set
            {
                _ControlContainer = value;
            }
        }

        string IAppFormRef.CurrentSysName
        {
            get
            {
                return null;
            }
            set
            {
            }
        }

        public XmlDocument SystemXml
        {
            get
            {
                return _SystemXml;
            }
            set
            {
                _SystemXml = value;
            }
        }

        public XmlDocument DataTreeXml
        {
            get
            {
                return _DataTreeXml;
            }
            set
            {
                _DataTreeXml = value;
            }
        }

        public XmlDocument DatabaseInfoXml
        {
            get
            {
                return _DatabaseInfoXml;
            }
            set
            {
                _DatabaseInfoXml = value;
            }
        }

        public Plugin.Parse.PluginCollection ColParsePlugin
        {
            get
            {
                return _ColParsePlugin;
            }
            set
            {
                _ColParsePlugin = value;
            }
        }

        public string ImageResPath
        {
            get
            {
                return _ImageResPath;
            }
            set
            {
                _ImageResPath = value;
            }
        }

        public User ConnUser
        {
            get { return _user; }
            set { _user = value; }
        }

        //cyf 20110602 add:�����û���Ӧ�Ľ�ɫ������
        public List<Role> LstRoleInfo
        {
            get { return _LstRoleInfo; }
            set { _LstRoleInfo = value; }
        }
        //end

        public object LayerTree
        {
            get
            {
                return _LayerTree;
            }
            set
            {
                _LayerTree = value;
            }
        }
        //added by chulili 2011-11-29 ͼ��Ŀ¼��AdvTree
        public object LayerAdvTree
        {
            get
            {
                return _LayerAdvTree; ;
            }
            set
            {
                _LayerAdvTree = value;
            }
        }
        //added by chulili 2011-12-15 ͼ��Ŀ¼�����ļ�·��
        public string LayerTreeXmlPath
        {
            get
            {
                return _LayerTreeXmlPath; ;
            }
            set
            {
                _LayerTreeXmlPath = value;
            }
        }
        //end added by chulili
        public List<string> ListUserPrivilegeID
        {
            get
            {
                return _ListUserPrivilegeID;
            }
            set
            {
                _ListUserPrivilegeID = value;
            }
        }

        //����������Ϣ
        public ICustomWks CurWksInfo
        {
            get
            {
                return _CurWks;
            }
            set
            {
                _CurWks = value;
            }
        }
        public ICustomWks TempWksInfo
        {
            get
            {
                return _TempWks;
            }
            set
            {
                _TempWks = value;
            }
        }

        #endregion
    }
    public class AppPrivileges : IAppPrivilegesRef
    {
        //added by chulili 20111120
        private IMapControlDefault _MapControl;                      //MapControl�ؼ�
        private ITOCControlDefault _TOCControl;                     //TOCControl�ؼ�
        //end added by chulili
        private object _LayerTree;                                      //added by chulili 20111119
        private object _LayerAdvTree;  //Ŀ¼��ͼ �������advTree
        private string _LayerTreeXmlPath;                               //added by chulili 20111215
        private Form _MainForm;                                        // ����
        private UserControl _MainUserControl;                         //�ؼ�UserControl
        private Control _StatusStrip;                                 // ״̬��
        private Control _ControlContainer;                             // �ؼ�����
        private XmlDocument _SystemXml;                                // ϵͳ������ͼXml�ڵ�
        private XmlDocument _DataTreeXml;                              // ������ͼXML�ڵ�
        private XmlDocument _DatabaseInfoXml;                          // ������ϢXML�ڵ�
        private Plugin.Parse.PluginCollection _ColParsePlugin;         // ϵͳ�������
        private Dictionary<string, System.Windows.Forms.ContextMenuStrip> _DicContextMenu;           //�Ҽ��˵�����
        private string _ImageResPath;


        //wgf 2011-5-18
        //--------------------------------------------------
        private System.Windows.Forms.TreeView _DataTabIndexTree;

        private System.Windows.Forms.DataGridView _GridCtrl;

        private System.Windows.Forms.RichTextBox _tipRichBox;

        private string _strLogFilePath;
        //--------------------------------------------------

        private DevExpress.XtraTreeList.TreeList _MainTree;          // ��������ͼ
        private DevExpress.XtraTreeList.TreeList _RoleTree;           // �û�����ͼ
        private DevExpress.XtraTreeList.TreeList _UserTree;       // �û���ͼ
        private DevExpress.XtraTreeList.TreeList _privilegeTree;       //Ȩ����
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup _CurrentPanel;       // ��ǰGroupPanel
        private User _user;                                          //���ӵ��û���Ϣ 
        //cyf 20110602 add
        private List<Role> _LstRoleInfo;                             //���ӵ��û���Ӧ�Ľ�ɫ��Ϣ
        //end

        private Thread _CurrentThread;                           //�������ݵĽ���(Ψһ)
        private List<string> _ListUserPrivilegeID;                   //�û�Ȩ�ޱ��

        //��ʽ��
        private ICustomWks _CurWks;
        //��ʱ��
        private ICustomWks _TempWks;

        //���캯��
        public AppPrivileges()
        {
        }

        public AppPrivileges(Form pForm, Control ControlContainer, List<string> ListUserPrivilegeID, XmlDocument SystemXml, XmlDocument DataTreeXml, XmlDocument DatabaseInfoXml, Plugin.Parse.PluginCollection ColParsePlugin, string ImageResPath, User V_user)
        {
            //��ϮAppFormRef ����
            _MainForm = pForm;
            _ControlContainer = ControlContainer;
            _SystemXml = SystemXml;
            _DataTreeXml = DataTreeXml;
            _DatabaseInfoXml = DatabaseInfoXml;
            _ColParsePlugin = ColParsePlugin;
            _ImageResPath = ImageResPath;
            _user = V_user;
            _ListUserPrivilegeID = ListUserPrivilegeID;
        }

        #region ��Ա
        public UserControl MainUserControl
        {
            get
            {
                return _MainUserControl;
            }
            set
            {
                _MainUserControl = value;
            }
        }
        public IMapControlDefault MapControl
        {
            get
            {
                return _MapControl;
            }
            set
            {
                _MapControl = value;
            }
        }
        public ITOCControlDefault TOCControl
        {
            get
            {
                return _TOCControl;
            }
            set
            {
                _TOCControl = value;
            }
        }
        public string Name
        {
            get
            {
                return _MainUserControl.Name;
            }
            set
            {
                _MainUserControl.Name = value;
            }
        }

        public string Caption
        {
            get
            {
                return _MainUserControl.Tag as string;
            }
            set
            {
                _MainUserControl.Tag = value;
            }
        }

        public bool Visible
        {

            get
            {
                if (_MainUserControl == null) return false;
                return _MainUserControl.Visible;
            }
            set
            {
                if (_MainUserControl == null) return;
                _MainUserControl.Visible = value;
            }
        }

        public bool Enabled
        {
            get
            {
                return _MainUserControl.Enabled;
            }
            set
            {
                _MainUserControl.Enabled = value;
            }
        }

        public Dictionary<string, System.Windows.Forms.ContextMenuStrip> DicContextMenu
        {
            get
            {
                return _DicContextMenu;
            }
            set
            {
                _DicContextMenu = value;
            }
        }

        public DevExpress.XtraTreeList.TreeList MainTree
        {
            get
            {
                return _MainTree;
            }
            set
            {
                _MainTree = value;
            }
        }

        public DevExpress.XtraTreeList.TreeList RoleTree
        {
            get
            {
                return _RoleTree;
            }
            set
            {
                _RoleTree = value;
            }
        }

        public DevExpress.XtraTreeList.TreeList UserTree
        {
            get
            {
                return _UserTree;
            }
            set
            {
                _UserTree = value;
            }
        }

        public DevExpress.XtraTreeList.TreeList PrivilegeTree
        {
            get
            {
                return _privilegeTree;
            }
            set
            {
                _privilegeTree = value;
            }
        }

        public Control StatusBar
        {
            get
            {
                return _StatusStrip;
            }
            set
            {
                _StatusStrip = value;
            }
        }

        public Thread CurrentThread
        {
            get
            {
                return _CurrentThread;
            }
            set
            {
                _CurrentThread = value;
            }
        }
        #endregion

        #region ״̬������
        public string OperatorTips
        {
            get;set;
        }

        public DevExpress.XtraEditors.Repository.RepositoryItemProgressBar ProgressBar
        {
            get; set;
        }

        //�ο�������cmb
        private DevExpress.XtraEditors.Repository.RepositoryItemComboBox _RefScaleCmb;
        public DevExpress.XtraEditors.Repository.RepositoryItemComboBox RefScaleCmb
        {
            get
            {
                return _RefScaleCmb;
            }
            set
            {
                _RefScaleCmb = value;
            }
        }
        //�ο��������Ƿ�ɼ�
        public bool RefScaleVisible
        {
            get; set;
        }
        //��ǰ�������Ƿ�ɼ�
        public bool CurScaleVisible
        {
            get; set;
        }
        //��ǰ������cmb
        public DevExpress.XtraEditors.Repository.RepositoryItemComboBox CurScaleCmb
        {
            get; set;
        }
        //end ������  �ӱ𴦿���
        //������ʾ�ı���
        private DevExpress.XtraEditors.Repository.RepositoryItemTextEdit _CoorTxt;
        public DevExpress.XtraEditors.Repository.RepositoryItemTextEdit CoorTxt
        {
            get
            {
                return _CoorTxt;
            }
            set
            {
                _CoorTxt = value;
            }
        }

        public string UserInfo
        {
            get; set;
        }

        public string CoorXY
        {
            get; set;
        }
        #endregion

        #region IAppFormDef ��Ա

        Form IAppFormRef.MainForm
        {
            get
            {
                return _MainForm;
            }
            set
            {
                _MainForm = value;
            }
        }

        public Control ControlContainer
        {
            get
            {
                return _ControlContainer;
            }
            set
            {
                _ControlContainer = value;
            }
        }

        string IAppFormRef.CurrentSysName
        {
            get
            {
                return null;
            }
            set
            {
            }
        }

        public XmlDocument SystemXml
        {
            get
            {
                return _SystemXml;
            }
            set
            {
                _SystemXml = value;
            }
        }

        public XmlDocument DataTreeXml
        {
            get
            {
                return _DataTreeXml;
            }
            set
            {
                _DataTreeXml = value;
            }
        }

        public XmlDocument DatabaseInfoXml
        {
            get
            {
                return _DatabaseInfoXml;
            }
            set
            {
                _DatabaseInfoXml = value;
            }
        }

        public Plugin.Parse.PluginCollection ColParsePlugin
        {
            get
            {
                return _ColParsePlugin;
            }
            set
            {
                _ColParsePlugin = value;
            }
        }

        public string ImageResPath
        {
            get
            {
                return _ImageResPath;
            }
            set
            {
                _ImageResPath = value;
            }
        }

        public User ConnUser
        {
            get { return _user; }
            set { _user = value; }
        }

        //cyf 20110602 add:�����û���Ӧ�Ľ�ɫ������
        public List<Role> LstRoleInfo
        {
            get { return _LstRoleInfo; }
            set { _LstRoleInfo = value; }
        }
        //end

        public object LayerTree
        {
            get
            {
                return _LayerTree;
            }
            set
            {
                _LayerTree = value;
            }
        }
        //added by chulili 2011-11-29 ͼ��Ŀ¼��AdvTree
        public object LayerAdvTree
        {
            get
            {
                return _LayerAdvTree; ;
            }
            set
            {
                _LayerAdvTree = value;
            }
        }
        //added by chulili 2011-12-15 ͼ��Ŀ¼�����ļ�·��
        public string LayerTreeXmlPath
        {
            get
            {
                return _LayerTreeXmlPath; ;
            }
            set
            {
                _LayerTreeXmlPath = value;
            }
        }
        //end added by chulili
        //wgf 2011-5-18
        //--------------------------------------------
        public System.Windows.Forms.TreeView DataTabIndexTree
        {
            get
            {
                return _DataTabIndexTree;
            }
            set
            {
                _DataTabIndexTree = value;
            }
        }
        public System.Windows.Forms.DataGridView GridCtrl
        {
            get
            {
                return _GridCtrl;
            }
            set
            {
                _GridCtrl = value;
            }
        }

        public System.Windows.Forms.RichTextBox tipRichBox
        {
            get
            {
                return _tipRichBox;
            }
            set
            {
                _tipRichBox = value;
            }
        }

        public string strLogFilePath
        {
            get
            {
                return _strLogFilePath;
            }
            set
            {
                _strLogFilePath = value;
            }
        }

        public List<string> ListUserPrivilegeID
        {
            get
            {
                return _ListUserPrivilegeID;
            }
            set
            {
                _ListUserPrivilegeID = value;
            }
        }

        //����������Ϣ
        public ICustomWks CurWksInfo
        {
            get
            {
                return _CurWks;
            }
            set
            {
                _CurWks = value;
            }
        }
        public ICustomWks TempWksInfo
        {
            get
            {
                return _TempWks;
            }
            set
            {
                _TempWks = value;
            }
        }
        //---------------------------------------------

        #endregion
    }

    public class AppGidUpdate : IAppGisUpdateRef, IAppFormRef
    {
        private Form _MainForm;                                        // ����
        private UserControl _MainUserControl;                         //�ؼ�UserControl
        private Control _StatusStrip;                                 // ״̬��
        private Control _ControlContainer;                             // �ؼ�����
        private XmlDocument _SystemXml;                                // ϵͳ������ͼXml�ڵ�
        private XmlDocument _DataTreeXml;                              // ������ͼXML�ڵ�
        private XmlDocument _DatabaseInfoXml;                          // ������ϢXML�ڵ�
        private Plugin.Parse.PluginCollection _ColParsePlugin;         // ϵͳ�������
        private Dictionary<string, System.Windows.Forms.ContextMenuStrip> _DicContextMenu;           //�Ҽ��˵�����
        private string _ImageResPath;                                  // ͼƬ��Դ·��
        private IMapDocument _MapDocument;                            //�洢���ĵ�����
        private IMapControlDefault _MapControl;                      //MapControl�ؼ�
        private AxMapControl _AxMapControl;
        PictureBox _p1; PictureBox _p2; PictureBox _p3; PictureBox _p4;//ʮ��˿
        private IGeometry _Geometry; //��ǰ���Ƶ�ͼ��Ҫ��
        private ISceneControlDefault _SceneControlDefault;
        private AxSceneControl _SceneControl;
        private IPageLayoutControlDefault _PageLayoutControlDefault;        //PageLayoutControl�ؼ�
        //private AxPageLayoutControl _PageLayoutControl;                 //PageLayoutControl�ؼ�
        private short _CurrentPrintPage = 0;                           //����ĵ�ҳ��
        private PrintDocument _PrintDocument;                       //��ӡ����ĵ�
        private ITOCControlDefault _TOCControl;                     //TOCControl�ؼ�
        private string _CurrentTool;                                   // ��ǰʹ�õ�TOOL����
        private object _CurrentControl;                             //��ǰͼ����ʾ�ؼ�
        private List<DevExpress.XtraEditors.Repository.RepositoryItemComboBox> _scaleBoxList;                        //��������ʾ        
        private Dictionary<DevExpress.XtraBars.Ribbon.RibbonPage, string> _dicTabs;          //��tab����ϵͳ����

        private string _LayerTreeXmlPath = "";  //added by chulili 20111101 
        private object _LayerTree;  //Ŀ¼��ͼ �������GeoLayerTreeLib.LayerManager.UcDataLib
        private object _LayerAdvTree;  //Ŀ¼��ͼ �������advTree
        private DevExpress.XtraTreeList.TreeList _DataTree;          // ���ݴ�����ͼ
        private DevExpress.XtraTreeList.TreeList _XZQTree;          // yjl20110924 add ��������ͼ
        private DevExpress.XtraTreeList.TreeList _ResultFileTree;      //added by chulili 20110923 �ɹ��б���ͼ
        private DevExpress.XtraTreeList.TreeList _ErrTree;           // ������ͼ
        private DevExpress.XtraTreeList.TreeList _ProjectTree;       // ������Ϣ��ͼ
        private DevExpress.XtraGrid.Views.Grid.GridView _UpdateDataGrid;   //���¶Աȱ��
        private DevExpress.XtraGrid.Views.Grid.GridView _DataCheckGrid;   //���ݼ����
        private XmlDocument _DBXmlDocument;                       //���ݹ�������XML�ļ�
        private User _user;                                          //���ӵ��û���Ϣ 
        //cyf 20110602 add
        private List<Role> _LstRoleInfo;                             //���ӵ��û���Ӧ�Ľ�ɫ��Ϣ
        //end
        private TreeView _ResultsTree;   //��ǰ�ɹ�Ŀ¼�� xisheng 20120612 ��ǰ��Ϊ��ͼ�ļ�����Ŀ¼��

        private Thread _CurrentThread;                           //�������ݵĽ���(Ψһ)

        //wgf20110518
        //--------------------------------------------------------
        //   private IFeatureWorkspace _pFeatureWorkspace;  //arcgis�����ռ�
        private System.Windows.Forms.TreeView _DataUnitTree;          // ���ݵ�Ԫ��ͼ
        private System.Windows.Forms.TreeView _DataIndexTree;          // ��ԴĿ¼��ͼ
        private System.Windows.Forms.TreeView _MapDocTree;          // ��ͼ�ĵ���ͼ
        private System.Windows.Forms.TreeView _TextDocTree;          // �ĵ���ͼ
        private System.Windows.Forms.TreeView _UserResultTree;          // �û��ɹ���ͼ
        private System.Windows.Forms.TreeView _MetadataTree;          // yjl20110926 add Ԫ������
        private DevExpress.XtraBars.Ribbon.RibbonPage _IndextabControl;//������tabcontrol ϯʤ
        private System.Windows.Forms.RichTextBox _DocControl; //�ĵ�����
        private System.Windows.Forms.RichTextBox _tipRichBox;
        private SysCommon.BottomQueryBar _QueryBar; //added by chulili 2012-08-09

        private SysGisDataSet _gisDataSet;           //���ݿ������ wgf
        private string _strLogFilePath;  //��־�ļ�
        //--------------------------------------------------------
        private List<string> _ListUserPrivilegeID;                   //�û�Ȩ�ޱ��

        //��ʽ��
        private ICustomWks _CurWks;
        //��ʱ��
        private ICustomWks _TempWks;

        //���캯��
        public AppGidUpdate()
        {
        }

        public AppGidUpdate(Form pForm, Control ControlContainer, List<string> ListUserPrivilegeID, XmlDocument SystemXml, XmlDocument DataTreeXml, XmlDocument DatabaseInfoXml, Plugin.Parse.PluginCollection ColParsePlugin, string ImageResPath, User V_user)
        {
            //��ϮAppFormRef ����
            _MainForm = pForm;
            _ControlContainer = ControlContainer;
            _SystemXml = SystemXml;
            _DataTreeXml = DataTreeXml;
            _DatabaseInfoXml = DatabaseInfoXml;
            _ColParsePlugin = ColParsePlugin;
            _ImageResPath = ImageResPath;
            _user = V_user;
            _ListUserPrivilegeID = ListUserPrivilegeID;
        }

        #region IAppSMPDRef ��Ա
        public UserControl MainUserControl
        {
            get
            {
                return _MainUserControl;
            }
            set
            {
                _MainUserControl = value;
            }
        }

        public string Name
        {
            get
            {
                return _MainUserControl.Name;
            }
            set
            {
                _MainUserControl.Name = value;
            }
        }

        public string Caption
        {
            get
            {
                return _MainUserControl.Tag as string;
            }
            set
            {
                _MainUserControl.Tag = value;
            }
        }

        public bool Visible
        {
            get
            {
                return _MainUserControl.Visible;
            }
            set
            {
                _MainUserControl.Visible = value;
            }
        }

        public bool Enabled
        {
            get
            {
                return _MainUserControl.Enabled;
            }
            set
            {
                _MainUserControl.Enabled = value;
            }
        }

        public Dictionary<string, System.Windows.Forms.ContextMenuStrip> DicContextMenu
        {
            get
            {
                return _DicContextMenu;
            }
            set
            {
                _DicContextMenu = value;
            }
        }

        public DevExpress.XtraTreeList.TreeList DataTree
        {
            get
            {
                return _DataTree;
            }
            set
            {
                _DataTree = value;
            }
        }
        public object LayerTree
        {
            get
            {
                return _LayerTree; ;
            }
            set
            {
                _LayerTree = value;
            }
        }
        //added by chulili 2011-11-29 ͼ��Ŀ¼��AdvTree
        public object LayerAdvTree
        {
            get
            {
                return _LayerAdvTree; ;
            }
            set
            {
                _LayerAdvTree = value;
            }
        }
        //end added by chulili
        public string LayerTreeXmlPath
        {
            get
            {
                return _LayerTreeXmlPath; ; ;
            }
            set
            {
                _LayerTreeXmlPath = value;
            }
        }
        public DevExpress.XtraTreeList.TreeList XZQTree
        {
            get
            {
                return _XZQTree;
            }
            set
            {
                _XZQTree = value;
            }
        }
        public DevExpress.XtraTreeList.TreeList ResultFileTree
        {
            get
            {
                return _ResultFileTree;
            }
            set
            {
                _ResultFileTree = value;
            }
        }
        public DevExpress.XtraTreeList.TreeList ErrTree
        {
            get
            {
                return _ErrTree;
            }
            set
            {
                _ErrTree = value;
            }
        }

        public DevExpress.XtraTreeList.TreeList ProjectTree
        {
            get
            {
                return _ProjectTree;
            }
            set
            {
                _ProjectTree = value;
            }
        }

        //wgf 20110518
        //-------------------------------------------------

        public System.Windows.Forms.RichTextBox DocControl
        {
            get
            {
                return _DocControl;
            }
            set
            {
                _DocControl = value;
            }
        }
        public System.Windows.Forms.TreeView DataUnitTree
        {
            get
            {
                return _DataUnitTree;
            }
            set
            {
                _DataUnitTree = value;
            }
        }

        public System.Windows.Forms.TreeView DataIndexTree
        {
            get
            {
                return _DataIndexTree;
            }
            set
            {
                _DataIndexTree = value;
            }
        }
        public TreeView ResultsTree//��ͼ�ļ�Ŀ¼ xisheng 20120612
        {
            get
            {
                return _ResultsTree;
            }
            set
            {
                _ResultsTree = value;
            }
        }
        public System.Windows.Forms.TreeView MapDocTree
        {
            get
            {
                return _MapDocTree;
            }
            set
            {
                _MapDocTree = value;
            }
        }

        public System.Windows.Forms.TreeView TextDocTree
        {
            get
            {
                return _TextDocTree;
            }
            set
            {
                _TextDocTree = value;
            }
        }

        public System.Windows.Forms.TreeView UserResultTree
        {
            get
            {
                return _UserResultTree;
            }
            set
            {
                _UserResultTree = value;
            }
        }
        public System.Windows.Forms.TreeView MetadataTree
        {
            get
            {
                return _MetadataTree;
            }
            set
            {
                _MetadataTree = value;
            }
        }

        public DevExpress.XtraBars.Ribbon.RibbonPage IndextabControl
        {
            get
            {
                return _IndextabControl;
            }
            set
            {
                _IndextabControl = value;
            }
        }

        public System.Windows.Forms.RichTextBox tipRichBox
        {
            get
            {
                return _tipRichBox;
            }
            set
            {
                _tipRichBox = value;
            }
        }
        public SysCommon.BottomQueryBar QueryBar
        {
            get
            {
                return _QueryBar;
            }
            set
            {
                _QueryBar = value;
            }
        }

        public string strLogFilePath
        {
            get
            {
                return _strLogFilePath;
            }
            set
            {
                _strLogFilePath = value;
            }
        }


        public SysGisDataSet gisDataSet
        {
            get
            {
                return _gisDataSet;
            }
            set
            {
                _gisDataSet = value;
            }
        }
        //--------------------------------------------------
        public DevExpress.XtraGrid.Views.Grid.GridView UpdateDataGrid
        {
            get
            {
                return _UpdateDataGrid;
            }
            set
            {
                _UpdateDataGrid = value;
            }
        }

        public DevExpress.XtraGrid.Views.Grid.GridView DataCheckGrid
        {
            get
            {
                return _DataCheckGrid;
            }
            set
            {
                _DataCheckGrid = value;
            }
        }

        public XmlDocument DBXmlDocument
        {
            get
            {
                return _DBXmlDocument;
            }
            set
            {
                _DBXmlDocument = value;
            }
        }

        public Control StatusBar
        {
            get
            {
                return _StatusStrip;
            }
            set
            {
                _StatusStrip = value;
            }
        }

        public Thread CurrentThread
        {
            get
            {
                return _CurrentThread;
            }
            set
            {
                _CurrentThread = value;
            }
        }
        #endregion

        #region ״̬������
        public string OperatorTips
        {
            get;set;
        }

        public DevExpress.XtraEditors.Repository.RepositoryItemProgressBar ProgressBar
        {
            get; set;
        }
        public DevExpress.XtraEditors.Repository.RepositoryItemComboBox RefScaleCmb
        {
            get; set;
        }
        //��ǰ������cmb
        public DevExpress.XtraEditors.Repository.RepositoryItemComboBox CurScaleCmb
        {
            get; set;
        }
        //end ������  �ӱ𴦿���
        //������ʾ�ı���
        private DevExpress.XtraEditors.Repository.RepositoryItemTextEdit _CoorTxt;
        public DevExpress.XtraEditors.Repository.RepositoryItemTextEdit CoorTxt
        {
            get
            {
                return _CoorTxt;
            }
            set
            {
                _CoorTxt = value;
            }
        }

        public string UserInfo
        {
            get; set;
        }

        public string CoorXY
        {
            get; set;
        }
        #endregion

        #region IAppArcGISRef ��Ա
        public string CurrentTool
        {
            get
            {
                return _CurrentTool;
            }
            set
            {
                _CurrentTool = value;
            }
        }

        public IMapDocument MapDocument
        {
            get
            {
                return _MapDocument;
            }
            set
            {
                _MapDocument = value;
            }
        }

        public PrintDocument MyDocument
        {
            get
            {
                return _PrintDocument;
            }
            set
            {
                _PrintDocument = value;
            }
        }
        public PictureBox p1
        {
            get
            { return _p1; }
            set
            { _p1 = value; }
        }

        public PictureBox p2
        {
            get
            { return _p2; }
            set
            { _p2 = value; }
        }

        public PictureBox p3
        {
            get
            { return _p3; }
            set
            { _p3 = value; }
        }

        public PictureBox p4
        {
            get
            { return _p4; }
            set
            { _p4 = value; }
        }
        public IGeometry pGeometry
        {
            get
            {
                return _Geometry;
            }
            set
            {
                _Geometry = value;
            }
        }
        public IMapControlDefault MapControl
        {
            get
            {
                return _MapControl;
            }
            set
            {
                _MapControl = value;
            }
        }

        public AxMapControl ArcGisMapControl
        {
            get
            {
                return _AxMapControl;
            }
            set
            {
                _AxMapControl = value;
            }
        }

        public ISceneControlDefault SceneControlDefault
        {
            get { return _SceneControlDefault; }
            set { _SceneControlDefault = value; }
        }

        public AxSceneControl SceneControl
        {
            get { return _SceneControl; }
            set { _SceneControl = value; }
        }

        public IPageLayoutControlDefault PageLayoutControl
        {
            get
            {
                return _PageLayoutControlDefault;
            }
            set
            {
                _PageLayoutControlDefault = value;
            }
        }
        public short CurrentPrintPage
        {
            get
            {
                return _CurrentPrintPage;
            }
            set
            {
                _CurrentPrintPage = value;
            }
        }

        //public AxPageLayoutControl PageLayoutControl
        //{
        //    get
        //    {
        //        return _PageLayoutControl;
        //    }
        //    set
        //    {
        //        _PageLayoutControl = value;
        //    }
        //}

        public ITOCControlDefault TOCControl
        {
            get
            {
                return _TOCControl;
            }
            set
            {
                _TOCControl = value;
            }
        }

        public object CurrentControl
        {
            get
            {
                return _CurrentControl;
            }
            set
            {
                _CurrentControl = value;
            }
        }
        public List<DevExpress.XtraEditors.Repository.RepositoryItemComboBox> ScaleBoxList
        {
            get
            {
                return _scaleBoxList;
            }
            set
            {
                _scaleBoxList = value;
            }
        }

        public Dictionary<DevExpress.XtraBars.Ribbon.RibbonPage, string> DicTabs
        {
            get
            {
                return _dicTabs;
            }
            set
            {
                _dicTabs = value;
            }
        }
        #endregion

        #region IAppFormDef ��Ա

        Form IAppFormRef.MainForm
        {
            get
            {
                return _MainForm;
            }
            set
            {
                _MainForm = value;
            }
        }

        public Control ControlContainer
        {
            get
            {
                return _ControlContainer;
            }
            set
            {
                _ControlContainer = value;
            }
        }

        string IAppFormRef.CurrentSysName
        {
            get
            {
                return null;
            }
            set
            {
            }
        }

        public XmlDocument SystemXml
        {
            get
            {
                return _SystemXml;
            }
            set
            {
                _SystemXml = value;
            }
        }

        public XmlDocument DataTreeXml
        {
            get
            {
                return _DataTreeXml;
            }
            set
            {
                _DataTreeXml = value;
            }
        }

        public XmlDocument DatabaseInfoXml
        {
            get
            {
                return _DatabaseInfoXml;
            }
            set
            {
                _DatabaseInfoXml = value;
            }
        }

        public Plugin.Parse.PluginCollection ColParsePlugin
        {
            get
            {
                return _ColParsePlugin;
            }
            set
            {
                _ColParsePlugin = value;
            }
        }

        public string ImageResPath
        {
            get
            {
                return _ImageResPath;
            }
            set
            {
                _ImageResPath = value;
            }
        }

        public User ConnUser
        {
            get { return _user; }
            set { _user = value; }
        }

        //cyf 20110602 add:�����û���Ӧ�Ľ�ɫ������
        public List<Role> LstRoleInfo
        {
            get { return _LstRoleInfo; }
            set { _LstRoleInfo = value; }
        }
        //end


        public List<string> ListUserPrivilegeID
        {
            get
            {
                return _ListUserPrivilegeID;
            }
            set
            {
                _ListUserPrivilegeID = value;
            }
        }

        //����������Ϣ
        public ICustomWks CurWksInfo
        {
            get
            {
                return _CurWks;
            }
            set
            {
                _CurWks = value;
            }
        }
        public ICustomWks TempWksInfo
        {
            get
            {
                return _TempWks;
            }
            set
            {
                _TempWks = value;
            }
        }
        #endregion
    }

    public class AppFileDB : IAppFileRef, IAppFormRef
    {
        private object _LayerTree;                                      //added by chulili 20111119
        private object _LayerAdvTree;  //Ŀ¼��ͼ �������advTree
        private string _LayerTreeXmlPath;                               //added by chulili 20111215
        //wgf 2011-5-18
        //--------------------------------------------------
        private System.Windows.Forms.TreeView _DataTabIndexTree;

        private System.Windows.Forms.DataGridView _GridCtrl;

        private System.Windows.Forms.RichTextBox _tipRichBox;

        private string _strLogFilePath;
        //--------------------------------------------------

        private Form _MainForm;                                        // ����
        private UserControl _MainUserControl;                         //�ؼ�UserControl
        private Control _StatusStrip;                                 // ״̬��
        private Control _ControlContainer;                             // �ؼ�����
        private XmlDocument _SystemXml;                                // ϵͳ������ͼXml�ڵ�
        private XmlDocument _DataTreeXml;                              // ������ͼXML�ڵ�
        private XmlDocument _DatabaseInfoXml;                          // ������ϢXML�ڵ�
        private Plugin.Parse.PluginCollection _ColParsePlugin;         // ϵͳ�������
        private Dictionary<string, System.Windows.Forms.ContextMenuStrip> _DicContextMenu;           //�Ҽ��˵�����
        private string _ImageResPath;                                  // ͼƬ��Դ·��
        private IMapDocument _MapDocument;                            //�洢���ĵ�����
        private IMapControlDefault _MapControl;                      //MapControl�ؼ�
        private AxMapControl _AxMapControl;
        private ISceneControlDefault _SceneControlDefault;
        private AxSceneControl _SceneControl;

        private IPageLayoutControlDefault _PageLayoutControl;        //PageLayoutControl�ؼ�
        private ITOCControlDefault _TOCControl;                     //TOCControl�ؼ�
        private string _CurrentTool;                                   // ��ǰʹ�õ�TOOL����
        //20110518
        private object _CurrentControl;                             //��ǰͼ����ʾ�ؼ�
        private List<DevExpress.XtraEditors.Repository.RepositoryItemComboBox> _scaleBoxList;                        //��������ʾ        
        private Dictionary<DevExpress.XtraBars.Ribbon.RibbonPage, string> _dicTabs;          //��tab����ϵͳ����
        //end


        private DevExpress.XtraTreeList.TreeList _ProjectTree;       // ������Ϣ��
        private DevExpress.XtraGrid.Views.Grid.GridView _DataInfoGrid;  //������Ϣ�б�
        private DevExpress.XtraGrid.Views.Grid.GridView _MetaDataGrid;  //Ԫ�����б�

        private DevExpress.XtraTreeList.TreeList _SysSettingTree;//ϵͳ������ͼ
        private DevExpress.XtraGrid.Views.Grid.GridView _SysSettingGrid;//ϵͳ�����б�

        //private DevExpress.XtraTreeList.TreeList _DataTree;          // ���ݴ�����ͼ
        //private DevExpress.XtraGrid.Views.Grid.GridView _UpdateGrid;  //���¶Ա��б�
        private DevExpress.XtraEditors.Repository.RepositoryItemTextEdit _txtDisplayPage;//���¶Ա��б��ҳ��ʾ�ı���
        private DevExpress.XtraBars.BarButtonItem _btnFirst;//���¶Ա��б��ҳ��ʾ��ť
        private DevExpress.XtraBars.BarButtonItem _btnLast;//���¶Ա��б��ҳ��ʾ��ť
        private DevExpress.XtraBars.BarButtonItem _btnPre;//���¶Ա��б��ҳ��ʾ��ť
        private DevExpress.XtraBars.BarButtonItem _btnNext;//���¶Ա��б��ҳ��ʾ��ť

        private Thread _CurrentThread;                           //�������ݵĽ���(Ψһ)
        private XmlDocument _DBXmlDocument;                       //���ݹ�������XML�ļ�
        private User _user;                                          //���ӵ��û���Ϣ

        //cyf 20110602 add
        private List<Role> _LstRoleInfo;                             //���ӵ��û���Ӧ�Ľ�ɫ��Ϣ
        //end

        private List<string> _ListUserPrivilegeID;                   //�û�Ȩ�ޱ��

        //��ʽ��
        private ICustomWks _CurWks;
        //��ʱ��
        private ICustomWks _TempWks;

        //���캯��
        public AppFileDB()
        {
        }

        public AppFileDB(Form pForm, Control ControlContainer, XmlDocument SystemXml, XmlDocument DataTreeXml, XmlDocument DatabaseInfoXml, Plugin.Parse.PluginCollection ColParsePlugin, string ImageResPath)
        {
            //��ϮAppFormRef ����
            _MainForm = pForm;
            _ControlContainer = ControlContainer;
            _SystemXml = SystemXml;
            _DataTreeXml = DataTreeXml;
            _DatabaseInfoXml = DatabaseInfoXml;
            _ColParsePlugin = ColParsePlugin;
            _ImageResPath = ImageResPath;
        }


        #region IAppFileRef ��Ա

        public UserControl MainUserControl
        {
            get
            {
                return _MainUserControl;
            }
            set
            {
                _MainUserControl = value;
            }
        }


        public DevExpress.XtraTreeList.TreeList ProjectTree
        {
            get
            {
                return _ProjectTree;
            }
            set
            {
                _ProjectTree = value;
            }
        }

        public DevExpress.XtraGrid.Views.Grid.GridView DataInfoGrid
        {
            get
            {
                return _DataInfoGrid;
            }
            set
            {
                _DataInfoGrid = value;
            }
        }

        public DevExpress.XtraGrid.Views.Grid.GridView MetaDataGrid
        {
            get
            {
                return _MetaDataGrid;
            }
            set
            {
                _MetaDataGrid = value;
            }
        }

        public DevExpress.XtraGrid.Views.Grid.GridView SysSettingGrid
        {
            get
            {
                return _SysSettingGrid;
            }
            set
            {
                _SysSettingGrid = value;
            }
        }

        public DevExpress.XtraTreeList.TreeList SysSettingTree
        {
            get
            {
                return _SysSettingTree;
            }
            set
            {
                _SysSettingTree = value;
            }
        }

        public XmlDocument DBXmlDocument
        {
            get
            {
                return _DBXmlDocument;
            }
            set
            {
                _DBXmlDocument = value;
            }
        }


        public DevExpress.XtraEditors.Repository.RepositoryItemTextEdit TxtDisplayPage
        {
            get
            {
                return _txtDisplayPage;
            }
            set
            {
                _txtDisplayPage = value;
            }
        }

        public DevExpress.XtraBars.BarButtonItem BtnFirst
        {
            get
            {
                return _btnFirst;
            }
            set
            {
                _btnFirst = value;
            }
        }
        public DevExpress.XtraBars.BarButtonItem BtnLast
        {
            get
            {
                return _btnLast;
            }
            set
            {
                _btnLast = value;
            }
        }
        public DevExpress.XtraBars.BarButtonItem BtnPre
        {
            get
            {
                return _btnPre;
            }
            set
            {
                _btnPre = value;
            }
        }
        public DevExpress.XtraBars.BarButtonItem BtnNext
        {
            get
            {
                return _btnNext;
            }
            set
            {
                _btnNext = value;
            }
        }



        public string Name
        {
            get
            {
                return _MainUserControl.Name;
            }
            set
            {
                _MainUserControl.Name = value;
            }
        }

        public string Caption
        {
            get
            {
                return _MainUserControl.Tag as string;
            }
            set
            {
                _MainUserControl.Tag = value;
            }
        }

        public bool Visible
        {
            get
            {
                return _MainUserControl.Visible;
            }
            set
            {
                _MainUserControl.Visible = value;
            }
        }

        public bool Enabled
        {
            get
            {
                return _MainUserControl.Enabled;
            }
            set
            {
                _MainUserControl.Enabled = value;
            }
        }

        public Dictionary<string, System.Windows.Forms.ContextMenuStrip> DicContextMenu
        {
            get
            {
                return _DicContextMenu;
            }
            set
            {
                _DicContextMenu = value;
            }
        }

        public Control StatusBar
        {
            get
            {
                return _StatusStrip;
            }
            set
            {
                _StatusStrip = value;
            }
        }

        public Thread CurrentThread
        {
            get
            {
                return _CurrentThread;
            }
            set
            {
                _CurrentThread = value;
            }
        }
        #endregion

        #region ״̬������
        public string OperatorTips
        {
            get;set;
        }

        public DevExpress.XtraEditors.Repository.RepositoryItemProgressBar ProgressBar
        {
            get; set;
        }

        //�ο�������cmb
        public DevExpress.XtraEditors.Repository.RepositoryItemComboBox RefScaleCmb
        {
            get; set;
        }
        //��ǰ������cmb
        public DevExpress.XtraEditors.Repository.RepositoryItemComboBox CurScaleCmb
        {
            get; set;
        }
        //������ʾ�ı���
        public DevExpress.XtraEditors.Repository.RepositoryItemTextEdit CoorTxt
        {
            get; set;
        }


        public string CoorXY
        {
            get; set;
        }

        public string UserInfo
        {
            get; set;
        }

        #endregion

        #region IAppArcGISRef ��Ա
        public string CurrentTool
        {
            get
            {
                return _CurrentTool;
            }
            set
            {
                _CurrentTool = value;
            }
        }

        public IMapDocument MapDocument
        {
            get
            {
                return _MapDocument;
            }
            set
            {
                _MapDocument = value;
            }
        }

        public IMapControlDefault MapControl
        {
            get
            {
                return _MapControl;
            }
            set
            {
                _MapControl = value;
            }
        }
        public PictureBox p1 { get; set; }
        public PictureBox p2 { get; set; }
        public PictureBox p3 { get; set; }
        public PictureBox p4 { get; set; }
        public AxMapControl ArcGisMapControl
        {
            get
            {
                return _AxMapControl;
            }
            set
            {
                _AxMapControl = value;
            }
        }

        public ISceneControlDefault SceneControlDefault
        {
            get { return _SceneControlDefault; }
            set { _SceneControlDefault = value; }
        }

        public AxSceneControl SceneControl
        {
            get { return _SceneControl; }
            set { _SceneControl = value; }
        }

        public IPageLayoutControlDefault PageLayoutControl
        {
            get
            {
                return _PageLayoutControl;
            }
            set
            {
                _PageLayoutControl = value;
            }
        }

        public ITOCControlDefault TOCControl
        {
            get
            {
                return _TOCControl;
            }
            set
            {
                _TOCControl = value;
            }
        }

        //cyf 20110518  
        public object CurrentControl
        {
            get
            {
                return _CurrentControl;
            }
            set
            {
                _CurrentControl = value;
            }
        }
        public List<DevExpress.XtraEditors.Repository.RepositoryItemComboBox> ScaleBoxList
        {
            get
            {
                return _scaleBoxList;
            }
            set
            {
                _scaleBoxList = value;
            }
        }

        public Dictionary<DevExpress.XtraBars.Ribbon.RibbonPage, string> DicTabs
        {
            get
            {
                return _dicTabs;
            }
            set
            {
                _dicTabs = value;
            }
        }
        //end

        #endregion

        #region IAppFormDef ��Ա

        Form IAppFormRef.MainForm
        {
            get
            {
                return _MainForm;
            }
            set
            {
                _MainForm = value;
            }
        }

        public Control ControlContainer
        {
            get
            {
                return _ControlContainer;
            }
            set
            {
                _ControlContainer = value;
            }
        }

        string IAppFormRef.CurrentSysName
        {
            get
            {
                return null;
            }
            set
            {
            }
        }

        public XmlDocument SystemXml
        {
            get
            {
                return _SystemXml;
            }
            set
            {
                _SystemXml = value;
            }
        }

        public XmlDocument DataTreeXml
        {
            get
            {
                return _DataTreeXml;
            }
            set
            {
                _DataTreeXml = value;
            }
        }

        public XmlDocument DatabaseInfoXml
        {
            get
            {
                return _DatabaseInfoXml;
            }
            set
            {
                _DatabaseInfoXml = value;
            }
        }

        public Plugin.Parse.PluginCollection ColParsePlugin
        {
            get
            {
                return _ColParsePlugin;
            }
            set
            {
                _ColParsePlugin = value;
            }
        }

        public string ImageResPath
        {
            get
            {
                return _ImageResPath;
            }
            set
            {
                _ImageResPath = value;
            }
        }

        public User ConnUser
        {
            get { return _user; }
            set { _user = value; }
        }

        //cyf 20110602 add:�����û���Ӧ�Ľ�ɫ������
        public List<Role> LstRoleInfo
        {
            get { return _LstRoleInfo; }
            set { _LstRoleInfo = value; }
        }
        //end

        public object LayerTree
        {
            get
            {
                return _LayerTree;
            }
            set
            {
                _LayerTree = value;
            }
        }
        //added by chulili 2011-11-29 ͼ��Ŀ¼��AdvTree
        public object LayerAdvTree
        {
            get
            {
                return _LayerAdvTree; ;
            }
            set
            {
                _LayerAdvTree = value;
            }
        }
        //added by chulili 2011-12-15 ͼ��Ŀ¼�����ļ�·��
        public string LayerTreeXmlPath
        {
            get
            {
                return _LayerTreeXmlPath; ;
            }
            set
            {
                _LayerTreeXmlPath = value;
            }
        }
        //end added by chulili
        //wgf 2011-5-18
        //--------------------------------------------
        public System.Windows.Forms.TreeView DataTabIndexTree
        {
            get
            {
                return _DataTabIndexTree;
            }
            set
            {
                _DataTabIndexTree = value;
            }
        }
        public System.Windows.Forms.DataGridView GridCtrl
        {
            get
            {
                return _GridCtrl;
            }
            set
            {
                _GridCtrl = value;
            }
        }

        public System.Windows.Forms.RichTextBox tipRichBox
        {
            get
            {
                return _tipRichBox;
            }
            set
            {
                _tipRichBox = value;
            }
        }

        public string strLogFilePath
        {
            get
            {
                return _strLogFilePath;
            }
            set
            {
                _strLogFilePath = value;
            }
        }

        public List<string> ListUserPrivilegeID
        {
            get
            {
                return _ListUserPrivilegeID;
            }
            set
            {
                _ListUserPrivilegeID = value;
            }
        }

        //����������Ϣ
        public ICustomWks CurWksInfo
        {
            get
            {
                return _CurWks;
            }
            set
            {
                _CurWks = value;
            }
        }
        public ICustomWks TempWksInfo
        {
            get
            {
                return _TempWks;
            }
            set
            {
                _TempWks = value;
            }
        }
        //---------------------------------------------

        #endregion
    }
    /// <summary>
    /// ���ݿ⼯�ɹ�����ϵͳ�ӿ���ʵ�� ���Ƿ����  20100927
    /// </summary>
    public class AppDBIntegra : IAppDBIntegraRef, IAppFormRef
    {
        private object _LayerAdvTree;  //Ŀ¼��ͼ �������advTree
        private object _LayerTree;  //Ŀ¼��ͼ added by chulili 20111119
        private string _LayerTreeXmlPath;                               //added by chulili 20111215
        private Form _MainForm;                                        // ����
        private UserControl _MainUserControl;                         //�ؼ�UserControl
        private Control _StatusStrip;                                 // ״̬��
        private Control _ControlContainer;                             // �ؼ�����
        private XmlDocument _SystemXml;                                // ϵͳ������ͼXml�ڵ�
        private XmlDocument _DataTreeXml;                              // ������ͼXML�ڵ�
        private XmlDocument _DatabaseInfoXml;                          // ������ϢXML�ڵ�
        private Plugin.Parse.PluginCollection _ColParsePlugin;         // ϵͳ�������
        private Dictionary<string, System.Windows.Forms.ContextMenuStrip> _DicContextMenu;           //�Ҽ��˵�����
        private string _ImageResPath;                                  // ͼƬ��Դ·��
        private IMapDocument _MapDocument;                            //�洢���ĵ�����
        private IMapControlDefault _MapControl;                      //MapControl�ؼ�
        private AxMapControl _AxMapControl;
        private ISceneControlDefault _SceneControlDefault;
        private AxSceneControl _SceneControl;
        private IPageLayoutControlDefault _PageLayoutControl;        //PageLayoutControl�ؼ�
        private ITOCControlDefault _TOCControl;                     //TOCControl�ؼ�
        private string _CurrentTool;                                   // ��ǰʹ�õ�TOOL����
        private DevExpress.XtraTreeList.TreeList _ProjectTree;       // ������Ϣ��ͼ
        //private DevExpress.XtraGrid.Views.Grid.GridView _UpdateDataGrid;   //���¶Աȱ��

        //private DevExpress.XtraGrid.Views.Grid.GridView _DataCheckGrid;   //���ݼ����
        private XmlDocument _DBXmlDocument;                       //���ݹ�������XML�ļ�

        private Thread _CurrentThread;                           //�������ݵĽ���(Ψһ)

        private User _user;                                          //���ӵ��û���Ϣ
        //cyf 20110602 add
        private List<Role> _LstRoleInfo;                             //���ӵ��û���Ӧ�Ľ�ɫ��Ϣ
        //end

        //20110518
        private object _CurrentControl;                             //��ǰͼ����ʾ�ؼ� 
        private List<DevExpress.XtraEditors.Repository.RepositoryItemComboBox> _scaleBoxList;                        //��������ʾ        
        private Dictionary<DevExpress.XtraBars.Ribbon.RibbonPage, string> _dicTabs;          //��tab����ϵͳ����
        //end

        private List<string> _ListUserPrivilegeID;                   //�û�Ȩ�ޱ��

        //��ʽ��
        private ICustomWks _CurWks;
        //��ʱ��
        private ICustomWks _TempWks;

        //���캯��
        public AppDBIntegra()
        {
        }

        public AppDBIntegra(Form pForm, Control ControlContainer, XmlDocument SystemXml, XmlDocument DataTreeXml, XmlDocument DatabaseInfoXml, Plugin.Parse.PluginCollection ColParsePlugin, string ImageResPath, User v_user, List<Role> v_ListRole)
        {
            //��ϮAppFormRef ����
            _MainForm = pForm;
            _ControlContainer = ControlContainer;
            _SystemXml = SystemXml;
            _DataTreeXml = DataTreeXml;
            _DatabaseInfoXml = DatabaseInfoXml;
            _ColParsePlugin = ColParsePlugin;
            _ImageResPath = ImageResPath;
            _user = v_user;
            _LstRoleInfo = v_ListRole;
        }

        #region IAppDBIntegraRef ��Ա
        public UserControl MainUserControl
        {
            get
            {
                return _MainUserControl;
            }
            set
            {
                _MainUserControl = value;
            }
        }
        public object LayerTree
        {
            get
            {
                return _LayerTree; ;
            }
            set
            {
                _LayerTree = value;
            }
        }
        public object LayerAdvTree
        {
            get
            {
                return _LayerAdvTree;
            }
            set
            {
                _LayerAdvTree = value;
            }
        }
        //added by chulili 2011-12-15 ͼ��Ŀ¼�����ļ�·��
        public string LayerTreeXmlPath
        {
            get
            {
                return _LayerTreeXmlPath; ;
            }
            set
            {
                _LayerTreeXmlPath = value;
            }
        }
        public string Name
        {
            get
            {
                return _MainUserControl.Name;
            }
            set
            {
                _MainUserControl.Name = value;
            }
        }

        public string Caption
        {
            get
            {
                return _MainUserControl.Tag as string;
            }
            set
            {
                _MainUserControl.Tag = value;
            }
        }

        public bool Visible
        {
            get
            {
                return _MainUserControl.Visible;
            }
            set
            {
                _MainUserControl.Visible = value;
            }
        }

        public bool Enabled
        {
            get
            {
                return _MainUserControl.Enabled;
            }
            set
            {
                _MainUserControl.Enabled = value;
            }
        }

        public Dictionary<string, System.Windows.Forms.ContextMenuStrip> DicContextMenu
        {
            get
            {
                return _DicContextMenu;
            }
            set
            {
                _DicContextMenu = value;
            }
        }

        public DevExpress.XtraTreeList.TreeList ProjectTree
        {
            get
            {
                return _ProjectTree;
            }
            set
            {
                _ProjectTree = value;
            }
        }

        public XmlDocument DBXmlDocument
        {
            get
            {
                return _DBXmlDocument;
            }
            set
            {
                _DBXmlDocument = value;
            }
        }

        public Control StatusBar
        {
            get
            {
                return _StatusStrip;
            }
            set
            {
                _StatusStrip = value;
            }
        }

        public Thread CurrentThread
        {
            get
            {
                return _CurrentThread;
            }
            set
            {
                _CurrentThread = value;
            }
        }
        #endregion

        #region ״̬������
        public string OperatorTips
        {
            get;set;
        }

        public DevExpress.XtraEditors.Repository.RepositoryItemProgressBar ProgressBar
        {
            get; set;
        }
        //�ο��������Ƿ�ɼ�
        public bool RefScaleVisible
        {
            get; set;
        }
        //��ǰ�������Ƿ�ɼ�
        public bool CurScaleVisible
        {
            get; set;
        }
        //�ο�������cmb
        public DevExpress.XtraEditors.Repository.RepositoryItemComboBox RefScaleCmb
        {
            get; set;
        }
        //��ǰ������cmb
        public DevExpress.XtraEditors.Repository.RepositoryItemComboBox CurScaleCmb
        {
            get; set;
        }
        //������ʾ�ı���
        public DevExpress.XtraEditors.Repository.RepositoryItemTextEdit CoorTxt
        {
            get; set;
        }

        public string UserInfo
        {
            get; set;
        }

        //public string CoorXY
        //{
        //    get
        //    {
        //        return string.Empty;
        //    }
        //    set
        //    {
        //    }
        //}
        //changed by chulili 20110722
        public string CoorXY
        {
            get; set;
        }
        #endregion

        #region IAppArcGISRef ��Ա
        public string CurrentTool
        {
            get
            {
                return _CurrentTool;
            }
            set
            {
                _CurrentTool = value;
            }
        }

        public IMapDocument MapDocument
        {
            get
            {
                return _MapDocument;
            }
            set
            {
                _MapDocument = value;
            }
        }

        public IMapControlDefault MapControl
        {
            get
            {
                return _MapControl;
            }
            set
            {
                _MapControl = value;
            }
        }

        public PictureBox p1 { get; set; }
        public PictureBox p2 { get; set; }
        public PictureBox p3 { get; set; }
        public PictureBox p4 { get; set; }
        public AxMapControl ArcGisMapControl
        {
            get
            {
                return _AxMapControl;
            }
            set
            {
                _AxMapControl = value;
            }
        }

        public ISceneControlDefault SceneControlDefault
        {
            get { return _SceneControlDefault; }
            set { _SceneControlDefault = value; }
        }

        public AxSceneControl SceneControl
        {
            get { return _SceneControl; }
            set { _SceneControl = value; }
        }

        public IPageLayoutControlDefault PageLayoutControl
        {
            get
            {
                return _PageLayoutControl;
            }
            set
            {
                _PageLayoutControl = value;
            }
        }

        public ITOCControlDefault TOCControl
        {
            get
            {
                return _TOCControl;
            }
            set
            {
                _TOCControl = value;
            }
        }

        //cyf 20110518  
        public object CurrentControl
        {
            get
            {
                return _CurrentControl;
            }
            set
            {
                _CurrentControl = value;
            }
        }
        public List<DevExpress.XtraEditors.Repository.RepositoryItemComboBox> ScaleBoxList
        {
            get
            {
                return _scaleBoxList;
            }
            set
            {
                _scaleBoxList = value;
            }
        }

        public Dictionary<DevExpress.XtraBars.Ribbon.RibbonPage, string> DicTabs
        {
            get
            {
                return _dicTabs;
            }
            set
            {
                _dicTabs = value;
            }
        }
        //end

        #endregion

        #region IAppFormDef ��Ա

        Form IAppFormRef.MainForm
        {
            get
            {
                return _MainForm;
            }
            set
            {
                _MainForm = value;
            }
        }

        public Control ControlContainer
        {
            get
            {
                return _ControlContainer;
            }
            set
            {
                _ControlContainer = value;
            }
        }

        string IAppFormRef.CurrentSysName
        {
            get
            {
                return null;
            }
            set
            {
            }
        }

        public XmlDocument SystemXml
        {
            get
            {
                return _SystemXml;
            }
            set
            {
                _SystemXml = value;
            }
        }

        public XmlDocument DataTreeXml
        {
            get
            {
                return _DataTreeXml;
            }
            set
            {
                _DataTreeXml = value;
            }
        }

        public XmlDocument DatabaseInfoXml
        {
            get
            {
                return _DatabaseInfoXml;
            }
            set
            {
                _DatabaseInfoXml = value;
            }
        }

        public Plugin.Parse.PluginCollection ColParsePlugin
        {
            get
            {
                return _ColParsePlugin;
            }
            set
            {
                _ColParsePlugin = value;
            }
        }

        public string ImageResPath
        {
            get
            {
                return _ImageResPath;
            }
            set
            {
                _ImageResPath = value;
            }
        }

        public User ConnUser
        {
            get { return _user; }
            set { _user = value; }
        }

        //cyf 20110602 add:�����û���Ӧ�Ľ�ɫ������
        public List<Role> LstRoleInfo
        {
            get { return _LstRoleInfo; }
            set { _LstRoleInfo = value; }
        }
        //end


        public List<string> ListUserPrivilegeID
        {
            get
            {
                return _ListUserPrivilegeID;
            }
            set
            {
                _ListUserPrivilegeID = value;
            }
        }

        //����������Ϣ
        public ICustomWks CurWksInfo
        {
            get
            {
                return _CurWks;
            }
            set
            {
                _CurWks = value;
            }
        }
        public ICustomWks TempWksInfo
        {
            get
            {
                return _TempWks;
            }
            set
            {
                _TempWks = value;
            }
        }
        #endregion
    }
    #endregion
}
