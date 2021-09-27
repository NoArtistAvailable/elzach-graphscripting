using System;
using System.Collections.Generic;
using UnityEngine;

namespace elZach.GraphScripting
{
    public class Root : Node
    {
        public Node Child;
        public override Color GetColor() => new Color(0.4f, 0.2f, 0.2f);

        internal override void GetParametersRecursive(ref List<TreeContainer.Parameter> parameters)
        {
            base.GetParametersRecursive(ref parameters);
            if(Child) Child.GetParametersRecursive(ref parameters);
        }
        protected override State OnUpdate()
        {
            // try
            // {
            return Child.Evaluate();
            // }
            // catch (Exception e)
            // {
            //     Console.WriteLine(e);
            //     throw;
            // }
            
        }

        public override Node Clone()
        {
            Root node = base.Clone() as Root;
            node.Child = Child?.Clone();
            return node;
        }
    }
}