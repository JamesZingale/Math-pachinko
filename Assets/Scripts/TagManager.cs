using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
public class TagManager : MonoBehaviour
{
    [MenuItem("Math Pinball/Add Required Tags")]
    public static void AddRequiredTags()
    {
        // Get the SerializedObject of the TagManager
        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        
        // Get the tags property
        SerializedProperty tagsProp = tagManager.FindProperty("tags");
        
        // Check if MusicPlayer tag already exists
        bool hasMusicPlayerTag = false;
        for (int i = 0; i < tagsProp.arraySize; i++)
        {
            SerializedProperty t = tagsProp.GetArrayElementAtIndex(i);
            if (t.stringValue == "MusicPlayer")
            {
                hasMusicPlayerTag = true;
                break;
            }
        }
        
        // Add MusicPlayer tag if it doesn't exist
        if (!hasMusicPlayerTag)
        {
            tagsProp.arraySize++;
            SerializedProperty newTag = tagsProp.GetArrayElementAtIndex(tagsProp.arraySize - 1);
            newTag.stringValue = "MusicPlayer";
        }
        
        // Apply changes
        tagManager.ApplyModifiedProperties();
        
        Debug.Log("Required tags have been added to the project.");
    }
}
#endif
