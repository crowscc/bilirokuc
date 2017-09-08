using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiliRoku.Bililivelib
{
    public class RoomInfo
    {
        //private string _version;
        private string _roomId;
        private string _remark; //备注
        private string _status; //状态
        private bool _isRun; //是否正在录播
        private string _saveLocation;   //这个是地址
        private string _savePath;
        private string _filename;
        private bool _runSetting;   //开始时是否运行
        private string _isDownloadCmt = "True"; //是否保存弹幕
        private string _isWaitStreaming = "True";   //等待主播开播自动开始

        public RoomInfo()
        {
            _runSetting = true; //默认开启
        }

        //public string Version { get => _version; set => _version = value; }
        public string RoomId { get => _roomId; set => _roomId = value; }
        public string Remark { get => _remark; set => _remark = value; }
        public string Status { get => _status; set => _status = value; }
        public bool IsRun { get => _isRun; set => _isRun = value; }
        public string SaveLocation { get => _saveLocation; set => _saveLocation = value; }
        public string SavePath { get => _savePath; set => _savePath = value; }
        public string Filename { get => _filename; set => _filename = value; }
        public string IsDownloadCmt { get => _isDownloadCmt; set => _isDownloadCmt = value; }
        public string IsWaitStreaming { get => _isWaitStreaming; set => _isWaitStreaming = value; }
        public bool RunSetting { get => _runSetting; set => _runSetting = value; }
    }
}
