using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Texture2DArrayTools))]
public class Texture2DArrayInceptor : UnityEditor.Editor
{
    private Texture2DArrayTools component = null;

    private void OnEnable()
    {
        if (component == null) { this.component = base.target as Texture2DArrayTools; }
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorGUILayout.Space(30);
        if (GUILayout.Button("Generate Texture Array")){ this.CreateTexture2DArray(); }
    }

    private void CreateTexture2DArray() {
        if (component == null || component.images.Count <= 0) return;
        Texture2DArray textures = new Texture2DArray(512, 1024, component.images.Count, TextureFormat.RGBA32, true, false); //1013x1443

        //¿½±´Í¼Æ¬½øÈëTexture2DArray
        //for (int i = 0; i < component.images.Count; i++) { Graphics.CopyTexture(component.images[i], 0, textures, i); }
        for (int i = 0; i < component.images.Count; i++) { textures.SetPixels(component.images[i].GetPixels(), i, 0); }

        textures.filterMode = FilterMode.Bilinear;
        textures.wrapMode = TextureWrapMode.Clamp;

        AssetDatabase.CreateAsset(textures, "Assets/art/textures/TexAttr.asset");
        AssetDatabase.SaveAssets();
    }
}
