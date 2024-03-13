using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using ColorUtility = UnityEngine.ColorUtility;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;
public class GKUtils : MonoBehaviour
{
    private static GKUtils _instance;
    private static GKUtils instance{
        get{
            if(_instance == null){
                _instance = new GameObject("GKUtils").AddComponent<GKUtils>();
            }
            return _instance;
        }
    }
    public static void RunAfterSeconds(System.Action action, float delay){
        instance.LocalRunAfterSeconds(action, delay);
    }
    private void LocalRunAfterSeconds(System.Action action, float delay){
        if(delay == 0){
            action();
            return;
        }
        StartCoroutine(Invoke(action, delay));
    }
    public IEnumerator Invoke(System.Action action, float delay){
        yield return new WaitForSeconds(delay);
        action();
    }
    public static float NegativeSqrt(float value){
        return Mathf.Sign(value) * Mathf.Sqrt(Mathf.Abs(value));
    }
    //PlayClipAt
    public static AudioSource PlayClipAt(AudioClip clip, Vector3 position, float volume=1.0f, float spatialBlend=0.0f){
        var tempGO = new GameObject("TempAudio"); // create the temp object
        tempGO.transform.position = position; // set its position
        var aSource = tempGO.AddComponent<AudioSource>(); // add an audio source
        aSource.clip = clip; // define the clip
        aSource.spatialBlend = spatialBlend;
        aSource.volume = volume;
        // set other aSource properties here, if desired
        aSource.Play(); // start the sound
        Destroy(tempGO, clip.length); // destroy object after clip duration
        return aSource; // return the AudioSource reference
    }
    #region Layer mask utils

    public static bool IsInLayerMask(int layer, LayerMask layerMask){
        return layerMask == (layerMask | (1 << layer));
    }
    public static void AddToLayerMask(ref LayerMask layerMask, int layer){
        layerMask = layerMask | (1 << layer);
    }
    public static void RemoveFromLayerMask(ref LayerMask layerMask, int layer){
        layerMask = layerMask & ~(1 << layer);
    }
    public static bool LayerMaskIsEmpty(LayerMask layerMask){
        return layerMask == 0;
    }
    public static T CopyComponent<T>(T original, GameObject destination) where T : Component
    {
        System.Type type = original.GetType();
        Component copy = destination.AddComponent(type);
        System.Reflection.FieldInfo[] fields = type.GetFields();
        foreach (System.Reflection.FieldInfo field in fields)
        {
            field.SetValue(copy, field.GetValue(original));
        }
        return copy as T;
    }
    public static bool TryParseVector2(string s, out Vector2 result)
    {
        result = Vector2.zero;
        string[] parts = s.Trim('(', ')').Split(',');
        if (parts.Length != 2) return false;
        float x, y;
        if (!float.TryParse(parts[0], out x)) return false;
        if (!float.TryParse(parts[1], out y)) return false;
        result = new Vector2(x, y);
        return true;
    }

    public static bool TryParseVector3(string s, out Vector3 result)
    {
        result = Vector3.zero;
        string[] parts = s.Trim('(', ')').Split(',');
        if (parts.Length != 3) return false;
        float x, y, z;
        if (!float.TryParse(parts[0], out x)) return false;
        if (!float.TryParse(parts[1], out y)) return false;
        if (!float.TryParse(parts[2], out z)) return false;
        result = new Vector3(x, y, z);
        return true;
    }

    public static bool TryParseVector4(string s, out Vector4 result)
    {
        result = Vector4.zero;
        string[] parts = s.Trim('(', ')').Split(',');
        if (parts.Length != 4) return false;
        float x, y, z, w;
        if (!float.TryParse(parts[0], out x)) return false;
        if (!float.TryParse(parts[1], out y)) return false;
        if (!float.TryParse(parts[2], out z)) return false;
        if (!float.TryParse(parts[3], out w)) return false;
        result = new Vector4(x, y, z, w);
        return true;
    }

