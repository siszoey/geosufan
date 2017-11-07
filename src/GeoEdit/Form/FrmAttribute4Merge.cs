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
using ESRI.ArcGIS.esriSystem;
namespace GeoEdit
{
    /// <summary>
    /// ���Ե�����
    /// </summary>
    public partial class FrmAttribute4Merge : DevComponents.DotNetBar.Office2007Form
    {
        static string text = "";//������¼�޸�֮ǰ��ֵ
        private Plugin.Application.IAppGISRef myHook;//��һ��MAP�����������������õ�MAP
        static string layer_name = "";//����һ����̬�������������յ�ǰLAYER������
        static string OID = "";//��¼��ǰ������OID��ֵ
        public  Hashtable hs_table_tree = new Hashtable();//������������TREE���õ���ֵ
        public  Hashtable hs_table_attribute = new Hashtable();//������ʾ��Ӧ������ֵ

        /// <summary>
        /// ���ù��캯����ֵ
        /// </summary>
        /// <param name="tb"></param>
        /// <param name="tb_show"></param>
        public FrmAttribute4Merge(Hashtable tb, Hashtable tb_show, Plugin.Application.IAppGISRef Hook)
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
            table.Columns.Add("�ֶ�����", typeof(string));
            table.Columns.Add("ֵ", typeof(string));

            Hashtable hs_tb = hs_table_attribute;

            string texts = layer_name + OID;//ȫƴ��һ��KEY
            char[] sp = { ',' };
            char[] sp1 ={ ' ' };
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
                for (int c = 0; c < Count; c++)
                {
                    DT_VIEW_Attriubte.Rows[c].Height = 18;
                    string value = DT_VIEW_Attriubte.Rows[c].Cells[0].Value.ToString().ToLower();
                    if (value == "objectid" || value == "shape" || value == "shape_length" || value == "shape_area" || value == "element")//ȷ���ؼ��ϲ��ܸ��ĵ�����
                    {
                        DT_VIEW_Attriubte.Rows[c].ReadOnly = true;
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
            table.Columns.Add("�ֶ�����", typeof(string));
            table.Columns.Add("ֵ", typeof(string));

            Hashtable hs_tb = hs_table_attribute;
            string temps = layer_name + OID;
            char[] sp = { ',' };
            char[] sp1 ={ ' ' };
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
            if (DT_VIEW_Attriubte.Rows.Count > 0)
            {
                for (int c = 0; c < Count; c++)
                {
                    DT_VIEW_Attriubte.Rows[c].Height = 18;
                    string value = DT_VIEW_Attriubte.Rows[c].Cells[0].Value.ToString().ToLower();
                    if (value == "objectid" || value == "shape" || value == "shape_length" || value == "shape_area" || value == "element")//ȷ���ؼ��ϲ��ܸ��ĵ�����
                    {
                        DT_VIEW_Attriubte.Rows[c].ReadOnly = true;
                    }
                }
            }
        }
        ///// <summary>
        ///// ����Ԫ���ֵ����ʱ�����б༭�޸�
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void DT_VIEW_Attriubte_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        //{
        //    int count = myHook.MapControl.Map.LayerCount;
        //    for (int n = 0; n < count; n++)
        //    {
        //        ILayer layer = myHook.MapControl.Map.get_Layer(n);
        //        if (layer.Name == layer_name)
        //        {
        //            IFeatureLayer F_layer = layer as IFeatureLayer;//��ͼ��ת��Ҫ�ز�ռ�
        //            IDataset det = F_layer.FeatureClass as IDataset;//ת�����ݼ��������õ��༭�ռ�
        //            WorkspaceEdit = det.Workspace as IWorkspaceEdit;//�õ�ʵ�ʵı༭�ռ�
        //            //����༭�ռ��δ���������ǾͿ���������ѿ������Թ�
        //            if (!WorkspaceEdit.IsBeingEdited())
        //            {
        //                WorkspaceEdit.StartEditing(true);//�����༭
        //            }
        //            try
        //            {
        //                WorkspaceEdit.StartEditOperation();//�༭������ʼ
        //                string temp = DT_VIEW_Attriubte.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();//�õ��޸ĺ��ֵ
        //                int oid = Convert.ToInt32(OID);//�õ�Ҫ���µ�OID
        //                F_layer.FeatureClass.GetFeature(oid).set_Value(e.RowIndex, temp);//����ֵ
        //                F_layer.FeatureClass.GetFeature(oid).Store();//���µ�PDB��洢
        //                WorkspaceEdit.StopEditOperation();//�����༭����
        //                UpdateHashTable(temp, e.ColumnIndex, e.RowIndex);
        //            }
        //            catch (Exception ex)
        //            {
        //                Re_readValue();
        //                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "��������������Ͳ���ȷ�����������룡");
        //            }
        //            break;
        //        }
        //    }
        //}
        ///// <summary>
        ///// �ڹرմ����ǣ���ʾ�ǲ���Ҫ�����޸ĵ�ֵ
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void FrmAttribute_FormClosing(object sender, FormClosingEventArgs e)
        //{
        //    //����ر�ʱ��������״̬��ԭ
        //    AttributeShow_state.state_value = false;
        //    AttributeShow_state.show_state = false;

        //    bool Has_edit = true;
        //    if (WorkspaceEdit != null)
        //    {
        //        WorkspaceEdit.HasEdits(ref Has_edit);//ȷ���༭�ռ��Ƿ����
        //        if (Has_edit)
        //        {
        //            DialogResult dia = MessageBox.Show("���Ƿ�Ҫ���������Ĳ�����", "��ʾ", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk);
        //            if (dia == DialogResult.Yes)
        //            {
        //                WorkspaceEdit.StopEditing(true);
        //            }
        //            else
        //            {
        //                WorkspaceEdit.StopEditing(false);
        //            }
        //        }
        //    }
        //}
        /// <summary>
        /// ������Ԫ��ʱ���õ�ֵ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DT_VIEW_Attriubte_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            text = DT_VIEW_Attriubte.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();//�õ��޸�ǰ��ֵ
        }
        ///// <summary>
        ///// ��������PDB���ֵ����ô��ʾ��ʱ��HASHTABLE���ֵҲҪ�޸ģ���Ȼ�ͻ᲻ͬ����ʾ
        ///// </summary>
        //private void UpdateHashTable(string Content, int index, int row)
        //{

