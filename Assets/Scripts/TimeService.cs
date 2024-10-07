using DG.Tweening;
using Newtonsoft.Json;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class TimeService : MonoBehaviour
{   

    public DateTime InitialTime;

    private const float OFFSET = 90f;
    private const string TIME_API_URL = "https://www.timeapi.io/api/Time/current/zone?timeZone=Europe/Moscow";    
    private const string OPEN_TIME_API_URL = "";

    [Header("ClockHand")]
    [SerializeField] private Transform _hourHand;
    [SerializeField] private Transform _minuteHand;
    [SerializeField] private Transform _secondHand;

    [Header("UI")]
    [SerializeField] private TMP_Text _timeText;
  
    private bool _isValidTime;

    private void Start()
    {       
        StartCoroutine(RequestTimeCoroutine());
    }

    private IEnumerator RequestTimeCoroutine()
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(TIME_API_URL))
        {

            yield return webRequest.SendWebRequest();
            Debug.Log(webRequest.downloadHandler.text);

            if (webRequest.isNetworkError || webRequest.isHttpError)
            {
                Debug.LogError("Ошибка: " + webRequest.error);
            }
            else
            {
                string response = webRequest.downloadHandler.text;
                if (response.Contains("rate limit exceeded") || response.Contains("authorization required"))
                {
                    Debug.LogError("API access restriction: " + response);
                }
                else
                {
                    string worldTimeApiResponse = webRequest.downloadHandler.text;
                    TimeAPI worldTimeApiData = JsonUtility.FromJson<TimeAPI>(worldTimeApiResponse);

                    if (worldTimeApiData.dateTime != null)
                    {
                        _isValidTime = true;
                        InitialTime = DateTime.Parse(worldTimeApiData.dateTime);                       
                        UpdateTime(InitialTime);
                        StartCoroutine(UpdateClockHandsCoroutine());
                    }
                    else
                    {
                        _isValidTime = false;
                    }                   
                }
            }
        }

        // Если время не корректно, запрашиваем время от OpenTimeAPI
        //if (!isValidTime)
        //{
        //    //using (UnityWebRequest webRequest = UnityWebRequest.Get(OPEN_TIME_API_URL))
        //    //{
        //    UnityWebRequest webRequestTwo = UnityWebRequest.Get(OPEN_TIME_API_URL);
        //    yield return webRequestTwo.SendWebRequest();

        //    string openTimeApiResponse = webRequestTwo.downloadHandler.text;
        //    OpenTimeApiResponse openTimeApiData = JsonUtility.FromJson<OpenTimeApiResponse>(openTimeApiResponse);
        //    
        //    if (openTimeApiData != null && openTimeApiData.time != null)
        //    {
        //        isValidTime = true;
        //        UpdateTime(openTimeApiData.time);
        //    }
        //    else
        //    {
        //        isValidTime = false;
        //    }

        //}
        //}

        // Если время корректно, обновляем время каждые 60 минут
        if (_isValidTime)
        {
            InvokeRepeating("RequestTimeCoroutine", 60 * 60, 60 * 60);
        }

        yield break;
    }

    private void UpdateTime(DateTime time)
    {        
        _timeText.text = time.ToString("HH:mm:ss");

        InitialTime = time;
       
        float hourAngle = (time.Hour % 12 + time.Minute / 60f) * 30f - OFFSET;
        float minuteAngle = time.Minute * 6f - OFFSET;
        float secondAngle = time.Second * 6f - OFFSET;

        _hourHand.DOLocalRotateQuaternion(Quaternion.Euler(0, 0, -hourAngle), 1f);
        _minuteHand.DOLocalRotateQuaternion(Quaternion.Euler(0, 0, -minuteAngle), 1f);
        _secondHand.DOLocalRotateQuaternion(Quaternion.Euler(0, 0, -secondAngle), 1f);
    }

    private IEnumerator UpdateClockHandsCoroutine()
    {
        DateTime currentTime = InitialTime;
        while (true)
        {
            UpdateTime(currentTime);
            currentTime = currentTime.AddSeconds(1);
            yield return new WaitForSeconds(1f); 
        }
    }
}


