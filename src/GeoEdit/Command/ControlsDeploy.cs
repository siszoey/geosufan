using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.SystemUI;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.esriSystem;
namespace GeoEdit
{
    public class ControlsDeploy : Plugin.Interface.CommandRefBase
    {
        private Plugin.Application.IAppGISRef myHook;
        Plugin.Application.IAppArcGISRef m_Hook;
        private ITool _tool = null;
        private ICommand _cmd = null;

        public ControlsDeploy()
        {

            base._Name = "GeoEdit.ControlsDeploy";
            base._Caption = "�����޸�";
            base._Tooltip = "�����޸�";
            base._Checked = false;
            base._Visible = true;
            base._Enabled = true;
            base._Message = "�����޸�";

        }

        /// <summary>
        /// �����ںϰ�ť�Ƿ�Ϊ���ã���Map�ϵĴ���ͼ�㣬����ѡ����Ҫ�أ����ұ༭״̬Ϊ��ʱ���ð�ť����
        /// </summary>
        public override bool Enabled
        {
            get
            {
                if (myHook == null) return false;
                if (myHook.MapControl == null) return false;
                if (MoData.v_CurWorkspaceEdit == null) return false;
                return true;
            }
        }
        public override string Message
        {
            get
            {
                Plugin.Application.IAppFormRef pAppFormRef = myHook as Plugin.Application.IAppFormRef;
                if (pAppFormRef != null)
                {
                    pAppFormRef.OperatorTips = base._Message;
                }
                return base._Message;
            }
        }

        public override void ClearMessage()
        {
            Plugin.Application.IAppFormRef pAppFormRef = myHook as Plugin.Application.IAppFormRef;
            if (pAppFormRef != null)
            {
                pAppFormRef.OperatorTips = string.Empty;
            }
        }

        public override void OnClick()
        {
            if (myHook == null) return;
            if (myHook.MapControl == null) return;

            if (_tool == null || _cmd == null || m_Hook == null) return;
            if (m_Hook.MapControl == null) return;

            m_Hook.MapControl.CurrentTool = _tool;
            m_Hook.CurrentTool = this.Name;
            
            
            
        }
        public override void OnCreate(Plugin.Application.IApplicationRef hook)
        {
            if (hook == null) return;
            myHook = hook as Plugin.Application.IAppGISRef;
            if (myHook.MapControl == null) return;
            
            m_Hook = myHook as Plugin.Application.IAppArcGISRef;
            _tool = new ChangsFeature();
            _cmd = _tool as ICommand;
            _cmd.OnCreate(m_Hook);
            


        }

    }
    /// <summary>
    /// ʹ��TOOL���߲��� �������޸ĵ��ʱ���ǿ�ѡ��Ĺ���
    /// </summary>
    public class ChangsFeature:BaseTool
    {
        private IHookHelper m_hookHelper;
        private IMapControlDefault m_MapControl;
        private Plugin.Application.IAppFormRef myHook ;
        private Plugin.Application.IAppGISRef smpdHook;
        public ChangsFeature()
        {
            base.m_category = "GeoCommon";
            base.m_caption = "ChangsFeature";
            base.m_message = "�ƶ�ѡ��Ҫ��";
            base.m_toolTip = "�ƶ�ѡ��Ҫ��";
            base.m_name = base.m_category + "_" + base.m_caption;
            try
            {
                base.m_cursor = new System.Windows.Forms.Cursor(GetType(), "Resources.Select.cur");
            }
            catch (Exception eError)
            {
                //******************************************
                //guozheng added System Exception log
                if (SysCommon.Log.Module.SysLog == null)
                    SysCommon.Log.Module.SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
                SysCommon.Log.Module.SysLog.Write(eError);
                //******************************************
            }
        }

        public override void OnCreate(object hook)
        {
            if (m_hookHelper == null)
                m_hookHelper = new HookHelperClass();
            myHook = hook as Plugin.Application.IAppFormRef;
            Plugin.Application.IAppArcGISRef HookGis = hook as Plugin.Application.IAppArcGISRef;
            smpdHook = hook as Plugin.Application.IAppGISRef;
            m_hookHelper.Hook = HookGis.MapControl;
            m_MapControl = HookGis.MapControl as IMapControlDefault;
            
        }
        public override void OnClick()
        {
        }

