using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.SystemUI;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.GeoDatabaseDistributed;
using ESRI.ArcGIS.Geometry;
using System.Collections;


namespace SysCommon.MapSheet
{
    public partial class frmMapSheet : Form
    {
        private bool m_Res;
        int pPicindex = 0;
        string MapPath;
        IWorkspace pWorkSpace;
        private ITool _tool = null;
        private ICommand _cmd = null;
        private string[] pMapNumSelected = null;
        private string pMapSheetName = null;

        string mapFrameName = "";
        public string MapFrameName
        {
            set { mapFrameName = value; }
        }
        //���ؽ��
        public bool Res
        {
            get { return m_Res; }
        }

        //�������ʱ������ⷶΧ
        private Dictionary<string, string[]> pMapNumExist4DBIMP = null;

        //�������ʱ����Ѿ�����ͼ����Χ���Ƿ�������ѡ��Χ�ص��������ظ�����
        public Dictionary<string, string[]> MapNumExist4DBIMP
        {
            set { pMapNumExist4DBIMP = value; }
        }

        //���ݸ���ʱ����ȡ���и��µķ�Χ
        private Dictionary<string, string[]> pMapNumExist4DBUpdate = null;

        //���ݸ�����ȡʱ����Ѿ���ȡ��ͼ����Χ���Ƿ�������ѡ��Χ�ص��������ظ���ȡ���и���
        public Dictionary<string, string[]> MapNumExist4DBUpdate
        {
            set { pMapNumExist4DBUpdate = value; }
        }

        //ѡ��ͼ����ʱ���Ƿ���������߸�����ȡ�ļ��
        private bool pMapNumCheck = false;

        //�Ƿ���в鿴ͼ�����ص������д�������־
        public bool MapNumCheck
        {
            set { pMapNumCheck = value; }
        }

        //MapSheetName���ԣ���ȡͼ����ϱ�ͼ������
        public string MapSheetName
        {
            get { return pMapSheetName; }
        }

        //MapNumSelected���ԣ���ȡѡ���ͼ��������
        public string[] MapNumSelected
        {
            get { return pMapNumSelected; }
            
        }

