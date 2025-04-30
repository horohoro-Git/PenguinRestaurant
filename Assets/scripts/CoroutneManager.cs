using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
//한글

public class CoroutneManager : MonoBehaviour
{
    private void Awake()
    {
        GameInstance.GameIns.coroutneManager = this;
    }

    static Dictionary<Coroutine, IEnumerator> coroutineDic = new Dictionary<Coroutine, IEnumerator>();
    //static Stack<Coroutine> coroutines = new Stack<Dictionary< Coroutine, IEnumerator>>();  
    public void StartCoroutines(IEnumerator numerator, ref Coroutine coroutine)
    {
        Coroutine co = StartCoroutine(numerator);
   
        coroutine = co;
  //      coroutineDic.Add(coroutine, numerator);
        Debug.Log("Corouine Count " +  coroutineDic.Count + " name " + numerator.ToString()); 
   //     coroutineDic.Add(numerator, coroutine);
   //     coroutines.Push({ numerator, coroutine} );
    }
    public void StopCoroutines(ref Coroutine coroutine)
    {
        if (coroutine != null)
        {
            Debug.Log("Corouine Count " + coroutineDic.Count + " name " + coroutine.ToString());
            //coroutineDic.Remove(coroutine);
            StopCoroutine(coroutine);
       //     coroutine = null;
       
        }
        else
        {
            Debug.Log("Coroutine NULL");
        }
    }

    public int GetCoroutines()
    {
        return coroutineDic.Count;
    }
}
