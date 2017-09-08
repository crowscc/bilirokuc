using BiliRoku.Bililivelib;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.IO;
namespace BiliRoku
{
    internal class LocalRoom
    {
        private const string roomInfoPath = @".\config\roomInfo.json";
        private List<RoomInfo> infoList = new List<RoomInfo>();
        private List<string> roomIdList = new List<string>();

        private static LocalRoom localRoom;
        private static readonly object locker = new object();


        public List<RoomInfo> GetInfoList()
        {
            return infoList;
        }

        private LocalRoom()
        {
            Init();
        }

        //双重锁定,学习单例模式
        public static LocalRoom GetLocalRoom()
        {
            if(localRoom == null)
            {
                lock(locker)
                {
                    if(localRoom == null)
                    {
                        localRoom = new LocalRoom();
                    }
                }
            }
            return localRoom;
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
            if (infoList == null || infoList.Count == 0)
            {
                infoList = new List<RoomInfo>();
                roomIdList = new List<string>();
            }
            else
            {
                foreach( var tempRoom in infoList)
                {
                    roomIdList.Add(tempRoom.RoomId);
                }
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
        public bool AddRoomInfo(RoomInfo info)
        {
            lock (locker)
            {
                if (roomIdList.Contains(info.RoomId))
                {
                    return false;
                }
                else
                {
                    infoList.Add(info);
                    SaveRoomInfo();
                    return true;
                }
            }
        }
    }
}
