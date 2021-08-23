using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using elZach.GraphScripting;
using UnityEngine;

public class RotateTransform : Node
{
    //[Binding(typeof(Transform))]
    // public string transformReference;
    // public Transform transform => director.bindings.Find(x => x.name == transformReference)?.data as Transform;

    [Binding]
    public Transform transform;
    public Vector3 toRotate;

    public override Color GetColor() => new Color(0.6f, 0.2f, 0.473f);

    protected override State OnUpdate()
    {
        transform.Rotate(toRotate);
        return State.Success;
    }
}
