using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Text;
using UnityEngine.Profiling;
//한글

public class FPS : MonoBehaviour
{
//#if USE_DEBUG
    private float deltaTime = 0.0f;
    private GUIStyle style;
    private int width;
    private int height;
    private long totalReservedMemory;
    private long totalAllocMemory;
    private long totalUnusedReservedMemory;
    public static double totalSize;
    float lowFPS;
  //  StringBuilder stringBuilder = new StringBuilder();
    void Awake()
    {
        this.width = Screen.width;
        this.height = Screen.height;
        this.style = new GUIStyle();
     
    }
  //  private float lastGCAlloc = 0f;
 //   private float lastFrameTime = 0f;

    WaitForSeconds tenSecnonds = new WaitForSeconds(10);

    float lastRecordedTime = 0f;
 //   float sampleInterval = 1f;
    void Update()
    {
        this.deltaTime += (Time.deltaTime - this.deltaTime) * 0.1f;
      
        lastRecordedTime += Time.deltaTime;
        if (Application.isPlaying && lastRecordedTime > 1)
        {
            lastRecordedTime = 0;
           
        }
    }


 
    private void ShowFPS()
    {
     //   Debug.Log(deltaTime)
        float msec = deltaTime * 1000.0f;
        float fps = 1.0f / deltaTime;

        // 텍스트 박스 설정
        Rect rect = new Rect(400, 0, width, height * 2 / 100);
        Rect rect2 = new Rect(400, 200, width, height * 2 / 100);
        this.style.alignment = TextAnchor.UpperLeft;
        this.style.fontSize = this.height * 4 / 100;
        this.style.normal.textColor = Color.yellow;

        // 텍스트 생성
        string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
        string memoryText = string.Format(
         "Memory: {0:0.00} MB\nSystem: {1:0.00} MB",
         Profiler.GetTotalAllocatedMemoryLong() / (1024f * 1024f),
         System.GC.GetTotalMemory(false) / (1024f * 1024f)
     );
        // 텍스트 표시
        GUI.Label(rect, text, style);
        GUI.Label(rect2, memoryText, style);
    
    }
 
    private void ShowReservedMemory()
    {
        Rect rect = new Rect(400, 100, this.width, this.height * 2 / 100);
        this.style.alignment = TextAnchor.UpperLeft;
        this.style.fontSize = this.height * 4 / 100;
        this.style.normal.textColor = Color.yellow;
        string text = string.Format("사용가능한 메모리: {0}MB", this.ConvertBytesToMegabytes(this.totalReservedMemory).ToString("N"));
        GUI.Label(rect, text, this.style);
    }
 
    private void ShowAllocMemory()
    {
        Rect rect = new Rect(400, 200, this.width, this.height * 2 / 100);
        this.style.alignment = TextAnchor.UpperLeft;
        this.style.fontSize = this.height * 4 / 100;
        this.style.normal.textColor = Color.yellow;
        string text = string.Format("사용하고있음: {0}MB", this.ConvertBytesToMegabytes(this.totalAllocMemory).ToString("N"));
        GUI.Label(rect, text, this.style);
    }
 
    private void ShowUnusedReservedMemory()
    {
        Rect rect = new Rect(400, 300, this.width, this.height * 2 / 100);
        this.style.alignment = TextAnchor.UpperLeft;
        this.style.fontSize = this.height * 4 / 100;
        this.style.normal.textColor = Color.yellow;
        string text = string.Format("남은량: {0}MB", this.ConvertBytesToMegabytes(this.totalUnusedReservedMemory).ToString("N"));
        GUI.Label(rect, text, this.style);
    }

    private void ShowCameraOrthoritySize()
    {
        Rect rect = new Rect(400, 400, this.width, this.height * 2 / 100);
        this.style.alignment = TextAnchor.UpperLeft;
        this.style.fontSize = this.height * 4 / 100;
        this.style.normal.textColor = Color.yellow;
        string text = string.Format("크기: {0}", totalSize);
        GUI.Label(rect, text, this.style);
    }
 
    void OnGUI()
    {
       
        this.ShowFPS();
 
    }
 
    private double ConvertBytesToMegabytes(long bytes)
    {
        return (bytes / 1024f) / 1024f;
    }
 
//#endif
}