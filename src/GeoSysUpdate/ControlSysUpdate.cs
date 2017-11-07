using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using SysCommon.Gis;
using SysCommon.Error;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Output;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.esriSystem;
using SysCommon;

namespace GeoSysUpdate
{
    public class ControlSysUpdate : Plugin.Interface.ControlRefBase
    {
        private Plugin.Application.IAppFormRef _hook;
        private UserControl _UserControl;
        private ITrackCancel m_TrackCancel;

        //���캯��
        public ControlSysUpdate()
        {
            base._Name = "GeoSysUpdate.ControlSysUpdate";
            base._Caption = "���ݹ���";
            base._Visible = false;
            base._Enabled = false;
        }

        public override bool Visible
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
                    if (_hook.CurrentSysName != base._Name)
                    {
                        base._Visible = false;
                        _UserControl.Visible = false;
                        ModDBOperator.WriteLog("v_AppGisUpdate.StatusBar.Visible false start");
                        ModData.v_AppGisUpdate.StatusBar.Visible = false;
                        ModDBOperator.WriteLog("v_AppGisUpdate.StatusBar.Visible false end");
                        return false;
                    }

                    base._Visible = true;
                    ModDBOperator.WriteLog("v_AppGisUpdate.StatusBar.Visible true start");
                    ModData.v_AppGisUpdate.StatusBar.Visible = true;
                    ModDBOperator.WriteLog("v_AppGisUpdate.StatusBar.Visible true end");
                    _UserControl.Visible = true;
                    
                    return true;
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
                    if (_hook.CurrentSysName != base._Name)
                    {
                        base._Enabled = false;
                        _UserControl.Enabled = false;
                        ModDBOperator.WriteLog("v_AppGisUpdate.StatusBar.Enabled false start");
                        ModData.v_AppGisUpdate.StatusBar.Enabled = false;
                        ModDBOperator.WriteLog("v_AppGisUpdate.StatusBar.Enabled false end");
                        return false;
                    }
                    if (!_UserControl.Enabled)
                    {
                        //changed by chulili20110718 �Ƚ�Ŀ¼������治�ɼ�������Ӧ�����л�
                        if (_hook.MainForm.Controls != null)
                        {
                            for (int i = 0; i < _hook.MainForm.Controls.Count; i++)
                            {
                                if (_hook.MainForm.Controls[i].Name.Equals("UCDataSourceManger"))
                                {
                                    _hook.MainForm.Controls[i].Enabled = false;
                                    break;
                                }
                            }
                        }
                        //end added by chulili 
                    }
                    base._Enabled = true;
                    ModDBOperator.WriteLog("v_AppGisUpdate.StatusBar.Enabled true start");
                    ModData.v_AppGisUpdate.StatusBar.Enabled = true;
                    ModDBOperator.WriteLog("v_AppGisUpdate.StatusBar.Enabled true end");
                    _UserControl.Enabled = true;
                    
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
            if (_hook == null) return;

            Plugin.Application.IAppFormRef pAppFormRef = _hook as Plugin.Application.IAppFormRef;
            ESRI.ArcGIS.Geodatabase.IWorkspace pWks = pAppFormRef.TempWksInfo.Wks;

            //�������ݸ�����ϵͳͬʱ�������ݿ�(����)
            if (ModData.v_SysDataSet == null)
            {
                //�������ݿ��������
                ModData.v_SysDataSet = new SysGisDataSet();
                ModData.v_SysDataSet.WorkSpace = pWks;
                ModData.Server = pAppFormRef.TempWksInfo.Server;
                ModData.Instance = pAppFormRef.TempWksInfo.Service;
                ModData.Database = pAppFormRef.TempWksInfo.DataBase;
                ModData.User = pAppFormRef.TempWksInfo.User;
                ModData.Password = pAppFormRef.TempWksInfo.PassWord;
                ModData.Version = pAppFormRef.TempWksInfo.Version;
                ModData.dbType = pAppFormRef.TempWksInfo.DBType;
            }
           
            //Ȩ�޿������
            ModData.v_AppGisUpdate = new Plugin.Application.AppGidUpdate(_hook.MainForm, _hook.ControlContainer,_hook.ListUserPrivilegeID,  _hook.SystemXml, _hook.DataTreeXml, _hook.DatabaseInfoXml, _hook.ColParsePlugin, _hook.ImageResPath,_hook.ConnUser);
            //ModData.v_AppGisUpdate = new Plugin.Application.AppGidUpdate(_hook.MainForm, _hook.ControlContainer, _hook.SystemXml, _hook.DataTreeXml, _hook.DatabaseInfoXml, _hook.ColParsePlugin, _hook.ImageResPath, _hook.ConnUser);
            ModData.v_AppGisUpdate.MyDocument = new System.Drawing.Printing.PrintDocument();
            ModData.v_AppGisUpdate.CurWksInfo = pAppFormRef.TempWksInfo;

            m_TrackCancel = new CancelTrackerClass();
            ModData.v_AppGisUpdate.MyDocument.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(MyDocument_PrintPage);
            _UserControl = new UserControlSMPD(this.Name, this.Caption);
            _hook.MainForm.Controls.Add(_UserControl);
            _hook.MainForm.Controls.Add(ModData.v_AppGisUpdate.StatusBar); 
            ModData.v_AppGisUpdate.UserInfo = "��ǰ��¼: " + _hook.ConnUser.TrueName;
            _hook.MainForm.FormClosing += new System.Windows.Forms.FormClosingEventHandler(MainForm_FormClosing);

