using System;
using System.Collections.Generic;
using System.Text;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geometry;
using System.Xml;
using System.Data;
using System.Windows.Forms;
using ESRI.ArcGIS.Display;

namespace GeoDBATool
{
    class ControlsDataJoinSearch : Plugin.Interface.CommandRefBase
    {
        private Plugin.Application.IAppGISRef m_Hook;
        private DataTable m_PolyLineSearchTable;
        private DataTable m_PolygonSearchTable;

        public ControlsDataJoinSearch()
        {
            base._Name = "GeoDBATool.ControlsDataJoinSetting";
            base._Caption = "�ӱ�Ҫ������";
            base._Tooltip = "";
            base._Visible = true;
            base._Enabled = true;
            base._Message = "�ӱ�Ҫ������";

        }

        public override bool Enabled
        {
            get
            {
                return true;
            }
        }

        public override string Message
        {
            get
            {
                Plugin.Application.IAppFormRef pAppFormRef = m_Hook as Plugin.Application.IAppFormRef;
                if (pAppFormRef != null)
                {
                    pAppFormRef.OperatorTips = base._Message;
                }
                return base._Message;
            }
        }

        public override void ClearMessage()
        {
            Plugin.Application.IAppFormRef pAppFormRef = m_Hook as Plugin.Application.IAppFormRef;
            if (pAppFormRef != null)
            {
                pAppFormRef.OperatorTips = string.Empty;
            }
        }