        public override void OnMouseDown(int Button, int Shift, int X, int Y)
        {
            if (Button != 1) return;

            //���õ�ѡ���ݲ�
            ISelectionEnvironment pSelectEnv = new SelectionEnvironmentClass();
            double Length = ModPublic.ConvertPixelsToMapUnits(m_hookHelper.ActiveView, pSelectEnv.SearchTolerance);

            IPoint pPoint = m_hookHelper.ActiveView.ScreenDisplay.DisplayTransformation.ToMapPoint(X, Y);
            IGeometry pGeometry = pPoint as IGeometry;
            ITopologicalOperator pTopo = pGeometry as ITopologicalOperator;
            IGeometry pBuffer = pTopo.Buffer(Length);

            //�����ܱ���ཻ����ᱻѡȡ
            pGeometry = m_MapControl.TrackRectangle() as IGeometry;
            bool bjustone = true;
            if (pGeometry != null)
            {
                if (pGeometry.IsEmpty)
                {
                    pGeometry = pBuffer;
                }
                else
                {
                    bjustone = false;
                }
            }
            else
            {
                pGeometry = pBuffer;
            }

            UID pUID = new UIDClass();
            pUID.Value = "{40A9E885-5533-11d0-98BE-00805F7CED21}";   //UID for IFeatureLayer
            IEnumLayer pEnumLayer = m_MapControl.Map.get_Layers(pUID, true);
            pEnumLayer.Reset();
            ILayer pLayer = pEnumLayer.Next();
            while (pLayer != null)
            {
                if (pLayer.Visible == false)
                {
                    pLayer = pEnumLayer.Next();
                    continue;
                }
                IFeatureLayer pFeatureLayer = pLayer as IFeatureLayer;
                if (pFeatureLayer.Selectable == false)
                {
                    pLayer = pEnumLayer.Next();
                    continue;
                }

                GetSelctionSet(pFeatureLayer, pGeometry, bjustone, Shift);

                pLayer = pEnumLayer.Next();
            }

            //����Mapѡ�����仯�¼�
            ISelectionEvents pSelectionEvents = m_hookHelper.FocusMap as ISelectionEvents;
            pSelectionEvents.SelectionChanged();

            //ˢ��
            m_hookHelper.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeoSelection, null, m_hookHelper.ActiveView.Extent);
            AttributeShow();
        }
        private void GetSelctionSet(IFeatureLayer pFeatureLayer, IGeometry pGeometry, bool bjustone, int Shift)
        {
            IFeatureClass pFeatureClass = pFeatureLayer.FeatureClass;
            //û�����༭�Ĳ���ѡ��
            IDataset pDataset = pFeatureClass as IDataset;
            IWorkspaceEdit pWorkspaceEdit = pDataset.Workspace as IWorkspaceEdit;
            if (!pWorkspaceEdit.IsBeingEdited()) return;
            switch (Shift)
            {
                case 1:   //����ѡ������
                    ModPublic.GetSelctionSet(pFeatureLayer, pGeometry, pFeatureClass, esriSelectionResultEnum.esriSelectionResultAdd, bjustone);
                    break;
                case 4:   //����ѡ������
                    ModPublic.GetSelctionSet(pFeatureLayer, pGeometry, pFeatureClass, esriSelectionResultEnum.esriSelectionResultSubtract, bjustone);
                    break;
                case 2:
                    ModPublic.GetSelctionSet(pFeatureLayer, pGeometry, pFeatureClass, esriSelectionResultEnum.esriSelectionResultXOR, bjustone);
                    break;
                default:   //�½�ѡ������
                    ModPublic.GetSelctionSet(pFeatureLayer, pGeometry, pFeatureClass, esriSelectionResultEnum.esriSelectionResultNew, bjustone);
                    break;
            }
        }


        public override void OnMouseUp(int Button, int Shift, int X, int Y)
        {
        }

