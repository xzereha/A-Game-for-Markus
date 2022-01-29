using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class WorldManager : MonoBehaviour
{

    public static WorldManager Instance
    {
        get
        {
            s_Instance ??= FindObjectOfType<WorldManager>();
            if(!s_Instance)
            {
                Debug.LogError("Can't find instance of WorldManager in the loaded scene");
            }
            return s_Instance;
        }
    }

    public static Vector3 CurrentCheckpoint => Instance.m_CurrentCheckpoint.Position;

    [SerializeField] private Camera m_Camera;
    [SerializeField] private WorldDesc heaven;
    [SerializeField] private WorldDesc hell;
    [SerializeField] private Checkpoint m_CurrentCheckpoint;
    [SerializeField] private bool m_ShouldWorldChange;
    [SerializeField] private float m_TimeBeforeChange;
    private static WorldManager s_Instance;
    private bool firstCurrent;
    private Color32 color;
    private float m_timeToFlip;

    public void ToggleAutoCycling()
    {
        m_ShouldWorldChange ^= true;
    }

    public void SwitchWorld()
    {
        Debug.Log("Swap World");
        firstCurrent ^= true;
        color = firstCurrent? heaven.skyColor : hell.skyColor;
        heaven.gameObject.SetActive(firstCurrent);
        hell?.gameObject.SetActive(!firstCurrent);
    }

    private void Start()
    {
        SwitchWorld();
        if(m_ShouldWorldChange)
            StartCoroutine(WorldCycling());
    }

    private void Update()
    {

    }

    private void OnEnable() 
    {
        Checkpoint.OnActivate += OnCheckpointChanged;
    }

    private void OnDisable() 
    {
        Checkpoint.OnActivate -= OnCheckpointChanged;
        s_Instance = s_Instance == this? null : s_Instance;
    }

    private void OnCheckpointChanged(Checkpoint checkpoint)
    {
        m_CurrentCheckpoint = checkpoint;
    }

    private void Reset() 
    {
        m_Camera ??= FindObjectOfType<Camera>();
    }

    private IEnumerator WorldCycling()
    {
        while(true)
        {
            m_timeToFlip += Time.deltaTime;
            m_Camera.backgroundColor = color;
            color = firstCurrent ? LerpViaHSB(color, hell.skyColor, 0.005f) : LerpViaHSB(color, heaven.skyColor, 0.005f);
            if(m_timeToFlip >= m_TimeBeforeChange)
            {
                m_timeToFlip = 0;
                SwitchWorld();
            }
            yield return null;
        }
    }

#if UNITY_EDITOR
    [ContextMenu("Add Checkpoints")]
    private void AddCheckpoints()
    {
        List<Checkpoint> activeCheckpoints = FindObjectsOfType<Checkpoint>().Where(c => c.Active == true).ToList();
        if(activeCheckpoints.Count != 1) Debug.LogError("The world needs to contain excactly one active checkpoint!");
        m_CurrentCheckpoint = activeCheckpoints.First();
    }

    private void OnGUI() 
    {
        if(GUI.Button(new Rect(0, 0, 100, 40), "Swap World"))
        {
            SwitchWorld();
        }
    }
#endif

    public Color LerpViaHSB(Color a, Color b, float t)
    {
        return HSBColor.Lerp(HSBColor.FromColor(a), HSBColor.FromColor(b), t).ToColor();
    }
}

[System.Serializable]
public struct HSBColor
{
    public float h;
    public float s;
    public float b;
    public float a;

    public HSBColor(float h, float s, float b, float a)
    {
        this.h = h;
        this.s = s;
        this.b = b;
        this.a = a;
    }

    public HSBColor(float h, float s, float b)
    {
        this.h = h;
        this.s = s;
        this.b = b;
        this.a = 1f;
    }

    public HSBColor(Color col)
    {
        HSBColor temp = FromColor(col);
        h = temp.h;
        s = temp.s;
        b = temp.b;
        a = temp.a;
    }