    public static bool TryParseQuaternion(string s, out Quaternion result)
    {
        result = Quaternion.identity;
        string[] parts = s.Trim('(', ')').Split(',');
        if (parts.Length != 4) return false;
        float x, y, z, w;
        if (!float.TryParse(parts[0], out x)) return false;
        if (!float.TryParse(parts[1], out y)) return false;
        if (!float.TryParse(parts[2], out z)) return false;
        if (!float.TryParse(parts[3], out w)) return false;
        result = new Quaternion(x, y, z, w);
        return true;
    }
    public static bool IsIgnoredProperty(string propertyName){
        switch(propertyName){
            case "m_Script":
            case "m_FileID":
            case "m_ObjectHideFlags":
            case "m_PathID":
            case "m_Name":
            case "m_EditorHideFlags":
            case "m_EditorClassIdentifier":
            case "syncDirection":
            case "syncMode":
            case "syncInterval":
                return true;
            default:
                return false;
        }
    }
    #if UNITY_EDITOR
    public static List<SerializedProperty> GetSerializedProperties(SerializedObject serializedObject, Component component){
        //Serialized object
        SerializedProperty propertyIterator = serializedObject.GetIterator();
        List<SerializedProperty> properties = new List<SerializedProperty>();
        while (propertyIterator.Next(true))
        {
            // Debug.Log("found property: " + propertyIterator.name);
            if(IsIgnoredProperty(propertyIterator.name)) continue;
            properties.Add(propertyIterator.Copy());
        }
        return properties;
    }
    public static string GetSerializedPropertyValueString(SerializedProperty property){
        SerializedPropertyType type = property.propertyType;
        switch (type)
        {
            case SerializedPropertyType.Integer:
                return property.intValue.ToString();
            case SerializedPropertyType.Boolean:
                return property.boolValue.ToString();
            case SerializedPropertyType.Float:
                return property.floatValue.ToString();
            case SerializedPropertyType.String:
                return property.stringValue;
            case SerializedPropertyType.Color:
                return ColorUtility.ToHtmlStringRGBA(property.colorValue);
            case SerializedPropertyType.LayerMask:
                return property.intValue.ToString();
            case SerializedPropertyType.Enum:
                return property.enumValueIndex.ToString();
            case SerializedPropertyType.Vector2:
                return property.vector2Value.ToString();
            case SerializedPropertyType.Vector3:
                return property.vector3Value.ToString();
            case SerializedPropertyType.Vector4:
                return property.vector4Value.ToString();
            case SerializedPropertyType.Character:
                return property.intValue.ToString();
            case SerializedPropertyType.Quaternion:
                return property.quaternionValue.ToString();
            default:
                return null;
        }
    }
    public static bool SetSerializedPropertyValueString(SerializedProperty property, string value){
        // Debug.Log("GetSerializedPropertyValueString(property): " + GetSerializedPropertyValueString(property) + " value: " + value + " property: " + property.name + " type: " + property.propertyType.ToString());
        // if (value == GetSerializedPropertyValueString(property)) return false;
        SerializedPropertyType type = property.propertyType;
        
        // Debug.Log("SetSerializedPropertyValueString: " +type.ToString()+" "+ property.name + " " + value);
        switch (type)
        {
            case SerializedPropertyType.Integer:
                property.intValue = int.Parse(value);
                break;
            case SerializedPropertyType.Boolean:
                property.boolValue = bool.Parse(value);
                break;
            case SerializedPropertyType.Float:
                property.floatValue = float.Parse(value);
                break;
            case SerializedPropertyType.String:
                property.stringValue = value;
                break;
            case SerializedPropertyType.Color:
                property.colorValue = ColorUtility.TryParseHtmlString(value, out Color color) ? color : Color.white;
                break;
            case SerializedPropertyType.LayerMask:
                property.intValue = int.Parse(value);
                break;
            case SerializedPropertyType.Enum:
                property.enumValueIndex = int.Parse(value);
                break;
            case SerializedPropertyType.Vector2:
                property.vector2Value = TryParseVector2(value, out Vector2 vector2) ? vector2 : Vector2.zero;
                break;
            case SerializedPropertyType.Vector3:
                property.vector3Value = TryParseVector3(value, out Vector3 vector3) ? vector3 : Vector3.zero;
                break;
            case SerializedPropertyType.Vector4:
                property.vector4Value = TryParseVector4(value, out Vector4 vector4) ? vector4 : Vector4.zero;
                break;
            case SerializedPropertyType.ArraySize:
                property.arraySize = int.Parse(value);
                break;
            case SerializedPropertyType.Character:
                property.intValue = int.Parse(value);
                break;
            case SerializedPropertyType.Quaternion:
                property.quaternionValue = TryParseQuaternion(value, out Quaternion quaternion) ? quaternion : Quaternion.identity;
                break;
            default:
                return false;
        }
        return true;
    }
    #endif
    public static bool SetFieldValue(Component component, FieldInfo field, string value){
        // Debug.Log("SetFieldValue: " + field.Name + " " + value);
        switch (field.FieldType.ToString())
        {
            case "System.Int32":
                field.SetValue(component, int.Parse(value));
                break;
            case "System.Boolean":
                field.SetValue(component, bool.Parse(value));
                break;
            case "System.Single":
                field.SetValue(component, float.Parse(value));
                break;
            case "System.String":
                field.SetValue(component, value);
                break;
            case "UnityEngine.Color":
                field.SetValue(component, ColorUtility.TryParseHtmlString(value, out Color color) ? color : Color.white);
                break;
            case "UnityEngine.Vector2":
                field.SetValue(component, TryParseVector2(value, out Vector2 vector2) ? vector2 : Vector2.zero);
                break;
            case "UnityEngine.Vector3":
                field.SetValue(component, TryParseVector3(value, out Vector3 vector3) ? vector3 : Vector3.zero);
                break;
            case "UnityEngine.Vector4":
                field.SetValue(component, TryParseVector4(value, out Vector4 vector4) ? vector4 : Vector4.zero);
                break;
            case "UnityEngine.Quaternion":
                field.SetValue(component, TryParseQuaternion(value, out Quaternion quaternion) ? quaternion : Quaternion.identity);
                break;
            default:
                return false;
        }
        return true;
    }
    #endregion
    
    
    public static float CalculateVelocityCancelFactor(Vector3 currentVelocity, Vector3 force)
    {
        float angle = Vector3.Angle(currentVelocity, force.normalized);
        float maxVelocityCancelAngle = 180f;
        float minVelocityCancelAngle = 0f;
        float halfVelocityCancelAngle = 90f;
        if (angle <= minVelocityCancelAngle)
        {
            Debug.Log("originalVelocity Calculation Case 1 happened");
            return 1f;
        }
        else if (angle >= maxVelocityCancelAngle)
        {
            Debug.Log("originalVelocity Calculation Case 2 happened");
            return 0f;
        }
        else if (angle <= halfVelocityCancelAngle)
        {
            Debug.Log("originalVelocity Calculation Case 3 happened");
            return 1f - 0.5f * (angle / halfVelocityCancelAngle);
        }
        else
        {
            Debug.Log("originalVelocity Calculation Case 4 happened");
            return 0.5f * ((maxVelocityCancelAngle - angle) / (maxVelocityCancelAngle - halfVelocityCancelAngle));
        }
    }
    
