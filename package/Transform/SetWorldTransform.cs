using System;
using UnityEngine;

namespace elZach.GraphScripting
{
    public class SetWorldTransform : Node
    {
        [Flags]
        public enum TransformProperties
        {
            Position = 1 << 1,
            Rotation = 1 << 2,
            Scale = 1 << 3
        }
        [Binding]
        public Transform target;
        public TransformProperties flags;
        [AssignPort] public Vector3 worldPosition;
        [AssignPort] public Quaternion worldRotation;
        [AssignPort] public Vector3 worldScale;
        
        protected override State OnUpdate()
        {
            if (flags.HasFlag(TransformProperties.Position)) target.position = worldPosition;
            if (flags.HasFlag(TransformProperties.Rotation)) target.rotation = worldRotation;
            if (flags.HasFlag(TransformProperties.Scale)) target.localScale = target.parent ? target.parent.TransformDirection(worldScale) : target.localScale = worldScale;
            return State.Running;
        }
    }
}