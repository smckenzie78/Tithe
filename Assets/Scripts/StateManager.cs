using UnityEngine;

public class StateManager : MonoBehaviour
{
    TimeManager timeManager;
    public float water;
    public float soil;
    public float wildlife;
    public float heat;
    public float cold;
    private float day = 1.0f;

    private bool droughtActive = false;

    void Awake()
    {
        timeManager = FindAnyObjectByType<TimeManager>();
    }
    void Start()
    {
        getCurrentState();
        getDay();
    }

    // Update is called once per frame
    void Update()
    {
        if (timeManager.day > day)
        {
            StateDegradation();
            day = timeManager.day;
            UpdateState();
        }
    }

    void getCurrentState()
    {
        if (PlayerPrefs.HasKey("water"))
        {
            water = PlayerPrefs.GetFloat("water");
        }
        else
        {
            water = 50.0f;
        }
        if (PlayerPrefs.HasKey("soil"))
        {
            soil = PlayerPrefs.GetFloat("soil");
        }
        else
        {
            soil = 50.0f;
        }
    }

    void getDay()
    {
        day = timeManager.day;
    }

    void StateDegradation()
    {
        water = Mathf.Max(water - 20.0f, 0);
    }

    void UpdateState()
    {
        if (water <= 30.0f && water > 10.0)
        {
            Debug.Log("The water levels are lowering");
            droughtActive = true;
        }
        else if (water <= 10.0f && water > 0)
        {
            Debug.Log("The water levels are extremely low");
        }
        else if (water <= 0)
        {
            Debug.Log("All the water has dried up!");
        }
    }
}
