using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts
{
    public static class PathFinder
    {
        //distance f-ion should return distance between two adjacent nodes
        //estimate should return distance between any node and destination node
        public static Path<Node> FindPath<Node>(
            Node start,
            Node destination,
            Func<Node, Node, int> distance,
            Func<Node, Node, double> estimate)
            where Node : IHasNeighbors<Node>
        {
            //set of already checked nodes
            var closed = new HashSet<Node>();
            //queued nodes in open set
            var queue = new PriorityQueue<double, Path<Node>>();
            queue.Enqueue(0f, new Path<Node>(start));

            while (!queue.IsEmpty)
            {
                var path = queue.Dequeue();

                if (closed.Contains(path.LastStep))
                    continue;
                if (path.LastStep.Equals(destination))
                    return path;

                closed.Add(path.LastStep);

                foreach (Node n in path.LastStep.Neighbors)
                {
                    double d = distance(path.LastStep, n);
                    //new step added without modifying current path
                    var newPath = path.AddStep(n, d);
                    queue.Enqueue(newPath.TotalCost + estimate(n, destination), newPath);
                }
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Node">element type to find neighbors for; generally tiles</typeparam>
        /// <param name="start">a node that we can start traversal from</param>
        /// <param name="maxDistance">number of nodes out from center to look</param>
        /// <returns>dictionary of reachable points and the min distance from start node</returns>
        public static Dictionary<Point, int> GetMovableArea<Node>(
            Node start,
            Func<GridObject, GridObject, int> distance,
            int maxDistance,
            Dictionary<Point,Node> board)
            where Node : GridObject, IHasNeighbors<Node>
        {
            var reachable = new Dictionary<Point, int>();
            var unvisited = new PriorityQueue<int,NodeWrapper<Node>>();

            //create wrappers and set distance to infinity.  add to univisited
            //foreach (Node n in board.Values )
            //{
            //    var wrappedNode = new NodeWrapper<Node>(n);             
            //    if (n == start)
            //    {
            //        wrappedNode.Distance = 0;
            //    }
            //    else
            //    {
            //        wrappedNode.Distance = 99999;
            //    }
            //    unvisited.Enqueue(wrappedNode.Distance,wrappedNode);            
            //}

            unvisited.Enqueue(0, new NodeWrapper<Node>(start,0));

            NodeWrapper<Node> currentNode;
            //will search entire graph every time.  need to cut off more than just updating reachable
            //on getting definitively past the maxDistance bound
            while (!unvisited.IsEmpty)
            {
                currentNode = unvisited.Dequeue();
                foreach (Node n in currentNode.Neighbors)
                {
                    int altDist = currentNode.Distance + distance(currentNode, n);
                    if (altDist <= maxDistance)                        
                    {
                        if(!reachable.ContainsKey(n.Location)){
                            reachable.Add(n.Location, altDist);                            
                            unvisited.Enqueue(altDist, new NodeWrapper<Node>(n,altDist));
                        }
                        else if (reachable[n.Location] > altDist)
                        {
                            reachable[n.Location] = altDist;
                            unvisited.Enqueue(altDist, new NodeWrapper<Node>(n,altDist));
                        }
                    }
                }
            }
            return reachable;
        }
    }
}

