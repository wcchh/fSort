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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;
using System.IO;

namespace fSort
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
        UserProject project = new UserProject();
        public MainWindow()
        {
            InitializeComponent();
            CenterWindowOnScreen();
            Log.initialize();
            ErrorHandle.Register("fSort");
        }
        private void CenterWindowOnScreen()
        {
            double screenWidth = System.Windows.SystemParameters.PrimaryScreenWidth;
            double screenHeight = System.Windows.SystemParameters.PrimaryScreenHeight;
            double windowWidth = this.Width;
            double windowHeight = this.Height;
            this.Left = (screenWidth / 2) - (windowWidth / 2);
            this.Top = (screenHeight / 2) - (windowHeight / 2);
        }
        private void ListStructure(System.Windows.Controls.TreeView treeView, string[] structureArray)
        {
            TreeViewItem CreateNode(int index)
            {
                TreeViewItem Node = null;
                if (index < structureArray.Length)
                {
                    Node = new TreeViewItem { Header = structureArray[index] };
                    Node.Items.Add(CreateNode(index + 1));
                }
                return Node;
            }

            treeView.Items.Clear();
            treeView.Items.Add(CreateNode(0));
            
            TreeViewItem tvi = treeView.Items.GetItemAt(0) as TreeViewItem;
            tvi.ExpandSubtree();
        }

        private void BTN_Execution_Click(object sender, RoutedEventArgs e)
        {
            TBX_SourceFolder.Text = TBX_SourceFolder.Text.TrimEnd('\\');
            TBX_TargetFolder.Text = TBX_TargetFolder.Text.TrimEnd('\\');
            //Check Data Correct
            if (!Directory.Exists(TBX_SourceFolder.Text))
            {
                System.Windows.MessageBox.Show("來源路徑不存在，請重新設定", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (!Directory.Exists(TBX_TargetFolder.Text))
            {
                System.Windows.MessageBox.Show("目的路徑不存在，請重新設定", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (TBX_TargetFolder.Text == TBX_SourceFolder.Text)
            {
                System.Windows.MessageBox.Show("來源路徑 與 目的路徑 相同，請重新設定", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            //Take off redoundent SPACE.
            string[] imageFileExts = TBX_ImageExtends.Text.Replace(" ", null).ToLower().Split(',');
            string[] videoFileExts = TBX_VideoExtends.Text.Replace(" ", null).ToLower().Split(',');
            TBX_ImageExtends.Text = string.Join(", ", project.imageFileExts);
            TBX_VideoExtends.Text = string.Join(", ", project.videoFileExts);

            List<string> fstruct = new List<string>() { CBX_Level1.Text, CBX_Level2.Text, CBX_Level3.Text };
            //建立專案
            project = new UserProject()
            {
                sourceRootPathname = TBX_SourceFolder.Text,
                targetRootPathname = TBX_TargetFolder.Text,
                imageFileExts = imageFileExts,
                videoFileExts = videoFileExts,
                fAction = (UserProject.FileAction)CBX_FileAct.SelectedIndex,
                folderStructure = fstruct.ToArray(),
                isRename = (CBX_FileRenameAsCopyMove.IsEnabled && CBX_FileRenameAsCopyMove.IsChecked == true),
                isTheSameContentByComparedBinary = (CBX_CHKBinary.IsEnabled && CBX_CHKBinary.IsChecked == true),
                isContentConfirmByComparedBinary = (CBX_ReCHKBinary.IsEnabled && CBX_ReCHKBinary.IsChecked == true),
                isAddInfoFile = (CBX_AddInfoFileAsCopyMove.IsEnabled && CBX_AddInfoFileAsCopyMove.IsChecked == true)
            };
            project.Save(); //紀錄

            if (project.isContentConfirmByComparedBinary && project.fAction == UserProject.FileAction.Move)
            {
                if (System.IO.Path.GetPathRoot(project.sourceRootPathname) == System.IO.Path.GetPathRoot(project.targetRootPathname))
                {
                    project.isContentConfirmByComparedBinary = false;
                }
                else
                {
                    //Change "project.fAction" to Copy + Delete.
                    project.fAction = UserProject.FileAction.CopyThenDelete;
                }
            }
            //執行
            ProcessWindow pw = new ProcessWindow()
            {
                project = project
            };
            pw.ShowDialog();
        }

        private string SelectFolder(string TargetFolderPathname)
        {
            string folderPathname = TargetFolderPathname;
            if (folderPathname != string.Empty)
            {
                if (!Directory.Exists(folderPathname))
                {
                    do
                    {
                        folderPathname = System.IO.Path.GetDirectoryName(folderPathname);
                    } while (folderPathname != string.Empty && !Directory.Exists(folderPathname));
                }
            }
            if (folderPathname == string.Empty)
            {
                // Default to the My Documents folder.
                folderPathname = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            }

            FolderBrowserDialog folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            //folderBrowserDialog1.ShowNewFolderButton = false;
            folderBrowserDialog1.Description = "選擇你要的資料夾";// "Select the directory that you want to use as the default.";
            folderBrowserDialog1.SelectedPath = folderPathname;

            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                folderPathname = folderBrowserDialog1.SelectedPath;
            } else
            {
                folderPathname = TargetFolderPathname;//Restore as Cancel.
            }
            return folderPathname;
        }
        private void BTN_SelectSourceFolder_Click(object sender, RoutedEventArgs e)
        {
            TBX_SourceFolder.Text = SelectFolder(TBX_SourceFolder.Text.Trim());
        }
        private void BTN_SelectTargetFolder_Click(object sender, RoutedEventArgs e)
        {
            TBX_TargetFolder.Text = SelectFolder(TBX_TargetFolder.Text.Trim());
        }

        private void BTN_OpenSourceFolder_Click(object sender, RoutedEventArgs e)
        {
            Util.ExplorerFileFolder(TBX_SourceFolder.Text);
        }
        private void BTN_OpenTargetFolder_Click(object sender, RoutedEventArgs e)
        {
            Util.ExplorerFileFolder(TBX_TargetFolder.Text);            
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //BTN_SaveConfig_Click(sender, null);
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            //string[] content = { "ONE" };            
            //project.sourceRootPathname = string.Join("\n", content);
            //Load project
            TBX_SourceFolder.Text = project.sourceRootPathname;
            TBX_TargetFolder.Text = project.targetRootPathname;
            TBX_ImageExtends.Text = string.Join(", ", project.imageFileExts);
            TBX_VideoExtends.Text = string.Join(", ", project.videoFileExts);

            CBX_FileAct.SelectedIndex = (int)project.fAction;
            CBX_FileRenameAsCopyMove.IsChecked = project.isRename;
            CBX_CHKBinary.IsChecked = project.isTheSameContentByComparedBinary;
            CBX_ReCHKBinary.IsChecked = project.isContentConfirmByComparedBinary;
            CBX_AddInfoFileAsCopyMove.IsChecked = project.isAddInfoFile;
            //Structure
            ListStructure(TV_Structure, project.folderStructure);
        }

        private void CBX_FileAct_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CBX_FileRenameAsCopyMove == null 
                || CBX_CHKBinary == null
                || CBX_ReCHKBinary == null)
            {
                return;
            }

            UserProject.FileAction fAction = (UserProject.FileAction)CBX_FileAct.SelectedIndex;
            switch (fAction)
            {
                case UserProject.FileAction.Copy:
                case UserProject.FileAction.Move:
                    CBX_FileRenameAsCopyMove.IsEnabled = true;
                    CBX_AddInfoFileAsCopyMove.IsEnabled = true;
                    CBX_CHKBinary.IsEnabled = true;
                    CBX_ReCHKBinary.IsEnabled = true;
                    break;
                case UserProject.FileAction.ListOnly:
                    CBX_FileRenameAsCopyMove.IsEnabled = false;
                    CBX_AddInfoFileAsCopyMove.IsEnabled = false;
                    CBX_CHKBinary.IsEnabled = false;
                    CBX_ReCHKBinary.IsEnabled = false;
                    break;
            }
        }

        private void BTN_SaveConfig_Click(object sender, RoutedEventArgs e)
        {
            if (project != null)
            {
                project.Save();
                System.Windows.MessageBox.Show("目前設定已儲存。", "Info");
            }
            else
            {
                System.Windows.MessageBox.Show("目前無設定檔...!", "Error");
            }
        }

        private void BTN_ResetConfig_Click(object sender, RoutedEventArgs e)
        {
            if (System.Windows.MessageBox.Show("確定清除目前設定?", "WARN", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                if (project != null)
                {
                    project.Reset();
                }
                Grid_Loaded(sender, e);
            }
        }

        private void BTN_OpenLogFolder_Click(object sender, RoutedEventArgs e)
        {
            string logFolderPathname = Log.logLocation + Log.logFoldername;
            
            Util.ExplorerFileFolder(logFolderPathname);
        }
    }
}
