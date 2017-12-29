using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;
using DotRas;

using System.Text.RegularExpressions;


namespace QcForm.Net
{
    /// <summary>
    /// VPN的基类信息
    /// </summary>
    public abstract class VpnBase
    {

        public string VpnController()
        {
            return string.Format(@"{0}\rasdial.exe",
                Environment.GetFolderPath(Environment.SpecialFolder.System));
        }

        /// <summary>
        /// 需要连接的ip或域名
        /// </summary>
        public string DomainIp { get; set; }
        /// <summary>
        /// 需要显示的VPN名称
        /// </summary>
        public string VpnName { get; set; }
        /// <summary>
        /// VPN的账号
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// VPN的密码
        /// </summary>
        public string PassWord { get; set; }
        /// <summary>
        /// VPN的链接类型
        /// </summary>
        public string VpnType { get; set; }

       

        public VpnBase()
        {
            this.VpnType = "(PPTP)";
        }
        /// <summary>
        /// 创建或更新VPN
        /// </summary>
        public abstract void CreateOrUpdate();
        /// <summary>
        /// 删除VPN
        /// </summary>
        public abstract void DeleteVPN();
        /// <summary>
        /// 链接VPN
        /// </summary>
        public abstract void VPNConnect();
        /// <summary>
        /// 断开VPN
        /// </summary>
        public abstract void VPNDisconnect();

        public Boolean isEmpty(string s)
        {
            return (string.Empty == s || s == null);
        }
    }

    public class VPN : VpnBase
    {

        public delegate void Connect(string msg);

        public event Connect OnConectSuccess;

        public event Connect OnConectFaild;

        public event Connect OnDisconectSuccess;

        public event Connect OnDisconectFaild;