        public override void OnClick()
        {
            /*  ִ�нӱ�Ҫ�ص�����
             *  �������̣�
             *    1.��ȡ����������Ϣ��ͼ����ϱ��ͼ�㡢����ӱ�ͼ���б��ӱ߲�����������xml�ļ��л�ȡ����
             *    2.ʵ��IMapframe�ӿڣ�ͨ���ӿڻ�ȡ�ӱߵ�Դ��Ŀ�껺�����򡢽ӱ߽߱磻
             *    3.���ӱ�Ҫ�ش�ѡ��ʵ��IDestinatDataset�ӿڣ�ʹ�ã�2�����صĻ�������ͨ���ӿڻ�ȡ���ӱ�ԴҪ�ء�Ŀ��Ҫ�ص�OID���ϣ�
             *    4.���ӱ�Ҫ�ؾ�ѡ��ʵ��ICheckOperation�ӿڣ�ͨ���ӿ��ڲ��裨3����OID�������жϽӱ����������ƥ���������FieldsControlList����ֵ���򲻽������Կ��ƣ���
             *    5.���ӿ�ICheckOperation���ص���Ϣ��ӳ�������ϡ�
             */
            frmJoinFeaSearch FeaSearch = new frmJoinFeaSearch();
            List<string> JoinFeaClsName = null;
            Dictionary<string, List<string>> JoinField = new Dictionary<string, List<string>>();//////�ӱ߿����ֶ�
            string MapFrameName = string.Empty;
            string MapFrameField = "";
            if (System.Windows.Forms.DialogResult.OK == FeaSearch.ShowDialog())
            {
                JoinFeaClsName = FeaSearch.JoinLayerName;
                JoinField = FeaSearch.FieldDic;
                MapFrameName = FeaSearch.MapFrameName;
                MapFrameField = FeaSearch.MapFrameField;
                IFeatureClass MapFrameFeaClss = null;
                #region ��ȡͼ����Χ
                int layercount = m_Hook.ArcGisMapControl.LayerCount;
                for (int i = 0; i < layercount; i++)
                {
                    ILayer getlayer = m_Hook.ArcGisMapControl.get_Layer(i);
                    if (getlayer.Name == MapFrameName)
                    {
                        IFeatureLayer FeaLayer = getlayer as IFeatureLayer;
                        MapFrameFeaClss = FeaLayer.FeatureClass;
                    }
                }
                #endregion
                List<IFeatureClass> JoinFeaCls = new List<IFeatureClass>();
                m_Hook.PolylineSearchGrid.Tag = JoinFeaCls;//////�����ӱߵ�ͼ���б����PolylineSearchGrid.Tag�Ϲ�ʹ��
                #region ��ȡ�ӱ�ͼ���б�
                layercount = m_Hook.ArcGisMapControl.LayerCount;
                for (int i = 0; i < layercount; i++)
                {
                    ILayer getlayer = m_Hook.ArcGisMapControl.get_Layer(i);
                    if (JoinFeaClsName.Contains(getlayer.Name))
                    {
                        IFeatureLayer FeaLayer = getlayer as IFeatureLayer;
                        IFeatureClass getFeaClss = FeaLayer.FeatureClass;
                        JoinFeaCls.Add(getFeaClss);
                    }
                }
                #endregion
                double dDisTo = -1;
                double dSeacherTo = -1;
                double dAngleTo = -1;
                double dLengthTo = -1;
                #region ��ȡ�ӱ߲���
                XmlDocument XmlDoc = new XmlDocument();
                XmlDoc.Load(ModData.v_JoinSettingXML);
                if (null == XmlDoc)
                {
                    SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "��ȡ�ӱ߲��������ļ�ʧ�ܣ�");
                    return;
                }
                XmlElement ele = XmlDoc.SelectSingleNode(".//�ӱ�����") as XmlElement;

                string sDisTo = ele.GetAttribute("�����ݲ�");
                string sSeacherTo = ele.GetAttribute("�����ݲ�");
                string sAngleTo = ele.GetAttribute("�Ƕ��ݲ�");
                string sLengthTo = ele.GetAttribute("�����ݲ�");
                string sJoinType = ele.GetAttribute("�ӱ�����");
                string sIsRemovePnt = ele.GetAttribute("ɾ������ζ����").Trim();
                string sIsSimplify = ele.GetAttribute("�򵥻�Ҫ��").Trim();
                try
                {
                    dDisTo = Convert.ToDouble(sDisTo);
                    dSeacherTo = Convert.ToDouble(sSeacherTo);
                    dAngleTo = Convert.ToDouble(sAngleTo);
                    dLengthTo = Convert.ToDouble(sLengthTo);
                }
                catch
                {
                    SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "�ӱ߲��������ļ��в�������ȷ��");
                    return;
                }
                #endregion
                if (MapFrameFeaClss == null)
                    return;
                int max = MapFrameFeaClss.FeatureCount(null);
                IFeatureCursor JoinFrameCur = MapFrameFeaClss.Search(null, false);
                IFeature MapFrameFea = JoinFrameCur.NextFeature();

                IMapframe pMapframe = null;//////////�ӱ�ͼ�����ʼ������׼ͼ���ӱ߻�Ǳ�׼ͼ���ӱߣ�
                if (sJoinType == "��׼ͼ��")
                    pMapframe = new ClsMapFrame();
                else
                    pMapframe = new ClsTaskFrame();
                pMapframe.MapFrameFea = MapFrameFeaClss;
                FrmProcessBar ProcessBar = new FrmProcessBar(max);
                ProcessBar.Show();
                ProcessBar.SetFrmProcessBarText( "���������ӱ�Ҫ��");
                long value = 0;
                this.m_PolyLineSearchTable.Rows.Clear();
                this.m_PolygonSearchTable.Rows.Clear();
                IDestinatDataset DesData = null;
                if (sJoinType == "��׼ͼ��")
                    DesData = new ClsDestinatDataset(true);/////��׼ͼ���ӱ�����
                else
                    DesData = new ClsDestinatDataset(false);/////�Ǳ�׼ͼ���ӱ�����
                /////////////////////////////////
                DesData.Angle_to = dAngleTo;
                if (sIsRemovePnt == "Y")//////ɾ��������϶���ĵ�
                    DesData.IsRemoveRedundantPnt = true;
                else
                    DesData.IsRemoveRedundantPnt = false;

