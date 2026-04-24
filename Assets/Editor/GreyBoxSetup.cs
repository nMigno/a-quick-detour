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

    [MenuItem("Tools/Fix Player Friction")]
    static void FixPlayerFriction()
    {
        const string matPath = "Assets/Physics/NoFriction.physicsmaterial2d";
        if (!System.IO.Directory.Exists("Assets/Physics"))
            System.IO.Directory.CreateDirectory("Assets/Physics");

        var mat = AssetDatabase.LoadAssetAtPath<PhysicsMaterial2D>(matPath);
        if (mat == null)
        {
            mat = new PhysicsMaterial2D("NoFriction");
            mat.friction = 0f;
            mat.bounciness = 0f;
            AssetDatabase.CreateAsset(mat, matPath);
            AssetDatabase.SaveAssets();
        }

        var player = GameObject.Find("Player");
        if (player == null) { Debug.LogError("No se encontró Player"); return; }

        var col = player.GetComponent<BoxCollider2D>();
        if (col == null) { Debug.LogError("Player no tiene BoxCollider2D"); return; }

        col.sharedMaterial = mat;
        EditorUtility.SetDirty(col);
        EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
        Debug.Log("Fricción del player corregida.");
    }
}
