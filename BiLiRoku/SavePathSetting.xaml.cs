using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;

namespace BiliRoku
{
    /// <summary>
    /// SavePathSetting.xaml 的交互逻辑
    /// </summary>
    public partial class SavePathSetting
    {
        private SaveFileDialog _sfd;
        private Config _config;

        public SavePathSetting()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _sfd = new SaveFileDialog { FileName = "savefile.flv" };
            _config = Config.GetConfig();
            SaveDirBox.Text = _config.SavePath == null ? "" : _config.SavePath;
            FilenameBox.Text = _config.FileName == null ? FilenameBox.Text : _config.FileName;
        }

        private void OpenSaveDialogButton_Click(object sender, RoutedEventArgs e)
        {
            if (_sfd.ShowDialog() == true)
            {
                SaveDirBox.Text = System.IO.Path.GetDirectoryName(_sfd.FileName);
            }
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (SaveDirBox.Text == "")
            {
                MessageBox.Show("保存路径不能为空。", "BiliRoku");
                return;
            }
            if (!System.IO.Directory.Exists(SaveDirBox.Text))
            {
                if (MessageBoxResult.OK != MessageBox.Show("目录不存在，确认将创建此目录", "确认？", MessageBoxButton.OKCancel))
                {
                    return;
                }
            }
            if (FilenameBox.Text == "")
            {
                MessageBox.Show("文件名不能为空。", "BiliRoku");
                return;
            }
            if (!System.IO.Path.HasExtension(FilenameBox.Text))
            {
                if (MessageBoxResult.OK == MessageBox.Show("文件路径不含扩展名。确认将自动添加“.flv”的扩展名。", "确认？", MessageBoxButton.OKCancel))
                {
                    FilenameBox.Text = FilenameBox.Text + ".flv";
                }
                else
                {
                    return;
                }
            }
            DialogResult = true;
            _config.SavePath = SaveDirBox.Text;
            _config.FileName = FilenameBox.Text;
            Close();
        }

        public string SavePath => SaveDirBox.Text;

        public string Filename => FilenameBox.Text;
    }
}
