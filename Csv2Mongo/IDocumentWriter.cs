using System;

namespace Csv2Mongo
{
	interface IDocumentWriter : IDisposable
	{
		Schema Schema { set; }
		void Write(Document document);
	}
}
