using System;
using System.Windows.Forms;
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
        BaseRibbonForm MainForm { get;}
        /// <summary>
        /// ��������
        /// </summary>
        string Name { get;}
        /// <summary>
        /// �������
        /// </summary>
        string Caption { get; }
        /// <summary>
        /// ϵͳ�������
        /// </summary>
        Parse.PluginCollection ColParsePlugin { get; set; }
        /// <summary>
        /// ��½���û�
        /// </summary>
        User ConnUser { get;  }
    }
    #endregion
    #region �ӿ�ʵ����
    public class AppForm : IAppFormRef
    {
        private BaseRibbonForm _MainForm;                                      
        private Parse.PluginCollection _ColParsePlugin;      
        public AppForm()
        {

        }
        public AppForm(BaseRibbonForm MainForm, Parse.PluginCollection ColParsePlugin):this()
        {
            _MainForm = MainForm;
            _ColParsePlugin = ColParsePlugin;
        }
        #region IDefAppForm ��Ա
        public BaseRibbonForm MainForm
        {
            get{return _MainForm;}
        }
        public string Name
        {
            get{return _MainForm.Name;}
        }
        public string Caption
        {
            get{return _MainForm.Text;}
        }
        public Parse.PluginCollection ColParsePlugin
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
        public User ConnUser
        {
            get;
        }
        #endregion
    }
    #endregion
}
