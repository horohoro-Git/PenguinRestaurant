using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanguageSetting : Settings
{
    
    public void Open()
    {
        
    }
    IEnumerator OpenAnimation()
    {
     //   animator.Play()
        yield return new WaitForSecondsRealtime(0.2f);
    }
}
