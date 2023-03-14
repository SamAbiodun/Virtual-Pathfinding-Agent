using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PerformanceTracker : MonoBehaviour
{
    public Text timerText;
    public Text distanceText;

    private float startTime;
    private float distance;
    public bool isRunning;

    private void Start()
    {
        // Initialize variables.
        distance = 0;
        isRunning = true;
        startTime = Time.time;
    }

    private void Update()
    {
        if (isRunning)
        {
            // Update timer text.
            float timeElapsed = Time.time - startTime;
            timerText.text = "Time: " + timeElapsed.ToString("F2");

            // Calculate distance covered since last frame.
            float distanceCovered = Vector3.Distance(transform.position, transform.position - transform.forward);

            // Update total distance.
            distance += distanceCovered;
            distanceText.text = "Distance: " + distance.ToString("F2") + " m";
        }
    }

    public void StopTracking()
    {
        isRunning = false;
        
    }
}
