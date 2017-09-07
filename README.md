# biliroku
bilibili 生放送（直播）录制

其中优质代码99%来自原项目：https://github.com/zyzsdy/biliroku
原作者：Zyzsdy
（主页 http://zyzsdy.com/biliroku)

我只是把原来的单线程录制改成了多线程，可以同时录制多个房间，以前没接触过C#，BUG多多。

Licensed by GPLv3 详情请见LICENSE文件

BiliRoku是是B站（bilibili）直播内容录制工具。
原理：直接接收直播的视频流并保存到本地。

使用方法：输入房间号。
房间号就是看直播的网址：http://live.bilibili.com/xxxxx
里面xxxxx的数字。

运行平台: .NET framework 4.0以上

## MediaInfo

本软件使用了MediaInfo（BSD）的库和代码