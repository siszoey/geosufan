using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Windows.Forms;

using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ADF.CATIDs;

using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.SystemUI;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.esriSystem;

namespace GeoEdit
{
    /// <summary>
    /// ����ˢ
    /// </summary>
    public class ControlsAttribute : Plugin.Interface.ToolRefBase
    {
        private Plugin.Application.IAppGISRef myHook;
        private ControlsAttributeCopy _ControlsAttributeCopy;
        private ITool _tool = null;
        private ICommand _cmd = null;

        public static IFeatureClass m_CurFeatCls;         //��ǰԴҪ�ص�featureclass

        /// <summary>
        /// ���캯�� �����ؼ��Ƿ���ʾ����ص�����˵��
        /// </summary>
        public ControlsAttribute()
        {

            base._Name = "GeoEdit.ControlsAttribute";
            base._Caption = "����ˢ";
            base._Tooltip = "����ˢ";
            base._Checked = false;
            base._Visible = true;
            base._Enabled = true;
            base._Message = "����ˢ";


        }

        /// <summary>
        /// �����ںϰ�ť�Ƿ�Ϊ���ã���Map�ϵĴ���ͼ�㣬����ѡ����Ҫ�أ����ұ༭״̬Ϊ��ʱ���ð�ť����
        /// </summary>
        public override bool Enabled
        {
            get
            {
                if (myHook == null) return false;
                if (_ControlsAttributeCopy == null) return false;
                if (myHook.MapControl == null) return false;
                if (MoData.v_CurWorkspaceEdit == null) return false;
                return true;
            }
        }

        public override bool Checked
        {
            get
            {
                if (_ControlsAttributeCopy == null) return false;
                if (_ControlsAttributeCopy.Exit == true)
                {
                    myHook.CurrentTool = null;
                    return false;
                }
                if (myHook.CurrentTool != this.Name) return false;
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
        /// <summary>
        /// ��Ҫ�������
        /// </summary>
        public override void OnClick()
        {
            if (myHook == null) return;
            if (myHook.MapControl == null) return;

            if (myHook.MapControl.Map.SelectionCount == 1)
            {
                AttributeShow_state.hs_Feature = new Hashtable();
                GetMapFeature();//�õ�ԴҪ��
            }
            else
            {
                myHook.CurrentTool = null;
                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "��ѡ��ο�Ҫ��,��������Ϊ1��");
                return;
            }

            myHook.MapControl.CurrentTool = _tool;
            myHook.CurrentTool = this.Name;
        }

        /// <summary>
        /// ��ʼ�������õ�����������Ŀؼ�
        /// </summary>
        /// <param name="hook"></param>
        public override void OnCreate(Plugin.Application.IApplicationRef hook)
        {
            if (hook == null) return;
            myHook = hook as Plugin.Application.IAppGISRef;
            if (myHook.MapControl == null) return;
            _ControlsAttributeCopy = new ControlsAttributeCopy();
            _tool = _ControlsAttributeCopy as ITool;
            _cmd = _tool as ICommand;
            _cmd.OnCreate(myHook.MapControl);

        }

        /// <summary>
        /// ��ȡMAP��ѡ���ԴҪ��
        /// </summary>
        private void GetMapFeature()
        {

            AttributeShow_state.state_brush = true;//��ȷ����ѡ��һ��Ҫ��Դʱ���Ÿı�״̬

            IEnumFeature IEnum_dataset = myHook.MapControl.Map.FeatureSelection as IEnumFeature;//�е���ѡ���Ҫ�ص����ݼ�
            IEnum_dataset.Reset();
            IFeature Feature = IEnum_dataset.Next();
            IWorkspace space = MoData.v_CurWorkspaceEdit as IWorkspace;//�õ���Ӧ�Ĳ����ռ�
            while (Feature != null)
            {
                m_CurFeatCls = Feature.Class as IFeatureClass;
                IDataset dataset_space = m_CurFeatCls as IDataset;//��ת��һ��Ҫ�ؼ���
                if (!space.Equals(dataset_space.Workspace))
                {
                    SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "�༭�ռ䲻һ�£�������ѡ��Ҫ�أ�");
                    myHook.MapControl.Map.ClearSelection();
                    return;
                    
                }

                string shape = "";//ȷ��Ҫ�ص�SHAPE��ʲô
                string Name = Feature.Class.AliasName;//����
                AttributeShow_state.OID = Feature.OID.ToString();//�õ�ԴOID
                #region �õ�Ҫ�ص�SHAPE���ͣ�ע�ǣ��棬�ߣ���
                if (Feature.FeatureType == esriFeatureType.esriFTAnnotation)
                {
                    shape = "ע��";
                }
                else
                {
                    IGeometry geometry = Feature.Shape;//�õ�Ҫ�صļ���ͼ��

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

                IFields fields = Feature.Fields;
                int count = fields.FieldCount;//�õ���Ҫ�ع��ж��ٸ�����
                AttributeShow_state.feature_count = count;//��ԴҪ�ص��ֶ�����ֵ���ܵ�ȫ����
                string value = "";//���ո�Ҫ�ص�ֵ
                for (int n = 0; n < count; n++)
                {
                    string name = fields.get_Field(n).Name.ToLower();
                    if (name == "shape")
                    {
                        value += shape + ",";
                    }
                    else
                    {
                        string F_value = Feature.get_Value(n).ToString();
                        if (F_value == string.Empty)
                        {
                            F_value = "null";
                        }
                        value += name + " " + F_value + ",";
                    }
                }

                string processStr = value.Substring(0, value.Length - 1);

                AttributeShow_state.hs_Feature.Add(Name, processStr);

                Feature = IEnum_dataset.Next();
            }
            myHook.MapControl.Map.ClearSelection();
            myHook.MapControl.ActiveView.Refresh();

        }
    }


