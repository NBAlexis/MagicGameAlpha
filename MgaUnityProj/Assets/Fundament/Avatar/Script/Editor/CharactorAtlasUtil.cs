using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using Debug = System.Diagnostics.Debug;

//This class is for atlas the charactor
public class CharactorAtlasUtil 
{
    public struct SRendererInfo
    {
        public string m_sTextureName;
        public string m_sMeshFilterName;
        public Shader m_pShader;
        public string m_sObjectPath;

        public Texture2D m_pTexture;
        public Mesh m_pMesh;
        public GameObject m_pObj;
    }

    public static void MakeAtlasReplace(GameObject pObj, string sParentFolder)
    {
        int iProgressFull = pObj.GetComponentsInChildren<MeshRenderer>(true).Length;
        iProgressFull = iProgressFull*3;
        int iProgressNow = 0;

        //========================================
        //Step 1, find all renderes need to atlas
        Dictionary<Shader, List<SRendererInfo>> typedRenderers = new Dictionary<Shader, List<SRendererInfo>>();
        foreach (MeshRenderer renderer in pObj.GetComponentsInChildren<MeshRenderer>(true))
        {
            ++iProgressNow;
            EditorUtility.DisplayProgressBar("正在收集Renderer信息", string.Format("{0}/{1}", iProgressNow, iProgressFull),
                iProgressNow/(float) iProgressFull);
            if (!typedRenderers.ContainsKey(renderer.sharedMaterial.shader))
            {
                typedRenderers.Add(renderer.sharedMaterial.shader, new List<SRendererInfo>());
            }
            typedRenderers[renderer.sharedMaterial.shader].Add(new SRendererInfo
            {
                m_sTextureName = AssetDatabase.GetAssetPath(renderer.sharedMaterial.mainTexture),
                m_sMeshFilterName = AssetDatabase.GetAssetPath(renderer.gameObject.GetComponent<MeshFilter>().sharedMesh),
                m_pShader = renderer.sharedMaterial.shader,
                m_sObjectPath = CommonFunctions.FindFullName(pObj, renderer.gameObject),
                m_pTexture = (Texture2D)renderer.sharedMaterial.mainTexture,
                m_pMesh = renderer.gameObject.GetComponent<MeshFilter>().sharedMesh,
                m_pObj = renderer.gameObject,
            });

            if (null == renderer.sharedMaterial.mainTexture)
            {
                CRuntimeLogger.LogError("有的材质没有贴图: " + CommonFunctions.FindFullName(pObj, renderer.gameObject));
                EditorUtility.ClearProgressBar();
                return;
            }
        }

        CommonFunctions.CreateFolder(Application.dataPath + "/" + sParentFolder + "/Textures/?");
        CommonFunctions.CreateFolder(Application.dataPath + "/" + sParentFolder + "/Meshes/?");
        GameObject copy = Object.Instantiate(pObj, pObj.transform.position + Vector3.left * 3.0f, pObj.transform.rotation) as GameObject;
        Debug.Assert(copy != null, "copy != null");
        copy.name = pObj.name + "_Atlas";

        foreach (KeyValuePair<Shader, List<SRendererInfo>> kvp in typedRenderers)
        {
            //========================================
            //Create Texture and Material
            Dictionary<string, Texture2D> uniqueTextures = new Dictionary<string, Texture2D>();
            foreach (SRendererInfo info in kvp.Value)
            {
                if (!uniqueTextures.ContainsKey(info.m_sTextureName))
                {
                    uniqueTextures.Add(info.m_sTextureName, info.m_pTexture);
                }
            }
            Texture2D[] allTextures = uniqueTextures.Values.ToArray();
            string sTextureFileName = sParentFolder + "/Textures/" + kvp.Key.name.Replace("/", "_").Replace(".", "_");
            Dictionary<string, Rect> textureDic = PackTexture(allTextures,
                sTextureFileName,
                kvp.Key);

            //========================================
            //Create Mesh with new UV
            Dictionary<string, SRendererInfo> uniqueMeshes = new Dictionary<string, SRendererInfo>();
            foreach (SRendererInfo info in kvp.Value)
            {
                string sName = "f_" + CommonFunctions.GetLastName(info.m_sMeshFilterName) + "_m_" + info.m_pMesh.name + "_t_" + info.m_pTexture.name;
                if (!uniqueMeshes.ContainsKey(sName))
                {
                    uniqueMeshes.Add(sName, info);
                }
            }

            Dictionary<string, string> oldNewMeshDic = CreateMeshes(
                uniqueMeshes.Values.ToArray(), 
                textureDic, 
                sParentFolder + "/Meshes/",
                iProgressNow,
                iProgressFull);

            iProgressNow += oldNewMeshDic.Count;

            //========================================
            //Create a new gameobject and replace the material and mesh filter
            foreach (SRendererInfo info in kvp.Value)
            {
                ++iProgressNow;
                EditorUtility.DisplayProgressBar("正在组装", string.Format("{0}/{1}", iProgressNow, iProgressFull),
                    iProgressNow / (float)iProgressFull);
                string sName = "f_" + CommonFunctions.GetLastName(info.m_sMeshFilterName) + "_m_" + info.m_pMesh.name + "_t_" + info.m_pTexture.name;
                string sPath = oldNewMeshDic[sName];
                GameObject toReplace = GameObject.Find(info.m_sObjectPath.Replace(pObj.name, pObj.name + "_Atlas"));
                toReplace.GetComponent<Renderer>().sharedMaterial = AssetDatabase.LoadAssetAtPath("Assets/" + sTextureFileName + ".mat", typeof(Material)) as Material;
                toReplace.GetComponent<Renderer>().useLightProbes = false;
                toReplace.GetComponent<Renderer>().shadowCastingMode = ShadowCastingMode.Off;
                toReplace.GetComponent<Renderer>().receiveShadows = false;
                toReplace.GetComponent<Renderer>().reflectionProbeUsage = ReflectionProbeUsage.Off;
                toReplace.GetComponent<MeshFilter>().sharedMesh = AssetDatabase.LoadAssetAtPath(sPath, typeof(Mesh)) as Mesh;
            }

            iProgressNow += oldNewMeshDic.Count;
        }
        EditorUtility.ClearProgressBar();
    }