        public VPN()
            : base()
        {

        }
        /// <summary>
        /// 创建或更新VPN
        /// </summary>
        public override void CreateOrUpdate()
        {
            try
            {
                if (base.isEmpty(base.DomainIp))
                {
                    throw new Exception("Vpn的链接域名或IP不能为空");
                }

                if (base.isEmpty(base.VpnName))
                {
                    throw new Exception("Vpn的链接名称不能为空");
                }

                RasDialer dialer = new RasDialer();
                RasPhoneBook allUsersPhoneBook = new RasPhoneBook();
                allUsersPhoneBook.Open(true);
                if (allUsersPhoneBook.Entries.Contains(base.VpnName))
                {
                    allUsersPhoneBook.Entries[base.VpnName].PhoneNumber = base.DomainIp;
                    allUsersPhoneBook.Entries[base.VpnName].Update();
                }
                else
                {
                    RasEntry entry = RasEntry.CreateVpnEntry(base.VpnName, base.DomainIp, RasVpnStrategy.PptpFirst, RasDevice.GetDeviceByName(base.VpnType, RasDeviceType.Vpn));
                    allUsersPhoneBook.Entries.Add(entry);
                    dialer.EntryName = base.VpnName;
                    dialer.PhoneBookPath = RasPhoneBook.GetPhoneBookPath(RasPhoneBookType.AllUsers);
                }

            }
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        /// 删除VPN
        /// </summary>
        public override void DeleteVPN()
        {
            RasDialer dialer = new RasDialer();
            RasPhoneBook allUsersPhoneBook = new RasPhoneBook();
            allUsersPhoneBook.Open(true);
            if (allUsersPhoneBook.Entries.Contains(base.VpnName))
            {
                allUsersPhoneBook.Entries.Remove(base.VpnName);
            }
        }

        /// <summary>
        /// 是否关闭
        /// </summary>
        private bool isClosed = false;
        /// <summary>
        /// 捕捉到的异步的消息内容
        /// </summary>
        private string result = "";

        /// <summary>
        /// 连接VPN
        /// </summary>
        public override void VPNConnect()
        {
            try
            {
                result = "";

                string args = string.Format("{0} {1} {2}", base.VpnName, base.UserName, base.PassWord);

                Process myProcess = new Process();
                myProcess.StartInfo.FileName = base.VpnController();
                myProcess.StartInfo.Arguments = args;      // 参数  

                myProcess.StartInfo.CreateNoWindow = true;
                myProcess.StartInfo.UseShellExecute = false;
                myProcess.StartInfo.RedirectStandardInput = true;
                myProcess.StartInfo.RedirectStandardOutput = true;
                myProcess.StartInfo.RedirectStandardError = true;  
                
                myProcess.OutputDataReceived += (sender, e) =>
                {
                    Application.DoEvents();
                    result = string.Format("{0}{1}", result, e.Data);
                };
                myProcess.ErrorDataReceived += (sender, e) =>
                {
                    Application.DoEvents();
                    if (e.Data!=null && this.OnConectFaild != null)
                    {
                        this.OnConectFaild("无法连接或连接失败，请联系管理员!");
                    }
                };

                myProcess.EnableRaisingEvents = true;                      // 启用Exited事件  
                myProcess.Exited += (sender, e) =>
                {
                    Application.DoEvents();

                    string content = result;

                    string patten = @"已(经|)连接[\r\n\w\s。]*命令已完成。";

                    Regex reg = new Regex(patten, RegexOptions.IgnoreCase);

                    MatchCollection mcs = reg.Matches(content);

                    if (mcs.Count > 0)
                    {
                        if (this.OnConectSuccess != null)
                        {
                            this.OnConectSuccess(string.Format("{0}连接成功.", this.VpnName));
                        }
                    }
                    else
                    {
                        if (this.OnConectFaild != null)
                        {
                            this.OnConectFaild("无法连接或连接失败，请联系管理员!");
                        }
                    }
                };

                myProcess.Start();
                myProcess.BeginOutputReadLine();
                myProcess.BeginErrorReadLine();  
                
                
            }
            catch (Exception Ex)
            {
                if ( this.OnConectFaild != null )
                {
                    this.OnConectFaild(Ex.Message.ToString());
                }

                Debug.Assert(false, Ex.ToString());
            }
        }       

        /// <summary>
        /// 断开VPN连接
        /// </summary>
        public override void VPNDisconnect()
        {
            try
            {

                result = "";

                string args = string.Format("{0} /Disconnect", base.VpnName);

                Process myProcess = new Process();
                myProcess.StartInfo.FileName = base.VpnController();
                myProcess.StartInfo.Arguments = args;

                myProcess.StartInfo.CreateNoWindow = true;
                myProcess.StartInfo.UseShellExecute = false;
                myProcess.StartInfo.RedirectStandardInput = true;
                myProcess.StartInfo.RedirectStandardOutput = true;
                myProcess.StartInfo.RedirectStandardError = true;

                myProcess.OutputDataReceived += (sender, e) =>
                {
                    Application.DoEvents();
                    result = string.Format("{0}{1}", result, e.Data);
                };
                myProcess.ErrorDataReceived += (sender, e) =>
                {
                    Application.DoEvents();
                    if (e.Data != null && this.OnConectFaild != null)
                    {
                        this.OnDisconectFaild("无法连接或连接失败，请联系管理员!");
                    }
                };

                myProcess.EnableRaisingEvents = true;                      // 启用Exited事件  
                myProcess.Exited += (sender, e) =>
                {
                    Application.DoEvents();

                    string content = result;

                    string patten = @"(没有连接|)[\r\n\s]*命令已完成。";

                    Regex reg = new Regex(patten, RegexOptions.IgnoreCase);

                    MatchCollection mcs = reg.Matches(content);

                    if (mcs.Count > 0)
                    {
                        if (this.OnDisconectSuccess != null)
                        {
                            this.OnDisconectSuccess(string.Format("{0}断开连接成功.", base.VpnName));
                        }
                    }
                    else
                    {
                        if (this.OnDisconectFaild != null)
                        {
                            this.OnDisconectFaild("无法断开连接或连接不存在，请联系管理员!");
                        }
                    }
                };

                myProcess.Start();
                myProcess.BeginOutputReadLine();
                myProcess.BeginErrorReadLine();  
                
                
              
                
            }
            catch (Exception Ex)
            {
                if (this.OnDisconectFaild != null)
                {
                    this.OnDisconectFaild(Ex.Message.ToString());
                }

                Debug.Assert(false, Ex.ToString());
            }
        }
    }

}