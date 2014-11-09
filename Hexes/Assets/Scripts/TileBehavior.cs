using UnityEngine;
using System.Collections;
namespace Assets.Scripts
{
    public class TileBehavior : MonoBehaviour
    {
        public Tile tile;
        //After attaching this script to hex tile prefab don't forget to initialize 
        //following materials with the ones we created earlier
        public Material OpaqueMaterial;
        public Material defaultMaterial;
        public Material originMaterial;
        public Material destMaterial;
        public Material pathMaterial;
        public Material mouseOverMaterial;

        private Material previousMaterial;

        void Start()
        {
            previousMaterial = defaultMaterial;

        }

        public void ResetMaterial()
        {
            this.renderer.material = previousMaterial;
        }

        //IMPORTANT: for methods like OnMouseEnter, OnMouseExit and so on to work, 
        //collider (Component -> Physics -> Mesh Collider) should be attached to the prefab
        void OnMouseEnter()
        {
            GridManager.instance.selectedTile = tile;
            //when mouse is over some tile, the tile is passable and the current tile 
            //is neither destination nor origin tile, change color to orange
            //if (tile.Passable && 
            //    (GridManager.instance.destTileTB == null ||
            //    this != GridManager.instance.destTileTB) &&
            //    (GridManager.instance.originTileTB != null || 
            //    this != GridManager.instance.originTileTB))
            //{
                this.previousMaterial = this.renderer.material;
                this.renderer.material = mouseOverMaterial;
            //}
        }

        //changes back to fully transparent material when mouse cursor is no longer hovering over the tile
        void OnMouseExit()
        {
            GridManager.instance.selectedTile = null;
            if(this.renderer.material != destMaterial &&
                this.renderer.material != originMaterial)
            {
                ResetMaterial();
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
                    GridManager.instance.destTileTB.renderer.material = defaultMaterial;                    
                }
                //set this tile as dest
                GridManager.instance.destTileTB = this;                
                GridManager.instance.destTileTB.renderer.material = destMaterial;
                this.previousMaterial = this.renderer.material;
                GridManager.instance.generateAndShowPath();
            }
            //if user left-clicks the tile set to origin
            if (Input.GetMouseButtonUp(0))
            {
                if (!tile.Passable) return;
                if(GridManager.instance.originTileTB != null &&
                    GridManager.instance.originTileTB != this)
                {
                    GridManager.instance.originTileTB.renderer.material = defaultMaterial; 
                }
                //set this tile as origin
                GridManager.instance.originTileTB = this;                
                GridManager.instance.originTileTB.renderer.material = originMaterial;
                this.previousMaterial = this.renderer.material;
                GridManager.instance.generateAndShowPath();
            }
        }
    }
}