using UnityEngine;

public class WeightSystem : MonoBehaviour
{
    [Header("Weight Settings")]
    [SerializeField] private float startingWeight = 1.0f;
    [SerializeField] private float weightPerPoint = 0.1f;

    private float currentWeight;

    private void Awake()
    {
        ResetWeight();
    }

    public void AddWeightForPoint()
    {
        currentWeight += weightPerPoint;

        Debug.Log("Current Weight: " + currentWeight);
    }

    public float GetCurrentWeight()
    {
        return currentWeight;
    }

    public void ResetWeight()
    {
        currentWeight = startingWeight;

        Debug.Log("Weight Reset: " + currentWeight);
    }
}