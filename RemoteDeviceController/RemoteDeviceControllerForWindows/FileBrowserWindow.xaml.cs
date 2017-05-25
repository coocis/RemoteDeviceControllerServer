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

using RemoteDeviceControllerForWindows.FileAccessServiceReference;
using RemoteDeviceControllerForWindows.ViewModels;
using RemoteDeviceController.Util;
using RemoteDeviceControllerForWindows.UserControls;
using System.Text.RegularExpressions;
using System.IO;
using System.Collections.ObjectModel;


namespace RemoteDeviceControllerForWindows
{
    /// <summary>
    /// Interaction logic for FileBrowserWindow.xaml
    /// </summary>
    public partial class FileBrowserWindow : Window
    {
        private FileBrowserWindowViewModel _viewModel;
        private FileAccessServiceClient _client;
        private bool _isDecryptMode = false;
        private string _currentDirectory;
        private string _currentDirectory_Decrypted;
        private List<FileInfoViewModel> _currentDirectoryFileInfos;
        private List<FileInfoViewModel> _currentDirectoryFileInfos_Decrypted;

        public string CurrentDirectory
        {
            get
            {
                return _currentDirectory;
            }

            private set
            {
                //check if it is "f:", add a "\"
                _currentDirectory = value;
                if (Regex.IsMatch(_currentDirectory, ":$"))
                {
                    _currentDirectory += "\\";
                }
            }
        }

        public string CurrentDirectory_Decrypted
        {
            get
            {
                return _currentDirectory_Decrypted;
            }

            set
            {
                _currentDirectory_Decrypted = value;
                if (Regex.IsMatch(_currentDirectory_Decrypted, ":$"))
                {
                    _currentDirectory_Decrypted += "\\";
                }
            }
        }

        public FileBrowserWindow(FileAccessServiceClient client)
        {
            InitializeComponent();
            _viewModel = DataContext as FileBrowserWindowViewModel;
            _client = client;

            CurrentDirectory = @"h:\";
            CurrentDirectory_Decrypted = DecryptCurrentDirectory();
            UpdateCurrentFileInfos();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            _client.Close();
        }

        private List<FileInfoViewModel> GetCurrentDirectoryFileInfos()
        {
            List<FileInfoViewModel> fileInfos = new List<FileInfoViewModel>();
            
            var folders = _client.GetDirectoryFolderNames(CurrentDirectory);
            foreach (var folder in folders)
            {
                FileInfoViewModel fileInfo = new FileInfoViewModel();
                fileInfo.Name = folder;
                fileInfo.IsFile = false;
                fileInfos.Add(fileInfo);
            }

            var files = _client.GetDirectoryFileNames(CurrentDirectory);
            foreach (var file in files)
            {
                FileInfoViewModel fileInfo = new FileInfoViewModel();
                fileInfo.Name = file;
                //if it is an image file, get its thumbnail
                if (Regex.IsMatch(fileInfo.Name, "[.](jpg|png|bmg)", RegexOptions.IgnoreCase))
                {
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.StreamSource = new MemoryStream(_client.GetFileThumbnail(CurrentDirectory + "\\" + file));
                    bitmap.EndInit();
                    fileInfo.Icon = bitmap;
                }
                fileInfo.IsFile = true;
                fileInfos.Add(fileInfo);
            }
            return fileInfos;
        }

        private List<FileInfoViewModel> DecryptCurrentDirectoryFileInfos()
        {
            List<FileInfoViewModel> fileInfos = new List<FileInfoViewModel>();
            foreach (var file in _currentDirectoryFileInfos)
            {
                if (file.Name.Contains(FileEncryptor.encryptedFileExtension))
                {
                    FileInfoViewModel fivm = new FileInfoViewModel();
                    fivm.Name = file.Name.Split('.')[0];
                    fivm.Name = TextEncryptor.Decrypt_Aes_Hex(fivm.Name);
                    fivm.IsFile = file.IsFile;
                    //if it is an image file, get its thumbnail
                    if (fivm.IsFile && Regex.IsMatch(fivm.Name, "[.](jpg|png|bmg)", RegexOptions.IgnoreCase))
                    {
                        BitmapImage bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.StreamSource = new MemoryStream(_client.GetFileThumbnail(CurrentDirectory + "\\" + file.Name));
                        bitmap.EndInit();
                        fivm.Icon = bitmap;
                    }
                    fileInfos.Add(fivm);
                }
                else
                {
                    fileInfos.Add(file);
                }
            }

            return fileInfos;
        }

