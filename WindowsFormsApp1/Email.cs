
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace LC5.Email
{
    public class MailHandler
    {
        private MailMessage _mailMessage;
        private string _host;
        private string _userName;
        private string _password;

        public MailHandler()
        {
        }

        /// <summary>
        /// 设置邮件信息
        /// </summary>
        /// <param name="subject">主体</param>
        /// <param name="body">内容</param>
        /// <param name="from">发件人</param>
        /// <param name="to">收件人</param>
        /// <param name="cc">抄送人</param>
        /// <param name="bcc">密件抄送人</param>
        /// <param name="isBodyHtml">内容是否为Html</param>
        public void SetMailMessage(string subject, string body, string from, string[] to, string[] cc, string[] bcc, bool isBodyHtml = true)
        {
            _mailMessage = new MailMessage();
            _mailMessage.Subject = subject;
            _mailMessage.Body = body;
            _mailMessage.IsBodyHtml = isBodyHtml;

            _mailMessage.From = new MailAddress(from);
            if (to != null)
            {
                foreach (var item in to)
                {
                    _mailMessage.To.Add(item);
                }
            }
            if (cc != null)
            {
                foreach (var item in cc)
                {
                    _mailMessage.CC.Add(item);
                }
            }
            if (bcc != null)
            {
                foreach (var item in bcc)
                {
                    _mailMessage.Bcc.Add(item);
                }
            }

            _mailMessage.BodyEncoding = System.Text.Encoding.UTF8;
        }

        /// <summary>
        /// 配置Smtp服务主机及身份验证
        /// </summary>
        /// <param name="host">Smtp主机名或Ip</param>
        /// <param name="userName">用户名</param>
        /// <param name="password">密码</param>
        public void SetSmtp(string host, string userName, string password)
        {
            this._host = host;
            this._userName = userName;
            this._password = password;
        }

        /// <summary>
        /// 发送邮件
        /// </summary>
        public void Send()
        {

            using (SmtpClient sc = new SmtpClient())
            {
                sc.Host = _host;
                sc.Port = 25;
                sc.DeliveryMethod = SmtpDeliveryMethod.Network;
                sc.Credentials = new System.Net.NetworkCredential(_userName, _password);
                sc.Send(_mailMessage);
            }

        }

        public string SendMail(string title, string content, string mailToAddress, string smptHost, string userName, string password)
        {


            if (string.IsNullOrWhiteSpace(smptHost))
            {
                return "SmtpHost为空";
            }
            if (string.IsNullOrWhiteSpace(userName))
            {
                return "发件人为空";
            }
            if (string.IsNullOrWhiteSpace(password))
            {
                return "发件人密码为空";
            }
            if (mailToAddress.Length == 0)
            {
                return "收件人列表为空";
            }

            var mailContent = @"<html><head><title>邮件内容</title></head>
                                <body>" + content + ":" + title + "</body></html>";

            SetSmtp(smptHost, userName, password);
            SetMailMessage(title, mailContent, userName, new string[] { mailToAddress }, null, null);
            string sql = "";
            try
            {
                Send();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return null;
        }
    }
}
