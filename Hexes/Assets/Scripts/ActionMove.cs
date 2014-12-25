using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts
{
    public class ActionMove : IAction
    {
        private Point start, dest;

        public ActionMove(Point start, Point dest)
        {
            this.start = start;
            this.dest = dest;
        }

        public void Execute()
        {
            GridManager.instance.Move(start, dest);
        }
    }
}
