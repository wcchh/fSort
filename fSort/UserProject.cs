using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using fSort.Properties;

namespace fSort
{
    public class UserProject
    {
        public enum FileAction
        {
            ListOnly,
            Move,
            Copy,
            CopyThenDelete
        }
        public string sourceRootPathname = string.Empty;//"C:\\";
        public string targetRootPathname = string.Empty;//"C:\\NewFolder";
        public string[] imageFileExts = { ".jpg", ".jpeg", ".png" };
        public string[] videoFileExts = { ".avi", ".mov", ".mpg", ".mpeg", ".mts", ".3gp" };
        public FileAction fAction = FileAction.Move;
        public string[] folderStructure = { "year", "month", "date" };
        public bool isRename = false;
        public bool isTheSameContentByComparedBinary = false;
        public bool isContentConfirmByComparedBinary = false;
        public bool isAddInfoFile = false;

        public UserProject()
        {
            Load();
            //Test data
            //sourceRootPathname = @"I:\測試資料夾\s";
            //targetRootPathname = @"I:\測試資料夾\t";
            //fAction = FileAction.ListOnly;
        }

        public void Save()
        {
            Settings.Default["sourceRootPathname"] = sourceRootPathname;
            Settings.Default["targetRootPathname"] = targetRootPathname;
            Settings.Default["imageFileExts"] = string.Join(" ", imageFileExts); 
            Settings.Default["videoFileExts"] = string.Join(" ", videoFileExts); 
            Settings.Default["fAction"] = (int)fAction;
            Settings.Default["folderStructure"] = string.Join(" ", folderStructure); 
            Settings.Default["isRename"] = isRename;
            Settings.Default["isTheSameContentByComparedBinary"] = isTheSameContentByComparedBinary;
            Settings.Default["isContentConfirmByComparedBinary"] = isContentConfirmByComparedBinary;
            Settings.Default["isAddInfoFile"] = isAddInfoFile;
            Settings.Default.Save();
        }

        public void Load()
        {            
            sourceRootPathname = Settings.Default["sourceRootPathname"].ToString();
            targetRootPathname = Settings.Default["targetRootPathname"].ToString();
            imageFileExts = Settings.Default["imageFileExts"].ToString().Split(' ').ToArray();
            videoFileExts = Settings.Default["videoFileExts"].ToString().Split(' ').ToArray();
            fAction = (FileAction)Settings.Default["fAction"];
            folderStructure = Settings.Default["folderStructure"].ToString().Split(' ').ToArray();
            isRename = (bool)Settings.Default["isRename"];
            isTheSameContentByComparedBinary = (bool)Settings.Default["isTheSameContentByComparedBinary"];
            isContentConfirmByComparedBinary = (bool)Settings.Default["isContentConfirmByComparedBinary"];
            isAddInfoFile = (bool)Settings.Default["isAddInfoFile"];
        }

        public void Reset()
        {
            Settings.Default.Reset();
            Load();
        }
    }
}