        private byte[] GetFile(string fileName)
        {
            string name = fileName;
            if (_isDecryptMode)
            {
                name = GetOriginalName(name);
            }
            return FileEncryptor.Decrypt(_client.GetFile(CurrentDirectory + "\\" + name));
        }

        private void OpenFolder(string folderName)
        {
            //check whether there is a "\" at the end of the CurrentDirectory
            if (Regex.IsMatch(CurrentDirectory, "\\\\$"))
            {
                CurrentDirectory += folderName;
            }
            else
            {
                CurrentDirectory += @"\" + folderName;
            }

            string s = folderName;
            if (s.Contains(FileEncryptor.encryptedFileExtension))
            {
                s = TextEncryptor.Decrypt_Aes_Hex(folderName.Split('.')[0]);
            }
            //check whether there is a "\" at the end of the CurrentDirectory
            if (Regex.IsMatch(CurrentDirectory_Decrypted, "\\\\$"))
            {
                CurrentDirectory_Decrypted += s;
            }
            else
            {
                CurrentDirectory_Decrypted += @"\" + s;
            }
            UpdateCurrentFileInfos();
        }

        private void ChangeDecryptMode()
        {
            _isDecryptMode = !_isDecryptMode;
            UpdateCurrentFileInfos();
        }

        private void UpdateCurrentFileInfos()
        {
            _currentDirectoryFileInfos = GetCurrentDirectoryFileInfos();
            _currentDirectoryFileInfos_Decrypted = DecryptCurrentDirectoryFileInfos();
            if (_isDecryptMode)
            {
                _viewModel.CurrentDirectory = CurrentDirectory_Decrypted;
                _viewModel.FileInfos =
                    new ObservableCollection<FileInfoViewModel>(_currentDirectoryFileInfos_Decrypted);
            }
            else
            {
                _viewModel.CurrentDirectory = CurrentDirectory;
                _viewModel.FileInfos =
                    new ObservableCollection<FileInfoViewModel>(_currentDirectoryFileInfos);
            }
        }

        private string GetOriginalName(string decryptedName)
        {
            if (decryptedName.Contains(FileEncryptor.encryptedFileExtension))
            {
                return decryptedName;
            }
            string name = decryptedName;
            int index = _currentDirectoryFileInfos_Decrypted.FindIndex(f => f.Name.Equals(name));
            name = _currentDirectoryFileInfos[index].Name;
            return name;
        }

        private string DecryptCurrentDirectory()
        {
            string result = CurrentDirectory;
            if (CurrentDirectory.Contains(FileEncryptor.encryptedFileExtension))
            {
                result = "";
                string[] ss = CurrentDirectory.Split('\\');
                foreach (var s in ss)
                {
                    if (!string.IsNullOrWhiteSpace(s))
                    {
                        string folder = s;
                        if (folder.Contains(FileEncryptor.encryptedFileExtension))
                        {
                            folder = TextEncryptor.Decrypt_Aes_Hex(folder.Split('.')[0]);
                        }
                        result += folder + "\\";
                    }
                }
                //delete the last "\"
                result.Remove(result.Length - 1);
            }
            return result;
        }

        #region UI event

        private void window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.D)
            {
                ChangeDecryptMode();
            }
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            //remove the folder name after the last "\"
            CurrentDirectory = Regex.Replace(CurrentDirectory, "\\\\[^\\\\]*$", "");
            CurrentDirectory_Decrypted = Regex.Replace(CurrentDirectory_Decrypted, "\\\\[^\\\\]*$", "");

