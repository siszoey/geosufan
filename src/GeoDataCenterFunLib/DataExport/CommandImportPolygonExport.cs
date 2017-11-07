using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.DataSourcesFile;
using ESRI.ArcGIS.esriSystem;

namespace GeoDataCenterFunLib
{
    public class CommandImportPolygonExport : Plugin.Interface.CommandRefBase
    {
        private Plugin.Application.IAppGisUpdateRef _AppHk;
        private Plugin.Application.IAppFormRef m_pAppForm;
        private ESRI.ArcGIS.SystemUI.ICommand m_pCommand;
        //Ϊ���Ƶ���ķ�Χ
        private IScreenDisplay m_pScreenDisplay;
        private IActiveViewEvents_Event m_pActiveViewEvents;
        private IActiveView m_pActiveView = null;
        private IPolygon m_Polygon;
        //Ϊ���Ƶ���ķ�Χ
        GeoDataExport.frmExport frm = null;
        public CommandImportPolygonExport()
        {
            base._Name = "GeoDataCenterFunLib.CommandImportPolygonExport";
            base._Caption = "���뷶Χ��ȡ";
            base._Tooltip = "���뷶Χ��ȡ";
            base._Checked = false;
            base._Visible = true;
            base._Enabled = false;
            base._Message = "���뷶Χ��ȡ";
        }

        /// <summary>
        /// ͼ���д�������ʱ����״̬Ϊ����ʱ�ſ���
        /// </summary>
        public override bool Enabled
        {
            
            get
            {
                try
                {
                    if (_AppHk.CurrentControl is ESRI.ArcGIS.Controls.ISceneControl) return false;
                    if (_AppHk.MapControl.LayerCount == 0)
                    {
                        base._Enabled = false;
                        return false;
                    }

                    base._Enabled = true;
                    return true;
                }
                catch
                {
                    base._Enabled = false;
                    return false;
                }
            }
        }

        public override string Message
        {
            get
            {
                Plugin.Application.IAppFormRef pAppFormRef = _AppHk as Plugin.Application.IAppFormRef;
                if (pAppFormRef != null)
                {
                    pAppFormRef.OperatorTips = base._Message;
                }
                return base._Message;
            }
        }

        public override void ClearMessage()
        {
            Plugin.Application.IAppFormRef pAppFormRef = _AppHk as Plugin.Application.IAppFormRef;
            if (pAppFormRef != null)
            {
                pAppFormRef.OperatorTips = string.Empty;
            }
        }

        #region
        
        #endregion


