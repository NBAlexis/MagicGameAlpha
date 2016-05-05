using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using System.IO;

public class EditorCommonFunctions
{
    public static Mesh LoadMeshAtPathWithName(string sPath, string name)
    {
        Object[] data = AssetDatabase.LoadAllAssetsAtPath(sPath);
        foreach (Object obj in data)
        {
            if (obj is Mesh && obj.name.Equals(name))
            {
                return (Mesh)obj;
            }
        }
        return null;
    }

    public static object GetReadWritable(object obj)
    {
        Texture2D tx = obj as Texture2D;
        if (null != tx)
        {
            string texturePath = AssetDatabase.GetAssetPath(tx);
            string sFullPath = "file:///" + Application.dataPath + texturePath.Substring(6, texturePath.Length - 6);
            WWW www = new WWW(sFullPath);
            Texture2D t1 = AssetDatabase.LoadAssetAtPath(texturePath, typeof(Texture2D)) as Texture2D;
            Debug.Assert(t1 != null, "t1 != null");
            Texture2D duplic = new Texture2D(t1.width, t1.height, t1.format, t1.mipmapCount > 0);
            www.LoadImageIntoTexture(duplic);
            return duplic;
        }
        Mesh mesh = obj as Mesh;
        if (null != mesh)
        {
            return LoadMeshAtPathWithName(AssetDatabase.GetAssetPath(mesh), mesh.name);
        }
        return null;
    }

    public static Dictionary<string, Rect> PackTextureInDataBase(Texture2D[] smallTextures, string sFileName, Shader shader, int iForceTransparent = 0)
    {
        Dictionary<string, Texture2D> res = new Dictionary<string, Texture2D>();
        foreach (Texture2D texture in smallTextures)
        {
            string texturePath = AssetDatabase.GetAssetPath(texture);
            if (!res.ContainsKey(texturePath))
            {
                string sFullPath = "file:///" + Application.dataPath + texturePath.Substring(6, texturePath.Length - 6);
                WWW www = new WWW(sFullPath);
                Texture2D t1 = AssetDatabase.LoadAssetAtPath(texturePath, typeof(Texture2D)) as Texture2D;
                Debug.Assert(t1 != null, "t1 != null");
                Texture2D duplic = new Texture2D(t1.width, t1.height, t1.format, t1.mipmapCount > 0);
                www.LoadImageIntoTexture(duplic);
                res.Add(texturePath, duplic);
            }
        }

        return PackTexture(res, sFileName, shader, iForceTransparent);
    }

    public static Dictionary<string, Rect> PackTexture(Dictionary<string, Texture2D> smallTextures, string sFileName, Shader shader, int iForceTransparent = 0)
    {
        Dictionary<string, Rect> res = new Dictionary<string, Rect>();
        List<Texture2D> textureList = new List<Texture2D>();
        List<string> textureIdList = new List<string>();

        foreach (string stexture in smallTextures.Keys)
        {
            textureIdList.Add(stexture);
            textureList.Add(smallTextures[stexture]);
        }
        Texture2D bigTexture = new Texture2D(2048, 2048);

        Rect[] rects = bigTexture.PackTextures(textureList.ToArray(), 0, 2048);
        for (int i = 0; i < rects.Length; ++i)
        {
            res.Add(textureIdList[i], rects[i]);
        }
        bigTexture.Apply(true);

        if (bigTexture.format != TextureFormat.ARGB32 && bigTexture.format != TextureFormat.RGB24)
        {
            Texture2D newTexture = new Texture2D(bigTexture.width, bigTexture.height);
            newTexture.SetPixels(bigTexture.GetPixels(0), 0);
            bigTexture = newTexture;
        }

        if (1 == iForceTransparent || (shader.renderQueue > 2400 && 2 != iForceTransparent))
        {
            byte[] pngData = bigTexture.EncodeToPNG();
            if (pngData != null)
            {
                File.WriteAllBytes(Application.dataPath + "/" + sFileName + ".png", pngData);
                AssetDatabase.Refresh();

                Material newMat = new Material(shader);
                newMat.mainTexture = AssetDatabase.LoadAssetAtPath("Assets/" + sFileName + ".png", typeof(Texture2D)) as Texture2D;
                AssetDatabase.CreateAsset(newMat, "Assets/" + sFileName + ".mat");
            }
        }
        else
        {
            byte[] pngData = bigTexture.EncodeToJPG();
            if (pngData != null)
            {
                File.WriteAllBytes(Application.dataPath + "/" + sFileName + ".jpg", pngData);
                AssetDatabase.Refresh();

                Material newMat = new Material(shader);
                newMat.mainTexture = AssetDatabase.LoadAssetAtPath("Assets/" + sFileName + ".jpg", typeof(Texture2D)) as Texture2D;
                AssetDatabase.CreateAsset(newMat, "Assets/" + sFileName + ".mat");
            }
        }
        AssetDatabase.Refresh();
        return res;
    }
}
