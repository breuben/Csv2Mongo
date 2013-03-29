using System.Security.Cryptography;
using System.Text;

namespace Csv2Mongo
{
	static class Hasher
	{
		public static string GetHashString(string toBeHashed)
		{
			byte[] buffer = Encoding.UTF8.GetBytes(toBeHashed);
			MD5 md5 = MD5.Create();

			byte[] hashBytes = md5.ComputeHash(buffer);
			return hashBytes.ToHexString();
		}

		public static string ToHexString(this byte[] buffer)
		{
			StringBuilder sb = new StringBuilder();

			foreach (byte b in buffer)
				sb.AppendFormat("{0:x2}", b);

			return sb.ToString();
		}
	}
}
