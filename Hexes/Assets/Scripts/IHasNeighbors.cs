using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts
{
    //interface that should be implemented by grid nodes used in E. Lippert's generic path finding implementation
    public interface IHasNeighbors<N>
    {
        IEnumerable<N> Neighbors { get; }
    }
}
