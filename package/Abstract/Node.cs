using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace elZach.GraphScripting
{
    public abstract class Node : ScriptableObject
    {
        public enum State{Running, Failure, Success}

        public string guid;
        [HideInInspector] public Vector2 position;
        [HideInInspector] public State state;
        [HideInInspector] public TreeContainer container;
        public TreeDirector director => container.director;
        public virtual Color GetColor() => new Color(0.25f,0.25f,0.25f);
        private bool started = false;
        public bool Started => started;
        

        public virtual List<TreeContainer.Parameter> GetPublicParameters()
        {
            return new List<TreeContainer.Parameter>();
        }
        
        public virtual void Init(TreeDirector director)
        {
            //this.director = director;
        }

        protected virtual void OnStart(){}
        protected virtual void OnStop(){}

        protected abstract State OnUpdate();
        
        public State Evaluate()
        {
            if (!started)
            {
                started = true;
                OnStart();
            }
            state = OnUpdate();
            if (state == State.Failure || state == State.Success)
            {
                started = false;
                OnStop();
            }
            return state;
        }

        public virtual Node Clone()
        {
            Node clone = Instantiate(this);
            clone.name = this.name;
            return clone;
        }
    }
}