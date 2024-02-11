using System.Net.Mail;
using System.Net;
using System.Text.RegularExpressions;

namespace ServMidMan.Helper
{
    public static class EmailVerificator
    {
        public static Dictionary<string,(string, DateTime)> emailWithVerification = new Dictionary<string, (string, DateTime)>();
        private static Timer _timer = new Timer(new TimerCallback(OnTimedEvent), null, 0, 300000);
        public static void SendVerificationEmail(string email, string verificationCode)
        {
            if (emailWithVerification == null)
            {
                emailWithVerification = new Dictionary<string, (string,DateTime)>();
            }
            if (_timer == null)
            {
                _timer = new Timer(new TimerCallback(OnTimedEvent), null, 0, 300000);
            }
            if (emailWithVerification.ContainsKey(email))
            {
				emailWithVerification.Remove(email);
			}
            emailWithVerification.Add(email, (verificationCode,DateTime.Now));
            // Set up the sender's email address and display name
            var fromAddress = new MailAddress("info.servmidman@salmadevelop.eu", "ServMidMan");

            // Create a new email address for the recipient
            var toAddress = new MailAddress(email);

            // Set the sender's email password (Note: You should not hardcode this in production)
            const string fromPassword = "myNewDatabasePassword1";

            // Set the email subject
            const string subject = "Email Verification";

            // Construct the email body including the verification code
            string body = "Your verification code is: " + verificationCode + ". Please enter this code to verify your email address.";


            // Set up the SMTP client with the SMTP server details
            var smtp = new SmtpClient
            {
                Host = "smtp.m1.websupport.sk", // Change this to your SMTP server
                Port = 587, // Port 587 is typically used for email submission
                EnableSsl = false, // Enable SSL/TLS encryption
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false, // Ensure you do not use default credentials
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword) // Set the sender's credentials
            };

            // Create a new email message
            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body
            })
            {
                try
                {
                    smtp.Send(message);
                }catch (Exception ex)
                {

                }

            }
        }

        private static void OnTimedEvent(object state)
        {
            // Iterate over dictionary and remove expired records
            foreach (var kvp in emailWithVerification.ToArray())
            {
                if ((DateTime.Now - kvp.Value.Item2).TotalMinutes >= 5)
                {
                    emailWithVerification.Remove(kvp.Key);
                    Console.WriteLine($"Record with email '{kvp.Key}' removed due to expiration.");
                }
            }
        }
        public static bool IsValidEmail(string email)
        {
            // Regular expression for basic email validation
            string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            Regex regex = new Regex(pattern);

            return regex.IsMatch(email);
        }
    }
}
