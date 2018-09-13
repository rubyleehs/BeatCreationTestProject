using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BeatAnim : MonoBehaviour {

    public int type;
    public Vector3 targetPos;
    public float targetPosRad;
    public GameObject line;
    public GameObject text;
    public float animDuration;
    public float lineDuration;
    public SpriteRenderer sprite;
    public Vector2 lineMinMaxHeight;
    public RectTransform rectT;
    public Vector3 missScale;
    public Vector3 hitScale;
    public Vector3 dmgStartScale;
    public float dmgAscensionSpeed;
    public float expandDur;
    public float waitDur;
    public float fadeDur;

    private Vector3 startScale;
    private Color startColor;
    private EnemyManager enemyManager;

    public void Awake()
    {
        rectT = this.GetComponent<RectTransform>();
        startScale = rectT.localScale;
    }

    public IEnumerator EventAnim(bool _beatCaptured,float _acc)//RMB SCALE
    {
        startColor = sprite.color;
        float _progress = 0;
        float _smoothProgress = 0;
        float _startTime = Time.time;
        if (_beatCaptured) StartCoroutine(AttackAnim(_acc));
        while (_progress < 1)
        {
            _progress = (Time.time - _startTime) / animDuration;
            _smoothProgress = Mathf.SmoothStep(0, 1, _progress);
            if (_beatCaptured) rectT.localScale = Vector3.Lerp(startScale, hitScale, _smoothProgress);
            else rectT.localScale = Vector3.Lerp(startScale, missScale, _smoothProgress);

            sprite.color = Color.Lerp(startColor, Color.clear, _smoothProgress);
            
            yield return null;
        }

        yield return new WaitForSeconds(lineDuration);
    }

    public IEnumerator AttackAnim(float _acc)
    {
        float _progress = 0;
        float _startTime = Time.time;
        Transform _atk = Instantiate(line, this.transform.position, Quaternion.identity).transform;
        TrailRenderer _trail = _atk.GetComponent<TrailRenderer>();
        _trail.startColor = sprite.color;
        _trail.endColor = sprite.color;

        Vector3[] _points = new Vector3[5] { this.transform.position, RandomPoint(8), 0.5f * (this.transform.position + targetPos) + Vector3.up * Random.Range(lineMinMaxHeight.x, lineMinMaxHeight.y),RandomPoint(6), targetPos + targetPosRad * (Vector3)Random.insideUnitCircle};
        while (_progress < 1)
        {
            Debug.Log(_progress);
            _progress = (Time.time - _startTime) / lineDuration;
            _atk.position = UIManager.GetBezierCurvePoint(_points, _progress);

            yield return null;
        }
        StartCoroutine(DealDamage(_atk.position, _acc));
    }
   
    public IEnumerator DealDamage(Vector3 _pos, float _acc)
    {
        Text _text = Instantiate(text, _pos, Quaternion.identity,this.transform.parent.parent.parent).GetComponent<Text>();
        int _damage = Mathf.CeilToInt(_acc * AudioManager.audioClipsInfo[type].baseDamage);
        if (_damage <= 0) _damage = 1;
        _text.text = _damage.ToString();
        float _progress = 0;
        float _smoothProgress = 0;
        float _timeStart = Time.time;
        Color _endColor = _text.color;
        Vector3 _endScale = _text.rectTransform.localScale;
        UIManager.enemyHolder.GetComponent<EnemyManager>().DealDamage(_damage);
        while (_progress < 1)
        {
            _progress = (Time.time - _timeStart) / expandDur;
            _smoothProgress = Mathf.SmoothStep(0, 1, _progress);
            _text.color = Color.Lerp(Color.clear, _endColor,_smoothProgress);
            _text.transform.localScale = Vector3.Lerp(dmgStartScale, _endScale, _smoothProgress);
            _text.transform.position += Vector3.up * dmgAscensionSpeed * Time.deltaTime;
            yield return null;
        }
        
        _timeStart = Time.time;
        _progress = 0;
        while(Time.time -_timeStart < waitDur)
        {
            _text.transform.position += Vector3.up * dmgAscensionSpeed * Time.deltaTime;
            yield return null;
        }
        _timeStart = Time.time;
        _progress = 0;
        while(_progress < 1)
        {
            _progress = (Time.time - _timeStart) / fadeDur;
            _text.color = Color.Lerp(_endColor, Color.clear, _progress * _progress);
            _text.transform.position += Vector3.up * dmgAscensionSpeed * Time.deltaTime;
            yield return null;
        }
    }

    public Vector3 RandomPoint(float _rad)
    {
        Vector3 _randPoint = targetPos + (Vector3)(Random.insideUnitCircle * targetPosRad * _rad);
        return _randPoint;
    }
}