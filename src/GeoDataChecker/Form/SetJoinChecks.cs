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
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.SystemUI;
using System.Threading;
using System.Xml;
using System.Text.RegularExpressions;
using System.IO;

namespace GeoDataChecker
{
    /// <summary>
    /// ���ܣ����ýӱ߼��ÿ��������Լ��
    /// ��д�ˣ�����
    /// </summary>
    public partial class SetJoinChecks : DevComponents.DotNetBar.Office2007Form
    {
        private Plugin.Application.IAppGISRef _AppHk;
        SysCommon.Gis.SysGisDataSet pGisDT = null;

        public SetJoinChecks(Plugin.Application.IAppGISRef AppHk)
        {
            InitializeComponent();

            _AppHk = AppHk;

            comBoxType.Items.AddRange(new object[] { "PDB", "GDB", "SDE" });
            comBoxType.SelectedIndex = 0;
        }
        private void btn_cancle_Click(object sender, EventArgs e)
        {
            int count = TreeLayer.Nodes.Count;
            for (int n = 0; n < count; n++)
            {
                TreeLayer.Nodes[n].Name = "";
            }
            this.Close();//�رմ���
        }

        private void SetJoinChecks_Load(object sender, EventArgs e)
        {
            //BindTree();
        }

        /// <summary>
        /// �ж��ǹ���Ա������ҵԱ ����ǹ���Ա�ͷ���1���������ҵԱ�ͷ���0
        /// </summary>
        /// <returns></returns>
        private int PurviewState()
        {
            XmlDocument doc = _AppHk.ProjectTree.Tag as XmlDocument;
            XmlElement Elment = doc.SelectSingleNode("//��½") as XmlElement;
            string state = Elment.GetAttribute("�Ƿ��ǹ�����Ա");
            if (state == "��") return 1;
            else return 0;
        }
        /// <summary>
        /// �ݲ�ֻ���������ֺ͵㣬����������ʾ����
        /// </summary>
        private bool ShowException()
        {
            bool state = true;
            Regex reg = new Regex(@"^-?([1-9]\d*\.\d*|0\.\d*[1-9]\d*|0?\.0+|0)$|^-?[1-9]\d*$");//������
            bool SearchBool = reg.IsMatch(txt_search.Text);//�����������
            bool AreaBool = reg.IsMatch(txt_area.Text);//��Χ�������
            if (!SearchBool || txt_search.Text == "")
            {
                errorProvider1.SetError(txt_search, "ֻ�������ֻ�С���Ҳ�Ϊ�գ�");
                state = false;
            }
            if (!AreaBool || txt_area.Text =="")
            {
                errorProvider2.SetError(txt_area, "ֻ�������ֻ�С���Ҳ�Ϊ�գ�");
                state = false;
            }
            return state;
        }
        /// <summary>
        /// ��ʼ�����ý����ϵ����в���ͼ�㣬�����¿�����Ĳ������ý��� ����ʼ������ʾ�б�
        /// </summary>
        /// <returns></returns>
        private void BindTree()
        {
            //JoinCheck JoinChecks = new JoinCheck();
            ////��ȡҪ�ӱ߼�������ͼ��
            //List<ILayer> listLayers = JoinChecks.GetCheckLayers(_AppHk.MapControl.Map, SetCheckState.CheckDataBaseName);
            //if (listLayers == null) return;
            //List<ILayer> listCheckLays = new List<ILayer>();
            //foreach (ILayer pTempLay in listLayers)
            //{
            //    IFeatureLayer pFeatLay = pTempLay as IFeatureLayer;
            //    if (pFeatLay == null) continue;
            //    if (pFeatLay.FeatureClass.FeatureType == esriFeatureType.esriFTAnnotation) continue;
            //    //�ӱ���Ե����ߺ��棬�������ǲ���ʱֻ����ߺ������м���
            //    if (!(pFeatLay.FeatureClass.ShapeType == esriGeometryType.esriGeometryPolyline || pFeatLay.FeatureClass.ShapeType == esriGeometryType.esriGeometryPolygon)) continue;

            //    listCheckLays.Add(pTempLay);
            //}
            //if (listCheckLays.Count == 0) return;

            //BindDataGrid(listCheckLays[0]);
            //foreach (ILayer layer in listCheckLays)
            //{
            //    DevComponents.AdvTree.Node node = new DevComponents.AdvTree.Node();
            //    node.Text = layer.Name;//���Ľڵ�����
            //    node.Tag = layer;//������ڽڵ��TAG�ϣ���������Ĳ���
            //    node.CheckBoxVisible = true;
            //    TreeLayer.Nodes.Add(node);
            //}
        }

