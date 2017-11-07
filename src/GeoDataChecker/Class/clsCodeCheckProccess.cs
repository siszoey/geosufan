using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Data.OleDb;

namespace GeoDataChecker
{
    class clsCodeCheckProccess:GOGISErrorTreator
    {

        public clsCodeCheckProccess()
        {
            ///������־����ģ�帴��mdb�ļ���bin\..\Log�ļ�����
            string DesLogPaht = Application.StartupPath + "\\..\\Log\\Log" + System.DateTime.Today.Year.ToString() + System.DateTime.Today.Month.ToString() + System.DateTime.Today.Day.ToString() + System.DateTime.Now.Hour.ToString() + System.DateTime.Now.Minute.ToString() + System.DateTime.Now.Second.ToString() + ".mdb";

            ///�����ļ�
            File.Copy(Application.StartupPath + "\\..\\Template\\Log.mdb", DesLogPaht);
           

            ///��¼��ǰ��־·��
            this._LogPath = DesLogPaht;

            ///�������ݿ�����
            _con = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + this._LogPath);
            _con.Open();
        }

        /// <summary>
        /// д����־
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public override void LogErr(object sender,ErrorEventArgs e)
        {
            try
            {
                if (e != null)
                {
                    for (int i = 0; i < e.OIDs.Length; i++)
                    {
                        string pSql = "insert into Errorlog(ErrorName,ErrorDescription,ErrorFeatureClassName,ErrorFeatureID,CheckTime) values('" + e.ErrorName + "','" + e.ErrDescription + "','" + e.FeatureClassName + "','" + e.OIDs[i].ToString() + "','" + e.CheckTime + "')";
                        OleDbCommand SqlCommand = new OleDbCommand(pSql, this._con);
                        SqlCommand.ExecuteNonQuery();
                    }
                }
            }
            catch(Exception ex)
            {
                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("����", ex.Message);
            }
        }
    }
}
