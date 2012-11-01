using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace RecurlyJS
{
	public class RecurlyJS : IRecurlyJS
	{
		private static string NONCE_PARAMETER = "nonce";
		private static string TIMESTAMP_PARAMETER = "timestamp";
		private static string SUBSCRIPTION_PARAMETER = "subscription%5Bplan_code%5D";

		private static string BILLING_INFO_UPDATE_PARAMETER = "account%5Baccount_code%5D";

		private static string TRANSACTION_AMOUNT_PARAMETER = "transaction%5Bamount_in_cents%5D";
		private static string TRANSACTION_CURRENCY_PARAMETER = "transaction%5Bcurrency%5D";

		/// <summary>
		/// Signs the subscription.
		/// </summary>
		/// <param name="planCode">The plan code.</param>
		/// <returns>Returns the signed string.</returns>
		public string SignSubscription(string planCode)
		{
			var subscription = string.Format("{0}={1}", SUBSCRIPTION_PARAMETER, planCode);

			return SignWithParameters(subscription);
		}

		/// <summary>
		/// Signs the billing info update.
		/// </summary>
		/// <param name="accountCode">The account code.</param>
		/// <returns>Returns the signed string.</returns>
		public string SignBillingInfoUpdate(string accountCode)
		{
			var account = string.Format("{0}={1}", BILLING_INFO_UPDATE_PARAMETER, accountCode);

			return SignWithParameters(account);
		}

		/// <summary>
		/// Signs the transaction.
		/// </summary>
		/// <param name="amountInCents">The amount in cents.</param>
		/// <param name="currencyCode">The (3-letter) currency code.</param>
		/// <returns>Returns the signed string.</returns>
		public string SignTransaction(int amountInCents, string currencyCode)
		{
			var amount = string.Format("{0}={1}", TRANSACTION_AMOUNT_PARAMETER, amountInCents);
			var currency = string.Format("{0}={1}", TRANSACTION_CURRENCY_PARAMETER, currencyCode);

			return SignWithParameters(amount, currency);
		}

		/// <summary>
		/// Fetches the result of a Recurly.js transaction.
		/// </summary>
		/// <remarks>
		/// It will return either a Subscription, BillingInfo, or Invoice object.
		/// </remarks>
		/// <param name="recurlyToken">The recurly token.</param>
		/// <returns>Returns a string, representing XML of the object.</returns>
		public async Task<string> Fetch(string recurlyToken)
		{
			HttpClient client = new HttpClient();
			client.DefaultRequestHeaders.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/xml"));
			client.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse(String.Join(" ", "Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes(RecurlyConfig.ApiKey))));

			return await client.GetStringAsync(String.Concat("https://api.recurly.com/v2/recurly_js/result/", recurlyToken)).ConfigureAwait(false);
		}

		private static string SignWithParameters(params string[] parameters)
		{
			var nonce = string.Format("{0}={1}", NONCE_PARAMETER, Guid.NewGuid().ToString());
			var timestamp = string.Format("{0}={1}", TIMESTAMP_PARAMETER, GetUnixTimeStamp(DateTime.UtcNow));

			List<string> signatureParameters = new List<string>();
			signatureParameters.Add(nonce);
			signatureParameters.AddRange(parameters);
			signatureParameters.Add(timestamp);

			var protectedString = String.Join("&", signatureParameters);

			return GenerateHMAC(protectedString) + "|" + protectedString;
		}

		private static string GenerateHMAC(string stringToHash)
		{
			var privateKey = RecurlyConfig.PrivateKey;
			var hasher = new HMACSHA1(UTF8Encoding.UTF8.GetBytes(privateKey));
			var hashBytes = hasher.ComputeHash(UTF8Encoding.UTF8.GetBytes(stringToHash));

			var hexDigest = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();

			return hexDigest;
		}

		private static int GetUnixTimeStamp(DateTime timestamp)
		{
			var referenceDate = new DateTime(1970, 1, 1);
			var ts = new TimeSpan(timestamp.Ticks - referenceDate.Ticks);

			return (Convert.ToInt32(ts.TotalSeconds));
		}
	}
}