        /// <summary>
        /// �DATAGRIDֵ
        /// </summary>
        /// <param name="layer"></param>
        private void BindDataGrid(ILayer layer)
        {

            IFeatureLayer f_layer = layer as IFeatureLayer;
            IFeatureClass F_class = f_layer.FeatureClass;
            IFields Fields = F_class.Fields;//�õ�Ҫ���൱�е������м���
            System.Data.DataTable table = new System.Data.DataTable();
            table.Columns.Add("�ֶ���", typeof(string));
            for (int n = 0; n < Fields.FieldCount; n++)
            {
                IField field = Fields.get_Field(n);
                if (field.Editable && field.Type != esriFieldType.esriFieldTypeGeometry && field.Type != esriFieldType.esriFieldTypeOID)
                {
                    System.Data.DataRow row = table.NewRow();
                    row[0] = Fields.get_Field(n).Name;//��������ֵ��ȥ
                    table.Rows.Add(row);
                }
            }
            dataGridViewX1.DataSource = table;
            dataGridViewX1.Tag = 0;
            dataGridViewX1.Columns[0].Width = 30;
            dataGridViewX1.Columns[0].ReadOnly = true;
            dataGridViewX1.Columns[1].Width = 218;
            dataGridViewX1.Columns[1].ReadOnly = true;
        }
        /// <summary>
        /// ������ڵ�ʱ���ı�DATAGRID�������
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeLayer_NodeClick(object sender, DevComponents.AdvTree.TreeNodeMouseEventArgs e)
        {
            //ѡ��ڵ�ʱ������һ��ѡ���������ֵ��ֵ������Ӧ�Ľڵ�
            if (e.Node.Index >= 0)
            {
                TreeLayer.Nodes[Convert.ToInt32(dataGridViewX1.Tag)].Name = "";
                for (int d = 0; d < dataGridViewX1.Rows.Count; d++)
                {
                    if (Convert.ToBoolean(dataGridViewX1.Rows[d].Cells[0].Value) == true)
                    {
                        string temp = dataGridViewX1.Rows[d].Cells[1].Value.ToString();
                        TreeLayer.Nodes[Convert.ToInt32(dataGridViewX1.Tag)].Name += temp + " ";
                    }

                }
            }
            //ѡ��ڵ�ʱ���б������Ƿ���ѡ���
            if (e.Node.Name != "")
            {
                #region �����ǰѡ�������������Ӧ�����Գ�ѡ���״̬

                ReadNode(e.Node);
                if (!e.Node.Checked)
                {
                    e.Node.Name = "";
                    return;
                }
                string Name = e.Node.Name.Trim();
                char[] sp = { ' ' };
                string[] StrName = Name.Split(sp);
                for (int c = 0; c < dataGridViewX1.Rows.Count; c++)
                {
                    for (int N = 0; N < StrName.Length; N++)
                    {
                        if (StrName[N] == dataGridViewX1.Rows[c].Cells[1].Value.ToString())
                        {
                            dataGridViewX1.Rows[c].Cells[0].Value = true;
                            break;
                        }
                    }
                }
                #endregion
            }
            else
            {
                #region û��ѡ������͸��ݽڵ��ϵĲ���������Ӧ����������
                if (e.Node.Checked)
                {
                    dataGridViewX1.Columns[0].ReadOnly = false;

                }
                else
                {
                    dataGridViewX1.Columns[0].ReadOnly = true;
                }
                ReadNode(e.Node);
                #endregion
            }


        }
        /// <summary>
        /// �������ϵĽڵ㣬��ȡ���ͼ��������԰����ʾ�ؼ�����
        /// </summary>
        /// <param name="node"></param>
        private void ReadNode(DevComponents.AdvTree.Node node)
        {
            if (dataGridViewX1.DataSource != null)
                dataGridViewX1.DataSource = null;
            ILayer layer = node.Tag as ILayer;//��ǰ�ڵ��ϵ�TAG
            IFeatureLayer f_layer = layer as IFeatureLayer;
            IFeatureClass F_class = f_layer.FeatureClass;
            IFields Fields = F_class.Fields;//�õ�Ҫ���൱�е������м���
            System.Data.DataTable table = new System.Data.DataTable();
            table.Columns.Add("�ֶ���", typeof(string));
            for (int n = 0; n < Fields.FieldCount; n++)
            {
                IField field = Fields.get_Field(n);
                if (field.Editable && field.Type != esriFieldType.esriFieldTypeGeometry && field.Type != esriFieldType.esriFieldTypeOID)
                {
                    System.Data.DataRow row = table.NewRow();
                    row[0] = Fields.get_Field(n).Name;//��������ֵ��ȥ
                    table.Rows.Add(row);
                }
            }
            dataGridViewX1.Tag = node.Index;
            dataGridViewX1.DataSource = table;
            dataGridViewX1.Columns[0].Width = 30;
            dataGridViewX1.Columns[1].Width = 218;
            dataGridViewX1.Columns[1].ReadOnly = true;
        }
        /// <summary>
        /// �رմ���ʱ�������������ѡ��
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SetJoinChecks_FormClosing(object sender, FormClosingEventArgs e)
        {
            int count = TreeLayer.Nodes.Count;
            for (int n = 0; n < count; n++)
            {
                TreeLayer.Nodes[n].Name = "";
            }
        }
        /// <summary>
        /// ������ʼ���ʱ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_enter_Click(object sender, EventArgs e)
        {
            Exception eError=null;

            if(txtRange.Text=="")
            {
                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "��ѡ��ӱ߷�Χ���ݿ⣡");
                return;
            }
            SysCommon.Gis.SysGisDataSet pRangeGisDB = new SysCommon.Gis.SysGisDataSet();
            pRangeGisDB.SetWorkspace(txtRange.Text.ToString().Trim(), SysCommon.enumWSType.PDB, out eError);
            if (eError != null)
            {
                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "�ӱ߷�Χ���ݿ�����ʧ�ܣ�");
                return;
            }

