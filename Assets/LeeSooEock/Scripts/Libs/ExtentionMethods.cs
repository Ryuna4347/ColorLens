using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtentionMethod
{
    static public void SetParentTm( this Transform self, Transform parent )
    {
        self.SetParent( parent );
        self.localPosition = Vector3.zero;
        self.localScale = Vector3.one;
        self.localRotation = Quaternion.identity;
    }
}
