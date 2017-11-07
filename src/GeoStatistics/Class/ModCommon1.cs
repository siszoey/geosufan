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

        public static void DoEightStatistics(IFeatureClass pFeatureClass,string strMjFieldName)
        {
            SysCommon.CProgress vProgress = new SysCommon.CProgress();       //��ӽ����� ygc 2012-10-24
            vProgress.ShowDescription = true;
            vProgress.ShowProgressNumber = true;
            vProgress.FakeProgress = true;
            vProgress.TopMost = true;
            vProgress.ShowProgress();
            try
            {
                vProgress.SetProgress("����ͳ���ֵ������ȼ�����......");
                //�ֵ������ȼ�
                DoLDZL_Statistic(pFeatureClass, strMjFieldName);
                vProgress.SetProgress("����ͳ���ֵ����÷���滮�������......");
                //�ֵ����÷���滮���
                DoLDLYFXGHMJ_Statistic(pFeatureClass, strMjFieldName);
                vProgress.SetProgress("����ͳ���ֵؽṹ��״����......");
                //�ֵؽṹ��״
                DoLDJGXZ_Statistic(pFeatureClass, strMjFieldName);
                vProgress.SetProgress("����ͳ���ֵؼ�ɭ������滮����......");
                //�ֵؼ�ɭ������滮
                DoLDSLMJGH_Statistic(pFeatureClass, strMjFieldName);
                //�ֵ���״
                vProgress.SetProgress("����ͳ�ƹ��Ҽ������ֵطֱ����ȼ���״����......");
                DoLDXZ_Statistic(pFeatureClass, strMjFieldName);
                //���Ҽ������ֵطֱ����ȼ���״
                DoGJGYHDJ_Statistic(pFeatureClass, strMjFieldName);
                vProgress.SetProgress("����ͳ���ֵر����ȼ�����......");
                //�ֵر����ȼ�
                DoLDBHDJ_Statistic(pFeatureClass, strMjFieldName);
                vProgress.SetProgress("����ͳ�ƹ��Ҽ������ֵع滮�������......");
                //���Ҽ������ֵع滮���
                DoGJGYLGHMJ_Statistic(pFeatureClass, strMjFieldName);
                vProgress.SetProgress("����ͳ���ֳ�����......");
                //�ֳ�����ͳ�Ʊ����� ygc 2012-10-10
                DoLCLDZL_Statistic(pFeatureClass, strMjFieldName);
                DoLCLDLYFXGHMJ_Statistic(pFeatureClass, strMjFieldName);
                DoLCLDJGXZ_Statistic(pFeatureClass, strMjFieldName);
                DoLCLDSLMJGH_Statistic(pFeatureClass, strMjFieldName);
                DoLCLDXZ_Statistic(pFeatureClass, strMjFieldName);
                DoLCGJGYHDJ_Statistic(pFeatureClass, strMjFieldName);
                DoLCLDBHDJ_Statistic(pFeatureClass, strMjFieldName);
                DoLCGJGYLGHMJ_Statistic(pFeatureClass, strMjFieldName);
            }
            catch
            { }
            finally
            {
                vProgress.Close();
            }
 
        }
        //ɾ����ʱ��
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
                //��ѯ�ؼ�ͳ������
                //ygc 2012-8-21
                string stringSQL = "create table EightTable_LDZL as select " + tableName + ".xian as ͳ�Ƶ�λ,sum(case when Zl_DJ between '1' and '5' then " + tableName + "." + strMjFieldName + " end) as �ϼ� ," +
                                    "sum(case when " + tableName + ".Zl_DJ='1' then " + tableName + "." + strMjFieldName + " end) as ��," +
                                     "sum(case when " + tableName + ".Zl_DJ='2' then " + tableName + "." + strMjFieldName + " end) as ��," +
                                     "sum(case when " + tableName + ".Zl_DJ='3' then " + tableName + "." + strMjFieldName + " end) as ��," +
                                     "sum(case when " + tableName + ".Zl_DJ='4' then " + tableName + "." + strMjFieldName + " end) as ����," +
                                     "sum(case when " + tableName + ".Zl_DJ='5' then " + tableName + "." + strMjFieldName + " end) as ���� " +
                                      "  from " + tableName +
                                      "  group by xian";
                pWorkspace.ExecuteSQL(stringSQL);
                //��ѯ����ͳ������
                string townsSQL = "insert into EightTable_LDZL select  " + tableName + ".xiang as ͳ�Ƶ�λ,sum(case when Zl_DJ between '1' and '5' then " + tableName + "." + strMjFieldName + " end) as �ϼ� ," +
                                  "sum(case when " + tableName + ".Zl_DJ='1' then " + tableName + "." + strMjFieldName + "  end) as ��," +
                                  "sum(case when " + tableName + ".Zl_DJ='2' then " + tableName + "." + strMjFieldName + "  end) as ��," +
                                  "sum(case when " + tableName + ".Zl_DJ='3' then " + tableName + "." + strMjFieldName + "  end) as ��," +
                                  "sum(case when " + tableName + ".Zl_DJ='4' then " + tableName + "." + strMjFieldName + "  end) as ����," +
                                  "sum(case when " + tableName + ".Zl_DJ='5' then " + tableName + "." + strMjFieldName + "  end) as ���� " +
                                  "  from " + tableName +
                                  "  group by xiang";
                pWorkspace.ExecuteSQL(townsSQL);
                //��ѯ�м����� ygc 2012-10-22
                string SHISQL = "insert into EightTable_LDZL select  substr(" + tableName + ".shi,1,4) as ͳ�Ƶ�λ,sum(case when Zl_DJ between '1' and '5' then " + tableName + "." + strMjFieldName + " end) as �ϼ� ," +
                  "sum(case when " + tableName + ".Zl_DJ='1' then " + tableName + "." + strMjFieldName + "  end) as ��," +
                  "sum(case when " + tableName + ".Zl_DJ='2' then " + tableName + "." + strMjFieldName + "  end) as ��," +
                  "sum(case when " + tableName + ".Zl_DJ='3' then " + tableName + "." + strMjFieldName + "  end) as ��," +
                  "sum(case when " + tableName + ".Zl_DJ='4' then " + tableName + "." + strMjFieldName + "  end) as ����," +
                  "sum(case when " + tableName + ".Zl_DJ='5' then " + tableName + "." + strMjFieldName + "  end) as ���� " +
                  "  from " + tableName +
                  "  group by substr(shi,1,4)";
                pWorkspace.ExecuteSQL(SHISQL);
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
                    string stringSQL = "create table EightTable_LDZL as select " + tableName + ".lc as ͳ�Ƶ�λ,sum(case when Zl_DJ between '1' and '5' then " + tableName + "." + strMjFieldName + " end) as �ϼ� ," +
                                       "sum(case when " + tableName + ".Zl_DJ='1' then " + tableName + "." + strMjFieldName + " end) as ��," +
                                        "sum(case when " + tableName + ".Zl_DJ='2' then " + tableName + "." + strMjFieldName + " end) as ��," +
                                        "sum(case when " + tableName + ".Zl_DJ='3' then " + tableName + "." + strMjFieldName + " end) as ��," +
                                        "sum(case when " + tableName + ".Zl_DJ='4' then " + tableName + "." + strMjFieldName + " end) as ����," +
                                        "sum(case when " + tableName + ".Zl_DJ='5' then " + tableName + "." + strMjFieldName + " end) as ���� " +
                                         "  from " + tableName +
                                         "  where lc <>' '"+
                                         "  group by lc";
                    pWorkspace.ExecuteSQL(stringSQL);
                }
                //��ѯ�ֳ�ͳ������
                string townsSQL = "insert into EightTable_LDZL select  " + tableName + ".lc as ͳ�Ƶ�λ,sum(case when Zl_DJ between '1' and '5' then " + tableName + "." + strMjFieldName + " end) as �ϼ� ," +
                                  "sum(case when " + tableName + ".Zl_DJ='1' then " + tableName + "." + strMjFieldName + "  end) as ��," +
                                  "sum(case when " + tableName + ".Zl_DJ='2' then " + tableName + "." + strMjFieldName + "  end) as ��," +
                                  "sum(case when " + tableName + ".Zl_DJ='3' then " + tableName + "." + strMjFieldName + "  end) as ��," +
                                  "sum(case when " + tableName + ".Zl_DJ='4' then " + tableName + "." + strMjFieldName + "  end) as ����," +
                                  "sum(case when " + tableName + ".Zl_DJ='5' then " + tableName + "." + strMjFieldName + "  end) as ���� " +
                                  "  from " + tableName +
                                  "  where lc <> ' '"+
                                  "  group by lc";
                pWorkspace.ExecuteSQL(townsSQL);
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
                //��ѯ�ؼ�ͳ������SQL���
                //ygc 2012-8-22
                string CitySQL = "create table EightTable_LDLYFXGHMJ as select " +
                                 tableName + ".xian as ͳ�Ƶ�λ," +
                                 "sum (case when " + tableName + ".sen_lin_lb in('11','12','21','22') and " + tableName + ".dl <= '114'then " + tableName + "." + StatisticsFieldName + " else 0 end) as �ϼ�," +
                                 "sum(case when " + tableName + ".sen_lin_lb between '11' and '12'and " + tableName + ".dl<='114' then " + tableName + "." + StatisticsFieldName + " else 0 end) as �����ֺϼ�," +
                                 "sum (case when " + tableName + ".sen_lin_lb ='11' and " + tableName + ".dl<='114' then " + tableName + "." + StatisticsFieldName + "  else 0 end) as �ص㹫���ֺϼ�," +
                                 "sum(case when " + tableName + ".sen_lin_lb='11' and " + tableName + ".dl<='112' and (substr(" + tableName + ".lz,1,2)='11'or substr(" + tableName + ".lz,1,2)='12') then " + tableName + "." + StatisticsFieldName + " else 0 end ) as �ص㹫��С��," +
                                 "sum (case when " + tableName + ".sen_lin_lb='11' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='11' then " + tableName + "." + StatisticsFieldName + " else 0 end) as �ص㹫�������," +
                                 "sum(case when " + tableName + ".sen_lin_lb='11'and " + tableName + ".dl<='112' and substr(" + tableName + ".lz,1,2)='12' then " + tableName + "." + StatisticsFieldName + " else 0 end ) as �ص㹫��������," +
                                 "sum(case when " + tableName + ".sen_lin_lb='11'and (((substr(" + tableName + ".lz,1,2)='11'or substr(" + tableName + ".lz,1,2)='12') and " + tableName + ".dl between '113' and '114')) and (substr(" + tableName + ".lz,1,2)<>'11' or substr(" + tableName + ".lz,1,2)<>'12') then " + tableName + "." + StatisticsFieldName + "  else 0 end) as  �ص㹫������," +
                                 "sum(case when " + tableName + ".sen_lin_lb ='12' and " + tableName + ".dl<='114' then " + tableName + "." + StatisticsFieldName + "   end ) as һ�㹫��ϼ�," +
                                 "sum(case when " + tableName + ".sen_lin_lb='12' and " + tableName + ".dl<='112' and (substr(" + tableName + ".lz,1,2)='11'or substr(" + tableName + ".lz,1,2)='12') then " + tableName + "." + StatisticsFieldName + " else 0 end ) as һ�㹫��С��," +
                                 "sum(case when " + tableName + ".sen_lin_lb='12' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='11' then " + tableName + "." + StatisticsFieldName + " else 0 end) as һ�㹫�������," +
                                 "sum(case when " + tableName + ".sen_lin_lb='12'and " + tableName + ".dl<='112' and substr(" + tableName + ".lz,1,2)='12' then " + tableName + "." + StatisticsFieldName + " else 0 end ) as һ�㹫��������," +
                                 "sum(case when " + tableName + ".sen_lin_lb='12'and ((substr(" + tableName + ".lz,1,2)<>'11' and substr(" + tableName + ".lz,1,2)<>'12') or (((substr(" + tableName + ".lz,1,2)='11' or substr(" + tableName + ".lz,1,2)='12')) and " + tableName + ".dl between '113' and '114')) then " + tableName + "." + StatisticsFieldName + " else 0 end) as һ�㹫������," +
                                 "sum(case when " + tableName + ".sen_lin_lb between '21' and '22'and " + tableName + ".dl<='114' then " + tableName + "." + StatisticsFieldName + " else 0 end ) as ��Ʒ�ֺϼ�," +
                                 "sum(case when " + tableName + ".sen_lin_lb ='21' and " + tableName + ".dl<='114' then " + tableName + "." + StatisticsFieldName + " else 0 end) as �ص���Ʒ�ֺϼ�," +
                                 "sum(case when " + tableName + ".sen_lin_lb='21' and " + tableName + ".dl<='114' and (substr(" + tableName + ".lz,1,2) between '23' and '25') then " + tableName + "." + StatisticsFieldName + " else 0 end ) as �ص���Ʒ��С��," +
                                 "sum(case when " + tableName + ".sen_lin_lb='21' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='23' then " + tableName + "." + StatisticsFieldName + " else 0 end) as �ص��ò���," +
                                 "sum(case when " + tableName + ".sen_lin_lb='21' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='25'and " + tableName + ".dl='114' then " + tableName + "." + StatisticsFieldName + " else 0 end) as �ص㾭����," +
                                 "sum(case when " + tableName + ".sen_lin_lb='21' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='24' then " + tableName + "." + StatisticsFieldName + " else 0 end) as �ص�н̼��," +
                                 "sum(case when " + tableName + ".sen_lin_lb='21'and ((substr(" + tableName + ".lz,1,2)<>'23' and substr(" + tableName + ".lz,1,2)<>'24' and substr(" + tableName + ".lz,1,2)<>'25') or (substr(" + tableName + ".lz,1,2)='23' and " + tableName + ".dl>'112') or (substr(" + tableName + ".lz,1,2)='25' and " + tableName + ".dl<>'114')) then " + tableName + "." + StatisticsFieldName + " else 0 end) as �ص�����," +
                                 "sum(case when " + tableName + ".sen_lin_lb ='22' and " + tableName + ".dl<='114' then " + tableName + "." + StatisticsFieldName + "  else 0 end) as һ����Ʒ�ֺϼ�," +
                                 "sum(case when " + tableName + ".sen_lin_lb='22' and " + tableName + ".dl<='114' and (substr(" + tableName + ".lz,1,2) between '23' and '25') then " + tableName + "." + StatisticsFieldName + " else 0 end ) as һ����Ʒ��С��," +
                                 "sum(case when " + tableName + ".sen_lin_lb='22' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='23' then " + tableName + "." + StatisticsFieldName + " else 0 end) as һ���ò���," +
                                 "sum(case when " + tableName + ".sen_lin_lb='22' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='25'and " + tableName + ".dl='114' then " + tableName + "." + StatisticsFieldName + " else 0 end) as һ�㾭����," +
                                 "sum(case when " + tableName + ".sen_lin_lb='22' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='24' then " + tableName + "." + StatisticsFieldName + " else 0 end) as һ��н̼��," +
                                 "sum(case when " + tableName + ".sen_lin_lb='22'and ((substr(" + tableName + ".lz,1,2)<>'23' and substr(" + tableName + ".lz,1,2)<>'24' and substr(" + tableName + ".lz,1,2)<>'25') or (substr(" + tableName + ".lz,1,2)='23' and " + tableName + ".dl>'112') or (substr(" + tableName + ".lz,1,2)='25' and " + tableName + ".dl<>'114')) then " + tableName + "." + StatisticsFieldName + " else 0 end) as һ������" +
                                 " from " + tableName +
                                 " group by xian";
                pWorkspace.ExecuteSQL(CitySQL);
                //��ѯ����ͳ������
                //ygc 2012-8-22
                string townsSQL = "insert  into EightTable_LDLYFXGHMJ  select " +
                                 tableName + ".xiang as ͳ�Ƶ�λ," +
                                 "sum (case when " + tableName + ".sen_lin_lb in('11','12','21','22') and " + tableName + ".dl <= '114'then " + tableName + "." + StatisticsFieldName + " else 0 end) as �ϼ�," +
                                 "sum(case when " + tableName + ".sen_lin_lb between '11' and '12'and " + tableName + ".dl<='114' then " + tableName + "." + StatisticsFieldName + " else 0 end) as �����ֺϼ�," +
                                 "sum (case when " + tableName + ".sen_lin_lb ='11' and " + tableName + ".dl<='114' then " + tableName + "." + StatisticsFieldName + "  else 0 end) as �ص㹫���ֺϼ�," +
                                 "sum(case when " + tableName + ".sen_lin_lb='11' and " + tableName + ".dl<='112' and (substr(" + tableName + ".lz,1,2)='11'or substr(" + tableName + ".lz,1,2)='12') then " + tableName + "." + StatisticsFieldName + " else 0 end ) as �ص㹫��С��," +
                                 "sum (case when " + tableName + ".sen_lin_lb='11' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='11' then " + tableName + "." + StatisticsFieldName + " else 0 end) as �ص㹫�������," +
                                 "sum(case when " + tableName + ".sen_lin_lb='11'and " + tableName + ".dl<='112' and substr(" + tableName + ".lz,1,2)='12' then " + tableName + "." + StatisticsFieldName + " else 0 end ) as �ص㹫��������," +
                                 "sum(case when " + tableName + ".sen_lin_lb='11'and (((substr(" + tableName + ".lz,1,2)='11'or substr(" + tableName + ".lz,1,2)='12') and " + tableName + ".dl between '113' and '114')) and (substr(" + tableName + ".lz,1,2)<>'11' or substr(" + tableName + ".lz,1,2)<>'12') then " + tableName + "." + StatisticsFieldName + "  else 0 end) as  �ص㹫������," +
                                 "sum(case when " + tableName + ".sen_lin_lb ='12' and " + tableName + ".dl<='114' then " + tableName + "." + StatisticsFieldName + "   end ) as һ�㹫��ϼ�," +
                                 "sum(case when " + tableName + ".sen_lin_lb='12' and " + tableName + ".dl<='112' and (substr(" + tableName + ".lz,1,2)='11'or substr(" + tableName + ".lz,1,2)='12') then " + tableName + "." + StatisticsFieldName + " else 0 end ) as һ�㹫��С��," +
                                 "sum(case when " + tableName + ".sen_lin_lb='12' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='11' then " + tableName + "." + StatisticsFieldName + " else 0 end) as һ�㹫�������," +
                                 "sum(case when " + tableName + ".sen_lin_lb='12'and " + tableName + ".dl<='112' and substr(" + tableName + ".lz,1,2)='12' then " + tableName + "." + StatisticsFieldName + " else 0 end ) as һ�㹫��������," +
                                 "sum(case when " + tableName + ".sen_lin_lb='12'and ((substr(" + tableName + ".lz,1,2)<>'11' and substr(" + tableName + ".lz,1,2)<>'12') or (((substr(" + tableName + ".lz,1,2)='11' or substr(" + tableName + ".lz,1,2)='12')) and " + tableName + ".dl between '113' and '114')) then " + tableName + "." + StatisticsFieldName + " else 0 end) as һ�㹫������," +
                                 "sum(case when " + tableName + ".sen_lin_lb between '21' and '22'and " + tableName + ".dl<='114' then " + tableName + "." + StatisticsFieldName + " else 0 end ) as ��Ʒ�ֺϼ�," +
                                 "sum(case when " + tableName + ".sen_lin_lb ='21' and " + tableName + ".dl<='114' then " + tableName + "." + StatisticsFieldName + " else 0 end) as �ص���Ʒ�ֺϼ�," +
                                 "sum(case when " + tableName + ".sen_lin_lb='21' and " + tableName + ".dl<='114' and (substr(" + tableName + ".lz,1,2) between '23' and '25') then " + tableName + "." + StatisticsFieldName + " else 0 end ) as �ص���Ʒ��С��," +
                                 "sum(case when " + tableName + ".sen_lin_lb='21' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='23' then " + tableName + "." + StatisticsFieldName + " else 0 end) as �ص��ò���," +
                                 "sum(case when " + tableName + ".sen_lin_lb='21' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='25'and " + tableName + ".dl='114' then " + tableName + "." + StatisticsFieldName + " else 0 end) as �ص㾭����," +
                                 "sum(case when " + tableName + ".sen_lin_lb='21' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='24' then " + tableName + "." + StatisticsFieldName + " else 0 end) as �ص�н̼��," +
                                 "sum(case when " + tableName + ".sen_lin_lb='21'and ((substr(" + tableName + ".lz,1,2)<>'23' and substr(" + tableName + ".lz,1,2)<>'24' and substr(" + tableName + ".lz,1,2)<>'25') or (substr(" + tableName + ".lz,1,2)='23' and " + tableName + ".dl>'112') or (substr(" + tableName + ".lz,1,2)='25' and " + tableName + ".dl<>'114')) then " + tableName + "." + StatisticsFieldName + " else 0 end) as �ص�����," +
                                 "sum(case when " + tableName + ".sen_lin_lb ='22' and " + tableName + ".dl<='114' then " + tableName + "." + StatisticsFieldName + "  else 0 end) as һ����Ʒ�ֺϼ�," +
                                 "sum(case when " + tableName + ".sen_lin_lb='22' and " + tableName + ".dl<='114' and (substr(" + tableName + ".lz,1,2) between '23' and '25') then " + tableName + "." + StatisticsFieldName + " else 0 end ) as һ����Ʒ��С��," +
                                 "sum(case when " + tableName + ".sen_lin_lb='22' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='23' then " + tableName + "." + StatisticsFieldName + " else 0 end) as һ���ò���," +
                                 "sum(case when " + tableName + ".sen_lin_lb='22' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='25'and " + tableName + ".dl='114' then " + tableName + "." + StatisticsFieldName + " else 0 end) as һ�㾭����," +
                                 "sum(case when " + tableName + ".sen_lin_lb='22' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='24' then " + tableName + "." + StatisticsFieldName + " else 0 end) as һ��н̼��," +
                                 "sum(case when " + tableName + ".sen_lin_lb='22'and ((substr(" + tableName + ".lz,1,2)<>'23' and substr(" + tableName + ".lz,1,2)<>'24' and substr(" + tableName + ".lz,1,2)<>'25') or (substr(" + tableName + ".lz,1,2)='23' and " + tableName + ".dl>'112') or (substr(" + tableName + ".lz,1,2)='25' and " + tableName + ".dl<>'114')) then " + tableName + "." + StatisticsFieldName + " else 0 end) as һ������" +
                                 " from " + tableName +
                                 " group by xiang";
                pWorkspace.ExecuteSQL(townsSQL);
                //��ѯ�м����� ygc 2012-10-22
                string SHISQL = "insert  into EightTable_LDLYFXGHMJ  select substr(" +
                 tableName + ".shi,1,4) as ͳ�Ƶ�λ," +
                 "sum (case when " + tableName + ".sen_lin_lb in('11','12','21','22') and " + tableName + ".dl <= '114'then " + tableName + "." + StatisticsFieldName + " else 0 end) as �ϼ�," +
                 "sum(case when " + tableName + ".sen_lin_lb between '11' and '12'and " + tableName + ".dl<='114' then " + tableName + "." + StatisticsFieldName + " else 0 end) as �����ֺϼ�," +
                 "sum (case when " + tableName + ".sen_lin_lb ='11' and " + tableName + ".dl<='114' then " + tableName + "." + StatisticsFieldName + "  else 0 end) as �ص㹫���ֺϼ�," +
                 "sum(case when " + tableName + ".sen_lin_lb='11' and " + tableName + ".dl<='112' and (substr(" + tableName + ".lz,1,2)='11'or substr(" + tableName + ".lz,1,2)='12') then " + tableName + "." + StatisticsFieldName + " else 0 end ) as �ص㹫��С��," +
                 "sum (case when " + tableName + ".sen_lin_lb='11' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='11' then " + tableName + "." + StatisticsFieldName + " else 0 end) as �ص㹫�������," +
                 "sum(case when " + tableName + ".sen_lin_lb='11'and " + tableName + ".dl<='112' and substr(" + tableName + ".lz,1,2)='12' then " + tableName + "." + StatisticsFieldName + " else 0 end ) as �ص㹫��������," +
                 "sum(case when " + tableName + ".sen_lin_lb='11'and (((substr(" + tableName + ".lz,1,2)='11'or substr(" + tableName + ".lz,1,2)='12') and " + tableName + ".dl between '113' and '114')) and (substr(" + tableName + ".lz,1,2)<>'11' or substr(" + tableName + ".lz,1,2)<>'12') then " + tableName + "." + StatisticsFieldName + "  else 0 end) as  �ص㹫������," +
                 "sum(case when " + tableName + ".sen_lin_lb ='12' and " + tableName + ".dl<='114' then " + tableName + "." + StatisticsFieldName + "   end ) as һ�㹫��ϼ�," +
                 "sum(case when " + tableName + ".sen_lin_lb='12' and " + tableName + ".dl<='112' and (substr(" + tableName + ".lz,1,2)='11'or substr(" + tableName + ".lz,1,2)='12') then " + tableName + "." + StatisticsFieldName + " else 0 end ) as һ�㹫��С��," +
                 "sum(case when " + tableName + ".sen_lin_lb='12' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='11' then " + tableName + "." + StatisticsFieldName + " else 0 end) as һ�㹫�������," +
                 "sum(case when " + tableName + ".sen_lin_lb='12'and " + tableName + ".dl<='112' and substr(" + tableName + ".lz,1,2)='12' then " + tableName + "." + StatisticsFieldName + " else 0 end ) as һ�㹫��������," +
                 "sum(case when " + tableName + ".sen_lin_lb='12'and ((substr(" + tableName + ".lz,1,2)<>'11' and substr(" + tableName + ".lz,1,2)<>'12') or (((substr(" + tableName + ".lz,1,2)='11' or substr(" + tableName + ".lz,1,2)='12')) and " + tableName + ".dl between '113' and '114')) then " + tableName + "." + StatisticsFieldName + " else 0 end) as һ�㹫������," +
                 "sum(case when " + tableName + ".sen_lin_lb between '21' and '22'and " + tableName + ".dl<='114' then " + tableName + "." + StatisticsFieldName + " else 0 end ) as ��Ʒ�ֺϼ�," +
                 "sum(case when " + tableName + ".sen_lin_lb ='21' and " + tableName + ".dl<='114' then " + tableName + "." + StatisticsFieldName + " else 0 end) as �ص���Ʒ�ֺϼ�," +
                 "sum(case when " + tableName + ".sen_lin_lb='21' and " + tableName + ".dl<='114' and (substr(" + tableName + ".lz,1,2) between '23' and '25') then " + tableName + "." + StatisticsFieldName + " else 0 end ) as �ص���Ʒ��С��," +
                 "sum(case when " + tableName + ".sen_lin_lb='21' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='23' then " + tableName + "." + StatisticsFieldName + " else 0 end) as �ص��ò���," +
                 "sum(case when " + tableName + ".sen_lin_lb='21' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='25'and " + tableName + ".dl='114' then " + tableName + "." + StatisticsFieldName + " else 0 end) as �ص㾭����," +
                 "sum(case when " + tableName + ".sen_lin_lb='21' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='24' then " + tableName + "." + StatisticsFieldName + " else 0 end) as �ص�н̼��," +
                 "sum(case when " + tableName + ".sen_lin_lb='21'and ((substr(" + tableName + ".lz,1,2)<>'23' and substr(" + tableName + ".lz,1,2)<>'24' and substr(" + tableName + ".lz,1,2)<>'25') or (substr(" + tableName + ".lz,1,2)='23' and " + tableName + ".dl>'112') or (substr(" + tableName + ".lz,1,2)='25' and " + tableName + ".dl<>'114')) then " + tableName + "." + StatisticsFieldName + " else 0 end) as �ص�����," +
                 "sum(case when " + tableName + ".sen_lin_lb ='22' and " + tableName + ".dl<='114' then " + tableName + "." + StatisticsFieldName + "  else 0 end) as һ����Ʒ�ֺϼ�," +
                 "sum(case when " + tableName + ".sen_lin_lb='22' and " + tableName + ".dl<='114' and (substr(" + tableName + ".lz,1,2) between '23' and '25') then " + tableName + "." + StatisticsFieldName + " else 0 end ) as һ����Ʒ��С��," +
                 "sum(case when " + tableName + ".sen_lin_lb='22' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='23' then " + tableName + "." + StatisticsFieldName + " else 0 end) as һ���ò���," +
                 "sum(case when " + tableName + ".sen_lin_lb='22' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='25'and " + tableName + ".dl='114' then " + tableName + "." + StatisticsFieldName + " else 0 end) as һ�㾭����," +
                 "sum(case when " + tableName + ".sen_lin_lb='22' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='24' then " + tableName + "." + StatisticsFieldName + " else 0 end) as һ��н̼��," +
                 "sum(case when " + tableName + ".sen_lin_lb='22'and ((substr(" + tableName + ".lz,1,2)<>'23' and substr(" + tableName + ".lz,1,2)<>'24' and substr(" + tableName + ".lz,1,2)<>'25') or (substr(" + tableName + ".lz,1,2)='23' and " + tableName + ".dl>'112') or (substr(" + tableName + ".lz,1,2)='25' and " + tableName + ".dl<>'114')) then " + tableName + "." + StatisticsFieldName + " else 0 end) as һ������" +
                 " from " + tableName +
                 " group by substr(shi,1,4)";
                pWorkspace.ExecuteSQL(SHISQL);
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
                                     "sum (case when " + tableName + ".sen_lin_lb in('11','12','21','22') and " + tableName + ".dl <= '114'then " + tableName + "." + StatisticsFieldName + " else 0 end) as �ϼ�," +
                                     "sum(case when " + tableName + ".sen_lin_lb between '11' and '12'and " + tableName + ".dl<='114' then " + tableName + "." + StatisticsFieldName + " else 0 end) as �����ֺϼ�," +
                                     "sum (case when " + tableName + ".sen_lin_lb ='11' and " + tableName + ".dl<='114' then " + tableName + "." + StatisticsFieldName + "  else 0 end) as �ص㹫���ֺϼ�," +
                                     "sum(case when " + tableName + ".sen_lin_lb='11' and " + tableName + ".dl<='112' and (substr(" + tableName + ".lz,1,2)='11'or substr(" + tableName + ".lz,1,2)='12') then " + tableName + "." + StatisticsFieldName + " else 0 end ) as �ص㹫��С��," +
                                     "sum (case when " + tableName + ".sen_lin_lb='11' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='11' then " + tableName + "." + StatisticsFieldName + " else 0 end) as �ص㹫�������," +
                                     "sum(case when " + tableName + ".sen_lin_lb='11'and " + tableName + ".dl<='112' and substr(" + tableName + ".lz,1,2)='12' then " + tableName + "." + StatisticsFieldName + " else 0 end ) as �ص㹫��������," +
                                     "sum(case when " + tableName + ".sen_lin_lb='11'and (((substr(" + tableName + ".lz,1,2)='11'or substr(" + tableName + ".lz,1,2)='12') and " + tableName + ".dl between '113' and '114')) and (substr(" + tableName + ".lz,1,2)<>'11' or substr(" + tableName + ".lz,1,2)<>'12') then " + tableName + "." + StatisticsFieldName + "  else 0 end) as  �ص㹫������," +
                                     "sum(case when " + tableName + ".sen_lin_lb ='12' and " + tableName + ".dl<='114' then " + tableName + "." + StatisticsFieldName + "   end ) as һ�㹫��ϼ�," +
                                     "sum(case when " + tableName + ".sen_lin_lb='12' and " + tableName + ".dl<='112' and (substr(" + tableName + ".lz,1,2)='11'or substr(" + tableName + ".lz,1,2)='12') then " + tableName + "." + StatisticsFieldName + " else 0 end ) as һ�㹫��С��," +
                                     "sum(case when " + tableName + ".sen_lin_lb='12' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='11' then " + tableName + "." + StatisticsFieldName + " else 0 end) as һ�㹫�������," +
                                     "sum(case when " + tableName + ".sen_lin_lb='12'and " + tableName + ".dl<='112' and substr(" + tableName + ".lz,1,2)='12' then " + tableName + "." + StatisticsFieldName + " else 0 end ) as һ�㹫��������," +
                                     "sum(case when " + tableName + ".sen_lin_lb='12'and ((substr(" + tableName + ".lz,1,2)<>'11' and substr(" + tableName + ".lz,1,2)<>'12') or (((substr(" + tableName + ".lz,1,2)='11' or substr(" + tableName + ".lz,1,2)='12')) and " + tableName + ".dl between '113' and '114')) then " + tableName + "." + StatisticsFieldName + " else 0 end) as һ�㹫������," +
                                     "sum(case when " + tableName + ".sen_lin_lb between '21' and '22'and " + tableName + ".dl<='114' then " + tableName + "." + StatisticsFieldName + " else 0 end ) as ��Ʒ�ֺϼ�," +
                                     "sum(case when " + tableName + ".sen_lin_lb ='21' and " + tableName + ".dl<='114' then " + tableName + "." + StatisticsFieldName + " else 0 end) as �ص���Ʒ�ֺϼ�," +
                                     "sum(case when " + tableName + ".sen_lin_lb='21' and " + tableName + ".dl<='114' and (substr(" + tableName + ".lz,1,2) between '23' and '25') then " + tableName + "." + StatisticsFieldName + " else 0 end ) as �ص���Ʒ��С��," +
                                     "sum(case when " + tableName + ".sen_lin_lb='21' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='23' then " + tableName + "." + StatisticsFieldName + " else 0 end) as �ص��ò���," +
                                     "sum(case when " + tableName + ".sen_lin_lb='21' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='25'and " + tableName + ".dl='114' then " + tableName + "." + StatisticsFieldName + " else 0 end) as �ص㾭����," +
                                     "sum(case when " + tableName + ".sen_lin_lb='21' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='24' then " + tableName + "." + StatisticsFieldName + " else 0 end) as �ص�н̼��," +
                                     "sum(case when " + tableName + ".sen_lin_lb='21'and ((substr(" + tableName + ".lz,1,2)<>'23' and substr(" + tableName + ".lz,1,2)<>'24' and substr(" + tableName + ".lz,1,2)<>'25') or (substr(" + tableName + ".lz,1,2)='23' and " + tableName + ".dl>'112') or (substr(" + tableName + ".lz,1,2)='25' and " + tableName + ".dl<>'114')) then " + tableName + "." + StatisticsFieldName + " else 0 end) as �ص�����," +
                                     "sum(case when " + tableName + ".sen_lin_lb ='22' and " + tableName + ".dl<='114' then " + tableName + "." + StatisticsFieldName + "  else 0 end) as һ����Ʒ�ֺϼ�," +
                                     "sum(case when " + tableName + ".sen_lin_lb='22' and " + tableName + ".dl<='114' and (substr(" + tableName + ".lz,1,2) between '23' and '25') then " + tableName + "." + StatisticsFieldName + " else 0 end ) as һ����Ʒ��С��," +
                                     "sum(case when " + tableName + ".sen_lin_lb='22' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='23' then " + tableName + "." + StatisticsFieldName + " else 0 end) as һ���ò���," +
                                     "sum(case when " + tableName + ".sen_lin_lb='22' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='25'and " + tableName + ".dl='114' then " + tableName + "." + StatisticsFieldName + " else 0 end) as һ�㾭����," +
                                     "sum(case when " + tableName + ".sen_lin_lb='22' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='24' then " + tableName + "." + StatisticsFieldName + " else 0 end) as һ��н̼��," +
                                     "sum(case when " + tableName + ".sen_lin_lb='22'and ((substr(" + tableName + ".lz,1,2)<>'23' and substr(" + tableName + ".lz,1,2)<>'24' and substr(" + tableName + ".lz,1,2)<>'25') or (substr(" + tableName + ".lz,1,2)='23' and " + tableName + ".dl>'112') or (substr(" + tableName + ".lz,1,2)='25' and " + tableName + ".dl<>'114')) then " + tableName + "." + StatisticsFieldName + " else 0 end) as һ������" +
                                     " from " + tableName +
                                     " where lc <>' '"+
                                     " group by lc";
                    pWorkspace.ExecuteSQL(CitySQL);
                }

                //��ѯ����ͳ������
                //ygc 2012-8-22
                string townsSQL = "insert  into EightTable_LDLYFXGHMJ  select " +
                                 tableName + ".lc as ͳ�Ƶ�λ," +
                                 "sum (case when " + tableName + ".sen_lin_lb in('11','12','21','22') and " + tableName + ".dl <= '114'then " + tableName + "." + StatisticsFieldName + " else 0 end) as �ϼ�," +
                                 "sum(case when " + tableName + ".sen_lin_lb between '11' and '12'and " + tableName + ".dl<='114' then " + tableName + "." + StatisticsFieldName + " else 0 end) as �����ֺϼ�," +
                                 "sum (case when " + tableName + ".sen_lin_lb ='11' and " + tableName + ".dl<='114' then " + tableName + "." + StatisticsFieldName + "  else 0 end) as �ص㹫���ֺϼ�," +
                                 "sum(case when " + tableName + ".sen_lin_lb='11' and " + tableName + ".dl<='112' and (substr(" + tableName + ".lz,1,2)='11'or substr(" + tableName + ".lz,1,2)='12') then " + tableName + "." + StatisticsFieldName + " else 0 end ) as �ص㹫��С��," +
                                 "sum (case when " + tableName + ".sen_lin_lb='11' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='11' then " + tableName + "." + StatisticsFieldName + " else 0 end) as �ص㹫�������," +
                                 "sum(case when " + tableName + ".sen_lin_lb='11'and " + tableName + ".dl<='112' and substr(" + tableName + ".lz,1,2)='12' then " + tableName + "." + StatisticsFieldName + " else 0 end ) as �ص㹫��������," +
                                 "sum(case when " + tableName + ".sen_lin_lb='11'and (((substr(" + tableName + ".lz,1,2)='11'or substr(" + tableName + ".lz,1,2)='12') and " + tableName + ".dl between '113' and '114')) and (substr(" + tableName + ".lz,1,2)<>'11' or substr(" + tableName + ".lz,1,2)<>'12') then " + tableName + "." + StatisticsFieldName + "  else 0 end) as  �ص㹫������," +
                                 "sum(case when " + tableName + ".sen_lin_lb ='12' and " + tableName + ".dl<='114' then " + tableName + "." + StatisticsFieldName + "   end ) as һ�㹫��ϼ�," +
                                 "sum(case when " + tableName + ".sen_lin_lb='12' and " + tableName + ".dl<='112' and (substr(" + tableName + ".lz,1,2)='11'or substr(" + tableName + ".lz,1,2)='12') then " + tableName + "." + StatisticsFieldName + " else 0 end ) as һ�㹫��С��," +
                                 "sum(case when " + tableName + ".sen_lin_lb='12' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='11' then " + tableName + "." + StatisticsFieldName + " else 0 end) as һ�㹫�������," +
                                 "sum(case when " + tableName + ".sen_lin_lb='12'and " + tableName + ".dl<='112' and substr(" + tableName + ".lz,1,2)='12' then " + tableName + "." + StatisticsFieldName + " else 0 end ) as һ�㹫��������," +
                                 "sum(case when " + tableName + ".sen_lin_lb='12'and ((substr(" + tableName + ".lz,1,2)<>'11' and substr(" + tableName + ".lz,1,2)<>'12') or (((substr(" + tableName + ".lz,1,2)='11' or substr(" + tableName + ".lz,1,2)='12')) and " + tableName + ".dl between '113' and '114')) then " + tableName + "." + StatisticsFieldName + " else 0 end) as һ�㹫������," +
                                 "sum(case when " + tableName + ".sen_lin_lb between '21' and '22'and " + tableName + ".dl<='114' then " + tableName + "." + StatisticsFieldName + " else 0 end ) as ��Ʒ�ֺϼ�," +
                                 "sum(case when " + tableName + ".sen_lin_lb ='21' and " + tableName + ".dl<='114' then " + tableName + "." + StatisticsFieldName + " else 0 end) as �ص���Ʒ�ֺϼ�," +
                                 "sum(case when " + tableName + ".sen_lin_lb='21' and " + tableName + ".dl<='114' and (substr(" + tableName + ".lz,1,2) between '23' and '25') then " + tableName + "." + StatisticsFieldName + " else 0 end ) as �ص���Ʒ��С��," +
                                 "sum(case when " + tableName + ".sen_lin_lb='21' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='23' then " + tableName + "." + StatisticsFieldName + " else 0 end) as �ص��ò���," +
                                 "sum(case when " + tableName + ".sen_lin_lb='21' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='25'and " + tableName + ".dl='114' then " + tableName + "." + StatisticsFieldName + " else 0 end) as �ص㾭����," +
                                 "sum(case when " + tableName + ".sen_lin_lb='21' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='24' then " + tableName + "." + StatisticsFieldName + " else 0 end) as �ص�н̼��," +
                                 "sum(case when " + tableName + ".sen_lin_lb='21'and ((substr(" + tableName + ".lz,1,2)<>'23' and substr(" + tableName + ".lz,1,2)<>'24' and substr(" + tableName + ".lz,1,2)<>'25') or (substr(" + tableName + ".lz,1,2)='23' and " + tableName + ".dl>'112') or (substr(" + tableName + ".lz,1,2)='25' and " + tableName + ".dl<>'114')) then " + tableName + "." + StatisticsFieldName + " else 0 end) as �ص�����," +
                                 "sum(case when " + tableName + ".sen_lin_lb ='22' and " + tableName + ".dl<='114' then " + tableName + "." + StatisticsFieldName + "  else 0 end) as һ����Ʒ�ֺϼ�," +
                                 "sum(case when " + tableName + ".sen_lin_lb='22' and " + tableName + ".dl<='114' and (substr(" + tableName + ".lz,1,2) between '23' and '25') then " + tableName + "." + StatisticsFieldName + " else 0 end ) as һ����Ʒ��С��," +
                                 "sum(case when " + tableName + ".sen_lin_lb='22' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='23' then " + tableName + "." + StatisticsFieldName + " else 0 end) as һ���ò���," +
                                 "sum(case when " + tableName + ".sen_lin_lb='22' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='25'and " + tableName + ".dl='114' then " + tableName + "." + StatisticsFieldName + " else 0 end) as һ�㾭����," +
                                 "sum(case when " + tableName + ".sen_lin_lb='22' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='24' then " + tableName + "." + StatisticsFieldName + " else 0 end) as һ��н̼��," +
                                 "sum(case when " + tableName + ".sen_lin_lb='22'and ((substr(" + tableName + ".lz,1,2)<>'23' and substr(" + tableName + ".lz,1,2)<>'24' and substr(" + tableName + ".lz,1,2)<>'25') or (substr(" + tableName + ".lz,1,2)='23' and " + tableName + ".dl>'112') or (substr(" + tableName + ".lz,1,2)='25' and " + tableName + ".dl<>'114')) then " + tableName + "." + StatisticsFieldName + " else 0 end) as һ������" +
                                 " from " + tableName +
                                 "  where lc <> ' '"+
                                 " group by lc";
                pWorkspace.ExecuteSQL(townsSQL);
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
                pWorkspace.ExecuteSQL("create table tempTable as select * from " + tableName);
                //������ʱ���е�Ȩ����Ϣ
                pWorkspace.ExecuteSQL("update tempTable set LD_QS='����' where LD_QS in('10','15')");
                pWorkspace.ExecuteSQL("update tempTable set LD_QS='����' where LD_QS in('21','22','23','16')");
                pWorkspace.ExecuteSQL("update tempTable set LD_QS='����' where LD_QS in('20')");
                //ͳ���ؼ�����
                string CitySQL = "create table EightTable_LDXZ as select " +
                                 "xian as ͳ�Ƶ�λ,LD_QS as Ȩ��,substr(QI_YUAN,1,1) as ��Դ," +
                                 "sum(case when DL <>' ' then " + StatisticsFieldName + " else 0 end) as ���������," +
                                 "sum(case when DL<=180 then " + StatisticsFieldName + " else 0 end) as �ֵغϼ�," +
                                 "sum(case when DL between '111' and '114' then " + StatisticsFieldName + " else 0 end) as ���ֵ�С��," +
                                 "sum(case when DL='111' OR DL='112' OR DL='114' then " + StatisticsFieldName + " else 0 end) as ��ľ��," +
                                 "sum(case when DL='113' then " + StatisticsFieldName + " else 0 end) as ����," +
                                 "sum(case when DL='114' then " + StatisticsFieldName + " else 0 end) as ������," +
                                 "sum(case when DL='120' then " + StatisticsFieldName + " else 0 end) as ���ֵ�," +
                                 "sum(case when DL between '131' and '133' then " + StatisticsFieldName + " else 0 end) as ��ľ�ֵ�С��," +
                                 "sum(case when DL='131' then " + StatisticsFieldName + " else 0 end) as �ع�," +
                                 "sum(case when DL='133' or DL='132' then " + StatisticsFieldName + " else 0 end ) as ������ľ��," +
                                 "sum(case when DL='141' or DL='142' then " + StatisticsFieldName + " else 0 end) as δ�������ֵ�," +
                                 "sum(case when DL='150' then " + StatisticsFieldName + " else 0 end) as ���Ե�," +
                                 "sum(case when DL='161' or DL='162' or DL='163' then " + StatisticsFieldName + " else 0 end) as ����ľ�ֵ�," +
                                 "sum(case when DL='171' or DL='172' or DL='173' or DL='174' then " + StatisticsFieldName + " else 0 end) as ���ֵ�," +
                                 "sum(case when DL='180' then  " + StatisticsFieldName + " else 0 end ) as �ָ���," +
                                 "sum(case when dl like '2%' then " + StatisticsFieldName + " else 0 end) as ���ֵ�," +
                                 "sum (case when dl=' ' then " + StatisticsFieldName + " else 0 end) as ɭ�ָ�����," +
                                 "sum(case when dl=' ' then " + StatisticsFieldName + " else 0 end) as ��ľ�̻���" +
                                 "  from tempTable" +
                                 "  where LD_QS is not null and LD_QS<>' ' and substr(QI_YUAN,1,1)<>' '" +
                                 "  group by xian,rollup(LD_QS),rollup(substr(QI_YUAN,1,1))" +
                                 "  order by LD_QS desc,substr(QI_YUAN,1,1) desc";
                pWorkspace.ExecuteSQL(CitySQL);
                //ͳ����������
                string townsSQL = "insert into EightTable_LDXZ  select " +
                                 "xiang as ͳ�Ƶ�λ,LD_QS as Ȩ��,substr(QI_YUAN,1,1) as ��Դ," +
                                 "sum(case when DL <>' ' then " + StatisticsFieldName + " else 0 end) as ���������," +
                                 "sum(case when DL<=180 then " + StatisticsFieldName + " else 0 end) as �ֵغϼ�," +
                                 "sum(case when DL between '111' and '114' then " + StatisticsFieldName + " else 0 end) as ���ֵ�С��," +
                                 "sum(case when DL='111' OR DL='112' OR DL='114' then " + StatisticsFieldName + " else 0 end) as ��ľ��," +
                                 "sum(case when DL='113' then " + StatisticsFieldName + " else 0 end) as ����," +
                                 "sum(case when DL='114' then " + StatisticsFieldName + " else 0 end) as ������," +
                                 "sum(case when DL='120' then " + StatisticsFieldName + " else 0 end) as ���ֵ�," +
                                 "sum(case when DL between '131' and '133' then " + StatisticsFieldName + " else 0 end) as ��ľ�ֵ�С��," +
                                 "sum(case when DL='131' then " + StatisticsFieldName + " else 0 end) as �ع�," +
                                 "sum(case when DL='133' or DL='132' then " + StatisticsFieldName + " else 0 end ) as ������ľ��," +
                                 "sum(case when DL='141' or DL='142' then " + StatisticsFieldName + " else 0 end) as δ�������ֵ�," +
                                 "sum(case when DL='150' then " + StatisticsFieldName + " else 0 end) as ���Ե�," +
                                 "sum(case when DL='161' or DL='162' or DL='163' then " + StatisticsFieldName + " else 0 end) as ����ľ�ֵ�," +
                                 "sum(case when DL='171' or DL='172' or DL='173' or DL='174' then " + StatisticsFieldName + " else 0 end) as ���ֵ�," +
                                 "sum(case when DL='180' then  " + StatisticsFieldName + " else 0 end ) as �ָ���," +
                                 "sum(case when dl like '2%' then " + StatisticsFieldName + " else 0 end) as ���ֵ�," +
                                 "sum (case when dl=' ' then " + StatisticsFieldName + " else 0 end) as ɭ�ָ�����," +
                                 "sum(case when dl=' ' then " + StatisticsFieldName + " else 0 end) as ��ľ�̻���" +
                                 "  from tempTable" +
                                 "  where LD_QS is not null and LD_QS<>' ' and substr(QI_YUAN,1,1)<>' '" +
                                 "  group by xiang,rollup(LD_QS),rollup(substr(QI_YUAN,1,1))" +
                                 "  order by LD_QS desc,substr(QI_YUAN,1,1) desc";
                pWorkspace.ExecuteSQL(townsSQL);
                //����м����� ygc 2012-10-22
                string SHISQL = "insert into EightTable_LDXZ  select " +
                 "substr(shi,1,4) as ͳ�Ƶ�λ,LD_QS as Ȩ��,substr(QI_YUAN,1,1) as ��Դ," +
                 "sum(case when DL <>' ' then " + StatisticsFieldName + " else 0 end) as ���������," +
                 "sum(case when DL<=180 then " + StatisticsFieldName + " else 0 end) as �ֵغϼ�," +
                 "sum(case when DL between '111' and '114' then " + StatisticsFieldName + " else 0 end) as ���ֵ�С��," +
                 "sum(case when DL='111' OR DL='112' OR DL='114' then " + StatisticsFieldName + " else 0 end) as ��ľ��," +
                 "sum(case when DL='113' then " + StatisticsFieldName + " else 0 end) as ����," +
                 "sum(case when DL='114' then " + StatisticsFieldName + " else 0 end) as ������," +
                 "sum(case when DL='120' then " + StatisticsFieldName + " else 0 end) as ���ֵ�," +
                 "sum(case when DL between '131' and '133' then " + StatisticsFieldName + " else 0 end) as ��ľ�ֵ�С��," +
                 "sum(case when DL='131' then " + StatisticsFieldName + " else 0 end) as �ع�," +
                 "sum(case when DL='133' or DL='132' then " + StatisticsFieldName + " else 0 end ) as ������ľ��," +
                 "sum(case when DL='141' or DL='142' then " + StatisticsFieldName + " else 0 end) as δ�������ֵ�," +
                 "sum(case when DL='150' then " + StatisticsFieldName + " else 0 end) as ���Ե�," +
                 "sum(case when DL='161' or DL='162' or DL='163' then " + StatisticsFieldName + " else 0 end) as ����ľ�ֵ�," +
                 "sum(case when DL='171' or DL='172' or DL='173' or DL='174' then " + StatisticsFieldName + " else 0 end) as ���ֵ�," +
                 "sum(case when DL='180' then  " + StatisticsFieldName + " else 0 end ) as �ָ���," +
                 "sum(case when dl like '2%' then " + StatisticsFieldName + " else 0 end) as ���ֵ�," +
                 "sum (case when dl=' ' then " + StatisticsFieldName + " else 0 end) as ɭ�ָ�����," +
                 "sum(case when dl=' ' then " + StatisticsFieldName + " else 0 end) as ��ľ�̻���" +
                 "  from tempTable" +
                 "  where LD_QS is not null and LD_QS<>' ' and substr(QI_YUAN,1,1)<>' '" +
                 "  group by substr(shi,1,4),rollup(LD_QS),rollup(substr(QI_YUAN,1,1))" +
                 "  order by LD_QS desc,substr(QI_YUAN,1,1) desc";
                pWorkspace.ExecuteSQL(SHISQL);
                //����ɭ�ָ�����
                pWorkspace.ExecuteSQL("update EightTable_LDXZ set ɭ�ָ�����=(���ֵ�С��+�ع�)/��������� * 100 where Ȩ�� is null");
                //������ľ�̻���
                pWorkspace.ExecuteSQL("update EightTable_LDXZ set ��ľ�̻���=(���ֵ�С��+��ľ�ֵ�С��)/��������� * 100 where Ȩ�� is null ");
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
                pWorkspace.ExecuteSQL("create table tempTable as select * from " + tableName);
                //������ʱ���е�Ȩ����Ϣ
                pWorkspace.ExecuteSQL("update tempTable set LD_QS='����' where LD_QS in('10','15')");
                pWorkspace.ExecuteSQL("update tempTable set LD_QS='����' where LD_QS in('21','22','23','16')");
                pWorkspace.ExecuteSQL("update tempTable set LD_QS='����' where LD_QS in('20')");
                if (!ExistTable(pWorkspace, "EightTable_LDXZ"))
                {
                    //ͳ���ؼ�����
                    string CitySQL = "create table EightTable_LDXZ as select " +
                                     "lc as ͳ�Ƶ�λ,LD_QS as Ȩ��,substr(QI_YUAN,1,1) as ��Դ," +
                                     "sum(case when DL <>' ' then " + StatisticsFieldName + " else 0 end) as ���������," +
                                     "sum(case when DL<=180 then " + StatisticsFieldName + " else 0 end) as �ֵغϼ�," +
                                     "sum(case when DL between '111' and '114' then " + StatisticsFieldName + " else 0 end) as ���ֵ�С��," +
                                     "sum(case when DL='111' OR DL='112' OR DL='114' then " + StatisticsFieldName + " else 0 end) as ��ľ��," +
                                     "sum(case when DL='113' then " + StatisticsFieldName + " else 0 end) as ����," +
                                     "sum(case when DL='114' then " + StatisticsFieldName + " else 0 end) as ������," +
                                     "sum(case when DL='120' then " + StatisticsFieldName + " else 0 end) as ���ֵ�," +
                                     "sum(case when DL between '131' and '133' then " + StatisticsFieldName + " else 0 end) as ��ľ�ֵ�С��," +
                                     "sum(case when DL='131' then " + StatisticsFieldName + " else 0 end) as �ع�," +
                                     "sum(case when DL='133' or DL='132' then " + StatisticsFieldName + " else 0 end ) as ������ľ��," +
                                     "sum(case when DL='141' or DL='142' then " + StatisticsFieldName + " else 0 end) as δ�������ֵ�," +
                                     "sum(case when DL='150' then " + StatisticsFieldName + " else 0 end) as ���Ե�," +
                                     "sum(case when DL='161' or DL='162' or DL='163' then " + StatisticsFieldName + " else 0 end) as ����ľ�ֵ�," +
                                     "sum(case when DL='171' or DL='172' or DL='173' or DL='174' then " + StatisticsFieldName + " else 0 end) as ���ֵ�," +
                                     "sum(case when DL='180' then  " + StatisticsFieldName + " else 0 end ) as �ָ���," +
                                     "sum(case when dl like '2%' then " + StatisticsFieldName + " else 0 end) as ���ֵ�," +
                                     "sum (case when dl=' ' then " + StatisticsFieldName + " else 0 end) as ɭ�ָ�����," +
                                     "sum(case when dl=' ' then " + StatisticsFieldName + " else 0 end) as ��ľ�̻���" +
                                     "  from tempTable" +
                                     "  where LD_QS is not null and LD_QS<>' ' and substr(QI_YUAN,1,1)<>' ' and lc<>' '" +
                                     "  group by lc,rollup(LD_QS),rollup(substr(QI_YUAN,1,1))" +
                                     "  order by LD_QS desc,substr(QI_YUAN,1,1) desc";
                    pWorkspace.ExecuteSQL(CitySQL);
                }
                //ͳ����������
                string townsSQL = "insert into EightTable_LDXZ  select " +
                                 "lc as ͳ�Ƶ�λ,LD_QS as Ȩ��,substr(QI_YUAN,1,1) as ��Դ," +
                                 "sum(case when DL <>' ' then " + StatisticsFieldName + " else 0 end) as ���������," +
                                 "sum(case when DL<=180 then " + StatisticsFieldName + " else 0 end) as �ֵغϼ�," +
                                 "sum(case when DL between '111' and '114' then " + StatisticsFieldName + " else 0 end) as ���ֵ�С��," +
                                 "sum(case when DL='111' OR DL='112' OR DL='114' then " + StatisticsFieldName + " else 0 end) as ��ľ��," +
                                 "sum(case when DL='113' then " + StatisticsFieldName + " else 0 end) as ����," +
                                 "sum(case when DL='114' then " + StatisticsFieldName + " else 0 end) as ������," +
                                 "sum(case when DL='120' then " + StatisticsFieldName + " else 0 end) as ���ֵ�," +
                                 "sum(case when DL between '131' and '133' then " + StatisticsFieldName + " else 0 end) as ��ľ�ֵ�С��," +
                                 "sum(case when DL='131' then " + StatisticsFieldName + " else 0 end) as �ع�," +
                                 "sum(case when DL='133' or DL='132' then " + StatisticsFieldName + " else 0 end ) as ������ľ��," +
                                 "sum(case when DL='141' or DL='142' then " + StatisticsFieldName + " else 0 end) as δ�������ֵ�," +
                                 "sum(case when DL='150' then " + StatisticsFieldName + " else 0 end) as ���Ե�," +
                                 "sum(case when DL='161' or DL='162' or DL='163' then " + StatisticsFieldName + " else 0 end) as ����ľ�ֵ�," +
                                 "sum(case when DL='171' or DL='172' or DL='173' or DL='174' then " + StatisticsFieldName + " else 0 end) as ���ֵ�," +
                                 "sum(case when DL='180' then  " + StatisticsFieldName + " else 0 end ) as �ָ���," +
                                 "sum(case when dl like '2%' then " + StatisticsFieldName + " else 0 end) as ���ֵ�," +
                                 "sum (case when dl=' ' then " + StatisticsFieldName + " else 0 end) as ɭ�ָ�����," +
                                 "sum(case when dl=' ' then " + StatisticsFieldName + " else 0 end) as ��ľ�̻���" +
                                 "  from tempTable" +
                                 "  where LD_QS is not null and LD_QS<>' ' and substr(QI_YUAN,1,1)<>' ' and lc<>' '" +
                                 "  group by lc,rollup(LD_QS),rollup(substr(QI_YUAN,1,1))" +
                                 "  order by LD_QS desc,substr(QI_YUAN,1,1) desc";
                pWorkspace.ExecuteSQL(townsSQL);
                //����ɭ�ָ�����
                pWorkspace.ExecuteSQL("update EightTable_LDXZ set ɭ�ָ�����=(���ֵ�С��+�ع�)/��������� * 100 where Ȩ�� is null");
                //������ľ�̻���
                pWorkspace.ExecuteSQL("update EightTable_LDXZ set ��ľ�̻���=(���ֵ�С��+��ľ�ֵ�С��)/��������� * 100 where Ȩ�� is null ");
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
                //��ѯ�ؼ�����SQL��� ygc 2012-8-23
                string CitySQL = "create table EightTable_LDSLMJGH as select " +
                                 "xian as ͳ�Ƶ�λ," +
                                 "sum(case when DL LIKE '1%' then " + StatisticsFieldName + " else 0 end) as ��״�ֵ�," +
                                 "sum(case when (DL LIKE '11%' OR DL='131') then " + StatisticsFieldName + " else 0 end) as ��״ɭ��," +
                                 "sum(case when substr(dl,1,2) between '21'and '25' and GHDL LIKE '1%' then " + StatisticsFieldName + " else 0 end) as ����ϼ�," +
                                 "sum(case when DL LIKE '24%' AND GHDL LIKE '1%' then " + StatisticsFieldName + " else 0 end) as ����δ���õ�," +
                                 "sum(case when DL LIKE '25%' AND GHDL LIKE '1%' then " + StatisticsFieldName + " else 0 end) as ���佨���õ�," +
                                 "sum(case when DL LIKE '22%' AND GHDL LIKE '1%' then " + StatisticsFieldName + " else 0 end) as �������ݵ�," +
                                 "sum(case when DL LIKE '21%' AND GHDL LIKE '1%' then " + StatisticsFieldName + " else 0 end) as �������," +
                                 "sum(case when DL LIKE '23%' AND GHDL LIKE '1%' then " + StatisticsFieldName + " else 0 end) as ��������," +
                                 "sum(case when substr(dl,1,2) between '21'and '25'and (GHDL LIKE '11%' OR GHDL ='131') then " + StatisticsFieldName + " else 0 end) as ɭ�����Ӻϼ�," +
                                 "sum(case when DL LIKE '22%' AND (GHDL LIKE '11%' OR GHDL ='131') then " + StatisticsFieldName + " else 0 end) as ɭ�������ֵ�," +
                                 "sum(case when DL LIKE '24%' AND (GHDL LIKE '11%' OR GHDL ='131') then " + StatisticsFieldName + " else 0 end) as ɭ������δ���õ�," +
                                 "sum(case when DL LIKE '25%' AND (GHDL LIKE '11%' OR GHDL ='131') then " + StatisticsFieldName + " else 0 end) as ɭ�����ӽ����õ�," +
                                 "sum(case when DL LIKE '22%' AND (GHDL LIKE '11%' OR GHDL ='131') then " + StatisticsFieldName + " else 0 end) as ɭ�����Ӳݵ�," +
                                 "sum(case when DL LIKE '21%' AND (GHDL LIKE '11%' OR GHDL ='131') then " + StatisticsFieldName + " else 0 end) as ɭ�����Ӹ���," +
                                 "sum(case when DL LIKE '23%' AND (GHDL LIKE '11%' OR GHDL ='131') then " + StatisticsFieldName + " else 0 end) as ɭ����������" +
                                 "  from " + tableName +
                                 "  group by xian";
                pWorkspace.ExecuteSQL(CitySQL);
                //��ѯ����ͳ������SQL ygc 2012-8-23
                string townsSQL = " insert into EightTable_LDSLMJGH  select " +
                                 "xiang as ͳ�Ƶ�λ," +
                                 "sum(case when DL LIKE '1%' then " + StatisticsFieldName + " else 0 end) as ��״�ֵ�," +
                                 "sum(case when (DL LIKE '11%' OR DL='131') then " + StatisticsFieldName + " else 0 end) as ��״ɭ��," +
                                 "sum(case when substr(dl,1,2) between '21'and '25' and GHDL LIKE '1%' then " + StatisticsFieldName + " else 0 end) as ����ϼ�," +
                                 "sum(case when DL LIKE '24%' AND GHDL LIKE '1%' then " + StatisticsFieldName + " else 0 end) as ����δ���õ�," +
                                 "sum(case when DL LIKE '25%' AND GHDL LIKE '1%' then " + StatisticsFieldName + " else 0 end) as ���佨���õ�," +
                                 "sum(case when DL LIKE '22%' AND GHDL LIKE '1%' then " + StatisticsFieldName + " else 0 end) as �������ݵ�," +
                                 "sum(case when DL LIKE '21%' AND GHDL LIKE '1%' then " + StatisticsFieldName + " else 0 end) as �������," +
                                 "sum(case when DL LIKE '23%' AND GHDL LIKE '1%' then " + StatisticsFieldName + " else 0 end) as ��������," +
                                 "sum(case when substr(dl,1,2) between '21'and '25'and (GHDL LIKE '11%' OR GHDL ='131') then " + StatisticsFieldName + " else 0 end) as ɭ�����Ӻϼ�," +
                                 "sum(case when DL LIKE '22%' AND (GHDL LIKE '11%' OR GHDL ='131') then " + StatisticsFieldName + " else 0 end) as ɭ�������ֵ�," +
                                 "sum(case when DL LIKE '24%' AND (GHDL LIKE '11%' OR GHDL ='131') then " + StatisticsFieldName + " else 0 end) as ɭ������δ���õ�," +
                                 "sum(case when DL LIKE '25%' AND (GHDL LIKE '11%' OR GHDL ='131') then " + StatisticsFieldName + " else 0 end) as ɭ�����ӽ����õ�," +
                                 "sum(case when DL LIKE '22%' AND (GHDL LIKE '11%' OR GHDL ='131') then " + StatisticsFieldName + " else 0 end) as ɭ�����Ӳݵ�," +
                                 "sum(case when DL LIKE '21%' AND (GHDL LIKE '11%' OR GHDL ='131') then " + StatisticsFieldName + " else 0 end) as ɭ�����Ӹ���," +
                                 "sum(case when DL LIKE '23%' AND (GHDL LIKE '11%' OR GHDL ='131') then " + StatisticsFieldName + " else 0 end) as ɭ����������" +
                                 "  from " + tableName +
                                 "  group by xiang";
                pWorkspace.ExecuteSQL(townsSQL);
                //����м����� ygc 2012-10-22
                string SHISQL = " insert into EightTable_LDSLMJGH  select " +
                 "substr(shi,1,4) as ͳ�Ƶ�λ," +
                 "sum(case when DL LIKE '1%' then " + StatisticsFieldName + " else 0 end) as ��״�ֵ�," +
                 "sum(case when (DL LIKE '11%' OR DL='131') then " + StatisticsFieldName + " else 0 end) as ��״ɭ��," +
                 "sum(case when substr(dl,1,2) between '21'and '25' and GHDL LIKE '1%' then " + StatisticsFieldName + " else 0 end) as ����ϼ�," +
                 "sum(case when DL LIKE '24%' AND GHDL LIKE '1%' then " + StatisticsFieldName + " else 0 end) as ����δ���õ�," +
                 "sum(case when DL LIKE '25%' AND GHDL LIKE '1%' then " + StatisticsFieldName + " else 0 end) as ���佨���õ�," +
                 "sum(case when DL LIKE '22%' AND GHDL LIKE '1%' then " + StatisticsFieldName + " else 0 end) as �������ݵ�," +
                 "sum(case when DL LIKE '21%' AND GHDL LIKE '1%' then " + StatisticsFieldName + " else 0 end) as �������," +
                 "sum(case when DL LIKE '23%' AND GHDL LIKE '1%' then " + StatisticsFieldName + " else 0 end) as ��������," +
                 "sum(case when substr(dl,1,2) between '21'and '25'and (GHDL LIKE '11%' OR GHDL ='131') then " + StatisticsFieldName + " else 0 end) as ɭ�����Ӻϼ�," +
                 "sum(case when DL LIKE '22%' AND (GHDL LIKE '11%' OR GHDL ='131') then " + StatisticsFieldName + " else 0 end) as ɭ�������ֵ�," +
                 "sum(case when DL LIKE '24%' AND (GHDL LIKE '11%' OR GHDL ='131') then " + StatisticsFieldName + " else 0 end) as ɭ������δ���õ�," +
                 "sum(case when DL LIKE '25%' AND (GHDL LIKE '11%' OR GHDL ='131') then " + StatisticsFieldName + " else 0 end) as ɭ�����ӽ����õ�," +
                 "sum(case when DL LIKE '22%' AND (GHDL LIKE '11%' OR GHDL ='131') then " + StatisticsFieldName + " else 0 end) as ɭ�����Ӳݵ�," +
                 "sum(case when DL LIKE '21%' AND (GHDL LIKE '11%' OR GHDL ='131') then " + StatisticsFieldName + " else 0 end) as ɭ�����Ӹ���," +
                 "sum(case when DL LIKE '23%' AND (GHDL LIKE '11%' OR GHDL ='131') then " + StatisticsFieldName + " else 0 end) as ɭ����������" +
                 "  from " + tableName +
                 "  group by substr(shi,1,4)";
                pWorkspace.ExecuteSQL(SHISQL);
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
                                     "sum(case when DL LIKE '1%' then " + StatisticsFieldName + " else 0 end) as ��״�ֵ�," +
                                     "sum(case when (DL LIKE '11%' OR DL='131') then " + StatisticsFieldName + " else 0 end) as ��״ɭ��," +
                                     "sum(case when substr(dl,1,2) between '21'and '25' and GHDL LIKE '1%' then " + StatisticsFieldName + " else 0 end) as ����ϼ�," +
                                     "sum(case when DL LIKE '24%' AND GHDL LIKE '1%' then " + StatisticsFieldName + " else 0 end) as ����δ���õ�," +
                                     "sum(case when DL LIKE '25%' AND GHDL LIKE '1%' then " + StatisticsFieldName + " else 0 end) as ���佨���õ�," +
                                     "sum(case when DL LIKE '22%' AND GHDL LIKE '1%' then " + StatisticsFieldName + " else 0 end) as �������ݵ�," +
                                     "sum(case when DL LIKE '21%' AND GHDL LIKE '1%' then " + StatisticsFieldName + " else 0 end) as �������," +
                                     "sum(case when DL LIKE '23%' AND GHDL LIKE '1%' then " + StatisticsFieldName + " else 0 end) as ��������," +
                                     "sum(case when substr(dl,1,2) between '21'and '25'and (GHDL LIKE '11%' OR GHDL ='131') then " + StatisticsFieldName + " else 0 end) as ɭ�����Ӻϼ�," +
                                     "sum(case when DL LIKE '22%' AND (GHDL LIKE '11%' OR GHDL ='131') then " + StatisticsFieldName + " else 0 end) as ɭ�������ֵ�," +
                                     "sum(case when DL LIKE '24%' AND (GHDL LIKE '11%' OR GHDL ='131') then " + StatisticsFieldName + " else 0 end) as ɭ������δ���õ�," +
                                     "sum(case when DL LIKE '25%' AND (GHDL LIKE '11%' OR GHDL ='131') then " + StatisticsFieldName + " else 0 end) as ɭ�����ӽ����õ�," +
                                     "sum(case when DL LIKE '22%' AND (GHDL LIKE '11%' OR GHDL ='131') then " + StatisticsFieldName + " else 0 end) as ɭ�����Ӳݵ�," +
                                     "sum(case when DL LIKE '21%' AND (GHDL LIKE '11%' OR GHDL ='131') then " + StatisticsFieldName + " else 0 end) as ɭ�����Ӹ���," +
                                     "sum(case when DL LIKE '23%' AND (GHDL LIKE '11%' OR GHDL ='131') then " + StatisticsFieldName + " else 0 end) as ɭ����������" +
                                     "  from " + tableName +
                                     "  where lc <>' '"+
                                     "  group by lc";
                    pWorkspace.ExecuteSQL(CitySQL);
                }

                //��ѯ����ͳ������SQL ygc 2012-8-23
                string townsSQL = " insert into EightTable_LDSLMJGH  select " +
                                 "lc as ͳ�Ƶ�λ," +
                                 "sum(case when DL LIKE '1%' then " + StatisticsFieldName + " else 0 end) as ��״�ֵ�," +
                                 "sum(case when (DL LIKE '11%' OR DL='131') then " + StatisticsFieldName + " else 0 end) as ��״ɭ��," +
                                 "sum(case when substr(dl,1,2) between '21'and '25' and GHDL LIKE '1%' then " + StatisticsFieldName + " else 0 end) as ����ϼ�," +
                                 "sum(case when DL LIKE '24%' AND GHDL LIKE '1%' then " + StatisticsFieldName + " else 0 end) as ����δ���õ�," +
                                 "sum(case when DL LIKE '25%' AND GHDL LIKE '1%' then " + StatisticsFieldName + " else 0 end) as ���佨���õ�," +
                                 "sum(case when DL LIKE '22%' AND GHDL LIKE '1%' then " + StatisticsFieldName + " else 0 end) as �������ݵ�," +
                                 "sum(case when DL LIKE '21%' AND GHDL LIKE '1%' then " + StatisticsFieldName + " else 0 end) as �������," +
                                 "sum(case when DL LIKE '23%' AND GHDL LIKE '1%' then " + StatisticsFieldName + " else 0 end) as ��������," +
                                 "sum(case when substr(dl,1,2) between '21'and '25'and (GHDL LIKE '11%' OR GHDL ='131') then " + StatisticsFieldName + " else 0 end) as ɭ�����Ӻϼ�," +
                                 "sum(case when DL LIKE '22%' AND (GHDL LIKE '11%' OR GHDL ='131') then " + StatisticsFieldName + " else 0 end) as ɭ�������ֵ�," +
                                 "sum(case when DL LIKE '24%' AND (GHDL LIKE '11%' OR GHDL ='131') then " + StatisticsFieldName + " else 0 end) as ɭ������δ���õ�," +
                                 "sum(case when DL LIKE '25%' AND (GHDL LIKE '11%' OR GHDL ='131') then " + StatisticsFieldName + " else 0 end) as ɭ�����ӽ����õ�," +
                                 "sum(case when DL LIKE '22%' AND (GHDL LIKE '11%' OR GHDL ='131') then " + StatisticsFieldName + " else 0 end) as ɭ�����Ӳݵ�," +
                                 "sum(case when DL LIKE '21%' AND (GHDL LIKE '11%' OR GHDL ='131') then " + StatisticsFieldName + " else 0 end) as ɭ�����Ӹ���," +
                                 "sum(case when DL LIKE '23%' AND (GHDL LIKE '11%' OR GHDL ='131') then " + StatisticsFieldName + " else 0 end) as ɭ����������" +
                                 "  from " + tableName +
                                 "  where lc<> ' '"+
                                 "  group by lc";
                pWorkspace.ExecuteSQL(townsSQL);
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
                //��ѯ�ؼ�ͳ������SQL���
                //ygc 2012-8-22
                string CitySQL = "create table EightTable_LDJGXZ as select " +
                                 tableName + ".xian as ͳ�Ƶ�λ,substr(QI_YUAN,1,1) as ��Դ," +
                                 "sum(case when " + tableName + ".sen_lin_lb between '11' and '12'and " + tableName + ".dl<='114' then " + tableName + "." + StatisticsFieldName + " else 0 end) as �����ֺϼ�," +
                                 "sum (case when " + tableName + ".sen_lin_lb ='11' and " + tableName + ".dl<='114' then " + tableName + "." + StatisticsFieldName + "  else 0 end) as �ص㹫���ֺϼ�," +
                                 "sum(case when " + tableName + ".sen_lin_lb='11' and " + tableName + ".dl<='112' and (substr(" + tableName + ".lz,1,2)='11'or substr(" + tableName + ".lz,1,2)='12') then " + tableName + "." + StatisticsFieldName + " else 0 end ) as �ص㹫��С��," +
                                 "sum (case when " + tableName + ".sen_lin_lb='11' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='11' then " + tableName + "." + StatisticsFieldName + " else 0 end) as �ص㹫�������," +
                                 "sum(case when " + tableName + ".sen_lin_lb='11'and " + tableName + ".dl<='112' and substr(" + tableName + ".lz,1,2)='12' then " + tableName + "." + StatisticsFieldName + " else 0 end ) as �ص㹫��������," +
                                 "sum(case when " + tableName + ".sen_lin_lb='11'and (((substr(" + tableName + ".lz,1,2)='11'or substr(" + tableName + ".lz,1,2)='12') and " + tableName + ".dl between '113' and '114')) and (substr(" + tableName + ".lz,1,2)<>'11' or substr(" + tableName + ".lz,1,2)<>'12') then " + tableName + "." + StatisticsFieldName + "  else 0 end) as  �ص㹫������," +
                                 "sum(case when " + tableName + ".sen_lin_lb ='12' and " + tableName + ".dl<='114' then " + tableName + "." + StatisticsFieldName + "   end ) as һ�㹫��ϼ�," +
                                 "sum(case when " + tableName + ".sen_lin_lb='12' and " + tableName + ".dl<='112' and (substr(" + tableName + ".lz,1,2)='11'or substr(" + tableName + ".lz,1,2)='12') then " + tableName + "." + StatisticsFieldName + " else 0 end ) as һ�㹫��С��," +
                                 "sum(case when " + tableName + ".sen_lin_lb='12' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='11' then " + tableName + "." + StatisticsFieldName + " else 0 end) as һ�㹫�������," +
                                 "sum(case when " + tableName + ".sen_lin_lb='12'and " + tableName + ".dl<='112' and substr(" + tableName + ".lz,1,2)='12' then " + tableName + "." + StatisticsFieldName + " else 0 end ) as һ�㹫��������," +
                                 "sum(case when " + tableName + ".sen_lin_lb='12'and ((substr(" + tableName + ".lz,1,2)<>'11' and substr(" + tableName + ".lz,1,2)<>'12') or (((substr(" + tableName + ".lz,1,2)='11' or substr(" + tableName + ".lz,1,2)='12')) and " + tableName + ".dl between '113' and '114')) then " + tableName + "." + StatisticsFieldName + " else 0 end) as һ�㹫������," +
                                 "sum(case when " + tableName + ".sen_lin_lb between '21' and '22'and " + tableName + ".dl<='114' then " + tableName + "." + StatisticsFieldName + " else 0 end ) as ��Ʒ�ֺϼ�," +
                                 "sum(case when " + tableName + ".sen_lin_lb ='21' and " + tableName + ".dl<='114' then " + tableName + "." + StatisticsFieldName + " else 0 end) as �ص���Ʒ�ֺϼ�," +
                                 "sum(case when " + tableName + ".sen_lin_lb='21' and " + tableName + ".dl<='114' and (substr(" + tableName + ".lz,1,2) between '23' and '25') then " + tableName + "." + StatisticsFieldName + " else 0 end ) as �ص���Ʒ��С��," +
                                 "sum(case when " + tableName + ".sen_lin_lb='21' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='23' then " + tableName + "." + StatisticsFieldName + " else 0 end) as �ص��ò���," +
                                 "sum(case when " + tableName + ".sen_lin_lb='21' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='25'and " + tableName + ".dl='114' then " + tableName + "." + StatisticsFieldName + " else 0 end) as �ص㾭����," +
                                 "sum(case when " + tableName + ".sen_lin_lb='21' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='24' then " + tableName + "." + StatisticsFieldName + " else 0 end) as �ص�н̼��," +
                                 "sum(case when " + tableName + ".sen_lin_lb='21'and ((substr(" + tableName + ".lz,1,2)<>'23' and substr(" + tableName + ".lz,1,2)<>'24' and substr(" + tableName + ".lz,1,2)<>'25') or (substr(" + tableName + ".lz,1,2)='23' and " + tableName + ".dl>'112') or (substr(" + tableName + ".lz,1,2)='25' and " + tableName + ".dl<>'114')) then " + tableName + "." + StatisticsFieldName + " else 0 end) as �ص�����," +
                                 "sum(case when " + tableName + ".sen_lin_lb ='22' and " + tableName + ".dl<='114' then " + tableName + "." + StatisticsFieldName + "  else 0 end) as һ����Ʒ�ֺϼ�," +
                                 "sum(case when " + tableName + ".sen_lin_lb='22' and " + tableName + ".dl<='114' and (substr(" + tableName + ".lz,1,2) between '23' and '25') then " + tableName + "." + StatisticsFieldName + " else 0 end ) as һ����Ʒ��С��," +
                                 "sum(case when " + tableName + ".sen_lin_lb='22' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='23' then " + tableName + "." + StatisticsFieldName + " else 0 end) as һ���ò���," +
                                 "sum(case when " + tableName + ".sen_lin_lb='22' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='25'and " + tableName + ".dl='114' then " + tableName + "." + StatisticsFieldName + " else 0 end) as һ�㾭����," +
                                 "sum(case when " + tableName + ".sen_lin_lb='22' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='24' then " + tableName + "." + StatisticsFieldName + " else 0 end) as һ��н̼��," +
                                 "sum(case when " + tableName + ".sen_lin_lb='22'and ((substr(" + tableName + ".lz,1,2)<>'23' and substr(" + tableName + ".lz,1,2)<>'24' and substr(" + tableName + ".lz,1,2)<>'25') or (substr(" + tableName + ".lz,1,2)='23' and " + tableName + ".dl>'112') or (substr(" + tableName + ".lz,1,2)='25' and " + tableName + ".dl<>'114')) then " + tableName + "." + StatisticsFieldName + " else 0 end) as һ������" +
                                 " from " + tableName +
                                 " where substr(QI_YUAN,1,1)<>' '" +
                                 " group by xian,rollup(substr(QI_YUAN,1,1)) " +
                                 " order by substr(QI_YUAN,1,1) ";
                pWorkspace.ExecuteSQL(CitySQL);
                //��ѯ����ͳ������
                //ygc 2012-8-22
                string townsSQL = "insert  into EightTable_LDJGXZ  select " +
                                 tableName + ".xiang as ͳ�Ƶ�λ,substr(QI_YUAN,1,1) as ��Դ," +
                                 "sum(case when " + tableName + ".sen_lin_lb between '11' and '12'and " + tableName + ".dl<='114' then " + tableName + "." + StatisticsFieldName + " else 0 end) as �����ֺϼ�," +
                                 "sum (case when " + tableName + ".sen_lin_lb ='11' and " + tableName + ".dl<='114' then " + tableName + "." + StatisticsFieldName + "  else 0 end) as �ص㹫���ֺϼ�," +
                                 "sum(case when " + tableName + ".sen_lin_lb='11' and " + tableName + ".dl<='112' and (substr(" + tableName + ".lz,1,2)='11'or substr(" + tableName + ".lz,1,2)='12') then " + tableName + "." + StatisticsFieldName + " else 0 end ) as �ص㹫��С��," +
                                 "sum (case when " + tableName + ".sen_lin_lb='11' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='11' then " + tableName + "." + StatisticsFieldName + " else 0 end) as �ص㹫�������," +
                                 "sum(case when " + tableName + ".sen_lin_lb='11'and " + tableName + ".dl<='112' and substr(" + tableName + ".lz,1,2)='12' then " + tableName + "." + StatisticsFieldName + " else 0 end ) as �ص㹫��������," +
                                 "sum(case when " + tableName + ".sen_lin_lb='11'and (((substr(" + tableName + ".lz,1,2)='11'or substr(" + tableName + ".lz,1,2)='12') and " + tableName + ".dl between '113' and '114')) and (substr(" + tableName + ".lz,1,2)<>'11' or substr(" + tableName + ".lz,1,2)<>'12') then " + tableName + "." + StatisticsFieldName + "  else 0 end) as  �ص㹫������," +
                                 "sum(case when " + tableName + ".sen_lin_lb ='12' and " + tableName + ".dl<='114' then " + tableName + "." + StatisticsFieldName + "   end ) as һ�㹫��ϼ�," +
                                 "sum(case when " + tableName + ".sen_lin_lb='12' and " + tableName + ".dl<='112' and (substr(" + tableName + ".lz,1,2)='11'or substr(" + tableName + ".lz,1,2)='12') then " + tableName + "." + StatisticsFieldName + " else 0 end ) as һ�㹫��С��," +
                                 "sum(case when " + tableName + ".sen_lin_lb='12' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='11' then " + tableName + "." + StatisticsFieldName + " else 0 end) as һ�㹫�������," +
                                 "sum(case when " + tableName + ".sen_lin_lb='12'and " + tableName + ".dl<='112' and substr(" + tableName + ".lz,1,2)='12' then " + tableName + "." + StatisticsFieldName + " else 0 end ) as һ�㹫��������," +
                                 "sum(case when " + tableName + ".sen_lin_lb='12'and ((substr(" + tableName + ".lz,1,2)<>'11' and substr(" + tableName + ".lz,1,2)<>'12') or (((substr(" + tableName + ".lz,1,2)='11' or substr(" + tableName + ".lz,1,2)='12')) and " + tableName + ".dl between '113' and '114')) then " + tableName + "." + StatisticsFieldName + " else 0 end) as һ�㹫������," +
                                 "sum(case when " + tableName + ".sen_lin_lb between '21' and '22'and " + tableName + ".dl<='114' then " + tableName + "." + StatisticsFieldName + " else 0 end ) as ��Ʒ�ֺϼ�," +
                                 "sum(case when " + tableName + ".sen_lin_lb ='21' and " + tableName + ".dl<='114' then " + tableName + "." + StatisticsFieldName + " else 0 end) as �ص���Ʒ�ֺϼ�," +
                                 "sum(case when " + tableName + ".sen_lin_lb='21' and " + tableName + ".dl<='114' and (substr(" + tableName + ".lz,1,2) between '23' and '25') then " + tableName + "." + StatisticsFieldName + " else 0 end ) as �ص���Ʒ��С��," +
                                 "sum(case when " + tableName + ".sen_lin_lb='21' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='23' then " + tableName + "." + StatisticsFieldName + " else 0 end) as �ص��ò���," +
                                 "sum(case when " + tableName + ".sen_lin_lb='21' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='25'and " + tableName + ".dl='114' then " + tableName + "." + StatisticsFieldName + " else 0 end) as �ص㾭����," +
                                 "sum(case when " + tableName + ".sen_lin_lb='21' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='24' then " + tableName + "." + StatisticsFieldName + " else 0 end) as �ص�н̼��," +
                                 "sum(case when " + tableName + ".sen_lin_lb='21'and ((substr(" + tableName + ".lz,1,2)<>'23' and substr(" + tableName + ".lz,1,2)<>'24' and substr(" + tableName + ".lz,1,2)<>'25') or (substr(" + tableName + ".lz,1,2)='23' and " + tableName + ".dl>'112') or (substr(" + tableName + ".lz,1,2)='25' and " + tableName + ".dl<>'114')) then " + tableName + "." + StatisticsFieldName + " else 0 end) as �ص�����," +
                                 "sum(case when " + tableName + ".sen_lin_lb ='22' and " + tableName + ".dl<='114' then " + tableName + "." + StatisticsFieldName + "  else 0 end) as һ����Ʒ�ֺϼ�," +
                                 "sum(case when " + tableName + ".sen_lin_lb='22' and " + tableName + ".dl<='114' and (substr(" + tableName + ".lz,1,2) between '23' and '25') then " + tableName + "." + StatisticsFieldName + " else 0 end ) as һ����Ʒ��С��," +
                                 "sum(case when " + tableName + ".sen_lin_lb='22' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='23' then " + tableName + "." + StatisticsFieldName + " else 0 end) as һ���ò���," +
                                 "sum(case when " + tableName + ".sen_lin_lb='22' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='25'and " + tableName + ".dl='114' then " + tableName + "." + StatisticsFieldName + " else 0 end) as һ�㾭����," +
                                 "sum(case when " + tableName + ".sen_lin_lb='22' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='24' then " + tableName + "." + StatisticsFieldName + " else 0 end) as һ��н̼��," +
                                 "sum(case when " + tableName + ".sen_lin_lb='22'and ((substr(" + tableName + ".lz,1,2)<>'23' and substr(" + tableName + ".lz,1,2)<>'24' and substr(" + tableName + ".lz,1,2)<>'25') or (substr(" + tableName + ".lz,1,2)='23' and " + tableName + ".dl>'112') or (substr(" + tableName + ".lz,1,2)='25' and " + tableName + ".dl<>'114')) then " + tableName + "." + StatisticsFieldName + " else 0 end) as һ������" +
                                 " from " + tableName +
                                 " where  substr(QI_YUAN,1,1)<>' '" +
                                 " group by xiang,rollup(substr(QI_YUAN,1,1)) " +
                                 " order by substr(QI_YUAN,1,1)";
                pWorkspace.ExecuteSQL(townsSQL);
                //��ѯ�м����� ygc 2012-10-22
                string SHISQL = "insert  into EightTable_LDJGXZ  select substr(" +
                 tableName + ".shi,1,4) as ͳ�Ƶ�λ,substr(QI_YUAN,1,1) as ��Դ," +
                 "sum(case when " + tableName + ".sen_lin_lb between '11' and '12'and " + tableName + ".dl<='114' then " + tableName + "." + StatisticsFieldName + " else 0 end) as �����ֺϼ�," +
                 "sum (case when " + tableName + ".sen_lin_lb ='11' and " + tableName + ".dl<='114' then " + tableName + "." + StatisticsFieldName + "  else 0 end) as �ص㹫���ֺϼ�," +
                 "sum(case when " + tableName + ".sen_lin_lb='11' and " + tableName + ".dl<='112' and (substr(" + tableName + ".lz,1,2)='11'or substr(" + tableName + ".lz,1,2)='12') then " + tableName + "." + StatisticsFieldName + " else 0 end ) as �ص㹫��С��," +
                 "sum (case when " + tableName + ".sen_lin_lb='11' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='11' then " + tableName + "." + StatisticsFieldName + " else 0 end) as �ص㹫�������," +
                 "sum(case when " + tableName + ".sen_lin_lb='11'and " + tableName + ".dl<='112' and substr(" + tableName + ".lz,1,2)='12' then " + tableName + "." + StatisticsFieldName + " else 0 end ) as �ص㹫��������," +
                 "sum(case when " + tableName + ".sen_lin_lb='11'and (((substr(" + tableName + ".lz,1,2)='11'or substr(" + tableName + ".lz,1,2)='12') and " + tableName + ".dl between '113' and '114')) and (substr(" + tableName + ".lz,1,2)<>'11' or substr(" + tableName + ".lz,1,2)<>'12') then " + tableName + "." + StatisticsFieldName + "  else 0 end) as  �ص㹫������," +
                 "sum(case when " + tableName + ".sen_lin_lb ='12' and " + tableName + ".dl<='114' then " + tableName + "." + StatisticsFieldName + "   end ) as һ�㹫��ϼ�," +
                 "sum(case when " + tableName + ".sen_lin_lb='12' and " + tableName + ".dl<='112' and (substr(" + tableName + ".lz,1,2)='11'or substr(" + tableName + ".lz,1,2)='12') then " + tableName + "." + StatisticsFieldName + " else 0 end ) as һ�㹫��С��," +
                 "sum(case when " + tableName + ".sen_lin_lb='12' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='11' then " + tableName + "." + StatisticsFieldName + " else 0 end) as һ�㹫�������," +
                 "sum(case when " + tableName + ".sen_lin_lb='12'and " + tableName + ".dl<='112' and substr(" + tableName + ".lz,1,2)='12' then " + tableName + "." + StatisticsFieldName + " else 0 end ) as һ�㹫��������," +
                 "sum(case when " + tableName + ".sen_lin_lb='12'and ((substr(" + tableName + ".lz,1,2)<>'11' and substr(" + tableName + ".lz,1,2)<>'12') or (((substr(" + tableName + ".lz,1,2)='11' or substr(" + tableName + ".lz,1,2)='12')) and " + tableName + ".dl between '113' and '114')) then " + tableName + "." + StatisticsFieldName + " else 0 end) as һ�㹫������," +
                 "sum(case when " + tableName + ".sen_lin_lb between '21' and '22'and " + tableName + ".dl<='114' then " + tableName + "." + StatisticsFieldName + " else 0 end ) as ��Ʒ�ֺϼ�," +
                 "sum(case when " + tableName + ".sen_lin_lb ='21' and " + tableName + ".dl<='114' then " + tableName + "." + StatisticsFieldName + " else 0 end) as �ص���Ʒ�ֺϼ�," +
                 "sum(case when " + tableName + ".sen_lin_lb='21' and " + tableName + ".dl<='114' and (substr(" + tableName + ".lz,1,2) between '23' and '25') then " + tableName + "." + StatisticsFieldName + " else 0 end ) as �ص���Ʒ��С��," +
                 "sum(case when " + tableName + ".sen_lin_lb='21' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='23' then " + tableName + "." + StatisticsFieldName + " else 0 end) as �ص��ò���," +
                 "sum(case when " + tableName + ".sen_lin_lb='21' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='25'and " + tableName + ".dl='114' then " + tableName + "." + StatisticsFieldName + " else 0 end) as �ص㾭����," +
                 "sum(case when " + tableName + ".sen_lin_lb='21' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='24' then " + tableName + "." + StatisticsFieldName + " else 0 end) as �ص�н̼��," +
                 "sum(case when " + tableName + ".sen_lin_lb='21'and ((substr(" + tableName + ".lz,1,2)<>'23' and substr(" + tableName + ".lz,1,2)<>'24' and substr(" + tableName + ".lz,1,2)<>'25') or (substr(" + tableName + ".lz,1,2)='23' and " + tableName + ".dl>'112') or (substr(" + tableName + ".lz,1,2)='25' and " + tableName + ".dl<>'114')) then " + tableName + "." + StatisticsFieldName + " else 0 end) as �ص�����," +
                 "sum(case when " + tableName + ".sen_lin_lb ='22' and " + tableName + ".dl<='114' then " + tableName + "." + StatisticsFieldName + "  else 0 end) as һ����Ʒ�ֺϼ�," +
                 "sum(case when " + tableName + ".sen_lin_lb='22' and " + tableName + ".dl<='114' and (substr(" + tableName + ".lz,1,2) between '23' and '25') then " + tableName + "." + StatisticsFieldName + " else 0 end ) as һ����Ʒ��С��," +
                 "sum(case when " + tableName + ".sen_lin_lb='22' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='23' then " + tableName + "." + StatisticsFieldName + " else 0 end) as һ���ò���," +
                 "sum(case when " + tableName + ".sen_lin_lb='22' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='25'and " + tableName + ".dl='114' then " + tableName + "." + StatisticsFieldName + " else 0 end) as һ�㾭����," +
                 "sum(case when " + tableName + ".sen_lin_lb='22' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='24' then " + tableName + "." + StatisticsFieldName + " else 0 end) as һ��н̼��," +
                 "sum(case when " + tableName + ".sen_lin_lb='22'and ((substr(" + tableName + ".lz,1,2)<>'23' and substr(" + tableName + ".lz,1,2)<>'24' and substr(" + tableName + ".lz,1,2)<>'25') or (substr(" + tableName + ".lz,1,2)='23' and " + tableName + ".dl>'112') or (substr(" + tableName + ".lz,1,2)='25' and " + tableName + ".dl<>'114')) then " + tableName + "." + StatisticsFieldName + " else 0 end) as һ������" +
                 " from " + tableName +
                 " where  substr(QI_YUAN,1,1)<>' '" +
                 " group by substr(shi,1,4),rollup(substr(QI_YUAN,1,1)) " +
                 " order by substr(QI_YUAN,1,1)";
                pWorkspace.ExecuteSQL(SHISQL);
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
                                     "sum(case when " + tableName + ".sen_lin_lb between '11' and '12'and " + tableName + ".dl<='114' then " + tableName + "." + StatisticsFieldName + " else 0 end) as �����ֺϼ�," +
                                     "sum (case when " + tableName + ".sen_lin_lb ='11' and " + tableName + ".dl<='114' then " + tableName + "." + StatisticsFieldName + "  else 0 end) as �ص㹫���ֺϼ�," +
                                     "sum(case when " + tableName + ".sen_lin_lb='11' and " + tableName + ".dl<='112' and (substr(" + tableName + ".lz,1,2)='11'or substr(" + tableName + ".lz,1,2)='12') then " + tableName + "." + StatisticsFieldName + " else 0 end ) as �ص㹫��С��," +
                                     "sum (case when " + tableName + ".sen_lin_lb='11' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='11' then " + tableName + "." + StatisticsFieldName + " else 0 end) as �ص㹫�������," +
                                     "sum(case when " + tableName + ".sen_lin_lb='11'and " + tableName + ".dl<='112' and substr(" + tableName + ".lz,1,2)='12' then " + tableName + "." + StatisticsFieldName + " else 0 end ) as �ص㹫��������," +
                                     "sum(case when " + tableName + ".sen_lin_lb='11'and (((substr(" + tableName + ".lz,1,2)='11'or substr(" + tableName + ".lz,1,2)='12') and " + tableName + ".dl between '113' and '114')) and (substr(" + tableName + ".lz,1,2)<>'11' or substr(" + tableName + ".lz,1,2)<>'12') then " + tableName + "." + StatisticsFieldName + "  else 0 end) as  �ص㹫������," +
                                     "sum(case when " + tableName + ".sen_lin_lb ='12' and " + tableName + ".dl<='114' then " + tableName + "." + StatisticsFieldName + "   end ) as һ�㹫��ϼ�," +
                                     "sum(case when " + tableName + ".sen_lin_lb='12' and " + tableName + ".dl<='112' and (substr(" + tableName + ".lz,1,2)='11'or substr(" + tableName + ".lz,1,2)='12') then " + tableName + "." + StatisticsFieldName + " else 0 end ) as һ�㹫��С��," +
                                     "sum(case when " + tableName + ".sen_lin_lb='12' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='11' then " + tableName + "." + StatisticsFieldName + " else 0 end) as һ�㹫�������," +
                                     "sum(case when " + tableName + ".sen_lin_lb='12'and " + tableName + ".dl<='112' and substr(" + tableName + ".lz,1,2)='12' then " + tableName + "." + StatisticsFieldName + " else 0 end ) as һ�㹫��������," +
                                     "sum(case when " + tableName + ".sen_lin_lb='12'and ((substr(" + tableName + ".lz,1,2)<>'11' and substr(" + tableName + ".lz,1,2)<>'12') or (((substr(" + tableName + ".lz,1,2)='11' or substr(" + tableName + ".lz,1,2)='12')) and " + tableName + ".dl between '113' and '114')) then " + tableName + "." + StatisticsFieldName + " else 0 end) as һ�㹫������," +
                                     "sum(case when " + tableName + ".sen_lin_lb between '21' and '22'and " + tableName + ".dl<='114' then " + tableName + "." + StatisticsFieldName + " else 0 end ) as ��Ʒ�ֺϼ�," +
                                     "sum(case when " + tableName + ".sen_lin_lb ='21' and " + tableName + ".dl<='114' then " + tableName + "." + StatisticsFieldName + " else 0 end) as �ص���Ʒ�ֺϼ�," +
                                     "sum(case when " + tableName + ".sen_lin_lb='21' and " + tableName + ".dl<='114' and (substr(" + tableName + ".lz,1,2) between '23' and '25') then " + tableName + "." + StatisticsFieldName + " else 0 end ) as �ص���Ʒ��С��," +
                                     "sum(case when " + tableName + ".sen_lin_lb='21' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='23' then " + tableName + "." + StatisticsFieldName + " else 0 end) as �ص��ò���," +
                                     "sum(case when " + tableName + ".sen_lin_lb='21' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='25'and " + tableName + ".dl='114' then " + tableName + "." + StatisticsFieldName + " else 0 end) as �ص㾭����," +
                                     "sum(case when " + tableName + ".sen_lin_lb='21' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='24' then " + tableName + "." + StatisticsFieldName + " else 0 end) as �ص�н̼��," +
                                     "sum(case when " + tableName + ".sen_lin_lb='21'and ((substr(" + tableName + ".lz,1,2)<>'23' and substr(" + tableName + ".lz,1,2)<>'24' and substr(" + tableName + ".lz,1,2)<>'25') or (substr(" + tableName + ".lz,1,2)='23' and " + tableName + ".dl>'112') or (substr(" + tableName + ".lz,1,2)='25' and " + tableName + ".dl<>'114')) then " + tableName + "." + StatisticsFieldName + " else 0 end) as �ص�����," +
                                     "sum(case when " + tableName + ".sen_lin_lb ='22' and " + tableName + ".dl<='114' then " + tableName + "." + StatisticsFieldName + "  else 0 end) as һ����Ʒ�ֺϼ�," +
                                     "sum(case when " + tableName + ".sen_lin_lb='22' and " + tableName + ".dl<='114' and (substr(" + tableName + ".lz,1,2) between '23' and '25') then " + tableName + "." + StatisticsFieldName + " else 0 end ) as һ����Ʒ��С��," +
                                     "sum(case when " + tableName + ".sen_lin_lb='22' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='23' then " + tableName + "." + StatisticsFieldName + " else 0 end) as һ���ò���," +
                                     "sum(case when " + tableName + ".sen_lin_lb='22' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='25'and " + tableName + ".dl='114' then " + tableName + "." + StatisticsFieldName + " else 0 end) as һ�㾭����," +
                                     "sum(case when " + tableName + ".sen_lin_lb='22' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='24' then " + tableName + "." + StatisticsFieldName + " else 0 end) as һ��н̼��," +
                                     "sum(case when " + tableName + ".sen_lin_lb='22'and ((substr(" + tableName + ".lz,1,2)<>'23' and substr(" + tableName + ".lz,1,2)<>'24' and substr(" + tableName + ".lz,1,2)<>'25') or (substr(" + tableName + ".lz,1,2)='23' and " + tableName + ".dl>'112') or (substr(" + tableName + ".lz,1,2)='25' and " + tableName + ".dl<>'114')) then " + tableName + "." + StatisticsFieldName + " else 0 end) as һ������" +
                                     " from " + tableName +
                                     " where substr(QI_YUAN,1,1)<>' ' and lc<>' '" +
                                     " group by lc,rollup(substr(QI_YUAN,1,1)) " +
                                     " order by substr(QI_YUAN,1,1) ";
                    pWorkspace.ExecuteSQL(CitySQL);
 
                }
                //��ѯ����ͳ������
                //ygc 2012-8-22
                string townsSQL = "insert  into EightTable_LDJGXZ  select " +
                                 tableName + ".lc as ͳ�Ƶ�λ,substr(QI_YUAN,1,1) as ��Դ," +
                                 "sum(case when " + tableName + ".sen_lin_lb between '11' and '12'and " + tableName + ".dl<='114' then " + tableName + "." + StatisticsFieldName + " else 0 end) as �����ֺϼ�," +
                                 "sum (case when " + tableName + ".sen_lin_lb ='11' and " + tableName + ".dl<='114' then " + tableName + "." + StatisticsFieldName + "  else 0 end) as �ص㹫���ֺϼ�," +
                                 "sum(case when " + tableName + ".sen_lin_lb='11' and " + tableName + ".dl<='112' and (substr(" + tableName + ".lz,1,2)='11'or substr(" + tableName + ".lz,1,2)='12') then " + tableName + "." + StatisticsFieldName + " else 0 end ) as �ص㹫��С��," +
                                 "sum (case when " + tableName + ".sen_lin_lb='11' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='11' then " + tableName + "." + StatisticsFieldName + " else 0 end) as �ص㹫�������," +
                                 "sum(case when " + tableName + ".sen_lin_lb='11'and " + tableName + ".dl<='112' and substr(" + tableName + ".lz,1,2)='12' then " + tableName + "." + StatisticsFieldName + " else 0 end ) as �ص㹫��������," +
                                 "sum(case when " + tableName + ".sen_lin_lb='11'and (((substr(" + tableName + ".lz,1,2)='11'or substr(" + tableName + ".lz,1,2)='12') and " + tableName + ".dl between '113' and '114')) and (substr(" + tableName + ".lz,1,2)<>'11' or substr(" + tableName + ".lz,1,2)<>'12') then " + tableName + "." + StatisticsFieldName + "  else 0 end) as  �ص㹫������," +
                                 "sum(case when " + tableName + ".sen_lin_lb ='12' and " + tableName + ".dl<='114' then " + tableName + "." + StatisticsFieldName + "   end ) as һ�㹫��ϼ�," +
                                 "sum(case when " + tableName + ".sen_lin_lb='12' and " + tableName + ".dl<='112' and (substr(" + tableName + ".lz,1,2)='11'or substr(" + tableName + ".lz,1,2)='12') then " + tableName + "." + StatisticsFieldName + " else 0 end ) as һ�㹫��С��," +
                                 "sum(case when " + tableName + ".sen_lin_lb='12' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='11' then " + tableName + "." + StatisticsFieldName + " else 0 end) as һ�㹫�������," +
                                 "sum(case when " + tableName + ".sen_lin_lb='12'and " + tableName + ".dl<='112' and substr(" + tableName + ".lz,1,2)='12' then " + tableName + "." + StatisticsFieldName + " else 0 end ) as һ�㹫��������," +
                                 "sum(case when " + tableName + ".sen_lin_lb='12'and ((substr(" + tableName + ".lz,1,2)<>'11' and substr(" + tableName + ".lz,1,2)<>'12') or (((substr(" + tableName + ".lz,1,2)='11' or substr(" + tableName + ".lz,1,2)='12')) and " + tableName + ".dl between '113' and '114')) then " + tableName + "." + StatisticsFieldName + " else 0 end) as һ�㹫������," +
                                 "sum(case when " + tableName + ".sen_lin_lb between '21' and '22'and " + tableName + ".dl<='114' then " + tableName + "." + StatisticsFieldName + " else 0 end ) as ��Ʒ�ֺϼ�," +
                                 "sum(case when " + tableName + ".sen_lin_lb ='21' and " + tableName + ".dl<='114' then " + tableName + "." + StatisticsFieldName + " else 0 end) as �ص���Ʒ�ֺϼ�," +
                                 "sum(case when " + tableName + ".sen_lin_lb='21' and " + tableName + ".dl<='114' and (substr(" + tableName + ".lz,1,2) between '23' and '25') then " + tableName + "." + StatisticsFieldName + " else 0 end ) as �ص���Ʒ��С��," +
                                 "sum(case when " + tableName + ".sen_lin_lb='21' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='23' then " + tableName + "." + StatisticsFieldName + " else 0 end) as �ص��ò���," +
                                 "sum(case when " + tableName + ".sen_lin_lb='21' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='25'and " + tableName + ".dl='114' then " + tableName + "." + StatisticsFieldName + " else 0 end) as �ص㾭����," +
                                 "sum(case when " + tableName + ".sen_lin_lb='21' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='24' then " + tableName + "." + StatisticsFieldName + " else 0 end) as �ص�н̼��," +
                                 "sum(case when " + tableName + ".sen_lin_lb='21'and ((substr(" + tableName + ".lz,1,2)<>'23' and substr(" + tableName + ".lz,1,2)<>'24' and substr(" + tableName + ".lz,1,2)<>'25') or (substr(" + tableName + ".lz,1,2)='23' and " + tableName + ".dl>'112') or (substr(" + tableName + ".lz,1,2)='25' and " + tableName + ".dl<>'114')) then " + tableName + "." + StatisticsFieldName + " else 0 end) as �ص�����," +
                                 "sum(case when " + tableName + ".sen_lin_lb ='22' and " + tableName + ".dl<='114' then " + tableName + "." + StatisticsFieldName + "  else 0 end) as һ����Ʒ�ֺϼ�," +
                                 "sum(case when " + tableName + ".sen_lin_lb='22' and " + tableName + ".dl<='114' and (substr(" + tableName + ".lz,1,2) between '23' and '25') then " + tableName + "." + StatisticsFieldName + " else 0 end ) as һ����Ʒ��С��," +
                                 "sum(case when " + tableName + ".sen_lin_lb='22' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='23' then " + tableName + "." + StatisticsFieldName + " else 0 end) as һ���ò���," +
                                 "sum(case when " + tableName + ".sen_lin_lb='22' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='25'and " + tableName + ".dl='114' then " + tableName + "." + StatisticsFieldName + " else 0 end) as һ�㾭����," +
                                 "sum(case when " + tableName + ".sen_lin_lb='22' and " + tableName + ".dl<='112'and substr(" + tableName + ".lz,1,2)='24' then " + tableName + "." + StatisticsFieldName + " else 0 end) as һ��н̼��," +
                                 "sum(case when " + tableName + ".sen_lin_lb='22'and ((substr(" + tableName + ".lz,1,2)<>'23' and substr(" + tableName + ".lz,1,2)<>'24' and substr(" + tableName + ".lz,1,2)<>'25') or (substr(" + tableName + ".lz,1,2)='23' and " + tableName + ".dl>'112') or (substr(" + tableName + ".lz,1,2)='25' and " + tableName + ".dl<>'114')) then " + tableName + "." + StatisticsFieldName + " else 0 end) as һ������" +
                                 " from " + tableName +
                                 " where  substr(QI_YUAN,1,1)<>' ' and lc<>' '" +
                                 " group by lc,rollup(substr(QI_YUAN,1,1)) " +
                                 " order by substr(QI_YUAN,1,1)";
                pWorkspace.ExecuteSQL(townsSQL);
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
                //ͨ��SQL������ؼ�ͳ������ ygc 2012-8-23
                string CitySQL = "create table EightTable_GJGYHDJ as select " +
                                 "xian as ͳ�Ƶ�λ,substr(QI_YUAN,1,1) as ��Դ," +
                                 "sum(case when BHDJ between '1' and '3' and dl<='112' then " + StatisticsFieldName + " else 0 end) as �ϼ�," +
                                 "sum(case when BHDJ between '1' and '3' and substr(lz,1,2)='11' and dl<='112' then " + StatisticsFieldName + " else 0 end) as �����ֺϼ�," +
                                 "sum(case when BHDJ between '1' and '3' and substr(lz,1,2)='12' and dl<='112' then " + StatisticsFieldName + " else 0 end) as �����ֺϼ�," +
                                 "sum(case when BHDJ between '1' and '3' and ((substr(lz,1,2)<>'11' and substr(lz,1,2)<>'12') or ((substr(lz,1,2)='11' or substr(lz,1,2)='12') and dl>'112')) then " + StatisticsFieldName + " else 0 end) as �����ϼ�," +
                                 "sum(case when BHDJ ='1' and dl<='112' then " + StatisticsFieldName + " else 0 end) as һ��С��," +
                                 "sum(case when BHDJ='1' and substr(lz,1,2)='11' and dl<='112' then " + StatisticsFieldName + " else 0 end ) as һ������," +
                                 "sum(case when BHDJ='1' and substr(lz,1,2)='12' and dl<='112' then " + StatisticsFieldName + " else 0 end) as һ������," +
                                 "sum(case when BHDJ='1' and ((substr(lz,1,2)<>'11' and substr(lz,1,2)<>'12') or ((substr(lz,1,2)='11' or substr(lz,1,2)='12') and dl>'112')) then " + StatisticsFieldName + " else 0 end) as һ������," +
                                 "sum(case when BHDJ ='2' and dl<='112' then " + StatisticsFieldName + " else 0 end) as ����С��," +
                                 "sum(case when BHDJ='2' and substr(lz,1,2)='11' and dl<='112' then " + StatisticsFieldName + " else 0 end) as ��������," +
                                 "sum(case when BHDJ='2' and substr(lz,1,2)='12' and dl<='112' then " + StatisticsFieldName + " else 0 end ) as ��������," +
                                 "sum(case when BHDJ='2' and ((substr(lz,1,2)<>'11' and substr(lz,1,2)<>'12') or ((substr(lz,1,2)='11' or substr(lz,1,2)='12') and dl>'112')) then " + StatisticsFieldName + " else 0 end) as ��������," +
                                 "sum(case when BHDJ ='3' and dl<='112' then " + StatisticsFieldName + " else 0 end) as ����С��," +
                                 "sum(case when BHDJ='3' and substr(lz,1,2)='11' and dl<='112' then " + StatisticsFieldName + " else 0 end) as ��������," +
                                 "sum(case when BHDJ='3' and substr(lz,1,2)='12' and dl<='112'then " + StatisticsFieldName + " else 0 end) as ��������," +
                                 "sum(case when BHDJ='3' and ((substr(lz,1,2)<>'11' and substr(lz,1,2)<>'12') or ((substr(lz,1,2)='11' or substr(lz,1,2)='12') and dl>'112')) then " + StatisticsFieldName + " else 0 end) as  ��������" +
                                 "  from " + tableName +
                                 "  where substr(QI_YUAN,1,1)<>' ' and QI_YUAN is not null and sq='1'" +
                                 "  group by xian,rollup(substr(QI_YUAN,1,1))" +
                                 "  order by substr(QI_YUAN,1,1) ";
                pWorkspace.ExecuteSQL(CitySQL);
                //����SQL����ѯͳ���������� ygc 2012-8-23
                string townsSQL = "insert into EightTable_GJGYHDJ select " +
                                 "xiang as ͳ�Ƶ�λ,substr(QI_YUAN,1,1) as ��Դ," +
                                 "sum(case when BHDJ between '1' and '3' and dl<='112' then " + StatisticsFieldName + " else 0 end) as �ϼ�," +
                                 "sum(case when BHDJ between '1' and '3' and substr(lz,1,2)='11' and dl<='112' then " + StatisticsFieldName + " else 0 end) as �����ֺϼ�," +
                                 "sum(case when BHDJ between '1' and '3' and substr(lz,1,2)='12' and dl<='112' then " + StatisticsFieldName + " else 0 end) as �����ֺϼ�," +
                                 "sum(case when BHDJ between '1' and '3' and ((substr(lz,1,2)<>'11' and substr(lz,1,2)<>'12') or ((substr(lz,1,2)='11' or substr(lz,1,2)='12') and dl>'112')) then " + StatisticsFieldName + " else 0 end) as �����ϼ�," +
                                 "sum(case when BHDJ ='1' and dl<='112' then " + StatisticsFieldName + " else 0 end) as һ��С��," +
                                 "sum(case when BHDJ='1' and substr(lz,1,2)='11' and dl<='112' then " + StatisticsFieldName + " else 0 end ) as һ������," +
                                 "sum(case when BHDJ='1' and substr(lz,1,2)='12' and dl<='112' then " + StatisticsFieldName + " else 0 end) as һ������," +
                                 "sum(case when BHDJ='1' and ((substr(lz,1,2)<>'11' and substr(lz,1,2)<>'12') or ((substr(lz,1,2)='11' or substr(lz,1,2)='12') and dl>'112')) then " + StatisticsFieldName + " else 0 end) as һ������," +
                                 "sum(case when  BHDJ ='2' and dl<='112' then " + StatisticsFieldName + " else 0 end) as ����С��," +
                                 "sum(case when BHDJ='2' and substr(lz,1,2)='11' and dl<='112' then " + StatisticsFieldName + " else 0 end) as ��������," +
                                 "sum(case when BHDJ='2' and substr(lz,1,2)='12' and dl<='112' then " + StatisticsFieldName + " else 0 end ) as ��������," +
                                 "sum(case when BHDJ='2' and ((substr(lz,1,2)<>'11' and substr(lz,1,2)<>'12') or ((substr(lz,1,2)='11' or substr(lz,1,2)='12') and dl>'112')) then " + StatisticsFieldName + " else 0 end) as ��������," +
                                 "sum(case when BHDJ ='3' and dl<='112' then " + StatisticsFieldName + " else 0 end) as ����С��," +
                                 "sum(case when BHDJ='3' and substr(lz,1,2)='11' and dl<='112' then " + StatisticsFieldName + " else 0 end) as ��������," +
                                 "sum(case when BHDJ='3' and substr(lz,1,2)='12' and dl<='112'then " + StatisticsFieldName + " else 0 end) as ��������," +
                                 "sum(case when BHDJ='3' and ((substr(lz,1,2)<>'11' and substr(lz,1,2)<>'12') or ((substr(lz,1,2)='11' or substr(lz,1,2)='12') and dl>'112')) then " + StatisticsFieldName + " else 0 end) as  ��������" +
                                 "  from " + tableName +
                                 "  where substr(QI_YUAN,1,1)<>' ' and QI_YUAN is not null and sq='1'" +
                                 "  group by xiang,rollup(substr(QI_YUAN,1,1))" +
                                 "  order by substr(QI_YUAN,1,1) ";
                pWorkspace.ExecuteSQL(townsSQL);
                //��ȡ�м����� ygc 2012-10-22
                string SHISQL = "insert into EightTable_GJGYHDJ select " +
                 "substr(shi,1,4) as ͳ�Ƶ�λ,substr(QI_YUAN,1,1) as ��Դ," +
                 "sum(case when BHDJ between '1' and '3' and dl<='112' then " + StatisticsFieldName + " else 0 end) as �ϼ�," +
                 "sum(case when BHDJ between '1' and '3' and substr(lz,1,2)='11' and dl<='112' then " + StatisticsFieldName + " else 0 end) as �����ֺϼ�," +
                 "sum(case when BHDJ between '1' and '3' and substr(lz,1,2)='12' and dl<='112' then " + StatisticsFieldName + " else 0 end) as �����ֺϼ�," +
                 "sum(case when BHDJ between '1' and '3' and ((substr(lz,1,2)<>'11' and substr(lz,1,2)<>'12') or ((substr(lz,1,2)='11' or substr(lz,1,2)='12') and dl>'112')) then " + StatisticsFieldName + " else 0 end) as �����ϼ�," +
                 "sum(case when BHDJ ='1' and dl<='112' then " + StatisticsFieldName + " else 0 end) as һ��С��," +
                 "sum(case when BHDJ='1' and substr(lz,1,2)='11' and dl<='112' then " + StatisticsFieldName + " else 0 end ) as һ������," +
                 "sum(case when BHDJ='1' and substr(lz,1,2)='12' and dl<='112' then " + StatisticsFieldName + " else 0 end) as һ������," +
                 "sum(case when BHDJ='1' and ((substr(lz,1,2)<>'11' and substr(lz,1,2)<>'12') or ((substr(lz,1,2)='11' or substr(lz,1,2)='12') and dl>'112')) then " + StatisticsFieldName + " else 0 end) as һ������," +
                 "sum(case when  BHDJ ='2' and dl<='112' then " + StatisticsFieldName + " else 0 end) as ����С��," +
                 "sum(case when BHDJ='2' and substr(lz,1,2)='11' and dl<='112' then " + StatisticsFieldName + " else 0 end) as ��������," +
                 "sum(case when BHDJ='2' and substr(lz,1,2)='12' and dl<='112' then " + StatisticsFieldName + " else 0 end ) as ��������," +
                 "sum(case when BHDJ='2' and ((substr(lz,1,2)<>'11' and substr(lz,1,2)<>'12') or ((substr(lz,1,2)='11' or substr(lz,1,2)='12') and dl>'112')) then " + StatisticsFieldName + " else 0 end) as ��������," +
                 "sum(case when BHDJ ='3' and dl<='112' then " + StatisticsFieldName + " else 0 end) as ����С��," +
                 "sum(case when BHDJ='3' and substr(lz,1,2)='11' and dl<='112' then " + StatisticsFieldName + " else 0 end) as ��������," +
                 "sum(case when BHDJ='3' and substr(lz,1,2)='12' and dl<='112'then " + StatisticsFieldName + " else 0 end) as ��������," +
                 "sum(case when BHDJ='3' and ((substr(lz,1,2)<>'11' and substr(lz,1,2)<>'12') or ((substr(lz,1,2)='11' or substr(lz,1,2)='12') and dl>'112')) then " + StatisticsFieldName + " else 0 end) as  ��������" +
                 "  from " + tableName +
                 "  where substr(QI_YUAN,1,1)<>' ' and QI_YUAN is not null and sq='1'" +
                 "  group by substr(shi,1,4),rollup(substr(QI_YUAN,1,1))" +
                 "  order by substr(QI_YUAN,1,1) ";
                pWorkspace.ExecuteSQL(SHISQL); 
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
                    //ͨ��SQL������ؼ�ͳ������ ygc 2012-8-23
                    string CitySQL = "create table EightTable_GJGYHDJ as select " +
                                     "lc as ͳ�Ƶ�λ,substr(QI_YUAN,1,1) as ��Դ," +
                                     "sum(case when BHDJ between '1' and '3' and dl<='112' then " + StatisticsFieldName + " else 0 end) as �ϼ�," +
                                     "sum(case when BHDJ between '1' and '3' and substr(lz,1,2)='11' and dl<='112' then " + StatisticsFieldName + " else 0 end) as �����ֺϼ�," +
                                     "sum(case when BHDJ between '1' and '3' and substr(lz,1,2)='12' and dl<='112' then " + StatisticsFieldName + " else 0 end) as �����ֺϼ�," +
                                     "sum(case when BHDJ between '1' and '3' and ((substr(lz,1,2)<>'11' and substr(lz,1,2)<>'12') or ((substr(lz,1,2)='11' or substr(lz,1,2)='12') and dl>'112')) then " + StatisticsFieldName + " else 0 end) as �����ϼ�," +
                                     "sum(case when BHDJ ='1' and dl<='112' then " + StatisticsFieldName + " else 0 end) as һ��С��," +
                                     "sum(case when BHDJ='1' and substr(lz,1,2)='11' and dl<='112' then " + StatisticsFieldName + " else 0 end ) as һ������," +
                                     "sum(case when BHDJ='1' and substr(lz,1,2)='12' and dl<='112' then " + StatisticsFieldName + " else 0 end) as һ������," +
                                     "sum(case when BHDJ='1' and ((substr(lz,1,2)<>'11' and substr(lz,1,2)<>'12') or ((substr(lz,1,2)='11' or substr(lz,1,2)='12') and dl>'112')) then " + StatisticsFieldName + " else 0 end) as һ������," +
                                     "sum(case when BHDJ ='2' and dl<='112' then " + StatisticsFieldName + " else 0 end) as ����С��," +
                                     "sum(case when BHDJ='2' and substr(lz,1,2)='11' and dl<='112' then " + StatisticsFieldName + " else 0 end) as ��������," +
                                     "sum(case when BHDJ='2' and substr(lz,1,2)='12' and dl<='112' then " + StatisticsFieldName + " else 0 end ) as ��������," +
                                     "sum(case when BHDJ='2' and ((substr(lz,1,2)<>'11' and substr(lz,1,2)<>'12') or ((substr(lz,1,2)='11' or substr(lz,1,2)='12') and dl>'112')) then " + StatisticsFieldName + " else 0 end) as ��������," +
                                     "sum(case when BHDJ ='3' and dl<='112' then " + StatisticsFieldName + " else 0 end) as ����С��," +
                                     "sum(case when BHDJ='3' and substr(lz,1,2)='11' and dl<='112' then " + StatisticsFieldName + " else 0 end) as ��������," +
                                     "sum(case when BHDJ='3' and substr(lz,1,2)='12' and dl<='112'then " + StatisticsFieldName + " else 0 end) as ��������," +
                                     "sum(case when BHDJ='3' and ((substr(lz,1,2)<>'11' and substr(lz,1,2)<>'12') or ((substr(lz,1,2)='11' or substr(lz,1,2)='12') and dl>'112')) then " + StatisticsFieldName + " else 0 end) as  ��������" +
                                     "  from " + tableName +
                                     "  where substr(QI_YUAN,1,1)<>' ' and QI_YUAN is not null and sq='1' and lc<>' '" +
                                     "  group by lc,rollup(substr(QI_YUAN,1,1))" +
                                     "  order by substr(QI_YUAN,1,1) ";
                    pWorkspace.ExecuteSQL(CitySQL);
                }

                //����SQL����ѯͳ���������� ygc 2012-8-23
                string townsSQL = "insert into EightTable_GJGYHDJ select " +
                                 "lc as ͳ�Ƶ�λ,substr(QI_YUAN,1,1) as ��Դ," +
                                 "sum(case when BHDJ between '1' and '3' and dl<='112' then " + StatisticsFieldName + " else 0 end) as �ϼ�," +
                                 "sum(case when BHDJ between '1' and '3' and substr(lz,1,2)='11' and dl<='112' then " + StatisticsFieldName + " else 0 end) as �����ֺϼ�," +
                                 "sum(case when BHDJ between '1' and '3' and substr(lz,1,2)='12' and dl<='112' then " + StatisticsFieldName + " else 0 end) as �����ֺϼ�," +
                                 "sum(case when BHDJ between '1' and '3' and ((substr(lz,1,2)<>'11' and substr(lz,1,2)<>'12') or ((substr(lz,1,2)='11' or substr(lz,1,2)='12') and dl>'112')) then " + StatisticsFieldName + " else 0 end) as �����ϼ�," +
                                 "sum(case when BHDJ ='1' and dl<='112' then " + StatisticsFieldName + " else 0 end) as һ��С��," +
                                 "sum(case when BHDJ='1' and substr(lz,1,2)='11' and dl<='112' then " + StatisticsFieldName + " else 0 end ) as һ������," +
                                 "sum(case when BHDJ='1' and substr(lz,1,2)='12' and dl<='112' then " + StatisticsFieldName + " else 0 end) as һ������," +
                                 "sum(case when BHDJ='1' and ((substr(lz,1,2)<>'11' and substr(lz,1,2)<>'12') or ((substr(lz,1,2)='11' or substr(lz,1,2)='12') and dl>'112')) then " + StatisticsFieldName + " else 0 end) as һ������," +
                                 "sum(case when  BHDJ ='2' and dl<='112' then " + StatisticsFieldName + " else 0 end) as ����С��," +
                                 "sum(case when BHDJ='2' and substr(lz,1,2)='11' and dl<='112' then " + StatisticsFieldName + " else 0 end) as ��������," +
                                 "sum(case when BHDJ='2' and substr(lz,1,2)='12' and dl<='112' then " + StatisticsFieldName + " else 0 end ) as ��������," +
                                 "sum(case when BHDJ='2' and ((substr(lz,1,2)<>'11' and substr(lz,1,2)<>'12') or ((substr(lz,1,2)='11' or substr(lz,1,2)='12') and dl>'112')) then " + StatisticsFieldName + " else 0 end) as ��������," +
                                 "sum(case when BHDJ ='3' and dl<='112' then " + StatisticsFieldName + " else 0 end) as ����С��," +
                                 "sum(case when BHDJ='3' and substr(lz,1,2)='11' and dl<='112' then " + StatisticsFieldName + " else 0 end) as ��������," +
                                 "sum(case when BHDJ='3' and substr(lz,1,2)='12' and dl<='112'then " + StatisticsFieldName + " else 0 end) as ��������," +
                                 "sum(case when BHDJ='3' and ((substr(lz,1,2)<>'11' and substr(lz,1,2)<>'12') or ((substr(lz,1,2)='11' or substr(lz,1,2)='12') and dl>'112')) then " + StatisticsFieldName + " else 0 end) as  ��������" +
                                 "  from " + tableName +
                                 "  where substr(QI_YUAN,1,1)<>' ' and QI_YUAN is not null and sq='1' and lc<>' '" +
                                 "  group by lc,rollup(substr(QI_YUAN,1,1))" +
                                 "  order by substr(QI_YUAN,1,1) ";
                pWorkspace.ExecuteSQL(townsSQL);
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
               //��ѯ�ؼ�ͳ������SQL���
               //ygc 2012-8-22
               string CitySQL = "create table EightTable_LDBHDJTable as select  " +
                                 tableName + ".xian as ͳ�Ƶ�λ," +
                                "sum(case when (BCLD<>'2' or bcld is null)and  bh_dj between '1'and '4' then " + tableName + "." + StatisticsFieldName + " else 0 end ) as ��״�ϼ�," +
                                "sum(case when  (BCLD<>'2' or bcld is null) AND BH_DJ='1' then  " + tableName + "." + StatisticsFieldName + " else 0 end) as ��״1��," +
                                "sum(case when  (BCLD<>'2' or bcld is null) AND BH_DJ='2'then  " + tableName + "." + StatisticsFieldName + " else 0 end) as ��״2��," +
                                "sum(case when  (BCLD<>'2' or bcld is null) AND BH_DJ='3'then  " + tableName + "." + StatisticsFieldName + " else 0 end) as ��״3��," +
                                "sum(case when  (BCLD<>'2' or bcld is null) AND BH_DJ='4'then  " + tableName + "." + StatisticsFieldName + " else 0 end) as ��״4��," +
                                "sum(case when  LB_GHBHDJ between '1'and '4'then " + tableName + "." + StatisticsFieldName + " end) as �滮�ϼ�," +
                                "sum(case when  LB_GHBHDJ='1' then  " + tableName + "." + StatisticsFieldName + " else 0 end) as �滮1��," +
                                "sum(case when  LB_GHBHDJ='2' then  " + tableName + "." + StatisticsFieldName + " else 0 end) as �滮2��," +
                                "sum(case when  LB_GHBHDJ='3' then  " + tableName + "." + StatisticsFieldName + " else 0 end) as �滮3��," +
                                "sum(case when  LB_GHBHDJ='4' then  " + tableName + "." + StatisticsFieldName + " else 0 end) as �滮4��" +
                                " from " + tableName +
                                " group by xian";
               pWorkspace.ExecuteSQL(CitySQL);
               //��ѯ�缶ͳ������SQL���
               //ygc 2012-8-22
               string townsSQL = "insert into EightTable_LDBHDJTable select  " +
                                 tableName + ".xiang as ͳ�Ƶ�λ," +
                                "sum(case when (BCLD<>'2' or bcld is null)and bh_dj between '1'and '4' then " + tableName + "." + StatisticsFieldName + " else 0 end ) as ��״�ϼ�," +
                                "sum(case when  (BCLD<>'2' or bcld is null) AND BH_DJ='1' then  " + tableName + "." + StatisticsFieldName + " else 0 end) as ��״1��," +
                                "sum(case when  (BCLD<>'2' or bcld is null) AND BH_DJ='2'then  " + tableName + "." + StatisticsFieldName + " else 0 end) as ��״2��," +
                                "sum(case when  (BCLD<>'2' or bcld is null) AND BH_DJ='3'then  " + tableName + "." + StatisticsFieldName + " else 0 end) as ��״3��," +
                                "sum(case when  (BCLD<>'2' or bcld is null) AND BH_DJ='4'then  " + tableName + "." + StatisticsFieldName + " else 0 end) as ��״4��," +
                                "sum(case when  LB_GHBHDJ between '1'and '4'then " + tableName + "." + StatisticsFieldName + " end) as �滮�ϼ�," +
                                "sum(case when  LB_GHBHDJ='1' then  " + tableName + "." + StatisticsFieldName + " else 0 end) as �滮1��," +
                                "sum(case when  LB_GHBHDJ='2' then  " + tableName + "." + StatisticsFieldName + " else 0 end) as �滮2��," +
                                "sum(case when  LB_GHBHDJ='3' then  " + tableName + "." + StatisticsFieldName + " else 0 end) as �滮3��," +
                                "sum(case when  LB_GHBHDJ='4' then  " + tableName + "." + StatisticsFieldName + " else 0 end) as �滮4��" +
                                " from " + tableName +
                                " group by xiang";
               pWorkspace.ExecuteSQL(townsSQL);
               //��ѯ�м����� ygc 2012-10-22
               string SHISQL = "insert into EightTable_LDBHDJTable select  substr(" +
                  tableName + ".shi,1,4) as ͳ�Ƶ�λ," +
                 "sum(case when (BCLD<>'2' or bcld is null)and bh_dj between '1'and '4' then " + tableName + "." + StatisticsFieldName + " else 0 end ) as ��״�ϼ�," +
                 "sum(case when  (BCLD<>'2' or bcld is null) AND BH_DJ='1' then  " + tableName + "." + StatisticsFieldName + " else 0 end) as ��״1��," +
                 "sum(case when  (BCLD<>'2' or bcld is null) AND BH_DJ='2'then  " + tableName + "." + StatisticsFieldName + " else 0 end) as ��״2��," +
                 "sum(case when  (BCLD<>'2' or bcld is null) AND BH_DJ='3'then  " + tableName + "." + StatisticsFieldName + " else 0 end) as ��״3��," +
                 "sum(case when  (BCLD<>'2' or bcld is null) AND BH_DJ='4'then  " + tableName + "." + StatisticsFieldName + " else 0 end) as ��״4��," +
                 "sum(case when  LB_GHBHDJ between '1'and '4'then " + tableName + "." + StatisticsFieldName + " end) as �滮�ϼ�," +
                 "sum(case when  LB_GHBHDJ='1' then  " + tableName + "." + StatisticsFieldName + " else 0 end) as �滮1��," +
                 "sum(case when  LB_GHBHDJ='2' then  " + tableName + "." + StatisticsFieldName + " else 0 end) as �滮2��," +
                 "sum(case when  LB_GHBHDJ='3' then  " + tableName + "." + StatisticsFieldName + " else 0 end) as �滮3��," +
                 "sum(case when  LB_GHBHDJ='4' then  " + tableName + "." + StatisticsFieldName + " else 0 end) as �滮4��" +
                 " from " + tableName +
                 " group by substr(shi,1,4)";
               pWorkspace.ExecuteSQL(SHISQL);
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
                                    "sum(case when (BCLD<>'2' or bcld is null)and  bh_dj between '1'and '4' then " + tableName + "." + StatisticsFieldName + " else 0 end ) as ��״�ϼ�," +
                                    "sum(case when  (BCLD<>'2' or bcld is null) AND BH_DJ='1' then  " + tableName + "." + StatisticsFieldName + " else 0 end) as ��״1��," +
                                    "sum(case when  (BCLD<>'2' or bcld is null) AND BH_DJ='2'then  " + tableName + "." + StatisticsFieldName + " else 0 end) as ��״2��," +
                                    "sum(case when  (BCLD<>'2' or bcld is null) AND BH_DJ='3'then  " + tableName + "." + StatisticsFieldName + " else 0 end) as ��״3��," +
                                    "sum(case when  (BCLD<>'2' or bcld is null) AND BH_DJ='4'then  " + tableName + "." + StatisticsFieldName + " else 0 end) as ��״4��," +
                                    "sum(case when  LB_GHBHDJ between '1'and '4'then " + tableName + "." + StatisticsFieldName + " end) as �滮�ϼ�," +
                                    "sum(case when  LB_GHBHDJ='1' then  " + tableName + "." + StatisticsFieldName + " else 0 end) as �滮1��," +
                                    "sum(case when  LB_GHBHDJ='2' then  " + tableName + "." + StatisticsFieldName + " else 0 end) as �滮2��," +
                                    "sum(case when  LB_GHBHDJ='3' then  " + tableName + "." + StatisticsFieldName + " else 0 end) as �滮3��," +
                                    "sum(case when  LB_GHBHDJ='4' then  " + tableName + "." + StatisticsFieldName + " else 0 end) as �滮4��" +
                                    " from " + tableName +
                                    "  where lc <>' '"+
                                    " group by lc";
                   pWorkspace.ExecuteSQL(CitySQL);
               }

               //��ѯ�缶ͳ������SQL���
               //ygc 2012-8-22
               string townsSQL = "insert into EightTable_LDBHDJTable select  " +
                                 tableName + ".lc as ͳ�Ƶ�λ," +
                                "sum(case when (BCLD<>'2' or bcld is null)and bh_dj between '1'and '4' then " + tableName + "." + StatisticsFieldName + " else 0 end ) as ��״�ϼ�," +
                                "sum(case when  (BCLD<>'2' or bcld is null) AND BH_DJ='1' then  " + tableName + "." + StatisticsFieldName + " else 0 end) as ��״1��," +
                                "sum(case when  (BCLD<>'2' or bcld is null) AND BH_DJ='2'then  " + tableName + "." + StatisticsFieldName + " else 0 end) as ��״2��," +
                                "sum(case when  (BCLD<>'2' or bcld is null) AND BH_DJ='3'then  " + tableName + "." + StatisticsFieldName + " else 0 end) as ��״3��," +
                                "sum(case when  (BCLD<>'2' or bcld is null) AND BH_DJ='4'then  " + tableName + "." + StatisticsFieldName + " else 0 end) as ��״4��," +
                                "sum(case when  LB_GHBHDJ between '1'and '4'then " + tableName + "." + StatisticsFieldName + " end) as �滮�ϼ�," +
                                "sum(case when  LB_GHBHDJ='1' then  " + tableName + "." + StatisticsFieldName + " else 0 end) as �滮1��," +
                                "sum(case when  LB_GHBHDJ='2' then  " + tableName + "." + StatisticsFieldName + " else 0 end) as �滮2��," +
                                "sum(case when  LB_GHBHDJ='3' then  " + tableName + "." + StatisticsFieldName + " else 0 end) as �滮3��," +
                                "sum(case when  LB_GHBHDJ='4' then  " + tableName + "." + StatisticsFieldName + " else 0 end) as �滮4��" +
                                " from " + tableName +
                                " where lc<>' '" +
                                " group by lc";
               pWorkspace.ExecuteSQL(townsSQL);
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
               pWorkspace.ExecuteSQL("create table temp_table_1 as select * from  " + tableName);
               pWorkspace.ExecuteSQL("create table temp_table_2 as select * from  " + tableName);
               //������ʱ�� ygc 2012-8-27 
               pWorkspace.ExecuteSQL("update temp_table_1 set lz='2������' where substr(lz,1,2)='11'");
               pWorkspace.ExecuteSQL("update temp_table_1 set lz='1������' where substr(lz,1,2)='12'");
               pWorkspace.ExecuteSQL("update temp_table_2 set lz='4�����ֵ�' where substr(lz,1,2)<>'11' and substr(lz,1,2)<>'12' and substr(lz,1,2)<>' '");
               pWorkspace.ExecuteSQL("update temp_table_2 set lz='3������' where (substr(lz,1,2)='11' or substr(lz,1,2)='12')");
               //ͨ��SQL����ؼ�ͳ������ ygc 2012-9-27 
               string CitySQL = " create table EightTable_GJGYLGHMJ as (select " +
                                "temp_table_1.xian as ͳ�Ƶ�λ,lz as ����," +
                                "sum(case when SQ='1' AND BHDJ between '1' and '3' then " + StatisticsFieldName + " else 0 end) as ��״�ϼ�," +
                                "sum(case when SQ='1' AND BHDJ='1' then " + StatisticsFieldName + " else 0 end) as ��״һ��," +
                                "sum(case when SQ='1' AND BHDJ='2' then " + StatisticsFieldName + " else 0 end) as ��״����," +
                                "sum(case when SQ='1' AND BHDJ='3' then " + StatisticsFieldName + " else 0 end) as ��״����," +
                                "sum(case when GHSQ='1' AND BHDJ between '1' and '3' then " + StatisticsFieldName + " else 0 end) as �滮�ϼ�," +
                                "sum(case when GHSQ='1' AND BHDJ='1' then " + StatisticsFieldName + " else 0 end) as �滮һ��," +
                                "sum(case when GHSQ='1' AND BHDJ='2'then " + StatisticsFieldName + " else 0 end) as �滮����," +
                                "sum(case when GHSQ='1' AND BHDJ='3' then " + StatisticsFieldName + " else 0 end) as �滮����" +
                                " from temp_table_1" +
                                " where  ( lz<>' 'and ghlz<>' ') and (lz='1������' or lz='2������')" +
                                " group by xian,lz" +
                                " union all" +
                                " select " +
                                " xian as ͳ�Ƶ�λ,lz as ����," +
                                "sum(case when SQ='1' AND BHDJ between '1' and '3' then " + StatisticsFieldName + " else 0 end) as ��״�ϼ�," +
                                "sum(case when SQ='1' AND BHDJ='1' then " + StatisticsFieldName + " else 0 end) as ��״һ��," +
                                "sum(case when SQ='1' AND BHDJ='2' then " + StatisticsFieldName + " else 0 end) as ��״����," +
                                "sum(case when SQ='1' AND BHDJ='3' then " + StatisticsFieldName + " else 0 end) as ��״����," +
                                "sum(case when GHSQ='1' AND BHDJ between '1' and '3' then " + StatisticsFieldName + " else 0 end) as �滮�ϼ�," +
                                "sum(case when GHSQ='1' AND BHDJ='1' then " + StatisticsFieldName + " else 0 end) as �滮һ��," +
                                "sum(case when GHSQ='1' AND BHDJ='2'then " + StatisticsFieldName + " else 0 end) as �滮����," +
                                "sum(case when GHSQ='1' AND BHDJ='3' then " + StatisticsFieldName + " else 0 end) as �滮����" +
                                " from temp_table_2" +
                                " where  ( lz<>' 'and ghlz<>' ')" +
                                " group by xian,rollup(lz))";
               pWorkspace.ExecuteSQL(CitySQL);
               //��SQL����ȡ�缶������ͳ������
               string townsSQL = " insert  into EightTable_GJGYLGHMJ  (select " +
                                "temp_table_1.xiang as ͳ�Ƶ�λ,lz as ����," +
                                "sum(case when SQ='1' AND BHDJ between '1' and '3' then " + StatisticsFieldName + " else 0 end) as ��״�ϼ�," +
                                "sum(case when SQ='1' AND BHDJ='1' then " + StatisticsFieldName + " else 0 end) as ��״һ��," +
                                "sum(case when SQ='1' AND BHDJ='2' then " + StatisticsFieldName + " else 0 end) as ��״����," +
                                "sum(case when SQ='1' AND BHDJ='3' then " + StatisticsFieldName + " else 0 end) as ��״����," +
                                "sum(case when GHSQ='1' AND BHDJ between '1' and '3' then " + StatisticsFieldName + " else 0 end) as �滮�ϼ�," +
                                "sum(case when GHSQ='1' AND BHDJ='1' then " + StatisticsFieldName + " else 0 end) as �滮һ��," +
                                "sum(case when GHSQ='1' AND BHDJ='2'then " + StatisticsFieldName + " else 0 end) as �滮����," +
                                "sum(case when GHSQ='1' AND BHDJ='3' then " + StatisticsFieldName + " else 0 end) as �滮����" +
                                " from temp_table_1" +
                                " where  ( lz<>' 'and ghlz<>' ') and (lz='1������' or lz='2������')" +
                                " group by xiang,lz" +
                                " union all" +
                                " select " +
                                " xiang as ͳ�Ƶ�λ,lz as ����," +
                                "sum(case when SQ='1' AND BHDJ between '1' and '3' then " + StatisticsFieldName + " else 0 end) as ��״�ϼ�," +
                                "sum(case when SQ='1' AND BHDJ='1' then " + StatisticsFieldName + " else 0 end) as ��״һ��," +
                                "sum(case when SQ='1' AND BHDJ='2' then " + StatisticsFieldName + " else 0 end) as ��״����," +
                                "sum(case when SQ='1' AND BHDJ='3' then " + StatisticsFieldName + " else 0 end) as ��״����," +
                                "sum(case when GHSQ='1' AND BHDJ between '1' and '3' then " + StatisticsFieldName + " else 0 end) as �滮�ϼ�," +
                                "sum(case when GHSQ='1' AND BHDJ='1' then " + StatisticsFieldName + " else 0 end) as �滮һ��," +
                                "sum(case when GHSQ='1' AND BHDJ='2'then " + StatisticsFieldName + " else 0 end) as �滮����," +
                                "sum(case when GHSQ='1' AND BHDJ='3' then " + StatisticsFieldName + " else 0 end) as �滮����" +
                                " from temp_table_2" +
                                " where  ( lz<>' 'and ghlz<>' ')" +
                                " group by xiang,rollup(lz))";
               pWorkspace.ExecuteSQL(townsSQL);
               //��ȡ�м�����a
               string SHISQL = " insert  into EightTable_GJGYLGHMJ  (select " +
                 "substr(temp_table_1.shi,1,4) as ͳ�Ƶ�λ,lz as ����," +
                 "sum(case when SQ='1' AND BHDJ between '1' and '3' then " + StatisticsFieldName + " else 0 end) as ��״�ϼ�," +
                 "sum(case when SQ='1' AND BHDJ='1' then " + StatisticsFieldName + " else 0 end) as ��״һ��," +
                 "sum(case when SQ='1' AND BHDJ='2' then " + StatisticsFieldName + " else 0 end) as ��״����," +
                 "sum(case when SQ='1' AND BHDJ='3' then " + StatisticsFieldName + " else 0 end) as ��״����," +
                 "sum(case when GHSQ='1' AND BHDJ between '1' and '3' then " + StatisticsFieldName + " else 0 end) as �滮�ϼ�," +
                 "sum(case when GHSQ='1' AND BHDJ='1' then " + StatisticsFieldName + " else 0 end) as �滮һ��," +
                 "sum(case when GHSQ='1' AND BHDJ='2'then " + StatisticsFieldName + " else 0 end) as �滮����," +
                 "sum(case when GHSQ='1' AND BHDJ='3' then " + StatisticsFieldName + " else 0 end) as �滮����" +
                 " from temp_table_1" +
                 " where  ( lz<>' 'and ghlz<>' ') and (lz='1������' or lz='2������')" +
                 " group by substr(shi,1,4),lz" +
                 " union all" +
                 " select " +
                 " substr(shi,1,4) as ͳ�Ƶ�λ,lz as ����," +
                 "sum(case when SQ='1' AND BHDJ between '1' and '3' then " + StatisticsFieldName + " else 0 end) as ��״�ϼ�," +
                 "sum(case when SQ='1' AND BHDJ='1' then " + StatisticsFieldName + " else 0 end) as ��״һ��," +
                 "sum(case when SQ='1' AND BHDJ='2' then " + StatisticsFieldName + " else 0 end) as ��״����," +
                 "sum(case when SQ='1' AND BHDJ='3' then " + StatisticsFieldName + " else 0 end) as ��״����," +
                 "sum(case when GHSQ='1' AND BHDJ between '1' and '3' then " + StatisticsFieldName + " else 0 end) as �滮�ϼ�," +
                 "sum(case when GHSQ='1' AND BHDJ='1' then " + StatisticsFieldName + " else 0 end) as �滮һ��," +
                 "sum(case when GHSQ='1' AND BHDJ='2'then " + StatisticsFieldName + " else 0 end) as �滮����," +
                 "sum(case when GHSQ='1' AND BHDJ='3' then " + StatisticsFieldName + " else 0 end) as �滮����" +
                 " from temp_table_2" +
                 " where  ( lz<>' 'and ghlz<>' ')" +
                 " group by substr(shi,1,4),rollup(lz))";
               pWorkspace.ExecuteSQL(SHISQL);
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
               pWorkspace.ExecuteSQL("create table temp_table_1 as select * from  " + tableName);
               pWorkspace.ExecuteSQL("create table temp_table_2 as select * from  " + tableName);
               //������ʱ�� ygc 2012-8-27 
               pWorkspace.ExecuteSQL("update temp_table_1 set lz='2������' where substr(lz,1,2)='11'");
               pWorkspace.ExecuteSQL("update temp_table_1 set lz='1������' where substr(lz,1,2)='12'");
               pWorkspace.ExecuteSQL("update temp_table_2 set lz='4�����ֵ�' where substr(lz,1,2)<>'11' and substr(lz,1,2)<>'12' and substr(lz,1,2)<>' '");
               pWorkspace.ExecuteSQL("update temp_table_2 set lz='3������' where (substr(lz,1,2)='11' or substr(lz,1,2)='12')");
               if (!ExistTable(pWorkspace, "EightTable_GJGYLGHMJ"))
               {
                   string CitySQL = " create table EightTable_GJGYLGHMJ as (select " +
                                   "temp_table_1.lc as ͳ�Ƶ�λ,lz as ����," +
                                   "sum(case when SQ='1' AND BHDJ between '1' and '3' then " + StatisticsFieldName + " else 0 end) as ��״�ϼ�," +
                                   "sum(case when SQ='1' AND BHDJ='1' then " + StatisticsFieldName + " else 0 end) as ��״һ��," +
                                   "sum(case when SQ='1' AND BHDJ='2' then " + StatisticsFieldName + " else 0 end) as ��״����," +
                                   "sum(case when SQ='1' AND BHDJ='3' then " + StatisticsFieldName + " else 0 end) as ��״����," +
                                   "sum(case when GHSQ='1' AND BHDJ between '1' and '3' then " + StatisticsFieldName + " else 0 end) as �滮�ϼ�," +
                                   "sum(case when GHSQ='1' AND BHDJ='1' then " + StatisticsFieldName + " else 0 end) as �滮һ��," +
                                   "sum(case when GHSQ='1' AND BHDJ='2'then " + StatisticsFieldName + " else 0 end) as �滮����," +
                                   "sum(case when GHSQ='1' AND BHDJ='3' then " + StatisticsFieldName + " else 0 end) as �滮����" +
                                   " from temp_table_1" +
                                   " where  ( lz<>' 'or ghlz<>' ') and (lz='1������' or lz='2������') and lc<>' '" +
                                   " group by lc,lz" +
                                   " union all" +
                                   " select " +
                                   " lc as ͳ�Ƶ�λ,lz as ����," +
                                   "sum(case when SQ='1' AND BHDJ between '1' and '3' then " + StatisticsFieldName + " else 0 end) as ��״�ϼ�," +
                                   "sum(case when SQ='1' AND BHDJ='1' then " + StatisticsFieldName + " else 0 end) as ��״һ��," +
                                   "sum(case when SQ='1' AND BHDJ='2' then " + StatisticsFieldName + " else 0 end) as ��״����," +
                                   "sum(case when SQ='1' AND BHDJ='3' then " + StatisticsFieldName + " else 0 end) as ��״����," +
                                   "sum(case when GHSQ='1' AND BHDJ between '1' and '3' then " + StatisticsFieldName + " else 0 end) as �滮�ϼ�," +
                                   "sum(case when GHSQ='1' AND BHDJ='1' then " + StatisticsFieldName + " else 0 end) as �滮һ��," +
                                   "sum(case when GHSQ='1' AND BHDJ='2'then " + StatisticsFieldName + " else 0 end) as �滮����," +
                                   "sum(case when GHSQ='1' AND BHDJ='3' then " + StatisticsFieldName + " else 0 end) as �滮����" +
                                   " from temp_table_2" +
                                   " where  ( lz<>' 'or ghlz<>' ') and lc<>' '" +
                                   " group by lc,rollup(lz))";
                   pWorkspace.ExecuteSQL(CitySQL);
               }

               //��SQL����ȡ�缶������ͳ������
               string townsSQL = " insert  into EightTable_GJGYLGHMJ  (select " +
                                "temp_table_1.lc as ͳ�Ƶ�λ,lz as ����," +
                                "sum(case when SQ='1' AND BHDJ between '1' and '3' then " + StatisticsFieldName + " else 0 end) as ��״�ϼ�," +
                                "sum(case when SQ='1' AND BHDJ='1' then " + StatisticsFieldName + " else 0 end) as ��״һ��," +
                                "sum(case when SQ='1' AND BHDJ='2' then " + StatisticsFieldName + " else 0 end) as ��״����," +
                                "sum(case when SQ='1' AND BHDJ='3' then " + StatisticsFieldName + " else 0 end) as ��״����," +
                                "sum(case when GHSQ='1' AND BHDJ between '1' and '3' then " + StatisticsFieldName + " else 0 end) as �滮�ϼ�," +
                                "sum(case when GHSQ='1' AND BHDJ='1' then " + StatisticsFieldName + " else 0 end) as �滮һ��," +
                                "sum(case when GHSQ='1' AND BHDJ='2'then " + StatisticsFieldName + " else 0 end) as �滮����," +
                                "sum(case when GHSQ='1' AND BHDJ='3' then " + StatisticsFieldName + " else 0 end) as �滮����" +
                                " from temp_table_1" +
                                " where  ( lz<>' 'or ghlz<>' ') and (lz='1������' or lz='2������') and lc<>' '" +
                                " group by lc,lz" +
                                " union all" +
                                " select " +
                                " lc as ͳ�Ƶ�λ,lz as ����," +
                                "sum(case when SQ='1' AND BHDJ between '1' and '3' then " + StatisticsFieldName + " else 0 end) as ��״�ϼ�," +
                                "sum(case when SQ='1' AND BHDJ='1' then " + StatisticsFieldName + " else 0 end) as ��״һ��," +
                                "sum(case when SQ='1' AND BHDJ='2' then " + StatisticsFieldName + " else 0 end) as ��״����," +
                                "sum(case when SQ='1' AND BHDJ='3' then " + StatisticsFieldName + " else 0 end) as ��״����," +
                                "sum(case when GHSQ='1' AND BHDJ between '1' and '3' then " + StatisticsFieldName + " else 0 end) as �滮�ϼ�," +
                                "sum(case when GHSQ='1' AND BHDJ='1' then " + StatisticsFieldName + " else 0 end) as �滮һ��," +
                                "sum(case when GHSQ='1' AND BHDJ='2'then " + StatisticsFieldName + " else 0 end) as �滮����," +
                                "sum(case when GHSQ='1' AND BHDJ='3' then " + StatisticsFieldName + " else 0 end) as �滮����" +
                                " from temp_table_2" +
                                " where  ( lz<>' 'or ghlz<>' ') and lc<>' '" +
                                " group by lc,rollup(lz))";
               pWorkspace.ExecuteSQL(townsSQL);

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

    }
}
