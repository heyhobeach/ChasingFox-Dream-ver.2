using UnityEngine;


namespace Collection{
    [CreateAssetMenu(fileName = "NewsScriptorble", menuName = "CollectionScriptorble/NewsScriptorble")]
    public class NewsScriptorble : ScriptableObject
    {
        public int id;
        public int chapter_info;
        public Sprite image;
        public string image_name;
    }

}