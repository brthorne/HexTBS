using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    public interface ITransformer
    {
        Vector3 GetWorldCoords(Point p);
        Point GetGridCoords(Vector3 v);
    }
}
