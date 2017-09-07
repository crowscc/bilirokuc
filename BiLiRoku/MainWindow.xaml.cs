using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using BiliRoku.Bililivelib;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace BiliRoku
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow
    {
        private Config _config;
        private List<Downloader> _downloaderList;
        private List<RoomInfo> _roomInfoList;

        public MainWindow()
        {
            InitializeComponent();
        }

        public void AppendLogln(string level, string logText)
        {
            Dispatcher.Invoke(()=> {
                infoBlock.AppendText("[" + level + " " + DateTime.Now.ToString("HH:mm:ss") + "] " + logText + "\n");
            });
        }

        private void startButton_Click(object sender, RoutedEventArgs e)
        {
            if (_downloaderList == null)
            {
                AppendLogln("ERROR", "初始化未成功，请尝试重启。");
                return;
            }
            foreach (Downloader down in _downloaderList)
            {
                if (down.RoomInfo.RunSetting == true)
                {
                    if (down.RoomInfo.IsRun == false)
                    {
                        down.Start();
                    }
                }
            }
        }

        public void SetProcessingBtn()
        {
            startButton.IsEnabled = false;
            startButton.Content = "处理中...";
        }
        public void SetStopBtn()
        {
            Dispatcher.Invoke(() =>
            {
                startButton.IsEnabled = true;
                roomIdBox.IsEnabled = true;
                saveCommentCheckBox.IsEnabled = true;
                waitForStreamCheckBox.IsEnabled = true;
                openSavepathConfigDialogButton.IsEnabled = true;
                startButton.Content = "开始";
            });
        }
        public void SetStartBtn()
        {
            Dispatcher.Invoke(() =>
            {
                startButton.IsEnabled = true;
                roomIdBox.IsEnabled = false;
                saveCommentCheckBox.IsEnabled = false;
                waitForStreamCheckBox.IsEnabled = false;
                openSavepathConfigDialogButton.IsEnabled = false;
                startButton.Content = "停止";
            });
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _config = Config.GetConfig();
            _roomInfoList = _config.GetInfoList();
            refreshData();

            Downloader tempDown;
            _downloaderList = new List<Downloader>();
            foreach (RoomInfo info in _roomInfoList)
            {
                tempDown = new Downloader(this);
                tempDown.RoomInfo = info;
                if (info.RunSetting == true)
                {
                    _downloaderList.Add(tempDown);
                }
            }

            AppendLogln("INFO", "启动成功。");
        }

        private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void CheckUpdate_OnResult(object sender, UpdateResultArgs result)
        {
            Dispatcher.Invoke(() =>
            {
                aboutLinkLabel.Content = "发现新版本：" + result.version;
                aboutLinkLabel.MouseLeftButtonUp -= aboutLinkLabel_MouseLeftButtonUp;
                aboutLinkLabel.MouseLeftButtonUp += (s, e) =>
                {
                    System.Diagnostics.Process.Start("explorer.exe", result.url);
                };
            });
        }

        private void CheckUpdate_OnInfo(object sender, string info)
        {
            AppendLogln("AutoUpdate", info);
        }

        private void aboutLinkLabel_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var about = new About {Owner = this};
            about.ShowDialog();
        }

        private void infoBlock_TextChanged(object sender, TextChangedEventArgs e)
        {
            infoBlock.ScrollToEnd();
        }


        private void openSavepathConfigDialogButton_Click(object sender, RoutedEventArgs e)
        {
            var savePathSetting = new SavePathSetting {Owner = this};
            if (savePathSetting.ShowDialog() == true)
            {
                var savepath = savePathSetting.SavePath;
                if(savepath[savepath.Length - 1] == '\\')
                {
                    savepath = savepath.Substring(0, savepath.Length - 1);
                }
                var filename = savePathSetting.Filename;
                savepathBox.Text = savepath + "\\" + filename;
            }
        }

        private void savepathTextLabel_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var filename = FlvDownloader.CompilePath(savepathBox.Text, roomIdBox.Text, remarkBox.Text);
            if (filename == "")
            {
                MessageBox.Show("你还没选文件呢！！！！！", "Error?");
            }
            else
            {
                var path = System.IO.Path.GetDirectoryName(filename);
                System.Diagnostics.Process.Start("explorer.exe", path);
            }
        }

        // 更新主界面dataGrid数据
        public void refreshData()
        {
            ObservableCollection<RoomInfo> memberData = new ObservableCollection<RoomInfo>();
            foreach (RoomInfo info in _roomInfoList)
            {
                memberData.Add(info);
            }
            dataGrid.DataContext = memberData;
            dataGrid.Items.Refresh();
        }

        // 添加按钮事件
        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            var roomId = roomIdBox.Text;
            var localPath = savepathBox.Text;
            var remark = remarkBox.Text;
            var isCmt = saveCommentCheckBox.IsChecked;
            var isWait = waitForStreamCheckBox.IsChecked;
            RoomInfo info = new RoomInfo();
            info.RoomId = roomId;
            info.SaveLocation = localPath;
            info.IsDownloadCmt = isCmt.ToString();
            info.IsWaitStreaming = isWait.ToString();
            info.Remark = remark;
            _config.AddRoomInfo(info);
            startNewThread(info);
            refreshData();
        }

        // 结束按钮事件
        private void endButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (Downloader tempDown in _downloaderList)
            {
                tempDown.Stop();
            }
            _config.SaveRoomInfo();
        }

        // 在线程list中添加新的线程
        private void startNewThread(RoomInfo info)
        {
            var tempDown = new Downloader(this);
            tempDown.RoomInfo = info;
            _downloaderList.Add(tempDown);
            tempDown.Start();
        }

        // dataGrid右键菜单
        private void MenuItem_Click(object sender, RoutedEventArgs e)//事件定义
        {
            RoomInfo info = (RoomInfo)dataGrid.SelectedItem;
            var index = dataGrid.SelectedIndex;
            MenuItem item = (MenuItem)sender;
            var header = item.Header.ToString();

            if ("开始录制".Equals(header))
            {
                if (info.IsRun == true)
                {
                    MessageBox.Show("录制中，不能重复录制");
                }
                else
                {
                    _roomInfoList[index].RunSetting = true;
                    var tempInfo = _roomInfoList[index];
                    var tempDown = new Downloader(this);
                    tempDown.RoomInfo = tempInfo;
                    _downloaderList.Add(tempDown);
                    tempDown.Start();
                }
            }
            else if ("停止录制".Equals(header))
            {
                if (info.IsRun == false)
                {
                    MessageBox.Show("已经处于停止状态，不要重复");
                }
                else
                {
                    var tempInfo = _roomInfoList[index];
                    foreach (Downloader x in _downloaderList)
                    {
                        if (x.RoomInfo.RoomId.Equals(tempInfo.RoomId))
                        {
                            x.Stop();
                            _downloaderList.Remove(x);
                            break;
                        }
                    }
                }
            }
            else if ("删除房间".Equals(header))
            {
                if (info.IsRun == true)
                {
                    foreach (Downloader x in _downloaderList)
                    {
                        if (x.RoomInfo.RoomId.Equals(info.RoomId))
                        {
                            x.Stop();
                            _downloaderList.Remove(x);
                            break;
                        }
                    }
                }
                foreach (RoomInfo x in _roomInfoList)
                {
                    if (x.RoomId.Equals(info.RoomId))
                    {
                        _roomInfoList.Remove(x);
                        break;
                    }
                }
            }
            else if ("设置停止".Equals(header))
            {
                if (info.IsRun == true)
                {
                    var tempInfo = _roomInfoList[index];
                    foreach (Downloader x in _downloaderList)
                    {
                        if (x.RoomInfo.RoomId.Equals(tempInfo.RoomId))
                        {
                            x.Stop();
                            _downloaderList.Remove(x);
                            break;
                        }
                    }
                }
                if (info.RunSetting == false)
                {
                    MessageBox.Show("已经处于永久停止状态");
                }
                else
                {
                    _roomInfoList[index].RunSetting = false;
                }
            }
            else if ("设置开始".Equals(header))
            {
                if (_roomInfoList[index].RunSetting == true)
                {
                    MessageBox.Show("已经设置开始，不要重复设置");
                }
                else
                {
                    _roomInfoList[index].RunSetting = true;
                }
            }
            _config.SaveRoomInfo();
            refreshData();
        }

        // dataGrid双击事件
        public void dataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            RoomInfo info = (RoomInfo)dataGrid.SelectedItem;
            roomIdBox.Text = info.RoomId;
            savepathBox.Text = info.SaveLocation;
        }
    }
}
