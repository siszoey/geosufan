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
    /// ������������ί�У�ʹ���ܹ��԰�ȫ�߳������������޴���ʾ
    /// ������д
    /// </summary>
    public class ClearDataCheckGrid
    {
        /// <summary>
        /// ����һ��ί�У�������������
        /// </summary>
        /// <param name="AppHk"></param>
        public delegate void ClearGrid(Plugin.Application.IAppGISRef AppHk);//����һ��ί��
        /// <summary>
        /// ����������ʾ�����ݷ���
        /// </summary>
        /// <param name="_AppHk"></param>
        public void ClearResult(Plugin.Application.IAppGISRef _AppHk)
        {
            if (_AppHk.DataCheckGrid.DataSource != null)//�����ʾ������֮ǰ�ļ�¼������Ҫ��֮ǰ���ܼ�����������Ա���ʾ��ǰ��
                _AppHk.DataCheckGrid.DataSource = null;
        }

        /// <summary>
        /// ʹ�ô�������������ί�м���صķ�����ʵ��ִ��
        /// </summary>
        /// <param name="pAppForm"></param>
        /// <param name="_AppHk"></param>
        public void Operate(Plugin.Application.IAppFormRef pAppForm, Plugin.Application.IAppGISRef _AppHk)
        {
            pAppForm.MainForm.Invoke(new ClearGrid(ClearResult), new object[] { _AppHk });
        }

        /// <summary>
        /// ����һ��������ʾ���������ί��
        /// </summary>
        /// <param name="hook"></param>
        private delegate void Delegate_CheckDataGrid(Plugin.Application.IAppGISRef hook);
        /// <summary>
        /// /ѡ�м������б�
        /// </summary>
        private void CheckDataGrid(Plugin.Application.IAppGISRef hook)
        {
            DevComponents.DotNetBar.PanelDockContainer PanelTip = hook.DataCheckGrid.Parent as DevComponents.DotNetBar.PanelDockContainer;
            if (PanelTip != null)
            {
                PanelTip.DockContainerItem.Selected = true;
            }
        }
        /// <summary>
        /// ʹ�������尲ȫί��,������ʾ�������
        /// </summary>
        /// <param name="pAppForm"></param>
        /// <param name="hook"></param>
        public void CheckDataGridShow(Plugin.Application.IAppFormRef pAppForm, Plugin.Application.IAppGISRef hook)
        {
            pAppForm.MainForm.Invoke(new Delegate_CheckDataGrid(CheckDataGrid), new object[] { hook });
        }
    }
}
