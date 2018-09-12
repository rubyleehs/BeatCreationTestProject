using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public float enemyShakeDur;
    public Vector2 enemyShakeRad;

    public int enemyMaxHP = 100;
    public int damageTaken;
    private float enemyCurHP;

    private Vector3 oriPos;

    private void Awake()
    {
        oriPos = this.transform.position;
    }

    public void DealDamage(int _dmg)
    {
        StartCoroutine(Shake());
        damageTaken += _dmg;
        enemyCurHP = enemyMaxHP - damageTaken;
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
}
