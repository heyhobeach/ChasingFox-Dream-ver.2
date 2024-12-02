using UnityEngine;


namespace Collection
{
    [CreateAssetMenu(fileName = "CollectionScriptorble", menuName = "CollectionScriptorble/CollectionScriptorble")]
    public class CollectionScriptorble : ScriptableObject
    {
        public string _name;
        [TextArea(3,8)]
        public string _context;
    }

}
