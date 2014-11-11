using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts
{
    public class NodeWrapper<Node> : GridObject, IHasNeighbors<Node>, IDistance
        where Node : GridObject, IHasNeighbors<Node>
    {
        Node node;
        public NodeWrapper(Node node, int distance)
            : base(node.Location)
        {
            this.node = node;
            this.Distance = distance;
        }

        public IEnumerable<Node> Neighbors
        {
            get { return node.Neighbors; }
        }

        public int Distance { get; set; }
    }
}
