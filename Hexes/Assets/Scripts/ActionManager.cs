using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    public class ActionManager : MonoBehaviour
    {
        public static ActionManager instance = null;

        private Queue<IAction> actions;

        public void GenerateMoves(Path<Tile> path)
        {
            //unwind the backward path enumeration
            List<Tile> tiles = new List<Tile>();
            foreach(Tile t in path)
            {
                tiles.Insert(0, t);
            }
            Tile prev = null;

            foreach(Tile t in tiles)
            {
                if(prev != null){
                    actions.Enqueue(new ActionMove(prev.Location, t.Location));
                }
                prev = t;
            }
        }

        void Awake()
        {
            instance = this;
        }

        void Start()
        {
            actions = new Queue<IAction>();
        }

        void Update()
        {
            if(actions.Count > 0)
            {
                actions.Dequeue().Execute();
            }
        }
    }
}
