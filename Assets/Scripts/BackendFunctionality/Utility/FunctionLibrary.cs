using UnityEngine;
using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FunctionLibrary : MonoBehaviour
{
    /// <summary>
    /// Used when some UI scene I'm looking at has some input fields to cycle through.
    /// </summary>
    /// <param name="_selectables"></param>
    /// <returns></returns>
    public static Selectable CycleBetweenSelectables(Selectable[]  _selectables)
    {
        int selected = Array.IndexOf(_selectables, EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>());

        if (selected >= 0)
        {
            int counter = 0;
            while (counter <= _selectables.Length)
            {
                var _check = selected + counter;
                if (_check > _selectables.Length - 1) _check -= _selectables.Length;

                var _inputField = _selectables[_check];

                if (_inputField.interactable && _inputField.isActiveAndEnabled && _check != selected)
                {
                    return _inputField;
                }
                else
                {
                    counter++;
                }
            }
        }

        return null;
    }
}

