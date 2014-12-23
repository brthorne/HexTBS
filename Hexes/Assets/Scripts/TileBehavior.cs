using UnityEngine;
using System.Collections;
namespace Assets.Scripts
{
    public class TileBehavior : MonoBehaviour
    {
        public Tile tile;
        [HideInInspector]
        public PlayerBehavior resident;
        //After attaching this script to hex tile prefab don't forget to initialize 
        //following materials with the ones we created earlier
        public Material OpaqueMaterial;
        public Material defaultMaterial;
        public Material originMaterial;
        public Material destMaterial;
        public Material pathMaterial;
        public Material mouseOverMaterial;

        private Stack previousMaterials;

        void Start()
        {
            previousMaterials = new Stack();
            previousMaterials.Push(defaultMaterial);
        }

        public void PopMaterial()
        {
            if (previousMaterials.Count > 0)
            {
                this.renderer.material = (Material)previousMaterials.Pop();
            }
        }

        public void PushMaterial()
        {
            previousMaterials.Push(this.renderer.material);
        }

        //IMPORTANT: for methods like OnMouseEnter, OnMouseExit and so on to work, 
        //collider (Component -> Physics -> Mesh Collider) should be attached to the prefab
        void OnMouseEnter()
        {
            GridManager.instance.selectedTile = tile;
            PushMaterial();
            this.renderer.material = mouseOverMaterial;
            Debug.Log(tile.ToString() + " <-> " + 
                    GridManager.instance.Transformer.GetWorldCoords(tile.Location).ToString() +
                    " <-> " + GridManager.instance.Transformer.GetGridCoords(GridManager.instance.Transformer.GetWorldCoords(tile.Location)).ToString());
        }

        //changes back to fully transparent material when mouse cursor is no longer hovering over the tile
        void OnMouseExit()
        {
            GridManager.instance.selectedTile = null;
            if(this.renderer.material != destMaterial &&
                this.renderer.material != originMaterial)
            {
                PopMaterial();
            }
        }

        //called every frame when mouse cursor is on this tile
        void OnMouseOver()
        {
            //if player right-clicks on the tile, set to destination
            if (Input.GetMouseButtonUp(1))
            {
                if (!tile.Passable) return;
                //if grid manager had an old destination, reset it to default
                if (GridManager.instance.destTileTB != null &&
                    GridManager.instance.destTileTB != this)
                {
                    GridManager.instance.destTileTB.PopMaterial();
                }
                //set this tile as dest
                GridManager.instance.destTileTB = this;
                PushMaterial();
                this.renderer.material = destMaterial;
                GridManager.instance.generateAndShowPath();
            }
            //if user left-clicks the tile set to origin
            if (Input.GetMouseButtonUp(0))
            {
                //if we find a player here then set to origin
                //then color all the paths available
                if (GridManager.instance.Players.ContainsKey(this.tile.Location))
                {
                    if (!tile.Passable) return;
                    if (GridManager.instance.originTileTB != null &&
                        GridManager.instance.originTileTB != this)
                    {
                        GridManager.instance.originTileTB.PopMaterial();
                    }
                    //set this tile as origin
                    GridManager.instance.originTileTB = this;
                    PushMaterial();
                    this.renderer.material = originMaterial;                    
                    GridManager.instance.HighlightMovableArea(tile);
                }

            }
        }

        public void SetReachableColor()
        {
            this.renderer.material = pathMaterial;
        }
    }
}