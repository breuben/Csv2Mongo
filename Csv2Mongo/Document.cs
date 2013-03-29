using System;
using System.Collections.Generic;

namespace Csv2Mongo
{
	public class FieldDefinition
	{
		public string Name;
		public Type Type;
	}

	public class Schema
	{
		public List<FieldDefinition>  FieldDefinitions { get; set; }
	}

	public class Document
	{
		internal Dictionary<string, object> values = new Dictionary<string, object>();
		public object this[string fieldName]
		{
			get { return values[fieldName]; }
		}
	}
}
