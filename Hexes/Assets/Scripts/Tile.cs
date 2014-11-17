using UnityEngine;
using System.Collections.Generic;
using System.Linq;


namespace Assets.Scripts
{
    public class Tile : GridObject, IHasNeighbors<Tile>
    {
        public bool Passable;

        public Tile(int x, int y): base(x,y)
        {
            Passable = true;
        }

        public IEnumerable<Tile> AllNeighbors { get; set; }
        public IEnumerable<Tile> Neighbors
        {
            get
            {
                return AllNeighbors.Where(o => o.Passable);
            }
        }

        //change of coordinates when moving in any direction
        //even rows are shifted left relative to odd rows
        public static List<Point> NeighbourShift(Point p)
        {
            List<Point> neighbors;
            if (p.Y % 2 == 0) 
            { 
                neighbors = new List<Point>
                {
                    new Point(-1, 0),
                    new Point(-1,1),
                    new Point(0,1),
                    new Point(1,0),
                    new Point(0,-1),
                    new Point(-1,-1)
                };
            }
            else
            {
                neighbors = new List<Point>
                {
                    new Point(-1,0),
                    new Point(0,1),
                    new Point(1,1),
                    new Point(1,0),
                    new Point(1,-1),
                    new Point(0,-1)
                };
            }
            return neighbors;
        }

        public void FindNeighbours(Dictionary<Point, Tile> Board,
            Vector2 BoardSize, bool EqualLineLengths)
        {
            List<Tile> neighbors = new List<Tile>();

            foreach (Point point in NeighbourShift(this.Location))
            {
                int neighbourX = X + point.X;
                int neighbourY = Y + point.Y;

                //If every second hexagon row has less hexagons than the first one, 
                //just skip the last one when we come to it
                if (neighbourY % 2 != 0 && !EqualLineLengths &&
                    neighbourX  == BoardSize.x - 1)
                    continue;
                //Check to determine if currently processed coordinate is still inside the board limits
                if (neighbourX >= 0 &&
                    neighbourX < BoardSize.x &&
                    neighbourY >= 0 && neighbourY < BoardSize.y)
                    neighbors.Add(Board[new Point(neighbourX, neighbourY)]);
            }

            AllNeighbors = neighbors;
        }
    }
}