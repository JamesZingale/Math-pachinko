using UnityEngine;
using UnityEditor;

public class CreatePhysicsMaterials : MonoBehaviour
{
    [MenuItem("Math Pinball/Create Physics Materials")]
    public static void CreateAllPhysicsMaterials()
    {
        CreateBouncyBallMaterial();
        CreateWallMaterial();
        CreateMathBallMaterial();
    }
    
    [MenuItem("Math Pinball/Physics Materials/Bouncy Ball")]
    public static void CreateBouncyBallMaterial()
    {
        // Create a directory for physics materials if it doesn't exist
        if (!AssetDatabase.IsValidFolder("Assets/Resources"))
        {
            AssetDatabase.CreateFolder("Assets", "Resources");
        }
        
        // Create a bouncy material for the player ball
        PhysicsMaterial bouncyMaterial = new PhysicsMaterial("BouncyBall");
        bouncyMaterial.bounciness = 0.8f;
        bouncyMaterial.dynamicFriction = 0.2f;
        bouncyMaterial.staticFriction = 0.2f;
        bouncyMaterial.frictionCombine = PhysicsMaterialCombine.Minimum;
        bouncyMaterial.bounceCombine = PhysicsMaterialCombine.Maximum;
        
        AssetDatabase.CreateAsset(bouncyMaterial, "Assets/Resources/BouncyBall.physicMaterial");
        Debug.Log("Created BouncyBall physics material at Assets/Resources/BouncyBall.physicMaterial");
    }
    
    [MenuItem("Math Pinball/Physics Materials/Wall")]
    public static void CreateWallMaterial()
    {
        // Create a directory for physics materials if it doesn't exist
        if (!AssetDatabase.IsValidFolder("Assets/Resources"))
        {
            AssetDatabase.CreateFolder("Assets", "Resources");
        }
        
        // Create a material for walls
        PhysicsMaterial wallMaterial = new PhysicsMaterial("Wall");
        wallMaterial.bounciness = 0.5f;
        wallMaterial.dynamicFriction = 0.0f;
        wallMaterial.staticFriction = 0.0f;
        wallMaterial.frictionCombine = PhysicsMaterialCombine.Minimum;
        wallMaterial.bounceCombine = PhysicsMaterialCombine.Average;
        
        AssetDatabase.CreateAsset(wallMaterial, "Assets/Resources/Wall.physicMaterial");
        Debug.Log("Created Wall physics material at Assets/Resources/Wall.physicMaterial");
    }
    
    [MenuItem("Math Pinball/Physics Materials/Math Ball")]
    public static void CreateMathBallMaterial()
    {
        // Create a directory for physics materials if it doesn't exist
        if (!AssetDatabase.IsValidFolder("Assets/Resources"))
        {
            AssetDatabase.CreateFolder("Assets", "Resources");
        }
        
        // Create a material for math balls
        PhysicsMaterial mathBallMaterial = new PhysicsMaterial("MathBall");
        mathBallMaterial.bounciness = 0.6f;
        mathBallMaterial.dynamicFriction = 0.3f;
        mathBallMaterial.staticFriction = 0.3f;
        mathBallMaterial.frictionCombine = PhysicsMaterialCombine.Average;
        mathBallMaterial.bounceCombine = PhysicsMaterialCombine.Average;
        
        AssetDatabase.CreateAsset(mathBallMaterial, "Assets/Resources/MathBall.physicMaterial");
        Debug.Log("Created MathBall physics material at Assets/Resources/MathBall.physicMaterial");
    }
}
