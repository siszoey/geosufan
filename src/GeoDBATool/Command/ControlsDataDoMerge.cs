using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using ESRI.ArcGIS.Geodatabase;
using System.Xml;

namespace GeoDBATool
{
    class ControlsDataDoMerge : Plugin.Interface.CommandRefBase
    {
        private Plugin.Application.IAppGISRef m_Hook;
        public ControlsDataDoMerge()
        {
            base._Name = "GeoDBATool.ControlsDataJoinSetting";
            base._Caption = "ִ���ں�";
            base._Tooltip = "";
            base._Visible = true;
            base._Enabled = true;
            base._Message = "ִ���ں�";

        }

        public override bool Enabled
        {
            get
            {
                if (m_Hook == null) return false;
                if (m_Hook.JoinMergeResultGrid.DataSource == null)
                    return false;
                if (((DataTable)m_Hook.JoinMergeResultGrid.DataSource).TableName != "JoinResultTable")
                    return false;
                return true;
            }
        }

        public override string Message
        {
            get
            {
                Plugin.Application.IAppFormRef pAppFormRef = m_Hook as Plugin.Application.IAppFormRef;
                if (pAppFormRef != null)
                {
                    pAppFormRef.OperatorTips = base._Message;
                }
                return base._Message;
            }
        }

        public override void ClearMessage()
        {
            Plugin.Application.IAppFormRef pAppFormRef = m_Hook as Plugin.Application.IAppFormRef;
            if (pAppFormRef != null)
            {
                pAppFormRef.OperatorTips = string.Empty;
            }
        }

        public override void OnClick()
        {
            if (m_Hook.JoinMergeResultGrid.DataSource == null)
                return ;
            if (((DataTable)m_Hook.JoinMergeResultGrid.DataSource).TableName != "JoinResultTable")
                return ;
            List<IFeatureClass>MergeFeaClsList=null;
            ////////��ȡ�ںϲ���///////
            XmlDocument XmlDoc = new XmlDocument();
            XmlDoc.Load(ModData.v_JoinSettingXML);
            if (null == XmlDoc)
            {
                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "��ȡ�ںϲ��������ļ�ʧ�ܣ�");
                return;
            }
            XmlElement ele = XmlDoc.SelectSingleNode(".//�ں�����") as XmlElement;
            string sIsDesAtrToOri = ele.GetAttribute("���Ը���").Trim();
            ////////
            
            MergeFeaClsList=m_Hook.PolylineSearchGrid.Tag as List<IFeatureClass>;
            if(null==MergeFeaClsList)
                return;
            IMergeOperation Meroper=new ClsMergeOperationer();
            Meroper.JoinFeaClss=MergeFeaClsList;
            //////////�ں�Ҫ�����Դ���ѡ��////////
            if (sIsDesAtrToOri=="Y")
                Meroper.SetDesValueToOri=true;
            else
                Meroper.SetDesValueToOri = false;
            /////////��־
            bool IsCreatLog = false;
            Exception ex = null;
            if (XmlDoc != null)
            {
                XmlElement ele2 = XmlDoc.SelectSingleNode(".//��־����") as XmlElement;
                string LogPath = ele2.GetAttribute("��־·��").Trim();
                if (string.IsNullOrEmpty(LogPath))
                {
                    IsCreatLog = false;
                    Meroper.CreatLog = false;
                }
                else
                {
                    IsCreatLog = true;
                    Meroper.CreatLog = true;
                }
                //////////////////��־�����Դ����¼��Ϣ
                string sAttriPro = string.Empty;
                if (sIsDesAtrToOri == "Y")
                    sAttriPro = "����";
                else 
                    sAttriPro = "�ۼ�";                    
                /////////////����־�������¼����־����
                if (!string.IsNullOrEmpty(LogPath))
                {
                    try
                    {
                        XmlDocument Doc = new XmlDocument();
                        Doc.Load(LogPath);
                        if (null != Doc)
                        {
                            XmlElement Setele = Doc.SelectSingleNode(".//�ںϲ���/����") as XmlElement;
                            Setele.SetAttribute("���Դ���", sAttriPro);
                            Doc.Save(LogPath);
                        }
                    }
                    catch
                    {
                    }
                }
            }
            if (IsCreatLog)
            {
                IJoinLOG JoinLog = new ClsJoinLog();
                JoinLog.onDataJoin_Start(2, out ex);
               
            }
            /////////
            DataTable UnionResTable = new DataTable();
            DataColumn dc1 = new DataColumn("���ݼ�", Type.GetType("System.String"));
            DataColumn dc2 = new DataColumn("Ҫ������", Type.GetType("System.String"));
            DataColumn dc3 = new DataColumn("ԴҪ��ID", Type.GetType("System.Int64"));
            dc3.DefaultValue = -1;
            DataColumn dc4 = new DataColumn("Ŀ��Ҫ��ID", Type.GetType("System.Int64"));
            dc4.DefaultValue = -1;
            DataColumn dc5 = new DataColumn("������", Type.GetType("System.String"));

