using BiliRoku.Bililivelib;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.IO;
namespace BiliRoku
{
    internal class Config
    {
        private const string roomInfoPath = @".\config\roomInfo.json";
        private List<RoomInfo> infoList = new List<RoomInfo>();

        private static Config config;
        private static readonly object locker = new object();


        public List<RoomInfo> GetInfoList()
        {
            return infoList;
        }

        private Config()
        {
            Init();
        }

        //双重锁定,学习单例模式
        public static Config GetConfig()
        {
            if(config == null)
            {
                lock(locker)
                {
                    if(config == null)
                    {
                        config = new Config();
                    }
                }
            }
            return config;
        }
        
        // 初始化时读取本地json数据到List中
        private void Init()
        {
            FileInfo fi = new FileInfo(roomInfoPath);
            var di = fi.Directory;
            if (!di.Exists)
            {
                di.Create();
            }
            FileStream _file = new FileStream(roomInfoPath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            using (StreamReader sr = new StreamReader(_file))
            {
                try
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Converters.Add(new JavaScriptDateTimeConverter());
                    serializer.NullValueHandling = NullValueHandling.Ignore;

                    //构建Json.net的读取流  
                    JsonReader reader = new JsonTextReader(sr);
                    //对读取出的Json.net的reader流进行反序列化，并装载到模型中  
                    infoList = serializer.Deserialize<List<RoomInfo>>(reader);
                }
                catch (Exception ex)
                {
                    ex.Message.ToString();
                }
            }
            if (infoList == null)
            {
                infoList = new List<RoomInfo>();
            }
        }
        
        // 将房间数据保存到本地
        public void SaveRoomInfo()
        {
            FileInfo fi = new FileInfo(roomInfoPath);
            var di = fi.Directory;
            if (!di.Exists)
            {
                di.Create();
            }
            FileStream _file = new FileStream(roomInfoPath, FileMode.Create, FileAccess.ReadWrite);
            using (StreamWriter sw = new StreamWriter(_file))
            {
                try
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Converters.Add(new JavaScriptDateTimeConverter());
                    serializer.NullValueHandling = NullValueHandling.Ignore;

                    //构建Json.net的写入流  
                    JsonWriter writer = new JsonTextWriter(sw);
                    //把模型数据序列化并写入Json.net的JsonWriter流中  
                    serializer.Serialize(writer, infoList);
                    //ser.Serialize(writer, ht);
                    writer.Close();
                    sw.Close();

                }
                catch (Exception ex)
                {
                    ex.Message.ToString();
                }
            }
        }

        // 添加房间信息
        public void AddRoomInfo(RoomInfo info)
        {
            lock (locker)
            {
                infoList.Add(info);
                SaveRoomInfo();
            }
        }
    }
}
