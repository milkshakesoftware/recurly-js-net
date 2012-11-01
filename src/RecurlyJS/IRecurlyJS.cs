using System.Threading.Tasks;

namespace RecurlyJS
{
	public interface IRecurlyJS
	{
		/// <summary>
		/// Signs the subscription.
		/// </summary>
		/// <param name="planCode">The plan code.</param>
		/// <returns>Returns the signed string.</returns>
		string SignSubscription(string planCode);

		/// <summary>
		/// Signs the billing info update.
		/// </summary>
		/// <param name="accountCode">The account code.</param>
		/// <returns>Returns the signed string.</returns>
		string SignBillingInfoUpdate(string accountCode);

		/// <summary>
		/// Signs the transaction.
		/// </summary>
		/// <param name="amountInCents">The amount in cents.</param>
		/// <param name="currencyCode">The (3-letter) currency code.</param>
		/// <returns>Returns the signed string.</returns>
		string SignTransaction(int amountInCents, string currencyCode);

		/// <summary>
		/// Fetches the result of a Recurly.js transaction.
		/// </summary>
		/// <remarks>
		/// It will return either a Subscription, BillingInfo, or Invoice object.
		/// </remarks>
		/// <param name="recurlyToken">The recurly token.</param>
		/// <returns>Returns a string, representing XML of the object.</returns>
		Task<string> Fetch(string recurlyToken);
	}
}