using System;
using System.Collections.Generic;
using System.Text;
using System.Data.OleDb;
using System.Data;

//������ݿ�Ķ�д����
namespace GeoDataCenterFunLib
{
    public class GeoDataCenterDbFun
    {
        //------------------------------------------
        //���ܣ����ݱ��ʽ��ȡaccess���е�����
        //����˵��
        //      strCon      ���ӱ��ʽ
        //      strExp      ��ѯ���ʽ
        //���أ���Ӧ����е�����ֵ
        //ʱ�䣺2011-2-12
        //ʵ�֣�
        //-------------------------------------------
        public string GetInfoFromMdbByExp(string strCon,string strExp)
        {
            string strValue = "";
            OleDbConnection mycon = new OleDbConnection(strCon);   //����OleDbConnection����ʵ�����������ݿ�
            OleDbCommand aCommand = new OleDbCommand(strExp, mycon);
            try
            {
                mycon.Open();

                //����datareader   ���������ӵ���     
                OleDbDataReader aReader = aCommand.ExecuteReader();
                if (aReader.Read())
                {
                    strValue = aReader[0].ToString();
                }

                //�ر�reader����     
                aReader.Close();

                //�ر�����,�����Ҫ     
                mycon.Close();
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return strValue;
        }

        //------------------------------------------
        //���ܣ����ݱ��ʽɾ�����߸���access���е�����
        //����˵��
        //      strCon      ���ӱ��ʽ
        //      strExp      SQL���ʽ
        //���ߣ�ϯʤ
        //ʱ�䣺2011-03-10
        //ʵ�֣�
        //-------------------------------------------
        public void ExcuteSqlFromMdb(string strCon, string strExp)
        {
             
            OleDbConnection mycon = new OleDbConnection(strCon);   //����OleDbConnection����ʵ�����������ݿ�
            OleDbCommand aCommand = new OleDbCommand(strExp, mycon);
           
                mycon.Open();
                aCommand.ExecuteNonQuery();
                //�ر�����,�����Ҫ     
                mycon.Close();
            
            //catch (System.Exception e)
            //{
            //    Console.WriteLine(e.Message);
            //}
            
           
        }

        //------------------------------------------
        //���ܣ����ݱ��ʽ��Ѱaccess���е�����
        //����˵��
        //      strCon      ���ӱ��ʽ
        //      strExp      SQL���ʽ
        //���ߣ�ϯʤ
        //ʱ�䣺2011-03-10
        //ʵ�֣�
        //-------------------------------------------
        public int ExcuteSqlFromMdbEx(string strCon, string strExp)
        {

            OleDbConnection mycon = new OleDbConnection(strCon);   //����OleDbConnection����ʵ�����������ݿ�
            OleDbCommand aCommand = new OleDbCommand(strExp, mycon);
            int ii = -1;
            mycon.Open();
            OleDbDataReader aReader = aCommand.ExecuteReader();
            if (aReader.Read() == true)
            {
                ii = 1;
            }
            //�ر�����,�����Ҫ     
            mycon.Close();

            return ii;
        }

        //------------------------------------------
        //���ܣ����ݱ��ʽ�õ�Access���е�id
        //����˵��
        //      strCon      ���ӱ��ʽ
        //      strExp      SQL���ʽ
        //���ߣ�ϯʤ
        //ʱ�䣺2011-03-10
        //ʵ�֣�
        //-------------------------------------------
        public int GetIDFromMdb(string strCon, string strExp)
        {
            int i=0;
            OleDbConnection mycon = new OleDbConnection(strCon);   //����OleDbConnection����ʵ�����������ݿ�
            OleDbCommand aCommand = new OleDbCommand(strExp, mycon);
            try
            { 
                mycon.Open();

                //����datareader   ���������ӵ���     
                OleDbDataReader aReader = aCommand.ExecuteReader();
                if (aReader.Read())
                {
                     i = Convert.ToInt32(aReader[0].ToString());
                }

                //�ر�reader����     
                aReader.Close();

                //�ر�����,�����Ҫ     
                mycon.Close();
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return i;
            
           
        }

        //------------------------------------------
        //���ܣ����ݱ��ʽ�õ�Access���е����������
        //����˵��
        //      strCon      ���ӱ��ʽ
        //      strExp      SQL���ʽ
        //���ߣ�ϯʤ
        //ʱ�䣺2011-03-10
        //ʵ�֣�
        //-------------------------------------------
        public int GetCountFromMdb(string strCon, string strExp)
        {
            int i=0 ;
            OleDbConnection mycon = new OleDbConnection(strCon);   //����OleDbConnection����ʵ�����������ݿ�
            OleDbCommand aCommand = new OleDbCommand(strExp, mycon);
            try
            {
                mycon.Open();
                i=Convert.ToInt32(aCommand.ExecuteScalar());
                //�ر�����,�����Ҫ     
                mycon.Close();
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return i;
        }
        
        //------------------------------------------
        //���ܣ����ݴ��ݵ�ֵ���µ�ͼ�����Ϣ��
        //���ߣ�ϯʤ
        //ʱ�䣺2011-03-10
        //ʵ�֣�
        //-------------------------------------------
        public void UpdateMdbInfoTable(string strBusness, string strYear, string strType, string strArea, string strScale)
        {
            string Layers="";
            GetDataTreeInitIndex dIndex = new GetDataTreeInitIndex();
            string mypath = dIndex.GetDbInfo();
            string strCon = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + mypath + ";Mode=ReadWrite|Share Deny None;Persist Security Info=False";  //�����������ݿ��ַ���
            string strExp = "";
            strExp= string.Format("select ͼ����� from ���ݱ���� where ҵ��������='{0}' and ���='{1}' and ��������='{2}' and ������='{3}'",
                strBusness, strYear, strArea, strScale);
            DataTable dt1= GetDataTableFromMdb(strCon, strExp);
            strExp = "select * from ��׼ר����Ϣ��";
            DataTable dt2 = GetDataTableFromMdb(strCon, strExp);
            for (int i = 0; i < dt2.Rows.Count; i++)//��һ��ѭ����׼ͼ��
            {
                Layers = "";
                for (int j = 0; j < dt1.Rows.Count; j++)//�ڶ���ѭ��ѭ���ж����ͼ���������û���ǹؼ�ͼ���
                {
                    
                    if (dt2.Rows[i]["�ؼ�ͼ��"].ToString().Equals("")||dt1.Rows[j][0].Equals(dt2.Rows[i]["�ؼ�ͼ��"]))//���ǹؼ�ͼ��
                    {
                        for (int k = 0; k < dt1.Rows.Count;k++)//������ѭ�����ͼ���а����ڸ�ר��ͼ������е�
                        {
                             if(GetExists(dt1.Rows[k][0].ToString(),dt2.Rows[i]["ͼ�����"].ToString()))
                                Layers += dt1.Rows[k][0].ToString()+"/";
                        }
                        j =  dt1.Rows.Count;//��ֹ�ڶ���ѭ��
                    }
                }
                if (Layers != "")
                {
                    Layers = Layers.Substring(0, Layers.LastIndexOf("/"));
                }
                    //�жϵ�ͼ�����Ϣ�� ���Ƿ���ڸü�¼ ���ھ͸��� �����ھ���д
                    strExp = string.Format("select * from ��ͼ�����Ϣ�� where ר������='{0}' and ���='{1}' and ��������='{2}' and ������='{3}'",dt2.Rows[i]["ר������"].ToString(), strYear,strArea,strScale);
                    int iReturn = ExcuteSqlFromMdbEx(strCon, strExp);
                    if (iReturn == -1)
                    {
                        string strBuffer = "select �������� from ���ݵ�Ԫ�� where �������� ='" + strArea + "'";
                        string strXzName = GetInfoFromMdbByExp(strCon, strBuffer);

                        strExp = string.Format("insert into ��ͼ�����Ϣ��(ר������,���,��������,��������,������,ͼ�����) values('{0}','{1}','{2}','{3}','{4}','{5}')", dt2.Rows[i]["ר������"].ToString(), strYear, strArea, strXzName, strScale, Layers);
                    }
                    else
                    {
                        strExp = "update ��ͼ�����Ϣ�� set ͼ�����='" + Layers + "' where ר������='" + dt2.Rows[i]["ר������"].ToString() + "' And �������� ='" + strArea + "' And ���='" +
                   strYear + "'And  ������='" + strScale + "'";
                    }
                
                  ExcuteSqlFromMdb(strCon, strExp);//ִ�и���
            }
             
           

        }
        //���ͼ��������Ƿ����ͼ��
        private bool GetExists(string layer, string layers)
        {
            bool exist = false;
            if (layers.Contains("/"))
            {
                string[] arrlayer = layers.Split('/');
                for (int ii = 0; ii < arrlayer.Length; ii++)
                {
                    if (arrlayer[ii].Trim() == layer)
                    {
                        exist = true;
                        break;
                    }
                }
            }
            else
            {
                if (layers.Trim() == layer)
                    exist = true;
            }
            return exist;
        }

        public List<string> GetDataReaderFromMdb(string strCon, string strExp)
        {  
            List<string> readlist = new List<string>();
            try
            {
                OleDbConnection conn = new OleDbConnection(strCon);
                conn.Open();
                OleDbCommand cmd = new OleDbCommand(strExp, conn);
                OleDbDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    readlist.Add(reader[0].ToString());
                }

                reader.Close();
                conn.Close();
            }
            catch { }
            return readlist;
        }
        public DataTable GetDataTableFromMdb(string strCon, string strExp)
        {
            OleDbConnection conn = new OleDbConnection(strCon);
            conn.Open();
            OleDbDataAdapter da = new OleDbDataAdapter(strExp, conn);
            DataTable dt = new DataTable();
            da.Fill(dt);
            conn.Close();
            return dt;
        }

    }
}
