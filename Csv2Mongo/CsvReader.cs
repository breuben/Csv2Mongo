using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Csv2Mongo
{
	public class CsvReader : IDocumentReader
	{
		private StreamReader reader;
		private Schema _schema;
		private Document _current;

		public CsvReader(string filepath, Encoding encoding)
		{
			reader = new StreamReader(filepath, encoding);
			parseHeader(reader.ReadLine());
		}

		private void parseHeader(string line)
		{
			var fields = SplitLine(line);

			List<FieldDefinition> fieldDefinitions = new List<FieldDefinition>();
			foreach (var field in fields)
			{
				fieldDefinitions.Add(parseFieldDefinition(field));
			}

			_schema = new Schema { FieldDefinitions = fieldDefinitions };
		}

		private FieldDefinition parseFieldDefinition(string field)
		{
			if (field.Contains(":"))
			{
				var components = field.Split(':');
				return new FieldDefinition { Name = components[0], Type = stringToType(components[1]) };
			}

			return new FieldDefinition { Name = field, Type = typeof(string) };
		}

		private Type stringToType(string typename)
		{
			switch (typename)
			{
				case "int":
					return typeof(int);
				case "double":
					return typeof(double);
				default:
					return typeof(string);
			}
		}

		private static List<string> SplitLine(string line)
		{
			var values = new List<string>();

			var matches = Regex.Matches(line, "(?:\"(?<m>[^\"]*)\")|(?<m>[^,]+)");
			foreach (Match match in matches)
				values.Add(match.Groups["m"].Value);

			return values;
		}

		public Schema Schema
		{
			get { return _schema; }
		}

		public bool Read()
		{
			string line = reader.ReadLine();

			if (line == null)
			{
				_current = null;
				return false;
			}

			_current = parseRecord(line);
			return true;
		}

		private Document parseRecord(string line)
		{
			var document = new Document();

			var values = SplitLine(line);

			for (int i = 0; i < Schema.FieldDefinitions.Count; i++)
			{
				var definition = Schema.FieldDefinitions[i];

				if (i < values.Count)
					document.values.Add(definition.Name, getValue(values[i], definition));
				else
					document.values.Add(definition.Name, null);
			}

			return document;
		}

		private object getValue(string value, FieldDefinition fieldDefinition)
		{
			if (fieldDefinition.Type == typeof(int))
				return Convert.ToInt32(value);

			if (fieldDefinition.Type == typeof(double))
				return Convert.ToDouble(value);

			return value.Trim();
		}

		public Document Current
		{
			get { return _current; }
		}

		public void Dispose()
		{
			if (reader != null)
				reader.Close();
		}
	}
}
