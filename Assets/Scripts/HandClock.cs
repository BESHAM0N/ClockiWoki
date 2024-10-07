using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public class HandClock : MonoBehaviour, IDragHandler
{
    private float _initialAngle;
    private float _sensitivity = 0.1f;
    private float _previousAngle = 0f;
    private float _rotationSpeed = 10f;

    public void OnDrag(PointerEventData eventData)
    {       
        Vector2 touchPosition = eventData.position;
        // Вычисляем угол между центром циферблата и координатами касания
        float targetAngle = Mathf.Atan2(touchPosition.y - transform.position.y, touchPosition.x - transform.position.x) * Mathf.Rad2Deg;
 
        float angleDifference = targetAngle - _previousAngle;
        
        if (angleDifference < 0f)
        {
            angleDifference += 360f;
        }
        
        if (angleDifference > 180f)
        {
            angleDifference -= 360f;
        }

        // Обновляем поворот объекта с помощью линейной интерполяции
        float smoothAngle = Mathf.LerpAngle(_previousAngle, _previousAngle + angleDifference, _rotationSpeed * Time.deltaTime);
        transform.localRotation = Quaternion.Euler(0, 0, smoothAngle);
       
        _previousAngle = smoothAngle;
    }

    private void Start()
    {      
        _previousAngle = transform.localRotation.eulerAngles.z;
    }
}
  



