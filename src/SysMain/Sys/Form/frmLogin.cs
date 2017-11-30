using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Xml;
using System.Text;
using System.Windows.Forms;
using SysCommon.Gis;
using SysCommon.Error;
using SysCommon.Authorize;
using System.IO;
using SysCommon;
using System.Collections;

namespace GDBM
{
    public partial class frmLogin :SysCommon.BaseForm
    {
        SysGisDB gisDb = null;                                  //���ݲ�����\
        enumWSType _curWsType=enumWSType.SDE;                               //��ǰ�����ռ�����
        bool _loginSuccess = false;                             //�жϵ�½�Ƿ�ɹ�
        private System.Collections.Hashtable _UserPasswordHash;//added by chulili ��¼�û�����Ĺ�ϣ��
        public bool LoginSuccess
        {
            get { return _loginSuccess; }
            set { _loginSuccess = value; }
        }

        public frmLogin(string type)
        {
            InitializeComponent();
            switch(type)
            {
                case "ORACLE":
                case "SQLSERVER":
                    _curWsType = enumWSType.SDE;
                    break;
                case "ACCESS":
                    _curWsType = enumWSType.PDB;
                    break;
                case "FILE":
                    _curWsType = enumWSType.GDB;
                    break;
            }
            try //zhangqi 2012-08-06
            {
                if (System.IO.File.Exists(Application.StartupPath + "\\..\\Res\\Pic\\ϵͳ��¼.jpg"))
                {
                    this.BackgroundImage = Image.FromFile(Application.StartupPath + "\\..\\Res\\Pic\\ϵͳ��¼.jpg");
                }
                else
                {
                    this.BackgroundImage = Image.FromFile(Application.StartupPath + "\\..\\Res\\Pic\\ϵͳ��¼.png");
                }
            }
            catch { }
        }

        public frmLogin()
        {
            InitializeComponent();
            if (!IsConnectFileExist())
            {
                //show config form
            }
        }
        /// <summary>             
        /// Get The Connect File
        /// </summary>
        /// <returns></returns>
        private bool IsConnectFileExist()
        {
            if (!System.IO.File.Exists(SysCommon.ModuleConfig.m_ConnectFileName))
            {
                return false;
            }
            else
            {
                //Read The Connect Info from the config file

            }
            return false;
        }
        private void buttonX1_Click(object sender, EventArgs e)
        {
            Exception eError;
            if (string.IsNullOrEmpty(this.comboBoxUser.Text) || string.IsNullOrEmpty(this.textBoxXPassword.Text))
            {
                ErrorHandle.ShowInform("��ʾ", "�û�/���벻��Ϊ�գ�");
                return;
            }

            if (ModuleOperator.CheckLogin(this.comboBoxUser.Text.Trim(), this.textBoxXPassword.Text.Trim(), ref gisDb, _curWsType, out eError))
            {

                NotePassword();

                _loginSuccess = true;
                Plugin.LogTable.user = this.comboBoxUser.Text.Trim();
                Plugin.LogTable.Writelog("��¼ϵͳ");
                WriteUserName(this.comboBoxUser.Text.Trim());
                this.DialogResult = DialogResult.OK;
            }
            else
            {
                _loginSuccess = false;
                if (eError != null)
                {
                    ErrorHandle.ShowInform("��ʾ", eError.Message);
                }
                else
                {
                    ErrorHandle.ShowInform("��ʾ", "�û���/�������");
                }
            }
        }
        //�Ƴ���ǰ�û�������
        private void RemovePassword()
        {
            if (_UserPasswordHash == null)
                return;
            string strUser = this.comboBoxUser.Text.Trim();
            foreach (DictionaryEntry de in _UserPasswordHash)
            {
                string strKey = de.Key.ToString();
                string strValue = de.Value.ToString();
                if (strKey.Equals(strUser))
                {
                    _UserPasswordHash.Remove(strKey);
                    break;
                }
            }
            SysCommon.Authorize.AuthorizeClass.Serialize(_UserPasswordHash, Mod.v_UserInfoPath);
        }
        //��ס��ǰ�û�������
        private void NotePassword()
        {
            if (_UserPasswordHash == null)
            {
                _UserPasswordHash = new System.Collections.Hashtable();
            }
            string strUser = this.comboBoxUser.Text.Trim();
            string strPassword = "";
            if (this.checkBoxNotPassWord.Checked)
            {
                strPassword = this.textBoxXPassword.Text.Trim();
            }
            bool isadd = false;
            foreach (DictionaryEntry de in _UserPasswordHash)
            {
                string strKey = de.Key.ToString();
                string strValue = de.Value.ToString();
                if (strKey.Equals(strUser))
                {
                    _UserPasswordHash[strKey] = strPassword;
                    isadd = true;
                    break;
                }                
            }
            if (!isadd) _UserPasswordHash.Add(strUser, strPassword);
            SysCommon.Authorize.AuthorizeClass.Serialize(_UserPasswordHash, Mod.v_UserInfoPath );
        }
        private void WriteUserName(string strName)
        {
            string strFile=Application.StartupPath + "\\User.txt";
            if (!File.Exists(strFile))
            {
                FileStream fs = File.Create(strFile);
                fs.Close();
            }

            StreamWriter sw = new StreamWriter(strFile);
            sw.Write(strName);
            sw.Close();
        }

