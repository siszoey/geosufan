using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using System.Xml;
using System.IO;
using System.Windows.Forms;
using ESRI.ArcGIS.DataSourcesGDB;
using System.Data;
using ESRI.ArcGIS.Display;

namespace GeoDataChecker
{
    /// <summary>
    /// ��˫���¼���ί���¼����岻ͬ��ִ�з�������ʵ�ֲ�ͬ�����µĶ�λ����
    /// ��д�ˣ�����
    /// </summary>
    public class Overridfunction
    {
        public static Plugin.Application.IAppGISRef _AppHk;
        static ILayer m_CurLayer = null;//����һ����ı���
        public static string name = "";
        public static void DataCheckGridDoubleClick(object sender, MouseEventArgs e)
        {
            DataGridView view = (DataGridView)sender;
            if (view.RowCount == 0) return;
            if (view.SelectedCells.Count == 0) return;
            switch (name)
            {
                case "�Խ���":
                    LineSelf(sender);
                    break;
                case "���ظ�":
                    DoubleLine(sender);
                    break;
                case "�ظ���":
                    DoublePoint(sender);
                    break;
                case "���ظ�":
                    DoublePolygon(sender);
                    break;
                case "����ͼ��":
                    CodeLayer(sender);
                    break;
                case "�����׼":
                    Code(sender);
                    break;
                case "���ҵ�":
                    HangPoint_click(sender);
                    break;
                case "�ӱ߼��":
                    JoinCheck(sender);
                    break;

            }
        }

        /// <summary>
        /// ˫���¼���˫���󣬶�λ��ָ���Ĳ�Ҫ�� NAME+������+OID+�� �ӱ߼��
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void JoinCheck(object sender)
        {
            DataGridView view = (DataGridView)sender;//����ǰ�����Ķ���ת�ɿؼ������Է������
            string temp = view.CurrentRow.Cells[0].Value.ToString();//�õ�����еĵ�һ����Ԫ������
            int DesID = Convert.ToInt32(view.CurrentRow.Cells[1].Value.ToString());//ѡ���еĵڶ�����Ԫ��
            IPoint pErrPnt = view.CurrentRow.Cells[2].Value as IPoint;
            string className = "";//Ҫ���������
            int OrginID = 0;//ԴҪ�����Ҫ��ID

            char[] p = { ' ' };//�Կո�Ϊ�ָ��
            char[] s ={ '��' };//�ԣ�Ϊ�ָ��
            string[] para = temp.Split(p);
            string[] org = para[0].Split(s);//�õ�ԴID��
            className = org[0];
            OrginID = Convert.ToInt32(org[1]);

            IFeature fu = null;//�õ�ԴҪ��
            IFeature Des_feature = null;//Ŀ��Ҫ��
            int n = _AppHk.MapControl.Map.LayerCount;
            if (n == 0) return;
            //�����ҳ�����ָ����
            for (int S = 0; S < n; S++)
            {
                ILayer layer = _AppHk.MapControl.Map.get_Layer(S);
                //�б��ǲ����飬����ǣ��ʹ�����ȡһ����
                if (layer is IGroupLayer)
                {
                    if (layer.Name == "�����ޱ�����" || layer.Name == "����������" || layer.Name == "���ƿ�����")
                    {
                        ICompositeLayer C_layer = layer as ICompositeLayer;//�õ����ͼ��
                        for (int c = 0; c < C_layer.Count; c++)
                        {
                            ILayer temp_layer = C_layer.get_Layer(c);
                            IFeatureLayer F_layer = temp_layer as IFeatureLayer;
                            IDataset set = F_layer.FeatureClass as IDataset;
                            if (className == set.Name)
                            {
                                m_CurLayer = temp_layer;
                                fu = F_layer.FeatureClass.GetFeature(OrginID);//�õ�Ҫ��
                                Des_feature = F_layer.FeatureClass.GetFeature(DesID);//Ŀ��Ҫ��
                                break;
                            }

                        }
                    }
                }

            }
            if (fu != null && Des_feature != null)
            {
                _AppHk.MapControl.Map.ClearSelection();//ÿ�ν���ǰ�����֮ǰѡ�����
                _AppHk.MapControl.Map.SelectFeature(m_CurLayer, fu);//�ڶ�Ӧ�Ĳ���ѡ��ָ����Ԫ��
                _AppHk.MapControl.Map.SelectFeature(m_CurLayer, Des_feature);//�ڶ�Ӧ�Ĳ���ѡ��ָ����Ԫ��
                //SysCommon.Gis.ModGisPub.ZoomToFeature(_AppHk.MapControl, fu);//��λ����Ӧ�Ĳ�

                ITopologicalOperator pTop = pErrPnt as ITopologicalOperator;
                IGeometry pGeometry = pTop.Buffer(30);
                IEnvelope pEnvelope = pGeometry.Envelope;

                if (pEnvelope == null) return;
                pEnvelope.Expand(12, 0, false);
                IActiveView pActiveView = _AppHk.MapControl.Map as IActiveView;
                pActiveView.Extent = pEnvelope;
                pActiveView.Refresh();
                Application.DoEvents();
                //������Ӧ�ĵ�
                if (pErrPnt != null)
                {
                    _AppHk.MapControl.ActiveView.ScreenDisplay.StartDrawing(_AppHk.MapControl.ActiveView.ScreenDisplay.hDC, (short)esriScreenCache.esriNoScreenCache);
                    esriSimpleMarkerStyle pStyle = esriSimpleMarkerStyle.esriSMSCircle;
                    _AppHk.MapControl.ActiveView.ScreenDisplay.SetSymbol(SetSnapSymbol(pStyle) as ISymbol);
                    _AppHk.MapControl.ActiveView.ScreenDisplay.DrawPoint(pErrPnt);
                    _AppHk.MapControl.ActiveView.ScreenDisplay.FinishDrawing();
                }
            }
        }