    public static Dictionary<string, Rect> PackTexture(Texture2D[] smallTextures, string sFileName, Shader shader)
    {
        Dictionary<string, Rect> res = new Dictionary<string, Rect>();
        List<Texture2D> textureList = new List<Texture2D>();
        foreach (Texture2D texture in smallTextures)
        {
            string texturePath = AssetDatabase.GetAssetPath(texture);
            string sFullPath = "file:///" + Application.dataPath + texturePath.Substring(6, texturePath.Length - 6);
            WWW www = new WWW(sFullPath);
            Texture2D t1 = AssetDatabase.LoadAssetAtPath(texturePath, typeof(Texture2D)) as Texture2D;
            Debug.Assert(t1 != null, "t1 != null");
            Texture2D duplic = new Texture2D(t1.width, t1.height, t1.format, t1.mipmapCount > 0);
            www.LoadImageIntoTexture(duplic);
            textureList.Add(duplic);
        }
        Texture2D bigTexture = new Texture2D(2048, 2048);

        Rect[] rects = bigTexture.PackTextures(textureList.ToArray(), 0, 2048);
        for (int i = 0; i < rects.Length; ++i)
        {
            res.Add(AssetDatabase.GetAssetPath(smallTextures[i]), rects[i]);
        }
        bigTexture.Apply(true);

        if (bigTexture.format != TextureFormat.ARGB32 && bigTexture.format != TextureFormat.RGB24)
        {
            Texture2D newTexture = new Texture2D(bigTexture.width, bigTexture.height);
            newTexture.SetPixels(bigTexture.GetPixels(0), 0);
            bigTexture = newTexture;
        }

        if (shader.renderQueue > 2400)
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

    public static Dictionary<string, string> CreateMeshes(SRendererInfo[] mesh, Dictionary<string, Rect> rectList, string sFileName, int iProgress, int iProgressFull)
    {
        Dictionary<string, string> ret = new Dictionary<string, string>();
        //Load Mesh
        foreach (SRendererInfo info in mesh)
        {
            Mesh theMesh = LoadMeshAtPathWithName(info.m_sMeshFilterName, info.m_pMesh.name);
            Mesh theMesh2 = new Mesh();
            ++iProgress;
            EditorUtility.DisplayProgressBar("正在生成Atlas模型", string.Format("{0}/{1}", iProgress, iProgressFull),
                iProgress / (float)iProgressFull);
            List<Vector3> list1 = new List<Vector3>();
            for (int i = 0; i < theMesh.vertices.Length; ++i)
            {
                list1.Add(theMesh.vertices[i]);
            }
            theMesh2.vertices = list1.ToArray();

            List<int> list2 = new List<int>();
            for (int i = 0; i < theMesh.triangles.Length; ++i)
            {
                list2.Add(theMesh.triangles[i]);
            }
            theMesh2.triangles = list2.ToArray();

            List<Vector2> list3 = new List<Vector2>();
            for (int i = 0; i < theMesh.uv.Length; ++i)
            {
                Vector2 vUV = theMesh.uv[i];
                if (theMesh.uv[i].x < -0.01f || theMesh.uv[i].y < -0.01f || theMesh.uv[i].x > 1.01f || theMesh.uv[i].y > 1.01f)
                {
                    vUV.x = Mathf.Repeat(theMesh.uv[i].x, 1.0f);
                    vUV.y = Mathf.Repeat(theMesh.uv[i].y, 1.0f);
                }
                else
                {
                    vUV.x = Mathf.Clamp01(theMesh.uv[i].x);
                    vUV.y = Mathf.Clamp01(theMesh.uv[i].y);
                }
                Rect rect = rectList[info.m_sTextureName];
                list3.Add(new Vector2(vUV.x * rect.width + rect.xMin, vUV.y * rect.height + rect.yMin));
            }
            theMesh2.uv = list3.ToArray();

            List<Vector2> list4 = new List<Vector2>();
            for (int i = 0; i < theMesh.uv2.Length; ++i)
            {
                list4.Add(theMesh.uv2[i]);
            }
            theMesh2.uv2 = list4.ToArray();

            List<Vector3> list5 = new List<Vector3>();
            for (int i = 0; i < theMesh.normals.Length; ++i)
            {
                list5.Add(theMesh.normals[i]);
            }
            theMesh2.normals = list5.ToArray();

            List<Color> list6 = new List<Color>();
            for (int i = 0; i < theMesh.colors.Length; ++i)
            {
                list6.Add(theMesh.colors[i]);
            }
            theMesh2.colors = list6.ToArray();

            List<Vector4> list7 = new List<Vector4>();
            for (int i = 0; i < theMesh.tangents.Length; ++i)
            {
                list7.Add(theMesh.tangents[i]);
            }
            theMesh2.tangents = list7.ToArray();

            theMesh2.Optimize();

            //Create Mesh
            string sName = "f_" + CommonFunctions.GetLastName(info.m_sMeshFilterName) + "_m_" + info.m_pMesh.name + "_t_" + info.m_pTexture.name;
            string thePath = "Assets/" + sFileName + sName + ".asset";
            AssetDatabase.CreateAsset(theMesh2, thePath);
            AssetDatabase.Refresh();
            if (ret.ContainsKey(sName))
            {
                CRuntimeLogger.LogError("有mesh的FBX文件重名！");
            }
            else
            {
                ret.Add(sName, thePath);
            }
        }

        return ret;
    }

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
}
