using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    /// <summary>
    /// robots in disguise
    /// </summary>
    public class Transformer : ITransformer
    {
        private float hexWidth, hexHeight;
        public Transformer(float hexWidth, float hexHeight)
        {
            this.hexWidth = hexWidth;
            this.hexHeight = hexHeight;
        }

        public Vector3 GetWorldCoords(Point p)
        {
            //Every second row is offset by half of the tile width
            float rowOffset = getRowOffset(p.Y);

            float x = p.X * hexWidth + rowOffset;
            //Every new line is offset in z direction by 3/4 of the hexagon height
            float z = p.Y * hexHeight * 0.75f;
            return new Vector3(x, 0, z);
        }

        public Point GetGridCoords(Vector3 v)
        {
            int y = Mathf.RoundToInt(v.z / hexHeight / 0.75f);
            float rowOffset = getRowOffset(y);
            int x = Mathf.RoundToInt((v.x - rowOffset) / hexWidth);
            return new Point(x, y);
        }

        //Every second row is offset by half of the tile width
        private float getRowOffset(int rowNumber)
        {
            return rowNumber % 2 == 0 ? 0 : hexWidth / 2;
        }
    }
}
