using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows;
using System.Runtime.ExceptionServices;
using System.Diagnostics; //Trace

namespace fSort
{
    public class Log
    {
        public static string logLocation = System.IO.Path.GetTempPath();
        public static string logFoldername = "fSort";
        public static string logFilename = String.Format("{0:0000}-{1:00}-{2:00}T{3:00}{4:00}.log", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute);

        public static bool initialize()
        {
            try
            {
                string logFolderPathname = logLocation + logFoldername;
                if (!Directory.Exists(logFolderPathname))
                {
                    Directory.CreateDirectory(logFolderPathname);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("[Log Initialize Failed] " + e.Message);
                return false;
            }
            return true;
        }
        public static void Debug(string message) { Trace.WriteLine(message); Trace.Flush();  } //Console.WriteLine(message);
        public static void Error(string message, bool TraceAlso = true)
        {
            message = string.Format("[{0}]{1}",DateTime.Now.ToString(), message);
            if (TraceAlso) Debug(message);
            
            using (var writer = new StreamWriter(logLocation + "\\" + logFoldername + "\\" + logFilename, true))
            {
                writer.WriteLine(message);
            }
        }
    }
    public class Util
    {
        public static void WalkDirectoryTree(System.IO.DirectoryInfo root, Func<System.IO.FileInfo, bool> GotFile, Func<System.IO.DirectoryInfo, bool> GotFolder = null)
        {
            System.IO.FileInfo[] files = null;
            System.IO.DirectoryInfo[] subDirs = null;

            // First, process all the files directly under this folder
            try
            {
                files = root.GetFiles("*.*");
            }
            // This is thrown if even one of the files requires permissions greater
            // than the application provides.
            catch (UnauthorizedAccessException)
            {
                // This code just writes out the message and continues to recurse.
                // You may decide to do something different here. For example, you
                // can try to elevate your privileges and access the file again.

                //log.Add(e.Message);
            }
            catch (System.IO.DirectoryNotFoundException e)
            {
                Console.WriteLine(e.Message);
            }

            if (files != null) //Empty array as an empty folder in normal case.
            {
                bool isContinue = true;

                if (GotFolder != null)
                {
                    GotFolder(root);
                }

                foreach (System.IO.FileInfo fi in files)
                {
                    // In this example, we only access the existing FileInfo object. If we
                    // want to open, delete or modify the file, then
                    // a try-catch block is required here to handle the case
                    // where the file has been deleted since the call to TraverseTree().
                    // Console.WriteLine(fi.FullName);
                    isContinue = GotFile(fi);
                    if (!isContinue) break;
                }

                if (isContinue)
                {
                    // Now find all the subdirectories under this directory.
                    subDirs = root.GetDirectories();

                    foreach (System.IO.DirectoryInfo dirInfo in subDirs)
                    {
                        // Resursive call for each subdirectory.
                        WalkDirectoryTree(dirInfo, GotFile, GotFolder);
                    }
                }
            }
        }
        public static EXIF GetExif(string filePathname)
        //public static DateTime? DateTaken(string filePathname)
        {
            //return DateTaken(System.Drawing.Image.FromFile(filePathname));
            EXIF exif = new EXIF();
            System.Drawing.Image img = null;

            try
            {
                img = System.Drawing.Image.FromFile(filePathname);
                exif.ExifDTOriginal = DateTaken(img, filePathname);
                if (exif.ExifDTOriginal == null)
                {
                    Log.Debug(filePathname + " has NO EXIF data");
                }
                else
                {
                    int Model = 0x0110;//Equipment model
                                       //0x010F;//Equipment manufacturer : Company
                    if (img.PropertyIdList.Contains(Model))
                    {
                        string modelTag = System.Text.Encoding.ASCII.GetString(img.GetPropertyItem(Model).Value);
                        modelTag = modelTag.TrimEnd('\0');
                        modelTag = modelTag.TrimEnd();
                        exif.EquipmentModel = modelTag;

                        //Console.Write("Model: " + modelTag);
                        //if (modelTag.IndexOf("9GF00") >= 0)
                        //{
                        //    Console.WriteLine(".<= " + filePathname);
                        //}
                        //else Console.WriteLine(".");
                    }
                }
            }
            catch (OutOfMemoryException e)
            {
                Log.Error("[OutOfMemoryException] " + filePathname + " : " + e.Message);
                //exif.ExifDTOriginal = null;
                exif = null;
            }
            finally
            {
                if (img != null)
                {
                    img.Dispose();
                }
            }

            return exif;
        }
        public static DateTime? DateTaken(System.Drawing.Image getImage, string fName = "")
        {
            DateTime? ret = null;
            int DateTakenValue = 0x9003; //36867; #ExifDTOriginal

            if (!getImage.PropertyIdList.Contains(DateTakenValue))
                return ret;

            int year = 1901, month = 1, day = 1, hour = 0, minute = 0, second = 0;
            try
            {
                string dateTakenTag = System.Text.Encoding.ASCII.GetString(getImage.GetPropertyItem(DateTakenValue).Value);
                string[] parts = dateTakenTag.Split(':', ' ');

                //if (dateTakenTag.Any(x => x == ':'))
                if (parts.Length == 2)
                {
                    year = int.Parse(parts[0].Substring(0, 4));
                    month = int.Parse(parts[0].Substring(4, 2));
                    day = int.Parse(parts[0].Substring(6, 2));
                    hour = int.Parse(parts[1].Substring(0, 2));
                    minute = int.Parse(parts[1].Substring(2, 2));
                    second = int.Parse(parts[1].Substring(4, 2));
                }
                else //if (parts.Length == 6)
                {
                    year = int.Parse(parts[0]);
                    month = int.Parse(parts[1]);
                    day = int.Parse(parts[2]);
                    hour = int.Parse(parts[3]);
                    minute = int.Parse(parts[4]);
                    second = int.Parse(parts[5]);
                }

                if (year != 0 && month != 0 && day != 0)
                {
                    ret = new DateTime(year, month, day, hour, minute, second);
                }
            }
            catch (Exception e)
            {
                Log.Error("[DateTaken Failed] " + e.Message + "\n(" + fName + ")");
            }
            return ret;
        }
        //void test()
        //{
        //    private DateTime getDateTaken(string inFullPath)
        //    {
        //        DateTime returnDateTime = DateTime.MinValue;
        //        try {
        //            FileStream picStream = new FileStream(inFullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
        //            BitmapSource bitSource = BitmapFrame.Create(picStream);
        //            picStream.Close();
        //            BitmapMetadata metaData = (BitmapMetadata)bitSource.Metadata;
        //            returnDateTime = DateTime.Parse(metaData.DateTaken);
        //        }
        //        catch
        //        {
        //            //do nothing 
        //        } return returnDateTime;
        //    }
        //}
        string TakeOffFinalSlash(string url)
        {
            url = url.EndsWith("\\") ? url.Substring(0, url.Length - 1) : url;
            return url;
        }

