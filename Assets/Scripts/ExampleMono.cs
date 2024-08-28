using UnityEngine;
using Toolkit.Utility.Attributes;

public class ExampleMono : MonoBehaviour
{
    [SceneName] public string scene;

    [SerializeField, SetProperty("Target")]
    private float _target;

    public float Target
    {
        get => _target;
        set
        {
            _target = value;
            Debug.Log("set target");
        }
    }
    
    public enum MyEnum
    {
        Test0,
        Test1,
    }

    [SerializeField, SetProperty("SceneName"), SceneName]
    private string _sceneName;

    public bool isShowPro = false;

    [PropertyActive("isShowPro", PropertyActiveAttribute.CompareType.Equal, true)]
    public GameObject cube;

    public string SceneName
    {
        get => _sceneName;
        set
        {
            _sceneName = value;
            scene = value;
            Debug.Log("SceneName Changed");
        }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        Font.textureRebuilt += FontOntextureRebuilt;
    }

    private void FontOntextureRebuilt(Font obj)
    {
        Debug.Log(obj.name);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
