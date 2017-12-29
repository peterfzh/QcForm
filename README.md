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

连接成功事件 vpn.OnConectSuccess += (string msg) => {  //链接成功后您要执行的事件 };

连接失败事件 vpn.OnConectFaild += (string msg) => {  //连接失败后您要执行的事件 };

断开成功事件 vpn.OnDisconectSuccess += (string msg) => {  //断开成功后您要执行的事件 };

断开失败事件 vpn.OnDisconectFaild += (string msg) => {  //断开失败后您要执行的事件 };


///VPN的链接
vpn.VPNConnect();

///VPN的断开
vpn.VPNDisconnect();



~~~



----------------------------------------------------------------------------------
### Win10 常见的问题
* ##### VPN无法链接（Win7下正常）  
	解决方法：  
	打开控制面板，点击所有控制面，网络链接，找到当前使用的已联网的无线网卡或者本地链接，右键属性找到Tcp/Ip6，然后取消它保存。重新使用VPN链接即可解决。