    public static HSBColor FromColor(Color color)
    {
        HSBColor ret = new HSBColor(0f, 0f, 0f, color.a);

        float r = color.r;
        float g = color.g;
        float b = color.b;

        float max = Mathf.Max(r, Mathf.Max(g, b));

        if (max <= 0)
        {
            return ret;
        }

        float min = Mathf.Min(r, Mathf.Min(g, b));
        float dif = max - min;

        if (max > min)
        {
            if (g == max)
            {
                ret.h = (b - r) / dif * 60f + 120f;
            }
            else if (b == max)
            {
                ret.h = (r - g) / dif * 60f + 240f;
            }
            else if (b > g)
            {
                ret.h = (g - b) / dif * 60f + 360f;
            }
            else
            {
                ret.h = (g - b) / dif * 60f;
            }
            if (ret.h < 0)
            {
                ret.h = ret.h + 360f;
            }
        }
        else
        {
            ret.h = 0;
        }

        ret.h *= 1f / 360f;
        ret.s = (dif / max) * 1f;
        ret.b = max;

        return ret;
    }

    public static Color ToColor(HSBColor hsbColor)
    {
        float r = hsbColor.b;
        float g = hsbColor.b;
        float b = hsbColor.b;
        if (hsbColor.s != 0)
        {
            float max = hsbColor.b;
            float dif = hsbColor.b * hsbColor.s;
            float min = hsbColor.b - dif;

            float h = hsbColor.h * 360f;

            if (h < 60f)
            {
                r = max;
                g = h * dif / 60f + min;
                b = min;
            }
            else if (h < 120f)
            {
                r = -(h - 120f) * dif / 60f + min;
                g = max;
                b = min;
            }
            else if (h < 180f)
            {
                r = min;
                g = max;
                b = (h - 120f) * dif / 60f + min;
            }
            else if (h < 240f)
            {
                r = min;
                g = -(h - 240f) * dif / 60f + min;
                b = max;
            }
            else if (h < 300f)
            {
                r = (h - 240f) * dif / 60f + min;
                g = min;
                b = max;
            }
            else if (h <= 360f)
            {
                r = max;
                g = min;
                b = -(h - 360f) * dif / 60 + min;
            }
            else
            {
                r = 0;
                g = 0;
                b = 0;
            }
        }

        return new Color(Mathf.Clamp01(r), Mathf.Clamp01(g), Mathf.Clamp01(b), hsbColor.a);
    }

    public Color ToColor()
    {
        return ToColor(this);
    }

    public override string ToString()
    {
        return "H:" + h + " S:" + s + " B:" + b;
    }

    public static HSBColor Lerp(HSBColor a, HSBColor b, float t)
    {
        float h, s;

        //check special case black (color.b==0): interpolate neither hue nor saturation!
        //check special case grey (color.s==0): don't interpolate hue!
        if (a.b == 0)
        {
            h = b.h;
            s = b.s;
        }
        else if (b.b == 0)
        {
            h = a.h;
            s = a.s;
        }
        else
        {
            if (a.s == 0)
            {
                h = b.h;
            }
            else if (b.s == 0)
            {
                h = a.h;
            }
            else
            {
                // works around bug with LerpAngle
                float angle = Mathf.LerpAngle(a.h * 360f, b.h * 360f, t);
                while (angle < 0f)
                    angle += 360f;
                while (angle > 360f)
                    angle -= 360f;
                h = angle / 360f;
            }
            s = Mathf.Lerp(a.s, b.s, t);
        }
        return new HSBColor(h, s, Mathf.Lerp(a.b, b.b, t), Mathf.Lerp(a.a, b.a, t));
    }

    public static void Test()
    {
        HSBColor color;

        color = new HSBColor(Color.red);
        Debug.Log("red: " + color);

        color = new HSBColor(Color.green);
        Debug.Log("green: " + color);

        color = new HSBColor(Color.blue);
        Debug.Log("blue: " + color);

        color = new HSBColor(Color.grey);
        Debug.Log("grey: " + color);

        color = new HSBColor(Color.white);
        Debug.Log("white: " + color);

        color = new HSBColor(new Color(0.4f, 1f, 0.84f, 1f));
        Debug.Log("0.4, 1f, 0.84: " + color);

        Debug.Log("164,82,84   .... 0.643137f, 0.321568f, 0.329411f  :" + ToColor(new HSBColor(new Color(0.643137f, 0.321568f, 0.329411f))));
    }
}
