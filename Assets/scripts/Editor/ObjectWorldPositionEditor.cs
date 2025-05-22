using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ObjectWorldPositionEditor : EditorWindow
{
    GameObject selectedObject;
    Vector3[] corners = new Vector3[4];
    int xVal;
    int yVal;
    int zVal;
    int xVal2;
    int yVal2;
    int zVal2;
    [MenuItem("Tools/Get Object's World Position")]
    public static void ShowWindow()
    {
        GetWindow<ObjectWorldPositionEditor>("World Position");
    }
    private void OnGUI()
    {
        GUILayout.Label("Select Object for View Min/Max WorldPosition");

        selectedObject = (GameObject)EditorGUILayout.ObjectField("Object", selectedObject, typeof(GameObject), true);

        if (GUILayout.Button("Show Positions"))
        {
            if (selectedObject != null)
            {
                ShowPositions();

            }
            else
            {
                Debug.LogWarning("Select HireArchy's Object");
            }
        }

        if (selectedObject != null)
        {
            for (int i = 0; i < 4; i++)
            {
                GUILayout.Label($"Corner {i + 1} : {corners[i]}");
            }

            GUILayout.Label($"X : {xVal}, Y : {yVal}, type : {zVal}");

            GUILayout.Label($"Floor X : {xVal2}, Y : {yVal2}, type : {zVal2}");
        }


    }

    void ShowPositions()
    {
      //  MeshRenderer meshRenderer = selectedObject.GetComponent<MeshRenderer>();
        Collider collider = selectedObject.GetComponentInChildren<Collider>();

        if (collider != null)
        {
            
            Bounds bounds = collider.bounds;

            Vector3 center = collider.bounds.center;

            Vector3 extents = collider.bounds.extents;


            corners[0] = new Vector3(center.x - extents.x, center.y, center.z - extents.z);
            corners[1] = new Vector3(center.x - extents.x, center.y, center.z + extents.z);
            corners[2] = new Vector3(center.x + extents.x, center.y, center.z + extents.z);
            corners[3] = new Vector3(center.x + extents.x, center.y, center.z - extents.z);

            /*
                        Vector3[] localCorners = new Vector3[4];
                        localCorners[0] = new Vector3(-extents.x, 0, -extents.z);
                        localCorners[1] = new Vector3(extents.x, 0, -extents.z); 
                        localCorners[2] = new Vector3(-extents.x, 0, extents.z);
                        localCorners[3] = new Vector3(extents.x, 0, extents.z); 

                        // 각 끝점을 회전된 방향에 맞게 변환 (회전 적용)
                        corners = new Vector3[4];
                        for (int i = 0; i < 4; i++)
                        {
                            // 회전 적용
                            corners[i] = selectedObject.transform.TransformPoint(localCorners[i]);
                            Debug.Log($"Corner {i + 1}: {corners[i]}");
                        }*/
            /* Vector3[] localCorners = new Vector3[4];
             localCorners[0] = new Vector3(-extents.x, 0, -extents.z);  // 좌측 하단
             localCorners[1] = new Vector3(extents.x, 0, -extents.z);   // 우측 하단
             localCorners[2] = new Vector3(-extents.x, 0, extents.z);   // 좌측 상단
             localCorners[3] = new Vector3(extents.x, 0, extents.z);    // 우측 상단

             // 객체의 회전된 방향에 맞게 각 코너들을 변환
             corners = new Vector3[4];
             for (int i = 0; i < 4; i++)
             {
                 // 각 코너를 월드 좌표로 변환
                 corners[i] = selectedObject.transform.TransformPoint(localCorners[i]);
                 Debug.Log($"Corner {i + 1}: {corners[i]}");
             }*/

            // OBB 시각화 (기본적으로 4개의 코너만 시각화)
            for (int i = 0; i < 4; i++)
            {
                // 인접한 코너들 간의 선을 그려서 OBB의 경계를 시각화
                Vector3 start = corners[i];
                Vector3 end = corners[(i + 1) % 4];  // 마지막 코너는 첫 번째 코너와 연결
                Debug.DrawLine(start, end, Color.red, 10);  // 선 색은 빨간색으로 설정
            }
        }
        else
        {
            Debug.LogWarning("Selected object does not have a MeshRenderer component!");
        }


        if (selectedObject)
        {
            Vector3 pos = selectedObject.transform.position;
            int minx = -50 / 2;
            int miny = -50 / 2;



            zVal = selectedObject.transform.rotation.z < 0 ? 0 : 1;
            if (zVal == 0)
            {
                float xx = pos.x % 1;
                if (xx > 0.5f)
                {
                    xVal = (int)(pos.x) / 2 - minx + 1;
                    yVal = (int)(pos.z) / 2 - miny;
                }
                else
                {
                    xVal = (int)(pos.x) / 2 - minx;
                    yVal = (int)(pos.z) / 2 - miny;
                }

            }
            else
            {
                float yy = pos.z % 1;
                if (yy > 0.5f)
                {
                    xVal = (int)(pos.x) / 2 - minx;
                    yVal = (int)(pos.z) / 2 - miny + 1;
                }
                else
                {
                    xVal = (int)(pos.x) / 2 - minx;
                    yVal = (int)(pos.z) / 2 - miny;
                }
            }
            //  GUILayout.Label($"Corner {i + 1} : {corners[i]}");
        }


        if (selectedObject)
        {
            Vector3 pos = selectedObject.transform.position;
            int minx = -50 / 2;
            int miny = -50 / 2;
            xVal2 = (int)(pos.x - 1) / 2 - minx;
            yVal2 = (int)(pos.z - 1) / 2 - miny;
            zVal2 = 0;

        }
    }
}