        public static string CreateFileMD5(string FilePathname)
        {
            string MD5Hash = "";
            using (FileStream fs = new FileStream(FilePathname, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
                byte[] hash = md5.ComputeHash(fs);
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hash.Length; i++)
                {
                    sb.Append(hash[i].ToString("X2"));
                }
                MD5Hash = sb.ToString();
            }
            return MD5Hash;
        }

        public static string CreateFileSHA256(string FilePathname)
        {
            string HashRes = "";
            using (FileStream fs = new FileStream(FilePathname, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                System.Security.Cryptography.SHA256 encode = System.Security.Cryptography.SHA256.Create();
                byte[] hash = encode.ComputeHash(fs);
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hash.Length; i++)
                {
                    sb.Append(hash[i].ToString("X2"));
                }
                HashRes = sb.ToString();
            }
            return HashRes;
        }

        public static string GetHash_WithGenToHashInfoFile(string file)
        {
            string theHash = null;
            string fileName = System.IO.Path.GetFileName(file).Trim().ToUpperInvariant();
            string hashInfo_FilePathname = System.IO.Path.GetDirectoryName(file) + "\\hash.txt";            

            theHash = Util.CreateFileSHA256(file);
            string[] content = { };

            if (File.Exists(hashInfo_FilePathname))
            {
                string oldHash = null;
                using (var reader = new StreamReader(hashInfo_FilePathname)) { content = reader.ReadToEnd().Split('\n'); };
                int index = 0;
                for (; index < content.Length; index++)
                {
                    var line = content[index];

                    string[] pair = line.Split('=');
                    if (pair[0] == fileName)
                    {
                        oldHash = pair[1];
                        break;
                    }
                }
                if (oldHash != null && oldHash != theHash)
                {
                    content[index] = fileName + "=" + theHash;
                    //using (var writer = new StreamWriter(hashInfo_FilePathname, true)) { writer.WriteLine(content); };

                    string fileContent = string.Join("\n", content);
                    File.WriteAllText(hashInfo_FilePathname, fileContent);
                }
            }
            else
            {
                File.WriteAllText(hashInfo_FilePathname, fileName + "=" + theHash);
            }

            return theHash;
        }
        public static string GetHash_FromHashInfoFile(string file)
        {
            string theHash = null;
            string fileName = System.IO.Path.GetFileName(file).Trim().ToUpperInvariant();
            string hashInfo_FilePathname = System.IO.Path.GetDirectoryName(file) + "\\hash.txt";
                //System.IO.Path.ChangeExtension(file, ".hash");
            if (File.Exists(hashInfo_FilePathname))
            {
                string[] content = { };
                using (var reader = new StreamReader(hashInfo_FilePathname)) { content = reader.ReadToEnd().Split('\n'); };
                foreach (var line in content)
                {
                    string[] pair = line.Split('=');
                    if (pair[0] == fileName)
                    {
                        theHash = pair[1];
                        break;
                    }
                }
            }
            return theHash;
        }
        public static string GetHash(string file)
        {
            string hashStr = GetHash_FromHashInfoFile(file);
            if (hashStr == null || hashStr == string.Empty)
            {
                hashStr = GetHash_WithGenToHashInfoFile(file);
            }
            return hashStr;
        }