        //���û���׽ʱ��ķ���
        private static ISimpleMarkerSymbol SetSnapSymbol(esriSimpleMarkerStyle pStyle)
        {
            ISimpleMarkerSymbol pMarkerSymbol = new SimpleMarkerSymbolClass();
            ISymbol pSymbol = pMarkerSymbol as ISymbol;

            IRgbColor pRgbColor = new RgbColorClass();
            pRgbColor.Transparency = 0;
            //�������ʽ���ƣ�������ǰ���ķ���
            pSymbol.ROP2 = esriRasterOpCode.esriROPXOrPen;
            pMarkerSymbol.Color = pRgbColor;
            pMarkerSymbol.Style = pStyle;

            //������������ʽ
            pRgbColor.Red = 255;
            pRgbColor.Blue = 0;
            pRgbColor.Green = 0;
            pRgbColor.Transparency = 230;
            pMarkerSymbol.Outline = true;
            pMarkerSymbol.OutlineColor = pRgbColor;
            pMarkerSymbol.OutlineSize = 1;
            pMarkerSymbol.Size = 12;

            return pMarkerSymbol;
        }
        /// <summary>
        /// �����ཻ
        /// </summary>
        /// <param name="sender"></param>
        private static void LineSelf(object sender)
        {
            DataGridView view = (DataGridView)sender;//����ǰ�����Ķ���ת�ɿؼ������Է������
            string temp = view.SelectedCells[0].Value.ToString();//�õ�����еĵ�һ����Ԫ������
            string className = "";//Ҫ���������
            int OrginID = 0;//ԴҪ�����Ҫ��ID

            char[] p = { ' ' };//�Կո�Ϊ�ָ��
            char[] s ={ '��' };//�ԣ�Ϊ�ָ��
            string[] para = temp.Split(p);
            className = para[0];//����
            string[] org = para[1].Split(s);//�õ�ԴID��
            OrginID = Convert.ToInt32(org[1]);//ԴID
            IFeature fu = null;//�õ�Ҫ��
            int n = _AppHk.MapControl.Map.LayerCount;
            if (n == 0) return;
            //�����ҳ�����ָ����
            for (int S = 0; S < n; S++)
            {
                ILayer layer = _AppHk.MapControl.Map.get_Layer(S);
                //�б��ǲ����飬����ǣ��ʹ�����ȡһ����
                if (layer is IGroupLayer)
                {
                    if (layer.Name == "�����ޱ�����" || layer.Name == "����������" || layer.Name == "���ƿ�����")
                    {
                        ICompositeLayer C_layer = layer as ICompositeLayer;//�õ����ͼ��
                        for (int c = 0; c < C_layer.Count; c++)
                        {
                            ILayer temp_layer = C_layer.get_Layer(c);
                            IFeatureLayer F_layer = temp_layer as IFeatureLayer;
                            IDataset set = F_layer.FeatureClass as IDataset;
                            if (className == set.Name)
                            {
                                m_CurLayer = temp_layer;
                                //IFeatureLayer f_layer = temp_layer as IFeatureLayer;//ת�ɶ�Ӧ��Ҫ�ز�
                                fu = F_layer.FeatureClass.GetFeature(OrginID);//�õ�Ҫ��
                                break;
                            }

                        }
                    }
                }

            }
            if (fu != null)
            {
                _AppHk.MapControl.Map.ClearSelection();
                _AppHk.MapControl.Map.SelectFeature(m_CurLayer, fu);//�ڶ�Ӧ�Ĳ���ѡ��ָ����Ԫ��

                SysCommon.Gis.ModGisPub.ZoomToFeature(_AppHk.MapControl, fu);//��λ����Ӧ�Ĳ�
            }
        }
        /// <summary>
        /// ���ظ�
        /// </summary>
        /// <param name="sender"></param>
        private static void DoubleLine(object sender)
        {
            DataGridView view = (DataGridView)sender;//����ǰ�����Ķ���ת�ɿؼ������Է������
            string temp = view.SelectedCells[0].Value.ToString();//�õ�����еĵ�һ����Ԫ������
            string className = "";//Ҫ���������
            int OrginID = 0;//ԴҪ�����Ҫ��ID
            int DestID = 0;//Ŀ��Ҫ��ID

            char[] p = { ' ' };//�Կո�Ϊ�ָ��
            char[] s ={ '��' };//�ԣ�Ϊ�ָ��
            string[] para = temp.Split(p);
            className = para[0];//����
            string[] org = para[1].Split(s);//�õ�ԴID��
            OrginID = Convert.ToInt32(org[1]);//ԴID
            string[] des = para[2].Split(s);//Ŀ��ԴID��
            DestID = Convert.ToInt32(des[1]);//Ŀ��ID
            IFeature fu = null;//�õ�Ҫ��
            int n = _AppHk.MapControl.Map.LayerCount;
            if (n == 0) return;
            //�����ҳ�����ָ����
            for (int S = 0; S < n; S++)
            {
                ILayer layer = _AppHk.MapControl.Map.get_Layer(S);
                //�б��ǲ����飬����ǣ��ʹ�����ȡһ����
                if (layer is IGroupLayer)
                {
                    if (layer.Name == "�����ޱ�����" || layer.Name == "����������" || layer.Name == "���ƿ�����")
                    {
                        ICompositeLayer C_layer = layer as ICompositeLayer;//�õ����ͼ��
                        for (int c = 0; c < C_layer.Count; c++)
                        {

                            ILayer temp_layer = C_layer.get_Layer(c);
                            IFeatureLayer F_layer = temp_layer as IFeatureLayer;
                            IDataset set = F_layer.FeatureClass as IDataset;
                            if (className == set.Name)
                            {
                                m_CurLayer = temp_layer;
                                fu = F_layer.FeatureClass.GetFeature(OrginID);//�õ�Ҫ��
                                break;
                            }

                        }
                    }
                }

            }
            if (fu != null)
            {
                _AppHk.MapControl.Map.ClearSelection();//ÿ�ν���ǰ�����֮ǰѡ�����
                _AppHk.MapControl.Map.SelectFeature(m_CurLayer, fu);//�ڶ�Ӧ�Ĳ���ѡ��ָ����Ԫ��
                SysCommon.Gis.ModGisPub.ZoomToFeature(_AppHk.MapControl, fu);//��λ����Ӧ�Ĳ�
            }

        }
        /// <summary>
        /// ���ظ�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void DoublePoint(object sender)
        {
            DataGridView view = (DataGridView)sender;//����ǰ�����Ķ���ת�ɿؼ������Է������
            string temp = view.SelectedCells[0].Value.ToString();//�õ�����еĵ�һ����Ԫ������
            string className = "";//Ҫ���������
            int OrginID = 0;//ԴҪ�����Ҫ��ID
            int DestID = 0;//Ŀ��Ҫ��ID

            char[] p = { ' ' };//�Կո�Ϊ�ָ��
            char[] s ={ '��' };//�ԣ�Ϊ�ָ��
            string[] para = temp.Split(p);
            className = para[0];//����
            string[] org = para[1].Split(s);//�õ�ԴID��
            OrginID = Convert.ToInt32(org[1]);//ԴID
            string[] des = para[2].Split(s);//Ŀ��ԴID��
            DestID = Convert.ToInt32(des[1]);//Ŀ��ID
            IFeature fu = null;//�õ�Ҫ��
            int n = _AppHk.MapControl.Map.LayerCount;
            if (n == 0) return;
            //�����ҳ�����ָ����
            for (int S = 0; S < n; S++)
            {
                ILayer layer = _AppHk.MapControl.Map.get_Layer(S);
                //�б��ǲ����飬����ǣ��ʹ�����ȡһ����
                if (layer is IGroupLayer)
                {
                    if (layer.Name == "�����ޱ�����" || layer.Name == "����������" || layer.Name == "���ƿ�����")
                    {
                        ICompositeLayer C_layer = layer as ICompositeLayer;//�õ����ͼ��
                        for (int c = 0; c < C_layer.Count; c++)
                        {

                            ILayer temp_layer = C_layer.get_Layer(c);
                            IFeatureLayer F_layer = temp_layer as IFeatureLayer;
                            IDataset set = F_layer.FeatureClass as IDataset;
                            if (className == set.Name)
                            {
                                m_CurLayer = temp_layer;
                                fu = F_layer.FeatureClass.GetFeature(OrginID);//�õ�Ҫ��
                                break;
                            }

                        }
                    }
                }

            }
            if (fu != null)
            {
                _AppHk.MapControl.Map.ClearSelection();//ÿ�ν���ǰ�����֮ǰѡ�����
                _AppHk.MapControl.Map.SelectFeature(m_CurLayer, fu);//�ڶ�Ӧ�Ĳ���ѡ��ָ����Ԫ��
                SysCommon.Gis.ModGisPub.ZoomToFeature(_AppHk.MapControl, fu);//��λ����Ӧ�Ĳ�
            }


        }
        /// <summary>
        /// ���ظ�
        /// </summary>
        /// <param name="sender"></param>
        private static void DoublePolygon(object sender)
        {
            DataGridView view = (DataGridView)sender;//����ǰ�����Ķ���ת�ɿؼ������Է������
            string temp = view.SelectedCells[0].Value.ToString();//�õ�����еĵ�һ����Ԫ������
            string className = "";//Ҫ���������
            int OrginID = 0;//ԴҪ�����Ҫ��ID
            int DestID = 0;//Ŀ��Ҫ��ID

            char[] p = { ' ' };//�Կո�Ϊ�ָ��
            char[] s ={ '��' };//�ԣ�Ϊ�ָ��
            string[] para = temp.Split(p);
            className = para[0];//����
            string[] org = para[1].Split(s);//�õ�ԴID��
            OrginID = Convert.ToInt32(org[1]);//ԴID
            string[] des = para[2].Split(s);//Ŀ��ԴID��
            DestID = Convert.ToInt32(des[1]);//Ŀ��ID
            IFeature fu = null;//�õ�Ҫ��
            int n = _AppHk.MapControl.Map.LayerCount;
            if (n == 0) return;
            //�����ҳ�����ָ����
            for (int S = 0; S < n; S++)
            {
                ILayer layer = _AppHk.MapControl.Map.get_Layer(S);
                //�б��ǲ����飬����ǣ��ʹ�����ȡһ����
                if (layer is IGroupLayer)
                {
                    if (layer.Name == "�����ޱ�����" || layer.Name == "����������" || layer.Name == "���ƿ�����")
                    {
                        ICompositeLayer C_layer = layer as ICompositeLayer;//�õ����ͼ��
                        for (int c = 0; c < C_layer.Count; c++)
                        {
                            ILayer temp_layer = C_layer.get_Layer(c);
                            IFeatureLayer F_layer = temp_layer as IFeatureLayer;
                            IDataset set = F_layer.FeatureClass as IDataset;
                            if (className == set.Name)
                            {
                                m_CurLayer = temp_layer;
                                //IFeatureLayer f_layer = temp_layer as IFeatureLayer;//ת�ɶ�Ӧ��Ҫ�ز�
                                fu = F_layer.FeatureClass.GetFeature(OrginID);//�õ�Ҫ��
                                break;
                            }

                        }
                    }
                }

            }
            if (fu != null)
            {
                _AppHk.MapControl.Map.ClearSelection();//ÿ�ν���ǰ�����֮ǰѡ�����
                _AppHk.MapControl.Map.SelectFeature(m_CurLayer, fu);//�ڶ�Ӧ�Ĳ���ѡ��ָ����Ԫ��
                SysCommon.Gis.ModGisPub.ZoomToFeature(_AppHk.MapControl, fu);//��λ����Ӧ�Ĳ�
            }

        }
        /// <summary>
        /// ����ͼ��
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void CodeLayer(object sender)
        {
            DataGridView view = (DataGridView)sender;//����ǰ�����Ķ���ת�ɿؼ������Է������
            string temp = view.SelectedCells[0].Value.ToString();//�õ�����еĵ�һ����Ԫ������
            string className = "";//Ҫ���������
            int OrginID = 0;//ԴҪ�����Ҫ��ID

            char[] p = { ' ' };//�Կո�Ϊ�ָ��
            char[] s ={ '��' };//�ԣ�Ϊ�ָ��
            string[] para = temp.Split(p);
            className = para[0];//����
            string[] org = para[1].Split(s);//�õ�ԴID��
            OrginID = Convert.ToInt32(org[1]);//ԴID
            IFeature fu = null;//�õ�Ҫ��
            int n = _AppHk.MapControl.Map.LayerCount;
            if (n == 0) return;
            //�����ҳ�����ָ����
            for (int S = 0; S < n; S++)
            {
                ILayer layer = _AppHk.MapControl.Map.get_Layer(S);
                //�б��ǲ����飬����ǣ��ʹ�����ȡһ����
                if (layer is IGroupLayer)
                {
                    if (layer.Name == "�����ޱ�����" || layer.Name == "����������" || layer.Name == "���ƿ�����")
                    {
                        ICompositeLayer C_layer = layer as ICompositeLayer;//�õ����ͼ��
                        for (int c = 0; c < C_layer.Count; c++)
                        {
                            ILayer temp_layer = C_layer.get_Layer(c);
                            IFeatureLayer F_layer = temp_layer as IFeatureLayer;
                            IDataset set = F_layer.FeatureClass as IDataset;
                            if (className == set.Name)
                            {
                                m_CurLayer = temp_layer;
                                fu = F_layer.FeatureClass.GetFeature(OrginID);//�õ�Ҫ��
                                break;
                            }

                        }
                    }
                }

            }
            if (fu != null)
            {
                _AppHk.MapControl.Map.ClearSelection();//ÿ�ν���ǰ�����֮ǰѡ�����
                _AppHk.MapControl.Map.SelectFeature(m_CurLayer, fu);//�ڶ�Ӧ�Ĳ���ѡ��ָ����Ԫ��
                SysCommon.Gis.ModGisPub.ZoomToFeature(_AppHk.MapControl, fu);//��λ����Ӧ�Ĳ�
            }


        }
        /// <summary>
        /// �����׼
        /// </summary>
        /// <param name="sender"></param>
        private static void Code(object sender)
        {
            DataGridView view = (DataGridView)sender;//����ǰ�����Ķ���ת�ɿؼ������Է������
            string temp = view.SelectedCells[0].Value.ToString();//�õ�����еĵ�һ����Ԫ������
            string className = "";//Ҫ���������
            int OrginID = 0;//ԴҪ�����Ҫ��ID

            char[] p = { ' ' };//�Կո�Ϊ�ָ��
            char[] s ={ '��' };//�ԣ�Ϊ�ָ��
            string[] para = temp.Split(p);
            className = para[0];//����
            string[] org = para[1].Split(s);//�õ�ԴID��
            OrginID = Convert.ToInt32(org[1]);//ԴID
            IFeature fu = null;//�õ�Ҫ��
            int n = _AppHk.MapControl.Map.LayerCount;
            if (n == 0) return;
            //�����ҳ�����ָ����
            for (int S = 0; S < n; S++)
            {
                ILayer layer = _AppHk.MapControl.Map.get_Layer(S);
                //�б��ǲ����飬����ǣ��ʹ�����ȡһ����
                if (layer is IGroupLayer)
                {
                    if (layer.Name == "�����ޱ�����" || layer.Name == "����������" || layer.Name == "���ƿ�����")
                    {
                        ICompositeLayer C_layer = layer as ICompositeLayer;//�õ����ͼ��
                        for (int c = 0; c < C_layer.Count; c++)
                        {
                            ILayer temp_layer = C_layer.get_Layer(c);
                            IFeatureLayer F_layer = temp_layer as IFeatureLayer;
                            IDataset set = F_layer.FeatureClass as IDataset;
                            if (className == set.Name)
                            {
                                m_CurLayer = temp_layer;
                                //IFeatureLayer f_layer = temp_layer as IFeatureLayer;//ת�ɶ�Ӧ��Ҫ�ز�
                                fu = F_layer.FeatureClass.GetFeature(OrginID);//�õ�Ҫ��
                                break;
                            }

                        }
                    }
                }

            }
            if (fu != null)
            {
                _AppHk.MapControl.Map.ClearSelection();//ÿ�ν���ǰ�����֮ǰѡ�����
                _AppHk.MapControl.Map.SelectFeature(m_CurLayer, fu);//�ڶ�Ӧ�Ĳ���ѡ��ָ����Ԫ��
                SysCommon.Gis.ModGisPub.ZoomToFeature(_AppHk.MapControl, fu);//��λ����Ӧ�Ĳ�
            }


        }
        /// <summary>
        /// ���ҵ�
        /// </summary>
        /// <param name="sender"></param>
        private static void HangPoint_click(object sender)
        {
            #region MyRegion
            DataGridView view = (DataGridView)sender;//����ǰ�����Ķ���ת�ɿؼ������Է������
            string temp = view.SelectedCells[0].Value.ToString();//�õ�����еĵ�һ����Ԫ������
            string className = "";//Ҫ���������
            int OrginID = 0;//ԴҪ�����Ҫ��ID

            char[] p = { ' ' };//�Կո�Ϊ�ָ��
            char[] s ={ '��' };//�ԣ�Ϊ�ָ��
            string[] para = temp.Split(p);
            className = para[0];//����
            string[] org = para[1].Split(s);//�õ�ԴID��
            OrginID = Convert.ToInt32(org[1]);//ԴID
            IFeature fu = null;//�õ�Ҫ��
            int n = _AppHk.MapControl.Map.LayerCount;
            if (n == 0) return;
            //�����ҳ�����ָ����
            for (int S = 0; S < n; S++)
            {
                ILayer layer = _AppHk.MapControl.Map.get_Layer(S);
                //�б��ǲ����飬����ǣ��ʹ�����ȡһ����
                if (layer is IGroupLayer)
                {
                    if (layer.Name == "�����ޱ�����" || layer.Name == "����������" || layer.Name == "���ƿ�����")
                    {
                        ICompositeLayer C_layer = layer as ICompositeLayer;//�õ����ͼ��
                        for (int c = 0; c < C_layer.Count; c++)
                        {
                            ILayer temp_layer = C_layer.get_Layer(c);
                            IFeatureLayer F_layer = temp_layer as IFeatureLayer;
                            IDataset set = F_layer.FeatureClass as IDataset;
                            if (className == set.Name)
                            {
                                m_CurLayer = temp_layer;
                                //IFeatureLayer f_layer = temp_layer as IFeatureLayer;//ת�ɶ�Ӧ��Ҫ�ز�
                                fu = F_layer.FeatureClass.GetFeature(OrginID);//�õ�Ҫ��
                                break;
                            }

                        }
                    }
                }

            }
            if (fu != null)
            {
                _AppHk.MapControl.Map.ClearSelection();//ÿ�ν���ǰ�����֮ǰѡ�����
                _AppHk.MapControl.Map.SelectFeature(m_CurLayer, fu);//�ڶ�Ӧ�Ĳ���ѡ��ָ����Ԫ��
                SysCommon.Gis.ModGisPub.ZoomToFeature(_AppHk.MapControl, fu);//��λ����Ӧ�Ĳ�
            }
            #endregion
        }
    }
}
