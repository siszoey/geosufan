using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.SystemUI;
using ESRI.ArcGIS.Geodatabase;

namespace GeoUtilities
{
    /// <summary>
    /// ����뾶�����ô���
    /// </summary>
    public partial class frmBufferSet : DevComponents.DotNetBar.Office2007Form
    {
        private IGeometry m_pGeometry;
        private IGeometry m_pBufferGeometry;
        public void setBufferGeometry(IGeometry pGeo)
        {
            m_pBufferGeometry = pGeo;
        }
        //�����С
        private double dBufferSize=1;
        public double BufferSize
        {
            get { return dBufferSize; }
            set { dBufferSize = value; }
        }
        private IMap m_pMap;
        IActiveView pActiveView = null;
        private IPolygon m_pPolygon;
        private IScreenDisplay m_pScreenDisplay;
        private bool m_bOk;
		private bool m_TextChange=false;//��¼�������Ƿ������ֵ//xisheng 20110802
        private int iValueLast;//��¼�ϴ� 20110802
        public bool Res
        {
            get
            {
                return m_bOk;
            }
        }
        ///ZQ  201119 add
        private esriSpatialRelEnum m_esriSpatialRelEnum = esriSpatialRelEnum.esriSpatialRelIntersects;
        public esriSpatialRelEnum pesriSpatialRelEnum
        {
            get
            {
                return m_esriSpatialRelEnum;
            }
        }
        ///end
        //��Enter���͡�BackSpace��������
        const char KEY_Enter = (char)Keys.Enter;
        const char KEY_BackSpace = (char)Keys.Back;

        //���������ֵ
        private char[] arrayChar = new char[] { '0','1','2','3','4','5','6','7','8','9','.'};
        //ZQ 2011 1126 modify
        //private IActiveViewEvents_Event m_pActiveViewEvents;
        private DevComponents.DotNetBar.Office2007Form m_Form;
        public frmBufferSet(IGeometry pGeometry, IMap pMap,DevComponents.DotNetBar.Office2007Form pForm)
        {
            //��������� �� ��Ļ��ʾ
            m_pMap = pMap;
            m_pGeometry = pGeometry;
            m_bOk = false;
            pActiveView = pMap as IActiveView;
            m_Form = pForm;
            //m_pActiveViewEvents = pActiveView as IActiveViewEvents_Event;
            m_pScreenDisplay = pActiveView.ScreenDisplay;
            //ZQ 2011 1126 modify
            SysCommon.ScreenDraw.list.Add(BufferSetAfterDraw);
            //try
            //{
            //    m_pActiveViewEvents.AfterDraw += new IActiveViewEvents_AfterDrawEventHandler ( m_pActiveViewEvents_AfterDraw );

            //}
            //catch 
            //{
            //}
            //��ʼ���ؼ� �� TrackBar
            InitializeComponent();
			this.groupBox1.Text = "���뻺��뾶";//20110802 xisheng
        }

        void frmBufferSet_Disposed(object sender, EventArgs e)
        {
            m_bOk = false;  
        }

        //����
        private void frmBufferSet_Load(object sender, EventArgs e)
        {
            InitializeTrackBar(m_pGeometry);
            this.TopMost = true;
            this.Text = "����뾶����";
            this.trackBar.TickFrequency = trackBar.Maximum / 10;//20110802 xisheng
            ///ZQ 20111119  add
            cmbSpatialRel.SelectedIndex = 0;
        }
        //ZQ 2011 1126 modify
        //����ȥʱ����
         internal void BufferSetAfterDraw(IDisplay Display, esriViewDrawPhase phase )
        //private void m_pActiveViewEvents_AfterDraw(IDisplay Display, esriViewDrawPhase phase )
        {
            if ( m_Form.IsDisposed == true || m_pBufferGeometry == null)
            {
                m_pPolygon = null;
                return;
            }
            if (phase == esriViewDrawPhase.esriViewForeground) drawgeometryXOR(null, m_pScreenDisplay);
        }

