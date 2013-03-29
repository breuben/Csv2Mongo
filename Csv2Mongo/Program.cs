using System;
using System.Text;
using MongoDB.Bson;

namespace Csv2Mongo
{
	class Program
	{
		const string connectionString = "mongodb://localhost";
		const string databaseName = "GeoData";

		private static void Main()
		{
			Console.WriteLine(Hasher.GetHashString("duchess"));
			Console.WriteLine(Hasher.GetHashString("duchess.amhscentral.com"));
			ObjectId id = ObjectId.GenerateNewId();
			Console.WriteLine(BitConverter.GetBytes(id.Machine).ToHexString());
		}

		private void bulkTransfer()
		{
			DateTime startTime = DateTime.Now;

			bulkTransfer(@"C:\users\breuben\downloads\Countries.txt", "Countries");
			bulkTransfer(@"C:\users\breuben\downloads\Regions.txt", "Regions");
			bulkTransfer(@"C:\users\breuben\downloads\Cities.txt", "Cities");

			DateTime endTime = DateTime.Now;

			Console.WriteLine("Elapsed time {0}s", (endTime-startTime).TotalSeconds);
		}

		private static void bulkTransfer(string csvFile, string collectionName)
		{
			IDocumentReader reader = new CsvReader(csvFile, Encoding.GetEncoding(1252));
			IDocumentWriter writer = new MongoDocumentWriter(connectionString, databaseName, collectionName);

			bulkTransfer(reader, writer);
		}

		private static void bulkTransfer(IDocumentReader reader, IDocumentWriter writer)
		{
			using (reader)
			{
				using (writer)
				{
					writer.Schema = reader.Schema;

					while (reader.Read())
					{
						var doc = reader.Current;
						writer.Write(doc);
					}
				}
			}
		}
	}
}
