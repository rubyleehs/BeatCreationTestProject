using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TuneScorer :MonoBehaviour
{
    public Text I_scoreText;

    public static List<float> progressRatioBetweenBeats;
    public static List<List<float>> progressRatioBetweenBeatTypes;
    public static List<int> noOfEachNoteType;
    public static float averageDurBetweenNotes;
    public static float sDDurBetweenNotes;

    public static List<float> averageDurBetweenNoteTypes;
    public static List<float> sDDurBetweenNoteTypes;

    public static List<float> scoreForNoteType;
    public static int currentScore =0;
    public static Text scoreText;

    public void Awake()
    {
        scoreText = I_scoreText;
    }
    public static void ScoreTune(TuneManager _tuneManager, float _softMinDurLowerLimit, float _typeCountTurnPoint)
    {
        currentScore = 0;
        progressRatioBetweenBeats = new List<float>();
        for (int i = 0; i < _tuneManager.tunes[0].tuneAllBeats.Count - 1; i++)
        {
            progressRatioBetweenBeats.Add(_tuneManager.tunes[0].tuneAllBeats[i + 1].progressRatioTime - _tuneManager.tunes[0].tuneAllBeats[i].progressRatioTime);
        }
        progressRatioBetweenBeatTypes = new List<List<float>>();
        noOfEachNoteType = new List<int>();
        for (int type = 0; type < _tuneManager.tunes[0].tuneTypeBeats.Count; type++)
        {
            progressRatioBetweenBeatTypes.Add(new List<float>());
            noOfEachNoteType.Add(_tuneManager.tunes[0].tuneTypeBeats.Count);
            for (int i = 0; i < _tuneManager.tunes[0].tuneTypeBeats[type].Count - 1; i++)
            {
                progressRatioBetweenBeatTypes[type].Add(_tuneManager.tunes[0].tuneTypeBeats[type][i + 1].progressRatioTime - _tuneManager.tunes[0].tuneTypeBeats[type][i].progressRatioTime);
            }
        }
        averageDurBetweenNotes = GetAverage(progressRatioBetweenBeats);
        sDDurBetweenNotes = GetSD(progressRatioBetweenBeats, averageDurBetweenNotes);
        averageDurBetweenNoteTypes = new List<float>();
        sDDurBetweenNoteTypes = new List<float>();

        for (int type = 0; type < _tuneManager.tunes[0].tuneTypeBeats.Count; type++)
        {
            averageDurBetweenNoteTypes.Add(GetAverage(progressRatioBetweenBeatTypes[type]));
            sDDurBetweenNoteTypes.Add(GetSD(progressRatioBetweenBeatTypes[type], averageDurBetweenNoteTypes[type]));
        }
        scoreForNoteType = new List<float>();
        float _overallTuneScoreMultiplier = Mathf.Min(1 / (1.1f - sDDurBetweenNotes), 1.5f) * Mathf.Max(1 / (0.8f + averageDurBetweenNotes), 0.9f) * Mathf.Pow(Mathf.Lerp(0, 1, averageDurBetweenNotes / _softMinDurLowerLimit), 2);//_softMinLowerLimit has o be between 0 - 1;
        for (int type = 0; type < _tuneManager.tunes[0].tuneTypeBeats.Count; type++)
        {
            float _typeMultiplier = Mathf.Max(1 - 2.5f * sDDurBetweenNoteTypes[type], 0.6f) * Mathf.Pow(Mathf.Lerp(0, 1, averageDurBetweenNoteTypes[type] / _softMinDurLowerLimit), 2);
            float _typeScore = (-0.25f * noOfEachNoteType[type]) + 0.5f * _typeCountTurnPoint + 2 / noOfEachNoteType[type]; //total type score rise till turning point

            scoreForNoteType.Add(Mathf.Pow(0.5f + 0.7f *_typeScore * _typeMultiplier + 0.3f * _typeScore * _overallTuneScoreMultiplier,2));
        }//Magic numbers galore. go check desmos

        float _maxTotScore = 0;
        string _perTypeScore = "Type Scores | ";
        for (int type = 0; type < _tuneManager.tunes[0].tuneTypeBeats.Count; type++)
        {
            if (scoreForNoteType[type] < 0) scoreForNoteType[type] = 1;
            _maxTotScore += scoreForNoteType[type] * noOfEachNoteType[type];
            _perTypeScore += scoreForNoteType[type] + " | ";
            AudioManager.audioClipsInfo[type].baseDamage = scoreForNoteType[type];
        }
        Debug.Log("Max Score : " + _maxTotScore);
        Debug.Log(_perTypeScore);
    }

    public static float GetAverage(List<float> _numbers)
    {
        float _total = 0;
        for (int i = 0; i < _numbers.Count; i++)
        {
            _total += _numbers[i];
        }
        return _total / (_numbers.Count);
    }

    public static float GetSD(List<float> _numbers, float _average)
    {
        float _total = 0;
        for (int i = 0; i < _numbers.Count; i++)
        {
            _total += _numbers[i] * _numbers[i];
        }
        return Mathf.Sqrt(_total / (_numbers.Count) - (_average* _average));
    }

    /*
    public static int GetDamageValue(int _type, float _acc)
    {
        Debug.Log(scoreForNoteType[_type]);
        int dmg = Mathf.CeilToInt(scoreForNoteType[_type] * _acc);

        currentScore += dmg;
        scoreText.text = "Tot. Dmg: " + currentScore;
        return dmg;
    }
    */
}