        //���ݼ��������趨��ʼֵ
        private void InitializeTrackBar(IGeometry pGeometry)
        {                       
            trackBar.Minimum = 1;
            trackBar.Maximum = 10000;//0802 xisheng
            trackBar.Value = dBufferSize <=1 ? 10: int.Parse(dBufferSize.ToString());

            switch (pGeometry.GeometryType)
            {
                case esriGeometryType.esriGeometryPoint:

                    trackBar.SmallChange = 1;
                    trackBar.LargeChange = 10000;//0802 xisheng
                    break;
                case esriGeometryType.esriGeometryPolyline:

                    trackBar.SmallChange = 1;
                    trackBar.LargeChange = 10000;
                    break;

                case esriGeometryType.esriGeometryPolygon:

                    trackBar.SmallChange = 1;
                    trackBar.LargeChange = 10000;
                    break;

                case esriGeometryType.esriGeometryBag:
                    trackBar.SmallChange = 1;
                    trackBar.LargeChange = 10000;
                    break;

                default:
                    MessageBox.Show("����ȷ�ļ�������!", "��ʾ");
                    this.Dispose(true);
                    return;
            }
        }



        //��ȡ��ǰ�Ļ����ļ�����
        private void get_BufferGeometry()
        {
            Error_Lable.Visible = false;
            Error_Lable.Text = "";
            //�����ϴεĻ��弸���� ����Ĩȥ��ͼ��
            if (m_pBufferGeometry != null) drawgeometryXOR(m_pBufferGeometry as IPolygon, m_pScreenDisplay);
            
            //��ȡ����뾶
            dBufferSize = Convert.ToDouble(txtBufferValue.Text);/*/ 10*/ ; //20110802 xisheng
            dBufferSize = dBufferSize < 1 ? 1 : dBufferSize;//���û���ֵ�������ó�0 xisheng 20110722
            if ( dBufferSize == 0.0) dBufferSize = 0.001;
            //ת������Ǿ�γ�ȵĵ�ͼ xisheng 20110731
            UnitConverter punitConverter = new UnitConverterClass();
            if (m_pMap.MapUnits == esriUnits.esriDecimalDegrees)
            {
                dBufferSize = punitConverter.ConvertUnits(dBufferSize, esriUnits.esriMeters, esriUnits.esriDecimalDegrees);
            }//ת������Ǿ�γ�ȵĵ�ͼ xisheng 20110731
            //==���ֱ�Ӳ�������ԭpGeometry�ᱻ�ı�
            //���п�¡����ȡtopoʵ��
            IClone pClone = (IClone)m_pGeometry;
            ITopologicalOperator pTopo;
            if (m_pGeometry.GeometryType != esriGeometryType.esriGeometryBag)
            {
                pTopo = pClone.Clone() as ITopologicalOperator;

                //topo�ǿ�����л��壬��ȡ������ m_pBufferGeometry
                if(pTopo != null)  m_pBufferGeometry = pTopo.Buffer(dBufferSize);  
            }
            else
            {
                IGeometryCollection pGeometryBag = (IGeometryCollection)pClone.Clone();
                pTopo = (ITopologicalOperator)pGeometryBag.get_Geometry(0);
                IGeometry pUnionGeom = pTopo.Buffer(dBufferSize);
                for (int i = 1; i < pGeometryBag.GeometryCount; i++)
                {
                    pTopo = (ITopologicalOperator)pGeometryBag.get_Geometry(i);
                    IGeometry pTempGeom = pTopo.Buffer(dBufferSize);
                    pTopo = (ITopologicalOperator)pUnionGeom;
                    pUnionGeom = pTopo.Union(pTempGeom);
                }
                m_pBufferGeometry = pUnionGeom;
            }
            // m_pBufferGeometryΪ�գ�ֱ�ӷ���
            if (m_pBufferGeometry == null) return;

            //�� m_pBufferGeometry��topo���м��ٻ��
            pTopo = m_pBufferGeometry as ITopologicalOperator;
            if(pTopo != null)  pTopo.Simplify();

            IPolygon pPolygon = m_pBufferGeometry as IPolygon;

            drawgeometryXOR(pPolygon, m_pScreenDisplay);
        }

