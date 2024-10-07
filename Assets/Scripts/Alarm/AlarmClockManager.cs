using System;
using UnityEngine;
using DG.Tweening;

public class AlarmClockManager : MonoBehaviour
{
    [SerializeField] private TimeService _timeService;

    private (int, int, int) _tuple;
    private AlarmPanel _alarmPanel;
    private DateTime _alarmTime;
    private bool _isDragging;
    private bool isAlarmSet;

    private void Start()
    {
        _alarmPanel = gameObject.GetComponent<AlarmPanel>();
        _alarmPanel.AlarmButton.onClick.AddListener(SetAlarm);
        _alarmPanel.AlarmConfirmButton.onClick.AddListener(ConfirmAlarm);
    }

    private void Update()
    {
        // Проверяем, достигнуто ли время будильника
        if (isAlarmSet && _timeService.InitialTime >= _alarmTime)
        {
            TriggerAlarm();
            isAlarmSet = false;
        }
    }

    private void FixedUpdate()
    {
        if (isAlarmSet && _timeService.InitialTime <= _alarmTime)
        {
            _alarmPanel.AlarmTimeText.gameObject.SetActive(true);
            TimeSpan remainingTime = _alarmTime - _timeService.InitialTime;
            _alarmPanel.AlarmTimeText.text = remainingTime.ToString(@"hh\:mm\:ss");
        }
    }

    private void SetAlarm()
    {
        _alarmPanel.AlarmWindow.transform.localScale = new Vector3(0, 0, 0);
        _alarmPanel.AlarmWindow.SetActive(true);
        _alarmPanel.AlarmButton.gameObject.SetActive(false);
        _alarmPanel.AlarmTimeText.gameObject.SetActive(false);
        Tween tween = _alarmPanel.AlarmWindow.transform.DOScale(new Vector3(1, 1, 1), 0.5f);
        tween.SetEase(Ease.OutCubic);
    }

    private void ConfirmAlarm()
    {      
        string inputTime = _alarmPanel.AlarmTimeInput.text;

        if (string.IsNullOrWhiteSpace(inputTime))
        {
            // Если поле пустое, получаем время с циферблата
            _tuple = GetTimeFromClockHands();
            inputTime = $"{_tuple.Item1}:{_tuple.Item2}:{_tuple.Item3}";
        }
        else
        {            
            if (ParseInputTime(inputTime, out _tuple))
            {
                Debug.Log($"Введенное время: {_tuple.Item1}:{_tuple.Item2}:{_tuple.Item3}");
            }
            else
            {
                Debug.LogError("Некорректный формат времени. Пожалуйста, введите время в формате Час:Минуты:Секунды.");
                return;
            }
        }       
       
        if (ValidateTime(out _alarmTime))
        {
            isAlarmSet = true;
            Debug.Log($"Будильник поставили на: {_alarmTime}");
        }
        else
        {
            Debug.LogError("Некорректный формат времени. Пожалуйста, введите время в формате Час:Минуты:Секунды.");
        }
        
        Tween tween = _alarmPanel.AlarmWindow.transform.DOScale(new Vector3(0, 0, 0), 0.5f);
        tween.SetEase(Ease.OutCubic);
        tween.OnComplete(() => _alarmPanel.AlarmWindow.SetActive(false));
    }

    private bool ValidateTime(out DateTime time)
    {
        try
        {
            time = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, _tuple.Item1, _tuple.Item2, _tuple.Item3);
            return true;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Ошибка валидации времени: {ex}");
            time = default;
            return false;
        }
    }

    private bool ParseInputTime(string inputTime, out (int, int, int) timeTuple)
    {
        timeTuple = (0, 0, 0); 
       
        string[] timeParts = inputTime.Split(':');
        if (timeParts.Length != 3)
        {
            return false; 
        }
        
        if (int.TryParse(timeParts[0], out int hours) &&
            int.TryParse(timeParts[1], out int minutes) &&
            int.TryParse(timeParts[2], out int seconds))
        {            
            timeTuple = (hours, minutes, seconds);
            return true;
        }

        return false;
    }

    private (int, int, int) GetTimeFromClockHands()
    {
        float hourAngle = _alarmPanel.HourHand.eulerAngles.z - 90f;
        float minuteAngle = _alarmPanel.MinuteHand.eulerAngles.z - 90f;
        float secondAngle = _alarmPanel.SecondHand.eulerAngles.z - 90f;

        //// Корректируем углы для обработки
        hourAngle = (hourAngle + 360) % 360;
        minuteAngle = (minuteAngle + 360) % 360;
        secondAngle = (secondAngle + 360) % 360;
              
        int hours = (int)(((360 - hourAngle) / 30) % 12); // 30 градусов на каждый час
        int minutes = (int)((360 - minuteAngle) / 6);   // 6 градусов на каждую минуту
        int seconds = (int)((360 - secondAngle) / 6);   // 6 градусов на каждую секунду
      
        // Если hours равен 0 и текущее время после полудня, устанавливаем на 12
        if (hours == 0 && _timeService.InitialTime.Hour >= 12)
        {
            hours = 12;
        }

        // Если hours равен 0 и текущее время до полудня, оставляем как 0
        else if (hours == 0)
        {
            hours = 12; // Установим на 12, если нужно, но в реальности часы должны быть 0
        }

        // Если время после полудня, добавляем 12 к часам
        if (_timeService.InitialTime.Hour >= 12 && hours < 12)
        {
            hours += 12;
        }
        
        if (minutes >= 60)
        {
            minutes = 0; 
        }

        if (seconds >= 60)
        {
            seconds = 0; 
        }      

        return (hours, minutes, seconds);        
    }

    private void TriggerAlarm()
    {
        _alarmPanel.AlarmTimeText.text = "Будильник сработал";
        _alarmPanel.AlarmButton.gameObject.SetActive(true);
    }
}

