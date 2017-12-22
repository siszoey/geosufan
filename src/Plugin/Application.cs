using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Windows.Forms;
using System.Data;
using System.Xml;
using System.Threading;
using System.Drawing.Printing;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using Fan.Common.Gis;
using ESRI.ArcGIS.Geometry;
using Fan.Common;
namespace Fan.Plugin.Application
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
        List<Role> LstRoleInfo { get; set; }
        /// <summary>
        /// �û�Ȩ�ޱ��
        /// </summary>
        List<string> ListUserPrivilegeID { get; set; }
        ICustomWks CurWksInfo { get; set; }
        ICustomWks TempWksInfo { get; set; }
        IMapControlDefault MapControl { get; set; }
        ITOCControlDefault TOCControl { get; set; }
        object LayerTree { get; set; }
        object LayerAdvTree { get; set; }
        string LayerTreeXmlPath { get; set; }
    }
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
    #endregion
    #region �ӿ�ʵ����
    public class AppForm : IAppFormRef
    {
        private IMapControlDefault _MapControl;                   
        private ITOCControlDefault _TOCControl;                    
        private object _LayerAdvTree;
        private object _LayerTree;                                    
        private string _LayerTreeXmlPath;                               
        private Form _MainForm;                                      
        private Control _StatusStrip;                                 
        private Control _ControlContainer;                            
        private string _CurrentTab;                                  
        private XmlDocument _SystemXml;                                
        private XmlDocument _DataTreeXml;                            
        private XmlDocument _DatabaseInfoXml;                          
        private Plugin.Parse.PluginCollection _ColParsePlugin;      
        private Dictionary<string, System.Windows.Forms.ContextMenuStrip> _DicContextMenu;           
        private string _ImageResPath;                                
        private User _user;                                          
        private List<Role> _LstRoleInfo;                            
        private List<string> _ListUserPrivilegeID;                 
        private ICustomWks _CurWks;
        private ICustomWks _TempWks;
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
        public List<Role> LstRoleInfo
        {
            get { return _LstRoleInfo; }
            set { _LstRoleInfo = value; }
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
    #endregion
}
