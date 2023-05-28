using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuestionnaireProject.Services
{
    public class MySmsService
    {
        public MySmsService(string phoneNumber, string message)
        {
            SmsServiceIran.ServiceSoapClient ss = new SmsServiceIran.ServiceSoapClient();
            string username = "vsms3281";
            string password = "13741374";
            string smstext = message;
            string mobileno = phoneNumber;
            string panelno = "50002202525";
            ss.SendSMS(username, password, smstext, mobileno, panelno);            
        }
    }
}