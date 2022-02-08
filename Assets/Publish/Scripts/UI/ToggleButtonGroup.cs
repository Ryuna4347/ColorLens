using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleButtonGroup : MonoBehaviour
{
    [SerializeField] public ToggleButton currToggle;
    [SerializeField] public List<ToggleButton> togglez;
    [SerializeField] public bool allowToggleOff = false;

    public void SetSelected(ToggleButton temp)
    {
        for (int i = 0; i < togglez.Count; i++)
        {
            if (togglez[i] == temp)
            {
                currToggle = temp;
            }
            else
                togglez[i].SetIsOn(false);
        }
    }

    public void DeselectAll()
    {
        currToggle = null;

        for (int i = 0; i < togglez.Count; i++)
        {
            togglez[i].SetIsOn(false);
        }
    }

    public void AddToggle(ToggleButton temp)
    {
        bool wasFound = false;
        for (int i = 0; i < togglez.Count; i++)
        {
            if (togglez[i] == temp)
                wasFound = true;
        }

        if (!wasFound)
            togglez.Add(temp);
    }
}