        //����ģ̬��ʾ,�õ����ζ���
        public IGeometry GetBufferGeometry()
        {
            this.ShowDialog();
            if (m_bOk)
            {
                //SysCommon.Gis.ModGisPub.DoDrawRangeNoRefresh(m_pMap, m_pBufferGeometry);
                pActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
            }
            return m_pBufferGeometry;
        }
        /// <summary>
        /// ����pGeometry��ͼ��
        /// </summary>
        /// <param name="pGeometry"> ������ʵ��</param>
        /// <param name="pScreenDisplay"> ��ǰ��Ļ��ʾ</param>
        private void drawgeometryXOR(IPolygon pPolygon, IScreenDisplay pScreenDisplay)
        {
            if (this.IsDisposed && m_bOk == false)//�������رջ���ȡ�� �Ͳ����� xisheng 2011.06.28
            {
                return;
            }
            ISimpleFillSymbol pFillSymbol = new SimpleFillSymbolClass();
            ISimpleLineSymbol pLineSymbol = new SimpleLineSymbolClass();

            try
            {
                //��ɫ����
                IRgbColor pRGBColor = new RgbColorClass();
                pRGBColor.UseWindowsDithering = false;
                ISymbol pSymbol = (ISymbol)pFillSymbol;
                pSymbol.ROP2 = esriRasterOpCode.esriROPNotXOrPen;

                pRGBColor.Red = 255;
                pRGBColor.Green = 170;
                pRGBColor.Blue = 0;
                pLineSymbol.Color = pRGBColor;

                pLineSymbol.Width = 0.8;
                pLineSymbol.Style = esriSimpleLineStyle.esriSLSSolid;
                pFillSymbol.Outline = pLineSymbol;

                pFillSymbol.Color = pRGBColor;
                pFillSymbol.Style = esriSimpleFillStyle.esriSFSDiagonalCross;

                pScreenDisplay.StartDrawing(m_pScreenDisplay.hDC, -1);  //esriScreenCache.esriNoScreenCache -1
                pScreenDisplay.SetSymbol(pSymbol);

                //�������ѻ����Ķ����
                if (pPolygon != null)
                {
                    pScreenDisplay.DrawPolygon(pPolygon);
                    m_pPolygon = pPolygon;
                }
                //�����ѻ����Ķ����
                else
                {
                    if (m_pPolygon != null)
                    {
                        pScreenDisplay.DrawPolygon(m_pPolygon);
                    }
                }
               
                pScreenDisplay.FinishDrawing();
            }
            catch (Exception ex)
            {
                MessageBox.Show("���ƻ��巶Χ����:" + ex.Message, "��ʾ");
                pFillSymbol = null;
            }
        }

        //ˢ��ʱ����
        public override void Refresh()
        {
            base.Refresh();
            drawgeometryXOR(m_pPolygon, m_pScreenDisplay);

            IActiveView pActiveView = m_pMap as IActiveView;
            pActiveView.PartialRefresh(esriViewDrawPhase.esriViewForeground, null, null);
        }

        private void frmBufferSet_KeyDown ( object sender , KeyEventArgs e )
        {
            if ( e.KeyCode == Keys.Escape )
            {
                this.Dispose ( true );
                this.Refresh ();
                m_pBufferGeometry = null;
                m_bOk = false;
            }
        }

        //ȷ�� ��ť
        private void cmd_OK_Click(object sender, EventArgs e)
        {
            if (m_pGeometry == null)
            {
                MessageBox.Show("û�н��л��崦��ļ���ʵ��!", "��ʾ");
                return;
            }

            get_BufferGeometry();
            m_bOk = true;
            m_pGeometry = null;
            //m_pPolygon = null;
            IActiveView pActiveView = m_pMap as IActiveView;
            //pActiveView.PartialRefresh(esriViewDrawPhase.esriViewForeground, null, pActiveView.Extent);
            this.Hide();
       
        }

