using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using DevComponents.DotNetBar.Controls;
using Microsoft.Office.Interop.Excel;
using Microsoft.Office.Core;

using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;

namespace GeoStatistics
{
    public class ModCommon
    {

        public static void DoEightStatistics(IFeatureClass pFeatureClass,string strMjFieldName,SysCommon.CProgress pProgress)
        {
            pProgress.SetProgress(1, "�����ֵ������ȼ�ͳ��");
            bool bRes = false;
            //�ֵ������ȼ�
            bRes = DoLDZL_Statistic(pFeatureClass, strMjFieldName);
            bRes=DoLCLDZL_Statistic(pFeatureClass, strMjFieldName); //�ֳ�����ͳ�Ʊ����� ygc 2012-10-10

            pProgress.SetProgress(2, "�����ֵ����÷���滮���ͳ��");
            //�ֵ����÷���滮���
            bRes=DoLDLYFXGHMJ_Statistic(pFeatureClass, strMjFieldName);
            bRes=DoLCLDLYFXGHMJ_Statistic(pFeatureClass, strMjFieldName); //�ֳ�����ͳ�Ʊ����� ygc 2012-10-10

            pProgress.SetProgress(3, "�����ֵؽṹ��״ͳ��");
            //�ֵؽṹ��״
            bRes=DoLDJGXZ_Statistic(pFeatureClass, strMjFieldName);
            bRes=DoLCLDJGXZ_Statistic(pFeatureClass, strMjFieldName); //�ֳ�����ͳ�Ʊ����� ygc 2012-10-10

            pProgress.SetProgress(4, "�����ֵؼ�ɭ������滮ͳ��");
            //�ֵؼ�ɭ������滮
            bRes=DoLDSLMJGH_Statistic(pFeatureClass, strMjFieldName);
            bRes=DoLCLDSLMJGH_Statistic(pFeatureClass, strMjFieldName); //�ֳ�����ͳ�Ʊ����� ygc 2012-10-10

            pProgress.SetProgress(5, "�����ֵ���״ͳ��");
            //�ֵ���״
            bRes=DoLDXZ_Statistic(pFeatureClass, strMjFieldName);
            bRes=DoLCLDXZ_Statistic(pFeatureClass, strMjFieldName); //�ֳ�����ͳ�Ʊ����� ygc 2012-10-10

            pProgress.SetProgress(6, "���й��Ҽ������ֵطֱ����ȼ���״ͳ��");
            //���Ҽ������ֵطֱ����ȼ���״
            bRes=DoGJGYHDJ_Statistic(pFeatureClass, strMjFieldName);
            bRes=DoLCGJGYHDJ_Statistic(pFeatureClass, strMjFieldName);//�ֳ�����ͳ�Ʊ����� ygc 2012-10-10

            pProgress.SetProgress(7, "�����ֵر����ȼ�ͳ��");
            //�ֵر����ȼ�
            bRes=DoLDBHDJ_Statistic(pFeatureClass, strMjFieldName);
            bRes=DoLCLDBHDJ_Statistic(pFeatureClass, strMjFieldName); //�ֳ�����ͳ�Ʊ����� ygc 2012-10-10

            pProgress.SetProgress(8, "���й��Ҽ������ֵع滮���ͳ��");
            //���Ҽ������ֵع滮���
            bRes=DoGJGYLGHMJ_Statistic(pFeatureClass, strMjFieldName);
            bRes=DoLCGJGYLGHMJ_Statistic(pFeatureClass, strMjFieldName); //�ֳ�����ͳ�Ʊ����� ygc 2012-10-10
            pProgress.SetProgress(9,"ͳ�����");
        }
        //ɾ����ʱ��;
        //ygc 2012-8-21
        private static void DropTable(IWorkspace pWks, string strTableName)
        {
            try
            {
                pWks.ExecuteSQL("drop table " + strTableName);

            }
            catch
            { }
        }
        //�ֵ������ȼ�
        private static bool DoLDZL_Statistic(IFeatureClass pFeatureClass, string strMjFieldName)
        {
            if (pFeatureClass == null)
            {
                return false;
            }
            IWorkspace pWorkspace = null;
            string tableName = "";
            try
            {
                pWorkspace = pFeatureClass.FeatureDataset.Workspace;
                tableName = (pFeatureClass as IDataset).Name;
            }
            catch (Exception ex)
            { }
            DropTable(pWorkspace, "EightTable_LDZL");
            try
            {
                string townsSQL = "create table EightTable_LDZL as select  substr(" + tableName + ".xiang,1,8) as ͳ�Ƶ�λ,sum(case when Zl_DJ between '1' and '5' then round(" + tableName + "." + strMjFieldName + ",2) else 0 end) as �ϼ� ," +
                                  "sum(case when " + tableName + ".Zl_DJ='1' then round(" + tableName + "." + strMjFieldName + ",2) else 0  end) as һ��," +
                                  "sum(case when " + tableName + ".Zl_DJ='2' then round(" + tableName + "." + strMjFieldName + ",2) else 0  end) as ����," +
                                  "sum(case when " + tableName + ".Zl_DJ='3' then round(" + tableName + "." + strMjFieldName + ",2) else 0  end) as ����," +
                                  "sum(case when " + tableName + ".Zl_DJ='4' then round(" + tableName + "." + strMjFieldName + ",2) else 0  end) as �ļ�," +
                                  "sum(case when " + tableName + ".Zl_DJ='5' then round(" + tableName + "." + strMjFieldName + ",2) else 0  end) as �弶 " +
                                  "  from " + tableName +
                                  "  group by substr(xiang,1,8)";
                pWorkspace.ExecuteSQL(townsSQL);
                pWorkspace.ExecuteSQL("alter table EightTable_LDZL modify ͳ�Ƶ�λ nvarchar2(20)");
                //��ѯ�ؼ�ͳ������
                //ygc 2012-8-21
                string stringSQL = "insert into EightTable_LDZL select " + tableName + ".xian as ͳ�Ƶ�λ,sum(case when Zl_DJ between '1' and '5' then round(" + tableName + "." + strMjFieldName + ",2) else 0 end) as �ϼ� ," +
                                    "sum(case when " + tableName + ".Zl_DJ='1' then round(" + tableName + "." + strMjFieldName + ",2) else 0 end) as һ��," +
                                     "sum(case when " + tableName + ".Zl_DJ='2' then round(" + tableName + "." + strMjFieldName + ",2) else 0 end) as ����," +
                                     "sum(case when " + tableName + ".Zl_DJ='3' then round(" + tableName + "." + strMjFieldName + ",2) else 0 end) as ����," +
                                     "sum(case when " + tableName + ".Zl_DJ='4' then round(" + tableName + "." + strMjFieldName + ",2) else 0 end) as �ļ�," +
                                     "sum(case when " + tableName + ".Zl_DJ='5' then round(" + tableName + "." + strMjFieldName + ",2) else 0 end) as �弶 " +
                                      "  from " + tableName +
                                      "  group by xian";
                pWorkspace.ExecuteSQL(stringSQL);
                //��ѯ����ͳ������
                
                //��ѯ�м����� ygc 2012-10-22
                string SHISQL = "insert into EightTable_LDZL select  substr(" + tableName + ".shi,1,4) as ͳ�Ƶ�λ,sum(case when Zl_DJ between '1' and '5' then round(" + tableName + "." + strMjFieldName + ",2) else 0 end) as �ϼ� ," +
                  "sum(case when " + tableName + ".Zl_DJ='1' then round(" + tableName + "." + strMjFieldName + ",2) else 0  end) as һ��," +
                  "sum(case when " + tableName + ".Zl_DJ='2' then round(" + tableName + "." + strMjFieldName + ",2) else 0  end) as ����," +
                  "sum(case when " + tableName + ".Zl_DJ='3' then round(" + tableName + "." + strMjFieldName + ",2) else 0  end) as ����," +
                  "sum(case when " + tableName + ".Zl_DJ='4' then round(" + tableName + "." + strMjFieldName + ",2) else 0  end) as �ļ�," +
                  "sum(case when " + tableName + ".Zl_DJ='5' then round(" + tableName + "." + strMjFieldName + ",2) else 0  end) as �弶 " +
                  "  from " + tableName +
                  "  group by substr(shi,1,4)";
                pWorkspace.ExecuteSQL(SHISQL);
                //��ѯʡ������ ygc 2012-10-24
                string SHENGSQL = "insert into EightTable_LDZL select  substr(" + tableName + ".sheng,1,2) as ͳ�Ƶ�λ,sum(case when Zl_DJ between '1' and '5' then round(" + tableName + "." + strMjFieldName + ",2) else 0 end) as �ϼ� ," +
                  "sum(case when " + tableName + ".Zl_DJ='1' then round(" + tableName + "." + strMjFieldName + ",2) else 0  end) as һ��," +
                  "sum(case when " + tableName + ".Zl_DJ='2' then round(" + tableName + "." + strMjFieldName + ",2) else 0  end) as ����," +
                  "sum(case when " + tableName + ".Zl_DJ='3' then round(" + tableName + "." + strMjFieldName + ",2) else 0  end) as ����," +
                  "sum(case when " + tableName + ".Zl_DJ='4' then round(" + tableName + "." + strMjFieldName + ",2) else 0  end) as �ļ�," +
                  "sum(case when " + tableName + ".Zl_DJ='5' then round(" + tableName + "." + strMjFieldName + ",2) else 0  end) as �弶 " +
                  "  from " + tableName +
                  "  group by substr(sheng,1,2)";
                pWorkspace.ExecuteSQL(SHENGSQL);
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }
        private static bool DoLCLDZL_Statistic(IFeatureClass pFeatureClass, string strMjFieldName)
        {
            if (pFeatureClass == null)
            {
                return false;
            }
            IWorkspace pWorkspace = null;
            string tableName = "";
            try
            {
                pWorkspace = pFeatureClass.FeatureDataset.Workspace;
                tableName = (pFeatureClass as IDataset).Name;
            }
            catch (Exception ex)
            { }
            try
            {
                if (!ExistTable(pWorkspace, "EightTable_LDZL"))
                {
                    string stringSQL = "create table EightTable_LDZL as select " + tableName + ".lc as ͳ�Ƶ�λ,sum(case when Zl_DJ between '1' and '5' then round(" + tableName + "." + strMjFieldName + ",2) else 0 end) as �ϼ� ," +
                                       "sum(case when " + tableName + ".Zl_DJ='1' then round(" + tableName + "." + strMjFieldName + ",2) else 0 end) as һ��," +
                                        "sum(case when " + tableName + ".Zl_DJ='2' then round(" + tableName + "." + strMjFieldName + ",2) else 0  end) as ����," +
                                        "sum(case when " + tableName + ".Zl_DJ='3' then round(" + tableName + "." + strMjFieldName + ",2) else 0 end) as ����," +
                                        "sum(case when " + tableName + ".Zl_DJ='4' then round(" + tableName + "." + strMjFieldName + ",2) else 0 end) as �ļ�," +
                                        "sum(case when " + tableName + ".Zl_DJ='5' then round(" + tableName + "." + strMjFieldName + ",2) else 0 end) as �弶 " +
                                         "  from " + tableName +
                                         "  where lc <>' '" +
                                         "  group by lc";
                    pWorkspace.ExecuteSQL(stringSQL);
                    pWorkspace.ExecuteSQL("alter table EightTable_LDZL modify ͳ�Ƶ�λ nvarchar2(20)");
                }
                else
                {
                    //��ѯ�ֳ�ͳ������
                    string townsSQL = "insert into EightTable_LDZL select  " + tableName + ".lc as ͳ�Ƶ�λ,sum(case when Zl_DJ between '1' and '5' then round(" + tableName + "." + strMjFieldName + ",2) else 0 end) as �ϼ� ," +
                                      "sum(case when " + tableName + ".Zl_DJ='1' then round(" + tableName + "." + strMjFieldName + ",2) else 0  end) as һ��," +
                                      "sum(case when " + tableName + ".Zl_DJ='2' then round(" + tableName + "." + strMjFieldName + ",2) else 0  end) as ����," +
                                      "sum(case when " + tableName + ".Zl_DJ='3' then round(" + tableName + "." + strMjFieldName + ",2)  else 0 end) as ����," +
                                      "sum(case when " + tableName + ".Zl_DJ='4' then round(" + tableName + "." + strMjFieldName + ",2) else 0 end) as �ļ�," +
                                      "sum(case when " + tableName + ".Zl_DJ='5' then round(" + tableName + "." + strMjFieldName + ",2) else 0 end) as �弶 " +
                                      "  from " + tableName +
                                      "  where lc <> ' '" +
                                      "  group by lc";
                    pWorkspace.ExecuteSQL(townsSQL);
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }
        //�ֵ����÷���滮���
        private static bool DoLDLYFXGHMJ_Statistic(IFeatureClass pFeatureClass, string StatisticsFieldName)
        {
            if (pFeatureClass == null)
            {
                return false;
            }
            IWorkspace pWorkspace = null;
            string tableName = "";
            try
            {
                pWorkspace = pFeatureClass.FeatureDataset.Workspace;
                tableName = (pFeatureClass as IDataset).Name;
            }
            catch (Exception ex)
            { }
            DropTable(pWorkspace, "EightTable_LDLYFXGHMJ");
            try
            {
                #region ִ��ͳ��
                //��ѯ����ͳ������
                //ygc 2012-8-22
                string townsSQL = "create table EightTable_LDLYFXGHMJ as select substr(" +
                                 tableName + ".xiang,1,8) as ͳ�Ƶ�λ," +
                                 "sum (case when " + tableName + ".sen_lin_lb in('11','12','21','22') and " + tableName + ".dl <= '114'then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as �ϼ�," +
                                 "sum(case when " + tableName + ".sen_lin_lb between '11' and '12'and " + tableName + ".dl<='114' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as �����ֺϼ�," +
                                 "sum (case when " + tableName + ".sen_lin_lb ='11' and " + tableName + ".dl<='114' then round(" + tableName + "." + StatisticsFieldName + ",2)  else 0 end) as �ص㹫���ֺϼ�," +
                                 "sum(case when " + tableName + ".sen_lin_lb='11' and " + tableName + ".dl<='112' and (substr(" + tableName + ".lz,1,2)='11'or substr(" + tableName + ".lz,1,2)='12') then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end ) as �ص㹫��С��," +
                                 "sum (case when " + tableName + ".sen_lin_lb='11' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='11' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as �ص㹫�������," +
                                 "sum(case when " + tableName + ".sen_lin_lb='11'and " + tableName + ".dl<='112' and substr(" + tableName + ".lz,1,2)='12' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end ) as �ص㹫��������," +
                                 "sum(case when " + tableName + ".sen_lin_lb='11'and (((substr(" + tableName + ".lz,1,2)='11'or substr(" + tableName + ".lz,1,2)='12') and " + tableName + ".dl between '113' and '114')) and (substr(" + tableName + ".lz,1,2)<>'11' or substr(" + tableName + ".lz,1,2)<>'12') then round(" + tableName + "." + StatisticsFieldName + ",2)  else 0 end) as  �ص㹫������," +
                                 "sum(case when " + tableName + ".sen_lin_lb ='12' and " + tableName + ".dl<='114' then round(" + tableName + "." + StatisticsFieldName + ",2)   end ) as һ�㹫��ϼ�," +
                                 "sum(case when " + tableName + ".sen_lin_lb='12' and " + tableName + ".dl<='112' and (substr(" + tableName + ".lz,1,2)='11'or substr(" + tableName + ".lz,1,2)='12') then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end ) as һ�㹫��С��," +
                                 "sum(case when " + tableName + ".sen_lin_lb='12' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='11' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as һ�㹫�������," +
                                 "sum(case when " + tableName + ".sen_lin_lb='12'and " + tableName + ".dl<='112' and substr(" + tableName + ".lz,1,2)='12' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end ) as һ�㹫��������," +
                                 "sum(case when " + tableName + ".sen_lin_lb='12'and ((substr(" + tableName + ".lz,1,2)<>'11' and substr(" + tableName + ".lz,1,2)<>'12') or (((substr(" + tableName + ".lz,1,2)='11' or substr(" + tableName + ".lz,1,2)='12')) and " + tableName + ".dl between '113' and '114')) then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as һ�㹫������," +
                                 "sum(case when " + tableName + ".sen_lin_lb between '21' and '22'and " + tableName + ".dl<='114' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end ) as ��Ʒ�ֺϼ�," +
                                 "sum(case when " + tableName + ".sen_lin_lb ='21' and " + tableName + ".dl<='114' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as �ص���Ʒ�ֺϼ�," +
                                 "sum(case when " + tableName + ".sen_lin_lb='21' and " + tableName + ".dl<='114' and (substr(" + tableName + ".lz,1,2) between '23' and '25') then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end ) as �ص���Ʒ��С��," +
                                 "sum(case when " + tableName + ".sen_lin_lb='21' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='23' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as �ص��ò���," +
                                 "sum(case when " + tableName + ".sen_lin_lb='21' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='25'and " + tableName + ".dl='114' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as �ص㾭����," +
                                 "sum(case when " + tableName + ".sen_lin_lb='21' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='24' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as �ص�н̼��," +
                                 "sum(case when " + tableName + ".sen_lin_lb='21'and ((substr(" + tableName + ".lz,1,2)<>'23' and substr(" + tableName + ".lz,1,2)<>'24' and substr(" + tableName + ".lz,1,2)<>'25') or (substr(" + tableName + ".lz,1,2)='23' and " + tableName + ".dl>'112') or (substr(" + tableName + ".lz,1,2)='25' and " + tableName + ".dl<>'114')) then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as �ص�����," +
                                 "sum(case when " + tableName + ".sen_lin_lb ='22' and " + tableName + ".dl<='114' then round(" + tableName + "." + StatisticsFieldName + ",2)  else 0 end) as һ����Ʒ�ֺϼ�," +
                                 "sum(case when " + tableName + ".sen_lin_lb='22' and " + tableName + ".dl<='114' and (substr(" + tableName + ".lz,1,2) between '23' and '25') then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end ) as һ����Ʒ��С��," +
                                 "sum(case when " + tableName + ".sen_lin_lb='22' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='23' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as һ���ò���," +
                                 "sum(case when " + tableName + ".sen_lin_lb='22' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='25'and " + tableName + ".dl='114' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as һ�㾭����," +
                                 "sum(case when " + tableName + ".sen_lin_lb='22' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='24' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as һ��н̼��," +
                                 "sum(case when " + tableName + ".sen_lin_lb='22'and ((substr(" + tableName + ".lz,1,2)<>'23' and substr(" + tableName + ".lz,1,2)<>'24' and substr(" + tableName + ".lz,1,2)<>'25') or (substr(" + tableName + ".lz,1,2)='23' and " + tableName + ".dl>'112') or (substr(" + tableName + ".lz,1,2)='25' and " + tableName + ".dl<>'114')) then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as һ������" +
                                 " from " + tableName +
                                 " group by substr(xiang,1,8)";
                pWorkspace.ExecuteSQL(townsSQL);
                pWorkspace.ExecuteSQL("alter table EightTable_LDLYFXGHMJ modify ͳ�Ƶ�λ nvarchar2(20)");
                //��ѯ�ؼ�ͳ������SQL���
                //ygc 2012-8-22
                string CitySQL = "insert  into EightTable_LDLYFXGHMJ  select " +
                                 tableName + ".xian as ͳ�Ƶ�λ," +
                                 "sum (case when " + tableName + ".sen_lin_lb in('11','12','21','22') and " + tableName + ".dl <= '114'then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as �ϼ�," +
                                 "sum(case when " + tableName + ".sen_lin_lb between '11' and '12'and " + tableName + ".dl<='114' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as �����ֺϼ�," +
                                 "sum (case when " + tableName + ".sen_lin_lb ='11' and " + tableName + ".dl<='114' then round(" + tableName + "." + StatisticsFieldName + ",2)  else 0 end) as �ص㹫���ֺϼ�," +
                                 "sum(case when " + tableName + ".sen_lin_lb='11' and " + tableName + ".dl<='112' and (substr(" + tableName + ".lz,1,2)='11'or substr(" + tableName + ".lz,1,2)='12') then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end ) as �ص㹫��С��," +
                                 "sum (case when " + tableName + ".sen_lin_lb='11' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='11' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as �ص㹫�������," +
                                 "sum(case when " + tableName + ".sen_lin_lb='11'and " + tableName + ".dl<='112' and substr(" + tableName + ".lz,1,2)='12' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end ) as �ص㹫��������," +
                                 "sum(case when " + tableName + ".sen_lin_lb='11'and (((substr(" + tableName + ".lz,1,2)='11'or substr(" + tableName + ".lz,1,2)='12') and " + tableName + ".dl between '113' and '114')) and (substr(" + tableName + ".lz,1,2)<>'11' or substr(" + tableName + ".lz,1,2)<>'12') then round(" + tableName + "." + StatisticsFieldName + ",2)  else 0 end) as  �ص㹫������," +
                                 "sum(case when " + tableName + ".sen_lin_lb ='12' and " + tableName + ".dl<='114' then round(" + tableName + "." + StatisticsFieldName + ",2)   end ) as һ�㹫��ϼ�," +
                                 "sum(case when " + tableName + ".sen_lin_lb='12' and " + tableName + ".dl<='112' and (substr(" + tableName + ".lz,1,2)='11'or substr(" + tableName + ".lz,1,2)='12') then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end ) as һ�㹫��С��," +
                                 "sum(case when " + tableName + ".sen_lin_lb='12' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='11' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as һ�㹫�������," +
                                 "sum(case when " + tableName + ".sen_lin_lb='12'and " + tableName + ".dl<='112' and substr(" + tableName + ".lz,1,2)='12' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end ) as һ�㹫��������," +
                                 "sum(case when " + tableName + ".sen_lin_lb='12'and ((substr(" + tableName + ".lz,1,2)<>'11' and substr(" + tableName + ".lz,1,2)<>'12') or (((substr(" + tableName + ".lz,1,2)='11' or substr(" + tableName + ".lz,1,2)='12')) and " + tableName + ".dl between '113' and '114')) then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as һ�㹫������," +
                                 "sum(case when " + tableName + ".sen_lin_lb between '21' and '22'and " + tableName + ".dl<='114' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end ) as ��Ʒ�ֺϼ�," +
                                 "sum(case when " + tableName + ".sen_lin_lb ='21' and " + tableName + ".dl<='114' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as �ص���Ʒ�ֺϼ�," +
                                 "sum(case when " + tableName + ".sen_lin_lb='21' and " + tableName + ".dl<='114' and (substr(" + tableName + ".lz,1,2) between '23' and '25') then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end ) as �ص���Ʒ��С��," +
                                 "sum(case when " + tableName + ".sen_lin_lb='21' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='23' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as �ص��ò���," +
                                 "sum(case when " + tableName + ".sen_lin_lb='21' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='25'and " + tableName + ".dl='114' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as �ص㾭����," +
                                 "sum(case when " + tableName + ".sen_lin_lb='21' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='24' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as �ص�н̼��," +
                                 "sum(case when " + tableName + ".sen_lin_lb='21'and ((substr(" + tableName + ".lz,1,2)<>'23' and substr(" + tableName + ".lz,1,2)<>'24' and substr(" + tableName + ".lz,1,2)<>'25') or (substr(" + tableName + ".lz,1,2)='23' and " + tableName + ".dl>'112') or (substr(" + tableName + ".lz,1,2)='25' and " + tableName + ".dl<>'114')) then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as �ص�����," +
                                 "sum(case when " + tableName + ".sen_lin_lb ='22' and " + tableName + ".dl<='114' then round(" + tableName + "." + StatisticsFieldName + ",2)  else 0 end) as һ����Ʒ�ֺϼ�," +
                                 "sum(case when " + tableName + ".sen_lin_lb='22' and " + tableName + ".dl<='114' and (substr(" + tableName + ".lz,1,2) between '23' and '25') then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end ) as һ����Ʒ��С��," +
                                 "sum(case when " + tableName + ".sen_lin_lb='22' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='23' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as һ���ò���," +
                                 "sum(case when " + tableName + ".sen_lin_lb='22' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='25'and " + tableName + ".dl='114' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as һ�㾭����," +
                                 "sum(case when " + tableName + ".sen_lin_lb='22' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='24' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as һ��н̼��," +
                                 "sum(case when " + tableName + ".sen_lin_lb='22'and ((substr(" + tableName + ".lz,1,2)<>'23' and substr(" + tableName + ".lz,1,2)<>'24' and substr(" + tableName + ".lz,1,2)<>'25') or (substr(" + tableName + ".lz,1,2)='23' and " + tableName + ".dl>'112') or (substr(" + tableName + ".lz,1,2)='25' and " + tableName + ".dl<>'114')) then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as һ������" +
                                 " from " + tableName +
                                 " group by xian";
                pWorkspace.ExecuteSQL(CitySQL);
                
                //��ѯ�м����� ygc 2012-10-22
                string SHISQL = "insert  into EightTable_LDLYFXGHMJ  select substr(" +
                 tableName + ".shi,1,4) as ͳ�Ƶ�λ," +
                 "sum (case when " + tableName + ".sen_lin_lb in('11','12','21','22') and " + tableName + ".dl <= '114'then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as �ϼ�," +
                 "sum(case when " + tableName + ".sen_lin_lb between '11' and '12'and " + tableName + ".dl<='114' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as �����ֺϼ�," +
                 "sum (case when " + tableName + ".sen_lin_lb ='11' and " + tableName + ".dl<='114' then round(" + tableName + "." + StatisticsFieldName + ",2)  else 0 end) as �ص㹫���ֺϼ�," +
                 "sum(case when " + tableName + ".sen_lin_lb='11' and " + tableName + ".dl<='112' and (substr(" + tableName + ".lz,1,2)='11'or substr(" + tableName + ".lz,1,2)='12') then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end ) as �ص㹫��С��," +
                 "sum (case when " + tableName + ".sen_lin_lb='11' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='11' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as �ص㹫�������," +
                 "sum(case when " + tableName + ".sen_lin_lb='11'and " + tableName + ".dl<='112' and substr(" + tableName + ".lz,1,2)='12' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end ) as �ص㹫��������," +
                 "sum(case when " + tableName + ".sen_lin_lb='11'and (((substr(" + tableName + ".lz,1,2)='11'or substr(" + tableName + ".lz,1,2)='12') and " + tableName + ".dl between '113' and '114')) and (substr(" + tableName + ".lz,1,2)<>'11' or substr(" + tableName + ".lz,1,2)<>'12') then round(" + tableName + "." + StatisticsFieldName + ",2)  else 0 end) as  �ص㹫������," +
                 "sum(case when " + tableName + ".sen_lin_lb ='12' and " + tableName + ".dl<='114' then round(" + tableName + "." + StatisticsFieldName + ",2)   end ) as һ�㹫��ϼ�," +
                 "sum(case when " + tableName + ".sen_lin_lb='12' and " + tableName + ".dl<='112' and (substr(" + tableName + ".lz,1,2)='11'or substr(" + tableName + ".lz,1,2)='12') then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end ) as һ�㹫��С��," +
                 "sum(case when " + tableName + ".sen_lin_lb='12' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='11' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as һ�㹫�������," +
                 "sum(case when " + tableName + ".sen_lin_lb='12'and " + tableName + ".dl<='112' and substr(" + tableName + ".lz,1,2)='12' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end ) as һ�㹫��������," +
                 "sum(case when " + tableName + ".sen_lin_lb='12'and ((substr(" + tableName + ".lz,1,2)<>'11' and substr(" + tableName + ".lz,1,2)<>'12') or (((substr(" + tableName + ".lz,1,2)='11' or substr(" + tableName + ".lz,1,2)='12')) and " + tableName + ".dl between '113' and '114')) then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as һ�㹫������," +
                 "sum(case when " + tableName + ".sen_lin_lb between '21' and '22'and " + tableName + ".dl<='114' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end ) as ��Ʒ�ֺϼ�," +
                 "sum(case when " + tableName + ".sen_lin_lb ='21' and " + tableName + ".dl<='114' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as �ص���Ʒ�ֺϼ�," +
                 "sum(case when " + tableName + ".sen_lin_lb='21' and " + tableName + ".dl<='114' and (substr(" + tableName + ".lz,1,2) between '23' and '25') then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end ) as �ص���Ʒ��С��," +
                 "sum(case when " + tableName + ".sen_lin_lb='21' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='23' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as �ص��ò���," +
                 "sum(case when " + tableName + ".sen_lin_lb='21' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='25'and " + tableName + ".dl='114' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as �ص㾭����," +
                 "sum(case when " + tableName + ".sen_lin_lb='21' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='24' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as �ص�н̼��," +
                 "sum(case when " + tableName + ".sen_lin_lb='21'and ((substr(" + tableName + ".lz,1,2)<>'23' and substr(" + tableName + ".lz,1,2)<>'24' and substr(" + tableName + ".lz,1,2)<>'25') or (substr(" + tableName + ".lz,1,2)='23' and " + tableName + ".dl>'112') or (substr(" + tableName + ".lz,1,2)='25' and " + tableName + ".dl<>'114')) then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as �ص�����," +
                 "sum(case when " + tableName + ".sen_lin_lb ='22' and " + tableName + ".dl<='114' then round(" + tableName + "." + StatisticsFieldName + ",2)  else 0 end) as һ����Ʒ�ֺϼ�," +
                 "sum(case when " + tableName + ".sen_lin_lb='22' and " + tableName + ".dl<='114' and (substr(" + tableName + ".lz,1,2) between '23' and '25') then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end ) as һ����Ʒ��С��," +
                 "sum(case when " + tableName + ".sen_lin_lb='22' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='23' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as һ���ò���," +
                 "sum(case when " + tableName + ".sen_lin_lb='22' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='25'and " + tableName + ".dl='114' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as һ�㾭����," +
                 "sum(case when " + tableName + ".sen_lin_lb='22' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='24' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as һ��н̼��," +
                 "sum(case when " + tableName + ".sen_lin_lb='22'and ((substr(" + tableName + ".lz,1,2)<>'23' and substr(" + tableName + ".lz,1,2)<>'24' and substr(" + tableName + ".lz,1,2)<>'25') or (substr(" + tableName + ".lz,1,2)='23' and " + tableName + ".dl>'112') or (substr(" + tableName + ".lz,1,2)='25' and " + tableName + ".dl<>'114')) then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as һ������" +
                 " from " + tableName +
                 " group by substr(shi,1,4)";
                pWorkspace.ExecuteSQL(SHISQL);
                //��ѯʡ������  ygc 2012-10-24
                string SHENGSQL = "insert  into EightTable_LDLYFXGHMJ  select substr(" +
                        tableName + ".sheng,1,2) as ͳ�Ƶ�λ," +
                        "sum (case when " + tableName + ".sen_lin_lb in('11','12','21','22') and " + tableName + ".dl <= '114'then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as �ϼ�," +
                        "sum(case when " + tableName + ".sen_lin_lb between '11' and '12'and " + tableName + ".dl<='114' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as �����ֺϼ�," +
                        "sum (case when " + tableName + ".sen_lin_lb ='11' and " + tableName + ".dl<='114' then round(" + tableName + "." + StatisticsFieldName + ",2)  else 0 end) as �ص㹫���ֺϼ�," +
                        "sum(case when " + tableName + ".sen_lin_lb='11' and " + tableName + ".dl<='112' and (substr(" + tableName + ".lz,1,2)='11'or substr(" + tableName + ".lz,1,2)='12') then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end ) as �ص㹫��С��," +
                        "sum (case when " + tableName + ".sen_lin_lb='11' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='11' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as �ص㹫�������," +
                        "sum(case when " + tableName + ".sen_lin_lb='11'and " + tableName + ".dl<='112' and substr(" + tableName + ".lz,1,2)='12' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end ) as �ص㹫��������," +
                        "sum(case when " + tableName + ".sen_lin_lb='11'and (((substr(" + tableName + ".lz,1,2)='11'or substr(" + tableName + ".lz,1,2)='12') and " + tableName + ".dl between '113' and '114')) and (substr(" + tableName + ".lz,1,2)<>'11' or substr(" + tableName + ".lz,1,2)<>'12') then round(" + tableName + "." + StatisticsFieldName + ",2)  else 0 end) as  �ص㹫������," +
                        "sum(case when " + tableName + ".sen_lin_lb ='12' and " + tableName + ".dl<='114' then round(" + tableName + "." + StatisticsFieldName + ",2)   end ) as һ�㹫��ϼ�," +
                        "sum(case when " + tableName + ".sen_lin_lb='12' and " + tableName + ".dl<='112' and (substr(" + tableName + ".lz,1,2)='11'or substr(" + tableName + ".lz,1,2)='12') then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end ) as һ�㹫��С��," +
                        "sum(case when " + tableName + ".sen_lin_lb='12' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='11' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as һ�㹫�������," +
                        "sum(case when " + tableName + ".sen_lin_lb='12'and " + tableName + ".dl<='112' and substr(" + tableName + ".lz,1,2)='12' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end ) as һ�㹫��������," +
                        "sum(case when " + tableName + ".sen_lin_lb='12'and ((substr(" + tableName + ".lz,1,2)<>'11' and substr(" + tableName + ".lz,1,2)<>'12') or (((substr(" + tableName + ".lz,1,2)='11' or substr(" + tableName + ".lz,1,2)='12')) and " + tableName + ".dl between '113' and '114')) then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as һ�㹫������," +
                        "sum(case when " + tableName + ".sen_lin_lb between '21' and '22'and " + tableName + ".dl<='114' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end ) as ��Ʒ�ֺϼ�," +
                        "sum(case when " + tableName + ".sen_lin_lb ='21' and " + tableName + ".dl<='114' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as �ص���Ʒ�ֺϼ�," +
                        "sum(case when " + tableName + ".sen_lin_lb='21' and " + tableName + ".dl<='114' and (substr(" + tableName + ".lz,1,2) between '23' and '25') then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end ) as �ص���Ʒ��С��," +
                        "sum(case when " + tableName + ".sen_lin_lb='21' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='23' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as �ص��ò���," +
                        "sum(case when " + tableName + ".sen_lin_lb='21' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='25'and " + tableName + ".dl='114' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as �ص㾭����," +
                        "sum(case when " + tableName + ".sen_lin_lb='21' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='24' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as �ص�н̼��," +
                        "sum(case when " + tableName + ".sen_lin_lb='21'and ((substr(" + tableName + ".lz,1,2)<>'23' and substr(" + tableName + ".lz,1,2)<>'24' and substr(" + tableName + ".lz,1,2)<>'25') or (substr(" + tableName + ".lz,1,2)='23' and " + tableName + ".dl>'112') or (substr(" + tableName + ".lz,1,2)='25' and " + tableName + ".dl<>'114')) then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as �ص�����," +
                        "sum(case when " + tableName + ".sen_lin_lb ='22' and " + tableName + ".dl<='114' then round(" + tableName + "." + StatisticsFieldName + ",2)  else 0 end) as һ����Ʒ�ֺϼ�," +
                        "sum(case when " + tableName + ".sen_lin_lb='22' and " + tableName + ".dl<='114' and (substr(" + tableName + ".lz,1,2) between '23' and '25') then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end ) as һ����Ʒ��С��," +
                        "sum(case when " + tableName + ".sen_lin_lb='22' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='23' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as һ���ò���," +
                        "sum(case when " + tableName + ".sen_lin_lb='22' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='25'and " + tableName + ".dl='114' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as һ�㾭����," +
                        "sum(case when " + tableName + ".sen_lin_lb='22' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='24' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as һ��н̼��," +
                        "sum(case when " + tableName + ".sen_lin_lb='22'and ((substr(" + tableName + ".lz,1,2)<>'23' and substr(" + tableName + ".lz,1,2)<>'24' and substr(" + tableName + ".lz,1,2)<>'25') or (substr(" + tableName + ".lz,1,2)='23' and " + tableName + ".dl>'112') or (substr(" + tableName + ".lz,1,2)='25' and " + tableName + ".dl<>'114')) then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as һ������" +
                        " from " + tableName +
                        " group by substr(sheng,1,2)";
                pWorkspace.ExecuteSQL(SHENGSQL);
                #endregion
                #region ����ͳ��
                //���¹�����С��
                //ygc 2012-8-22
                string UpdateXJ = "update EightTable_LDLYFXGHMJ set �ص㹫��С��=�ص㹫�������+ �ص㹫��������";
                pWorkspace.ExecuteSQL(UpdateXJ);
                //�����ص㹫���ֺϼ� ygc 2012-8-22
                string UpdateZDFYHJ = "update EightTable_LDLYFXGHMJ set �ص㹫���ֺϼ�=�ص㹫��С��+�ص㹫������";
                pWorkspace.ExecuteSQL(UpdateZDFYHJ);
                //����һ�㹫��С�� ygc 2012-8-22
                string UpdateYBGYXJ = "update EightTable_LDLYFXGHMJ set һ�㹫��С��=һ�㹫�������+һ�㹫��������";
                pWorkspace.ExecuteSQL(UpdateYBGYXJ);
                //����һ�㹫��ϼ� ygc 2012-8-22
                string UpdateYBGYHJ = "update EightTable_LDLYFXGHMJ set һ�㹫��ϼ�=һ�㹫��С��+һ�㹫������";
                pWorkspace.ExecuteSQL(UpdateYBGYHJ);
                //���¹����ֺϼ� ygc 2012-8-22
                pWorkspace.ExecuteSQL("update EightTable_LDLYFXGHMJ set �����ֺϼ�=һ�㹫��ϼ� + �ص㹫���ֺϼ�");
                //�����ص���Ʒ��С�� ygc 2012-8-22
                pWorkspace.ExecuteSQL("update EightTable_LDLYFXGHMJ set �ص���Ʒ��С��=�ص��ò���+�ص㾭����+�ص�н̼��");
                //�����ص���Ʒ�ֺϼ� ygc 2012-8-22
                pWorkspace.ExecuteSQL("update EightTable_LDLYFXGHMJ set �ص���Ʒ�ֺϼ�=�ص���Ʒ��С��+�ص�����");
                //����һ����Ʒ��С�� ygc 2012-8-22
                pWorkspace.ExecuteSQL("update EightTable_LDLYFXGHMJ set һ����Ʒ��С��=һ���ò���+һ�㾭����+һ��н̼��");
                //����һ����Ʒ�ֺϼ� ygc 2012-8-22
                pWorkspace.ExecuteSQL("update EightTable_LDLYFXGHMJ set һ����Ʒ�ֺϼ�=һ����Ʒ��С��+һ������");
                //������Ʒ�ֺϼ� ygc 2012-8-22
                pWorkspace.ExecuteSQL("update EightTable_LDLYFXGHMJ set ��Ʒ�ֺϼ� =һ����Ʒ�ֺϼ�+�ص���Ʒ�ֺϼ�");
                //�����ܼ� ygc 2012-8-22
                pWorkspace.ExecuteSQL("update EightTable_LDLYFXGHMJ set �ϼ�=��Ʒ�ֺϼ�+�����ֺϼ�");
                #endregion
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }
        private static bool DoLCLDLYFXGHMJ_Statistic(IFeatureClass pFeatureClass, string StatisticsFieldName)
        {
            if (pFeatureClass == null)
            {
                return false;
            }
            IWorkspace pWorkspace = null;
            string tableName = "";
            try
            {
                pWorkspace = pFeatureClass.FeatureDataset.Workspace;
                tableName = (pFeatureClass as IDataset).Name;
            }
            catch (Exception ex)
            { }
            try
            {
                #region ִ��ͳ��
                if (!ExistTable(pWorkspace, "EightTable_LDLYFXGHMJ"))
                {
                    //��ѯ�ؼ�ͳ������SQL���
                    //ygc 2012-8-22
                    string CitySQL = "create table EightTable_LDLYFXGHMJ as select " +
                                     tableName + ".lc as ͳ�Ƶ�λ," +
                                     "sum (case when " + tableName + ".sen_lin_lb in('11','12','21','22') and " + tableName + ".dl <= '114'then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as �ϼ�," +
                                     "sum(case when " + tableName + ".sen_lin_lb between '11' and '12'and " + tableName + ".dl<='114' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as �����ֺϼ�," +
                                     "sum (case when " + tableName + ".sen_lin_lb ='11' and " + tableName + ".dl<='114' then round(" + tableName + "." + StatisticsFieldName + ",2)  else 0 end) as �ص㹫���ֺϼ�," +
                                     "sum(case when " + tableName + ".sen_lin_lb='11' and " + tableName + ".dl<='112' and (substr(" + tableName + ".lz,1,2)='11'or substr(" + tableName + ".lz,1,2)='12') then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end ) as �ص㹫��С��," +
                                     "sum (case when " + tableName + ".sen_lin_lb='11' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='11' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as �ص㹫�������," +
                                     "sum(case when " + tableName + ".sen_lin_lb='11'and " + tableName + ".dl<='112' and substr(" + tableName + ".lz,1,2)='12' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end ) as �ص㹫��������," +
                                     "sum(case when " + tableName + ".sen_lin_lb='11'and (((substr(" + tableName + ".lz,1,2)='11'or substr(" + tableName + ".lz,1,2)='12') and " + tableName + ".dl between '113' and '114')) and (substr(" + tableName + ".lz,1,2)<>'11' or substr(" + tableName + ".lz,1,2)<>'12') then round(" + tableName + "." + StatisticsFieldName + ",2)  else 0 end) as  �ص㹫������," +
                                     "sum(case when " + tableName + ".sen_lin_lb ='12' and " + tableName + ".dl<='114' then round(" + tableName + "." + StatisticsFieldName + ",2)   end ) as һ�㹫��ϼ�," +
                                     "sum(case when " + tableName + ".sen_lin_lb='12' and " + tableName + ".dl<='112' and (substr(" + tableName + ".lz,1,2)='11'or substr(" + tableName + ".lz,1,2)='12') then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end ) as һ�㹫��С��," +
                                     "sum(case when " + tableName + ".sen_lin_lb='12' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='11' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as һ�㹫�������," +
                                     "sum(case when " + tableName + ".sen_lin_lb='12'and " + tableName + ".dl<='112' and substr(" + tableName + ".lz,1,2)='12' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end ) as һ�㹫��������," +
                                     "sum(case when " + tableName + ".sen_lin_lb='12'and ((substr(" + tableName + ".lz,1,2)<>'11' and substr(" + tableName + ".lz,1,2)<>'12') or (((substr(" + tableName + ".lz,1,2)='11' or substr(" + tableName + ".lz,1,2)='12')) and " + tableName + ".dl between '113' and '114')) then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as һ�㹫������," +
                                     "sum(case when " + tableName + ".sen_lin_lb between '21' and '22'and " + tableName + ".dl<='114' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end ) as ��Ʒ�ֺϼ�," +
                                     "sum(case when " + tableName + ".sen_lin_lb ='21' and " + tableName + ".dl<='114' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as �ص���Ʒ�ֺϼ�," +
                                     "sum(case when " + tableName + ".sen_lin_lb='21' and " + tableName + ".dl<='114' and (substr(" + tableName + ".lz,1,2) between '23' and '25') then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end ) as �ص���Ʒ��С��," +
                                     "sum(case when " + tableName + ".sen_lin_lb='21' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='23' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as �ص��ò���," +
                                     "sum(case when " + tableName + ".sen_lin_lb='21' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='25'and " + tableName + ".dl='114' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as �ص㾭����," +
                                     "sum(case when " + tableName + ".sen_lin_lb='21' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='24' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as �ص�н̼��," +
                                     "sum(case when " + tableName + ".sen_lin_lb='21'and ((substr(" + tableName + ".lz,1,2)<>'23' and substr(" + tableName + ".lz,1,2)<>'24' and substr(" + tableName + ".lz,1,2)<>'25') or (substr(" + tableName + ".lz,1,2)='23' and " + tableName + ".dl>'112') or (substr(" + tableName + ".lz,1,2)='25' and " + tableName + ".dl<>'114')) then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as �ص�����," +
                                     "sum(case when " + tableName + ".sen_lin_lb ='22' and " + tableName + ".dl<='114' then round(" + tableName + "." + StatisticsFieldName + ",2)  else 0 end) as һ����Ʒ�ֺϼ�," +
                                     "sum(case when " + tableName + ".sen_lin_lb='22' and " + tableName + ".dl<='114' and (substr(" + tableName + ".lz,1,2) between '23' and '25') then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end ) as һ����Ʒ��С��," +
                                     "sum(case when " + tableName + ".sen_lin_lb='22' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='23' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as һ���ò���," +
                                     "sum(case when " + tableName + ".sen_lin_lb='22' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='25'and " + tableName + ".dl='114' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as һ�㾭����," +
                                     "sum(case when " + tableName + ".sen_lin_lb='22' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='24' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as һ��н̼��," +
                                     "sum(case when " + tableName + ".sen_lin_lb='22'and ((substr(" + tableName + ".lz,1,2)<>'23' and substr(" + tableName + ".lz,1,2)<>'24' and substr(" + tableName + ".lz,1,2)<>'25') or (substr(" + tableName + ".lz,1,2)='23' and " + tableName + ".dl>'112') or (substr(" + tableName + ".lz,1,2)='25' and " + tableName + ".dl<>'114')) then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as һ������" +
                                     " from " + tableName +
                                     " where lc <>' '" +
                                     " group by lc";
                    pWorkspace.ExecuteSQL(CitySQL);
                    pWorkspace.ExecuteSQL("alter table EightTable_LDLYFXGHMJ modify ͳ�Ƶ�λ nvarchar2(20)");
                }
                else
                {
                    //��ѯ����ͳ������
                    //ygc 2012-8-22
                    string townsSQL = "insert  into EightTable_LDLYFXGHMJ  select " +
                                     tableName + ".lc as ͳ�Ƶ�λ," +
                                     "sum (case when " + tableName + ".sen_lin_lb in('11','12','21','22') and " + tableName + ".dl <= '114'then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as �ϼ�," +
                                     "sum(case when " + tableName + ".sen_lin_lb between '11' and '12'and " + tableName + ".dl<='114' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as �����ֺϼ�," +
                                     "sum (case when " + tableName + ".sen_lin_lb ='11' and " + tableName + ".dl<='114' then round(" + tableName + "." + StatisticsFieldName + ",2)  else 0 end) as �ص㹫���ֺϼ�," +
                                     "sum(case when " + tableName + ".sen_lin_lb='11' and " + tableName + ".dl<='112' and (substr(" + tableName + ".lz,1,2)='11'or substr(" + tableName + ".lz,1,2)='12') then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end ) as �ص㹫��С��," +
                                     "sum (case when " + tableName + ".sen_lin_lb='11' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='11' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as �ص㹫�������," +
                                     "sum(case when " + tableName + ".sen_lin_lb='11'and " + tableName + ".dl<='112' and substr(" + tableName + ".lz,1,2)='12' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end ) as �ص㹫��������," +
                                     "sum(case when " + tableName + ".sen_lin_lb='11'and (((substr(" + tableName + ".lz,1,2)='11'or substr(" + tableName + ".lz,1,2)='12') and " + tableName + ".dl between '113' and '114')) and (substr(" + tableName + ".lz,1,2)<>'11' or substr(" + tableName + ".lz,1,2)<>'12') then round(" + tableName + "." + StatisticsFieldName + ",2)  else 0 end) as  �ص㹫������," +
                                     "sum(case when " + tableName + ".sen_lin_lb ='12' and " + tableName + ".dl<='114' then round(" + tableName + "." + StatisticsFieldName + ",2)   end ) as һ�㹫��ϼ�," +
                                     "sum(case when " + tableName + ".sen_lin_lb='12' and " + tableName + ".dl<='112' and (substr(" + tableName + ".lz,1,2)='11'or substr(" + tableName + ".lz,1,2)='12') then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end ) as һ�㹫��С��," +
                                     "sum(case when " + tableName + ".sen_lin_lb='12' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='11' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as һ�㹫�������," +
                                     "sum(case when " + tableName + ".sen_lin_lb='12'and " + tableName + ".dl<='112' and substr(" + tableName + ".lz,1,2)='12' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end ) as һ�㹫��������," +
                                     "sum(case when " + tableName + ".sen_lin_lb='12'and ((substr(" + tableName + ".lz,1,2)<>'11' and substr(" + tableName + ".lz,1,2)<>'12') or (((substr(" + tableName + ".lz,1,2)='11' or substr(" + tableName + ".lz,1,2)='12')) and " + tableName + ".dl between '113' and '114')) then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as һ�㹫������," +
                                     "sum(case when " + tableName + ".sen_lin_lb between '21' and '22'and " + tableName + ".dl<='114' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end ) as ��Ʒ�ֺϼ�," +
                                     "sum(case when " + tableName + ".sen_lin_lb ='21' and " + tableName + ".dl<='114' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as �ص���Ʒ�ֺϼ�," +
                                     "sum(case when " + tableName + ".sen_lin_lb='21' and " + tableName + ".dl<='114' and (substr(" + tableName + ".lz,1,2) between '23' and '25') then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end ) as �ص���Ʒ��С��," +
                                     "sum(case when " + tableName + ".sen_lin_lb='21' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='23' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as �ص��ò���," +
                                     "sum(case when " + tableName + ".sen_lin_lb='21' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='25'and " + tableName + ".dl='114' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as �ص㾭����," +
                                     "sum(case when " + tableName + ".sen_lin_lb='21' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='24' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as �ص�н̼��," +
                                     "sum(case when " + tableName + ".sen_lin_lb='21'and ((substr(" + tableName + ".lz,1,2)<>'23' and substr(" + tableName + ".lz,1,2)<>'24' and substr(" + tableName + ".lz,1,2)<>'25') or (substr(" + tableName + ".lz,1,2)='23' and " + tableName + ".dl>'112') or (substr(" + tableName + ".lz,1,2)='25' and " + tableName + ".dl<>'114')) then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as �ص�����," +
                                     "sum(case when " + tableName + ".sen_lin_lb ='22' and " + tableName + ".dl<='114' then round(" + tableName + "." + StatisticsFieldName + ",2)  else 0 end) as һ����Ʒ�ֺϼ�," +
                                     "sum(case when " + tableName + ".sen_lin_lb='22' and " + tableName + ".dl<='114' and (substr(" + tableName + ".lz,1,2) between '23' and '25') then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end ) as һ����Ʒ��С��," +
                                     "sum(case when " + tableName + ".sen_lin_lb='22' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='23' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as һ���ò���," +
                                     "sum(case when " + tableName + ".sen_lin_lb='22' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='25'and " + tableName + ".dl='114' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as һ�㾭����," +
                                     "sum(case when " + tableName + ".sen_lin_lb='22' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='24' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as һ��н̼��," +
                                     "sum(case when " + tableName + ".sen_lin_lb='22'and ((substr(" + tableName + ".lz,1,2)<>'23' and substr(" + tableName + ".lz,1,2)<>'24' and substr(" + tableName + ".lz,1,2)<>'25') or (substr(" + tableName + ".lz,1,2)='23' and " + tableName + ".dl>'112') or (substr(" + tableName + ".lz,1,2)='25' and " + tableName + ".dl<>'114')) then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as һ������" +
                                     " from " + tableName +
                                     "  where lc <> ' '" +
                                     " group by lc";
                    pWorkspace.ExecuteSQL(townsSQL);
                }
                #endregion
                #region ����ͳ��
                //���¹�����С��
                //ygc 2012-8-22
                string UpdateXJ = "update EightTable_LDLYFXGHMJ set �ص㹫��С��=�ص㹫�������+ �ص㹫��������";
                pWorkspace.ExecuteSQL(UpdateXJ);
                //�����ص㹫���ֺϼ� ygc 2012-8-22
                string UpdateZDFYHJ = "update EightTable_LDLYFXGHMJ set �ص㹫���ֺϼ�=�ص㹫��С��+�ص㹫������";
                pWorkspace.ExecuteSQL(UpdateZDFYHJ);
                //����һ�㹫��С�� ygc 2012-8-22
                string UpdateYBGYXJ = "update EightTable_LDLYFXGHMJ set һ�㹫��С��=һ�㹫�������+һ�㹫��������";
                pWorkspace.ExecuteSQL(UpdateYBGYXJ);
                //����һ�㹫��ϼ� ygc 2012-8-22
                string UpdateYBGYHJ = "update EightTable_LDLYFXGHMJ set һ�㹫��ϼ�=һ�㹫��С��+һ�㹫������";
                pWorkspace.ExecuteSQL(UpdateYBGYHJ);
                //���¹����ֺϼ� ygc 2012-8-22
                pWorkspace.ExecuteSQL("update EightTable_LDLYFXGHMJ set �����ֺϼ�=һ�㹫��ϼ� + �ص㹫���ֺϼ�");
                //�����ص���Ʒ��С�� ygc 2012-8-22
                pWorkspace.ExecuteSQL("update EightTable_LDLYFXGHMJ set �ص���Ʒ��С��=�ص��ò���+�ص㾭����+�ص�н̼��");
                //�����ص���Ʒ�ֺϼ� ygc 2012-8-22
                pWorkspace.ExecuteSQL("update EightTable_LDLYFXGHMJ set �ص���Ʒ�ֺϼ�=�ص���Ʒ��С��+�ص�����");
                //����һ����Ʒ��С�� ygc 2012-8-22
                pWorkspace.ExecuteSQL("update EightTable_LDLYFXGHMJ set һ����Ʒ��С��=һ���ò���+һ�㾭����+һ��н̼��");
                //����һ����Ʒ�ֺϼ� ygc 2012-8-22
                pWorkspace.ExecuteSQL("update EightTable_LDLYFXGHMJ set һ����Ʒ�ֺϼ�=һ����Ʒ��С��+һ������");
                //������Ʒ�ֺϼ� ygc 2012-8-22
                pWorkspace.ExecuteSQL("update EightTable_LDLYFXGHMJ set ��Ʒ�ֺϼ� =һ����Ʒ�ֺϼ�+�ص���Ʒ�ֺϼ�");
                //�����ܼ� ygc 2012-8-22
                pWorkspace.ExecuteSQL("update EightTable_LDLYFXGHMJ set �ϼ�=��Ʒ�ֺϼ�+�����ֺϼ�");
                #endregion
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }
        //�ֵ���״
        //ͨ��SQL������ֵ���״ͳ������ ygc 2012-8-24
        private static bool DoLDXZ_Statistic(IFeatureClass pFeatureClass, string StatisticsFieldName)
        {
            if (pFeatureClass == null)
            {
                return false;
            }
            IWorkspace pWorkspace = null;
            string tableName = "";
            try
            {
                pWorkspace = pFeatureClass.FeatureDataset.Workspace;
                tableName = (pFeatureClass as IDataset).Name;
            }
            catch (Exception ex)
            { }

            DropTable(pWorkspace, "tempTable");
            DropTable(pWorkspace, "EightTable_LDXZ");
            //������ʱ��
            try
            {
                pWorkspace.ExecuteSQL("create table tempTable as select sheng,shi,xian,xiang,LD_QS,DL,QI_YUAN,SUM(" + StatisticsFieldName + ") AS mj from " + tableName + " group by sheng,shi,xian,xiang,LD_QS,DL,QI_YUAN");
                //������ʱ���е�Ȩ����Ϣ
                pWorkspace.ExecuteSQL("update tempTable set LD_QS='����' where LD_QS in('10','15')");
                pWorkspace.ExecuteSQL("update tempTable set LD_QS='����' where LD_QS in('21','22','23','16')");
                pWorkspace.ExecuteSQL("update tempTable set LD_QS='����' where LD_QS in('20')");

                //���û����ۣ��ն���
                //����ʱ����Ϊ�ƹ���������򣬽������д���
                //��������򣬷��ֵر�����Դ��������Դ���޷���ͳ�ƽ��������Ӧ���У�����һ����������Ȼ���˹����У��ֽ����Ϊ��Ȼ�С�
                //�ֵ���150��180ԭ��������ͬ
                pWorkspace.ExecuteSQL("update temptable set qi_yuan='19' where dl between '161' and '180'");
                pWorkspace.ExecuteSQL("update temptable set qi_yuan='29' where dl = '150'");
                pWorkspace.ExecuteSQL("update temptable set qi_yuan='19' where qi_yuan=' '");
                //ygc 2013-3-12

                //ͳ����������
                string townsSQL = "create table EightTable_LDXZ as select " +
                                 "substr(xiang,1,8) as ͳ�Ƶ�λ,LD_QS as Ȩ��,substr(QI_YUAN,1,1) as ��Դ," +
                                 "sum(case when DL <>' ' then round(mj,2) else 0 end) as ���������," +
                                 "sum(case when DL<=180 then round(mj,2) else 0 end) as �ֵغϼ�," +
                                 "sum(case when DL between '111' and '114' then round(mj,2) else 0 end) as ���ֵ�С��," +
                                 "sum(case when DL='111' OR DL='112' OR DL='114' then round(mj,2) else 0 end) as ��ľ��," +
                                 "sum(case when DL='113' then round(mj,2) else 0 end) as ����," +
                                 "sum(case when DL='114' then round(mj,2) else 0 end) as ������," +
                                 "sum(case when DL='120' then round(mj,2) else 0 end) as ���ֵ�," +
                                 "sum(case when DL between '131' and '133' then round(mj,2) else 0 end) as ��ľ�ֵ�С��," +
                                 "sum(case when DL='131' then round(mj,2) else 0 end) as �ع�," +
                                 "sum(case when DL='133' or DL='132' then round(mj,2) else 0 end ) as ������ľ��," +
                                 "sum(case when DL='141' or DL='142' then round(mj,2) else 0 end) as δ�������ֵ�," +
                                 "sum(case when DL='150' then round(mj,2) else 0 end) as ���Ե�," +
                                 "sum(case when DL='161' or DL='162' or DL='163' then round(mj,2) else 0 end) as ����ľ�ֵ�," +
                                 "sum(case when DL='171' or DL='172' or DL='173' or DL='174' then round(mj,2) else 0 end) as ���ֵ�," +
                                 "sum(case when DL='180' then  round(mj,2) else 0 end ) as �ָ���," +
                                 "sum(case when dl like '2%' then round(mj,2) else 0 end) as ���ֵ�," +
                                 "sum (case when dl=' ' then round(mj,2) else 0 end) as ɭ�ָ�����," +
                                 "sum(case when dl=' ' then round(mj,2) else 0 end) as ��ľ�̻���" +
                                 "  from tempTable" +
                                 "  where LD_QS is not null and LD_QS<>' ' and substr(QI_YUAN,1,1)<>' '" +
                                 "  group by substr(xiang,1,8),rollup(LD_QS),rollup(substr(QI_YUAN,1,1))" +
                                 "  order by LD_QS desc,substr(QI_YUAN,1,1) desc";
                pWorkspace.ExecuteSQL(townsSQL);
                pWorkspace.ExecuteSQL("alter table EightTable_LDXZ modify ͳ�Ƶ�λ nvarchar2(20)");
                //ͳ���ؼ�����
                string CitySQL = "insert into EightTable_LDXZ  select " +
                                 "xian as ͳ�Ƶ�λ,LD_QS as Ȩ��,substr(QI_YUAN,1,1) as ��Դ," +
                                 "sum(case when DL <>' ' then round(mj,2) else 0 end) as ���������," +
                                 "sum(case when DL<=180 then round(mj,2) else 0 end) as �ֵغϼ�," +
                                 "sum(case when DL between '111' and '114' then round(mj,2) else 0 end) as ���ֵ�С��," +
                                 "sum(case when DL='111' OR DL='112' OR DL='114' then round(mj,2) else 0 end) as ��ľ��," +
                                 "sum(case when DL='113' then round(mj,2) else 0 end) as ����," +
                                 "sum(case when DL='114' then round(mj,2) else 0 end) as ������," +
                                 "sum(case when DL='120' then round(mj,2) else 0 end) as ���ֵ�," +
                                 "sum(case when DL between '131' and '133' then round(mj,2) else 0 end) as ��ľ�ֵ�С��," +
                                 "sum(case when DL='131' then round(mj,2) else 0 end) as �ع�," +
                                 "sum(case when DL='133' or DL='132' then round(mj,2) else 0 end ) as ������ľ��," +
                                 "sum(case when DL='141' or DL='142' then round(mj,2) else 0 end) as δ�������ֵ�," +
                                 "sum(case when DL='150' then round(mj,2) else 0 end) as ���Ե�," +
                                 "sum(case when DL='161' or DL='162' or DL='163' then round(mj,2) else 0 end) as ����ľ�ֵ�," +
                                 "sum(case when DL='171' or DL='172' or DL='173' or DL='174' then round(mj,2) else 0 end) as ���ֵ�," +
                                 "sum(case when DL='180' then  round(mj,2) else 0 end ) as �ָ���," +
                                 "sum(case when dl like '2%' then round(mj,2) else 0 end) as ���ֵ�," +
                                 "sum (case when dl=' ' then round(mj,2) else 0 end) as ɭ�ָ�����," +
                                 "sum(case when dl=' ' then round(mj,2) else 0 end) as ��ľ�̻���" +
                                 "  from tempTable" +
                                 "  where LD_QS is not null and LD_QS<>' ' and substr(QI_YUAN,1,1)<>' '" +
                                 "  group by xian,rollup(LD_QS),rollup(substr(QI_YUAN,1,1))" +
                                 "  order by LD_QS desc,substr(QI_YUAN,1,1) desc";
                pWorkspace.ExecuteSQL(CitySQL);
                
                //����м����� ygc 2012-10-22
                string SHISQL = "insert into EightTable_LDXZ  select " +
                 "substr(shi,1,4) as ͳ�Ƶ�λ,LD_QS as Ȩ��,substr(QI_YUAN,1,1) as ��Դ," +
                 "sum(case when DL <>' ' then round(mj,2) else 0 end) as ���������," +
                 "sum(case when DL<=180 then round(mj,2) else 0 end) as �ֵغϼ�," +
                 "sum(case when DL between '111' and '114' then round(mj,2) else 0 end) as ���ֵ�С��," +
                 "sum(case when DL='111' OR DL='112' OR DL='114' then round(mj,2) else 0 end) as ��ľ��," +
                 "sum(case when DL='113' then round(mj,2) else 0 end) as ����," +
                 "sum(case when DL='114' then round(mj,2) else 0 end) as ������," +
                 "sum(case when DL='120' then round(mj,2) else 0 end) as ���ֵ�," +
                 "sum(case when DL between '131' and '133' then round(mj,2) else 0 end) as ��ľ�ֵ�С��," +
                 "sum(case when DL='131' then round(mj,2) else 0 end) as �ع�," +
                 "sum(case when DL='133' or DL='132' then round(mj,2) else 0 end ) as ������ľ��," +
                 "sum(case when DL='141' or DL='142' then round(mj,2) else 0 end) as δ�������ֵ�," +
                 "sum(case when DL='150' then round(mj,2) else 0 end) as ���Ե�," +
                 "sum(case when DL='161' or DL='162' or DL='163' then round(mj,2) else 0 end) as ����ľ�ֵ�," +
                 "sum(case when DL='171' or DL='172' or DL='173' or DL='174' then round(mj,2) else 0 end) as ���ֵ�," +
                 "sum(case when DL='180' then  round(mj,2) else 0 end ) as �ָ���," +
                 "sum(case when dl like '2%' then round(mj,2) else 0 end) as ���ֵ�," +
                 "sum (case when dl=' ' then round(mj,2) else 0 end) as ɭ�ָ�����," +
                 "sum(case when dl=' ' then round(mj,2) else 0 end) as ��ľ�̻���" +
                 "  from tempTable" +
                 "  where LD_QS is not null and LD_QS<>' ' and substr(QI_YUAN,1,1)<>' '" +
                 "  group by substr(shi,1,4),rollup(LD_QS),rollup(substr(QI_YUAN,1,1))" +
                 "  order by LD_QS desc,substr(QI_YUAN,1,1) desc";
                pWorkspace.ExecuteSQL(SHISQL);
                //��ѯʡ������ ygc 2012-10-24
                string SHENGSQL = "insert into EightTable_LDXZ  select " +
                 "substr(sheng,1,2) as ͳ�Ƶ�λ,LD_QS as Ȩ��,substr(QI_YUAN,1,1) as ��Դ," +
                 "sum(case when DL <>' ' then round(mj,2) else 0 end) as ���������," +
                 "sum(case when DL<=180 then round(mj,2) else 0 end) as �ֵغϼ�," +
                 "sum(case when DL between '111' and '114' then round(mj,2) else 0 end) as ���ֵ�С��," +
                 "sum(case when DL='111' OR DL='112' OR DL='114' then round(mj,2) else 0 end) as ��ľ��," +
                 "sum(case when DL='113' then round(mj,2) else 0 end) as ����," +
                 "sum(case when DL='114' then round(mj,2) else 0 end) as ������," +
                 "sum(case when DL='120' then round(mj,2) else 0 end) as ���ֵ�," +
                 "sum(case when DL between '131' and '133' then round(mj,2) else 0 end) as ��ľ�ֵ�С��," +
                 "sum(case when DL='131' then round(mj,2) else 0 end) as �ع�," +
                 "sum(case when DL='133' or DL='132' then round(mj,2) else 0 end ) as ������ľ��," +
                 "sum(case when DL='141' or DL='142' then round(mj,2) else 0 end) as δ�������ֵ�," +
                 "sum(case when DL='150' then round(mj,2) else 0 end) as ���Ե�," +
                 "sum(case when DL='161' or DL='162' or DL='163' then round(mj,2) else 0 end) as ����ľ�ֵ�," +
                 "sum(case when DL='171' or DL='172' or DL='173' or DL='174' then round(mj,2) else 0 end) as ���ֵ�," +
                 "sum(case when DL='180' then  round(mj,2) else 0 end ) as �ָ���," +
                 "sum(case when dl like '2%' then round(mj,2) else 0 end) as ���ֵ�," +
                 "sum (case when dl=' ' then round(mj,2) else 0 end) as ɭ�ָ�����," +
                 "sum(case when dl=' ' then round(mj,2) else 0 end) as ��ľ�̻���" +
                 "  from tempTable" +
                 "  where LD_QS is not null and LD_QS<>' ' and substr(QI_YUAN,1,1)<>' '" +
                 "  group by substr(sheng,1,2),rollup(LD_QS),rollup(substr(QI_YUAN,1,1))" +
                 "  order by LD_QS desc,substr(QI_YUAN,1,1) desc";
                pWorkspace.ExecuteSQL(SHENGSQL);
                //����ɭ�ָ�����
                pWorkspace.ExecuteSQL("update EightTable_LDXZ set ɭ�ָ�����=round((���ֵ�С��+�ع�)/��������� * 100,2) where Ȩ�� is null");
                //������ľ�̻���
                pWorkspace.ExecuteSQL("update EightTable_LDXZ set ��ľ�̻���=round((���ֵ�С��+��ľ�ֵ�С��)/��������� * 100,2) where Ȩ�� is null ");
            }
            catch (Exception ex)
            {
                return false;
            }
            return true ;
        }
        private static bool DoLCLDXZ_Statistic(IFeatureClass pFeatureClass, string StatisticsFieldName)
        {
            if (pFeatureClass == null)
            {
                return false;
            }
            IWorkspace pWorkspace = null;
            string tableName = "";
            try
            {
                pWorkspace = pFeatureClass.FeatureDataset.Workspace;
                tableName = (pFeatureClass as IDataset).Name;
            }
            catch (Exception ex)
            { }

            DropTable(pWorkspace, "tempTable");
            //������ʱ��
            try
            {
                pWorkspace.ExecuteSQL("create table tempTable as select LC,LD_QS,DL,QI_YUAN,SUM(" + StatisticsFieldName + ") AS mj from " + tableName + " group by LC,LD_QS,DL,QI_YUAN");
                //������ʱ���е�Ȩ����Ϣ
                pWorkspace.ExecuteSQL("update tempTable set LD_QS='����' where LD_QS in('10','15')");
                pWorkspace.ExecuteSQL("update tempTable set LD_QS='����' where LD_QS in('21','22','23','16')");
                pWorkspace.ExecuteSQL("update tempTable set LD_QS='����' where LD_QS in('20')");
                if (!ExistTable(pWorkspace, "EightTable_LDXZ"))
                {
                    //ͳ���ؼ�����
                    string CitySQL = "create table EightTable_LDXZ as select " +
                                     "lc as ͳ�Ƶ�λ,LD_QS as Ȩ��,substr(QI_YUAN,1,1) as ��Դ," +
                                     "sum(case when DL <>' ' then round(mj,2) else 0 end) as ���������," +
                                     "sum(case when DL<=180 then round(mj,2) else 0 end) as �ֵغϼ�," +
                                     "sum(case when DL between '111' and '114' then round(mj,2) else 0 end) as ���ֵ�С��," +
                                     "sum(case when DL='111' OR DL='112' OR DL='114' then round(mj,2) else 0 end) as ��ľ��," +
                                     "sum(case when DL='113' then round(mj,2) else 0 end) as ����," +
                                     "sum(case when DL='114' then round(mj,2) else 0 end) as ������," +
                                     "sum(case when DL='120' then round(mj,2) else 0 end) as ���ֵ�," +
                                     "sum(case when DL between '131' and '133' then round(mj,2) else 0 end) as ��ľ�ֵ�С��," +
                                     "sum(case when DL='131' then round(mj,2) else 0 end) as �ع�," +
                                     "sum(case when DL='133' or DL='132' then round(mj,2) else 0 end ) as ������ľ��," +
                                     "sum(case when DL='141' or DL='142' then round(mj,2) else 0 end) as δ�������ֵ�," +
                                     "sum(case when DL='150' then round(mj,2) else 0 end) as ���Ե�," +
                                     "sum(case when DL='161' or DL='162' or DL='163' then round(mj,2) else 0 end) as ����ľ�ֵ�," +
                                     "sum(case when DL='171' or DL='172' or DL='173' or DL='174' then round(mj,2) else 0 end) as ���ֵ�," +
                                     "sum(case when DL='180' then  round(mj,2) else 0 end ) as �ָ���," +
                                     "sum(case when dl like '2%' then round(mj,2) else 0 end) as ���ֵ�," +
                                     "sum (case when dl=' ' then round(mj,2) else 0 end) as ɭ�ָ�����," +
                                     "sum(case when dl=' ' then round(mj,2) else 0 end) as ��ľ�̻���" +
                                     "  from tempTable" +
                                     "  where LD_QS is not null and LD_QS<>' ' and substr(QI_YUAN,1,1)<>' ' and lc<>' '" +
                                     "  group by lc,rollup(LD_QS),rollup(substr(QI_YUAN,1,1))" +
                                     "  order by LD_QS desc,substr(QI_YUAN,1,1) desc";
                    pWorkspace.ExecuteSQL(CitySQL);
                    pWorkspace.ExecuteSQL("alter table EightTable_LDXZ modify ͳ�Ƶ�λ nvarchar2(20)");
                }
                else
                {
                    //ͳ����������
                    string townsSQL = "insert into EightTable_LDXZ  select " +
                                     "lc as ͳ�Ƶ�λ,LD_QS as Ȩ��,substr(QI_YUAN,1,1) as ��Դ," +
                                     "sum(case when DL <>' ' then round(mj,2) else 0 end) as ���������," +
                                     "sum(case when DL<=180 then round(mj,2) else 0 end) as �ֵغϼ�," +
                                     "sum(case when DL between '111' and '114' then round(mj,2) else 0 end) as ���ֵ�С��," +
                                     "sum(case when DL='111' OR DL='112' OR DL='114' then round(mj,2) else 0 end) as ��ľ��," +
                                     "sum(case when DL='113' then round(mj,2) else 0 end) as ����," +
                                     "sum(case when DL='114' then round(mj,2) else 0 end) as ������," +
                                     "sum(case when DL='120' then round(mj,2) else 0 end) as ���ֵ�," +
                                     "sum(case when DL between '131' and '133' then round(mj,2) else 0 end) as ��ľ�ֵ�С��," +
                                     "sum(case when DL='131' then round(mj,2) else 0 end) as �ع�," +
                                     "sum(case when DL='133' or DL='132' then round(mj,2) else 0 end ) as ������ľ��," +
                                     "sum(case when DL='141' or DL='142' then round(mj,2) else 0 end) as δ�������ֵ�," +
                                     "sum(case when DL='150' then round(mj,2) else 0 end) as ���Ե�," +
                                     "sum(case when DL='161' or DL='162' or DL='163' then round(mj,2) else 0 end) as ����ľ�ֵ�," +
                                     "sum(case when DL='171' or DL='172' or DL='173' or DL='174' then round(mj,2) else 0 end) as ���ֵ�," +
                                     "sum(case when DL='180' then  round(mj,2) else 0 end ) as �ָ���," +
                                     "sum(case when dl like '2%' then round(mj,2) else 0 end) as ���ֵ�," +
                                     "sum (case when dl=' ' then round(mj,2) else 0 end) as ɭ�ָ�����," +
                                     "sum(case when dl=' ' then round(mj,2) else 0 end) as ��ľ�̻���" +
                                     "  from tempTable" +
                                     "  where LD_QS is not null and LD_QS<>' ' and substr(QI_YUAN,1,1)<>' ' and lc<>' '" +
                                     "  group by lc,rollup(LD_QS),rollup(substr(QI_YUAN,1,1))" +
                                     "  order by LD_QS desc,substr(QI_YUAN,1,1) desc";
                    pWorkspace.ExecuteSQL(townsSQL);
                }
                //����ɭ�ָ�����
                pWorkspace.ExecuteSQL("update EightTable_LDXZ set ɭ�ָ�����=round((���ֵ�С��+�ع�)/��������� * 100,2) where Ȩ�� is null");
                //������ľ�̻���
                pWorkspace.ExecuteSQL("update EightTable_LDXZ set ��ľ�̻���=round((���ֵ�С��+��ľ�ֵ�С��)/��������� * 100,2) where Ȩ�� is null ");
                pWorkspace.ExecuteSQL("alter table EightTable_LDXZ modify ��Դ nvarchar2(10)");
                UpdateStatistictable(pWorkspace, "EightTable_LDXZ", "��Դ", "��Ȼ", "1");
                UpdateStatistictable(pWorkspace, "EightTable_LDXZ", "��Դ", "�˹�", "2");
            
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }
        //�ֵؼ�ɭ������滮
        //ͨ��SQL������ֵؼ�ɭ������滮ͳ����
        //ygc 2012-8-23
        private static bool DoLDSLMJGH_Statistic(IFeatureClass pFeatureClass, string StatisticsFieldName)
        {
            if (pFeatureClass == null)
            {
                return false;
            }
            IWorkspace pWorkspace = null;
            string tableName = "";
            try
            {
                pWorkspace = pFeatureClass.FeatureDataset.Workspace;
                tableName = (pFeatureClass as IDataset).Name;
            }
            catch (Exception ex)
            { }

            DropTable(pWorkspace, "EightTable_LDSLMJGH");
            try
            {

                //��ѯ����ͳ������SQL ygc 2012-8-23
                string townsSQL = "create table EightTable_LDSLMJGH as select  " +
                                 "substr(xiang,1,8) as ͳ�Ƶ�λ," +
                                 "sum(case when DL LIKE '1%' then round(" + StatisticsFieldName + ",2) else 0 end) as ��״�ֵ�," +
                                 "sum(case when (DL LIKE '11%' OR DL='131') then round(" + StatisticsFieldName + ",2) else 0 end) as ��״ɭ��," +
                                 "sum(case when substr(dl,1,2) between '21'and '25' and GHDL LIKE '1%' then round(" + StatisticsFieldName + ",2) else 0 end) as ����ϼ�," +
                                 "sum(case when DL LIKE '24%' AND GHDL LIKE '1%' then round(" + StatisticsFieldName + ",2) else 0 end) as ����δ���õ�," +
                                 "sum(case when DL LIKE '25%' AND GHDL LIKE '1%' then round(" + StatisticsFieldName + ",2) else 0 end) as ���佨���õ�," +
                                 "sum(case when DL LIKE '22%' AND GHDL LIKE '1%' then round(" + StatisticsFieldName + ",2) else 0 end) as �������ݵ�," +
                                 "sum(case when DL LIKE '21%' AND GHDL LIKE '1%' then round(" + StatisticsFieldName + ",2) else 0 end) as �������," +
                                 "sum(case when DL LIKE '23%' AND GHDL LIKE '1%' then round(" + StatisticsFieldName + ",2) else 0 end) as ��������," +
                                 "sum(case when substr(dl,1,2) between '21'and '25'and (GHDL LIKE '11%' OR GHDL ='131') then round(" + StatisticsFieldName + ",2) else 0 end) as ɭ�����Ӻϼ�," +
                                 "sum(case when DL LIKE '22%' AND (GHDL LIKE '11%' OR GHDL ='131') then round(" + StatisticsFieldName + ",2) else 0 end) as ɭ�������ֵ�," +
                                 "sum(case when DL LIKE '24%' AND (GHDL LIKE '11%' OR GHDL ='131') then round(" + StatisticsFieldName + ",2) else 0 end) as ɭ������δ���õ�," +
                                 "sum(case when DL LIKE '25%' AND (GHDL LIKE '11%' OR GHDL ='131') then round(" + StatisticsFieldName + ",2) else 0 end) as ɭ�����ӽ����õ�," +
                                 "sum(case when DL LIKE '22%' AND (GHDL LIKE '11%' OR GHDL ='131') then round(" + StatisticsFieldName + ",2) else 0 end) as ɭ�����Ӳݵ�," +
                                 "sum(case when DL LIKE '21%' AND (GHDL LIKE '11%' OR GHDL ='131') then round(" + StatisticsFieldName + ",2) else 0 end) as ɭ�����Ӹ���," +
                                 "sum(case when DL LIKE '23%' AND (GHDL LIKE '11%' OR GHDL ='131') then round(" + StatisticsFieldName + ",2) else 0 end) as ɭ����������" +
                                 "  from " + tableName +
                                 "  group by substr(xiang,1,8)";
                pWorkspace.ExecuteSQL(townsSQL);
                pWorkspace.ExecuteSQL("alter table EightTable_LDSLMJGH modify ͳ�Ƶ�λ nvarchar2(20)");
                //��ѯ�ؼ�����SQL��� ygc 2012-8-23
                string CitySQL = "insert into EightTable_LDSLMJGH  select " +
                                 "xian as ͳ�Ƶ�λ," +
                                 "sum(case when DL LIKE '1%' then round(" + StatisticsFieldName + ",2) else 0 end) as ��״�ֵ�," +
                                 "sum(case when (DL LIKE '11%' OR DL='131') then round(" + StatisticsFieldName + ",2) else 0 end) as ��״ɭ��," +
                                 "sum(case when substr(dl,1,2) between '21'and '25' and GHDL LIKE '1%' then round(" + StatisticsFieldName + ",2) else 0 end) as ����ϼ�," +
                                 "sum(case when DL LIKE '24%' AND GHDL LIKE '1%' then round(" + StatisticsFieldName + ",2) else 0 end) as ����δ���õ�," +
                                 "sum(case when DL LIKE '25%' AND GHDL LIKE '1%' then round(" + StatisticsFieldName + ",2) else 0 end) as ���佨���õ�," +
                                 "sum(case when DL LIKE '22%' AND GHDL LIKE '1%' then round(" + StatisticsFieldName + ",2) else 0 end) as �������ݵ�," +
                                 "sum(case when DL LIKE '21%' AND GHDL LIKE '1%' then round(" + StatisticsFieldName + ",2) else 0 end) as �������," +
                                 "sum(case when DL LIKE '23%' AND GHDL LIKE '1%' then round(" + StatisticsFieldName + ",2) else 0 end) as ��������," +
                                 "sum(case when substr(dl,1,2) between '21'and '25'and (GHDL LIKE '11%' OR GHDL ='131') then round(" + StatisticsFieldName + ",2) else 0 end) as ɭ�����Ӻϼ�," +
                                 "sum(case when DL LIKE '22%' AND (GHDL LIKE '11%' OR GHDL ='131') then round(" + StatisticsFieldName + ",2) else 0 end) as ɭ�������ֵ�," +
                                 "sum(case when DL LIKE '24%' AND (GHDL LIKE '11%' OR GHDL ='131') then round(" + StatisticsFieldName + ",2) else 0 end) as ɭ������δ���õ�," +
                                 "sum(case when DL LIKE '25%' AND (GHDL LIKE '11%' OR GHDL ='131') then round(" + StatisticsFieldName + ",2) else 0 end) as ɭ�����ӽ����õ�," +
                                 "sum(case when DL LIKE '22%' AND (GHDL LIKE '11%' OR GHDL ='131') then round(" + StatisticsFieldName + ",2) else 0 end) as ɭ�����Ӳݵ�," +
                                 "sum(case when DL LIKE '21%' AND (GHDL LIKE '11%' OR GHDL ='131') then round(" + StatisticsFieldName + ",2) else 0 end) as ɭ�����Ӹ���," +
                                 "sum(case when DL LIKE '23%' AND (GHDL LIKE '11%' OR GHDL ='131') then round(" + StatisticsFieldName + ",2) else 0 end) as ɭ����������" +
                                 "  from " + tableName +
                                 "  group by xian";
                pWorkspace.ExecuteSQL(CitySQL);
                
                //����м����� ygc 2012-10-22
                string SHISQL = " insert into EightTable_LDSLMJGH  select " +
                 "substr(shi,1,4) as ͳ�Ƶ�λ," +
                 "sum(case when DL LIKE '1%' then round(" + StatisticsFieldName + ",2) else 0 end) as ��״�ֵ�," +
                 "sum(case when (DL LIKE '11%' OR DL='131') then round(" + StatisticsFieldName + ",2) else 0 end) as ��״ɭ��," +
                 "sum(case when substr(dl,1,2) between '21'and '25' and GHDL LIKE '1%' then round(" + StatisticsFieldName + ",2) else 0 end) as ����ϼ�," +
                 "sum(case when DL LIKE '24%' AND GHDL LIKE '1%' then round(" + StatisticsFieldName + ",2) else 0 end) as ����δ���õ�," +
                 "sum(case when DL LIKE '25%' AND GHDL LIKE '1%' then round(" + StatisticsFieldName + ",2) else 0 end) as ���佨���õ�," +
                 "sum(case when DL LIKE '22%' AND GHDL LIKE '1%' then round(" + StatisticsFieldName + ",2) else 0 end) as �������ݵ�," +
                 "sum(case when DL LIKE '21%' AND GHDL LIKE '1%' then round(" + StatisticsFieldName + ",2) else 0 end) as �������," +
                 "sum(case when DL LIKE '23%' AND GHDL LIKE '1%' then round(" + StatisticsFieldName + ",2) else 0 end) as ��������," +
                 "sum(case when substr(dl,1,2) between '21'and '25'and (GHDL LIKE '11%' OR GHDL ='131') then round(" + StatisticsFieldName + ",2) else 0 end) as ɭ�����Ӻϼ�," +
                 "sum(case when DL LIKE '22%' AND (GHDL LIKE '11%' OR GHDL ='131') then round(" + StatisticsFieldName + ",2) else 0 end) as ɭ�������ֵ�," +
                 "sum(case when DL LIKE '24%' AND (GHDL LIKE '11%' OR GHDL ='131') then round(" + StatisticsFieldName + ",2) else 0 end) as ɭ������δ���õ�," +
                 "sum(case when DL LIKE '25%' AND (GHDL LIKE '11%' OR GHDL ='131') then round(" + StatisticsFieldName + ",2) else 0 end) as ɭ�����ӽ����õ�," +
                 "sum(case when DL LIKE '22%' AND (GHDL LIKE '11%' OR GHDL ='131') then round(" + StatisticsFieldName + ",2) else 0 end) as ɭ�����Ӳݵ�," +
                 "sum(case when DL LIKE '21%' AND (GHDL LIKE '11%' OR GHDL ='131') then round(" + StatisticsFieldName + ",2) else 0 end) as ɭ�����Ӹ���," +
                 "sum(case when DL LIKE '23%' AND (GHDL LIKE '11%' OR GHDL ='131') then round(" + StatisticsFieldName + ",2) else 0 end) as ɭ����������" +
                 "  from " + tableName +
                 "  group by substr(shi,1,4)";
                pWorkspace.ExecuteSQL(SHISQL);
                //��ѯʡ������ ygc 2012-10-24
                string SHENGSQL = " insert into EightTable_LDSLMJGH  select " +
                 "substr(sheng,1,2) as ͳ�Ƶ�λ," +
                 "sum(case when DL LIKE '1%' then round(" + StatisticsFieldName + ",2) else 0 end) as ��״�ֵ�," +
                 "sum(case when (DL LIKE '11%' OR DL='131') then round(" + StatisticsFieldName + ",2) else 0 end) as ��״ɭ��," +
                 "sum(case when substr(dl,1,2) between '21'and '25' and GHDL LIKE '1%' then round(" + StatisticsFieldName + ",2) else 0 end) as ����ϼ�," +
                 "sum(case when DL LIKE '24%' AND GHDL LIKE '1%' then round(" + StatisticsFieldName + ",2) else 0 end) as ����δ���õ�," +
                 "sum(case when DL LIKE '25%' AND GHDL LIKE '1%' then round(" + StatisticsFieldName + ",2) else 0 end) as ���佨���õ�," +
                 "sum(case when DL LIKE '22%' AND GHDL LIKE '1%' then round(" + StatisticsFieldName + ",2) else 0 end) as �������ݵ�," +
                 "sum(case when DL LIKE '21%' AND GHDL LIKE '1%' then round(" + StatisticsFieldName + ",2) else 0 end) as �������," +
                 "sum(case when DL LIKE '23%' AND GHDL LIKE '1%' then round(" + StatisticsFieldName + ",2) else 0 end) as ��������," +
                 "sum(case when substr(dl,1,2) between '21'and '25'and (GHDL LIKE '11%' OR GHDL ='131') then round(" + StatisticsFieldName + ",2) else 0 end) as ɭ�����Ӻϼ�," +
                 "sum(case when DL LIKE '22%' AND (GHDL LIKE '11%' OR GHDL ='131') then round(" + StatisticsFieldName + ",2) else 0 end) as ɭ�������ֵ�," +
                 "sum(case when DL LIKE '24%' AND (GHDL LIKE '11%' OR GHDL ='131') then round(" + StatisticsFieldName + ",2) else 0 end) as ɭ������δ���õ�," +
                 "sum(case when DL LIKE '25%' AND (GHDL LIKE '11%' OR GHDL ='131') then round(" + StatisticsFieldName + ",2) else 0 end) as ɭ�����ӽ����õ�," +
                 "sum(case when DL LIKE '22%' AND (GHDL LIKE '11%' OR GHDL ='131') then round(" + StatisticsFieldName + ",2) else 0 end) as ɭ�����Ӳݵ�," +
                 "sum(case when DL LIKE '21%' AND (GHDL LIKE '11%' OR GHDL ='131') then round(" + StatisticsFieldName + ",2) else 0 end) as ɭ�����Ӹ���," +
                 "sum(case when DL LIKE '23%' AND (GHDL LIKE '11%' OR GHDL ='131') then round(" + StatisticsFieldName + ",2) else 0 end) as ɭ����������" +
                 "  from " + tableName +
                 "  group by substr(sheng,1,2)";
                pWorkspace.ExecuteSQL(SHENGSQL);
                //����ɭ�����Ӻϼ� ygc 2012-8-23
                pWorkspace.ExecuteSQL("update EightTable_LDSLMJGH set ɭ�����Ӻϼ�= ɭ�������ֵ�+ɭ������δ���õ�+ɭ�����ӽ����õ�+ɭ�����Ӳݵ�+ɭ�����Ӹ���+ɭ����������");
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;

        }
        private static bool DoLCLDSLMJGH_Statistic(IFeatureClass pFeatureClass, string StatisticsFieldName)
        {
            if (pFeatureClass == null)
            {
                return false;
            }
            IWorkspace pWorkspace = null;
            string tableName = "";
            try
            {
                pWorkspace = pFeatureClass.FeatureDataset.Workspace;
                tableName = (pFeatureClass as IDataset).Name;
            }
            catch (Exception ex)
            { }
            try
            {
                if (!ExistTable(pWorkspace, "EightTable_LDSLMJGH"))
                {
                    //��ѯ�ؼ�����SQL��� ygc 2012-8-23
                    string CitySQL = "create table EightTable_LDSLMJGH as select " +
                                     "lc as ͳ�Ƶ�λ," +
                                     "sum(case when DL LIKE '1%' then round(" + StatisticsFieldName + ",2) else 0 end) as ��״�ֵ�," +
                                     "sum(case when (DL LIKE '11%' OR DL='131') then round(" + StatisticsFieldName + ",2) else 0 end) as ��״ɭ��," +
                                     "sum(case when substr(dl,1,2) between '21'and '25' and GHDL LIKE '1%' then round(" + StatisticsFieldName + ",2) else 0 end) as ����ϼ�," +
                                     "sum(case when DL LIKE '24%' AND GHDL LIKE '1%' then round(" + StatisticsFieldName + ",2) else 0 end) as ����δ���õ�," +
                                     "sum(case when DL LIKE '25%' AND GHDL LIKE '1%' then round(" + StatisticsFieldName + ",2) else 0 end) as ���佨���õ�," +
                                     "sum(case when DL LIKE '22%' AND GHDL LIKE '1%' then round(" + StatisticsFieldName + ",2) else 0 end) as �������ݵ�," +
                                     "sum(case when DL LIKE '21%' AND GHDL LIKE '1%' then round(" + StatisticsFieldName + ",2) else 0 end) as �������," +
                                     "sum(case when DL LIKE '23%' AND GHDL LIKE '1%' then round(" + StatisticsFieldName + ",2) else 0 end) as ��������," +
                                     "sum(case when substr(dl,1,2) between '21'and '25'and (GHDL LIKE '11%' OR GHDL ='131') then round(" + StatisticsFieldName + ",2) else 0 end) as ɭ�����Ӻϼ�," +
                                     "sum(case when DL LIKE '22%' AND (GHDL LIKE '11%' OR GHDL ='131') then round(" + StatisticsFieldName + ",2) else 0 end) as ɭ�������ֵ�," +
                                     "sum(case when DL LIKE '24%' AND (GHDL LIKE '11%' OR GHDL ='131') then round(" + StatisticsFieldName + ",2) else 0 end) as ɭ������δ���õ�," +
                                     "sum(case when DL LIKE '25%' AND (GHDL LIKE '11%' OR GHDL ='131') then round(" + StatisticsFieldName + ",2) else 0 end) as ɭ�����ӽ����õ�," +
                                     "sum(case when DL LIKE '22%' AND (GHDL LIKE '11%' OR GHDL ='131') then round(" + StatisticsFieldName + ",2) else 0 end) as ɭ�����Ӳݵ�," +
                                     "sum(case when DL LIKE '21%' AND (GHDL LIKE '11%' OR GHDL ='131') then round(" + StatisticsFieldName + ",2) else 0 end) as ɭ�����Ӹ���," +
                                     "sum(case when DL LIKE '23%' AND (GHDL LIKE '11%' OR GHDL ='131') then round(" + StatisticsFieldName + ",2) else 0 end) as ɭ����������" +
                                     "  from " + tableName +
                                     "  where lc <>' '" +
                                     "  group by lc";
                    pWorkspace.ExecuteSQL(CitySQL);
                    pWorkspace.ExecuteSQL("alter table EightTable_LDSLMJGH modify ͳ�Ƶ�λ nvarchar2(20)");
                }
                else
                {
                    //��ѯ����ͳ������SQL ygc 2012-8-23
                    string townsSQL = " insert into EightTable_LDSLMJGH  select " +
                                     "lc as ͳ�Ƶ�λ," +
                                     "sum(case when DL LIKE '1%' then round(" + StatisticsFieldName + ",2) else 0 end) as ��״�ֵ�," +
                                     "sum(case when (DL LIKE '11%' OR DL='131') then round(" + StatisticsFieldName + ",2) else 0 end) as ��״ɭ��," +
                                     "sum(case when substr(dl,1,2) between '21'and '25' and GHDL LIKE '1%' then round(" + StatisticsFieldName + ",2) else 0 end) as ����ϼ�," +
                                     "sum(case when DL LIKE '24%' AND GHDL LIKE '1%' then round(" + StatisticsFieldName + ",2) else 0 end) as ����δ���õ�," +
                                     "sum(case when DL LIKE '25%' AND GHDL LIKE '1%' then round(" + StatisticsFieldName + ",2) else 0 end) as ���佨���õ�," +
                                     "sum(case when DL LIKE '22%' AND GHDL LIKE '1%' then round(" + StatisticsFieldName + ",2) else 0 end) as �������ݵ�," +
                                     "sum(case when DL LIKE '21%' AND GHDL LIKE '1%' then round(" + StatisticsFieldName + ",2) else 0 end) as �������," +
                                     "sum(case when DL LIKE '23%' AND GHDL LIKE '1%' then round(" + StatisticsFieldName + ",2) else 0 end) as ��������," +
                                     "sum(case when substr(dl,1,2) between '21'and '25'and (GHDL LIKE '11%' OR GHDL ='131') then round(" + StatisticsFieldName + ",2) else 0 end) as ɭ�����Ӻϼ�," +
                                     "sum(case when DL LIKE '22%' AND (GHDL LIKE '11%' OR GHDL ='131') then round(" + StatisticsFieldName + ",2) else 0 end) as ɭ�������ֵ�," +
                                     "sum(case when DL LIKE '24%' AND (GHDL LIKE '11%' OR GHDL ='131') then round(" + StatisticsFieldName + ",2) else 0 end) as ɭ������δ���õ�," +
                                     "sum(case when DL LIKE '25%' AND (GHDL LIKE '11%' OR GHDL ='131') then round(" + StatisticsFieldName + ",2) else 0 end) as ɭ�����ӽ����õ�," +
                                     "sum(case when DL LIKE '22%' AND (GHDL LIKE '11%' OR GHDL ='131') then round(" + StatisticsFieldName + ",2) else 0 end) as ɭ�����Ӳݵ�," +
                                     "sum(case when DL LIKE '21%' AND (GHDL LIKE '11%' OR GHDL ='131') then round(" + StatisticsFieldName + ",2) else 0 end) as ɭ�����Ӹ���," +
                                     "sum(case when DL LIKE '23%' AND (GHDL LIKE '11%' OR GHDL ='131') then round(" + StatisticsFieldName + ",2) else 0 end) as ɭ����������" +
                                     "  from " + tableName +
                                     "  where lc<> ' '" +
                                     "  group by lc";
                    pWorkspace.ExecuteSQL(townsSQL);
                }
                //����ɭ�����Ӻϼ� ygc 2012-8-23
                pWorkspace.ExecuteSQL("update EightTable_LDSLMJGH set ɭ�����Ӻϼ�= ɭ�������ֵ�+ɭ������δ���õ�+ɭ�����ӽ����õ�+ɭ�����Ӳݵ�+ɭ�����Ӹ���+ɭ����������");
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;

        }
        //�ֵؽṹ��״
        private static bool DoLDJGXZ_Statistic(IFeatureClass pFeatureClass, string StatisticsFieldName)
        {
            if (pFeatureClass == null)
            {
                return false;
            }
            IWorkspace pWorkspace = null;
            string tableName = "";
            try
            {
                pWorkspace = pFeatureClass.FeatureDataset.Workspace;
                tableName = (pFeatureClass as IDataset).Name;
            }
            catch (Exception ex)
            { }
            DropTable(pWorkspace, "EightTable_LDJGXZ");
            try
            {
                #region ִ��ͳ��
                //��ѯ����ͳ������
                //ygc 2012-8-22
                string townsSQL = "create table EightTable_LDJGXZ as select substr(" +
                                 tableName + ".xiang,1,8) as ͳ�Ƶ�λ,substr(QI_YUAN,1,1) as ��Դ," +
                                 "sum(case when " + tableName + ".sen_lin_lb between '11' and '12'and " + tableName + ".dl<='114' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as �����ֺϼ�," +
                                 "sum (case when " + tableName + ".sen_lin_lb ='11' and " + tableName + ".dl<='114' then round(" + tableName + "." + StatisticsFieldName + ",2)  else 0 end) as �ص㹫���ֺϼ�," +
                                 "sum(case when " + tableName + ".sen_lin_lb='11' and " + tableName + ".dl<='112' and (substr(" + tableName + ".lz,1,2)='11'or substr(" + tableName + ".lz,1,2)='12') then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end ) as �ص㹫��С��," +
                                 "sum (case when " + tableName + ".sen_lin_lb='11' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='11' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as �ص㹫�������," +
                                 "sum(case when " + tableName + ".sen_lin_lb='11'and " + tableName + ".dl<='112' and substr(" + tableName + ".lz,1,2)='12' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end ) as �ص㹫��������," +
                                 "sum(case when " + tableName + ".sen_lin_lb='11'and (((substr(" + tableName + ".lz,1,2)='11'or substr(" + tableName + ".lz,1,2)='12') and " + tableName + ".dl between '113' and '114')) and (substr(" + tableName + ".lz,1,2)<>'11' or substr(" + tableName + ".lz,1,2)<>'12') then round(" + tableName + "." + StatisticsFieldName + ",2)  else 0 end) as  �ص㹫������," +
                                 "sum(case when " + tableName + ".sen_lin_lb ='12' and " + tableName + ".dl<='114' then round(" + tableName + "." + StatisticsFieldName + ",2)   end ) as һ�㹫��ϼ�," +
                                 "sum(case when " + tableName + ".sen_lin_lb='12' and " + tableName + ".dl<='112' and (substr(" + tableName + ".lz,1,2)='11'or substr(" + tableName + ".lz,1,2)='12') then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end ) as һ�㹫��С��," +
                                 "sum(case when " + tableName + ".sen_lin_lb='12' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='11' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as һ�㹫�������," +
                                 "sum(case when " + tableName + ".sen_lin_lb='12'and " + tableName + ".dl<='112' and substr(" + tableName + ".lz,1,2)='12' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end ) as һ�㹫��������," +
                                 "sum(case when " + tableName + ".sen_lin_lb='12'and ((substr(" + tableName + ".lz,1,2)<>'11' and substr(" + tableName + ".lz,1,2)<>'12') or (((substr(" + tableName + ".lz,1,2)='11' or substr(" + tableName + ".lz,1,2)='12')) and " + tableName + ".dl between '113' and '114')) then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as һ�㹫������," +
                                 "sum(case when " + tableName + ".sen_lin_lb between '21' and '22'and " + tableName + ".dl<='114' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end ) as ��Ʒ�ֺϼ�," +
                                 "sum(case when " + tableName + ".sen_lin_lb ='21' and " + tableName + ".dl<='114' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as �ص���Ʒ�ֺϼ�," +
                                 "sum(case when " + tableName + ".sen_lin_lb='21' and " + tableName + ".dl<='114' and (substr(" + tableName + ".lz,1,2) between '23' and '25') then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end ) as �ص���Ʒ��С��," +
                                 "sum(case when " + tableName + ".sen_lin_lb='21' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='23' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as �ص��ò���," +
                                 "sum(case when " + tableName + ".sen_lin_lb='21' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='25'and " + tableName + ".dl='114' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as �ص㾭����," +
                                 "sum(case when " + tableName + ".sen_lin_lb='21' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='24' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as �ص�н̼��," +
                                 "sum(case when " + tableName + ".sen_lin_lb='21'and ((substr(" + tableName + ".lz,1,2)<>'23' and substr(" + tableName + ".lz,1,2)<>'24' and substr(" + tableName + ".lz,1,2)<>'25') or (substr(" + tableName + ".lz,1,2)='23' and " + tableName + ".dl>'112') or (substr(" + tableName + ".lz,1,2)='25' and " + tableName + ".dl<>'114')) then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as �ص�����," +
                                 "sum(case when " + tableName + ".sen_lin_lb ='22' and " + tableName + ".dl<='114' then round(" + tableName + "." + StatisticsFieldName + ",2)  else 0 end) as һ����Ʒ�ֺϼ�," +
                                 "sum(case when " + tableName + ".sen_lin_lb='22' and " + tableName + ".dl<='114' and (substr(" + tableName + ".lz,1,2) between '23' and '25') then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end ) as һ����Ʒ��С��," +
                                 "sum(case when " + tableName + ".sen_lin_lb='22' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='23' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as һ���ò���," +
                                 "sum(case when " + tableName + ".sen_lin_lb='22' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='25'and " + tableName + ".dl='114' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as һ�㾭����," +
                                 "sum(case when " + tableName + ".sen_lin_lb='22' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='24' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as һ��н̼��," +
                                 "sum(case when " + tableName + ".sen_lin_lb='22'and ((substr(" + tableName + ".lz,1,2)<>'23' and substr(" + tableName + ".lz,1,2)<>'24' and substr(" + tableName + ".lz,1,2)<>'25') or (substr(" + tableName + ".lz,1,2)='23' and " + tableName + ".dl>'112') or (substr(" + tableName + ".lz,1,2)='25' and " + tableName + ".dl<>'114')) then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as һ������" +
                                 " from " + tableName +
                                 " where  substr(QI_YUAN,1,1)<>' '" +
                                 " group by substr(xiang,1,8),rollup(substr(QI_YUAN,1,1)) " +
                                 " order by substr(QI_YUAN,1,1)";
                pWorkspace.ExecuteSQL(townsSQL);
                pWorkspace.ExecuteSQL("alter table EightTable_LDJGXZ modify ͳ�Ƶ�λ nvarchar2(20)");

                //��ѯ�ؼ�ͳ������SQL���
                //ygc 2012-8-22
                string CitySQL = "insert  into EightTable_LDJGXZ  select " +
                                 tableName + ".xian as ͳ�Ƶ�λ,substr(QI_YUAN,1,1) as ��Դ," +
                                 "sum(case when " + tableName + ".sen_lin_lb between '11' and '12'and " + tableName + ".dl<='114' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as �����ֺϼ�," +
                                 "sum (case when " + tableName + ".sen_lin_lb ='11' and " + tableName + ".dl<='114' then round(" + tableName + "." + StatisticsFieldName + ",2)  else 0 end) as �ص㹫���ֺϼ�," +
                                 "sum(case when " + tableName + ".sen_lin_lb='11' and " + tableName + ".dl<='112' and (substr(" + tableName + ".lz,1,2)='11'or substr(" + tableName + ".lz,1,2)='12') then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end ) as �ص㹫��С��," +
                                 "sum (case when " + tableName + ".sen_lin_lb='11' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='11' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as �ص㹫�������," +
                                 "sum(case when " + tableName + ".sen_lin_lb='11'and " + tableName + ".dl<='112' and substr(" + tableName + ".lz,1,2)='12' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end ) as �ص㹫��������," +
                                 "sum(case when " + tableName + ".sen_lin_lb='11'and (((substr(" + tableName + ".lz,1,2)='11'or substr(" + tableName + ".lz,1,2)='12') and " + tableName + ".dl between '113' and '114')) and (substr(" + tableName + ".lz,1,2)<>'11' or substr(" + tableName + ".lz,1,2)<>'12') then round(" + tableName + "." + StatisticsFieldName + ",2)  else 0 end) as  �ص㹫������," +
                                 "sum(case when " + tableName + ".sen_lin_lb ='12' and " + tableName + ".dl<='114' then round(" + tableName + "." + StatisticsFieldName + ",2)   end ) as һ�㹫��ϼ�," +
                                 "sum(case when " + tableName + ".sen_lin_lb='12' and " + tableName + ".dl<='112' and (substr(" + tableName + ".lz,1,2)='11'or substr(" + tableName + ".lz,1,2)='12') then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end ) as һ�㹫��С��," +
                                 "sum(case when " + tableName + ".sen_lin_lb='12' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='11' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as һ�㹫�������," +
                                 "sum(case when " + tableName + ".sen_lin_lb='12'and " + tableName + ".dl<='112' and substr(" + tableName + ".lz,1,2)='12' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end ) as һ�㹫��������," +
                                 "sum(case when " + tableName + ".sen_lin_lb='12'and ((substr(" + tableName + ".lz,1,2)<>'11' and substr(" + tableName + ".lz,1,2)<>'12') or (((substr(" + tableName + ".lz,1,2)='11' or substr(" + tableName + ".lz,1,2)='12')) and " + tableName + ".dl between '113' and '114')) then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as һ�㹫������," +
                                 "sum(case when " + tableName + ".sen_lin_lb between '21' and '22'and " + tableName + ".dl<='114' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end ) as ��Ʒ�ֺϼ�," +
                                 "sum(case when " + tableName + ".sen_lin_lb ='21' and " + tableName + ".dl<='114' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as �ص���Ʒ�ֺϼ�," +
                                 "sum(case when " + tableName + ".sen_lin_lb='21' and " + tableName + ".dl<='114' and (substr(" + tableName + ".lz,1,2) between '23' and '25') then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end ) as �ص���Ʒ��С��," +
                                 "sum(case when " + tableName + ".sen_lin_lb='21' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='23' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as �ص��ò���," +
                                 "sum(case when " + tableName + ".sen_lin_lb='21' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='25'and " + tableName + ".dl='114' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as �ص㾭����," +
                                 "sum(case when " + tableName + ".sen_lin_lb='21' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='24' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as �ص�н̼��," +
                                 "sum(case when " + tableName + ".sen_lin_lb='21'and ((substr(" + tableName + ".lz,1,2)<>'23' and substr(" + tableName + ".lz,1,2)<>'24' and substr(" + tableName + ".lz,1,2)<>'25') or (substr(" + tableName + ".lz,1,2)='23' and " + tableName + ".dl>'112') or (substr(" + tableName + ".lz,1,2)='25' and " + tableName + ".dl<>'114')) then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as �ص�����," +
                                 "sum(case when " + tableName + ".sen_lin_lb ='22' and " + tableName + ".dl<='114' then round(" + tableName + "." + StatisticsFieldName + ",2)  else 0 end) as һ����Ʒ�ֺϼ�," +
                                 "sum(case when " + tableName + ".sen_lin_lb='22' and " + tableName + ".dl<='114' and (substr(" + tableName + ".lz,1,2) between '23' and '25') then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end ) as һ����Ʒ��С��," +
                                 "sum(case when " + tableName + ".sen_lin_lb='22' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='23' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as һ���ò���," +
                                 "sum(case when " + tableName + ".sen_lin_lb='22' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='25'and " + tableName + ".dl='114' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as һ�㾭����," +
                                 "sum(case when " + tableName + ".sen_lin_lb='22' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='24' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as һ��н̼��," +
                                 "sum(case when " + tableName + ".sen_lin_lb='22'and ((substr(" + tableName + ".lz,1,2)<>'23' and substr(" + tableName + ".lz,1,2)<>'24' and substr(" + tableName + ".lz,1,2)<>'25') or (substr(" + tableName + ".lz,1,2)='23' and " + tableName + ".dl>'112') or (substr(" + tableName + ".lz,1,2)='25' and " + tableName + ".dl<>'114')) then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as һ������" +
                                 " from " + tableName +
                                 " where substr(QI_YUAN,1,1)<>' '" +
                                 " group by xian,rollup(substr(QI_YUAN,1,1)) " +
                                 " order by substr(QI_YUAN,1,1) ";
                pWorkspace.ExecuteSQL(CitySQL);
                
                //��ѯ�м����� ygc 2012-10-22
                string SHISQL = "insert  into EightTable_LDJGXZ  select  substr(" +
                 tableName + ".shi,1,4) as ͳ�Ƶ�λ,substr(QI_YUAN,1,1) as ��Դ," +
                 "sum(case when " + tableName + ".sen_lin_lb between '11' and '12'and " + tableName + ".dl<='114' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as �����ֺϼ�," +
                 "sum (case when " + tableName + ".sen_lin_lb ='11' and " + tableName + ".dl<='114' then round(" + tableName + "." + StatisticsFieldName + ",2)  else 0 end) as �ص㹫���ֺϼ�," +
                 "sum(case when " + tableName + ".sen_lin_lb='11' and " + tableName + ".dl<='112' and (substr(" + tableName + ".lz,1,2)='11'or substr(" + tableName + ".lz,1,2)='12') then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end ) as �ص㹫��С��," +
                 "sum (case when " + tableName + ".sen_lin_lb='11' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='11' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as �ص㹫�������," +
                 "sum(case when " + tableName + ".sen_lin_lb='11'and " + tableName + ".dl<='112' and substr(" + tableName + ".lz,1,2)='12' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end ) as �ص㹫��������," +
                 "sum(case when " + tableName + ".sen_lin_lb='11'and (((substr(" + tableName + ".lz,1,2)='11'or substr(" + tableName + ".lz,1,2)='12') and " + tableName + ".dl between '113' and '114')) and (substr(" + tableName + ".lz,1,2)<>'11' or substr(" + tableName + ".lz,1,2)<>'12') then round(" + tableName + "." + StatisticsFieldName + ",2)  else 0 end) as  �ص㹫������," +
                 "sum(case when " + tableName + ".sen_lin_lb ='12' and " + tableName + ".dl<='114' then round(" + tableName + "." + StatisticsFieldName + ",2)   end ) as һ�㹫��ϼ�," +
                 "sum(case when " + tableName + ".sen_lin_lb='12' and " + tableName + ".dl<='112' and (substr(" + tableName + ".lz,1,2)='11'or substr(" + tableName + ".lz,1,2)='12') then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end ) as һ�㹫��С��," +
                 "sum(case when " + tableName + ".sen_lin_lb='12' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='11' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as һ�㹫�������," +
                 "sum(case when " + tableName + ".sen_lin_lb='12'and " + tableName + ".dl<='112' and substr(" + tableName + ".lz,1,2)='12' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end ) as һ�㹫��������," +
                 "sum(case when " + tableName + ".sen_lin_lb='12'and ((substr(" + tableName + ".lz,1,2)<>'11' and substr(" + tableName + ".lz,1,2)<>'12') or (((substr(" + tableName + ".lz,1,2)='11' or substr(" + tableName + ".lz,1,2)='12')) and " + tableName + ".dl between '113' and '114')) then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as һ�㹫������," +
                 "sum(case when " + tableName + ".sen_lin_lb between '21' and '22'and " + tableName + ".dl<='114' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end ) as ��Ʒ�ֺϼ�," +
                 "sum(case when " + tableName + ".sen_lin_lb ='21' and " + tableName + ".dl<='114' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as �ص���Ʒ�ֺϼ�," +
                 "sum(case when " + tableName + ".sen_lin_lb='21' and " + tableName + ".dl<='114' and (substr(" + tableName + ".lz,1,2) between '23' and '25') then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end ) as �ص���Ʒ��С��," +
                 "sum(case when " + tableName + ".sen_lin_lb='21' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='23' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as �ص��ò���," +
                 "sum(case when " + tableName + ".sen_lin_lb='21' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='25'and " + tableName + ".dl='114' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as �ص㾭����," +
                 "sum(case when " + tableName + ".sen_lin_lb='21' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='24' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as �ص�н̼��," +
                 "sum(case when " + tableName + ".sen_lin_lb='21'and ((substr(" + tableName + ".lz,1,2)<>'23' and substr(" + tableName + ".lz,1,2)<>'24' and substr(" + tableName + ".lz,1,2)<>'25') or (substr(" + tableName + ".lz,1,2)='23' and " + tableName + ".dl>'112') or (substr(" + tableName + ".lz,1,2)='25' and " + tableName + ".dl<>'114')) then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as �ص�����," +
                 "sum(case when " + tableName + ".sen_lin_lb ='22' and " + tableName + ".dl<='114' then round(" + tableName + "." + StatisticsFieldName + ",2)  else 0 end) as һ����Ʒ�ֺϼ�," +
                 "sum(case when " + tableName + ".sen_lin_lb='22' and " + tableName + ".dl<='114' and (substr(" + tableName + ".lz,1,2) between '23' and '25') then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end ) as һ����Ʒ��С��," +
                 "sum(case when " + tableName + ".sen_lin_lb='22' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='23' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as һ���ò���," +
                 "sum(case when " + tableName + ".sen_lin_lb='22' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='25'and " + tableName + ".dl='114' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as һ�㾭����," +
                 "sum(case when " + tableName + ".sen_lin_lb='22' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='24' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as һ��н̼��," +
                 "sum(case when " + tableName + ".sen_lin_lb='22'and ((substr(" + tableName + ".lz,1,2)<>'23' and substr(" + tableName + ".lz,1,2)<>'24' and substr(" + tableName + ".lz,1,2)<>'25') or (substr(" + tableName + ".lz,1,2)='23' and " + tableName + ".dl>'112') or (substr(" + tableName + ".lz,1,2)='25' and " + tableName + ".dl<>'114')) then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as һ������" +
                 " from " + tableName +
                 " where  substr(QI_YUAN,1,1)<>' '" +
                 " group by substr(shi,1,4),rollup(substr(QI_YUAN,1,1)) " +
                 " order by substr(QI_YUAN,1,1)";
                pWorkspace.ExecuteSQL(SHISQL);
                //��ѯʡ������ ygc 2012-10-24 
                string SHENGSQL = "insert  into EightTable_LDJGXZ  select  substr(" +
                 tableName + ".sheng,1,2) as ͳ�Ƶ�λ,substr(QI_YUAN,1,1) as ��Դ," +
                 "sum(case when " + tableName + ".sen_lin_lb between '11' and '12'and " + tableName + ".dl<='114' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as �����ֺϼ�," +
                 "sum (case when " + tableName + ".sen_lin_lb ='11' and " + tableName + ".dl<='114' then round(" + tableName + "." + StatisticsFieldName + ",2)  else 0 end) as �ص㹫���ֺϼ�," +
                 "sum(case when " + tableName + ".sen_lin_lb='11' and " + tableName + ".dl<='112' and (substr(" + tableName + ".lz,1,2)='11'or substr(" + tableName + ".lz,1,2)='12') then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end ) as �ص㹫��С��," +
                 "sum (case when " + tableName + ".sen_lin_lb='11' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='11' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as �ص㹫�������," +
                 "sum(case when " + tableName + ".sen_lin_lb='11'and " + tableName + ".dl<='112' and substr(" + tableName + ".lz,1,2)='12' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end ) as �ص㹫��������," +
                 "sum(case when " + tableName + ".sen_lin_lb='11'and (((substr(" + tableName + ".lz,1,2)='11'or substr(" + tableName + ".lz,1,2)='12') and " + tableName + ".dl between '113' and '114')) and (substr(" + tableName + ".lz,1,2)<>'11' or substr(" + tableName + ".lz,1,2)<>'12') then round(" + tableName + "." + StatisticsFieldName + ",2)  else 0 end) as  �ص㹫������," +
                 "sum(case when " + tableName + ".sen_lin_lb ='12' and " + tableName + ".dl<='114' then round(" + tableName + "." + StatisticsFieldName + ",2)   end ) as һ�㹫��ϼ�," +
                 "sum(case when " + tableName + ".sen_lin_lb='12' and " + tableName + ".dl<='112' and (substr(" + tableName + ".lz,1,2)='11'or substr(" + tableName + ".lz,1,2)='12') then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end ) as һ�㹫��С��," +
                 "sum(case when " + tableName + ".sen_lin_lb='12' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='11' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as һ�㹫�������," +
                 "sum(case when " + tableName + ".sen_lin_lb='12'and " + tableName + ".dl<='112' and substr(" + tableName + ".lz,1,2)='12' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end ) as һ�㹫��������," +
                 "sum(case when " + tableName + ".sen_lin_lb='12'and ((substr(" + tableName + ".lz,1,2)<>'11' and substr(" + tableName + ".lz,1,2)<>'12') or (((substr(" + tableName + ".lz,1,2)='11' or substr(" + tableName + ".lz,1,2)='12')) and " + tableName + ".dl between '113' and '114')) then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as һ�㹫������," +
                 "sum(case when " + tableName + ".sen_lin_lb between '21' and '22'and " + tableName + ".dl<='114' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end ) as ��Ʒ�ֺϼ�," +
                 "sum(case when " + tableName + ".sen_lin_lb ='21' and " + tableName + ".dl<='114' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as �ص���Ʒ�ֺϼ�," +
                 "sum(case when " + tableName + ".sen_lin_lb='21' and " + tableName + ".dl<='114' and (substr(" + tableName + ".lz,1,2) between '23' and '25') then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end ) as �ص���Ʒ��С��," +
                 "sum(case when " + tableName + ".sen_lin_lb='21' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='23' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as �ص��ò���," +
                 "sum(case when " + tableName + ".sen_lin_lb='21' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='25'and " + tableName + ".dl='114' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as �ص㾭����," +
                 "sum(case when " + tableName + ".sen_lin_lb='21' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='24' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as �ص�н̼��," +
                 "sum(case when " + tableName + ".sen_lin_lb='21'and ((substr(" + tableName + ".lz,1,2)<>'23' and substr(" + tableName + ".lz,1,2)<>'24' and substr(" + tableName + ".lz,1,2)<>'25') or (substr(" + tableName + ".lz,1,2)='23' and " + tableName + ".dl>'112') or (substr(" + tableName + ".lz,1,2)='25' and " + tableName + ".dl<>'114')) then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as �ص�����," +
                 "sum(case when " + tableName + ".sen_lin_lb ='22' and " + tableName + ".dl<='114' then round(" + tableName + "." + StatisticsFieldName + ",2)  else 0 end) as һ����Ʒ�ֺϼ�," +
                 "sum(case when " + tableName + ".sen_lin_lb='22' and " + tableName + ".dl<='114' and (substr(" + tableName + ".lz,1,2) between '23' and '25') then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end ) as һ����Ʒ��С��," +
                 "sum(case when " + tableName + ".sen_lin_lb='22' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='23' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as һ���ò���," +
                 "sum(case when " + tableName + ".sen_lin_lb='22' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='25'and " + tableName + ".dl='114' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as һ�㾭����," +
                 "sum(case when " + tableName + ".sen_lin_lb='22' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='24' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as һ��н̼��," +
                 "sum(case when " + tableName + ".sen_lin_lb='22'and ((substr(" + tableName + ".lz,1,2)<>'23' and substr(" + tableName + ".lz,1,2)<>'24' and substr(" + tableName + ".lz,1,2)<>'25') or (substr(" + tableName + ".lz,1,2)='23' and " + tableName + ".dl>'112') or (substr(" + tableName + ".lz,1,2)='25' and " + tableName + ".dl<>'114')) then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as һ������" +
                 " from " + tableName +
                 " where  substr(QI_YUAN,1,1)<>' '" +
                 " group by substr(sheng,1,2),rollup(substr(QI_YUAN,1,1)) " +
                 " order by substr(QI_YUAN,1,1)";
                pWorkspace.ExecuteSQL(SHENGSQL);
                #endregion
                #region ����ͳ��
                //���¹�����С��
                //ygc 2012-8-22
                string UpdateXJ = "update EightTable_LDJGXZ set �ص㹫��С��=�ص㹫�������+ �ص㹫��������";
                pWorkspace.ExecuteSQL(UpdateXJ);
                //�����ص㹫���ֺϼ� ygc 2012-8-22
                string UpdateZDFYHJ = "update EightTable_LDJGXZ set �ص㹫���ֺϼ�=�ص㹫��С��+�ص㹫������";
                pWorkspace.ExecuteSQL(UpdateZDFYHJ);
                //����һ�㹫��С�� ygc 2012-8-22
                string UpdateYBGYXJ = "update EightTable_LDJGXZ set һ�㹫��С��=һ�㹫�������+һ�㹫��������";
                pWorkspace.ExecuteSQL(UpdateYBGYXJ);
                //����һ�㹫��ϼ� ygc 2012-8-22
                string UpdateYBGYHJ = "update EightTable_LDJGXZ set һ�㹫��ϼ�=һ�㹫��С��+һ�㹫������";
                pWorkspace.ExecuteSQL(UpdateYBGYHJ);
                //���¹����ֺϼ� ygc 2012-8-22
                pWorkspace.ExecuteSQL("update EightTable_LDJGXZ set �����ֺϼ�=һ�㹫��ϼ� + �ص㹫���ֺϼ�");
                //�����ص���Ʒ��С�� ygc 2012-8-22
                pWorkspace.ExecuteSQL("update EightTable_LDJGXZ set �ص���Ʒ��С��=�ص��ò���+�ص㾭����+�ص�н̼��");
                //�����ص���Ʒ�ֺϼ� ygc 2012-8-22
                pWorkspace.ExecuteSQL("update EightTable_LDJGXZ set �ص���Ʒ�ֺϼ�=�ص���Ʒ��С��+�ص�����");
                //����һ����Ʒ��С�� ygc 2012-8-22
                pWorkspace.ExecuteSQL("update EightTable_LDJGXZ set һ����Ʒ��С��=һ���ò���+һ�㾭����+һ��н̼��");
                //����һ����Ʒ�ֺϼ� ygc 2012-8-22
                pWorkspace.ExecuteSQL("update EightTable_LDJGXZ set һ����Ʒ�ֺϼ�=һ����Ʒ��С��+һ������");
                //������Ʒ�ֺϼ� ygc 2012-8-22
                pWorkspace.ExecuteSQL("update EightTable_LDJGXZ set ��Ʒ�ֺϼ� =һ����Ʒ�ֺϼ�+�ص���Ʒ�ֺϼ�");
                #endregion
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }
        private static bool DoLCLDJGXZ_Statistic(IFeatureClass pFeatureClass, string StatisticsFieldName)
        {
            if (pFeatureClass == null)
            {
                return false;
            }
            IWorkspace pWorkspace = null;
            string tableName = "";
            try
            {
                pWorkspace = pFeatureClass.FeatureDataset.Workspace;
                tableName = (pFeatureClass as IDataset).Name;
            }
            catch (Exception ex)
            { }
            try
            {
                #region ִ��ͳ��
                if (!ExistTable(pWorkspace, "EightTable_LDJGXZ"))
                {
                    //��ѯ�ؼ�ͳ������SQL���
                    //ygc 2012-8-22
                    string CitySQL = "create table EightTable_LDJGXZ as select " +
                                     tableName + ".lc as ͳ�Ƶ�λ,substr(QI_YUAN,1,1) as ��Դ," +
                                     "sum(case when " + tableName + ".sen_lin_lb between '11' and '12'and " + tableName + ".dl<='114' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as �����ֺϼ�," +
                                     "sum (case when " + tableName + ".sen_lin_lb ='11' and " + tableName + ".dl<='114' then round(" + tableName + "." + StatisticsFieldName + ",2)  else 0 end) as �ص㹫���ֺϼ�," +
                                     "sum(case when " + tableName + ".sen_lin_lb='11' and " + tableName + ".dl<='112' and (substr(" + tableName + ".lz,1,2)='11'or substr(" + tableName + ".lz,1,2)='12') then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end ) as �ص㹫��С��," +
                                     "sum (case when " + tableName + ".sen_lin_lb='11' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='11' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as �ص㹫�������," +
                                     "sum(case when " + tableName + ".sen_lin_lb='11'and " + tableName + ".dl<='112' and substr(" + tableName + ".lz,1,2)='12' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end ) as �ص㹫��������," +
                                     "sum(case when " + tableName + ".sen_lin_lb='11'and (((substr(" + tableName + ".lz,1,2)='11'or substr(" + tableName + ".lz,1,2)='12') and " + tableName + ".dl between '113' and '114')) and (substr(" + tableName + ".lz,1,2)<>'11' or substr(" + tableName + ".lz,1,2)<>'12') then round(" + tableName + "." + StatisticsFieldName + ",2)  else 0 end) as  �ص㹫������," +
                                     "sum(case when " + tableName + ".sen_lin_lb ='12' and " + tableName + ".dl<='114' then round(" + tableName + "." + StatisticsFieldName + ",2)   end ) as һ�㹫��ϼ�," +
                                     "sum(case when " + tableName + ".sen_lin_lb='12' and " + tableName + ".dl<='112' and (substr(" + tableName + ".lz,1,2)='11'or substr(" + tableName + ".lz,1,2)='12') then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end ) as һ�㹫��С��," +
                                     "sum(case when " + tableName + ".sen_lin_lb='12' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='11' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as һ�㹫�������," +
                                     "sum(case when " + tableName + ".sen_lin_lb='12'and " + tableName + ".dl<='112' and substr(" + tableName + ".lz,1,2)='12' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end ) as һ�㹫��������," +
                                     "sum(case when " + tableName + ".sen_lin_lb='12'and ((substr(" + tableName + ".lz,1,2)<>'11' and substr(" + tableName + ".lz,1,2)<>'12') or (((substr(" + tableName + ".lz,1,2)='11' or substr(" + tableName + ".lz,1,2)='12')) and " + tableName + ".dl between '113' and '114')) then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as һ�㹫������," +
                                     "sum(case when " + tableName + ".sen_lin_lb between '21' and '22'and " + tableName + ".dl<='114' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end ) as ��Ʒ�ֺϼ�," +
                                     "sum(case when " + tableName + ".sen_lin_lb ='21' and " + tableName + ".dl<='114' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as �ص���Ʒ�ֺϼ�," +
                                     "sum(case when " + tableName + ".sen_lin_lb='21' and " + tableName + ".dl<='114' and (substr(" + tableName + ".lz,1,2) between '23' and '25') then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end ) as �ص���Ʒ��С��," +
                                     "sum(case when " + tableName + ".sen_lin_lb='21' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='23' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as �ص��ò���," +
                                     "sum(case when " + tableName + ".sen_lin_lb='21' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='25'and " + tableName + ".dl='114' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as �ص㾭����," +
                                     "sum(case when " + tableName + ".sen_lin_lb='21' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='24' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as �ص�н̼��," +
                                     "sum(case when " + tableName + ".sen_lin_lb='21'and ((substr(" + tableName + ".lz,1,2)<>'23' and substr(" + tableName + ".lz,1,2)<>'24' and substr(" + tableName + ".lz,1,2)<>'25') or (substr(" + tableName + ".lz,1,2)='23' and " + tableName + ".dl>'112') or (substr(" + tableName + ".lz,1,2)='25' and " + tableName + ".dl<>'114')) then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as �ص�����," +
                                     "sum(case when " + tableName + ".sen_lin_lb ='22' and " + tableName + ".dl<='114' then round(" + tableName + "." + StatisticsFieldName + ",2)  else 0 end) as һ����Ʒ�ֺϼ�," +
                                     "sum(case when " + tableName + ".sen_lin_lb='22' and " + tableName + ".dl<='114' and (substr(" + tableName + ".lz,1,2) between '23' and '25') then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end ) as һ����Ʒ��С��," +
                                     "sum(case when " + tableName + ".sen_lin_lb='22' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='23' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as һ���ò���," +
                                     "sum(case when " + tableName + ".sen_lin_lb='22' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='25'and " + tableName + ".dl='114' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as һ�㾭����," +
                                     "sum(case when " + tableName + ".sen_lin_lb='22' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='24' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as һ��н̼��," +
                                     "sum(case when " + tableName + ".sen_lin_lb='22'and ((substr(" + tableName + ".lz,1,2)<>'23' and substr(" + tableName + ".lz,1,2)<>'24' and substr(" + tableName + ".lz,1,2)<>'25') or (substr(" + tableName + ".lz,1,2)='23' and " + tableName + ".dl>'112') or (substr(" + tableName + ".lz,1,2)='25' and " + tableName + ".dl<>'114')) then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as һ������" +
                                     " from " + tableName +
                                     " where substr(QI_YUAN,1,1)<>' ' and lc<>' '" +
                                     " group by lc,rollup(substr(QI_YUAN,1,1)) " +
                                     " order by substr(QI_YUAN,1,1) ";
                    pWorkspace.ExecuteSQL(CitySQL);
                    pWorkspace.ExecuteSQL("alter table EightTable_LDJGXZ modify ͳ�Ƶ�λ nvarchar2(20)");

                }
                else
                {
                    //��ѯ����ͳ������
                    //ygc 2012-8-22
                    string townsSQL = "insert  into EightTable_LDJGXZ  select " +
                                     tableName + ".lc as ͳ�Ƶ�λ,substr(QI_YUAN,1,1) as ��Դ," +
                                     "sum(case when " + tableName + ".sen_lin_lb between '11' and '12'and " + tableName + ".dl<='114' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as �����ֺϼ�," +
                                     "sum (case when " + tableName + ".sen_lin_lb ='11' and " + tableName + ".dl<='114' then round(" + tableName + "." + StatisticsFieldName + ",2)  else 0 end) as �ص㹫���ֺϼ�," +
                                     "sum(case when " + tableName + ".sen_lin_lb='11' and " + tableName + ".dl<='112' and (substr(" + tableName + ".lz,1,2)='11'or substr(" + tableName + ".lz,1,2)='12') then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end ) as �ص㹫��С��," +
                                     "sum (case when " + tableName + ".sen_lin_lb='11' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='11' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as �ص㹫�������," +
                                     "sum(case when " + tableName + ".sen_lin_lb='11'and " + tableName + ".dl<='112' and substr(" + tableName + ".lz,1,2)='12' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end ) as �ص㹫��������," +
                                     "sum(case when " + tableName + ".sen_lin_lb='11'and (((substr(" + tableName + ".lz,1,2)='11'or substr(" + tableName + ".lz,1,2)='12') and " + tableName + ".dl between '113' and '114')) and (substr(" + tableName + ".lz,1,2)<>'11' or substr(" + tableName + ".lz,1,2)<>'12') then round(" + tableName + "." + StatisticsFieldName + ",2)  else 0 end) as  �ص㹫������," +
                                     "sum(case when " + tableName + ".sen_lin_lb ='12' and " + tableName + ".dl<='114' then round(" + tableName + "." + StatisticsFieldName + ",2)   end ) as һ�㹫��ϼ�," +
                                     "sum(case when " + tableName + ".sen_lin_lb='12' and " + tableName + ".dl<='112' and (substr(" + tableName + ".lz,1,2)='11'or substr(" + tableName + ".lz,1,2)='12') then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end ) as һ�㹫��С��," +
                                     "sum(case when " + tableName + ".sen_lin_lb='12' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='11' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as һ�㹫�������," +
                                     "sum(case when " + tableName + ".sen_lin_lb='12'and " + tableName + ".dl<='112' and substr(" + tableName + ".lz,1,2)='12' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end ) as һ�㹫��������," +
                                     "sum(case when " + tableName + ".sen_lin_lb='12'and ((substr(" + tableName + ".lz,1,2)<>'11' and substr(" + tableName + ".lz,1,2)<>'12') or (((substr(" + tableName + ".lz,1,2)='11' or substr(" + tableName + ".lz,1,2)='12')) and " + tableName + ".dl between '113' and '114')) then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as һ�㹫������," +
                                     "sum(case when " + tableName + ".sen_lin_lb between '21' and '22'and " + tableName + ".dl<='114' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end ) as ��Ʒ�ֺϼ�," +
                                     "sum(case when " + tableName + ".sen_lin_lb ='21' and " + tableName + ".dl<='114' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as �ص���Ʒ�ֺϼ�," +
                                     "sum(case when " + tableName + ".sen_lin_lb='21' and " + tableName + ".dl<='114' and (substr(" + tableName + ".lz,1,2) between '23' and '25') then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end ) as �ص���Ʒ��С��," +
                                     "sum(case when " + tableName + ".sen_lin_lb='21' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='23' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as �ص��ò���," +
                                     "sum(case when " + tableName + ".sen_lin_lb='21' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='25'and " + tableName + ".dl='114' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as �ص㾭����," +
                                     "sum(case when " + tableName + ".sen_lin_lb='21' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='24' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as �ص�н̼��," +
                                     "sum(case when " + tableName + ".sen_lin_lb='21'and ((substr(" + tableName + ".lz,1,2)<>'23' and substr(" + tableName + ".lz,1,2)<>'24' and substr(" + tableName + ".lz,1,2)<>'25') or (substr(" + tableName + ".lz,1,2)='23' and " + tableName + ".dl>'112') or (substr(" + tableName + ".lz,1,2)='25' and " + tableName + ".dl<>'114')) then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as �ص�����," +
                                     "sum(case when " + tableName + ".sen_lin_lb ='22' and " + tableName + ".dl<='114' then round(" + tableName + "." + StatisticsFieldName + ",2)  else 0 end) as һ����Ʒ�ֺϼ�," +
                                     "sum(case when " + tableName + ".sen_lin_lb='22' and " + tableName + ".dl<='114' and (substr(" + tableName + ".lz,1,2) between '23' and '25') then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end ) as һ����Ʒ��С��," +
                                     "sum(case when " + tableName + ".sen_lin_lb='22' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='23' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as һ���ò���," +
                                     "sum(case when " + tableName + ".sen_lin_lb='22' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='25'and " + tableName + ".dl='114' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as һ�㾭����," +
                                     "sum(case when " + tableName + ".sen_lin_lb='22' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='24' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as һ��н̼��," +
                                     "sum(case when " + tableName + ".sen_lin_lb='22'and ((substr(" + tableName + ".lz,1,2)<>'23' and substr(" + tableName + ".lz,1,2)<>'24' and substr(" + tableName + ".lz,1,2)<>'25') or (substr(" + tableName + ".lz,1,2)='23' and " + tableName + ".dl>'112') or (substr(" + tableName + ".lz,1,2)='25' and " + tableName + ".dl<>'114')) then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as һ������" +
                                     " from " + tableName +
                                     " where  substr(QI_YUAN,1,1)<>' ' and lc<>' '" +
                                     " group by lc,rollup(substr(QI_YUAN,1,1)) " +
                                     " order by substr(QI_YUAN,1,1)";
                    pWorkspace.ExecuteSQL(townsSQL);
                    pWorkspace.ExecuteSQL("alter table EightTable_LDJGXZ modify ��Դ nvarchar2(10)");
                    UpdateStatistictable(pWorkspace, "EightTable_LDJGXZ", "��Դ", "��Ȼ", "1");
                    UpdateStatistictable(pWorkspace, "EightTable_LDJGXZ", "��Դ", "�˹�", "2");
                }
                #endregion
                #region ����ͳ��
                //���¹�����С��
                //ygc 2012-8-22
                string UpdateXJ = "update EightTable_LDJGXZ set �ص㹫��С��=�ص㹫�������+ �ص㹫��������";
                pWorkspace.ExecuteSQL(UpdateXJ);
                //�����ص㹫���ֺϼ� ygc 2012-8-22
                string UpdateZDFYHJ = "update EightTable_LDJGXZ set �ص㹫���ֺϼ�=�ص㹫��С��+�ص㹫������";
                pWorkspace.ExecuteSQL(UpdateZDFYHJ);
                //����һ�㹫��С�� ygc 2012-8-22
                string UpdateYBGYXJ = "update EightTable_LDJGXZ set һ�㹫��С��=һ�㹫�������+һ�㹫��������";
                pWorkspace.ExecuteSQL(UpdateYBGYXJ);
                //����һ�㹫��ϼ� ygc 2012-8-22
                string UpdateYBGYHJ = "update EightTable_LDJGXZ set һ�㹫��ϼ�=һ�㹫��С��+һ�㹫������";
                pWorkspace.ExecuteSQL(UpdateYBGYHJ);
                //���¹����ֺϼ� ygc 2012-8-22
                pWorkspace.ExecuteSQL("update EightTable_LDJGXZ set �����ֺϼ�=һ�㹫��ϼ� + �ص㹫���ֺϼ�");
                //�����ص���Ʒ��С�� ygc 2012-8-22
                pWorkspace.ExecuteSQL("update EightTable_LDJGXZ set �ص���Ʒ��С��=�ص��ò���+�ص㾭����+�ص�н̼��");
                //�����ص���Ʒ�ֺϼ� ygc 2012-8-22
                pWorkspace.ExecuteSQL("update EightTable_LDJGXZ set �ص���Ʒ�ֺϼ�=�ص���Ʒ��С��+�ص�����");
                //����һ����Ʒ��С�� ygc 2012-8-22
                pWorkspace.ExecuteSQL("update EightTable_LDJGXZ set һ����Ʒ��С��=һ���ò���+һ�㾭����+һ��н̼��");
                //����һ����Ʒ�ֺϼ� ygc 2012-8-22
                pWorkspace.ExecuteSQL("update EightTable_LDJGXZ set һ����Ʒ�ֺϼ�=һ����Ʒ��С��+һ������");
                //������Ʒ�ֺϼ� ygc 2012-8-22
                pWorkspace.ExecuteSQL("update EightTable_LDJGXZ set ��Ʒ�ֺϼ� =һ����Ʒ�ֺϼ�+�ص���Ʒ�ֺϼ�");
                #endregion
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }
        //���Ҽ������ֵطֱ����ȼ���״
        //ͨ��SQL����ù��Ҽ������ֵطֱ����ȼ���״ͳ�Ʊ� ygc 2012-8-23
        private static bool DoGJGYHDJ_Statistic(IFeatureClass pFeatureClass, string StatisticsFieldName)
        {
            if (pFeatureClass == null)
            {
                return false;
            }
            IWorkspace pWorkspace = null;
            string tableName = "";
            try
            {
                pWorkspace = pFeatureClass.FeatureDataset.Workspace;
                tableName = (pFeatureClass as IDataset).Name;
            }
            catch (Exception ex)
            { }
            DropTable(pWorkspace, "EightTable_GJGYHDJ");
            try
            {
                //����SQL����ѯͳ���������� ygc 2012-8-23
                string townsSQL = "create table EightTable_GJGYHDJ as select " +
                                 "substr(xiang,1,8) as ͳ�Ƶ�λ,substr(QI_YUAN,1,1) as ��Դ," +
                                 "sum(case when BHDJ between '1' and '3' and dl<='112' then round(" + StatisticsFieldName + ",2) else 0 end) as �ϼ�," +
                                 "sum(case when BHDJ between '1' and '3' and substr(lz,1,2)='11' and dl<='112' then round(" + StatisticsFieldName + ",2) else 0 end) as �����ֺϼ�," +
                                 "sum(case when BHDJ between '1' and '3' and substr(lz,1,2)='12' and dl<='112' then round(" + StatisticsFieldName + ",2) else 0 end) as �����ֺϼ�," +
                                 "sum(case when BHDJ between '1' and '3' and ((substr(lz,1,2)<>'11' and substr(lz,1,2)<>'12') or ((substr(lz,1,2)='11' or substr(lz,1,2)='12') and dl>'112')) then round(" + StatisticsFieldName + ",2) else 0 end) as �����ϼ�," +
                                 "sum(case when BHDJ ='1' and dl<='112' then round(" + StatisticsFieldName + ",2) else 0 end) as һ��С��," +
                                 "sum(case when BHDJ='1' and substr(lz,1,2)='11' and dl<='112' then round(" + StatisticsFieldName + ",2) else 0 end ) as һ������," +
                                 "sum(case when BHDJ='1' and substr(lz,1,2)='12' and dl<='112' then round(" + StatisticsFieldName + ",2) else 0 end) as һ������," +
                                 "sum(case when BHDJ='1' and ((substr(lz,1,2)<>'11' and substr(lz,1,2)<>'12') or ((substr(lz,1,2)='11' or substr(lz,1,2)='12') and dl>'112')) then round(" + StatisticsFieldName + ",2) else 0 end) as һ������," +
                                 "sum(case when  BHDJ ='2' and dl<='112' then round(" + StatisticsFieldName + ",2) else 0 end) as ����С��," +
                                 "sum(case when BHDJ='2' and substr(lz,1,2)='11' and dl<='112' then round(" + StatisticsFieldName + ",2) else 0 end) as ��������," +
                                 "sum(case when BHDJ='2' and substr(lz,1,2)='12' and dl<='112' then round(" + StatisticsFieldName + ",2) else 0 end ) as ��������," +
                                 "sum(case when BHDJ='2' and ((substr(lz,1,2)<>'11' and substr(lz,1,2)<>'12') or ((substr(lz,1,2)='11' or substr(lz,1,2)='12') and dl>'112')) then round(" + StatisticsFieldName + ",2) else 0 end) as ��������," +
                                 "sum(case when BHDJ ='3' and dl<='112' then round(" + StatisticsFieldName + ",2) else 0 end) as ����С��," +
                                 "sum(case when BHDJ='3' and substr(lz,1,2)='11' and dl<='112' then round(" + StatisticsFieldName + ",2) else 0 end) as ��������," +
                                 "sum(case when BHDJ='3' and substr(lz,1,2)='12' and dl<='112'then round(" + StatisticsFieldName + ",2) else 0 end) as ��������," +
                                 "sum(case when BHDJ='3' and ((substr(lz,1,2)<>'11' and substr(lz,1,2)<>'12') or ((substr(lz,1,2)='11' or substr(lz,1,2)='12') and dl>'112')) then round(" + StatisticsFieldName + ",2) else 0 end) as  ��������" +
                                 "  from " + tableName +
                                 "  where substr(QI_YUAN,1,1)<>' ' and QI_YUAN is not null and sq='1'" +
                                 "  group by substr(xiang,1,8),rollup(substr(QI_YUAN,1,1))" +
                                 "  order by substr(QI_YUAN,1,1) ";
                pWorkspace.ExecuteSQL(townsSQL);
                pWorkspace.ExecuteSQL("alter table EightTable_GJGYHDJ modify ͳ�Ƶ�λ nvarchar2(20)");
                //ͨ��SQL������ؼ�ͳ������ ygc 2012-8-23
                string CitySQL = "insert into EightTable_GJGYHDJ select " +
                                 "xian as ͳ�Ƶ�λ,substr(QI_YUAN,1,1) as ��Դ," +
                                 "sum(case when BHDJ between '1' and '3' and dl<='112' then round(" + StatisticsFieldName + ",2) else 0 end) as �ϼ�," +
                                 "sum(case when BHDJ between '1' and '3' and substr(lz,1,2)='11' and dl<='112' then round(" + StatisticsFieldName + ",2) else 0 end) as �����ֺϼ�," +
                                 "sum(case when BHDJ between '1' and '3' and substr(lz,1,2)='12' and dl<='112' then round(" + StatisticsFieldName + ",2) else 0 end) as �����ֺϼ�," +
                                 "sum(case when BHDJ between '1' and '3' and ((substr(lz,1,2)<>'11' and substr(lz,1,2)<>'12') or ((substr(lz,1,2)='11' or substr(lz,1,2)='12') and dl>'112')) then round(" + StatisticsFieldName + ",2) else 0 end) as �����ϼ�," +
                                 "sum(case when BHDJ ='1' and dl<='112' then round(" + StatisticsFieldName + ",2) else 0 end) as һ��С��," +
                                 "sum(case when BHDJ='1' and substr(lz,1,2)='11' and dl<='112' then round(" + StatisticsFieldName + ",2) else 0 end ) as һ������," +
                                 "sum(case when BHDJ='1' and substr(lz,1,2)='12' and dl<='112' then round(" + StatisticsFieldName + ",2) else 0 end) as һ������," +
                                 "sum(case when BHDJ='1' and ((substr(lz,1,2)<>'11' and substr(lz,1,2)<>'12') or ((substr(lz,1,2)='11' or substr(lz,1,2)='12') and dl>'112')) then round(" + StatisticsFieldName + ",2) else 0 end) as һ������," +
                                 "sum(case when BHDJ ='2' and dl<='112' then round(" + StatisticsFieldName + ",2) else 0 end) as ����С��," +
                                 "sum(case when BHDJ='2' and substr(lz,1,2)='11' and dl<='112' then round(" + StatisticsFieldName + ",2) else 0 end) as ��������," +
                                 "sum(case when BHDJ='2' and substr(lz,1,2)='12' and dl<='112' then round(" + StatisticsFieldName + ",2) else 0 end ) as ��������," +
                                 "sum(case when BHDJ='2' and ((substr(lz,1,2)<>'11' and substr(lz,1,2)<>'12') or ((substr(lz,1,2)='11' or substr(lz,1,2)='12') and dl>'112')) then round(" + StatisticsFieldName + ",2) else 0 end) as ��������," +
                                 "sum(case when BHDJ ='3' and dl<='112' then round(" + StatisticsFieldName + ",2) else 0 end) as ����С��," +
                                 "sum(case when BHDJ='3' and substr(lz,1,2)='11' and dl<='112' then round(" + StatisticsFieldName + ",2) else 0 end) as ��������," +
                                 "sum(case when BHDJ='3' and substr(lz,1,2)='12' and dl<='112'then round(" + StatisticsFieldName + ",2) else 0 end) as ��������," +
                                 "sum(case when BHDJ='3' and ((substr(lz,1,2)<>'11' and substr(lz,1,2)<>'12') or ((substr(lz,1,2)='11' or substr(lz,1,2)='12') and dl>'112')) then round(" + StatisticsFieldName + ",2) else 0 end) as  ��������" +
                                 "  from " + tableName +
                                 "  where substr(QI_YUAN,1,1)<>' ' and QI_YUAN is not null and sq='1'" +
                                 "  group by xian,rollup(substr(QI_YUAN,1,1))" +
                                 "  order by substr(QI_YUAN,1,1) ";
                pWorkspace.ExecuteSQL(CitySQL);
                
                //��ȡ�м����� ygc 2012-10-22
                string SHISQL = "insert into EightTable_GJGYHDJ select " +
                 "substr(shi,1,4) as ͳ�Ƶ�λ,substr(QI_YUAN,1,1) as ��Դ," +
                 "sum(case when BHDJ between '1' and '3' and dl<='112' then round(" + StatisticsFieldName + ",2) else 0 end) as �ϼ�," +
                 "sum(case when BHDJ between '1' and '3' and substr(lz,1,2)='11' and dl<='112' then round(" + StatisticsFieldName + ",2) else 0 end) as �����ֺϼ�," +
                 "sum(case when BHDJ between '1' and '3' and substr(lz,1,2)='12' and dl<='112' then round(" + StatisticsFieldName + ",2) else 0 end) as �����ֺϼ�," +
                 "sum(case when BHDJ between '1' and '3' and ((substr(lz,1,2)<>'11' and substr(lz,1,2)<>'12') or ((substr(lz,1,2)='11' or substr(lz,1,2)='12') and dl>'112')) then round(" + StatisticsFieldName + ",2) else 0 end) as �����ϼ�," +
                 "sum(case when BHDJ ='1' and dl<='112' then round(" + StatisticsFieldName + ",2) else 0 end) as һ��С��," +
                 "sum(case when BHDJ='1' and substr(lz,1,2)='11' and dl<='112' then round(" + StatisticsFieldName + ",2) else 0 end ) as һ������," +
                 "sum(case when BHDJ='1' and substr(lz,1,2)='12' and dl<='112' then round(" + StatisticsFieldName + ",2) else 0 end) as һ������," +
                 "sum(case when BHDJ='1' and ((substr(lz,1,2)<>'11' and substr(lz,1,2)<>'12') or ((substr(lz,1,2)='11' or substr(lz,1,2)='12') and dl>'112')) then round(" + StatisticsFieldName + ",2) else 0 end) as һ������," +
                 "sum(case when  BHDJ ='2' and dl<='112' then round(" + StatisticsFieldName + ",2) else 0 end) as ����С��," +
                 "sum(case when BHDJ='2' and substr(lz,1,2)='11' and dl<='112' then round(" + StatisticsFieldName + ",2) else 0 end) as ��������," +
                 "sum(case when BHDJ='2' and substr(lz,1,2)='12' and dl<='112' then round(" + StatisticsFieldName + ",2) else 0 end ) as ��������," +
                 "sum(case when BHDJ='2' and ((substr(lz,1,2)<>'11' and substr(lz,1,2)<>'12') or ((substr(lz,1,2)='11' or substr(lz,1,2)='12') and dl>'112')) then round(" + StatisticsFieldName + ",2) else 0 end) as ��������," +
                 "sum(case when BHDJ ='3' and dl<='112' then round(" + StatisticsFieldName + ",2) else 0 end) as ����С��," +
                 "sum(case when BHDJ='3' and substr(lz,1,2)='11' and dl<='112' then round(" + StatisticsFieldName + ",2) else 0 end) as ��������," +
                 "sum(case when BHDJ='3' and substr(lz,1,2)='12' and dl<='112'then round(" + StatisticsFieldName + ",2) else 0 end) as ��������," +
                 "sum(case when BHDJ='3' and ((substr(lz,1,2)<>'11' and substr(lz,1,2)<>'12') or ((substr(lz,1,2)='11' or substr(lz,1,2)='12') and dl>'112')) then round(" + StatisticsFieldName + ",2) else 0 end) as  ��������" +
                 "  from " + tableName +
                 "  where substr(QI_YUAN,1,1)<>' ' and QI_YUAN is not null and sq='1'" +
                 "  group by substr(shi,1,4),rollup(substr(QI_YUAN,1,1))" +
                 "  order by substr(QI_YUAN,1,1) ";
                pWorkspace.ExecuteSQL(SHISQL); 
                //��ѯʡ������ ygc 2012-10-24
                string SHENGSQL = "insert into EightTable_GJGYHDJ select " +
                 "substr(sheng,1,2) as ͳ�Ƶ�λ,substr(QI_YUAN,1,1) as ��Դ," +
                 "sum(case when BHDJ between '1' and '3' and dl<='112' then round(" + StatisticsFieldName + ",2) else 0 end) as �ϼ�," +
                 "sum(case when BHDJ between '1' and '3' and substr(lz,1,2)='11' and dl<='112' then round(" + StatisticsFieldName + ",2) else 0 end) as �����ֺϼ�," +
                 "sum(case when BHDJ between '1' and '3' and substr(lz,1,2)='12' and dl<='112' then round(" + StatisticsFieldName + ",2) else 0 end) as �����ֺϼ�," +
                 "sum(case when BHDJ between '1' and '3' and ((substr(lz,1,2)<>'11' and substr(lz,1,2)<>'12') or ((substr(lz,1,2)='11' or substr(lz,1,2)='12') and dl>'112')) then round(" + StatisticsFieldName + ",2) else 0 end) as �����ϼ�," +
                 "sum(case when BHDJ ='1' and dl<='112' then round(" + StatisticsFieldName + ",2) else 0 end) as һ��С��," +
                 "sum(case when BHDJ='1' and substr(lz,1,2)='11' and dl<='112' then round(" + StatisticsFieldName + ",2) else 0 end ) as һ������," +
                 "sum(case when BHDJ='1' and substr(lz,1,2)='12' and dl<='112' then round(" + StatisticsFieldName + ",2) else 0 end) as һ������," +
                 "sum(case when BHDJ='1' and ((substr(lz,1,2)<>'11' and substr(lz,1,2)<>'12') or ((substr(lz,1,2)='11' or substr(lz,1,2)='12') and dl>'112')) then round(" + StatisticsFieldName + ",2) else 0 end) as һ������," +
                 "sum(case when  BHDJ ='2' and dl<='112' then round(" + StatisticsFieldName + ",2) else 0 end) as ����С��," +
                 "sum(case when BHDJ='2' and substr(lz,1,2)='11' and dl<='112' then round(" + StatisticsFieldName + ",2) else 0 end) as ��������," +
                 "sum(case when BHDJ='2' and substr(lz,1,2)='12' and dl<='112' then round(" + StatisticsFieldName + ",2) else 0 end ) as ��������," +
                 "sum(case when BHDJ='2' and ((substr(lz,1,2)<>'11' and substr(lz,1,2)<>'12') or ((substr(lz,1,2)='11' or substr(lz,1,2)='12') and dl>'112')) then round(" + StatisticsFieldName + ",2) else 0 end) as ��������," +
                 "sum(case when BHDJ ='3' and dl<='112' then round(" + StatisticsFieldName + ",2) else 0 end) as ����С��," +
                 "sum(case when BHDJ='3' and substr(lz,1,2)='11' and dl<='112' then round(" + StatisticsFieldName + ",2) else 0 end) as ��������," +
                 "sum(case when BHDJ='3' and substr(lz,1,2)='12' and dl<='112'then round(" + StatisticsFieldName + ",2) else 0 end) as ��������," +
                 "sum(case when BHDJ='3' and ((substr(lz,1,2)<>'11' and substr(lz,1,2)<>'12') or ((substr(lz,1,2)='11' or substr(lz,1,2)='12') and dl>'112')) then round(" + StatisticsFieldName + ",2) else 0 end) as  ��������" +
                 "  from " + tableName +
                 "  where substr(QI_YUAN,1,1)<>' ' and QI_YUAN is not null and sq='1'" +
                 "  group by substr(sheng,1,2),rollup(substr(QI_YUAN,1,1))" +
                 "  order by substr(QI_YUAN,1,1) ";
                pWorkspace.ExecuteSQL(SHENGSQL); 

                //���ºϼ�
                pWorkspace.ExecuteSQL("update EightTable_GJGYHDJ set �ϼ�= �����ֺϼ�+�����ֺϼ�+�����ϼ�");
                //����һ���ϼ�
                pWorkspace.ExecuteSQL("update EightTable_GJGYHDJ set һ��С��=һ������+һ������+һ������ ");
                //���¶���С��
                pWorkspace.ExecuteSQL("update EightTable_GJGYHDJ set ����С��=��������+��������+��������");
                //��������С��
                pWorkspace.ExecuteSQL("update EightTable_GJGYHDJ set ����С��=��������+��������+��������");

            }
            catch (Exception ex)
            {
                return false;
            }
            return true ;
        }
        private static bool DoLCGJGYHDJ_Statistic(IFeatureClass pFeatureClass, string StatisticsFieldName)
        {
            if (pFeatureClass == null)
            {
                return false;
            }
            IWorkspace pWorkspace = null;
            string tableName = "";
            try
            {
                pWorkspace = pFeatureClass.FeatureDataset.Workspace;
                tableName = (pFeatureClass as IDataset).Name;
            }
            catch (Exception ex)
            { }
            try
            {
                if (!ExistTable(pWorkspace, "EightTable_GJGYHDJ"))
                {
                    //����SQL����ѯͳ���������� ygc 2012-8-23
                    string townsSQL = "create table EightTable_GJGYHDJ as select " +
                                     "lc as ͳ�Ƶ�λ,substr(QI_YUAN,1,1) as ��Դ," +
                                     "sum(case when BHDJ between '1' and '3' and dl<='112' then round(" + StatisticsFieldName + ",2) else 0 end) as �ϼ�," +
                                     "sum(case when BHDJ between '1' and '3' and substr(lz,1,2)='11' and dl<='112' then round(" + StatisticsFieldName + ",2) else 0 end) as �����ֺϼ�," +
                                     "sum(case when BHDJ between '1' and '3' and substr(lz,1,2)='12' and dl<='112' then round(" + StatisticsFieldName + ",2) else 0 end) as �����ֺϼ�," +
                                     "sum(case when BHDJ between '1' and '3' and ((substr(lz,1,2)<>'11' and substr(lz,1,2)<>'12') or ((substr(lz,1,2)='11' or substr(lz,1,2)='12') and dl>'112')) then round(" + StatisticsFieldName + ",2) else 0 end) as �����ϼ�," +
                                     "sum(case when BHDJ ='1' and dl<='112' then round(" + StatisticsFieldName + ",2) else 0 end) as һ��С��," +
                                     "sum(case when BHDJ='1' and substr(lz,1,2)='11' and dl<='112' then round(" + StatisticsFieldName + ",2) else 0 end ) as һ������," +
                                     "sum(case when BHDJ='1' and substr(lz,1,2)='12' and dl<='112' then round(" + StatisticsFieldName + ",2) else 0 end) as һ������," +
                                     "sum(case when BHDJ='1' and ((substr(lz,1,2)<>'11' and substr(lz,1,2)<>'12') or ((substr(lz,1,2)='11' or substr(lz,1,2)='12') and dl>'112')) then round(" + StatisticsFieldName + ",2) else 0 end) as һ������," +
                                     "sum(case when  BHDJ ='2' and dl<='112' then round(" + StatisticsFieldName + ",2) else 0 end) as ����С��," +
                                     "sum(case when BHDJ='2' and substr(lz,1,2)='11' and dl<='112' then round(" + StatisticsFieldName + ",2) else 0 end) as ��������," +
                                     "sum(case when BHDJ='2' and substr(lz,1,2)='12' and dl<='112' then round(" + StatisticsFieldName + ",2) else 0 end ) as ��������," +
                                     "sum(case when BHDJ='2' and ((substr(lz,1,2)<>'11' and substr(lz,1,2)<>'12') or ((substr(lz,1,2)='11' or substr(lz,1,2)='12') and dl>'112')) then round(" + StatisticsFieldName + ",2) else 0 end) as ��������," +
                                     "sum(case when BHDJ ='3' and dl<='112' then round(" + StatisticsFieldName + ",2) else 0 end) as ����С��," +
                                     "sum(case when BHDJ='3' and substr(lz,1,2)='11' and dl<='112' then round(" + StatisticsFieldName + ",2) else 0 end) as ��������," +
                                     "sum(case when BHDJ='3' and substr(lz,1,2)='12' and dl<='112'then round(" + StatisticsFieldName + ",2) else 0 end) as ��������," +
                                     "sum(case when BHDJ='3' and ((substr(lz,1,2)<>'11' and substr(lz,1,2)<>'12') or ((substr(lz,1,2)='11' or substr(lz,1,2)='12') and dl>'112')) then round(" + StatisticsFieldName + ",2) else 0 end) as  ��������" +
                                     "  from " + tableName +
                                     "  where substr(QI_YUAN,1,1)<>' ' and QI_YUAN is not null and sq='1' and lc<>' '" +
                                     "  group by lc,rollup(substr(QI_YUAN,1,1))" +
                                     "  order by substr(QI_YUAN,1,1) ";
                    pWorkspace.ExecuteSQL(townsSQL);
                    pWorkspace.ExecuteSQL("alter table EightTable_GJGYHDJ modify ͳ�Ƶ�λ nvarchar2(20)");

                }
                else 
                {
                    //ͨ��SQL������ؼ�ͳ������ ygc 2012-8-23
                    string CitySQL = "insert into EightTable_GJGYHDJ select " +
                                     "lc as ͳ�Ƶ�λ,substr(QI_YUAN,1,1) as ��Դ," +
                                     "sum(case when BHDJ between '1' and '3' and dl<='112' then round(" + StatisticsFieldName + ",2) else 0 end) as �ϼ�," +
                                     "sum(case when BHDJ between '1' and '3' and substr(lz,1,2)='11' and dl<='112' then round(" + StatisticsFieldName + ",2) else 0 end) as �����ֺϼ�," +
                                     "sum(case when BHDJ between '1' and '3' and substr(lz,1,2)='12' and dl<='112' then round(" + StatisticsFieldName + ",2) else 0 end) as �����ֺϼ�," +
                                     "sum(case when BHDJ between '1' and '3' and ((substr(lz,1,2)<>'11' and substr(lz,1,2)<>'12') or ((substr(lz,1,2)='11' or substr(lz,1,2)='12') and dl>'112')) then round(" + StatisticsFieldName + ",2) else 0 end) as �����ϼ�," +
                                     "sum(case when BHDJ ='1' and dl<='112' then round(" + StatisticsFieldName + ",2) else 0 end) as һ��С��," +
                                     "sum(case when BHDJ='1' and substr(lz,1,2)='11' and dl<='112' then round(" + StatisticsFieldName + ",2) else 0 end ) as һ������," +
                                     "sum(case when BHDJ='1' and substr(lz,1,2)='12' and dl<='112' then round(" + StatisticsFieldName + ",2) else 0 end) as һ������," +
                                     "sum(case when BHDJ='1' and ((substr(lz,1,2)<>'11' and substr(lz,1,2)<>'12') or ((substr(lz,1,2)='11' or substr(lz,1,2)='12') and dl>'112')) then round(" + StatisticsFieldName + ",2) else 0 end) as һ������," +
                                     "sum(case when BHDJ ='2' and dl<='112' then round(" + StatisticsFieldName + ",2) else 0 end) as ����С��," +
                                     "sum(case when BHDJ='2' and substr(lz,1,2)='11' and dl<='112' then round(" + StatisticsFieldName + ",2) else 0 end) as ��������," +
                                     "sum(case when BHDJ='2' and substr(lz,1,2)='12' and dl<='112' then round(" + StatisticsFieldName + ",2) else 0 end ) as ��������," +
                                     "sum(case when BHDJ='2' and ((substr(lz,1,2)<>'11' and substr(lz,1,2)<>'12') or ((substr(lz,1,2)='11' or substr(lz,1,2)='12') and dl>'112')) then round(" + StatisticsFieldName + ",2) else 0 end) as ��������," +
                                     "sum(case when BHDJ ='3' and dl<='112' then round(" + StatisticsFieldName + ",2) else 0 end) as ����С��," +
                                     "sum(case when BHDJ='3' and substr(lz,1,2)='11' and dl<='112' then round(" + StatisticsFieldName + ",2) else 0 end) as ��������," +
                                     "sum(case when BHDJ='3' and substr(lz,1,2)='12' and dl<='112'then round(" + StatisticsFieldName + ",2) else 0 end) as ��������," +
                                     "sum(case when BHDJ='3' and ((substr(lz,1,2)<>'11' and substr(lz,1,2)<>'12') or ((substr(lz,1,2)='11' or substr(lz,1,2)='12') and dl>'112')) then round(" + StatisticsFieldName + ",2) else 0 end) as  ��������" +
                                     "  from " + tableName +
                                     "  where substr(QI_YUAN,1,1)<>' ' and QI_YUAN is not null and sq='1' and lc<>' '" +
                                     "  group by lc,rollup(substr(QI_YUAN,1,1))" +
                                     "  order by substr(QI_YUAN,1,1) ";
                    pWorkspace.ExecuteSQL(CitySQL);
                }

                
                //���ºϼ�
                pWorkspace.ExecuteSQL("update EightTable_GJGYHDJ set �ϼ�= �����ֺϼ�+�����ֺϼ�+�����ϼ�");
                //����һ���ϼ�
                pWorkspace.ExecuteSQL("update EightTable_GJGYHDJ set һ��С��=һ������+һ������+һ������ ");
                //���¶���С��
                pWorkspace.ExecuteSQL("update EightTable_GJGYHDJ set ����С��=��������+��������+��������");
                //��������С��
                pWorkspace.ExecuteSQL("update EightTable_GJGYHDJ set ����С��=��������+��������+��������");

                pWorkspace.ExecuteSQL("alter table EightTable_GJGYHDJ modify ��Դ nvarchar2(10)");
                UpdateStatistictable(pWorkspace, "EightTable_GJGYHDJ", "��Դ", "��Ȼ", "1");
                UpdateStatistictable(pWorkspace, "EightTable_GJGYHDJ", "��Դ", "�˹�", "2");
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

       private static bool DoLDBHDJ_Statistic(IFeatureClass pFeatureClass, string StatisticsFieldName)
       {
           if (pFeatureClass == null)
           {
               return false;
           }
           IWorkspace pWorkspace = null;
           string tableName = "";
           try
           {
               pWorkspace = pFeatureClass.FeatureDataset.Workspace;
               tableName = (pFeatureClass as IDataset).Name;
           }
           catch (Exception ex)
           { }
           DropTable(pWorkspace, "EightTable_LDBHDJTable");
           try
           {
               //��ѯ�缶ͳ������SQL���
               //ygc 2012-8-22
               string townsSQL = "create table EightTable_LDBHDJTable as select substr(" +
                                 tableName + ".xiang,1,8) as ͳ�Ƶ�λ," +
                                "sum(case when (BCLD<>'2' or bcld is null)and bh_dj between '1'and '4' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end ) as ��״�ϼ�," +
                                "sum(case when  (BCLD<>'2' or bcld is null) AND BH_DJ='1' then  round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as ��״1��," +
                                "sum(case when  (BCLD<>'2' or bcld is null) AND BH_DJ='2'then  round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as ��״2��," +
                                "sum(case when  (BCLD<>'2' or bcld is null) AND BH_DJ='3'then  round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as ��״3��," +
                                "sum(case when  (BCLD<>'2' or bcld is null) AND BH_DJ='4'then  round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as ��״4��," +
                                "sum(case when  LB_GHBHDJ between '1'and '4'then round(" + tableName + "." + StatisticsFieldName + ",2) end) as �滮�ϼ�," +
                                "sum(case when  LB_GHBHDJ='1' then  round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as �滮1��," +
                                "sum(case when  LB_GHBHDJ='2' then  round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as �滮2��," +
                                "sum(case when  LB_GHBHDJ='3' then  round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as �滮3��," +
                                "sum(case when  LB_GHBHDJ='4' then  round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as �滮4��" +
                                " from " + tableName +
                                " group by substr(xiang,1,8)";
               pWorkspace.ExecuteSQL(townsSQL);
               pWorkspace.ExecuteSQL("alter table EightTable_LDBHDJTable modify ͳ�Ƶ�λ nvarchar2(20)");
               //��ѯ�ؼ�ͳ������SQL���
               //ygc 2012-8-22
               string CitySQL = "insert into EightTable_LDBHDJTable select " +
                                 tableName + ".xian as ͳ�Ƶ�λ," +
                                "sum(case when (BCLD<>'2' or bcld is null)and  bh_dj between '1'and '4' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end ) as ��״�ϼ�," +
                                "sum(case when  (BCLD<>'2' or bcld is null) AND BH_DJ='1' then  round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as ��״1��," +
                                "sum(case when  (BCLD<>'2' or bcld is null) AND BH_DJ='2'then  round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as ��״2��," +
                                "sum(case when  (BCLD<>'2' or bcld is null) AND BH_DJ='3'then  round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as ��״3��," +
                                "sum(case when  (BCLD<>'2' or bcld is null) AND BH_DJ='4'then  round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as ��״4��," +
                                "sum(case when  LB_GHBHDJ between '1'and '4'then round(" + tableName + "." + StatisticsFieldName + ",2) end) as �滮�ϼ�," +
                                "sum(case when  LB_GHBHDJ='1' then  round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as �滮1��," +
                                "sum(case when  LB_GHBHDJ='2' then  round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as �滮2��," +
                                "sum(case when  LB_GHBHDJ='3' then  round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as �滮3��," +
                                "sum(case when  LB_GHBHDJ='4' then  round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as �滮4��" +
                                " from " + tableName +
                                " group by xian";
               pWorkspace.ExecuteSQL(CitySQL);
               
               //��ѯ�м����� ygc 2012-10-22
               string SHISQL = "insert into EightTable_LDBHDJTable select  substr(" +
                  tableName + ".shi,1,4) as ͳ�Ƶ�λ," +
                 "sum(case when (BCLD<>'2' or bcld is null)and bh_dj between '1'and '4' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end ) as ��״�ϼ�," +
                 "sum(case when  (BCLD<>'2' or bcld is null) AND BH_DJ='1' then  round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as ��״1��," +
                 "sum(case when  (BCLD<>'2' or bcld is null) AND BH_DJ='2'then  round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as ��״2��," +
                 "sum(case when  (BCLD<>'2' or bcld is null) AND BH_DJ='3'then  round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as ��״3��," +
                 "sum(case when  (BCLD<>'2' or bcld is null) AND BH_DJ='4'then  round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as ��״4��," +
                 "sum(case when  LB_GHBHDJ between '1'and '4'then round(" + tableName + "." + StatisticsFieldName + ",2) end) as �滮�ϼ�," +
                 "sum(case when  LB_GHBHDJ='1' then  round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as �滮1��," +
                 "sum(case when  LB_GHBHDJ='2' then  round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as �滮2��," +
                 "sum(case when  LB_GHBHDJ='3' then  round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as �滮3��," +
                 "sum(case when  LB_GHBHDJ='4' then  round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as �滮4��" +
                 " from " + tableName +
                 " group by substr(shi,1,4)";
               pWorkspace.ExecuteSQL(SHISQL);
               //��ѯʡ������ ygc 2012-10-24
               string SHENGSQL = "insert into EightTable_LDBHDJTable select  substr(" +
                  tableName + ".sheng,1,2) as ͳ�Ƶ�λ," +
                 "sum(case when (BCLD<>'2' or bcld is null)and bh_dj between '1'and '4' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end ) as ��״�ϼ�," +
                 "sum(case when  (BCLD<>'2' or bcld is null) AND BH_DJ='1' then  round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as ��״1��," +
                 "sum(case when  (BCLD<>'2' or bcld is null) AND BH_DJ='2'then  round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as ��״2��," +
                 "sum(case when  (BCLD<>'2' or bcld is null) AND BH_DJ='3'then  round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as ��״3��," +
                 "sum(case when  (BCLD<>'2' or bcld is null) AND BH_DJ='4'then  round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as ��״4��," +
                 "sum(case when  LB_GHBHDJ between '1'and '4'then round(" + tableName + "." + StatisticsFieldName + ",2) end) as �滮�ϼ�," +
                 "sum(case when  LB_GHBHDJ='1' then  round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as �滮1��," +
                 "sum(case when  LB_GHBHDJ='2' then  round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as �滮2��," +
                 "sum(case when  LB_GHBHDJ='3' then  round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as �滮3��," +
                 "sum(case when  LB_GHBHDJ='4' then  round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as �滮4��" +
                 " from " + tableName +
                 " group by substr(sheng,1,2)";
               pWorkspace.ExecuteSQL(SHENGSQL);
           }
           catch (Exception ex)
           {
               return false;
           }
           return true ;
       }
       private static bool DoLCLDBHDJ_Statistic(IFeatureClass pFeatureClass, string StatisticsFieldName)
       {
           if (pFeatureClass == null)
           {
               return false;
           }
           IWorkspace pWorkspace = null;
           string tableName = "";
           try
           {
               pWorkspace = pFeatureClass.FeatureDataset.Workspace;
               tableName = (pFeatureClass as IDataset).Name;
           }
           catch (Exception ex)
           { }
           try
           {
               if (!ExistTable(pWorkspace, "EightTable_LDBHDJTable"))
               {
                   //��ѯ�ؼ�ͳ������SQL���
                   //ygc 2012-8-22
                   string CitySQL = "create table EightTable_LDBHDJTable as select  " +
                                     tableName + ".lc as ͳ�Ƶ�λ," +
                                    "sum(case when (BCLD<>'2' or bcld is null)and  bh_dj between '1'and '4' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end ) as ��״�ϼ�," +
                                    "sum(case when  (BCLD<>'2' or bcld is null) AND BH_DJ='1' then  round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as ��״1��," +
                                    "sum(case when  (BCLD<>'2' or bcld is null) AND BH_DJ='2'then  round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as ��״2��," +
                                    "sum(case when  (BCLD<>'2' or bcld is null) AND BH_DJ='3'then  round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as ��״3��," +
                                    "sum(case when  (BCLD<>'2' or bcld is null) AND BH_DJ='4'then  round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as ��״4��," +
                                    "sum(case when  LB_GHBHDJ between '1'and '4'then round(" + tableName + "." + StatisticsFieldName + ",2) end) as �滮�ϼ�," +
                                    "sum(case when  LB_GHBHDJ='1' then  round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as �滮1��," +
                                    "sum(case when  LB_GHBHDJ='2' then  round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as �滮2��," +
                                    "sum(case when  LB_GHBHDJ='3' then  round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as �滮3��," +
                                    "sum(case when  LB_GHBHDJ='4' then  round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as �滮4��" +
                                    " from " + tableName +
                                    "  where lc <>' '" +
                                    " group by lc";
                   pWorkspace.ExecuteSQL(CitySQL);
                   pWorkspace.ExecuteSQL("alter table EightTable_LDBHDJTable modify ͳ�Ƶ�λ nvarchar2(20)");
               }
               else
               {

                   //��ѯ�缶ͳ������SQL���
                   //ygc 2012-8-22
                   string townsSQL = "insert into EightTable_LDBHDJTable select  " +
                                     tableName + ".lc as ͳ�Ƶ�λ," +
                                    "sum(case when (BCLD<>'2' or bcld is null)and bh_dj between '1'and '4' then round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end ) as ��״�ϼ�," +
                                    "sum(case when  (BCLD<>'2' or bcld is null) AND BH_DJ='1' then  round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as ��״1��," +
                                    "sum(case when  (BCLD<>'2' or bcld is null) AND BH_DJ='2'then  round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as ��״2��," +
                                    "sum(case when  (BCLD<>'2' or bcld is null) AND BH_DJ='3'then  round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as ��״3��," +
                                    "sum(case when  (BCLD<>'2' or bcld is null) AND BH_DJ='4'then  round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as ��״4��," +
                                    "sum(case when  LB_GHBHDJ between '1'and '4'then round(" + tableName + "." + StatisticsFieldName + ",2) end) as �滮�ϼ�," +
                                    "sum(case when  LB_GHBHDJ='1' then  round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as �滮1��," +
                                    "sum(case when  LB_GHBHDJ='2' then  round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as �滮2��," +
                                    "sum(case when  LB_GHBHDJ='3' then  round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as �滮3��," +
                                    "sum(case when  LB_GHBHDJ='4' then  round(" + tableName + "." + StatisticsFieldName + ",2) else 0 end) as �滮4��" +
                                    " from " + tableName +
                                    " where lc<>' '" +
                                    " group by lc";
                   pWorkspace.ExecuteSQL(townsSQL);
               }
           }
           catch (Exception ex)
           {
               return false;
           }
           return true;
       }
       //ͨ��SQL ����ù��Ҽ������ֵع滮���ͳ�Ʊ� ygc 2012-8-27
       private static bool DoGJGYLGHMJ_Statistic(IFeatureClass pFeatureClass, string StatisticsFieldName)
       {

           if (pFeatureClass == null)
           {
               return false;
           }
           IWorkspace pWorkspace = null;
           string tableName = "";
           try
           {
               pWorkspace = pFeatureClass.FeatureDataset.Workspace;
               tableName = (pFeatureClass as IDataset).Name;
           }
           catch (Exception ex)
           { }
           DropTable(pWorkspace, "EightTable_GJGYLGHMJ");
           DropTable(pWorkspace, "temp_table_1");
           DropTable(pWorkspace, "temp_table_2");
           try
           {
               //������ʱ�� ygc 2012-8-27
               pWorkspace.ExecuteSQL("create table temp_table_1 as select xiang,xian,shi,sheng,lz,ghlz,SQ,GHSQ,BHDJ,sum(" + StatisticsFieldName + ") as mj from  " + tableName + " group by xiang,xian,shi,sheng,lz,ghlz,SQ,GHSQ,BHDJ");
               pWorkspace.ExecuteSQL("create table temp_table_2 as select * from  temp_table_1");
               pWorkspace.ExecuteSQL("alter table temp_table_1 modify lz nvarchar2(20)");
               pWorkspace.ExecuteSQL("alter table temp_table_2 modify lz nvarchar2(20)");
               //������ʱ�� ygc 2012-8-27 
               pWorkspace.ExecuteSQL("update temp_table_1 set lz='2������' where substr(lz,1,2)='11'");
               pWorkspace.ExecuteSQL("update temp_table_1 set lz='1������' where substr(lz,1,2)='12'");
               pWorkspace.ExecuteSQL("update temp_table_2 set lz='4�����ֵ�' where substr(lz,1,2)<>'11' and substr(lz,1,2)<>'12' and substr(lz,1,2)<>' '");
               pWorkspace.ExecuteSQL("update temp_table_2 set lz='3������' where (substr(lz,1,2)='11' or substr(lz,1,2)='12')");

               //��SQL����ȡ�缶������ͳ������
               string townsSQL = " create table EightTable_GJGYLGHMJ as (select " +
                                "substr(temp_table_1.xiang,1,8) as ͳ�Ƶ�λ,lz as ����," +
                                "sum(case when SQ='1' AND BHDJ between '1' and '3' then round(mj,2) else 0 end) as ��״�ϼ�," +
                                "sum(case when SQ='1' AND BHDJ='1' then round(mj,2) else 0 end) as ��״һ��," +
                                "sum(case when SQ='1' AND BHDJ='2' then round(mj,2) else 0 end) as ��״����," +
                                "sum(case when SQ='1' AND BHDJ='3' then round(mj,2) else 0 end) as ��״����," +
                                "sum(case when GHSQ='1' AND BHDJ between '1' and '3' then round(mj,2) else 0 end) as �滮�ϼ�," +
                                "sum(case when GHSQ='1' AND BHDJ='1' then round(mj,2) else 0 end) as �滮һ��," +
                                "sum(case when GHSQ='1' AND BHDJ='2'then round(mj,2) else 0 end) as �滮����," +
                                "sum(case when GHSQ='1' AND BHDJ='3' then round(mj,2) else 0 end) as �滮����" +
                                " from temp_table_1" +
                                " where  ( lz<>' 'and ghlz<>' ') and (lz='1������' or lz='2������')" +
                                " group by substr(xiang,1,8),lz" +
                                " union all" +
                                " select " +
                                " substr(xiang,1,8) as ͳ�Ƶ�λ,lz as ����," +
                                "sum(case when SQ='1' AND BHDJ between '1' and '3' then round(mj,2) else 0 end) as ��״�ϼ�," +
                                "sum(case when SQ='1' AND BHDJ='1' then round(mj,2) else 0 end) as ��״һ��," +
                                "sum(case when SQ='1' AND BHDJ='2' then round(mj,2) else 0 end) as ��״����," +
                                "sum(case when SQ='1' AND BHDJ='3' then round(mj,2) else 0 end) as ��״����," +
                                "sum(case when GHSQ='1' AND BHDJ between '1' and '3' then round(mj,2) else 0 end) as �滮�ϼ�," +
                                "sum(case when GHSQ='1' AND BHDJ='1' then round(mj,2) else 0 end) as �滮һ��," +
                                "sum(case when GHSQ='1' AND BHDJ='2'then round(mj,2) else 0 end) as �滮����," +
                                "sum(case when GHSQ='1' AND BHDJ='3' then round(mj,2) else 0 end) as �滮����" +
                                " from temp_table_2" +
                                " where  ( lz<>' 'and ghlz<>' ')" +
                                " group by substr(xiang,1,8),rollup(lz))";
               pWorkspace.ExecuteSQL(townsSQL);
               pWorkspace.ExecuteSQL("alter table EightTable_GJGYLGHMJ modify ͳ�Ƶ�λ nvarchar2(20)");
               //ͨ��SQL����ؼ�ͳ������ ygc 2012-9-27 
               string CitySQL = " insert  into EightTable_GJGYLGHMJ  (select " +
                                "temp_table_1.xian as ͳ�Ƶ�λ,lz as ����," +
                                "sum(case when SQ='1' AND BHDJ between '1' and '3' then round(mj,2) else 0 end) as ��״�ϼ�," +
                                "sum(case when SQ='1' AND BHDJ='1' then round(mj,2) else 0 end) as ��״һ��," +
                                "sum(case when SQ='1' AND BHDJ='2' then round(mj,2) else 0 end) as ��״����," +
                                "sum(case when SQ='1' AND BHDJ='3' then round(mj,2) else 0 end) as ��״����," +
                                "sum(case when GHSQ='1' AND BHDJ between '1' and '3' then round(mj,2) else 0 end) as �滮�ϼ�," +
                                "sum(case when GHSQ='1' AND BHDJ='1' then round(mj,2) else 0 end) as �滮һ��," +
                                "sum(case when GHSQ='1' AND BHDJ='2'then round(mj,2) else 0 end) as �滮����," +
                                "sum(case when GHSQ='1' AND BHDJ='3' then round(mj,2) else 0 end) as �滮����" +
                                " from temp_table_1" +
                                " where  ( lz<>' 'and ghlz<>' ') and (lz='1������' or lz='2������')" +
                                " group by xian,lz" +
                                " union all" +
                                " select " +
                                " xian as ͳ�Ƶ�λ,lz as ����," +
                                "sum(case when SQ='1' AND BHDJ between '1' and '3' then round(mj,2) else 0 end) as ��״�ϼ�," +
                                "sum(case when SQ='1' AND BHDJ='1' then round(mj,2) else 0 end) as ��״һ��," +
                                "sum(case when SQ='1' AND BHDJ='2' then round(mj,2) else 0 end) as ��״����," +
                                "sum(case when SQ='1' AND BHDJ='3' then round(mj,2) else 0 end) as ��״����," +
                                "sum(case when GHSQ='1' AND BHDJ between '1' and '3' then round(mj,2) else 0 end) as �滮�ϼ�," +
                                "sum(case when GHSQ='1' AND BHDJ='1' then round(mj,2) else 0 end) as �滮һ��," +
                                "sum(case when GHSQ='1' AND BHDJ='2'then round(mj,2) else 0 end) as �滮����," +
                                "sum(case when GHSQ='1' AND BHDJ='3' then round(mj,2) else 0 end) as �滮����" +
                                " from temp_table_2" +
                                " where  ( lz<>' 'and ghlz<>' ')" +
                                " group by xian,rollup(lz))";
               pWorkspace.ExecuteSQL(CitySQL);
               
               //��ȡ�м�����a
               string SHISQL = " insert  into EightTable_GJGYLGHMJ  (select " +
                 "substr(temp_table_1.shi,1,4) as ͳ�Ƶ�λ,lz as ����," +
                 "sum(case when SQ='1' AND BHDJ between '1' and '3' then round(mj,2) else 0 end) as ��״�ϼ�," +
                 "sum(case when SQ='1' AND BHDJ='1' then round(mj,2) else 0 end) as ��״һ��," +
                 "sum(case when SQ='1' AND BHDJ='2' then round(mj,2) else 0 end) as ��״����," +
                 "sum(case when SQ='1' AND BHDJ='3' then round(mj,2) else 0 end) as ��״����," +
                 "sum(case when GHSQ='1' AND BHDJ between '1' and '3' then round(mj,2) else 0 end) as �滮�ϼ�," +
                 "sum(case when GHSQ='1' AND BHDJ='1' then round(mj,2) else 0 end) as �滮һ��," +
                 "sum(case when GHSQ='1' AND BHDJ='2'then round(mj,2) else 0 end) as �滮����," +
                 "sum(case when GHSQ='1' AND BHDJ='3' then round(mj,2) else 0 end) as �滮����" +
                 " from temp_table_1" +
                 " where  ( lz<>' 'and ghlz<>' ') and (lz='1������' or lz='2������')" +
                 " group by substr(shi,1,4),lz" +
                 " union all" +
                 " select " +
                 " substr(shi,1,4) as ͳ�Ƶ�λ,lz as ����," +
                 "sum(case when SQ='1' AND BHDJ between '1' and '3' then round(mj,2) else 0 end) as ��״�ϼ�," +
                 "sum(case when SQ='1' AND BHDJ='1' then round(mj,2) else 0 end) as ��״һ��," +
                 "sum(case when SQ='1' AND BHDJ='2' then round(mj,2) else 0 end) as ��״����," +
                 "sum(case when SQ='1' AND BHDJ='3' then round(mj,2) else 0 end) as ��״����," +
                 "sum(case when GHSQ='1' AND BHDJ between '1' and '3' then round(mj,2) else 0 end) as �滮�ϼ�," +
                 "sum(case when GHSQ='1' AND BHDJ='1' then round(mj,2) else 0 end) as �滮һ��," +
                 "sum(case when GHSQ='1' AND BHDJ='2'then round(mj,2) else 0 end) as �滮����," +
                 "sum(case when GHSQ='1' AND BHDJ='3' then round(mj,2) else 0 end) as �滮����" +
                 " from temp_table_2" +
                 " where  ( lz<>' 'and ghlz<>' ')" +
                 " group by substr(shi,1,4),rollup(lz))";
               pWorkspace.ExecuteSQL(SHISQL);
               //��ѯʡ������ ygc 2012-10-24
               string SHENGSQL = " insert  into EightTable_GJGYLGHMJ  (select " +
                 "substr(temp_table_1.sheng,1,2) as ͳ�Ƶ�λ,lz as ����," +
                 "sum(case when SQ='1' AND BHDJ between '1' and '3' then round(mj,2) else 0 end) as ��״�ϼ�," +
                 "sum(case when SQ='1' AND BHDJ='1' then round(mj,2) else 0 end) as ��״һ��," +
                 "sum(case when SQ='1' AND BHDJ='2' then round(mj,2) else 0 end) as ��״����," +
                 "sum(case when SQ='1' AND BHDJ='3' then round(mj,2) else 0 end) as ��״����," +
                 "sum(case when GHSQ='1' AND BHDJ between '1' and '3' then round(mj,2) else 0 end) as �滮�ϼ�," +
                 "sum(case when GHSQ='1' AND BHDJ='1' then round(mj,2) else 0 end) as �滮һ��," +
                 "sum(case when GHSQ='1' AND BHDJ='2'then round(mj,2) else 0 end) as �滮����," +
                 "sum(case when GHSQ='1' AND BHDJ='3' then round(mj,2) else 0 end) as �滮����" +
                 " from temp_table_1" +
                 " where  ( lz<>' 'and ghlz<>' ') and (lz='1������' or lz='2������')" +
                 " group by substr(sheng,1,2),lz" +
                 " union all" +
                 " select " +
                 " substr(sheng,1,2) as ͳ�Ƶ�λ,lz as ����," +
                 "sum(case when SQ='1' AND BHDJ between '1' and '3' then round(mj,2) else 0 end) as ��״�ϼ�," +
                 "sum(case when SQ='1' AND BHDJ='1' then round(mj,2) else 0 end) as ��״һ��," +
                 "sum(case when SQ='1' AND BHDJ='2' then round(mj,2) else 0 end) as ��״����," +
                 "sum(case when SQ='1' AND BHDJ='3' then round(mj,2) else 0 end) as ��״����," +
                 "sum(case when GHSQ='1' AND BHDJ between '1' and '3' then round(mj,2) else 0 end) as �滮�ϼ�," +
                 "sum(case when GHSQ='1' AND BHDJ='1' then round(mj,2) else 0 end) as �滮һ��," +
                 "sum(case when GHSQ='1' AND BHDJ='2'then round(mj,2) else 0 end) as �滮����," +
                 "sum(case when GHSQ='1' AND BHDJ='3' then round(mj,2) else 0 end) as �滮����" +
                 " from temp_table_2" +
                 " where  ( lz<>' 'and ghlz<>' ')" +
                 " group by substr(sheng,1,2),rollup(lz))";
               pWorkspace.ExecuteSQL(SHENGSQL);
               DropTable(pWorkspace, "temp_table_1");
               DropTable(pWorkspace, "temp_table_2");
           }
           catch (Exception ex)
           {
               return false;
           }
           return true;
       }
       private static bool DoLCGJGYLGHMJ_Statistic(IFeatureClass pFeatureClass, string StatisticsFieldName)
       {

           if (pFeatureClass == null)
           {
               return false;
           }
           IWorkspace pWorkspace = null;
           string tableName = "";
           try
           {
               pWorkspace = pFeatureClass.FeatureDataset.Workspace;
               tableName = (pFeatureClass as IDataset).Name;
           }
           catch (Exception ex)
           { }
           DropTable(pWorkspace, "temp_table_1");
           DropTable(pWorkspace, "temp_table_2");
           try
           {
               //������ʱ�� ygc 2012-8-27
               pWorkspace.ExecuteSQL("create table temp_table_1 as select lc,SQ,GHSQ,BHDJ,lz,ghlz,sum(" + StatisticsFieldName + ") as mj from  " + tableName + " group by lc,SQ,GHSQ,BHDJ,lz,ghlz");
               pWorkspace.ExecuteSQL("create table temp_table_2 as select * from  temp_table_1");
               pWorkspace.ExecuteSQL("alter table temp_table_1 modify lz nvarchar2(20)");
               pWorkspace.ExecuteSQL("alter table temp_table_2 modify lz nvarchar2(20)");
               //������ʱ�� ygc 2012-8-27 
               pWorkspace.ExecuteSQL("update temp_table_1 set lz='2������' where substr(lz,1,2)='11'");
               pWorkspace.ExecuteSQL("update temp_table_1 set lz='1������' where substr(lz,1,2)='12'");
               pWorkspace.ExecuteSQL("update temp_table_2 set lz='4�����ֵ�' where substr(lz,1,2)<>'11' and substr(lz,1,2)<>'12' and substr(lz,1,2)<>' '");
               pWorkspace.ExecuteSQL("update temp_table_2 set lz='3������' where (substr(lz,1,2)='11' or substr(lz,1,2)='12')");
               if (!ExistTable(pWorkspace, "EightTable_GJGYLGHMJ"))
               {
                   string CitySQL = " create table EightTable_GJGYLGHMJ as (select " +
                                   "temp_table_1.lc as ͳ�Ƶ�λ,lz as ����," +
                                   "sum(case when SQ='1' AND BHDJ between '1' and '3' then round(mj,2) else 0 end) as ��״�ϼ�," +
                                   "sum(case when SQ='1' AND BHDJ='1' then round(mj,2) else 0 end) as ��״һ��," +
                                   "sum(case when SQ='1' AND BHDJ='2' then round(mj,2) else 0 end) as ��״����," +
                                   "sum(case when SQ='1' AND BHDJ='3' then round(mj,2) else 0 end) as ��״����," +
                                   "sum(case when GHSQ='1' AND BHDJ between '1' and '3' then round(mj,2) else 0 end) as �滮�ϼ�," +
                                   "sum(case when GHSQ='1' AND BHDJ='1' then round(mj,2) else 0 end) as �滮һ��," +
                                   "sum(case when GHSQ='1' AND BHDJ='2'then round(mj,2) else 0 end) as �滮����," +
                                   "sum(case when GHSQ='1' AND BHDJ='3' then round(mj,2) else 0 end) as �滮����" +
                                   " from temp_table_1" +
                                   " where  ( lz<>' 'or ghlz<>' ') and (lz='1������' or lz='2������') and lc<>' '" +
                                   " group by lc,lz" +
                                   " union all" +
                                   " select " +
                                   " lc as ͳ�Ƶ�λ,lz as ����," +
                                   "sum(case when SQ='1' AND BHDJ between '1' and '3' then round(mj,2) else 0 end) as ��״�ϼ�," +
                                   "sum(case when SQ='1' AND BHDJ='1' then round(mj,2) else 0 end) as ��״һ��," +
                                   "sum(case when SQ='1' AND BHDJ='2' then round(mj,2) else 0 end) as ��״����," +
                                   "sum(case when SQ='1' AND BHDJ='3' then round(mj,2) else 0 end) as ��״����," +
                                   "sum(case when GHSQ='1' AND BHDJ between '1' and '3' then round(mj,2) else 0 end) as �滮�ϼ�," +
                                   "sum(case when GHSQ='1' AND BHDJ='1' then round(mj,2) else 0 end) as �滮һ��," +
                                   "sum(case when GHSQ='1' AND BHDJ='2'then round(mj,2) else 0 end) as �滮����," +
                                   "sum(case when GHSQ='1' AND BHDJ='3' then round(mj,2) else 0 end) as �滮����" +
                                   " from temp_table_2" +
                                   " where  ( lz<>' 'or ghlz<>' ') and lc<>' '" +
                                   " group by lc,rollup(lz))";
                   pWorkspace.ExecuteSQL(CitySQL);
                   pWorkspace.ExecuteSQL("alter table EightTable_GJGYLGHMJ modify ͳ�Ƶ�λ nvarchar2(20)");
               }
               else
               {
                   //��SQL����ȡ�缶������ͳ������
                   string townsSQL = " insert  into EightTable_GJGYLGHMJ  (select " +
                                    "temp_table_1.lc as ͳ�Ƶ�λ,lz as ����," +
                                    "sum(case when SQ='1' AND BHDJ between '1' and '3' then round(mj,2) else 0 end) as ��״�ϼ�," +
                                    "sum(case when SQ='1' AND BHDJ='1' then round(mj,2) else 0 end) as ��״һ��," +
                                    "sum(case when SQ='1' AND BHDJ='2' then round(mj,2) else 0 end) as ��״����," +
                                    "sum(case when SQ='1' AND BHDJ='3' then round(mj,2) else 0 end) as ��״����," +
                                    "sum(case when GHSQ='1' AND BHDJ between '1' and '3' then round(mj,2) else 0 end) as �滮�ϼ�," +
                                    "sum(case when GHSQ='1' AND BHDJ='1' then round(mj,2) else 0 end) as �滮һ��," +
                                    "sum(case when GHSQ='1' AND BHDJ='2'then round(mj,2) else 0 end) as �滮����," +
                                    "sum(case when GHSQ='1' AND BHDJ='3' then round(mj,2) else 0 end) as �滮����" +
                                    " from temp_table_1" +
                                    " where  ( lz<>' 'or ghlz<>' ') and (lz='1������' or lz='2������') and lc<>' '" +
                                    " group by lc,lz" +
                                    " union all" +
                                    " select " +
                                    " lc as ͳ�Ƶ�λ,lz as ����," +
                                    "sum(case when SQ='1' AND BHDJ between '1' and '3' then round(mj,2) else 0 end) as ��״�ϼ�," +
                                    "sum(case when SQ='1' AND BHDJ='1' then round(mj,2) else 0 end) as ��״һ��," +
                                    "sum(case when SQ='1' AND BHDJ='2' then round(mj,2) else 0 end) as ��״����," +
                                    "sum(case when SQ='1' AND BHDJ='3' then round(mj,2) else 0 end) as ��״����," +
                                    "sum(case when GHSQ='1' AND BHDJ between '1' and '3' then round(mj,2) else 0 end) as �滮�ϼ�," +
                                    "sum(case when GHSQ='1' AND BHDJ='1' then round(mj,2) else 0 end) as �滮һ��," +
                                    "sum(case when GHSQ='1' AND BHDJ='2'then round(mj,2) else 0 end) as �滮����," +
                                    "sum(case when GHSQ='1' AND BHDJ='3' then round(mj,2) else 0 end) as �滮����" +
                                    " from temp_table_2" +
                                    " where  ( lz<>' 'or ghlz<>' ') and lc<>' '" +
                                    " group by lc,rollup(lz))";
                   pWorkspace.ExecuteSQL(townsSQL);
               }
               DropTable(pWorkspace, "temp_table_1");
               DropTable(pWorkspace, "temp_table_2");
           }
           catch (Exception ex)
           {
               return false;
           }
           return true;
       }
        /// <summary>
        /// ��DataGridView�ؼ������ݵ�����Excel
        /// </summary>
        /// <param name="gridView">DataGridView����</param>
        /// <returns></returns>
        public static bool ExportDataGridview(DataGridViewX gridView,string defaultName)
        {
            Application excel = null;
            Workbook wb = null;
            try
            {
                if (gridView.Rows.Count == 0)
                    return false;
                //����Excel����
                excel = new Microsoft.Office.Interop.Excel.Application();
                wb = excel.Application.Workbooks.Add(true);
                excel.Visible = true;
                wb.Application.ActiveWindow.Caption=defaultName;
                
                //�����ֶ�����
                for (int i = 0; i < gridView.ColumnCount; i++)
                {
                    excel.Cells[1, i + 1] = gridView.Columns[i].HeaderText;
                }
                //�������
                for (int i = 0; i < gridView.RowCount; i++)
                {
                    for (int j = 0; j < gridView.ColumnCount; j++)
                    {
                        if (gridView[j, i].ValueType == typeof(string))
                        {
                            if (gridView[j, i].Value != null)
                            {
                                excel.Cells[i + 2, j + 1] = "'" + gridView[j, i].Value.ToString();
                            }
                        }
                        else
                        {
                            if (gridView[j, i].Value != null)
                            {
                                excel.Cells[i + 2, j + 1] = gridView[j, i].Value.ToString();
                            }
                        }
                    }
                }
                FileDialog fd = wb.Application.get_FileDialog(Microsoft.Office.Core.MsoFileDialogType.msoFileDialogSaveAs);
                fd.InitialFileName = defaultName;
                int result=fd.Show();
                if (result == 0) return true;
                string fileName=fd.InitialFileName;
                if (!string.IsNullOrEmpty(fileName))
                {
                    if (fileName.IndexOf(".xls")==-1)
                    {
                        fileName += ".xls";
                    }
                    wb.SaveAs(fileName, XlFileFormat.xlWorkbookNormal, Type.Missing, Type.Missing, Type.Missing, Type.Missing, XlSaveAsAccessMode.xlNoChange, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// ��DataGridView�ؼ������ݵ�����Excel
        /// </summary>
        /// <param name="gridView">DataGridView����</param>
        /// <returns></returns>
        public static bool ExportDataGridview(DataGridViewX gridView,List<string> lstFields, string defaultName)
        {
            Application excel = null;
            Workbook wb = null;
            try
            {
                if (gridView.Rows.Count == 0)
                    return false;
                //����Excel����
                excel = new Microsoft.Office.Interop.Excel.Application();
                wb = excel.Application.Workbooks.Add(true);
                excel.Visible = true;
                wb.Application.ActiveWindow.Caption = defaultName;

                //�����ֶ�����
                //for (int i = 0; i < gridView.ColumnCount; i++)
                //{
                //    if(!lstFields.Contains(gridView.Columns[i].HeaderText)) continue;
                //    excel.Cells[1, i + 1] = gridView.Columns[i].HeaderText;
                //}

                for (int i = 0; i < lstFields.Count; i++)
                {
                    //if (!lstFields.Contains(gridView.Columns[i].HeaderText)) continue;
                    excel.Cells[1, i + 1] = gridView.Columns[lstFields[i]].HeaderText;
                }

                //�������
                for (int i = 0; i < gridView.RowCount; i++)
                {
                    for (int j = 0; j < lstFields.Count; j++)
                    {
                        //if (!lstFields.Contains(gridView.Columns[j].HeaderText)) continue;
                        int intFieldIndex=gridView.Columns.IndexOf(gridView.Columns[lstFields[j]]);

                        if (gridView[intFieldIndex, i].ValueType == typeof(string))
                        {
                            if (gridView[intFieldIndex, i].Value != null)
                            {
                                excel.Cells[i + 2, j + 1] = "'" + gridView[intFieldIndex, i].Value.ToString();
                            }
                        }
                        else
                        {
                            if (gridView[intFieldIndex, i].Value != null)
                            {
                                excel.Cells[i + 2, j + 1] = gridView[intFieldIndex, i].Value.ToString();
                            }
                        }
                    }
                }
                FileDialog fd = wb.Application.get_FileDialog(Microsoft.Office.Core.MsoFileDialogType.msoFileDialogSaveAs);
                fd.InitialFileName = defaultName;
                int result = fd.Show();
                if (result == 0) return true;
                string fileName = fd.InitialFileName;
                if (!string.IsNullOrEmpty(fileName))
                {
                    if (fileName.IndexOf(".xls") == -1)
                    {
                        fileName += ".xls";
                    }
                    wb.SaveAs(fileName, XlFileFormat.xlWorkbookNormal, Type.Missing, Type.Missing, Type.Missing, Type.Missing, XlSaveAsAccessMode.xlNoChange, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        //�жϱ��Ƿ���� ygc 2012-10-10 
        private static bool ExistTable(IWorkspace pWorkspace, string TableName)
        {
            try
            {
                ITable pTable = (pWorkspace as IFeatureWorkspace).OpenTable(TableName);
                if (pTable != null) return true;
            }
            catch
            {
                return false;
            }
            return true;
        }
        //���ж���ͳ�� ygc 2012-12-6
        public  static void DoEcostatistic(IFeatureClass iFeatureclass, string strMjFieldName, string strXJFielldName,string strZSFieldName,SysCommon.CProgress pProgress)
        {
            pProgress.SetProgress(1, "�����ò������������伶ͳ��");
            bool bRes = false;
            bRes = XZQYCLMJXJ_Statistic(iFeatureclass, strXJFielldName, strMjFieldName);
            bRes = LCYCLMJXJ_Statistic(iFeatureclass, strXJFielldName, strMjFieldName);

            pProgress.SetProgress(2, "��̬�����֣��أ�ͳ��");
            bRes = XZQSTGYL_Statistic(iFeatureclass, strMjFieldName);
            bRes = LCSTGYL_Statistic(iFeatureclass, strMjFieldName);

            pProgress.SetProgress(3, "��ľ��������������ͳ��");
           bRes= XZQQMLMJ_Statistic(iFeatureclass, strXJFielldName, strMjFieldName);
           //bRes = LCQMLMJ_Statistic(iFeatureclass, strXJFielldName, strMjFieldName);

           pProgress.SetProgress(4, "����ͳ��");
           bRes = XZQLZ_Statistic(iFeatureclass, strXJFielldName, strMjFieldName);

           pProgress.SetProgress(5, "������ͳ��");
           bRes = XZQJJL_Statistic(iFeatureclass, strZSFieldName, strMjFieldName);
           //bRes = LCJJL_Statistic(iFeatureclass, strZSFieldName, strMjFieldName);

           pProgress.SetProgress(6, "��ľ��ͳ��");
           bRes = XZQGML_Statistic(iFeatureclass, strMjFieldName);
          // bRes = LCCML_Statistic(iFeatureclass, strMjFieldName);

           pProgress.SetProgress(7, "�����������ͳ��");
           bRes = XZQGLTDMJ_Statistic(iFeatureclass, strMjFieldName);

           pProgress.SetProgress(8, "����ɭ�֡���ľ������ͳ��");
           bRes = XZQGLSLMJ_Statistic(iFeatureclass, strXJFielldName, strZSFieldName, strZSFieldName);

        }
        #region �ò������������伶ͳ��
        //�ò������������伶ͳ�� ygc 2012-12-11
        private static bool XZQYCLMJXJ_Statistic(IFeatureClass pFeatureClass, string strXJFielldName, string StatisticsFieldName)
        {
            if (pFeatureClass == null)
            {
                return false;
            }
            IWorkspace pWorkspace = null;
            string tableName = "";
            try
            {
                pWorkspace = pFeatureClass.FeatureDataset.Workspace;
                tableName = (pFeatureClass as IDataset).Name;
            }
            catch (Exception ex)
            { }
            DropTable(pWorkspace, "EcosTable_YCLMJXJ");
             //ͨ��SQL�����ͳ�ƽ�� ygc 2012-12-11
             try
             {
                 string xianSQL = "create table EcosTable_YCLMJXJ as select xian as ͳ�Ƶ�λ,lmqs as ��ľʹ��Ȩ,lz as ������," +
                                    "sum(case when llz between '1' and '8' then round(" + StatisticsFieldName + ",2) else 0 end) as �ϼ����," +
                                    "sum(case when llz between '1' and '8' then round(xjl,2) else 0 end) as �ϼ����," +
                                    "sum(case when llz='1' then round(" + StatisticsFieldName + ",2) else 0 end) as һ�伶���," +
                                    "sum(case when llz='1' then round(" + strXJFielldName + ",2) else 0 end) as һ�伶���," +
                                    "sum(case when llz='2' then round(" + StatisticsFieldName + ",2) else 0 end ) as ���伶���," +
                                    "sum(case when llz='2' then round(" + strXJFielldName + ",2) else 0 end) as ���伶���," +
                                    "sum(case when llz='3' then round(" + StatisticsFieldName + ",2) else 0 end) as ���伶���," +
                                    "sum(case when llz='3' then round(" + strXJFielldName + ",2) else 0 end) as ���伶���," +
                                    "sum(case when llz='4' then round(" + StatisticsFieldName + ",2) else 0 end) as ���伶���," +
                                    "sum(case when llz='4' then round(" + strXJFielldName + ",2) else 0 end) as ���伶���," +
                                    "sum(case when llz='5' then round(" + StatisticsFieldName + ",2) else 0 end) as ���伶���," +
                                    "sum(case when llz='5' then round(" + strXJFielldName + ",2) else 0 end) as ���伶���," +
                                    "sum(case when llz='6' then round(" + StatisticsFieldName + ",2) else 0 end) as ���伶���," +
                                    "sum(case when llz='6' then round(" + strXJFielldName + ",2) else 0 end) as ���伶���," +
                                    "sum(case when llz='7' then round(" + StatisticsFieldName + ",2) else 0 end) as ���伶���," +
                                    "sum(case when llz='7' then round(" + strXJFielldName + ",2) else 0 end) as ���伶���," +
                                    "sum(case when llz='8' then round(" + strXJFielldName + ",2) else 0 end) as ���伶���," +
                                    "sum(case when llz='8' then round(" + strXJFielldName + ",2) else 0 end) as ���伶���," +
                                    "sum(case when llz not in ('1','2','3','4','5','6','7','8') then round(" + StatisticsFieldName + ",2) else 0 end) as �������伶���," +
                                    "sum(case when llz not in ('1','2','3','4','5','6','7','8') then round(" + strXJFielldName + ",2) else 0 end) as �������伶���" +
                                    "  from " + tableName +
                                    "  where lmqs<> ' ' and lz between '231' and '233'" +
                                    "  group by xian,rollup(lmqs),rollup(lz)";
                 pWorkspace.ExecuteSQL(xianSQL);
                 string xiangSQL = "insert into EcosTable_YCLMJXJ ( select substr(xiang,1,8) as ͳ�Ƶ�λ,lmqs as ��ľʹ��Ȩ,lz as ������," +
                                    "sum(case when llz between '1' and '8' then round(" + StatisticsFieldName + ",2) else 0 end) as �ϼ����," +
                                    "sum(case when llz between '1' and '8' then round(xjl,2) else 0 end) as �ϼ����," +
                                    "sum(case when llz='1' then round(" + StatisticsFieldName + ",2) else 0 end) as һ�伶���," +
                                    "sum(case when llz='1' then round(" + strXJFielldName + ",2) else 0 end) as һ�伶���," +
                                    "sum(case when llz='2' then round(" + StatisticsFieldName + ",2) else 0 end ) as ���伶���," +
                                    "sum(case when llz='2' then round(" + strXJFielldName + ",2) else 0 end) as ���伶���," +
                                    "sum(case when llz='3' then round(" + StatisticsFieldName + ",2) else 0 end) as ���伶���," +
                                    "sum(case when llz='3' then round(" + strXJFielldName + ",2) else 0 end) as ���伶���," +
                                    "sum(case when llz='4' then round(" + StatisticsFieldName + ",2) else 0 end) as ���伶���," +
                                    "sum(case when llz='4' then round(" + strXJFielldName + ",2) else 0 end) as ���伶���," +
                                    "sum(case when llz='5' then round(" + StatisticsFieldName + ",2) else 0 end) as ���伶���," +
                                    "sum(case when llz='5' then round(" + strXJFielldName + ",2) else 0 end) as ���伶���," +
                                    "sum(case when llz='6' then round(" + StatisticsFieldName + ",2) else 0 end) as ���伶���," +
                                    "sum(case when llz='6' then round(" + strXJFielldName + ",2) else 0 end) as ���伶���," +
                                    "sum(case when llz='7' then round(" + StatisticsFieldName + ",2) else 0 end) as ���伶���," +
                                    "sum(case when llz='7' then round(" + strXJFielldName + ",2) else 0 end) as ���伶���," +
                                    "sum(case when llz='8' then round(" + StatisticsFieldName + ",2) else 0 end) as ���伶���," +
                                    "sum(case when llz='8' then round(" + strXJFielldName + ",2) else 0 end) as ���伶���," +
                                    "sum(case when llz not in ('1','2','3','4','5','6','7','8') then round(" + StatisticsFieldName + ",2) else 0 end) as �������伶���," +
                                    "sum(case when llz not in ('1','2','3','4','5','6','7','8') then round(" + strXJFielldName + ",2) else 0 end) as �������伶���" +
                                    "  from " + tableName +
                                    "  where lmqs<> ' ' and lz between '231' and '233'" +
                                    "  group by substr(xiang,1,8),rollup(lmqs),rollup(lz))";
                 pWorkspace.ExecuteSQL(xiangSQL);
                 string shiSQL = "insert into EcosTable_YCLMJXJ ( select substr(shi,1,4) as ͳ�Ƶ�λ,lmqs as ��ľʹ��Ȩ,lz as ������," +
                                    "sum(case when llz between '1' and '8' then round(" + StatisticsFieldName + ",2) else 0 end) as �ϼ����," +
                                    "sum(case when llz between '1' and '8' then round(xjl,2) else 0 end) as �ϼ����," +
                                    "sum(case when llz='1' then round(" + StatisticsFieldName + ",2) else 0 end) as һ�伶���," +
                                    "sum(case when llz='1' then round(" + strXJFielldName + ",2) else 0 end) as һ�伶���," +
                                    "sum(case when llz='2' then round(" + StatisticsFieldName + ",2) else 0 end ) as ���伶���," +
                                    "sum(case when llz='2' then round(" + strXJFielldName + ",2) else 0 end) as ���伶���," +
                                    "sum(case when llz='3' then round(" + StatisticsFieldName + ",2) else 0 end) as ���伶���," +
                                    "sum(case when llz='3' then round(" + strXJFielldName + ",2) else 0 end) as ���伶���," +
                                    "sum(case when llz='4' then round(" + StatisticsFieldName + ",2) else 0 end) as ���伶���," +
                                    "sum(case when llz='4' then round(" + strXJFielldName + ",2) else 0 end) as ���伶���," +
                                    "sum(case when llz='5' then round(" + StatisticsFieldName + ",2) else 0 end) as ���伶���," +
                                    "sum(case when llz='5' then round(" + strXJFielldName + ",2) else 0 end) as ���伶���," +
                                    "sum(case when llz='6' then round(" + StatisticsFieldName + ",2) else 0 end) as ���伶���," +
                                    "sum(case when llz='6' then round(" + strXJFielldName + ",2) else 0 end) as ���伶���," +
                                    "sum(case when llz='7' then round(" + StatisticsFieldName + ",2) else 0 end) as ���伶���," +
                                    "sum(case when llz='7' then round(" + strXJFielldName + ",2) else 0 end) as ���伶���," +
                                    "sum(case when llz='8' then round(" + StatisticsFieldName + ",2) else 0 end) as ���伶���," +
                                    "sum(case when llz='8' then round(" + strXJFielldName + ",2) else 0 end) as ���伶���," +
                                    "sum(case when llz not in ('1','2','3','4','5','6','7','8') then round(" + StatisticsFieldName + ",2) else 0 end) as �������伶���," +
                                    "sum(case when llz not in ('1','2','3','4','5','6','7','8') then round(" + strXJFielldName + ",2) else 0 end) as �������伶���" +
                                    "  from " + tableName +
                                    "  where lmqs<> ' ' and lz between '231' and '233'" +
                                    "  group by substr(shi,1,4),rollup(lmqs),rollup(lz))";
                 pWorkspace.ExecuteSQL(shiSQL);
                 string shengSQL = "insert into EcosTable_YCLMJXJ ( select substr(sheng,1,2) as ͳ�Ƶ�λ,lmqs as ��ľʹ��Ȩ,lz as ������," +
                                    "sum(case when llz between '1' and '8' then round(" + StatisticsFieldName + ",2) else 0 end) as �ϼ����," +
                                    "sum(case when llz between '1' and '8' then round(xjl,2) else 0 end) as �ϼ����," +
                                    "sum(case when llz='1' then round(" + StatisticsFieldName + ",2) else 0 end) as һ�伶���," +
                                    "sum(case when llz='1' then round(" + strXJFielldName + ",2) else 0 end) as һ�伶���," +
                                    "sum(case when llz='2' then round(" + StatisticsFieldName + ",2) else 0 end ) as ���伶���," +
                                    "sum(case when llz='2' then round(" + strXJFielldName + ",2) else 0 end) as ���伶���," +
                                    "sum(case when llz='3' then round(" + StatisticsFieldName + ",2) else 0 end) as ���伶���," +
                                    "sum(case when llz='3' then round(" + strXJFielldName + ",2) else 0 end) as ���伶���," +
                                    "sum(case when llz='4' then round(" + StatisticsFieldName + ",2) else 0 end) as ���伶���," +
                                    "sum(case when llz='4' then round(" + strXJFielldName + ",2) else 0 end) as ���伶���," +
                                    "sum(case when llz='5' then round(" + StatisticsFieldName + ",2) else 0 end) as ���伶���," +
                                    "sum(case when llz='5' then round(" + strXJFielldName + ",2) else 0 end) as ���伶���," +
                                    "sum(case when llz='6' then round(" + StatisticsFieldName + ",2) else 0 end) as ���伶���," +
                                    "sum(case when llz='6' then round(" + strXJFielldName + ",2) else 0 end) as ���伶���," +
                                    "sum(case when llz='7' then round(" + StatisticsFieldName + ",2) else 0 end) as ���伶���," +
                                    "sum(case when llz='7' then round(" + strXJFielldName + ",2) else 0 end) as ���伶���," +
                                    "sum(case when llz='8' then round(" + StatisticsFieldName + ",2) else 0 end) as ���伶���," +
                                    "sum(case when llz='8' then round(" + strXJFielldName + ",2) else 0 end) as ���伶���," +
                                    "sum(case when llz not in ('1','2','3','4','5','6','7','8') then round(" + StatisticsFieldName + ",2) else 0 end) as �������伶���," +
                                    "sum(case when llz not in ('1','2','3','4','5','6','7','8') then round(" + strXJFielldName + ",2) else 0 end) as �������伶���" +
                                    "  from " + tableName +
                                    "  where lmqs<> ' ' and lz between '231' and '233'" +
                                    "  group by substr(sheng,1,2),rollup(lmqs),rollup(lz))";
                 pWorkspace.ExecuteSQL(shengSQL);

             }
             catch (Exception ex)
             {
                 return false;
             }
             return true;
        }
        private static bool LCYCLMJXJ_Statistic(IFeatureClass pFeatureClass, string strXJFielldName, string StatisticsFieldName)
        {
            if (pFeatureClass == null)
            {
                return false;
            }
            IWorkspace pWorkspace = null;
            string tableName = "";
            try
            {
                pWorkspace = pFeatureClass.FeatureDataset.Workspace;
                tableName = (pFeatureClass as IDataset).Name;
            }
            catch (Exception ex)
            { }
            try
            {
                if (!ExistTable(pWorkspace, "EcosTable_YCLMJXJ"))
                {
                    string lcSQL = "create table EcosTable_YCLMJXJ as select lc as ͳ�Ƶ�λ,lmqs as ��ľʹ��Ȩ,lz as ������," +
                                        "sum(case when llz between '1' and '8' then round(" + StatisticsFieldName + ",2) else 0 end) as �ϼ����," +
                                        "sum(case when llz between '1' and '8' then round(xjl,2) else 0 end) as �ϼ����," +
                                        "sum(case when llz='1' then round(" + StatisticsFieldName + ",2) else 0 end) as һ�伶���," +
                                        "sum(case when llz='1' then round(" + strXJFielldName + ",2) else 0 end) as һ�伶���," +
                                        "sum(case when llz='2' then round(" + StatisticsFieldName + ",2) else 0 end ) as ���伶���," +
                                        "sum(case when llz='2' then round(" + strXJFielldName + ",2) else 0 end) as ���伶���," +
                                        "sum(case when llz='3' then round(" + StatisticsFieldName + ",2) else 0 end) as ���伶���," +
                                        "sum(case when llz='3' then round(" + strXJFielldName + ",2) else 0 end) as ���伶���," +
                                        "sum(case when llz='4' then round(" + StatisticsFieldName + ",2) else 0 end) as ���伶���," +
                                        "sum(case when llz='4' then round(" + strXJFielldName + ",2) else 0 end) as ���伶���," +
                                        "sum(case when llz='5' then round(" + StatisticsFieldName + ",2) else 0 end) as ���伶���," +
                                        "sum(case when llz='5' then round(" + strXJFielldName + ",2) else 0 end) as ���伶���," +
                                        "sum(case when llz='6' then round(" + StatisticsFieldName + ",2) else 0 end) as ���伶���," +
                                        "sum(case when llz='6' then round(" + strXJFielldName + ",2) else 0 end) as ���伶���," +
                                        "sum(case when llz='7' then round(" + StatisticsFieldName + ",2) else 0 end) as ���伶���," +
                                        "sum(case when llz='7' then round(" + strXJFielldName + ",2) else 0 end) as ���伶���," +
                                        "sum(case when llz='8' then round(" + strXJFielldName + ",2) else 0 end) as ���伶���," +
                                        "sum(case when llz='8' then round(" + strXJFielldName + ",2) else 0 end) as ���伶���," +
                                        "sum(case when llz not in ('1','2','3','4','5','6','7','8') then round(" + StatisticsFieldName + ",2) else 0 end) as �������伶���," +
                                        "sum(case when llz not in ('1','2','3','4','5','6','7','8') then round(" + strXJFielldName + ",2) else 0 end) as �������伶���" +
                                        "  from " + tableName +
                                        "  where lmqs<> ' ' and lz between '231' and '233' and lc<>' '" +
                                        "  group by lc,rollup(lmqs),rollup(lz)";
                    pWorkspace.ExecuteSQL(lcSQL);
                }
                else
                {
                    string lcSQL1 = "insert into EcosTable_YCLMJXJ ( select lc as ͳ�Ƶ�λ,lmqs as ��ľʹ��Ȩ,lz as ������," +
                                        "sum(case when llz between '1' and '8' then round(" + StatisticsFieldName + ",2) else 0 end) as �ϼ����," +
                                        "sum(case when llz between '1' and '8' then round(xjl,2) else 0 end) as �ϼ����," +
                                        "sum(case when llz='1' then round(" + StatisticsFieldName + ",2) else 0 end) as һ�伶���," +
                                        "sum(case when llz='1' then round(" + strXJFielldName + ",2) else 0 end) as һ�伶���," +
                                        "sum(case when llz='2' then round(" + StatisticsFieldName + ",2) else 0 end ) as ���伶���," +
                                        "sum(case when llz='2' then round(" + strXJFielldName + ",2) else 0 end) as ���伶���," +
                                        "sum(case when llz='3' then round(" + StatisticsFieldName + ",2) else 0 end) as ���伶���," +
                                        "sum(case when llz='3' then round(" + strXJFielldName + ",2) else 0 end) as ���伶���," +
                                        "sum(case when llz='4' then round(" + StatisticsFieldName + ",2) else 0 end) as ���伶���," +
                                        "sum(case when llz='4' then round(" + strXJFielldName + ",2) else 0 end) as ���伶���," +
                                        "sum(case when llz='5' then round(" + StatisticsFieldName + ",2) else 0 end) as ���伶���," +
                                        "sum(case when llz='5' then round(" + strXJFielldName + ",2) else 0 end) as ���伶���," +
                                        "sum(case when llz='6' then round(" + StatisticsFieldName + ",2) else 0 end) as ���伶���," +
                                        "sum(case when llz='6' then round(" + strXJFielldName + ",2) else 0 end) as ���伶���," +
                                        "sum(case when llz='7' then round(" + StatisticsFieldName + ",2) else 0 end) as ���伶���," +
                                        "sum(case when llz='7' then round(" + strXJFielldName + ",2) else 0 end) as ���伶���," +
                                        "sum(case when llz='8' then round(" + strXJFielldName + ",2) else 0 end) as ���伶���," +
                                        "sum(case when llz='8' then round(" + strXJFielldName + ",2) else 0 end) as ���伶���," +
                                        "sum(case when llz not in ('1','2','3','4','5','6','7','8') then round(" + StatisticsFieldName + ",2) else 0 end) as �������伶���," +
                                        "sum(case when llz not in ('1','2','3','4','5','6','7','8') then round(" + strXJFielldName + ",2) else 0 end) as �������伶���" +
                                        "  from " + tableName +
                                        "  where lmqs<> ' ' and lz between '231' and '233' and lc<>' '" +
                                        "  group by lc,rollup(lmqs),rollup(lz))";
                    pWorkspace.ExecuteSQL(lcSQL1);
                    pWorkspace.ExecuteSQL("alter table EcosTable_YCLMJXJ modify ������ nvarchar2(10)");
                    pWorkspace.ExecuteSQL("alter table EcosTable_YCLMJXJ modify ��ľʹ��Ȩ nvarchar2(10)");
                    Dictionary<string, string> dicLZ = dicGetFieldValue(pWorkspace, "�����ֵ�");
                    UpadateStatistictable(pWorkspace, dicLZ, "EcosTable_YCLMJXJ", "������");
                    UpdateStatistictable(pWorkspace, "EcosTable_YCLMJXJ", "��ľʹ��Ȩ", "����", "1");
                    UpdateStatistictable(pWorkspace, "EcosTable_YCLMJXJ", "��ľʹ��Ȩ", "����", "2");
                    UpdateStatistictable(pWorkspace, "EcosTable_YCLMJXJ", "��ľʹ��Ȩ", "����", "3");
                    UpdateStatistictable(pWorkspace, "EcosTable_YCLMJXJ", "��ľʹ��Ȩ", "����", "9");
                    UpdateStatistictable(pWorkspace, "EcosTable_YCLMJXJ", "��ľʹ��Ȩ", "����", "B");
                    UpdateStatistictable(pWorkspace, "EcosTable_YCLMJXJ", "��ľʹ��Ȩ", "����", "0");


                }
            }
            catch (System.Exception ex)
            {
                return false;
            }
            return true;
        }
        #endregion
        #region ��̬�����֣��أ�ͳ��
        //��̬�����֣��أ�ͳ�� ygc 2012-12-11
        private static bool XZQSTGYL_Statistic(IFeatureClass pFeatureClass, string StatisticsFieldName)
        {
            if (pFeatureClass == null)
            {
                return false;
            }
            IWorkspace pWorkspace = null;
            string tableName = "";
            try
            {
                pWorkspace = pFeatureClass.FeatureDataset.Workspace;
                tableName = (pFeatureClass as IDataset).Name;
            }
            catch (Exception ex)
            { }
            DropTable(pWorkspace, "EcosTable_STGYL");
            try
            {
                string xianSQL = "create table EcosTable_STGYL as select xian as ͳ�Ƶ�λ,gclb as �������,sq as ��Ȩ�ȼ�,bhdj as �����ȼ�," +
                                    "sum(case when dl like'1%' then round(" + StatisticsFieldName + ",2) else 0 end ) as �ϼ�," +
                                    "sum(case when dl between '111' and '114' then round(" + StatisticsFieldName + ",2) else 0 end ) as ���ֵ�С��," +
                                    "sum(case when dl between '111' and '112' then round(" + StatisticsFieldName + ",2) else 0 end) as ��ľ��С��," +
                                    "sum(case when dl='111' then round(" + StatisticsFieldName + ",2) else 0 end ) as ����," +
                                    "sum(case when dl='112' then round(" + StatisticsFieldName + ",2) else 0 end ) as  �콻��," +
                                    "sum(case when dl='114' then round(" + StatisticsFieldName + ",2) else 0 end ) as ������," +
                                    "sum(case when dl='113' then round(" + StatisticsFieldName + ",2) else 0 end) as ����," +
                                    "sum(case when dl='120' then round(" + StatisticsFieldName + ",2) else 0 end ) as ���ֵ�," +
                                    "sum(case when dl between '131' and '132' then round(" + StatisticsFieldName + ",2) else 0 end) as ��ľ��С��," +
                                    "sum(case when dl='131' then round(" + StatisticsFieldName + ",2) else 0 end) as �����ر�涨��ľ��," +
                                    "sum(case when dl='132' then round(" + StatisticsFieldName + ",2) else 0 end) as ������ľ��," +
                                    "sum(case when dl between '141' and '142' then round(" + StatisticsFieldName + ",2) else 0 end) as δ�������ֵ�С��," +
                                    "sum(case when dl='141' then round(" + StatisticsFieldName + ",2) else 0 end) as �˹�����δ���ֵ�," +
                                    "sum(case when dl='142' then round(" + StatisticsFieldName + ",2) else 0 end) as ����δ���ֵ�," +
                                    "sum(case when dl='150' then round(" + StatisticsFieldName + ",2) else 0 end) as ���Ե�," +
                                    "sum(case when dl between '161' and '163' then round(" + StatisticsFieldName + ",2) else 0 end) as ����ľ�ֵ�С��," +
                                    "sum(case when dl='161' then round(" + StatisticsFieldName + ",2) else 0 end ) as �ɷ�����," +
                                    "sum(case when dl='162' then round(" + StatisticsFieldName + ",2) else 0 end ) as ���ռ���," +
                                    "sum(case when dl='163' then round(" + StatisticsFieldName + ",2) else 0 end ) as ��������ľ�ֵ�," +
                                    "sum(case when dl between '171' and '172' then round(" + StatisticsFieldName + ",2) else 0 end) as ���ֵ�С��," +
                                    "sum(case when dl='171' then round(" + StatisticsFieldName + ",2) else 0 end) as ���ֻ�ɽ�ĵ�," +
                                    "sum(case when dl='172' then round(" + StatisticsFieldName + ",2) else 0 end) as ����ɳ�ĵ�," +
                                    "sum(case when dl='173' then round(" + StatisticsFieldName + ",2) else 0 end) as �������ֵ�," +
                                     "sum(case when dl='174' then round(" + StatisticsFieldName + ",2) else 0 end) as �˸�," +
                                      "sum(case when dl='180' then round(" + StatisticsFieldName + ",2) else 0 end) as ���������ֵ�" +
                                    "  from " + tableName +
                                    "  where gclb<>' 'and sq<>' ' and bhdj<>' '" +
                                    "  group by xian ,rollup(gclb),rollup(sq),rollup(bhdj)";
                pWorkspace.ExecuteSQL(xianSQL);
                string xiangSQL = "insert into EcosTable_STGYL ( select substr(xiang,1,8) as ͳ�Ƶ�λ,gclb as �������,sq as ��Ȩ�ȼ�,bhdj as �����ȼ�," +
                                    "sum(case when dl like'1%' then round(" + StatisticsFieldName + ",2) else 0 end ) as �ϼ�," +
                                    "sum(case when dl between '111' and '114' then round(" + StatisticsFieldName + ",2) else 0 end ) as ���ֵ�С��," +
                                    "sum(case when dl between '111' and '112' then round(" + StatisticsFieldName + ",2) else 0 end) as ��ľ��С��," +
                                    "sum(case when dl='111' then round(" + StatisticsFieldName + ",2) else 0 end ) as ����," +
                                    "sum(case when dl='112' then round(" + StatisticsFieldName + ",2) else 0 end ) as  �콻��," +
                                    "sum(case when dl='114' then round(" + StatisticsFieldName + ",2) else 0 end ) as ������," +
                                    "sum(case when dl='113' then round(" + StatisticsFieldName + ",2) else 0 end) as ����," +
                                    "sum(case when dl='120' then round(" + StatisticsFieldName + ",2) else 0 end ) as ���ֵ�," +
                                    "sum(case when dl between '131' and '132' then round(" + StatisticsFieldName + ",2) else 0 end) as ��ľ��С��," +
                                    "sum(case when dl='131' then round(" + StatisticsFieldName + ",2) else 0 end) as �����ر�涨��ľ��," +
                                    "sum(case when dl='132' then round(" + StatisticsFieldName + ",2) else 0 end) as ������ľ��," +
                                    "sum(case when dl between '141' and '142' then round(" + StatisticsFieldName + ",2) else 0 end) as δ�������ֵ�С��," +
                                    "sum(case when dl='141' then round(" + StatisticsFieldName + ",2) else 0 end) as �˹�����δ���ֵ�," +
                                    "sum(case when dl='142' then round(" + StatisticsFieldName + ",2) else 0 end) as ����δ���ֵ�," +
                                    "sum(case when dl='150' then round(" + StatisticsFieldName + ",2) else 0 end) as ���Ե�," +
                                    "sum(case when dl between '161' and '163' then round(" + StatisticsFieldName + ",2) else 0 end) as ����ľ�ֵ�С��," +
                                    "sum(case when dl='161' then round(" + StatisticsFieldName + ",2) else 0 end ) as �ɷ�����," +
                                    "sum(case when dl='162' then round(" + StatisticsFieldName + ",2) else 0 end ) as ���ռ���," +
                                    "sum(case when dl='163' then round(" + StatisticsFieldName + ",2) else 0 end ) as ��������ľ�ֵ�," +
                                    "sum(case when dl between '171' and '172' then round(" + StatisticsFieldName + ",2) else 0 end) as ���ֵ�С��," +
                                    "sum(case when dl='171' then round(" + StatisticsFieldName + ",2) else 0 end) as ���ֻ�ɽ�ĵ�," +
                                    "sum(case when dl='172' then round(" + StatisticsFieldName + ",2) else 0 end) as ����ɳ�ĵ�," +
                                    "sum(case when dl='173' then round(" + StatisticsFieldName + ",2) else 0 end) as �������ֵ�," +
                                    "sum(case when dl='174' then round(" + StatisticsFieldName + ",2) else 0 end) as �˸�," +
                                    "sum(case when dl='180' then round(" + StatisticsFieldName + ",2) else 0 end) as ���������ֵ�" +
                                    "  from " + tableName +
                                    "  where gclb<>' 'and sq<>' ' and bhdj<>' '" +
                                    "  group by substr(xiang,1,8) ,rollup(gclb),rollup(sq),rollup(bhdj))";
                pWorkspace.ExecuteSQL(xiangSQL);
                string shiSQL = "insert into EcosTable_STGYL ( select substr(shi,1,4) as ͳ�Ƶ�λ,gclb as �������,sq as ��Ȩ�ȼ�,bhdj as �����ȼ�," +
                                    "sum(case when dl like'1%' then round(" + StatisticsFieldName + ",2) else 0 end ) as �ϼ�," +
                                    "sum(case when dl between '111' and '114' then round(" + StatisticsFieldName + ",2) else 0 end ) as ���ֵ�С��," +
                                    "sum(case when dl between '111' and '112' then round(" + StatisticsFieldName + ",2) else 0 end) as ��ľ��С��," +
                                    "sum(case when dl='111' then round(" + StatisticsFieldName + ",2) else 0 end ) as ����," +
                                    "sum(case when dl='112' then round(" + StatisticsFieldName + ",2) else 0 end ) as  �콻��," +
                                    "sum(case when dl='114' then round(" + StatisticsFieldName + ",2) else 0 end ) as ������," +
                                    "sum(case when dl='113' then round(" + StatisticsFieldName + ",2) else 0 end) as ����," +
                                    "sum(case when dl='120' then round(" + StatisticsFieldName + ",2) else 0 end ) as ���ֵ�," +
                                    "sum(case when dl between '131' and '132' then round(" + StatisticsFieldName + ",2) else 0 end) as ��ľ��С��," +
                                    "sum(case when dl='131' then round(" + StatisticsFieldName + ",2) else 0 end) as �����ر�涨��ľ��," +
                                    "sum(case when dl='132' then round(" + StatisticsFieldName + ",2) else 0 end) as ������ľ��," +
                                    "sum(case when dl between '141' and '142' then round(" + StatisticsFieldName + ",2) else 0 end) as δ�������ֵ�С��," +
                                    "sum(case when dl='141' then round(" + StatisticsFieldName + ",2) else 0 end) as �˹�����δ���ֵ�," +
                                    "sum(case when dl='142' then round(" + StatisticsFieldName + ",2) else 0 end) as ����δ���ֵ�," +
                                    "sum(case when dl='150' then round(" + StatisticsFieldName + ",2) else 0 end) as ���Ե�," +
                                    "sum(case when dl between '161' and '163' then round(" + StatisticsFieldName + ",2) else 0 end) as ����ľ�ֵ�С��," +
                                    "sum(case when dl='161' then round(" + StatisticsFieldName + ",2) else 0 end ) as �ɷ�����," +
                                    "sum(case when dl='162' then round(" + StatisticsFieldName + ",2) else 0 end ) as ���ռ���," +
                                    "sum(case when dl='163' then round(" + StatisticsFieldName + ",2) else 0 end ) as ��������ľ�ֵ�," +
                                    "sum(case when dl between '171' and '172' then round(" + StatisticsFieldName + ",2) else 0 end) as ���ֵ�С��," +
                                    "sum(case when dl='171' then round(" + StatisticsFieldName + ",2) else 0 end) as ���ֻ�ɽ�ĵ�," +
                                    "sum(case when dl='172' then round(" + StatisticsFieldName + ",2) else 0 end) as ����ɳ�ĵ�," +
                                    "sum(case when dl='173' then round(" + StatisticsFieldName + ",2) else 0 end) as �������ֵ�," +
                                    "sum(case when dl='174' then round(" + StatisticsFieldName + ",2) else 0 end) as �˸�," +
                                    "sum(case when dl='180' then round(" + StatisticsFieldName + ",2) else 0 end) as ���������ֵ�" +
                                    "  from " + tableName +
                                    "  where gclb<>' 'and sq<>' ' and bhdj<>' '" +
                                    "  group by substr(shi,1,4) ,rollup(gclb),rollup(sq),rollup(bhdj))";
                pWorkspace.ExecuteSQL(shiSQL);
                string shengSQL = "insert into EcosTable_STGYL ( select substr(sheng,1,2) as ͳ�Ƶ�λ,gclb as �������,sq as ��Ȩ�ȼ�,bhdj as �����ȼ�," +
                                    "sum(case when dl like'1%' then round(" + StatisticsFieldName + ",2) else 0 end ) as �ϼ�," +
                                    "sum(case when dl between '111' and '114' then round(" + StatisticsFieldName + ",2) else 0 end ) as ���ֵ�С��," +
                                    "sum(case when dl between '111' and '112' then round(" + StatisticsFieldName + ",2) else 0 end) as ��ľ��С��," +
                                    "sum(case when dl='111' then round(" + StatisticsFieldName + ",2) else 0 end ) as ����," +
                                    "sum(case when dl='112' then round(" + StatisticsFieldName + ",2) else 0 end ) as  �콻��," +
                                    "sum(case when dl='114' then round(" + StatisticsFieldName + ",2) else 0 end ) as ������," +
                                    "sum(case when dl='113' then round(" + StatisticsFieldName + ",2) else 0 end) as ����," +
                                    "sum(case when dl='120' then round(" + StatisticsFieldName + ",2) else 0 end ) as ���ֵ�," +
                                    "sum(case when dl between '131' and '132' then round(" + StatisticsFieldName + ",2) else 0 end) as ��ľ��С��," +
                                    "sum(case when dl='131' then round(" + StatisticsFieldName + ",2) else 0 end) as �����ر�涨��ľ��," +
                                    "sum(case when dl='132' then round(" + StatisticsFieldName + ",2) else 0 end) as ������ľ��," +
                                    "sum(case when dl between '141' and '142' then round(" + StatisticsFieldName + ",2) else 0 end) as δ�������ֵ�С��," +
                                    "sum(case when dl='141' then round(" + StatisticsFieldName + ",2) else 0 end) as �˹�����δ���ֵ�," +
                                    "sum(case when dl='142' then round(" + StatisticsFieldName + ",2) else 0 end) as ����δ���ֵ�," +
                                    "sum(case when dl='150' then round(" + StatisticsFieldName + ",2) else 0 end) as ���Ե�," +
                                    "sum(case when dl between '161' and '163' then round(" + StatisticsFieldName + ",2) else 0 end) as ����ľ�ֵ�С��," +
                                    "sum(case when dl='161' then round(" + StatisticsFieldName + ",2) else 0 end ) as �ɷ�����," +
                                    "sum(case when dl='162' then round(" + StatisticsFieldName + ",2) else 0 end ) as ���ռ���," +
                                    "sum(case when dl='163' then round(" + StatisticsFieldName + ",2) else 0 end ) as ��������ľ�ֵ�," +
                                    "sum(case when dl between '171' and '172' then round(" + StatisticsFieldName + ",2) else 0 end) as ���ֵ�С��," +
                                    "sum(case when dl='171' then round(" + StatisticsFieldName + ",2) else 0 end) as ���ֻ�ɽ�ĵ�," +
                                    "sum(case when dl='172' then round(" + StatisticsFieldName + ",2) else 0 end) as ����ɳ�ĵ�," +
                                    "sum(case when dl='173' then round(" + StatisticsFieldName + ",2) else 0 end) as �������ֵ�," +
                                    "sum(case when dl='174' then round(" + StatisticsFieldName + ",2) else 0 end) as �˸�," +
                                    "sum(case when dl='180' then round(" + StatisticsFieldName + ",2) else 0 end) as ���������ֵ�" +
                                    "  from " + tableName +
                                    "  where gclb<>' 'and sq<>' ' and bhdj<>' '" +
                                    "  group by substr(sheng,1,2) ,rollup(gclb),rollup(sq),rollup(bhdj))";
                pWorkspace.ExecuteSQL(shengSQL);
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }
        private static bool LCSTGYL_Statistic(IFeatureClass pFeatureClass, string StatisticsFieldName)
        {
            if (pFeatureClass == null)
            {
                return false;
            }
            IWorkspace pWorkspace = null;
            string tableName = "";
            try
            {
                pWorkspace = pFeatureClass.FeatureDataset.Workspace;
                tableName = (pFeatureClass as IDataset).Name;
            }
            catch (Exception ex)
            { }
            try
            {
                if (!ExistTable(pWorkspace, "EcosTable_YCLMJXJ"))
                {
                    string LCSQL = "create table EcosTable_STGYL as select lc as ͳ�Ƶ�λ,gclb as �������,sq as ��Ȩ�ȼ�,bhdj as �����ȼ�," +
                                    "sum(case when dl like'1%' then round(" + StatisticsFieldName + ",2) else 0 end ) as �ϼ�," +
                                    "sum(case when dl between '111' and '114' then round(" + StatisticsFieldName + ",2) else 0 end ) as ���ֵ�С��," +
                                    "sum(case when dl between '111' and '112' then round(" + StatisticsFieldName + ",2) else 0 end) as ��ľ��С��," +
                                    "sum(case when dl='111' then round(" + StatisticsFieldName + ",2) else 0 end ) as ����," +
                                    "sum(case when dl='112' then round(" + StatisticsFieldName + ",2) else 0 end ) as  �콻��," +
                                    "sum(case when dl='114' then round(" + StatisticsFieldName + ",2) else 0 end ) as ������," +
                                    "sum(case when dl='113' then round(" + StatisticsFieldName + ",2) else 0 end) as ����," +
                                    "sum(case when dl='120' then round(" + StatisticsFieldName + ",2) else 0 end ) as ���ֵ�," +
                                    "sum(case when dl between '131' and '132' then round(" + StatisticsFieldName + ",2) else 0 end) as ��ľ��С��," +
                                    "sum(case when dl='131' then round(" + StatisticsFieldName + ",2) else 0 end) as �����ر�涨��ľ��," +
                                    "sum(case when dl='132' then round(" + StatisticsFieldName + ",2) else 0 end) as ������ľ��," +
                                    "sum(case when dl between '141' and '142' then round(" + StatisticsFieldName + ",2) else 0 end) as δ�������ֵ�С��," +
                                    "sum(case when dl='141' then round(" + StatisticsFieldName + ",2) else 0 end) as �˹�����δ���ֵ�," +
                                    "sum(case when dl='142' then round(" + StatisticsFieldName + ",2) else 0 end) as ����δ���ֵ�," +
                                    "sum(case when dl='150' then round(" + StatisticsFieldName + ",2) else 0 end) as ���Ե�," +
                                    "sum(case when dl between '161' and '163' then round(" + StatisticsFieldName + ",2) else 0 end) as ����ľ�ֵ�С��," +
                                    "sum(case when dl='161' then round(" + StatisticsFieldName + ",2) else 0 end ) as �ɷ�����," +
                                    "sum(case when dl='162' then round(" + StatisticsFieldName + ",2) else 0 end ) as ���ռ���," +
                                    "sum(case when dl='163' then round(" + StatisticsFieldName + ",2) else 0 end ) as ��������ľ�ֵ�," +
                                    "sum(case when dl between '171' and '172' then round(" + StatisticsFieldName + ",2) else 0 end) as ���ֵ�С��," +
                                    "sum(case when dl='171' then round(" + StatisticsFieldName + ",2) else 0 end) as ���ֻ�ɽ�ĵ�," +
                                    "sum(case when dl='172' then round(" + StatisticsFieldName + ",2) else 0 end) as ����ɳ�ĵ�," +
                                    "sum(case when dl='173' then round(" + StatisticsFieldName + ",2) else 0 end) as �������ֵ�," +
                                    "sum(case when dl='174' then round(" + StatisticsFieldName + ",2) else 0 end) as �˸�," +
                                    "sum(case when dl='180' then round(" + StatisticsFieldName + ",2) else 0 end) as ���������ֵ�" +
                                    "  from " + tableName +
                                    "  where gclb<>' 'and sq<>' ' and bhdj<>' ' and lc<>' '" +
                                    "  group by lc ,rollup(gclb),rollup(sq),rollup(bhdj)";
                    pWorkspace.ExecuteSQL(LCSQL);
                }
                else
                {
                    string LC = "insert into EcosTable_STGYL ( select lc as ͳ�Ƶ�λ,gclb as �������,sq as ��Ȩ�ȼ�,bhdj as �����ȼ�," +
                                    "sum(case when dl like'1%' then round(" + StatisticsFieldName + ",2) else 0 end ) as �ϼ�," +
                                    "sum(case when dl between '111' and '114' then round(" + StatisticsFieldName + ",2) else 0 end ) as ���ֵ�С��," +
                                    "sum(case when dl between '111' and '112' then round(" + StatisticsFieldName + ",2) else 0 end) as ��ľ��С��," +
                                    "sum(case when dl='111' then round(" + StatisticsFieldName + ",2) else 0 end ) as ����," +
                                    "sum(case when dl='112' then round(" + StatisticsFieldName + ",2) else 0 end ) as  �콻��," +
                                    "sum(case when dl='114' then round(" + StatisticsFieldName + ",2) else 0 end ) as ������," +
                                    "sum(case when dl='113' then round(" + StatisticsFieldName + ",2) else 0 end) as ����," +
                                    "sum(case when dl='120' then round(" + StatisticsFieldName + ",2) else 0 end ) as ���ֵ�," +
                                    "sum(case when dl between '131' and '132' then round(" + StatisticsFieldName + ",2) else 0 end) as ��ľ��С��," +
                                    "sum(case when dl='131' then round(" + StatisticsFieldName + ",2) else 0 end) as �����ر�涨��ľ��," +
                                    "sum(case when dl='132' then round(" + StatisticsFieldName + ",2) else 0 end) as ������ľ��," +
                                    "sum(case when dl between '141' and '142' then round(" + StatisticsFieldName + ",2) else 0 end) as δ�������ֵ�С��," +
                                    "sum(case when dl='141' then round(" + StatisticsFieldName + ",2) else 0 end) as �˹�����δ���ֵ�," +
                                    "sum(case when dl='142' then round(" + StatisticsFieldName + ",2) else 0 end) as ����δ���ֵ�," +
                                    "sum(case when dl='150' then round(" + StatisticsFieldName + ",2) else 0 end) as ���Ե�," +
                                    "sum(case when dl between '161' and '163' then round(" + StatisticsFieldName + ",2) else 0 end) as ����ľ�ֵ�С��," +
                                    "sum(case when dl='161' then round(" + StatisticsFieldName + ",2) else 0 end ) as �ɷ�����," +
                                    "sum(case when dl='162' then round(" + StatisticsFieldName + ",2) else 0 end ) as ���ռ���," +
                                    "sum(case when dl='163' then round(" + StatisticsFieldName + ",2) else 0 end ) as ��������ľ�ֵ�," +
                                    "sum(case when dl between '171' and '172' then round(" + StatisticsFieldName + ",2) else 0 end) as ���ֵ�С��," +
                                    "sum(case when dl='171' then round(" + StatisticsFieldName + ",2) else 0 end) as ���ֻ�ɽ�ĵ�," +
                                    "sum(case when dl='172' then round(" + StatisticsFieldName + ",2) else 0 end) as ����ɳ�ĵ�," +
                                    "sum(case when dl='173' then round(" + StatisticsFieldName + ",2) else 0 end) as �������ֵ�," +
                                    "sum(case when dl='174' then round(" + StatisticsFieldName + ",2) else 0 end) as �˸�," +
                                    "sum(case when dl='180' then round(" + StatisticsFieldName + ",2) else 0 end) as ���������ֵ�" +
                                    "  from " + tableName +
                                    "  where gclb<>' 'and sq<>' ' and bhdj<>' ' and lc<>' '" +
                                    "  group by lc ,rollup(gclb),rollup(sq),rollup(bhdj))";
                    pWorkspace.ExecuteSQL(LC);
                    pWorkspace.ExecuteSQL("alter table EcosTable_STGYL modify ������� nvarchar2(20)");
                    pWorkspace.ExecuteSQL("alter table EcosTable_STGYL modify ��Ȩ�ȼ� nvarchar2(10)");
                    pWorkspace.ExecuteSQL("alter table EcosTable_STGYL modify �����ȼ� nvarchar2(10)");
                    Dictionary<string, string> dicGC = dicGetFieldValue(pWorkspace, "��������ֵ�");
                    UpadateStatistictable(pWorkspace, dicGC, "EcosTable_STGYL", "�������");
                    UpdateStatistictable(pWorkspace, "EcosTable_STGYL", "��Ȩ�ȼ�", "���ҹ�����", "1");
                    UpdateStatistictable(pWorkspace, "EcosTable_STGYL", "��Ȩ�ȼ�", "�ط�������", "2");
                    UpdateStatistictable(pWorkspace, "EcosTable_STGYL", "��Ȩ�ȼ�", "����", "B");
                    UpdateStatistictable(pWorkspace, "EcosTable_STGYL", "��Ȩ�ȼ�", "����", "0");
                    UpdateStatistictable(pWorkspace, "EcosTable_STGYL", "�����ȼ�", "����", "1");
                    UpdateStatistictable(pWorkspace, "EcosTable_STGYL", "�����ȼ�", "�ص�", "2");
                    UpdateStatistictable(pWorkspace, "EcosTable_STGYL", "�����ȼ�", "һ��", "3");
                    UpdateStatistictable(pWorkspace, "EcosTable_STGYL", "�����ȼ�", "����", "0");
                    UpdateStatistictable(pWorkspace, "EcosTable_STGYL", "�����ȼ�", "����", "B");
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }
        #endregion
        #region ��ľ��������������ͳ��
        private static bool XZQQMLMJ_Statistic(IFeatureClass pFeatureClass, string strXJFielldName, string StatisticsFieldName)
        {
            if (pFeatureClass == null)
            {
                return false;
            }
            IWorkspace pWorkspace = null;
            string tableName = "";
            try
            {
                pWorkspace = pFeatureClass.FeatureDataset.Workspace;
                tableName = (pFeatureClass as IDataset).Name;
            }
            catch (Exception ex)
            { }
            DropTable(pWorkspace, "EcosTable_QMLMJ");
            DropTable(pWorkspace, "Temp_QML");
            try
            {
                //������ʱ��
                pWorkspace.ExecuteSQL("create table Temp_QML as select xian,xiang,sheng,shi,qi_yuan,llz,dl,zysz,lc,"+StatisticsFieldName+","+strXJFielldName+"  from "+tableName);
                //������ʱ��
                pWorkspace.ExecuteSQL("update Temp_QML set qi_yuan='��Ȼ' where qi_yuan like'1%'");
                pWorkspace.ExecuteSQL("update Temp_QML set qi_yuan='�˹�' where qi_yuan like '2%'");
                //ͳ������
                string xianSQL = "create table EcosTable_QMLMJ as select xian as ͳ�Ƶ�λ," +
                                   "qi_yuan  as ��Դ,zysz as ����,dl as ��ľ��,"+
                                   "sum( case when LLZ between '1' and '5' then round("+StatisticsFieldName+",2) else 0 end) as ���С��,"+
                                   "sum( case when LLZ between '1' and '5' then round(" + strXJFielldName + ",2) else 0 end) as  ���С��," +
                                   "sum( case when LLZ='1'then round(" + StatisticsFieldName + ",2) else 0 end) as ���������," +
                                   "sum(case when LLZ='1' then round(" + strXJFielldName + ",2) else 0 end) as ���������," +
                                   "sum(case when LLZ='2' then round(" + StatisticsFieldName + ",2) else 0 end) as ���������," +
                                   "sum(case when LLZ='2' then round(" + strXJFielldName + ",2) else 0 end) as ���������," +
                                   "sum(case when LLZ='3' then round(" + StatisticsFieldName + ",2) else 0 end) as ���������," +
                                   "sum(case when LLZ='3' then round(" + strXJFielldName + ",2) else 0 end) as ���������," +
                                   "sum(case when LLZ='4' then round(" + StatisticsFieldName + ",2) else 0 end) as ���������," +
                                   "sum(case when LLZ ='4' then round(" + strXJFielldName + ",2) else 0 end ) as ���������," +
                                   "sum(case when LLZ='5' then round(" + StatisticsFieldName + ",2) else 0 end ) as ���������," +
                                   "sum(case when LLZ='5' then round(" + strXJFielldName + ",2) else 0 end) as ���������" +
                                   "  from Temp_QML" +
                                   "  where dl between '111' and '112' and zysz in('120','130','150') and qi_yuan<> ' '" +
                                   "  group by xian ,rollup(zysz),rollup(qi_yuan��,rollup(dl) ";
                pWorkspace.ExecuteSQL(xianSQL);
                string xiangSQL = "insert into EcosTable_QMLMJ ( select substr(xiang,1,8) as ͳ�Ƶ�λ," +
                                   " qi_yuan  as ��Դ,zysz as ����,dl as ��ľ��," +
                                   "sum( case when LLZ between '1' and '5' then round(" + StatisticsFieldName + ",2) else 0 end) as ���С��," +
                                   "sum( case when LLZ between '1' and '5' then round(" + strXJFielldName + ",2) else 0 end) as  ���С��," +
                                   "sum( case when LLZ='1'then round(" + StatisticsFieldName + ",2) else 0 end) as ���������," +
                                   "sum(case when LLZ='1' then round(" + strXJFielldName + ",2) else 0 end) as ���������," +
                                   "sum(case when LLZ='2' then round(" + StatisticsFieldName + ",2) else 0 end) as ���������," +
                                   "sum(case when LLZ='2' then round(" + strXJFielldName + ",2) else 0 end) as ���������," +
                                   "sum(case when LLZ='3' then round(" + StatisticsFieldName + ",2) else 0 end) as ���������," +
                                   "sum(case when LLZ='3' then round(" + strXJFielldName + ",2) else 0 end) as ���������," +
                                   "sum(case when LLZ='4' then round(" + StatisticsFieldName + ",2) else 0 end) as ���������," +
                                   "sum(case when LLZ ='4' then round(" + strXJFielldName + ",2) else 0 end ) as ���������," +
                                   "sum(case when LLZ='5' then round(" + StatisticsFieldName + ",2) else 0 end ) as ���������," +
                                   "sum(case when LLZ='5' then round(" + strXJFielldName + ",2) else 0 end) as ���������" +
                                   "  from Temp_QML"  +
                                   "  where dl between '111' and '112' and zysz in('120','130','150')  and qi_yuan<> ' '" +
                                   "  group by substr(xiang,1,8) ,rollup(zysz),rollup(qi_yuan��,rollup(dl))";
                pWorkspace.ExecuteSQL(xiangSQL);
                string shiSQL = "insert into EcosTable_QMLMJ ( select substr(shi,1,4) as ͳ�Ƶ�λ," +
                                   " qi_yuan  as ��Դ,zysz as ����,dl as ��ľ��," +
                                   "sum( case when LLZ between '1' and '5' then round(" + StatisticsFieldName + ",2) else 0 end) as ���С��," +
                                   "sum( case when LLZ between '1' and '5' then round(" + strXJFielldName + ",2) else 0 end) as  ���С��," +
                                   "sum( case when LLZ='1'then round(" + StatisticsFieldName + ",2) else 0 end) as ���������," +
                                   "sum(case when LLZ='1' then round(" + strXJFielldName + ",2) else 0 end) as ���������," +
                                   "sum(case when LLZ='2' then round(" + StatisticsFieldName + ",2) else 0 end) as ���������," +
                                   "sum(case when LLZ='2' then round(" + strXJFielldName + ",2) else 0 end) as ���������," +
                                   "sum(case when LLZ='3' then round(" + StatisticsFieldName + ",2) else 0 end) as ���������," +
                                   "sum(case when LLZ='3' then round(" + strXJFielldName + ",2) else 0 end) as ���������," +
                                   "sum(case when LLZ='4' then round(" + StatisticsFieldName + ",2) else 0 end) as ���������," +
                                   "sum(case when LLZ ='4' then round(" + strXJFielldName + ",2) else 0 end ) as ���������," +
                                   "sum(case when LLZ='5' then round(" + StatisticsFieldName + ",2) else 0 end ) as ���������," +
                                   "sum(case when LLZ='5' then round(" + strXJFielldName + ",2) else 0 end) as ���������" +
                                   "  from Temp_QML" +
                                   "  where dl between '111' and '112' and zysz in('120','130','150') and qi_yuan<> ' '" +
                                   "  group by substr(shi,1,4) ,rollup(zysz),rollup(qi_yuan��,rollup(dl)) ";
                pWorkspace.ExecuteSQL(shiSQL);
                string shengSQL = "insert into EcosTable_QMLMJ ( select substr(sheng,1,2) as ͳ�Ƶ�λ," +
                                   " qi_yuan as ��Դ,zysz as ����,dl as ��ľ��," +
                                   "sum( case when LLZ between '1' and '5' then round(" + StatisticsFieldName + ",2) else 0 end) as ���С��," +
                                   "sum( case when LLZ between '1' and '5' then round(" + strXJFielldName + ",2) else 0 end) as  ���С��," +
                                   "sum( case when LLZ='1'then round(" + StatisticsFieldName + ",2) else 0 end) as ���������," +
                                   "sum(case when LLZ='1' then round(" + strXJFielldName + ",2) else 0 end) as ���������," +
                                   "sum(case when LLZ='2' then round(" + StatisticsFieldName + ",2) else 0 end) as ���������," +
                                   "sum(case when LLZ='2' then round(" + strXJFielldName + ",2) else 0 end) as ���������," +
                                   "sum(case when LLZ='3' then round(" + StatisticsFieldName + ",2) else 0 end) as ���������," +
                                   "sum(case when LLZ='3' then round(" + strXJFielldName + ",2) else 0 end) as ���������," +
                                   "sum(case when LLZ='4' then round(" + StatisticsFieldName + ",2) else 0 end) as ���������," +
                                   "sum(case when LLZ ='4' then round(" + strXJFielldName + ",2) else 0 end ) as ���������," +
                                   "sum(case when LLZ='5' then round(" + StatisticsFieldName + ",2) else 0 end ) as ���������," +
                                   "sum(case when LLZ='5' then round(" + strXJFielldName + ",2) else 0 end) as ���������" +
                                   "  from Temp_QML"  +
                                   "  where dl between '111' and '112' and zysz in('120','130','150') and qi_yuan<> ' '" +
                                   "  group by substr(sheng,1,2) ,rollup(zysz),rollup(qi_yuan��,rollup(dl)) ";
                pWorkspace.ExecuteSQL(shengSQL);
                string LC = "insert into  EcosTable_QMLMJ ( select lc as ͳ�Ƶ�λ," +
                               " qi_yuan  as ��Դ,zysz as ����,dl as ��ľ��," +
                               "sum( case when LLZ between '1' and '5' then round(" + StatisticsFieldName + ",2) else 0 end) as ���С��," +
                               "sum( case when LLZ between '1' and '5' then round(" + strXJFielldName + ",2) else 0 end) as  ���С��," +
                               "sum( case when LLZ='1'then round(" + StatisticsFieldName + ",2) else 0 end) as ���������," +
                               "sum(case when LLZ='1' then round(" + strXJFielldName + ",2) else 0 end) as ���������," +
                               "sum(case when LLZ='2' then round(" + StatisticsFieldName + ",2) else 0 end) as ���������," +
                               "sum(case when LLZ='2' then round(" + strXJFielldName + ",2) else 0 end) as ���������," +
                               "sum(case when LLZ='3' then round(" + StatisticsFieldName + ",2) else 0 end) as ���������," +
                               "sum(case when LLZ='3' then round(" + strXJFielldName + ",2) else 0 end) as ���������," +
                               "sum(case when LLZ='4' then round(" + StatisticsFieldName + ",2) else 0 end) as ���������," +
                               "sum(case when LLZ ='4' then round(" + strXJFielldName + ",2) else 0 end ) as ���������," +
                               "sum(case when LLZ='5' then round(" + StatisticsFieldName + ",2) else 0 end ) as ���������," +
                               "sum(case when LLZ='5' then round(" + strXJFielldName + ",2) else 0 end) as ���������" +
                               "  from Temp_QML" +
                               "  where dl between '111' and '112' and zysz in('120','130','150') and lc<>' ' and qi_yuan<> ' '" +
                               "  group by lc ,rollup(zysz),rollup(qi_yuan��,rollup(dl))";
                pWorkspace.ExecuteSQL(LC);
                Dictionary<string, string> dicSZ = dicGetFieldValue(pWorkspace, "�����ֵ�");
                UpadateStatistictable(pWorkspace, dicSZ, "EcosTable_QMLMJ", "����");
                Dictionary<string, string> dicLZ = dicGetFieldValue(pWorkspace, "�����ֵ�");
                UpadateStatistictable(pWorkspace, dicLZ, "EcosTable_QMLMJ", "��ľ��");
            }
            catch (System.Exception ex)
            {
                return false;
            }
            return true;
        }
        //private static bool LCQMLMJ_Statistic(IFeatureClass pFeatureClass, string strXJFielldName, string StatisticsFieldName)
        //{
        //    if (pFeatureClass == null)
        //    {
        //        return false;
        //    }
        //    IWorkspace pWorkspace = null;
        //    string tableName = "";
        //    try
        //    {
        //        pWorkspace = pFeatureClass.FeatureDataset.Workspace;
        //        tableName = (pFeatureClass as IDataset).Name;
        //    }
        //    catch (Exception ex)
        //    { }
        //    try
        //    {
        //        if (!ExistTable(pWorkspace, "EcosTable_QMLMJ"))
        //        {
        //          string LCSQL= "create table EcosTable_QMLMJ as select lc as ͳ�Ƶ�λ," +
        //                           "case when qi_yuan like '1%' then '��Ȼ' else '�˹�' end as ��Դ,zysz as ����,dl as ��ľ��," +
        //                           "sum( case when LLZ between '1' and '5' then round(" + StatisticsFieldName + ",2) else 0 end) as ���С��," +
        //                           "sum( case when LLZ between '1' and '5' then round(" + strXJFielldName + ",2) else 0 end) as  ���С��," +
        //                           "sum( case when LLZ='1'then round(" + StatisticsFieldName + ",2) else 0 end) as ���������," +
        //                           "sum(case when LLZ='1' then round(" + strXJFielldName + ",2) else 0 end) as ���������," +
        //                           "sum(case when LLZ='2' then round(" + StatisticsFieldName + ",2) else 0 end) as ���������," +
        //                           "sum(case when LLZ='2' then round(" + strXJFielldName + ",2) else 0 end) as ���������," +
        //                           "sum(case when LLZ='3' then round(" + StatisticsFieldName + ",2) else 0 end) as ���������," +
        //                           "sum(case when LLZ='3' then round(" + strXJFielldName + ",2) else 0 end) as ���������," +
        //                           "sum(case when LLZ='4' then round(" + StatisticsFieldName + ",2) else 0 end) as ���������," +
        //                           "sum(case when LLZ ='4' then round(" + strXJFielldName + ",2) else 0 end ) as ���������," +
        //                           "sum(case when LLZ='5' then round(" + StatisticsFieldName + ",2) else 0 end ) as ���������," +
        //                           "sum(case when LLZ='5' then round(" + strXJFielldName + ",2) else 0 end) as ���������" +
        //                           "  from " + tableName +
        //                           "  where dl between '111' and '112' and zysz in('120','130','150') and lc<>' 'and qi_yuan<> ' '" +
        //                           "  group by lc ,rollup(zysz),rollup(qi_yuan��,rollup(dl)";
        //          pWorkspace.ExecuteSQL(LCSQL);

        //        }
        //        else
        //        {
        //            string LC = "insert into  EcosTable_QMLMJ ( select lc as ͳ�Ƶ�λ," +
        //                           "case when qi_yuan like '1%' then '��Ȼ' else '�˹�' end as ��Դ,zysz as ����,dl as ��ľ��," +
        //                           "sum( case when LLZ between '1' and '5' then round(" + StatisticsFieldName + ",2) else 0 end) as ���С��," +
        //                           "sum( case when LLZ between '1' and '5' then round(" + strXJFielldName + ",2) else 0 end) as  ���С��," +
        //                           "sum( case when LLZ='1'then round(" + StatisticsFieldName + ",2) else 0 end) as ���������," +
        //                           "sum(case when LLZ='1' then round(" + strXJFielldName + ",2) else 0 end) as ���������," +
        //                           "sum(case when LLZ='2' then round(" + StatisticsFieldName + ",2) else 0 end) as ���������," +
        //                           "sum(case when LLZ='2' then round(" + strXJFielldName + ",2) else 0 end) as ���������," +
        //                           "sum(case when LLZ='3' then round(" + StatisticsFieldName + ",2) else 0 end) as ���������," +
        //                           "sum(case when LLZ='3' then round(" + strXJFielldName + ",2) else 0 end) as ���������," +
        //                           "sum(case when LLZ='4' then round(" + StatisticsFieldName + ",2) else 0 end) as ���������," +
        //                           "sum(case when LLZ ='4' then round(" + strXJFielldName + ",2) else 0 end ) as ���������," +
        //                           "sum(case when LLZ='5' then round(" + StatisticsFieldName + ",2) else 0 end ) as ���������," +
        //                           "sum(case when LLZ='5' then round(" + strXJFielldName + ",2) else 0 end) as ���������" +
        //                           "  from " + tableName +
        //                           "  where dl between '111' and '112' and zysz in('120','130','150') and lc<>' ' and qi_yuan<> ' '" +
        //                           "  group by lc ,rollup(zysz),rollup(qi_yuan��,rollup(dl))";
        //            pWorkspace.ExecuteSQL(LC);
        //        }
        //    }
        //    catch (System.Exception ex)
        //    {
        //        return false;
        //    }
        //    return true;
        //}
        #endregion
        #region ����ͳ��
        private static bool XZQLZ_Statistic(IFeatureClass pFeatureClass, string strXJFielldName, string StatisticsFieldName)
        {
            if (pFeatureClass == null)
            {
                return false;
            }
            IWorkspace pWorkspace = null;
            string tableName = "";
            try
            {
                pWorkspace = pFeatureClass.FeatureDataset.Workspace;
                tableName = (pFeatureClass as IDataset).Name;
            }
            catch (Exception ex)
            { }
            DropTable(pWorkspace, "tempTable_LZ");
            DropTable(pWorkspace, "EcosTable_LZ");
            try
            {
                //������ʱ��
                pWorkspace.ExecuteSQL("create table tempTable_LZ as select lc,xian,sheng,shi,xiang,case when lz<>' ' then lz end  as ����,case when lz<>' ' then lz end as ������,LLZ,DL," + StatisticsFieldName + "," + strXJFielldName + ",JJLZS  from  " + tableName + "  where lz<>' '");
                //�޸���ʱ��
                pWorkspace.ExecuteSQL("alter table tempTable_LZ modify ���� nvarchar2(10)");
                pWorkspace.ExecuteSQL("alter table tempTable_LZ modify ������ nvarchar2(10)");
                pWorkspace.ExecuteSQL("update tempTable_LZ set ����='������' where ���� between '111' and '117'");
                pWorkspace.ExecuteSQL("update tempTable_LZ set ����='������;��' where ���� between '121' and '127'");
                pWorkspace.ExecuteSQL("update tempTable_LZ set ����='�ò���' where ���� between '231' and '233'");
                pWorkspace.ExecuteSQL("update tempTable_LZ set ����='н̿��' where ����='240'");
                pWorkspace.ExecuteSQL("update tempTable_LZ set ����='������' where ���� between '251' and '255'");
                //ͳ������
                string xianSQL = "create table EcosTable_LZ as select xian as ͳ�Ƶ�λ,����,������," +
                                    "sum(case when (llz between 1 and 5) and (dl='111'or dl='120') then round(" + strXJFielldName + ",2) else 0 end) as ����ľ�������," +
                                    "sum (case when dl between '111' and '114' then round(" + StatisticsFieldName + ",2) else 0 end) as ���ֵ����С��," +
                                    "sum(case when llz between 1 and 5 and(dl='111' or dl='112') then round(" + StatisticsFieldName + ",2) else 0 end) as ��ľ�����С��," +
                                    "sum(case when llz between 1 and 5 and(dl='111' or dl='112') then round(" + strXJFielldName + ",2) else 0 end) as ��ľ�����С��," +
                                    "sum(case when LLZ=1 and (DL='111' or DL='112') then round(" + StatisticsFieldName + ",2) else 0 end) as ���������," +
                                    "sum(case when LLZ=1 and (DL='111' or DL='112') then round(" + strXJFielldName + ",2) else 0 end) as ���������," +
                                    "sum(case when LLZ=2 and (DL='111' or DL='112') then round(" + StatisticsFieldName + ",2) else 0 end) as ������sum���," +
                                    "sum(case when LLZ=2 and (DL='111' or DL='112') then round(" + strXJFielldName + ",2) else 0 end) as ���������," +
                                    "sum(case when LLZ=3 and (DL='111' or DL='112') then round(" + StatisticsFieldName + ",2) else 0 end) as ���������," +
                                    "sum(case when LLZ=3 and (DL='111' or DL='112') then round(" + strXJFielldName + ",2) else 0 end) as ���������," +
                                    "sum(case when LLZ=4 and (DL='111' or DL='112') then round(" + StatisticsFieldName + ",2) else 0 end) as ���������," +
                                    "sum(case when LLZ=4 and (DL='111' or DL='112') then round(" + strXJFielldName + ",2) else 0 end) as ���������," +
                                    "sum(case when LLZ=5 and (DL='111' or DL='112') then round(" + StatisticsFieldName + ",2) else 0 end) as ���������," +
                                    "sum(case when LLZ=5 and (DL='111' or DL='112') then round(" + strXJFielldName + ",2) else 0 end) as ���������," +
                                    "sum(case when dl='114' then round(" + StatisticsFieldName + ",2) else 0 end) as ���������," +
                                    "sum(case when dl='113' then round(" + StatisticsFieldName + ",2) else 0 end ) as  �������," +
                                    "sum(case when dl='120' then round(" + StatisticsFieldName + ",2) else 0 end ) as  �������," +
                                    "sum(case when dl='120' then round(" + strXJFielldName + ",2) else 0 end ) as �������," +
                                    "sum(case when dl between '131' and '132' then round(" + StatisticsFieldName + ",2) else 0 end) as ��ľ�����С��," +
                                    "sum(case when dl='131' then round(" + StatisticsFieldName + ",2) else 0 end) as �����ر�涨��ľ�����," +
                                    "sum(case when dl='132' then round(" + StatisticsFieldName + ",2) else 0 end) as ������ľ�����" +
                                    "  from tempTable_LZ" +
                                    "  where xjl<>0"+
                                    "  group by xian,rollup(����),rollup(�����֣�";
                pWorkspace.ExecuteSQL(xianSQL);
                string xiangSQL = "insert into EcosTable_LZ( select substr(xiang,1,8) as ͳ�Ƶ�λ,����,������," +
                                    "sum(case when (llz between 1 and 5) and (dl='111'or dl='120') then round(" + strXJFielldName + ",2) else 0 end) as ����ľ�������," +
                                    "sum (case when dl between '111' and '114' then round(" + StatisticsFieldName + ",2) else 0 end) as ���ֵ����С��," +
                                    "sum(case when llz between 1 and 5 and(dl='111' or dl='112') then round(" + StatisticsFieldName + ",2) else 0 end) as ��ľ�����С��," +
                                    "sum(case when llz between 1 and 5 and(dl='111' or dl='112') then round(" + strXJFielldName + ",2) else 0 end) as ��ľ�����С��," +
                                    "sum(case when LLZ=1 and (DL='111' or DL='112') then round(" + StatisticsFieldName + ",2) else 0 end) as ���������," +
                                    "sum(case when LLZ=1 and (DL='111' or DL='112') then round(" + strXJFielldName + ",2) else 0 end) as ���������," +
                                    "sum(case when LLZ=2 and (DL='111' or DL='112') then round(" + StatisticsFieldName + ",2) else 0 end) as ������sum���," +
                                    "sum(case when LLZ=2 and (DL='111' or DL='112') then round(" + strXJFielldName + ",2) else 0 end) as ���������," +
                                    "sum(case when LLZ=3 and (DL='111' or DL='112') then round(" + StatisticsFieldName + ",2) else 0 end) as ���������," +
                                    "sum(case when LLZ=3 and (DL='111' or DL='112') then round(" + strXJFielldName + ",2) else 0 end) as ���������," +
                                    "sum(case when LLZ=4 and (DL='111' or DL='112') then round(" + StatisticsFieldName + ",2) else 0 end) as ���������," +
                                    "sum(case when LLZ=4 and (DL='111' or DL='112') then round(" + strXJFielldName + ",2) else 0 end) as ���������," +
                                    "sum(case when LLZ=5 and (DL='111' or DL='112') then round(" + StatisticsFieldName + ",2) else 0 end) as ���������," +
                                    "sum(case when LLZ=5 and (DL='111' or DL='112') then round(" + strXJFielldName + ",2) else 0 end) as ���������," +
                                    "sum(case when dl='114' then round(" + StatisticsFieldName + ",2) else 0 end) as ���������," +
                                    "sum(case when dl='113' then round(" + StatisticsFieldName + ",2) else 0 end ) as  �������," +
                                    "sum(case when dl='120' then round(" + StatisticsFieldName + ",2) else 0 end ) as  �������," +
                                    "sum(case when dl='120' then round(" + strXJFielldName + ",2) else 0 end ) as �������," +
                                    "sum(case when dl between '131' and '132' then round(" + StatisticsFieldName + ",2) else 0 end) as ��ľ�����С��," +
                                    "sum(case when dl='131' then round(" + StatisticsFieldName + ",2) else 0 end) as �����ر�涨��ľ�����," +
                                    "sum(case when dl='132' then round(" + StatisticsFieldName + ",2) else 0 end) as ������ľ�����" +
                                    "  from tempTable_LZ" +
                                    "  where xjl<>0" +
                                    "  group by substr(xiang,1,8),rollup(����),rollup(�����֣�)";
                pWorkspace.ExecuteSQL(xiangSQL);
                string shiSQL = "insert into EcosTable_LZ ( select substr(shi,1,4) as ͳ�Ƶ�λ,����,������," +
                                    "sum(case when (llz between 1 and 5) and (dl='111'or dl='120') then round(" + strXJFielldName + ",2) else 0 end) as ����ľ�������," +
                                    "sum (case when dl between '111' and '114' then round(" + StatisticsFieldName + ",2) else 0 end) as ���ֵ����С��," +
                                    "sum(case when llz between 1 and 5 and(dl='111' or dl='112') then round(" + StatisticsFieldName + ",2) else 0 end) as ��ľ�����С��," +
                                    "sum(case when llz between 1 and 5 and(dl='111' or dl='112') then round(" + strXJFielldName + ",2) else 0 end) as ��ľ�����С��," +
                                    "sum(case when LLZ=1 and (DL='111' or DL='112') then round(" + StatisticsFieldName + ",2) else 0 end) as ���������," +
                                    "sum(case when LLZ=1 and (DL='111' or DL='112') then round(" + strXJFielldName + ",2) else 0 end) as ���������," +
                                    "sum(case when LLZ=2 and (DL='111' or DL='112') then round(" + StatisticsFieldName + ",2) else 0 end) as ������sum���," +
                                    "sum(case when LLZ=2 and (DL='111' or DL='112') then round(" + strXJFielldName + ",2) else 0 end) as ���������," +
                                    "sum(case when LLZ=3 and (DL='111' or DL='112') then round(" + StatisticsFieldName + ",2) else 0 end) as ���������," +
                                    "sum(case when LLZ=3 and (DL='111' or DL='112') then round(" + strXJFielldName + ",2) else 0 end) as ���������," +
                                    "sum(case when LLZ=4 and (DL='111' or DL='112') then round(" + StatisticsFieldName + ",2) else 0 end) as ���������," +
                                    "sum(case when LLZ=4 and (DL='111' or DL='112') then round(" + strXJFielldName + ",2) else 0 end) as ���������," +
                                    "sum(case when LLZ=5 and (DL='111' or DL='112') then round(" + StatisticsFieldName + ",2) else 0 end) as ���������," +
                                    "sum(case when LLZ=5 and (DL='111' or DL='112') then round(" + strXJFielldName + ",2) else 0 end) as ���������," +
                                    "sum(case when dl='114' then round(" + StatisticsFieldName + ",2) else 0 end) as ���������," +
                                    "sum(case when dl='113' then round(" + StatisticsFieldName + ",2) else 0 end ) as  �������," +
                                    "sum(case when dl='120' then round(" + StatisticsFieldName + ",2) else 0 end ) as  �������," +
                                    "sum(case when dl='120' then round(" + strXJFielldName + ",2) else 0 end ) as �������," +
                                    "sum(case when dl between '131' and '132' then round(" + StatisticsFieldName + ",2) else 0 end) as ��ľ�����С��," +
                                    "sum(case when dl='131' then round(" + StatisticsFieldName + ",2) else 0 end) as �����ر�涨��ľ�����," +
                                    "sum(case when dl='132' then round(" + StatisticsFieldName + ",2) else 0 end) as ������ľ�����" +
                                    "  from tempTable_LZ" +
                                    "  where xjl<>0" +
                                    "  group by substr(shi,1,4),rollup(����),rollup(�����֣�)";
                pWorkspace.ExecuteSQL(shiSQL);
                string shengSQL = "insert into EcosTable_LZ ( select substr(sheng,1,2) as ͳ�Ƶ�λ,����,������," +
                                    "sum(case when (llz between 1 and 5) and (dl='111'or dl='120') then round(" + strXJFielldName + ",2) else 0 end) as ����ľ�������," +
                                    "sum (case when dl between '111' and '114' then round(" + StatisticsFieldName + ",2) else 0 end) as ���ֵ����С��," +
                                    "sum(case when llz between 1 and 5 and(dl='111' or dl='112') then round(" + StatisticsFieldName + ",2) else 0 end) as ��ľ�����С��," +
                                    "sum(case when llz between 1 and 5 and(dl='111' or dl='112') then round(" + strXJFielldName + ",2) else 0 end) as ��ľ�����С��," +
                                    "sum(case when LLZ=1 and (DL='111' or DL='112') then round(" + StatisticsFieldName + ",2) else 0 end) as ���������," +
                                    "sum(case when LLZ=1 and (DL='111' or DL='112') then round(" + strXJFielldName + ",2) else 0 end) as ���������," +
                                    "sum(case when LLZ=2 and (DL='111' or DL='112') then round(" + StatisticsFieldName + ",2) else 0 end) as ������sum���," +
                                    "sum(case when LLZ=2 and (DL='111' or DL='112') then round(" + strXJFielldName + ",2) else 0 end) as ���������," +
                                    "sum(case when LLZ=3 and (DL='111' or DL='112') then round(" + StatisticsFieldName + ",2) else 0 end) as ���������," +
                                    "sum(case when LLZ=3 and (DL='111' or DL='112') then round(" + strXJFielldName + ",2) else 0 end) as ���������," +
                                    "sum(case when LLZ=4 and (DL='111' or DL='112') then round(" + StatisticsFieldName + ",2) else 0 end) as ���������," +
                                    "sum(case when LLZ=4 and (DL='111' or DL='112') then round(" + strXJFielldName + ",2) else 0 end) as ���������," +
                                    "sum(case when LLZ=5 and (DL='111' or DL='112') then round(" + StatisticsFieldName + ",2) else 0 end) as ���������," +
                                    "sum(case when LLZ=5 and (DL='111' or DL='112') then round(" + strXJFielldName + ",2) else 0 end) as ���������," +
                                    "sum(case when dl='114' then round(" + StatisticsFieldName + ",2) else 0 end) as ���������," +
                                    "sum(case when dl='113' then round(" + StatisticsFieldName + ",2) else 0 end ) as  �������," +
                                    "sum(case when dl='120' then round(" + StatisticsFieldName + ",2) else 0 end ) as  �������," +
                                    "sum(case when dl='120' then round(" + strXJFielldName + ",2) else 0 end ) as �������," +
                                    "sum(case when dl between '131' and '132' then round(" + StatisticsFieldName + ",2) else 0 end) as ��ľ�����С��," +
                                    "sum(case when dl='131' then round(" + StatisticsFieldName + ",2) else 0 end) as �����ر�涨��ľ�����," +
                                    "sum(case when dl='132' then round(" + StatisticsFieldName + ",2) else 0 end) as ������ľ�����" +
                                    "  from tempTable_LZ" +
                                    "  where xjl<>0" +
                                    "  group by substr(sheng,1,2),rollup(����),rollup(�����֣�)";
                pWorkspace.ExecuteSQL(shengSQL);
                string LC = "insert into EcosTable_LZ ( select lc as ͳ�Ƶ�λ,����,������," +
                                    "sum(case when (llz between 1 and 5) and (dl='111'or dl='120') then round(" + strXJFielldName + ",2) else 0 end) as ����ľ�������," +
                                    "sum (case when dl between '111' and '114' then round(" + StatisticsFieldName + ",2) else 0 end) as ���ֵ����С��," +
                                    "sum(case when llz between 1 and 5 and(dl='111' or dl='112') then round(" + StatisticsFieldName + ",2) else 0 end) as ��ľ�����С��," +
                                    "sum(case when llz between 1 and 5 and(dl='111' or dl='112') then round(" + strXJFielldName + ",2) else 0 end) as ��ľ�����С��," +
                                    "sum(case when LLZ=1 and (DL='111' or DL='112') then round(" + StatisticsFieldName + ",2) else 0 end) as ���������," +
                                    "sum(case when LLZ=1 and (DL='111' or DL='112') then round(" + strXJFielldName + ",2) else 0 end) as ���������," +
                                    "sum(case when LLZ=2 and (DL='111' or DL='112') then round(" + StatisticsFieldName + ",2) else 0 end) as ������sum���," +
                                    "sum(case when LLZ=2 and (DL='111' or DL='112') then round(" + strXJFielldName + ",2) else 0 end) as ���������," +
                                    "sum(case when LLZ=3 and (DL='111' or DL='112') then round(" + StatisticsFieldName + ",2) else 0 end) as ���������," +
                                    "sum(case when LLZ=3 and (DL='111' or DL='112') then round(" + strXJFielldName + ",2) else 0 end) as ���������," +
                                    "sum(case when LLZ=4 and (DL='111' or DL='112') then round(" + StatisticsFieldName + ",2) else 0 end) as ���������," +
                                    "sum(case when LLZ=4 and (DL='111' or DL='112') then round(" + strXJFielldName + ",2) else 0 end) as ���������," +
                                    "sum(case when LLZ=5 and (DL='111' or DL='112') then round(" + StatisticsFieldName + ",2) else 0 end) as ���������," +
                                    "sum(case when LLZ=5 and (DL='111' or DL='112') then round(" + strXJFielldName + ",2) else 0 end) as ���������," +
                                    "sum(case when dl='114' then round(" + StatisticsFieldName + ",2) else 0 end) as ���������," +
                                    "sum(case when dl='113' then round(" + StatisticsFieldName + ",2) else 0 end ) as  �������," +
                                    "sum(case when dl='120' then round(" + StatisticsFieldName + ",2) else 0 end ) as  �������," +
                                    "sum(case when dl='120' then round(" + strXJFielldName + ",2) else 0 end ) as �������," +
                                    "sum(case when dl between '131' and '132' then round(" + StatisticsFieldName + ",2) else 0 end) as ��ľ�����С��," +
                                    "sum(case when dl='131' then round(" + StatisticsFieldName + ",2) else 0 end) as �����ر�涨��ľ�����," +
                                    "sum(case when dl='132' then round(" + StatisticsFieldName + ",2) else 0 end) as ������ľ�����" +
                                    "  from tempTable_LZ" +
                                    "  where xjl<>0 and lc<>' '" +
                                    "  group by lc,rollup(����),rollup(�����֣�)";
                pWorkspace.ExecuteSQL(LC);
                Dictionary<string, string> dicLZ = dicGetFieldValue(pWorkspace, "�����ֵ�");
                UpadateStatistictable(pWorkspace, dicLZ, "EcosTable_LZ", "������");

            }
            catch (System.Exception ex)
            {
                return false;
            }
            return true;
        }
        #endregion
        #region ������ͳ��
        //������ͳ��
        private static bool XZQJJL_Statistic(IFeatureClass pFeatureClass, string ZSFieldName, string StatisticsFieldName)   
        {
            if (pFeatureClass == null)
            {
                return false;
            }
            IWorkspace pWorkspace = null;
            string tableName = "";
            try
            {
                pWorkspace = pFeatureClass.FeatureDataset.Workspace;
                tableName = (pFeatureClass as IDataset).Name;
            }
            catch (Exception ex)
            { }
            DropTable(pWorkspace, "TempTable_JJL");
            DropTable(pWorkspace, "EcosTable_JJL");
            try
            {
                //������ʱ��
                pWorkspace.ExecuteSQL("create table TempTable_JJL as select xian,sheng,shi,xiang,dl,lc,zysz,ll,lmqs,qi_yuan," + StatisticsFieldName + "," + ZSFieldName + " from " + tableName);
                // ������ʱ��
                pWorkspace.ExecuteSQL("update TempTable_JJL set qi_yuan='�˹�' where qi_yuan like'1%'");
                pWorkspace.ExecuteSQL("update TempTable_JJL set qi_yuan='��Ȼ' where qi_yuan like'2%'");

                string xianSQL = "create table EcosTable_JJL as select xian as ͳ�Ƶ�λ,lmqs as ��ľʹ��Ȩ," +
                                  " qi_yuan as ��Դ,zysz as ����,"+
                                  "sum(case when dl='114' and zysz like'7%' and ll>0 then round(" + StatisticsFieldName + ",2) else 0 end ) as ��ľ����ϼ�," +
                                  "sum(case when dl='114' and zysz like'7%' and ll>0 then " + ZSFieldName + " else 0 end) as ��ľ�����ϼ�," +
                                  "sum(case when dl='114' and zysz like '7%' and ll<=3 then round(" + StatisticsFieldName + ",2) else 0 end ) as ��ľ��ǰ�����," +
                                  "sum(case when dl='114' and zysz like'7%' and ll <=3 then " + ZSFieldName + " else 0 end ) as ��ľ��ǰ������," +
                                  "sum(case when dl='114' and zysz like '7%' and ll between 3 and 6 then round(" + StatisticsFieldName + ",2) else 0 end) as ��ľ���������," +
                                  "sum(case when dl='114' and zysz like'7%' and ll between 3 and 6 then " + ZSFieldName + " else 0 end ) as ��ľ����������," +
                                  "sum(case when dl='114' and zysz like '7%' and ll between 6 and 15 then round(" + StatisticsFieldName + ",2) else 0 end) as ��ľʢ�������," +
                                  "sum(case when dl='114' and zysz like'7%' and ll between 6 and 15 then " + ZSFieldName + " else 0 end) as ��ľʢ��������," +
                                  "sum(case when dl='114' and zysz like'7%' and ll >= 15 then round(" + StatisticsFieldName + ",2) else 0 end) as ��ľ˥�������," +
                                  "sum(case when dl='114' and zysz like'7%' and ll>=15 then " + ZSFieldName + " else 0 end) as ��ľ˥��������," +
                                  "sum(case when dl='114' and zysz like'9%' and ll >=0 then round(" + StatisticsFieldName + ",2) else 0 end ) as ��ľ�ϼ�," +
                                  "sum(case when dl='114' and zysz like'9%' and ll<=3 then round(" + StatisticsFieldName + ",2) else 0 end) ��ľ��ǰ�����," +
                                  "sum(case when dl='114' and zysz like'9%' and ll between 3 and 6 then round(" + StatisticsFieldName + ",2) else 0 end ) as ��ľ���������," +
                                  "sum(case when dl='114' and zysz like'9%' and ll between 6 and 15 then round(" + StatisticsFieldName + ",2) else 0 end) as ��ľʢ�������," +
                                  "sum(case when dl='114' and zysz like'9%' and ll>=15 then round(" + StatisticsFieldName + ",2) else 0 end) as ��ľ˥�������" +
                                  "  from TempTable_JJL"  +
                                  "  where lmqs<>' ' and qi_yuan <>' ' and zysz between '702' and '704'"+
                                  "  group by xian ,rollup(lmqs),rollup(qi_yuan),rollup(zysz)";
                pWorkspace.ExecuteSQL(xianSQL);
                string xiangSQL = "insert into  EcosTable_JJL ( select substr(xiang,1,8) as ͳ�Ƶ�λ,lmqs as ��ľʹ��Ȩ," +
                                  " qi_yuan  as ��Դ,zysz as ����," +
                                  "sum(case when dl='114' and zysz like'7%' and ll>0 then round(" + StatisticsFieldName + ",2) else 0 end ) as ��ľ����ϼ�," +
                                  "sum(case when dl='114' and zysz like'7%' and ll>0 then " + ZSFieldName + " else 0 end) as ��ľ�����ϼ�," +
                                  "sum(case when dl='114' and zysz like '7%' and ll<=3 then round(" + StatisticsFieldName + ",2) else 0 end ) as ��ľ��ǰ�����," +
                                  "sum(case when dl='114' and zysz like'7%' and ll <=3 then " + ZSFieldName + " else 0 end ) as ��ľ��ǰ������," +
                                  "sum(case when dl='114' and zysz like '7%' and ll between 3 and 6 then round(" + StatisticsFieldName + ",2) else 0 end) as ��ľ���������," +
                                  "sum(case when dl='114' and zysz like'7%' and ll between 3 and 6 then " + ZSFieldName + " else 0 end ) as ��ľ����������," +
                                  "sum(case when dl='114' and zysz like '7%' and ll between 6 and 15 then round(" + StatisticsFieldName + ",2) else 0 end) as ��ľʢ�������," +
                                  "sum(case when dl='114' and zysz like'7%' and ll between 6 and 15 then " + ZSFieldName + " else 0 end) as ��ľʢ��������," +
                                  "sum(case when dl='114' and zysz like'7%' and ll >= 15 then round(" + StatisticsFieldName + ",2) else 0 end) as ��ľ˥�������," +
                                  "sum(case when dl='114' and zysz like'7%' and ll>=15 then " + ZSFieldName + " else 0 end) as ��ľ˥��������," +
                                  "sum(case when dl='114' and zysz like'9%' and ll >=0 then round(" + StatisticsFieldName + ",2) else 0 end ) as ��ľ�ϼ�," +
                                  "sum(case when dl='114' and zysz like'9%' and ll<=3 then round(" + StatisticsFieldName + ",2) else 0 end) ��ľ��ǰ�����," +
                                  "sum(case when dl='114' and zysz like'9%' and ll between 3 and 6 then round(" + StatisticsFieldName + ",2) else 0 end ) as ��ľ���������," +
                                  "sum(case when dl='114' and zysz like'9%' and ll between 6 and 15 then round(" + StatisticsFieldName + ",2) else 0 end) as ��ľʢ�������," +
                                  "sum(case when dl='114' and zysz like'9%' and ll>=15 then round(" + StatisticsFieldName + ",2) else 0 end) as ��ľ˥�������" +
                                  "  from TempTable_JJL" +
                                  "  where lmqs<>' ' and qi_yuan <>' ' and zysz between '702' and '704'" +
                                  "  group by substr(xiang,1,8) ,rollup(lmqs),rollup(qi_yuan),rollup(zysz))";
                pWorkspace.ExecuteSQL(xiangSQL);
                string shiSQL = "insert into  EcosTable_JJL ( select substr(shi,1,4) as ͳ�Ƶ�λ,lmqs as ��ľʹ��Ȩ," +
                                  " qi_yuan  as ��Դ,zysz as ����," +
                                  "sum(case when dl='114' and zysz like'7%' and ll>0 then round(" + StatisticsFieldName + ",2) else 0 end ) as ��ľ����ϼ�," +
                                  "sum(case when dl='114' and zysz like'7%' and ll>0 then " + ZSFieldName + " else 0 end) as ��ľ�����ϼ�," +
                                  "sum(case when dl='114' and zysz like '7%' and ll<=3 then round(" + StatisticsFieldName + ",2) else 0 end ) as ��ľ��ǰ�����," +
                                  "sum(case when dl='114' and zysz like'7%' and ll <=3 then " + ZSFieldName + " else 0 end ) as ��ľ��ǰ������," +
                                  "sum(case when dl='114' and zysz like '7%' and ll between 3 and 6 then round(" + StatisticsFieldName + ",2) else 0 end) as ��ľ���������," +
                                  "sum(case when dl='114' and zysz like'7%' and ll between 3 and 6 then " + ZSFieldName + " else 0 end ) as ��ľ����������," +
                                  "sum(case when dl='114' and zysz like '7%' and ll between 6 and 15 then round(" + StatisticsFieldName + ",2) else 0 end) as ��ľʢ�������," +
                                  "sum(case when dl='114' and zysz like'7%' and ll between 6 and 15 then " + ZSFieldName + " else 0 end) as ��ľʢ��������," +
                                  "sum(case when dl='114' and zysz like'7%' and ll >= 15 then round(" + StatisticsFieldName + ",2) else 0 end) as ��ľ˥�������," +
                                  "sum(case when dl='114' and zysz like'7%' and ll>=15 then " + ZSFieldName + " else 0 end) as ��ľ˥��������," +
                                  "sum(case when dl='114' and zysz like'9%' and ll >=0 then round(" + StatisticsFieldName + ",2) else 0 end ) as ��ľ�ϼ�," +
                                  "sum(case when dl='114' and zysz like'9%' and ll<=3 then round(" + StatisticsFieldName + ",2) else 0 end) ��ľ��ǰ�����," +
                                  "sum(case when dl='114' and zysz like'9%' and ll between 3 and 6 then round(" + StatisticsFieldName + ",2) else 0 end ) as ��ľ���������," +
                                  "sum(case when dl='114' and zysz like'9%' and ll between 6 and 15 then round(" + StatisticsFieldName + ",2) else 0 end) as ��ľʢ�������," +
                                  "sum(case when dl='114' and zysz like'9%' and ll>=15 then round(" + StatisticsFieldName + ",2) else 0 end) as ��ľ˥�������" +
                                  "  from TempTable_JJL"  +
                                  "  where lmqs<>' ' and qi_yuan <>' ' and zysz between '702' and '704'" +
                                  "  group by substr(shi,1,4) ,rollup(lmqs),rollup(qi_yuan),rollup(zysz))";
                pWorkspace.ExecuteSQL(shiSQL);
                string shengSQL = "insert into  EcosTable_JJL ( select substr(sheng,1,2) as ͳ�Ƶ�λ,lmqs as ��ľʹ��Ȩ," +
                                  " qi_yuan  as ��Դ,zysz as ����," +
                                  "sum(case when dl='114' and zysz like'7%' and ll>0 then round(" + StatisticsFieldName + ",2) else 0 end ) as ��ľ����ϼ�," +
                                  "sum(case when dl='114' and zysz like'7%' and ll>0 then " + ZSFieldName + " else 0 end) as ��ľ�����ϼ�," +
                                  "sum(case when dl='114' and zysz like '7%' and ll<=3 then round(" + StatisticsFieldName + ",2) else 0 end ) as ��ľ��ǰ�����," +
                                  "sum(case when dl='114' and zysz like'7%' and ll <=3 then " + ZSFieldName + " else 0 end ) as ��ľ��ǰ������," +
                                  "sum(case when dl='114' and zysz like '7%' and ll between 3 and 6 then round(" + StatisticsFieldName + ",2) else 0 end) as ��ľ���������," +
                                  "sum(case when dl='114' and zysz like'7%' and ll between 3 and 6 then " + ZSFieldName + " else 0 end ) as ��ľ����������," +
                                  "sum(case when dl='114' and zysz like '7%' and ll between 6 and 15 then round(" + StatisticsFieldName + ",2) else 0 end) as ��ľʢ�������," +
                                  "sum(case when dl='114' and zysz like'7%' and ll between 6 and 15 then " + ZSFieldName + " else 0 end) as ��ľʢ��������," +
                                  "sum(case when dl='114' and zysz like'7%' and ll >= 15 then round(" + StatisticsFieldName + ",2) else 0 end) as ��ľ˥�������," +
                                  "sum(case when dl='114' and zysz like'7%' and ll>=15 then " + ZSFieldName + " else 0 end) as ��ľ˥��������," +
                                  "sum(case when dl='114' and zysz like'9%' and ll >=0 then round(" + StatisticsFieldName + ",2) else 0 end ) as ��ľ�ϼ�," +
                                  "sum(case when dl='114' and zysz like'9%' and ll<=3 then round(" + StatisticsFieldName + ",2) else 0 end) ��ľ��ǰ�����," +
                                  "sum(case when dl='114' and zysz like'9%' and ll between 3 and 6 then round(" + StatisticsFieldName + ",2) else 0 end ) as ��ľ���������," +
                                  "sum(case when dl='114' and zysz like'9%' and ll between 6 and 15 then round(" + StatisticsFieldName + ",2) else 0 end) as ��ľʢ�������," +
                                  "sum(case when dl='114' and zysz like'9%' and ll>=15 then round(" + StatisticsFieldName + ",2) else 0 end) as ��ľ˥�������" +
                                  "  from TempTable_JJL"  +
                                  "  where lmqs<>' ' and qi_yuan <>' ' and zysz between '702' and '704'" +
                                  "  group by substr(sheng,1,2) ,rollup(lmqs),rollup(qi_yuan),rollup(zysz))";
                pWorkspace.ExecuteSQL(shengSQL);
                string LC = "insert into  EcosTable_JJL ( select lc as ͳ�Ƶ�λ,lmqs as ��ľʹ��Ȩ," +
                                 " qi_yuan  as ��Դ,zysz as ����," +
                                 "sum(case when dl='114' and zysz like'7%' and ll>0 then round(" + StatisticsFieldName + ",2) else 0 end ) as ��ľ����ϼ�," +
                                 "sum(case when dl='114' and zysz like'7%' and ll>0 then " + ZSFieldName + " else 0 end) as ��ľ�����ϼ�," +
                                 "sum(case when dl='114' and zysz like '7%' and ll<=3 then round(" + StatisticsFieldName + ",2) else 0 end ) as ��ľ��ǰ�����," +
                                 "sum(case when dl='114' and zysz like'7%' and ll <=3 then " + ZSFieldName + " else 0 end ) as ��ľ��ǰ������," +
                                 "sum(case when dl='114' and zysz like '7%' and ll between 3 and 6 then round(" + StatisticsFieldName + ",2) else 0 end) as ��ľ���������," +
                                 "sum(case when dl='114' and zysz like'7%' and ll between 3 and 6 then " + ZSFieldName + " else 0 end ) as ��ľ����������," +
                                 "sum(case when dl='114' and zysz like '7%' and ll between 6 and 15 then round(" + StatisticsFieldName + ",2) else 0 end) as ��ľʢ�������," +
                                 "sum(case when dl='114' and zysz like'7%' and ll between 6 and 15 then " + ZSFieldName + " else 0 end) as ��ľʢ��������," +
                                 "sum(case when dl='114' and zysz like'7%' and ll >= 15 then round(" + StatisticsFieldName + ",2) else 0 end) as ��ľ˥�������," +
                                 "sum(case when dl='114' and zysz like'7%' and ll>=15 then " + ZSFieldName + " else 0 end) as ��ľ˥��������," +
                                 "sum(case when dl='114' and zysz like'9%' and ll >=0 then round(" + StatisticsFieldName + ",2) else 0 end ) as ��ľ�ϼ�," +
                                 "sum(case when dl='114' and zysz like'9%' and ll<=3 then round(" + StatisticsFieldName + ",2) else 0 end) ��ľ��ǰ�����," +
                                 "sum(case when dl='114' and zysz like'9%' and ll between 3 and 6 then round(" + StatisticsFieldName + ",2) else 0 end ) as ��ľ���������," +
                                 "sum(case when dl='114' and zysz like'9%' and ll between 6 and 15 then round(" + StatisticsFieldName + ",2) else 0 end) as ��ľʢ�������," +
                                 "sum(case when dl='114' and zysz like'9%' and ll>=15 then round(" + StatisticsFieldName + ",2) else 0 end) as ��ľ˥�������" +
                                 "  from TempTable_JJL"  +
                                 "  where lmqs<>' ' and qi_yuan <>' ' and zysz between '702' and '704' and lc<>' '" +
                                 "  group by lc ,rollup(lmqs),rollup(qi_yuan),rollup(zysz))";
                pWorkspace.ExecuteSQL(LC);
                DropTable(pWorkspace, "TempTable_JJL");
                pWorkspace.ExecuteSQL("alter table EcosTable_JJL modify ��ľʹ��Ȩ nvarchar2(10)");
                pWorkspace.ExecuteSQL("alter table EcosTable_JJL modify ���� nvarchar2(10)");
                pWorkspace.ExecuteSQL("update EcosTable_JJL set ��ľʹ��Ȩ ='����' where ��ľʹ��Ȩ='1'");
                pWorkspace.ExecuteSQL("update EcosTable_JJL set ��ľʹ��Ȩ='����' where ��ľʹ��Ȩ='2'");
                pWorkspace.ExecuteSQL("update EcosTable_JJL set ��ľʹ��Ȩ='����' where ��ľʹ��Ȩ='3'");
                pWorkspace.ExecuteSQL("update EcosTable_JJL set ��ľʹ��Ȩ='����' where ��ľʹ��Ȩ='9'");
                pWorkspace.ExecuteSQL("update EcosTable_JJL set ��ľʹ��Ȩ='����' where ��ľʹ��Ȩ='0'");
                pWorkspace.ExecuteSQL("update EcosTable_JJL set ��ľʹ��Ȩ='����' where ��ľʹ��Ȩ='B'");
                Dictionary<string, string> dicSZ = dicGetFieldValue(pWorkspace, "�����ֵ�");
                UpadateStatistictable(pWorkspace, dicSZ, "EcosTable_JJL", "����");
            }
            catch (System.Exception ex)
            {
                DropTable(pWorkspace, "TempTable_JJL");
                return false;
            }
            return true;
        }
        //private static bool LCJJL_Statistic(IFeatureClass pFeatureClass, string ZSFieldName, string StatisticsFieldName)
        //{
        //    if (pFeatureClass == null)
        //    {
        //        return false;
        //    }
        //    IWorkspace pWorkspace = null;
        //    string tableName = "";
        //    try
        //    {
        //        pWorkspace = pFeatureClass.FeatureDataset.Workspace;
        //        tableName = (pFeatureClass as IDataset).Name;
        //    }
        //    catch (Exception ex)
        //    { }
        //    try
        //    {
        //        if (!ExistTable(pWorkspace, "EcosTable_JJL"))
        //        {
        //            string LCSQL = "create table EcosTable_JJL as select lc as ͳ�Ƶ�λ,lmqs as ��ľʹ��Ȩ," +
        //                              "case when qi_yuan like '1%' then '�˹�' else  '��Ȼ' end as ��Դ,zysz as ����," +
        //                              "sum(case when dl='114' and zysz like'7%' and ll>0 then round(" + StatisticsFieldName + ",2) else 0 end ) as ��ľ����ϼ�," +
        //                              "sum(case when dl='114' and zysz like'7%' and ll>0 then " + ZSFieldName + " else 0 end) as ��ľ�����ϼ�," +
        //                              "sum(case when dl='114' and zysz like '7%' and ll<=3 then round(" + StatisticsFieldName + ",2) else 0 end ) as ��ľ��ǰ�����," +
        //                              "sum(case when dl='114' and zysz like'7%' and ll <=3 then " + ZSFieldName + " else 0 end ) as ��ľ��ǰ������," +
        //                              "sum(case when dl='114' and zysz like '7%' and ll between 3 and 6 then round(" + StatisticsFieldName + ",2) else 0 end) as ��ľ���������," +
        //                              "sum(case when dl='114' and zysz like'7%' and ll between 3 and 6 then " + ZSFieldName + " else 0 end ) as ��ľ����������," +
        //                              "sum(case when dl='114' and zysz like '7%' and ll between 6 and 15 then round(" + StatisticsFieldName + ",2) end) as ��ľʢ�������," +
        //                              "sum(case when dl='114' and zysz like'7%' and ll between 6 and 15 then " + ZSFieldName + " else 0 end) as ��ľʢ��������," +
        //                              "sum(case when dl='114' and zysz like'7%' and ll >= 15 then round(" + StatisticsFieldName + ",2) else 0 end) as ��ľ˥�������," +
        //                              "sum(case when dl='114' and zysz like'7%' and ll>=15 then " + ZSFieldName + " else 0 end) as ��ľ˥��������," +
        //                              "sum(case when dl='114' and zysz like'9%' and ll >=0 then round(" + StatisticsFieldName + ",2) else 0 end ) as ��ľ�ϼ�," +
        //                              "sum(case when dl='114' and zysz like'9%' and ll<=3 then round(" + StatisticsFieldName + ",2) else 0 end) ��ľ��ǰ�����," +
        //                              "sum(case when dl='114' and zysz like'9%' and ll between 3 and 6 then round(" + StatisticsFieldName + ",2) else 0 end ) as ��ľ���������," +
        //                              "sum(case when dl='114' and zysz like'9%' and ll between 6 and 15 then round(" + StatisticsFieldName + ",2) else 0 end) as ��ľʢ�������," +
        //                              "sum(case when dl='114' and zysz like'9%' and ll>=15 then round(" + StatisticsFieldName + ",2) else 0 end) as ��ľ˥�������" +
        //                              "  from " + tableName +
        //                              "  where lmqs<>' ' and qi_yuan <>' ' and zysz between '702' and '704' and lc<>' '" +
        //                              "  group by lc ,rollup(lmqs),rollup(qi_yuan),rollup(zysz)";
        //            pWorkspace.ExecuteSQL(LCSQL);
        //        }
        //        else
        //        {
        //            string LC = "insert into  EcosTable_JJL ( select lc as ͳ�Ƶ�λ,lmqs as ��ľʹ��Ȩ," +
        //                          "case when qi_yuan like '1%' then '�˹�' else  '��Ȼ' end as ��Դ,zysz as ����," +
        //                          "sum(case when dl='114' and zysz like'7%' and ll>0 then round(" + StatisticsFieldName + ",2) else 0 end ) as ��ľ����ϼ�," +
        //                          "sum(case when dl='114' and zysz like'7%' and ll>0 then " + ZSFieldName + " else 0 end) as ��ľ�����ϼ�," +
        //                          "sum(case when dl='114' and zysz like '7%' and ll<=3 then round(" + StatisticsFieldName + ",2) else 0 end ) as ��ľ��ǰ�����," +
        //                          "sum(case when dl='114' and zysz like'7%' and ll <=3 then " + ZSFieldName + " else 0 end ) as ��ľ��ǰ������," +
        //                          "sum(case when dl='114' and zysz like '7%' and ll between 3 and 6 then round(" + StatisticsFieldName + ",2) else 0 end) as ��ľ���������," +
        //                          "sum(case when dl='114' and zysz like'7%' and ll between 3 and 6 then " + ZSFieldName + " else 0 end ) as ��ľ����������," +
        //                          "sum(case when dl='114' and zysz like '7%' and ll between 6 and 15 then round(" + StatisticsFieldName + ",2) end) as ��ľʢ�������," +
        //                          "sum(case when dl='114' and zysz like'7%' and ll between 6 and 15 then " + ZSFieldName + " else 0 end) as ��ľʢ��������," +
        //                          "sum(case when dl='114' and zysz like'7%' and ll >= 15 then round(" + StatisticsFieldName + ",2) else 0 end) as ��ľ˥�������," +
        //                          "sum(case when dl='114' and zysz like'7%' and ll>=15 then " + ZSFieldName + " else 0 end) as ��ľ˥��������," +
        //                          "sum(case when dl='114' and zysz like'9%' and ll >=0 then round(" + StatisticsFieldName + ",2) else 0 end ) as ��ľ�ϼ�," +
        //                          "sum(case when dl='114' and zysz like'9%' and ll<=3 then round(" + StatisticsFieldName + ",2) else 0 end) ��ľ��ǰ�����," +
        //                          "sum(case when dl='114' and zysz like'9%' and ll between 3 and 6 then round(" + StatisticsFieldName + ",2) else 0 end ) as ��ľ���������," +
        //                          "sum(case when dl='114' and zysz like'9%' and ll between 6 and 15 then round(" + StatisticsFieldName + ",2) else 0 end) as ��ľʢ�������," +
        //                          "sum(case when dl='114' and zysz like'9%' and ll>=15 then round(" + StatisticsFieldName + ",2) else 0 end) as ��ľ˥�������" +
        //                          "  from " + tableName +
        //                          "  where lmqs<>' ' and qi_yuan <>' ' and zysz between '702' and '704' and lc<>' '" +
        //                          "  group by lc ,rollup(lmqs),rollup(qi_yuan),rollup(zysz))";
        //            pWorkspace.ExecuteSQL(LC);
        //        }
        //    }
        //    catch (System.Exception ex)
        //    {
        //        return false;
        //    }
        //    return true;
        //}
        #endregion
        #region ��ľ��ͳ��
        private static bool XZQGML_Statistic(IFeatureClass pFeatureClass, string StatisticsFieldName)
        {
            if (pFeatureClass == null)
            {
                return false;
            }
            IWorkspace pWorkspace = null;
            string tableName = "";
            try
            {
                pWorkspace = pFeatureClass.FeatureDataset.Workspace;
                tableName = (pFeatureClass as IDataset).Name;
            }
            catch (Exception ex)
            { }
            DropTable(pWorkspace, "temp_GM");
            DropTable(pWorkspace, "EcosTable_GML");
            try
            {
                //������ʱ��
                pWorkspace.ExecuteSQL("create table temp_GM as select xian,xiang,shi,sheng," + StatisticsFieldName + ",qi_yuan,dl,gmgd,zysz,lmqs,lc from " + tableName);
                //������ʱ��
                pWorkspace.ExecuteSQL("update temp_GM set qi_yuan='��Ȼ' where qi_yuan like'1%'");
                pWorkspace.ExecuteSQL("update temp_GM set qi_yuan='�˹�' where qi_yuan like'2%'");
                string xianSQL = "create table EcosTable_GML as select xian as ͳ�Ƶ�λ,lmqs as ʹ��Ȩ," +
                                  " qi_yuan as ��Դ,zysz as ��������," +
                                  "sum(case when dl between '131' and '132' and gmgd>=30 then round(" + StatisticsFieldName + ",2) else 0 end ) as �ϼ�," +
                                  "sum(case when dl between '131' and '132' and  gmgd between 30 and 50 then round(" + StatisticsFieldName + ",2) else 0 end)  ��ϼ�," +
                                  "sum(case when dl between '131' and '132' and gmgd between 50 and 70 then round(" + StatisticsFieldName + ",2) else 0 end ) as �кϼ�," +
                                  "sum(case when dl between '131' and '132' and gmgd >70 then round(" + StatisticsFieldName + ",2) else 0 end ) as �ܺϼ�," +
                                  "sum(case when dl='131' and gmgd>=30 then round(" + StatisticsFieldName + ",2) else 0 end) as �����ر�涨��ľ��С��," +
                                  "sum(case when dl='131' and gmgd between 30 and 50 then round(" + StatisticsFieldName + ",2) else 0 end) as �����ر�涨��ľ����," +
                                  "sum(case when dl='131'and gmgd between 50 and 70 then round(" + StatisticsFieldName + ",2) else 0 end) as �����ر�涨��ľ����," +
                                  "sum(case when dl='131' and gmgd>70 then round(" + StatisticsFieldName + ",2) else 0 end) as �����ر�涨��ľ����," +
                                  "sum(case when dl='132' and gmgd >= 30 then round(" + StatisticsFieldName + ",2) else 0 end) as ������ľ��С��," +
                                  "sum(case when dl='132' and gmgd between 30 and 50 then round(" + StatisticsFieldName + ",2) else 0 end) as ������ľ����," +
                                  "sum(case when dl='132' and gmgd between 50 and 70 then round(" + StatisticsFieldName + ",2) else 0 end) as ������ľ����," +
                                  "sum(case when dl='132' and gmgd >= 70 then round(" + StatisticsFieldName + ",2) else 0 end ) as ������ľ����" +
                                  "  from temp_GM" +
                                  "  where qi_yuan <>' ' and zysz in('120','150','130','170')" +
                                  "  group  by xian ,rollup(lmqs),rollup(qi_yuan),rollup(zysz)";
                pWorkspace.ExecuteSQL(xianSQL);
                string xiangSQL = "insert into EcosTable_GML ( select substr(xiang,1,8) as ͳ�Ƶ�λ,lmqs as ʹ��Ȩ," +
                                  " qi_yuan as ��Դ,zysz as ��������," +
                                  "sum(case when dl between '131' and '132' and gmgd>=30 then round(" + StatisticsFieldName + ",2) else 0 end ) as �ϼ�," +
                                  "sum(case when dl between '131' and '132' and  gmgd between 30 and 50 then round(" + StatisticsFieldName + ",2) else 0 end)  ��ϼ�," +
                                  "sum(case when dl between '131' and '132' and gmgd between 50 and 70 then round(" + StatisticsFieldName + ",2) else 0 end ) as �кϼ�," +
                                  "sum(case when dl between '131' and '132' and gmgd >70 then round(" + StatisticsFieldName + ",2) else 0 end ) as �ܺϼ�," +
                                  "sum(case when dl='131' and gmgd>=30 then round(" + StatisticsFieldName + ",2) else 0 end) as �����ر�涨��ľ��С��," +
                                  "sum(case when dl='131' and gmgd between 30 and 50 then round(" + StatisticsFieldName + ",2) else 0 end) as �����ر�涨��ľ����," +
                                  "sum(case when dl='131'and gmgd between 50 and 70 then round(" + StatisticsFieldName + ",2) else 0 end) as �����ر�涨��ľ����," +
                                  "sum(case when dl='131' and gmgd>70 then round(" + StatisticsFieldName + ",2) else 0 end) as �����ر�涨��ľ����," +
                                  "sum(case when dl='132' and gmgd >= 30 then round(" + StatisticsFieldName + ",2) else 0 end) as ������ľ��С��," +
                                  "sum(case when dl='132' and gmgd between 30 and 50 then round(" + StatisticsFieldName + ",2) else 0 end) as ������ľ����," +
                                  "sum(case when dl='132' and gmgd between 50 and 70 then round(" + StatisticsFieldName + ",2) else 0 end) as ������ľ����," +
                                  "sum(case when dl='132' and gmgd >= 70 then round(" + StatisticsFieldName + ",2) else 0 end ) as ������ľ����" +
                                  "  from temp_GM" +
                                  "  where qi_yuan <>' ' and zysz in('120','150','130','170')" +
                                  "  group  by substr(xiang,1,8) ,rollup(lmqs),rollup(qi_yuan),rollup(zysz))";
                pWorkspace.ExecuteSQL(xiangSQL);
                string shiSQL = "insert into EcosTable_GML ( select substr(shi,1,4) as ͳ�Ƶ�λ,lmqs as ʹ��Ȩ," +
                                  "qi_yuan as ��Դ,zysz as ��������," +
                                  "sum(case when dl between '131' and '132' and gmgd>=30 then round(" + StatisticsFieldName + ",2) else 0 end ) as �ϼ�," +
                                  "sum(case when dl between '131' and '132' and  gmgd between 30 and 50 then round(" + StatisticsFieldName + ",2) else 0 end)  ��ϼ�," +
                                  "sum(case when dl between '131' and '132' and gmgd between 50 and 70 then round(" + StatisticsFieldName + ",2) else 0 end ) as �кϼ�," +
                                  "sum(case when dl between '131' and '132' and gmgd >70 then round(" + StatisticsFieldName + ",2) else 0 end ) as �ܺϼ�," +
                                  "sum(case when dl='131' and gmgd>=30 then round(" + StatisticsFieldName + ",2) else 0 end) as �����ر�涨��ľ��С��," +
                                  "sum(case when dl='131' and gmgd between 30 and 50 then round(" + StatisticsFieldName + ",2) else 0 end) as �����ر�涨��ľ����," +
                                  "sum(case when dl='131'and gmgd between 50 and 70 then round(" + StatisticsFieldName + ",2) else 0 end) as �����ر�涨��ľ����," +
                                  "sum(case when dl='131' and gmgd>70 then round(" + StatisticsFieldName + ",2) else 0 end) as �����ر�涨��ľ����," +
                                  "sum(case when dl='132' and gmgd >= 30 then round(" + StatisticsFieldName + ",2) else 0 end) as ������ľ��С��," +
                                  "sum(case when dl='132' and gmgd between 30 and 50 then round(" + StatisticsFieldName + ",2) else 0 end) as ������ľ����," +
                                  "sum(case when dl='132' and gmgd between 50 and 70 then round(" + StatisticsFieldName + ",2) else 0 end) as ������ľ����," +
                                  "sum(case when dl='132' and gmgd >= 70 then round(" + StatisticsFieldName + ",2) else 0 end ) as ������ľ����" +
                                  "  from temp_GM" +
                                  "  where qi_yuan <>' ' and zysz in('120','150','130','170')" +
                                  "  group  by substr(shi,1,4),rollup(lmqs) ,rollup(qi_yuan),rollup(zysz))";
                pWorkspace.ExecuteSQL(shiSQL);
                string shengSQL = "insert into EcosTable_GML ( select substr(sheng,1,2) as ͳ�Ƶ�λ,lmqs as ʹ��Ȩ," +
                                  " qi_yuan as ��Դ,zysz as ��������," +
                                  "sum(case when dl between '131' and '132' and gmgd>=30 then round(" + StatisticsFieldName + ",2) else 0 end ) as �ϼ�," +
                                  "sum(case when dl between '131' and '132' and  gmgd between 30 and 50 then round(" + StatisticsFieldName + ",2) else 0 end)  ��ϼ�," +
                                  "sum(case when dl between '131' and '132' and gmgd between 50 and 70 then round(" + StatisticsFieldName + ",2) else 0 end ) as �кϼ�," +
                                  "sum(case when dl between '131' and '132' and gmgd >70 then round(" + StatisticsFieldName + ",2) else 0 end ) as �ܺϼ�," +
                                  "sum(case when dl='131' and gmgd>=30 then round(" + StatisticsFieldName + ",2) else 0 end) as �����ر�涨��ľ��С��," +
                                  "sum(case when dl='131' and gmgd between 30 and 50 then round(" + StatisticsFieldName + ",2) else 0 end) as �����ر�涨��ľ����," +
                                  "sum(case when dl='131'and gmgd between 50 and 70 then round(" + StatisticsFieldName + ",2) else 0 end) as �����ر�涨��ľ����," +
                                  "sum(case when dl='131' and gmgd>70 then round(" + StatisticsFieldName + ",2) else 0 end) as �����ر�涨��ľ����," +
                                  "sum(case when dl='132' and gmgd >= 30 then round(" + StatisticsFieldName + ",2) else 0 end) as ������ľ��С��," +
                                  "sum(case when dl='132' and gmgd between 30 and 50 then round(" + StatisticsFieldName + ",2) else 0 end) as ������ľ����," +
                                  "sum(case when dl='132' and gmgd between 50 and 70 then round(" + StatisticsFieldName + ",2) else 0 end) as ������ľ����," +
                                  "sum(case when dl='132' and gmgd >= 70 then round(" + StatisticsFieldName + ",2) else 0 end ) as ������ľ����" +
                                  "  from temp_GM" +
                                  "  where qi_yuan <>' ' and zysz in('120','150','130','170')" +
                                  "  group  by substr(sheng,1,2) ,rollup(lmqs),rollup(qi_yuan),rollup(zysz))";
                pWorkspace.ExecuteSQL(shengSQL);
                string LC = "insert into EcosTable_GML (select lc as ͳ�Ƶ�λ,lmqs as ʹ��Ȩ" +
                              ",qi_yuan  as ��Դ,zysz as ��������," +
                              "sum(case when dl between '131' and '132' and gmgd>=30 then round(" + StatisticsFieldName + ",2) else 0 end ) as �ϼ�," +
                              "sum(case when dl between '131' and '132' and  gmgd between 30 and 50 then round(" + StatisticsFieldName + ",2) else 0 end)  ��ϼ�," +
                              "sum(case when dl between '131' and '132' and gmgd between 50 and 70 then round(" + StatisticsFieldName + ",2) else 0 end ) as �кϼ�," +
                              "sum(case when dl between '131' and '132' and gmgd >70 then round(" + StatisticsFieldName + ",2) else 0 end ) as �ܺϼ�," +
                              "sum(case when dl='131' and gmgd>=30 then round(" + StatisticsFieldName + ",2) else 0 end) as �����ر�涨��ľ��С��," +
                              "sum(case when dl='131' and gmgd between 30 and 50 then round(" + StatisticsFieldName + ",2) else 0 end) as �����ر�涨��ľ����," +
                              "sum(case when dl='131'and gmgd between 50 and 70 then round(" + StatisticsFieldName + ",2) else 0 end) as �����ر�涨��ľ����," +
                              "sum(case when dl='131' and gmgd>70 then round(" + StatisticsFieldName + ",2) else 0 end) as �����ر�涨��ľ����," +
                              "sum(case when dl='132' and gmgd >= 30 then round(" + StatisticsFieldName + ",2) else 0 end) as ������ľ��С��," +
                              "sum(case when dl='132' and gmgd between 30 and 50 then round(" + StatisticsFieldName + ",2) else 0 end) as ������ľ����," +
                              "sum(case when dl='132' and gmgd between 50 and 70 then round(" + StatisticsFieldName + ",2) else 0 end) as ������ľ����," +
                              "sum(case when dl='132' and gmgd >= 70 then round(" + StatisticsFieldName + ",2) else 0 end ) as ������ľ����" +
                              "  from temp_GM" +
                              "  where qi_yuan <>' ' and zysz in('120','150','130','170') and lc<>' '" +
                              "  group  by lc ,rollup(lmqs),rollup(qi_yuan),rollup(zysz))";
                pWorkspace.ExecuteSQL(LC);
                DropTable(pWorkspace, "temp_GM");
                pWorkspace.ExecuteSQL("alter table EcosTable_GML modify ʹ��Ȩ nvarchar2(10)");
                pWorkspace.ExecuteSQL("alter table EcosTable_GML modify �������� nvarchar2(10)");
                pWorkspace.ExecuteSQL("update EcosTable_GML set ʹ��Ȩ ='����' where ʹ��Ȩ='1'");
                pWorkspace.ExecuteSQL("update EcosTable_GML set ʹ��Ȩ='����' where ʹ��Ȩ='2'");
                pWorkspace.ExecuteSQL("update EcosTable_GML set ʹ��Ȩ='����' where ʹ��Ȩ='3'");
                pWorkspace.ExecuteSQL("update EcosTable_GML set ʹ��Ȩ='����' where ʹ��Ȩ='9'");
                pWorkspace.ExecuteSQL("update EcosTable_GML set ʹ��Ȩ='����' where ʹ��Ȩ='0'");
                pWorkspace.ExecuteSQL("update EcosTable_GML set ʹ��Ȩ='����' where ʹ��Ȩ='B'");
                Dictionary <string ,string > dicSZ=dicGetFieldValue(pWorkspace ,"�����ֵ�");
                UpadateStatistictable(pWorkspace, dicSZ, "EcosTable_GML", "��������");
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }
        //private static bool LCCML_Statistic(IFeatureClass pFeatureClass, string StatisticsFieldName)
        //{
        //    if (pFeatureClass == null)
        //    {
        //        return false;
        //    }
        //    IWorkspace pWorkspace = null;
        //    string tableName = "";
        //    try
        //    {
        //        pWorkspace = pFeatureClass.FeatureDataset.Workspace;
        //        tableName = (pFeatureClass as IDataset).Name;
        //    }
        //    catch (Exception ex)
        //    { }
        //    try
        //    {
        //        if (!ExistTable(pWorkspace, "EcosTable_GML"))
        //        {
        //            string LCSQL = "create table EcosTable_GML as select lc as ͳ�Ƶ�λ," +
        //                              "case when qi_yuan like'1%' then ' ��Ȼ' else '�˹�' end as ��Դ,zysz as ��������," +
        //                              "sum(case when dl between '131' and '132' and gmgd>=30 then round(" + StatisticsFieldName + ",2) else 0 end ) as �ϼ�," +
        //                              "sum(case when dl between '131' and '132' and  gmgd between 30 and 50 then round(" + StatisticsFieldName + ",2) else 0 end)  ��ϼ�," +
        //                              "sum(case when dl between '131' and '132' and gmgd between 50 and 70 then round(" + StatisticsFieldName + ",2) else 0 end ) as �кϼ�," +
        //                              "sum(case when dl between '131' and '132' and gmgd >70 then round(" + StatisticsFieldName + ",2) else 0 end ) as �ܺϼ�," +
        //                              "sum(case when dl='131' and gmgd>=30 then round(" + StatisticsFieldName + ",2) else 0 end) as �����ر�涨��ľ��С��," +
        //                              "sum(case when dl='131' and gmgd between 30 and 50 then round(" + StatisticsFieldName + ",2) else 0 end) as �����ر�涨��ľ����," +
        //                              "sum(case when dl='131'and gmgd between 50 and 70 then round(" + StatisticsFieldName + ",2) else 0 end) as �����ر�涨��ľ����," +
        //                              "sum(case when dl='131' and gmgd>70 then round(" + StatisticsFieldName + ",2) else 0 end) as �����ر�涨��ľ����," +
        //                              "sum(case when dl='132' and gmgd >= 30 then round(" + StatisticsFieldName + ",2) else 0 end) as ������ľ��С��," +
        //                              "sum(case when dl='132' and gmgd between 30 and 50 then round(" + StatisticsFieldName + ",2) else 0 end) as ������ľ����," +
        //                              "sum(case when dl='132' and gmgd between 50 and 70 then round(" + StatisticsFieldName + ",2) else 0 end) as ������ľ����," +
        //                              "sum(case when dl='132' and gmgd >= 70 then round(" + StatisticsFieldName + ",2) else 0 end ) as ������ľ����" +
        //                              "  from " + tableName +
        //                              "  where qi_yuan <>' ' and zysz in('120','150','130','170') and lc<>' '" +
        //                              "  group  by lc ,rollup(qi_yuan),rollup(zysz)";
        //            pWorkspace.ExecuteSQL(LCSQL);
        //        }
        //        else
        //        {
        //            string LC = "insert into EcosTable_GML (select lc as ͳ�Ƶ�λ," +
        //                              "case when qi_yuan like'1%' then ' ��Ȼ' else '�˹�' end as ��Դ,zysz as ��������," +
        //                              "sum(case when dl between '131' and '132' and gmgd>=30 then round(" + StatisticsFieldName + ",2) else 0 end ) as �ϼ�," +
        //                              "sum(case when dl between '131' and '132' and  gmgd between 30 and 50 then round(" + StatisticsFieldName + ",2) else 0 end)  ��ϼ�," +
        //                              "sum(case when dl between '131' and '132' and gmgd between 50 and 70 then round(" + StatisticsFieldName + ",2) else 0 end ) as �кϼ�," +
        //                              "sum(case when dl between '131' and '132' and gmgd >70 then round(" + StatisticsFieldName + ",2) else 0 end ) as �ܺϼ�," +
        //                              "sum(case when dl='131' and gmgd>=30 then round(" + StatisticsFieldName + ",2) else 0 end) as �����ر�涨��ľ��С��," +
        //                              "sum(case when dl='131' and gmgd between 30 and 50 then round(" + StatisticsFieldName + ",2) else 0 end) as �����ر�涨��ľ����," +
        //                              "sum(case when dl='131'and gmgd between 50 and 70 then round(" + StatisticsFieldName + ",2) else 0 end) as �����ر�涨��ľ����," +
        //                              "sum(case when dl='131' and gmgd>70 then round(" + StatisticsFieldName + ",2) else 0 end) as �����ر�涨��ľ����," +
        //                              "sum(case when dl='132' and gmgd >= 30 then round(" + StatisticsFieldName + ",2) else 0 end) as ������ľ��С��," +
        //                              "sum(case when dl='132' and gmgd between 30 and 50 then round(" + StatisticsFieldName + ",2) else 0 end) as ������ľ����," +
        //                              "sum(case when dl='132' and gmgd between 50 and 70 then round(" + StatisticsFieldName + ",2) else 0 end) as ������ľ����," +
        //                              "sum(case when dl='132' and gmgd >= 70 then round(" + StatisticsFieldName + ",2) else 0 end ) as ������ľ����" +
        //                              "  from " + tableName +
        //                              "  where qi_yuan <>' ' and zysz in('120','150','130','170') and lc<>' '" +
        //                              "  group  by lc ,rollup(qi_yuan),rollup(zysz))";
        //            pWorkspace.ExecuteSQL(LC);
        //        }
        //    }
        //    catch (System.Exception ex)
        //    {
        //        return false;
        //    }
        //    return true;
        //}
        #endregion
        #region �����������ͳ��
        private static bool XZQGLTDMJ_Statistic(IFeatureClass pFeatureClass, string StatisticsFieldName)
        {
            if (pFeatureClass == null)
            {
                return false;
            }
            IWorkspace pWorkspace = null;
            string tableName = "";
            try
            {
                pWorkspace = pFeatureClass.FeatureDataset.Workspace;
                tableName = (pFeatureClass as IDataset).Name;
            }
            catch (Exception ex)
            { }
            DropTable(pWorkspace, "tempTable_GLTDMJ");
            DropTable(pWorkspace, "EcosTable_GLTDMJ");
            try
            {
                //������ʱ��
                pWorkspace.ExecuteSQL("create table tempTable_GLTDMJ as select xian,xiang,shi,sheng,ld_qs,lz,dl,lc,sen_lin_lb,"+StatisticsFieldName+" from " + tableName + "");
                pWorkspace.ExecuteSQL("alter table tempTable_GLTDMJ  modify ld_qs NVARCHAR2(10)");
                pWorkspace.ExecuteSQL("update tempTable_GLTDMJ set ld_qs='�����ֵ�' where ld_qs like'1%'");
                pWorkspace.ExecuteSQL("update tempTable_GLTDMJ set ld_qs='�����ֵ�' where ld_qs like'2%' and ld_qs<>'20'");
                pWorkspace.ExecuteSQL("update tempTable_GLTDMJ set ld_qs='�����ֵ�' where ld_qs='20'");
                pWorkspace.ExecuteSQL("update tempTable_GLTDMJ set  sen_lin_lb='��̬������' where sen_lin_lb like '1%'");
                pWorkspace.ExecuteSQL("update tempTable_GLTDMJ set  sen_lin_lb='��Ʒ��' where sen_lin_lb like '2%'");
                //ͳ��SQL���
                string xianSQL = "create table EcosTable_GLTDMJ as select "+
                                "xian as ͳ�Ƶ�λ,"+
                                "ld_qs as ����ʹ��Ȩ,sen_lin_lb as ɭ�����," +
                                "sum(case when lz<>' 'then round(" + StatisticsFieldName + ",2) else 0 end) as �����," +
                                "sum(case when dl like '1%' then round(" + StatisticsFieldName + ",2) else 0 end) as �ֵغϼ�," +
                                "sum(case when dl between '111' and '113' then round(" + StatisticsFieldName + ",2) else 0 end) as ���ֵ�С��," +
                                "sum(case when dl between '111' and '112' then round(" + StatisticsFieldName + ",2) else 0 end) as ��ľ��С��," +
                                "sum(case when dl='111' then round(" + StatisticsFieldName + ",2) else 0 end) as ����," +
                                "sum(case when dl='112' then round(" + StatisticsFieldName + ",2) else 0 end ) as �콻��," +
                                "sum(case when dl='114' then round(" + StatisticsFieldName + ",2) else 0 end) as ������," +
                                "sum(case when dl='113' then round(" + StatisticsFieldName + ",2) else 0 end) as ����," +
                                "sum(case when dl='120' then round(" + StatisticsFieldName + ",2) else 0 end) as ���ֵ�," +
                                "sum(case when dl between '131' and '132' then round(" + StatisticsFieldName + ",2) else 0 end ) as ��ľ�ֵ�С��," +
                                "sum(case when dl='131' then round(" + StatisticsFieldName + ",2)  else 0 end) as �����ر�涨��ľ��," +
                                "sum(case when dl='132' then round(" + StatisticsFieldName + ",2) else 0 end) as ��ľ������," +
                                "sum(case when dl='133' then round(" + StatisticsFieldName + ",2)  else 0 end) as ������ľ��," +
                                "sum(case when dl between '141' and '142' then round(" + StatisticsFieldName + ",2) else 0 end) as δ�������ֵ�С��," +
                                "sum(case when dl='141' then round(" + StatisticsFieldName + ",2) else 0 end) as �˹�����δ���ֵ�," +
                                "sum(case when dl='142' then round(" + StatisticsFieldName + ",2) else 0 end) as ����δ���ֵ�," +
                                "sum(case when dl='150'then round(" + StatisticsFieldName + ",2) else 0 end) as ���Ե�," +
                                "sum(case when dl between '161' and '163' then round(" + StatisticsFieldName + ",2) else 0 end) as ����ľ�ֵ�С��," +
                                "sum(case when dl='161' then round(" + StatisticsFieldName + ",2) else 0 end) as �ɷ�����," +
                                "sum(case when dl='162' then round(" + StatisticsFieldName + ",2) else 0 end) as ���ռ���," +
                                "sum(case when dl='163' then round(" + StatisticsFieldName + ",2) else 0 end) as ��������ľ�ֵ�," +
                                "sum(case when dl between '171' and'174' then round(" + StatisticsFieldName + ",2) else 0 end) as ���ֵ�С��," +
                                "sum(case when dl='171' then round(" + StatisticsFieldName + ",2) else 0 end ) as ���ֻ�ɽ�ĵ�," +
                                "sum(case when dl='172' then round(" + StatisticsFieldName + ",2) else 0 end) as ����ɳ�ĵ�," +
                                "sum(case when dl='173' then round(" + StatisticsFieldName + ",2) else 0 end) as �������ֵ�," +
                                "sum(case when dl='174' then round(" + StatisticsFieldName + ",2) else 0 end) as �˸���," +
                                "sum(case when dl='180' then round(" + StatisticsFieldName + ",2) else 0 end) as ���������ֵ�," +
                                "sum(case when dl like '2%' then round(" + StatisticsFieldName + ",2) else 0 end ) as ���ֵ�," +
                                "sum(" + StatisticsFieldName + ") as ɭ�ָ�����,sum(" + StatisticsFieldName + ") as ��ľ�̻���" +
                                "  from tempTable_GLTDMJ" +
                                "  where dl<>' ' and sen_lin_lb<>' '" +
                                "  group by xian,rollup(ld_qs),rollup(sen_lin_lb)";
                pWorkspace.ExecuteSQL(xianSQL);
                string xiangSQL = "insert into EcosTable_GLTDMJ ( select " +
                                "substr(xiang,1,8) as ͳ�Ƶ�λ," +
                                "ld_qs as ����ʹ��Ȩ,sen_lin_lb as ɭ�����," +
                                "sum(case when lz<>' 'then round(" + StatisticsFieldName + ",2) else 0 end) as �����," +
                                "sum(case when dl like '1%' then round(" + StatisticsFieldName + ",2) else 0 end) as �ֵغϼ�," +
                                "sum(case when dl between '111' and '113' then round(" + StatisticsFieldName + ",2) else 0 end) as ���ֵ�С��," +
                                "sum(case when dl between '111' and '112' then round(" + StatisticsFieldName + ",2) else 0 end) as ��ľ��С��," +
                                "sum(case when dl='111' then round(" + StatisticsFieldName + ",2) else 0 end) as ����," +
                                "sum(case when dl='112' then round(" + StatisticsFieldName + ",2) else 0 end ) as �콻��," +
                                "sum(case when dl='114' then round(" + StatisticsFieldName + ",2) else 0 end) as ������," +
                                "sum(case when dl='113' then round(" + StatisticsFieldName + ",2) else 0 end) as ����," +
                                "sum(case when dl='120' then round(" + StatisticsFieldName + ",2) else 0 end) as ���ֵ�," +
                                "sum(case when dl between '131' and '132' then round(" + StatisticsFieldName + ",2) else 0 end ) as ��ľ�ֵ�С��," +
                                "sum(case when dl='131' then round(" + StatisticsFieldName + ",2)  else 0 end) as �����ر�涨��ľ��," +
                                "sum(case when dl='132' then round(" + StatisticsFieldName + ",2) else 0 end) as ��ľ������," +
                                "sum(case when dl='133' then round(" + StatisticsFieldName + ",2)  else 0 end) as ������ľ��," +
                                "sum(case when dl between '141' and '142' then round(" + StatisticsFieldName + ",2) else 0 end) as δ�������ֵ�С��," +
                                "sum(case when dl='141' then round(" + StatisticsFieldName + ",2) else 0 end) as �˹�����δ���ֵ�," +
                                "sum(case when dl='142' then round(" + StatisticsFieldName + ",2) else 0 end) as ����δ���ֵ�," +
                                "sum(case when dl='150'then round(" + StatisticsFieldName + ",2) else 0 end) as ���Ե�," +
                                "sum(case when dl between '161' and '163' then round(" + StatisticsFieldName + ",2) else 0 end) as ����ľ�ֵ�С��," +
                                "sum(case when dl='161' then round(" + StatisticsFieldName + ",2) else 0 end) as �ɷ�����," +
                                "sum(case when dl='162' then round(" + StatisticsFieldName + ",2) else 0 end) as ���ռ���," +
                                "sum(case when dl='163' then round(" + StatisticsFieldName + ",2) else 0 end) as ��������ľ�ֵ�," +
                                "sum(case when dl between '171' and'174' then round(" + StatisticsFieldName + ",2) else 0 end) as ���ֵ�С��," +
                                "sum(case when dl='171' then round(" + StatisticsFieldName + ",2) else 0 end ) as ���ֻ�ɽ�ĵ�," +
                                "sum(case when dl='172' then round(" + StatisticsFieldName + ",2) else 0 end) as ����ɳ�ĵ�," +
                                "sum(case when dl='173' then round(" + StatisticsFieldName + ",2) else 0 end) as �������ֵ�," +
                                "sum(case when dl='174' then round(" + StatisticsFieldName + ",2) else 0 end) as �˸���," +
                                "sum(case when dl='180' then round(" + StatisticsFieldName + ",2) else 0 end) as ���������ֵ�," +
                                "sum(case when dl like '2%' then round(" + StatisticsFieldName + ",2) else 0 end ) as ���ֵ�," +
                                "sum(" + StatisticsFieldName + ") as ɭ�ָ�����,sum(" + StatisticsFieldName + ") as ��ľ�̻���" +
                                "  from tempTable_GLTDMJ" +
                                "  where dl<>' 'and sen_lin_lb<>' '" +
                                "  group by substr(xiang,1,8),rollup(ld_qs),rollup(sen_lin_lb))";
                pWorkspace.ExecuteSQL(xiangSQL);
                string shiSQL = "insert into EcosTable_GLTDMJ ( select " +
                                "substr(shi,1,4) as ͳ�Ƶ�λ," +
                                "ld_qs as ����ʹ��Ȩ,sen_lin_lb as ɭ�����," +
                                "sum(case when lz<>' 'then round(" + StatisticsFieldName + ",2) else 0 end) as �����," +
                                "sum(case when dl like '1%' then round(" + StatisticsFieldName + ",2) else 0 end) as �ֵغϼ�," +
                                "sum(case when dl between '111' and '113' then round(" + StatisticsFieldName + ",2) else 0 end) as ���ֵ�С��," +
                                "sum(case when dl between '111' and '112' then round(" + StatisticsFieldName + ",2) else 0 end) as ��ľ��С��," +
                                "sum(case when dl='111' then round(" + StatisticsFieldName + ",2) else 0 end) as ����," +
                                "sum(case when dl='112' then round(" + StatisticsFieldName + ",2) else 0 end ) as �콻��," +
                                "sum(case when dl='114' then round(" + StatisticsFieldName + ",2) else 0 end) as ������," +
                                "sum(case when dl='113' then round(" + StatisticsFieldName + ",2) else 0 end) as ����," +
                                "sum(case when dl='120' then round(" + StatisticsFieldName + ",2) else 0 end) as ���ֵ�," +
                                "sum(case when dl between '131' and '132' then round(" + StatisticsFieldName + ",2) else 0 end ) as ��ľ�ֵ�С��," +
                                "sum(case when dl='131' then round(" + StatisticsFieldName + ",2)  else 0 end) as �����ر�涨��ľ��," +
                                "sum(case when dl='132' then round(" + StatisticsFieldName + ",2) else 0 end) as ��ľ������," +
                                "sum(case when dl='133' then round(" + StatisticsFieldName + ",2)  else 0 end) as ������ľ��," +
                                "sum(case when dl between '141' and '142' then round(" + StatisticsFieldName + ",2) else 0 end) as δ�������ֵ�С��," +
                                "sum(case when dl='141' then round(" + StatisticsFieldName + ",2) else 0 end) as �˹�����δ���ֵ�," +
                                "sum(case when dl='142' then round(" + StatisticsFieldName + ",2) else 0 end) as ����δ���ֵ�," +
                                "sum(case when dl='150'then round(" + StatisticsFieldName + ",2) else 0 end) as ���Ե�," +
                                "sum(case when dl between '161' and '163' then round(" + StatisticsFieldName + ",2) else 0 end) as ����ľ�ֵ�С��," +
                                "sum(case when dl='161' then round(" + StatisticsFieldName + ",2) else 0 end) as �ɷ�����," +
                                "sum(case when dl='162' then round(" + StatisticsFieldName + ",2) else 0 end) as ���ռ���," +
                                "sum(case when dl='163' then round(" + StatisticsFieldName + ",2) else 0 end) as ��������ľ�ֵ�," +
                                "sum(case when dl between '171' and'174' then round(" + StatisticsFieldName + ",2) else 0 end) as ���ֵ�С��," +
                                "sum(case when dl='171' then round(" + StatisticsFieldName + ",2) else 0 end ) as ���ֻ�ɽ�ĵ�," +
                                "sum(case when dl='172' then round(" + StatisticsFieldName + ",2) else 0 end) as ����ɳ�ĵ�," +
                                "sum(case when dl='173' then round(" + StatisticsFieldName + ",2) else 0 end) as �������ֵ�," +
                                "sum(case when dl='174' then round(" + StatisticsFieldName + ",2) else 0 end) as �˸���," +
                                "sum(case when dl='180' then round(" + StatisticsFieldName + ",2) else 0 end) as ���������ֵ�," +
                                "sum(case when dl like '2%' then round(" + StatisticsFieldName + ",2) else 0 end ) as ���ֵ�," +
                                "sum(" + StatisticsFieldName + ") as ɭ�ָ�����,sum(" + StatisticsFieldName + ") as ��ľ�̻���" +
                                "  from tempTable_GLTDMJ" +
                                "  where dl<>' 'and sen_lin_lb<>' '" +
                                "  group by substr(shi,1,4),rollup(ld_qs),rollup(sen_lin_lb))";
                pWorkspace.ExecuteSQL(shiSQL);
                string shengSQL = "insert into EcosTable_GLTDMJ ( select " +
                                "substr(sheng,1,2) as ͳ�Ƶ�λ," +
                                "ld_qs as ����ʹ��Ȩ,sen_lin_lb as ɭ�����," +
                                "sum(case when lz<>' 'then round(" + StatisticsFieldName + ",2) else 0 end) as �����," +
                                "sum(case when dl like '1%' then round(" + StatisticsFieldName + ",2) else 0 end) as �ֵغϼ�," +
                                "sum(case when dl between '111' and '113' then round(" + StatisticsFieldName + ",2) else 0 end) as ���ֵ�С��," +
                                "sum(case when dl between '111' and '112' then round(" + StatisticsFieldName + ",2) else 0 end) as ��ľ��С��," +
                                "sum(case when dl='111' then round(" + StatisticsFieldName + ",2) else 0 end) as ����," +
                                "sum(case when dl='112' then round(" + StatisticsFieldName + ",2) else 0 end ) as �콻��," +
                                "sum(case when dl='114' then round(" + StatisticsFieldName + ",2) else 0 end) as ������," +
                                "sum(case when dl='113' then round(" + StatisticsFieldName + ",2) else 0 end) as ����," +
                                "sum(case when dl='120' then round(" + StatisticsFieldName + ",2) else 0 end) as ���ֵ�," +
                                "sum(case when dl between '131' and '132' then round(" + StatisticsFieldName + ",2) else 0 end ) as ��ľ�ֵ�С��," +
                                "sum(case when dl='131' then round(" + StatisticsFieldName + ",2)  else 0 end) as �����ر�涨��ľ��," +
                                "sum(case when dl='132' then round(" + StatisticsFieldName + ",2) else 0 end) as ��ľ������," +
                                "sum(case when dl='133' then round(" + StatisticsFieldName + ",2)  else 0 end) as ������ľ��," +
                                "sum(case when dl between '141' and '142' then round(" + StatisticsFieldName + ",2) else 0 end) as δ�������ֵ�С��," +
                                "sum(case when dl='141' then round(" + StatisticsFieldName + ",2) else 0 end) as �˹�����δ���ֵ�," +
                                "sum(case when dl='142' then round(" + StatisticsFieldName + ",2) else 0 end) as ����δ���ֵ�," +
                                "sum(case when dl='150'then round(" + StatisticsFieldName + ",2) else 0 end) as ���Ե�," +
                                "sum(case when dl between '161' and '163' then round(" + StatisticsFieldName + ",2) else 0 end) as ����ľ�ֵ�С��," +
                                "sum(case when dl='161' then round(" + StatisticsFieldName + ",2) else 0 end) as �ɷ�����," +
                                "sum(case when dl='162' then round(" + StatisticsFieldName + ",2) else 0 end) as ���ռ���," +
                                "sum(case when dl='163' then round(" + StatisticsFieldName + ",2) else 0 end) as ��������ľ�ֵ�," +
                                "sum(case when dl between '171' and'174' then round(" + StatisticsFieldName + ",2) else 0 end) as ���ֵ�С��," +
                                "sum(case when dl='171' then round(" + StatisticsFieldName + ",2) else 0 end ) as ���ֻ�ɽ�ĵ�," +
                                "sum(case when dl='172' then round(" + StatisticsFieldName + ",2) else 0 end) as ����ɳ�ĵ�," +
                                "sum(case when dl='173' then round(" + StatisticsFieldName + ",2) else 0 end) as �������ֵ�," +
                                "sum(case when dl='174' then round(" + StatisticsFieldName + ",2) else 0 end) as �˸���," +
                                "sum(case when dl='180' then round(" + StatisticsFieldName + ",2) else 0 end) as ���������ֵ�," +
                                "sum(case when dl like '2%' then round(" + StatisticsFieldName + ",2) else 0 end ) as ���ֵ�," +
                                "sum(" + StatisticsFieldName + ") as ɭ�ָ�����,sum(" + StatisticsFieldName + ") as ��ľ�̻���" +
                                "  from tempTable_GLTDMJ" +
                                "  where dl<>' 'and sen_lin_lb<>' '" +
                                "  group by substr(sheng,1,2),rollup(ld_qs),rollup(sen_lin_lb))";
                pWorkspace.ExecuteSQL(shengSQL);
                string LCSQL = "insert into EcosTable_GLTDMJ ( select " +
                                "lc as ͳ�Ƶ�λ," +
                                "ld_qs as ����ʹ��Ȩ,sen_lin_lb as ɭ�����," +
                                "sum(case when lz<>' 'then round(" + StatisticsFieldName + ",2) else 0 end) as �����," +
                                "sum(case when dl like '1%' then round(" + StatisticsFieldName + ",2) else 0 end) as �ֵغϼ�," +
                                "sum(case when dl between '111' and '113' then round(" + StatisticsFieldName + ",2) else 0 end) as ���ֵ�С��," +
                                "sum(case when dl between '111' and '112' then round(" + StatisticsFieldName + ",2) else 0 end) as ��ľ��С��," +
                                "sum(case when dl='111' then round(" + StatisticsFieldName + ",2) else 0 end) as ����," +
                                "sum(case when dl='112' then round(" + StatisticsFieldName + ",2) else 0 end ) as �콻��," +
                                "sum(case when dl='114' then round(" + StatisticsFieldName + ",2) else 0 end) as ������," +
                                "sum(case when dl='113' then round(" + StatisticsFieldName + ",2) else 0 end) as ����," +
                                "sum(case when dl='120' then round(" + StatisticsFieldName + ",2) else 0 end) as ���ֵ�," +
                                "sum(case when dl between '131' and '132' then round(" + StatisticsFieldName + ",2) else 0 end ) as ��ľ�ֵ�С��," +
                                "sum(case when dl='131' then round(" + StatisticsFieldName + ",2)  else 0 end) as �����ر�涨��ľ��," +
                                "sum(case when dl='132' then round(" + StatisticsFieldName + ",2) else 0 end) as ��ľ������," +
                                "sum(case when dl='133' then round(" + StatisticsFieldName + ",2)  else 0 end) as ������ľ��," +
                                "sum(case when dl between '141' and '142' then round(" + StatisticsFieldName + ",2) else 0 end) as δ�������ֵ�С��," +
                                "sum(case when dl='141' then round(" + StatisticsFieldName + ",2) else 0 end) as �˹�����δ���ֵ�," +
                                "sum(case when dl='142' then round(" + StatisticsFieldName + ",2) else 0 end) as ����δ���ֵ�," +
                                "sum(case when dl='150'then round(" + StatisticsFieldName + ",2) else 0 end) as ���Ե�," +
                                "sum(case when dl between '161' and '163' then round(" + StatisticsFieldName + ",2) else 0 end) as ����ľ�ֵ�С��," +
                                "sum(case when dl='161' then round(" + StatisticsFieldName + ",2) else 0 end) as �ɷ�����," +
                                "sum(case when dl='162' then round(" + StatisticsFieldName + ",2) else 0 end) as ���ռ���," +
                                "sum(case when dl='163' then round(" + StatisticsFieldName + ",2) else 0 end) as ��������ľ�ֵ�," +
                                "sum(case when dl between '171' and'174' then round(" + StatisticsFieldName + ",2) else 0 end) as ���ֵ�С��," +
                                "sum(case when dl='171' then round(" + StatisticsFieldName + ",2) else 0 end ) as ���ֻ�ɽ�ĵ�," +
                                "sum(case when dl='172' then round(" + StatisticsFieldName + ",2) else 0 end) as ����ɳ�ĵ�," +
                                "sum(case when dl='173' then round(" + StatisticsFieldName + ",2) else 0 end) as �������ֵ�," +
                                "sum(case when dl='174' then round(" + StatisticsFieldName + ",2) else 0 end) as �˸���," +
                                "sum(case when dl='180' then round(" + StatisticsFieldName + ",2) else 0 end) as ���������ֵ�," +
                                "sum(case when dl like '2%' then round(" + StatisticsFieldName + ",2) else 0 end ) as ���ֵ�," +
                                "sum(" + StatisticsFieldName + ") as ɭ�ָ�����,sum(" + StatisticsFieldName + ") as ��ľ�̻���" +
                                "  from tempTable_GLTDMJ" +
                                "  where dl<>' 'and sen_lin_lb<>' ' and lc<>' '" +
                                "  group by lc,rollup(ld_qs),rollup(sen_lin_lb))";
                pWorkspace.ExecuteSQL(LCSQL);
                DropTable(pWorkspace, "tempTable_GLTDMJ");
                //����ɭ�ָ����ʺ���ҵ������
                pWorkspace.ExecuteSQL("update EcosTable_GLTDMJ set ɭ�ָ�����=0 ");
                pWorkspace.ExecuteSQL("update EcosTable_GLTDMJ set ��ľ�̻���=0 ");
                pWorkspace.ExecuteSQL("update EcosTable_GLTDMJ set ɭ�ָ�����=round(((��ľ��С��+�����ر�涨��ľ��)/�����)*100 ,2) where ����ʹ��Ȩ is null and ɭ����� is null");
                pWorkspace.ExecuteSQL("update EcosTable_GLTDMJ set ��ľ�̻���=round(((��ľ��С��+��ľ�ֵ�С��)/�����)*100,2) where ����ʹ��Ȩ is null and ɭ����� is null");

              //  UpdateTotal(pWorkspace, "����ʹ��Ȩ", "�ϼ�", "EcosTable_GLTDMJ");
               // UpdateTotal(pWorkspace, "ɭ�����", "С��", "EcosTable_GLTDMJ");
            }
            catch (System.Exception ex)
            {
                return false;
            }
            return true;
        }
        #endregion
        #region ����ɭ�֡���ľ������ͳ��
        private static bool XZQGLSLMJ_Statistic(IFeatureClass pFeatureClass, string strXJFielldName,string ZSFieldName, string StatisticsFieldName)
        {
            if (pFeatureClass == null)
            {
                return false;
            }
            IWorkspace pWorkspace = null;
            string tableName = "";
            try
            {
                pWorkspace = pFeatureClass.FeatureDataset.Workspace;
                tableName = (pFeatureClass as IDataset).Name;
            }
            catch (Exception ex)
            { }
            DropTable(pWorkspace, "EcosTable_GLSLMJ");
            try
            {
                string xianSQL = "create table EcosTable_GLSLMJ as select  xian as ͳ�Ƶ�λ,lmqs as ��ľʹ��Ȩ," +
                                    "sum (case when (dl= '142'or dl='141'or dl='120'or dl='114' or dl='112'or dl='111') then round(" + strXJFielldName + ",2) else 0 end ) as ����ľ�������," +
                                    "sum( case when dl between '111' and '114' then  round(" + StatisticsFieldName + ",2) else 0 end) as ���ֵ�����ϼ�," +
                                    "sum(case when dl between '111' and '112' then  round(" + StatisticsFieldName + ",2) else 0 end ) as ��ľ�����С��," +
                                    "sum(case when dl between '111' and '112' then round(" + strXJFielldName + ",2) else 0 end) as ��ľ�����С��," +
                                    "sum(case when dl ='111' then round(" + StatisticsFieldName + ",2) else 0 end ) as �������," +
                                    "sum( case when dl='111' then round(" + strXJFielldName + ",2) else 0 end) as �������," +
                                    "sum( case when dl='112' then round(" + StatisticsFieldName + ",2) else 0 end) as �콻�����," +
                                    "sum(case when dl='112' then round(" + strXJFielldName + ",2) else 0 end) as �콻�����," +
                                    "sum(case when dl='114' then round(" + StatisticsFieldName + ",2) else 0 end) as ���������," +
                                    "sum(case when dl='114' then round (" + strXJFielldName + ",2) else 0 end) as  ���������," +
                                    "sum(case when dl='113' then  round(" + StatisticsFieldName + ",2) else 0 end) as �������," +
                                    "sum(case when dl='113' then " + ZSFieldName + " else 0 end)/10000 as ��������," +
                                    "sum(case when dl='120' then round(" + StatisticsFieldName + ",2) else 0 end) as  ���ֵ����," +
                                    "sum(case when dl='120' then round(" + strXJFielldName + ",2) else 0 end) as ���ֵ����," +
                                    "sum(case when dl='141' then  " + ZSFieldName + "  else 0 end)/10000 as ����������," +
                                    "sum(case when dl='141' then round(" + strXJFielldName + ",2) else 0 end) as ���������," +
                                    "sum(case when dl='142' then  " + ZSFieldName + "  else 0 end)/10000 as ɢ��ľ����," +
                                    "sum(case when dl='142' then round(" + strXJFielldName + ",2) else 0 end) as ɢ��ľ���" +
                                    " from " + tableName +
                                    " where lmqs<>' '" +
                                    " group by xian,rollup(lmqs)";
                pWorkspace.ExecuteSQL(xianSQL);
                string xiangSQL = "insert into EcosTable_GLSLMJ ( select  substr(xiang,1,8) as ͳ�Ƶ�λ,lmqs as ��ľʹ��Ȩ," +
                                    "sum (case when (dl= '142'or dl='141'or dl='120'or dl='114' or dl='112'or dl='111') then round(" + strXJFielldName + ",2) else 0 end ) as ����ľ�������," +
                                    "sum( case when dl between '111' and '114' then  round(" + StatisticsFieldName + ",2) else 0 end) as ���ֵ�����ϼ�," +
                                    "sum(case when dl between '111' and '112' then  round(" + StatisticsFieldName + ",2) else 0 end ) as ��ľ�����С��," +
                                    "sum(case when dl between '111' and '112' then round(" + strXJFielldName + ",2) else 0 end) as ��ľ�����С��," +
                                    "sum(case when dl ='111' then round(" + StatisticsFieldName + ",2) else 0 end ) as �������," +
                                    "sum( case when dl='111' then round(" + strXJFielldName + ",2) else 0 end) as �������," +
                                    "sum( case when dl='112' then round(" + StatisticsFieldName + ",2) else 0 end) as �콻�����," +
                                    "sum(case when dl='112' then round(" + strXJFielldName + ",2) else 0 end) as �콻�����," +
                                    "sum(case when dl='114' then round(" + StatisticsFieldName + ",2) else 0 end) as ���������," +
                                    "sum(case when dl='114' then round (" + strXJFielldName + ",2) else 0 end) as  ���������," +
                                    "sum(case when dl='113' then  round(" + StatisticsFieldName + ",2) else 0 end) as �������," +
                                    "sum(case when dl='113' then " + ZSFieldName + " else 0 end)/10000 as ��������," +
                                    "sum(case when dl='120' then round(" + StatisticsFieldName + ",2) else 0 end) as  ���ֵ����," +
                                    "sum(case when dl='120' then round(" + strXJFielldName + ",2) else 0 end) as ���ֵ����," +
                                    "sum(case when dl='141' then  " + ZSFieldName + "  else 0 end)/10000 as ����������," +
                                    "sum(case when dl='141' then round(" + strXJFielldName + ",2) else 0 end) as ���������," +
                                    "sum(case when dl='142' then  " + ZSFieldName + "  else 0 end)/10000 as ɢ��ľ����," +
                                    "sum(case when dl='142' then round(" + strXJFielldName + ",2) else 0 end) as ɢ��ľ���" +
                                    " from " + tableName +
                                    " where lmqs<>' '" +
                                    " group by substr(xiang,1,8),rollup(lmqs))";
                pWorkspace.ExecuteSQL(xiangSQL);
                string shiSQL = "insert into EcosTable_GLSLMJ ( select  substr(shi,1,4) as ͳ�Ƶ�λ,lmqs as ��ľʹ��Ȩ," +
                                    "sum (case when (dl= '142'or dl='141'or dl='120'or dl='114' or dl='112'or dl='111') then round(" + strXJFielldName + ",2) else 0 end ) as ����ľ�������," +
                                    "sum( case when dl between '111' and '114' then  round(" + StatisticsFieldName + ",2) else 0 end) as ���ֵ�����ϼ�," +
                                    "sum(case when dl between '111' and '112' then  round(" + StatisticsFieldName + ",2) else 0 end ) as ��ľ�����С��," +
                                    "sum(case when dl between '111' and '112' then round(" + strXJFielldName + ",2) else 0 end) as ��ľ�����С��," +
                                    "sum(case when dl ='111' then round(" + StatisticsFieldName + ",2) else 0 end ) as �������," +
                                    "sum( case when dl='111' then round(" + strXJFielldName + ",2) else 0 end) as �������," +
                                    "sum( case when dl='112' then round(" + StatisticsFieldName + ",2) else 0 end) as �콻�����," +
                                    "sum(case when dl='112' then round(" + strXJFielldName + ",2) else 0 end) as �콻�����," +
                                    "sum(case when dl='114' then round(" + StatisticsFieldName + ",2) else 0 end) as ���������," +
                                    "sum(case when dl='114' then round (" + strXJFielldName + ",2) else 0 end) as  ���������," +
                                    "sum(case when dl='113' then  round(" + StatisticsFieldName + ",2) else 0 end) as �������," +
                                    "sum(case when dl='113' then " + ZSFieldName + " else 0 end)/10000 as ��������," +
                                    "sum(case when dl='120' then round(" + StatisticsFieldName + ",2) else 0 end) as  ���ֵ����," +
                                    "sum(case when dl='120' then round(" + strXJFielldName + ",2) else 0 end) as ���ֵ����," +
                                    "sum(case when dl='141' then  " + ZSFieldName + "  else 0 end)/10000 as ����������," +
                                    "sum(case when dl='141' then round(" + strXJFielldName + ",2) else 0 end) as ���������," +
                                    "sum(case when dl='142' then  " + ZSFieldName + "  else 0 end)/10000 as ɢ��ľ����," +
                                    "sum(case when dl='142' then round(" + strXJFielldName + ",2) else 0 end) as ɢ��ľ���" +
                                    " from " + tableName +
                                    " where lmqs<>' '" +
                                    " group by substr(shi,1,4),rollup(lmqs))";
                pWorkspace.ExecuteSQL(shiSQL);
                string shengSQL = "insert into EcosTable_GLSLMJ ( select  substr(sheng,1,2) as ͳ�Ƶ�λ,lmqs as ��ľʹ��Ȩ," +
                                    "sum (case when (dl= '142'or dl='141'or dl='120'or dl='114' or dl='112'or dl='111') then round(" + strXJFielldName + ",2) else 0 end ) as ����ľ�������," +
                                    "sum( case when dl between '111' and '114' then  round(" + StatisticsFieldName + ",2) else 0 end) as ���ֵ�����ϼ�," +
                                    "sum(case when dl between '111' and '112' then  round(" + StatisticsFieldName + ",2) else 0 end ) as ��ľ�����С��," +
                                    "sum(case when dl between '111' and '112' then round(" + strXJFielldName + ",2) else 0 end) as ��ľ�����С��," +
                                    "sum(case when dl ='111' then round(" + StatisticsFieldName + ",2) else 0 end ) as �������," +
                                    "sum( case when dl='111' then round(" + strXJFielldName + ",2) else 0 end) as �������," +
                                    "sum( case when dl='112' then round(" + StatisticsFieldName + ",2) else 0 end) as �콻�����," +
                                    "sum(case when dl='112' then round(" + strXJFielldName + ",2) else 0 end) as �콻�����," +
                                    "sum(case when dl='114' then round(" + StatisticsFieldName + ",2) else 0 end) as ���������," +
                                    "sum(case when dl='114' then round (" + strXJFielldName + ",2) else 0 end) as  ���������," +
                                    "sum(case when dl='113' then  round(" + StatisticsFieldName + ",2) else 0 end) as �������," +
                                    "sum(case when dl='113' then " + ZSFieldName + " else 0 end)/10000 as ��������," +
                                    "sum(case when dl='120' then round(" + StatisticsFieldName + ",2) else 0 end) as  ���ֵ����," +
                                    "sum(case when dl='120' then round(" + strXJFielldName + ",2) else 0 end) as ���ֵ����," +
                                    "sum(case when dl='141' then  " + ZSFieldName + "  else 0 end)/10000 as ����������," +
                                    "sum(case when dl='141' then round(" + strXJFielldName + ",2) else 0 end) as ���������," +
                                    "sum(case when dl='142' then  " + ZSFieldName + "  else 0 end)/10000 as ɢ��ľ����," +
                                    "sum(case when dl='142' then round(" + strXJFielldName + ",2) else 0 end) as ɢ��ľ���" +
                                    " from " + tableName +
                                    " where lmqs<>' '" +
                                    " group by substr(sheng,1,2),rollup(lmqs))";
                pWorkspace.ExecuteSQL(shengSQL);
                string lcSQL = "insert into EcosTable_GLSLMJ ( select  lc as ͳ�Ƶ�λ,lmqs as ��ľʹ��Ȩ," +
                                    "sum (case when (dl= '142'or dl='141'or dl='120'or dl='114' or dl='112'or dl='111') then round(" + strXJFielldName + ",2) else 0 end ) as ����ľ�������," +
                                    "sum( case when dl between '111' and '114' then  round(" + StatisticsFieldName + ",2) else 0 end) as ���ֵ�����ϼ�," +
                                    "sum(case when dl between '111' and '112' then  round(" + StatisticsFieldName + ",2) else 0 end ) as ��ľ�����С��," +
                                    "sum(case when dl between '111' and '112' then round(" + strXJFielldName + ",2) else 0 end) as ��ľ�����С��," +
                                    "sum(case when dl ='111' then round(" + StatisticsFieldName + ",2) else 0 end ) as �������," +
                                    "sum( case when dl='111' then round(" + strXJFielldName + ",2) else 0 end) as �������," +
                                    "sum( case when dl='112' then round(" + StatisticsFieldName + ",2) else 0 end) as �콻�����," +
                                    "sum(case when dl='112' then round(" + strXJFielldName + ",2) else 0 end) as �콻�����," +
                                    "sum(case when dl='114' then round(" + StatisticsFieldName + ",2) else 0 end) as ���������," +
                                    "sum(case when dl='114' then round (" + strXJFielldName + ",2) else 0 end) as  ���������," +
                                    "sum(case when dl='113' then  round(" + StatisticsFieldName + ",2) else 0 end) as �������," +
                                    "sum(case when dl='113' then " + ZSFieldName + " else 0 end)/10000 as ��������," +
                                    "sum(case when dl='120' then round(" + StatisticsFieldName + ",2) else 0 end) as  ���ֵ����," +
                                    "sum(case when dl='120' then round(" + strXJFielldName + ",2) else 0 end) as ���ֵ����," +
                                    "sum(case when dl='141' then  " + ZSFieldName + "  else 0 end)/10000 as ����������," +
                                    "sum(case when dl='141' then round(" + strXJFielldName + ",2) else 0 end) as ���������," +
                                    "sum(case when dl='142' then  " + ZSFieldName + "  else 0 end)/10000 as ɢ��ľ����," +
                                    "sum(case when dl='142' then round(" + strXJFielldName + ",2) else 0 end) as ɢ��ľ���" +
                                    " from " + tableName +
                                    " where lmqs<>' '" +
                                    " group by lc,rollup(lmqs))";
                pWorkspace.ExecuteSQL(lcSQL);
                //���±�
                pWorkspace.ExecuteSQL("alter table EcosTable_GLSLMJ modify ��ľʹ��Ȩ nvarchar2(10)");
                pWorkspace.ExecuteSQL("update EcosTable_GLSLMJ set ��ľʹ��Ȩ ='����' where ��ľʹ��Ȩ='1'");
                pWorkspace.ExecuteSQL("update EcosTable_GLSLMJ set ��ľʹ��Ȩ='����' where ��ľʹ��Ȩ='2'");
                pWorkspace.ExecuteSQL("update EcosTable_GLSLMJ set ��ľʹ��Ȩ='����' where ��ľʹ��Ȩ='3'");
                pWorkspace.ExecuteSQL("update EcosTable_GLSLMJ set ��ľʹ��Ȩ='����' where ��ľʹ��Ȩ='9'");
            }
            catch (System.Exception ex)
            {
                return false;
            }
            return true;
        }
        #endregion
        //���ݸ���
        private static bool UpdateStatistictable(IWorkspace pWorkspace, string tableName, string FieldName, string newValue,string oldValue)
        {
            if (pWorkspace == null)
                return false;
            if (tableName == "") return false;
            try
            {
                string strSQL = "update " + tableName + " set " + FieldName + "='" + newValue + "' where  " + FieldName + "='" + oldValue + "'";
                pWorkspace.ExecuteSQL(strSQL);
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }
        //���ֶκ���
        private static bool UpadateStatistictable(IWorkspace pWorkspace, Dictionary<string, string> dicField, string tableName, string FieldName)
        {
            bool flag = false;
            if (pWorkspace == null)
                return false;
            if (tableName == "") return false;
            if (dicField == null || dicField.Count == 0) return false;
            foreach (string key in dicField.Keys)
            {
                flag = UpdateStatistictable(pWorkspace, tableName, FieldName, dicField[key], key);
            }
            return flag;
        }
        //�����ϼ�
        private static bool UpdateTotal(IWorkspace pWorkspace, string FieldName, string newvalue,string tableName)
        {
            if (pWorkspace == null || tableName == "" || FieldName == "")
            {
                return false;
            }
            try
            {
                pWorkspace.ExecuteSQL("update "+tableName +" set "+FieldName +"='"+newvalue+"' where "+FieldName +" is null");
            }
            catch (System.Exception ex)
            {
                return false;
            }
            return true;
        }
        private static Dictionary<string, string> dicGetFieldValue(IWorkspace pWorkspace,string taleName)
        {
            if (pWorkspace == null) return null;
            Dictionary<string, string> newdic = new Dictionary<string, string>();
            try
            {
                ITable pTable = (pWorkspace as IFeatureWorkspace).OpenTable(taleName);
                ICursor pCursor = pTable.Search(null, false);
                IRow pRow = pCursor.NextRow();
                int codeIndex=0;
                int NameIndex=0;
                if(pRow !=null)
                {
                    codeIndex=pRow .Fields .FindField ("Code");
                    NameIndex =pRow .Fields .FindField ("Name");
                }
                while (pRow != null)
                {
                    newdic.Add(pRow .get_Value (codeIndex).ToString (),pRow .get_Value (NameIndex).ToString ());
                    pRow = pCursor.NextRow();
                }
                System.Runtime.InteropServices.Marshal.ReleaseComObject(pCursor);
                
            }
            catch(Exception ex)
            {
                return null;
            }
            return newdic;
        }
        //���и�����Ϣͳ�� ygc 2013-01-16
        public static void DoReportStatistic(IFeatureClass pFeatureClass, string strMjFieldName,string strxjlFieldName,SysCommon.CProgress pProgress)
        {
                //������Ϣͳ��
            pProgress.SetProgress(1, "���б�����Ϣͳ��");
            bool bRes = false;
            bRes = BHXX_Statistic(pFeatureClass, strMjFieldName);
            //���е�����Ϣͳ��
            pProgress.SetProgress(2, "���е���Ϣͳ��");
            bRes = DLXX_Statistic(pFeatureClass, strMjFieldName);
            //���������Ϣ
            pProgress.SetProgress(3, "���й��������Ϣͳ��");
            bRes = GCLBXX_Statistic(pFeatureClass, strMjFieldName);
            //������Ϣͳ��
            pProgress.SetProgress(4, "���л�����Ϣͳ��");
            bRes = JBXX_Statistic(pFeatureClass, strMjFieldName,strxjlFieldName);
            //�ֵؽṹ��Ϣͳ��
            pProgress.SetProgress(5, "�����ֵؽṹ��Ϣͳ��");
            bRes = LDJGXX_Statistic(pFeatureClass, strMjFieldName);
            //������Ϣͳ��
            pProgress.SetProgress(6, "����������Ϣͳ��");
            bRes = LZXX_Statistic(pFeatureClass, strMjFieldName);
            //��Դ��Ϣͳ��
            pProgress.SetProgress(7, "������Դ��Ϣͳ��");
            bRes = QYXX_Statistic(pFeatureClass, strMjFieldName);
            //Ȩ����Ϣͳ��
            pProgress.SetProgress(8, "����Ȩ����Ϣͳ��");
            bRes = QSXX_Statistic(pFeatureClass, strMjFieldName);
            //�ֺ��ȼ�
            pProgress.SetProgress(9, "�����ֺ��ȼ���Ϣͳ��");
            bRes = ZHDJ_Statistic(pFeatureClass, strMjFieldName);
            //�ֺ�����
            pProgress.SetProgress(10, "�����ֺ�������Ϣͳ��");
            bRes = ZHLX_Statistic(pFeatureClass, strMjFieldName);
            //�����ȼ���Ϣͳ��
            pProgress.SetProgress(11, "���������ȼ���Ϣͳ��");
            bRes = ZLDJ_Statistic(pFeatureClass, strMjFieldName);
            //��Ҫ������Ϣͳ��
            pProgress.SetProgress(12, "������Ҫ������Ϣͳ��");
            bRes = ZYSZ_Statistic(pFeatureClass, strMjFieldName);
        }
        //������Ϣͳ�� ygc 2013-01-16
        private static bool BHXX_Statistic(IFeatureClass pFeatureClass, string strMjFieldName)
        {
            if (pFeatureClass == null)
            {
                return false;
            }
            IWorkspace pWorkspace = null;
            string tableName = "";
            try
            {
                pWorkspace = pFeatureClass.FeatureDataset.Workspace;
                tableName = (pFeatureClass as IDataset).Name;
            }
            catch (Exception ex)
            { }
            DropTable(pWorkspace, "DATA_BHXX");
            //ͨ��SQL������ͳ��
            try
            {
                string cunSQL = "create table DATA_BHXX as select substr(cun,1,10) as ͳ�Ƶ�λ,"+
                                    "sum(case when bhdj <>' ' then round("+strMjFieldName+",2) else 0 end) as �ϼ�," +
                                    "sum(case when bhdj='1' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                    "sum(case when bhdj='2' then round(" + strMjFieldName + ",2) else 0 end) as �ص�," +
                                    "sum(case when bhdj='3' then round(" + strMjFieldName + ",2) else 0 end) as һ��  " +
                                    " from  "+tableName +
                                    " group by substr(cun,1,10)";
                pWorkspace.ExecuteSQL(cunSQL);
                string shengSQL = "insert into DATA_BHXX (select substr(sheng,1,2) as ͳ�Ƶ�λ," +
                                    "sum(case when bhdj <>' ' then round(" + strMjFieldName + ",2) else 0 end) as �ϼ�," +
                                    "sum(case when bhdj='1' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                    "sum(case when bhdj='2' then round(" + strMjFieldName + ",2) else 0 end) as �ص�," +
                                    "sum(case when bhdj='3' then round(" + strMjFieldName + ",2) else 0 end) as һ��  " +
                                    " from "+tableName  +
                                    " group by substr(sheng,1,2))";
                pWorkspace.ExecuteSQL(shengSQL);
                string shiSQL = "insert into DATA_BHXX (select substr(shi,1,4) as ͳ�Ƶ�λ," +
                                    "sum(case when bhdj <>' ' then round(" + strMjFieldName + ",2) else 0 end) as �ϼ�," +
                                    "sum(case when bhdj='1' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                    "sum(case when bhdj='2' then round(" + strMjFieldName + ",2) else 0 end) as �ص�," +
                                    "sum(case when bhdj='3' then round(" + strMjFieldName + ",2) else 0 end) as һ��  " +
                                    " from  " + tableName +
                                    " group by substr(shi,1,4))";
                pWorkspace.ExecuteSQL(shiSQL);
                string xianSQL = "insert into DATA_BHXX (select substr(xian,1,6) as ͳ�Ƶ�λ," +
                                    "sum(case when bhdj <>' ' then round(" + strMjFieldName + ",2) else 0 end) as �ϼ�," +
                                    "sum(case when bhdj='1' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                    "sum(case when bhdj='2' then round(" + strMjFieldName + ",2) else 0 end) as �ص�," +
                                    "sum(case when bhdj='3' then round(" + strMjFieldName + ",2) else 0 end) as һ��  " +
                                    " from " + tableName +
                                    " group by substr(xian,1,6))";
                pWorkspace.ExecuteSQL(xianSQL);
                string xiangSQL = "insert into DATA_BHXX (select substr(xiang,1,8) as ͳ�Ƶ�λ," +
                                    "sum(case when bhdj <>' ' then round(" + strMjFieldName + ",2) else 0 end) as �ϼ�," +
                                    "sum(case when bhdj='1' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                    "sum(case when bhdj='2' then round(" + strMjFieldName + ",2) else 0 end) as �ص�," +
                                    "sum(case when bhdj='3' then round(" + strMjFieldName + ",2) else 0 end) as һ��  " +
                                    " from " + tableName +
                                    " group by substr(xiang,1,8))";
                pWorkspace.ExecuteSQL(xiangSQL);
                string LCSQL = "insert into DATA_BHXX (select lc as ͳ�Ƶ�λ," +
                                    "sum(case when bhdj <>' ' then round(" + strMjFieldName + ",2) else 0 end) as �ϼ�," +
                                    "sum(case when bhdj='1' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                    "sum(case when bhdj='2' then round(" + strMjFieldName + ",2) else 0 end) as �ص�," +
                                    "sum(case when bhdj='3' then round(" + strMjFieldName + ",2) else 0 end) as һ��  " +
                                    " from " + tableName +
                                    " group by lc)";
                pWorkspace.ExecuteSQL(LCSQL);
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }
        //������Ϣͳ�� ygc 2013-01-16
        private static bool DLXX_Statistic(IFeatureClass pFeatureClass, string strMjFieldName)
        {
            if (pFeatureClass == null)
            {
                return false;
            }
            IWorkspace pWorkspace = null;
            string tableName = "";
            try
            {
                pWorkspace = pFeatureClass.FeatureDataset.Workspace;
                tableName = (pFeatureClass as IDataset).Name;
            }
            catch (Exception ex)
            { }
            DropTable(pWorkspace, "DATA_DLXX");
            try
            {
                string cunSQL = "create table DATA_DLXX as select substr(cun,1,10) as ͳ�Ƶ�λ," +
                                "sum( case when dl<>' ' then round(" + strMjFieldName + ",2) else 0 end) as �ϼ�," +
                                "sum(case when dl ='111' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when dl='112' then round(" + strMjFieldName + ",2) else 0 end) as �콻��," +
                                "sum(case when dl ='113' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when dl ='114' then round(" + strMjFieldName + ",2) else 0 end) as ������," +
                                "sum( case when dl='120' then round(" + strMjFieldName + ",2) else 0 end) as ���ֵ�," +
                                "sum( case when dl='131' then round(" + strMjFieldName + ",2) else 0 end) as �����ر�涨�Ĺ�ľ�ֵ�," +
                                "sum( case when dl='132' then round(" + strMjFieldName + ",2) else 0 end) as ������ľ�ֵ�," +
                                "sum(case when dl='141' then round (" + strMjFieldName + ",2) else 0 end) as �˹�����δ���ֵ�," +
                                "sum(case when dl='142' then round(" + strMjFieldName + ",2) else 0 end) as ����δ���ֵ�," +
                                "sum(case when dl='150' then round(" + strMjFieldName + ",2) else 0 end) as ���Ե�," +
                                "sum(case when dl='161' then round(" + strMjFieldName + ",2) else 0 end) as �ɷ�����," +
                                "sum(case when dl='162' then round(" + strMjFieldName + ",2) else 0 end) as ���ռ���," +
                                "sum(case when dl='163' then round(" + strMjFieldName + ",2) else 0 end) as ��������ľ�ֵ�," +
                                "sum(case when dl='171' then round(" + strMjFieldName + ",2) else 0 end) as ���ֻ�ɽ�ĵ�," +
                                "sum(case when dl='172' then round(" + strMjFieldName + ",2) else 0 end) as  ����ɳ�ĵ�," +
                                "sum(case when dl='173' then round(" + strMjFieldName + ",2) else 0 end) as �������ֵ�," +
                                "sum(case when dl='174' then round(" + strMjFieldName + ",2) else 0 end) as �˸���," +
                                "sum(case when dl='180' then round(" + strMjFieldName + ",2) else 0 end) as ���������ֵ�," +
                                "sum(case when dl='211' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when dl='212' then round(" + strMjFieldName + ",2) else 0 end) as ����ũ���ֵ�," +
                                "sum(case when dl='220' then round(" + strMjFieldName + ",2) else 0 end) as ���ݵ�," +
                                "sum(case when dl='221' then round(" + strMjFieldName + ",2) else 0 end) as ��Ȼ�ݵ�," +
                                "sum(case when dl='222' then round(" + strMjFieldName + ",2) else 0 end) as �����ݵ�," +
                                "sum(case when dl='223' then round(" + strMjFieldName + ",2) else 0 end) as �˹��ݵ�," +
                                "sum(case when dl='231' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when dl='232' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when dl='233' then round(" + strMjFieldName + ",2) else 0 end) as ˮ��," +
                                "sum(case when dl='234' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when dl='241' then round(" + strMjFieldName + ",2) else 0 end) as �Ĳݵ�," +
                                "sum(case when dl='242' then round(" + strMjFieldName + ",2) else 0 end) as �μ��," +
                                "sum(case when dl='243' then round(" + strMjFieldName + ",2) else 0 end) as ɳ��," +
                                "sum(case when dl='244' then round(" + strMjFieldName + ",2) else 0 end) as ������," +
                                "sum( case when dl='245' then round(" + strMjFieldName + ",2) else 0 end) as ����ʯ����," +
                                "sum(case when dl='246' then round(" + strMjFieldName + ",2) else 0 end) as ̲Ϳ," +
                                "sum(case when dl='247' then round(" + strMjFieldName + ",2) else 0 end) as ����δ���õ�," +
                                "sum(case when dl='251' then round(" + strMjFieldName + ",2) else 0 end) as �������õ�," +
                                "sum(case when dl='252' then round(" + strMjFieldName + ",2) else 0 end) as �������㽨���õ�," +
                                "sum(case when dl='253' then round(" + strMjFieldName + ",2) else 0 end) as ��ͨ�����õ�," +
                                "sum( case when dl='254' then round(" + strMjFieldName + ",2) else 0 end) as  �����õ�  " +
                                "  from " + tableName +
                                "  group by substr(cun,1,10)";
                pWorkspace.ExecuteSQL(cunSQL);
                string shengSQL = "insert into DATA_DLXX (select substr(sheng,1,2) as ͳ�Ƶ�λ," +
                                "sum( case when dl<>' ' then round(" + strMjFieldName + ",2) else 0 end) as �ϼ�," +
                                "sum(case when dl ='111' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when dl='112' then round(" + strMjFieldName + ",2) else 0 end) as �콻��," +
                                "sum(case when dl ='113' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when dl ='114' then round(" + strMjFieldName + ",2) else 0 end) as ������," +
                                "sum( case when dl='120' then round(" + strMjFieldName + ",2) else 0 end) as ���ֵ�," +
                                "sum( case when dl='131' then round(" + strMjFieldName + ",2) else 0 end) as �����ر�涨�Ĺ�ľ�ֵ�," +
                                "sum( case when dl='132' then round(" + strMjFieldName + ",2) else 0 end) as ������ľ�ֵ�," +
                                "sum(case when dl='141' then round (" + strMjFieldName + ",2) else 0 end) as �˹�����δ���ֵ�," +
                                "sum(case when dl='142' then round(" + strMjFieldName + ",2) else 0 end) as ����δ���ֵ�," +
                                "sum(case when dl='150' then round(" + strMjFieldName + ",2) else 0 end) as ���Ե�," +
                                "sum(case when dl='161' then round(" + strMjFieldName + ",2) else 0 end) as �ɷ�����," +
                                "sum(case when dl='162' then round(" + strMjFieldName + ",2) else 0 end) as ���ռ���," +
                                "sum(case when dl='163' then round(" + strMjFieldName + ",2) else 0 end) as ��������ľ�ֵ�," +
                                "sum(case when dl='171' then round(" + strMjFieldName + ",2) else 0 end) as ���ֻ�ɽ�ĵ�," +
                                "sum(case when dl='172' then round(" + strMjFieldName + ",2) else 0 end) as  ����ɳ�ĵ�," +
                                "sum(case when dl='173' then round(" + strMjFieldName + ",2) else 0 end) as �������ֵ�," +
                                "sum(case when dl='174' then round(" + strMjFieldName + ",2) else 0 end) as �˸���," +
                                "sum(case when dl='180' then round(" + strMjFieldName + ",2) else 0 end) as ���������ֵ�," +
                                "sum(case when dl='211' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when dl='212' then round(" + strMjFieldName + ",2) else 0 end) as ����ũ���ֵ�," +
                                "sum(case when dl='220' then round(" + strMjFieldName + ",2) else 0 end) as ���ݵ�," +
                                "sum(case when dl='221' then round(" + strMjFieldName + ",2) else 0 end) as ��Ȼ�ݵ�," +
                                "sum(case when dl='222' then round(" + strMjFieldName + ",2) else 0 end) as �����ݵ�," +
                                "sum(case when dl='223' then round(" + strMjFieldName + ",2) else 0 end) as �˹��ݵ�," +
                                "sum(case when dl='231' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when dl='232' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when dl='233' then round(" + strMjFieldName + ",2) else 0 end) as ˮ��," +
                                "sum(case when dl='234' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when dl='241' then round(" + strMjFieldName + ",2) else 0 end) as �Ĳݵ�," +
                                "sum(case when dl='242' then round(" + strMjFieldName + ",2) else 0 end) as �μ��," +
                                "sum(case when dl='243' then round(" + strMjFieldName + ",2) else 0 end) as ɳ��," +
                                "sum(case when dl='244' then round(" + strMjFieldName + ",2) else 0 end) as ������," +
                                "sum( case when dl='245' then round(" + strMjFieldName + ",2) else 0 end) as ����ʯ����," +
                                "sum(case when dl='246' then round(" + strMjFieldName + ",2) else 0 end) as ̲Ϳ," +
                                "sum(case when dl='247' then round(" + strMjFieldName + ",2) else 0 end) as ����δ���õ�," +
                                "sum(case when dl='251' then round(" + strMjFieldName + ",2) else 0 end) as �������õ�," +
                                "sum(case when dl='252' then round(" + strMjFieldName + ",2) else 0 end) as �������㽨���õ�," +
                                "sum(case when dl='253' then round(" + strMjFieldName + ",2) else 0 end) as ��ͨ�����õ�," +
                                "sum( case when dl='254' then round(" + strMjFieldName + ",2) else 0 end) as  �����õ�" +
                                "  from " + tableName +
                                "  group by substr(sheng,1,2))";
                pWorkspace.ExecuteSQL(shengSQL);
                string shiSQL = "insert into DATA_DLXX (select substr(shi,1,4) as ͳ�Ƶ�λ," +
                                "sum( case when dl<>' ' then round(" + strMjFieldName + ",2) else 0 end) as �ϼ�," +
                                "sum(case when dl ='111' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when dl='112' then round(" + strMjFieldName + ",2) else 0 end) as �콻��," +
                                "sum(case when dl ='113' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when dl ='114' then round(" + strMjFieldName + ",2) else 0 end) as ������," +
                                "sum( case when dl='120' then round(" + strMjFieldName + ",2) else 0 end) as ���ֵ�," +
                                "sum( case when dl='131' then round(" + strMjFieldName + ",2) else 0 end) as �����ر�涨�Ĺ�ľ�ֵ�," +
                                "sum( case when dl='132' then round(" + strMjFieldName + ",2) else 0 end) as ������ľ�ֵ�," +
                                "sum(case when dl='141' then round (" + strMjFieldName + ",2) else 0 end) as �˹�����δ���ֵ�," +
                                "sum(case when dl='142' then round(" + strMjFieldName + ",2) else 0 end) as ����δ���ֵ�," +
                                "sum(case when dl='150' then round(" + strMjFieldName + ",2) else 0 end) as ���Ե�," +
                                "sum(case when dl='161' then round(" + strMjFieldName + ",2) else 0 end) as �ɷ�����," +
                                "sum(case when dl='162' then round(" + strMjFieldName + ",2) else 0 end) as ���ռ���," +
                                "sum(case when dl='163' then round(" + strMjFieldName + ",2) else 0 end) as ��������ľ�ֵ�," +
                                "sum(case when dl='171' then round(" + strMjFieldName + ",2) else 0 end) as ���ֻ�ɽ�ĵ�," +
                                "sum(case when dl='172' then round(" + strMjFieldName + ",2) else 0 end) as  ����ɳ�ĵ�," +
                                "sum(case when dl='173' then round(" + strMjFieldName + ",2) else 0 end) as �������ֵ�," +
                                "sum(case when dl='174' then round(" + strMjFieldName + ",2) else 0 end) as �˸���," +
                                "sum(case when dl='180' then round(" + strMjFieldName + ",2) else 0 end) as ���������ֵ�," +
                                "sum(case when dl='211' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when dl='212' then round(" + strMjFieldName + ",2) else 0 end) as ����ũ���ֵ�," +
                                "sum(case when dl='220' then round(" + strMjFieldName + ",2) else 0 end) as ���ݵ�," +
                                "sum(case when dl='221' then round(" + strMjFieldName + ",2) else 0 end) as ��Ȼ�ݵ�," +
                                "sum(case when dl='222' then round(" + strMjFieldName + ",2) else 0 end) as �����ݵ�," +
                                "sum(case when dl='223' then round(" + strMjFieldName + ",2) else 0 end) as �˹��ݵ�," +
                                "sum(case when dl='231' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when dl='232' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when dl='233' then round(" + strMjFieldName + ",2) else 0 end) as ˮ��," +
                                "sum(case when dl='234' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when dl='241' then round(" + strMjFieldName + ",2) else 0 end) as �Ĳݵ�," +
                                "sum(case when dl='242' then round(" + strMjFieldName + ",2) else 0 end) as �μ��," +
                                "sum(case when dl='243' then round(" + strMjFieldName + ",2) else 0 end) as ɳ��," +
                                "sum(case when dl='244' then round(" + strMjFieldName + ",2) else 0 end) as ������," +
                                "sum( case when dl='245' then round(" + strMjFieldName + ",2) else 0 end) as ����ʯ����," +
                                "sum(case when dl='246' then round(" + strMjFieldName + ",2) else 0 end) as ̲Ϳ," +
                                "sum(case when dl='247' then round(" + strMjFieldName + ",2) else 0 end) as ����δ���õ�," +
                                "sum(case when dl='251' then round(" + strMjFieldName + ",2) else 0 end) as �������õ�," +
                                "sum(case when dl='252' then round(" + strMjFieldName + ",2) else 0 end) as �������㽨���õ�," +
                                "sum(case when dl='253' then round(" + strMjFieldName + ",2) else 0 end) as ��ͨ�����õ�," +
                                "sum( case when dl='254' then round(" + strMjFieldName + ",2) else 0 end) as  �����õ�" +
                                "  from " + tableName +
                                "  group by substr(shi,1,4))";
                pWorkspace.ExecuteSQL(shiSQL);
                string xianSQL = "insert into DATA_DLXX (select substr(xian,1,6) as ͳ�Ƶ�λ," +
                                "sum( case when dl<>' ' then round(" + strMjFieldName + ",2) else 0 end) as �ϼ�," +
                                "sum(case when dl ='111' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when dl='112' then round(" + strMjFieldName + ",2) else 0 end) as �콻��," +
                                "sum(case when dl ='113' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when dl ='114' then round(" + strMjFieldName + ",2) else 0 end) as ������," +
                                "sum( case when dl='120' then round(" + strMjFieldName + ",2) else 0 end) as ���ֵ�," +
                                "sum( case when dl='131' then round(" + strMjFieldName + ",2) else 0 end) as �����ر�涨�Ĺ�ľ�ֵ�," +
                                "sum( case when dl='132' then round(" + strMjFieldName + ",2) else 0 end) as ������ľ�ֵ�," +
                                "sum(case when dl='141' then round (" + strMjFieldName + ",2) else 0 end) as �˹�����δ���ֵ�," +
                                "sum(case when dl='142' then round(" + strMjFieldName + ",2) else 0 end) as ����δ���ֵ�," +
                                "sum(case when dl='150' then round(" + strMjFieldName + ",2) else 0 end) as ���Ե�," +
                                "sum(case when dl='161' then round(" + strMjFieldName + ",2) else 0 end) as �ɷ�����," +
                                "sum(case when dl='162' then round(" + strMjFieldName + ",2) else 0 end) as ���ռ���," +
                                "sum(case when dl='163' then round(" + strMjFieldName + ",2) else 0 end) as ��������ľ�ֵ�," +
                                "sum(case when dl='171' then round(" + strMjFieldName + ",2) else 0 end) as ���ֻ�ɽ�ĵ�," +
                                "sum(case when dl='172' then round(" + strMjFieldName + ",2) else 0 end) as  ����ɳ�ĵ�," +
                                "sum(case when dl='173' then round(" + strMjFieldName + ",2) else 0 end) as �������ֵ�," +
                                "sum(case when dl='174' then round(" + strMjFieldName + ",2) else 0 end) as �˸���," +
                                "sum(case when dl='180' then round(" + strMjFieldName + ",2) else 0 end) as ���������ֵ�," +
                                "sum(case when dl='211' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when dl='212' then round(" + strMjFieldName + ",2) else 0 end) as ����ũ���ֵ�," +
                                "sum(case when dl='220' then round(" + strMjFieldName + ",2) else 0 end) as ���ݵ�," +
                                "sum(case when dl='221' then round(" + strMjFieldName + ",2) else 0 end) as ��Ȼ�ݵ�," +
                                "sum(case when dl='222' then round(" + strMjFieldName + ",2) else 0 end) as �����ݵ�," +
                                "sum(case when dl='223' then round(" + strMjFieldName + ",2) else 0 end) as �˹��ݵ�," +
                                "sum(case when dl='231' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when dl='232' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when dl='233' then round(" + strMjFieldName + ",2) else 0 end) as ˮ��," +
                                "sum(case when dl='234' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when dl='241' then round(" + strMjFieldName + ",2) else 0 end) as �Ĳݵ�," +
                                "sum(case when dl='242' then round(" + strMjFieldName + ",2) else 0 end) as �μ��," +
                                "sum(case when dl='243' then round(" + strMjFieldName + ",2) else 0 end) as ɳ��," +
                                "sum(case when dl='244' then round(" + strMjFieldName + ",2) else 0 end) as ������," +
                                "sum( case when dl='245' then round(" + strMjFieldName + ",2) else 0 end) as ����ʯ����," +
                                "sum(case when dl='246' then round(" + strMjFieldName + ",2) else 0 end) as ̲Ϳ," +
                                "sum(case when dl='247' then round(" + strMjFieldName + ",2) else 0 end) as ����δ���õ�," +
                                "sum(case when dl='251' then round(" + strMjFieldName + ",2) else 0 end) as �������õ�," +
                                "sum(case when dl='252' then round(" + strMjFieldName + ",2) else 0 end) as �������㽨���õ�," +
                                "sum(case when dl='253' then round(" + strMjFieldName + ",2) else 0 end) as ��ͨ�����õ�," +
                                "sum( case when dl='254' then round(" + strMjFieldName + ",2) else 0 end) as  �����õ�" +
                                "  from " + tableName +
                                "  group by substr(xian,1,6))";
                pWorkspace.ExecuteSQL(xianSQL);
                string xiangSQL = "insert into DATA_DLXX (select substr(xiang,1,8) as ͳ�Ƶ�λ," +
                                "sum( case when dl<>' ' then round(" + strMjFieldName + ",2) else 0 end) as �ϼ�," +
                                "sum(case when dl ='111' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when dl='112' then round(" + strMjFieldName + ",2) else 0 end) as �콻��," +
                                "sum(case when dl ='113' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when dl ='114' then round(" + strMjFieldName + ",2) else 0 end) as ������," +
                                "sum( case when dl='120' then round(" + strMjFieldName + ",2) else 0 end) as ���ֵ�," +
                                "sum( case when dl='131' then round(" + strMjFieldName + ",2) else 0 end) as �����ر�涨�Ĺ�ľ�ֵ�," +
                                "sum( case when dl='132' then round(" + strMjFieldName + ",2) else 0 end) as ������ľ�ֵ�," +
                                "sum(case when dl='141' then round (" + strMjFieldName + ",2) else 0 end) as �˹�����δ���ֵ�," +
                                "sum(case when dl='142' then round(" + strMjFieldName + ",2) else 0 end) as ����δ���ֵ�," +
                                "sum(case when dl='150' then round(" + strMjFieldName + ",2) else 0 end) as ���Ե�," +
                                "sum(case when dl='161' then round(" + strMjFieldName + ",2) else 0 end) as �ɷ�����," +
                                "sum(case when dl='162' then round(" + strMjFieldName + ",2) else 0 end) as ���ռ���," +
                                "sum(case when dl='163' then round(" + strMjFieldName + ",2) else 0 end) as ��������ľ�ֵ�," +
                                "sum(case when dl='171' then round(" + strMjFieldName + ",2) else 0 end) as ���ֻ�ɽ�ĵ�," +
                                "sum(case when dl='172' then round(" + strMjFieldName + ",2) else 0 end) as  ����ɳ�ĵ�," +
                                "sum(case when dl='173' then round(" + strMjFieldName + ",2) else 0 end) as �������ֵ�," +
                                "sum(case when dl='174' then round(" + strMjFieldName + ",2) else 0 end) as �˸���," +
                                "sum(case when dl='180' then round(" + strMjFieldName + ",2) else 0 end) as ���������ֵ�," +
                                "sum(case when dl='211' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when dl='212' then round(" + strMjFieldName + ",2) else 0 end) as ����ũ���ֵ�," +
                                "sum(case when dl='220' then round(" + strMjFieldName + ",2) else 0 end) as ���ݵ�," +
                                "sum(case when dl='221' then round(" + strMjFieldName + ",2) else 0 end) as ��Ȼ�ݵ�," +
                                "sum(case when dl='222' then round(" + strMjFieldName + ",2) else 0 end) as �����ݵ�," +
                                "sum(case when dl='223' then round(" + strMjFieldName + ",2) else 0 end) as �˹��ݵ�," +
                                "sum(case when dl='231' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when dl='232' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when dl='233' then round(" + strMjFieldName + ",2) else 0 end) as ˮ��," +
                                "sum(case when dl='234' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when dl='241' then round(" + strMjFieldName + ",2) else 0 end) as �Ĳݵ�," +
                                "sum(case when dl='242' then round(" + strMjFieldName + ",2) else 0 end) as �μ��," +
                                "sum(case when dl='243' then round(" + strMjFieldName + ",2) else 0 end) as ɳ��," +
                                "sum(case when dl='244' then round(" + strMjFieldName + ",2) else 0 end) as ������," +
                                "sum( case when dl='245' then round(" + strMjFieldName + ",2) else 0 end) as ����ʯ����," +
                                "sum(case when dl='246' then round(" + strMjFieldName + ",2) else 0 end) as ̲Ϳ," +
                                "sum(case when dl='247' then round(" + strMjFieldName + ",2) else 0 end) as ����δ���õ�," +
                                "sum(case when dl='251' then round(" + strMjFieldName + ",2) else 0 end) as �������õ�," +
                                "sum(case when dl='252' then round(" + strMjFieldName + ",2) else 0 end) as �������㽨���õ�," +
                                "sum(case when dl='253' then round(" + strMjFieldName + ",2) else 0 end) as ��ͨ�����õ�," +
                                "sum( case when dl='254' then round(" + strMjFieldName + ",2) else 0 end) as  �����õ�" +
                                "  from " + tableName +
                                "  group by substr(xiang,1,8))";
                pWorkspace.ExecuteSQL(xiangSQL);
                string lcSQL = "insert into DATA_DLXX (select lc as ͳ�Ƶ�λ," +
                                "sum( case when dl<>' ' then round(" + strMjFieldName + ",2) else 0 end) as �ϼ�," +
                                "sum(case when dl ='111' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when dl='112' then round(" + strMjFieldName + ",2) else 0 end) as �콻��," +
                                "sum(case when dl ='113' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when dl ='114' then round(" + strMjFieldName + ",2) else 0 end) as ������," +
                                "sum( case when dl='120' then round(" + strMjFieldName + ",2) else 0 end) as ���ֵ�," +
                                "sum( case when dl='131' then round(" + strMjFieldName + ",2) else 0 end) as �����ر�涨�Ĺ�ľ�ֵ�," +
                                "sum( case when dl='132' then round(" + strMjFieldName + ",2) else 0 end) as ������ľ�ֵ�," +
                                "sum(case when dl='141' then round (" + strMjFieldName + ",2) else 0 end) as �˹�����δ���ֵ�," +
                                "sum(case when dl='142' then round(" + strMjFieldName + ",2) else 0 end) as ����δ���ֵ�," +
                                "sum(case when dl='150' then round(" + strMjFieldName + ",2) else 0 end) as ���Ե�," +
                                "sum(case when dl='161' then round(" + strMjFieldName + ",2) else 0 end) as �ɷ�����," +
                                "sum(case when dl='162' then round(" + strMjFieldName + ",2) else 0 end) as ���ռ���," +
                                "sum(case when dl='163' then round(" + strMjFieldName + ",2) else 0 end) as ��������ľ�ֵ�," +
                                "sum(case when dl='171' then round(" + strMjFieldName + ",2) else 0 end) as ���ֻ�ɽ�ĵ�," +
                                "sum(case when dl='172' then round(" + strMjFieldName + ",2) else 0 end) as  ����ɳ�ĵ�," +
                                "sum(case when dl='173' then round(" + strMjFieldName + ",2) else 0 end) as �������ֵ�," +
                                "sum(case when dl='174' then round(" + strMjFieldName + ",2) else 0 end) as �˸���," +
                                "sum(case when dl='180' then round(" + strMjFieldName + ",2) else 0 end) as ���������ֵ�," +
                                "sum(case when dl='211' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when dl='212' then round(" + strMjFieldName + ",2) else 0 end) as ����ũ���ֵ�," +
                                "sum(case when dl='220' then round(" + strMjFieldName + ",2) else 0 end) as ���ݵ�," +
                                "sum(case when dl='221' then round(" + strMjFieldName + ",2) else 0 end) as ��Ȼ�ݵ�," +
                                "sum(case when dl='222' then round(" + strMjFieldName + ",2) else 0 end) as �����ݵ�," +
                                "sum(case when dl='223' then round(" + strMjFieldName + ",2) else 0 end) as �˹��ݵ�," +
                                "sum(case when dl='231' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when dl='232' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when dl='233' then round(" + strMjFieldName + ",2) else 0 end) as ˮ��," +
                                "sum(case when dl='234' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when dl='241' then round(" + strMjFieldName + ",2) else 0 end) as �Ĳݵ�," +
                                "sum(case when dl='242' then round(" + strMjFieldName + ",2) else 0 end) as �μ��," +
                                "sum(case when dl='243' then round(" + strMjFieldName + ",2) else 0 end) as ɳ��," +
                                "sum(case when dl='244' then round(" + strMjFieldName + ",2) else 0 end) as ������," +
                                "sum( case when dl='245' then round(" + strMjFieldName + ",2) else 0 end) as ����ʯ����," +
                                "sum(case when dl='246' then round(" + strMjFieldName + ",2) else 0 end) as ̲Ϳ," +
                                "sum(case when dl='247' then round(" + strMjFieldName + ",2) else 0 end) as ����δ���õ�," +
                                "sum(case when dl='251' then round(" + strMjFieldName + ",2) else 0 end) as �������õ�," +
                                "sum(case when dl='252' then round(" + strMjFieldName + ",2) else 0 end) as �������㽨���õ�," +
                                "sum(case when dl='253' then round(" + strMjFieldName + ",2) else 0 end) as ��ͨ�����õ�," +
                                "sum( case when dl='254' then round(" + strMjFieldName + ",2) else 0 end) as  �����õ�" +
                                "  from " + tableName +
                                "  group by lc)";
                pWorkspace.ExecuteSQL(lcSQL);
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }
        //���������Ϣͳ�� ygc 2013-01-17
        private static bool GCLBXX_Statistic(IFeatureClass pFeatureClass, string strMjFieldName)
        {
            if (pFeatureClass == null)
            {
                return false;
            }
            IWorkspace pWorkspace = null;
            string tableName = "";
            try
            {
                pWorkspace = pFeatureClass.FeatureDataset.Workspace;
                tableName = (pFeatureClass as IDataset).Name;
            }
            catch (Exception ex)
            { }
            DropTable(pWorkspace, "DATA_GCLB");
            try
            {
                string cunSQL = "create table DATA_GCLB as select substr(cun,1,10) as ͳ�Ƶ�λ," +
                                    "sum(case when gclb<>' ' then round("+strMjFieldName+",2) else 0 end) as �ϼ�," +
                                    "sum(case when gclb='12' then round(" + strMjFieldName + ",2) else 0 end) as ��Ȼ�ֱ�������," +
                                    "sum(case when gclb='21' then round(" + strMjFieldName + ",2) else 0 end) as ���������ֹ���," +
                                    "sum(case when gclb='26' then round(" + strMjFieldName + ",2) else 0 end) as ̫��ɽ�̻�����," +
                                    "sum(case when gclb='27' then round(" + strMjFieldName + ",2) else 0 end) as ƽԭ�̻�����," +
                                    "sum(case when gclb='30'  then round(" + strMjFieldName + ",2) else 0 end) as �˸����ֹ���," +
                                    "sum(case when gclb='40' then round(" + strMjFieldName + ",2) else 0 end) as �����ɳԴ������," +
                                    "sum(case when gclb='51' then round(" + strMjFieldName + ",2) else 0 end) as Ұ����ֲ����Ҽ�������," +
                                    "sum(case when gclb='52' then round(" + strMjFieldName + ",2) else 0 end) as Ұ����ֲ��ط���������," +
                                    "sum(case when gclb='53' then round(" + strMjFieldName + ",2) else 0 end) as Ұ����ֲ���з���������," +
                                    "sum(case when gclb='54' then round(" + strMjFieldName + ",2) else 0 end) as Ұ����ֲ���ط���������," +
                                    "sum(case when gclb='60' then round(" + strMjFieldName + ",2) else 0 end) as ��������ֻ��ؽ��蹤��," +
                                    "sum(case when gclb='61' then round(" + strMjFieldName + ",2) else 0 end) as ʡ��ͨ���̻�����," +
                                    "sum(case when gclb='62' then round(" + strMjFieldName + ",2) else 0 end) as ʡ��ƽԭ�̻�����," +
                                    "sum(case when gclb='63' then round(" + strMjFieldName + ",2) else 0 end) as ʡ����ͨ���߻�ɽ�̻�����," +
                                    "sum(case when gclb='64' then round(" + strMjFieldName + ",2) else 0 end) as ʡ��԰�ִ����̻�����," +
                                    "sum(case when gclb='65' then round(" + strMjFieldName + ",2) else 0 end) as ʡ�������̻�����," +
                                    "sum(case when gclb='66' then round(" + strMjFieldName + ",2) else 0 end) as ʡ�������̻�����," +
                                    "sum(case when gclb='70' then round(" + strMjFieldName + ",2) else 0 end) as ȫ��ʪ�ر�������," +
                                    "sum(case when gclb='71' then round(" + strMjFieldName + ",2) else 0 end) as ���Ź���̬�������������蹤��," +
                                    "sum(case when gclb='80' then round(" + strMjFieldName + ",2) else 0 end) as �ص㹫���־�Ӫ����," +
                                    "sum(case when gclb='81' then round(" + strMjFieldName + ",2) else 0 end) as �׶�ˮ��Դ�ɳ������ù���," +
                                    "sum(case when gclb='82' then round(" + strMjFieldName + ",2) else 0 end) as �ںӿ������κӵ�������," +
                                    "sum(case when gclb='83' then round(" + strMjFieldName + ",2) else 0 end) as С��ˮ����ˮ�����蹤��," +
                                    "sum(case when gclb='84' then round(" + strMjFieldName + ",2) else 0 end) as ʡ��Ӧ��ˮԴ����," +
                                    "sum(case when gclb='85' then round(" + strMjFieldName + ",2) else 0 end) as ʡ��ũ��ˮ����ȹ���," +
                                    "sum(case when gclb='86' then round(" + strMjFieldName + ",2) else 0 end) as ʡ��ˮ�������ٵذӹ���," +
                                    "sum(case when gclb='87' then round(" + strMjFieldName + ",2) else 0 end) as ʡ��ũ����ˮ��ȫ����," +
                                    "sum(case when gclb='88' then round(" + strMjFieldName + ",2) else 0 end) as ʡ�������ˮ����," +
                                    "sum(case when gclb='89' then round(" + strMjFieldName + ",2) else 0 end) as ʡ��ˮԴ��������," +
                                    "sum(case when gclb='90' then round(" + strMjFieldName + ",2) else 0 end) as ������ҵ����," +
                                    "sum(case when gclb='91' then round(" + strMjFieldName + ",2) else 0 end) as ����ũҵ����," +
                                    "sum(case when gclb='92' then round(" + strMjFieldName + ",2) else 0 end) as ����ˮ������  " +
                                    "  from "+tableName +
                                    "  group by substr(cun,1,10)";
                pWorkspace.ExecuteSQL(cunSQL);
                string shengSQL = "insert into DATA_GCLB ( select substr(sheng,1,2) as ͳ�Ƶ�λ," +
                                    "sum(case when gclb<>' ' then round(" + strMjFieldName + ",2) else 0 end) as �ϼ�," +
                                    "sum(case when gclb='12' then round(" + strMjFieldName + ",2) else 0 end) as ��Ȼ�ֱ�������," +
                                    "sum(case when gclb='21' then round(" + strMjFieldName + ",2) else 0 end) as ���������ֹ���," +
                                    "sum(case when gclb='26' then round(" + strMjFieldName + ",2) else 0 end) as ̫��ɽ�̻�����," +
                                    "sum(case when gclb='27' then round(" + strMjFieldName + ",2) else 0 end) as ƽԭ�̻�����," +
                                    "sum(case when gclb='30'  then round(" + strMjFieldName + ",2) else 0 end) as �˸����ֹ���," +
                                    "sum(case when gclb='40' then round(" + strMjFieldName + ",2) else 0 end) as �����ɳԴ������," +
                                    "sum(case when gclb='51' then round(" + strMjFieldName + ",2) else 0 end) as Ұ����ֲ����Ҽ�������," +
                                    "sum(case when gclb='52' then round(" + strMjFieldName + ",2) else 0 end) as Ұ����ֲ��ط���������," +
                                    "sum(case when gclb='53' then round(" + strMjFieldName + ",2) else 0 end) as Ұ����ֲ���з���������," +
                                    "sum(case when gclb='54' then round(" + strMjFieldName + ",2) else 0 end) as Ұ����ֲ���ط���������," +
                                    "sum(case when gclb='60' then round(" + strMjFieldName + ",2) else 0 end) as ��������ֻ��ؽ��蹤��," +
                                    "sum(case when gclb='61' then round(" + strMjFieldName + ",2) else 0 end) as ʡ��ͨ���̻�����," +
                                    "sum(case when gclb='62' then round(" + strMjFieldName + ",2) else 0 end) as ʡ��ƽԭ�̻�����," +
                                    "sum(case when gclb='63' then round(" + strMjFieldName + ",2) else 0 end) as ʡ����ͨ���߻�ɽ�̻�����," +
                                    "sum(case when gclb='64' then round(" + strMjFieldName + ",2) else 0 end) as ʡ��԰�ִ����̻�����," +
                                    "sum(case when gclb='65' then round(" + strMjFieldName + ",2) else 0 end) as ʡ�������̻�����," +
                                    "sum(case when gclb='66' then round(" + strMjFieldName + ",2) else 0 end) as ʡ�������̻�����," +
                                    "sum(case when gclb='70' then round(" + strMjFieldName + ",2) else 0 end) as ȫ��ʪ�ر�������," +
                                    "sum(case when gclb='71' then round(" + strMjFieldName + ",2) else 0 end) as ���Ź���̬�������������蹤��," +
                                    "sum(case when gclb='80' then round(" + strMjFieldName + ",2) else 0 end) as �ص㹫���־�Ӫ����," +
                                    "sum(case when gclb='81' then round(" + strMjFieldName + ",2) else 0 end) as �׶�ˮ��Դ�ɳ������ù���," +
                                    "sum(case when gclb='82' then round(" + strMjFieldName + ",2) else 0 end) as �ںӿ������κӵ�������," +
                                    "sum(case when gclb='83' then round(" + strMjFieldName + ",2) else 0 end) as С��ˮ����ˮ�����蹤��," +
                                    "sum(case when gclb='84' then round(" + strMjFieldName + ",2) else 0 end) as ʡ��Ӧ��ˮԴ����," +
                                    "sum(case when gclb='85' then round(" + strMjFieldName + ",2) else 0 end) as ʡ��ũ��ˮ����ȹ���," +
                                    "sum(case when gclb='86' then round(" + strMjFieldName + ",2) else 0 end) as ʡ��ˮ�������ٵذӹ���," +
                                    "sum(case when gclb='87' then round(" + strMjFieldName + ",2) else 0 end) as ʡ��ũ����ˮ��ȫ����," +
                                    "sum(case when gclb='88' then round(" + strMjFieldName + ",2) else 0 end) as ʡ�������ˮ����," +
                                    "sum(case when gclb='89' then round(" + strMjFieldName + ",2) else 0 end) as ʡ��ˮԴ��������," +
                                    "sum(case when gclb='90' then round(" + strMjFieldName + ",2) else 0 end) as ������ҵ����," +
                                    "sum(case when gclb='91' then round(" + strMjFieldName + ",2) else 0 end) as ����ũҵ����," +
                                    "sum(case when gclb='92' then round(" + strMjFieldName + ",2) else 0 end) as ����ˮ������  " +
                                    "  from " + tableName +
                                    "  group by substr(sheng,1,2))";
                pWorkspace.ExecuteSQL(shengSQL);
                string shiSQL = "insert into DATA_GCLB ( select substr(shi,1,4) as ͳ�Ƶ�λ," +
                                    "sum(case when gclb<>' ' then round(" + strMjFieldName + ",2) else 0 end) as �ϼ�," +
                                    "sum(case when gclb='12' then round(" + strMjFieldName + ",2) else 0 end) as ��Ȼ�ֱ�������," +
                                    "sum(case when gclb='21' then round(" + strMjFieldName + ",2) else 0 end) as ���������ֹ���," +
                                    "sum(case when gclb='26' then round(" + strMjFieldName + ",2) else 0 end) as ̫��ɽ�̻�����," +
                                    "sum(case when gclb='27' then round(" + strMjFieldName + ",2) else 0 end) as ƽԭ�̻�����," +
                                    "sum(case when gclb='30'  then round(" + strMjFieldName + ",2) else 0 end) as �˸����ֹ���," +
                                    "sum(case when gclb='40' then round(" + strMjFieldName + ",2) else 0 end) as �����ɳԴ������," +
                                    "sum(case when gclb='51' then round(" + strMjFieldName + ",2) else 0 end) as Ұ����ֲ����Ҽ�������," +
                                    "sum(case when gclb='52' then round(" + strMjFieldName + ",2) else 0 end) as Ұ����ֲ��ط���������," +
                                    "sum(case when gclb='53' then round(" + strMjFieldName + ",2) else 0 end) as Ұ����ֲ���з���������," +
                                    "sum(case when gclb='54' then round(" + strMjFieldName + ",2) else 0 end) as Ұ����ֲ���ط���������," +
                                    "sum(case when gclb='60' then round(" + strMjFieldName + ",2) else 0 end) as ��������ֻ��ؽ��蹤��," +
                                    "sum(case when gclb='61' then round(" + strMjFieldName + ",2) else 0 end) as ʡ��ͨ���̻�����," +
                                    "sum(case when gclb='62' then round(" + strMjFieldName + ",2) else 0 end) as ʡ��ƽԭ�̻�����," +
                                    "sum(case when gclb='63' then round(" + strMjFieldName + ",2) else 0 end) as ʡ����ͨ���߻�ɽ�̻�����," +
                                    "sum(case when gclb='64' then round(" + strMjFieldName + ",2) else 0 end) as ʡ��԰�ִ����̻�����," +
                                    "sum(case when gclb='65' then round(" + strMjFieldName + ",2) else 0 end) as ʡ�������̻�����," +
                                    "sum(case when gclb='66' then round(" + strMjFieldName + ",2) else 0 end) as ʡ�������̻�����," +
                                    "sum(case when gclb='70' then round(" + strMjFieldName + ",2) else 0 end) as ȫ��ʪ�ر�������," +
                                    "sum(case when gclb='71' then round(" + strMjFieldName + ",2) else 0 end) as ���Ź���̬�������������蹤��," +
                                    "sum(case when gclb='80' then round(" + strMjFieldName + ",2) else 0 end) as �ص㹫���־�Ӫ����," +
                                    "sum(case when gclb='81' then round(" + strMjFieldName + ",2) else 0 end) as �׶�ˮ��Դ�ɳ������ù���," +
                                    "sum(case when gclb='82' then round(" + strMjFieldName + ",2) else 0 end) as �ںӿ������κӵ�������," +
                                    "sum(case when gclb='83' then round(" + strMjFieldName + ",2) else 0 end) as С��ˮ����ˮ�����蹤��," +
                                    "sum(case when gclb='84' then round(" + strMjFieldName + ",2) else 0 end) as ʡ��Ӧ��ˮԴ����," +
                                    "sum(case when gclb='85' then round(" + strMjFieldName + ",2) else 0 end) as ʡ��ũ��ˮ����ȹ���," +
                                    "sum(case when gclb='86' then round(" + strMjFieldName + ",2) else 0 end) as ʡ��ˮ�������ٵذӹ���," +
                                    "sum(case when gclb='87' then round(" + strMjFieldName + ",2) else 0 end) as ʡ��ũ����ˮ��ȫ����," +
                                    "sum(case when gclb='88' then round(" + strMjFieldName + ",2) else 0 end) as ʡ�������ˮ����," +
                                    "sum(case when gclb='89' then round(" + strMjFieldName + ",2) else 0 end) as ʡ��ˮԴ��������," +
                                    "sum(case when gclb='90' then round(" + strMjFieldName + ",2) else 0 end) as ������ҵ����," +
                                    "sum(case when gclb='91' then round(" + strMjFieldName + ",2) else 0 end) as ����ũҵ����," +
                                    "sum(case when gclb='92' then round(" + strMjFieldName + ",2) else 0 end) as ����ˮ������  " +
                                    "  from " + tableName +
                                    "  group by substr(shi,1,4))";
                pWorkspace.ExecuteSQL(shiSQL);
                string xianSQL = "insert into DATA_GCLB ( select substr(xian,1,6) as ͳ�Ƶ�λ," +
                                    "sum(case when gclb<>' ' then round(" + strMjFieldName + ",2) else 0 end) as �ϼ�," +
                                    "sum(case when gclb='12' then round(" + strMjFieldName + ",2) else 0 end) as ��Ȼ�ֱ�������," +
                                    "sum(case when gclb='21' then round(" + strMjFieldName + ",2) else 0 end) as ���������ֹ���," +
                                    "sum(case when gclb='26' then round(" + strMjFieldName + ",2) else 0 end) as ̫��ɽ�̻�����," +
                                    "sum(case when gclb='27' then round(" + strMjFieldName + ",2) else 0 end) as ƽԭ�̻�����," +
                                    "sum(case when gclb='30'  then round(" + strMjFieldName + ",2) else 0 end) as �˸����ֹ���," +
                                    "sum(case when gclb='40' then round(" + strMjFieldName + ",2) else 0 end) as �����ɳԴ������," +
                                    "sum(case when gclb='51' then round(" + strMjFieldName + ",2) else 0 end) as Ұ����ֲ����Ҽ�������," +
                                    "sum(case when gclb='52' then round(" + strMjFieldName + ",2) else 0 end) as Ұ����ֲ��ط���������," +
                                    "sum(case when gclb='53' then round(" + strMjFieldName + ",2) else 0 end) as Ұ����ֲ���з���������," +
                                    "sum(case when gclb='54' then round(" + strMjFieldName + ",2) else 0 end) as Ұ����ֲ���ط���������," +
                                    "sum(case when gclb='60' then round(" + strMjFieldName + ",2) else 0 end) as ��������ֻ��ؽ��蹤��," +
                                    "sum(case when gclb='61' then round(" + strMjFieldName + ",2) else 0 end) as ʡ��ͨ���̻�����," +
                                    "sum(case when gclb='62' then round(" + strMjFieldName + ",2) else 0 end) as ʡ��ƽԭ�̻�����," +
                                    "sum(case when gclb='63' then round(" + strMjFieldName + ",2) else 0 end) as ʡ����ͨ���߻�ɽ�̻�����," +
                                    "sum(case when gclb='64' then round(" + strMjFieldName + ",2) else 0 end) as ʡ��԰�ִ����̻�����," +
                                    "sum(case when gclb='65' then round(" + strMjFieldName + ",2) else 0 end) as ʡ�������̻�����," +
                                    "sum(case when gclb='66' then round(" + strMjFieldName + ",2) else 0 end) as ʡ�������̻�����," +
                                    "sum(case when gclb='70' then round(" + strMjFieldName + ",2) else 0 end) as ȫ��ʪ�ر�������," +
                                    "sum(case when gclb='71' then round(" + strMjFieldName + ",2) else 0 end) as ���Ź���̬�������������蹤��," +
                                    "sum(case when gclb='80' then round(" + strMjFieldName + ",2) else 0 end) as �ص㹫���־�Ӫ����," +
                                    "sum(case when gclb='81' then round(" + strMjFieldName + ",2) else 0 end) as �׶�ˮ��Դ�ɳ������ù���," +
                                    "sum(case when gclb='82' then round(" + strMjFieldName + ",2) else 0 end) as �ںӿ������κӵ�������," +
                                    "sum(case when gclb='83' then round(" + strMjFieldName + ",2) else 0 end) as С��ˮ����ˮ�����蹤��," +
                                    "sum(case when gclb='84' then round(" + strMjFieldName + ",2) else 0 end) as ʡ��Ӧ��ˮԴ����," +
                                    "sum(case when gclb='85' then round(" + strMjFieldName + ",2) else 0 end) as ʡ��ũ��ˮ����ȹ���," +
                                    "sum(case when gclb='86' then round(" + strMjFieldName + ",2) else 0 end) as ʡ��ˮ�������ٵذӹ���," +
                                    "sum(case when gclb='87' then round(" + strMjFieldName + ",2) else 0 end) as ʡ��ũ����ˮ��ȫ����," +
                                    "sum(case when gclb='88' then round(" + strMjFieldName + ",2) else 0 end) as ʡ�������ˮ����," +
                                    "sum(case when gclb='89' then round(" + strMjFieldName + ",2) else 0 end) as ʡ��ˮԴ��������," +
                                    "sum(case when gclb='90' then round(" + strMjFieldName + ",2) else 0 end) as ������ҵ����," +
                                    "sum(case when gclb='91' then round(" + strMjFieldName + ",2) else 0 end) as ����ũҵ����," +
                                    "sum(case when gclb='92' then round(" + strMjFieldName + ",2) else 0 end) as ����ˮ������  " +
                                    "  from " + tableName +
                                    "  group by substr(xian,1,6))";
                pWorkspace.ExecuteSQL(xianSQL);
                string xiangSQL = "insert into DATA_GCLB ( select substr(xiang,1,8) as ͳ�Ƶ�λ," +
                                    "sum(case when gclb<>' ' then round(" + strMjFieldName + ",2) else 0 end) as �ϼ�," +
                                    "sum(case when gclb='12' then round(" + strMjFieldName + ",2) else 0 end) as ��Ȼ�ֱ�������," +
                                    "sum(case when gclb='21' then round(" + strMjFieldName + ",2) else 0 end) as ���������ֹ���," +
                                    "sum(case when gclb='26' then round(" + strMjFieldName + ",2) else 0 end) as ̫��ɽ�̻�����," +
                                    "sum(case when gclb='27' then round(" + strMjFieldName + ",2) else 0 end) as ƽԭ�̻�����," +
                                    "sum(case when gclb='30'  then round(" + strMjFieldName + ",2) else 0 end) as �˸����ֹ���," +
                                    "sum(case when gclb='40' then round(" + strMjFieldName + ",2) else 0 end) as �����ɳԴ������," +
                                    "sum(case when gclb='51' then round(" + strMjFieldName + ",2) else 0 end) as Ұ����ֲ����Ҽ�������," +
                                    "sum(case when gclb='52' then round(" + strMjFieldName + ",2) else 0 end) as Ұ����ֲ��ط���������," +
                                    "sum(case when gclb='53' then round(" + strMjFieldName + ",2) else 0 end) as Ұ����ֲ���з���������," +
                                    "sum(case when gclb='54' then round(" + strMjFieldName + ",2) else 0 end) as Ұ����ֲ���ط���������," +
                                    "sum(case when gclb='60' then round(" + strMjFieldName + ",2) else 0 end) as ��������ֻ��ؽ��蹤��," +
                                    "sum(case when gclb='61' then round(" + strMjFieldName + ",2) else 0 end) as ʡ��ͨ���̻�����," +
                                    "sum(case when gclb='62' then round(" + strMjFieldName + ",2) else 0 end) as ʡ��ƽԭ�̻�����," +
                                    "sum(case when gclb='63' then round(" + strMjFieldName + ",2) else 0 end) as ʡ����ͨ���߻�ɽ�̻�����," +
                                    "sum(case when gclb='64' then round(" + strMjFieldName + ",2) else 0 end) as ʡ��԰�ִ����̻�����," +
                                    "sum(case when gclb='65' then round(" + strMjFieldName + ",2) else 0 end) as ʡ�������̻�����," +
                                    "sum(case when gclb='66' then round(" + strMjFieldName + ",2) else 0 end) as ʡ�������̻�����," +
                                    "sum(case when gclb='70' then round(" + strMjFieldName + ",2) else 0 end) as ȫ��ʪ�ر�������," +
                                    "sum(case when gclb='71' then round(" + strMjFieldName + ",2) else 0 end) as ���Ź���̬�������������蹤��," +
                                    "sum(case when gclb='80' then round(" + strMjFieldName + ",2) else 0 end) as �ص㹫���־�Ӫ����," +
                                    "sum(case when gclb='81' then round(" + strMjFieldName + ",2) else 0 end) as �׶�ˮ��Դ�ɳ������ù���," +
                                    "sum(case when gclb='82' then round(" + strMjFieldName + ",2) else 0 end) as �ںӿ������κӵ�������," +
                                    "sum(case when gclb='83' then round(" + strMjFieldName + ",2) else 0 end) as С��ˮ����ˮ�����蹤��," +
                                    "sum(case when gclb='84' then round(" + strMjFieldName + ",2) else 0 end) as ʡ��Ӧ��ˮԴ����," +
                                    "sum(case when gclb='85' then round(" + strMjFieldName + ",2) else 0 end) as ʡ��ũ��ˮ����ȹ���," +
                                    "sum(case when gclb='86' then round(" + strMjFieldName + ",2) else 0 end) as ʡ��ˮ�������ٵذӹ���," +
                                    "sum(case when gclb='87' then round(" + strMjFieldName + ",2) else 0 end) as ʡ��ũ����ˮ��ȫ����," +
                                    "sum(case when gclb='88' then round(" + strMjFieldName + ",2) else 0 end) as ʡ�������ˮ����," +
                                    "sum(case when gclb='89' then round(" + strMjFieldName + ",2) else 0 end) as ʡ��ˮԴ��������," +
                                    "sum(case when gclb='90' then round(" + strMjFieldName + ",2) else 0 end) as ������ҵ����," +
                                    "sum(case when gclb='91' then round(" + strMjFieldName + ",2) else 0 end) as ����ũҵ����," +
                                    "sum(case when gclb='92' then round(" + strMjFieldName + ",2) else 0 end) as ����ˮ������  " +
                                    "  from " + tableName +
                                    "  group by substr(xiang,1,8))";
                pWorkspace.ExecuteSQL(xiangSQL);
                string lcSQL = "insert into DATA_GCLB ( select lc as ͳ�Ƶ�λ," +
                                    "sum(case when gclb<>' ' then round(" + strMjFieldName + ",2) else 0 end) as �ϼ�," +
                                    "sum(case when gclb='12' then round(" + strMjFieldName + ",2) else 0 end) as ��Ȼ�ֱ�������," +
                                    "sum(case when gclb='21' then round(" + strMjFieldName + ",2) else 0 end) as ���������ֹ���," +
                                    "sum(case when gclb='26' then round(" + strMjFieldName + ",2) else 0 end) as ̫��ɽ�̻�����," +
                                    "sum(case when gclb='27' then round(" + strMjFieldName + ",2) else 0 end) as ƽԭ�̻�����," +
                                    "sum(case when gclb='30'  then round(" + strMjFieldName + ",2) else 0 end) as �˸����ֹ���," +
                                    "sum(case when gclb='40' then round(" + strMjFieldName + ",2) else 0 end) as �����ɳԴ������," +
                                    "sum(case when gclb='51' then round(" + strMjFieldName + ",2) else 0 end) as Ұ����ֲ����Ҽ�������," +
                                    "sum(case when gclb='52' then round(" + strMjFieldName + ",2) else 0 end) as Ұ����ֲ��ط���������," +
                                    "sum(case when gclb='53' then round(" + strMjFieldName + ",2) else 0 end) as Ұ����ֲ���з���������," +
                                    "sum(case when gclb='54' then round(" + strMjFieldName + ",2) else 0 end) as Ұ����ֲ���ط���������," +
                                    "sum(case when gclb='60' then round(" + strMjFieldName + ",2) else 0 end) as ��������ֻ��ؽ��蹤��," +
                                    "sum(case when gclb='61' then round(" + strMjFieldName + ",2) else 0 end) as ʡ��ͨ���̻�����," +
                                    "sum(case when gclb='62' then round(" + strMjFieldName + ",2) else 0 end) as ʡ��ƽԭ�̻�����," +
                                    "sum(case when gclb='63' then round(" + strMjFieldName + ",2) else 0 end) as ʡ����ͨ���߻�ɽ�̻�����," +
                                    "sum(case when gclb='64' then round(" + strMjFieldName + ",2) else 0 end) as ʡ��԰�ִ����̻�����," +
                                    "sum(case when gclb='65' then round(" + strMjFieldName + ",2) else 0 end) as ʡ�������̻�����," +
                                    "sum(case when gclb='66' then round(" + strMjFieldName + ",2) else 0 end) as ʡ�������̻�����," +
                                    "sum(case when gclb='70' then round(" + strMjFieldName + ",2) else 0 end) as ȫ��ʪ�ر�������," +
                                    "sum(case when gclb='71' then round(" + strMjFieldName + ",2) else 0 end) as ���Ź���̬�������������蹤��," +
                                    "sum(case when gclb='80' then round(" + strMjFieldName + ",2) else 0 end) as �ص㹫���־�Ӫ����," +
                                    "sum(case when gclb='81' then round(" + strMjFieldName + ",2) else 0 end) as �׶�ˮ��Դ�ɳ������ù���," +
                                    "sum(case when gclb='82' then round(" + strMjFieldName + ",2) else 0 end) as �ںӿ������κӵ�������," +
                                    "sum(case when gclb='83' then round(" + strMjFieldName + ",2) else 0 end) as С��ˮ����ˮ�����蹤��," +
                                    "sum(case when gclb='84' then round(" + strMjFieldName + ",2) else 0 end) as ʡ��Ӧ��ˮԴ����," +
                                    "sum(case when gclb='85' then round(" + strMjFieldName + ",2) else 0 end) as ʡ��ũ��ˮ����ȹ���," +
                                    "sum(case when gclb='86' then round(" + strMjFieldName + ",2) else 0 end) as ʡ��ˮ�������ٵذӹ���," +
                                    "sum(case when gclb='87' then round(" + strMjFieldName + ",2) else 0 end) as ʡ��ũ����ˮ��ȫ����," +
                                    "sum(case when gclb='88' then round(" + strMjFieldName + ",2) else 0 end) as ʡ�������ˮ����," +
                                    "sum(case when gclb='89' then round(" + strMjFieldName + ",2) else 0 end) as ʡ��ˮԴ��������," +
                                    "sum(case when gclb='90' then round(" + strMjFieldName + ",2) else 0 end) as ������ҵ����," +
                                    "sum(case when gclb='91' then round(" + strMjFieldName + ",2) else 0 end) as ����ũҵ����," +
                                    "sum(case when gclb='92' then round(" + strMjFieldName + ",2) else 0 end) as ����ˮ������  " +
                                    "  from " + tableName +
                                    "  group by lc)";
                pWorkspace.ExecuteSQL(lcSQL);
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }
        //������Ϣͳ�� ygc 2013-01-17
        private static bool JBXX_Statistic(IFeatureClass pFeatureClass, string strMjFieldName,string strXJLFieldName)
        {
            if (pFeatureClass == null)
            {
                return false;
            }
            IWorkspace pWorkspace = null;
            string tableName = "";
            try
            {
                pWorkspace = pFeatureClass.FeatureDataset.Workspace;
                tableName = (pFeatureClass as IDataset).Name;
            }
            catch (Exception ex)
            { }
            DropTable(pWorkspace, "DATA_JBXX");
            try
            {
                string cunSQL = "create table DATA_JBXX as select substr(cun,1,10) as ͳ�Ƶ�λ," +
                                "sum(case when dl<> ' ' then round("+strMjFieldName+",2) else 0 end) as �������," +
                                "sum(case when dl like'1%' then round(" + strMjFieldName + ",2) else 0 end ) �ֵ����," +
                                "sum(case when dl like'2%' then round(" + strMjFieldName + ",2) else 0 end) ���ֵ����," +
                                "sum(case when dl in ('111','112','131' )then round(" + strMjFieldName + ") else 0 end ) as ɭ�ָ�����," +
                                "sum(case when dl in ('111','112','131','132','133' )then round(" + strMjFieldName + ") else 0 end ) as ��ľ������," +
                                "sum(case when dl <>' ' then round("+strXJLFieldName+",2) else 0 end ) as �����  " +
                                "  from "+tableName +
                                "  group by substr(cun,1,10) ";
                pWorkspace.ExecuteSQL(cunSQL);
                string shengSQL = "insert into  DATA_JBXX ( select substr(sheng,1,2) as ͳ�Ƶ�λ," +
                                "sum(case when dl<> ' ' then round(" + strMjFieldName + ",2) else 0 end) as �������," +
                                "sum(case when dl like'1%' then round(" + strMjFieldName + ",2) else 0 end ) �ֵ����," +
                                "sum(case when dl like'2%' then round(" + strMjFieldName + ",2) else 0 end) ���ֵ����," +
                                "sum(case when dl in ('111','112','131' )then round(" + strMjFieldName + ") else 0 end ) as ɭ�ָ�����," +
                                "sum(case when dl in ('111','112','131','132','133' )then round(" + strMjFieldName + ") else 0 end ) as ��ľ������," +
                                "sum(case when dl <>' ' then round(" + strXJLFieldName + ",2) else 0 end ) as �����  " +
                                "  from " + tableName +
                                "  group by substr(sheng,1,2)) ";
                pWorkspace.ExecuteSQL(shengSQL);
                string shiSQL = "insert into  DATA_JBXX ( select substr(shi,1,4) as ͳ�Ƶ�λ," +
                                "sum(case when dl<> ' ' then round(" + strMjFieldName + ",2) else 0 end) as �������," +
                                "sum(case when dl like'1%' then round(" + strMjFieldName + ",2) else 0 end ) �ֵ����," +
                                "sum(case when dl like'2%' then round(" + strMjFieldName + ",2) else 0 end) ���ֵ����," +
                                "sum(case when dl in ('111','112','131' )then round(" + strMjFieldName + ") else 0 end ) as ɭ�ָ�����," +
                                "sum(case when dl in ('111','112','131','132','133' )then round(" + strMjFieldName + ") else 0 end ) as ��ľ������," +
                                "sum(case when dl <>' ' then round(" + strXJLFieldName + ",2) else 0 end ) as �����  " +
                                "  from " + tableName +
                                "  group by substr(shi,1,4)) ";
                pWorkspace.ExecuteSQL(shiSQL);
                string xianSQL = "insert into  DATA_JBXX ( select substr(xian,1,6) as ͳ�Ƶ�λ," +
                                "sum(case when dl<> ' ' then round(" + strMjFieldName + ",2) else 0 end) as �������," +
                                "sum(case when dl like'1%' then round(" + strMjFieldName + ",2) else 0 end ) �ֵ����," +
                                "sum(case when dl like'2%' then round(" + strMjFieldName + ",2) else 0 end) ���ֵ����," +
                                "sum(case when dl in ('111','112','131' )then round(" + strMjFieldName + ") else 0 end ) as ɭ�ָ�����," +
                                "sum(case when dl in ('111','112','131','132','133' )then round(" + strMjFieldName + ") else 0 end ) as ��ľ������," +
                                "sum(case when dl <>' ' then round(" + strXJLFieldName + ",2) else 0 end ) as �����  " +
                                "  from " + tableName +
                                "  group by substr(xian,1,6)) ";
                pWorkspace.ExecuteSQL(xianSQL);
                string xiangSQL = "insert into  DATA_JBXX ( select substr(xiang,1,8) as ͳ�Ƶ�λ," +
                                "sum(case when dl<> ' ' then round(" + strMjFieldName + ",2) else 0 end) as �������," +
                                "sum(case when dl like'1%' then round(" + strMjFieldName + ",2) else 0 end ) �ֵ����," +
                                "sum(case when dl like'2%' then round(" + strMjFieldName + ",2) else 0 end) ���ֵ����," +
                                "sum(case when dl in ('111','112','131' )then round(" + strMjFieldName + ") else 0 end ) as ɭ�ָ�����," +
                                "sum(case when dl in ('111','112','131','132','133' )then round(" + strMjFieldName + ") else 0 end ) as ��ľ������," +
                                "sum(case when dl <>' ' then round(" + strXJLFieldName + ",2) else 0 end ) as �����  " +
                                "  from " + tableName +
                                "  group by substr(xiang,1,8)) ";
                pWorkspace.ExecuteSQL(xiangSQL);
                string lcSQL = "insert into  DATA_JBXX ( select lc as ͳ�Ƶ�λ," +
                                "sum(case when dl<> ' ' then round(" + strMjFieldName + ",2) else 0 end) as �������," +
                                "sum(case when dl like'1%' then round(" + strMjFieldName + ",2) else 0 end ) �ֵ����," +
                                "sum(case when dl like'2%' then round(" + strMjFieldName + ",2) else 0 end) ���ֵ����," +
                                "sum(case when dl in ('111','112','131' )then round(" + strMjFieldName + ") else 0 end ) as ɭ�ָ�����," +
                                "sum(case when dl in ('111','112','131','132','133' )then round(" + strMjFieldName + ") else 0 end ) as ��ľ������," +
                                "sum(case when dl <>' ' then round(" + strXJLFieldName + ",2) else 0 end ) as �����  " +
                                "  from " + tableName +
                                "  group by lc) ";
                pWorkspace.ExecuteSQL(lcSQL);
                //���¸�����
                pWorkspace.ExecuteSQL("update data_jbxx set ɭ�ָ�����=round((ɭ�ָ�����/�������)*100,2)");
                pWorkspace.ExecuteSQL("update data_jbxx set ��ľ������=round((��ľ������/�������)*100,2)");
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }
        //�ֵؽṹ��Ϣͳ�� ygc 2013-01-17
        private static bool LDJGXX_Statistic(IFeatureClass pFeatureClass, string strMjFieldName)
        {
            if (pFeatureClass == null)
            {
                return false;
            }
            IWorkspace pWorkspace = null;
            string tableName = "";
            try
            {
                pWorkspace = pFeatureClass.FeatureDataset.Workspace;
                tableName = (pFeatureClass as IDataset).Name;
            }
            catch (Exception ex)
            { }
            DropTable(pWorkspace, "DATA_LDJG");
            try
            {
                string cunSQL="create table DATA_LDJG as select substr(cun,1,10) as ͳ�Ƶ�λ,"+
                                "sum(case when sen_lin_lb <>' ' then  round("+strMjFieldName+",2) else 0 end) as �ϼ�,"+
                                "sum(case when sen_lin_lb like'1%' then round("+strMjFieldName+",2) else 0 end) as  ������С��,"+
                                "sum(case when sen_lin_lb='11' then round("+strMjFieldName+",2) else 0 end) as �ص㹫����,"+
                                "sum(case when sen_lin_lb='12' then round("+strMjFieldName+",2) else 0 end) as  һ�㹫����,"+
                                "sum(case when sen_lin_lb like'2%' then round("+strMjFieldName+",2) else 0 end) as ��Ʒ��С��,"+
                                "sum(case when sen_lin_lb ='21' then round("+strMjFieldName+",2) else 0 end) as �ص���Ʒ�� ��"+
                                "sum(case when sen_lin_lb='22' then round("+strMjFieldName+",2) else 0 end) as һ����Ʒ��"+
                                "  from "+tableName+
                                "  group by substr(cun,1,10)";
                pWorkspace .ExecuteSQL (cunSQL);
                string shengSQL="insert into DATA_LDJG (select substr(sheng,1,2) as ͳ�Ƶ�λ,"+
                                "sum(case when sen_lin_lb <>' ' then  round("+strMjFieldName+",2) else 0 end) as �ϼ�,"+
                                "sum(case when sen_lin_lb like'1%' then round("+strMjFieldName+",2) else 0 end) as  ������С��,"+
                                "sum(case when sen_lin_lb='11' then round("+strMjFieldName+",2) else 0 end) as �ص㹫����,"+
                                "sum(case when sen_lin_lb='12' then round("+strMjFieldName+",2) else 0 end) as  һ�㹫����,"+
                                "sum(case when sen_lin_lb like'2%' then round("+strMjFieldName+",2) else 0 end) as ��Ʒ��С��,"+
                                "sum(case when sen_lin_lb ='21' then round("+strMjFieldName+",2) else 0 end) as �ص���Ʒ�� ��"+
                                "sum(case when sen_lin_lb='22' then round("+strMjFieldName+",2) else 0 end) as һ����Ʒ��"+
                                "  from "+tableName+
                                "  group by substr(sheng,1,2))";
                pWorkspace .ExecuteSQL (shengSQL );
                string shiSQL="insert into DATA_LDJG (select substr(shi,1,4) as ͳ�Ƶ�λ,"+
                                "sum(case when sen_lin_lb <>' ' then  round("+strMjFieldName+",2) else 0 end) as �ϼ�,"+
                                "sum(case when sen_lin_lb like'1%' then round("+strMjFieldName+",2) else 0 end) as  ������С��,"+
                                "sum(case when sen_lin_lb='11' then round("+strMjFieldName+",2) else 0 end) as �ص㹫����,"+
                                "sum(case when sen_lin_lb='12' then round("+strMjFieldName+",2) else 0 end) as  һ�㹫����,"+
                                "sum(case when sen_lin_lb like'2%' then round("+strMjFieldName+",2) else 0 end) as ��Ʒ��С��,"+
                                "sum(case when sen_lin_lb ='21' then round("+strMjFieldName+",2) else 0 end) as �ص���Ʒ�� ��"+
                                "sum(case when sen_lin_lb='22' then round("+strMjFieldName+",2) else 0 end) as һ����Ʒ��"+
                                "  from "+tableName+
                                "  group by substr(shi,1,4))";
                pWorkspace .ExecuteSQL (shiSQL );
                string xianSQL="insert into DATA_LDJG (select substr(xian,1,6) as ͳ�Ƶ�λ,"+
                                "sum(case when sen_lin_lb <>' ' then  round("+strMjFieldName+",2) else 0 end) as �ϼ�,"+
                                "sum(case when sen_lin_lb like'1%' then round("+strMjFieldName+",2) else 0 end) as  ������С��,"+
                                "sum(case when sen_lin_lb='11' then round("+strMjFieldName+",2) else 0 end) as �ص㹫����,"+
                                "sum(case when sen_lin_lb='12' then round("+strMjFieldName+",2) else 0 end) as  һ�㹫����,"+
                                "sum(case when sen_lin_lb like'2%' then round("+strMjFieldName+",2) else 0 end) as ��Ʒ��С��,"+
                                "sum(case when sen_lin_lb ='21' then round("+strMjFieldName+",2) else 0 end) as �ص���Ʒ�� ��"+
                                "sum(case when sen_lin_lb='22' then round("+strMjFieldName+",2) else 0 end) as һ����Ʒ��"+
                                "  from "+tableName+
                                "  group by substr(xian,1,6))";
                pWorkspace .ExecuteSQL (xianSQL  );
                string xiangSQL="insert into DATA_LDJG (select substr(xiang,1,8) as ͳ�Ƶ�λ,"+
                                "sum(case when sen_lin_lb <>' ' then  round("+strMjFieldName+",2) else 0 end) as �ϼ�,"+
                                "sum(case when sen_lin_lb like'1%' then round("+strMjFieldName+",2) else 0 end) as  ������С��,"+
                                "sum(case when sen_lin_lb='11' then round("+strMjFieldName+",2) else 0 end) as �ص㹫����,"+
                                "sum(case when sen_lin_lb='12' then round("+strMjFieldName+",2) else 0 end) as  һ�㹫����,"+
                                "sum(case when sen_lin_lb like'2%' then round("+strMjFieldName+",2) else 0 end) as ��Ʒ��С��,"+
                                "sum(case when sen_lin_lb ='21' then round("+strMjFieldName+",2) else 0 end) as �ص���Ʒ�� ��"+
                                "sum(case when sen_lin_lb='22' then round("+strMjFieldName+",2) else 0 end) as һ����Ʒ��"+
                                "  from "+tableName+
                                "  group by substr(xiang,1,8))";
                pWorkspace .ExecuteSQL (xiangSQL );
                string lcSQL="insert into DATA_LDJG (select lc as ͳ�Ƶ�λ,"+
                                "sum(case when sen_lin_lb <>' ' then  round("+strMjFieldName+",2) else 0 end) as �ϼ�,"+
                                "sum(case when sen_lin_lb like'1%' then round("+strMjFieldName+",2) else 0 end) as  ������С��,"+
                                "sum(case when sen_lin_lb='11' then round("+strMjFieldName+",2) else 0 end) as �ص㹫����,"+
                                "sum(case when sen_lin_lb='12' then round("+strMjFieldName+",2) else 0 end) as  һ�㹫����,"+
                                "sum(case when sen_lin_lb like'2%' then round("+strMjFieldName+",2) else 0 end) as ��Ʒ��С��,"+
                                "sum(case when sen_lin_lb ='21' then round("+strMjFieldName+",2) else 0 end) as �ص���Ʒ�� ��"+
                                "sum(case when sen_lin_lb='22' then round("+strMjFieldName+",2) else 0 end) as һ����Ʒ��"+
                                "  from "+tableName+
                                "  group by lc)";
                pWorkspace.ExecuteSQL (lcSQL );
            }
            catch (Exception ex)
            {
                return false ;
            }
            return true;
        }
        //������Ϣͳ�� ygc 2013-01-17
        private static bool LZXX_Statistic(IFeatureClass pFeatureClass, string strMjFieldName)
        {
            if (pFeatureClass == null)
            {
                return false;
            }
            IWorkspace pWorkspace = null;
            string tableName = "";
            try
            {
                pWorkspace = pFeatureClass.FeatureDataset.Workspace;
                tableName = (pFeatureClass as IDataset).Name;
            }
            catch (Exception ex)
            { }
            DropTable(pWorkspace, "DATA_LZXX");
            try
            {
                string cunSQL = "create table DATA_LZXX as select substr(cun,1,10) as ͳ�Ƶ�λ," +
                                "sum(case when lz<>' ' then round("+strMjFieldName+",2) else 0 end ) as �ϼ�," +
                                "sum(case when lz between '111' and '117' then round(" + strMjFieldName + ",2) else 0 end) as ������С��," +
                                "sum(case when lz='111' then round(" + strMjFieldName + ",2) else 0 end) as ˮԴ������," +
                                "sum(case when lz='112' then round(" + strMjFieldName + ",2) else 0 end) as ˮ��������," +
                                "sum(case when lz='113' then round(" + strMjFieldName + ",2) else 0 end) as �����ɳ��," +
                                "sum(case when lz='114' then round(" + strMjFieldName + ",2) else 0 end) as ũ������������," +
                                "sum(case when lz='115' then round(" + strMjFieldName + ",2) else 0 end) as ������," +
                                "sum(case when lz='116' then round(" + strMjFieldName + ",2) else 0 end) as ��·��," +
                                "sum(case when lz='117' then round(" + strMjFieldName + ",2) else 0 end) as ����������," +
                                "sum(case when lz between '121' and '127' then round(" + strMjFieldName + ",2) else 0 end ) as ������;��С��," +
                                "sum(case when lz='121' then round(" + strMjFieldName + ",2) else 0 end) as ������," +
                                "sum(case when lz='122' then round(" + strMjFieldName + ",2) else 0 end) as ʵ����," +
                                "sum(case when lz='123' then round(" + strMjFieldName + ",2) else 0 end) as ĸ����," +
                                "sum(case when lz='124' then round(" + strMjFieldName + ",2) else 0 end) as ����������," +
                                "sum(case when lz='125' then round(" + strMjFieldName + ",2) else 0 end) as �羰��," +
                                "sum(case when lz='126' then round(" + strMjFieldName + ",2) else 0 end) as ��ʤ�ż��͸���������," +
                                "sum(case when lz='127' then round(" + strMjFieldName + ",2) else 0 end) as  ��Ȼ������," +
                                "sum(case when lz between '231' and '233' then round(" + strMjFieldName + ",2) else 0 end ) as �ò���С��," +
                                "sum(case when lz='231' then round(" + strMjFieldName + ",2) else 0 end) as һ���ò���," +
                                "sum(case when lz='232' then round(" + strMjFieldName + ",2) else 0 end) as  ��������ò���," +
                                "sum(case when lz='233' then round(" + strMjFieldName + ",2) else 0 end) as ���ַ��ڹ�ҵԭ�����ò���," +
                                "sum(case when lz='240' then round(" + strMjFieldName + ",2) else 0 end) as н̿��," +
                                "sum(case when lz between '251' and '255' then round(" + strMjFieldName + ",2) else 0 end) as ������С��," +
                                "sum(case when lz='251' then round(" + strMjFieldName + ",2) else 0 end) as ������," +
                                "sum(case when lz='252' then round(" + strMjFieldName + ",2) else 0 end) as ʳ��ԭ����," +
                                "sum(case when lz='253' then round(" + strMjFieldName + ",2) else 0 end) as �ֻ���ҵԭ����," +
                                "sum(case when lz='254' then round(" + strMjFieldName + ",2) else 0 end) as ҩ�ò���," +
                                "sum(case when lz='255' then round(" + strMjFieldName + ",2) else 0 end) as ����������" +
                                "  from "+tableName+
                                "  group by substr(cun,1,10)";
                pWorkspace.ExecuteSQL(cunSQL);
                string shengSQL = "insert  into DATA_LZXX ( select substr(sheng,1,2) as ͳ�Ƶ�λ," +
                                "sum(case when lz<>' ' then round(" + strMjFieldName + ",2) else 0 end ) as �ϼ�," +
                                "sum(case when lz between '111' and '117' then round(" + strMjFieldName + ",2) else 0 end) as ������С��," +
                                "sum(case when lz='111' then round(" + strMjFieldName + ",2) else 0 end) as ˮԴ������," +
                                "sum(case when lz='112' then round(" + strMjFieldName + ",2) else 0 end) as ˮ��������," +
                                "sum(case when lz='113' then round(" + strMjFieldName + ",2) else 0 end) as �����ɳ��," +
                                "sum(case when lz='114' then round(" + strMjFieldName + ",2) else 0 end) as ũ������������," +
                                "sum(case when lz='115' then round(" + strMjFieldName + ",2) else 0 end) as ������," +
                                "sum(case when lz='116' then round(" + strMjFieldName + ",2) else 0 end) as ��·��," +
                                "sum(case when lz='117' then round(" + strMjFieldName + ",2) else 0 end) as ����������," +
                                "sum(case when lz between '121' and '127' then round(" + strMjFieldName + ",2) else 0 end ) as ������;��С��," +
                                "sum(case when lz='121' then round(" + strMjFieldName + ",2) else 0 end) as ������," +
                                "sum(case when lz='122' then round(" + strMjFieldName + ",2) else 0 end) as ʵ����," +
                                "sum(case when lz='123' then round(" + strMjFieldName + ",2) else 0 end) as ĸ����," +
                                "sum(case when lz='124' then round(" + strMjFieldName + ",2) else 0 end) as ����������," +
                                "sum(case when lz='125' then round(" + strMjFieldName + ",2) else 0 end) as �羰��," +
                                "sum(case when lz='126' then round(" + strMjFieldName + ",2) else 0 end) as ��ʤ�ż��͸���������," +
                                "sum(case when lz='127' then round(" + strMjFieldName + ",2) else 0 end) as  ��Ȼ������," +
                                "sum(case when lz between '231' and '233' then round(" + strMjFieldName + ",2) else 0 end ) as �ò���С��," +
                                "sum(case when lz='231' then round(" + strMjFieldName + ",2) else 0 end) as һ���ò���," +
                                "sum(case when lz='232' then round(" + strMjFieldName + ",2) else 0 end) as  ��������ò���," +
                                "sum(case when lz='233' then round(" + strMjFieldName + ",2) else 0 end) as ���ַ��ڹ�ҵԭ�����ò���," +
                                "sum(case when lz='240' then round(" + strMjFieldName + ",2) else 0 end) as н̿��," +
                                "sum(case when lz between '251' and '255' then round(" + strMjFieldName + ",2) else 0 end) as ������С��," +
                                "sum(case when lz='251' then round(" + strMjFieldName + ",2) else 0 end) as ������," +
                                "sum(case when lz='252' then round(" + strMjFieldName + ",2) else 0 end) as ʳ��ԭ����," +
                                "sum(case when lz='253' then round(" + strMjFieldName + ",2) else 0 end) as �ֻ���ҵԭ����," +
                                "sum(case when lz='254' then round(" + strMjFieldName + ",2) else 0 end) as ҩ�ò���," +
                                "sum(case when lz='255' then round(" + strMjFieldName + ",2) else 0 end) as ����������" +
                                "  from " + tableName +
                                "  group by substr(sheng,1,2))";
                pWorkspace.ExecuteSQL(shengSQL);
                string shiSQL = "insert  into DATA_LZXX ( select substr(shi,1,4) as ͳ�Ƶ�λ," +
                                "sum(case when lz<>' ' then round(" + strMjFieldName + ",2) else 0 end ) as �ϼ�," +
                                "sum(case when lz between '111' and '117' then round(" + strMjFieldName + ",2) else 0 end) as ������С��," +
                                "sum(case when lz='111' then round(" + strMjFieldName + ",2) else 0 end) as ˮԴ������," +
                                "sum(case when lz='112' then round(" + strMjFieldName + ",2) else 0 end) as ˮ��������," +
                                "sum(case when lz='113' then round(" + strMjFieldName + ",2) else 0 end) as �����ɳ��," +
                                "sum(case when lz='114' then round(" + strMjFieldName + ",2) else 0 end) as ũ������������," +
                                "sum(case when lz='115' then round(" + strMjFieldName + ",2) else 0 end) as ������," +
                                "sum(case when lz='116' then round(" + strMjFieldName + ",2) else 0 end) as ��·��," +
                                "sum(case when lz='117' then round(" + strMjFieldName + ",2) else 0 end) as ����������," +
                                "sum(case when lz between '121' and '127' then round(" + strMjFieldName + ",2) else 0 end ) as ������;��С��," +
                                "sum(case when lz='121' then round(" + strMjFieldName + ",2) else 0 end) as ������," +
                                "sum(case when lz='122' then round(" + strMjFieldName + ",2) else 0 end) as ʵ����," +
                                "sum(case when lz='123' then round(" + strMjFieldName + ",2) else 0 end) as ĸ����," +
                                "sum(case when lz='124' then round(" + strMjFieldName + ",2) else 0 end) as ����������," +
                                "sum(case when lz='125' then round(" + strMjFieldName + ",2) else 0 end) as �羰��," +
                                "sum(case when lz='126' then round(" + strMjFieldName + ",2) else 0 end) as ��ʤ�ż��͸���������," +
                                "sum(case when lz='127' then round(" + strMjFieldName + ",2) else 0 end) as  ��Ȼ������," +
                                "sum(case when lz between '231' and '233' then round(" + strMjFieldName + ",2) else 0 end ) as �ò���С��," +
                                "sum(case when lz='231' then round(" + strMjFieldName + ",2) else 0 end) as һ���ò���," +
                                "sum(case when lz='232' then round(" + strMjFieldName + ",2) else 0 end) as  ��������ò���," +
                                "sum(case when lz='233' then round(" + strMjFieldName + ",2) else 0 end) as ���ַ��ڹ�ҵԭ�����ò���," +
                                "sum(case when lz='240' then round(" + strMjFieldName + ",2) else 0 end) as н̿��," +
                                "sum(case when lz between '251' and '255' then round(" + strMjFieldName + ",2) else 0 end) as ������С��," +
                                "sum(case when lz='251' then round(" + strMjFieldName + ",2) else 0 end) as ������," +
                                "sum(case when lz='252' then round(" + strMjFieldName + ",2) else 0 end) as ʳ��ԭ����," +
                                "sum(case when lz='253' then round(" + strMjFieldName + ",2) else 0 end) as �ֻ���ҵԭ����," +
                                "sum(case when lz='254' then round(" + strMjFieldName + ",2) else 0 end) as ҩ�ò���," +
                                "sum(case when lz='255' then round(" + strMjFieldName + ",2) else 0 end) as ����������" +
                                "  from " + tableName +
                                "  group by substr(shi,1,4))";
                pWorkspace.ExecuteSQL(shiSQL);
                string xianSQL = "insert  into DATA_LZXX ( select substr(xian,1,6) as ͳ�Ƶ�λ," +
                                "sum(case when lz<>' ' then round(" + strMjFieldName + ",2) else 0 end ) as �ϼ�," +
                                "sum(case when lz between '111' and '117' then round(" + strMjFieldName + ",2) else 0 end) as ������С��," +
                                "sum(case when lz='111' then round(" + strMjFieldName + ",2) else 0 end) as ˮԴ������," +
                                "sum(case when lz='112' then round(" + strMjFieldName + ",2) else 0 end) as ˮ��������," +
                                "sum(case when lz='113' then round(" + strMjFieldName + ",2) else 0 end) as �����ɳ��," +
                                "sum(case when lz='114' then round(" + strMjFieldName + ",2) else 0 end) as ũ������������," +
                                "sum(case when lz='115' then round(" + strMjFieldName + ",2) else 0 end) as ������," +
                                "sum(case when lz='116' then round(" + strMjFieldName + ",2) else 0 end) as ��·��," +
                                "sum(case when lz='117' then round(" + strMjFieldName + ",2) else 0 end) as ����������," +
                                "sum(case when lz between '121' and '127' then round(" + strMjFieldName + ",2) else 0 end ) as ������;��С��," +
                                "sum(case when lz='121' then round(" + strMjFieldName + ",2) else 0 end) as ������," +
                                "sum(case when lz='122' then round(" + strMjFieldName + ",2) else 0 end) as ʵ����," +
                                "sum(case when lz='123' then round(" + strMjFieldName + ",2) else 0 end) as ĸ����," +
                                "sum(case when lz='124' then round(" + strMjFieldName + ",2) else 0 end) as ����������," +
                                "sum(case when lz='125' then round(" + strMjFieldName + ",2) else 0 end) as �羰��," +
                                "sum(case when lz='126' then round(" + strMjFieldName + ",2) else 0 end) as ��ʤ�ż��͸���������," +
                                "sum(case when lz='127' then round(" + strMjFieldName + ",2) else 0 end) as  ��Ȼ������," +
                                "sum(case when lz between '231' and '233' then round(" + strMjFieldName + ",2) else 0 end ) as �ò���С��," +
                                "sum(case when lz='231' then round(" + strMjFieldName + ",2) else 0 end) as һ���ò���," +
                                "sum(case when lz='232' then round(" + strMjFieldName + ",2) else 0 end) as  ��������ò���," +
                                "sum(case when lz='233' then round(" + strMjFieldName + ",2) else 0 end) as ���ַ��ڹ�ҵԭ�����ò���," +
                                "sum(case when lz='240' then round(" + strMjFieldName + ",2) else 0 end) as н̿��," +
                                "sum(case when lz between '251' and '255' then round(" + strMjFieldName + ",2) else 0 end) as ������С��," +
                                "sum(case when lz='251' then round(" + strMjFieldName + ",2) else 0 end) as ������," +
                                "sum(case when lz='252' then round(" + strMjFieldName + ",2) else 0 end) as ʳ��ԭ����," +
                                "sum(case when lz='253' then round(" + strMjFieldName + ",2) else 0 end) as �ֻ���ҵԭ����," +
                                "sum(case when lz='254' then round(" + strMjFieldName + ",2) else 0 end) as ҩ�ò���," +
                                "sum(case when lz='255' then round(" + strMjFieldName + ",2) else 0 end) as ����������" +
                                "  from " + tableName +
                                "  group by substr(xian,1,6))";
                pWorkspace.ExecuteSQL(xianSQL);
                string xiangSQL = "insert  into DATA_LZXX ( select substr(xiang,1,8) as ͳ�Ƶ�λ," +
                                "sum(case when lz<>' ' then round(" + strMjFieldName + ",2) else 0 end ) as �ϼ�," +
                                "sum(case when lz between '111' and '117' then round(" + strMjFieldName + ",2) else 0 end) as ������С��," +
                                "sum(case when lz='111' then round(" + strMjFieldName + ",2) else 0 end) as ˮԴ������," +
                                "sum(case when lz='112' then round(" + strMjFieldName + ",2) else 0 end) as ˮ��������," +
                                "sum(case when lz='113' then round(" + strMjFieldName + ",2) else 0 end) as �����ɳ��," +
                                "sum(case when lz='114' then round(" + strMjFieldName + ",2) else 0 end) as ũ������������," +
                                "sum(case when lz='115' then round(" + strMjFieldName + ",2) else 0 end) as ������," +
                                "sum(case when lz='116' then round(" + strMjFieldName + ",2) else 0 end) as ��·��," +
                                "sum(case when lz='117' then round(" + strMjFieldName + ",2) else 0 end) as ����������," +
                                "sum(case when lz between '121' and '127' then round(" + strMjFieldName + ",2) else 0 end ) as ������;��С��," +
                                "sum(case when lz='121' then round(" + strMjFieldName + ",2) else 0 end) as ������," +
                                "sum(case when lz='122' then round(" + strMjFieldName + ",2) else 0 end) as ʵ����," +
                                "sum(case when lz='123' then round(" + strMjFieldName + ",2) else 0 end) as ĸ����," +
                                "sum(case when lz='124' then round(" + strMjFieldName + ",2) else 0 end) as ����������," +
                                "sum(case when lz='125' then round(" + strMjFieldName + ",2) else 0 end) as �羰��," +
                                "sum(case when lz='126' then round(" + strMjFieldName + ",2) else 0 end) as ��ʤ�ż��͸���������," +
                                "sum(case when lz='127' then round(" + strMjFieldName + ",2) else 0 end) as  ��Ȼ������," +
                                "sum(case when lz between '231' and '233' then round(" + strMjFieldName + ",2) else 0 end ) as �ò���С��," +
                                "sum(case when lz='231' then round(" + strMjFieldName + ",2) else 0 end) as һ���ò���," +
                                "sum(case when lz='232' then round(" + strMjFieldName + ",2) else 0 end) as  ��������ò���," +
                                "sum(case when lz='233' then round(" + strMjFieldName + ",2) else 0 end) as ���ַ��ڹ�ҵԭ�����ò���," +
                                "sum(case when lz='240' then round(" + strMjFieldName + ",2) else 0 end) as н̿��," +
                                "sum(case when lz between '251' and '255' then round(" + strMjFieldName + ",2) else 0 end) as ������С��," +
                                "sum(case when lz='251' then round(" + strMjFieldName + ",2) else 0 end) as ������," +
                                "sum(case when lz='252' then round(" + strMjFieldName + ",2) else 0 end) as ʳ��ԭ����," +
                                "sum(case when lz='253' then round(" + strMjFieldName + ",2) else 0 end) as �ֻ���ҵԭ����," +
                                "sum(case when lz='254' then round(" + strMjFieldName + ",2) else 0 end) as ҩ�ò���," +
                                "sum(case when lz='255' then round(" + strMjFieldName + ",2) else 0 end) as ����������" +
                                "  from " + tableName +
                                "  group by substr(xiang,1,8))";
                pWorkspace.ExecuteSQL(xiangSQL);
                string lcSQL = "insert  into DATA_LZXX ( select lc as ͳ�Ƶ�λ," +
                                "sum(case when lz<>' ' then round(" + strMjFieldName + ",2) else 0 end ) as �ϼ�," +
                                "sum(case when lz between '111' and '117' then round(" + strMjFieldName + ",2) else 0 end) as ������С��," +
                                "sum(case when lz='111' then round(" + strMjFieldName + ",2) else 0 end) as ˮԴ������," +
                                "sum(case when lz='112' then round(" + strMjFieldName + ",2) else 0 end) as ˮ��������," +
                                "sum(case when lz='113' then round(" + strMjFieldName + ",2) else 0 end) as �����ɳ��," +
                                "sum(case when lz='114' then round(" + strMjFieldName + ",2) else 0 end) as ũ������������," +
                                "sum(case when lz='115' then round(" + strMjFieldName + ",2) else 0 end) as ������," +
                                "sum(case when lz='116' then round(" + strMjFieldName + ",2) else 0 end) as ��·��," +
                                "sum(case when lz='117' then round(" + strMjFieldName + ",2) else 0 end) as ����������," +
                                "sum(case when lz between '121' and '127' then round(" + strMjFieldName + ",2) else 0 end ) as ������;��С��," +
                                "sum(case when lz='121' then round(" + strMjFieldName + ",2) else 0 end) as ������," +
                                "sum(case when lz='122' then round(" + strMjFieldName + ",2) else 0 end) as ʵ����," +
                                "sum(case when lz='123' then round(" + strMjFieldName + ",2) else 0 end) as ĸ����," +
                                "sum(case when lz='124' then round(" + strMjFieldName + ",2) else 0 end) as ����������," +
                                "sum(case when lz='125' then round(" + strMjFieldName + ",2) else 0 end) as �羰��," +
                                "sum(case when lz='126' then round(" + strMjFieldName + ",2) else 0 end) as ��ʤ�ż��͸���������," +
                                "sum(case when lz='127' then round(" + strMjFieldName + ",2) else 0 end) as  ��Ȼ������," +
                                "sum(case when lz between '231' and '233' then round(" + strMjFieldName + ",2) else 0 end ) as �ò���С��," +
                                "sum(case when lz='231' then round(" + strMjFieldName + ",2) else 0 end) as һ���ò���," +
                                "sum(case when lz='232' then round(" + strMjFieldName + ",2) else 0 end) as  ��������ò���," +
                                "sum(case when lz='233' then round(" + strMjFieldName + ",2) else 0 end) as ���ַ��ڹ�ҵԭ�����ò���," +
                                "sum(case when lz='240' then round(" + strMjFieldName + ",2) else 0 end) as н̿��," +
                                "sum(case when lz between '251' and '255' then round(" + strMjFieldName + ",2) else 0 end) as ������С��," +
                                "sum(case when lz='251' then round(" + strMjFieldName + ",2) else 0 end) as ������," +
                                "sum(case when lz='252' then round(" + strMjFieldName + ",2) else 0 end) as ʳ��ԭ����," +
                                "sum(case when lz='253' then round(" + strMjFieldName + ",2) else 0 end) as �ֻ���ҵԭ����," +
                                "sum(case when lz='254' then round(" + strMjFieldName + ",2) else 0 end) as ҩ�ò���," +
                                "sum(case when lz='255' then round(" + strMjFieldName + ",2) else 0 end) as ����������" +
                                "  from " + tableName +
                                "  group by lc)";
                pWorkspace.ExecuteSQL(lcSQL);
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }
        //��Դ��Ϣͳ�� ygc 2013-01-17
        private static bool QYXX_Statistic(IFeatureClass pFeatureClass, string strMjFieldName)
        {
            if (pFeatureClass == null)
            {
                return false;
            }
            IWorkspace pWorkspace = null;
            string tableName = "";
            try
            {
                pWorkspace = pFeatureClass.FeatureDataset.Workspace;
                tableName = (pFeatureClass as IDataset).Name;
            }
            catch (Exception ex)
            { }
            DropTable(pWorkspace, "DATA_QYXX");
            try
            {
                string cunSQL = "create table DATA_QYXX as select substr(cun,1,10) as ͳ�Ƶ�λ," +
                                "sum(case when qi_yuan <>' ' then round("+strMjFieldName+",2) else 0 end ) as �ϼ�," +
                                "sum(case when qi_yuan like'1%' then round(" + strMjFieldName + ",2) else 0 end) as  ��ȻС��," +
                                "sum(case when qi_yuan ='11' then round(" + strMjFieldName + ",2) else 0 end) as ����Ȼ," +
                                "sum(case when qi_yuan='12' then round(" + strMjFieldName + ",2) else 0 end) as �˹��ٽ�," +
                                "sum(case when qi_yuan='13' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when qi_yuan like'2%' then round(" + strMjFieldName + ",2) else 0 end) as �˹�С��," +
                                "sum(case when qi_yuan ='21' then round(" + strMjFieldName + ",2) else 0 end) as ֲ��," +
                                "sum(case when qi_yuan='22' then round(" + strMjFieldName + ",2) else 0 end) as  ֱ��," +
                                "sum(case when qi_yuan='23' then round(" + strMjFieldName + ",2) else 0 end) as  �ɲ�," +
                                "sum(case when qi_yuan='24' then round(" + strMjFieldName + ",2) else 0 end) as �˹�����" +
                                "  from "+tableName +
                                "  group by substr(cun,1,10)";
                pWorkspace.ExecuteSQL(cunSQL);
                string shengSQL = "insert into  DATA_QYXX ( select substr(sheng,1,2) as ͳ�Ƶ�λ," +
                                "sum(case when qi_yuan <>' ' then round(" + strMjFieldName + ",2) else 0 end ) as �ϼ�," +
                                "sum(case when qi_yuan like'1%' then round(" + strMjFieldName + ",2) else 0 end) as  ��ȻС��," +
                                "sum(case when qi_yuan ='11' then round(" + strMjFieldName + ",2) else 0 end) as ����Ȼ," +
                                "sum(case when qi_yuan='12' then round(" + strMjFieldName + ",2) else 0 end) as �˹��ٽ�," +
                                "sum(case when qi_yuan='13' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when qi_yuan like'2%' then round(" + strMjFieldName + ",2) else 0 end) as �˹�С��," +
                                "sum(case when qi_yuan ='21' then round(" + strMjFieldName + ",2) else 0 end) as ֲ��," +
                                "sum(case when qi_yuan='22' then round(" + strMjFieldName + ",2) else 0 end) as  ֱ��," +
                                "sum(case when qi_yuan='23' then round(" + strMjFieldName + ",2) else 0 end) as  �ɲ�," +
                                "sum(case when qi_yuan='24' then round(" + strMjFieldName + ",2) else 0 end) as �˹�����" +
                                "  from " + tableName +
                                "  group by substr(sheng,1,2))";
                pWorkspace.ExecuteSQL(shengSQL);
                string shiSQL = "insert into  DATA_QYXX ( select substr(shi,1,4) as ͳ�Ƶ�λ," +
                                "sum(case when qi_yuan <>' ' then round(" + strMjFieldName + ",2) else 0 end ) as �ϼ�," +
                                "sum(case when qi_yuan like'1%' then round(" + strMjFieldName + ",2) else 0 end) as  ��ȻС��," +
                                "sum(case when qi_yuan ='11' then round(" + strMjFieldName + ",2) else 0 end) as ����Ȼ," +
                                "sum(case when qi_yuan='12' then round(" + strMjFieldName + ",2) else 0 end) as �˹��ٽ�," +
                                "sum(case when qi_yuan='13' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when qi_yuan like'2%' then round(" + strMjFieldName + ",2) else 0 end) as �˹�С��," +
                                "sum(case when qi_yuan ='21' then round(" + strMjFieldName + ",2) else 0 end) as ֲ��," +
                                "sum(case when qi_yuan='22' then round(" + strMjFieldName + ",2) else 0 end) as  ֱ��," +
                                "sum(case when qi_yuan='23' then round(" + strMjFieldName + ",2) else 0 end) as  �ɲ�," +
                                "sum(case when qi_yuan='24' then round(" + strMjFieldName + ",2) else 0 end) as �˹�����" +
                                "  from " + tableName +
                                "  group by substr(shi,1,4))";
                pWorkspace.ExecuteSQL(shiSQL);
                string xianSQL = "insert into  DATA_QYXX ( select substr(xian,1,6) as ͳ�Ƶ�λ," +
                                "sum(case when qi_yuan <>' ' then round(" + strMjFieldName + ",2) else 0 end ) as �ϼ�," +
                                "sum(case when qi_yuan like'1%' then round(" + strMjFieldName + ",2) else 0 end) as  ��ȻС��," +
                                "sum(case when qi_yuan ='11' then round(" + strMjFieldName + ",2) else 0 end) as ����Ȼ," +
                                "sum(case when qi_yuan='12' then round(" + strMjFieldName + ",2) else 0 end) as �˹��ٽ�," +
                                "sum(case when qi_yuan='13' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when qi_yuan like'2%' then round(" + strMjFieldName + ",2) else 0 end) as �˹�С��," +
                                "sum(case when qi_yuan ='21' then round(" + strMjFieldName + ",2) else 0 end) as ֲ��," +
                                "sum(case when qi_yuan='22' then round(" + strMjFieldName + ",2) else 0 end) as  ֱ��," +
                                "sum(case when qi_yuan='23' then round(" + strMjFieldName + ",2) else 0 end) as  �ɲ�," +
                                "sum(case when qi_yuan='24' then round(" + strMjFieldName + ",2) else 0 end) as �˹�����" +
                                "  from " + tableName +
                                "  group by substr(xian,1,6))";
                pWorkspace.ExecuteSQL(xianSQL);
                string xiangSQL = "insert into  DATA_QYXX ( select substr(xiang,1,8) as ͳ�Ƶ�λ," +
                                "sum(case when qi_yuan <>' ' then round(" + strMjFieldName + ",2) else 0 end ) as �ϼ�," +
                                "sum(case when qi_yuan like'1%' then round(" + strMjFieldName + ",2) else 0 end) as  ��ȻС��," +
                                "sum(case when qi_yuan ='11' then round(" + strMjFieldName + ",2) else 0 end) as ����Ȼ," +
                                "sum(case when qi_yuan='12' then round(" + strMjFieldName + ",2) else 0 end) as �˹��ٽ�," +
                                "sum(case when qi_yuan='13' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when qi_yuan like'2%' then round(" + strMjFieldName + ",2) else 0 end) as �˹�С��," +
                                "sum(case when qi_yuan ='21' then round(" + strMjFieldName + ",2) else 0 end) as ֲ��," +
                                "sum(case when qi_yuan='22' then round(" + strMjFieldName + ",2) else 0 end) as  ֱ��," +
                                "sum(case when qi_yuan='23' then round(" + strMjFieldName + ",2) else 0 end) as  �ɲ�," +
                                "sum(case when qi_yuan='24' then round(" + strMjFieldName + ",2) else 0 end) as �˹�����" +
                                "  from " + tableName +
                                "  group by substr(xiang,1,8))";
                pWorkspace.ExecuteSQL(xiangSQL);
                string lcSQL = "insert into  DATA_QYXX ( select lc as ͳ�Ƶ�λ," +
                                "sum(case when qi_yuan <>' ' then round(" + strMjFieldName + ",2) else 0 end ) as �ϼ�," +
                                "sum(case when qi_yuan like'1%' then round(" + strMjFieldName + ",2) else 0 end) as  ��ȻС��," +
                                "sum(case when qi_yuan ='11' then round(" + strMjFieldName + ",2) else 0 end) as ����Ȼ," +
                                "sum(case when qi_yuan='12' then round(" + strMjFieldName + ",2) else 0 end) as �˹��ٽ�," +
                                "sum(case when qi_yuan='13' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when qi_yuan like'2%' then round(" + strMjFieldName + ",2) else 0 end) as �˹�С��," +
                                "sum(case when qi_yuan ='21' then round(" + strMjFieldName + ",2) else 0 end) as ֲ��," +
                                "sum(case when qi_yuan='22' then round(" + strMjFieldName + ",2) else 0 end) as  ֱ��," +
                                "sum(case when qi_yuan='23' then round(" + strMjFieldName + ",2) else 0 end) as  �ɲ�," +
                                "sum(case when qi_yuan='24' then round(" + strMjFieldName + ",2) else 0 end) as �˹�����" +
                                "  from " + tableName +
                                "  group by lc��";
                pWorkspace.ExecuteSQL(lcSQL);
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }
        //Ȩ����Ϣͳ�� ygc 2013-01-17
        private static bool QSXX_Statistic(IFeatureClass pFeatureClass, string strMjFieldName)
        {
            if (pFeatureClass == null)
            {
                return false;
            }
            IWorkspace pWorkspace = null;
            string tableName = "";
            try
            {
                pWorkspace = pFeatureClass.FeatureDataset.Workspace;
                tableName = (pFeatureClass as IDataset).Name;
            }
            catch (Exception ex)
            { }
            DropTable(pWorkspace, "DATA_QSXX");
            try
            {
                string cunSQL = "create table DATA_QSXX as select substr(cun,1,10) as ͳ�Ƶ�λ," +
                                "sum(case when ld_qs='10' then  round("+strMjFieldName+",2) else 0 end) as ����," +
                                "sum(case when ld_qs in ('21','22','23') then round(" + strMjFieldName + ",2) else 0 end ) as ����С��," +
                                "sum(case when ld_qs='21' then round(" + strMjFieldName + ",2) else 0 end) as ũ����ͥ�а���Ӫ," +
                                "sum(case when ld_qs='22' then round(" + strMjFieldName + ",2) else 0 end) as ����������Ӫ," +
                                "sum(case when ld_qs='23' then round(" + strMjFieldName + ",2) else 0 end) as ���徭����֯��Ӫ," +
                                "sum(case when ld_qs='20' then round(" + strMjFieldName + ",2) else 0 end) as  ����" +
                                "  from "+tableName +
                                "  group by substr(cun ,1,10) ";
                pWorkspace.ExecuteSQL(cunSQL);
                string shengSQL = "insert into  DATA_QSXX ( select substr(sheng,1,2) as ͳ�Ƶ�λ," +
                                "sum(case when ld_qs='10' then  round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when ld_qs in ('21','22','23') then round(" + strMjFieldName + ",2) else 0 end ) as ����С��," +
                                "sum(case when ld_qs='21' then round(" + strMjFieldName + ",2) else 0 end) as ũ����ͥ�а���Ӫ," +
                                "sum(case when ld_qs='22' then round(" + strMjFieldName + ",2) else 0 end) as ����������Ӫ," +
                                "sum(case when ld_qs='23' then round(" + strMjFieldName + ",2) else 0 end) as ���徭����֯��Ӫ," +
                                "sum(case when ld_qs='20' then round(" + strMjFieldName + ",2) else 0 end) as  ����" +
                                "  from " + tableName +
                                "  group by substr(sheng ,1,2)) ";
                pWorkspace.ExecuteSQL(shengSQL);
                string shiSQL = "insert into  DATA_QSXX ( select substr(shi,1,4) as ͳ�Ƶ�λ," +
                                "sum(case when ld_qs='10' then  round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when ld_qs in ('21','22','23') then round(" + strMjFieldName + ",2) else 0 end ) as ����С��," +
                                "sum(case when ld_qs='21' then round(" + strMjFieldName + ",2) else 0 end) as ũ����ͥ�а���Ӫ," +
                                "sum(case when ld_qs='22' then round(" + strMjFieldName + ",2) else 0 end) as ����������Ӫ," +
                                "sum(case when ld_qs='23' then round(" + strMjFieldName + ",2) else 0 end) as ���徭����֯��Ӫ," +
                                "sum(case when ld_qs='20' then round(" + strMjFieldName + ",2) else 0 end) as  ����" +
                                "  from " + tableName +
                                "  group by substr(shi ,1,4)) ";
                pWorkspace.ExecuteSQL(shiSQL);
                string xianSQL = "insert into  DATA_QSXX ( select substr(xian,1,6) as ͳ�Ƶ�λ," +
                                "sum(case when ld_qs='10' then  round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when ld_qs in ('21','22','23') then round(" + strMjFieldName + ",2) else 0 end ) as ����С��," +
                                "sum(case when ld_qs='21' then round(" + strMjFieldName + ",2) else 0 end) as ũ����ͥ�а���Ӫ," +
                                "sum(case when ld_qs='22' then round(" + strMjFieldName + ",2) else 0 end) as ����������Ӫ," +
                                "sum(case when ld_qs='23' then round(" + strMjFieldName + ",2) else 0 end) as ���徭����֯��Ӫ," +
                                "sum(case when ld_qs='20' then round(" + strMjFieldName + ",2) else 0 end) as  ����" +
                                "  from " + tableName +
                                "  group by substr(xian ,1,6)) ";
                pWorkspace.ExecuteSQL(xianSQL);
                string xiangSQL = "insert into  DATA_QSXX ( select substr(xiang,1,8) as ͳ�Ƶ�λ," +
                                "sum(case when ld_qs='10' then  round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when ld_qs in ('21','22','23') then round(" + strMjFieldName + ",2) else 0 end ) as ����С��," +
                                "sum(case when ld_qs='21' then round(" + strMjFieldName + ",2) else 0 end) as ũ����ͥ�а���Ӫ," +
                                "sum(case when ld_qs='22' then round(" + strMjFieldName + ",2) else 0 end) as ����������Ӫ," +
                                "sum(case when ld_qs='23' then round(" + strMjFieldName + ",2) else 0 end) as ���徭����֯��Ӫ," +
                                "sum(case when ld_qs='20' then round(" + strMjFieldName + ",2) else 0 end) as  ����" +
                                "  from " + tableName +
                                "  group by substr(xiang ,1,8)) ";
                pWorkspace.ExecuteSQL(xiangSQL);
                string lcSQL = "insert into  DATA_QSXX ( select lc as ͳ�Ƶ�λ," +
                                "sum(case when ld_qs='10' then  round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when ld_qs in ('21','22','23') then round(" + strMjFieldName + ",2) else 0 end ) as ����С��," +
                                "sum(case when ld_qs='21' then round(" + strMjFieldName + ",2) else 0 end) as ũ����ͥ�а���Ӫ," +
                                "sum(case when ld_qs='22' then round(" + strMjFieldName + ",2) else 0 end) as ����������Ӫ," +
                                "sum(case when ld_qs='23' then round(" + strMjFieldName + ",2) else 0 end) as ���徭����֯��Ӫ," +
                                "sum(case when ld_qs='20' then round(" + strMjFieldName + ",2) else 0 end) as  ����" +
                                "  from " + tableName +
                                "  group by lc) ";
                pWorkspace.ExecuteSQL(lcSQL);
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;

        }
        //�ֺ��ȼ���Ϣͳ�� ygc 2013-01-17
        private static bool ZHDJ_Statistic(IFeatureClass pFeatureClass, string strMjFieldName)
        {
            if (pFeatureClass == null)
            {
                return false;
            }
            IWorkspace pWorkspace = null;
            string tableName = "";
            try
            {
                pWorkspace = pFeatureClass.FeatureDataset.Workspace;
                tableName = (pFeatureClass as IDataset).Name;
            }
            catch (Exception ex)
            { }
            DropTable(pWorkspace, "DATA_ZHDJ");
            try
            {
                string cunSQL = "create table DATA_ZHDJ as select substr(cun,1,10) as ͳ�Ƶ�λ," +
                                "sum(case when disaster_c<>' ' then round("+strMjFieldName+",2) else 0 end) as �ϼ�," +
                                "sum(case when disaster_c='0'  then round(" + strMjFieldName + ",2) else 0 end) as ��," +
                                "sum(case when disaster_c='1' then round(" + strMjFieldName + ",2) else 0 end) as ��," +
                                "sum(case when disaster_c='2' then round(" + strMjFieldName + ",2) else 0 end) as ��," +
                                "sum(case when disaster_c='3' then round(" + strMjFieldName + ",2) else 0 end) as ��" +
                                "  from "+tableName +
                                "  group by substr(cun,1,10)";
                pWorkspace.ExecuteSQL(cunSQL);
                string shengSQL = "insert into  DATA_ZHDJ ( select substr(sheng,1,2) as ͳ�Ƶ�λ," +
                                "sum(case when disaster_c<>' ' then round(" + strMjFieldName + ",2) else 0 end) as �ϼ�," +
                                "sum(case when disaster_c='0'  then round(" + strMjFieldName + ",2) else 0 end) as ��," +
                                "sum(case when disaster_c='1' then round(" + strMjFieldName + ",2) else 0 end) as ��," +
                                "sum(case when disaster_c='2' then round(" + strMjFieldName + ",2) else 0 end) as ��," +
                                "sum(case when disaster_c='3' then round(" + strMjFieldName + ",2) else 0 end) as ��" +
                                "  from " + tableName +
                                "  group by substr(sheng,1,2))";
                pWorkspace.ExecuteSQL(shengSQL);
                string shiSQL = "insert into  DATA_ZHDJ ( select substr(shi,1,4) as ͳ�Ƶ�λ," +
                                "sum(case when disaster_c<>' ' then round(" + strMjFieldName + ",2) else 0 end) as �ϼ�," +
                                "sum(case when disaster_c='0'  then round(" + strMjFieldName + ",2) else 0 end) as ��," +
                                "sum(case when disaster_c='1' then round(" + strMjFieldName + ",2) else 0 end) as ��," +
                                "sum(case when disaster_c='2' then round(" + strMjFieldName + ",2) else 0 end) as ��," +
                                "sum(case when disaster_c='3' then round(" + strMjFieldName + ",2) else 0 end) as ��" +
                                "  from " + tableName +
                                "  group by substr(shi,1,4))";
                pWorkspace.ExecuteSQL(shiSQL);
                string xianSQL = "insert into  DATA_ZHDJ ( select substr(xian,1,6) as ͳ�Ƶ�λ," +
                                "sum(case when disaster_c<>' ' then round(" + strMjFieldName + ",2) else 0 end) as �ϼ�," +
                                "sum(case when disaster_c='0'  then round(" + strMjFieldName + ",2) else 0 end) as ��," +
                                "sum(case when disaster_c='1' then round(" + strMjFieldName + ",2) else 0 end) as ��," +
                                "sum(case when disaster_c='2' then round(" + strMjFieldName + ",2) else 0 end) as ��," +
                                "sum(case when disaster_c='3' then round(" + strMjFieldName + ",2) else 0 end) as ��" +
                                "  from " + tableName +
                                "  group by substr(xian,1,6))";
                pWorkspace.ExecuteSQL(xianSQL);
                string xiangSQL = "insert into  DATA_ZHDJ ( select substr(xiang,1,8) as ͳ�Ƶ�λ," +
                                "sum(case when disaster_c<>' ' then round(" + strMjFieldName + ",2) else 0 end) as �ϼ�," +
                                "sum(case when disaster_c='0'  then round(" + strMjFieldName + ",2) else 0 end) as ��," +
                                "sum(case when disaster_c='1' then round(" + strMjFieldName + ",2) else 0 end) as ��," +
                                "sum(case when disaster_c='2' then round(" + strMjFieldName + ",2) else 0 end) as ��," +
                                "sum(case when disaster_c='3' then round(" + strMjFieldName + ",2) else 0 end) as ��" +
                                "  from " + tableName +
                                "  group by substr(xiang,1,8))";
                pWorkspace.ExecuteSQL(xiangSQL);
                string lcSQL = "insert into  DATA_ZHDJ ( select lc as ͳ�Ƶ�λ," +
                                "sum(case when disaster_c<>' ' then round(" + strMjFieldName + ",2) else 0 end) as �ϼ�," +
                                "sum(case when disaster_c='0'  then round(" + strMjFieldName + ",2) else 0 end) as ��," +
                                "sum(case when disaster_c='1' then round(" + strMjFieldName + ",2) else 0 end) as ��," +
                                "sum(case when disaster_c='2' then round(" + strMjFieldName + ",2) else 0 end) as ��," +
                                "sum(case when disaster_c='3' then round(" + strMjFieldName + ",2) else 0 end) as ��" +
                                "  from " + tableName +
                                "  group by lc)";
                pWorkspace .ExecuteSQL(lcSQL );

            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }
        //�ֺ�������Ϣͳ�� ygc 2013-01-17
        private static bool ZHLX_Statistic(IFeatureClass pFeatureClass, string strMjFieldName)
        {
            if (pFeatureClass == null)
            {
                return false;
            }
            IWorkspace pWorkspace = null;
            string tableName = "";
            try
            {
                pWorkspace = pFeatureClass.FeatureDataset.Workspace;
                tableName = (pFeatureClass as IDataset).Name;
            }
            catch (Exception ex)
            { }
            DropTable(pWorkspace, "DATA_ZHLX");
            try
            {
                string cunSQL = "create table DATA_ZHLX as select substr(cun,1,10)  as ͳ�Ƶ�λ," +
                                "sum(case when dispe<>' ' then round("+strMjFieldName+",2) else 0 end ) as �ϼ�," +
                                "sum(case when dispe between '11' and '12' then round(" + strMjFieldName + ",2) else 0 end ) as ���溦С��," +
                                "sum(case when dispe='11' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when dispe='12' then round(" + strMjFieldName + ",2) else 0 end) as �溦," +
                                "sum(case when dispe='20' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when dispe between '31' and '34' then round(" + strMjFieldName + ",2) else 0 end) as �����ֺ�С��," +
                                "sum(case when dispe ='31' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when dispe='32' then round(" + strMjFieldName + ",2) else 0 end) as ѩѹ," +
                                "sum(case when dispe='33' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when dispe='34' then round(" + strMjFieldName + ",3) else 0 end) as �ɺ� ," +
                                "sum(case when dispe='40' then round(" + strMjFieldName + ",2) else 0 end) as �����ֺ�," +
                                "sum(case when dispe='00' then round(" + strMjFieldName + ",2) else 0 end) as ���ֺ�" +
                                "  from "+tableName +
                                "  group by substr(cun,1,10)";
                pWorkspace.ExecuteSQL(cunSQL);
                string shengSQL = "insert into  DATA_ZHLX  (select substr(sheng,1,2)  as ͳ�Ƶ�λ," +
                                "sum(case when dispe<>' ' then round(" + strMjFieldName + ",2) else 0 end ) as �ϼ�," +
                                "sum(case when dispe between '11' and '12' then round(" + strMjFieldName + ",2) else 0 end ) as ���溦С��," +
                                "sum(case when dispe='11' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when dispe='12' then round(" + strMjFieldName + ",2) else 0 end) as �溦," +
                                "sum(case when dispe='20' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when dispe between '31' and '34' then round(" + strMjFieldName + ",2) else 0 end) as �����ֺ�С��," +
                                "sum(case when dispe ='31' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when dispe='32' then round(" + strMjFieldName + ",2) else 0 end) as ѩѹ," +
                                "sum(case when dispe='33' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when dispe='34' then round(" + strMjFieldName + ",3) else 0 end) as �ɺ� ," +
                                "sum(case when dispe='40' then round(" + strMjFieldName + ",2) else 0 end) as �����ֺ�," +
                                "sum(case when dispe='00' then round(" + strMjFieldName + ",2) else 0 end) as ���ֺ�" +
                                "  from " + tableName +
                                "  group by substr(sheng,1,2))";
                pWorkspace.ExecuteSQL(shengSQL);
                string shiSQL = "insert into  DATA_ZHLX  (select substr(shi,1,4)  as ͳ�Ƶ�λ," +
                                "sum(case when dispe<>' ' then round(" + strMjFieldName + ",2) else 0 end ) as �ϼ�," +
                                "sum(case when dispe between '11' and '12' then round(" + strMjFieldName + ",2) else 0 end ) as ���溦С��," +
                                "sum(case when dispe='11' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when dispe='12' then round(" + strMjFieldName + ",2) else 0 end) as �溦," +
                                "sum(case when dispe='20' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when dispe between '31' and '34' then round(" + strMjFieldName + ",2) else 0 end) as �����ֺ�С��," +
                                "sum(case when dispe ='31' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when dispe='32' then round(" + strMjFieldName + ",2) else 0 end) as ѩѹ," +
                                "sum(case when dispe='33' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when dispe='34' then round(" + strMjFieldName + ",3) else 0 end) as �ɺ� ," +
                                "sum(case when dispe='40' then round(" + strMjFieldName + ",2) else 0 end) as �����ֺ�," +
                                "sum(case when dispe='00' then round(" + strMjFieldName + ",2) else 0 end) as ���ֺ�" +
                                "  from " + tableName +
                                "  group by substr(shi,1,4))";
                pWorkspace.ExecuteSQL(shiSQL);
                string xianSQL = "insert into  DATA_ZHLX  (select substr(xian,1,6)  as ͳ�Ƶ�λ," +
                                "sum(case when dispe<>' ' then round(" + strMjFieldName + ",2) else 0 end ) as �ϼ�," +
                                "sum(case when dispe between '11' and '12' then round(" + strMjFieldName + ",2) else 0 end ) as ���溦С��," +
                                "sum(case when dispe='11' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when dispe='12' then round(" + strMjFieldName + ",2) else 0 end) as �溦," +
                                "sum(case when dispe='20' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when dispe between '31' and '34' then round(" + strMjFieldName + ",2) else 0 end) as �����ֺ�С��," +
                                "sum(case when dispe ='31' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when dispe='32' then round(" + strMjFieldName + ",2) else 0 end) as ѩѹ," +
                                "sum(case when dispe='33' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when dispe='34' then round(" + strMjFieldName + ",3) else 0 end) as �ɺ� ," +
                                "sum(case when dispe='40' then round(" + strMjFieldName + ",2) else 0 end) as �����ֺ�," +
                                "sum(case when dispe='00' then round(" + strMjFieldName + ",2) else 0 end) as ���ֺ�" +
                                "  from " + tableName +
                                "  group by substr(xian,1,6))";
                pWorkspace.ExecuteSQL(xianSQL);
                string xiangSQL = "insert into  DATA_ZHLX  (select substr(xiang,1,8)  as ͳ�Ƶ�λ," +
                                "sum(case when dispe<>' ' then round(" + strMjFieldName + ",2) else 0 end ) as �ϼ�," +
                                "sum(case when dispe between '11' and '12' then round(" + strMjFieldName + ",2) else 0 end ) as ���溦С��," +
                                "sum(case when dispe='11' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when dispe='12' then round(" + strMjFieldName + ",2) else 0 end) as �溦," +
                                "sum(case when dispe='20' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when dispe between '31' and '34' then round(" + strMjFieldName + ",2) else 0 end) as �����ֺ�С��," +
                                "sum(case when dispe ='31' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when dispe='32' then round(" + strMjFieldName + ",2) else 0 end) as ѩѹ," +
                                "sum(case when dispe='33' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when dispe='34' then round(" + strMjFieldName + ",3) else 0 end) as �ɺ� ," +
                                "sum(case when dispe='40' then round(" + strMjFieldName + ",2) else 0 end) as �����ֺ�," +
                                "sum(case when dispe='00' then round(" + strMjFieldName + ",2) else 0 end) as ���ֺ�" +
                                "  from " + tableName +
                                "  group by substr(xiang,1,8))";
                pWorkspace.ExecuteSQL(xiangSQL);
                string lcSQL = "insert into  DATA_ZHLX  (select lc  as ͳ�Ƶ�λ," +
                                "sum(case when dispe<>' ' then round(" + strMjFieldName + ",2) else 0 end ) as �ϼ�," +
                                "sum(case when dispe between '11' and '12' then round(" + strMjFieldName + ",2) else 0 end ) as ���溦С��," +
                                "sum(case when dispe='11' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when dispe='12' then round(" + strMjFieldName + ",2) else 0 end) as �溦," +
                                "sum(case when dispe='20' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when dispe between '31' and '34' then round(" + strMjFieldName + ",2) else 0 end) as �����ֺ�С��," +
                                "sum(case when dispe ='31' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when dispe='32' then round(" + strMjFieldName + ",2) else 0 end) as ѩѹ," +
                                "sum(case when dispe='33' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when dispe='34' then round(" + strMjFieldName + ",3) else 0 end) as �ɺ� ," +
                                "sum(case when dispe='40' then round(" + strMjFieldName + ",2) else 0 end) as �����ֺ�," +
                                "sum(case when dispe='00' then round(" + strMjFieldName + ",2) else 0 end) as ���ֺ�" +
                                "  from " + tableName +
                                "  group by lc)";
                pWorkspace.ExecuteSQL(lcSQL);
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }
        //�����ȼ���Ϣͳ�� ygc 2013-01-17
        private static bool ZLDJ_Statistic(IFeatureClass pFeatureClass, string strMjFieldName)
        {
            if (pFeatureClass == null)
            {
                return false;
            }
            IWorkspace pWorkspace = null;
            string tableName = "";
            try
            {
                pWorkspace = pFeatureClass.FeatureDataset.Workspace;
                tableName = (pFeatureClass as IDataset).Name;
            }
            catch (Exception ex)
            { }
            DropTable(pWorkspace, "DATA_ZLDJXX");
            try
            {
                string cunSQL = "create table DATA_ZLDJXX as select substr(cun,1,10) as ͳ�Ƶ�λ," +
                                "sum(case when zl_dj<> ' ' then round("+strMjFieldName+",2) else 0 end) as �ϼ�," +
                                "sum(case when zl_dj='1' then round(" + strMjFieldName + ",2) else 0 end) as  һ��," +
                                "sum(case when zl_dj='2' then round(" + strMjFieldName + ",2) else 0 end) as  ����," +
                                "sum(case when zl_dj='3' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when zl_dj='4' then round(" + strMjFieldName + ",2) else 0 end) as �ļ���" +
                                "sum(case when zl_dj='5' then round(" + strMjFieldName + ",2) else 0 end) as �弶" +
                                "  from "+tableName +
                                "  group by substr(cun,1,10)";
                pWorkspace.ExecuteSQL(cunSQL);
                string shengSQL = "insert into  DATA_ZLDJXX ( select substr(sheng,1,2) as ͳ�Ƶ�λ," +
                                "sum(case when zl_dj<> ' ' then round(" + strMjFieldName + ",2) else 0 end) as �ϼ�," +
                                "sum(case when zl_dj='1' then round(" + strMjFieldName + ",2) else 0 end) as  һ��," +
                                "sum(case when zl_dj='2' then round(" + strMjFieldName + ",2) else 0 end) as  ����," +
                                "sum(case when zl_dj='3' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when zl_dj='4' then round(" + strMjFieldName + ",2) else 0 end) as �ļ���" +
                                "sum(case when zl_dj='5' then round(" + strMjFieldName + ",2) else 0 end) as �弶" +
                                "  from " + tableName +
                                "  group by substr(sheng,1,2))";
                pWorkspace.ExecuteSQL(shengSQL);
                string shiSQL = "insert into  DATA_ZLDJXX ( select substr(shi,1,4) as ͳ�Ƶ�λ," +
                                "sum(case when zl_dj<> ' ' then round(" + strMjFieldName + ",2) else 0 end) as �ϼ�," +
                                "sum(case when zl_dj='1' then round(" + strMjFieldName + ",2) else 0 end) as  һ��," +
                                "sum(case when zl_dj='2' then round(" + strMjFieldName + ",2) else 0 end) as  ����," +
                                "sum(case when zl_dj='3' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when zl_dj='4' then round(" + strMjFieldName + ",2) else 0 end) as �ļ���" +
                                "sum(case when zl_dj='5' then round(" + strMjFieldName + ",2) else 0 end) as �弶" +
                                "  from " + tableName +
                                "  group by substr(shi,1,4))";
                pWorkspace.ExecuteSQL(shiSQL);
                string xianSQL = "insert into  DATA_ZLDJXX ( select substr(xian,1,6) as ͳ�Ƶ�λ," +
                                "sum(case when zl_dj<> ' ' then round(" + strMjFieldName + ",2) else 0 end) as �ϼ�," +
                                "sum(case when zl_dj='1' then round(" + strMjFieldName + ",2) else 0 end) as  һ��," +
                                "sum(case when zl_dj='2' then round(" + strMjFieldName + ",2) else 0 end) as  ����," +
                                "sum(case when zl_dj='3' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when zl_dj='4' then round(" + strMjFieldName + ",2) else 0 end) as �ļ���" +
                                "sum(case when zl_dj='5' then round(" + strMjFieldName + ",2) else 0 end) as �弶" +
                                "  from " + tableName +
                                "  group by substr(xian,1,6))";
                pWorkspace.ExecuteSQL(xianSQL);
                string xiangSQL = "insert into  DATA_ZLDJXX ( select substr(xiang,1,8) as ͳ�Ƶ�λ," +
                                "sum(case when zl_dj<> ' ' then round(" + strMjFieldName + ",2) else 0 end) as �ϼ�," +
                                "sum(case when zl_dj='1' then round(" + strMjFieldName + ",2) else 0 end) as  һ��," +
                                "sum(case when zl_dj='2' then round(" + strMjFieldName + ",2) else 0 end) as  ����," +
                                "sum(case when zl_dj='3' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when zl_dj='4' then round(" + strMjFieldName + ",2) else 0 end) as �ļ���" +
                                "sum(case when zl_dj='5' then round(" + strMjFieldName + ",2) else 0 end) as �弶" +
                                "  from " + tableName +
                                "  group by substr(xiang,1,8))";
                pWorkspace.ExecuteSQL(xiangSQL);
                string lcSQL = "insert into  DATA_ZLDJXX ( select lc as ͳ�Ƶ�λ," +
                                "sum(case when zl_dj<> ' ' then round(" + strMjFieldName + ",2) else 0 end) as �ϼ�," +
                                "sum(case when zl_dj='1' then round(" + strMjFieldName + ",2) else 0 end) as  һ��," +
                                "sum(case when zl_dj='2' then round(" + strMjFieldName + ",2) else 0 end) as  ����," +
                                "sum(case when zl_dj='3' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when zl_dj='4' then round(" + strMjFieldName + ",2) else 0 end) as �ļ���" +
                                "sum(case when zl_dj='5' then round(" + strMjFieldName + ",2) else 0 end) as �弶" +
                                "  from " + tableName +
                                "  group by lc)";
                pWorkspace.ExecuteSQL(lcSQL);
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }
        //��Ҫ������Ϣ ygc 2013-01-17
        private static bool ZYSZ_Statistic(IFeatureClass pFeatureClass, string strMjFieldName)
        {
            if (pFeatureClass == null)
            {
                return false;
            }
            IWorkspace pWorkspace = null;
            string tableName = "";
            try
            {
                pWorkspace = pFeatureClass.FeatureDataset.Workspace;
                tableName = (pFeatureClass as IDataset).Name;
            }
            catch (Exception ex)
            { }
            DropTable(pWorkspace, "DATA_SZXX");
            try
            {
                string cunSQL = "create table DATA_SZXX as select substr(cun,1,10) as ͳ�Ƶ�λ," +
                                "sum(case when zysz<>' ' then round("+strMjFieldName+",2) else 0 end) as �ϼ�," +
                                "sum(case when zysz='905' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when zysz='906' then round(" + strMjFieldName + ",2) else 0 end) as Ů��," +
                                "sum(case when zysz='907' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when zysz='908' then round(" + strMjFieldName + ",2) else 0 end) as �޻���," +
                                "sum(case when zysz='909' then round(" + strMjFieldName + ",2) else 0 end) as ��ľ������," +
                                "sum(case when zysz='910' then round(" + strMjFieldName + ",2) else 0 end) as ɳ��," +
                                "sum(case when zysz='911' then round(" + strMjFieldName + ",2) else 0 end) as ɽ��," +
                                "sum(case when zysz='912' then round(" + strMjFieldName + ",2) else 0 end) as ɽ��," +
                                "sum(case when zysz='913' then round(" + strMjFieldName + ",2) else 0 end) as ���߾�," +
                                "sum(case when zysz='914' then round(" + strMjFieldName + ",2) else 0 end) as ����ľ," +
                                "sum(case when zysz='915' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when zysz='916' then round(" + strMjFieldName + ",2) else 0 end) as �ƴ�õ," +
                                "sum(case when zysz='917' then round(" + strMjFieldName + ",2) else 0 end) as ɽ���," +
                                "sum(case when zysz='918' then round(" + strMjFieldName + ",2) else 0 end) as ���뻱," +
                                "sum(case when zysz='919' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when zysz='920' then round(" + strMjFieldName + ",2) else 0 end) as ��֦��," +
                                "sum(case when zysz='921' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when zysz='922' then round(" + strMjFieldName + ",2) else 0 end) as ���," +
                                "sum(case when zysz='923' then round(" + strMjFieldName + ",2) else 0 end) as ���," +
                                "sum(case when zysz='924' then round(" + strMjFieldName + ",2) else 0 end) as �����," +
                                "sum(case when zysz='925' then round(" + strMjFieldName + ",2) else 0 end) as ������," +
                                "sum(case when zysz='926' then round(" + strMjFieldName + ",2) else 0 end) as �̶�," +
                                "sum(case when zysz='927' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when zysz='928' then round(" + strMjFieldName + ",2) else 0 end) as ��������," +
                                "sum(case when zysz='929' then round(" + strMjFieldName + ",2) else 0 end) as �����," +
                                "sum(case when zysz='930' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when zysz='931' then round(" + strMjFieldName + ",2) else 0 end) as ��ì," +
                                "sum(case when zysz='932' then round(" + strMjFieldName + ",2) else 0 end) as С��," +
                                "sum(case when zysz='933' then round(" + strMjFieldName + ",2) else 0 end) as �ؽ�," +
                                "sum(case when zysz='934' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when zysz='935' then round(" + strMjFieldName + ",2) else 0 end) as ��˷�黨," +
                                "sum(case when zysz='936' then round(" + strMjFieldName + ",2) else 0 end) as ������," +
                                "sum(case when zysz='937' then round(" + strMjFieldName + ",2) else 0 end) as ����ľ," +
                                "sum(case when zysz='938' then round(" + strMjFieldName + ",2) else 0 end) as ��ɽ��," +
                                "sum(case when zysz='939' then round(" + strMjFieldName + ",2) else 0 end) as ������," +
                                "sum(case when zysz='940' then round(" + strMjFieldName + ",2) else 0 end) as ������," +
                                "sum(case when zysz='999' then round(" + strMjFieldName + ",2) else 0 end) as ������ľ����," +
                                "sum(case when zysz='702' then round(" + strMjFieldName + ",2) else 0 end) as ƻ��," +
                                "sum(case when zysz='703' then round(" + strMjFieldName + ",2) else 0 end) as ��," +
                                "sum(case when zysz='704' then round(" + strMjFieldName + ",2) else 0 end) as ��," +
                                "sum(case when zysz='705' then round(" + strMjFieldName + ",2) else 0 end) as ��," +
                                "sum(case when zysz='706' then round(" + strMjFieldName + ",2) else 0 end) as ��," +
                                "sum(case when zysz='707' then round(" + strMjFieldName + ",2) else 0 end) as ��," +
                                "sum(case when zysz='708' then round(" + strMjFieldName + ",2) else 0 end) as ���," +
                                "sum(case when zysz='709' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when zysz='710' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when zysz='711' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when zysz='720' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when zysz='722' then round(" + strMjFieldName + ",2) else 0 end) as ����������," +
                                "sum(case when zysz='749' then round(" + strMjFieldName + ",2) else 0 end) as ������������," +
                                "sum(case when zysz='758' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when zysz='799' then round(" + strMjFieldName + ",2) else 0 end) as �����," +
                                "sum(case when zysz='851' then round(" + strMjFieldName + ",2) else 0 end) as ɣ��," +
                                "sum(case when zysz='120' then round(" + strMjFieldName + ",2) else 0 end) as ��ɼ," +
                                "sum(case when zysz='150' then round(" + strMjFieldName + ",2) else 0 end) as ��Ҷ��," +
                                "sum(case when zysz='170' then round(" + strMjFieldName + ",2) else 0 end) as ������," +
                                "sum(case when zysz='200' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when zysz='210' then round(" + strMjFieldName + ",2) else 0 end) as ��ɽ��," +
                                "sum(case when zysz='350' then round(" + strMjFieldName + ",2) else 0 end) as ���," +
                                "sum(case when zysz='411' then round(" + strMjFieldName + ",2) else 0 end) as ˨Ƥ��," +
                                "sum(case when zysz='412' then round(" + strMjFieldName + ",2) else 0 end) as �ɶ���," +
                                "sum(case when zysz='413' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when zysz='414' then round(" + strMjFieldName + ",2) else 0 end) as �����," +
                                "sum(case when zysz='420' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when zysz='421' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when zysz='490' then round(" + strMjFieldName + ",2) else 0 end) as Ӳ����," +
                                "sum(case when zysz='491' then round(" + strMjFieldName + ",2) else 0 end) as �̻�," +
                                "sum(case when zysz='492' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when zysz='493' then round(" + strMjFieldName + ",2) else 0 end) as �^��ľ," +
                                "sum(case when zysz='494' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when zysz='530' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when zysz='531' then round(" + strMjFieldName + ",2) else 0 end) as ��������," +
                                "sum(case when zysz='535' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when zysz='540' then round(" + strMjFieldName + ",2) else 0 end) as ��ͩ," +
                                "sum(case when zysz='545' then round(" + strMjFieldName + ",2) else 0 end) as ����������ľ," +
                                "sum(case when zysz='590' then round(" + strMjFieldName + ",2) else 0 end) as ������," +
                                "sum(case when zysz='591' then round(" + strMjFieldName + ",2) else 0 end) as ɽ��," +
                                "sum(case when zysz='670' then round(" + strMjFieldName + ",2) else 0 end) as ������," +
                                "sum(case when zysz='130' then round(" + strMjFieldName + ",2) else 0 end) as ����ɼ," +
                                "sum(case when zysz='360' then round(" + strMjFieldName + ",2) else 0 end) as ���," +
                                "sum(case when zysz='380' then round(" + strMjFieldName + ",2) else 0 end) as �춹ɼ," +
                                "sum(case when zysz='753' then round(" + strMjFieldName + ",2) else 0 end) as �Ĺڹ�," +
                                "sum(case when zysz='271' then round(" + strMjFieldName + ",2) else 0 end) as ��Ƥ��" +
                                "  from "+tableName +
                                "  group by  substr(cun,1,10)";
                pWorkspace.ExecuteSQL(cunSQL);
                string shengSQL = "insert into  DATA_SZXX ( select substr(sheng,1,2) as ͳ�Ƶ�λ," +
                                "sum(case when zysz<>' ' then round(" + strMjFieldName + ",2) else 0 end) as �ϼ�," +
                                "sum(case when zysz='905' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when zysz='906' then round(" + strMjFieldName + ",2) else 0 end) as Ů��," +
                                "sum(case when zysz='907' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when zysz='908' then round(" + strMjFieldName + ",2) else 0 end) as �޻���," +
                                "sum(case when zysz='909' then round(" + strMjFieldName + ",2) else 0 end) as ��ľ������," +
                                "sum(case when zysz='910' then round(" + strMjFieldName + ",2) else 0 end) as ɳ��," +
                                "sum(case when zysz='911' then round(" + strMjFieldName + ",2) else 0 end) as ɽ��," +
                                "sum(case when zysz='912' then round(" + strMjFieldName + ",2) else 0 end) as ɽ��," +
                                "sum(case when zysz='913' then round(" + strMjFieldName + ",2) else 0 end) as ���߾�," +
                                "sum(case when zysz='914' then round(" + strMjFieldName + ",2) else 0 end) as ����ľ," +
                                "sum(case when zysz='915' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when zysz='916' then round(" + strMjFieldName + ",2) else 0 end) as �ƴ�õ," +
                                "sum(case when zysz='917' then round(" + strMjFieldName + ",2) else 0 end) as ɽ���," +
                                "sum(case when zysz='918' then round(" + strMjFieldName + ",2) else 0 end) as ���뻱," +
                                "sum(case when zysz='919' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when zysz='920' then round(" + strMjFieldName + ",2) else 0 end) as ��֦��," +
                                "sum(case when zysz='921' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when zysz='922' then round(" + strMjFieldName + ",2) else 0 end) as ���," +
                                "sum(case when zysz='923' then round(" + strMjFieldName + ",2) else 0 end) as ���," +
                                "sum(case when zysz='924' then round(" + strMjFieldName + ",2) else 0 end) as �����," +
                                "sum(case when zysz='925' then round(" + strMjFieldName + ",2) else 0 end) as ������," +
                                "sum(case when zysz='926' then round(" + strMjFieldName + ",2) else 0 end) as �̶�," +
                                "sum(case when zysz='927' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when zysz='928' then round(" + strMjFieldName + ",2) else 0 end) as ��������," +
                                "sum(case when zysz='929' then round(" + strMjFieldName + ",2) else 0 end) as �����," +
                                "sum(case when zysz='930' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when zysz='931' then round(" + strMjFieldName + ",2) else 0 end) as ��ì," +
                                "sum(case when zysz='932' then round(" + strMjFieldName + ",2) else 0 end) as С��," +
                                "sum(case when zysz='933' then round(" + strMjFieldName + ",2) else 0 end) as �ؽ�," +
                                "sum(case when zysz='934' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when zysz='935' then round(" + strMjFieldName + ",2) else 0 end) as ��˷�黨," +
                                "sum(case when zysz='936' then round(" + strMjFieldName + ",2) else 0 end) as ������," +
                                "sum(case when zysz='937' then round(" + strMjFieldName + ",2) else 0 end) as ����ľ," +
                                "sum(case when zysz='938' then round(" + strMjFieldName + ",2) else 0 end) as ��ɽ��," +
                                "sum(case when zysz='939' then round(" + strMjFieldName + ",2) else 0 end) as ������," +
                                "sum(case when zysz='940' then round(" + strMjFieldName + ",2) else 0 end) as ������," +
                                "sum(case when zysz='999' then round(" + strMjFieldName + ",2) else 0 end) as ������ľ����," +
                                "sum(case when zysz='702' then round(" + strMjFieldName + ",2) else 0 end) as ƻ��," +
                                "sum(case when zysz='703' then round(" + strMjFieldName + ",2) else 0 end) as ��," +
                                "sum(case when zysz='704' then round(" + strMjFieldName + ",2) else 0 end) as ��," +
                                "sum(case when zysz='705' then round(" + strMjFieldName + ",2) else 0 end) as ��," +
                                "sum(case when zysz='706' then round(" + strMjFieldName + ",2) else 0 end) as ��," +
                                "sum(case when zysz='707' then round(" + strMjFieldName + ",2) else 0 end) as ��," +
                                "sum(case when zysz='708' then round(" + strMjFieldName + ",2) else 0 end) as ���," +
                                "sum(case when zysz='709' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when zysz='710' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when zysz='711' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when zysz='720' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when zysz='722' then round(" + strMjFieldName + ",2) else 0 end) as ����������," +
                                "sum(case when zysz='749' then round(" + strMjFieldName + ",2) else 0 end) as ������������," +
                                "sum(case when zysz='758' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when zysz='799' then round(" + strMjFieldName + ",2) else 0 end) as �����," +
                                "sum(case when zysz='851' then round(" + strMjFieldName + ",2) else 0 end) as ɣ��," +
                                "sum(case when zysz='120' then round(" + strMjFieldName + ",2) else 0 end) as ��ɼ," +
                                "sum(case when zysz='150' then round(" + strMjFieldName + ",2) else 0 end) as ��Ҷ��," +
                                "sum(case when zysz='170' then round(" + strMjFieldName + ",2) else 0 end) as ������," +
                                "sum(case when zysz='200' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when zysz='210' then round(" + strMjFieldName + ",2) else 0 end) as ��ɽ��," +
                                "sum(case when zysz='350' then round(" + strMjFieldName + ",2) else 0 end) as ���," +
                                "sum(case when zysz='411' then round(" + strMjFieldName + ",2) else 0 end) as ˨Ƥ��," +
                                "sum(case when zysz='412' then round(" + strMjFieldName + ",2) else 0 end) as �ɶ���," +
                                "sum(case when zysz='413' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when zysz='414' then round(" + strMjFieldName + ",2) else 0 end) as �����," +
                                "sum(case when zysz='420' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when zysz='421' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when zysz='490' then round(" + strMjFieldName + ",2) else 0 end) as Ӳ����," +
                                "sum(case when zysz='491' then round(" + strMjFieldName + ",2) else 0 end) as �̻�," +
                                "sum(case when zysz='492' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when zysz='493' then round(" + strMjFieldName + ",2) else 0 end) as �^��ľ," +
                                "sum(case when zysz='494' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when zysz='530' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when zysz='531' then round(" + strMjFieldName + ",2) else 0 end) as ��������," +
                                "sum(case when zysz='535' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when zysz='540' then round(" + strMjFieldName + ",2) else 0 end) as ��ͩ," +
                                "sum(case when zysz='545' then round(" + strMjFieldName + ",2) else 0 end) as ����������ľ," +
                                "sum(case when zysz='590' then round(" + strMjFieldName + ",2) else 0 end) as ������," +
                                "sum(case when zysz='591' then round(" + strMjFieldName + ",2) else 0 end) as ɽ��," +
                                "sum(case when zysz='670' then round(" + strMjFieldName + ",2) else 0 end) as ������," +
                                "sum(case when zysz='130' then round(" + strMjFieldName + ",2) else 0 end) as ����ɼ," +
                                "sum(case when zysz='360' then round(" + strMjFieldName + ",2) else 0 end) as ���," +
                                "sum(case when zysz='380' then round(" + strMjFieldName + ",2) else 0 end) as �춹ɼ," +
                                "sum(case when zysz='753' then round(" + strMjFieldName + ",2) else 0 end) as �Ĺڹ�," +
                                "sum(case when zysz='271' then round(" + strMjFieldName + ",2) else 0 end) as ��Ƥ��" +
                                "  from " + tableName +
                                "  group by  substr(sheng,1,2))";
                pWorkspace.ExecuteSQL(shengSQL);
                string shiSQL = "insert into  DATA_SZXX ( select substr(shi,1,4) as ͳ�Ƶ�λ," +
                                "sum(case when zysz<>' ' then round(" + strMjFieldName + ",2) else 0 end) as �ϼ�," +
                                "sum(case when zysz='905' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when zysz='906' then round(" + strMjFieldName + ",2) else 0 end) as Ů��," +
                                "sum(case when zysz='907' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when zysz='908' then round(" + strMjFieldName + ",2) else 0 end) as �޻���," +
                                "sum(case when zysz='909' then round(" + strMjFieldName + ",2) else 0 end) as ��ľ������," +
                                "sum(case when zysz='910' then round(" + strMjFieldName + ",2) else 0 end) as ɳ��," +
                                "sum(case when zysz='911' then round(" + strMjFieldName + ",2) else 0 end) as ɽ��," +
                                "sum(case when zysz='912' then round(" + strMjFieldName + ",2) else 0 end) as ɽ��," +
                                "sum(case when zysz='913' then round(" + strMjFieldName + ",2) else 0 end) as ���߾�," +
                                "sum(case when zysz='914' then round(" + strMjFieldName + ",2) else 0 end) as ����ľ," +
                                "sum(case when zysz='915' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when zysz='916' then round(" + strMjFieldName + ",2) else 0 end) as �ƴ�õ," +
                                "sum(case when zysz='917' then round(" + strMjFieldName + ",2) else 0 end) as ɽ���," +
                                "sum(case when zysz='918' then round(" + strMjFieldName + ",2) else 0 end) as ���뻱," +
                                "sum(case when zysz='919' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when zysz='920' then round(" + strMjFieldName + ",2) else 0 end) as ��֦��," +
                                "sum(case when zysz='921' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when zysz='922' then round(" + strMjFieldName + ",2) else 0 end) as ���," +
                                "sum(case when zysz='923' then round(" + strMjFieldName + ",2) else 0 end) as ���," +
                                "sum(case when zysz='924' then round(" + strMjFieldName + ",2) else 0 end) as �����," +
                                "sum(case when zysz='925' then round(" + strMjFieldName + ",2) else 0 end) as ������," +
                                "sum(case when zysz='926' then round(" + strMjFieldName + ",2) else 0 end) as �̶�," +
                                "sum(case when zysz='927' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when zysz='928' then round(" + strMjFieldName + ",2) else 0 end) as ��������," +
                                "sum(case when zysz='929' then round(" + strMjFieldName + ",2) else 0 end) as �����," +
                                "sum(case when zysz='930' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when zysz='931' then round(" + strMjFieldName + ",2) else 0 end) as ��ì," +
                                "sum(case when zysz='932' then round(" + strMjFieldName + ",2) else 0 end) as С��," +
                                "sum(case when zysz='933' then round(" + strMjFieldName + ",2) else 0 end) as �ؽ�," +
                                "sum(case when zysz='934' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when zysz='935' then round(" + strMjFieldName + ",2) else 0 end) as ��˷�黨," +
                                "sum(case when zysz='936' then round(" + strMjFieldName + ",2) else 0 end) as ������," +
                                "sum(case when zysz='937' then round(" + strMjFieldName + ",2) else 0 end) as ����ľ," +
                                "sum(case when zysz='938' then round(" + strMjFieldName + ",2) else 0 end) as ��ɽ��," +
                                "sum(case when zysz='939' then round(" + strMjFieldName + ",2) else 0 end) as ������," +
                                "sum(case when zysz='940' then round(" + strMjFieldName + ",2) else 0 end) as ������," +
                                "sum(case when zysz='999' then round(" + strMjFieldName + ",2) else 0 end) as ������ľ����," +
                                "sum(case when zysz='702' then round(" + strMjFieldName + ",2) else 0 end) as ƻ��," +
                                "sum(case when zysz='703' then round(" + strMjFieldName + ",2) else 0 end) as ��," +
                                "sum(case when zysz='704' then round(" + strMjFieldName + ",2) else 0 end) as ��," +
                                "sum(case when zysz='705' then round(" + strMjFieldName + ",2) else 0 end) as ��," +
                                "sum(case when zysz='706' then round(" + strMjFieldName + ",2) else 0 end) as ��," +
                                "sum(case when zysz='707' then round(" + strMjFieldName + ",2) else 0 end) as ��," +
                                "sum(case when zysz='708' then round(" + strMjFieldName + ",2) else 0 end) as ���," +
                                "sum(case when zysz='709' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when zysz='710' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when zysz='711' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when zysz='720' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when zysz='722' then round(" + strMjFieldName + ",2) else 0 end) as ����������," +
                                "sum(case when zysz='749' then round(" + strMjFieldName + ",2) else 0 end) as ������������," +
                                "sum(case when zysz='758' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when zysz='799' then round(" + strMjFieldName + ",2) else 0 end) as �����," +
                                "sum(case when zysz='851' then round(" + strMjFieldName + ",2) else 0 end) as ɣ��," +
                                "sum(case when zysz='120' then round(" + strMjFieldName + ",2) else 0 end) as ��ɼ," +
                                "sum(case when zysz='150' then round(" + strMjFieldName + ",2) else 0 end) as ��Ҷ��," +
                                "sum(case when zysz='170' then round(" + strMjFieldName + ",2) else 0 end) as ������," +
                                "sum(case when zysz='200' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when zysz='210' then round(" + strMjFieldName + ",2) else 0 end) as ��ɽ��," +
                                "sum(case when zysz='350' then round(" + strMjFieldName + ",2) else 0 end) as ���," +
                                "sum(case when zysz='411' then round(" + strMjFieldName + ",2) else 0 end) as ˨Ƥ��," +
                                "sum(case when zysz='412' then round(" + strMjFieldName + ",2) else 0 end) as �ɶ���," +
                                "sum(case when zysz='413' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when zysz='414' then round(" + strMjFieldName + ",2) else 0 end) as �����," +
                                "sum(case when zysz='420' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when zysz='421' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when zysz='490' then round(" + strMjFieldName + ",2) else 0 end) as Ӳ����," +
                                "sum(case when zysz='491' then round(" + strMjFieldName + ",2) else 0 end) as �̻�," +
                                "sum(case when zysz='492' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when zysz='493' then round(" + strMjFieldName + ",2) else 0 end) as �^��ľ," +
                                "sum(case when zysz='494' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when zysz='530' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when zysz='531' then round(" + strMjFieldName + ",2) else 0 end) as ��������," +
                                "sum(case when zysz='535' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when zysz='540' then round(" + strMjFieldName + ",2) else 0 end) as ��ͩ," +
                                "sum(case when zysz='545' then round(" + strMjFieldName + ",2) else 0 end) as ����������ľ," +
                                "sum(case when zysz='590' then round(" + strMjFieldName + ",2) else 0 end) as ������," +
                                "sum(case when zysz='591' then round(" + strMjFieldName + ",2) else 0 end) as ɽ��," +
                                "sum(case when zysz='670' then round(" + strMjFieldName + ",2) else 0 end) as ������," +
                                "sum(case when zysz='130' then round(" + strMjFieldName + ",2) else 0 end) as ����ɼ," +
                                "sum(case when zysz='360' then round(" + strMjFieldName + ",2) else 0 end) as ���," +
                                "sum(case when zysz='380' then round(" + strMjFieldName + ",2) else 0 end) as �춹ɼ," +
                                "sum(case when zysz='753' then round(" + strMjFieldName + ",2) else 0 end) as �Ĺڹ�," +
                                "sum(case when zysz='271' then round(" + strMjFieldName + ",2) else 0 end) as ��Ƥ��" +
                                "  from " + tableName +
                                "  group by  substr(shi,1,4))";
                pWorkspace.ExecuteSQL(shiSQL);
                string xianSQL = "insert into  DATA_SZXX ( select substr(xian,1,6) as ͳ�Ƶ�λ," +
                                "sum(case when zysz<>' ' then round(" + strMjFieldName + ",2) else 0 end) as �ϼ�," +
                                "sum(case when zysz='905' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when zysz='906' then round(" + strMjFieldName + ",2) else 0 end) as Ů��," +
                                "sum(case when zysz='907' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when zysz='908' then round(" + strMjFieldName + ",2) else 0 end) as �޻���," +
                                "sum(case when zysz='909' then round(" + strMjFieldName + ",2) else 0 end) as ��ľ������," +
                                "sum(case when zysz='910' then round(" + strMjFieldName + ",2) else 0 end) as ɳ��," +
                                "sum(case when zysz='911' then round(" + strMjFieldName + ",2) else 0 end) as ɽ��," +
                                "sum(case when zysz='912' then round(" + strMjFieldName + ",2) else 0 end) as ɽ��," +
                                "sum(case when zysz='913' then round(" + strMjFieldName + ",2) else 0 end) as ���߾�," +
                                "sum(case when zysz='914' then round(" + strMjFieldName + ",2) else 0 end) as ����ľ," +
                                "sum(case when zysz='915' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when zysz='916' then round(" + strMjFieldName + ",2) else 0 end) as �ƴ�õ," +
                                "sum(case when zysz='917' then round(" + strMjFieldName + ",2) else 0 end) as ɽ���," +
                                "sum(case when zysz='918' then round(" + strMjFieldName + ",2) else 0 end) as ���뻱," +
                                "sum(case when zysz='919' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when zysz='920' then round(" + strMjFieldName + ",2) else 0 end) as ��֦��," +
                                "sum(case when zysz='921' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when zysz='922' then round(" + strMjFieldName + ",2) else 0 end) as ���," +
                                "sum(case when zysz='923' then round(" + strMjFieldName + ",2) else 0 end) as ���," +
                                "sum(case when zysz='924' then round(" + strMjFieldName + ",2) else 0 end) as �����," +
                                "sum(case when zysz='925' then round(" + strMjFieldName + ",2) else 0 end) as ������," +
                                "sum(case when zysz='926' then round(" + strMjFieldName + ",2) else 0 end) as �̶�," +
                                "sum(case when zysz='927' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when zysz='928' then round(" + strMjFieldName + ",2) else 0 end) as ��������," +
                                "sum(case when zysz='929' then round(" + strMjFieldName + ",2) else 0 end) as �����," +
                                "sum(case when zysz='930' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when zysz='931' then round(" + strMjFieldName + ",2) else 0 end) as ��ì," +
                                "sum(case when zysz='932' then round(" + strMjFieldName + ",2) else 0 end) as С��," +
                                "sum(case when zysz='933' then round(" + strMjFieldName + ",2) else 0 end) as �ؽ�," +
                                "sum(case when zysz='934' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when zysz='935' then round(" + strMjFieldName + ",2) else 0 end) as ��˷�黨," +
                                "sum(case when zysz='936' then round(" + strMjFieldName + ",2) else 0 end) as ������," +
                                "sum(case when zysz='937' then round(" + strMjFieldName + ",2) else 0 end) as ����ľ," +
                                "sum(case when zysz='938' then round(" + strMjFieldName + ",2) else 0 end) as ��ɽ��," +
                                "sum(case when zysz='939' then round(" + strMjFieldName + ",2) else 0 end) as ������," +
                                "sum(case when zysz='940' then round(" + strMjFieldName + ",2) else 0 end) as ������," +
                                "sum(case when zysz='999' then round(" + strMjFieldName + ",2) else 0 end) as ������ľ����," +
                                "sum(case when zysz='702' then round(" + strMjFieldName + ",2) else 0 end) as ƻ��," +
                                "sum(case when zysz='703' then round(" + strMjFieldName + ",2) else 0 end) as ��," +
                                "sum(case when zysz='704' then round(" + strMjFieldName + ",2) else 0 end) as ��," +
                                "sum(case when zysz='705' then round(" + strMjFieldName + ",2) else 0 end) as ��," +
                                "sum(case when zysz='706' then round(" + strMjFieldName + ",2) else 0 end) as ��," +
                                "sum(case when zysz='707' then round(" + strMjFieldName + ",2) else 0 end) as ��," +
                                "sum(case when zysz='708' then round(" + strMjFieldName + ",2) else 0 end) as ���," +
                                "sum(case when zysz='709' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when zysz='710' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when zysz='711' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when zysz='720' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when zysz='722' then round(" + strMjFieldName + ",2) else 0 end) as ����������," +
                                "sum(case when zysz='749' then round(" + strMjFieldName + ",2) else 0 end) as ������������," +
                                "sum(case when zysz='758' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when zysz='799' then round(" + strMjFieldName + ",2) else 0 end) as �����," +
                                "sum(case when zysz='851' then round(" + strMjFieldName + ",2) else 0 end) as ɣ��," +
                                "sum(case when zysz='120' then round(" + strMjFieldName + ",2) else 0 end) as ��ɼ," +
                                "sum(case when zysz='150' then round(" + strMjFieldName + ",2) else 0 end) as ��Ҷ��," +
                                "sum(case when zysz='170' then round(" + strMjFieldName + ",2) else 0 end) as ������," +
                                "sum(case when zysz='200' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when zysz='210' then round(" + strMjFieldName + ",2) else 0 end) as ��ɽ��," +
                                "sum(case when zysz='350' then round(" + strMjFieldName + ",2) else 0 end) as ���," +
                                "sum(case when zysz='411' then round(" + strMjFieldName + ",2) else 0 end) as ˨Ƥ��," +
                                "sum(case when zysz='412' then round(" + strMjFieldName + ",2) else 0 end) as �ɶ���," +
                                "sum(case when zysz='413' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when zysz='414' then round(" + strMjFieldName + ",2) else 0 end) as �����," +
                                "sum(case when zysz='420' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when zysz='421' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when zysz='490' then round(" + strMjFieldName + ",2) else 0 end) as Ӳ����," +
                                "sum(case when zysz='491' then round(" + strMjFieldName + ",2) else 0 end) as �̻�," +
                                "sum(case when zysz='492' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when zysz='493' then round(" + strMjFieldName + ",2) else 0 end) as �^��ľ," +
                                "sum(case when zysz='494' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when zysz='530' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when zysz='531' then round(" + strMjFieldName + ",2) else 0 end) as ��������," +
                                "sum(case when zysz='535' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when zysz='540' then round(" + strMjFieldName + ",2) else 0 end) as ��ͩ," +
                                "sum(case when zysz='545' then round(" + strMjFieldName + ",2) else 0 end) as ����������ľ," +
                                "sum(case when zysz='590' then round(" + strMjFieldName + ",2) else 0 end) as ������," +
                                "sum(case when zysz='591' then round(" + strMjFieldName + ",2) else 0 end) as ɽ��," +
                                "sum(case when zysz='670' then round(" + strMjFieldName + ",2) else 0 end) as ������," +
                                "sum(case when zysz='130' then round(" + strMjFieldName + ",2) else 0 end) as ����ɼ," +
                                "sum(case when zysz='360' then round(" + strMjFieldName + ",2) else 0 end) as ���," +
                                "sum(case when zysz='380' then round(" + strMjFieldName + ",2) else 0 end) as �춹ɼ," +
                                "sum(case when zysz='753' then round(" + strMjFieldName + ",2) else 0 end) as �Ĺڹ�," +
                                "sum(case when zysz='271' then round(" + strMjFieldName + ",2) else 0 end) as ��Ƥ��" +
                                "  from " + tableName +
                                "  group by  substr(xian,1,6))";
                pWorkspace.ExecuteSQL(xianSQL);
                string xiangSQL = "insert into  DATA_SZXX ( select substr(xiang,1,8) as ͳ�Ƶ�λ," +
                                "sum(case when zysz<>' ' then round(" + strMjFieldName + ",2) else 0 end) as �ϼ�," +
                                "sum(case when zysz='905' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when zysz='906' then round(" + strMjFieldName + ",2) else 0 end) as Ů��," +
                                "sum(case when zysz='907' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when zysz='908' then round(" + strMjFieldName + ",2) else 0 end) as �޻���," +
                                "sum(case when zysz='909' then round(" + strMjFieldName + ",2) else 0 end) as ��ľ������," +
                                "sum(case when zysz='910' then round(" + strMjFieldName + ",2) else 0 end) as ɳ��," +
                                "sum(case when zysz='911' then round(" + strMjFieldName + ",2) else 0 end) as ɽ��," +
                                "sum(case when zysz='912' then round(" + strMjFieldName + ",2) else 0 end) as ɽ��," +
                                "sum(case when zysz='913' then round(" + strMjFieldName + ",2) else 0 end) as ���߾�," +
                                "sum(case when zysz='914' then round(" + strMjFieldName + ",2) else 0 end) as ����ľ," +
                                "sum(case when zysz='915' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when zysz='916' then round(" + strMjFieldName + ",2) else 0 end) as �ƴ�õ," +
                                "sum(case when zysz='917' then round(" + strMjFieldName + ",2) else 0 end) as ɽ���," +
                                "sum(case when zysz='918' then round(" + strMjFieldName + ",2) else 0 end) as ���뻱," +
                                "sum(case when zysz='919' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when zysz='920' then round(" + strMjFieldName + ",2) else 0 end) as ��֦��," +
                                "sum(case when zysz='921' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when zysz='922' then round(" + strMjFieldName + ",2) else 0 end) as ���," +
                                "sum(case when zysz='923' then round(" + strMjFieldName + ",2) else 0 end) as ���," +
                                "sum(case when zysz='924' then round(" + strMjFieldName + ",2) else 0 end) as �����," +
                                "sum(case when zysz='925' then round(" + strMjFieldName + ",2) else 0 end) as ������," +
                                "sum(case when zysz='926' then round(" + strMjFieldName + ",2) else 0 end) as �̶�," +
                                "sum(case when zysz='927' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when zysz='928' then round(" + strMjFieldName + ",2) else 0 end) as ��������," +
                                "sum(case when zysz='929' then round(" + strMjFieldName + ",2) else 0 end) as �����," +
                                "sum(case when zysz='930' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when zysz='931' then round(" + strMjFieldName + ",2) else 0 end) as ��ì," +
                                "sum(case when zysz='932' then round(" + strMjFieldName + ",2) else 0 end) as С��," +
                                "sum(case when zysz='933' then round(" + strMjFieldName + ",2) else 0 end) as �ؽ�," +
                                "sum(case when zysz='934' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when zysz='935' then round(" + strMjFieldName + ",2) else 0 end) as ��˷�黨," +
                                "sum(case when zysz='936' then round(" + strMjFieldName + ",2) else 0 end) as ������," +
                                "sum(case when zysz='937' then round(" + strMjFieldName + ",2) else 0 end) as ����ľ," +
                                "sum(case when zysz='938' then round(" + strMjFieldName + ",2) else 0 end) as ��ɽ��," +
                                "sum(case when zysz='939' then round(" + strMjFieldName + ",2) else 0 end) as ������," +
                                "sum(case when zysz='940' then round(" + strMjFieldName + ",2) else 0 end) as ������," +
                                "sum(case when zysz='999' then round(" + strMjFieldName + ",2) else 0 end) as ������ľ����," +
                                "sum(case when zysz='702' then round(" + strMjFieldName + ",2) else 0 end) as ƻ��," +
                                "sum(case when zysz='703' then round(" + strMjFieldName + ",2) else 0 end) as ��," +
                                "sum(case when zysz='704' then round(" + strMjFieldName + ",2) else 0 end) as ��," +
                                "sum(case when zysz='705' then round(" + strMjFieldName + ",2) else 0 end) as ��," +
                                "sum(case when zysz='706' then round(" + strMjFieldName + ",2) else 0 end) as ��," +
                                "sum(case when zysz='707' then round(" + strMjFieldName + ",2) else 0 end) as ��," +
                                "sum(case when zysz='708' then round(" + strMjFieldName + ",2) else 0 end) as ���," +
                                "sum(case when zysz='709' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when zysz='710' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when zysz='711' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when zysz='720' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when zysz='722' then round(" + strMjFieldName + ",2) else 0 end) as ����������," +
                                "sum(case when zysz='749' then round(" + strMjFieldName + ",2) else 0 end) as ������������," +
                                "sum(case when zysz='758' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when zysz='799' then round(" + strMjFieldName + ",2) else 0 end) as �����," +
                                "sum(case when zysz='851' then round(" + strMjFieldName + ",2) else 0 end) as ɣ��," +
                                "sum(case when zysz='120' then round(" + strMjFieldName + ",2) else 0 end) as ��ɼ," +
                                "sum(case when zysz='150' then round(" + strMjFieldName + ",2) else 0 end) as ��Ҷ��," +
                                "sum(case when zysz='170' then round(" + strMjFieldName + ",2) else 0 end) as ������," +
                                "sum(case when zysz='200' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when zysz='210' then round(" + strMjFieldName + ",2) else 0 end) as ��ɽ��," +
                                "sum(case when zysz='350' then round(" + strMjFieldName + ",2) else 0 end) as ���," +
                                "sum(case when zysz='411' then round(" + strMjFieldName + ",2) else 0 end) as ˨Ƥ��," +
                                "sum(case when zysz='412' then round(" + strMjFieldName + ",2) else 0 end) as �ɶ���," +
                                "sum(case when zysz='413' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when zysz='414' then round(" + strMjFieldName + ",2) else 0 end) as �����," +
                                "sum(case when zysz='420' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when zysz='421' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when zysz='490' then round(" + strMjFieldName + ",2) else 0 end) as Ӳ����," +
                                "sum(case when zysz='491' then round(" + strMjFieldName + ",2) else 0 end) as �̻�," +
                                "sum(case when zysz='492' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when zysz='493' then round(" + strMjFieldName + ",2) else 0 end) as �^��ľ," +
                                "sum(case when zysz='494' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when zysz='530' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when zysz='531' then round(" + strMjFieldName + ",2) else 0 end) as ��������," +
                                "sum(case when zysz='535' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when zysz='540' then round(" + strMjFieldName + ",2) else 0 end) as ��ͩ," +
                                "sum(case when zysz='545' then round(" + strMjFieldName + ",2) else 0 end) as ����������ľ," +
                                "sum(case when zysz='590' then round(" + strMjFieldName + ",2) else 0 end) as ������," +
                                "sum(case when zysz='591' then round(" + strMjFieldName + ",2) else 0 end) as ɽ��," +
                                "sum(case when zysz='670' then round(" + strMjFieldName + ",2) else 0 end) as ������," +
                                "sum(case when zysz='130' then round(" + strMjFieldName + ",2) else 0 end) as ����ɼ," +
                                "sum(case when zysz='360' then round(" + strMjFieldName + ",2) else 0 end) as ���," +
                                "sum(case when zysz='380' then round(" + strMjFieldName + ",2) else 0 end) as �춹ɼ," +
                                "sum(case when zysz='753' then round(" + strMjFieldName + ",2) else 0 end) as �Ĺڹ�," +
                                "sum(case when zysz='271' then round(" + strMjFieldName + ",2) else 0 end) as ��Ƥ��" +
                                "  from " + tableName +
                                "  group by  substr(xiang,1,8))";
                pWorkspace.ExecuteSQL(xiangSQL);
                string lcSQL = "insert into  DATA_SZXX ( select lc as ͳ�Ƶ�λ," +
                                "sum(case when zysz<>' ' then round(" + strMjFieldName + ",2) else 0 end) as �ϼ�," +
                                "sum(case when zysz='905' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when zysz='906' then round(" + strMjFieldName + ",2) else 0 end) as Ů��," +
                                "sum(case when zysz='907' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when zysz='908' then round(" + strMjFieldName + ",2) else 0 end) as �޻���," +
                                "sum(case when zysz='909' then round(" + strMjFieldName + ",2) else 0 end) as ��ľ������," +
                                "sum(case when zysz='910' then round(" + strMjFieldName + ",2) else 0 end) as ɳ��," +
                                "sum(case when zysz='911' then round(" + strMjFieldName + ",2) else 0 end) as ɽ��," +
                                "sum(case when zysz='912' then round(" + strMjFieldName + ",2) else 0 end) as ɽ��," +
                                "sum(case when zysz='913' then round(" + strMjFieldName + ",2) else 0 end) as ���߾�," +
                                "sum(case when zysz='914' then round(" + strMjFieldName + ",2) else 0 end) as ����ľ," +
                                "sum(case when zysz='915' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when zysz='916' then round(" + strMjFieldName + ",2) else 0 end) as �ƴ�õ," +
                                "sum(case when zysz='917' then round(" + strMjFieldName + ",2) else 0 end) as ɽ���," +
                                "sum(case when zysz='918' then round(" + strMjFieldName + ",2) else 0 end) as ���뻱," +
                                "sum(case when zysz='919' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when zysz='920' then round(" + strMjFieldName + ",2) else 0 end) as ��֦��," +
                                "sum(case when zysz='921' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when zysz='922' then round(" + strMjFieldName + ",2) else 0 end) as ���," +
                                "sum(case when zysz='923' then round(" + strMjFieldName + ",2) else 0 end) as ���," +
                                "sum(case when zysz='924' then round(" + strMjFieldName + ",2) else 0 end) as �����," +
                                "sum(case when zysz='925' then round(" + strMjFieldName + ",2) else 0 end) as ������," +
                                "sum(case when zysz='926' then round(" + strMjFieldName + ",2) else 0 end) as �̶�," +
                                "sum(case when zysz='927' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when zysz='928' then round(" + strMjFieldName + ",2) else 0 end) as ��������," +
                                "sum(case when zysz='929' then round(" + strMjFieldName + ",2) else 0 end) as �����," +
                                "sum(case when zysz='930' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when zysz='931' then round(" + strMjFieldName + ",2) else 0 end) as ��ì," +
                                "sum(case when zysz='932' then round(" + strMjFieldName + ",2) else 0 end) as С��," +
                                "sum(case when zysz='933' then round(" + strMjFieldName + ",2) else 0 end) as �ؽ�," +
                                "sum(case when zysz='934' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when zysz='935' then round(" + strMjFieldName + ",2) else 0 end) as ��˷�黨," +
                                "sum(case when zysz='936' then round(" + strMjFieldName + ",2) else 0 end) as ������," +
                                "sum(case when zysz='937' then round(" + strMjFieldName + ",2) else 0 end) as ����ľ," +
                                "sum(case when zysz='938' then round(" + strMjFieldName + ",2) else 0 end) as ��ɽ��," +
                                "sum(case when zysz='939' then round(" + strMjFieldName + ",2) else 0 end) as ������," +
                                "sum(case when zysz='940' then round(" + strMjFieldName + ",2) else 0 end) as ������," +
                                "sum(case when zysz='999' then round(" + strMjFieldName + ",2) else 0 end) as ������ľ����," +
                                "sum(case when zysz='702' then round(" + strMjFieldName + ",2) else 0 end) as ƻ��," +
                                "sum(case when zysz='703' then round(" + strMjFieldName + ",2) else 0 end) as ��," +
                                "sum(case when zysz='704' then round(" + strMjFieldName + ",2) else 0 end) as ��," +
                                "sum(case when zysz='705' then round(" + strMjFieldName + ",2) else 0 end) as ��," +
                                "sum(case when zysz='706' then round(" + strMjFieldName + ",2) else 0 end) as ��," +
                                "sum(case when zysz='707' then round(" + strMjFieldName + ",2) else 0 end) as ��," +
                                "sum(case when zysz='708' then round(" + strMjFieldName + ",2) else 0 end) as ���," +
                                "sum(case when zysz='709' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when zysz='710' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when zysz='711' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when zysz='720' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when zysz='722' then round(" + strMjFieldName + ",2) else 0 end) as ����������," +
                                "sum(case when zysz='749' then round(" + strMjFieldName + ",2) else 0 end) as ������������," +
                                "sum(case when zysz='758' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when zysz='799' then round(" + strMjFieldName + ",2) else 0 end) as �����," +
                                "sum(case when zysz='851' then round(" + strMjFieldName + ",2) else 0 end) as ɣ��," +
                                "sum(case when zysz='120' then round(" + strMjFieldName + ",2) else 0 end) as ��ɼ," +
                                "sum(case when zysz='150' then round(" + strMjFieldName + ",2) else 0 end) as ��Ҷ��," +
                                "sum(case when zysz='170' then round(" + strMjFieldName + ",2) else 0 end) as ������," +
                                "sum(case when zysz='200' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when zysz='210' then round(" + strMjFieldName + ",2) else 0 end) as ��ɽ��," +
                                "sum(case when zysz='350' then round(" + strMjFieldName + ",2) else 0 end) as ���," +
                                "sum(case when zysz='411' then round(" + strMjFieldName + ",2) else 0 end) as ˨Ƥ��," +
                                "sum(case when zysz='412' then round(" + strMjFieldName + ",2) else 0 end) as �ɶ���," +
                                "sum(case when zysz='413' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when zysz='414' then round(" + strMjFieldName + ",2) else 0 end) as �����," +
                                "sum(case when zysz='420' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when zysz='421' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when zysz='490' then round(" + strMjFieldName + ",2) else 0 end) as Ӳ����," +
                                "sum(case when zysz='491' then round(" + strMjFieldName + ",2) else 0 end) as �̻�," +
                                "sum(case when zysz='492' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when zysz='493' then round(" + strMjFieldName + ",2) else 0 end) as �^��ľ," +
                                "sum(case when zysz='494' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when zysz='530' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when zysz='531' then round(" + strMjFieldName + ",2) else 0 end) as ��������," +
                                "sum(case when zysz='535' then round(" + strMjFieldName + ",2) else 0 end) as ����," +
                                "sum(case when zysz='540' then round(" + strMjFieldName + ",2) else 0 end) as ��ͩ," +
                                "sum(case when zysz='545' then round(" + strMjFieldName + ",2) else 0 end) as ����������ľ," +
                                "sum(case when zysz='590' then round(" + strMjFieldName + ",2) else 0 end) as ������," +
                                "sum(case when zysz='591' then round(" + strMjFieldName + ",2) else 0 end) as ɽ��," +
                                "sum(case when zysz='670' then round(" + strMjFieldName + ",2) else 0 end) as ������," +
                                "sum(case when zysz='130' then round(" + strMjFieldName + ",2) else 0 end) as ����ɼ," +
                                "sum(case when zysz='360' then round(" + strMjFieldName + ",2) else 0 end) as ���," +
                                "sum(case when zysz='380' then round(" + strMjFieldName + ",2) else 0 end) as �춹ɼ," +
                                "sum(case when zysz='753' then round(" + strMjFieldName + ",2) else 0 end) as �Ĺڹ�," +
                                "sum(case when zysz='271' then round(" + strMjFieldName + ",2) else 0 end) as ��Ƥ��" +
                                "  from " + tableName +
                                "  group by  lc)";
                pWorkspace.ExecuteSQL(lcSQL);
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }
        
    }
}
