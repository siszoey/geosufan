using System;
using System.Data;
using System.Windows.Forms;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.Geoprocessor;
using ESRI.ArcGIS.Geoprocessing;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.DataSourcesFile;

namespace GeoUtilities
{
    public partial class frmGeoTrans : DevComponents.DotNetBar.Office2007Form
    {
        private string strColName = "����";
        private string m_strInPrj = "";
        private string m_strOutPrj = "";
        private bool _Writelog = true;  //added by chulili 2012-09-07 �Ƿ�д��־
        public bool WriteLog
        {
            get
            {
                return _Writelog;
            }
            set
            {
                _Writelog = value;
            }
        }
        public frmGeoTrans()
        {
            InitializeComponent();
        }

        private void cmdCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cmdOk_Click(object sender, EventArgs e)
        {
            if (!CheckPrj()) return;

            string strSourceExtension=System.IO.Path.GetExtension(this.txtSource.Text);
            string strExtension=System.IO.Path.GetExtension(this.txtGdbFile.Text);
            //if ((strExtension == ".gdb" || strExtension==".mdb") && (strSourceExtension!=".gdb" && strSourceExtension!=".mdb"))
            //{
            // MessageBox.Show("������ȴ������뾫�ȣ��޷�����ת����", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //   return;
           // }

            try
            {
                this.Cursor = System.Windows.Forms.Cursors.WaitCursor;
                //���ȴ���ת��
                this.lblTips.Text = "����ת��...";
                GetTns();  

                //��������ռ�
                this.lblTips.Text = "����������ݿ�...";

                if (System.IO.Path.GetExtension(this.txtGdbFile.Text) == ".gdb")
                {
                    CreateFileGdb();
                }
                else if (System.IO.Path.GetExtension(this.txtGdbFile.Text)==".mdb")
                {
                    CreatePGdb();
                }
                else
                {
                    System.IO.Directory.CreateDirectory(this.txtGdbFile.Text);
                }
                if (this.WriteLog)
                {
                    Plugin.LogTable.Writelog("��ѡ������ݽ�������ת��");
                    Plugin.LogTable.Writelog("Դ����:" + txtSource.Text + ",Ŀ������:" + txtGdbFile.Text + ",ת������" + cmboTnsMethod.Text);
                }
                //��������ת��
                ProjectClass();

                this.lblTips.Text = "����ת�����";
                this.progressBarX1.Visible = false;
                this.Cursor = System.Windows.Forms.Cursors.Default;
                if (this.WriteLog)
                {
                    Plugin.LogTable.Writelog("����ת�����");
                }

                MessageBox.Show("����ת����ɡ�", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
            catch (Exception ex)
            {
                this.lblTips.Text = "ת�����̳��ִ���";
                this.Cursor = System.Windows.Forms.Cursors.Default;
                this.progressBarX1.Visible = false;
                if (this.WriteLog)
                {
                    Plugin.LogTable.Writelog("ת�����̳��ִ���(" + ex.Message + "),����ֹת��");
                }
                MessageBox.Show("ת�����̳��ִ���"+ex.Message, "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            
        }

        //����Ƿ����ת�������� ��Ҫ�ǽ����ϵ�
        private bool CheckPrj()
        {
            if (this.m_strOutPrj == "" || this.m_strInPrj == "")
            {
                MessageBox.Show("���������Ŀռ�ο�Ϊ�ա�", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }

            int intIn = 0;
            for (int i = 0; i < this.lstLyrFile.Items.Count; i++)
            {
                if (this.lstLyrFile.Items[i].Checked) intIn++;
            }
            if (intIn < 1)
            {
                MessageBox.Show("��Ҫѡ���������ת����ͼ�㡣", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }

            if(this.dtTns.DataSource==null) return true;
            DataTable dt=this.dtTns.DataSource as DataTable;

            int inttns=0;
            for (inttns = 0; inttns < dt.Rows.Count; inttns++)
            {
                DataRow dr = dt.Rows[inttns];
                object obj = dr["tnsValue"];
                if (obj == null) break;

                double dblTest = 0;
                if (!double.TryParse(obj.ToString(), out dblTest)) break;
            }
            if (inttns < dt.Rows.Count)
            {
                MessageBox.Show("ת������Ϊ�ջ򲻷��Ϲ���", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }

            // ��ʾ������ ����������
            this.progressBarX1.Maximum = intIn;
            this.progressBarX1.Minimum = 0;
            this.progressBarX1.Value = 0;
            this.progressBarX1.Step = 1;
            this.progressBarX1.Visible = true;

            return true;
        }

        //��ӱ任��ʽ
        private void InitFrm()
        {
            this.cmboTnsMethod.Items.Add("���ڵ���������ת����");//GEOCENTRIC_TRANSLATION
            this.cmboTnsMethod.Items.Add("���ڵ����߲���ת����");//COORDINATE_FRAME
            this.cmboTnsMethod.Items.Add("Ī���˹��ת����");//MOLODENSKY
            this.cmboTnsMethod.Items.Add("λ��ʸ����");//POSITION_VECTOR
            this.cmboTnsMethod.SelectedIndex = 0;
            //this.cmboTnsMethod.Items.Add("");
        }
        private void frmGeoTrans_Load(object sender, EventArgs e)
        {
            InitFrm();
            InitListViewStyle(this.lstLyrFile);
        }

        private void cmboTnsMethod_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmboTnsMethod.SelectedIndex < 0) return;

            DataTable dt = new DataTable();
            dt.Columns.Add("tnsName", typeof(string));
            dt.Columns.Add("tnsValue", typeof(double));

            string strMethod = cmboTnsMethod.Text;
            if (strMethod.Contains("���ڵ���������ת����") || strMethod.Contains("Ī���˹��ת����"))
            {
                DataRow drXoff = dt.NewRow();
                drXoff["tnsName"] = "Xƫ��(��)";
                drXoff["tnsValue"] = 0;
                dt.Rows.Add(drXoff);

                DataRow drYoff = dt.NewRow();
                drYoff["tnsName"] = "Yƫ��(��)";
                drYoff["tnsValue"] = 0;
                dt.Rows.Add(drYoff);

                DataRow drZoff = dt.NewRow();
                drZoff["tnsName"] = "Zƫ��(��)";
                drZoff["tnsValue"] = 0;
                dt.Rows.Add(drZoff);
            }
            else if (strMethod.Contains("λ��ʸ����") || strMethod.Contains("���ڵ����߲���ת����"))
            {
                DataRow drXoff = dt.NewRow();
                drXoff["tnsName"] = "Xƫ��(��)";
                drXoff["tnsValue"] = 0;
                dt.Rows.Add(drXoff);

                DataRow drYoff = dt.NewRow();
                drYoff["tnsName"] = "Yƫ��(��)";
                drYoff["tnsValue"] = 0;
                dt.Rows.Add(drYoff);

                DataRow drZoff = dt.NewRow();
                drZoff["tnsName"] = "Zƫ��(��)";
                drZoff["tnsValue"] = 0;
                dt.Rows.Add(drZoff);

                DataRow drXRotation = dt.NewRow();
                drXRotation["tnsName"] = "X��ת(��)";
                drXRotation["tnsValue"] = 0;
                dt.Rows.Add(drXRotation);

                DataRow drYRotation = dt.NewRow();
                drYRotation["tnsName"] = "Y��ת(��)";
                drYRotation["tnsValue"] = 0;
                dt.Rows.Add(drYRotation);

                DataRow drZRotaion = dt.NewRow();
                drZRotaion["tnsName"] = "Z��ת(��)";
                drZRotaion["tnsValue"] = 0;
                dt.Rows.Add(drZRotaion);

                DataRow drScale = dt.NewRow();
                drScale["tnsName"] = "��������(�������)";
                drScale["tnsValue"] = 0;
                dt.Rows.Add(drScale);
            }
            else
            {
            }

            this.dtTns.DataSource = dt;
        }

        private void btnSource_Click(object sender, EventArgs e)
        {
            if (rdoGDB.Checked)
                btnGDB_Click(sender, e);
            else if (rdoMDB.Checked)
                btnMDB_Click(sender, e);
            else
                btnSHP_Click(sender, e);//yjl20110822 modify
        }

        private void LstAllLyrFile(IWorkspace pWks)
		 {
            try
        {
            IFeatureWorkspace pFeaWks = pWks as IFeatureWorkspace;
            if (pFeaWks == null) return;

            ESRI.ArcGIS.Geometry.ISpatialReference pSpa = null;

            IEnumDatasetName pEnumFeaCls = pWks.get_DatasetNames(esriDatasetType.esriDTFeatureClass);
            IDatasetName pFeaClsName = pEnumFeaCls.Next();
            while (pFeaClsName != null)
            {
                if (pSpa == null)
                {
                    IName pPrjName = pFeaClsName as IName;
                    IFeatureClass pFeaCls = pPrjName.Open() as IFeatureClass;
                    IGeoDataset pGeodataset = pFeaCls as IGeoDataset;

                    //��ÿռ�ο� Դ��
                    pSpa = pGeodataset.SpatialReference;
                    m_strInPrj = ExportToESRISpatialReference(pSpa);
                }

                this.lstLyrFile.Items.Add(pFeaClsName.Name);
                pFeaClsName = pEnumFeaCls.Next();
            }

            IEnumDatasetName pEnumDataNames = pWks.get_DatasetNames(esriDatasetType.esriDTFeatureDataset);
            IDatasetName pDatasetName = pEnumDataNames.Next();
            while (pDatasetName != null)
            {
                IEnumDatasetName pSubNames = pDatasetName.SubsetNames;
                IDatasetName pSubName = pSubNames.Next();
                while (pSubName != null)
                {
                    if (pSpa == null)
                    {
                        IName pPrjName = pSubName as IName;
                        IFeatureClass pFeaCls = pPrjName.Open() as IFeatureClass;
                        IGeoDataset pGeodataset = pFeaCls as IGeoDataset;

                        //��ÿռ�ο� Դ��
                        pSpa = pGeodataset.SpatialReference;
                        m_strInPrj = ExportToESRISpatialReference(pSpa);
                    }

                    this.lstLyrFile.Items.Add(pSubName.Name);
                    pSubName = pSubNames.Next();
                }

                pDatasetName = pEnumDataNames.Next();
            }

            for (int i = 0; i < this.lstLyrFile.Items.Count; i++)
            {
                this.lstLyrFile.Items[i].Checked = true;
			}
            }
            catch
            {
            }
        }

        private void InitListViewStyle(ListView plv)
        {
            // ʧ����ʱ��������ѡ��
            plv.HideSelection = false;

            // Set the view to show details.
            plv.View = View.Details;
            // Allow the user to edit item text.
            plv.LabelEdit = false;
            // Allow the user to rearrange columns.
            plv.AllowColumnReorder = true;
            // Display check boxes.
            plv.CheckBoxes = true;   //_heluyuan_20071117_modify
            // Select the item and subitems when selection is made.
            plv.FullRowSelect = true;
            // Sort the items in the list in ascending order.
            plv.Sorting = SortOrder.Ascending;

            plv.Columns.Add(strColName, -2, HorizontalAlignment.Center);
            //plv.Columns.Add(strAliasName, -2, HorizontalAlignment.Left);
        }

        private void lstLyrFile_MouseDown(object sender, MouseEventArgs e)
        {

        }

        private void lstLyrFile_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.A)
            {
                foreach (ListViewItem item in this.lstLyrFile.Items)
                {
                    item.Selected = true;
                }
            }
        }

        private IWorkspace GetWorkspace(string sFilePath, int wstype)
        {
            IWorkspace pWks = null;

            try
            {
                IPropertySet pPropSet = new PropertySetClass();
                switch (wstype)
                {
                    case 1:
                        AccessWorkspaceFactory pAccessFact = new AccessWorkspaceFactoryClass();
                        pPropSet.SetProperty("DATABASE", sFilePath);
                        pWks = pAccessFact.Open(pPropSet, 0);
                        pAccessFact = null;
                        break;
                    case 2:
                        FileGDBWorkspaceFactoryClass pFileGDBFact = new FileGDBWorkspaceFactoryClass();
                        pPropSet.SetProperty("DATABASE", sFilePath);
                        pWks = pFileGDBFact.Open(pPropSet, 0);
                        pFileGDBFact = null;
                        break;
                    case 3:
                        break;
                }
                pPropSet = null;
                return pWks;
            }
            catch
            {
                return null;
            }
        }

        private void btnGdbFile_Click(object sender, EventArgs e)
        {
            OutFile();
        }

        private void OutFile()
        {
            SaveFileDialog savefile = new SaveFileDialog();
            savefile.Filter = "*.gdb|*.gdb|*.mdb|*.mdb|*.shp|*.shp";

            if (savefile.ShowDialog() == DialogResult.OK)
            {
                string str = savefile.FileName;
                if (System.IO.Path.GetExtension(str) == ".shp")
                {
                    this.txtGdbFile.Text = str.Substring(0,str.Length-4);
                    return;
                }

                this.txtGdbFile.Text = str;
            }
        }

        private void btnPrjFile_Click(object sender, EventArgs e)
        {
            string sInstall = ReadRegistry("SOFTWARE\\ESRI\\CoreRuntime");
            if (sInstall == "") //added by chulili 2012-11-13 ƽ̨��ArcGIS9.3����ArcGIS10����Ӧ��ע���·��Ҫ�޸�
            {
                sInstall = ReadRegistry("SOFTWARE\\ESRI\\Engine10.0\\CoreRuntime");
            }
            if (sInstall == "")
            {
                sInstall = ReadRegistry("SOFTWARE\\ESRI\\Desktop10.0\\CoreRuntime");
            }   //added by chulili 2012-11-13  end
            sInstall = sInstall + "\\Coordinate Systems";

            OpenFileDialog filedialog = new OpenFileDialog();
            filedialog.Multiselect = false;
            filedialog.InitialDirectory = sInstall;
            filedialog.Filter = "*.prj|*.prj";
            filedialog.Title = "ѡ��prj�ļ�";
            if (filedialog.ShowDialog() == DialogResult.OK)
            {
                string str = filedialog.FileName;
                this.txtPrjFile.Text = str;

                //ֱ�Ӷ�ȡ����ϵͳ
				try
                {
                ESRI.ArcGIS.Geometry.ISpatialReferenceFactory pPrjFac = new ESRI.ArcGIS.Geometry.SpatialReferenceEnvironmentClass();
                ESRI.ArcGIS.Geometry.ISpatialReference pSpa=pPrjFac.CreateESRISpatialReferenceFromPRJFile(str);

                m_strOutPrj = ExportToESRISpatialReference(pSpa);
				 }
                catch
                {
                    
                }
            }
        }

        public string ExportToESRISpatialReference(ISpatialReference spatialReference)
        {
            int bytes = 0;
            string buffer = null;
            ISpatialReference projectedCoordinateSystem = spatialReference;
            IESRISpatialReferenceGEN parameterExport = projectedCoordinateSystem as IESRISpatialReferenceGEN;
            parameterExport.ExportToESRISpatialReference(out buffer,out bytes);

            return buffer;
        }

        //ִ��ת����gp
        private IVariantArray GetTns()
        {
            IVariantArray pTempArray = new VarArrayClass();
            string strMethod = "";
            GetTnsInfo(ref strMethod, ref pTempArray);

            ESRI.ArcGIS.DataManagementTools.CreateCustomGeoTransformation tns = new ESRI.ArcGIS.DataManagementTools.CreateCustomGeoTransformation();
            tns.custom_geot = strMethod;
            tns.geot_name = "temp";
            tns.in_coor_system = m_strInPrj;
            tns.out_coor_system = m_strOutPrj;

            Geoprocessor gp = new Geoprocessor();
            gp.OverwriteOutput = true;

            if (!RunTool(gp, tns))
            {
                return null;
            }

            return null;
        }

        //���ת���ķ�����ת���Ĳ���
        private void GetTnsInfo(ref string strMethod, ref IVariantArray pArray)
        {
            pArray = new VarArrayClass();
            if (this.cmboTnsMethod.SelectedIndex < 0) return;

            DataTable dt = this.dtTns.DataSource as DataTable;

            strMethod = this.cmboTnsMethod.Text;
            if (strMethod == "���ڵ���������ת����")
            {
                pArray.Add("PARAMETER['X_Axis_Translation'," + GetTnsValue("Xƫ��(��)", dt).ToString() + "]");
                pArray.Add("PARAMETER['Y_Axis_Translation'," + GetTnsValue("Yƫ��(��)", dt).ToString() + "]");
                pArray.Add("PARAMETER['Z_Axis_Translation'," + GetTnsValue("Zƫ��(��)", dt).ToString() + "]");
            }
            else if (strMethod == "λ��ʸ����")
            {
                pArray.Add("PARAMETER['X_Axis_Translation'," + GetTnsValue("Xƫ��(��)", dt).ToString() + "]");
                pArray.Add("PARAMETER['Y_Axis_Translation'," + GetTnsValue("Yƫ��(��)", dt).ToString() + "]");
                pArray.Add("PARAMETER['Z_Axis_Translation'," + GetTnsValue("Zƫ��(��)", dt).ToString() + "]");
                pArray.Add("PARAMETER['X_Axis_Rotation'," + GetTnsValue("X��ת(��)", dt).ToString() + "]");
                pArray.Add("PARAMETER['Y_Axis_Rotation'," + GetTnsValue("Y��ת(��)", dt).ToString() + "]");
                pArray.Add("PARAMETER['Z_Axis_Rotation'," + GetTnsValue("Z��ת(��)", dt).ToString() + "]");
                pArray.Add("PARAMETER['Scale_Difference'," + GetTnsValue("��������(�������)", dt).ToString() + "]");
            }
            else if (strMethod == "")
            {
            }

            string strArr="";
            for (int i = 0; i < pArray.Count; i++)
            {
                string strTemp = pArray.get_Element(i).ToString();
                if (strArr == "")
                {
                    strArr = strTemp;
                }
                else
                {
                    strArr = strArr + "," + strTemp;
                }
            }

            strMethod ="GEOGTRAN[METHOD['" +GetMethod(strMethod) + "']," + strArr;
        }

        /// <summary>
        /// ��������Ӣ��ת��
        /// </summary>
        /// <param name="strMethod">���ķ�����</param>
        /// <returns></returns>
        private string GetMethod(string strMethod)
        {
           
            switch (strMethod.Trim())
            {
                case "���ڵ���������ת����":
                    strMethod = "GEOCENTRIC_TRANSLATION";
                    break;
                case "���ڵ����߲���ת����":
                    strMethod = "COORDINATE_FRAME";
                    break;
                case "Ī���˹��ת����":
                    strMethod = "MOLODENSKY";
                    break;
                case "λ��ʸ����":
                    strMethod = "POSITION_VECTOR";
                    break;
            }
            return strMethod;
        }
        //
        private double GetTnsValue(string strName, DataTable dt)
        {
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow dr = dt.Rows[i];
                string strAtt = dr["tnsName"].ToString();
                if (strAtt.ToUpper() == strName.ToUpper())
                {
                    object obj = dr["tnsValue"];
                    if (obj == null) return 0;

                    double dbltemp = 0;
                    double.TryParse(obj.ToString(), out dbltemp);
                    return dbltemp;
                }
            }

            return 0;
        }


        //���������gdb
        private void CreateFileGdb()
        {
            string strfileName = this.txtGdbFile.Text;
            if (strfileName == "") return;

            string strPath = System.IO.Path.GetDirectoryName(strfileName);
            string strName = System.IO.Path.GetFileName(strfileName);

            ESRI.ArcGIS.DataManagementTools.CreateFileGDB creategdb = new ESRI.ArcGIS.DataManagementTools.CreateFileGDB();
            creategdb.out_folder_path = strPath;
            creategdb.out_name = strName;

            //
            Geoprocessor gp = new Geoprocessor();
            gp.OverwriteOutput = true;

            if (!RunTool(gp, creategdb))
            {
                return;
            }

        }

        //���������gdb
        private void CreatePGdb()
        {
            string strfileName = this.txtGdbFile.Text;
            if (strfileName == "") return;

            string strPath = System.IO.Path.GetDirectoryName(strfileName);
            string strName = System.IO.Path.GetFileNameWithoutExtension(strfileName);

            ESRI.ArcGIS.DataManagementTools.CreatePersonalGDB creategdb = new ESRI.ArcGIS.DataManagementTools.CreatePersonalGDB();
            creategdb.out_folder_path = strPath;
            creategdb.out_name = strName;

            //
            Geoprocessor gp = new Geoprocessor();
            gp.OverwriteOutput = true;

            if (!RunTool(gp, creategdb))
            {
                return;
            }

        }

        //��ͶӰ�任
        private void ProjectClass()
        {

            if (this.lstLyrFile.Items.Count < 0) return;

            for (int i = 0; i < this.lstLyrFile.Items.Count; i++)
            {
                if (!this.lstLyrFile.Items[i].Checked) continue;
                string strFeaName = this.lstLyrFile.Items[i].Text;
                this.lblTips.Text = "���ڽ�������ת����" + strFeaName;

                ESRI.ArcGIS.DataManagementTools.Project prjtool = new ESRI.ArcGIS.DataManagementTools.Project();
                prjtool.in_dataset = this.txtSource.Text + "\\"+ strFeaName;
                if (rdoSHP.Checked && !prjtool.in_dataset.ToString().EndsWith(".shp"))
                    prjtool.in_dataset += ".shp";//shp�ļ������Ӻ�׺��
                prjtool.out_dataset = this.txtGdbFile.Text + "\\" + strFeaName;
                prjtool.out_coor_system = m_strOutPrj;

                //���ǵ�ֱ����ͶӰ�ķ�ʽ ������ƫ�ƺ���ת
                if (this.dtTns.DataSource != null)
                {
                    prjtool.transform_method = "temp";
                }

                //
                Geoprocessor gp = new Geoprocessor();
                gp.OverwriteOutput = true;

                RunTool(gp, prjtool);

                this.progressBarX1.PerformStep();
            }

        }

        //gp��ִ��
        private void ExecuteGP(IVariantArray parameters, string strToolName)
        {
            ITrackCancel pTrackCancel = null;

            //�ҵ�tool
            ESRI.ArcGIS.Geoprocessing.GeoProcessor _geoPro = new GeoProcessor();
            pTrackCancel = new TrackCancelClass();
            IVariantArray pVArray = new VarArrayClass();

            IGPEnvironmentManager pgpEnv = new GPEnvironmentManager();
            IGPMessages pGpMessages; //= _geoPro.Validate(parameters, false, pgpEnv);
            IGPComHelper pGPCOMHelper = new GpDispatch();

            //�����ǹؼ����������ֵ�Ļ�����ô�ͻᱨ��
            IGPEnvironmentManager pEnvMgr = pGPCOMHelper.EnvironmentManager;
            pgpEnv.PersistAll = true;
            pGpMessages = new GPMessagesClass();

            // Execute the model tool by name.
            _geoPro.Execute(strToolName, parameters, pTrackCancel);
        }

        //�ж�gp����
        private bool RunTool(Geoprocessor geoprocessor, IGPProcess process)
        {
            // Set the overwrite output option to true
            geoprocessor.OverwriteOutput = true;

            try
            {
                geoprocessor.Execute(process, null);
                return true;

            }
            catch (Exception err)
            {
                Console.WriteLine(err.Message);
                return false;
            }
        }

        // Function for returning the tool messages.
        private void ReturnMessages(Geoprocessor gp)
        {
            string ms = "";
            if (gp.MessageCount > 0)
            {
                for (int Count = 0; Count <= gp.MessageCount - 1; Count++)
                {
                    ms += gp.GetMessage(Count);
                }
            }
        }


        private string ReadRegistry(string p)
        {
            /// <summary> 
            /// ��ע�����ȡ��ָ�������·�� 

            /// </summary> 

            /// <param name="sKey"> </param> 

            /// <returns> </returns> 

            //Open the subkey for reading 

            Microsoft.Win32.RegistryKey rk = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(p, true);

            if (rk == null) return "";

            // Get the data from a specified item in the key. 

            return (string)rk.GetValue("InstallDir");
        }

        private void dtTns_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.ColumnIndex < 0) return;
            if (this.dtTns.Columns[e.ColumnIndex].Name == "tnsName")
            {
                this.dtTns.ReadOnly = true;
            }
            else
            {
                this.dtTns.ReadOnly = false;
            }
        }