        //trackBar ֵ�ı��¼�����ӦtxtBufferValue����ʾ
        private void trackBar_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                //trackBar��Ϊdoule���ٴ���
                int dValue = Convert.ToInt32(trackBar.Value /*/ 10.0 */);
				if (m_TextChange)
                {
                    m_TextChange = false;
                    return;//�������text��ɵ�ʹ�����������ֵ���ı�ԭ��text xisheng 20110802

                } txtBufferValue.Text = dValue < 1 ? "1" : dValue.ToString();

                /*xisheng delete 20110802
                //���ı���Ӧ�� ��Ϊ����ֵ��m_pBufferGeometry;
                get_BufferGeometry();
                this.Refresh();
                 */

            }
            catch
            {

            }
        }

        private void cmd_Cancel_Click(object sender, EventArgs e)
        {
            //this.Dispose(true);
            this.Close();
            this.DialogResult = DialogResult.Cancel;
            //this.Refresh();
            //m_pBufferGeometry = null;
            //m_bOk = false;
            //drawgeometryXOR(m_pPolygon, m_pScreenDisplay);changed by xisheng
        }

        //added by xisheng 06.28
        private void txtBufferValue_TextChanged(object sender, EventArgs e)
        {
            try
            {
               	m_TextChange = false;//20110802 xisheng
                Error_Lable.Visible = false;
                //����������������ȷ�������� xisheng 2011.07.11
                int iValue = (int)(Convert.ToDouble(txtBufferValue.Text));
				iValueLast = iValue;//��¼�ϴ�  20110802 xisheng
                if (iValue > trackBar.Maximum)
                {
                //    this.Error_Lable.Text = "����뾶���������ֵ";
                //    this.Error_Lable.Visible = true;
                //    txtBufferValue.Text = Convert.ToString(trackBar.Maximum);//20110802 xisheng
                    iValue = trackBar.Maximum;
                    m_TextChange = true;
                }
                if (iValue < 0)
                {
                    this.Error_Lable.Text = "����뾶����Ϊ����";
                    this.Error_Lable.Visible = true;
                    //infoFrm.ShowDialog(this);
                    txtBufferValue.Text = Convert.ToString(1);
                    iValue = 1;
                }
                trackBar.Value = iValue;
       
				//���ı���Ӧ�� ��Ϊ����ֵ��m_pBufferGeometry; added by xisheng 20110802
                get_BufferGeometry();
                this.Refresh();
            }
            catch(Exception ex)
            {
                if (ex.Message.ToString() == "�쳣���� HRESULT:0x80040238")
                {
                    this.Error_Lable.Text = "��Ч�������򣬳�����Ч�����ռ�";
                }
                else
                {
                    this.Error_Lable.Text = "��Ч���룬��������������";
                }
                this.Error_Lable.Visible = true;
                txtBufferValue.Text = Convert.ToString(iValueLast);//20110802 xisheng
                return;
            }

        }

        private void frmBufferSet_FormClosed(object sender, FormClosedEventArgs e)
        {
            //this.Dispose(true);
            this.Refresh();
            m_pBufferGeometry = null;
            m_bOk = false;
            SysCommon.ScreenDraw.list.Remove(BufferSetAfterDraw);
        }
        /// <summary>
        /// ZQ 20111119  add
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbSpatialRel_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch(cmbSpatialRel.SelectedItem.ToString())
            {
                case"�ཻ":
                    m_esriSpatialRelEnum = esriSpatialRelEnum.esriSpatialRelIntersects;
                    break;
                case"���":
                    m_esriSpatialRelEnum = esriSpatialRelEnum.esriSpatialRelTouches;
                    break;
                case "��Խ":
                    m_esriSpatialRelEnum = esriSpatialRelEnum.esriSpatialRelCrosses;
                    break;
                case "����":
                    m_esriSpatialRelEnum = esriSpatialRelEnum.esriSpatialRelContains;
                    break;
            }
        }
    }
}