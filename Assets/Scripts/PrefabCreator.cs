using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class PrefabCreator : MonoBehaviour
{
    [Header("Ball Settings")]
    public float ballRadius = 0.5f;
    public float ballMass = 1f;
    public float ballBounciness = 0.7f;
    public PhysicsMaterial ballPhysicsMaterial;
    
    [Header("Number Balls")]
    public Color numberBallColor = new Color(0.2f, 0.6f, 1f);
    public int minNumber = 0;
    public int maxNumber = 9;
    
    [Header("Operator Balls")]
    public Color addColor = new Color(1f, 0.5f, 0.5f);
    public Color subtractColor = new Color(0.5f, 1f, 0.5f);
    public Color multiplyColor = new Color(1f, 1f, 0.5f);
    public Color divideColor = new Color(0.5f, 0.5f, 1f);
    
    [Header("Boundary Settings")]
    public Vector3 wallSize = new Vector3(10f, 1f, 1f);
    public Color boundaryColor = new Color(0.3f, 0.3f, 0.3f);
    
    [Header("Output Settings")]
    public string prefabOutputPath = "Assets/Prefabs";
    
    #if UNITY_EDITOR
    [ContextMenu("Create All Prefabs")]
    public void CreateAllPrefabs()
    {
        CreateNumberBalls();
        CreateOperatorBalls();
        CreateBoundaries();
        CreateLauncher();
        Debug.Log("All prefabs created successfully!");
    }
    
    [ContextMenu("Create Number Balls")]
    public void CreateNumberBalls()
    {
        // Create directory if it doesn't exist
        string numberPath = Path.Combine(prefabOutputPath, "Numbers");
        if (!Directory.Exists(numberPath))
        {
            Directory.CreateDirectory(numberPath);
        }
        
        // Create number balls (0-9)
        for (int i = minNumber; i <= maxNumber; i++)
        {
            string ballName = "Number_" + i;
            GameObject ballObject = CreateBasicBall(ballName, numberBallColor);
            
            // Add MathBall component and set properties
            MathBall mathBall = ballObject.GetComponent<MathBall>();
            if (mathBall == null)
            {
                mathBall = ballObject.AddComponent<MathBall>();
            }
            mathBall.ballType = MathBall.BallType.Number;
            mathBall.value = i.ToString();
            
            // Add TextMesh for displaying the number
            TextMesh textMesh = ballObject.GetComponentInChildren<TextMesh>();
            if (textMesh == null)
            {
                GameObject textObj = new GameObject("Text");
                textObj.transform.SetParent(ballObject.transform);
                textObj.transform.localPosition = new Vector3(0, 0, -0.51f); // Slightly in front of the ball
                textObj.transform.localRotation = Quaternion.Euler(0, 180, 0); // Face the camera
                textMesh = textObj.AddComponent<TextMesh>();
            }
            textMesh.text = i.ToString();
            textMesh.fontSize = 48;
            textMesh.alignment = TextAlignment.Center;
            textMesh.anchor = TextAnchor.MiddleCenter;
            textMesh.color = Color.black;
            
            // Save as prefab
            string prefabPath = Path.Combine(numberPath, ballName + ".prefab");
            SavePrefab(ballObject, prefabPath);
        }
        
        Debug.Log("Number ball prefabs created successfully!");
    }
    
    [ContextMenu("Create Operator Balls")]
    public void CreateOperatorBalls()
    {
        // Create directory if it doesn't exist
        string operatorPath = Path.Combine(prefabOutputPath, "Operators");
        if (!Directory.Exists(operatorPath))
        {
            Directory.CreateDirectory(operatorPath);
        }
        
        // Define operators and their colors
        Dictionary<string, Color> operators = new Dictionary<string, Color>
        {
            { "+", addColor },
            { "-", subtractColor },
            { "*", multiplyColor },
            { "/", divideColor }
        };
        
        // Create operator balls
        foreach (var op in operators)
        {
            string ballName = "Operator_" + (op.Key == "*" ? "multiply" : 
                              op.Key == "/" ? "divide" : 
                              op.Key == "+" ? "add" : "subtract");
            
            GameObject ballObject = CreateBasicBall(ballName, op.Value);
            
            // Add MathBall component and set properties
            MathBall mathBall = ballObject.GetComponent<MathBall>();
            if (mathBall == null)
            {
                mathBall = ballObject.AddComponent<MathBall>();
            }
            mathBall.ballType = MathBall.BallType.Operator;
            mathBall.value = op.Key;
            
            // Add TextMesh for displaying the operator
            TextMesh textMesh = ballObject.GetComponentInChildren<TextMesh>();
            if (textMesh == null)
            {
                GameObject textObj = new GameObject("Text");
                textObj.transform.SetParent(ballObject.transform);
                textObj.transform.localPosition = new Vector3(0, 0, -0.51f); // Slightly in front of the ball
                textObj.transform.localRotation = Quaternion.Euler(0, 180, 0); // Face the camera
                textMesh = textObj.AddComponent<TextMesh>();
            }
            textMesh.text = op.Key;
            textMesh.fontSize = 48;
            textMesh.alignment = TextAlignment.Center;
            textMesh.anchor = TextAnchor.MiddleCenter;
            textMesh.color = Color.black;
            
            // Save as prefab
            string prefabPath = Path.Combine(operatorPath, ballName + ".prefab");
            SavePrefab(ballObject, prefabPath);
        }
        
        Debug.Log("Operator ball prefabs created successfully!");
    }
    
    [ContextMenu("Create Boundaries")]
    public void CreateBoundaries()
    {
        // Create directory if it doesn't exist
        string boundaryPath = Path.Combine(prefabOutputPath, "Boundaries");
        if (!Directory.Exists(boundaryPath))
        {
            Directory.CreateDirectory(boundaryPath);
        }
        
        // Create standard wall
        GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wall.name = "Wall";
        wall.transform.localScale = wallSize;
        
        // Set material color
        Renderer wallRenderer = wall.GetComponent<Renderer>();
        if (wallRenderer != null)
        {
            Material wallMaterial = new Material(Shader.Find("Standard"));
            wallMaterial.color = boundaryColor;
            wallRenderer.material = wallMaterial;
            
            // Save the material as an asset
            string materialPath = Path.Combine(prefabOutputPath, "Materials");
            if (!Directory.Exists(materialPath))
            {
                Directory.CreateDirectory(materialPath);
            }
            AssetDatabase.CreateAsset(wallMaterial, Path.Combine(materialPath, "WallMaterial.mat"));
        }
        
        // Add boundary manager
        BoundaryManager boundaryManager = wall.AddComponent<BoundaryManager>();
        boundaryManager.bounceForce = 10f;
        boundaryManager.isDeadZone = false;
        
        // Save as prefab
        string wallPath = Path.Combine(boundaryPath, "Wall.prefab");
        SavePrefab(wall, wallPath);
        
        // Create dead zone
        GameObject deadZone = GameObject.CreatePrimitive(PrimitiveType.Cube);
        deadZone.name = "DeadZone";
        deadZone.transform.localScale = wallSize;
        
        // Set material color
        Renderer dzRenderer = deadZone.GetComponent<Renderer>();
        if (dzRenderer != null)
        {
            Material dzMaterial = new Material(Shader.Find("Standard"));
            dzMaterial.color = Color.red;
            dzRenderer.material = dzMaterial;
            
            // Save the material as an asset
            string materialPath = Path.Combine(prefabOutputPath, "Materials");
            if (!Directory.Exists(materialPath))
            {
                Directory.CreateDirectory(materialPath);
            }
            AssetDatabase.CreateAsset(dzMaterial, Path.Combine(materialPath, "DeadZoneMaterial.mat"));
        }
        
        // Add boundary manager
        BoundaryManager dzManager = deadZone.AddComponent<BoundaryManager>();
        dzManager.isDeadZone = true;
        
        // Save as prefab
        string dzPath = Path.Combine(boundaryPath, "DeadZone.prefab");
        SavePrefab(deadZone, dzPath);
        
        Debug.Log("Boundary prefabs created successfully!");
    }
    
    [ContextMenu("Create Launcher")]
    public void CreateLauncher()
    {
        // Create directory if it doesn't exist
        string launcherPath = Path.Combine(prefabOutputPath, "Gameplay");
        if (!Directory.Exists(launcherPath))
        {
            Directory.CreateDirectory(launcherPath);
        }
        
        // Create launcher
        GameObject launcher = GameObject.CreatePrimitive(PrimitiveType.Cube);
        launcher.name = "Launcher";
        launcher.transform.localScale = new Vector3(1f, 3f, 1f);
        
        // Set material color
        Renderer launcherRenderer = launcher.GetComponent<Renderer>();
        if (launcherRenderer != null)
        {
            Material launcherMaterial = new Material(Shader.Find("Standard"));
            launcherMaterial.color = new Color(0.8f, 0.8f, 0.8f);
            launcherRenderer.material = launcherMaterial;
            
            // Save the material as an asset
            string materialPath = Path.Combine(prefabOutputPath, "Materials");
            if (!Directory.Exists(materialPath))
            {
                Directory.CreateDirectory(materialPath);
            }
            AssetDatabase.CreateAsset(launcherMaterial, Path.Combine(materialPath, "LauncherMaterial.mat"));
        }
        
        // Add launcher script
        launcher.AddComponent<PinballLauncher>();
        
        // Create launch point
        GameObject launchPoint = new GameObject("LaunchPoint");
        launchPoint.transform.SetParent(launcher.transform);
        launchPoint.transform.localPosition = new Vector3(0, 1.5f, 0); // Top of the launcher
        
        // Set the launch point reference
        PinballLauncher launcherScript = launcher.GetComponent<PinballLauncher>();
        if (launcherScript != null)
        {
            launcherScript.launchPoint = launchPoint.transform;
        }
        
        // Create player ball prefab
        GameObject playerBall = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        playerBall.name = "PlayerBall";
        playerBall.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        
        // Add rigidbody
        Rigidbody rb = playerBall.AddComponent<Rigidbody>();
        rb.mass = 1f;
        rb.linearDamping = 0.5f;
        rb.angularDamping = 0.5f;
        rb.useGravity = true;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        
        // Add player ball script
        playerBall.AddComponent<PlayerBall>();
        
        // Set tag
        playerBall.tag = "Player";
        
        // Save player ball as prefab
        string playerBallPath = Path.Combine(launcherPath, "PlayerBall.prefab");
        SavePrefab(playerBall, playerBallPath);
        
        // Set the player ball reference in the launcher
        if (launcherScript != null)
        {
            // We need to load the prefab we just saved
            GameObject playerBallPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(playerBallPath);
            launcherScript.ballPrefab = playerBallPrefab;
        }
        
        // Save launcher as prefab
        string launcherPrefabPath = Path.Combine(launcherPath, "Launcher.prefab");
        SavePrefab(launcher, launcherPrefabPath);
        
        Debug.Log("Launcher and player ball prefabs created successfully!");
    }
    #endif
    
    private GameObject CreateBasicBall(string name, Color color)
    {
        // Create a sphere primitive
        GameObject ball = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        ball.name = name;
        ball.transform.localScale = new Vector3(ballRadius * 2, ballRadius * 2, ballRadius * 2);
        
        // Set material color
        Renderer renderer = ball.GetComponent<Renderer>();
        if (renderer != null)
        {
            Material ballMaterial = new Material(Shader.Find("Standard"));
            ballMaterial.color = color;
            renderer.material = ballMaterial;
            
            #if UNITY_EDITOR
            // Save the material as an asset
            string materialPath = Path.Combine(prefabOutputPath, "Materials");
            if (!Directory.Exists(materialPath))
            {
                Directory.CreateDirectory(materialPath);
            }
            AssetDatabase.CreateAsset(ballMaterial, Path.Combine(materialPath, name + "Material.mat"));
            #endif
        }
        
        // Add rigidbody
        Rigidbody rb = ball.AddComponent<Rigidbody>();
        rb.mass = ballMass;
        rb.linearDamping = 0.5f;
        rb.angularDamping = 0.5f;
        rb.useGravity = true;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        
        // Set physics material if available
        Collider collider = ball.GetComponent<Collider>();
        if (collider != null)
        {
            if (ballPhysicsMaterial != null)
            {
                collider.material = ballPhysicsMaterial;
            }
            else
            {
                #if UNITY_EDITOR
                // Create a default physics material
                PhysicsMaterial physicsMaterial = new PhysicsMaterial(name + "Physics");
                physicsMaterial.bounciness = ballBounciness;
                physicsMaterial.dynamicFriction = 0.4f;
                physicsMaterial.staticFriction = 0.4f;
                collider.material = physicsMaterial;
                
                // Save the physics material as an asset
                string materialPath = Path.Combine(prefabOutputPath, "Materials");
                if (!Directory.Exists(materialPath))
                {
                    Directory.CreateDirectory(materialPath);
                }
                AssetDatabase.CreateAsset(physicsMaterial, Path.Combine(materialPath, name + "Physics.physicMaterial"));
                #endif
            }
        }
        
        // Add tag
        ball.tag = "MathBall";
        
        return ball;
    }
    
    #if UNITY_EDITOR
    private void SavePrefab(GameObject obj, string path)
    {
        // Create the prefab
        #if UNITY_2018_3_OR_NEWER
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(obj, path);
        #else
        GameObject prefab = PrefabUtility.CreatePrefab(path, obj);
        #endif
        
        // Destroy the temporary object
        DestroyImmediate(obj);
    }
    #endif
}
