using UnityEngine;
using System.Collections.Generic;

public enum KeyColor
{
    Red,
    Blue,
    Green
}

public class UnlockModel : MonoBehaviour
{
    [Header("Game Settings")]
    [SerializeField] private int totalCells = 36;
    [SerializeField] private int requiredKeys = 3;

    private KeyColor _targetColor;
    private List<KeyColor> _cellColors = new List<KeyColor>();
    private int _keysCollected = 0;

    public KeyColor TargetColor => _targetColor;
    public int KeysCollected => _keysCollected;
    public int RequiredKeys => requiredKeys;
    public List<KeyColor> CellColors => _cellColors;

    public void Initialize()
    {
        SelectRandomTargetColor();
        GenerateCellColors();
        _keysCollected = 0;
    }

    private void SelectRandomTargetColor()
    {
        int randomIndex = Random.Range(0, 3);
        _targetColor = (KeyColor)randomIndex;
        Debug.Log($"Selected target color: {_targetColor}");
    }

    private void GenerateCellColors()
    {
        _cellColors.Clear();

        // Сначала добавляем минимум 3 нужных ключа
        for (int i = 0; i < requiredKeys; i++)
        {
            _cellColors.Add(_targetColor);
        }

        // Заполняем остальные ячейки случайными цветами
        for (int i = requiredKeys; i < totalCells; i++)
        {
            KeyColor randomColor = (KeyColor)Random.Range(0, 3);
            _cellColors.Add(randomColor);
        }

        // Перемешиваем массив для случайного размещения
        for (int i = 0; i < _cellColors.Count; i++)
        {
            KeyColor temp = _cellColors[i];
            int randomIndex = Random.Range(i, _cellColors.Count);
            _cellColors[i] = _cellColors[randomIndex];
            _cellColors[randomIndex] = temp;
        }

        Debug.Log($"Generated {_cellColors.Count} cell colors. Target color {_targetColor} appears {CountTargetKeys()} times.");
    }

    private int CountTargetKeys()
    {
        int count = 0;
        foreach (var color in _cellColors)
        {
            if (color == _targetColor)
                count++;
        }
        return count;
    }

    public bool TryCollectKey(KeyColor keyColor)
    {
        if (keyColor == _targetColor && _keysCollected < requiredKeys)
        {
            _keysCollected++;
            Debug.Log($"Key collected! Progress: {_keysCollected}/{requiredKeys}");
            return true;
        }
        return false;
    }

    public bool IsLockUnlocked()
    {
        return _keysCollected >= requiredKeys;
    }

    public void Reset()
    {
        _keysCollected = 0;
        Initialize();
    }
}