    #if UNITY_EDITOR
    public static List<Object> LoadAllPrefabsAtPath(string path) {
        List<Object> objects = new List<Object>();

        if (Directory.Exists(path)) {
            ProcessDirectory(path, objects);
        } else {
            Debug.LogError("Directory not found: " + path);
        }

        return objects;
    }
    private static void ProcessDirectory(string directoryPath, List<Object> objects) {
        string[] files = Directory.GetFiles(directoryPath, "*.prefab");
        foreach (string filePath in files) {
            objects.Add(AssetDatabase.LoadMainAssetAtPath(filePath));
            Debug.Log("Loaded " + filePath);
        }

        string[] subdirectories = Directory.GetDirectories(directoryPath);
        foreach (string subdirectory in subdirectories) {
            ProcessDirectory(subdirectory, objects);
        }
    }
    #endif
    public static Vector3 GetRandomizedRaycastDirection(Vector3 raycastDirection, float spread){
        raycastDirection = raycastDirection.normalized * 2;
        raycastDirection += new Vector3(Random.Range(-spread, spread), Random.Range(-spread, spread), Random.Range(-spread, spread));
        return raycastDirection;
    }
    public static Vector3 OptimizeAimingDirection(Vector3 direction, float downwardThreshold=10f){
        if(Vector3.Angle(direction, Vector3.down) < downwardThreshold){
            direction = Vector3.Lerp(Vector3.down, direction, Vector3.Angle(direction, Vector3.down)/downwardThreshold);
        }
        return direction;
    }
    public static string GetRandomGuid(){
        return Guid.NewGuid().ToString().Replace('-', '_');
    }
    public static Dictionary<string, int> GetWordCountDict(string line, List<string> phraseList)
    {
        Dictionary<string, int> dict = new Dictionary<string, int>();

        // Split line into different phrases
        string[] words = line.Split(new char[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries);

        foreach (string phrase in phraseList)
        {
            int count = 0;

            // Iterate over the words array to count occurrences of the current phrase
            for (int i = 0; i < words.Length - phrase.Split(' ').Length + 1; i++)
            {
                bool match = true;

                // Check if the current phrase matches the words from the line
                for (int j = 0; j < phrase.Split(' ').Length; j++)
                {
                    if (words[i + j] != phrase.Split(' ')[j])
                    {
                        match = false;
                        break;
                    }
                }

                if (match)
                    count++;
            }

            dict[phrase] = count;
        }

        return dict;
    }
    public static Vector3 AddPitch(Vector3 forwardVector, float pitch){
        Vector3 rightVector = Vector3.Cross(Vector3.up, forwardVector).normalized;
        forwardVector = Quaternion.AngleAxis(pitch, rightVector) * forwardVector;
        return forwardVector;
    }
    public static Vector3 AddYaw(Vector3 forwardVector, float yaw){
        Vector3 rightVector = Vector3.Cross(Vector3.up, forwardVector).normalized;
        Vector3 upVector = Vector3.Cross(forwardVector, rightVector).normalized;
        forwardVector = Quaternion.AngleAxis(yaw, upVector) * forwardVector;
        return forwardVector;
    }
    public static string GetMethodSignatureText(MethodInfo methodInfo){
        //If has parameters
        if(methodInfo.GetParameters().Length > 0){
            string parameters = "";
            foreach(var parameter in methodInfo.GetParameters()){
                parameters += parameter.ParameterType.Name + " " + parameter.Name + ", ";
            }
            parameters = parameters.Substring(0, parameters.Length - 2);
            return methodInfo.ReturnType.Name + " " + methodInfo.Name + "(" + parameters + ")";
        }
        //If has no parameters
        return methodInfo.ReturnType.Name + " " + methodInfo.Name + "()";
    }
}
