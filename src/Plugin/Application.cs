using System;
using System.Windows.Forms;
using Fan.Common;
using Fan.Plugin.Parse;
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
        /// ������
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
        PluginCollection ColParsePlugin { get; }
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
        private PluginCollection _ColParsePlugin;      
        public AppForm()
        {

        }
        public AppForm(BaseRibbonForm MainForm, PluginCollection ColParsePlugin):this()
        {
            _MainForm = MainForm;
            _ColParsePlugin = ColParsePlugin;
        }
        #region AppForm ��Ա
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
        }
        public User ConnUser
        {
            get;
        }
        #endregion
    }
    #endregion
}