            UpdateCurrentFileInfos();
        }
        private void goButton_Click(object sender, RoutedEventArgs e)
        {
            CurrentDirectory = currentDirectoryTextBox.Text;
            CurrentDirectory_Decrypted = DecryptCurrentDirectory();
            if (_isDecryptMode)
            {
                ChangeDecryptMode();
            }
            else
            {
                UpdateCurrentFileInfos();
            }
        }
        private void FileInfoControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            FileInfoControl fic = sender as FileInfoControl;
            string name = fic.FileName;
            if (_isDecryptMode)
            {
                name = GetOriginalName(name);
            }
            if (fic.IsFile)
            {
                if (Regex.IsMatch(fic.FileName, "[.](jpg|png|bmp)"))
                {
                    List<string> images = (from f in _currentDirectoryFileInfos
                                          where Regex.IsMatch(f.Name, "[.](jpg|png|bmp)")
                                          select f.Name).ToList();
                    ImageViewerWindow ivw = new ImageViewerWindow(CurrentDirectory, images, fic.FileName);
                    ivw.Show();
                }
                if (Regex.IsMatch(fic.FileName, "[.](mp4)"))
                {
                    MediaPlayerWindow mpw = new MediaPlayerWindow();
                    mpw.SetMedia(CurrentDirectory + "\\" + fic.FileName);
                    mpw.Show();
                }
                if (Regex.IsMatch(fic.FileName, "[.](txt)"))
                {
                    TextViewerWindow tvw = new TextViewerWindow(CurrentDirectory, fic.FileName);
                    tvw.Show();
                }
            }
            else
            {
                OpenFolder(name);
            }
        }

        private void FileInfoControl_SaveAsButton_OnClick(FileInfoControl fic)
        {
            string remoteFullPath = CurrentDirectory + "\\" + fic.FileName;
            string localDirectory = "";
            localDirectory = InputDialog.Show("Path:");
            if (string.IsNullOrWhiteSpace(localDirectory))
            {
                return;
            }
            //if localDirectory is not end with "\"
            if (Regex.IsMatch(localDirectory, "[^\\\\]$"))
            {
                localDirectory += "\\";
            }
            //the file path may be illegal
            try
            {
                if (fic.IsFile)
                {
                    //if the file is too large, an exception will be thrown
                    //larger than 1G I think? 300MB is ok, if there is enough memory
                    try
                    {
                        SaveFileAs(_client.GetFile(remoteFullPath), localDirectory, fic.FileName);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                    //if wait for GC itself to do it, it may happen after 3 files have already been in memory
                    GC.Collect();
                }
                else
                {
                    SaveFolderAs(remoteFullPath, localDirectory + fic.FileName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void FileInfoControl_DecryptAsButton_OnClick(FileInfoControl fic)
        {
            string remoteFullPath = CurrentDirectory + "\\" + fic.FileName;
            string localDirectory = "";
            localDirectory = InputDialog.Show("Path:");
            if (string.IsNullOrWhiteSpace(localDirectory))
            {
                return;
            }
            //if localDirectory is not end with "\"
            if (Regex.IsMatch(localDirectory, "[^\\\\]$"))
            {
                localDirectory += "\\";
            }
            //the file path may be illegal
            try
            {
                if (fic.IsFile)
                {
                    DecryptFileAs(_client.GetFile(remoteFullPath), localDirectory, fic.FileName);
                }
                else
                {
                    DecryptFolderAs(remoteFullPath, localDirectory + fic.FileName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void FileInfoControl_EncryptAsButton_OnClick(FileInfoControl fic)
        {
            string remoteFullPath = CurrentDirectory + "\\" + fic.FileName;
            string localDirectory = "";
            localDirectory = InputDialog.Show("Path:");
            if (string.IsNullOrWhiteSpace(localDirectory))
            {
                return;
            }
            //if localDirectory is not end with "\"
            if (Regex.IsMatch(localDirectory, "[^\\\\]$"))
            {
                localDirectory += "\\";
            }
            //the file path may be illegal
            try
            {
                if (fic.IsFile)
                {
                    EncryptFileAs(_client.GetFile(remoteFullPath), localDirectory, fic.FileName);
                }
                else
                {
                    EncryptFolderAs(remoteFullPath, localDirectory + fic.FileName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
#endregion //UI event

        #region Save file
        private void DecryptFileAs(byte[] file, string fileDirectory, string fileName)
        {
            string name = fileName;
            byte[] bs = file;
            if (fileName.Contains(FileEncryptor.encryptedFileExtension))
            {
                name = TextEncryptor.Decrypt_Aes_Hex(fileName.Split('.')[0]);
                bs = FileEncryptor.Decrypt(file);
            }
            SaveFileAs(bs, fileDirectory, name);
        }

        private void EncryptFileAs(byte[] file, string fileDirectory, string fileName)
        {
            string name = fileName;
            byte[] bs = file;
            if (!fileName.Contains(FileEncryptor.encryptedFileExtension))
            {
                name = TextEncryptor.Encrypt_Aes_Hex(fileName) + FileEncryptor.encryptedFileExtension;
                bs = FileEncryptor.Encrypt(file);
            }
            SaveFileAs(bs, fileDirectory, name);
        }

        private void DecryptFolderAs(string remotePath, string localPath)
        {
            string folderName = remotePath.Split('\\').Last();
            if (folderName.Contains(FileEncryptor.encryptedFileExtension))
            {
                folderName = folderName.Split('.')[0];
                //last folder name of the full path
                localPath = Regex.Replace(localPath, "[^\\\\]+$", TextEncryptor.Decrypt_Aes_Hex(folderName));
            }
            if (!Directory.Exists(localPath))
            {
                Directory.CreateDirectory(localPath);
            }
            List<string> fileNames = new List<string>();
            fileNames = new List<string>(_client.GetDirectoryFolderNames(remotePath));
            foreach (var fileName in fileNames)
            {
                DecryptFolderAs(remotePath + "\\" + fileName, localPath + "\\" + fileName);
            }
            fileNames = new List<string>(_client.GetDirectoryFileNames(remotePath));
            foreach (var fileName in fileNames)
            {
                DecryptFileAs(_client.GetFile(remotePath + "\\" + fileName), localPath, fileName);
            }
        }

        private void EncryptFolderAs(string remotePath, string localPath)
        {
            string folderName = remotePath.Split('\\').Last();
            if (!folderName.Contains(FileEncryptor.encryptedFileExtension))
            {
                //last folder name of the full path
                localPath = Regex.Replace(localPath, "[^\\\\]+$", TextEncryptor.Encrypt_Aes_Hex(folderName));
                localPath += FileEncryptor.encryptedFileExtension;
            }
            if (!Directory.Exists(localPath))
            {
                Directory.CreateDirectory(localPath);
            }
            List<string> fileNames = new List<string>();
            fileNames = new List<string>(_client.GetDirectoryFolderNames(remotePath));
            foreach (var fileName in fileNames)
            {
                EncryptFolderAs(remotePath + "\\" + fileName, localPath + "\\" + fileName);
            }
            fileNames = new List<string>(_client.GetDirectoryFileNames(remotePath));
            foreach (var fileName in fileNames)
            {
                EncryptFileAs(_client.GetFile(remotePath + "\\" + fileName), localPath, fileName);
            }
        }

        private void SaveFileAs(byte[] file, string fileDirectory, string fileName)
        {
            if (!Directory.Exists(fileDirectory))
            {
                Directory.CreateDirectory(fileDirectory);
            }
            using (FileStream fs = File.Create(fileDirectory + "\\" + fileName))
            {
                fs.Write(file, 0, file.Length);
            }
        }

        private void SaveFolderAs(string remotePath, string localPath)
        {
            if (!Directory.Exists(localPath))
            {
                Directory.CreateDirectory(localPath);
            }
            List<string> fileNames = new List<string>();
            fileNames = new List<string>(_client.GetDirectoryFolderNames(remotePath));
            foreach (var fileName in fileNames)
            {
                SaveFolderAs(remotePath + "\\" + fileName, localPath + "\\" + fileName);
            }
            fileNames = new List<string>(_client.GetDirectoryFileNames(remotePath));
            foreach (var fileName in fileNames)
            {
                SaveFileAs(_client.GetFile(remotePath + "\\" + fileName), localPath, fileName);
            }
        }
        #endregion

    }
}
