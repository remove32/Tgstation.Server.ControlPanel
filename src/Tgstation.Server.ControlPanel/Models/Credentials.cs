﻿using Newtonsoft.Json;
using System;
using System.Security.Cryptography;
using System.Text;

namespace Tgstation.Server.ControlPanel.Models
{
	public class Credentials
	{
		public string Username { get; set; }

		[JsonIgnore]
		public string Password
		{
			get => Decrypt();
			set => Encrypt(value);
		}

#pragma warning disable CA1819 // Properties should not return arrays
		public byte[] CipherText { get; set; }
#pragma warning restore CA1819 // Properties should not return arrays

		public byte[] Entropy { get; set; }
		
		void Encrypt(string cleartext)
		{
			var clearTextBytes = Encoding.UTF8.GetBytes(cleartext);
			try
			{
				byte[] bentropy = new byte[20];
				using (var rng = new RNGCryptoServiceProvider())
					rng.GetBytes(bentropy);

				CipherText = ProtectedData.Protect(clearTextBytes, bentropy, DataProtectionScope.CurrentUser);

				Entropy = bentropy;
			}
			catch (PlatformNotSupportedException)
			{
				CipherText = clearTextBytes;
			}
		}
		
		public string Decrypt()
		{
			byte[] clearTextBytes;
			try
			{
				clearTextBytes = ProtectedData.Unprotect(CipherText, Entropy, DataProtectionScope.CurrentUser);
			}
			catch (PlatformNotSupportedException)
			{
				clearTextBytes = CipherText;
			}
			return Encoding.UTF8.GetString(clearTextBytes);
		}

	}
}