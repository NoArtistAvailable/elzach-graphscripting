using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using elZach.GraphScripting;
using UnityEngine;

public class RotateTransform : Node
{
    //[Binding(typeof(Transform))]
    public string transformReference;
    public Transform transform => director.test.Find(x => x.name == transformReference)?.data as Transform;
    public Vector3 toRotate;

    public override Color GetColor() => new Color(0.6f, 0.2f, 0.473f);

    protected override State OnUpdate()
    {
        Transform nono = null;
        nono.Rotate(toRotate);
        transform.Rotate(toRotate);
        return State.Success;
    }
}