        public override void OnMouseMove(int Button, int Shift, int X, int Y)
        {
        }
        //���߲�����ʱ�ͷŴ���ȱ���
        public override bool Deactivate()
        {
            return true;
        }
        /// <summary>
        /// ��ʾ���� ���������
        /// </summary>
        public void AttributeShow()
        {
            EnterSelectValueCount();//����ȷ����ͼ��û��ѡ���Ҫ��
            ///�����ǵ����Դ���û����ʾ��������ʾ������͸�������
            if (AttributeShow_state.state_value)
            {
                if (!AttributeShow_state.show_state)
                {
                    AttributeShow_state.Temp_frm = new FrmAttribute(GetData_tree(), GetData_View(), smpdHook);//��һ��ֵ�����嵱��
                    AttributeShow_state.Temp_frm.Owner = myHook.MainForm;
                    AttributeShow_state.Temp_frm.ShowInTaskbar = false;
                    AttributeShow_state.Temp_frm.Show();
                    AttributeShow_state.show_state = true;//��������״̬
                    ControlsDeploy d = new ControlsDeploy();
                    smpdHook.ArcGisMapControl.OnSelectionChanged += new EventHandler(Select);
                }
                
            }
        }
        /// <summary>
        /// MAP��ѡ�񼯸���ʱ�������¼�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Select(object sender, EventArgs e)
        {
            if (AttributeShow_state.show_state == true && AttributeShow_state.state_value == true)
            {
                Hashtable hs_tree = GetData_tree();//��TREE��ֵ
                Hashtable hs_show = GetData_View();//����ʾ�ؼ�����ֵ

                //��MAP�ϣ����ܻ�ѡ��һ���յ�Ҫ�أ������б����Ƿ�Ϊ��
                if (hs_show != null && hs_tree != null)
                {
                    AttributeShow_state.Temp_frm.hs_table_tree = hs_tree;//�������޸Ĵ������¸�ֵ treeֵ
                    AttributeShow_state.Temp_frm.hs_table_attribute = hs_show;//�������޸Ĵ������¸�ֵ attributeֵ
                    AttributeShow_state.Temp_frm.DataMain();
                }
                else
                {
                    AttributeShow_state.Temp_frm.DT_VIEW_Attriubte.DataSource = null;//�����������ֵΪ�վ͸����帳ֵһ����
                    AttributeShow_state.Temp_frm.treeview_Name.Nodes.Clear();//������ϵĽڵ�
                }

            }
        }

        /// <summary>
        /// ȷ����ͼ���Ƿ���ѡ���Ҫ��
        /// </summary>
        private void EnterSelectValueCount()
        {
            IEnumFeature features = smpdHook.ArcGisMapControl.Map.FeatureSelection as IEnumFeature;
            features.Reset();
            IFeature feature = features.Next();
            if (feature != null)
            {
                AttributeShow_state.state_value = true;//ȷ��������ѡ�е�ֵ
            }
        }

