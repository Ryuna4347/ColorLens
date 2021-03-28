using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectChapter : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (IsClearManager.instance.GetStar((int.Parse(gameObject.name) - 1) * 12) > 0)
        {
            GetComponent<Button>().interactable = true;
            transform.Find("Lock").gameObject.SetActive(false);
        }
        else
        {
            return;
        }
    }
}
