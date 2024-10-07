using UnityEngine;
using UnityEngine.UI;

public class AdaptiveLayout : MonoBehaviour
{
    [SerializeField] GridLayoutGroup _gridLayoutGroup;  

    private void Start()
    {        
        _gridLayoutGroup.constraintCount = 3;
        _gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        _gridLayoutGroup.cellSize = new Vector2(400, 400);
        _gridLayoutGroup.spacing = new Vector2(10, 60);
        
        AdaptUI();
    }

    private void OnRectTransformDimensionsChange()
    {       
        AdaptUI();
    }

    private void AdaptUI()
    {               
        if (Screen.width > Screen.height)
        {
            // Landscape
            _gridLayoutGroup.constraintCount = 3;
            _gridLayoutGroup.childAlignment = TextAnchor.MiddleCenter;
            _gridLayoutGroup.startCorner = GridLayoutGroup.Corner.UpperRight;
        }
        else
        {
            // Portrait
            _gridLayoutGroup.constraintCount = 1;
            _gridLayoutGroup.childAlignment = TextAnchor.UpperLeft;
            _gridLayoutGroup.startCorner = GridLayoutGroup.Corner.UpperRight;
        }
    }
}
