using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.IO;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geometry;

namespace GeoDBATool
{
    public partial class frmJoinFeaSearch : DevComponents.DotNetBar.Office2007Form
    {
        private double _dDisTo;
        private double _dSeacherTo;
        private double _dAngleTo;
        private double _dLengthTo;
        private XmlElement _ConnectInfo;
        private��SysCommon.Gis.SysGisDataSet pGisDT = null;        //////�ӱ�ͼ�����Ӷ���
        private��SysCommon.Gis.SysGisDataSet MapFramepGisDT = null;//////ͼ����ϱ����Ӷ���
        private Dictionary<string ,List<string>> m_DicFieldsConctrolList;/////�ӱ߿��������ֶ�
        private List<string> _JoinLayerName;/////////////////////����ӱ�ͼ�������б�
        public List<string> JoinLayerName
        {
            get { return this._JoinLayerName; }
        }
        private string _MapFrameName;///////////////////////////ͼ����ϱ�ͼ����
        public string MapFrameName
        {
            get { return this._MapFrameName; }
        }

        private string _MapFrameField=string.Empty;///////////////ͼ���ֶ�
        public string MapFrameField
        {
            get { return this._MapFrameField; }
        }

        private Dictionary<string ,List<string>> m_FieldDic;////////�ӱ߿��������ֶ�
        public Dictionary<string, List<string>> FieldDic
        {
            get { return this.m_FieldDic; }
        }

        public frmJoinFeaSearch()
        {
            InitializeComponent();
            this._JoinLayerName = null;
            this._MapFrameName = string.Empty;
            _JoinLayerName = new List<string>();
            _MapFrameName = string.Empty;
            rbServer.Checked = true;
            rbExistdata.Checked = true;
            this.list_JoinLayer.Items.Clear();
            this.com_Project.Items.Clear();
            this.com_DataBase.Items.Clear();
            ////////��ȡ������Ϣ
            //XmlDocument XmlDoc = new XmlDocument();
            //if (File.Exists(ModData.v_projectXML))
            //{
            //    XmlDoc.Load(ModData.v_projectXML);
            //    if (null == XmlDoc)
            //    {
            //        SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "��ȡ������Ϣʧ�ܣ�");
            //        return;
            //    }
            //    XmlNodeList ProjectNodeList = XmlDoc.SelectSingleNode(".//���̹���").ChildNodes;
            //    if (null != ProjectNodeList)
            //    {
            //        for (int i = 0; i < ProjectNodeList.Count; i++)
            //        {
            //            XmlElement nodeele = ProjectNodeList[i] as XmlElement;
            //            string projectName = nodeele.GetAttribute("����");
            //            this.com_Project.Items.Add(projectName);
            //        }
            //    }
            //}
            try
            {
                DevComponents.AdvTree.Node ProjectNode = ModData.v_AppGIS.DataTree.Nodes[0];
                string projectName = ProjectNode.Text;
                this.com_Project.Items.Add(projectName);
                this.com_DataBase.Items.Add("���ƿ�");
                this.com_DataBase.Items.Add("��ʷ��");
            }
            catch
            {
            }
           
            this.m_DicFieldsConctrolList = new Dictionary<string, List<string>>();
        }

        private void frmConSet_Load(object sender, EventArgs e)
        {
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
            double dDisTo = -1;
            double dSeacherTo = -1;
            double dAngleTo = -1;
            double dLengthTo = -1;
            try
            {
                dDisTo = Convert.ToDouble(sDisTo);
                dSeacherTo = Convert.ToDouble(sSeacherTo);
                dAngleTo = Convert.ToDouble(sAngleTo);
                dLengthTo = Convert.ToDouble(sLengthTo);
            }
            catch (Exception er)
            {
                //*******************************************************************
                //guozheng added
                if (ModData.SysLog != null)
                {
                    ModData.SysLog.Write(er, null, DateTime.Now);
                }
                else
                {
                    ModData.SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
                    ModData.SysLog.Write(er, null, DateTime.Now);
                }
                //********************************************************************
                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "�ӱ߲��������ļ��в�������ȷ��");
                return;
            }
            this._dAngleTo = dAngleTo;
            this._dDisTo = dDisTo;
            this._dLengthTo = dLengthTo;
            this._dSeacherTo = dSeacherTo;
            try
            {
                this.com_Project.Items.Clear();
                this.com_DataBase.Items.Clear();
                DevComponents.AdvTree.Node ProjectNode = ModData.v_AppGIS.ProjectTree.Nodes[0];
                string projectName = ProjectNode.Text;
                this.com_Project.Items.Add(projectName);
                this.com_DataBase.Items.Add("���ƿ�");
                this.com_DataBase.Items.Add("��ʷ��");
            }
            catch
            {
            }
        }

