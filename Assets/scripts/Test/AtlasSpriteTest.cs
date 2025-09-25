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

        // SpriteAtlas�� �ִ� ù ��° Sprite ��������
        Sprite[] sprites = new Sprite[atlas.spriteCount];
        atlas.GetSprites(sprites);

        if (sprites.Length == 0)
        {
            Debug.LogError("No sprites found in the atlas.");
            return;
        }

        Texture2D texture = sprites[0].texture;

        // �ؽ�ó �б� �����ϰ� ����� (����)
        RenderTexture rt = RenderTexture.GetTemporary(texture.width, texture.height);
        Graphics.Blit(texture, rt);

        RenderTexture.active = rt;
        Texture2D readableTex = new Texture2D(texture.width, texture.height, TextureFormat.RGBA32, false);
        readableTex.ReadPixels(new Rect(0, 0, texture.width, texture.height), 0, 0);
        readableTex.Apply();
        RenderTexture.active = null;
        RenderTexture.ReleaseTemporary(rt);

        // PNG�� ����
        byte[] bytes = readableTex.EncodeToPNG();
        string path = Application.persistentDataPath + "/ExportedAtlas.png";
        File.WriteAllBytes(path, bytes);
        Debug.Log("Atlas exported to: " + path);
    }
}
