using UnityEngine;

public class WeightSystem : MonoBehaviour
{
    [Header("Weight Settings")]
    [SerializeField] private float startingWeight = 1.0f;

    // Each collected point adds 0.04x weight.
    [SerializeField] private float weightPerPoint = 0.04f;

    // Prevent the car from becoming impossible to control.
    [SerializeField] private float maximumWeight = 1.80f;

    private float currentWeight;

    private void Awake()
    {
        ResetWeight();
    }

    public void AddWeightForPoint()
    {
        currentWeight += weightPerPoint;

        currentWeight = Mathf.Min(
            currentWeight,
            maximumWeight
        );

        Debug.Log("Current Weight: " + currentWeight);
    }

    public void RemoveWeightForPoint()
    {
        currentWeight -= weightPerPoint;

        currentWeight = Mathf.Max(
            currentWeight,
            startingWeight
        );

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