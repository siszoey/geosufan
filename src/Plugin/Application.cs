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
        DevComponents.DotNetBar.ProgressBarItem ProgressBar { get; set; }

        //�ο�������cmb
        DevComponents.DotNetBar.ComboBoxItem RefScaleCmb { get; set; }

        //��ǰ������cmb
        DevComponents.DotNetBar.ComboBoxItem CurScaleCmb { get; set; }

        //������ʾ�ı���
        DevComponents.DotNetBar.TextBoxItem CoorTxt { get; set; }
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
        Dictionary<string, DevComponents.DotNetBar.ContextMenuBar> DicContextMenu { get; set; }
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
        /// ��������ʾ
        /// </summary>
        List<DevComponents.DotNetBar.ComboBoxItem> ScaleBoxList { get; set; }

        /// <summary>
        /// ��tab����ϵͳ����
        /// </summary>
        Dictionary<DevComponents.DotNetBar.RibbonTabItem, string> DicTabs { get; set; }
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
        DevComponents.AdvTree.AdvTree DataTree { get; set; }

        /// <summary>
        /// ������Ϣ��ͼ
        /// </summary>
        DevComponents.AdvTree.AdvTree ProjectTree { get; set; }

        /// <summary>
        /// ���ݹ�������XML�ļ�
        /// </summary>
        XmlDocument DBXmlDocument { get; set; }

        /// <summary>
        /// ���¶Ա��б�
        /// </summary>
        DevComponents.DotNetBar.Controls.DataGridViewX UpdateGrid { get; set; }

        /// <summary>
        /// ���¶Ա��б��ҳ��Ϣ��ʾ�ı���
        /// </summary>
        DevComponents.DotNetBar.TextBoxItem TxtDisplayPage { get; set; }
        /// <summary>
        /// ���¶Ա��б�ť
        /// </summary>
        DevComponents.DotNetBar.ButtonItem BtnFirst { get; set; }
        DevComponents.DotNetBar.ButtonItem BtnLast { get; set; }
        DevComponents.DotNetBar.ButtonItem BtnPre { get; set; }
        DevComponents.DotNetBar.ButtonItem BtnNext { get; set; }

        /// <summary>
        /// ����������б�
        /// </summary>
        DevComponents.DotNetBar.Controls.DataGridViewX PolylineSearchGrid { get; set; }

        /// <summary>
        /// ������������
        /// </summary>
        DevComponents.DotNetBar.Controls.DataGridViewX PolygonSearchGrid { get; set; }

        /// <summary>
        /// �ӱ��ںϼ�¼���
        /// </summary>
        DevComponents.DotNetBar.Controls.DataGridViewX JoinMergeResultGrid { get; set; }

        /// <summary>
        /// ���ݼ���б�
        /// </summary>
        DevComponents.DotNetBar.Controls.DataGridViewX DataCheckGrid { get; set; }

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
        /// ���ݴ�����ͼ
        /// </summary>
        DevComponents.AdvTree.AdvTree DataTree { get; set; }

        /// <summary>
        /// ���¶Ա��б��ҳ��Ϣ��ʾ�ı���
        /// </summary>
        DevComponents.DotNetBar.TextBoxItem TxtDisplayPage { get; set; }
        /// <summary>
        /// ���¶Ա��б�ť
        /// </summary>
        DevComponents.DotNetBar.ButtonItem BtnFirst { get; set; }
        DevComponents.DotNetBar.ButtonItem BtnLast { get; set; }
        DevComponents.DotNetBar.ButtonItem BtnPre { get; set; }
        DevComponents.DotNetBar.ButtonItem BtnNext { get; set; }

        /// <summary>
        /// ������ͼ
        /// </summary>
        DevComponents.AdvTree.AdvTree ErrTree { get; set; }

        /// <summary>
        /// ������Ϣ��ͼ
        /// </summary>
        DevComponents.AdvTree.AdvTree ProjectTree { get; set; }

        /// <summary>
        /// ���¶Աȷ����б�
        /// </summary>
        DevComponents.DotNetBar.Controls.DataGridViewX UpdateDataGrid { get; set; }

        /// <summary>
        /// ���ݼ���б�
        /// </summary>
        DevComponents.DotNetBar.Controls.DataGridViewX DataCheckGrid { get; set; }

        /// <summary>
        /// ���ݼ���б�
        /// </summary>
        //DevComponents.DotNetBar.Controls.DataGridViewX DataCheckGrid { get;set;}

        /// <summary>
        /// ���ݹ�������XML�ļ�
        /// </summary>
        XmlDocument DBXmlDocument { get; set; }

        /// <summary>
        /// �������ݵĽ���(Ψһ)
        /// </summary>
        Thread CurrentThread { get; set; }
    }

     ///<summary>
     ///����������ϵͳ�������Խӿ�
     ///</summary>
    //public interface IAppDBProjectRef : IAppArcGISRef
    //{
    //    /// <summary>
    //    /// �û��ؼ�
    //    /// </summary>
    //    UserControl MainUserControl { get; set; }

    //    /// <summary>
    //    ///  ���ݹ���XML
    //    /// </summary>
    //    XmlDocument DBMainXml { get; set; }

    //    /// <summary>
    //    /// ҵ����ͼ
    //    /// </summary>
    //    DevComponents.AdvTree.AdvTree CaseDataTree { get; set; }

    //    /// <summary>
    //    /// ���ݿ�������ͼ
    //    /// </summary>
    //    DevComponents.AdvTree.AdvTree DBDataTree { get; set; }
    //}

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
        DevComponents.AdvTree.AdvTree MainTree { get; set; }

        /// <summary>
        /// �û�����ͼ
        /// </summary>
        DevComponents.AdvTree.AdvTree RoleTree { get; set; }

        /// <summary>
        /// �û���ͼ
        /// </summary>
        DevComponents.AdvTree.AdvTree UserTree { get; set; }

        /// <summary>
        /// Ȩ����ͼ
        /// </summary>
        DevComponents.AdvTree.AdvTree PrivilegeTree { get; set; }
        /// <summary>
        /// ��ǰ��GroupPanel
        /// </summary>
        /// 
        DevComponents.DotNetBar.Controls.GroupPanel CurrentPanel { get; set; }

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

        DevComponents.DotNetBar.TabControl IndextabControl { get; set; }

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
        DevComponents.AdvTree.AdvTree DataTree { get; set; }

        /// <summary>
        /// ������ͼ
        /// </summary>
        DevComponents.AdvTree.AdvTree ErrTree { get; set; }

        /// <summary>
        /// ������Ϣ��ͼ
        /// </summary>
        DevComponents.AdvTree.AdvTree ProjectTree { get; set; }

        /// <summary>
        /// ���¶Աȷ����б�
        /// </summary>
        DevComponents.DotNetBar.Controls.DataGridViewX UpdateDataGrid { get; set; }

        /// <summary>
        /// ���ݼ���б�
        /// </summary>
        //DevComponents.DotNetBar.Controls.DataGridViewX DataCheckGrid { get;set;}

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
        DevComponents.AdvTree.AdvTree ProjectTree { get; set; }
        /// <summary>
        /// ������Ϣ�б�
        /// </summary>
        DevComponents.DotNetBar.Controls.DataGridViewX DataInfoGrid { get; set; }
        /// <summary>
        /// Ԫ�����б�
        /// </summary>
        DevComponents.DotNetBar.Controls.DataGridViewX MetaDataGrid { get; set; }
        /// <summary>
        /// ϵͳ�����б�
        /// </summary>
        DevComponents.DotNetBar.Controls.DataGridViewX SysSettingGrid { get; set; }
        /// <summary>
        /// ϵͳ������ͼ
        /// </summary>
        DevComponents.AdvTree.AdvTree SysSettingTree { get; set; }
        /// <summary>
        /// ���ݹ�������XML�ļ�
        /// </summary>
        XmlDocument DBXmlDocument { get; set; }
        /// <summary>
        /// ������Ϣ�б��ҳ��Ϣ��ʾ�ı���
        /// </summary>
        DevComponents.DotNetBar.TextBoxItem TxtDisplayPage { get; set; }
        /// <summary>
        /// ������Ϣ�б�ť
        /// </summary>
        DevComponents.DotNetBar.ButtonItem BtnFirst { get; set; }
        DevComponents.DotNetBar.ButtonItem BtnLast { get; set; }
        DevComponents.DotNetBar.ButtonItem BtnPre { get; set; }
        DevComponents.DotNetBar.ButtonItem BtnNext { get; set; }
        /// <summary>
        /// �������ݵĽ���(Ψһ)
        /// </summary>
        Thread CurrentThread { get; set; }
    }
    /// <summary>
    /// Oracle Spatial ���⹤�߹������Խӿ�
    /// </summary>
    //public interface IAppOracleSpatialRef : IAppFormRef
    //{
    //    /// <summary>
    //    /// �û��ؼ�
    //    /// </summary>
    //    UserControl MainUserControl { get; set; }

    //    /// <summary>
    //    /// ������Ϣ��ͼ
    //    /// </summary>
    //    DevComponents.AdvTree.AdvTree OraProjectTree { get; set; }

    //    /// <summary>
    //    /// ͼ����Ϣ��ͼ
    //    /// </summary>
    //    DevComponents.AdvTree.AdvTree LayerTree { get; set; }

    //    /// <summary>
    //    /// ͼƬ��ʾ��
    //    /// </summary>
    //    System.Windows.Forms.PictureBox PictureBox { get; set; }

    //    /// <summary>
    //    /// ���ݹ�������XML�ļ�
    //    /// </summary>
    //    XmlDocument DBXmlDocument { get; set; }
    //    /// <summary>
    //    /// �������ݵĽ���(Ψһ)
    //    /// </summary>
    //    Thread CurrentThread { get; set; }
    //}

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
        DevComponents.AdvTree.AdvTree ProjectTree { get; set; }
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

    /// <summary>
    /// �߳����ݿ������ϵͳ����ӿڶ���  ���Ƿ����  20100929
    /// </summary>
    //public interface IAppDBContourRef : IAppArcGISRef
    //{
    //    /// <summary>
    //    /// �û��ؼ�
    //    /// </summary>
    //    UserControl MainUserControl { get; set; }


    //    /// <summary>
    //    /// ������Ϣ��ͼ
    //    /// </summary>
    //    DevComponents.AdvTree.AdvTree ProjectTree { get; set; }

    //    /// <summary>
    //    /// ���ݹ�������XML�ļ�
    //    /// </summary>
    //    XmlDocument DBXmlDocument { get; set; }

    //    /// <summary>
    //    /// �������ݵĽ���(Ψһ)
    //    /// </summary>
    //    Thread CurrentThread { get; set; }
    //}


    /// <summary>
    /// Ӱ�����ݿ������ϵͳ����ӿڶ���  ���Ƿ����  20100929
    /// </summary>
    //public interface IAppDBImageRef : IAppArcGISRef
    //{
    //    /// <summary>
    //    /// �û��ؼ�
    //    /// </summary>
    //    UserControl MainUserControl { get; set; }


    //    /// <summary>
    //    /// ������Ϣ��ͼ
    //    /// </summary>
    //    DevComponents.AdvTree.AdvTree ProjectTree { get; set; }

    //    /// <summary>
    //    /// ���ݹ�������XML�ļ�
    //    /// </summary>
    //    XmlDocument DBXmlDocument { get; set; }

    //    /// <summary>
    //    /// �������ݵĽ���(Ψһ)
    //    /// </summary>
    //    Thread CurrentThread { get; set; }
    //}

    /// <summary>
    /// �������ݿ������ϵͳ����ӿڶ���  ���Ƿ����  20100930
    /// </summary>
    //public interface IAppDBAddressRef : IAppArcGISRef
    //{
    //    /// <summary>
    //    /// �û��ؼ�
    //    /// </summary>
    //    UserControl MainUserControl { get; set; }


    //    /// <summary>
    //    /// ������Ϣ��ͼ
    //    /// </summary>
    //    DevComponents.AdvTree.AdvTree ProjectTree { get; set; }

    //    /// <summary>
    //    /// ���ݹ�������XML�ļ�
    //    /// </summary>
    //    XmlDocument DBXmlDocument { get; set; }

    //    /// <summary>
    //    /// �������ݵĽ���(Ψһ)
    //    /// </summary>
    //    Thread CurrentThread { get; set; }
    //}

    /// <summary>
    /// ����������ݿ������ϵͳ����ӿڶ���  ���Ƿ����  20100930
    /// </summary>
    //public interface IAppDBEntityRef : IAppArcGISRef
    //{
    //    /// <summary>
    //    /// �û��ؼ�
    //    /// </summary>
    //    UserControl MainUserControl { get; set; }


    //    /// <summary>
    //    /// ������Ϣ��ͼ
    //    /// </summary>
    //    DevComponents.AdvTree.AdvTree ProjectTree { get; set; }

    //    /// <summary>
    //    /// ���ݹ�������XML�ļ�
    //    /// </summary>
    //    XmlDocument DBXmlDocument { get; set; }

    //    /// <summary>
    //    /// �������ݵĽ���(Ψһ)
    //    /// </summary>
    //    Thread CurrentThread { get; set; }
    //}


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
        private Dictionary<string, DevComponents.DotNetBar.ContextMenuBar> _DicContextMenu;           //�Ҽ��˵�����
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

        public Dictionary<string, DevComponents.DotNetBar.ContextMenuBar> DicContextMenu
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

        private DevComponents.DotNetBar.ProgressBarItem _ProgressBar;
        public DevComponents.DotNetBar.ProgressBarItem ProgressBar
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
        private DevComponents.DotNetBar.ComboBoxItem _RefScaleCmb;
        public DevComponents.DotNetBar.ComboBoxItem RefScaleCmb
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
        private DevComponents.DotNetBar.ComboBoxItem _CurScaleCmb;
        public DevComponents.DotNetBar.ComboBoxItem CurScaleCmb
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
        private DevComponents.DotNetBar.TextBoxItem _CoorTxt;
        public DevComponents.DotNetBar.TextBoxItem CoorTxt
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
        private Dictionary<string, DevComponents.DotNetBar.ContextMenuBar> _DicContextMenu;           //�Ҽ��˵�����
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
        private List<DevComponents.DotNetBar.ComboBoxItem> _scaleBoxList;                        //��������ʾ        
        private Dictionary<DevComponents.DotNetBar.RibbonTabItem, string> _dicTabs;          //��tab����ϵͳ����
        //end

        private DevComponents.AdvTree.AdvTree _ProjectTree;       // ������Ϣ��ͼ
        private DevComponents.AdvTree.AdvTree _DataTree;          // ���ݴ�����ͼ
        private DevComponents.DotNetBar.Controls.DataGridViewX _UpdateGrid;  //���¶Ա��б�
        private DevComponents.DotNetBar.TextBoxItem _txtDisplayPage;//���¶Ա��б��ҳ��ʾ�ı���
        private DevComponents.DotNetBar.ButtonItem _btnFirst;//���¶Ա��б��ҳ��ʾ��ť
        private DevComponents.DotNetBar.ButtonItem _btnLast;//���¶Ա��б��ҳ��ʾ��ť
        private DevComponents.DotNetBar.ButtonItem _btnPre;//���¶Ա��б��ҳ��ʾ��ť
        private DevComponents.DotNetBar.ButtonItem _btnNext;//���¶Ա��б��ҳ��ʾ��ť
        private DevComponents.DotNetBar.Controls.DataGridViewX _PolylineSearchGrid;  //�ӱ����ͼ�¼��
        private DevComponents.DotNetBar.Controls.DataGridViewX _PolygonSearchGrid;  //�ӱ߶���μ�¼��
        private DevComponents.DotNetBar.Controls.DataGridViewX _JoinMergeResultGrid;  //�ӱ��ںϽ����¼��

        private DevComponents.DotNetBar.Controls.DataGridViewX _DataCheckGrid;   //���ݼ����

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


            #region �Զ���״̬��
            DevComponents.DotNetBar.Bar statusBar = new DevComponents.DotNetBar.Bar();
            statusBar.Name = "GisStatusBar";
            statusBar.TabStop = false;
            statusBar.Stretch = true;
            statusBar.Style = DevComponents.DotNetBar.eDotNetBarStyle.Office2007;
            statusBar.AccessibleRole = System.Windows.Forms.AccessibleRole.StatusBar;
            statusBar.AntiAlias = true;
            statusBar.BarType = DevComponents.DotNetBar.eBarType.StatusBar;
            statusBar.Dock = System.Windows.Forms.DockStyle.Bottom;
            statusBar.GrabHandleStyle = DevComponents.DotNetBar.eGrabHandleStyle.ResizeHandle;
            statusBar.Dock = System.Windows.Forms.DockStyle.Bottom;

            //������ʾ����
            DevComponents.DotNetBar.LabelItem aLabelItem = new DevComponents.DotNetBar.LabelItem();
            aLabelItem.Name = "GisLabel";
            aLabelItem.Stretch = true;
            aLabelItem.PaddingLeft = 2;
            aLabelItem.PaddingRight = 5;
            aLabelItem.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));

            //������
            DevComponents.DotNetBar.ProgressBarItem progressBarItem = new DevComponents.DotNetBar.ProgressBarItem();
            progressBarItem.ChunkGradientAngle = 0F;
            progressBarItem.MenuVisibility = DevComponents.DotNetBar.eMenuVisibility.VisibleAlways;
            progressBarItem.Name = "GisprogressBarItem";
            progressBarItem.RecentlyUsed = false;
            progressBarItem.Stretch = true;
            progressBarItem.Visible = false;
            progressBarItem.ColorTable = DevComponents.DotNetBar.eProgressBarItemColor.Paused;

            //�ο�������������
            DevComponents.DotNetBar.LabelItem RefLabelItem = new DevComponents.DotNetBar.LabelItem();
            RefLabelItem.Visible = false;   //changed by chulili 20110729 ���ο����������أ�Ŀǰû�õ���
            RefLabelItem.Text = "�ο�������:";//"ReferenceScale:";
            RefLabelItem.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));

            //�ο�������������
            DevComponents.DotNetBar.ComboBoxItem RefScaleCmb = new DevComponents.DotNetBar.ComboBoxItem();
            RefScaleCmb.Name = "RefScaleCmbItem";
            RefScaleCmb.Visible = false;//changed by chulili 20110729 ���ο����������أ�Ŀǰû�õ���
            RefScaleCmb.Enabled = true;
            RefScaleCmb.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDown;
            //cyf 20110615 add:��ӹ��������
            object[] objScale = null;
            int intWidth;
            ModScale.GetScaleConfig(out objScale,out intWidth );
            RefScaleCmb.Items.AddRange(objScale);//changed by chulili 20110731 ȥ��0,500
            //end

            //��ǰ������������
            DevComponents.DotNetBar.LabelItem CurLabelItem = new DevComponents.DotNetBar.LabelItem();
            CurLabelItem.Visible = true;
            CurLabelItem.Text = "��ǰ������:";// "CurrentScale:";
            CurLabelItem.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));


            //��ǰ������������
            DevComponents.DotNetBar.ComboBoxItem CurScaleCmb = new DevComponents.DotNetBar.ComboBoxItem();
            CurScaleCmb.Name = "CurScaleCmbItem";
            CurScaleCmb.Visible = true;
            CurScaleCmb.Enabled = true;
            CurScaleCmb.ComboWidth = intWidth;
            CurScaleCmb.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDown;
            //cyf 20110615 add:��ӹ��������
            CurScaleCmb.Items.AddRange(objScale);


            //�����ı���
            DevComponents.DotNetBar.TextBoxItem CoorTxt = new DevComponents.DotNetBar.TextBoxItem();
            CoorTxt.Name = "CoorTxtItem";
            CoorTxt.Visible = true;
            CoorTxt.Enabled = true;

            //ͼ�ϵ���������
            DevComponents.DotNetBar.LabelItem aCoorLabelItem = new DevComponents.DotNetBar.LabelItem();
            aCoorLabelItem.Name = "GisCoorLabel";
            aCoorLabelItem.Stretch = true;
            aCoorLabelItem.PaddingLeft = 2;
            aCoorLabelItem.PaddingRight = 2;
            aCoorLabelItem.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            //�û���Ϣ����
            DevComponents.DotNetBar.LabelItem aUserInfoLabelItem = new DevComponents.DotNetBar.LabelItem();
            aUserInfoLabelItem.Name = "GisUserInfoLabel";
            aUserInfoLabelItem.Stretch = true;
            aUserInfoLabelItem.PaddingLeft = 2;
            aUserInfoLabelItem.PaddingRight = 2;
            aUserInfoLabelItem.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));

            statusBar.Items.AddRange(new DevComponents.DotNetBar.BaseItem[] { aLabelItem, progressBarItem,RefLabelItem,RefScaleCmb, CurLabelItem, CurScaleCmb });
            this.StatusBar = statusBar;
            #endregion
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

        public DevComponents.AdvTree.AdvTree DataTree
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

        public DevComponents.AdvTree.AdvTree ProjectTree
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

        public DevComponents.DotNetBar.Controls.DataGridViewX UpdateGrid
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
        public DevComponents.DotNetBar.Controls.DataGridViewX PolylineSearchGrid
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
        public DevComponents.DotNetBar.Controls.DataGridViewX PolygonSearchGrid
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
        public DevComponents.DotNetBar.Controls.DataGridViewX JoinMergeResultGrid
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




        public DevComponents.DotNetBar.TextBoxItem TxtDisplayPage
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

        public DevComponents.DotNetBar.ButtonItem BtnFirst
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
        public DevComponents.DotNetBar.ButtonItem BtnLast
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
        public DevComponents.DotNetBar.ButtonItem BtnPre
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
        public DevComponents.DotNetBar.ButtonItem BtnNext
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

        public DevComponents.DotNetBar.Controls.DataGridViewX DataCheckGrid
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

        public Dictionary<string, DevComponents.DotNetBar.ContextMenuBar> DicContextMenu
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
            get
            {
                try
                {
                    DevComponents.DotNetBar.Bar statusBar = this.StatusBar as DevComponents.DotNetBar.Bar;
                    return statusBar.Items["GisLabel"].Text;
                }
                catch
                {
                    return string.Empty;
                }
            }
            set
            {
                try
                {
                    DevComponents.DotNetBar.Bar statusBar = this.StatusBar as DevComponents.DotNetBar.Bar;
                    statusBar.Items["GisLabel"].Text = value;
                }
                catch
                {
                }
            }
        }

        public DevComponents.DotNetBar.ProgressBarItem ProgressBar
        {
            get
            {
                try
                {
                    DevComponents.DotNetBar.Bar statusBar = this.StatusBar as DevComponents.DotNetBar.Bar;
                    return statusBar.Items["GisprogressBarItem"] as DevComponents.DotNetBar.ProgressBarItem;
                }
                catch
                {
                    return null;
                }
            }
            set
            {
                try
                {
                    DevComponents.DotNetBar.Bar statusBar = this.StatusBar as DevComponents.DotNetBar.Bar;
                    statusBar.Items["GisprogressBarItem"] = value;
                }
                catch
                {
                }
            }
        }

        //�ο�������cmb
        public DevComponents.DotNetBar.ComboBoxItem RefScaleCmb
        {
            get
            {
                try
                {
                    DevComponents.DotNetBar.Bar statusBar = this.StatusBar as DevComponents.DotNetBar.Bar;
                    return statusBar.Items["RefScaleCmbItem"] as DevComponents.DotNetBar.ComboBoxItem;
                }
                catch
                {
                    return null;
                }
            }
            set
            {
                try
                {
                    DevComponents.DotNetBar.Bar statusBar = this.StatusBar as DevComponents.DotNetBar.Bar;
                    statusBar.Items["RefScaleCmbItem"] = value;
                }
                catch
                {
                }
            }
        }
        //��ǰ������cmb
        public DevComponents.DotNetBar.ComboBoxItem CurScaleCmb
        {
            get
            {
                try
                {
                    DevComponents.DotNetBar.Bar statusBar = this.StatusBar as DevComponents.DotNetBar.Bar;
                    return statusBar.Items["CurScaleCmbItem"] as DevComponents.DotNetBar.ComboBoxItem;
                }
                catch
                {
                    return null;
                }
            }
            set
            {
                try
                {
                    DevComponents.DotNetBar.Bar statusBar = this.StatusBar as DevComponents.DotNetBar.Bar;
                    statusBar.Items["CurScaleCmbItem"] = value;
                }
                catch
                {
                }
            }
        }
        //������ʾ�ı���
        public DevComponents.DotNetBar.TextBoxItem CoorTxt
        {
            get
            {
                try
                {
                    DevComponents.DotNetBar.Bar statusBar = this.StatusBar as DevComponents.DotNetBar.Bar;
                    return statusBar.Items["CoorTxtItem"] as DevComponents.DotNetBar.TextBoxItem;
                }
                catch
                {
                    return null;
                }
            }
            set
            {
                try
                {
                    DevComponents.DotNetBar.Bar statusBar = this.StatusBar as DevComponents.DotNetBar.Bar;
                    statusBar.Items["CoorTxtItem"] = value;
                }
                catch
                {
                }
            }
        }


        public string CoorXY
        {
            get
            {
                try
                {
                    DevComponents.DotNetBar.Bar statusBar = this.StatusBar as DevComponents.DotNetBar.Bar;
                    return statusBar.Items["GisCoorLabel"].Text;
                }
                catch
                {
                    return string.Empty;
                }
            }
            set
            {
                try
                {
                    DevComponents.DotNetBar.Bar statusBar = this.StatusBar as DevComponents.DotNetBar.Bar;
                    statusBar.Items["GisCoorLabel"].Text = value;
                }
                catch
                {
                }
            }
        }

        public string UserInfo
        {
            get
            {
                try
                {
                    DevComponents.DotNetBar.Bar statusBar = this.StatusBar as DevComponents.DotNetBar.Bar;
                    return statusBar.Items["GisUserInfoLabel"].Text;
                }
                catch
                {
                    return string.Empty;
                }
            }
            set
            {
                try
                {
                    DevComponents.DotNetBar.Bar statusBar = this.StatusBar as DevComponents.DotNetBar.Bar;
                    statusBar.Items["GisUserInfoLabel"].Text = value;
                }
                catch
                {
                }
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

       //public PictureBox p1 { get; set; }
       //public PictureBox p2 { get; set; }
       //public PictureBox p3 { get; set; }
       //public PictureBox p4 { get; set; }

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
        public List<DevComponents.DotNetBar.ComboBoxItem> ScaleBoxList
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

        public Dictionary<DevComponents.DotNetBar.RibbonTabItem, string> DicTabs
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
        private Dictionary<string, DevComponents.DotNetBar.ContextMenuBar> _DicContextMenu;           //�Ҽ��˵�����
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
        private List<DevComponents.DotNetBar.ComboBoxItem> _scaleBoxList;                        //��������ʾ        
        private Dictionary<DevComponents.DotNetBar.RibbonTabItem, string> _dicTabs;          //��tab����ϵͳ����
        //end


        private DevComponents.AdvTree.AdvTree _DataTree;          // ���ݴ�����ͼ
        private DevComponents.AdvTree.AdvTree _ErrTree;           // ������ͼ
        private DevComponents.AdvTree.AdvTree _ProjectTree;       // ������Ϣ��ͼ
        private DevComponents.DotNetBar.Controls.DataGridViewX _UpdateDataGrid;   //���¶Աȱ��

        private DevComponents.DotNetBar.TextBoxItem _txtDisplayPage;//���¶Ա��б��ҳ��ʾ�ı���
        private DevComponents.DotNetBar.ButtonItem _btnFirst;//���¶Ա��б��ҳ��ʾ��ť
        private DevComponents.DotNetBar.ButtonItem _btnLast;//���¶Ա��б��ҳ��ʾ��ť
        private DevComponents.DotNetBar.ButtonItem _btnPre;//���¶Ա��б��ҳ��ʾ��ť
        private DevComponents.DotNetBar.ButtonItem _btnNext;//���¶Ա��б��ҳ��ʾ��ť

        private DevComponents.DotNetBar.Controls.DataGridViewX _DataCheckGrid;   //���ݼ����
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

            #region �Զ���״̬��
            DevComponents.DotNetBar.Bar statusBar = new DevComponents.DotNetBar.Bar();
            statusBar.Name = "SMPDStatusBar";
            statusBar.TabStop = false;
            statusBar.Stretch = true;
            statusBar.Style = DevComponents.DotNetBar.eDotNetBarStyle.Office2007;
            statusBar.AccessibleRole = System.Windows.Forms.AccessibleRole.StatusBar;
            statusBar.AntiAlias = true;
            statusBar.BarType = DevComponents.DotNetBar.eBarType.StatusBar;
            statusBar.Dock = System.Windows.Forms.DockStyle.Bottom;
            statusBar.GrabHandleStyle = DevComponents.DotNetBar.eGrabHandleStyle.ResizeHandle;
            statusBar.Dock = System.Windows.Forms.DockStyle.Bottom;

            //������ʾ����
            DevComponents.DotNetBar.LabelItem aLabelItem = new DevComponents.DotNetBar.LabelItem();
            aLabelItem.Name = "SMPDLabel";
            aLabelItem.Stretch = true;
            aLabelItem.PaddingLeft = 2;
            aLabelItem.PaddingRight = 5;
            aLabelItem.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));

            //������
            DevComponents.DotNetBar.ProgressBarItem progressBarItem = new DevComponents.DotNetBar.ProgressBarItem();
            progressBarItem.ChunkGradientAngle = 0F;
            progressBarItem.MenuVisibility = DevComponents.DotNetBar.eMenuVisibility.VisibleAlways;
            progressBarItem.Name = "SMPDprogressBarItem";
            progressBarItem.RecentlyUsed = false;
            progressBarItem.Stretch = true;
            progressBarItem.Visible = false;
            progressBarItem.ColorTable = DevComponents.DotNetBar.eProgressBarItemColor.Paused;

            //�ο�������������
            DevComponents.DotNetBar.LabelItem RefLabelItem = new DevComponents.DotNetBar.LabelItem();
            RefLabelItem.Visible = true;
            RefLabelItem.Text = "�ο�������:";//"ReferenceScale:";
            RefLabelItem.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));

            //�ο�������������
            DevComponents.DotNetBar.ComboBoxItem RefScaleCmb = new DevComponents.DotNetBar.ComboBoxItem();
            RefScaleCmb.Name = "RefScaleCmbItem";
            RefScaleCmb.Visible = true;
            RefScaleCmb.Enabled = true;
            RefScaleCmb.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDown;
            object[] objScale = null;
            ModScale.GetScaleConfig(out objScale );
            RefScaleCmb.Items.AddRange(objScale);

            //��ǰ������������
            DevComponents.DotNetBar.LabelItem CurLabelItem = new DevComponents.DotNetBar.LabelItem();
            CurLabelItem.Visible = true;
            CurLabelItem.Text = "��ǰ������:";// "CurrentScale:";
            CurLabelItem.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));

            //��ǰ������������
            DevComponents.DotNetBar.ComboBoxItem CurScaleCmb = new DevComponents.DotNetBar.ComboBoxItem();
            CurScaleCmb.Name = "CurScaleCmbItem";
            CurScaleCmb.Visible = true;
            CurScaleCmb.Enabled = true;
            CurScaleCmb.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDown;
            CurScaleCmb.Items.AddRange(objScale);

            //�����ı���
            DevComponents.DotNetBar.TextBoxItem CoorTxt = new DevComponents.DotNetBar.TextBoxItem();
            CoorTxt.Name = "CoorTxtItem";
            CoorTxt.Visible = true;
            CoorTxt.Enabled = true;

            //�û���Ϣ����
            DevComponents.DotNetBar.LabelItem aUserInfoLabelItem = new DevComponents.DotNetBar.LabelItem();
            aUserInfoLabelItem.Name = "SMPDGisUserInfoLabel";
            aUserInfoLabelItem.Stretch = true;
            aUserInfoLabelItem.PaddingLeft = 2;
            aUserInfoLabelItem.PaddingRight = 2;
            aUserInfoLabelItem.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));

            statusBar.Items.AddRange(new DevComponents.DotNetBar.BaseItem[] { aLabelItem, progressBarItem, RefLabelItem, RefScaleCmb, CurLabelItem, CurScaleCmb });
            this.StatusBar = statusBar;
            #endregion
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

        public Dictionary<string, DevComponents.DotNetBar.ContextMenuBar> DicContextMenu
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

        public DevComponents.AdvTree.AdvTree DataTree
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

        public DevComponents.DotNetBar.TextBoxItem TxtDisplayPage
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

        public DevComponents.DotNetBar.ButtonItem BtnFirst
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
        public DevComponents.DotNetBar.ButtonItem BtnLast
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
        public DevComponents.DotNetBar.ButtonItem BtnPre
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
        public DevComponents.DotNetBar.ButtonItem BtnNext
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

        public DevComponents.AdvTree.AdvTree ErrTree
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

        public DevComponents.AdvTree.AdvTree ProjectTree
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

        public DevComponents.DotNetBar.Controls.DataGridViewX UpdateDataGrid
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

        public DevComponents.DotNetBar.Controls.DataGridViewX DataCheckGrid
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
            get
            {
                try
                {
                    DevComponents.DotNetBar.Bar statusBar = this.StatusBar as DevComponents.DotNetBar.Bar;
                    return statusBar.Items["SMPDLabel"].Text;
                }
                catch
                {
                    return string.Empty;
                }
            }
            set
            {
                try
                {
                    DevComponents.DotNetBar.Bar statusBar = this.StatusBar as DevComponents.DotNetBar.Bar;
                    statusBar.Items["SMPDLabel"].Text = value;
                }
                catch
                {
                }
            }
        }

        public DevComponents.DotNetBar.ProgressBarItem ProgressBar
        {
            get
            {
                try
                {
                    DevComponents.DotNetBar.Bar statusBar = this.StatusBar as DevComponents.DotNetBar.Bar;
                    return statusBar.Items["SMPDprogressBarItem"] as DevComponents.DotNetBar.ProgressBarItem;
                }
                catch
                {
                    return null;
                }
            }
            set
            {
                try
                {
                    DevComponents.DotNetBar.Bar statusBar = this.StatusBar as DevComponents.DotNetBar.Bar;
                    statusBar.Items["SMPDprogressBarItem"] = value;
                }
                catch
                {
                }
            }
        }

        //�ο�������cmb
        public DevComponents.DotNetBar.ComboBoxItem RefScaleCmb
        {
            get
            {
                try
                {
                    DevComponents.DotNetBar.Bar statusBar = this.StatusBar as DevComponents.DotNetBar.Bar;
                    return statusBar.Items["RefScaleCmbItem"] as DevComponents.DotNetBar.ComboBoxItem;
                }
                catch
                {
                    return null;
                }
            }
            set
            {
                try
                {
                    DevComponents.DotNetBar.Bar statusBar = this.StatusBar as DevComponents.DotNetBar.Bar;
                    statusBar.Items["RefScaleCmbItem"] = value;
                }
                catch
                {
                }
            }
        }
        //��ǰ������cmb
        public DevComponents.DotNetBar.ComboBoxItem CurScaleCmb
        {
            get
            {
                try
                {
                    DevComponents.DotNetBar.Bar statusBar = this.StatusBar as DevComponents.DotNetBar.Bar;
                    return statusBar.Items["CurScaleCmbItem"] as DevComponents.DotNetBar.ComboBoxItem;
                }
                catch
                {
                    return null;
                }
            }
            set
            {
                try
                {
                    DevComponents.DotNetBar.Bar statusBar = this.StatusBar as DevComponents.DotNetBar.Bar;
                    statusBar.Items["CurScaleCmbItem"] = value;
                }
                catch
                {
                }
            }
        }
        //������ʾ�ı���
        public DevComponents.DotNetBar.TextBoxItem CoorTxt
        {
            get
            {
                try
                {
                    DevComponents.DotNetBar.Bar statusBar = this.StatusBar as DevComponents.DotNetBar.Bar;
                    return statusBar.Items["CoorTxtItem"] as DevComponents.DotNetBar.TextBoxItem;
                }
                catch
                {
                    return null;
                }
            }
            set
            {
                try
                {
                    DevComponents.DotNetBar.Bar statusBar = this.StatusBar as DevComponents.DotNetBar.Bar;
                    statusBar.Items["CoorTxtItem"] = value;
                }
                catch
                {
                }
            }
        }

        public string UserInfo
        {
            get
            {
                try
                {
                    DevComponents.DotNetBar.Bar statusBar = this.StatusBar as DevComponents.DotNetBar.Bar;
                    return statusBar.Items["SMPDGisUserInfoLabel"].Text;
                }
                catch
                {
                    return string.Empty;
                }
            }
            set
            {
                try
                {
                    DevComponents.DotNetBar.Bar statusBar = this.StatusBar as DevComponents.DotNetBar.Bar;
                    statusBar.Items["SMPDGisUserInfoLabel"].Text = value;
                }
                catch
                {
                }
            }
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
        public List<DevComponents.DotNetBar.ComboBoxItem> ScaleBoxList
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

        public Dictionary<DevComponents.DotNetBar.RibbonTabItem, string> DicTabs
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
        private Dictionary<string, DevComponents.DotNetBar.ContextMenuBar> _DicContextMenu;           //�Ҽ��˵�����
        private string _ImageResPath;


        //wgf 2011-5-18
        //--------------------------------------------------
        private System.Windows.Forms.TreeView _DataTabIndexTree;

        private System.Windows.Forms.DataGridView _GridCtrl;

        private System.Windows.Forms.RichTextBox _tipRichBox;

        private string _strLogFilePath;
        //--------------------------------------------------

        private DevComponents.AdvTree.AdvTree _MainTree;          // ��������ͼ
        private DevComponents.AdvTree.AdvTree _RoleTree;           // �û�����ͼ
        private DevComponents.AdvTree.AdvTree _UserTree;       // �û���ͼ
        private DevComponents.AdvTree.AdvTree _privilegeTree;       //Ȩ����
        private DevComponents.DotNetBar.Controls.GroupPanel _CurrentPanel;       // ��ǰGroupPanel
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

            #region �Զ���״̬��
            DevComponents.DotNetBar.Bar statusBar = new DevComponents.DotNetBar.Bar();
            statusBar.Name = "SMPDStatusBar";
            statusBar.TabStop = false;
            statusBar.Stretch = true;
            statusBar.Style = DevComponents.DotNetBar.eDotNetBarStyle.Office2007;
            statusBar.AccessibleRole = System.Windows.Forms.AccessibleRole.StatusBar;
            statusBar.AntiAlias = true;
            statusBar.BarType = DevComponents.DotNetBar.eBarType.StatusBar;
            statusBar.Dock = System.Windows.Forms.DockStyle.Bottom;
            statusBar.GrabHandleStyle = DevComponents.DotNetBar.eGrabHandleStyle.ResizeHandle;
            statusBar.Dock = System.Windows.Forms.DockStyle.Bottom;

            //������ʾ����
            DevComponents.DotNetBar.LabelItem aLabelItem = new DevComponents.DotNetBar.LabelItem();
            aLabelItem.Name = "SMPDLabel";
            aLabelItem.Stretch = true;
            aLabelItem.PaddingLeft = 2;
            aLabelItem.PaddingRight = 5;
            aLabelItem.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));

            //������
            DevComponents.DotNetBar.ProgressBarItem progressBarItem = new DevComponents.DotNetBar.ProgressBarItem();
            progressBarItem.ChunkGradientAngle = 0F;
            progressBarItem.MenuVisibility = DevComponents.DotNetBar.eMenuVisibility.VisibleAlways;
            progressBarItem.Name = "SMPDprogressBarItem";
            progressBarItem.RecentlyUsed = false;
            progressBarItem.Stretch = true;
            progressBarItem.Visible = false;
            progressBarItem.ColorTable = DevComponents.DotNetBar.eProgressBarItemColor.Paused;
            //�ο�������������
            DevComponents.DotNetBar.LabelItem RefLabelItem = new DevComponents.DotNetBar.LabelItem();
            RefLabelItem.Visible = true;
            RefLabelItem.Name = "RefScaleLabel";
            RefLabelItem.Text = "�ο�������:";//"ReferenceScale:";
            RefLabelItem.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));

            //�ο�������������
            DevComponents.DotNetBar.ComboBoxItem RefScaleCmb = new DevComponents.DotNetBar.ComboBoxItem();
            RefScaleCmb.Name = "RefScaleCmbItem";
            RefScaleCmb.Visible = true;
            RefScaleCmb.Enabled = true;
            RefScaleCmb.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDown;
            //cyf 20110615 add:��ӹ��������
            RefScaleCmb.Items.AddRange(new object[] { 0, 500, 1000, 2000, 5000, 10000, 50000, 250000, 1000000 });
            //end

            //��ǰ������������
            DevComponents.DotNetBar.LabelItem CurLabelItem = new DevComponents.DotNetBar.LabelItem();
            CurLabelItem.Visible = true;
            CurLabelItem.Name = "CurScaleLabel";
            CurLabelItem.Text = "��ǰ������:";// "CurrentScale:";
            CurLabelItem.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));


            //��ǰ������������
            DevComponents.DotNetBar.ComboBoxItem CurScaleCmb = new DevComponents.DotNetBar.ComboBoxItem();
            CurScaleCmb.Name = "CurScaleCmbItem";
            CurScaleCmb.Visible = true;
            CurScaleCmb.Enabled = true;
            CurScaleCmb.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDown;
            //cyf 20110615 add:��ӹ��������
            CurScaleCmb.Items.AddRange(new object[] { 0, 500, 1000, 2000, 5000, 10000, 50000, 250000, 1000000 });


            //�����ı���
            DevComponents.DotNetBar.TextBoxItem CoorTxt = new DevComponents.DotNetBar.TextBoxItem();
            CoorTxt.Name = "CoorTxtItem";
            CoorTxt.Visible = true;
            CoorTxt.Enabled = true;

            //�û���Ϣ����
            DevComponents.DotNetBar.LabelItem aUserInfoLabelItem = new DevComponents.DotNetBar.LabelItem();
            aUserInfoLabelItem.Name = "SMPDGisUserInfoLabel";
            aUserInfoLabelItem.Stretch = true;
            aUserInfoLabelItem.PaddingLeft = 2;
            aUserInfoLabelItem.PaddingRight = 2;
            aUserInfoLabelItem.TextAlignment = System.Drawing.StringAlignment.Far;
            aUserInfoLabelItem.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));

            //ͼ�ϵ���������
            DevComponents.DotNetBar.LabelItem aCoorLabelItem = new DevComponents.DotNetBar.LabelItem();
            aCoorLabelItem.Name = "GisCoorLabel";
            aCoorLabelItem.Stretch = true;
            aCoorLabelItem.PaddingLeft = 2;
            aCoorLabelItem.PaddingRight = 2;
            aCoorLabelItem.TextAlignment = System.Drawing.StringAlignment.Far;
            aCoorLabelItem.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));

            //cyf 20110615 modify:
            //statusBar.Items.AddRange(new DevComponents.DotNetBar.BaseItem[] { aLabelItem, progressBarItem, aUserInfoLabelItem, aCoorLabelItem });
            statusBar.Items.AddRange(new DevComponents.DotNetBar.BaseItem[] { aLabelItem, aUserInfoLabelItem, aCoorLabelItem, RefLabelItem, RefScaleCmb, CurLabelItem, CurScaleCmb });
            this.StatusBar = statusBar;
            #endregion
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

        public Dictionary<string, DevComponents.DotNetBar.ContextMenuBar> DicContextMenu
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

        public DevComponents.AdvTree.AdvTree MainTree
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

        public DevComponents.AdvTree.AdvTree RoleTree
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

        public DevComponents.AdvTree.AdvTree UserTree
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

        public DevComponents.AdvTree.AdvTree PrivilegeTree
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

        public DevComponents.DotNetBar.Controls.GroupPanel CurrentPanel
        {
            get
            {
                return _CurrentPanel;
            }
            set
            {
                _CurrentPanel = value;
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
            get
            {
                try
                {
                    DevComponents.DotNetBar.Bar statusBar = this.StatusBar as DevComponents.DotNetBar.Bar;
                    return statusBar.Items["SMPDLabel"].Text;
                }
                catch
                {
                    return string.Empty;
                }
            }
            set
            {
                try
                {
                    DevComponents.DotNetBar.Bar statusBar = this.StatusBar as DevComponents.DotNetBar.Bar;
                    statusBar.Items["SMPDLabel"].Text = value;
                }
                catch
                {
                }
            }
        }

        public DevComponents.DotNetBar.ProgressBarItem ProgressBar
        {
            get
            {
                try
                {
                    DevComponents.DotNetBar.Bar statusBar = this.StatusBar as DevComponents.DotNetBar.Bar;
                    return statusBar.Items["SMPDprogressBarItem"] as DevComponents.DotNetBar.ProgressBarItem;
                }
                catch
                {
                    return null;
                }
            }
            set
            {
                try
                {
                    DevComponents.DotNetBar.Bar statusBar = this.StatusBar as DevComponents.DotNetBar.Bar;
                    statusBar.Items["SMPDprogressBarItem"] = value;
                }
                catch
                {
                }
            }
        }

        //�ο�������cmb
        private DevComponents.DotNetBar.ComboBoxItem _RefScaleCmb;
        public DevComponents.DotNetBar.ComboBoxItem RefScaleCmb
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
            get
            {
                DevComponents.DotNetBar.Bar statusBar = this.StatusBar as DevComponents.DotNetBar.Bar;
                return statusBar.Items["RefScaleLabel"].Visible;
            }
            set
            {
                DevComponents.DotNetBar.Bar statusBar = this.StatusBar as DevComponents.DotNetBar.Bar;
                statusBar.Items["RefScaleCmbItem"].Visible = value;
                statusBar.Items["RefScaleLabel"].Visible = value;
            }
        }
        //��ǰ�������Ƿ�ɼ�
        public bool CurScaleVisible
        {
            get
            {
                DevComponents.DotNetBar.Bar statusBar = this.StatusBar as DevComponents.DotNetBar.Bar;
                return statusBar.Items["CurScaleLabel"].Visible;
            }
            set
            {
                DevComponents.DotNetBar.Bar statusBar = this.StatusBar as DevComponents.DotNetBar.Bar;
                statusBar.Items["CurScaleCmbItem"].Visible = value;
                statusBar.Items["CurScaleLabel"].Visible = value;
            }
        }
        //��ǰ������cmb
        public DevComponents.DotNetBar.ComboBoxItem CurScaleCmb
        {
            get
            {
                try
                {
                    DevComponents.DotNetBar.Bar statusBar = this.StatusBar as DevComponents.DotNetBar.Bar;
                    return statusBar.Items["CurScaleCmbItem"] as DevComponents.DotNetBar.ComboBoxItem;
                }
                catch
                {
                    return null;
                }
            }
            set
            {
                try
                {
                    DevComponents.DotNetBar.Bar statusBar = this.StatusBar as DevComponents.DotNetBar.Bar;
                    statusBar.Items["CurScaleCmbItem"] = value;
                }
                catch
                {
                }
            }
        }
        //end ������  �ӱ𴦿���
        //������ʾ�ı���
        private DevComponents.DotNetBar.TextBoxItem _CoorTxt;
        public DevComponents.DotNetBar.TextBoxItem CoorTxt
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
            get
            {
                try
                {
                    DevComponents.DotNetBar.Bar statusBar = this.StatusBar as DevComponents.DotNetBar.Bar;
                    return statusBar.Items["SMPDGisUserInfoLabel"].Text;
                }
                catch
                {
                    return string.Empty;
                }
            }
            set
            {
                try
                {
                    DevComponents.DotNetBar.Bar statusBar = this.StatusBar as DevComponents.DotNetBar.Bar;
                    statusBar.Items["SMPDGisUserInfoLabel"].Text = value;
                }
                catch
                {
                }
            }
        }

        public string CoorXY
        {
            get
            {
                try
                {
                    DevComponents.DotNetBar.Bar statusBar = this.StatusBar as DevComponents.DotNetBar.Bar;
                    return statusBar.Items["GisCoorLabel"].Text;
                }
                catch
                {
                    return string.Empty;
                }
            }
            set
            {
                try
                {
                    DevComponents.DotNetBar.Bar statusBar = this.StatusBar as DevComponents.DotNetBar.Bar;
                    statusBar.Items["GisCoorLabel"].Text = value;
                }
                catch
                {
                }
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
        private Dictionary<string, DevComponents.DotNetBar.ContextMenuBar> _DicContextMenu;           //�Ҽ��˵�����
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
        private List<DevComponents.DotNetBar.ComboBoxItem> _scaleBoxList;                        //��������ʾ        
        private Dictionary<DevComponents.DotNetBar.RibbonTabItem, string> _dicTabs;          //��tab����ϵͳ����

        private string _LayerTreeXmlPath = "";  //added by chulili 20111101 
        private object _LayerTree;  //Ŀ¼��ͼ �������GeoLayerTreeLib.LayerManager.UcDataLib
        private object _LayerAdvTree;  //Ŀ¼��ͼ �������advTree
        private DevComponents.AdvTree.AdvTree _DataTree;          // ���ݴ�����ͼ
        private DevComponents.AdvTree.AdvTree _XZQTree;          // yjl20110924 add ��������ͼ
        private DevComponents.AdvTree.AdvTree _ResultFileTree;      //added by chulili 20110923 �ɹ��б���ͼ
        private DevComponents.AdvTree.AdvTree _ErrTree;           // ������ͼ
        private DevComponents.AdvTree.AdvTree _ProjectTree;       // ������Ϣ��ͼ
        private DevComponents.DotNetBar.Controls.DataGridViewX _UpdateDataGrid;   //���¶Աȱ��
        private DevComponents.DotNetBar.Controls.DataGridViewX _DataCheckGrid;   //���ݼ����
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
        private DevComponents.DotNetBar.TabControl _IndextabControl;//������tabcontrol ϯʤ
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

            #region �Զ���״̬��
            DevComponents.DotNetBar.Bar statusBar = new DevComponents.DotNetBar.Bar();
            statusBar.Name = "SMPDStatusBar";
            statusBar.TabStop = false;
            statusBar.Stretch = true;
            statusBar.Style = DevComponents.DotNetBar.eDotNetBarStyle.Office2007;
            statusBar.AccessibleRole = System.Windows.Forms.AccessibleRole.StatusBar;
            statusBar.AntiAlias = true;
            statusBar.BarType = DevComponents.DotNetBar.eBarType.StatusBar;
            statusBar.Dock = System.Windows.Forms.DockStyle.Bottom;
            statusBar.GrabHandleStyle = DevComponents.DotNetBar.eGrabHandleStyle.ResizeHandle;
            statusBar.Dock = System.Windows.Forms.DockStyle.Bottom;

            //������ʾ����
            DevComponents.DotNetBar.LabelItem aLabelItem = new DevComponents.DotNetBar.LabelItem();
            aLabelItem.Name = "SMPDLabel";
            aLabelItem.Stretch = true;
            aLabelItem.PaddingLeft = 2;
            aLabelItem.PaddingRight = 5;
            aLabelItem.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));

            //������
            DevComponents.DotNetBar.ProgressBarItem progressBarItem = new DevComponents.DotNetBar.ProgressBarItem();
            progressBarItem.ChunkGradientAngle = 0F;
            progressBarItem.MenuVisibility = DevComponents.DotNetBar.eMenuVisibility.VisibleAlways;
            progressBarItem.Name = "SMPDprogressBarItem";
            progressBarItem.RecentlyUsed = false;
            progressBarItem.Stretch = true;
            progressBarItem.Visible = false;
            progressBarItem.ColorTable = DevComponents.DotNetBar.eProgressBarItemColor.Paused;
            //�ο�������������
            DevComponents.DotNetBar.LabelItem RefLabelItem = new DevComponents.DotNetBar.LabelItem();
            RefLabelItem.Visible = false;   //changed by chulili 20110729 ���ο����������أ�Ŀǰû�õ���
            RefLabelItem.Text = "�ο�������:";//"ReferenceScale:";
            RefLabelItem.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));

            //�ο�������������
            DevComponents.DotNetBar.ComboBoxItem RefScaleCmb = new DevComponents.DotNetBar.ComboBoxItem();
            RefScaleCmb.Name = "RefScaleCmbItem";
            RefScaleCmb.Visible = false; //changed by chulili 20110729 ���ο����������أ�Ŀǰû�õ���
            RefScaleCmb.Enabled = true;
            RefScaleCmb.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDown;
            //cyf 20110615 add:��ӹ��������
            object[] objScale = null;
            int intWidth;
            ModScale.GetScaleConfig(out objScale, out intWidth);
            RefScaleCmb.Items.AddRange(objScale);//changed by chulili 20110731 ȥ��0,500
            //end

            //��ǰ������������
            DevComponents.DotNetBar.LabelItem CurLabelItem = new DevComponents.DotNetBar.LabelItem();
            CurLabelItem.Visible = true;
            CurLabelItem.Text = "��ǰ������:";// "CurrentScale:";
            CurLabelItem.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));


            //��ǰ������������
            DevComponents.DotNetBar.ComboBoxItem CurScaleCmb = new DevComponents.DotNetBar.ComboBoxItem();
            CurScaleCmb.Name = "CurScaleCmbItem";
            CurScaleCmb.Visible = true;
            CurScaleCmb.Enabled = true;
            CurScaleCmb.ComboWidth = intWidth;
            CurScaleCmb.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDown;
            //cyf 20110615 add:��ӹ��������
            CurScaleCmb.Items.AddRange(objScale);//changed by chulili 20110731 ȥ��0,500,1000,2000


            //�����ı���
            DevComponents.DotNetBar.TextBoxItem CoorTxt = new DevComponents.DotNetBar.TextBoxItem();
            CoorTxt.Name = "CoorTxtItem";
            CoorTxt.Visible = true;
            CoorTxt.Enabled = true;

            //�û���Ϣ����
            DevComponents.DotNetBar.LabelItem aUserInfoLabelItem = new DevComponents.DotNetBar.LabelItem();
            aUserInfoLabelItem.Name = "SMPDGisUserInfoLabel";
            //aUserInfoLabelItem.Stretch = true;
            aUserInfoLabelItem.PaddingLeft = 2;
            aUserInfoLabelItem.PaddingRight = 2;
            aUserInfoLabelItem.TextAlignment = System.Drawing.StringAlignment.Center;   //TEXT������ʾ���м� by chulili 20111117
            aUserInfoLabelItem.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));

            //ͼ�ϵ���������
            DevComponents.DotNetBar.LabelItem aCoorLabelItem = new DevComponents.DotNetBar.LabelItem();
            aCoorLabelItem.Name = "GisCoorLabel";
            aCoorLabelItem.Stretch = true;
            aCoorLabelItem.PaddingLeft = 5;
            aCoorLabelItem.PaddingRight = 5;
            aCoorLabelItem.TextAlignment = System.Drawing.StringAlignment.Far;  //TEXT������ʾ���м� by chulili 20111117
            aCoorLabelItem.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));

            //cyf 20110615 modify:
            //statusBar.Items.AddRange(new DevComponents.DotNetBar.BaseItem[] { aLabelItem, progressBarItem, aUserInfoLabelItem, aCoorLabelItem });
            statusBar.Items.AddRange(new DevComponents.DotNetBar.BaseItem[] { aLabelItem, aUserInfoLabelItem, aCoorLabelItem, RefLabelItem, RefScaleCmb, CurLabelItem, CurScaleCmb });
            
            this.StatusBar = statusBar;
            #endregion
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

        public Dictionary<string, DevComponents.DotNetBar.ContextMenuBar> DicContextMenu
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

        public DevComponents.AdvTree.AdvTree DataTree
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
        public DevComponents.AdvTree.AdvTree XZQTree
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
        public DevComponents.AdvTree.AdvTree ResultFileTree
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
        public DevComponents.AdvTree.AdvTree ErrTree
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

        public DevComponents.AdvTree.AdvTree ProjectTree
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

        public DevComponents.DotNetBar.TabControl IndextabControl
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
        public DevComponents.DotNetBar.Controls.DataGridViewX UpdateDataGrid
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

        public DevComponents.DotNetBar.Controls.DataGridViewX DataCheckGrid
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
            get
            {
                try
                {
                    DevComponents.DotNetBar.Bar statusBar = this.StatusBar as DevComponents.DotNetBar.Bar;
                    return statusBar.Items["SMPDLabel"].Text;
                }
                catch
                {
                    return string.Empty;
                }
            }
            set
            {
                try
                {
                    DevComponents.DotNetBar.Bar statusBar = this.StatusBar as DevComponents.DotNetBar.Bar;
                    statusBar.Items["SMPDLabel"].Text = value;
                }
                catch
                {
                }
            }
        }

        public DevComponents.DotNetBar.ProgressBarItem ProgressBar
        {
            get
            {
                try
                {
                    DevComponents.DotNetBar.Bar statusBar = this.StatusBar as DevComponents.DotNetBar.Bar;
                    return statusBar.Items["SMPDprogressBarItem"] as DevComponents.DotNetBar.ProgressBarItem;
                }
                catch
                {
                    return null;
                }
            }
            set
            {
                try
                {
                    DevComponents.DotNetBar.Bar statusBar = this.StatusBar as DevComponents.DotNetBar.Bar;
                    statusBar.Items["SMPDprogressBarItem"] = value;
                }
                catch
                {
                }
            }
        }

        ////�ο�������cmb
        //private DevComponents.DotNetBar.ComboBoxItem _RefScaleCmb;
        //public DevComponents.DotNetBar.ComboBoxItem RefScaleCmb
        //{
        //    get
        //    {
        //        return _RefScaleCmb;
        //    }
        //    set
        //    {
        //        _RefScaleCmb = value;
        //    }
        //}
        ////��ǰ������cmb
        //private DevComponents.DotNetBar.ComboBoxItem _CurScaleCmb;
        //public DevComponents.DotNetBar.ComboBoxItem CurScaleCmb
        //{
        //    get
        //    {
        //        return _CurScaleCmb;
        //    }
        //    set
        //    {
        //        _CurScaleCmb = value;
        //    }
        //}
        //������  ��AppGIS�п��� 20110705
        //�ο�������cmb
        public DevComponents.DotNetBar.ComboBoxItem RefScaleCmb
        {
            get
            {
                try
                {
                    DevComponents.DotNetBar.Bar statusBar = this.StatusBar as DevComponents.DotNetBar.Bar;
                    return statusBar.Items["RefScaleCmbItem"] as DevComponents.DotNetBar.ComboBoxItem;
                }
                catch
                {
                    return null;
                }
            }
            set
            {
                try
                {
                    DevComponents.DotNetBar.Bar statusBar = this.StatusBar as DevComponents.DotNetBar.Bar;
                    statusBar.Items["RefScaleCmbItem"] = value;
                }
                catch
                {
                }
            }
        }
        //��ǰ������cmb
        public DevComponents.DotNetBar.ComboBoxItem CurScaleCmb
        {
            get
            {
                try
                {
                    DevComponents.DotNetBar.Bar statusBar = this.StatusBar as DevComponents.DotNetBar.Bar;
                    return statusBar.Items["CurScaleCmbItem"] as DevComponents.DotNetBar.ComboBoxItem;
                }
                catch
                {
                    return null;
                }
            }
            set
            {
                try
                {
                    DevComponents.DotNetBar.Bar statusBar = this.StatusBar as DevComponents.DotNetBar.Bar;
                    statusBar.Items["CurScaleCmbItem"] = value;
                }
                catch
                {
                }
            }
        }
        //end ������  �ӱ𴦿���
        //������ʾ�ı���
        private DevComponents.DotNetBar.TextBoxItem _CoorTxt;
        public DevComponents.DotNetBar.TextBoxItem CoorTxt
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
            get
            {
                try
                {
                    DevComponents.DotNetBar.Bar statusBar = this.StatusBar as DevComponents.DotNetBar.Bar;
                    return statusBar.Items["SMPDGisUserInfoLabel"].Text;
                }
                catch
                {
                    return string.Empty;
                }
            }
            set
            {
                try
                {
                    DevComponents.DotNetBar.Bar statusBar = this.StatusBar as DevComponents.DotNetBar.Bar;
                    statusBar.Items["SMPDGisUserInfoLabel"].Text = value;
                }
                catch
                {
                }
            }
        }

        public string CoorXY
        {
            get
            {
                try
                {
                    DevComponents.DotNetBar.Bar statusBar = this.StatusBar as DevComponents.DotNetBar.Bar;
                    return statusBar.Items["GisCoorLabel"].Text;
                }
                catch
                {
                    return string.Empty;
                }
            }
            set
            {
                try
                {
                    DevComponents.DotNetBar.Bar statusBar = this.StatusBar as DevComponents.DotNetBar.Bar;
                    statusBar.Items["GisCoorLabel"].Text = value;
                }
                catch
                {
                }
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
        public List<DevComponents.DotNetBar.ComboBoxItem> ScaleBoxList
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

        public Dictionary<DevComponents.DotNetBar.RibbonTabItem, string> DicTabs
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
        private Dictionary<string, DevComponents.DotNetBar.ContextMenuBar> _DicContextMenu;           //�Ҽ��˵�����
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
        private List<DevComponents.DotNetBar.ComboBoxItem> _scaleBoxList;                        //��������ʾ        
        private Dictionary<DevComponents.DotNetBar.RibbonTabItem, string> _dicTabs;          //��tab����ϵͳ����
        //end


        private DevComponents.AdvTree.AdvTree _ProjectTree;       // ������Ϣ��
        private DevComponents.DotNetBar.Controls.DataGridViewX _DataInfoGrid;  //������Ϣ�б�
        private DevComponents.DotNetBar.Controls.DataGridViewX _MetaDataGrid;  //Ԫ�����б�

        private DevComponents.AdvTree.AdvTree _SysSettingTree;//ϵͳ������ͼ
        private DevComponents.DotNetBar.Controls.DataGridViewX _SysSettingGrid;//ϵͳ�����б�

        //private DevComponents.AdvTree.AdvTree _DataTree;          // ���ݴ�����ͼ
        //private DevComponents.DotNetBar.Controls.DataGridViewX _UpdateGrid;  //���¶Ա��б�
        private DevComponents.DotNetBar.TextBoxItem _txtDisplayPage;//���¶Ա��б��ҳ��ʾ�ı���
        private DevComponents.DotNetBar.ButtonItem _btnFirst;//���¶Ա��б��ҳ��ʾ��ť
        private DevComponents.DotNetBar.ButtonItem _btnLast;//���¶Ա��б��ҳ��ʾ��ť
        private DevComponents.DotNetBar.ButtonItem _btnPre;//���¶Ա��б��ҳ��ʾ��ť
        private DevComponents.DotNetBar.ButtonItem _btnNext;//���¶Ա��б��ҳ��ʾ��ť

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

            #region �Զ���״̬��
            DevComponents.DotNetBar.Bar statusBar = new DevComponents.DotNetBar.Bar();
            statusBar.Name = "GisStatusBar";
            statusBar.TabStop = false;
            statusBar.Stretch = true;
            statusBar.Style = DevComponents.DotNetBar.eDotNetBarStyle.Office2007;
            statusBar.AccessibleRole = System.Windows.Forms.AccessibleRole.StatusBar;
            statusBar.AntiAlias = true;
            statusBar.BarType = DevComponents.DotNetBar.eBarType.StatusBar;
            statusBar.Dock = System.Windows.Forms.DockStyle.Bottom;
            statusBar.GrabHandleStyle = DevComponents.DotNetBar.eGrabHandleStyle.ResizeHandle;
            statusBar.Dock = System.Windows.Forms.DockStyle.Bottom;

            //������ʾ����
            DevComponents.DotNetBar.LabelItem aLabelItem = new DevComponents.DotNetBar.LabelItem();
            aLabelItem.Name = "GisLabel";
            aLabelItem.Stretch = true;
            aLabelItem.PaddingLeft = 2;
            aLabelItem.PaddingRight = 5;
            aLabelItem.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));

            //������
            DevComponents.DotNetBar.ProgressBarItem progressBarItem = new DevComponents.DotNetBar.ProgressBarItem();
            progressBarItem.ChunkGradientAngle = 0F;
            progressBarItem.MenuVisibility = DevComponents.DotNetBar.eMenuVisibility.VisibleAlways;
            progressBarItem.Name = "GisprogressBarItem";
            progressBarItem.RecentlyUsed = false;
            progressBarItem.Stretch = true;
            progressBarItem.Visible = false;
            progressBarItem.ColorTable = DevComponents.DotNetBar.eProgressBarItemColor.Paused;

            //�ο�������������
            DevComponents.DotNetBar.LabelItem RefLabelItem = new DevComponents.DotNetBar.LabelItem();
            RefLabelItem.Visible = true;
            RefLabelItem.Text = "�ο�������:";//"ReferenceScale:";
            RefLabelItem.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));

            //�ο�������������
            DevComponents.DotNetBar.ComboBoxItem RefScaleCmb = new DevComponents.DotNetBar.ComboBoxItem();
            RefScaleCmb.Name = "RefScaleCmbItem";
            RefScaleCmb.Visible = true;
            RefScaleCmb.Enabled = true;
            RefScaleCmb.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDown;
            RefScaleCmb.Items.AddRange(new object[] { 5000, 10000, 25000, 50000 });

            //��ǰ������������
            DevComponents.DotNetBar.LabelItem CurLabelItem = new DevComponents.DotNetBar.LabelItem();
            CurLabelItem.Visible = true;
            CurLabelItem.Text = "��ǰ������:";// "CurrentScale:";
            CurLabelItem.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));


            //��ǰ������������
            DevComponents.DotNetBar.ComboBoxItem CurScaleCmb = new DevComponents.DotNetBar.ComboBoxItem();
            CurScaleCmb.Name = "CurScaleCmbItem";
            CurScaleCmb.Visible = true;
            CurScaleCmb.Enabled = true;
            CurScaleCmb.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDown;
            CurScaleCmb.Items.AddRange(new object[] {  5000, 10000, 25000, 50000 });


            //�����ı���
            DevComponents.DotNetBar.TextBoxItem CoorTxt = new DevComponents.DotNetBar.TextBoxItem();
            CoorTxt.Name = "CoorTxtItem";
            CoorTxt.Visible = true;
            CoorTxt.Enabled = true;

            //ͼ�ϵ���������
            DevComponents.DotNetBar.LabelItem aCoorLabelItem = new DevComponents.DotNetBar.LabelItem();
            aCoorLabelItem.Name = "GisCoorLabel";
            aCoorLabelItem.Stretch = true;
            aCoorLabelItem.PaddingLeft = 2;
            aCoorLabelItem.PaddingRight = 2;
            aCoorLabelItem.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            //�û���Ϣ����
            DevComponents.DotNetBar.LabelItem aUserInfoLabelItem = new DevComponents.DotNetBar.LabelItem();
            aUserInfoLabelItem.Name = "GisUserInfoLabel";
            aUserInfoLabelItem.Stretch = true;
            aUserInfoLabelItem.PaddingLeft = 2;
            aUserInfoLabelItem.PaddingRight = 2;
            aUserInfoLabelItem.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));

            statusBar.Items.AddRange(new DevComponents.DotNetBar.BaseItem[] { aLabelItem, progressBarItem, RefLabelItem, RefScaleCmb, CurLabelItem, CurScaleCmb });
            this.StatusBar = statusBar;
            #endregion
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


        public DevComponents.AdvTree.AdvTree ProjectTree
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

        public DevComponents.DotNetBar.Controls.DataGridViewX DataInfoGrid
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

        public DevComponents.DotNetBar.Controls.DataGridViewX MetaDataGrid
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

        public DevComponents.DotNetBar.Controls.DataGridViewX SysSettingGrid
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

        public DevComponents.AdvTree.AdvTree SysSettingTree
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


        public DevComponents.DotNetBar.TextBoxItem TxtDisplayPage
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

        public DevComponents.DotNetBar.ButtonItem BtnFirst
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
        public DevComponents.DotNetBar.ButtonItem BtnLast
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
        public DevComponents.DotNetBar.ButtonItem BtnPre
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
        public DevComponents.DotNetBar.ButtonItem BtnNext
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

        public Dictionary<string, DevComponents.DotNetBar.ContextMenuBar> DicContextMenu
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
            get
            {
                try
                {
                    DevComponents.DotNetBar.Bar statusBar = this.StatusBar as DevComponents.DotNetBar.Bar;
                    return statusBar.Items["GisLabel"].Text;
                }
                catch
                {
                    return string.Empty;
                }
            }
            set
            {
                try
                {
                    DevComponents.DotNetBar.Bar statusBar = this.StatusBar as DevComponents.DotNetBar.Bar;
                    statusBar.Items["GisLabel"].Text = value;
                }
                catch
                {
                }
            }
        }

        public DevComponents.DotNetBar.ProgressBarItem ProgressBar
        {
            get
            {
                try
                {
                    DevComponents.DotNetBar.Bar statusBar = this.StatusBar as DevComponents.DotNetBar.Bar;
                    return statusBar.Items["GisprogressBarItem"] as DevComponents.DotNetBar.ProgressBarItem;
                }
                catch
                {
                    return null;
                }
            }
            set
            {
                try
                {
                    DevComponents.DotNetBar.Bar statusBar = this.StatusBar as DevComponents.DotNetBar.Bar;
                    statusBar.Items["GisprogressBarItem"] = value;
                }
                catch
                {
                }
            }
        }

        //�ο�������cmb
        public DevComponents.DotNetBar.ComboBoxItem RefScaleCmb
        {
            get
            {
                try
                {
                    DevComponents.DotNetBar.Bar statusBar = this.StatusBar as DevComponents.DotNetBar.Bar;
                    return statusBar.Items["RefScaleCmbItem"] as DevComponents.DotNetBar.ComboBoxItem;
                }
                catch
                {
                    return null;
                }
            }
            set
            {
                try
                {
                    DevComponents.DotNetBar.Bar statusBar = this.StatusBar as DevComponents.DotNetBar.Bar;
                    statusBar.Items["RefScaleCmbItem"] = value;
                }
                catch
                {
                }
            }
        }
        //��ǰ������cmb
        public DevComponents.DotNetBar.ComboBoxItem CurScaleCmb
        {
            get
            {
                try
                {
                    DevComponents.DotNetBar.Bar statusBar = this.StatusBar as DevComponents.DotNetBar.Bar;
                    return statusBar.Items["CurScaleCmbItem"] as DevComponents.DotNetBar.ComboBoxItem;
                }
                catch
                {
                    return null;
                }
            }
            set
            {
                try
                {
                    DevComponents.DotNetBar.Bar statusBar = this.StatusBar as DevComponents.DotNetBar.Bar;
                    statusBar.Items["CurScaleCmbItem"] = value;
                }
                catch
                {
                }
            }
        }
        //������ʾ�ı���
        public DevComponents.DotNetBar.TextBoxItem CoorTxt
        {
            get
            {
                try
                {
                    DevComponents.DotNetBar.Bar statusBar = this.StatusBar as DevComponents.DotNetBar.Bar;
                    return statusBar.Items["CoorTxtItem"] as DevComponents.DotNetBar.TextBoxItem;
                }
                catch
                {
                    return null;
                }
            }
            set
            {
                try
                {
                    DevComponents.DotNetBar.Bar statusBar = this.StatusBar as DevComponents.DotNetBar.Bar;
                    statusBar.Items["CoorTxtItem"] = value;
                }
                catch
                {
                }
            }
        }


        public string CoorXY
        {
            get
            {
                try
                {
                    DevComponents.DotNetBar.Bar statusBar = this.StatusBar as DevComponents.DotNetBar.Bar;
                    return statusBar.Items["GisCoorLabel"].Text;
                }
                catch
                {
                    return string.Empty;
                }
            }
            set
            {
                try
                {
                    DevComponents.DotNetBar.Bar statusBar = this.StatusBar as DevComponents.DotNetBar.Bar;
                    statusBar.Items["GisCoorLabel"].Text = value;
                }
                catch
                {
                }
            }
        }

        public string UserInfo
        {
            get
            {
                try
                {
                    DevComponents.DotNetBar.Bar statusBar = this.StatusBar as DevComponents.DotNetBar.Bar;
                    return statusBar.Items["GisUserInfoLabel"].Text;
                }
                catch
                {
                    return string.Empty;
                }
            }
            set
            {
                try
                {
                    DevComponents.DotNetBar.Bar statusBar = this.StatusBar as DevComponents.DotNetBar.Bar;
                    statusBar.Items["GisUserInfoLabel"].Text = value;
                }
                catch
                {
                }
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
        public List<DevComponents.DotNetBar.ComboBoxItem> ScaleBoxList
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

        public Dictionary<DevComponents.DotNetBar.RibbonTabItem, string> DicTabs
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
        private Dictionary<string, DevComponents.DotNetBar.ContextMenuBar> _DicContextMenu;           //�Ҽ��˵�����
        private string _ImageResPath;                                  // ͼƬ��Դ·��
        private IMapDocument _MapDocument;                            //�洢���ĵ�����
        private IMapControlDefault _MapControl;                      //MapControl�ؼ�
        private AxMapControl _AxMapControl;
        private ISceneControlDefault _SceneControlDefault;
        private AxSceneControl _SceneControl;
        private IPageLayoutControlDefault _PageLayoutControl;        //PageLayoutControl�ؼ�
        private ITOCControlDefault _TOCControl;                     //TOCControl�ؼ�
        private string _CurrentTool;                                   // ��ǰʹ�õ�TOOL����
        private DevComponents.AdvTree.AdvTree _ProjectTree;       // ������Ϣ��ͼ
        //private DevComponents.DotNetBar.Controls.DataGridViewX _UpdateDataGrid;   //���¶Աȱ��

        //private DevComponents.DotNetBar.Controls.DataGridViewX _DataCheckGrid;   //���ݼ����
        private XmlDocument _DBXmlDocument;                       //���ݹ�������XML�ļ�

        private Thread _CurrentThread;                           //�������ݵĽ���(Ψһ)

        private User _user;                                          //���ӵ��û���Ϣ
        //cyf 20110602 add
        private List<Role> _LstRoleInfo;                             //���ӵ��û���Ӧ�Ľ�ɫ��Ϣ
        //end

        //20110518
        private object _CurrentControl;                             //��ǰͼ����ʾ�ؼ� 
        private List<DevComponents.DotNetBar.ComboBoxItem> _scaleBoxList;                        //��������ʾ        
        private Dictionary<DevComponents.DotNetBar.RibbonTabItem, string> _dicTabs;          //��tab����ϵͳ����
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

            #region �Զ���״̬��
            DevComponents.DotNetBar.Bar statusBar = new DevComponents.DotNetBar.Bar();
            statusBar.Name = "SMPDStatusBar";
            statusBar.TabStop = false;
            statusBar.Stretch = true;
            statusBar.Style = DevComponents.DotNetBar.eDotNetBarStyle.Office2007;
            statusBar.AccessibleRole = System.Windows.Forms.AccessibleRole.StatusBar;
            statusBar.AntiAlias = true;
            statusBar.BarType = DevComponents.DotNetBar.eBarType.StatusBar;
            statusBar.Dock = System.Windows.Forms.DockStyle.Bottom;
            statusBar.GrabHandleStyle = DevComponents.DotNetBar.eGrabHandleStyle.ResizeHandle;
            statusBar.Dock = System.Windows.Forms.DockStyle.Bottom;

            //������ʾ����
            DevComponents.DotNetBar.LabelItem aLabelItem = new DevComponents.DotNetBar.LabelItem();
            aLabelItem.Name = "SMPDLabel";
            aLabelItem.Stretch = true;
            aLabelItem.PaddingLeft = 2;
            aLabelItem.PaddingRight = 5;
            aLabelItem.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));

            //������
            DevComponents.DotNetBar.ProgressBarItem progressBarItem = new DevComponents.DotNetBar.ProgressBarItem();
            progressBarItem.ChunkGradientAngle = 0F;
            progressBarItem.MenuVisibility = DevComponents.DotNetBar.eMenuVisibility.VisibleAlways;
            progressBarItem.Name = "SMPDprogressBarItem";
            progressBarItem.RecentlyUsed = false;
            progressBarItem.Stretch = true;
            progressBarItem.Visible = false;
            progressBarItem.ColorTable = DevComponents.DotNetBar.eProgressBarItemColor.Paused;

            //�ο�������������
            DevComponents.DotNetBar.LabelItem RefLabelItem = new DevComponents.DotNetBar.LabelItem();
            RefLabelItem.Visible = true;
            //added by chulili 20110722Ϊ�ο������߿ؼ���������
            RefLabelItem.Name = "RefScaleLabel";
            //end added by chulili
            RefLabelItem.Text = "�ο�������:";//"ReferenceScale:";
            RefLabelItem.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));

            //�ο�������������
            DevComponents.DotNetBar.ComboBoxItem RefScaleCmb = new DevComponents.DotNetBar.ComboBoxItem();
            RefScaleCmb.Name = "RefScaleCmbItem";
            RefScaleCmb.Visible = true;
            RefScaleCmb.Enabled = true;
            object[] objScale = null;
            int intWidth;
            ModScale.GetScaleConfig(out objScale,out intWidth );
            RefScaleCmb.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDown;
            RefScaleCmb.Items.AddRange(objScale);

            //��ǰ������������
            DevComponents.DotNetBar.LabelItem CurLabelItem = new DevComponents.DotNetBar.LabelItem();
            CurLabelItem.Visible = true;
            //added by chulili 20110722Ϊ��ǰ�����߿ؼ���������
            CurLabelItem.Name = "CurScaleLabel";
            //end added by chulili
            CurLabelItem.Text = "��ǰ������:";// "CurrentScale:";
            CurLabelItem.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));

            //��ǰ������������
            DevComponents.DotNetBar.ComboBoxItem CurScaleCmb = new DevComponents.DotNetBar.ComboBoxItem();
            CurScaleCmb.Name = "CurScaleCmbItem";
            CurScaleCmb.Visible = true;
            CurScaleCmb.Enabled = true;
            CurScaleCmb.ComboWidth = intWidth;
            CurScaleCmb.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDown;
            CurScaleCmb.Items.AddRange(objScale);

            //�����ı���
            DevComponents.DotNetBar.TextBoxItem CoorTxt = new DevComponents.DotNetBar.TextBoxItem();
            CoorTxt.Name = "CoorTxtItem";
            CoorTxt.Visible = true;
            CoorTxt.Enabled = true;

            //ͼ�ϵ��������� added by chulili 20110722
            DevComponents.DotNetBar.LabelItem aCoorLabelItem = new DevComponents.DotNetBar.LabelItem();
            aCoorLabelItem.Name = "GisCoorLabel";
            aCoorLabelItem.Stretch = true;
            aCoorLabelItem.PaddingLeft = 2;
            aCoorLabelItem.PaddingRight = 2;
            aCoorLabelItem.TextAlignment = System.Drawing.StringAlignment.Far;
            aCoorLabelItem.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            //end added by chulili 

            //�û���Ϣ����
            DevComponents.DotNetBar.LabelItem aUserInfoLabelItem = new DevComponents.DotNetBar.LabelItem();
            aUserInfoLabelItem.Name = "SMPDGisUserInfoLabel";
            aUserInfoLabelItem.Stretch = true;
            aUserInfoLabelItem.PaddingLeft = 10;//changed by chulili 20110722 ��߾������ô�һ�㣬ʹ��ǰ�ؼ�������
            aUserInfoLabelItem.PaddingRight = 2;
            aUserInfoLabelItem.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));

            //changed by chulili 20110722 ��״̬������ӿؼ�����Ŀ¼����ʱ����ʾX\Y����
            //statusBar.Items.AddRange(new DevComponents.DotNetBar.BaseItem[] { aLabelItem, progressBarItem, RefLabelItem, RefScaleCmb, CurLabelItem, CurScaleCmb });
            statusBar.Items.AddRange(new DevComponents.DotNetBar.BaseItem[] { aLabelItem, aUserInfoLabelItem, progressBarItem, aCoorLabelItem, RefLabelItem, RefScaleCmb, CurLabelItem, CurScaleCmb });
            this.StatusBar = statusBar;
            #endregion
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

        public Dictionary<string, DevComponents.DotNetBar.ContextMenuBar> DicContextMenu
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

        public DevComponents.AdvTree.AdvTree ProjectTree
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
            get
            {
                try
                {
                    DevComponents.DotNetBar.Bar statusBar = this.StatusBar as DevComponents.DotNetBar.Bar;
                    return statusBar.Items["SMPDLabel"].Text;
                }
                catch
                {
                    return string.Empty;
                }
            }
            set
            {
                try
                {
                    DevComponents.DotNetBar.Bar statusBar = this.StatusBar as DevComponents.DotNetBar.Bar;
                    statusBar.Items["SMPDLabel"].Text = value;
                }
                catch
                {
                }
            }
        }

        public DevComponents.DotNetBar.ProgressBarItem ProgressBar
        {
            get
            {
                try
                {
                    DevComponents.DotNetBar.Bar statusBar = this.StatusBar as DevComponents.DotNetBar.Bar;
                    return statusBar.Items["SMPDprogressBarItem"] as DevComponents.DotNetBar.ProgressBarItem;
                }
                catch
                {
                    return null;
                }
            }
            set
            {
                try
                {
                    DevComponents.DotNetBar.Bar statusBar = this.StatusBar as DevComponents.DotNetBar.Bar;
                    statusBar.Items["SMPDprogressBarItem"] = value;
                }
                catch
                {
                }
            }
        }
        //�ο��������Ƿ�ɼ�
        public bool RefScaleVisible
        {
            get
            {
                DevComponents.DotNetBar.Bar statusBar = this.StatusBar as DevComponents.DotNetBar.Bar;
                return statusBar.Items["RefScaleLabel"].Visible;
            }
            set
            {
                DevComponents.DotNetBar.Bar statusBar = this.StatusBar as DevComponents.DotNetBar.Bar;
                statusBar.Items["RefScaleCmbItem"].Visible = value;
                statusBar.Items["RefScaleLabel"].Visible = value;
            }
        }
        //��ǰ�������Ƿ�ɼ�
        public bool CurScaleVisible
        {
            get
            {
                DevComponents.DotNetBar.Bar statusBar = this.StatusBar as DevComponents.DotNetBar.Bar;
                return statusBar.Items["CurScaleLabel"].Visible;
            }
            set
            {
                DevComponents.DotNetBar.Bar statusBar = this.StatusBar as DevComponents.DotNetBar.Bar;
                statusBar.Items["CurScaleCmbItem"].Visible = value;
                statusBar.Items["CurScaleLabel"].Visible = value;
            }
        }
        //�ο�������cmb
        public DevComponents.DotNetBar.ComboBoxItem RefScaleCmb
        {
            get
            {
                try
                {
                    DevComponents.DotNetBar.Bar statusBar = this.StatusBar as DevComponents.DotNetBar.Bar;
                    return statusBar.Items["RefScaleCmbItem"] as DevComponents.DotNetBar.ComboBoxItem;
                }
                catch
                {
                    return null;
                }
            }
            set
            {
                try
                {
                    DevComponents.DotNetBar.Bar statusBar = this.StatusBar as DevComponents.DotNetBar.Bar;
                    statusBar.Items["RefScaleCmbItem"] = value;
                }
                catch
                {
                }
            }
        }
        //��ǰ������cmb
        public DevComponents.DotNetBar.ComboBoxItem CurScaleCmb
        {
            get
            {
                try
                {
                    DevComponents.DotNetBar.Bar statusBar = this.StatusBar as DevComponents.DotNetBar.Bar;
                    return statusBar.Items["CurScaleCmbItem"] as DevComponents.DotNetBar.ComboBoxItem;
                }
                catch
                {
                    return null;
                }
            }
            set
            {
                try
                {
                    DevComponents.DotNetBar.Bar statusBar = this.StatusBar as DevComponents.DotNetBar.Bar;
                    statusBar.Items["CurScaleCmbItem"] = value;
                }
                catch
                {
                }
            }
        }
        //������ʾ�ı���
        public DevComponents.DotNetBar.TextBoxItem CoorTxt
        {
            get
            {
                try
                {
                    DevComponents.DotNetBar.Bar statusBar = this.StatusBar as DevComponents.DotNetBar.Bar;
                    return statusBar.Items["CoorTxtItem"] as DevComponents.DotNetBar.TextBoxItem;
                }
                catch
                {
                    return null;
                }
            }
            set
            {
                try
                {
                    DevComponents.DotNetBar.Bar statusBar = this.StatusBar as DevComponents.DotNetBar.Bar;
                    statusBar.Items["CoorTxtItem"] = value;
                }
                catch
                {
                }
            }
        }

        public string UserInfo
        {
            get
            {
                try
                {
                    DevComponents.DotNetBar.Bar statusBar = this.StatusBar as DevComponents.DotNetBar.Bar;
                    return statusBar.Items["SMPDGisUserInfoLabel"].Text;
                }
                catch
                {
                    return string.Empty;
                }
            }
            set
            {
                try
                {
                    DevComponents.DotNetBar.Bar statusBar = this.StatusBar as DevComponents.DotNetBar.Bar;
                    statusBar.Items["SMPDGisUserInfoLabel"].Text = value;
                }
                catch
                {
                }
            }
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
            get
            {
                try
                {
                    DevComponents.DotNetBar.Bar statusBar = this.StatusBar as DevComponents.DotNetBar.Bar;
                    return statusBar.Items["GisCoorLabel"].Text;
                }
                catch
                {
                    return string.Empty;
                }
            }
            set
            {
                try
                {
                    DevComponents.DotNetBar.Bar statusBar = this.StatusBar as DevComponents.DotNetBar.Bar;
                    statusBar.Items["GisCoorLabel"].Text = value;
                }
                catch
                {
                }
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
        public List<DevComponents.DotNetBar.ComboBoxItem> ScaleBoxList
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

        public Dictionary<DevComponents.DotNetBar.RibbonTabItem, string> DicTabs
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

        #region �����ݿ�combox guozheng 2010-10-8
        private DevComponents.DotNetBar.Controls.ComboBoxEx m_cmbEntiDB;////����������ݿ� 
        public DevComponents.DotNetBar.Controls.ComboBoxEx cmbEntiDB
        {
            get { return m_cmbEntiDB; }
            set { this.m_cmbEntiDB = value; }
        }
        private DevComponents.DotNetBar.Controls.ComboBoxEx m_cmbFileDB;////�ɹ��ļ����ݿ� 
        public DevComponents.DotNetBar.Controls.ComboBoxEx cmbFileDB
        {
            get { return m_cmbFileDB; }
            set { this.m_cmbFileDB = value; }
        }
        private DevComponents.DotNetBar.Controls.ComboBoxEx m_cmbAdressDB;////�������ݿ� 
        public DevComponents.DotNetBar.Controls.ComboBoxEx cmbAdressDB
        {
            get { return m_cmbAdressDB; }
            set { this.m_cmbAdressDB = value; }
        }
        private DevComponents.DotNetBar.Controls.ComboBoxEx m_cmbDemDB;////�߳����ݿ�
        public DevComponents.DotNetBar.Controls.ComboBoxEx cmbDemDB
        {
            get { return m_cmbDemDB; }
            set { this.m_cmbDemDB = value; }
        }
        private DevComponents.DotNetBar.Controls.ComboBoxEx m_cmbFeatureDB;////���Ҫ�����ݿ�
        public DevComponents.DotNetBar.Controls.ComboBoxEx cmbFeatureDB
        {
            get { return m_cmbFeatureDB; }
            set { this.m_cmbFeatureDB = value; }
        }
        private DevComponents.DotNetBar.Controls.ComboBoxEx m_cmbImageDB;////Ӱ�����ݿ�
        public DevComponents.DotNetBar.Controls.ComboBoxEx cmbImageDB
        {
            get { return m_cmbImageDB; }
            set { this.m_cmbImageDB = value; }
        }
        #endregion
    }
    #endregion
}