            UnionResTable.Columns.Add(dc1);
            UnionResTable.Columns.Add(dc2);
            UnionResTable.Columns.Add(dc3);
            UnionResTable.Columns.Add(dc4);
            UnionResTable.Columns.Add(dc5);
            DataTable JoinResultTable=m_Hook.JoinMergeResultGrid.DataSource as DataTable;
            /////////////////
           // DataTable UnionResTable = JoinResultTable;
            UnionResTable.TableName = "MergeResultTable";
          //  UnionResTable.Rows.Clear();
             if (JoinResultTable != null)//////�ںϲ��������Ƕ��Ҫ���໥�ںϵ������
            {
                FrmProcessBar ProcessBar = new FrmProcessBar(JoinResultTable.Rows.Count);
                int max = JoinResultTable.Rows.Count;
                ProcessBar.Show();
               
                while (JoinResultTable.Rows.Count > 0)
                {
                   
                    List<int> lDeleRowNo = new List<int>();
                    long OriFeaOID = -1;
                    List<long> lDesFeaOID = new List<long>();
                    /////��ȡ��һ��
                    string DataSetName = string.Empty;
                    string type = string.Empty;
                    long OriOID = -1;
                    long DesOID = -1;
                    string result = string.Empty;
                    try
                    {
                        DataSetName = JoinResultTable.Rows[0]["���ݼ�"].ToString().Trim();
                        type = JoinResultTable.Rows[0]["Ҫ������"].ToString().Trim();
                        OriOID = Convert.ToInt64(JoinResultTable.Rows[0]["ԴҪ��ID"].ToString());
                        DesOID = Convert.ToInt64(JoinResultTable.Rows[0]["Ŀ��Ҫ��ID"].ToString());
                        result = JoinResultTable.Rows[0]["������"].ToString().Trim();
                    }
                    catch
                    {
                        return;
                    }
                    OriFeaOID = OriOID;
                    ProcessBar.SetFrmProcessBarText("����Ҫ�أ�" + OriFeaOID);
                    ProcessBar.SetFrmProcessBarValue(max - JoinResultTable.Rows.Count);
                    System.Windows.Forms.Application.DoEvents();
                    if (result == "�ѽӱ�")
                    {
                        lDesFeaOID.Add(DesOID);
                    }
                    lDeleRowNo.Add(0);
                    /////����ʣ�µ��У������ڻ��ӱ߹�ϵ��¼���ں��б��У�ͬʱ��¼����Щ�н���ɾ��      
                    #region ����ʣ�µ��У������ڻ��ӱ߹�ϵ��¼���ں��б��У�ͬʱ��¼����Щ�н���ɾ��
                    //////�����μ���������©����һ���߶�δ�Խ�ӱ߽߱�ʱ��һ��ѭ���������ܻ���©�����ں�Ҫ�أ�����ѭ��ʹ��©�������٣�
                    GetAllunionFea(OriOID,ref lDesFeaOID,ref lDeleRowNo, JoinResultTable, type, DataSetName);
                    GetAllunionFea(OriOID, ref lDesFeaOID, ref lDeleRowNo, JoinResultTable, type, DataSetName);
                    #endregion
                    //////�ںϼ�¼�б��е�Ҫ��
                    string sunionres = string.Empty;
                    #region  �ںϼ�¼�б��е�Ҫ��
                    if (lDesFeaOID != null && OriFeaOID!=-1)
                    {
                        for (int i = 0; i < lDesFeaOID.Count; i++)
                        {
                            long UnioOid = lDesFeaOID[i];
                            ProcessBar.SetFrmProcessBarText("�ں�Ҫ�أ�" + OriFeaOID + "��" + UnioOid);
                            System.Windows.Forms.Application.DoEvents();
                            if (type == "Polyline")
                            {
                                if (Meroper.MergePolyline(DataSetName, OriFeaOID, UnioOid))
                                    sunionres = "���ں�";
                                else
                                    sunionres = "δ�ں�";
                            }
                            else if (type == "Polygon")
                            {
                                if(Meroper.MergePolygon(DataSetName, OriFeaOID, UnioOid))
                                    sunionres = "���ں�";
                                else
                                    sunionres = "δ�ں�";
                            }
                        }
                    }
                    #endregion
                    DataRow addrow = UnionResTable.NewRow();
                    addrow["���ݼ�"] = DataSetName;
                    addrow["Ҫ������"] = type;
                    addrow["ԴҪ��ID"] = OriFeaOID;
                    addrow["Ŀ��Ҫ��ID"] = 0;
                    addrow["������"] = sunionres;
                    UnionResTable.Rows.Add(addrow);
                    
                    #region ɾ����
                    if (null != lDeleRowNo)
                    {
                        for (int i = 0; i < lDeleRowNo.Count; i++)
                        {
                            JoinResultTable.Rows.Remove(JoinResultTable.Rows[lDeleRowNo[i]-i]);
                        }
                    }
                    #endregion

                }
                ProcessBar.Close();
             }
             if (IsCreatLog)
             {
                 IJoinLOG JoinLog = new ClsJoinLog();
                 JoinLog.onDataJoin_Terminate(2, out ex);

             }
            //// JoinResultTable.TableName = "MergeResultTable";
            //// ((DataTable)m_Hook.JoinMergeResultGrid.DataSource).TableName = "MergeResultTable";
             m_Hook.JoinMergeResultGrid.DataSource = UnionResTable;
             ControlsDataJoinSearch.SelectALL(m_Hook.JoinMergeResultGrid);//ѡ�����У�xisheng 20110901
        }

