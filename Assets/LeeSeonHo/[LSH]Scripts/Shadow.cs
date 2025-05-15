#if HAS_DOTWEEN
using DG.Tweening;
#endif
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shadow : MonoBehaviour
{
    public Image shadowImage;
    public Transform model;
    public RectTransform rectTransform;
  //  GameInstance gameInstance = new GameInstance();
    // Start is called before the first frame update
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }
    // Update is called once per frame
    void Update()
    {
       
        float f = 1 - this.transform.position.y / 5;
        if (f < 0) f = 0;
        Vector3 newSize = GameInstance.GetVector3(f, f, f);
        rectTransform.localScale = newSize;// new Vector3(f, f, f);

        if(model !=null)
        {
            Vector3 rectPosition = GameInstance.GetVector3(model.position.x, 0.1f + model.position.y, model.position.z);
            Vector3 rectEuler = GameInstance.GetVector3(90, 0, -model.rotation.eulerAngles.y);

            rectTransform.position = rectPosition;//new Vector3(model.position.x, 0.1f + model.position.y, model.position.z);
            rectTransform.rotation = Quaternion.Euler(rectEuler);//Quaternion.Euler(new Vector3(90,0, -model.rotation.eulerAngles.y));
            //rectTransform.localScale =  
        }
     //   if (model == null) Debug.Log(GetComponentInParent<Employee>().gameObject.name);
       // transform.rotation = model.rotation;
    }

    public void Setup()
    {
        shadowImage.sprite = AssetLoader.loadedAtlases["Town"].GetSprite("shadow_circle"); //AssetLoader.atlasSprites["shadow_circle"];
    }
    public void SetSize(Vector3 size, float offset_x, float offset_z)
    {
        Debug.Log(size);
    }
}
