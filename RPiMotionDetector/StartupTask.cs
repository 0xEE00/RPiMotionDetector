using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using Windows.ApplicationModel.Background;
using Windows.Devices.Gpio;
using Windows.ApplicationModel.Email;
using System.Threading.Tasks;
using EASendMailRT;

// The Background Application template is documented at http://go.microsoft.com/fwlink/?LinkID=533884&clcid=0x409

namespace RPiMotionDetector
{
    public sealed class StartupTask : IBackgroundTask
    {
        private const int PIR_PIN = 4;
        private GpioPin PinPIR;

        public void Run(IBackgroundTaskInstance taskInstance)
        {
            // 
            // TODO: Insert code to perform background work
            //
            // If you start any asynchronous methods here, prevent the task
            // from closing prematurely by using BackgroundTaskDeferral as
            // described in http://aka.ms/backgroundtaskdeferral
            //
            BackgroundTaskDeferral deferral = taskInstance.GetDeferral();
            //while (true)
            //{
            InitializeGPIO();
            //await SendMail();
            //    await Task.Delay(60000);
            //}
            deferral.Complete();


        }

        private void InitializeGPIO()
        {
            try
            {
                var gpio = GpioController.GetDefault();
                PinPIR = gpio.OpenPin(PIR_PIN);
                PinPIR.DebounceTimeout = new TimeSpan(0, 0, 0, 0, 50); //PIR motion sensor - Ignore changes in value of less than 50ms
                PinPIR.SetDriveMode(GpioPinDriveMode.Input); //set the mode as Input (we are READING a signal from this port)
                PinPIR.ValueChanged += PinPIR_ValueChanged; //when this value changes (motion is detected), the function is called
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async void PinPIR_ValueChanged(GpioPin sender, GpioPinValueChangedEventArgs args)
        {
            try
            {
                if (args.Edge == GpioPinEdge.RisingEdge)
                    await SendMail();

                //MailMessage mail = new MailMessage("you@yourcompany.com", "user@hotmail.com");
                //SmtpClient client = new SmtpClient();
                //client.Port = 25;
                //client.DeliveryMethod = SmtpDeliveryMethod.Network;
                //client.UseDefaultCredentials = false;
                //client.Host = "smtp.gmail.com";
                //mail.Subject = "this is a test email.";
                //mail.Body = "this is my test email body";
                //client.Send(mail);

            }
            catch (Exception)
            {

                throw;
            }
            finally
            {

            }
        }

        private async Task SendMail()
        {
            try
            {
                SmtpMail oMail = new SmtpMail("TryIt");
                SmtpClient oSmtp = new SmtpClient();

                // Set sender email address, please change it to yours
                oMail.From = new MailAddress("blanusaf1@gmail.com");

                // Add recipient email address, please change it to yours
                oMail.To.Add(new MailAddress("drazen.blanusa@nps.rs"));

                // Set email subject
                oMail.Subject = "Detekcija pokreta";

                // Set Html body
                oMail.HtmlBody = "<font size=5>Detektovan je pokret!!!</font> <font color=red><b>Detektovan je pokret!!!</b></font>";

                // get a file path from PicturesLibrary,
                // to access files in PicturesLibrary, you MUST have "Pictures Library" checked in
                // your project -> Package.appxmanifest -> Capabilities




                //Windows.Storage.StorageFile file =
                //    await Windows.Storage.KnownFolders.PicturesLibrary.GetFileAsync("test.jpg");

                //string attfile = file.Path;
                //Attachment oAttachment = await oMail.AddAttachmentAsync(attfile);




                // if you want to add attachment from remote URL instead of local file.
                // string attfile = "http://www.emailarchitect.net/test.jpg";
                // Attachment oAttachment = await oMail.AddAttachmentAsync(attfile);

                // you can change the Attachment name by
                // oAttachment.Name = "mytest.jpg";

                // Your SMTP server address
                SmtpServer oServer = new SmtpServer("smtp.gmail.com")
                {

                    // User and password for ESMTP authentication
                    User = "blanusaf1@gmail.com",
                    Password = "puhtegeyvfctyvsa",

                    // If your SMTP server requires TLS connection on 25 port, please add this line
                    // oServer.ConnectType = SmtpConnectType.ConnectSSLAuto;

                    // If your SMTP server requires SSL connection on 465 port, please add this line
                    Port = 465,
                    ConnectType = SmtpConnectType.ConnectSSLAuto
                };

                await oSmtp.SendMailAsync(oServer, oMail);
            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}
