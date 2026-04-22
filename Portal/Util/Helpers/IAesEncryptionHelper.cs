using System.Threading.Tasks;

namespace portalAdministrativoSISEC.Util.Helpers
{
	public interface IAesEncryptionHelper
	{
		Task<string> Encrypt(string plainText);

		Task<string> Decrypt(string cipherText);
	}
}
