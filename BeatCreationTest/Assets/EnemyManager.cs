using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyManager : MonoBehaviour
{
    public List<Sprite> enemySprites;
    public List<Color> enemyColors;

    public float enemyShakeDur;
    public Vector2 enemyShakeRad;
    public Transform enemyHpBar;
    public Text enemyHpText;
    public Color overkillHpColor;

    public int enemyMaxHP = 30;
    public int damageTaken;
    private int enemyCurHP;
    private SpriteRenderer hpBarSprite;
    private Color hpBarOriColor;

    private Vector3 oriPos;

    public Transform enemyTransform;
    public float enemyDeathAnimDur;
    public float enemyDeathSpinSpeed;
    public float enemyDeathMoveRad;

    public float enemySpawnDur;

    public static int curEnemyCount = 1;
    private int overkillDmg;
    private void Awake()
    {
        oriPos = this.transform.position;
        hpBarSprite = enemyHpBar.GetComponent<SpriteRenderer>();
        hpBarOriColor = hpBarSprite.color;
    }

    private void Start()
    {
        CreateNewEnemy();
    }

    public void DealDamage(int _dmg)
    {
        StartCoroutine(Shake());
        damageTaken += _dmg;
        enemyCurHP = enemyMaxHP - damageTaken;
        UpdateHpBar();
    }


    public IEnumerator Shake()
    {
        float _progress = 0;
        float _startTime = Time.time;
        while (_progress < 1)
        {
            _progress = (Time.time - _startTime) / enemyShakeDur;

            this.transform.position = oriPos + Random.insideUnitCircle.x * enemyShakeRad.x * Vector3.right + Random.insideUnitCircle.y * enemyShakeRad.y * Vector3.up;
            yield return null;
        }
        this.transform.position = oriPos;
    }

    public void UpdateHpBar()
    {
        float _hPRatio = ((float)enemyCurHP / (float)enemyMaxHP);
        Debug.Log(_hPRatio + " = " + enemyCurHP + " / " +enemyMaxHP);
        enemyHpBar.localPosition = Mathf.LerpUnclamped(-0.5f, 0, _hPRatio) * Vector3.right;
        enemyHpBar.localScale = Vector3.LerpUnclamped(new Vector3(0, 1, 1), Vector3.one, _hPRatio);
        if (_hPRatio < 0)
        {
            hpBarSprite.color = overkillHpColor;
            overkillDmg = -enemyCurHP;
        }
        else hpBarSprite.color = hpBarOriColor;

        enemyHpText.text = enemyCurHP + " / " + enemyMaxHP + " ";
    }

    public void ResetEnemy()
    {
        if(damageTaken < enemyMaxHP)
        {
            Debug.Log("Failed! Try Again!");
            damageTaken = 0;
            enemyCurHP = enemyMaxHP;
            UpdateHpBar();
        }
        else
        {
            StartCoroutine(KillCurEnemy());
            CreateNewEnemy();
            Debug.Log("creating new enemy");
        }
    }

    public IEnumerator KillCurEnemy()
    {
        float _progress = 0;
        float _smoothProgress = 0;
        float _startTime = Time.time;
        SpriteRenderer _enemySprite = enemyTransform.GetComponent<SpriteRenderer>();
        Color _enemyStartColor = _enemySprite.color;
        Vector3 _enemyStartScale = enemyTransform.localScale;
        Vector3 _randMoveDir = Random.insideUnitCircle * enemyDeathMoveRad;
        Vector3 _oriPos = enemyTransform.position;
        while (_progress < 1)
        {
            _progress = (Time.time - _startTime) / enemyDeathAnimDur;
            _smoothProgress = Mathf.SmoothStep(0, 1, _progress);
            _enemySprite.color = Color.Lerp(_enemyStartColor, Color.clear, _smoothProgress);
            enemyTransform.localScale = Vector3.Lerp(_enemyStartScale, Vector3.zero, _smoothProgress);
            enemyTransform.position = Vector3.Lerp(_oriPos, _oriPos + _randMoveDir, _progress);
            enemyTransform.rotation *= Quaternion.Euler(Vector3.forward * enemyDeathSpinSpeed);
            yield return null;
        }
        _progress = 0;
        _startTime = Time.time;
        _enemyStartColor = enemyColors[Random.Range(0, enemyColors.Count)];
        _enemySprite.sprite = enemySprites[Random.Range(0, enemySprites.Count)];
        enemyTransform.position = _oriPos;
        while(_progress < 1)
        {
            _progress = (Time.time - _startTime) / enemyDeathAnimDur;
            _smoothProgress = Mathf.SmoothStep(0, 1, _progress);
            enemyTransform.localScale = Vector3.Lerp(Vector3.zero, _enemyStartScale, _smoothProgress);
            _enemySprite.color = Color.Lerp(Color.clear, _enemyStartColor, _smoothProgress);
            yield return null;
        }
    }

    public void CreateNewEnemy()
    {

        TuneScorer.currentScore += Mathf.CeilToInt(curEnemyCount * overkillDmg * overkillDmg * 0.33f);
        TuneScorer.scoreText.text = "Score: " + TuneScorer.currentScore;

        curEnemyCount++;
        enemyMaxHP = Mathf.Max(10,enemyMaxHP) + Mathf.CeilToInt((curEnemyCount * curEnemyCount *0.2f + overkillDmg * 0.15f));
        enemyCurHP = enemyMaxHP;
        damageTaken = 0;
        UpdateHpBar();//
    }
}
