using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Threading;
namespace Fan.DataManagerView
{
    public class ControlCenterFrame : Plugin.Interface.ControlRefBase
    {
        private Plugin.Application.IAppFormRef _hook;
        //private SetControl _ControlSet;

        //���캯��
        public ControlCenterFrame()
        {
            base._Name = "GeoDataManagerFrame.ControlCenterFrame";
            base._Caption = "�������Ĺ���ϵͳ";
            base._Visible = false;
            base._Enabled = false;
        }
        public override bool Visible
          {
            get
            {
                try
                {
                    if (_hook != null)
                    {
                        //if (_hook.CurrentSysName != base._Name)
                        //{
                        //    base._Visible = false;
                        //    //_ControlSet.Visible = false;
                        //    //ModFrameData.v_AppGisUpdate.StatusBar.Visible = false;
                        //    return false;
                        //}

                        //base._Visible = true;
                        //_ControlSet.Visible = true;
                        //ModFrameData.v_AppGisUpdate.StatusBar.Visible = true;
                        return true;
                    }
                    else
                    {
                        base._Visible = false;
                        return false;
                    }
                }
                catch
                {
                    base._Visible = false;
                    return false;
                }
            }
        }

        public override bool Enabled
        {
            get
            {
                try
                {
                    if (_hook == null)
                    {
                        base._Enabled = false;
                        return false;
                    }
                    //if (_hook.CurrentSysName != base._Name)
                    //{
                    //    base._Enabled = false;
                    //    //_ControlSet.Enabled = false;
                    //    //ModFrameData.v_AppGisUpdate.StatusBar.Enabled = false;
                    //    return false;
                    //}

                    base._Enabled = true;
                    //_ControlSet.Enabled = true;
                    //ModFrameData.v_AppGisUpdate.StatusBar.Enabled = true;
                    return true;
                }
                catch
                {
                    base._Enabled = false;
                    return false;
                }
            }
        }

        public override void OnCreate(Plugin.Application.IApplicationRef hook)
        {
            _hook = hook as Plugin.Application.IAppFormRef;

            if (_hook == null)
                return;
            //plugin app.cs

            //(���壬����������ϵͳ�������ڵ㣬��������ͼxml�ڵ㣬����������Ϣxml�ڵ㣬ϵͳ������ϣ�)
            //ModFrameData.v_AppGisUpdate = new Plugin.Application.AppGidUpdate(_hook.MainForm, _hook.ControlContainer, _hook.ListUserPrivilegeID, 
                //_hook.SystemXml, _hook.DataTreeXml, _hook.DatabaseInfoXml, _hook.ColParsePlugin, _hook.ImageResPath, _hook.ConnUser);
            //ModFrameData.v_AppGisUpdate.MyDocument = new System.Drawing.Printing.PrintDocument();
      
            //_ControlSet = new SetControl(this.Name, this.Caption);

            //_hook.MainForm.Controls.Add(_ControlSet);
            //_hook.MainForm.Controls.Add(ModFrameData.v_AppGisUpdate.StatusBar);  //������ܵײ�״̬��
            //20110518 ��ǰ��¼�û�
            //ModFrameData.v_AppGisUpdate.UserInfo = "��ǰ��½: " + _hook.ConnUser.TrueName;  //�ڵײ�״̬������ʾ��¼��Ϣ
            //_hook.MainForm.FormClosing += new FormClosingEventHandler(MainForm_FormClosing);
            //20110518  cyf  ���
  /*          _ControlSet.EnabledChanged += new EventHandler(_ControlSet_EnabledChanged); */ // Enable�¼��������������ݿ⹤����ͼ����ĳ�ʼ��

        }

        //���˳�ϵͳǰ�����ڴ�������Ӧ��ʾ
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
           //Plugin.Application.IAppGisUpdateRef pApp = ModFrameData.v_AppGisUpdate as Plugin.Application.IAppGisUpdateRef;
            //if (pApp == null) return;
            //if (pApp.CurrentThread != null)
            //{
            //    if (pApp.CurrentThread.ThreadState != ThreadState.Stopped)
            //    {
            //        pApp.CurrentThread.Suspend();
            //        if (SysCommon.Error.ErrorHandle.ShowFrmInformation("ȷ��", "ȡ��", "��ǰ�������ڽ���,�Ƿ���ֹ�˳�?") == true)
            //        {
            //            pApp.CurrentThread.Resume();
            //            pApp.CurrentThread.Abort();
            //        }
            //        else
            //        {
            //            pApp.CurrentThread.Resume();
            //            e.Cancel = true;
            //        }
            //    }
            //}
        }

        private void _ControlSet_EnabledChanged(object sender, EventArgs e)
        {
            //if (_ControlSet.Enabled)
            //{
            //}
            //else
            //{
               
            //}
        }

    }
}