        public frmMapSheet(string vMapPath)
        {
            MapPath = vMapPath;
            InitializeComponent();
        }
        //����һ�����죬���������Ĵ���̳���
        public frmMapSheet()
        {
            InitializeComponent();
        }
        /// <summary>
        /// ����ͼ����ϱ�
        /// </summary>
        public void AddMap()
        {
            IWorkspaceFactory pWorkSpaceFactory = new AccessWorkspaceFactoryClass();
            IFeatureWorkspace pFeatureWorkSpace;
            IDataset pDataSet;
            IFeatureClass pFeatureClass=null ;
            IFeatureLayer pFeatureLayer = new FeatureLayerClass();
            ILayer pLayer;
            string LayerName;
            if (MapPath != "" && MapPath != null)
            {
                pWorkSpace = pWorkSpaceFactory.OpenFromFile(MapPath, 0);
                if (pWorkSpace != null)
                {
                    pFeatureWorkSpace = pWorkSpace as IFeatureWorkspace;

                    //���紫���˱�������Ϣ����ֻ�������������ߵ�ͼ����ϱ�����comboBoxScale��Ϊ������
                    if (vStrScale == "")
                    {
                        //�о�Ҫ�������ƣ����ص��ؼ���
                        IEnumDataset pEnumDataset = pWorkSpace.get_Datasets(ESRI.ArcGIS.Geodatabase.esriDatasetType.esriDTAny);
                        pEnumDataset.Reset();
                        IDataset pDataset = pEnumDataset.Next();
                        string pFirstMapName = null;
                        do
                        {
                            if (pDataset != null)
                            {
                                //��������ݼ�
                                if (pDataset is IFeatureDataset)
                                {
                                    //IFeatureDataset pFeatureDataset = pDataset as IFeatureDataset ;
                                    //if (pFeatureDataset != null)
                                    //{
                                    //    pEnumDataset = pFeatureDataset.Subsets();
                                    //}
                                }
                                //�����Ҫ����
                                else if (pDataset is IFeatureClass)
                                {
                                    this.comboBoxScale.Items.Add(pDataset.Name);
                                    if (pFirstMapName == null)
                                    {
                                        pFirstMapName = pDataset.Name;
                                        this.comboBoxScale.Text = pDataset.Name;
                                    }
                                    pDataset = pEnumDataset.Next();
                                }
                            }
                        } while (pDataset != null);

                        pFeatureClass = pFeatureWorkSpace.OpenFeatureClass(pFirstMapName);
                    }
                    else
                    {
                        //string MapFrameName = GetMapFrameName(vStrScale);
                        if (mapFrameName != "")
                        {
                            this.comboBoxScale.Items.Add(mapFrameName);
                            this.comboBoxScale.Text = mapFrameName;
                            this.comboBoxScale.Enabled = false;
                            pFeatureClass = pFeatureWorkSpace.OpenFeatureClass(mapFrameName);
                        }
                    }
                    if (pFeatureClass != null)
                    {
                        //ͼ����ʾ��ע
                        LabelingFeatureLayer(pFeatureLayer, "map_newno");

                        //ͼ��͸��
                        pDataSet = pFeatureClass as IDataset;
                        LayerName = pDataSet.Name;
                        pFeatureLayer.FeatureClass = pFeatureClass;
                        pFeatureLayer.Name = LayerName;
                        pLayer = pFeatureLayer as ILayer;
                        MapSheetControl.AddLayer(pLayer);
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    return;
                }
            }
        }
        /// <summary>
        /// ��עָ��ͼ���ָ���ֶ�
        /// </summary>
        /// <param name="vFeatureLayer"></param>
        /// <param name="vLabelField"></param>
        private void LabelingFeatureLayer(IFeatureLayer vFeatureLayer, string vLabelField)
        {
            IAnnotateLayerPropertiesCollection pAnnotateLayerPropertiesCollection = null;
            IGeoFeatureLayer pGeoFeatureLayer = vFeatureLayer as IGeoFeatureLayer;

            pAnnotateLayerPropertiesCollection = pGeoFeatureLayer.AnnotationProperties as IAnnotateLayerPropertiesCollection;

        }

        private void frmMapSheet_Load(object sender, EventArgs e)
        {
            AddMap();
        }

        #region �Ŵ���С��ѡ�񹤾�
        /// <summary>
        /// ���ι���
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolPan_Click(object sender, EventArgs e)
        {
            _tool = new ControlsMapPanToolClass();
            _cmd = _tool as ICommand;
            _cmd.OnCreate(this.MapSheetControl.Object);
            //֪ͨmap�ؼ���ǰ����
            this.MapSheetControl.CurrentTool = _tool;
            _cmd.OnClick();
        }

        /// <summary>
        /// ѡ��Ԫ�ع���
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolSelect_Click(object sender, EventArgs e)
        {
            _tool = new ControlsSelectToolClass();
            _cmd = _tool as ICommand;
            _cmd.OnCreate(this.MapSheetControl.Object);
            //֪ͨmap�ؼ���ǰ����
            this.MapSheetControl.CurrentTool = _tool;
            _cmd.OnClick();
        }
        /// <summary>
        /// ����Ŵ󹤾�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolZoomIn_Click(object sender, EventArgs e)
        {
            _tool = new ControlsMapZoomInToolClass();
            _cmd = _tool as ICommand;
            _cmd.OnCreate(this.MapSheetControl.Object);
            //֪ͨmap�ؼ���ǰ����
            this.MapSheetControl.CurrentTool = _tool;
            _cmd.OnClick();
        }
        /// <summary>
        /// ������С����
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolZoomOut_Click(object sender, EventArgs e)
        {
            _tool = new ControlsMapZoomOutToolClass();
            _cmd = _tool as ICommand;
            _cmd.OnCreate(this.MapSheetControl.Object);
            //֪ͨmap�ؼ���ǰ����
            this.MapSheetControl.CurrentTool = _tool;
            _cmd.OnClick();
        }
        /// <summary>
        /// ����ͼ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void commandLastExtendBack_Click(object sender, EventArgs e)
        {
            _cmd = new ControlsMapZoomToLastExtentBackCommandClass();
            _cmd.OnCreate(this.MapSheetControl.Object);
            _cmd.OnClick();
        }
        /// <summary>
        /// ǰ����ͼ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void commandLastExtendForward_Click(object sender, EventArgs e)
        {
            _cmd = new ControlsMapZoomToLastExtentForwardCommand();
            _cmd.OnCreate(this.MapSheetControl.Object);
            _cmd.OnClick();
        }
        /// <summary>
        /// ѡ��Ҫ�ع���
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolSelectFeatures_Click(object sender, EventArgs e)
        {
            _tool = new ControlsSelectFeaturesToolClass();
            _cmd = _tool as ICommand;
            _cmd.OnCreate(this.MapSheetControl.Object);
            //֪ͨmap�ؼ���ǰ����
            this.MapSheetControl.CurrentTool = _tool;
            _cmd.OnClick();
        }
        /// <summary>
        /// ���ѡ��
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolClearSelect_Click(object sender, EventArgs e)
        {
            _cmd = new ControlsClearSelectionCommandClass();
            _cmd.OnCreate(this.MapSheetControl.Object);
            _cmd.OnClick();
        }
        /// <summary>
        /// ˢ��ͼ��
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void commandRefresh_Click(object sender, EventArgs e)
        {
            _cmd = new ControlsMapRefreshViewCommandClass();
            _cmd.OnCreate(this.MapSheetControl.Object);
            _cmd.OnClick();
        }
        /// <summary>
        /// ȫͼ���
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void commandFullExtend_Click(object sender, EventArgs e)
        {
            _cmd = new ControlsMapFullExtentCommandClass();
            _cmd.OnCreate(this.MapSheetControl.Object);
            _cmd.OnClick();
        } 
        #endregion

        /// <summary>
        /// ������������ؼ��е����ݷ����ı�ʱ�Ĵ�����
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBoxScale_SelectedIndexChanged(object sender, EventArgs e)
        {
            IFeatureClass pFeatureClass = null;
            IFeatureLayer pFeaturelayer = new FeatureLayerClass();
            ILayer pLayer = null;
            IFeatureWorkspace pFeatureWorkspace = pWorkSpace as IFeatureWorkspace;
            if (pFeatureWorkspace != null)
            {
                MapSheetControl.Map.ClearLayers();

                pFeatureClass = pFeatureWorkspace.OpenFeatureClass(comboBoxScale.Text);
                if (pFeatureClass != null)
                {
                    pFeaturelayer.FeatureClass= pFeatureClass;
                    pFeaturelayer.Name = comboBoxScale.Text;
                    pLayer = pFeaturelayer as ILayer;

                    MapSheetControl.AddLayer(pLayer);
                    pMapSheetName = comboBoxScale.Text;
                    balloonTip2.Enabled = false;
                }
            }
        }

        /// <summary>
        /// ��ͼ�ؼ��е�ѡ�񼯷����仯ʱ�Ĵ�����
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MapSheetControl_OnSelectionChanged(object sender, EventArgs e)
        {
            
            IFeatureWorkspace pFeatureWorkspace = pWorkSpace as IFeatureWorkspace;
            IActiveView pActiview = MapSheetControl.Map as IActiveView;
            IFeatureLayer pFeaturelayer=null;
            IFeatureSelection pFeatureSelection=null;
            IFeatureClass pFeatureClass=null;
            IFeatureCursor pFeatureCur=null;
            ICursor pCur = null;
            IFields pFields=null;
            IFeature pFeat=null;
            string MapNum = null;

            //��õ�ǰ��ͼ����
            IMap pMap = pActiview.FocusMap as IMap;

            //��õ�ǰͼ����ϱ��
            pFeaturelayer = pMap.get_Layer(0) as IFeatureLayer;
            pFeatureClass=pFeaturelayer.FeatureClass as IFeatureClass;
            pFields=pFeatureClass.Fields as IFields;
            Int32 i =pFields.FindField("map_newno");

            pFeatureSelection=pFeaturelayer as IFeatureSelection;

            //���������������
            this.checkedListBoxNo.Items.Clear();
            this.richTextBox1.Text = "";
            this.pictureBox4.Visible = false;
            this.pictureBox3.Visible = false;
            //��ȡ��ѡ�е�Ҫ��

            pFeatureSelection.SelectionSet.Search(null, false, out pCur);
            pFeatureCur = pCur as IFeatureCursor;

            pFeat = pFeatureCur.NextFeature();

            for (int j = 0; j < pFeatureSelection.SelectionSet.Count; j++)
            {
                MapNum = pFeat.get_Value(i).ToString();
                //�����ظ����루��Ϊ����¼��ڵ�һ�γ�����ʱ�򣬸ú������Ǳ�ִ�����Σ�
                if (this.checkedListBoxNo.Items.Contains(MapNum) == false)
                {
                    this.checkedListBoxNo.Items.Add(MapNum);
                    //������һ����¼Ϊѡ��
                    this.checkedListBoxNo.SetItemChecked(this.checkedListBoxNo.Items.Count-1, true);
                }
                pFeat = pFeatureCur.NextFeature();
            }

            pMapNumSelected=null;

            if(pFeatureSelection.SelectionSet.Count>0)
            {
                pMapNumSelected = new string[pFeatureSelection.SelectionSet.Count ];

                for (int k = 0; k <= pFeatureSelection.SelectionSet.Count - 1; k++)
                {
                    pMapNumSelected[k] = this.checkedListBoxNo.Items[k].ToString();
    			 
                }

                #region ѡ��ͼ��ʱ�ķ����жϺ���

                if(pMapNumCheck)
                {
                    MapOverlapAnanysis(pMapNumSelected,pMapNumExist4DBIMP,pMapNumExist4DBUpdate);
                }

                #endregion
                
            }
            //do
            //{
            //    pFeat = pFeatureCur.NextFeature();
            //    if (pFeat != null)
            //    {
            //        MapNum = pFeat.get_Value(i).ToString();
            //        //һ���ظ����루��Ϊ����¼��ڵ�һ�γ�����ʱ�򣬸ú������Ǳ�ִ�����Σ�
            //        if (this.checkedListBoxNo.Items.Contains(MapNum) == false)
            //        {
            //            this.checkedListBoxNo.Items.Add(MapNum);
            //        }
            //    }
            //    pFeat = pFeatureCur.NextFeature();
            //} while (pFeat != null);
            

        }

        /// <summary>
        /// ͼ���ص��Լ�飬���������ʾ����־�б����
        /// </summary>
        /// <param name="pMapNumSelected"></param>
        /// <param name="pMapNumExist4DBIMP"></param>
        /// <param name="pMapNumExist4DBUpdate"></param>
        private void MapOverlapAnanysis(string[] pMapNumSelected, Dictionary<string, string[]> pMapNumExist4DBIMP, Dictionary<string, string[]> pMapNumExist4DBUpdate)
        {
            if (pMapNumSelected == null)
            {
                return;
            }
            else
            {
                //������ⷶΧ�Ƿ��ص�
                if (pMapNumExist4DBIMP != null)
                {
                    object[] TheSameNum = null;
                    string[] MapNumExist = pMapNumExist4DBIMP[comboBoxScale.Text];   //��ȡ��ǰ������ͼ���µ�����ͼ���ż���

                    TheSameNum=CompareArrayNSelectSame(MapNumExist, pMapNumSelected).ToArray();

                    //����ͬ������������־����ʾ����
                    if (TheSameNum.Length>0)
                    {
                        for (int i = 0; i <= TheSameNum.Length - 1; i++)
                        {
                            richTextBox1.Text = richTextBox1.Text + "\r\n" + "��Χ��" + TheSameNum[i] + "������ⷶΧ�ص���";
                        }
                        this.pictureBox4.Visible = true;
                        this.pictureBox3.Visible = false;
                    }
                    else
                    {
                        this.pictureBox3.Visible = true;
                        this.pictureBox4.Visible = false;
                    }
                }

                //����������ȡ��Χ�Ƿ��ص�
                if (pMapNumExist4DBUpdate != null)
                {
                    object[] TheSameNum = null;
                    string[] MapNumExist = pMapNumExist4DBIMP[comboBoxScale.Text];   //��ȡ��ǰ������ͼ���µ�����ͼ���ż���

                    TheSameNum = CompareArrayNSelectSame(MapNumExist, pMapNumSelected).ToArray();

                    //����ͬ������������־����ʾ����
                    if (TheSameNum.Length > 0)
                    {
                        for (int i = 0; i <= TheSameNum.Length - 1; i++)
                        {
                            richTextBox1.Text = richTextBox1.Text + "\r\n" + "��Χ��" + TheSameNum[i] + "������ȡ��Χ�ص���";
                        }
                        this.pictureBox4.Visible = true;
                        this.pictureBox3.Visible = false;
                    }
                    else
                    {
                        this.pictureBox3.Visible = true;
                        this.pictureBox4.Visible = false;
                    }
                }
            }
        }

        private ArrayList CompareArrayNSelectSame(string[] MapNumExist, string[] pMapNumSelected)
        {
            ArrayList array1 = new ArrayList();
            ArrayList array2 = new ArrayList();

            for (int i = 0; i <= MapNumExist.Length - 1; i++)
            {
                array1.Add(MapNumExist[i]);

            }

            for (int j = 0; j <= MapNumSelected.Length - 1; j++)
            {

                if (array1.Contains(MapNumSelected[j]))
                {
                    array2.Add(MapNumSelected[j]);
                }

            }

            return array2;
        }




        /// <summary>
        /// �����µİ�ťΪEnterʱ������������ѡ�и������ҵ�ͼ��
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SearchNumBox_KeyDown(object sender, KeyEventArgs e)
        {
            balloonTip2.Enabled = false;

            if (e.KeyCode == Keys.Enter)
            {
                if (SearchNumBox.Text != null)
                {
                    DoSearch(SearchNumBox.Text, comboBoxScale.Text);
                }
            }

            if (string.IsNullOrEmpty(SearchNumBox.Text))
            {
                balloonTip1.Enabled =true;
                pictureBox2.Visible = false;
                pictureBox1.Visible = false;
            }
            else
            {
                balloonTip1.Enabled =false;
            }
        }
        /// <summary>
        /// ����ͼ�����ƣ���ѯ��ѡ��ͼ����
        /// </summary>
        /// <param name="vMapNum"></param>
        /// <param name="vFeaturecassName"></param>
        private void DoSearch(string vMapNum, string vFeaturecassName)
        {
            bool pFind=false;
            IFeatureClass pFeatureClass = null;
            IQueryFilter pQueryFilter=new QueryFilterClass();
            pQueryFilter.WhereClause="[map_newno]='"+vMapNum+"'";
            IFeature pFeature = null;

            IEnvelope pUnoinEvelope = new EnvelopeClass();

            IFeatureWorkspace pFeatureworspace = pWorkSpace as IFeatureWorkspace;
            pFeatureClass = pFeatureworspace.OpenFeatureClass(vFeaturecassName);

            if (pFeatureClass!=null)
            {
                ISelectionSet pSelectionset= pFeatureClass.Select(pQueryFilter,esriSelectionType.esriSelectionTypeHybrid, esriSelectionOption.esriSelectionOptionNormal, pWorkSpace);
                IEnumIDs enumIDs = pSelectionset.IDs;
                int iD = enumIDs.Next();

                while (iD!=-1)
                {
                    pFind = true;
                    pFeature = pFeatureClass.GetFeature(iD);

                    pUnoinEvelope.Union(pFeature.Extent);

                    iD = enumIDs.Next();
                }

                if (pFind)
                {
                    pUnoinEvelope.Expand(2, 2, true);
                    MapSheetControl.Extent = pUnoinEvelope;
                }

                #region �����Ƿ��ҵ�������������ݺ�ͼƬ��ʽ
                if (pFind)
                {
                    pictureBox2.Visible = false;
                    pictureBox1.Visible = true;
                    balloonTip2.Enabled = false;
                }
                else
                {
                    pictureBox1.Visible = false;
                    pictureBox2.Visible = true;
                    balloonTip2.Enabled = true;
                } 
                #endregion
            }

        }

        private void cmdCancel_Click_1(object sender, EventArgs e)
        {
            pMapNumSelected = null;
            m_Res = false;
            this.Close();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            pPicindex += 1;
            if (pPicindex > 2)
            {
                pPicindex = 0;
            }
            if (pictureBox3.Visible)
            { pictureBox3.Load(Application.StartupPath + "\\..\\Res\\Pic\\SysCommom.MapSheet.NoDoubt" + pPicindex + ".png"); }
            if (pictureBox4.Visible)
            { pictureBox4.Load(Application.StartupPath + "\\..\\Res\\Pic\\SysCommom.MapSheet.Doubt" + pPicindex + ".png"); }
        }

        private void pictureBox4_VisibleChanged(object sender, EventArgs e)
        {
            if (pictureBox4.Visible)
            {
                timer1.Enabled = true;
                timer2.Enabled = true;

            }
            else
            {
                timer1.Enabled = false;
                timer2.Enabled = false;
            }
        }

        private void pictureBox3_VisibleChanged(object sender, EventArgs e)
        {
            if (pictureBox3.Visible)
            {
                pictureBox4.Visible = false;
                timer1.Enabled = true;
                timer2.Enabled = true;

            }
            else
            {
                timer1.Enabled = false;
                timer2.Enabled = false;
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;

            if (pictureBox3.Visible)
            { pictureBox3.Load(Application.StartupPath + "\\..\\Res\\Pic\\SysCommom.MapSheet.NoDoubt0.png"); }
            if (pictureBox4.Visible)
            { pictureBox4.Load(Application.StartupPath + "\\..\\Res\\Pic\\SysCommom.MapSheet.Doubt0.png"); }
        }

        private void cmdOk_Click(object sender, EventArgs e)
        {
            //���ص�ǰ��ͼ����ϱ�����
            vSelectMapName = comboBoxScale.Text.Trim();
            
            //���»���б���ѡ�е�ͼ���ţ���Ϊ�п����˹��������ѡ�е�ͼ����
            pMapNumSelected = null;

            ArrayList pMapNumArray=new ArrayList();
            for (int k = 0; k <= this.checkedListBoxNo.Items.Count-1; k++)
            {
                if(this.checkedListBoxNo.GetItemChecked(k))
                {
                    pMapNumArray.Add(this.checkedListBoxNo.Items[k].ToString());
                }

            }
            pMapNumSelected=pMapNumArray.ToArray(typeof(string)) as string[];

            pMapNumArray = null;
            m_Res = true;
            this.Close();
        }
        /// <summary>
        /// ѡ��Ԫ��
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bubbleButton4_Click(object sender, DevComponents.DotNetBar.ClickEventArgs e)
        {
            _tool = new ControlsSelectToolClass();
            _cmd = _tool as ICommand;
            _cmd.OnCreate(this.MapSheetControl.Object);
            //֪ͨmap�ؼ���ǰ����
            this.MapSheetControl.CurrentTool = _tool;
            _cmd.OnClick();
        }
        /// <summary>
        /// ���ι���
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bubbleButton3_Click(object sender, DevComponents.DotNetBar.ClickEventArgs e)
        {
            _tool = new ControlsMapPanToolClass();
            _cmd = _tool as ICommand;
            _cmd.OnCreate(this.MapSheetControl.Object);
            //֪ͨmap�ؼ���ǰ����
            this.MapSheetControl.CurrentTool = _tool;
            _cmd.OnClick();
        }
        /// <summary>
        /// ����Ŵ�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bubbleButton1_Click(object sender, DevComponents.DotNetBar.ClickEventArgs e)
        {
            _tool = new ControlsMapZoomInToolClass();
            _cmd = _tool as ICommand;
            _cmd.OnCreate(this.MapSheetControl.Object);
            //֪ͨmap�ؼ���ǰ����
            this.MapSheetControl.CurrentTool = _tool;
            _cmd.OnClick();
        }
        /// <summary>
        /// ������С
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bubbleButton2_Click(object sender, DevComponents.DotNetBar.ClickEventArgs e)
        {
            _tool = new ControlsMapZoomOutToolClass();
            _cmd = _tool as ICommand;
            _cmd.OnCreate(this.MapSheetControl.Object);
            //֪ͨmap�ؼ���ǰ����
            this.MapSheetControl.CurrentTool = _tool;
            _cmd.OnClick();
        }
        /// <summary>
        /// ǰ����ͼ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bubbleButton5_Click(object sender, DevComponents.DotNetBar.ClickEventArgs e)
        {
            _cmd = new ControlsMapZoomToLastExtentBackCommandClass();
            _cmd.OnCreate(this.MapSheetControl.Object);
            _cmd.OnClick();
        }
        /// <summary>
        /// ����ͼ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bubbleButton6_Click(object sender, DevComponents.DotNetBar.ClickEventArgs e)
        {
            _cmd = new ControlsMapZoomToLastExtentForwardCommand();
            _cmd.OnCreate(this.MapSheetControl.Object);
            _cmd.OnClick();
        }
        /// <summary>
        /// ѡ��Ҫ��
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bubbleButton10_Click(object sender, DevComponents.DotNetBar.ClickEventArgs e)
        {
            _tool = new ControlsSelectFeaturesToolClass();
            _cmd = _tool as ICommand;
            _cmd.OnCreate(this.MapSheetControl.Object);
            //֪ͨmap�ؼ���ǰ����
            this.MapSheetControl.CurrentTool = _tool;
            _cmd.OnClick();
        }
        /// <summary>
        /// ���ѡ��
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bubbleButton8_Click(object sender, DevComponents.DotNetBar.ClickEventArgs e)
        {
            _cmd = new ControlsClearSelectionCommandClass();
            _cmd.OnCreate(this.MapSheetControl.Object);
            _cmd.OnClick();
        }
        /// <summary>
        /// ˢ����ͼ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bubbleButton9_Click(object sender, DevComponents.DotNetBar.ClickEventArgs e)
        {
            _cmd = new ControlsMapRefreshViewCommandClass();
            _cmd.OnCreate(this.MapSheetControl.Object);
            _cmd.OnClick();
        }
        /// <summary>
        /// ȫͼ���
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bubbleButton7_Click(object sender, DevComponents.DotNetBar.ClickEventArgs e)
        {
            _cmd = new ControlsMapFullExtentCommandClass();
            _cmd.OnCreate(this.MapSheetControl.Object);
            _cmd.OnClick();
        }
        
        //�ѵ�ǰ��ͼ����ϱ����ƴ�����   fangmiao  20090702
        private string vSelectMapName = "";
        public string SelectMapName
        {
            get { return vSelectMapName; }
        }

        //������������Ϣ     fangmiao  20090706
        private string vStrScale = "";
        public string StrScale
        {
            set { vStrScale = value; }
        }

        //�ڶ��з����Ǳ����жϷ�   fangmiao  20090707
       
        //��ӱ�ע  fangmiao  20090708
        private void AddLabel()
        {
            IGeoFeatureLayer pGeoFeatLayer;
            if (this.MapSheetControl.LayerCount == 0) return;
            pGeoFeatLayer = this.MapSheetControl.get_Layer(this.MapSheetControl.LayerCount - 1) as IGeoFeatureLayer;
            pGeoFeatLayer.DisplayField = "MAP_NEWNO";

            IAnnotateLayerPropertiesCollection pAnnoProps = null;
            pAnnoProps = pGeoFeatLayer.AnnotationProperties;

            ILineLabelPosition pPosition = null;
            pPosition = new LineLabelPositionClass();
            pPosition.Parallel = true;
            pPosition.Above = true;

            ILineLabelPlacementPriorities pPlacement = new LineLabelPlacementPrioritiesClass();
            IBasicOverposterLayerProperties4 pBasic = new BasicOverposterLayerPropertiesClass();
            pBasic.FeatureType = esriBasicOverposterFeatureType.esriOverposterPolyline;
            pBasic.LineLabelPlacementPriorities = pPlacement;
            pBasic.LineLabelPosition = pPosition;
            pBasic.BufferRatio = 0;
            pBasic.FeatureWeight = esriBasicOverposterWeight.esriHighWeight;
            pBasic.NumLabelsOption = esriBasicNumLabelsOption.esriOneLabelPerPart;
            //pBasic.PlaceOnlyInsidePolygon = true;//���ڵ����ڲ���ʾ��ע  deleted by chulili s20111018 �����ϲ�û���������ã���仰Ӧע�͵����������Ǵ���

            ILabelEngineLayerProperties pLabelEngine = null;
            pLabelEngine = new LabelEngineLayerPropertiesClass();
            pLabelEngine.BasicOverposterLayerProperties = pBasic as IBasicOverposterLayerProperties;
            pLabelEngine.Expression = "[" + "MAP_NEWNO" + "]";
            pLabelEngine.Symbol.Size = 8;

            IAnnotateLayerProperties pAnnoLayerProps = null;
            pAnnoLayerProps = pLabelEngine as IAnnotateLayerProperties;
            pAnnoLayerProps.LabelWhichFeatures =esriLabelWhichFeatures.esriAllFeatures;
            pAnnoProps.Clear();
            pAnnoProps.Add(pAnnoLayerProps);
                    
            pGeoFeatLayer.DisplayAnnotation =true;
            this.MapSheetControl.ActiveView.Refresh();

        }
        //��ӱ�ע
        private void RemoveLabel()
        {
            IGeoFeatureLayer pGeoFeatLayer=null;
            pGeoFeatLayer = this.MapSheetControl.get_Layer(this.MapSheetControl.LayerCount - 1) as IGeoFeatureLayer;
            pGeoFeatLayer.DisplayAnnotation = false;
            this.MapSheetControl.ActiveView.Refresh();
        }
        private void bubbleButton5_Click_1(object sender, DevComponents.DotNetBar.ClickEventArgs e)
        {
            AddLabel();
        }
        //�Ƴ���ע
        private void bubbleButton6_Click_1(object sender, DevComponents.DotNetBar.ClickEventArgs e)
        {
            RemoveLabel();
        }

    }
}