        public static bool isTheSameFileByCHKHash(string src, string tar)
        {            
            //0. 建 hash 值, Copy後 OR 不存在。
            //1. 取 hash 值 (src/tar) 比對。
            return (GetHash(src) == GetHash(tar));
        }
        public static bool isTheSameFileByCHKBinary(string src, string tar)
        {
            bool FileStreamEquals(Stream stream1, Stream stream2)
            {
                const int bufferSize = 2048;
                byte[] buffer1 = new byte[bufferSize]; //buffer size
                byte[] buffer2 = new byte[bufferSize];
                while (true)
                {
                    int count1 = stream1.Read(buffer1, 0, bufferSize);
                    int count2 = stream2.Read(buffer2, 0, bufferSize);

                    if (count1 != count2)
                        return false;

                    if (count1 == 0)
                        return true;

                    // You might replace the following with an efficient "memcmp"
                    if (!buffer1.Take(count1).SequenceEqual(buffer2.Take(count2)))
                        return false;
                }
            }

            bool isTheSame = true;
            // Check the file size and CRC equality here..
            using (var file1 = new FileStream(src, FileMode.Open))
            using (var file2 = new FileStream(tar, FileMode.Open))
                isTheSame = FileStreamEquals(file1, file2);

            return isTheSame;
        }
    
        public static bool isTheSameTextFileByCHKBinary(string src, string tar)
        {
            bool isTheSame = true;

            string[] c1 = { }, c2 = { };
            using (var reader = new StreamReader(src))
            {
                c1 = reader.ReadToEnd().Split('\n');
            }
            using (var reader = new StreamReader(tar))
            {
                c2 = reader.ReadToEnd().Split('\n');
            }

            if (c1.Length == c2.Length)
            {
                int i = 0;
                for (; i < c1.Length; i++)
                {
                    if (c1[i] != c2[i])
                    {
                        isTheSame = false;
                        break;
                    }
                }
            }
            else
            {
                isTheSame = false;
            }

            return isTheSame;
        }

