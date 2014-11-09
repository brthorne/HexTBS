using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    public class GridManager : MonoBehaviour
    {
        public Dictionary<Point, Tile> Board;

        //selectedTile stores the tile mouse cursor is hovering on
        public Tile selectedTile = null;
        //TB of the tile which is the start of the path
        public TileBehavior originTileTB = null;
        //TB of the tile which is the end of the path
        public TileBehavior destTileTB = null;

        public static GridManager instance = null;

        //following public variable is used to store the hex model prefab;
        //instantiate it by dragging the prefab on this variable using unity editor
        public GameObject Hex;
        //next two variables can also be instantiated using unity editor
        public int gridWidthInHexes = 10;
        public int gridHeightInHexes = 10;

        //Line should be initialised to some 3d object that can fit nicely in the center of a 
        //hex tile and will be used to indicate the path. For example, it can be just a simple small 
        //sphere with some material attached to it. Initialise the variable using inspector pane.
        public GameObject PathMarker;
        //List to hold "Lines" indicating the path
        List<GameObject> pathMarkers;

        public GameObject Wizard;

        //Hexagon tile width and height in game world
        private float hexWidth;
        private float hexHeight;


        void Awake()
        {
            instance = this;
        }

        void createGrid()
        {            
            GameObject hexGridGO = new GameObject("HexGrid");
            Board = new Dictionary<Point, Tile>();

            for (float y = 0; y < gridHeightInHexes; y++)
            {
                float sizeX = gridWidthInHexes;
                //if the offset row sticks up, reduce the number of hexes in a row
                //not capping ground size 
                //if (y % 2 != 0 && (gridSize.x + 0.5) * hexWidth > groundWidth)
                //    sizeX--;

                for (float x = 0; x < sizeX; x++)
                {
                    GameObject hex = (GameObject)Instantiate(Hex);
                    Vector2 gridPos = new Vector2(x, y);
                    hex.transform.position = calcWorldCoord(gridPos);
                    hex.transform.parent = hexGridGO.transform;
                    //TileBehabiour object is retrieved
                    var tb = hex.GetComponent<TileBehavior>();
                    //y / 2 is subtracted from x because we are using straight axis coordinate system
                    tb.tile = new Tile((int)x - (int)(y / 2), (int)y);
                    Board.Add(new Point((int)gridPos.x, (int)gridPos.y), tb.tile);
                }
            }
            //build adjacency lists
            foreach (Tile t in Board.Values)
            {
                t.FindNeighbours(Board, new Vector2(gridWidthInHexes, gridHeightInHexes),true);
            }
        }

        private void DrawPath(IEnumerable<Tile> path)
        {
            if (this.pathMarkers == null)
                this.pathMarkers = new List<GameObject>();
            //Destroy game objects which used to indicate the path
            this.pathMarkers.ForEach(Destroy);
            this.pathMarkers.Clear();

            //Lines game object is used to hold all the "Line" game objects indicating the path
            GameObject lines = GameObject.Find("Lines");
            if (lines == null)
                lines = new GameObject("Lines");
            foreach (Tile tile in path)
            {
                var marker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                //calcWorldCoord method uses squiggly axis coordinates so we add y / 2 to convert x 
                //coordinate from straight axis coordinate system
                Vector2 gridPos = new Vector2(tile.X + tile.Y / 2, tile.Y);
                marker.transform.position = calcWorldCoord(gridPos);
                this.pathMarkers.Add(marker);
                marker.transform.parent = lines.transform;
            }
        }

        public void generateAndShowPath()
        {
            //Don't do anything if origin or destination is not defined yet
            if (originTileTB == null || destTileTB == null)
            {
                DrawPath(new List<Tile>());
                return;
            }
            //We assume that the distance between any two adjacent tiles is 1
            //If you want to have some mountains, rivers, dirt roads or something else which 
            //might slow down the player you should replace the function with something that suits 
            //better your needs
            Func<Tile, Tile, double> distance = (node1, node2) => 1;
            Func<Tile, Tile, double> estimate = (node1, node2) => 
                Mathf.Sqrt(Mathf.Pow(node2.Location.X - node1.Location.X, 2) + 
                Mathf.Pow(node2.Location.Y - node1.Location.Y, 2));
            var path = PathFinder.FindPath(originTileTB.tile, destTileTB.tile,
                distance, estimate);
            DrawPath(path);
        }

        //Method to initialise Hexagon width and height
        void setSizes()
        {
            //renderer component attached to the Hex prefab is used to get the current width and height
            hexWidth = Hex.renderer.bounds.size.x;
            hexHeight = Hex.renderer.bounds.size.z;
        }

        //Method to calculate the position of the first hexagon tile
        //The center of the hex grid is (0,0,0)
        Vector3 calcInitPos()
        {
            Vector3 initPos;
            //the initial position will be in the left upper corner
            initPos = new Vector3(-hexWidth * gridWidthInHexes / 2f + hexWidth / 2, 0,
                gridHeightInHexes / 2f * hexHeight - hexHeight / 2);

            return initPos;
        }

        //method used to convert hex grid coordinates to game world coordinates
        public Vector3 calcWorldCoord(Vector2 gridPos)
        {
            //Position of the first hex tile
            Vector3 initPos = calcInitPos();
            //Every second row is offset by half of the tile width
            float offset = 0;
            if (gridPos.y % 2 != 0)
                offset = hexWidth / 2;

            float x = initPos.x + offset + gridPos.x * hexWidth;
            //Every new line is offset in z direction by 3/4 of the hexagon height
            float z = initPos.z - gridPos.y * hexHeight * 0.75f;
            return new Vector3(x, 0, z);
        }

        //The grid should be generated on game start
        void Start()
        {
            setSizes();
            createGrid();
            createPlayers();
        }

        private void createPlayers()
        {
            Instantiate(Wizard, calcWorldCoord(new Vector2(0, 0)), Quaternion.identity);   
        }
    }
}