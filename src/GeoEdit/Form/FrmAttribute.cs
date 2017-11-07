using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.SystemUI;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
namespace GeoEdit
{
    /// <summary>
    /// ���Ե�����
    /// </summary>
    public partial class FrmAttribute : DevComponents.DotNetBar.Office2007Form
    {

        static string text = "";//������¼�޸�֮ǰ��ֵ
        private Plugin.Application.IAppGISRef myHook;//��һ��MAP�����������������õ�MAP
        static string layer_name = "";//����һ����̬�������������յ�ǰLAYER������
        static string OID = "";//��¼��ǰ������OID��ֵ
        public Hashtable hs_table_tree = new Hashtable();//������������TREE���õ���ֵ
        public Hashtable hs_table_attribute = new Hashtable();//������ʾ��Ӧ������ֵ

        /// <summary>
        /// ���ù��캯����ֵ
        /// </summary>
        /// <param name="tb"></param>
        /// <param name="tb_show"></param>
        public FrmAttribute(Hashtable tb, Hashtable tb_show, Plugin.Application.IAppGISRef Hook)
        {
            myHook = Hook;
            hs_table_tree = tb;
            hs_table_attribute = tb_show;
            InitializeComponent();

        }

        /// <summary>
        /// ��MAP��ѡ���Ҫ����ʾ�ڿؼ���
        /// </summary>
        public void DataMain()
        {
            Hashtable table = hs_table_tree;
            if (table == null) return;
            treeview(table);
            treeview_Name.Nodes[0].Nodes[0].CheckState = CheckState.Checked;
        }
        /// <summary>
        /// �״ν������壬��һ����ʼ��SHOW,Ĭ��Ϊ��һ���ڵ���ϵ�һ���ӽڵ��TEXTֵ��Ϊһ��KEY���õ����������
        /// </summary>
        private void Initialize_show()
        {
            DataTable table = new DataTable();
            table.Columns.Add("�ֶ���", typeof(string));
            table.Columns.Add("ֵ", typeof(string));

            Hashtable hs_tb = hs_table_attribute;

            string texts = layer_name + OID;//ȫƴ��һ��KEY
            char[] sp = { ',' };
            char[] sp1 = { ' ' };
            int count = hs_tb.Count;//����ֵ����Ŀ
            string[] temp = hs_tb[texts].ToString().Substring(0, hs_tb[texts].ToString().Length - 1).Split(sp);//���ȶ��ִ�������Ϊ��β����һ�����š����ȴ��������ٽ��зָ�����Ҫ������
            for (int n = 0; n < temp.Length; n++)
            {
                string[] value = temp[n].Split(sp1);//�ٴεĽ��зָ�õ���������Ҫ������
                DataRow row = table.NewRow();//����һ����
                row[0] = value[0];
                row[1] = value[1];
                table.Rows.Add(row);

            }

            DT_VIEW_Attriubte.DataSource = table;//�����ݰ���ؼ���ʾ
            DT_VIEW_Attriubte.Columns[0].Width = 80;//ȷ���еĿ�
            DT_VIEW_Attriubte.Columns[1].Width = 180;
            int Count = DT_VIEW_Attriubte.Rows.Count - 1;//���һ��
            DT_VIEW_Attriubte.Rows[Count].ReadOnly = true;
            if (DT_VIEW_Attriubte.Rows.Count > 0)
            {
                DT_VIEW_Attriubte.Columns[0].SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;//���õ�һ�в�������
                DT_VIEW_Attriubte.Columns[1].SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;//�ڶ��в�������
                for (int c = 0; c < Count; c++)
                {
                    DT_VIEW_Attriubte.Rows[c].Height = 18;
                    string value = DT_VIEW_Attriubte.Rows[c].Cells[0].Value.ToString().ToLower();
                    if (value == "objectid" || value == "shape" || value == "shape_length" || value == "shape_area" || value == "element")//ȷ���ؼ��ϲ��ܸ��ĵ�����
                    {
                        DT_VIEW_Attriubte.Rows[c].ReadOnly = true;
                    }
                    else
                    {
                        DT_VIEW_Attriubte.Rows[c].Cells[0].ReadOnly = true;//���е��ֶ����ǲ����޸ĵ�
                    }
                }
            }
        }
        /// <summary>
        /// �����ݰ�����ϣ�����ʾ�ڵ�
        /// </summary>
        /// <param name="tb"></param>
        private void treeview(Hashtable tb)
        {
            if (treeview_Name.Nodes.Count > 0)
            {
                treeview_Name.Nodes.Clear();
            }
            bool state = false;//Ҫ�һ����ʼ��SHOW�����Ը���״̬�ؼ���ʾ����
            foreach (DictionaryEntry de in tb)
            {

                char[] sp = { ' ' };
                DevComponents.AdvTree.Node nodes = new DevComponents.AdvTree.Node();
                nodes.Text = de.Key.ToString();
                string[] temp = de.Value.ToString().Split(sp);
                for (int n = 0; n < temp.Length; n++)
                {

                    DevComponents.AdvTree.Node node = new DevComponents.AdvTree.Node();
                    node.Text = temp[n];
                    if (!state)
                    {

                        layer_name = de.Key.ToString();//��һ��ͼ����
                        OID = temp[n];//��һ��OID
                        Initialize_show();//���÷�������ʾ���Գ�ʼ��Ĭ��Ϊ��һ��
                        state = true;
                    }
                    nodes.Nodes.Add(node);
                    node.Parent.Expand();//չ��
                }
                treeview_Name.Nodes.Add(nodes);//���ڵ�ӵ�����

            }
        }
        /// <summary>
        /// ��������
        /// </summary>
        public DevComponents.AdvTree.AdvTree TreeName
        {
            get { return treeview_Name; }
            set { treeview_Name = value; }
        }
        public void FrmAttribute_Load(object sender, EventArgs e)
        {
            DataMain();//����������

        }
        /// <summary>
        /// ���¶�ȡHASHTABLE���ֵ,���°��ʾ��ֵ
        /// </summary>
        private void Re_readValue()
        {
            DataTable table = new DataTable();
            table.Columns.Add("�ֶ���", typeof(string));
            table.Columns.Add("ֵ", typeof(string));

            Hashtable hs_tb = hs_table_attribute;
            string temps = layer_name + OID;
            char[] sp = { ',' };
            char[] sp1 = { ' ' };
            int count = hs_tb.Count;//����ֵ����Ŀ
            string[] temp = hs_tb[temps].ToString().Substring(0, hs_tb[temps].ToString().Length - 1).Split(sp);//���ȶ��ִ�������Ϊ��β����һ�����š����ȴ��������ٽ��зָ�����Ҫ������
            for (int n = 0; n < temp.Length; n++)
            {
                string[] value = temp[n].Split(sp1);//�ٴεĽ��зָ�õ���������Ҫ������
                DataRow row = table.NewRow();//����һ����
                row[0] = value[0];
                row[1] = value[1];
                table.Rows.Add(row);

            }
            DT_VIEW_Attriubte.DataSource = table;//�����ݰ���ؼ���ʾ
            DT_VIEW_Attriubte.Columns[0].Width = 80;//ȷ���еĿ�
            DT_VIEW_Attriubte.Columns[1].Width = 180;
            int Count = DT_VIEW_Attriubte.Rows.Count - 1;//���һ��
            DT_VIEW_Attriubte.Rows[Count].ReadOnly = true;
            DataGridShow(DT_VIEW_Attriubte);//���ÿ������Եķ���
        }
        /// <summary>
        /// ����Ԫ���ֵ����ʱ�����б༭�޸�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DT_VIEW_Attriubte_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            int count = myHook.MapControl.Map.LayerCount;
            for (int n = 0; n < count; n++)
            {
                ILayer layer = myHook.MapControl.Map.get_Layer(n);
                if (layer == null) continue;
                //�����б𹤳̴򿪵������ǲ���������ʽ��չ��
                if (layer is IGroupLayer)
                {
                    if (layer.Name == "ʾ��ͼ")
                    {
                        continue;
                    }
                    #region �������������ͨ��
                    IWorkspace space = MoData.v_CurWorkspaceEdit as IWorkspace;//�õ���Ӧ�Ĳ����ռ�
                    ICompositeLayer pComLayer = layer as ICompositeLayer;
                    for (int j = 0; j < pComLayer.Count; j++)
                    {
                        ILayer mLayer = pComLayer.get_Layer(j);
                        if (mLayer == null) return;
                        IFeatureLayer layer_space = mLayer as IFeatureLayer;//�õ�Ҫ�ز�
                        IDataset dataset_space = layer_space.FeatureClass as IDataset;//��ת��һ��Ҫ�ؼ���

                        if (!space.Equals(dataset_space.Workspace))
                        {
                            SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "����ͬһ�������ռ��£����ܱ༭��");
                            return;//��������Ŀռ䲻һ���ͽ�����һ��
                        }
                        //////////��SDEͼ�����ƽ��д���
                        string getname = string.Empty;
                        if (layer_space.DataSourceType == "SDE Feature Class")
                            getname = layer_name.Substring(layer_name.IndexOf('.') + 1);
                        else getname = layer_name;

                        if (mLayer.Name == getname)
                        {
                            IFeatureLayer F_layer = mLayer as IFeatureLayer;//��ͼ��ת��Ҫ�ز�

                            try
                            {
                                MoData.v_CurWorkspaceEdit.StartEditOperation();//�༭������ʼ
                                string temp = DT_VIEW_Attriubte.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();//�õ��޸ĺ��ֵ
                                int oid = Convert.ToInt32(OID);//�õ�Ҫ���µ�OID
                                int index = F_layer.FeatureClass.FindField(DT_VIEW_Attriubte.Rows[e.RowIndex].Cells[0].Value.ToString());//�õ����е�����Ҫ����
                                F_layer.FeatureClass.GetFeature(oid).set_Value(index, temp);//����ֵ
                                F_layer.FeatureClass.GetFeature(oid).Store();//���µ�PDB��洢
                                MoData.v_CurWorkspaceEdit.StopEditOperation();//�����༭����
                                UpdateHashTable(temp, e.ColumnIndex, e.RowIndex);
                            }
                            catch (Exception eError)
                            {
                                //******************************************
                                //guozheng added System Exception log
                                if (SysCommon.Log.Module.SysLog == null)
                                    SysCommon.Log.Module.SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
                                SysCommon.Log.Module.SysLog.Write(eError);
                                //******************************************
                                Re_readValue();
                                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "��������������Ͳ���ȷ�����������룡");
                            }
                            //break;
                            return;
                        }
                    }
                    #endregion
                }
                else
                {
                    #region ��������飬ֱ�ӱ���
                    if (layer.Name == layer_name)
                    {
                        IFeatureLayer F_layer = layer as IFeatureLayer;//��ͼ��ת��Ҫ�ز�ռ�
                        IDataset det = F_layer.FeatureClass as IDataset;//ת�����ݼ��������õ��༭�ռ�
                        try
                        {
                            MoData.v_CurWorkspaceEdit.StartEditOperation();//�༭������ʼ
                            string temp = DT_VIEW_Attriubte.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();//�õ��޸ĺ��ֵ
                            int oid = Convert.ToInt32(OID);//�õ�Ҫ���µ�OID
                            F_layer.FeatureClass.GetFeature(oid).set_Value(e.RowIndex, temp);//����ֵ
                            F_layer.FeatureClass.GetFeature(oid).Store();//���µ�PDB��洢
                            MoData.v_CurWorkspaceEdit.StopEditOperation();//�����༭����
                            UpdateHashTable(temp, e.ColumnIndex, e.RowIndex);
                        }
                        catch (Exception eError)
                        {
                            //******************************************
                            //guozheng added System Exception log
                            if (SysCommon.Log.Module.SysLog == null)
                                SysCommon.Log.Module.SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
                            SysCommon.Log.Module.SysLog.Write(eError);
                            //******************************************
                            Re_readValue();
                            SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "��������������Ͳ���ȷ�����������룡");
                        }
                        break;
                    }
                    #endregion
                }
            }
        }
        /// <summary>
        /// �ڹرմ���ʱ����״̬��ԭ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmAttribute_FormClosing(object sender, FormClosingEventArgs e)
        {
            //����ر�ʱ��������״̬��ԭ
            AttributeShow_state.state_value = false;
            AttributeShow_state.show_state = false;
        }
        /// <summary>
        /// ������Ԫ��ʱ���õ�ֵ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DT_VIEW_Attriubte_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            //�ڵ���ʱ���Ȱ�ֵ���������Ա�������Ͳ����޸Ľ����˻�ȥ��ʹ��TRY��ԭ������Ϊ�п��ܵ����ĵط�����
            try
            {
                text = DT_VIEW_Attriubte.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();//�õ��޸�ǰ��ֵ
            }
            catch
            {
                return;
            }
        }
        /// <summary>
        /// ��������PDB���ֵ����ô��ʾ��ʱ��HASHTABLE���ֵҲҪ�޸ģ���Ȼ�ͻ᲻ͬ����ʾ
        /// </summary>
        private void UpdateHashTable(string Content, int index, int row)
        {

            string Value = "";//���ո��º��hashtableֵ
            string temp = layer_name + OID;//�õ�HASHTABLE��KEY
            char[] sp = { ',' };
            char[] sp1 = { ' ' };
            int count = hs_table_attribute.Count;//����ֵ����Ŀ
            string[] temps = hs_table_attribute[temp].ToString().Substring(0, hs_table_attribute[temp].ToString().Length - 1).Split(sp);//���ȶ��ִ�������Ϊ��β����һ�����š����ȴ��������ٽ��зָ�����Ҫ������
            for (int n = 0; n < temps.Length; n++)
            {
                if (n == row)
                {
                    string[] value = temps[n].Split(sp1);//�ٴεĽ��зָ�õ���������Ҫ������
                    Value += value[0] + " " + Content + ",";
                }
                else
                {
                    Value += temps[n] + ",";
                }
            }
            hs_table_attribute[temp] = Value;
        }
        /// <summary>
        /// �����ڵ�ʱ����ʾ��Ӧ��ֵ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeview_Name_NodeClick(object sender, DevComponents.AdvTree.TreeNodeMouseEventArgs e)
        {
            DataTable table = new DataTable();
            table.Columns.Add("�ֶ���", typeof(string));
            table.Columns.Add("ֵ", typeof(string));

            Hashtable hs_tb = hs_table_attribute;
            if (e.Node.Parent == null)
            {
                return;
            }
            else
            {

                layer_name = e.Node.Parent.Text;//����
                OID = e.Node.Text;//OID

                string texts = e.Node.Parent.Text + e.Node.Text;//�ڵ������
                char[] sp = { ',' };
                char[] sp1 = { ' ' };
                int count = hs_tb.Count;//����ֵ����Ŀ
                string[] temp = hs_tb[texts].ToString().Substring(0, hs_tb[texts].ToString().Length - 1).Split(sp);//���ȶ��ִ�������Ϊ��β����һ�����š����ȴ��������ٽ��зָ�����Ҫ������
                for (int n = 0; n < temp.Length; n++)
                {
                    string[] value = temp[n].Split(sp1);//�ٴεĽ��зָ�õ���������Ҫ������
                    DataRow row = table.NewRow();//����һ����
                    row[0] = value[0];
                    row[1] = value[1];
                    table.Rows.Add(row);

                }
                DT_VIEW_Attriubte.DataSource = table;//�����ݰ���ؼ���ʾ
                DT_VIEW_Attriubte.Columns[0].Width = 80;//ȷ���еĿ�
                DT_VIEW_Attriubte.Columns[1].Width = 180;
                int Count = DT_VIEW_Attriubte.Rows.Count - 1;//���һ��
                DT_VIEW_Attriubte.Rows[Count].ReadOnly = true;
                DataGridShow(DT_VIEW_Attriubte);//���ÿ���������ʾ�Ϳ������ɱ༭�ķ���
            }
        }
        /// <summary>
        /// ������ʾ�����ƿɱ༭����
        /// </summary>
        private void DataGridShow(DevComponents.DotNetBar.Controls.DataGridViewX DT_VIEW_Attriubte)
        {
            if (DT_VIEW_Attriubte.Rows.Count > 0)
            {
                ArrayList list = MoData.GetOnlyReadAtt[layer_name] as ArrayList;//�����޸����Եļ���
                DT_VIEW_Attriubte.Columns[0].SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;//���õ�һ�в�������
                DT_VIEW_Attriubte.Columns[1].SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;//�ڶ��в�������
                for (int c = 0; c < DT_VIEW_Attriubte.Rows.Count; c++)
                {
                    DT_VIEW_Attriubte.Rows[c].Height = 18;
                    string value = DT_VIEW_Attriubte.Rows[c].Cells[0].Value.ToString();
                    //�б����Ե������Ƿ����б���
                    if (list.Contains(value))
                    {
                        DT_VIEW_Attriubte.Rows[c].ReadOnly = true;
                    }
                    else
                    {
                        DT_VIEW_Attriubte.Rows[c].Cells[0].ReadOnly = true;//���е��ֶ����ǲ����޸ĵ�
                    }
                }
            }
        }
        private void treeview_Name_MouseDoubleClick(object sender, MouseEventArgs e)
        {


        }
        /// <summary>
        /// ˫���ڵ㣬�Զ�λ��ͼ���ϵ�Ҫ��
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeview_Name_NodeDoubleClick(object sender, DevComponents.AdvTree.TreeNodeMouseEventArgs e)
        {
            AttributeShow_state.doubleclick = true;//ȷ�����������˫����λ����Ӱ��Ҫ�ر��
            if (e.Node.Parent == null)
            {
                return;
            }
            else
            {
                layer_name = e.Node.Parent.Text;//����
                OID = e.Node.Text;//OID

                int Count = myHook.MapControl.Map.LayerCount;//�������
                for (int n = 0; n < Count; n++)
                {
                    ILayer layer = myHook.MapControl.get_Layer(n);
                    if (layer is IGroupLayer)
                    {
                        if (layer.Name == "ʾ��ͼ")
                        {
                            continue;
                        }
                        #region �������������ͨ��
                        ICompositeLayer pComLayer = layer as ICompositeLayer;
                        for (int j = 0; j < pComLayer.Count; j++)
                        {
                            ILayer mLayer = pComLayer.get_Layer(j);
                            if (mLayer.Name == layer_name)
                            {
                                IFeatureLayer f_layer = mLayer as IFeatureLayer;//����ת��Ҫ�ز�
                                int oid = Convert.ToInt32(OID);//��OIDת������
                                IFeature feature = f_layer.FeatureClass.GetFeature(oid);//�õ���Ӧ��Ҫ��
                                IGeoDataset pGeoDt = f_layer.FeatureClass as IGeoDataset;
                                ISpatialReference pSpatialRef = null;
                                if (pGeoDt != null)
                                {
                                    pSpatialRef = pGeoDt.SpatialReference;
                                }

                                //myHook.MapControl.Map.ClearSelection();//�����ѡ���Ҫ��
                                myHook.MapControl.Map.SelectFeature(mLayer, feature);
                                SysCommon.Gis.ModGisPub.ZoomToFeature(myHook.MapControl, feature, pSpatialRef);//��λ����Ӧ�Ĳ�,���ķŴ�
                                myHook.MapControl.ActiveView.Refresh();
                                Application.DoEvents();
                                myHook.MapControl.FlashShape(feature.Shape, 2, 500, null);
                                break;
                            }
                        }
                        #endregion
                    }
                    else
                    {

                        if (layer.Name == layer_name)
                        {
                            IFeatureLayer f_layer = layer as IFeatureLayer;//����ת��Ҫ�ز�
                            int oid = Convert.ToInt32(OID);//��OIDת������
                            IFeature feature = f_layer.FeatureClass.GetFeature(oid);//�õ���Ӧ��Ҫ��
                            myHook.MapControl.Map.SelectFeature(layer, feature);

                            IGeoDataset pGeoDt = f_layer.FeatureClass as IGeoDataset;
                            ISpatialReference pSpatialRef = null;
                            if (pGeoDt != null)
                            {
                                pSpatialRef = pGeoDt.SpatialReference;
                            }

                            SysCommon.Gis.ModGisPub.ZoomToFeature(myHook.MapControl, feature, pSpatialRef);//��λ����Ӧ�Ĳ�,���ķŴ�
                            myHook.MapControl.ActiveView.Refresh();
                            Application.DoEvents();
                            myHook.MapControl.FlashShape(feature.Shape, 2, 500, null);
                            break;
                        }
                    }
                }
            }
        }

        private void DT_VIEW_Attriubte_MouseDoubleClick(object sender, MouseEventArgs e)
        {

        }
    }
}