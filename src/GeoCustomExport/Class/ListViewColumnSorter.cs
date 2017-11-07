using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Windows.Forms;

namespace GeoCustomExport
{
    class ListViewColumnSorter : System.Collections.IComparer
    {
        private int ColumnToSort;

        // SortOrder ��һ���о��ͱ�
        private SortOrder OrderOfSort;
        private CaseInsensitiveComparer ObjectCompare;

        public ListViewColumnSorter()
        {
            // �������г�ʼ���� 0��
            ColumnToSort = 0;

            // ������˳���ʼ���� None ��
            OrderOfSort = SortOrder.None;

            // ��ʼ�� CaseInsensitiveComparer �����
            ObjectCompare = new CaseInsensitiveComparer();
        }

        public int SortColumn
        {
            get
            {
                return ColumnToSort;
            }

            set
            {
                ColumnToSort = value;
            }
        }

        public SortOrder Order
        {
            get
            {
                return OrderOfSort;
            }

            set
            {
                OrderOfSort = value;
            }
        }

        #region IComparer ��Ա
        int IComparer.Compare(object x, object y)
        {
            int compareResult;
            ListViewItem listviewX;
            ListViewItem listviewY;

            // ��Ҫ���ȽϵĶ���ת���� ListViewItem ����
            listviewX = (ListViewItem)(x);
            listviewY = (ListViewItem)(y);

            // �Ƚ�������Ŀ��
            compareResult = ObjectCompare.Compare(listviewX.SubItems[ColumnToSort].Text, listviewY.SubItems[ColumnToSort].Text);

            // ���ȽϽ�����ء�
            if (OrderOfSort == SortOrder.Ascending)
            {
                // ��������ѡȡ�����رȽϲ����ĵ��ͽ����
                return compareResult;
            }
            else if (OrderOfSort == SortOrder.Descending)
            {
                // �ݼ�����ѡȡ�����رȽϲ���֮����ĸ�ֵ��
                return (-compareResult);
            }
            else
            {
                // �����������򴫻� 0 ��
                return 0;
            }
        }
        #endregion
    }
}
