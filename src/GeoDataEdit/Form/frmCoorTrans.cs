using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;


using ESRI.ArcGIS.Geodatabase;

using ESRI.ArcGIS.DataSourcesFile;

using ESRI.ArcGIS.SystemUI;
using ESRI.ArcGIS.esriSystem;
//using GeoDataCenterFunLib;
using ESRI.ArcGIS.Geometry;

using ESRI.ArcGIS.DataSourcesGDB;
using System.Xml;
using GeoDataCenterFunLib;



namespace GeoDataEdit
{
    /// <summary>
    /// ���ߣ�yjl
    /// ���ڣ�2011.06.22
    /// ˵������ȡ���Ƶ��������ת���Ĵ���
    /// </summary>
    public partial class frmCoorTrans : DevComponents.DotNetBar.Office2007Form
    {
        public frmCoorTrans()
        {
            InitializeComponent();
            this.Height = 343;
        }

        private string shpPath = "";
        private IWorkspace pWorkspace = null;
        private IFeatureClass pFeaClass=null;
        private List<IPoint> pSrcPts = null;//Դ���Ƶ㼯��
        private List<IPoint> pToPts = null;//Ŀ����Ƶ㼯��
        private Dictionary<string, string> pResult;//��¼�ɹ���ʧ�ܵ�Ҫ����
        private ITransformation pTransformation;//ת����
        private IWorkspaceFactory pWF;

        private void buttonX1_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "���Ƶ��ļ�|*.txt";
            openFileDialog1.FileName = "";
            openFileDialog1.Multiselect = false;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                txtCtrlPtPath.Text = openFileDialog1.FileName;
                fillDGView(openFileDialog1.OpenFile());
                initTransformation();
 
            }