        //    string Value = "";//���ո��º��hashtableֵ
        //    string temp = layer_name + OID;//�õ�HASHTABLE��KEY
        //    char[] sp = { ',' };
        //    char[] sp1 ={ ' ' };
        //    int count = hs_table_attribute.Count;//����ֵ����Ŀ
        //    string[] temps = hs_table_attribute[temp].ToString().Substring(0, hs_table_attribute[temp].ToString().Length - 1).Split(sp);//���ȶ��ִ�������Ϊ��β����һ�����š����ȴ��������ٽ��зָ�����Ҫ������
        //    for (int n = 0; n < temps.Length; n++)
        //    {
        //        if (n == row)
        //        {
        //            string[] value = temps[n].Split(sp1);//�ٴεĽ��зָ�õ���������Ҫ������
        //            Value += value[0] + " " + Content + ",";
        //        }
        //        else
        //        {
        //            Value += temps[n] + ",";
        //        }
        //    }
        //    hs_table_attribute[temp] = Value;
        //}
        /// <summary>
        /// �����ڵ�ʱ����ʾ��Ӧ��ֵ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeview_Name_NodeClick(object sender, DevComponents.AdvTree.TreeNodeMouseEventArgs e)
        {
            DataTable table = new DataTable();
            table.Columns.Add("�ֶ�����", typeof(string));
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
                char[] sp1 ={ ' ' };
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
                    for (int c = 0; c < Count; c++)
                    {
                        DT_VIEW_Attriubte.Rows[c].Height = 18;
                        string value = DT_VIEW_Attriubte.Rows[c].Cells[0].Value.ToString().ToLower();
                        if (value == "objectid" || value == "shape" || value == "shape_length" || value == "shape_area" || value == "element")//ȷ���ؼ��ϲ��ܸ��ĵ�����
                        {
                            DT_VIEW_Attriubte.Rows[c].ReadOnly = true;
                        }
                    }
                }


                //��λ
                UID pUID = new UIDClass();
                pUID.Value = "{40A9E885-5533-11d0-98BE-00805F7CED21}";              //UID for IFeatureLayer
                IEnumLayer pEnumLayer = myHook.MapControl.Map.get_Layers(pUID, true);
                pEnumLayer.Reset();
                ILayer pLayer = pEnumLayer.Next();
                while(pLayer!=null)
                {
                    if (pLayer.Name == layer_name)
                    {
                        IFeatureLayer f_layer = pLayer as IFeatureLayer;//����ת��Ҫ�ز�
                        int oid = Convert.ToInt32(OID);//��OIDת������
                        IFeature feature = f_layer.FeatureClass.GetFeature(oid);//�õ���Ӧ��Ҫ��
                        myHook.MapControl.FlashShape(feature.Shape, 2, 500, null);
                        break;
                    }

                    pLayer = pEnumLayer.Next();
                }
            }
        }

        /// <summary>
        /// ˫���ڵ㣬ȷ��Ҫ������Ҫ��
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
                ControlsMerge.SavedOID = Convert.ToInt32(OID);   //����ѡ���Ҫ��

                this.Close();
            }
        }
    }
}