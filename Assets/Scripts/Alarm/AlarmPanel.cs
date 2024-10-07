using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AlarmPanel : MonoBehaviour
{
    public Button AlarmButton;
    public Button AlarmConfirmButton;
    public TMP_InputField AlarmTimeInput;
    public TMP_Text AlarmTimeText;
    public GameObject AlarmWindow;
    public Transform HourHand;
    public Transform MinuteHand;
    public Transform SecondHand;
}