        private void btnGDB_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folder = new FolderBrowserDialog();
            folder.ShowNewFolderButton = false;
            folder.Description = "ѡ��FGDB�ļ�";
            if (folder.ShowDialog() == DialogResult.OK)
            {
                string str = folder.SelectedPath;
                this.txtSource.Text = str;
            }
            else
            {
                return;
            }

            IWorkspace pWks = GetWorkspace(this.txtSource.Text, 2);
            if (pWks == null)
            {
                this.txtSource.Text = "";
                MessageBox.Show("����FGDB�Ĺ����ռ䡣", "ϵͳ��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

			this.lstLyrFile.Items.Clear();
            LstAllLyrFile(pWks);
        }

        private void btnMDB_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "*.mdb|*.mdb";
            openFile.Title = "ѡ��mdb�ļ�";
            openFile.Multiselect = false;
            if (openFile.ShowDialog() == DialogResult.OK)
            {
                string str = openFile.FileName;
                this.txtSource.Text = str;
            }
            else
            {
                return;
            }

            IWorkspace pWks = GetWorkspace(this.txtSource.Text, 1);
            if (pWks == null)
            {
                this.txtSource.Text = "";
                MessageBox.Show("����PGDB�Ĺ����ռ䡣", "ϵͳ��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
			this.lstLyrFile.Items.Clear();

            LstAllLyrFile(pWks);
        }

        private void btnSHP_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "*.shp|*.shp";
            openFile.Title = "ѡ��shp�ļ�";
            openFile.Multiselect = true ;

            ESRI.ArcGIS.Geometry.ISpatialReference pSpa = null;

            if (openFile.ShowDialog() == DialogResult.OK)
            {
                string[] strs = openFile.FileNames;
                this.lstLyrFile.Items.Clear();
                for (int i = 0; i < strs.GetLength(0); i++)
                {
                    string str = strs[i];
                    this.lstLyrFile.Items.Add(System.IO.Path.GetFileNameWithoutExtension(str));

                    //���һ��Դ���ݵĿռ�ο�
                    //��ÿռ�ο� Դ��

                    if (pSpa == null)
                    {
                        IWorkspaceFactory pFac = new ShapefileWorkspaceFactory();
                        IWorkspace pWks= pFac.OpenFromFile(System.IO.Path.GetDirectoryName(str), 0);
                        if (pWks == null) continue;
                        IFeatureWorkspace pFeaWks = pWks as IFeatureWorkspace;
                        IFeatureClass pFeaCls = pFeaWks.OpenFeatureClass(System.IO.Path.GetFileNameWithoutExtension(str));
                        if (pFeaCls == null) continue;
                        IGeoDataset pGeoData = pFeaCls as IGeoDataset;

                        pSpa = pGeoData.SpatialReference;
                        if (pSpa == null) continue;
                        m_strInPrj = ExportToESRISpatialReference(pSpa);
                        pFeaWks = null;
                    }
                }

                for (int i = 0; i < this.lstLyrFile.Items.Count; i++)
                {
                    this.lstLyrFile.Items[i].Checked = true;
                }

                this.txtSource.Text = System.IO.Path.GetDirectoryName(strs[0]);
            }
        }

        private void btnTAGGDB_Click(object sender, EventArgs e)
        {

        }

        private void btnSelAll_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem lvi in lstLyrFile.Items)
            {
                lvi.Checked = true;
            }
        }

        private void btnSelReverse_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem lvi in lstLyrFile.Items)
            {
                if (!lvi.Checked)
                    lvi.Checked = true;
                else
                    lvi.Checked = false;
            }
        }

        private void dtTns_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            MessageBox.Show("��������ֵ", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

    }
}