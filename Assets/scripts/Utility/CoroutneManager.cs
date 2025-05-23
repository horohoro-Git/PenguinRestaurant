using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
//한글

public class CoroutneManager
{
    public static WaitForSeconds waitForzerofive = new WaitForSeconds(0.5f);
    public static WaitForSeconds waitForzeroone = new WaitForSeconds(0.1f);
    public static WaitForSeconds waitForzerothree = new WaitForSeconds(0.3f);
    public static WaitForSeconds waitFortwo = new WaitForSeconds(2f);
    public static WaitForSeconds waitForone = new WaitForSeconds(1f);
    public static WaitForSeconds waitForfive = new WaitForSeconds(5f);
    public static WaitForSecondsRealtime waitForfive_real = new WaitForSecondsRealtime(5f);


    /* private void Awake()
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
     }*/
}