            txtCtrlPtPath.ForeColor = Color.Black;
          

        }
        //���dgv
        private void fillDGView(Stream inStream)
        {
            StreamReader sr = new StreamReader(inStream);
            dgvCtrlPt.Rows.Clear();
            while (!sr.EndOfStream)
            {
                string str = sr.ReadLine();
                string[] strs = str.Split(',', ':');
                if (strs.Length != 6)
                    continue;
                dgvCtrlPt.Rows.Add(strs[0], strs[1], strs[2], strs[3], strs[4], strs[5]);
 
            }
            
        }
        //����ת����
        private void initTransformation()
        {
            pSrcPts = new List<IPoint>();
            pToPts = new List<IPoint>();
            pTransformation = new AffineTransformation2DClass();
            IAffineTransformation2D3GEN pAffineTrans3 = pTransformation as IAffineTransformation2D3GEN;
            
            for (int i = 0; i < dgvCtrlPt.Rows.Count; i++)//��
            {
                IPoint ptSrc = new PointClass();//Դ��
                ptSrc.PutCoords(Convert.ToDouble(dgvCtrlPt[0, i].Value), Convert.ToDouble(dgvCtrlPt[1, i].Value));
                pSrcPts.Add(ptSrc);
                IPoint ptTo = new PointClass();//Ŀ���
                ptTo.PutCoords(Convert.ToDouble(dgvCtrlPt[3, i].Value), Convert.ToDouble(dgvCtrlPt[4, i].Value));
                pToPts.Add(ptTo);
            }
            IPoint[] aSrcPts = pSrcPts.ToArray();
            IPoint[] aToPoint = pToPts.ToArray();
            pAffineTrans3.DefineFromControlPoints(ref aSrcPts, ref aToPoint);
            for (int i = 0; i < dgvCtrlPt.Rows.Count; i++)//��
            {
                double fromPtError=0,toPtError=0;
                pAffineTrans3.GetControlPointError(i,ref fromPtError,ref toPtError);//��ȡÿ�����Ƶ�в�
                dgvCtrlPt[6, i].Value = fromPtError.ToString("F06");
            }
            double fromRMSerror=0,toRMSerror=0;
            pAffineTrans3.GetRMSError(ref fromRMSerror, ref toRMSerror);
            if(fromRMSerror<0.05)
                lblRMS.Text = "��׼�RMS����" + fromRMSerror.ToString("F06")+".���Խ���ת����";
            else
                lblRMS.Text = "��׼�RMS����" + fromRMSerror.ToString("F06") + ".���̫�����������Ƶ����ݣ�";
        }
        /// <summary>
        /// ʵ�ֶ�Ҫ���������ķ���任
        /// </summary>
        /// <param name="inFC">Ҫ����</param>
        /// <param name="inTransformation">ת����</param>
        private void coordTransfermation(IFeatureClass inFC, ITransformation inTransformation)
        {
            
            IFeatureCursor pFCursor = inFC.Update(null, false);
            IFeature pFeature = pFCursor.NextFeature();
            while (pFeature != null)
            {
                IGeometry shpTransformed = pFeature.ShapeCopy;
                ITransform2D pTransform2D = shpTransformed as ITransform2D;
                pTransform2D.Transform(esriTransformDirection.esriTransformForward, inTransformation);
                pFeature.Shape = shpTransformed;
                //int id = inFC.FindField("LAYER_OID");
                //if((inFC as IDataset).Name=="�ڵ�_Project54_1")
                //pFeature.set_Value(id,"1");

                pFCursor.UpdateFeature(pFeature);
                //cursor����
                pFeature = pFCursor.NextFeature();
            }
            Marshal.ReleaseComObject(pFCursor);//�ͷ�cursor
            ISpatialReference unKnownSR = new UnknownCoordinateSystemClass();
            IGeoDatasetSchemaEdit pGDSE = inFC as IGeoDatasetSchemaEdit;
            if (pGDSE.CanAlterSpatialReference)
                pGDSE.AlterSpatialReference(unKnownSR);//����Ҫ�����ͶӰ
            IFeatureClassManage pFCM = inFC as IFeatureClassManage;
            pFCM.UpdateExtent();//����Ҫ�������ֵ��Χ
            IGeoDataset pGD = inFC as IGeoDataset;
            IEnvelope ppp = pGD.Extent;
        }
        //��shp�ļ�
        private IFeatureClass openShp()
        {
            if (txtCtrlPtPath.Text == "") 
                return null;
            string wsPath=System.IO.Path.GetDirectoryName(txtCtrlPtPath.Text);
            string shpName=System.IO.Path.GetFileName(txtCtrlPtPath.Text);
            string exten=System.IO.Path.GetExtension(txtCtrlPtPath.Text);
            pWorkspace = new ShapefileWorkspaceFactoryClass().OpenFromFile(wsPath,0);
            IFeatureClass pFeatureClass=(pWorkspace as IFeatureWorkspace).OpenFeatureClass(shpName.Remove(shpName.Length-4));
            return pFeatureClass;
            
        }
    

        private void btnOK_Click(object sender, EventArgs e)
        {

            SysCommon.CProgress vProgress = new SysCommon.CProgress("����ת��,���Ժ�");
            vProgress.EnableCancel = false;
            vProgress.ShowDescription = false;
            vProgress.FakeProgress = true;
            vProgress.TopMost = true;
            vProgress.ShowProgress();
            
            Application.DoEvents();
            bool result = false;
            if (txtCtrlPtPath.Text == "��һ�����Ƶ��ļ���txt��" || txtSrcPath.Text == "Դ���ݹ����ռ�·��" || txtToPath.Text == "ת��������ݵĹ����ռ�·��")
            {
                vProgress.Close();
                return;
            }
            try
            {
            if(rdoSHP.Checked)
            {
                DirectoryInfo dir = new DirectoryInfo(txtSrcPath.Text);
                FileInfo[] files = dir.GetFiles();
                if (!Directory.Exists(txtToPath.Text))
                    Directory.CreateDirectory(txtToPath.Text);//�ж�Ŀ¼���ڷ�
                foreach (FileInfo file in files)
                {
                    file.CopyTo(System.IO.Path.Combine(txtToPath.Text, file.Name), true);
                }
                pWF = new ShapefileWorkspaceFactoryClass();
            }
            else if(rdoGDB.Checked)
            {
                DirectoryInfo dir2 = new DirectoryInfo(txtSrcPath.Text);
                FileInfo[] files2 = dir2.GetFiles();
                if (!Directory.Exists(txtToPath.Text))
                    Directory.CreateDirectory(txtToPath.Text);//�ж�Ŀ¼���ڷ�
                else
                {
                    if (MessageBox.Show("�ļ��Ѵ��ڡ�ȷ�����ǣ�", "��ʾ", MessageBoxButtons.OKCancel,
                MessageBoxIcon.Information) != DialogResult.OK)
                        return;
                    Directory.Delete(txtToPath.Text, true);
                    Directory.CreateDirectory(txtToPath.Text);

                }
                foreach (FileInfo file in files2)
                {
                    file.CopyTo(System.IO.Path.Combine(txtToPath.Text, file.Name), true);
                }
                pWF = new FileGDBWorkspaceFactoryClass();
            }
            else
            {
                File.Copy(txtSrcPath.Text, txtToPath.Text, true);
                pWF = new AccessWorkspaceFactoryClass();
            }

                pResult = new Dictionary<string, string>();
                lstViewResult.Items.Clear();
                pWorkspace = pWF.OpenFromFile(txtToPath.Text, 0);
                IWorkspaceEdit pWorkSpaceEdit = pWorkspace as IWorkspaceEdit;
                pWorkSpaceEdit.StartEditing(false);
                IEnumDataset enumDS = pWorkspace.get_Datasets(esriDatasetType.esriDTFeatureClass);
                IDataset pDs = enumDS.Next();
                while (pDs != null)
                {
                    try
                    {
                        IFeatureClass pFC = pDs as IFeatureClass;
                        coordTransfermation(pFC, pTransformation);
                        pResult.Add(pDs.Name, "ת���ɹ�");
                    }
                    catch(Exception ex)
                    {
                        pResult.Add(pDs.Name, "ת��ʧ��/"+ex.Message);
                        if (ex.Message == "The coordinates or measures are out of bounds.")
                           pResult[pDs.Name]="ת��ʧ��/���Ƶ����곬����Ҫ����������߽�";
                        pDs.Delete();
 
                    }
                    finally
                    {
                        pDs = enumDS.Next();
                    }
                }
                IEnumDataset enumDS1 = pWorkspace.get_Datasets(esriDatasetType.esriDTFeatureDataset);
                IDataset pDs1 = enumDS1.Next();
                while (pDs1 != null)
                {
                    IEnumDataset pED = pDs1.Subsets;
                    IDataset pDs2 = pED.Next();
                    while (pDs2 != null)
                    {
                        try
                        {
                            IFeatureClass pFC2 = pDs2 as IFeatureClass;
                            coordTransfermation(pFC2, pTransformation);
                            pResult.Add(pDs2.Name, "ת���ɹ�");
                        }
                        catch (Exception ex)
                        {

                            pResult.Add(pDs2.Name, "ת��ʧ��/" + ex.Message);
                            if (ex.Message == "The coordinates or measures are out of bounds.")
                                pResult[pDs2.Name] = "ת��ʧ��/���Ƶ����곬����Ҫ����������߽�";
                            pDs2.Delete();
                        }
                        finally
                        {
                            pDs2 = pED.Next();
                        }
                    }
                    pDs1 = enumDS1.Next();
                }
                pWorkSpaceEdit.StopEditing(true);
                //vProgress.Close();
                //result = true;
                //if (result == true)
                //{
                //    MessageBox.Show("����ת���ɹ���ɣ�", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);

                //}
                foreach (KeyValuePair<string, string> kvp in pResult)
                {
                    ListViewItem lvi = lstViewResult.Items.Add(kvp.Key);
                    lvi.SubItems.Add(kvp.Value);
                }
                lstViewResult.Refresh();
                this.Height = 529;

            }
            catch (Exception ex)
            {
                

                MessageBox.Show("����ת��ʧ�ܣ�" + ex.Message, "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                vProgress.Close();

            }
            finally
            {
                vProgress.Close();
                pWorkspace = null;
                pWF = null;
            }


                lblRMS.Text = "";
            Application.DoEvents();
            
        }
   
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dgvCtrlPt_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
        
        }

        private void btnSrcPath_Click(object sender, EventArgs e)
        {
           
                if(rdoGDB.Checked||rdoSHP.Checked)
                {
                    FolderBrowserDialog pFBD = new FolderBrowserDialog();

                    if (pFBD.ShowDialog() == DialogResult.OK)
                    {
                        txtSrcPath.Text = pFBD.SelectedPath;
                        txtSrcPath.ForeColor = Color.Black;
                    }
                }
                else
                {
                      openFileDialog1.Filter = "Դ���ݹ����ռ�(mdb)|*.mdb";
                    openFileDialog1.FileName = "";
                    openFileDialog1.Multiselect = false;
                    if (openFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        txtSrcPath.Text = openFileDialog1.FileName;
                        txtSrcPath.ForeColor = Color.Black;
                    }
                }
        }

        private void btnToPath_Click(object sender, EventArgs e)
        {
               if(rdoGDB.Checked||rdoSHP.Checked)
                {
                    FolderBrowserDialog pFBD = new FolderBrowserDialog();

                    if (pFBD.ShowDialog() == DialogResult.OK)
                    {
                        txtToPath.Text = pFBD.SelectedPath;
                        txtToPath.ForeColor = Color.Black;
                    }
               }
               else
               {
                   SaveFileDialog savFD=new SaveFileDialog();
                    savFD.Filter = "Ŀ�����ݹ����ռ�(mdb)|*.mdb";
                    savFD.FileName = "";
                    if (savFD.ShowDialog() == DialogResult.OK)
                    {
                        txtToPath.Text = savFD.FileName;
                        txtToPath.ForeColor = Color.Black;
                    }
               }      
        }

        private void btnOpenxml_Click(object sender, EventArgs e)
        {
            try
            {
                openFileDialog1.Filter = "���������ļ���xml��|*.xml";
                openFileDialog1.FileName = "";
                openFileDialog1.Multiselect = false;
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load(openFileDialog1.FileName);
                    XmlNode xn1 = xmlDoc.FirstChild.NextSibling;
                    XmlNode xn2 = xn1.FirstChild;
                    XmlNode xn3 = xn2.FirstChild;
                    txtCtrlPtPath.Text = xn3.Attributes[0].Value;
                    txtCtrlPtPath.ForeColor = Color.Black;
                    Stream strm = new System.IO.FileStream(txtCtrlPtPath.Text, FileMode.Open, FileAccess.Read);
                    fillDGView(strm);
                    initTransformation();
                    strm.Close();
                    XmlNode xn3_2 = xn3.NextSibling;
                    cboxSrcType.SelectedIndex = cboxSrcType.FindStringExact(xn3_2.Attributes[0].Value);
                    XmlNode xn3_3 = xn3_2.NextSibling;
                    txtSrcPath.Text = xn3_3.Attributes[0].Value;
                    txtSrcPath.ForeColor = Color.Black;
                    XmlNode xn3_4 = xn3_3.NextSibling;
                    txtToPath.Text = xn3_4.Attributes[0].Value;
                    txtToPath.ForeColor = Color.Black;
                    xmlDoc = null;
                }
            }
            catch
            {
 
            }
            
        }

        private void btnSaveXml_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog pSFD = new SaveFileDialog();
                pSFD.Filter = "���������ļ���xml��|*.xml";
              
                if (pSFD.ShowDialog() == DialogResult.OK)
                {
                    File.Copy(Application.StartupPath + "\\..\\res\\xml\\coorTrancfg.xml", pSFD.FileName, true);
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load(pSFD.FileName);
                    XmlNode xn1 = xmlDoc.FirstChild.NextSibling;
                    XmlNode xn2 = xn1.FirstChild;
                    XmlNode xn3 = xn2.FirstChild;
                    xn3.Attributes[0].Value = txtCtrlPtPath.Text;
                    XmlNode xn3_2 = xn3.NextSibling;
                    xn3_2.Attributes[0].Value = cboxSrcType.SelectedItem.ToString();
                    XmlNode xn3_3 = xn3_2.NextSibling;
                    xn3_3.Attributes[0].Value = txtSrcPath.Text;
                    XmlNode xn3_4 = xn3_3.NextSibling;
                    xn3_4.Attributes[0].Value = txtToPath.Text;

                    xmlDoc.Save(pSFD.FileName);
                    xmlDoc = null;
                }
            }
            catch
            {
 
            }
        }

        private void cboxSrcType_SelectedValueChanged(object sender, EventArgs e)
        {
            
        }

        private void txtCtrlPtPath_Enter(object sender, EventArgs e)
        {
            if ((sender as DevComponents.DotNetBar.Controls.TextBoxX).Name == "txtCtrlPtPath"
                && (sender as DevComponents.DotNetBar.Controls.TextBoxX).Text == "��һ�����Ƶ��ļ���txt��")
            {
                txtCtrlPtPath.Text = "";
                txtCtrlPtPath.ForeColor = Color.Black;
            }
            if ((sender as DevComponents.DotNetBar.Controls.TextBoxX).Name == "txtSrcPath"
                && (sender as DevComponents.DotNetBar.Controls.TextBoxX).Text == "Դ���ݹ����ռ�·��")
            {
                txtSrcPath.Text = "";
                txtSrcPath.ForeColor = Color.Black;
            }
            if ((sender as DevComponents.DotNetBar.Controls.TextBoxX).Name == "txtToPath"
                && (sender as DevComponents.DotNetBar.Controls.TextBoxX).Text == "ת��������ݵĹ����ռ�·��")
            {
                txtToPath.Text = "";
                txtToPath.ForeColor = Color.Black;
            }
        }

        private void txtCtrlPtPath_Leave(object sender, EventArgs e)
        {
            if ((sender as DevComponents.DotNetBar.Controls.TextBoxX).Name == "txtCtrlPtPath"
                && (sender as DevComponents.DotNetBar.Controls.TextBoxX).Text == "")
            {
                txtCtrlPtPath.Text = "��һ�����Ƶ��ļ���txt��";
                txtCtrlPtPath.ForeColor = Color.Gray;
            }
            if ((sender as DevComponents.DotNetBar.Controls.TextBoxX).Name == "txtSrcPath"
                && (sender as DevComponents.DotNetBar.Controls.TextBoxX).Text == "")
            {txtSrcPath.Text = "Դ���ݹ����ռ�·��";
                txtSrcPath.ForeColor=Color.Gray;
            }
            if ((sender as DevComponents.DotNetBar.Controls.TextBoxX).Name == "txtToPath"
                && (sender as DevComponents.DotNetBar.Controls.TextBoxX).Text == "")
            {txtToPath.Text = "ת��������ݵĹ����ռ�·��";
                txtToPath.ForeColor=Color.Gray;
             }
        }

        private void frmRdCtrlPt_Load(object sender, EventArgs e)
        {
            cboxSrcType.SelectedIndex = 0;
        }

        private void btnReturn_Click(object sender, EventArgs e)
        {
            this.Height = 343;
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            if (lstViewResult.Items.Count == 0)
                return;
            SaveFileDialog pSFD = new SaveFileDialog();
            pSFD.Filter = "�ı��ļ�(.txt)|*.txt";
            if (pSFD.ShowDialog() == DialogResult.OK)
            {
                Stream tmpStream = pSFD.OpenFile();
                StreamWriter sw = new StreamWriter(tmpStream);
                for (int i = 0; i < lstViewResult.Items.Count; i++)
                {
                    sw.WriteLine(lstViewResult.Items[i].Text + "\t" + lstViewResult.Items[i].SubItems[1].Text);
                }
                MessageBox.Show("����ɹ���", "��ʾ", MessageBoxButtons.OK,
                         MessageBoxIcon.Information);
                sw.Close();
                tmpStream.Close();
                pSFD.Dispose();
            }
        }

        private void rdoSHP_CheckedChanged(object sender, EventArgs e)
        {
            if (!rdoSHP.Checked)
            {
                txtSrcPath.Text = "";
                txtToPath.Text = "";
            }
        }

        private void rdoMDB_CheckedChanged(object sender, EventArgs e)
        {
            if (!rdoMDB.Checked)
            {
                txtSrcPath.Text = "";
                txtToPath.Text = "";
            }
        }

        private void rdoGDB_CheckedChanged(object sender, EventArgs e)
        {
            if (!rdoGDB.Checked)
            {
                txtSrcPath.Text = "";
                txtToPath.Text = "";
            }
        }

     


    }
}