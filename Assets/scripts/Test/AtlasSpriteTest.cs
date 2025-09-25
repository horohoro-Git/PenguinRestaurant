using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.U2D;

public class AtlasSpriteTest : MonoBehaviour
{
    public SpriteAtlas atlas;

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            ExportAtlas();
        }
    }

    [ContextMenu("Export Atlas To PNG")]
    void ExportAtlas()
    {
        if (atlas == null)
        {
            Debug.LogError("Atlas is not assigned.");
            return;
        }

        // SpriteAtlas에 있는 첫 번째 Sprite 가져오기
        Sprite[] sprites = new Sprite[atlas.spriteCount];
        atlas.GetSprites(sprites);

        if (sprites.Length == 0)
        {
            Debug.LogError("No sprites found in the atlas.");
            return;
        }

        Texture2D texture = sprites[0].texture;

        // 텍스처 읽기 가능하게 만들기 (복사)
        RenderTexture rt = RenderTexture.GetTemporary(texture.width, texture.height);
        Graphics.Blit(texture, rt);

        RenderTexture.active = rt;
        Texture2D readableTex = new Texture2D(texture.width, texture.height, TextureFormat.RGBA32, false);
        readableTex.ReadPixels(new Rect(0, 0, texture.width, texture.height), 0, 0);
        readableTex.Apply();
        RenderTexture.active = null;
        RenderTexture.ReleaseTemporary(rt);

        // PNG로 저장
        byte[] bytes = readableTex.EncodeToPNG();
        string path = Application.persistentDataPath + "/ExportedAtlas.png";
        File.WriteAllBytes(path, bytes);
        Debug.Log("Atlas exported to: " + path);
    }
}
