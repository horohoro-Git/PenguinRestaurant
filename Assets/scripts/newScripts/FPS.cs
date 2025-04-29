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
       //   Font font = Resources.GetBuiltinResource<Font>("Arial.ttf");
   //     this.style.font = font;
        //UnityEngine.Profiling.Profiler.logFile = "mylog.log";
    //    UnityEngine.Profiling.Profiler.enabled = true;
    }
  //  private float lastGCAlloc = 0f;
 //   private float lastFrameTime = 0f;

    WaitForSeconds tenSecnonds = new WaitForSeconds(10);
    private void Start()
    {
       // Resources.UnloadUnusedAssets();
        // AssetBundle.UnloadAllAssetBundles(true);
        //Resources.UnloadAll();
        //  StartCoroutine(ResetLowFPS());
    }

    IEnumerator ResetLowFPS()
    {

        while (true)
        {
            lowFPS = 0.0f;
            yield return tenSecnonds;
        }
    }
    float lastRecordedTime = 0f;
 //   float sampleInterval = 1f;
    void Update()
    {
        this.deltaTime += (Time.deltaTime - this.deltaTime) * 0.1f;
        /*  this.deltaTime += (Time.deltaTime - this.deltaTime) * 0.1f;

          this.totalReservedMemory = UnityEngine.Profiling.Profiler.GetTotalReservedMemoryLong();
          this.totalAllocMemory = UnityEngine.Profiling.Profiler.GetTotalAllocatedMemoryLong();
          this.totalUnusedReservedMemory = UnityEngine.Profiling.Profiler.GetTotalUnusedReservedMemoryLong();*/

        // 매 프레임마다 메모리 상태 확인 (로그 출력)
        /*Debug.Log($"Allocated Memory: {Profiler.GetTotalAllocatedMemoryLong() / 1024f / 1024f} MB");
        Debug.Log($"Reserved Memory: {Profiler.GetTotalReservedMemoryLong() / 1024f / 1024f} MB");*/
        lastRecordedTime += Time.deltaTime;
        if (Application.isPlaying && lastRecordedTime > 1)
        {
         //   Resources.UnloadUnusedAssets();
            lastRecordedTime = 0;
            // 실제 게임 실행 중에만 할당된 메모리 추적
         //   Debug.Log($"Allocated Memory: {Profiler.GetTotalAllocatedMemoryLong() / 1024f / 1024f} MB");
           // Debug.Log($"Allocated Memory: {Profiler.GetTotalAllocatedMemoryLong() / 1024f / 1024f} MB");
         //   Debug.Log($"Reserved Memory (Play Mode): {Profiler.GetTotalReservedMemoryLong() / 1024f / 1024f} MB");
        }
    }


 
    private void ShowFPS()
    {
     //   Debug.Log(deltaTime)
        float msec = deltaTime * 1000.0f;
        float fps = 1.0f / deltaTime;

        // 텍스트 박스 설정
        Rect rect = new Rect(400, 0, width, height * 2 / 100);
        this.style.alignment = TextAnchor.UpperLeft;
        this.style.fontSize = this.height * 4 / 100;
        this.style.normal.textColor = Color.yellow;
        // 텍스트 생성
        string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);

        // 텍스트 표시
        GUI.Label(rect, text, style);
        /*   Rect rect = new Rect(400, 0, this.width, this.height * 2 / 100);
           this.style.alignment = TextAnchor.UpperLeft;
           this.style.fontSize = this.height * 4 / 100;
           this.style.normal.textColor = Color.yellow;
           float msec = this.deltaTime * 1000.0f;
           float fps = 1.0f / this.deltaTime;
           if (lowFPS == 0 || lowFPS > fps)
           {
               lowFPS = fps;
           }

           // stringBuilder.Clear();  // 이전 내용 초기화
           //  stringBuilder.AppendFormat("{0:0.0} ms ({1:0.} fps)", msec, fps);
           string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
           GUI.Label(rect, text, this.style);*/
        //  Rect rect2 = new Rect(400, 500, this.width, this.height * 2 / 100);
        //this.style.alignment = TextAnchor.UpperLeft;
        //  this.style.fontSize = this.height * 4 / 100;
        //  this.style.normal.textColor = Color.yellow;
        //  string text2 = string.Format("LowFPS {0:0.0}", lowFPS);
        //  GUI.Label(rect2, text2, this.style);
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
 
    /*    this.ShowReservedMemory();
 
        this.ShowAllocMemory();
 
        this.ShowUnusedReservedMemory();

        this.ShowCameraOrthoritySize();*//**/
    }
 
    private double ConvertBytesToMegabytes(long bytes)
    {
        return (bytes / 1024f) / 1024f;
    }
 
//#endif
}