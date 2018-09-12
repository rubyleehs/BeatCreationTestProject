using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour {

    public TuneManager tuneManager;

    public RectTransform uIBG;
    public float uIBGScreenRatio;
    public float I_missAnimDuration;

    public Transform I_timerBar;
    public Transform[] I_tuneVisualizerBars;
    public Transform I_beatParent;
    public GameObject I_beatGO;
    public Color barBGMissColor;
    public Color barBGCaptureColor;

    public float eventDuration;

    public static Transform timerBar;
    public static Transform[] tuneVisualizerBars;
    public static Transform beatParent;
    public static GameObject beatGO;

    //screenData
    public static Camera cam;
    private Canvas canvas;
    private RectTransform canvasRT;
    private static Vector2 camScreenSize;
    private static Color barBGOriColor;
    private SpriteRenderer[] visualizerBarsSpriteRenderers;
    public static float lastEventTime = 0;
    public static Transform enemyHolder;
    public Transform I_enemyHolder;

    private void Awake()
    {
        enemyHolder = I_enemyHolder;
        timerBar = I_timerBar;
        tuneVisualizerBars = I_tuneVisualizerBars;
        beatParent = I_beatParent;
        beatGO = I_beatGO;
        barBGOriColor = tuneVisualizerBars[0].GetComponent<SpriteRenderer>().color;
        visualizerBarsSpriteRenderers = new SpriteRenderer[2] { tuneVisualizerBars[0].GetComponent<SpriteRenderer>(), tuneVisualizerBars[1].GetComponent<SpriteRenderer>() };
        UpdateScreenData();
        SetupUIBG();
    }

    void UpdateScreenData()
    {
        cam = Camera.main;
        canvas = this.GetComponent<Canvas>();
        canvasRT = canvas.GetComponent<RectTransform>();
        camScreenSize = new Vector3(canvasRT.rect.width,canvasRT.rect.height);
    }

    void SetupUIBG()
    {
        uIBG.pivot = new Vector3(0f, -1f);
        uIBG.localScale = new Vector3(camScreenSize.x, camScreenSize.y * uIBGScreenRatio,1);

        uIBG.localPosition = new Vector3(0, 0.5f * (uIBG.localScale.y - camScreenSize.y), 1);

    }

    public static void SetTimerLine(float _ratio)
    {
        timerBar.position = GetVisualizerRatioPosition(_ratio);
    }

    public static Vector2 GetVisualizerRatioPosition(float _ratio)
    {
        if (_ratio < 0.5f) return tuneVisualizerBars[0].position + Vector3.right * tuneVisualizerBars[0].lossyScale.x * (2 * _ratio - 0.5f);
        else if (_ratio <= 1f) return tuneVisualizerBars[1].position + Vector3.right * tuneVisualizerBars[1].lossyScale.x * (2 * _ratio - 1.5f);
        else return tuneVisualizerBars[0].position + Vector3.right * -0.5f * tuneVisualizerBars[0].lossyScale.x;
    }

    public static BeatAnim CreateBeatTypeVisual(int _beatTypeIndex, float _ratio)
    {
        BeatAnim _beat = Instantiate(beatGO, GetVisualizerRatioPosition(_ratio), Quaternion.identity, beatParent).GetComponent<BeatAnim>();
        _beat.type = _beatTypeIndex;
        SpriteRenderer _spriteRenderer = _beat.GetComponent<SpriteRenderer>();
        _spriteRenderer.color = AudioManager.audioClipsInfo[_beatTypeIndex].color;
        return _beat;
    }
    public static void ClearTuneVisuals()
    {
        Debug.Log("Beat Visuals Cleared!");
        for (int i = 0; i < beatParent.childCount; i++)
        {
            //Debug.Log(beatParent.GetChild(i).gameObject);
            Destroy(beatParent.GetChild(i).gameObject);
        } 
    }

    public static void LoadTuneVisuals(List<BeatData> _tuneData)
    {
        ClearTuneVisuals();
        Debug.Log("Beat Visuals Loaded!");
        for (int i = 0; i < _tuneData.Count; i++)
        {
            CreateBeatTypeVisual(_tuneData[i].typeIndex, _tuneData[i].progressRatioTime);
        }
    }

    public void InitiateBarAnim(bool _beatCaptured)
    {
        //CameraShake.Shake(0.15f, 0.28f);
        lastEventTime = Time.time;
        for (int i = 0; i < tuneVisualizerBars.Length; i++)
        {
            if (_beatCaptured) tuneVisualizerBars[i].GetComponent<SpriteRenderer>().color = barBGCaptureColor;
            else tuneVisualizerBars[i].GetComponent<SpriteRenderer>().color = barBGMissColor;
        }
    }

    private void Update()
    {
        for (int i = 0; i < tuneVisualizerBars.Length; i++)
        {
            visualizerBarsSpriteRenderers[i].color = Color.Lerp(visualizerBarsSpriteRenderers[i].color, barBGOriColor, (Time.time - lastEventTime)/eventDuration);
        }
    }

    public static Vector3 GetBezierCurvePoint(Vector3[] bezierNodes, float t)
    {
        if (bezierNodes.Length == 0)
        {
            Debug.Log("Cannot Form Bezier Curve With No Point, returning Vector3.Zero");
            return Vector3.zero;
        }
        if (bezierNodes.Length == 1)
        {
            Debug.Log("Cannot Form Bezier Curve With Single Point, Returning Original Point");
            return bezierNodes[0];
        }
        if (t > 1 || t < 0)
        {
            //Debug.Log("Start/End Points of Bezier Outside Range. t value should be between 0 and 1. May have error");
        }

        List<Vector3> _finalPoints = new List<Vector3>();
        _finalPoints.AddRange(bezierNodes);
        List<Vector3> _nodes = new List<Vector3>();

        while (_finalPoints.Count > 1)
        {
            _nodes.Clear();
            _nodes.AddRange(_finalPoints);
            _finalPoints.Clear();
            for (int i = 0; i < _nodes.Count - 1; i++)
            {
                _finalPoints.Add(Vector3.Lerp(_nodes[i], _nodes[i + 1], t));
            }

        }
        return _finalPoints[0];
    }
}
