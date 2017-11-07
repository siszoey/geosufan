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

namespace GeoDataChecker
{
    /// <summary>
    /// ������ʱʹ�õ���״̬����ص�ȫ�ֱ��� ��һЩ�ɹ��õĺ���ί��
    /// </summary>
   public class SetCheckState
    {
       public static bool GeoCor = false;//����ȷ����鹦�����Ƿ����õ������͹رձ༭�Ĳ���
       public static bool CheckState = false;//����״̬�����ݶ�Ӧ������������
       public static ITopology pT=null;//����
       public static IGeoDataset Geodatabase = null;//����һ������
       public static ITopologyRuleContainer pRuleCont;//һ�����˹�����ļ�������
       public static string CheckDataBaseName = "";//��ǰѡ��Ŀ�������
       /// <summary>
       /// ����һ��ί�У����������Ի���
       /// </summary>
       /// <param name="tip"></param>
       /// <param name="content"></param>
       private delegate void ShowContent(string tip, string content);
       /// <summary>
       /// �����Ի���ľ���ʵ�ֺ���
       /// </summary>
       /// <param name="tip"></param>
       /// <param name="content"></param>
       private static void ShowMessage(string tip, string content)
       {
           SysCommon.Error.ErrorHandle.ShowFrmErrorHandle(tip, content);//����֮ǰ������ĵ����Ի�����
       }
       /// <summary>
       /// ʵ�ʵ��ú��� �����Ի���
       /// </summary>
       /// <param name="pAppForm"></param>
       public static void Message(Plugin.Application.IAppFormRef pAppForm,string tip,string content)
       {

           pAppForm.MainForm.Invoke(new ShowContent(ShowMessage), new object[] { tip, content });
       }
       /// <summary>
       /// ״̬����ʾ���ݵ�ί��
       /// </summary>
       /// <param name="pAppForm"></param>
       /// <param name="tip"></param>
       private delegate void ShowTips(Plugin.Application.IAppFormRef pAppForm, string tip);
       /// <summary>
       /// ״̬����ʾ���ݵķ���
       /// </summary>
       /// <param name="pAppForm"></param>
       /// <param name="tip"></param>
       private static void ShowTipsFun(Plugin.Application.IAppFormRef pAppForm, string tip)
       {
           pAppForm.OperatorTips = tip;//�����Ǵ����������ʾ��״̬����
       }
       /// <summary>
       /// ��ʹ���߳�ʱ������ؼ�Ҫʹ�ô���İ�ȫ�߳�ί�н��е���
       /// </summary>
       /// <param name="pAppForm"></param>
       /// <param name="tip"></param>
       public static void CheckShowTips(Plugin.Application.IAppFormRef pAppForm, string tip)
       {
           pAppForm.MainForm.Invoke(new ShowTips(ShowTipsFun), new object[] { pAppForm, tip });
       }
       /// <summary>
       /// ��ʼ�������ͼ ʹ�����˹������� ί��
       /// </summary>
       /// <param name="list"></param>
       private delegate void Ini_Tree(ArrayList list,Plugin.Application.IAppGISRef pAppForm);
       /// <summary>
       /// ��ʼ��������Ҫ����
       /// </summary>
       /// <param name="list"></param>
       /// <param name="pAppForm"></param>
       private static void Ini_trees(ArrayList list, Plugin.Application.IAppGISRef pAppForm)
       {
           ImageList imglist = pAppForm.DataTree.ImageList;
           pAppForm.DataTree.Nodes.Clear();
           pAppForm.DataTree.Columns.Clear();
           DevComponents.AdvTree.Node Firstnode = new DevComponents.AdvTree.Node();
           Firstnode.Text = Overridfunction.name;
           Firstnode.Image = imglist.Images[3];
           foreach (IFeatureClass TempClass in list)
           {
               IDataset ds = TempClass as IDataset;
               DevComponents.AdvTree.Node TempNode = new DevComponents.AdvTree.Node();
               TempNode.Text = ds.Name;
               TempNode.Image = imglist.Images[6];
               Firstnode.Nodes.Add(TempNode);
           }
           Firstnode.Expand();
           pAppForm.DataTree.Nodes.Add(Firstnode);
       }
       /// <summary>
       /// ��ϼ�鹦�ܳ�ʼ����ͼ
       /// </summary>
       /// <param name="list"></param>
       /// <param name="pAppForm"></param>
       public static void TreeIni_Fun(ArrayList list, Plugin.Application.IAppGISRef pAppForm)
       {
           Plugin.Application.IAppFormRef Form = pAppForm as Plugin.Application.IAppFormRef;
           Form.MainForm.Invoke(new Ini_Tree(Ini_trees), new object[] { list, pAppForm });
       }
       /// <summary>
       /// ���ý��Ȳ���ʱѡ�����ڵ�
       /// </summary>
       /// <param name="name"></param>
       /// <param name="index"></param>
       private delegate void TreeCheck(string name, int index, Plugin.Application.IAppGISRef pAppForm);
       /// <summary>
       /// ѡ�еľ��巽��
       /// </summary>
       /// <param name="name"></param>
       /// <param name="index"></param>
       /// <param name="pAppForm"></param>
       private static void TreeCheck_Fun(string name, int index, Plugin.Application.IAppGISRef pAppForm)
       {
           if (pAppForm.DataTree.Nodes[0].Nodes[index].Text == name)
           {
               pAppForm.DataTree.SelectedNode=pAppForm.DataTree.Nodes[0].Nodes[index];//ѡ�����ڵ�

           }
       }
       /// <summary>
       /// �ɹ��ⲿ���õ�ѡ�з��� ���ڵ�ѡ��
       /// </summary>
       /// <param name="name"></param>
       /// <param name="index"></param>
       /// <param name="pAppForm"></param>
       public static void TreeCheckFun(string name, int index, Plugin.Application.IAppGISRef pAppForm)
       {
           Plugin.Application.IAppFormRef Form = pAppForm as Plugin.Application.IAppFormRef;
           Form.MainForm.Invoke(new TreeCheck(TreeCheck_Fun), new object[] {name,index, pAppForm });
       }
   }
}