            if (ShowException())
            {
                #region ������־������Ϣ
                string logPath = txtLog.Text;
                if (logPath.Trim() == string.Empty)
                {
                    SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "��ѡ����־���·��!");
                    return;
                }
                if (File.Exists(logPath))
                {
                    SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "�����ļ��Ѵ���!\n" + logPath);
                }

                //������־��ϢEXCEL��ʽ
                SysCommon.DataBase.SysDataBase pSysLog = new SysCommon.DataBase.SysDataBase();
                pSysLog.SetDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + logPath + "; Extended Properties=Excel 8.0;", SysCommon.enumDBConType.OLEDB, SysCommon.enumDBType.ACCESS, out eError);
                if (eError != null)
                {
                    SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "��־��Ϣ������ʧ�ܣ�");
                    return;
                }
                string strCreateTableSQL = @" CREATE TABLE ";
                strCreateTableSQL += @" ������־ ";
                strCreateTableSQL += @" ( ";
                strCreateTableSQL += @" ��鹦���� VARCHAR, ";
                strCreateTableSQL += @" �������� VARCHAR, ";
                strCreateTableSQL += @" �������� VARCHAR, ";
                strCreateTableSQL += @" ����ͼ��1 VARCHAR, ";
                strCreateTableSQL += @" ����OID1 VARCHAR, ";
                strCreateTableSQL += @" ����ͼ��2 VARCHAR, ";
                strCreateTableSQL += @" ����OID2 VARCHAR, ";
                strCreateTableSQL += @" ��λ��X VARCHAR , ";
                strCreateTableSQL += @" ��λ��Y VARCHAR , ";
                strCreateTableSQL += @" ���ʱ�� VARCHAR ,";
                strCreateTableSQL += @" �����ļ�·�� VARCHAR ";
                strCreateTableSQL += @" ) ";

                pSysLog.UpdateTable(strCreateTableSQL, out eError);
                if (eError != null)
                {
                    SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "�����ͷ����");
                    return;
                }

                #endregion

                double AreaValue = Convert.ToDouble(txt_area.Text);//��Χ�ݲ�
                double SearchValue = Convert.ToDouble(txt_search.Text);//�����ݲ�
                DataCheckClass JoinChecks = new DataCheckClass(_AppHk);
                //����־��������Ϣ�ͱ�����������
                JoinChecks.ErrDbCon = pSysLog.DbConn;
                JoinChecks.ErrTableName = "������־";
                JoinChecks.AREAValue = AreaValue;
                JoinChecks.SEARCHValue = SearchValue;

                SysCommon.DataBase.SysTable pSysTable = new SysCommon.DataBase.SysTable();
                string conStr = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + TopologyCheckClass.GeoDataCheckParaPath;
                pSysTable.SetDbConnection(conStr, SysCommon.enumDBConType.OLEDB, SysCommon.enumDBType.ACCESS, out eError);
                if (eError != null)
                {
                    SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "GIS���ݼ�����ñ�����ʧ�ܣ�");
                    return;
                }
                //��ýӱ߷�Χ��ͼ�������ֶ���
                DataTable mTable = GetParaValueTable(pSysTable, 39, out eError);
                if (eError != null)
                {
                    SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "��ѯGIS���ݼ�����ñ�ʧ�ܣ�");
                    return;
                }
                if (mTable.Rows.Count == 0)
                {
                    SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "δ���нӱ߼����������ã�");
                    return;
                }
                string rangeLayerName = mTable.Rows[0]["ͼ��"].ToString().Trim();
                string rangeFieldName = mTable.Rows[0]["�ֶ���"].ToString().Trim();
                if (rangeFieldName == "" || rangeLayerName == "")
                {
                    SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "�ӱ߼��������ò�������");
                    return;
                }

                //���ý����ϳ�ʼ��ʾ�����Ա�
                TreeLayer.Nodes[Convert.ToInt32(dataGridViewX1.Tag)].Name = "";
                for (int d = 0; d < dataGridViewX1.Rows.Count; d++)
                {
                    if (Convert.ToBoolean(dataGridViewX1.Rows[d].Cells[0].Value) == true)
                    {
                        string temp = dataGridViewX1.Rows[d].Cells[1].Value.ToString();
                        TreeLayer.Nodes[Convert.ToInt32(dataGridViewX1.Tag)].Name += temp + " ";
                    }

                }

                if (!JoinChecks.Initialize_Tree(pRangeGisDB, rangeLayerName, rangeFieldName, _AppHk.DataTree, TreeLayer, _AppHk, out eError))
                {
                    SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "��ʼ����ͼʧ�ܣ�");
                    return;
                }
                //int Pur = PurviewState();//��֤�ǹ���Ա������ҵԱ

                ///˵������ҵԱ��ͼ��ȥ���нӱߴ�������Ա����������Χȥ�ӱߴ���
                //if (Pur == 1)
                //{
                    ////����Ա
                    ////��ʼ�����ݴ�����ͼ
                    //if (!JoinChecks.Initialize_Tree("��������Χ", "CASE_NAME", _AppHk.DataTree, TreeLayer, _AppHk))
                    //{
                    //    return;
                    //}
                //}
                //else if(Pur == 0)
                //{
                //    //��ҵԱ
                //    //��ʼ�����ݴ�����ͼ
                //    if (!JoinChecks.Initialize_Tree("ͼ����Χ", "MAP_ID", _AppHk.DataTree, TreeLayer, _AppHk))
                //    {
                //        return;
                //    }
                //}
                this.Close();
                //���ݽӱ߼�� 
                //���߳����ٶȻ������Ҫ������Ҫ�ؿռ��ѯʱ
                //System.Threading.ParameterizedThreadStart parstart = new System.Threading.ParameterizedThreadStart(JoinChecks.DoJoinCheck);
                //Thread aThread = new Thread(parstart);
                //_AppHk.CurrentThread = aThread;
                //aThread.Priority = ThreadPriority.Highest;
                //aThread.Start(_AppHk as object);, Pur
                int Pur = 0;        //��ͼ�����нӱ�
                JoinChecks.DoJoinCheck(_AppHk as object,Pur);
            }
        }

        private void btnDB_Click(object sender, EventArgs e)
        {
            switch (comBoxType.Text)
            {
                case "SDE":

                    break;

                case "GDB":
                    FolderBrowserDialog pFolderBrowser = new FolderBrowserDialog();
                    if (pFolderBrowser.ShowDialog() == DialogResult.OK)
                    {
                        if (!pFolderBrowser.SelectedPath.EndsWith(".gdb"))
                        {
                            SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "��ѡ��GDB");
                            return;
                        }
                        txtDB.Text = pFolderBrowser.SelectedPath;
                    }
                    break;

                case "PDB":
                    OpenFileDialog OpenFile = new OpenFileDialog();
                    OpenFile.Title = "ѡ��PDB����";
                    OpenFile.Filter = "PDB����(*.mdb)|*.mdb";
                    if (OpenFile.ShowDialog() == DialogResult.OK)
                    {
                        txtDB.Text = OpenFile.FileName;
                    }
                    break;

                default:
                    break;
            }
        }

        private void btnCon_Click(object sender, EventArgs e)
        {
            Exception eError = null;

            //�������ݿ�
            pGisDT = new SysCommon.Gis.SysGisDataSet();
            if (comBoxType.Text.Trim() == "SDE")
            {
                pGisDT.SetWorkspace(txtServer.Text, txtInstance.Text, txtDB.Text, txtUser.Text, txtPassword.Text, txtVersion.Text, out eError);
                if (eError != null)
                {
                    SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "���ݿ�����ʧ�ܣ�");
                    return;
                }
            }
            else if (comBoxType.Text.Trim() == "GDB")
            {
                pGisDT.SetWorkspace(txtDB.Text.Trim(), SysCommon.enumWSType.GDB, out eError);
                if (eError != null)
                {
                    SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "���ݿ�����ʧ�ܣ�");
                    return;
                }
            }
            else
            {
                pGisDT.SetWorkspace(txtDB.Text.Trim(), SysCommon.enumWSType.PDB, out eError);
                if (eError != null)
                {
                    SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "���ݿ�����ʧ�ܣ�");
                    return;
                }
            }

            //������ݿ��е�����Ҫ����
            List<IDataset> LstDT = pGisDT.GetAllFeatureClass();
            //Ҫ���м���Ҫ����
            List<ILayer> LstLayer = new List<ILayer>();
            for(int i=0;i<LstDT.Count;i++)
            {
                IFeatureClass pFeaCls = LstDT[i] as IFeatureClass;
                if(pFeaCls.FeatureType==esriFeatureType.esriFTAnnotation ) continue;
                if(pFeaCls.ShapeType==esriGeometryType.esriGeometryPolyline||pFeaCls.ShapeType==esriGeometryType.esriGeometryPolygon )
                {
                    //����Ҫ�غ���Ҫ�ؽ��нӱ߼��
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

            //��ʼ���ӱ߼�����
            BindDataGrid(LstLayer[0]);
            for (int j = 0; j < LstLayer.Count; j++)
            {
                DevComponents.AdvTree.Node node = new DevComponents.AdvTree.Node();
                node.Text =((LstLayer[j] as IFeatureLayer).FeatureClass as IDataset).Name  ;//���Ľڵ�����
                node.Tag = LstLayer[j];//������ڽڵ��TAG�ϣ���������Ĳ���
                node.CheckBoxVisible = true;
                TreeLayer.Nodes.Add(node);
            }
        }

        private void btnLog_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.OverwritePrompt = false;
            saveFile.Title = "����ΪEXCEL��ʽ";
            saveFile.Filter = "EXCEL��ʽ(*.xls)|*.xls";
            if (saveFile.ShowDialog() == DialogResult.OK)
            {
                txtLog.Text = saveFile.FileName;
            }
        }

        private void buttonX1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Title = "ѡ������Χ";
            openFile.Filter = "PDB����(*.mdb)|*.mdb";
            if (openFile.ShowDialog() == DialogResult.OK)
            {
                txtRange.Text = openFile.FileName;
            }
        }

           /// <summary>
        /// ���Ҳ���ֵ���
        /// </summary>
        /// <param name="pFeaDataset"></param>
        /// <param name="pSysTable"></param>
        /// <param name="checkParaID">����ID��Ψһ��ʶ�������</param>
        /// <param name="eError"></param>
        /// <returns></returns>
        private DataTable  GetParaValueTable(SysCommon.DataBase.SysTable pSysTable,int checkParaID,out Exception eError)
        {
            eError = null;
            DataTable mTable = null;

            string selStr = "select * from GeoCheckPara where ����ID=" + checkParaID;
            DataTable pTable = pSysTable.GetSQLTable(selStr, out eError);
            if (eError != null)
            {
                eError = new Exception("��ѯ�����󣬱���Ϊ��GeoCheckPara������IDΪ:" + checkParaID);
                return null ;
            }

            if (pTable == null || pTable.Rows.Count == 0)
            {
                eError = new Exception("�Ҳ�����¼������IDΪ:" + checkParaID);
                return null ;
            }
            string ParaType = pTable.Rows[0]["��������"].ToString().Trim();            //��������
            if (ParaType == "GeoCheckParaValue")
            {
                int ParaValue=int.Parse(pTable.Rows[0]["����ֵ"].ToString().Trim());   //����ֵ��������ʶ�������
                string str = "select * from GeoCheckParaValue where �������=" + ParaValue;
                mTable = pSysTable.GetSQLTable(str, out eError);
                if (eError != null)
                {
                    eError = new Exception("��ѯ�����󣬱���Ϊ��GeoCheckParaValue���������Ϊ:" + ParaValue);
                    return null ;
                }
            }
            return mTable;
        }

        private void comBoxType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comBoxType.Text != "SDE")
            {
                btnDB.Visible = true;
                txtDB.Size = new Size(txtServer.Size.Width - btnDB.Size.Width, txtDB.Size.Height);
                txtServer.Enabled = false;
                txtInstance.Enabled = false;
                txtUser.Enabled = false;
                txtPassword.Enabled = false;
                txtVersion.Enabled = false;
            }
            else
            {
                btnDB.Visible = false;
                txtDB.Size = new Size(txtServer.Size.Width, txtDB.Size.Height);
                txtServer.Enabled = true;
                txtInstance.Enabled = true;
                txtUser.Enabled = true;
                txtPassword.Enabled = true;
                txtVersion.Enabled = true;

            }
        }
    }
}