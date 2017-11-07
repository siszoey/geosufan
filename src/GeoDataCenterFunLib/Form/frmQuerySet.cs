using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Geodatabase;

namespace GeoDataCenterFunLib
{
    //��ѯ����
    public enum queryType
    {
        Cross = 1,
        Within = 2,
        Contain = 3,
        Intersect=4
    }

    /// <summary>
    /// ���ߣ�yjl
    /// ���ڣ�2011.05.24
    /// ˵�����ཻ������Ͱ�����ѯ���ô���
    /// </summary>
    public partial class frmQuerySet : DevComponents.DotNetBar.Office2007Form
    {
       
        //ȫ�ֹ�������
        public queryType QueryType;
        public List<ILayer> lstQueryedLayer;//����ѯ��ͼ�㼯��
        public IGeometry GeometryBag;//����ͼ��
        //ȫ���ڲ�����
        private IMap pMap;
        private List<ILayer> lstSelectLayer;
        
        
        //���캯��
        public frmQuerySet(IMap inMap,queryType inQType)
        {
            InitializeComponent();
            rdSelect.Checked = true;
            QueryType = inQType;
            pMap = inMap;
            initForm();
            initControls();
            //m_list = list;
            //m_listview = listview;
        }
        //��ʼ������
        private void initForm()
        {
            switch (QueryType)
            {
                case queryType.Cross :
                    this.Text = "��Խ��ѯ����";
                    break;
                case queryType.Contain:
                    this.Text = "������ѯ����";
                    break;
                case queryType.Intersect:
                    this.Text = "�ཻ��ѯ����";
                    break;
                case queryType.Within:
                    this.Text = "�����ѯ����";
                    break;
            } 
        }
        //��ʼ���ؼ�
        private void initControls()  
        {
            lstLayer.Items.Clear();
            lstQueryedLayer = new List<ILayer>();
            lstSelectLayer = new List<ILayer>();
            for (int i = 0; i < pMap.LayerCount; i++)
            {
                if (pMap.get_Layer(i) is IFeatureLayer)
                {
                    IFeatureLayer pFL = pMap.get_Layer(i) as IFeatureLayer;
                    if (pFL.FeatureClass.FeatureType==esriFeatureType.esriFTSimple && pFL.Valid && pFL.Visible)//����Ч���ɼ��Ϳ�ѡ
                    {
                        if (pFL.Selectable)
                        {
                            switch (QueryType)
                            {
                                case queryType.Cross :
                                    lstSelectLayer.Add(pFL);//�����ѯͼ�㼯��
                                    cboxSearchLayer.Items.Add(pFL.Name);
                                    break;
                                case queryType.Contain:
                                    if (pFL.FeatureClass.ShapeType == esriGeometryType.esriGeometryPolygon)
                                    {
                                        lstSelectLayer.Add(pFL);//�����ѯͼ�㼯��
                                        cboxSearchLayer.Items.Add(pFL.Name);
                                    }
                                    break;
                                case queryType.Intersect:
                                    lstSelectLayer.Add(pFL);//�����ѯͼ�㼯��
                                    cboxSearchLayer.Items.Add(pFL.Name);
                                    break;
                                case queryType.Within:
                                    lstSelectLayer.Add(pFL);//�����ѯͼ�㼯��
                                    cboxSearchLayer.Items.Add(pFL.Name);
                                    break;
                            }
                            
                        }//����ѡ
                        switch (QueryType)
                        {
                            case queryType.Cross :
                                lstQueryedLayer.Add(pFL);//���뱻��ѯͼ�㼯��
                                break;
                            case queryType.Contain:
                                lstQueryedLayer.Add(pFL);//���뱻��ѯͼ�㼯��
                                break;
                            case queryType.Intersect:
                                lstQueryedLayer.Add(pFL);//���뱻��ѯͼ�㼯��
                                break;
                            case queryType.Within:
                                if (pFL.FeatureClass.ShapeType == esriGeometryType.esriGeometryPolygon)
                                {
                                    lstQueryedLayer.Add(pFL);//���뱻��ѯͼ�㼯��
                                }
                                break;
                        }
                        
                    }//����Ч�ɼ�

                }//����Ҫ�ز�
                else if (pMap.get_Layer(i) is IGroupLayer)
                {
                    ICompositeLayer pCL = pMap.get_Layer(i) as ICompositeLayer;
                    for (int j = 0; j < pCL.Count; j++)
                    {
                        if (pCL.get_Layer(j) is IFeatureLayer)
                        {
                            IFeatureLayer pFL1 = pCL.get_Layer(j) as IFeatureLayer;
                            if (pFL1.FeatureClass.FeatureType == esriFeatureType.esriFTSimple && pFL1.Valid && pFL1.Visible)//����Ч���ɼ��Ϳ�ѡ
                            {
                                if (pFL1.Selectable)
                                {
                                    switch (QueryType)
                                    {
                                        case queryType.Cross :
                                            lstSelectLayer.Add(pFL1);//�����ѯͼ�㼯��
                                            cboxSearchLayer.Items.Add(pFL1.Name);
                                            break;
                                        case queryType.Contain:
                                            if (pFL1.FeatureClass.ShapeType == esriGeometryType.esriGeometryPolygon)
                                            {
                                                lstSelectLayer.Add(pFL1);//�����ѯͼ�㼯��
                                                cboxSearchLayer.Items.Add(pFL1.Name);
                                            }
                                            break;
                                        case queryType.Intersect:
                                            lstSelectLayer.Add(pFL1);//�����ѯͼ�㼯��
                                            cboxSearchLayer.Items.Add(pFL1.Name);
                                            break;
                                        case queryType.Within:
                                            lstSelectLayer.Add(pFL1);//�����ѯͼ�㼯��
                                            cboxSearchLayer.Items.Add(pFL1.Name);
                                            break;
                                    }

                                }//����ѡ
                                switch (QueryType)
                                {
                                    case queryType.Cross :
                                        lstQueryedLayer.Add(pFL1);//���뱻��ѯͼ�㼯��
                                        break;
                                    case queryType.Contain:
                                        lstQueryedLayer.Add(pFL1);//���뱻��ѯͼ�㼯��
                                        break;
                                    case queryType.Intersect:
                                        lstQueryedLayer.Add(pFL1);//���뱻��ѯͼ�㼯��
                                        break;
                                    case queryType.Within:
                                        if (pFL1.FeatureClass.ShapeType == esriGeometryType.esriGeometryPolygon)
                                        {
                                            lstQueryedLayer.Add(pFL1);//���뱻��ѯͼ�㼯��
                                        }
                                        break;
                                }

                            }//����Ч�ɼ�
 
                        }
                    }
                }
            }//forѭ��
            if (cboxSearchLayer.Items.Count > 0)
                cboxSearchLayer.SelectedIndex = 0;
            //��ʼ������ѯ��listview��ʹ��������ѯ��
            lstLayer.Items.Clear();
            foreach (ILayer pLayer in lstQueryedLayer)
            {
                if (pLayer.Name != cboxSearchLayer.Text)
                    lstLayer.Items.Add(pLayer.Name).Tag = pLayer;
            }

        }
        private void button2_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem lvi in lstLayer.Items)
            {
                if (lvi.Checked == true)
                {
                    lvi.Checked = false;
                }
                else
                {
                    lvi.Checked = true;
                }
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            //���ɱ���ѯ��ͼ�㼯�Դ����ѯ�������
            lstQueryedLayer = new List<ILayer>();
            foreach (ListViewItem lvi in lstLayer.Items)
            {
                if (lvi.Checked == true)
                {
                    lstQueryedLayer.Add(lvi.Tag as ILayer);
                }
            }
            //����ѡ��Ĳ�ѯͼ�������ͼ��
            IFeatureLayer pFL=null;
            foreach (ILayer pLayer in lstSelectLayer)
            {
                if (pLayer.Name == cboxSearchLayer.Text)
                    pFL = pLayer as IFeatureLayer;
            }
            IGeometryBag gmBag = new GeometryBagClass();
            gmBag.SpatialReference=pMap.SpatialReference;//����ռ�ο�����������ͼ�ν�ʧȥ�ο�
            IGeometryCollection gmCollection = gmBag as IGeometryCollection;
            if (rdSelect.Checked)//�����ѡ�� ѡ���Ҫ����ѡ��״̬
            {
                ISelectionSet pSelSet = (pFL as IFeatureSelection).SelectionSet;
                ICursor pCursor;
                pSelSet.Search(null, false, out pCursor);
                IFeature pFeature = (pCursor as IFeatureCursor).NextFeature();
                object obj = Type.Missing;
                while (pFeature != null)
                {
                    gmCollection.AddGeometry(pFeature.ShapeCopy, ref obj, ref obj);
                    pFeature = (pCursor as IFeatureCursor).NextFeature();
                }
            }
            else//�����ѡ�� ȫ��Ҫ����ѡ��״̬
            {
                IFeatureCursor pFeaCursor = pFL.FeatureClass.Search(null, false);
                IFeature pFea = pFeaCursor.NextFeature();
                object obj = Type.Missing;
                while (pFea != null)
                {
                    gmCollection.AddGeometry(pFea.ShapeCopy, ref obj, ref obj);
                    pFea = pFeaCursor.NextFeature();
                }
            }
            ISpatialIndex pSI = gmBag as ISpatialIndex;//�ؽ�����
            pSI.AllowIndexing = true;
            pSI.Invalidate();
            GeometryBag = gmBag;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem lvi in lstLayer.Items)
            {
                lvi.Checked = true;
            }
        }

        private void cboxSearchLayer_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboxSearchLayer.Text== "")
                return;
            //��ʼ��listView��ʹ��������ѯ��
            lstLayer.Items.Clear();
            foreach (ILayer pLayer in lstQueryedLayer)
            {
                if (pLayer.Name != cboxSearchLayer.Text)
                    lstLayer.Items.Add(pLayer.Name).Tag = pLayer;
            }
        }

        private void btnCancle_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        
    }
}