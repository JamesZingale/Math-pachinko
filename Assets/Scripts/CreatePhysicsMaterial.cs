using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
public class CreatePhysicsMaterial : MonoBehaviour
{
    [MenuItem("Assets/Create/Physics Materials/Bouncy Ball Material")]
    public static void CreateBouncyBallMaterial()
    {
        // Create a physics material for the balls
        PhysicsMaterial2D bouncyMaterial = new PhysicsMaterial2D("BouncyBall");
        bouncyMaterial.bounciness = 0.8f;
        bouncyMaterial.friction = 0.2f;
        
        // Create the directory if it doesn't exist
        if (!System.IO.Directory.Exists("Assets/Materials"))
        {
            System.IO.Directory.CreateDirectory("Assets/Materials");
        }
        
        // Save the material as an asset
        AssetDatabase.CreateAsset(bouncyMaterial, "Assets/Materials/BouncyBall.physicsMaterial2D");
        AssetDatabase.SaveAssets();
        
        // Show the material in the project window
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = bouncyMaterial;
        
        Debug.Log("Created bouncy ball physics material at Assets/Materials/BouncyBall.physicsMaterial2D");
    }
    
    [MenuItem("Assets/Create/Physics Materials/Wall Material")]
    public static void CreateWallMaterial()
    {
        // Create a physics material for walls
        PhysicsMaterial2D wallMaterial = new PhysicsMaterial2D("Wall");
        wallMaterial.bounciness = 0.6f;
        wallMaterial.friction = 0.0f;
        
        // Create the directory if it doesn't exist
        if (!System.IO.Directory.Exists("Assets/Materials"))
        {
            System.IO.Directory.CreateDirectory("Assets/Materials");
        }
        
        // Save the material as an asset
        AssetDatabase.CreateAsset(wallMaterial, "Assets/Materials/Wall.physicsMaterial2D");
        AssetDatabase.SaveAssets();
        
        // Show the material in the project window
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = wallMaterial;
        
        Debug.Log("Created wall physics material at Assets/Materials/Wall.physicsMaterial2D");
    }
}
#endif
