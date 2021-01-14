using UnityEngine;
using UnityEditor;

[CreateAssetMenu]
public class Item : ScriptableObject {
   
    [SerializeField] string id;
    public string ID {get {return id;}}
    public string ItemName;
    public Sprite Icon;
    [Range(1, 999)]
    public int MaximumStacks;
    private int CurrentStacks;

    [TextArea(15, 20)]
    public string ItemDescription; 
    
    private void Start() {
        CurrentStacks = MaximumStacks;
    }
    
    private void OnValidate() {
        //string path = AssetDatabase.GetAssetPath(this);
        //id = AssetDatabase.AssetPathToGUID(path);
        CurrentStacks = MaximumStacks;
    }

    public virtual Item GetCopy() {return this;}    

    public virtual void Destroy() {}              
    
}