    public class ControlsAttributeCopy : BaseTool
    {
        private IHookHelper m_hookHelper;
        private IMapControlDefault m_MapControl;

        private bool m_bExit;
        public bool Exit
        {
            get
            {
                return m_bExit;
            }
        }

        //��ķ���
        public ControlsAttributeCopy()
        {
            base.m_category = "GeoCommon";
            base.m_caption = "AttributeCopy";
            base.m_message = "����ˢ";
            base.m_toolTip = "����ˢ";
            base.m_name = base.m_category + "_" + base.m_caption;
            try
            {
                
                base.m_cursor = new System.Windows.Forms.Cursor(GetType(), "Resources.AttributeCopy.cur");
            }
            catch(Exception eError)
            {
                //******************************************
                //guozheng added System Exception log
                if (SysCommon.Log.Module.SysLog == null)
                    SysCommon.Log.Module.SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
                SysCommon.Log.Module.SysLog.Write(eError);
                //******************************************
            }
        }

        #region Overriden Class Methods

        /// <summary>
        /// Occurs when this tool is created
        /// </summary>
        /// <param name="hook">Instance of the application</param>
        public override void OnCreate(object hook)
        {
            if (m_hookHelper == null)
                m_hookHelper = new HookHelperClass();

            m_hookHelper.Hook = hook;
            m_MapControl = hook as IMapControlDefault;
        }

        /// <summary>
        /// Occurs when this tool is clicked
        /// </summary>
        public override void OnClick()
        {
            //֧�ֿ�ݼ�
            m_MapControl.KeyIntercept = 1;  //esriKeyInterceptArrowKeys
        }

