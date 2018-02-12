using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
//
using System.Threading;
using System.IO;
using System.Drawing;

namespace fSort
{
    /// <summary>
    /// ProcessWindow.xaml 的互動邏輯
    /// </summary>
    public partial class ProcessWindow : Window
    {
        public UserProject project;
        RunOption stepControl = new RunOption();

        public ProcessWindow()
        {
            InitializeComponent();
        }

        public async void ProgressStart()
        {
            DateTime ST = DateTime.Now;
            LB_SpendTime.Content = string.Format("{0};   始:{1}, 終: {2}", 0, ST.ToString("HH:mm"), "進行中...");
            await AsyncProgress();
            
            
            DateTime ET = DateTime.Now;
            TimeSpan TS = ET - ST;
            string ts = TS.ToString(@"dd'日'hh'小時'mm'分'ss'秒'");           
            LB_SpendTime.Content = string.Format("{0};   始:{1}, 終: {2}", ts, ST.ToString("HH:mm"), ET.ToString("HH:mm"));

            LB_DamagedFileCount.Content = stepControl.Report_DamagedFileCount;
            LB_IgnoredFileCount.Content = stepControl.Report_IgnoredFileCount;
            LB_CopyFailedFileCount.Content = stepControl.Report_CopyFailedFileCount;

            BTN_FlowControl.Content = "結束";
        }
        Task AsyncProgress()
        {            
            //return Task.Run(() => { Thread.Sleep(1000 * 2); }); 
            Process p = new Process()
            {
                project = project,
            };
            p.ProgressUpdate += (s, e) => Dispatcher.Invoke((Action)delegate ()
            {
                // update UI
                ProcessListBox.Items.Add(e);
                ProcessListBox.SelectedIndex = ProcessListBox.Items.Count - 1;
                ProcessListBox.ScrollIntoView(ProcessListBox.SelectedItem);

                LB_ProcessCount.Content = ProcessListBox.Items.Count -1;

                Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Background,
                                      new Action(delegate { }));
            });

            string TimeStr = DateTime.Now.ToString("yyyyMMdd-HHmmss");
            stepControl.Report_DamagedFilePathame = Util.LogFileCreate(project.sourceRootPathname + @"\FileDamagedList-" + TimeStr + ".txt");
            stepControl.Report_IgnoredFilePathame = Util.LogFileCreate(project.sourceRootPathname + @"\FileIgnoredList-" + TimeStr + ".txt");
            stepControl.Report_CopyFailedFilePathame = Util.LogFileCreate(project.sourceRootPathname + @"\FileCopyFailedList-" + TimeStr + ".txt");
            
            return Task.Run(() => p.Run(stepControl)); 
        }

        private void BTN_FlowControl_Click(object sender, RoutedEventArgs e)
        {
            switch(BTN_FlowControl.Content.ToString())
            {
                case "暫停": //PAUSE
                    stepControl.Step_ForceProgressTo = "pause";                    
                    BTN_FlowControl.Content = "繼續";
                    break;
                case "繼續": //RESUME
                    stepControl.Step_ForceProgressTo = "resume";                    
                    BTN_FlowControl.Content = "暫停";
                    break;
                case "結束": //END
                    Close();
                    break;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //Cancel Stop if In Progress
            stepControl.Step_ForceProgressTo = "stop";
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Application curApp = Application.Current;
            Window mainWindow = curApp.MainWindow;
            this.Left = mainWindow.Left + (mainWindow.Width - this.ActualWidth) / 2;
            this.Top = mainWindow.Top + (mainWindow.Height - this.ActualHeight) / 2;
            //Update Message
            SettingsListBox.Items.Add("來源資料夾: " + project.sourceRootPathname);
            SettingsListBox.Items.Add("目標資料夾: " + project.targetRootPathname);
            SettingsListBox.Items.Add("動作: " + project.fAction);
            SettingsListBox.Items.Add("更名: " + project.isRename);
            SettingsListBox.Items.Add("包含檔案類型: " + string.Join(",", project.imageFileExts) + "," + string.Join(",", project.videoFileExts));

            //Start Process            
            ProgressStart();
        }

        private void ProcessListBox_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ProcessListBox.UnselectAll();
        }
        private void OpenFolder_Click(object sender, RoutedEventArgs e, bool isSrc = true)
        {
            if (ProcessListBox.SelectedIndex == -1)
            {
                return;
            }
            string InfoStr = ProcessListBox.SelectedItem.ToString();
            int bkIndex = InfoStr.IndexOf(PREDEF._ACTIONSIGNSTRING_);
            if (bkIndex > 0)
            {
                //string srcFilePathname = InfoStr.Substring(0, bkIndex);
                //string tarFilePathname = InfoStr.Substring(bkIndex + PREDEF._ACTIONSIGNSTRING_.Length);
                if (isSrc)
                    Util.ExplorerFileFolder(InfoStr.Substring(0, bkIndex));
                else
                    Util.ExplorerFileFolder(InfoStr.Substring(bkIndex + PREDEF._ACTIONSIGNSTRING_.Length));
            } else
            {
                MessageBox.Show("所選項目無可用資訊。","INFO",MessageBoxButton.OK);            
            }
        }
        private void MenuItemOpenSFolder_Click(object sender, RoutedEventArgs e)
        {
            OpenFolder_Click(sender, e);
        }
        private void MenuItemOpenTFolder_Click(object sender, RoutedEventArgs e)
        {
            OpenFolder_Click(sender, e, false);
        }

