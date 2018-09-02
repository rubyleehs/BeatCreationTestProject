using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour {

    public RectTransform uIBG;
    public float uIBGScreenRatio;

    public Transform I_timerBar;
    public Transform[] I_tuneVisualizerBars;
    public Transform I_beatParent;
    public GameObject I_beatGO;

    public static Transform timerBar;
    public static Transform[] tuneVisualizerBars;
    public static Transform beatParent;
    public static GameObject beatGO;

    //screenData
    public static Camera cam;
    private Canvas canvas;
    private RectTransform canvasRT;
    private static Vector2 camScreenSize;


    private void Awake()
    {
        timerBar = I_timerBar;
        tuneVisualizerBars = I_tuneVisualizerBars;
        beatParent = I_beatParent;
        beatGO = I_beatGO;
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

    public static void CreateBeatTypeVisual(int _beatTypeIndex, float _ratio)
    {
        Transform _beat = Instantiate(beatGO, GetVisualizerRatioPosition(_ratio), Quaternion.identity, beatParent).transform;
        SpriteRenderer _spriteRenderer = _beat.GetComponent<SpriteRenderer>();
        _spriteRenderer.color = AudioManager.audioClipsInfo[_beatTypeIndex].color;
    }
}