        public override void OnKeyUp(int keyCode, int Shift)
        {
            if (m_MapControl.Map.SelectionCount > 0)
            {
                AttributeShow_state.state_brush = false;//ֻ�е���ѡ���ʱ��Ż��״̬�Ļ���
            }
            if (!AttributeShow_state.state_brush)//�����б��Ƿ�ѡ����ԴҪ��
            {
                if (keyCode == 13)   //���б�ǰ�ǲ��ǵ������ǻس�
                {
                    CopyAttribute();//����Ҫ�����Ե�COPY
                }
            }

            if (keyCode == 27) //�˳�
            {
                m_MapControl.Map.ClearSelection();
                m_MapControl.ActiveView.Refresh();
                m_MapControl.CurrentTool = null;
                m_bExit = true;
            }
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
                IFeatureLayer pFeatureLayer = pLayer as IFeatureLayer;
                IFeatureClass pFeatureClass = pFeatureLayer.FeatureClass;

                if (pFeatureClass.ObjectClassID != ControlsAttribute.m_CurFeatCls.ObjectClassID)
                {
                    pLayer = pEnumLayer.Next();
                    continue;
                }

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

                pLayer = pEnumLayer.Next();
            }

            //ˢ��
            m_hookHelper.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeoSelection, null, m_hookHelper.ActiveView.Extent);
        }

        public override void OnMouseUp(int Button, int Shift, int X, int Y)
        {

        }

        public override void OnMouseMove(int Button, int Shift, int X, int Y)
        {
        }

        public override void OnDblClick()
        {

        }
        public override bool Deactivate()
        {
            return true;
        }
        #endregion

        /// <summary>
        /// ��ԴҪ�ص�����COPY��ͬ��ѡ�е�Ҫ��
        /// </summary>
        private void CopyAttribute()
        {

            char[] sp ={ ' ' };//�Կո�ָ�
            char[] sp1 ={ ',' };//�Զ��ŷָ�
            string Feature_value = "";
            foreach (DictionaryEntry de in AttributeShow_state.hs_Feature)
            {
                Feature_value = de.Value.ToString();//�õ�ԴOID������ֵ
            }
            string[] Field = Feature_value.Split(sp1);//�Զ��ŷָ�

            IEnumFeature F_dateset = m_MapControl.Map.FeatureSelection as IEnumFeature;//�õ�MAP��ѡ�е�Ҫ�ؼ�
            F_dateset.Reset();
            IFeature Feature = F_dateset.Next();//ȡ����һ��Ҫ��
            if (Feature == null)
            {
                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "��û��ѡ���κ�Ҫ�޸ĵ�Ŀ��Ҫ�����ԣ�");
                return;
            }
            MoData.v_CurWorkspaceEdit.StartEditOperation();//�����༭����
            while (Feature != null)
            {
                IFeatureClass F_class = Feature.Class as IFeatureClass;

                //MoData.v_CurWorkspaceEdit.RedoEditOperation();//�����ɻ���
                if (Feature.FeatureType != esriFeatureType.esriFTAnnotation)//�б���ע��ʱ��������
                {

                    //COPYʱ����ԴҪ�����Գ���
                    if (Feature.OID.ToString() != AttributeShow_state.OID)
                    {
                        for (int n = 0; n < AttributeShow_state.feature_count; n++)
                        {

                            string value = Feature.Fields.get_Field(n).Name.ToLower();
                            if (value == "objectid" || value == "shape" || value == "shape_length" || value == "shape_area" || value == "element")//ȷ���ؼ��ϲ��ܸ��ĵ�����
                            {
                                continue;//�������Щ�̶�������ֶ���ô����ֵ�ǲ���COPY������һ��һ���ֶθ�ֵ
                            }
                            else
                            {
                                string[] tempStr = Field[n].Split(sp);//�Կո�ָ�õ��ֶ�
                                string Value = tempStr[1];//ֵ
                                Feature.set_Value(n, Value);

                            }
                        }
                    }
                }
                Feature.Store();

                Feature = F_dateset.Next();//������һ��Ҫ��
            }
            MoData.v_CurWorkspaceEdit.StopEditOperation();//�����༭����

            SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "�޸ĳɹ���");
            m_MapControl.Map.ClearSelection();//���MAP�ϵ�ѡ��
            m_MapControl.ActiveView.Refresh();
            AttributeShow_state.state_brush = true;//ȷ�������Ѳ���������ˢ�����ı�״̬���ú���Ĳ������Խ��в�����

        }
    }
}
