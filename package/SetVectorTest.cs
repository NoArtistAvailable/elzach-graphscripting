using UnityEngine;
using elZach.GraphScripting;

public class SetVectorTest : Decorator
{
    [Binding] public Referenced<Vector3> vector; //sample how to do a binded property    
    //public override Color GetColor() => new Color(0.3f,0.5f,0.4f); //use for custom node color
    
    //gets evaluated once everytime node gets entered
    protected override void OnStart()
    {
        
    }
    
    //gets evaluated everytime -> return State.Running to stay in node
    protected override State OnUpdate()
    {
        Debug.Log($"Vector value is {vector.Value}.");
        vector.Value += Vector3.up;
        return Child.Evaluate();
    }
}
