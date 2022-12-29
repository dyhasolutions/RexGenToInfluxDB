using DbcParserLib.Model;
using System.Linq;

namespace DbcParserLib.Parsers
{
    public class NodeLineParser : ILineParser
    {
        private const string NodeLineStarter = "BU_:";

        public bool TryParse(string line, IDbcBuilder builder)
        {
            if (line.TrimStart().StartsWith(NodeLineStarter) == false)
                return false;

            foreach (var nodeName in line.SplitBySpace().Skip(1))
            {
                var node = new Node()
                {
                    Name = nodeName
                };
                builder.AddNode(node);
            }

            return true;
        }
    }
}