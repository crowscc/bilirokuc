using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiliRoku
{
    class Config
    {
        private string _savePath;
        private string _fileName;
        private string _saveLoaction;
        private static Config _config;
        private static readonly object locker = new object();

        public string SavePath
        {
            get => _savePath;
            set
            {
                SetValue("SavePath", value);
                _savePath = value;
            }
        }

        public string FileName
        {
            get => _fileName;
            set
            {
                SetValue("FileName",value);
                _fileName = value;
            }
        }

        public string SaveLoaction
        {
            get => _saveLoaction;
            set
            {
                SetValue("SaveLoaction", value);
                _saveLoaction = value;
            }
        }

        private Config()
        {
            Init();
        }

        public static Config GetConfig()
        {
            if(_config == null)
            {
                lock (locker)
                {
                    if(_config == null)
                    {
                        _config = new Config();
                    }
                }
            }
            return _config;
        }

        private void SetValue(string key, string value)
        {
            System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            if (config.AppSettings.Settings[key] == null)
            {
                config.AppSettings.Settings.Add(key, value);
            }
            else
            {
                config.AppSettings.Settings[key].Value = value;
            }
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }

        private string GetValue(string key)
        {
            System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            if(config.AppSettings.Settings[key] == null)
            {
                return null;
            }
            else
            {
                return config.AppSettings.Settings[key].Value;
            }
        }

        private void Init()
        {
            _savePath = GetValue("SavePath");
            _fileName = GetValue("FileName");
            _saveLoaction = GetValue("SaveLoaction");
        }


    }
}
