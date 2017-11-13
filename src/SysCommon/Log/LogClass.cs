using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace SysCommon.Log
{
    //������־
    public class SysLocalLog
    {
        private StreamWriter _StreamWriter;

        //���·��
        private string _strSavePath;
        public string SavePath
        {
            get
            {
                return _strSavePath;
            }
            set
            {
                _strSavePath = value;
            }
        }

        //���캯��
        public SysLocalLog()
        {
        }

        public SysLocalLog(string strSavePath)
        {
            _strSavePath = strSavePath;
        }

        public void CreateLogFile(string strFileName)
        {
            if (_strSavePath == string.Empty) return;
            if (!Directory.Exists(_strSavePath))
            {
                Directory.CreateDirectory(_strSavePath);
            }

            if (!File.Exists(_strSavePath + @"\" + strFileName))
            {
                FileStream pFileStream = File.Create(_strSavePath + @"\" + strFileName);
                pFileStream.Close();
            }

            _StreamWriter = new StreamWriter(_strSavePath + @"\" + strFileName);
        }

        // д�뱾����־(�������Ĺ��캯����CreateLogFile������ʹ��)
        public void WriteLocalLog(string content)
        {
            if (_StreamWriter == null) return;
            _StreamWriter.Write(_StreamWriter.NewLine);
            _StreamWriter.Write(content);
            _StreamWriter.Write(_StreamWriter.NewLine);
        }

        /// <summary>
        /// д�뱾����־(����ʹ��)
        /// </summary>
        /// <param name="content">��־����</param>
        /// <param name="filepath">�ļ�·��</param>
        /// <returns>д�뱾����־�Ƿ�ɹ�</returns>
        public void WriteLocalLog(string content, string filepath)
        {
            if (!File.Exists(filepath))
            {
                _strSavePath = filepath.Substring(0, filepath.LastIndexOf("\\"));
                string fileName = filepath.Substring(filepath.LastIndexOf("\\"));
                CreateLogFile(fileName);
            }

            if (_StreamWriter == null)
            {
                _StreamWriter = new StreamWriter(filepath);
            }

            WriteLocalLog(content);
        }

        public void LogClose()
        {
            if (_StreamWriter != null)
            {
                _StreamWriter.Close();
                _StreamWriter = null;
            }
        }
    }

    //���ݿ���־
    public class SysDBLog
    {
    }
}
