﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    public class GridManager : MonoBehaviour
    {
        [HideInInspector]
        public Dictionary<Point, Tile> Board;
        public Dictionary<Point, PlayerBehavior> Players;

        //selectedTile stores the tile mouse cursor is hovering on
        [HideInInspector]
        public TileBehavior mouseOverTile = null;
        //TB of the tile which is the start of the path
        [HideInInspector]
        public TileBehavior originTileTB = null;
        //TB of the tile which is the end of the path
        [HideInInspector]
        public TileBehavior destTileTB = null;

        public static GridManager instance = null;
        [HideInInspector]
        public ITransformer Transformer = null;

        public GameObject PathMarker;
        List<GameObject> pathMarkers;

        //following public variable is used to store the hex model prefab;
        //instantiate it by dragging the prefab on this variable using unity editor
        public GameObject Hex;
        //next two variables can also be instantiated using unity editor
        public int gridWidthInHexes = 10;
        public int gridHeightInHexes = 10;

        public GameObject Wizard;

        Path<Tile> mouseoverPath;

        Func<Tile, Tile, int> distance = (node1, node2) => 1;
        Func<Tile, Tile, double> estimate = (node1, node2) =>
            Mathf.Sqrt(Mathf.Pow(node2.Location.X - node1.Location.X, 2) +
            Mathf.Pow(node2.Location.Y - node1.Location.Y, 2)) - 1;

        void Awake()
        {
            instance = this;
        }

        //The grid should be generated on game start
        void Start()
        {
            Transformer = new Transformer(Hex.renderer.bounds.size.x, Hex.renderer.bounds.size.z);
            createGrid();
            createPlayers();
            
        }

        void createGrid()
        {            
            GameObject hexGridGO = new GameObject("HexGrid");
            Board = new Dictionary<Point, Tile>();

            for (int y = 0; y < gridHeightInHexes; y++)
            {
                for (int x = 0; x < gridWidthInHexes; x++)
                {
                    GameObject hex = (GameObject)Instantiate(Hex);
                    var gridPos = new Point(x, y);
                    hex.transform.position = Transformer.GetWorldCoords(gridPos);
                    hex.transform.parent = hexGridGO.transform;
                    var tb = hex.GetComponent<TileBehavior>();
                    tb.tile = new Tile(x, y);
                    Board.Add(gridPos, tb.tile);
                }
            }
            //build adjacency lists
            foreach (Tile t in Board.Values)
            {
                t.FindNeighbours(Board, new Vector2(gridWidthInHexes, gridHeightInHexes),true);
            }
        }

        private void highlightPath(Path<Tile> path)
        {
            foreach (Tile t in path)
            {
                TileBehavior tb = GameObject.FindObjectsOfType<TileBehavior>().Where(n =>
                    n.tile.Location.X == t.X &&
                    n.tile.Location.Y == t.Y).First<TileBehavior>();
                tb.SetPathColor();
            }
        }

        private void createPlayers()
        {
            Players = new Dictionary<Point,PlayerBehavior>();
            var startLocation = new Point(4, 4);
            var wizard = (GameObject)Instantiate(Wizard, Transformer.GetWorldCoords(startLocation), Quaternion.identity);
            PlayerBehavior pb = wizard.GetComponent<PlayerBehavior>();
            pb.Speed = 3;
            Players.Add(startLocation,pb);
        }

        public void HighlightMovableArea(Tile startTile)
        {
            PlayerBehavior player = Players[startTile.Location];
            if (player == null) return;
            foreach(KeyValuePair<Point,int> pair in PathFinder.GetMovableArea<Tile>(startTile,(node1, node2) => 1,player.Speed,Board))
            {
                TileBehavior tb = GameObject.FindObjectsOfType<TileBehavior>().Where(n => 
                            n.tile.Location.X == pair.Key.X && 
                            n.tile.Location.Y == pair.Key.Y).First<TileBehavior>();
                        //use tb to set the tile coloring
                        tb.SetReachableColor();
            }            
        }

        public void HighlightMouseoverPath(bool go)
        {
            if (go)
            {
                if (originTileTB != null)
                {
                    mouseoverPath = PathFinder.FindPath(originTileTB.tile, mouseOverTile.tile,
                        distance, estimate); 
                    highlightPath(mouseoverPath);
                }
            }
            else if(mouseoverPath != null)
            {
                foreach (Tile t in mouseoverPath)
                {
                    TileBehavior tb = GameObject.FindObjectsOfType<TileBehavior>().Where(n =>
                        n.tile.Location.X == t.X &&
                        n.tile.Location.Y == t.Y).First<TileBehavior>();
                    tb.PopMaterial();
                }
                mouseoverPath = null;
            }
        }

        public void CreateMoves()
        {
            ActionManager.instance.GenerateMoves(mouseoverPath);
        }

        private void UnSetOriginTile()
        {
            originTileTB.PopMaterial();
            originTileTB = null;
        }

        /// <summary>
        /// magic hand that moves from start to dest without respect to rules.  
        /// </summary>
        /// <param name="start"></param>
        /// <param name="dest"></param>
        public void Move(Point start, Point dest)
        {                      
            PlayerBehavior pb = Players[start];
            pb.transform.position = Transformer.GetWorldCoords(dest);
            Players.Add(dest, pb);
            Players.Remove(start);
        }
    }
}