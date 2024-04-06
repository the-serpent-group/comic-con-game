using System.Collections;

using UnityEngine;

public enum Day
{
    Monday,
    Tuesday,
    Wednesday,
    Thursday,
    Friday,
    Saturday,
    Sunday,
}

public class DayManager : MonoBehaviour
{
    public float time; 
    public Day today;
    public string businessStatus; 

    private float timeScale = 600f; 

    void Start()
    {
        time = 0f; 
        today = Day.Monday; 
        StartCoroutine(TimeOfDay());
    }

    IEnumerator TimeOfDay()
    {
        while (true)
        {
            UpdateBusinessStatus();
            time += Time.deltaTime * timeScale / 60; 

            if (time >= 24f) 
            {
                time = 0f; 
                today = (Day)(((int)today + 1) % 7); 
            }

            yield return null;
        }
    }

    void UpdateBusinessStatus()
    {
        if (today == Day.Sunday)
        {
            businessStatus = "Closed"; 
        }
        else
        {
            bool isPeakHours = time >= 12.00f && time <= 14.00f;            
            bool isPeakDay = today == Day.Monday || today == Day.Friday || today == Day.Saturday;

            if (isPeakDay && isPeakHours)
            {
                businessStatus = "Busy";
            }
            else
            {
                businessStatus = "Not Busy";
            }
        }

        int hours = (int)time;
        int minutes = (int)((time - hours) * 60);

        string formattedTime = string.Format("{0:00}:{1:00}", hours, minutes);

        Debug.Log($"Day: {today}, Time: {formattedTime}, Status: {businessStatus}");
    }
}
