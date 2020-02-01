using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class time : MonoBehaviour
{
    private float day = 0.5f;
    public float min;
    public float hours;
    private const float DaytoSecRat = 600f;
    public Text hrsclock;
    public Text minclock;
    public float sun;
    public Text am;


    // Start is called before the first frame update
    void Start()
    {
        hrsclock.text = hours.ToString();

        minclock.text = min.ToString();
        Light light = GetComponent<Light>();
        Debug.Log(light.color);
        am.text = am.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        day += Time.deltaTime / DaytoSecRat;

        float daynorm = day % 1f;

        hours = Mathf.Floor((daynorm * 24f));

        if (hours < 1) {
            hours = 12;
        }

        hrsclock.text = Mathf.Round(hours).ToString();

        min = ((daynorm * 24f) % 1f) * 60f;

        minclock.text = Mathf.Round(min).ToString("00");

        if (daynorm < 0.5)
        {
            sun = daynorm * 2f;
            am.text = ("am");
        }
        else {
            sun = (1f - daynorm) * 2f;
            am.text = ("pm");
            if (hours > 12) {
                hours = hours - 12;
            }
        }

        hrsclock.text = Mathf.Round(hours).ToString();

        GetComponent<Light>().color = new Color ( .5f * sun * 2, .5f * sun * 2, .8f, 1f);
     }

}
