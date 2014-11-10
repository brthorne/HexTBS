using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Assets.Scripts;
using UnityEngine;

namespace Hexes.Test
{
    [TestClass]
    public class TransformerTest
    {
        private ITransformer x;
        [TestInitialize]
        public void Before()
        {
            x = new Transformer(.73f,.73f);
            
        }

        [TestMethod]
        public void GridToWorldAndBack11()
        {
            Point p = new Point(1,1);
            Vector3 worldCoord = x.GetWorldCoords(p);
            Point q = x.GetGridCoords(worldCoord);
            Assert.AreEqual<Point>(p, q);
        }

        [TestMethod]
        public void GridToWorldAndBack33()
        {
            Point p = new Point(3, 3);
            Vector3 worldCoord = x.GetWorldCoords(p);
            Point q = x.GetGridCoords(worldCoord);
            Assert.AreEqual<Point>(p, q);
        }

        [TestMethod]
        public void GridToWorldAndBack00()
        {
            Point p = new Point(0, 0);
            Vector3 worldCoord = x.GetWorldCoords(p);
            Point q = x.GetGridCoords(worldCoord);
            Assert.AreEqual<Point>(p, q);
        }
    }
}
