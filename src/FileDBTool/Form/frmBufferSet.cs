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

namespace FileDBTool
{
    /// <summary>
    /// ����뾶�����ô���
    /// </summary>
    public partial class frmBufferSet : DevComponents.DotNetBar.Office2007Form
    {
        private IGeometry m_pGeometry;
        private IGeometry m_pBufferGeometry;

        private IMap m_pMap;
        private IPolygon m_pPolygon;
        private IScreenDisplay m_pScreenDisplay;
        private bool m_bOk;
        public bool Res
        {
            get
            {
                return m_bOk;
            }
        }

        //��Enter���͡�BackSpace��������
        const char KEY_Enter = (char)Keys.Enter;
        const char KEY_BackSpace = (char)Keys.Back;

        //���������ֵ
        private char[] arrayChar = new char[] { '0','1','2','3','4','5','6','7','8','9','.'};

        private IActiveViewEvents_Event m_pActiveViewEvents;

        public frmBufferSet(IGeometry pGeometry, IMap pMap)
        {
            //��������� �� ��Ļ��ʾ
            m_pMap = pMap;
            m_pGeometry = pGeometry;
            m_bOk = false;
            IActiveView pActiveView = pMap as IActiveView;

            m_pActiveViewEvents = pActiveView as IActiveViewEvents_Event;
            m_pScreenDisplay = pActiveView.ScreenDisplay;
            try
            {
                m_pActiveViewEvents.AfterDraw += new IActiveViewEvents_AfterDrawEventHandler ( m_pActiveViewEvents_AfterDraw );

            }
            catch 
            {
            }
            //��ʼ���ؼ� �� TrackBar
            InitializeComponent();
            InitializeTrackBar(m_pGeometry);

            this.TopMost = true;
        }

        void frmBufferSet_Disposed(object sender, EventArgs e)
        {
            m_bOk = false;  
        }

        //����
        private void frmBufferSet_Load(object sender, EventArgs e)
        {
            this.Text = "����뾶����";
            this.trackBar.TickFrequency = 100;
        }

        //����ȥʱ����
        private void m_pActiveViewEvents_AfterDraw(IDisplay Display, esriViewDrawPhase phase )
        {
            if (this.IsDisposed == true) return;
            if (phase == esriViewDrawPhase.esriViewForeground) drawgeometryXOR(null, m_pScreenDisplay);
        }

        //���ݼ��������趨��ʼֵ
        private void InitializeTrackBar(IGeometry pGeometry)
        {                       
            trackBar.Minimum = 0;
            trackBar.Maximum = 1000;
            trackBar.Value = 0;

            switch (pGeometry.GeometryType)
            {
                case esriGeometryType.esriGeometryPoint:

                    trackBar.SmallChange = 1;
                    trackBar.LargeChange = 100;
                    break;
                case esriGeometryType.esriGeometryPolyline:

                    trackBar.SmallChange = 1;
                    trackBar.LargeChange = 100;
                    break;

                case esriGeometryType.esriGeometryPolygon:

                    trackBar.SmallChange = 1;
                    trackBar.LargeChange = 100;
                    break;

                case esriGeometryType.esriGeometryBag:
                    trackBar.SmallChange = 1;
                    trackBar.LargeChange = 100;
                    break;

                default:
                    MessageBox.Show("����ȷ�ļ�������!", "ϵͳ��ʾ");
                    this.Dispose(true);
                    return;
            }
        }



        //��ȡ��ǰ�Ļ����ļ�����
        private void get_BufferGeometry()
        {
            //�����ϴεĻ��弸���� ����Ĩȥ��ͼ��
            if (m_pBufferGeometry != null) drawgeometryXOR(m_pBufferGeometry as IPolygon, m_pScreenDisplay);
            
            //��ȡ����뾶
            double dBufferSize = ((double)trackBar.Value /*/ 10*/);
            dBufferSize = dBufferSize < 1 ? 1 : dBufferSize;
            //if ( dBufferSize == 0.0) dBufferSize = 0.001;

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
            return m_pBufferGeometry;
        }