            //cyf 20110615 add:��ӱ����ߴ����¼�
            ModData.v_AppGisUpdate.RefScaleCmb.SelectedIndexChanged += new EventHandler(RefScaleCmb_SelectedIndexChanged);
            ModData.v_AppGisUpdate.CurScaleCmb.SelectedIndexChanged += new EventHandler(CurScaleCmb_SelectedIndexChanged);

            //��ӻس��¼��Զ��������
            DevComponents.DotNetBar.Controls.ComboBoxEx vComboEx = ModData.v_AppGisUpdate.CurScaleCmb.ComboBoxEx;
            vComboEx.KeyDown += new KeyEventHandler(vComboEx_KeyDown);

        }
        //�������ݷ���
        public override void LoadData()
        {
            if (_UserControl != null)
            {
                UserControlSMPD pUC = _UserControl as UserControlSMPD;
                if (pUC != null)
                {
                    pUC.LoadData();
                }
            }

        }

        //��Ӧ�س�ʱ�� �ı䵱ǰ��ʾ������
        void vComboEx_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;

            DevComponents.DotNetBar.Controls.ComboBoxEx vComboEx = sender as DevComponents.DotNetBar.Controls.ComboBoxEx;
            string strScale = vComboEx.Text;
            double dblSacle = 0;

            try
            {
                if (double.TryParse(strScale, out dblSacle))
                {
                    ModData.v_AppGisUpdate.MapControl.Map.MapScale = dblSacle;
                    ModData.v_AppGisUpdate.MapControl.ActiveView.Refresh();
                }
                else
                {
                    vComboEx.Text = ModData.v_AppGisUpdate.MapControl.Map.MapScale.ToString();
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// ��ӡ�¼�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void MyDocument_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            short dpi = (short)e.Graphics.DpiX;
            IEnvelope devBounds = new EnvelopeClass();
            IPage page = ModData.v_AppGisUpdate.PageLayoutControl.Page;

            short printPageCount;
            printPageCount = ModData.v_AppGisUpdate.PageLayoutControl.get_PrinterPageCount(0);
            ModData.v_AppGisUpdate.CurrentPrintPage++;

            IPrinter printer = ModData.v_AppGisUpdate.PageLayoutControl.Printer;
            page.GetDeviceBounds(printer, ModData.v_AppGisUpdate.CurrentPrintPage, 0, dpi, devBounds);

            tagRECT deviceRect;
            //Returns the coordinates of lower, left and upper, right corners
            double xmin, ymin, xmax, ymax;
            devBounds.QueryCoords(out xmin, out ymin, out xmax, out ymax);
            //initialize the structure for the device boundaries
            deviceRect.bottom = (int)ymax;
            deviceRect.left = (int)xmin;
            deviceRect.top = (int)ymin;
            deviceRect.right = (int)xmax;

            //determine the visible bounds of the currently printed page
            IEnvelope visBounds = new EnvelopeClass();
            page.GetPageBounds(printer, ModData.v_AppGisUpdate.CurrentPrintPage, 0, visBounds);

            //get a handle to the graphics device that the print preview will be drawn to
            IntPtr hdc = e.Graphics.GetHdc();

            //print the page to the graphics device using the specified boundaries 
            ModData.v_AppGisUpdate.PageLayoutControl.ActiveView.Output(hdc.ToInt32(), dpi, ref deviceRect, visBounds, m_TrackCancel);

            //release the graphics device handle
            e.Graphics.ReleaseHdc(hdc);

            //check if further pages have to be printed
            if (ModData.v_AppGisUpdate.CurrentPrintPage < printPageCount)
                e.HasMorePages = true;
            else
                e.HasMorePages = false;
        }

        /// <summary>
        /// ���˳�ϵͳǰ�����ڴ�������Ӧ��ʾ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Plugin.Application.IAppGisUpdateRef pApp = ModData.v_AppGisUpdate as Plugin.Application.IAppGisUpdateRef;
            if (pApp == null) return;
            if (pApp.CurrentThread != null)
            {
                pApp.CurrentThread.Suspend();
                if (SysCommon.Error.ErrorHandle.ShowFrmInformation("ȷ��", "ȡ��", "��ǰ�������ڽ���,�Ƿ���ֹ�˳�?") == true)
                {
                    pApp.CurrentThread.Resume();
                    pApp.CurrentThread.Abort();
                }
                else
                {
                    pApp.CurrentThread.Resume();
                    e.Cancel = true;
                }
            }
        }


        //�ο��������¼������Ƿ����
        private void RefScaleCmb_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                ModData.v_AppGisUpdate.MapControl.Map.ReferenceScale = Convert.ToDouble(ModData.v_AppGisUpdate.RefScaleCmb.SelectedItem.ToString().Trim());
            }
            catch
            {
                ModData.v_AppGisUpdate.RefScaleCmb.Text = ModData.v_AppGisUpdate.MapControl.Map.ReferenceScale.ToString("0");
            }
            ModData.v_AppGisUpdate.MapControl.ActiveView.Refresh();
        }
        //��ǰ�������¼� ���Ƿ����
        private void CurScaleCmb_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                ModData.v_AppGisUpdate.MapControl.Map.MapScale = Convert.ToDouble(ModData.v_AppGisUpdate.CurScaleCmb.SelectedItem.ToString().Trim());
            }
            catch
            {
                ModData.v_AppGisUpdate.CurScaleCmb.Text = ModData.v_AppGisUpdate.MapControl.Map.MapScale.ToString("0");
            }
            ModData.v_AppGisUpdate.MapControl.ActiveView.Refresh();
        }
    }
}
