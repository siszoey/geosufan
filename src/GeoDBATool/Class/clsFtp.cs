using System.Net;
using System;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;

namespace GeoDBATool
{
    public class FTP_Class
    {
        private  string ftpServerIP;//ftp������ip
        private string ftpUserID;//�û�id
        private string ftpPassword;//����


        private FtpWebRequest reqFTP;

        public FTP_Class()
        {

        }

        public FTP_Class(string IP,string ID,string PassWord)
        {
            this.ftpServerIP = IP;
            this.ftpUserID = ID;
            this.ftpPassword = PassWord;
        }

        #region ��������
        /// <summary>
        /// ��������
        /// </summary>
        /// <param name="ftpServerIP"></param>
        /// <param name="ftpUserID"></param>
        /// <param name="ftpPassword"></param>
        /// <param name="ErrorStr">����ִ��״̬����Succeed��Ϊ�ɹ�</param>
        public void Connecttest(string ftpServerIP, string ftpUserID, string ftpPassword, out string ErrorStr)
        {

            try
            {
                ErrorStr = "";
                // ����uri����FtpWebRequest����
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri("ftp://" + ftpServerIP));
                // ָ�����ݴ�������
                reqFTP.UseBinary = true;
                // ftp�û���������
                reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                Connect("ftp://" + ftpServerIP + "/");
                reqFTP.Method = WebRequestMethods.Ftp.ListDirectory;
                reqFTP.EnableSsl = false;///////////////�Ƿ�ʹ�ü��ܣ�trueΪʹ�ü��ܣ�����ģʽ���޷�ʹ�ü���
                reqFTP.Timeout = 10000;
                WebResponse response = reqFTP.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream(), System.Text.Encoding.UTF8);//�����ļ���
                string line = reader.ReadLine();
                ErrorStr = "Succeed";
            }
            catch (Exception ex)
            {
                ErrorStr = string.Format("��{0},����ʧ��", ex.Message);
            }
        }
        #endregion

        #region ����
        /// <summary>
        /// ����
        /// </summary>
        /// <param name="path"></param>
        private void Connect(string path)//����ftp
        {
            // ����uri����FtpWebRequest����
            reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(path));
            // ָ�����ݴ�������
            reqFTP.UseBinary = true;
            // ftp�û���������
            reqFTP.KeepAlive = false;
            reqFTP.Timeout = 10000;
            reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
        }
        #endregion

        #region ftp���õ�¼��Ϣ
        /// <summary>
        /// ftp��¼��Ϣ
        /// </summary>
        /// <param name="ftpServerIP">ftpServerIP</param>
        /// <param name="ftpUserID">ftpUserID</param>
        /// <param name="ftpPassword">ftpPassword</param>
        public void FtpUpDown(string ftpServerIP, string ftpUserID, string ftpPassword)
        {
            this.ftpServerIP = ftpServerIP;
            this.ftpUserID = ftpUserID;
            this.ftpPassword = ftpPassword;
        }
        #endregion

        #region ��ȡ�ļ��б�
        /// <summary>
        /// ��ȡ�ļ��б�
        /// </summary>
        /// <param name="path"></param>
        /// <param name="WRMethods"></param>
        /// <returns></returns>
        private string[] GetFileList(string path, string WRMethods,out string err )//����Ĵ���ʾ������δ�ftp�������ϻ���ļ��б�
        {
            string[] downloadFiles;
            StringBuilder result = new StringBuilder();
            bool read = false;
            try
            {
                err="";
                Connect(path);
                reqFTP.Method = WRMethods;
                reqFTP.Timeout = 10000;
                reqFTP.KeepAlive = false;
                WebResponse response = reqFTP.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream(), System.Text.Encoding.UTF8);//�����ļ���
                string line = reader.ReadLine();
                while (line != null)
                {
                    result.Append(line);
                    result.Append("\n");
                    line = reader.ReadLine();
                    read = true;
                }
                // to remove the trailing '\n'
                if (!read)
                {
                    err = "Succeed";
                    return null;
                }
                result.Remove(result.ToString().LastIndexOf('\n'), 1);
                reader.Close();
                response.Close();
                err = "Succeed";
                return result.ToString().Split('\n');
                
            }
            catch (Exception ex)
            {
                err=ex.Message;
                downloadFiles = null;
                return downloadFiles;
            }
            finally
            
            {
                if (null != result)
                    result = null;
            }
        }
        public string[] GetFileList(string path,out string error)//����Ĵ���ʾ������δ�ftp�������ϻ���ļ��б�
        {
            return GetFileList("ftp://" + ftpServerIP + "/" + path, WebRequestMethods.Ftp.ListDirectory,out error);
        }
        public string[] GetFileList(out string error)//����Ĵ���ʾ������δ�ftp�������ϻ���ļ��б�
        {
            return GetFileList("ftp://" + ftpServerIP + "/", WebRequestMethods.Ftp.ListDirectory,out error);
        }
        #endregion

        #region �ϴ��ļ�
        /// <summary>
        /// �ϴ��ļ�
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="path"></param>
        public bool Upload(string filename, string path, string savename,out string errorinfo) //����Ĵ���ʵ���˴�ftp�����������ļ��Ĺ���
        {
            //FrmProcessBar frmbar = new FrmProcessBar();
            //frmbar.Show();
            path = path.Replace("\\", "/");
            FileInfo fileInf = new FileInfo(filename);

            savename = savename.Replace("+", "-");// ����Ƿ��ַ�
            savename = savename.Replace("#", "-");//
            string uri;
            if (string.IsNullOrEmpty(path))
                uri = "ftp://" + ftpServerIP + "/" + savename;
            else
                uri = "ftp://" + ftpServerIP + "/" + path + "/" + savename;
            Connect(uri);//����    

            // Ĭ��Ϊtrue�����Ӳ��ᱻ�ر�
            // ��һ������֮��ִ��
            reqFTP.KeepAlive = false;
            reqFTP.Timeout = 10000;
            // ָ��ִ��ʲô����
            reqFTP.Method = WebRequestMethods.Ftp.UploadFile;
            // �ϴ��ļ�ʱ֪ͨ�������ļ��Ĵ�С
            reqFTP.ContentLength =  fileInf.Length; 
            // �����С����Ϊkb 
            long procMax = fileInf.Length / 2048;
            //���������ֵ
            //frmbar.SetFrmProcessBarMax(procMax);
            //string task = "�����ϴ��ļ���" + savename;
            //frmbar.SetFrmProcessBarText(task);
            int value = 0;
            int buffLength = 2048;
            byte[] buff = new byte[buffLength];
            int contentLen;
            // ��һ���ļ���(System.IO.FileStream) ȥ���ϴ����ļ�
            
            try
            {
                FileStream fs = fileInf.OpenRead();
                // ���ϴ����ļ�д����
                Stream strm = reqFTP.GetRequestStream();
                // ÿ�ζ��ļ�����kb
                contentLen = fs.Read(buff, 0, buffLength);
                // ������û�н���
                while (contentLen != 0)
                {
                    value++;
                    //frmbar.SetFrmProcessBarValue(value);
                    Application.DoEvents();
                    // �����ݴ�file stream д��upload stream 
                    strm.Write(buff, 0, contentLen);
                    contentLen = fs.Read(buff, 0, buffLength);
                }
                // �ر�������
                strm.Close();
                fs.Close();
                errorinfo = "Succeed";
                return true;
            }
            catch (Exception ex)
            {
                errorinfo = string.Format("��{0},�޷�����ϴ�", ex.Message);
                return false;
            }
            finally
            {
                //frmbar.Dispose();
                //frmbar.Close();
            }
            
        }
        #endregion

        #region �����ļ�
        /// <summary>
        /// �����ļ�
        /// </summary>
        /// <param name="filename"></param>
        public bool Upload(string filename, long size, string path, out string errorinfo) //����Ĵ���ʵ���˴�ftp�����������ļ��Ĺ���
        {
            FrmProcessBar frmbar = new FrmProcessBar();
            frmbar.Show();
            path = path.Replace("\\", "/");
            FileInfo fileInf = new FileInfo(filename);
            //string uri = "ftp://" + path + "/" + fileInf.Name;
            string uri = "ftp://" + ftpServerIP + "/" + path + filename;
            Connect(uri);//����         
            // Ĭ��Ϊtrue�����Ӳ��ᱻ�ر�
            // ��һ������֮��ִ��
            reqFTP.KeepAlive = false;
            reqFTP.Timeout = 10000;
            // ָ��ִ��ʲô����         
            reqFTP.Method = WebRequestMethods.Ftp.AppendFile;
            // �ϴ��ļ�ʱ֪ͨ�������ļ��Ĵ�С
            reqFTP.ContentLength = fileInf.Length;
            // �����С����Ϊkb 
            long procMax = (fileInf.Length -size)/ 2048;
            frmbar.SetFrmProcessBarMax(procMax);
            frmbar.SetFrmProcessBarText("���������ļ���" + filename);
            int buffLength = 2048;
            byte[] buff = new byte[buffLength];
            int contentLen;
            // ��һ���ļ���(System.IO.FileStream) ȥ���ϴ����ļ�
            FileStream fs = fileInf.OpenRead();
            int value = 0;
            try
            {
                
                StreamReader dsad = new StreamReader(fs);
                fs.Seek(size, SeekOrigin.Begin);
                // ���ϴ����ļ�д����
                Stream strm = reqFTP.GetRequestStream();
                // ÿ�ζ��ļ�����kb
                contentLen = fs.Read(buff, 0, buffLength);
                // ������û�н���
                while (contentLen != 0)
                {
                    // �����ݴ�file stream д��upload stream 
                    value++;
                    frmbar.SetFrmProcessBarValue(value);
                    Application.DoEvents();
                    strm.Write(buff, 0, contentLen);
                    contentLen = fs.Read(buff, 0, buffLength);
                    
                }
                // �ر�������
                strm.Close();
                fs.Close();
                errorinfo = "Succeed";
                return true;
            }
            catch (Exception ex)
            {
                errorinfo = string.Format("��{0},�޷�����ϴ�", ex.Message);
                return false;
            }
            finally
            {
                if (null != fileInf)
                    fileInf = null;
                frmbar.Close();
                frmbar.Dispose();
            }
        }
        #endregion

        #region �����ļ�
        /// <summary>
        /// �����ļ�
        /// </summary>
        /// <param name="ftpfilepath">��Ը�Ŀ¼���ļ�����·��</param>
        /// <param name="filePath">���ر����ļ�������·��</param>
        /// <param name="fileName">�ļ���</param>
        /// <returns></returns>
        public bool Download(string ftpfilepath, string filePath, string fileName, out string errorinfo)////����Ĵ���ʵ���˴�ftp�����������ļ��Ĺ���
        {
            Exception ex = null;
            errorinfo="";
            FrmProcessBar ProcBar = new FrmProcessBar();
            ProcBar.Show();
            try
            {
                filePath = filePath.Replace("�ҵĵ���\\", "");
                String onlyFileName = Path.GetFileName(fileName);
                string newFileName = filePath;
                if (File.Exists(newFileName))
                {
                    errorinfo = string.Format("�����ļ�{0}�Ѵ���,�޷�����", newFileName);
                    return false;
                }
                ftpfilepath = ftpfilepath.Replace("\\", "/");
                string url = "ftp://" + ftpServerIP + "/" + ftpfilepath;
                Connect(url);//���� 
                reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                reqFTP.KeepAlive = false;
                reqFTP.Timeout = 10000;
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                Stream ftpStream = response.GetResponseStream();
                long cl = response.ContentLength;
                int bufferSize = 2048;

                int readCount;
                byte[] buffer = new byte[bufferSize];
                readCount = ftpStream.Read(buffer, 0, bufferSize);
                FileStream outputStream = new FileStream(newFileName, FileMode.Create);
                long ProcBarValue = 0;
                //////
                long all = GetFileSize(ftpfilepath, out ex);
                {
                    if (null != ex) return false;
                }
                ProcBar.SetFrmProcessBarMax(all / 2048);
                //////
                while (readCount > 0)
                {
                    ProcBarValue++;
                    ProcBar.SetFrmProcessBarText("���������ļ���" + fileName);
                    ProcBar.SetFrmProcessBarValue(ProcBarValue);
                    Application.DoEvents();
                    outputStream.Write(buffer, 0, readCount);
                    readCount = ftpStream.Read(buffer, 0, bufferSize);
                }
                ftpStream.Close();
                outputStream.Close();
                response.Close();
                errorinfo = "Succeed";
                return true;

            }
            catch (Exception exx)
            {
                errorinfo = string.Format("��{0},�޷�����", exx.Message);
                return false;
            }
            finally
            {
                ProcBar.Dispose();
                ProcBar.Close();
            }
           
        }
        #endregion

        #region ɾ���ļ�
        /// <summary>
        /// ɾ���ļ�
        /// </summary>
        /// <param name="fileName">��Ը�Ŀ¼�µ��ļ�����·��</param>
        public void DeleteFileName(string fileName,out string error)
        {
            try
            {
                error="";
                // FileInfo fileInf = new FileInfo(fileName);
                string uri = "ftp://" + ftpServerIP + "/" + fileName;
                Connect(uri);//����         
                // Ĭ��Ϊtrue�����Ӳ��ᱻ�ر�
                // ��һ������֮��ִ��
                reqFTP.KeepAlive = false;
                reqFTP.Timeout = 10000;
                // ָ��ִ��ʲô����
                reqFTP.Method = WebRequestMethods.Ftp.DeleteFile;
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();

                response.Close();
                error="Succeed";
            }
            catch (Exception ex)
            {
                error=ex.Message;
            }
        }
        #endregion

        #region ��ftp�ϴ���Ŀ¼
        /// <summary>
        /// ��ftp�ϴ���Ŀ¼
        /// </summary>
        /// <param name="dirName">��Ը�Ŀ¼�µ������ļ���·��</param>
        public void MakeDir(string dirName,out string error)
        {
            error = "";
            try
            {
                string uri = "ftp://" + ftpServerIP + "/" + dirName;
                Connect(uri);//����      
                reqFTP.Method = WebRequestMethods.Ftp.MakeDirectory;
                reqFTP.Timeout = 10000;
                reqFTP.KeepAlive = false;
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                response.Close();
                error = "Succeed";
            }
            catch (Exception ex)
            {
                error = string.Format("��{0},�޷�����", ex.Message);
            }
        }
        #endregion

        #region ɾ��ftp��Ŀ¼
        /// <summary>
        /// ɾ��ftp��Ŀ¼
        /// </summary>
        /// <param name="dirName">��Ը�Ŀ¼�µ����������ļ���·��</param>
        public void delDir(string dirName, out string Error)
        {
            try
            {
                string uri = "ftp://" + ftpServerIP + "/" + dirName;
                Connect(uri);//����      
                reqFTP.Method = WebRequestMethods.Ftp.RemoveDirectory;
                reqFTP.Timeout = 10000;
                reqFTP.KeepAlive = false;
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                response.Close();
                Error = "Succeed";
            }
            catch (Exception ex)
            {
                // MessageBox.Show(ex.Message);
                Error = string.Format("��{0},�޷�����", ex.Message);
            }
        }
        #endregion

        #region ���ftp���ļ���С
        /// <summary>
        /// ���ftp���ļ���С
        /// </summary>
        /// <param name="filename">��Ը�Ŀ¼�µ������ļ�·��</param>
        /// <returns></returns>
        public long GetFileSize(string filename,out Exception ex)
        {
            long fileSize = 0;
            ex = null;
            filename = filename.Replace("\\", "/");
            try
            {
                // FileInfo fileInf = new FileInfo(filename);
                //string uri1 = "ftp://" + ftpServerIP + "/" + fileInf.Name;
                // string uri = filename;
                string uri = "ftp://" + ftpServerIP + "/"+filename;
                Connect(uri);//����      
                reqFTP.Method = WebRequestMethods.Ftp.GetFileSize;
                reqFTP.Timeout = 10000;
                reqFTP.KeepAlive = false;
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                fileSize = response.ContentLength;
                response.Close();
            }
            catch (Exception newex)
            {
                ex = newex;
                return -1;
            }
            return fileSize;
        }
        #endregion

        #region ftp���ļ�����
        /// <summary>
        /// ftp���ļ�����
        /// </summary>
        /// <param name="currentFilename">��Ը�Ŀ¼�µ������ļ���</param>
        /// <param name="newFilename">�µ��ļ���</param>
        public void Rename(string currentFilename, string newFilename,out string error)
        {
            try
            {
                error="";
                //FileInfo fileInf = new FileInfo(currentFilename);
                string uri = "ftp://" + ftpServerIP + "/" + currentFilename;
                Connect(uri);//����
                reqFTP.Method = WebRequestMethods.Ftp.Rename;
                reqFTP.KeepAlive = false;
                reqFTP.Timeout = 10000;
                reqFTP.RenameTo = newFilename;
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                //Stream ftpStream = response.GetResponseStream();
                //ftpStream.Close();
                response.Close();
                error="Succeed";
            }
            catch (Exception ex)
            {
                error=ex.Message;
            }
        }
        #endregion

        #region ����ļ�����
        /// <summary>
        /// ����ļ�����
        /// </summary>
        /// <returns></returns>
        public string[] GetFilesDetailList(out string error)
        {
            return GetFileList("ftp://" + ftpServerIP + "/", WebRequestMethods.Ftp.ListDirectoryDetails,out error);
        }
        /// <summary>
        /// ����ļ�����
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public string[] GetFilesDetailList(string path,out string error)
        {
            path = path.Replace("\\", "/");
            return GetFileList("ftp://" + path, WebRequestMethods.Ftp.ListDirectoryDetails,out error);
        }
        #endregion

        #region ����ļ��У���������Ŀ¼��
        /// <summary>
        /// ���ָ����·���µ��ļ���,Path��Ŀ¼��·��
        /// </summary>
        /// <returns></returns>
        public string[] GetFloder(string Path,out string error)
        {
            string[] FileDetial;
            string[] Floder=null;//���ڷ���
            bool Isnull = false;
            StringBuilder result = new StringBuilder();
            try
            {
                error = "";
                if ("" == Path || null == Path)
                    FileDetial = GetFileList("ftp://" + ftpServerIP + "/", WebRequestMethods.Ftp.ListDirectoryDetails,out error);
                else
                    FileDetial = GetFileList("ftp://" + ftpServerIP + "/" + Path + "/", WebRequestMethods.Ftp.ListDirectoryDetails, out error);
                if (null != FileDetial || 0 > FileDetial.Length)
                {
                    foreach (string FileStr in FileDetial)
                    {
                        string File = FileStr.Trim();
                        string newfileinfo = File.Substring(0, 3);
                        if ("drw" == newfileinfo)
                        {
                            int strl = File.Length;
                            int indexm = File.LastIndexOf(':');
                            int start = indexm + 4;
                            int end = strl - start;
                            string floder = File.Substring(start, end);
                            if ("." == floder || ".." == floder)
                                continue;
                            else
                            {
                                result.Append(floder);
                                result.Append("\n");
                                Isnull = true;
                            }
                        }


                    }

                    if (Isnull)
                    {
                        result.Remove(result.ToString().LastIndexOf('\n'), 1);
                        Floder = result.ToString().Split('\n');
                    }


                }
                error = "Succeed";
            }
                
            catch (Exception ex)
            {
                error=ex.Message;
                Isnull = false;
            }

            if (true == Isnull)
                return Floder;
            else
                return null;
            
        }

        // *----------------------------------------------------------------------------------
        // *�� �� �ߣ����Ƿ� 
        // *����ʱ�䣺20110616
        // *���ܺ��������FTP��ָ���ļ������������Ŀ¼�Լ���Ŀ¼
        // *��    ����ָ�����ļ���Ŀ¼��true���Ƿ����ָ���ļ���Ŀ¼�µ��ļ��У����쳣
        // *------------------------------------------------------------------------------------
        public List<string> GetSubDirectory(string path, bool beGetSubDir, out Exception pError)
        {
            pError = null;
            string[] FileDetial;
            string error = "";
            List<string> LstDir = new List<string>();
            try
            {
                error = "";
                if (beGetSubDir)
                {
                    if ("" == path || null == path)
                        FileDetial = GetFileList("ftp://" + ftpServerIP + "/", WebRequestMethods.Ftp.ListDirectoryDetails, out error);
                    else
                        FileDetial = GetFileList("ftp://" + ftpServerIP + "/" + path + "/", WebRequestMethods.Ftp.ListDirectoryDetails, out error);
                    if (error != "Succeed")
                    {
                        pError = new Exception("��ȡ�ļ�Ŀ¼�б�ʧ��");
                        return null;
                    }
                    if (null != FileDetial || 0 > FileDetial.Length)
                    {
                        foreach (string FileStr in FileDetial)
                        {
                            string File = FileStr.Trim();
                            string newfileinfo = File.Substring(0, 3);
                            if ("drw" == newfileinfo)
                            {
                                int strl = File.Length;
                                int indexm = File.LastIndexOf(':');
                                int start = indexm + 4;
                                int end = strl - start;
                                string floder = File.Substring(start, end);
                                if ("." == floder || ".." == floder)
                                    continue;
                                else
                                {
                                    if (path != "")
                                    {
                                        if (!LstDir.Contains(path + "/" + floder))
                                        {
                                            LstDir.Add(path + "/" + floder);
                                        }
                                    }
                                    else
                                    {
                                        if (!LstDir.Contains(floder))
                                        {
                                            LstDir.Add(floder);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                if (LstDir != null)
                {
                    if (LstDir.Count == 0)
                    {
                        beGetSubDir = false;
                    }
                    else
                    {
                        beGetSubDir = true;
                        int pCount = LstDir.Count;
                        for (int j = 0; j < pCount; j++)
                        {
                            string pSubDir = LstDir[j];
                            List<string> tempSubDirLst = new List<string>();
                            tempSubDirLst = GetSubDirectory(pSubDir, beGetSubDir, out pError);
                            if (pError != null)
                            {
                                pError = new Exception("��ȡ�ļ�Ŀ¼�б�ʧ��");
                                return null;
                            }
                            if (tempSubDirLst != null)
                            {
                                if (tempSubDirLst.Count > 0)
                                {
                                    LstDir.AddRange(tempSubDirLst);
                                }
                            }
                        }
                    }
                }
               
                return LstDir;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return null;
            }
            
        }

        #endregion

        public void Close()
        {
            if (null != this.reqFTP)
            {
                reqFTP.Abort();
                reqFTP = null;
            }
        }
 
    }
}
