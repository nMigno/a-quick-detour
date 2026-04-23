using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.IO;

public class GreyBoxSetup
{
    [MenuItem("Tools/Setup Grey Boxes")]
    static void SetupGreyBoxes()
    {
        var sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Square.png");
        if (sprite == null)
        {
            Debug.LogError("No se encontró Assets/Sprites/Square.png");
            return;
        }

        foreach (var sr in Object.FindObjectsByType<SpriteRenderer>(FindObjectsSortMode.None))
        {
            sr.sprite = sprite;
            sr.color = new Color(0.6f, 0.6f, 0.6f);
            EditorUtility.SetDirty(sr);
        }

        AssetDatabase.SaveAssets();
        EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
        Debug.Log("Grey boxes configurados.");
    }
}
