using UnityEngine;

public class ConditionalRequireComponentAttribute : MonoBehaviour
{
    private System.Type[] componentTypes;

    public ConditionalRequireComponentAttribute(params System.Type[] componentTypes)
    {
        this.componentTypes = componentTypes;
    }

    private void Awake()
    {
        // Check if all the conditional components exist on the game object and add them if they don't
        foreach (System.Type componentType in componentTypes)
        {
            if (componentType == null)
            {
                Debug.LogWarning("ConditionalRequireComponentAttribute: Invalid type specified");
                continue;
            }

            if (!gameObject.GetComponent(componentType))
            {
                gameObject.AddComponent(componentType);
                Debug.Log("ConditionalRequireComponentAttribute: Added " + componentType.Name + " component to " + gameObject.name);
            }
        }

        // Add the required component if all the conditional components exist
        if (AllComponentsExist(componentTypes))
        {
            System.Type requiredType = this.GetType();
            if (!gameObject.GetComponent(requiredType))
            {
                gameObject.AddComponent(requiredType);
                Debug.Log("ConditionalRequireComponentAttribute: Added " + requiredType.Name + " component to " + gameObject.name);
            }
        }
        else
        {
            Debug.LogWarning("ConditionalRequireComponentAttribute: One or more conditional components are missing from " + gameObject.name + ", so the required component was not added");
        }
    }

    private bool AllComponentsExist(System.Type[] componentTypes)
    {
        foreach (System.Type componentType in componentTypes)
        {
            if (!gameObject.GetComponent(componentType))
            {
                return false;
            }
        }
        return true;
    }
}