        private void btn_Connect_Click(object sender, EventArgs e)
        {
            Exception ex = null;
            GetConnectInfo(out ex);
            if (ex!=null)
            {
                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", ex.Message);
                return;
            }
            GetDataBaseLayer();
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
           //////�ӿ�����ѡ��ͼ����ϱ�
            if (rbExistdata.Checked == true)
            {
                this.com_MapNoField.Items.Clear();
                this.com_MapFrameList.Text = "";
                this.com_MapFrameList.Items.Clear();
                if (this.list_JoinLayer.Items.Count == 0)
                {
                    return;
                }
                for (int i = 0; i < this.list_JoinLayer.Items.Count; i++)
                {
                    this.com_MapFrameList.Items.Add(this.list_JoinLayer.Items[i].ToString());
                }
                MapFramepGisDT = pGisDT;
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            /////���ⲿmdb�ļ�ѡ��ͼ����ϱ�
            Exception ex = null;
            this.com_MapFrameList.Text = "";
            if (rbOutdata.Checked == true)
            {
                this.com_MapNoField.Items.Clear();
                this.com_MapFrameList.Text = "";
                this.com_MapFrameList.Items.Clear();
                OpenFileDialog openFile = new OpenFileDialog();
                openFile.Title = "ѡ������Χ";
                openFile.Filter = "ESRI�������ݿ�(*.mdb)|*.mdb";
                if (openFile.ShowDialog() != DialogResult.OK)
                {
                    rbExistdata.Checked = true;
                    return;
                }

                label_loadState.Visible = true;
                label_loadState.Text = "���ڻ�ȡͼ��";
                System.Windows.Forms.Application.DoEvents();
                string smdbPah = openFile.FileName;
                MapFramepGisDT = new SysCommon.Gis.SysGisDataSet();
                MapFramepGisDT.SetWorkspace(smdbPah, SysCommon.enumWSType.PDB, out ex);
                List<IDataset> LstDT = MapFramepGisDT.GetAllFeatureClass();
                List<ILayer> LstLayer = new List<ILayer>();
                for (int i = 0; i < LstDT.Count; i++)
                {
                    IFeatureClass pFeaCls = LstDT[i] as IFeatureClass;
                    if (pFeaCls.FeatureType == esriFeatureType.esriFTAnnotation) continue;
                    if (pFeaCls.ShapeType == esriGeometryType.esriGeometryPolygon)
                    {
                        //ֻ����ͼ��
                        IFeatureLayer pFeaLayer = new FeatureLayerClass();
                        pFeaLayer.FeatureClass = pFeaCls;
                        ILayer pLayer = pFeaLayer as ILayer;
                        if (!LstLayer.Contains(pLayer))
                        {
                            LstLayer.Add(pLayer);
                            label_loadState.Text = "�������ͼ�㣺" + pLayer.Name;
                            System.Windows.Forms.Application.DoEvents();
                        }
                    }
                }
                if (LstLayer.Count == 0)
                {
                    return;
                }
                //��ͼ���б�����Ӵ�ѡ��ͼ��

                for (int j = 0; j < LstLayer.Count; j++)
                {
                    string layername = ((LstLayer[j] as IFeatureLayer).FeatureClass as IDataset).Name;
                    this.com_MapFrameList.Items.Add(layername);
                }
                if (com_MapFrameList.Items.Count > 0) com_MapFrameList.SelectedIndex = 0;//Ĭ��ѡ���һ�� xisheng
                label_loadState.Visible = false;

            }
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void OK_Click(object sender, EventArgs e)
        {
            if (this.list_JoinLayer.Items.Count == 0)
            {
                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "û�нӱ�ͼ��!");
                return;
            }

            bool IsSelect = false;
            for (int i = 0; i < list_JoinLayer.Items.Count; i++)
            {
                CheckState ChecStste = list_JoinLayer.GetItemCheckState(i);
                if (ChecStste == CheckState.Checked)
                {
                    IsSelect = true;
                    break;
                }
            }
            if (!IsSelect)
            {
                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "û�нӱ�ͼ��!");
                return;
            }

            if (string.IsNullOrEmpty(this.com_MapFrameList.Text))
            {
                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "û��ѡ��ͼ����Χ���ݣ�");
                // this.DialogResult = DialogResult.Abort;
                return;
            }
            if (string.IsNullOrEmpty(this.com_MapNoField.Text))
            {
                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "û��ѡ��ͼ����Χ���ݵ�ͼ�����ֶΣ�");
                // this.DialogResult = DialogResult.Abort;
                return;
            }
            this._MapFrameField = this.com_MapNoField.Text.Trim();
            //////��ͼ����ӵ���ͼ�ؼ���
            Exception ex = null; 
            this._JoinLayerName = new List<string>();
            string Fname = this.com_MapFrameList.Text.Trim();
           
            //if (rbServer.Checked == true)/////�������е�ͼ����ӵ���ͼ�ؼ���
            //{
            //    #region ��ʾ�ӱ�ͼ��
            //    ModData.v_AppGIS.ArcGisMapControl.ClearLayers();
            //    for (int i = 0; i < this.list_JoinLayer.Items.Count; i++)
            //    {
            //        if (this.list_JoinLayer.GetItemChecked(i) == true)
            //        {
                        
            //            string layerName=this.list_JoinLayer.Items[i].ToString().Trim();                        
            //            IFeatureClass FeaCls = this.pGisDT.GetFeatureClass(layerName,out ex);
            //            if (null == ex)
            //            {
            //                IFeatureLayer Fealayer = new FeatureLayerClass();
            //                Fealayer.FeatureClass = FeaCls;
            //                Fealayer.Name = (FeaCls as IDataset).Name;
            //                this._JoinLayerName.Add(layerName);
            //                ModData.v_AppGIS.ArcGisMapControl.AddLayer(Fealayer as ILayer);
            //            }
            //        }
            //    }
            //    #endregion
            //    #region ��ʾͼ����Χͼ��

            //    if (this._JoinLayerName.Contains(Fname))
            //    {
            //        SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "�ӱ�ͼ���д�����ͼ����Χͼ��ͬ��ͼ�㣡");
            //        //this.DialogResult = DialogResult.Abort;
            //        return;
            //    }
            //    IFeatureClass MapFeaCls = null;
            //    if (rbExistdata.Checked == true)
            //    {
            //        MapFeaCls = this.pGisDT.GetFeatureClass(Fname, out ex);
            //    }
            //    else
            //    {
            //        MapFeaCls = this.MapFramepGisDT.GetFeatureClass(Fname, out ex);
            //    }
            //    if (null == ex)
            //    {
            //        IFeatureLayer Fealayer = new FeatureLayerClass();
            //        Fealayer.FeatureClass = MapFeaCls;
            //        Fealayer.Name = (MapFeaCls as IDataset).Name;
            //        ILayerEffects EffLayer = Fealayer as ILayerEffects;
            //        if (EffLayer.SupportsTransparency)
            //            EffLayer.Transparency = 30;
            //        ModData.v_AppGIS.ArcGisMapControl.AddLayer(Fealayer as ILayer, 0);
            //    }
           
            //    #endregion
            //}
            //else if (rbMapLayer.Checked == true)//////�ڵ�ͼ�ؼ��ϻ�ȡ�ӱ�ͼ��
            //{
                #region �ڵ�ͼ�ؼ��ϻ�ȡ�ӱ�ͼ��
                this._JoinLayerName = new List<string>();
                for (int i = 0; i < this.list_JoinLayer.Items.Count; i++)
                {
                    if (this.list_JoinLayer.GetItemChecked(i) == true)
                    {
                        string layerName = this.list_JoinLayer.Items[i].ToString().Trim();
                        this._JoinLayerName.Add(layerName);                       
                    }
                }
                #endregion
                #region ��ʾͼ����Χͼ��
                if (rbExistdata.Checked == true)
                {
                    this._MapFrameName = Fname;
                }
                else if (rbOutdata.Checked == true)
                {
                    for (int i = 0; i < this.list_JoinLayer.Items.Count;i++ )
                    {
                        if (this.list_JoinLayer.Items[i].ToString().Trim() == Fname)
                        {
                            SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "�ӱ�ͼ���д�����ͼ����Χͼ��ͬ��ͼ�㣡");
                            return;
                        }
                    }
                    IFeatureClass MapFeaCls = this.MapFramepGisDT.GetFeatureClass(Fname, out ex);
                    if (null == ex)
                    {
                        IFeatureLayer Fealayer = new FeatureLayerClass();
                        Fealayer.FeatureClass = MapFeaCls;
                        Fealayer.Name = (MapFeaCls as IDataset).Name;
                        ILayerEffects EffLayer = Fealayer as ILayerEffects;
                        if (EffLayer.SupportsTransparency)
                            EffLayer.Transparency = 30;
                        ModData.v_AppGIS.ArcGisMapControl.AddLayer(Fealayer as ILayer, 0);
                    }
                }


                #endregion
            //}
            this._MapFrameName = Fname;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        
        /// <summary>
        /// ͨ�����̻�ȡͼ���б�
        /// </summary>
        /// <param name="ex"></param>
        private void GetConnectInfo(out Exception ex)
        {
            ex = null;
            if (string.IsNullOrEmpty(this.com_Project.Text))
            {
                //SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "��ѡ��һ�����̣�");
                return;
            }
            if (string.IsNullOrEmpty(this.com_DataBase.Text))
            {
                //SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "��ѡ��һ������!");
                return;
            }
            string ProjectName = this.com_Project.Text.Trim();
            string DataBaseName = this.com_DataBase.Text.Trim();
            //////////////��ȡ��Ӧ�Ŀ����������Ϣ
            string type = string.Empty;
            string server = string.Empty;
            string servername = string.Empty;
            string databasepath = string.Empty;
            string user = string.Empty;
            string password = string.Empty;
            string version = string.Empty;
            /////////////////
            XmlDocument XmlDoc = new XmlDocument();
            //cyf 20110713 modify
            XmlDoc.Load(ModData.v_projectDetalXML);
            //XmlDoc.Load(ModData.v_projectXML);
            if (null == XmlDoc)
            {
                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "��ȡ������Ϣʧ�ܣ�");
                return;
            }
            XmlElement ProjectEle = XmlDoc.SelectSingleNode(".//���̹���/����[@����='" + ProjectName + "']") as XmlElement;
            if (ProjectEle == null)
            {
                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "���ݿ⹤�̼�¼�ļ����޷��ҵ����̣�" + ProjectName);
                return;
            }
            XmlElement DataBaseEle = ProjectEle.SelectSingleNode(".//����/" + DataBaseName) as XmlElement;
            if (null == DataBaseEle)
            {
                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "���ݿ⹤�̼�¼�ļ����޷��ҵ����̣�" + ProjectName + "�п��壺" + DataBaseName);
                return;
            }
            XmlElement ConnectInfo = DataBaseEle.SelectSingleNode(".//������Ϣ") as XmlElement;
            this._ConnectInfo = ConnectInfo;
            XmlElement Base = DataBaseEle.SelectSingleNode(".//������Ϣ/����") as XmlElement;
            type = ConnectInfo.GetAttribute("����");
            server = ConnectInfo.GetAttribute("������");
            servername = ConnectInfo.GetAttribute("������");
            databasepath = ConnectInfo.GetAttribute("���ݿ�");
            user = ConnectInfo.GetAttribute("�û�");
            password = ConnectInfo.GetAttribute("����");
            version = ConnectInfo.GetAttribute("�汾");
            /////////////////////////////ʹ��������Ϣ�����������ݿ�
            pGisDT = new SysCommon.Gis.SysGisDataSet();
            if (string.IsNullOrEmpty(type))
            {
                ex = new Exception("����δ��ʼ����");
                return;
            }
            if (type == "SDE")
            {
                pGisDT.SetWorkspace(server, servername, databasepath, user, password, version, out ex);
                if (ex != null)
                {
                    SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "���ݿ�����ʧ�ܣ�");
                    return;
                }
            }
            else if (type == "GDB")
            {
                pGisDT.SetWorkspace(databasepath, SysCommon.enumWSType.GDB, out ex);
                if (ex != null)
                {
                    SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "���ݿ�����ʧ�ܣ�");
                    return;
                }
            }
            else if (type == "PDB")
            {
                pGisDT.SetWorkspace(databasepath, SysCommon.enumWSType.PDB, out ex);
                if (ex != null)
                {
                    SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "���ݿ�����ʧ�ܣ�");
                    return;
                }
            }          
        }
        /// <summary>
        /// ��ȡ���ݿ��ͼ���б�
        /// </summary>
        private void GetDataBaseLayer()
        {
            if (this._ConnectInfo == null)
                return;
            //////////////��ȡ��Ӧ�Ŀ����������Ϣ
            string type = string.Empty;
            string server = string.Empty;
            string servername = string.Empty;
            string databasepath = string.Empty;
            string user = string.Empty;
            string password = string.Empty;
            string version = string.Empty;
            /////////////////

            XmlElement ConnectInfo = this._ConnectInfo;
            try
            {
                type = ConnectInfo.GetAttribute("����");
                server = ConnectInfo.GetAttribute("������");
                servername = ConnectInfo.GetAttribute("������");
                databasepath = ConnectInfo.GetAttribute("���ݿ�");
                user = ConnectInfo.GetAttribute("�û�");
                password = ConnectInfo.GetAttribute("����");
                version = ConnectInfo.GetAttribute("�汾");
            }
            catch (Exception e)
            {
                //*******************************************************************
                //guozheng added
                if (ModData.SysLog != null)
                {
                    ModData.SysLog.Write(e, null, DateTime.Now);
                }
                else
                {
                    ModData.SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
                    ModData.SysLog.Write(e, null, DateTime.Now);
                }
                //********************************************************************
                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "��ȡ������Ϣʧ�ܣ�");
                return;
            }
            /////////////////////////////ʹ��������Ϣ�����������ݿ�
            pGisDT = new SysCommon.Gis.SysGisDataSet();
            if (string.IsNullOrEmpty(type))
            {
                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "����δ��ʼ����");
                return;
            }
            Exception ex = null;
            if (type == "SDE")
            {
                pGisDT.SetWorkspace(server, servername, databasepath, user, password, version, out ex);
                if (ex != null)
                {
                    SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "���ݿ�����ʧ�ܣ�");
                    return;
                }
            }
            else if (type == "GDB")
            {
                pGisDT.SetWorkspace(databasepath, SysCommon.enumWSType.GDB, out ex);
                if (ex != null)
                {
                    SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "���ݿ�����ʧ�ܣ�");
                    return;
                }
            }
            else if (type == "PDB")
            {
                pGisDT.SetWorkspace(databasepath, SysCommon.enumWSType.PDB, out ex);
                if (ex != null)
                {
                    SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "���ݿ�����ʧ�ܣ�");
                    return;
                }
            }

            if (rbServer.Checked == true)
            {
                //������ݿ��е�����Ҫ������ӵ�ͼ���б��й��û�ѡ��
                List<IDataset> LstDT = pGisDT.GetAllFeatureClass();
                List<ILayer> LstLayer = new List<ILayer>();
                for (int i = 0; i < LstDT.Count; i++)
                {
                    IFeatureClass pFeaCls = LstDT[i] as IFeatureClass;
                    //*************************************************
                    //////�ж��û�������ʷ��
                    if (this.com_DataBase.Text == "���ƿ�")
                    {
                        if (LstDT[i].Name.EndsWith("_GOH") || LstDT[i].Name.EndsWith("_GOH"))
                            continue;
                    }
                    else if (this.com_DataBase.Text == "��ʷ��")
                    {
                        if (!LstDT[i].Name.EndsWith("_GOH") && !LstDT[i].Name.EndsWith("_GOH"))
                            continue;
                    }
                    else continue;
                    //************************************************

                    if (pFeaCls.FeatureType == esriFeatureType.esriFTAnnotation) continue;
                    if (pFeaCls.ShapeType == esriGeometryType.esriGeometryPolyline || pFeaCls.ShapeType == esriGeometryType.esriGeometryPolygon)
                    {
                        //ֻ����������ͼ��
                        IFeatureLayer pFeaLayer = new FeatureLayerClass();
                        pFeaLayer.FeatureClass = pFeaCls;
                        ILayer pLayer = pFeaLayer as ILayer;
                        if (!LstLayer.Contains(pLayer))
                        {
                            LstLayer.Add(pLayer);
                        }
                    }
                }
                if (LstLayer.Count == 0)
                {
                    return;
                }
                //��ͼ���б�����Ӵ�ѡ��ͼ��
                this.list_JoinLayer.Items.Clear();
                for (int j = 0; j < LstLayer.Count; j++)
                {
                    string layername = ((LstLayer[j] as IFeatureLayer).FeatureClass as IDataset).Name;
                    this.list_JoinLayer.Items.Add(layername);
                }
                if (rbExistdata.Checked == true)
                {
                    this.com_MapFrameList.Items.Clear();
                    if (this.list_JoinLayer.Items.Count == 0)
                    {
                        return;
                    }
                    for (int i = 0; i < this.list_JoinLayer.Items.Count; i++)
                    {
                        this.com_MapFrameList.Items.Add(this.list_JoinLayer.Items[i].ToString());
                    }
                }
            }
        }
        /// <summary>
        /// ͼ����ϱ�ͼ����Դѡ��ı�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void com_MapFrameList_SelectedIndexChanged(object sender, EventArgs e)
        {
            Exception ex=null;
            if (string.IsNullOrEmpty(this.com_MapFrameList.Text))
                return;
            string MapFName = this.com_MapFrameList.Text.Trim();
            IFeatureClass MapFeacls = null;
            if (rbExistdata.Checked == true)///���������л�ȡͼ�����ͼ��
            {
                if (rbServer.Checked == true)////���������л�ȡ
                {
                   MapFeacls= this.pGisDT.GetFeatureClass(MapFName, out ex);
                    if (null != ex)
                        return;
                }
                else if (rbMapLayer.Checked == true)////ͼ�������л�ȡ
                {
                    ILayer GetLayer = GetLayAtMapByName(MapFName);
                    IFeatureLayer GetFeaLayer=GetLayer as IFeatureLayer;
                    if (null==GetFeaLayer){ex =new Exception("��ȡͼ����ϱ�ͼ��ʧ��");return;}
                    MapFeacls = GetFeaLayer.FeatureClass;
                }               
            }
            else if (rbOutdata.Checked == true) /////�ⲿ�����л�ȡ
            {
                if (this.MapFramepGisDT == null)
                    return;
                MapFeacls = this.MapFramepGisDT.GetFeatureClass(MapFName, out ex);
                if (null != ex)
                    return;
            }
            /////������ȡͼ����ϱ�ͼ����ֶΣ���ӵ��б���
            if (null != MapFeacls)
            {
                this.com_MapNoField.Items.Clear();
                for (int i = 0; i < MapFeacls.Fields.FieldCount; i++)
                {
                    if (MapFeacls.Fields.get_Field(i).Type == esriFieldType.esriFieldTypeOID || MapFeacls.Fields.get_Field(i).Type == esriFieldType.esriFieldTypeGeometry)
                        continue;
                    this.com_MapNoField.Items.Add(MapFeacls.Fields.get_Field(i).Name.Trim());
                }
            }
        }

        private void addMapFrameLayer()//////��ͼ����ϱ��ͼ����ʾ�ڵ�ͼ�ؼ���
        {
            Exception ex = null;
            if (string.IsNullOrEmpty(this.com_MapFrameList.Text))
                return;
            string MapFName = this.com_MapFrameList.Text.Trim();
            IFeatureClass MapFeacls = null;
            if (rbExistdata.Checked == true)///���������л�ȡͼ�����ͼ��
            {
                if (rbServer.Checked == true)////���������л�ȡ
                {
                    MapFeacls = this.pGisDT.GetFeatureClass(MapFName, out ex);
                    if (null != ex)
                        return;
                }
                else if (rbMapLayer.Checked == true)////ͼ�������л�ȡ
                {
                    int layercount = ModData.v_AppGIS.ArcGisMapControl.LayerCount;
                    if (layercount == 0)
                        return;
                    for (int i = 0; i < layercount; i++)
                    {
                        IFeatureLayer Fealayer = ModData.v_AppGIS.ArcGisMapControl.get_Layer(i) as IFeatureLayer;
                        if ((Fealayer.FeatureClass as IDataset).Name == MapFName)
                        {
                            MapFeacls = Fealayer.FeatureClass;
                            break;
                        }
                    }
                }
            }
            else if (rbOutdata.Checked == true) /////�ⲿ�����л�ȡ
            {
                if (this.MapFramepGisDT == null)
                    return;
                MapFeacls = this.MapFramepGisDT.GetFeatureClass(MapFName, out ex);
                if (null != ex)
                    return;
            }
            /////����ͼ��
            if (null != MapFeacls)
            {
                
            }
        }

        private void btn_SelectAll_Click(object sender, EventArgs e)
        {
            if (this.list_JoinLayer.Items.Count == 0)
                return;
            for (int i = 0; i < this.list_JoinLayer.Items.Count; i++)
            {
                this.list_JoinLayer.SetItemChecked(i, true);
            }
        }

        private void btn_Reverse_Click(object sender, EventArgs e)
        {
            if (this.list_JoinLayer.Items.Count == 0)
                return;
            for (int i = 0; i < this.list_JoinLayer.Items.Count; i++)
            {
                if (this.list_JoinLayer.GetItemChecked(i))
                    this.list_JoinLayer.SetItemChecked(i, false);
                else
                    this.list_JoinLayer.SetItemChecked(i, true);
            }
        }

        private void btn_clear_Click(object sender, EventArgs e)
        {
            if (this.list_JoinLayer.Items.Count == 0)
                return;
            for (int i = 0; i < this.list_JoinLayer.Items.Count; i++)
            {
                this.list_JoinLayer.SetItemChecked(i, false);
            }
        }

        private void btn_ControlFields_Click(object sender, EventArgs e)
        {
            List<IFeatureClass> list_Fea = new List<IFeatureClass>();
            Exception ex = null;
            //////////��ȡѡ�еĽӱ�ͼ��///////////
            //if (rbServer.Checked == true)/////����ͼ��
            //{
            //    #region ��ȡͼ��
            //    for (int i = 0; i < this.list_JoinLayer.Items.Count; i++)
            //    {
            //        if (this.list_JoinLayer.GetItemChecked(i) == true)
            //        {

            //            string layerName = this.list_JoinLayer.Items[i].ToString().Trim();
            //            IFeatureClass FeaCls = this.pGisDT.GetFeatureClass(layerName, out ex);
            //            if (null == ex)
            //            {
            //                list_Fea.Add(FeaCls);
            //            }
            //        }
            //    }
            //    #endregion              
            //}
            //else if (rbMapLayer.Checked == true)//////��ͼ�ؼ�ͼ��
            //{
                #region ��ȡͼ��
                this._JoinLayerName = new List<string>();
                for (int i = 0; i < this.list_JoinLayer.Items.Count; i++)
                {
                    if (this.list_JoinLayer.GetItemChecked(i) == true)
                    {
                        string layerName = this.list_JoinLayer.Items[i].ToString().Trim();
                        for (int j = 0; j < ModData.v_AppGIS.MapControl.LayerCount; j++)
                        {
                            if (layerName == ModData.v_AppGIS.MapControl.get_Layer(j).Name)
                            {
                                ILayer getlayer = ModData.v_AppGIS.MapControl.get_Layer(j);
                                IFeatureLayer fea = getlayer as IFeatureLayer;
                                if (fea != null)
                                {
                                    IFeatureClass getFeaCls = fea.FeatureClass;
                                    if (getFeaCls != null) list_Fea.Add(getFeaCls);
                                }
                                
                            }
                        }
                    }
                }
                #endregion
            //}
            ///////////��ȡÿ��ͼ��Ŀ��������ֶ�///////
            if (list_Fea.Count > 0)
            {
                Dictionary<string, List<string>> GetFields = new Dictionary<string, List<string>>();
                frmSetControlFields setFrm = new frmSetControlFields(list_Fea, this.m_FieldDic);
                if (DialogResult.Yes == setFrm.ShowDialog())
                {
                    this.m_FieldDic = setFrm.FieldDic;
                }

            }
        }
        //***************************************************************************
        //guozheng added 2010-12-30
        //***************************************************************************
        /// <summary>
        /// ͨ��һ�����ƻ�ȡ��ͼ�ؼ��ϵ�һ��ͼ��
        /// </summary>
        /// <param name="LayerName">ͼ����</param>
        /// <returns>�ҵ�����ILayer���Ҳ�������NULL</returns>
        private ILayer GetLayAtMapByName(string LayerName)
        {
            for (int i = 0; i < ModData.v_AppGIS.ArcGisMapControl.LayerCount; i++)
            {
                ILayer player = ModData.v_AppGIS.ArcGisMapControl.get_Layer(i);
                if (null == player) continue;
                if (player is IGroupLayer)
                {
                    ILayer GetLayer= GetLayerAtGropLayer(LayerName, player as IGroupLayer);
                    if (GetLayer != null) return GetLayer;
                 }
                else 
                {
                    IFeatureLayer GetFeaLayer = player as IFeatureLayer;
                    if (GetFeaLayer == null) continue;
                    else if ((GetFeaLayer.FeatureClass as IDataset).Name == LayerName)
                        return player;
                }
            }
            return null;
        }
        //***************************************************************************
        //guozheng added 2010-12-30
        //***************************************************************************
        /// <summary>
        /// ��GroupLayer��ͨ�������ƻ�ȡһ��ͼ�㣨�ݹ飩
        /// </summary>
        /// <param name="LayerName">ͼ����</param>
        /// <param name="pGroupLayer">IGroupLayer����</param>
        /// <returns>�ҵ�����ILayer���Ҳ�������NULL</returns>
        private ILayer GetLayerAtGropLayer(string LayerName,IGroupLayer pGroupLayer)
        {
            ICompositeLayer pComLayer = pGroupLayer as ICompositeLayer;
            if (null == pComLayer) return null;
            int ComCount = pComLayer.Count;
            for (int i = 0; i < ComCount; i++)
            {
                ILayer pLayer = pComLayer.get_Layer(i);
                if (pLayer is IGroupLayer)
                {
                    ILayer GetLayer = GetLayerAtGropLayer(LayerName, pLayer as IGroupLayer);
                    if (null != GetLayer) return GetLayer;
                }
                else
                {
                    IFeatureLayer GetFeaLayer = pLayer as IFeatureLayer;
                    if (null == GetFeaLayer) continue;
                    if ((GetFeaLayer.FeatureClass as IDataset).Name == LayerName)
                        return pLayer;
                }
            }
            return null;
        }

        private void list_JoinLayer_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                int CruIndex = this.list_JoinLayer.SelectedIndex;
                if (this.list_JoinLayer.GetItemChecked(CruIndex))
                    this.list_JoinLayer.SetItemChecked(CruIndex, false);
                else
                    this.list_JoinLayer.SetItemChecked(CruIndex, true);
            }
            catch
            {
            }
        }

        private void btn_Loaddata_Click(object sender, EventArgs e)
        {
            this.list_JoinLayer.Items.Clear();
            this.com_MapNoField.Items.Clear();
            this.com_MapFrameList.Items.Clear();
            if (rbExistdata.Checked == true)
            {
                this.com_MapFrameList.Items.Clear();
            }
            int layerCont = ModData.v_AppGIS.ArcGisMapControl.LayerCount;
            rbMapLayer.Checked = true;
            if (layerCont == 0)
            {
                MessageBox.Show("���ȼ�����ʱ�����ݵ���ͼ��!", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            int count = 0;
            if (layerCont > 0)
            {
                for (int j = 0; j < layerCont; j++)
                {
                    ILayer layer = ModData.v_AppGIS.ArcGisMapControl.get_Layer(j);
                    if (layer is IGroupLayer)
                    {
                        ICompositeLayer pComLayer = (ICompositeLayer)layer;
                        int ComCount = pComLayer.Count;
                        for (int i = 0; i < ComCount; i++)
                        {
                            IFeatureLayer pLayer = pComLayer.get_Layer(i) as IFeatureLayer;
                            if (null == pLayer) continue;
                            IFeatureClass FeaClss = pLayer.FeatureClass;
                            if (null == FeaClss)
                                continue;
                            if (FeaClss.ShapeType == esriGeometryType.esriGeometryPolygon || FeaClss.ShapeType == esriGeometryType.esriGeometryPolyline)
                            {
                                this.list_JoinLayer.Items.Add((FeaClss as IDataset).Name);
                                this.list_JoinLayer.SetItemChecked(count, true);
                                count++;
                                if (rbExistdata.Checked == true)
                                {
                                    this.com_MapFrameList.Items.Add((FeaClss as IDataset).Name);
                                }
                            }
                        }
                    }
                    else
                    {
                        IFeatureLayer FeaLayer = layer as IFeatureLayer;
                        if (null == FeaLayer) continue;
                        IFeatureClass FeaClss = FeaLayer.FeatureClass;
                        if (null == FeaClss)
                            continue;
                        if (FeaClss.ShapeType == esriGeometryType.esriGeometryPolygon || FeaClss.ShapeType == esriGeometryType.esriGeometryPolyline)
                        {
                            this.list_JoinLayer.Items.Add((FeaClss as IDataset).Name);
                            this.list_JoinLayer.SetItemChecked(count, true);
                            count++;
                            if (rbExistdata.Checked == true)
                            {
                                this.com_MapFrameList.Items.Add((FeaClss as IDataset).Name); ;
                            }
                        }
                    }
                }

            }
        }

        private void com_MapNoField_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}