        private string GetUserName()
        {
            string strName = "";
            string strFile=Application.StartupPath + "\\User.txt";
            if (File.Exists(strFile))
            {
                StreamReader sr = new StreamReader(strFile);
                strName = sr.ReadLine();
                sr.Close();
            }

            return strName;
        }

        private void buttonX3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmLogin_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (gisDb != null)
            {
                gisDb.CloseWorkspace(true);
            }
        }
        //��ʼ���û����б����������û����������Ǽ�ס������û�����
        private void InitComboxUser()
        {
            System.Collections.Hashtable conset = conset = new System.Collections.Hashtable();
            if (File.Exists(Mod.v_UserInfoPath) == true)
            {
                SysCommon.Authorize.AuthorizeClass.Deserialize(ref conset, Mod.v_UserInfoPath);
                _UserPasswordHash = conset;
                foreach (DictionaryEntry de in conset)
                {
                    string strKey = de.Key.ToString();
                    string strValue = de.Value.ToString();
                    this.comboBoxUser.Properties.Items.Add(strKey);
                }
            }
        }

        private void frmLogin_Load(object sender, EventArgs e)
        {
            InitComboxUser();
            this.comboBoxUser.Text = GetUserName();            
        }

        private void frmLogin_Activated(object sender, EventArgs e)
        {
            if (this.comboBoxUser.Text != "")
            {
                this.textBoxXPassword.Focus();
            }
        }

        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            const int WM_NCHITTEST = 0x84;
            const int HTCLIENT = 0x01;
            const int HTCAPTION = 0x02;
            const int WM_NCLBUTTONDBLCLK = 0xA3;
            switch (m.Msg)
            {
                case 0x4e:
                case 0xd:
                case 0xe:
                case 0x14:
                    base.WndProc(ref m);
                    break;
                case WM_NCHITTEST://��������λ�ú�����϶�����
                    this.DefWndProc(ref m);
                    if (m.Result.ToInt32() == HTCLIENT)
                    {
                        m.Result = new IntPtr(HTCAPTION);
                        return;
                    }
                    break;
                case WM_NCLBUTTONDBLCLK://��ֹ˫�����
                    Console.WriteLine(this.WindowState);
                    return;
                default:
                    base.WndProc(ref m);
                    break;
            }
        }
        //���������иı���ѡ�û�
        private void comboBoxUser_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_UserPasswordHash == null)
                return;
            string strUser = this.comboBoxUser.Text.Trim();

            foreach (DictionaryEntry de in _UserPasswordHash)
            {
                string strKey = de.Key.ToString();
                string strValue = de.Value.ToString();
                if (strKey.Equals(strUser))
                {
                    this.textBoxXPassword.Text = strValue;
                    if (!strValue.Equals(""))
                    {
                        this.checkBoxNotPassWord.Checked = true;
                    }
                    else
                    {
                        this.checkBoxNotPassWord.Checked = false;
                    }
                    return;
                }
            }
            this.checkBoxNotPassWord.Checked = false;
        }

    }
}