        public override void OnCreate(Plugin.Application.IApplicationRef hook)
        {
            m_Hook = hook as Plugin.Application.IAppGISRef;
            if (m_Hook == null) return;
        }
        private void GetAllunionFea( long OID,ref List<long>DesOID,ref List<int>DeleRowNo , DataTable joinTable ,string type,string DatasetName)
        {

            for (int i = 1; i < joinTable.Rows.Count; i++)
            {
                if (DeleRowNo.Contains(i))
                    continue;
                if (m_Hook.JoinMergeResultGrid.Rows[i].Cells[0].Value == null)
                {
                    if (!DeleRowNo.Contains(i))
                        DeleRowNo.Add(i);
                    continue;
                }
                if (((bool)m_Hook.JoinMergeResultGrid.Rows[i].Cells[0].Value) == false)
                {
                    if (!DeleRowNo.Contains(i))
                        DeleRowNo.Add(i);
                    continue;
                }                   
                if (DatasetName == joinTable.Rows[i]["���ݼ�"].ToString().Trim() && type == joinTable.Rows[i]["Ҫ������"].ToString().Trim())
                {
                    long getOriOid = -1;
                    long getDesOid = -1;
                    string getresult = string.Empty;
                    try
                    {
                        getOriOid = Convert.ToInt64(joinTable.Rows[i]["ԴҪ��ID"].ToString());
                        getDesOid = Convert.ToInt64(joinTable.Rows[i]["Ŀ��Ҫ��ID"].ToString());
                        getresult = joinTable.Rows[i]["������"].ToString().Trim();
                    }
                    catch
                    {
                        continue;
                    }

                    if (getOriOid == OID || DesOID.Contains(getOriOid))
                    {
                        DeleRowNo.Add(i);
                        if (getresult == "�ѽӱ�")
                        {
                            if (!DesOID.Contains(getDesOid))
                            {
                                DesOID.Add(getDesOid);                               
                            }                            
                        }
                        if (!DeleRowNo.Contains(i))
                             DeleRowNo.Add(i);
                    }
                    else if (getDesOid == OID || DesOID.Contains(getDesOid))
                    {
                        DeleRowNo.Add(i);
                        if (getresult == "�ѽӱ�")
                        {
                            if (!DesOID.Contains(getOriOid))
                            {
                                DesOID.Add(getOriOid);
                                
                            }                           
                        }
                        if (!DeleRowNo.Contains(i))
                             DeleRowNo.Add(i);
                    }

                }
            }
        }
    }
}