        /// <summary>
        /// ����pGeometry��ͼ��
        /// </summary>
        /// <param name="pGeometry"> ������ʵ��</param>
        /// <param name="pScreenDisplay"> ��ǰ��Ļ��ʾ</param>
        private void drawgeometryXOR(IPolygon pPolygon, IScreenDisplay pScreenDisplay)
        {
            IFillSymbol pFillSymbol = new SimpleFillSymbolClass();
            ISimpleLineSymbol pLineSymbol = new SimpleLineSymbolClass();

            try
            {
                //��ɫ����
                IRgbColor pRGBColor = new RgbColorClass();
                pRGBColor.UseWindowsDithering = false;
                pRGBColor.Red = 45;
                pRGBColor.Green = 45;
                pRGBColor.Blue = 45;

                //�������Լ�����
                ISymbol pSymbol = pFillSymbol as ISymbol;
                pSymbol.ROP2 = esriRasterOpCode.esriROPXOrPen;
                pFillSymbol.Color = pRGBColor;

                //��Ե����ɫ�Լ�����
                ISymbol pLSymbol = pLineSymbol as ISymbol;
                pLSymbol.ROP2 = esriRasterOpCode.esriROPXOrPen;
                pRGBColor.Red = 145;
                pRGBColor.Green = 145;
                pRGBColor.Blue = 145;
                pLineSymbol.Color = (IColor)pRGBColor;

                pLineSymbol.Width = 0.8;
                pLineSymbol.Style = esriSimpleLineStyle.esriSLSSolid;
                pFillSymbol.Outline = pLineSymbol;

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
                MessageBox.Show("û�н��л��崦��ļ���ʵ��!", "ϵͳ��ʾ");
                return;
            }

            get_BufferGeometry();
            m_bOk = true;
            m_pGeometry = null;
            m_pPolygon = null;
            IActiveView pActiveView = m_pMap as IActiveView;
            pActiveView.PartialRefresh(esriViewDrawPhase.esriViewForeground, null, pActiveView.Extent);
            this.Close();//.Hide();
        }

        //trackBar ֵ�ı��¼�����ӦtxtBufferValue����ʾ
        private void trackBar_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                //trackBar��Ϊdoule���ٴ���
                int dValue = Convert.ToInt32(trackBar.Value /*/ 10.0 */);
                txtBufferValue.Text = dValue < 1 ? "1" : dValue.ToString();

                //���ı���Ӧ�� ��Ϊ����ֵ��m_pBufferGeometry;
                get_BufferGeometry();
            }
            catch
            {

            }
        }

        //txtBufferValue �������¼� ��У�鲢��Ӧ�ı�
        private void txtBufferValue_KeyPress(object sender, KeyPressEventArgs e)
        {
            //textBox������.���˸���Ӧ��8���˸��

            try
            {
                
                if (e.KeyChar == '\r')//KEY_Enter.CompareTo(e.KeyChar) == 0)
                {
                    int iValue = (int)(Convert.ToDouble(txtBufferValue.Text));
                    if (iValue > trackBar.Maximum)
                    {
                        SysCommon.Error.frmErrorHandle infoFrm = new SysCommon.Error.frmErrorHandle("��ʾ", "����뾶���������ֵ��");
                        infoFrm.ShowDialog(this);
                       // trackBar.Maximum = iValue / 2 * 3;
                        trackBar.TickFrequency = Convert.ToInt32(trackBar.Maximum);
                        iValue = trackBar.Maximum;
                    }
                    if (iValue < 0)
                    {
                        SysCommon.Error.frmErrorHandle infoFrm = new SysCommon.Error.frmErrorHandle("��ʾ", "����뾶����Ϊ������");
                        infoFrm.ShowDialog(this);
                        txtBufferValue.Text = Convert.ToString(0);
                        iValue = 0;
                    }
                    trackBar.Value = iValue;

                }
            
            }
            catch
            {
                SysCommon.Error.frmErrorHandle infoFrm = new SysCommon.Error.frmErrorHandle("��ʾ", "��Ч���ַ��������������֣�");
                infoFrm.ShowDialog(this);
                return;
            }
        }

        private void cmd_Cancel_Click(object sender, EventArgs e)
        {
            this.Dispose(true);
            this.Refresh();
            m_pBufferGeometry = null;
            m_bOk = false;
            drawgeometryXOR(m_pPolygon, m_pScreenDisplay);
        }
    }
}