        /// <summary>
        /// ��MAP�ϵõ�ѡ���Ҫ������
        /// <para>��ʽ�磺name oid������NAME��ָ��Ҫ��������֣�OIDָ����Ҫ�ص�ID</para>
        /// <remarks>��Ҫ������������ϵ���</remarks>
        /// </summary>
        private Hashtable GetData_tree()
        {
            Hashtable table = new Hashtable();//����һ��KEY VALUE
            IEnumFeature f = m_hookHelper.FocusMap.FeatureSelection as IEnumFeature;//�е���ѡ���Ҫ�ص����ݼ�
            f.Reset();
            IFeature feature = f.Next();
            string name = "";
            string oid = "";
            #region ͨ���������KEYΪ��Ҫ��������VALUEΪ��OID�����Ҫ��������KEY�У���ô���ۼӵ�֮ǰ��ֵ����
            while (feature != null)
            {
                IObjectClass obj = feature.Class;
                name = feature.Class.AliasName;
                oid = feature.OID.ToString();
                if (table.Count == 0)
                {
                    table.Add(name, oid);
                }
                else
                {
                    if (table.ContainsKey(name))
                    {
                        string temp = table[name].ToString() + " " + oid;
                        table[name] = temp;
                    }
                    else
                    {
                        table.Add(name, oid);
                    }
                }
                feature = f.Next();

            }
            #endregion

            int K_count = table.Count;//�ж������ֵ��ǲ��ǿյ�
            if (K_count == 0)
            {
                table = null;
            }
            return table;
        }
        /// <summary>
        /// �õ���ѡ���Ҫ�ز����޸ĵ��ֶ�
        /// </summary>
        /// <param name="layer"></param>
        /// <param name="feature"></param>
        private void GetOnlyReadAttribute(string layer, IFeature feature)
        {
            ArrayList list = new ArrayList();//��Ų����޸ĵ�������
            IFields fields = feature.Fields;
            for (int n = 0; n < fields.FieldCount; n++)
            {
                IField field = feature.Fields.get_Field(n);
                string name = field.Name;
                if (field.Editable && field.Type != esriFieldType.esriFieldTypeGeometry && field.Type != esriFieldType.esriFieldTypeOID)
                {
                    continue;
                }
                else
                {
                    list.Add(name);
                }
            }
            MoData.GetOnlyReadAtt.Add(layer, list);//�����ֺ������б�����


        }
        /// <summary>
        /// ��Ҫ��������ʾ��ӦOID������ֵ ��������ʾ�ؼ��ṩ����
        /// <para>key=name+oid value=field +value</para>
        /// <remarks>���������ֵ䣬��Ҫ��������ƺ͵�����OID��Ϊһ��KEY��Ȼ���OID����ֶκ�ֵȡ��������ֵ</remarks>
        /// </summary>
        /// <returns></returns>
        private Hashtable GetData_View()
        {

            Hashtable table = new Hashtable();
            IEnumFeature f_dataset = m_hookHelper.FocusMap.FeatureSelection as IEnumFeature;//ȡ�õ�ͼ�ؼ��ϱ�ѡ���Ҫ�ؼ�

            f_dataset.Reset();
            IFeature feature = f_dataset.Next();//ȡ����һ��Ҫ��
            string name = "";
            string oid = "";
            #region ������ӦҪ�����¶�ӦOIDҪ�صļ�¼ֵ
            while (feature != null)
            {
                IDataset ds = feature.Class as IDataset;
                name = ds.Name;
                if (MoData.GetOnlyReadAtt == null)
                {
                    GetOnlyReadAttribute(name, feature);//����ֻ���Եķ���
                }
                else
                {
                    if (!MoData.GetOnlyReadAtt.ContainsKey(name))
                    {
                        GetOnlyReadAttribute(name, feature);//����ֻ���Եķ���
                    }
                }
                oid = feature.OID.ToString();

                string key = name + oid;//ʹ��KEYֵ,��Ҫ��������Ƽ��ϵ�����OID��ϳ�һ��KEY
                string Value = "";//ʹ��VALUE

                string shape = "";//ȷ��Ҫ�ص�SHAPE��ʲô
                #region �õ�Ҫ�ص�SHAPE���ͣ�ע�ǣ��棬�ߣ���
                if (feature.FeatureType == esriFeatureType.esriFTAnnotation)
                {
                    shape = "ע��";
                }
                else
                {
                    IGeometry geometry = feature.Shape;//�õ�Ҫ�صļ���ͼ��

                    switch (geometry.GeometryType.ToString())//ȷ�����ļ���Ҫ������
                    {
                        case "esriGeometryPolygon":
                            shape = "��";
                            break;
                        case "esriGeometryPolyline":
                            shape = "��";
                            break;
                        case "esriGeometryPoint":
                            shape = "��";
                            break;
                    }
                }
                #endregion

                IFields fields = feature.Fields;
                int k = fields.FieldCount;
                #region �˱�����Ҫ�������õ�һ����Ҫ�صļ�¼������������ϳ�һ��VALUE
                for (int s = 0; s < k; s++)
                {

                    IField field = fields.get_Field(s);
                    string str = "";
                    string f_value = feature.get_Value(s).ToString();//�õ���Ӧ��ֵ
                    if (field.Name.ToLower() == "shape")
                    {
                        str = field.Name + " " + shape;//�õ���Ӧ��SHAPE����
                    }
                    else if (field.Name.ToLower() == "element")
                    {
                        str = field.Name + " blob";
                    }
                    else
                    {
                        if (f_value != string.Empty)
                        {
                            str = field.Name + " " + f_value;
                        }
                        else
                        {
                            str = field.Name + " " + "null";
                        }
                    }
                    Value += str + ",";
                }
                #endregion
                #region ��KEY��VALUE���뵽�����ֵ䵱�У��Ա�����ʹ��
                table.Add(key, Value);
                #endregion
                feature = f_dataset.Next();

            }
            #endregion

            if (table.Count == 0)
            {
                table = null;
            }
            return table;
        }
    }
}
