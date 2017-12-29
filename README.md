# QcForm @Author:GloryFu
  QcForm是Csharp自建的类库，随着项目的开发逐步加入越来越多的功能

----------------------------------------------------------------------------------
### QcForm.Net.VPN

* ##### VPN的动态创建、删除、链接和断开。

~~~C#
/// 使用实例
QcForm.Net.VPN vpn = new QcForm.Net.VPN()
{
	DomainIp = 你的VPN地址,
	VpnName = VPN的名称,
	UserName = VPN的账号,
	PassWord = VPN的密码,
};

///创建或更新
vpn.CreateOrUpdate();

///VPN的异步委托事件

//连接成功事件 
vpn.OnConectSuccess += (string msg) => {  //链接成功后您要执行的事件 };

//连接失败事件 
vpn.OnConectFaild += (string msg) => {  //连接失败后您要执行的事件 };

//断开成功事件 
vpn.OnDisconectSuccess += (string msg) => {  //断开成功后您要执行的事件 };

//断开失败事件 
vpn.OnDisconectFaild += (string msg) => {  //断开失败后您要执行的事件 };

///VPN的链接
vpn.VPNConnect();

///VPN的断开
vpn.VPNDisconnect();
~~~

* ##### 异步的网络请求

~~~C#
/// 使用实例
QcForm.Net.HttpEntry http = new QcForm.Net.HttpEntry()
{
	Url = 请求的地址,
	Data = 发送的数据(a=a1&b=b1),
	Method = Mothed.POST | Mothed.GET,
};

///Http的异步委托事件

//请求失败事件 
http.OnError += (string msg) => {  //请求失败后您要执行的事件 };

//505,404,400等请求的事件 
http.OnNotFound += (string msg) => {  //505,404,400等请求失败后您要执行的事件 };

//请求成功后 
http.Request += (object htmlstring) => {  //请求成功后您要执行的事件 };

///执行请求
http.doRequest();
~~~



----------------------------------------------------------------------------------
### Win10 常见的问题
* ##### VPN无法链接（Win7下正常）  
	解决方法：  
	打开控制面板，点击所有控制面，网络链接，找到当前使用的已联网的无线网卡或者本地链接，右键属性找到Tcp/Ip6，然后取消它保存。重新使用VPN链接即可解决。