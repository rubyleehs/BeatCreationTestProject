using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIScaler : MonoBehaviour {

    public RectTransform uIBG;
    public float uIBGScreenRatio;


    //screenData
    public static Camera cam;
    private Canvas canvas;
    private RectTransform canvasRT;
    private static Vector2 camScreenSize;


    private void Awake()
    {
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
}