        private void BTN_ReportFileDamaged_Click(object sender, RoutedEventArgs e)
        {
            Util.NotepadOpenFile(stepControl.Report_DamagedFilePathame);
        }

        private void BTN_ReportFileIgnored_Click(object sender, RoutedEventArgs e)
        {
            Util.NotepadOpenFile(stepControl.Report_IgnoredFilePathame);
        }

        private void BTN_ReportCopyFailed_Click(object sender, RoutedEventArgs e)
        {
            Util.NotepadOpenFile(stepControl.Report_CopyFailedFilePathame);
        }        
    }
    class PREDEF
    {
        public const string _ACTIONSIGNSTRING_ = "  ====>  ";
    }
    class RunOption
    {
        public string Step_ForceProgressTo = "none";
        public string Report_DamagedFilePathame;
        public string Report_IgnoredFilePathame;
        public string Report_CopyFailedFilePathame;

        public int Report_DamagedFileCount = 0;
        public int Report_IgnoredFileCount = 0;
        public int Report_CopyFailedFileCount = 0;
    }
    class Process
    {
        public UserProject project;
        public event EventHandler<string> ProgressUpdate; 

        // 1. Search file with filename extension list
        // 2. Rebuild target file by format and exif data
        // 3. Search target location, create folder if not exist (Reference source folder name)
        // 4. Check file duplication, then ...? 
        //        duplication rule: same name with size and time, may binary.
        //        move to duplication folder, add dup number with dup the dup files. 
        // 5. Move /Copy file to target location
        // 6. [Move] also check and remove the empty folder. (Remove Thumb.db picasa.ini)
        public void Run(RunOption sc)
        {            
            if (project == null) return;
            //project = new UserProject();
            int nFileCheckCount = 0, nFileActCount = 0;
            MyFolderInfo folderInfo = new MyFolderInfo();

            StreamWriter swDamaged = new StreamWriter(sc.Report_DamagedFilePathame);
            swDamaged.WriteLine("[Start at " + DateTime.Now + "]");            
            StreamWriter swIgnored = new StreamWriter(sc.Report_IgnoredFilePathame);
            swIgnored.WriteLine("[Start at " + DateTime.Now + "]");
            swDamaged.AutoFlush = true;
            swIgnored.AutoFlush = true;

            StreamWriter swCopyFailed = null;
            if (project.isContentConfirmByComparedBinary && (project.fAction == UserProject.FileAction.Copy || project.fAction == UserProject.FileAction.CopyThenDelete))
            {
                swCopyFailed = new StreamWriter(sc.Report_CopyFailedFilePathame);
                swCopyFailed.WriteLine("[Start at " + DateTime.Now + "]");
                swCopyFailed.AutoFlush = true;
            }

            ForeachFolderFile((fi) => {
                //ProgressUpdate.Invoke(this, fi.FullName);
                nFileCheckCount++;
                string sourceFile = fi.FullName;
                string destinationFile = string.Empty, destinationDupFile = string.Empty, FileState = string.Empty;
                string CameraModel = string.Empty;
                DateTime fileDateTime = new DateTime();

                //support file Extensions
                //if (Array.IndexOf(project.imageFileExts, fi.Extension.ToLower()) >= 0)
                if (project.imageFileExts.Contains(fi.Extension.ToLowerInvariant()))
                {
                    fileDateTime = fi.LastWriteTime;

                    if (EXIF.FileExtensions.Contains(fi.Extension.ToLowerInvariant()))
                    {
                        EXIF exif = Util.GetExif(fi.FullName);
                        if (exif == null)
                        {
                            fileDateTime = new DateTime(); //Skip act with this file.
                            FileState = "檔案損毀";
                            swDamaged.WriteLine(fi.FullName);
                            sc.Report_DamagedFileCount++;
                        }
                        else
                        {
                            CameraModel = exif.EquipmentModel;
                            DateTime? dt = exif.ExifDTOriginal;
                            if (dt != null)
                            {
                                fileDateTime = (DateTime)dt;                                
                            }

                            FileState = fileDateTime.ToString();
                        }
                    }
                }
                else if (project.videoFileExts.Contains(fi.Extension.ToLowerInvariant()))
                //if (Array.IndexOf(project.videoFileExts, fi.Extension.ToLower()) >= 0)
                {
                    fileDateTime = fi.LastWriteTime;
                    //Console.WriteLine(">" + fi.FullName + fi.LastWriteTime.ToLocalTime().ToString());                    
                //} else if (fi.Name == "Info")
                //{
                //    //Merge to target .. But we don't know the target path!
                } else
                {
                    FileState = "忽略";
                    swIgnored.WriteLine(fi.FullName);
                    sc.Report_IgnoredFileCount++;
                }

                if (!fileDateTime.Equals(new DateTime()))
                {
                    //ProgressUpdate.Invoke(this, fi.FullName);
                    //Destination file folder and file name.                    
                    destinationFile = BuildDestinationFullFilePathname(fi, fileDateTime, project);
                    string destinationPath = System.IO.Path.GetDirectoryName(destinationFile);

                    //File Action
                    switch (project.fAction)
                    {
                        case UserProject.FileAction.CopyThenDelete:
                        case UserProject.FileAction.Copy:
                            bool isCopySuccess = false;
                            if (!Util.ifHasTheSameFile(sourceFile, destinationFile, project.isTheSameContentByComparedBinary)) 
                            {
                                if (!Directory.Exists(destinationPath))
                                {
                                    Directory.CreateDirectory(destinationPath);//create if folder not exist
                                }

                                System.IO.File.Copy(sourceFile, destinationFile);
                                Util.GetHash_WithGenToHashInfoFile(destinationFile);
                                isCopySuccess = true;
                            }

                            AddFolderInfo(folderInfo, sourceFile, destinationFile);

                            if (isCopySuccess)
                            {
                                if (project.isContentConfirmByComparedBinary)
                                {
                                    if (!Util.ifHasTheSameFile(sourceFile, destinationFile, true))
                                    {
                                        //Result is not the same!
                                        isCopySuccess = false;
                                        if (swCopyFailed != null)
                                        {
                                            swCopyFailed.WriteLine(sourceFile);
                                            sc.Report_CopyFailedFileCount++;
                                        }
                                    }
                                }
                                if (project.fAction == UserProject.FileAction.CopyThenDelete && isCopySuccess)
                                {
                                    System.IO.File.Delete(sourceFile);
                                }
                            }
                            break;
                        case UserProject.FileAction.Move:
                            if (Util.ifHasTheSameFile(sourceFile, destinationFile, project.isTheSameContentByComparedBinary)) 
                            {
                                //get Destination Duplicated foder name path
                                string phase2 = destinationFile.Substring(project.targetRootPathname.Length);
                                destinationFile = project.targetRootPathname + @"\Dup" + phase2;
                                destinationPath = System.IO.Path.GetDirectoryName(destinationFile);//Update new path
                            }
                            if (!Directory.Exists(destinationPath))
                            {
                                Directory.CreateDirectory(destinationPath);//create if folder not exist
                            }

                            MoveWithoutReplace(sourceFile, destinationFile, (CameraModel == string.Empty) ? "INCREASENUMBER" : CameraModel);
                            AddFolderInfo(folderInfo, sourceFile, destinationFile);
                            break;
                        case UserProject.FileAction.ListOnly:
                        default:
                            break;
                    }
                    nFileActCount++;
                    FileState = destinationFile;
                }
                ProgressUpdate.Invoke(this, sourceFile + PREDEF._ACTIONSIGNSTRING_ + FileState);

                //Leave progress controlable.
                bool ret = true;
                switch (sc.Step_ForceProgressTo)
                {
                    case "pause":
                        do
                        {
                            Thread.Sleep(1000);
                        } while (sc.Step_ForceProgressTo == "pause");                        
                        break;
                    case "resume":
                        break;
                    case "stop":
                        ret = false;
                        break;
                }
                return ret;
            });

            string FileEndStr = "[End at " + DateTime.Now + "]";
            swDamaged.WriteLine(FileEndStr);
            swDamaged.Close();
            swDamaged.Dispose();
            swIgnored.WriteLine(FileEndStr);
            swIgnored.Close();
            swIgnored.Dispose();
            if (swCopyFailed != null)
            {
                swCopyFailed.WriteLine(FileEndStr);
                swCopyFailed.Close();
                swCopyFailed.Dispose();
            }

            ProgressUpdate.Invoke(this, string.Format(@"Total Files: {0}, Total Act Filecount: {1}", 
                nFileCheckCount, 
                nFileActCount));
        }

        public static void AddFolderInfo(MyFolderInfo folderInfo, string sourceFile, string destinationFile)
        {
            string srcFolderPathName = System.IO.Path.GetDirectoryName(sourceFile);
            string tarFolderPathName = System.IO.Path.GetDirectoryName(destinationFile);
            folderInfo.CopyIfNotExist(srcFolderPathName, tarFolderPathName);

            //1. get src folder name.
            string srcInfoText = System.IO.Path.GetFileName(System.IO.Path.GetDirectoryName(sourceFile));//srcFolderName

            if (!folderInfo.isJustRecorded(srcInfoText, tarFolderPathName))
            {
                //2. append to "moreMediaInfo.txt" under tar folder.
                folderInfo.Record(srcInfoText, tarFolderPathName);
            }
        }

        void MoveWithoutReplace(string src, string tar, string appendStr = "INCREASENUMBER")
        {
            string newTar = tar,
                tarFullFolder = System.IO.Path.GetDirectoryName(tar), //Full path without final "\".
                tarFilename = System.IO.Path.GetFileNameWithoutExtension(tar),//file name only.
                tarExt = System.IO.Path.GetExtension(tar); //Include ".".

            if (File.Exists(newTar))
            {
                if (appendStr != "INCREASENUMBER")
                {                    
                    newTar = string.Format("{0}\\{1}-{2}{3}", tarFullFolder, tarFilename, appendStr, tarExt);
                }

                int Count = 0;
                while (File.Exists(newTar))
                {
                    newTar = string.Format("{0}\\{1}-{2}{3}", tarFullFolder, tarFilename, ++Count, tarExt);
                }
            }

            System.IO.File.Move(src, newTar);
        }
        string BuildDestinationFullFilePathname(System.IO.FileInfo sourceFi, DateTime fileDateTime, UserProject project)
        {
            string GetLevelName(int index)
            {
                string name = String.Empty;
                //[ 西元年分 月份 日期 (無) ]
                switch (project.folderStructure[index])
                {
                    case "西元年分":
                        name = fileDateTime.Year.ToString() + "年";
                        break;
                    case "月份":
                        name = fileDateTime.Month.ToString() + "月";
                        break;
                    case "日期":
                        name = fileDateTime.Day.ToString() + "日";
                        break;
                }
                return name;
            }
            //---------------------------------------------------------------------------------
            string dFilePathname = null, subFolder = @"\";

            //Lv: [ 西元年分 月份 日期 (無) ]
            subFolder += GetLevelName(0) + @"\";
            subFolder += GetLevelName(1) + @"\";
            string Lv3 = GetLevelName(2);
            subFolder += (Lv3 != String.Empty)?Lv3 + @"\" : "";

            string newFileName = sourceFi.Name;

            if (project.isRename)
            {
                newFileName = string.Format("{0:0000}{1:00}{2:00}{3:00}{4:00}{5:00}",
                    fileDateTime.Year,
                    fileDateTime.Month,
                    fileDateTime.Day,
                    fileDateTime.Hour,
                    fileDateTime.Minute,
                    fileDateTime.Second
                ) + sourceFi.Extension;
            }
            dFilePathname = project.targetRootPathname + subFolder + newFileName;

            return dFilePathname;
        }

        void ForeachFolderFile(Func<System.IO.FileInfo, bool> CheckFile)
        {
            DirectoryInfo root = new DirectoryInfo(project.sourceRootPathname);
            //new DirectoryInfo(@"I:\測試資料夾");
            //new DirectoryInfo(@"C:\Users\張文忠\Documents\ExifFileSort");

            //List Dir / File
            Util.WalkDirectoryTree(root, CheckFile); 
        }
        //File GetFile(string Path, List<string> Exts)
        //{
        //    //File f = new
        //    return null;
        //}
    }
}