                if (sIsSimplify == "Y")///////Ҫ�ؼ򵥻�������ж��geometry��Ҫ�أ�
                    DesData.IsGeometrySimplify = true;
                else
                    DesData.IsGeometrySimplify = false;
                //////////////////////////////////////
                ele = XmlDoc.SelectSingleNode(".//��־����") as XmlElement;
                string sLogPath = ele.GetAttribute("��־·��").Trim();



                DesData.JoinFeatureClass = JoinFeaCls;
                //////����ÿһ��ͼ�������ӱ�Ҫ��/////
                if (!string.IsNullOrEmpty(sLogPath))
                {
                    IJoinLOG JoinLog = new ClsJoinLog();
                    Exception ex = null;
                    JoinLog.onDataJoin_Start(0, out ex);
                }
                while (MapFrameFea != null)
                {
                    value += 1;
                    ProcessBar.SetFrmProcessBarValue(value);
                    Application.DoEvents();
                    int index = MapFrameFea.Fields.FindField(MapFrameField);
                    string No = string.Empty;
                    try
                    {
                        if (index > 0)
                            No = MapFrameFea.get_Value(index).ToString();
                    }
                    catch
                    {
                        No = string.Empty;
                    }
                    ProcessBar.SetFrmProcessBarText("���ڴ���ͼ����" + No);
                    Application.DoEvents();
                    pMapframe.OriMapFrame = MapFrameFea;

                    IGeometry OriArea = null;
                    IGeometry DesArea = null;
                    ProcessBar.SetFrmProcessBarText("���ڴ���ͼ����" + No + "�������ɻ�����");
                    Application.DoEvents();
                    try
                    {
                        pMapframe.GetBufferArea(dDisTo, out OriArea, out DesArea);
                    }
                    catch
                    {
                        SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ��", "���ɽӱ�����������ʧ�ܣ�\n����ͼ����Χͼ���Ƿ�������ȷ��");
                        ProcessBar.Close();
                        return;
                    }
                    ////////////////////////////////////
                    //IElement ele2 = null;
                    //IPolygonElement pPolElemnt = new PolygonElementClass();
                    //IFillShapeElement pFillShapeElement = (IFillShapeElement)pPolElemnt;
                    //pFillShapeElement.Symbol = GetDrawSymbol(0, 255, 0);
                    //ele2 = pFillShapeElement as IElement;
                    //ele2.Geometry = DesArea;
                    //IGraphicsContainer pMapGraphics = (IGraphicsContainer)m_Hook.ArcGisMapControl.Map;
                    //pMapGraphics.AddElement(ele2, 0);
                    //m_Hook.ArcGisMapControl.ActiveView.Refresh();

                    //IElement ele3 = null;
                    //pPolElemnt = new PolygonElementClass();
                    //pFillShapeElement = (IFillShapeElement)pPolElemnt;
                    //pFillShapeElement.Symbol = GetDrawSymbol(255, 0, 0);
                    //ele3 = pFillShapeElement as IElement;
                    //ele3.Geometry = OriArea;
                    ////IGraphicsContainer pMapGraphics = (IGraphicsContainer)this.axMapControl.Map;
                    //pMapGraphics.AddElement(ele3, 0);

                    /////////////////////////////////////
                    if (!string.IsNullOrEmpty(No))
                        ProcessBar.SetFrmProcessBarText("���ڴ���ͼ����" + No + ",���ڻ�ȡ�ӱ�Ҫ��");
                    else
                        ProcessBar.SetFrmProcessBarText("���ڻ�ȡ�ӱ�Ҫ��");
                    Application.DoEvents();
                    Dictionary<string, List<long>> OriOidDic = DesData.GetFeaturesByGeometry(OriArea, true);////�ӱ�ԴҪ�ؼ�¼
                    Dictionary<string, List<long>> DesOidDic = DesData.GetFeaturesByGeometry(DesArea, false);////�ӱ�Ŀ��Ҫ�ؼ�¼
                    ICheckOperation CheckOper = new ClsCheckOperationer();
                    CheckOper.Angel_Tolerrance = dAngleTo;/////////////�Ƕ��ݲ�
                    CheckOper.borderline = pMapframe.Getborderline();//�ӱ߽߱�
                    CheckOper.Dis_Tolerance = dDisTo;//////////////////�����ݲ�
                    CheckOper.Search_Tolerrance = dSeacherTo;//////////�����ݲ�
                    CheckOper.Length_Tolerrance = dLengthTo;///////////�����ݲ�
                    CheckOper.DesBufferArea = DesArea;/////////////////Ŀ������������
                    CheckOper.OriBufferArea = OriArea;/////////////////Դ����������

                    if (!string.IsNullOrEmpty(sLogPath))
                        CheckOper.CreatLog = true;
                    else
                        CheckOper.CreatLog = false;
                    if (null != OriOidDic)
                    {

                        foreach (KeyValuePair<string, List<long>> item in OriOidDic)
                        {
                            string OriFeaName = item.Key;
                            if (!string.IsNullOrEmpty(No))
                                ProcessBar.SetFrmProcessBarText("���ڴ���ͼ����" + No + ",��������ͼ�㣺" + OriFeaName);
                            else
                                ProcessBar.SetFrmProcessBarText("��������ͼ�㣺" + OriFeaName);
                            Application.DoEvents();
                            List<long> OriFeaOIDL = item.Value;
                            List<long> DesFeaOIDL = null;
                            if (DesOidDic == null) continue;
                            if (DesOidDic.ContainsKey(OriFeaName))
                            {
                                DesFeaOIDL = DesOidDic[OriFeaName];
                            }
                            if (null != OriFeaOIDL && null != DesFeaOIDL)
                            {
                                CheckOper.DesFeaturesOID = DesFeaOIDL;
                                CheckOper.OriFeaturesOID = OriFeaOIDL;
                                IFeatureClass JoinFea = DesData.TargetFeatureClass(OriFeaName);
                                if (null != JoinFeaCls)
                                {
                                    CheckOper.DestinatFeaCls = JoinFea;
                                    if (null != JoinField)
                                    {
                                        foreach (KeyValuePair<string, List<string>> getitem in JoinField)
                                        {
                                            if (getitem.Key == (JoinFea as IDataset).Name)
                                                CheckOper.FieldsControlList = getitem.Value;
                                        }
                                    }
                                    esriGeometryType GeoType = CheckOper.GetDatasetGeometryType();
                                    if (GeoType == esriGeometryType.esriGeometryPolyline)
                                    {
                                        if (!string.IsNullOrEmpty(No))
                                            ProcessBar.SetFrmProcessBarText("���ڴ���ͼ����" + No + ",��������ͼ�㣺" + OriFeaName + ",����������Ҫ�ؼ�¼");
                                        else
                                            ProcessBar.SetFrmProcessBarText("��������ͼ�㣺" + OriFeaName + ",����������Ҫ�ؼ�¼");
                                        Application.DoEvents();
                                        DataTable table = CheckOper.GetPolylineDesFeatureOIDByOriFeature();
                                        if (null != table)
                                        {
                                            for (int i = 0; i < table.Rows.Count; i++)
                                            {
                                                if (!string.IsNullOrEmpty(No))
                                                    ProcessBar.SetFrmProcessBarText("���ڴ���ͼ����" + No + "��������ͼ�㣺" + OriFeaName + "����������Ӽ�¼���б�");
                                                else
                                                    ProcessBar.SetFrmProcessBarText("��������ͼ�㣺" + OriFeaName + "����������Ӽ�¼���б�");
                                                Application.DoEvents();
                                                this.m_PolyLineSearchTable.Rows.Add(table.Rows[i].ItemArray);
                                            }

                                        }

                                    }
                                    if (GeoType == esriGeometryType.esriGeometryPolygon)
                                    {
                                        if (!string.IsNullOrEmpty(No))
                                            ProcessBar.SetFrmProcessBarText("���ڴ���ͼ����" + No + "��������ͼ�㣺" + OriFeaName + "�������������Ҫ�ؼ�¼");
                                        else
                                            ProcessBar.SetFrmProcessBarText("��������ͼ�㣺" + OriFeaName + "�������������Ҫ�ؼ�¼");
                                        Application.DoEvents();
                                        DataTable table = CheckOper.GetPolygonDesFeatureOIDByOriFeature();
                                        if (null != table)
                                        {
                                            for (int i = 0; i < table.Rows.Count; i++)
                                            {
                                                if (!string.IsNullOrEmpty(No))
                                                    ProcessBar.SetFrmProcessBarText("���ڴ���ͼ����" + No + "��������ͼ�㣺" + OriFeaName + "����������Ӽ�¼���б�");
                                                else
                                                    ProcessBar.SetFrmProcessBarText("��������ͼ�㣺" + OriFeaName + "����������Ӽ�¼���б�");
                                                Application.DoEvents();
                                                this.m_PolygonSearchTable.Rows.Add(table.Rows[i].ItemArray);
                                            }

                                        }
                                    }
                                }
                            }
                        }
                    }
                    MapFrameFea = JoinFrameCur.NextFeature();
                }
                if (!string.IsNullOrEmpty(sLogPath))
                {
                    IJoinLOG JoinLog = new ClsJoinLog();
                    Exception ex = null;
                    JoinLog.onDataJoin_Terminate(0, out ex);
                }
                m_Hook.PolygonSearchGrid.DataSource = null;
                m_Hook.PolylineSearchGrid.DataSource = null;
                m_Hook.PolygonSearchGrid.DataSource = this.m_PolygonSearchTable;
                m_Hook.PolylineSearchGrid.DataSource = this.m_PolyLineSearchTable;
                SelectALL(m_Hook.PolylineSearchGrid);
                SelectALL(m_Hook.PolygonSearchGrid);
                MessageBox.Show("�߼�¼������" + this.m_PolyLineSearchTable.Rows.Count + ";���¼������" + this.m_PolygonSearchTable.Rows.Count);
                ProcessBar.Close();
            }
        }

        /// <summary>
        /// ���б��е�һ��ѡ�� xisheng 20110829
        /// </summary>
        public static void SelectALL(DevComponents.DotNetBar.Controls.DataGridViewX GetDataGrid)
        {
            try
            {
                for (int i = 0; i < GetDataGrid.Rows.Count; i++)
                {
                    GetDataGrid.Rows[i].Cells[0].Value = true;
                }
                if (GetDataGrid.Columns.Contains("OriPtn"))
                {
                    GetDataGrid.Columns["OriPtn"].Visible = false;
                    GetDataGrid.Columns["DesPtn"].Visible = false;
                }
                if (GetDataGrid.Columns.Contains("OriLineIndex"))
                {
                    GetDataGrid.Columns["OriLineIndex"].Visible = false;
                    GetDataGrid.Columns["DesLineIndex"].Visible = false;
                }
            }
            catch { }
        }
        public override void OnCreate(Plugin.Application.IApplicationRef hook)
        {
            m_Hook = hook as Plugin.Application.IAppGISRef;
            if (m_Hook == null) return;
            /////////////////��ʼ�����ͽӱ�������
            m_PolyLineSearchTable = new DataTable();
            m_PolyLineSearchTable.TableName = "PolylineSearchTable";
            DataColumn dc1 = new DataColumn("���ݼ�", Type.GetType("System.String"));
            //DataColumn dc1 = new DataColumn("���ݼ�", Type.GetType("System.String"));
            DataColumn dc2 = new DataColumn("Ҫ������", Type.GetType("System.String"));
            //DataColumn dc2 = new DataColumn("Ҫ������", Type.GetType("System.String"));
            DataColumn dc3 = new DataColumn("ԴҪ��ID", Type.GetType("System.Int64"));
            //DataColumn dc3 = new DataColumn("ԴҪ��ID", Type.GetType("System.Int64"));
            dc3.DefaultValue = -1;
            DataColumn dc4 = new DataColumn("OriPtn", Type.GetType("System.String"));
            DataColumn dc5 = new DataColumn("Ŀ��Ҫ��ID", Type.GetType("System.Int64"));
            //DataColumn dc5 = new DataColumn("Ŀ��Ҫ��ID", Type.GetType("System.Int64"));
            dc5.DefaultValue = -1;
            DataColumn dc6 = new DataColumn("DesPtn", Type.GetType("System.String"));
            DataColumn dc7 = new DataColumn("�ӱ�״̬", Type.GetType("System.String"));
            m_PolyLineSearchTable.Columns.Add(dc1);
            m_PolyLineSearchTable.Columns.Add(dc2);
            m_PolyLineSearchTable.Columns.Add(dc3);
            m_PolyLineSearchTable.Columns.Add(dc4);
            m_PolyLineSearchTable.Columns.Add(dc5);
            m_PolyLineSearchTable.Columns.Add(dc6);
            m_PolyLineSearchTable.Columns.Add(dc7);
            //////////////////////////////////////
            ////��ʼ������νӱ�������
            m_PolygonSearchTable = new DataTable();
            m_PolygonSearchTable.TableName = "PolygonSearchTable";
            dc1 = new DataColumn("���ݼ�", Type.GetType("System.String"));
            dc2 = new DataColumn("Ҫ������", Type.GetType("System.String"));
            dc3 = new DataColumn("ԴҪ��ID", Type.GetType("System.Int64"));
            dc3.DefaultValue = -1;
            dc4 = new DataColumn("OriLineIndex", Type.GetType("System.Int64"));
            dc4.DefaultValue = -1;
            dc5 = new DataColumn("Ŀ��Ҫ��ID", Type.GetType("System.Int64"));
            dc5.DefaultValue = -1;
            dc6 = new DataColumn("DesLineIndex", Type.GetType("System.Int64"));
            dc6.DefaultValue = -1;
            dc7 = new DataColumn("�ӱ�״̬", Type.GetType("System.String"));
            m_PolygonSearchTable.Columns.Add(dc1);
            m_PolygonSearchTable.Columns.Add(dc2);
            m_PolygonSearchTable.Columns.Add(dc3);
            m_PolygonSearchTable.Columns.Add(dc4);
            m_PolygonSearchTable.Columns.Add(dc5);
            m_PolygonSearchTable.Columns.Add(dc6);
            m_PolygonSearchTable.Columns.Add(dc7);
            ////////////////////////////////////////////////////
        }
        public static ISimpleFillSymbol GetDrawSymbol(int intRed, int intGreen, int intBlue)
        {
            ISimpleFillSymbol pFillSymbol = new SimpleFillSymbolClass();
            ISimpleLineSymbol pLineSymbol = new SimpleLineSymbolClass();

            IRgbColor pRGBColor = new RgbColorClass();
            pRGBColor.UseWindowsDithering = false;

            ISymbol pSymbol = (ISymbol)pFillSymbol;
            pSymbol.ROP2 = esriRasterOpCode.esriROPNotXOrPen;

            pRGBColor.Red = intRed;
            pRGBColor.Green = intGreen;
            pRGBColor.Blue = intBlue;
            pLineSymbol.Color = pRGBColor;

            pLineSymbol.Width = 0.8;
            pLineSymbol.Style = esriSimpleLineStyle.esriSLSSolid;
            pFillSymbol.Outline = pLineSymbol;

            pFillSymbol.Color = pRGBColor;
            pFillSymbol.Style = esriSimpleFillStyle.esriSFSDiagonalCross;

            return pFillSymbol;
        }
    }
}
