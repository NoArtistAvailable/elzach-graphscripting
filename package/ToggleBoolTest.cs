namespace elZach.GraphScripting
{
    public class ToggleBoolTest : Node
    {
        public bool returnValue;

        [AssignPort(isInput = false)] public bool GetBoolToggled()
        {
            returnValue = !returnValue;
            return returnValue;
        }

        [AssignPort(isInput = false)]
        public bool AlwaysFalse() => false;
        
        [AssignPort(isInput = false)]
        public bool AlwaysTrue() => true;

        
        protected override State OnUpdate()
        {
            throw new System.NotImplementedException();
        }
    }
}