        public static bool ifHasTheSameFile(string src, string tar, bool isBinaryChk = false)
        {
            bool isTheSame = false;

            if (src == tar)
                isTheSame = true;
            else
            {
                if (File.Exists(tar))
                {
                    if (isBinaryChk)
                    {
                        //isTheSame = Util.isTheSameFileByCHKBinary(src, tar);
                        isTheSame = Util.isTheSameFileByCHKHash(src, tar);
                    }
                    else
                    {
                        FileInfo srcFi = new System.IO.FileInfo(src);
                        FileInfo tarFi = new System.IO.FileInfo(tar);
                        //compare file size
                        if (srcFi.Length == tarFi.Length && srcFi.LastWriteTime == tarFi.LastWriteTime)
                        {
                            isTheSame = true;

                            if (EXIF.FileExtensions.Contains(srcFi.Extension.ToLowerInvariant()))
                            {
                                //compare date-time exif
                                EXIF srcExif = Util.GetExif(src);
                                EXIF tarExif = Util.GetExif(tar);
                                if (srcExif != null && tarExif != null)
                                {
                                    if (srcExif.ExifDTOriginal != null && tarExif.ExifDTOriginal != null)
                                    {
                                        if (!srcExif.ExifDTOriginal.Equals(tarExif.ExifDTOriginal))
                                        {
                                            isTheSame = false;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return isTheSame;
        }
        public static void ExplorerFileFolder(string FilePathname)
        {
            if (!File.Exists(FilePathname) && !Directory.Exists(FilePathname))
            {
                do
                {
                    FilePathname = System.IO.Path.GetDirectoryName(FilePathname);
                } while (!Directory.Exists(FilePathname));
            }
            if (FilePathname != null)
            {
                string TheExplorer = @"C:\Windows\explorer.exe";
                string argument = @"/select, " + FilePathname;
                System.Diagnostics.Process.Start(TheExplorer, argument);
            }
            else
            {
                MessageBox.Show("所選位置不可用。", "INFO", MessageBoxButton.OK);
            }
        }
        public static void NotepadOpenFile(string FilePathname)
        {
            if (!File.Exists(FilePathname))
            {
                MessageBox.Show("所選檔案不存在。", "ERROR", MessageBoxButton.OK);
                return;
            }

            string TheApp = Environment.GetEnvironmentVariable("windir") + @"\system32\notepad.exe";//@"C:\Windows\explorer.exe";
            string argument = FilePathname;
            System.Diagnostics.Process.Start(TheApp, argument);
        }

        public static string LogFileCreate(string FileName)
        {
            bool isWritable(string FilePathname)
            {
                bool ret = true;
                bool fileAlreadyExist = File.Exists(FilePathname);

                StreamWriter sw = null;
                try
                {
                    sw = new StreamWriter(FilePathname);
                    //sw.WriteLine();
                }
                catch
                {
                    ret = false;
                }
                finally
                {
                    if (sw != null) sw.Close();
                }

                if (!fileAlreadyExist && File.Exists(FilePathname))
                {
                    File.Delete(FilePathname);
                }
                return ret;
            }
            string Report_FilePathame = FileName;
            if (!isWritable(Report_FilePathame))
            {
                Report_FilePathame = System.IO.Path.GetTempPath();
                if (!isWritable(Report_FilePathame))
                {
                    Report_FilePathame = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                }
            }
            return Report_FilePathame;
        }

        public static void StringListToFile(string filePathName, List <string> StrList)
        {
            StreamWriter swWriter = null;
            try
            {
                swWriter = new StreamWriter(filePathName);
                foreach (string Text in StrList)
                {
                    swWriter.WriteLine(Text);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("記錄檔寫入失敗:\n[名稱] " + filePathName + "\n[錯誤] " + e.Message);
            }
            finally
            {
                if (swWriter != null)
                {
                    swWriter.Close();
                    swWriter.Dispose();
                }
            }
        }
    }

    public class EXIF
    {
        //Exif可以附加於JPEG、TIFF、RIFF等檔案(維基百科)
        public static string[] FileExtensions = { ".jpg", ".jpeg", ".tif", ".tiff", ".rif", ".riff" };

        public string EquipmentModel = "";
        public DateTime? ExifDTOriginal = null;
    }

    public class MyFolderInfo
    {
        public class InfoData : IEquatable<InfoData>
        {
            public List<string> FolderInfo = new List<string>();
            public string FolderPathName = string.Empty;

            public bool Equals(InfoData other)
            {
                bool contained = false;
                if (this.FolderPathName == other.FolderPathName)
                {
                    foreach (string info in other.FolderInfo)
                    {
                        if (this.FolderInfo.Contains<string>(info))
                        {
                            contained = true;
                            break;
                        }
                    }
                }
                return contained;
            }
        }

        string InfoFilename = "Info.txt";
        List<InfoData> History = new List<InfoData>();
        int HistoryLimit = 5;

        public MyFolderInfo(int limit = 5, string filename = "Info.txt")
        {
            HistoryLimit = limit;
            InfoFilename = filename;
        }

        public void CopyIfNotExist(string srcFolderPathName, string tarFolderPathName)
        {
            string srcInfoFilePathname = srcFolderPathName + "\\" + InfoFilename;
            string tarInfoFilePathname = tarFolderPathName + "\\" + InfoFilename;
            if (File.Exists(srcInfoFilePathname))
            {
                if (!File.Exists(tarInfoFilePathname))
                {
                    File.Copy(srcInfoFilePathname, tarInfoFilePathname);
                }
            }
        }
        public bool isJustRecorded(string srcFolderInfo, string tarFolderPathName, bool checkWithFile = true)
        {
            bool hasDone = false;
            //1. Check from history cache.
            if (History.Contains(new InfoData() {
                FolderPathName = tarFolderPathName.Trim(),
                FolderInfo = { srcFolderInfo.Trim() },
            }))
            {
                hasDone = true;
            } 
            else
            {
                if (checkWithFile)
                {
                    //2. Check from target file.
                    hasDone = isRecordedInFile(srcFolderInfo, tarFolderPathName);
                }
            }

            return hasDone;
        }
        public bool isRecordedInFile(string srcFolderInfo, string tarFolderPathName)
        {
            if (srcFolderInfo.Trim() == string.Empty) return true;

            string InfoFilePathname = tarFolderPathName + "\\" + InfoFilename;
            bool hasDone = false;
            InfoData iData = new InfoData()
            {
                FolderPathName = tarFolderPathName.Trim(),
                FolderInfo = { },
            };

            if (File.Exists(InfoFilePathname))
            {
                string[] content = { };
                using (var reader = new StreamReader(InfoFilePathname))
                {
                    content = reader.ReadToEnd().Split('\n');
                }
                foreach (string ln in content)
                {
                    string item = ln.Trim();
                    iData.FolderInfo.Add(item);

                    if (!hasDone)
                    {
                        if (item == srcFolderInfo)
                        {
                            hasDone = true;
                            //break;
                        }
                    }
                }
                
                HistoryAddItem(iData);
            }
            return hasDone;
        }
        public void Record(string srcFolderInfo, string tarFolderPathName)
        {
            string InfoFilePathname = tarFolderPathName + "\\" + InfoFilename;
            using (var writer = new StreamWriter(InfoFilePathname, true))
            {
                writer.WriteLine(srcFolderInfo);
            }
            HistoryAddItem(new InfoData()
            {
                FolderPathName = tarFolderPathName.Trim(),
                FolderInfo = { srcFolderInfo.Trim() },
            });
        }
        void HistoryAddItem(InfoData item)
        {
            bool hasDone = false;
            foreach (var iData in History)
            {
                if (iData.FolderPathName == item.FolderPathName)
                {
                    foreach (string info in item.FolderInfo)
                    {
                        if (!iData.FolderInfo.Contains<string>(info))
                        {
                            iData.FolderInfo.Add(info);
                        }
                    }
                    hasDone = true;
                    break;
                }
            }
            if (!hasDone)
            {
                History.Add(item);
                if (History.Count > HistoryLimit)
                {
                    History.RemoveAt(0);
                }
            }
        }
    }//public class MyFolderInfo

    public class ErrorHandle
    {
        private static string _app_name = "crash";
        private static string _app_ver = String.Empty;

        private static string _exception_msg = string.Empty;
        private static Type _first_execption_type = typeof(object);
        private static string _first_exception_msg = string.Empty;
        
        //public static void RegCatchException(string appname)
        public static void Register(string appname)
        {
            _app_name = appname;

            var obj = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            string version = string.Format("Ver {0}.{1}.{2}.{3}", obj.Major, obj.Minor, obj.Build, obj.Revision);
            _app_ver = version;

            AppDomain.CurrentDomain.FirstChanceException += new EventHandler<FirstChanceExceptionEventArgs>(FirstChanceHandler);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(UnhandledExeceptionHandler);
        }

        static void FirstChanceHandler(object source, FirstChanceExceptionEventArgs arg)
        {
            Console.WriteLine("FirstChanceException event raised in {0}: {1}",
                AppDomain.CurrentDomain.FriendlyName, arg.Exception.Message);

            bool dump = true;

            if (null != _first_execption_type)
            {
                if (_first_execption_type == arg.Exception.GetType() &&
                    String.Compare(_exception_msg, arg.Exception.Message) == 0)
                {
                    return;
                }
            }

            System.Text.StringBuilder msg = new System.Text.StringBuilder();
            System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace();
            string stackmsg = st.ToString();

            if (arg.Exception.GetType() == typeof(System.IO.IOException))
            {
                if (-1 != stackmsg.IndexOf("LocalDataKit.IsFileLocked("))
                {
                    dump = false;
                    //Trace.WriteLine(arg.Exception.Message); Trace.Flush();
                    Log.Error(arg.Exception.Message);
                }
            }

            if (dump)
            {
                DateTime ts = DateTime.Now;
                msg.AppendLine(String.Format("{0} App {1} {2} Crash", ts.ToString(), _app_name, _app_ver));
                msg.AppendLine();
                msg.AppendLine(arg.Exception.GetType().FullName);
                msg.AppendLine(arg.Exception.Message);
                msg.AppendLine(stackmsg);
                msg.AppendLine();

                _first_execption_type = arg.Exception.GetType();
                _exception_msg = arg.Exception.Message;
                _first_exception_msg = msg.ToString();

                //Trace.WriteLine(msg);Trace.Flush();
                Log.Error(_first_exception_msg);
            }
        }

        static void UnhandledExeceptionHandler(object source, UnhandledExceptionEventArgs arg)
        {
            Console.WriteLine("UnhandledException event raised in {0}: {1}",
                AppDomain.CurrentDomain.FriendlyName, arg.ExceptionObject.GetType());

            DateTime ts = DateTime.Now;
            string dumpmsg = _first_exception_msg;
            _first_exception_msg = string.Empty;

            if (arg.ExceptionObject.GetType() != _first_execption_type && string.Empty == dumpmsg)
            {
                System.Text.StringBuilder msg = new System.Text.StringBuilder();
                msg.AppendLine(String.Format("{0} App {1} {2} Crash", ts.ToString(), _app_name, _app_ver));
                msg.AppendLine();
                msg.AppendLine(arg.ExceptionObject.GetType().FullName);
                System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace();
                msg.AppendLine(st.ToString());
                msg.AppendLine();

                dumpmsg = msg.ToString();
            }

            //Trace.WriteLine(dumpmsg); Trace.Flush();
            Log.Error(dumpmsg);
        }
    }
}
