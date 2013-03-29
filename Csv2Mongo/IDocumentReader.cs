using System;

namespace Csv2Mongo
{
	internal interface IDocumentReader : IDisposable
	{
		Schema Schema { get; }
		bool Read();
		Document Current { get; }
	}
}
