using System;
using Microsoft.Office.Interop.Outlook;

namespace GTD.Util
{
    public class OutlookAppointment
    {
        public void AddAppointment()
        {
            //这个方法需要本地有outlook，引入com  microsoft.office.interop.outlook 16版本
            //from http://www.codeproject.com/Tips/84321/Setting-up-an-MS-Outlook-Appointment-C
            Application outlookApplication = new Application();
            AppointmentItem oAppointment = (AppointmentItem)outlookApplication.CreateItem(OlItemType.olAppointmentItem);
            oAppointment.Subject = "约会标题"; // set the subject
            oAppointment.Body = "约会内容"; // set the body
            oAppointment.Location = "任意地点"; // set the location
            oAppointment.Start = Convert.ToDateTime(DateTime.Now); // Set the start date 
            oAppointment.End = Convert.ToDateTime(DateTime.Now.AddMinutes(2)); // End date 
            oAppointment.ReminderSet = true; // Set the reminder
            oAppointment.ReminderMinutesBeforeStart = 5; // reminder time
            oAppointment.Importance = OlImportance.olImportanceHigh; // appointment importance
            oAppointment.BusyStatus = OlBusyStatus.olBusy;
            oAppointment.AllDayEvent = false;
            oAppointment.Save();
            //到这里已经保存到本地outlook中了


            ////Step 6 // send the details in email
            //MailItem mailItem = oAppointment.ForwardAsVcal();
            //// email address to send to 
            //mailItem.To = "mailto:me@decodedsolutions.co.uk";
            //// send 
            //mailItem.Send();
        }
    }
}