using UnityEngine;
using UnityEngine.UI;


namespace Collection
{
    [CreateAssetMenu(fileName = "CollectionScriptorble", menuName = "CollectionScriptorble/CollectionScriptorble")]
    public class CollectionScriptorble : ScriptableObject
    {
        public bool is_collect = true;
        public int id;
        public Sprite image;
        public string _name;
        [TextArea(3,8)]
        public string _context;
    }

}
