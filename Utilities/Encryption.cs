using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MauloaDemo.Utilities {

	public static class Encryption {
		private const string _passfrase = "VisualSystemsKeys";

		public static string Encrypt(string original) {
			MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
			byte[] pwdhash = hashmd5.ComputeHash(Encoding.UTF8.GetBytes(_passfrase));

			TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();
			des.Key = pwdhash;
			des.Mode = CipherMode.ECB;
			byte[] buff = Encoding.UTF8.GetBytes(original);

			string encrypted = Convert.ToBase64String(des.CreateEncryptor().TransformFinalBlock(buff, 0, buff.Length));
			return encrypted;
		}

		public static string Decrypt(string encrypted) {
			if (string.IsNullOrWhiteSpace(encrypted)) {
				encrypted = String.Empty;
			}

			MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
			byte[] pwdhash = hashmd5.ComputeHash(Encoding.UTF8.GetBytes(_passfrase));

			TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();
			des.Key = pwdhash;
			des.Mode = CipherMode.ECB;

			byte[] buff = Convert.FromBase64String(encrypted);

			string decrypted = Encoding.UTF8.GetString(des.CreateDecryptor().TransformFinalBlock(buff, 0, buff.Length));
			return decrypted;
		}
	}
}