using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class PhysicsSetupTool : EditorWindow
{
    private bool setupNumberBalls = true;
    private bool setupOperatorBalls = true;
    private bool setupBoundaries = true;
    private bool setupPlayerBall = true;
    
    private Color numberBallColor = new Color(0.2f, 0.6f, 1f);
    private Color operatorBallColor = new Color(1f, 0.5f, 0.2f);
    private Color boundaryColor = new Color(0.3f, 0.3f, 0.3f);
    private Color playerBallColor = new Color(1f, 1f, 1f);
    
    [MenuItem("Math Pinball/Physics Setup Tool")]
    public static void ShowWindow()
    {
        GetWindow<PhysicsSetupTool>("Physics Setup Tool");
    }
    
    private void OnGUI()
    {
        GUILayout.Label("Math Pinball Physics Setup Tool", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        
        GUILayout.Label("Select which objects to set up:", EditorStyles.boldLabel);
        setupNumberBalls = EditorGUILayout.Toggle("Number Balls", setupNumberBalls);
        setupOperatorBalls = EditorGUILayout.Toggle("Operator Balls", setupOperatorBalls);
        setupBoundaries = EditorGUILayout.Toggle("Boundaries", setupBoundaries);
        setupPlayerBall = EditorGUILayout.Toggle("Player Ball", setupPlayerBall);
        
        EditorGUILayout.Space();
        GUILayout.Label("Colors:", EditorStyles.boldLabel);
        numberBallColor = EditorGUILayout.ColorField("Number Ball Color", numberBallColor);
        operatorBallColor = EditorGUILayout.ColorField("Operator Ball Color", operatorBallColor);
        boundaryColor = EditorGUILayout.ColorField("Boundary Color", boundaryColor);
        playerBallColor = EditorGUILayout.ColorField("Player Ball Color", playerBallColor);
        
        EditorGUILayout.Space();
        if (GUILayout.Button("Set Up Physics Components"))
        {
            SetupPhysicsComponents();
        }
        
        EditorGUILayout.Space();
        if (GUILayout.Button("Create Physics Materials"))
        {
            CreatePhysicsMaterials.CreateAllPhysicsMaterials();
        }
    }
    
    private void SetupPhysicsComponents()
    {
        int setupCount = 0;
        
        // Find all GameObjects in the scene
        GameObject[] allObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        
        // Load physics materials
        PhysicsMaterial bouncyMaterial = AssetDatabase.LoadAssetAtPath<PhysicsMaterial>("Assets/Resources/BouncyBall.physicMaterial");
        PhysicsMaterial wallMaterial = AssetDatabase.LoadAssetAtPath<PhysicsMaterial>("Assets/Resources/Wall.physicMaterial");
        PhysicsMaterial mathBallMaterial = AssetDatabase.LoadAssetAtPath<PhysicsMaterial>("Assets/Resources/MathBall.physicMaterial");
        
        // Create materials if they don't exist
        if (bouncyMaterial == null || wallMaterial == null || mathBallMaterial == null)
        {
            CreatePhysicsMaterials.CreateAllPhysicsMaterials();
            bouncyMaterial = AssetDatabase.LoadAssetAtPath<PhysicsMaterial>("Assets/Resources/BouncyBall.physicMaterial");
            wallMaterial = AssetDatabase.LoadAssetAtPath<PhysicsMaterial>("Assets/Resources/Wall.physicMaterial");
            mathBallMaterial = AssetDatabase.LoadAssetAtPath<PhysicsMaterial>("Assets/Resources/MathBall.physicMaterial");
        }
        
        foreach (GameObject obj in allObjects)
        {
            // Setup Number Balls
            if (setupNumberBalls && obj.name.Contains("Number") || 
                (obj.GetComponent<MathBall>() != null && obj.GetComponent<MathBall>().ballType == MathBall.BallType.Number))
            {
                SetupMathBall(obj, MathBall.BallType.Number, mathBallMaterial, numberBallColor);
                setupCount++;
            }
            
            // Setup Operator Balls
            else if (setupOperatorBalls && obj.name.Contains("Operator") || 
                    (obj.GetComponent<MathBall>() != null && obj.GetComponent<MathBall>().ballType == MathBall.BallType.Operator))
            {
                SetupMathBall(obj, MathBall.BallType.Operator, mathBallMaterial, operatorBallColor);
                setupCount++;
            }
            
            // Setup Boundaries
            else if (setupBoundaries && (obj.name.Contains("Boundary") || obj.name.Contains("Wall") || obj.name.Contains("Obstacle")))
            {
                SetupBoundary(obj, wallMaterial, boundaryColor);
                setupCount++;
            }
            
            // Setup Player Ball
            else if (setupPlayerBall && (obj.name.Contains("Player") || obj.name.Contains("Ball") && obj.GetComponent<PlayerBall>() != null))
            {
                SetupPlayerBall(obj, bouncyMaterial, playerBallColor);
                setupCount++;
            }
        }
        
        Debug.Log($"Physics setup complete. Set up {setupCount} objects.");
    }
    
    // Public method that can be called from other scripts
    public static void SetupPhysicsComponentsExternal(bool setupNumberBalls, bool setupOperatorBalls, 
                                                     bool setupBoundaries, bool setupPlayerBall)
    {
        PhysicsSetupTool tool = CreateInstance<PhysicsSetupTool>();
        tool.setupNumberBalls = setupNumberBalls;
        tool.setupOperatorBalls = setupOperatorBalls;
        tool.setupBoundaries = setupBoundaries;
        tool.setupPlayerBall = setupPlayerBall;
        tool.SetupPhysicsComponents();
    }
    
    private void SetupMathBall(GameObject obj, MathBall.BallType type, PhysicsMaterial material, Color color)
    {
        // Add MathBall component if it doesn't exist
        MathBall mathBall = obj.GetComponent<MathBall>();
        if (mathBall == null)
        {
            mathBall = obj.AddComponent<MathBall>();
        }
        
        // Set ball type
        mathBall.ballType = type;
        
        // Set value based on name if not already set
        if (string.IsNullOrEmpty(mathBall.value))
        {
            if (type == MathBall.BallType.Number)
            {
                // Try to extract a number from the name
                string name = obj.name;
                foreach (char c in name)
                {
                    if (char.IsDigit(c))
                    {
                        mathBall.value = c.ToString();
                        break;
                    }
                }
                
                // Default to "0" if no number found
                if (string.IsNullOrEmpty(mathBall.value))
                {
                    mathBall.value = "0";
                }
            }
            else
            {
                // Try to extract an operator from the name
                string name = obj.name.ToLower();
                if (name.Contains("add") || name.Contains("plus") || name.Contains("+"))
                {
                    mathBall.value = "+";
                }
                else if (name.Contains("subtract") || name.Contains("minus") || name.Contains("-"))
                {
                    mathBall.value = "-";
                }
                else if (name.Contains("multiply") || name.Contains("times") || name.Contains("*"))
                {
                    mathBall.value = "*";
                }
                else if (name.Contains("divide") || name.Contains("div") || name.Contains("/"))
                {
                    mathBall.value = "/";
                }
                else
                {
                    // Default to "+" if no operator found
                    mathBall.value = "+";
                }
            }
        }
        
        // Set tag based on type
        obj.tag = (type == MathBall.BallType.Number) ? "NumberBall" : "OperatorBall";
        
        // Add or configure Rigidbody
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = obj.AddComponent<Rigidbody>();
        }
        
        rb.mass = 1.0f;
        rb.linearDamping = 0.5f;
        rb.angularDamping = 0.5f;
        rb.useGravity = true;
        rb.isKinematic = false; // Allow physics to move the ball
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        
        // Add or configure SphereCollider
        SphereCollider collider = obj.GetComponent<SphereCollider>();
        if (collider == null)
        {
            collider = obj.AddComponent<SphereCollider>();
        }
        
        collider.radius = 0.5f;
        collider.material = material;
        
        // Add or configure Renderer
        Renderer renderer = obj.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = color;
        }
        else
        {
            // If no renderer, try to add a basic mesh
            if (obj.GetComponent<MeshFilter>() == null)
            {
                MeshFilter meshFilter = obj.AddComponent<MeshFilter>();
                meshFilter.mesh = Resources.GetBuiltinResource<Mesh>("Sphere.fbx");
            }
            
            MeshRenderer meshRenderer = obj.AddComponent<MeshRenderer>();
            Material newMaterial = new Material(Shader.Find("Standard"));
            newMaterial.color = color;
            meshRenderer.material = newMaterial;
        }
    }
    
    private void SetupBoundary(GameObject obj, PhysicsMaterial material, Color color)
    {
        // Add BoundaryManager component if it doesn't exist
        BoundaryManager boundary = obj.GetComponent<BoundaryManager>();
        if (boundary == null)
        {
            boundary = obj.AddComponent<BoundaryManager>();
        }
        
        // Set tag
        obj.tag = "Boundary";
        
        // Add or configure BoxCollider
        BoxCollider collider = obj.GetComponent<BoxCollider>();
        if (collider == null)
        {
            collider = obj.AddComponent<BoxCollider>();
        }
        
        collider.material = material;
        
        // Add or configure Renderer
        Renderer renderer = obj.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = color;
        }
        else
        {
            // If no renderer, try to add a basic mesh
            if (obj.GetComponent<MeshFilter>() == null)
            {
                MeshFilter meshFilter = obj.AddComponent<MeshFilter>();
                meshFilter.mesh = Resources.GetBuiltinResource<Mesh>("Cube.fbx");
            }
            
            MeshRenderer meshRenderer = obj.AddComponent<MeshRenderer>();
            Material newMaterial = new Material(Shader.Find("Standard"));
            newMaterial.color = color;
            meshRenderer.material = newMaterial;
        }
    }
    
    private void SetupPlayerBall(GameObject obj, PhysicsMaterial material, Color color)
    {
        // Add PlayerBall component if it doesn't exist
        PlayerBall playerBall = obj.GetComponent<PlayerBall>();
        if (playerBall == null)
        {
            playerBall = obj.AddComponent<PlayerBall>();
        }
        
        // Set tag
        obj.tag = "Player";
        
        // Add or configure Rigidbody
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = obj.AddComponent<Rigidbody>();
        }
        
        rb.mass = 1.0f;
        rb.linearDamping = 0.1f;
        rb.angularDamping = 0.1f;
        rb.useGravity = true;
        rb.isKinematic = false;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        
        // Add or configure SphereCollider
        SphereCollider collider = obj.GetComponent<SphereCollider>();
        if (collider == null)
        {
            collider = obj.AddComponent<SphereCollider>();
        }
        
        collider.radius = 0.5f;
        collider.material = material;
        
        // Add or configure Renderer
        Renderer renderer = obj.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = color;
        }
        else
        {
            // If no renderer, try to add a basic mesh
            if (obj.GetComponent<MeshFilter>() == null)
            {
                MeshFilter meshFilter = obj.AddComponent<MeshFilter>();
                meshFilter.mesh = Resources.GetBuiltinResource<Mesh>("Sphere.fbx");
            }
            
            MeshRenderer meshRenderer = obj.AddComponent<MeshRenderer>();
            Material newMaterial = new Material(Shader.Find("Standard"));
            newMaterial.color = color;
            meshRenderer.material = newMaterial;
        }
        
        // Add TrailRenderer if it doesn't exist
        if (obj.GetComponent<TrailRenderer>() == null)
        {
            TrailRenderer trail = obj.AddComponent<TrailRenderer>();
            trail.time = 0.3f;
            trail.startWidth = 0.1f;
            trail.endWidth = 0.0f;
            trail.material = new Material(Shader.Find("Sprites/Default"));
            
            // Set up a simple gradient
            Gradient gradient = new Gradient();
            gradient.SetKeys(
                new GradientColorKey[] { new GradientColorKey(color, 0.0f), new GradientColorKey(color, 1.0f) },
                new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(0.0f, 1.0f) }
            );
            
            trail.colorGradient = gradient;
        }
    }
}