        //20110921 XISHENG  ������txt��Χ�ĸĳɵ���MDB��shp
        public override void OnClick()
        {
            if (_AppHk == null) return;
            if (this.WriteLog)
            {
                Plugin.LogTable.Writelog(base._Caption);//xisheng ��־��¼
            }
            OpenFileDialog dlg = new OpenFileDialog();
            //dlg.Filter = "�������ݿ�(*.mdb)|*.mdb|shp����|*.shp|�ı��ļ�|*.txt";
            //dlg.Filter = "�������ݿ�(*.mdb)|*.mdb|shp����|*.shp|�ı��ļ�|*.txt";
            dlg.Filter = "shp����|*.shp|�������ݿ�(*.mdb)|*.mdb|�ļ����ݿ�(*.gdb)|gdb";
            if (dlg.ShowDialog() == DialogResult.Cancel)
                return;
            IPolygon pGon = new PolygonClass();
           pGon=SysCommon.ModPublicFun.GetPolyGonFromFile(dlg.FileName);
           if (pGon == null) return;
           ESRI.ArcGIS.Carto.IMap pMap = _AppHk.MapControl.ActiveView.FocusMap;
            IArea pArea=pGon as IArea;
            double area=pArea.Area;
            GetArea(ref area,pMap);
            double dArea = SysCommon.ModSysSetting.GetExportAreaOfUser(Plugin.ModuleCommon.TmpWorkSpace, m_pAppForm.ConnUser);

            if (dArea>=0&&area > dArea)
           {
               MessageBox.Show("������ȡ������", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
               return;
           }
            drawgeometryXOR(pGon, m_pScreenDisplay);
            ///ZQ  20111027 add  �ж������ֵ��Ƿ��ʼ��
            if (SysCommon.ModField._DicFieldName.Count == 0)
            {
                SysCommon.ModField.InitNameDic(Plugin.ModuleCommon.TmpWorkSpace, SysCommon.ModField._DicFieldName, "���Զ��ձ�");
            }
            frm = new GeoDataExport.frmExport(pMap, pGon as ESRI.ArcGIS.Geometry.IGeometry);
            frm.WriteLog = this.WriteLog; //ygc 2012-9-11 �����Ƿ�д��־
            frm.m_area = area;
            frm.FormClosed +=new FormClosedEventHandler(frm_FormClosed);
            frm.ShowDialog();
        }

        private void GetArea(ref double area, IMap pMap)
        {
            switch (pMap.MapUnits)
            {
                case esriUnits.esriKilometers:
                    area = (Math.Abs(area)) * 1000000;
                    break;
                case esriUnits.esriMeters:
                case esriUnits.esriUnknownUnits:
                    area = Math.Abs(area);
                    break;
                case esriUnits.esriDecimalDegrees:
                    //ת������Ǿ�γ�ȵĵ�ͼ xisheng 20110731
                    UnitConverter punitConverter = new UnitConverterClass();
                    area = punitConverter.ConvertUnits(Math.Abs(area), esriUnits.esriMeters, esriUnits.esriDecimalDegrees);
                    break;
                default:
                    area = 0;
                    break;
            }

        }


       

        //����ر�ʱ ˢ��ǰ��
        private void frm_FormClosed(object sender, FormClosedEventArgs e)
        {
            m_Polygon = null;
            m_pActiveView.PartialRefresh(esriViewDrawPhase.esriViewForeground, null, null);
        }
        public override void OnCreate(Plugin.Application.IApplicationRef hook)
        {
            if (hook == null) return;
            _AppHk = hook as Plugin.Application.IAppGisUpdateRef;
            if (_AppHk.MapControl == null) return;
            m_pAppForm = _AppHk as Plugin.Application.IAppFormRef;

            m_pActiveView = _AppHk.MapControl.Map as IActiveView;

            m_pActiveViewEvents = m_pActiveView as IActiveViewEvents_Event;
            m_pScreenDisplay = m_pActiveView.ScreenDisplay;
            try
            {
                m_pActiveViewEvents.AfterDraw += new IActiveViewEvents_AfterDrawEventHandler(m_pActiveViewEvents_AfterDraw);

            }
            catch
            {
            }
        }
        //��ȥ�ػ�
        private void m_pActiveViewEvents_AfterDraw(IDisplay Display, esriViewDrawPhase phase)
        {
            if (frm!=null && !frm.IsDisposed)
            {
                drawgeometryXOR(null, m_pScreenDisplay);
            }

        }
        //���Ƶ���ķ�Χ
        private void drawgeometryXOR(IPolygon pPolygon, IScreenDisplay pScreenDisplay)
        {
            ISimpleFillSymbol pFillSymbol = new SimpleFillSymbolClass();
            ISimpleLineSymbol pLineSymbol = new SimpleLineSymbolClass();

            try
            {
                //��ɫ����
                IRgbColor pRGBColor = new RgbColorClass();
                pRGBColor.UseWindowsDithering = false;
                ISymbol pSymbol = (ISymbol)pFillSymbol;
                pSymbol.ROP2 = esriRasterOpCode.esriROPNotXOrPen;

                pRGBColor.Red = 255;
                pRGBColor.Green = 170;
                pRGBColor.Blue = 0;
                pLineSymbol.Color = pRGBColor;

                pLineSymbol.Width = 0.8;
                pLineSymbol.Style = esriSimpleLineStyle.esriSLSSolid;
                pFillSymbol.Outline = pLineSymbol;

                pFillSymbol.Color = pRGBColor;
                pFillSymbol.Style = esriSimpleFillStyle.esriSFSDiagonalCross;

                pScreenDisplay.StartDrawing(m_pScreenDisplay.hDC, -1);  //esriScreenCache.esriNoScreenCache -1
                pScreenDisplay.SetSymbol(pSymbol);

                //�������ѻ����Ķ����
                if (pPolygon != null)
                {
                    pScreenDisplay.DrawPolygon(pPolygon);
                    m_Polygon = pPolygon;
                }
                //�����ѻ����Ķ����
                else
                {
                    if (m_Polygon != null)
                    {
                        pScreenDisplay.DrawPolygon(m_Polygon);
                    }
                }

                pScreenDisplay.FinishDrawing();
            }
            catch (Exception ex)
            {
                MessageBox.Show("���Ƶ��뷶Χ����:" + ex.Message, "��ʾ");
                pFillSymbol = null;
            }
        }
    }
}
