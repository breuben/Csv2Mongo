using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Csv2Mongo
{
	public class MongoDocumentWriter : IDocumentWriter
	{
		private readonly MongoDatabase _db;
		private readonly MongoCollection<BsonDocument> _collection;
		private Schema _schema;

		List<BsonDocument> buffer = new List<BsonDocument>();
		const int maxBufferSize = 1;

		public MongoDocumentWriter(string connectionString, string databaseName, string collectionName)
		{
			var id = new ObjectId();
			var server = new MongoClient(connectionString).GetServer();
			_db = server.GetDatabase(databaseName);

			if (_db.CollectionExists(collectionName))
				_db.DropCollection(collectionName);

			_db.CreateCollection(collectionName);
			_collection = _db.GetCollection(collectionName);
		}

		public Schema Schema
		{
			set { _schema = value; }
			private get { return _schema; }
		}

		public void Write(Document document)
		{
			var bsonDocument = GetBsonDocument(document);
			//_collection.Insert(bsonDocument);

			buffer.Add(bsonDocument);
			if (buffer.Count >= maxBufferSize)
				Flush();
		}

		private void Flush()
		{
			_collection.InsertBatch(buffer);
			buffer.Clear();
		}

		private BsonDocument GetBsonDocument(Document document)
		{
			var bsonDocument = new BsonDocument();

			for (int i = 0; i < Schema.FieldDefinitions.Count; i++)
			{
				var definition = Schema.FieldDefinitions[i];
				bsonDocument.Add(definition.Name, BsonTypeMapper.MapToBsonValue(document[definition.Name]));
			}

			return bsonDocument;
		}

		public void Dispose()
		{
			if (buffer.Count > 0)
				Flush();
		}
	}
}
