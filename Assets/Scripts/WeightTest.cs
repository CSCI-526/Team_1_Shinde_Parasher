using UnityEngine;

public class WeightTest : MonoBehaviour
{
    private WeightSystem weightSystem;

    private void Start()
    {
        weightSystem = GetComponent<WeightSystem>();
    }

    private void Update()
    {
        // Press P to simulate collecting a point
        if (Input.GetKeyDown(KeyCode.P))
        {
            weightSystem.AddWeightForPoint();
        }

        // Press R to reset weight
        if (Input.GetKeyDown(KeyCode.R))
        {
            weightSystem.ResetWeight();
        }
    }
}