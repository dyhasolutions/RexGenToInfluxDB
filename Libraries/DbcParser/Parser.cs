using System.IO;
using System.Collections.Generic;
using DbcParserLib.Parsers;

namespace DbcParserLib
{
    public static class Parser
    {
        private static IEnumerable<ILineParser> LineParsers = new List<ILineParser>()
        {
            new IgnoreLineParser(), // Used to skip line we know we want to skip
            new NodeLineParser(),
            new MessageLineParser(),
            new CommentLineParser(),
            new SignalLineParser(),
            new ValueTableLineParser(),
            new PropertiesLineParser(),
            new UnknownLineParser() // Used as a catch all 
        };

        public static Dbc ParseFromPath(string dbcPath)
        {
            using (var fileStream = new FileStream(dbcPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                return ParseFromStream(fileStream);
            }
        } 

        public static Dbc ParseFromStream(Stream dbcStream)
        {
            using(var reader = new StreamReader(dbcStream))
            {
                return ParseFromReader(reader);
            }
        } 

        public static Dbc Parse(string dbcText)
        {
            using(var reader = new StringReader(dbcText))
            {
                return ParseFromReader(reader);
            }
        } 

        private static Dbc ParseFromReader(TextReader reader)
        {
            var builder = new DbcBuilder();

            while(reader.Peek() >= 0)
                ParseLine(reader.ReadLine(), builder);

            return builder.Build();
        }

        private static void ParseLine(string line, IDbcBuilder builder)
        {
            if(string.IsNullOrWhiteSpace(line))
                return;

            foreach(var parser in LineParsers)
            {
                if(parser.TryParse(line, builder))
                    break;
            }
        } 
    }
}