using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[CreateAssetMenu(fileName = "Track")]
public class Map : ScriptableObject
{
    public Sprite image;
    public string sceneName;
    public Material skybox;
    public PostProcessProfile profile;
}
