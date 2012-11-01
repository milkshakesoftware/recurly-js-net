using System;
using System.Configuration;

namespace RecurlyJS
{
	internal sealed class RecurlyConfig
	{
		/// <summary>
		/// Recurly API Key
		/// </summary>
		public static string ApiKey { get { return GetRecurlyAppSetting("ApiKey"); } }

		/// <summary>
		/// Recurly Site Subdomain
		/// </summary>
		public static string Subdomain { get { return GetRecurlyAppSetting("Subdomain"); } }

		/// <summary>
		/// Recurly Private Key for Transparent Post API
		/// </summary>
		public static string PrivateKey { get { return GetRecurlyAppSetting("PrivateKey"); } }

		/// <summary>
		/// Recurly Environment: Production or Sandbox
		/// </summary>
		public static EnvironmentType Environment
		{
			get
			{
				string environment = GetRecurlyAppSetting("Environment");

				return (EnvironmentType)Enum.Parse(typeof(EnvironmentType), environment);
			}
		}

		private static string GetRecurlyAppSetting(string name)
		{
			return ConfigurationManager.AppSettings.Get(String.Concat("Recurly:", name));
		}
	}
}