using TMPro;
using UnityEngine;
using UnityEngine.WSA;

public class TimeManager : MonoBehaviour
{
    public float day = 1.0f;
    private float daysInYear = 120;
    private float timeInDay;
    [SerializeField] private float endOfDay = 600.0f;
    [SerializeField] private enum Season { Spring, Summer, Autumn, Winter };
    [SerializeField] private TextMeshProUGUI DayCounter;
    [SerializeField] private TextMeshProUGUI TimeCounter;
    Season currentSeason;
    void Awake()
    {
        getDay();
        getTimeInDay();
        getSeason();
        DayCounter.text = "Day " + day.ToString() + "/" + daysInYear.ToString();
    }

    void Update()
    {
        timeInDay += Time.deltaTime;
        //Time Counter for debugging purposes, safe to remove
        TimeCounter.text = timeInDay.ToString();
        //^Time Counter for debugging purposes, safe to remove^
        if (timeInDay >= endOfDay)
        {
            AdvanceDay();
        }
    }

    void getDay()
    {
        if (PlayerPrefs.HasKey("Day"))
        {
            day = PlayerPrefs.GetFloat("Day");
        }
        else
        {
            day = 1.0f;
        }
    }

    void getTimeInDay()
    {
        if (PlayerPrefs.HasKey("TimeInDay"))
        {
            timeInDay = PlayerPrefs.GetFloat("TimeInDay");
        }
        else
        {
            timeInDay = 0;
        }
    }

    void getSeason()
    {
        if (day / daysInYear <= 0.25f)
        {
            currentSeason = Season.Spring;
        }
        else if (day / daysInYear <= 0.50f)
        {
            currentSeason = Season.Summer;
        }
        else if (day / daysInYear <= 0.75f)
        {
            currentSeason = Season.Autumn;
        }
        else
        {
            currentSeason = Season.Winter;
        }
    }

    void AdvanceDay()
    {
        day++;
        timeInDay = 0;
        DayCounter.text = "Day " + day.ToString() + "/" + daysInYear.ToString();
        getSeason();
    }
}
