using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using Debug = UnityEngine.Debug;

//This class is for atlas the charactor
public class CharactorAtlasUtil
{
    #region New Version

    public class SCharRendererFullInfo
    {
        public string m_sFullTexName;

        public string m_sMeshFilterName;
        public string m_sMeshName;
        public string m_sObjectPath;
        public GameObject m_pObj;

        public Texture2D m_pMainTexture;
        public Texture2D m_pColorTexture;
        public Texture2D m_pShaTexture;
        public Mesh m_pMesh;

        public Mesh[] m_pCombinedMesh;
        public string[] m_sTextureNames;
        public GameObject[] m_pCombinedMeshObj;
        public bool m_bDiscard;
        public bool m_bCombine;
        public string m_pTransfParentPath;
    }

    private static List<string[]> GetHumanoidCombineList()
    {
        List<string[]> ret = new List<string[]>();
        CHumanoidCombineTable table = new CHumanoidCombineTable();
        table.Load();
        foreach (CHumanoidCombineTableElement one in table.m_pElement)
        {
            bool bValid = true;
            foreach (string oneComp in one.m_sCombine)
            {
                if (string.IsNullOrEmpty(oneComp))
                {
                    CRuntimeLogger.LogWarning("Invalid element in Combine Table: ID = " + one.m_iID);
                    bValid = false;
                    break;
                }
            }

            if (bValid)
            {
                if (one.m_sCombine.Length < 2)
                {
                    CRuntimeLogger.LogWarning("Invalid element in Combine Table: ID = " + one.m_iID);
                    bValid = false;
                }
            }

            if (bValid)
            {
                ret.Add(one.m_sCombine);
            }
        }
        return ret;
    }

    public static void MakeAtlasReplaceWithCloth(GameObject pObj, string sParentFolder)
    {
        #region Step0: Count Progress

        int iMC = (int)ECharactorMainColor.ECMC_Max;
        int iSC = (int)ECharactorSubColor.ECSC_Max + 1;
        List<string[]> combineList = GetHumanoidCombineList();
        int iProgressCount = pObj.GetComponentsInChildren<MeshRenderer>(true).Length + combineList.Count;
        int iProgressFull = iProgressCount * (2 + 3 * iMC * iSC);
        int iProgressNow = 0;

        #endregion

        #region Step1: make gameobject table and make dirs (1x)

        CommonFunctions.CreateFolder(Application.dataPath + "/" + sParentFolder + "/Textures/?");
        for (int i = 0; i < iMC; ++i)
        {
            for (int j = 0; j < iSC; ++j)
            {
                CommonFunctions.CreateFolder(Application.dataPath + "/" + sParentFolder + "/Resources/CharMesh/" + string.Format("M{0}S{1}/?", i, j));
            }
        }

        List<SCharRendererFullInfo> info = new List<SCharRendererFullInfo>();
        Dictionary<string, Texture2D[]> contextureCombine = new Dictionary<string, Texture2D[]>();
        Dictionary<string, int> infoIndex = new Dictionary<string, int>();
        foreach (MeshRenderer renderer in pObj.GetComponentsInChildren<MeshRenderer>(true))
        {
            ++iProgressNow;
            EditorUtility.DisplayProgressBar("正在收集Renderer信息", string.Format("{0}/{1}", iProgressNow, iProgressFull),
                iProgressNow / (float)iProgressFull);

            Texture2D mainTex = renderer.sharedMaterial.GetTexture("_MainTex") as Texture2D;
            Texture2D colorTex = renderer.sharedMaterial.GetTexture("_ColorTex") as Texture2D;
            Texture2D shaTex = renderer.sharedMaterial.GetTexture("_ShadowTex") as Texture2D;

            if (null == mainTex || null == colorTex || null == shaTex)
            {
                CRuntimeLogger.LogError("有的材质没有贴图: " + CommonFunctions.FindFullName(pObj, renderer.gameObject));
                EditorUtility.ClearProgressBar();
                return;
            }

            string sTextureName = CommonFunctions.BuildStringOrder(new[] {mainTex.name, colorTex.name, shaTex.name});
            Mesh mesh = renderer.gameObject.GetComponent<MeshFilter>().sharedMesh;

            info.Add(new SCharRendererFullInfo
            {
                m_sFullTexName = sTextureName,
                m_pMesh = mesh,
                m_sMeshFilterName = AssetDatabase.GetAssetPath(mesh),
                m_sMeshName = mesh.name,
                m_pObj = renderer.gameObject,
                m_sObjectPath = CommonFunctions.FindFullName(pObj, renderer.gameObject),
                m_pMainTexture = mainTex,
                m_pColorTexture = colorTex,
                m_pShaTexture = shaTex,
                m_pCombinedMesh = new Mesh[0],
                m_bDiscard = false,
                m_pCombinedMeshObj = null,
                m_bCombine = false,
            });
            infoIndex.Add(CommonFunctions.FindFullName(pObj, renderer.gameObject), info.Count - 1);
            if (!contextureCombine.ContainsKey(sTextureName))
            {
                contextureCombine.Add(sTextureName, new[] { mainTex, colorTex, shaTex });
            }
        }

        iProgressNow = iProgressCount;

        #endregion

        #region Step2: make colored textures (20x)

        CCharactorColor colors = new CCharactorColor();
        colors.Load();
        Dictionary<string, Texture2D> outTexture = new Dictionary<string, Texture2D>();
        for (int i = 0; i < iMC; ++i)
        {
            for (int j = 0; j < iSC; ++j)
            {
                Color mainC = colors[string.Format("M{0}", i + 1)].m_cColor;
                Color subC = Color.white * 0.7f;
                if (j < (int) ECharactorSubColor.ECSC_Max)
                {
                    subC = colors[string.Format("S{0}", j + 1)].m_cColor;
                }
                
                foreach (KeyValuePair<string, Texture2D[]> kvp in contextureCombine)
                {
                    ++iProgressNow;
                    EditorUtility.DisplayProgressBar("正在阵营着色", string.Format("{0}/{1}", iProgressNow, iProgressFull),
                        iProgressNow / (float)iProgressFull);
                    Texture2D outPutT = GetTexture2DWithColor(kvp.Value[0], kvp.Value[1], kvp.Value[2], mainC, subC, j == (int)ECharactorSubColor.ECSC_Max);
                    outTexture.Add(string.Format("{0}_M{1}S{2}", kvp.Key, i, j), outPutT);
                }
                iProgressNow = (i*iSC + j + 2)*iProgressCount;
            }            
        }

        Shader unlit = Shader.Find("Unlit/Texture");
        string sPackTextureName = sParentFolder + "/Textures/" + unlit.name.Replace("/", "_").Replace(".", "_");
        Dictionary<string, Rect> packed = EditorCommonFunctions.PackTexture(outTexture, sPackTextureName, unlit, EditorCommonFunctions.EPackColorFormat.ForcePng);

        #endregion

        #region Step3: Combine Batch (1x)

        List<string> sDisableList = new List<string>();
        foreach (string[] combines in combineList)
        {
            ++iProgressNow;
            EditorUtility.DisplayProgressBar("正在收集合并Batch信息", string.Format("{0}/{1}", iProgressNow, iProgressFull),
                iProgressNow / (float)iProgressFull);

            GameObject parent = info[infoIndex[pObj.name + "/" + combines[0]]].m_pObj;
            SCharRendererFullInfo cmb = new SCharRendererFullInfo
            {
                m_sObjectPath = CommonFunctions.FindFullName(pObj, parent.transform.parent.gameObject) + "/",
                m_bDiscard = false,
                m_pObj = null,
                m_pTransfParentPath = pObj.name + "/" + combines[0],
                m_bCombine = true,
            };
            string sMeshFilterName = "";
            List<GameObject> cmbobjs = new List<GameObject>();
            List<Mesh> cmbmeshes = new List<Mesh>();
            List<string> cmbtextures = new List<string>();

            foreach (string t in combines)
            {
                if (!sDisableList.Contains(pObj.name + "/" + t))
                {
                    sDisableList.Add(pObj.name + "/" + t);
                }
                GameObject theObj = info[infoIndex[pObj.name + "/" + t]].m_pObj;
                cmb.m_sObjectPath += theObj.name;
                sMeshFilterName += theObj.name;
                cmbobjs.Add(theObj);
                cmbmeshes.Add(info[infoIndex[pObj.name + "/" + t]].m_pMesh);
                cmbtextures.Add(info[infoIndex[pObj.name + "/" + t]].m_sFullTexName);
            }
            cmb.m_pCombinedMeshObj = cmbobjs.ToArray();
            cmb.m_pCombinedMesh = cmbmeshes.ToArray();
            cmb.m_sTextureNames = cmbtextures.ToArray();
            cmb.m_sMeshFilterName = sMeshFilterName;
            info.Add(cmb);
        }

        foreach (string namecmbdisable in sDisableList)
        {
            info[infoIndex[namecmbdisable]].m_bDiscard = true;
        }

        iProgressNow = (2 + iMC * iSC)*iProgressCount;

        #endregion

        #region Step4: Create Meshes (20x)

        Dictionary<string, string> meshDic = CreateMeshes(info.ToArray(), packed, 
            sParentFolder + "/Resources/CharMesh/", iProgressNow, iProgressFull, iMC, iSC);
        AssetDatabase.Refresh();
        iProgressNow = (2 + iMC * iSC * 2) * iProgressCount;

        #endregion

        #region Step5: Assemble Game Objects (20x)

        //Create the matrix
        for (int i = 0; i < iMC; ++i)
        {
            for (int j = 0; j < iSC; ++j)
            {
                CreateObj(i, j, info, meshDic, "Assets/" + sPackTextureName + ".mat", pObj, iProgressNow, iProgressFull);
                iProgressNow = ((2 + iMC * iSC * 2) + ((i * iSC) + j + 1)) * iProgressCount;
            }
        }

        #endregion

        EditorUtility.ClearProgressBar();
    }

    public static Texture2D GetTexture2DWithColor(Texture2D pMain, Texture2D pColor, Texture2D pShadow, Color cMain, Color cSub, bool bIsInv)
    {
        Texture2D pMainR = (Texture2D) EditorCommonFunctions.GetReadWritable(pMain);
        Color32[] pxMainR = pMainR.GetPixels32();
        Texture2D pColorR = (Texture2D)EditorCommonFunctions.GetReadWritable(pColor);
        Color32[] pxColorR = pColorR.GetPixels32();
        Texture2D pShadowR = (Texture2D)EditorCommonFunctions.GetReadWritable(pShadow);
        Color32[] pxShadowR = pShadowR.GetPixels32();
        int iWidth = Mathf.Max(pMainR.width, pColorR.width);
        int iHeight = Mathf.Max(pMainR.height, pColorR.height);
        Texture2D ret1 = new Texture2D(iWidth, iHeight, TextureFormat.RGB24, false);
        Color32[] res132_1 = new Color32[iWidth * iHeight];

        for (int i = 0; i < iWidth; ++i)
        {
            for (int j = 0; j < iHeight; ++j)
            {
                Color cMainTex = pxMainR[(i * pMainR.width / iWidth) * pMainR.height + j * pMainR.height / iHeight];
                Color cColorTex = pxColorR[(i * pColorR.width / iWidth) * pColorR.height + j * pColorR.height / iHeight];
                Color cShadowTex = pxShadowR[(i * pShadowR.width / iWidth) * pShadowR.height + j * pShadowR.height / iHeight];

				float shadowcol = Mathf.Clamp01((cShadowTex.r + 0.2f) * (1.0f + cColorTex.b * cColorTex.r));
                float rate = cColorTex.b;
                float srate = cColorTex.r;
                Color cor = cSub * cColorTex.g + cMain * cColorTex.r;
                Color realC = cMainTex * (1.0f - rate) + cor * rate;
                realC = 1.5f * shadowcol * realC;
                realC.a = 1.0f;

                if (bIsInv)
                {
                    realC = (new Color(realC.r - 0.5f, realC.g - 0.5f, realC.b - 0.5f) * Mathf.Clamp(srate, 0.2f, 0.8f) + new Color(0.5f, 0.5f, 0.5f)) * 0.3f + 0.1f * cMain + 0.1f * Color.white;
                }

                res132_1[i * iHeight + j] = realC;
            }
        }
        ret1.SetPixels32(res132_1);
        return ret1;
    }

    public static Dictionary<string, string> CreateMeshes(SCharRendererFullInfo[] mesh, Dictionary<string, Rect> rectList, 
        string sFileName, int iProgress, int iProgressFull, int iMc, int iSc)
    {
        Dictionary<string, string> ret = new Dictionary<string, string>();
        //Load Mesh
        foreach (SCharRendererFullInfo info in mesh)
        {
            if (!info.m_bDiscard)
            {
                for (int i = 0; i < iMc; ++i)
                {
                    for (int j = 0; j < iSc; ++j)
                    {
                        ++iProgress;
                        EditorUtility.DisplayProgressBar("正在生成Atlas模型", string.Format("{0}/{1}", iProgress, iProgressFull),
                            iProgress / (float)iProgressFull);


                        if (!info.m_bCombine)
                        {
                            #region Not Combine

                            string sTexture1 = string.Format("{0}_M{1}S{2}", info.m_sFullTexName, i, j);

                            Mesh theMesh = (Mesh) EditorCommonFunctions.GetReadWritable(info.m_pMesh);
                            Mesh theMesh1 = new Mesh();

                            theMesh1.SetVertices(theMesh.vertices.ToList());
                            theMesh1.SetTriangles(theMesh.triangles.ToList(), 0);

                            List<Vector2> list1 = new List<Vector2>();
                            for (int k = 0; k < theMesh.uv.Length; ++k)
                            {
                                Vector2 vUV = theMesh.uv[k];
                                if (theMesh.uv[k].x < -0.01f || theMesh.uv[k].y < -0.01f || theMesh.uv[k].x > 1.01f ||
                                    theMesh.uv[k].y > 1.01f)
                                {
                                    CRuntimeLogger.LogWarning("UV is not in 0 - 1, clampled!: FBX:" +
                                                              CommonFunctions.GetLastName(info.m_sMeshFilterName)
                                                              + " Model:" + info.m_pMesh.name);
                                }

                                vUV.x = Mathf.Clamp01(theMesh.uv[k].x);
                                vUV.y = Mathf.Clamp01(theMesh.uv[k].y);

                                Rect rect1 = rectList[sTexture1];
                                list1.Add(new Vector2(vUV.x*rect1.width + rect1.xMin, vUV.y*rect1.height + rect1.yMin));
                            }
                            theMesh1.uv = list1.ToArray();

                            theMesh1.uv2 = null;
                            theMesh1.normals = null;
                            theMesh1.colors = null;
                            theMesh1.tangents = null;

                            ;

                            //Create Mesh
                            string sName1 = string.Format("M{0}S{1}/", i, j) +
                                            CommonFunctions.GetLastName(info.m_sMeshFilterName)
                                            + "_" + info.m_pMesh.name
                                            + "_" + info.m_sFullTexName;

                            AssetDatabase.CreateAsset(theMesh1, "Assets/" + sFileName + sName1 + ".asset");
                            if (ret.ContainsKey(sName1))
                            {
                                CRuntimeLogger.LogWarning("有mesh的FBX文件重名！" + sName1);
                            }
                            else
                            {
                                ret.Add(sName1, "Assets/" + sFileName + sName1 + ".asset");
                            }
                            #endregion
                        }
                        else
                        {
                            #region Combine

                            List<Vector3> poses = new List<Vector3>();
                            List<int> indexes = new List<int>();
                            List<Vector2> uvs1 = new List<Vector2>();

                            for (int objIndex = 0; objIndex < info.m_pCombinedMeshObj.Length; ++objIndex)
                            {
                                string sTexture1 = string.Format("{0}_M{1}S{2}", info.m_sTextureNames[objIndex], i, j);
                                Mesh theMesh = (Mesh)EditorCommonFunctions.GetReadWritable(info.m_pCombinedMesh[objIndex]);
                                int iOldPosNum = poses.Count;

                                if (0 == objIndex)
                                {
                                    poses = theMesh.vertices.ToList();
                                }
                                else
                                {
                                    foreach (Vector3 vets in theMesh.vertices)
                                    {
                                        Vector3 worldPos = info.m_pCombinedMeshObj[objIndex].transform.localToWorldMatrix.MultiplyVector(vets) + info.m_pCombinedMeshObj[objIndex].transform.position;
                                        poses.Add(info.m_pCombinedMeshObj[0].transform.worldToLocalMatrix.MultiplyVector(worldPos - info.m_pCombinedMeshObj[0].transform.position));
                                    }
                                }

                                if (0 == objIndex)
                                {
                                    indexes = theMesh.triangles.ToList();
                                }
                                else
                                {
                                    foreach (int oneind in theMesh.triangles)
                                    {
                                        indexes.Add(oneind + iOldPosNum);
                                    }
                                }

                                for (int k = 0; k < theMesh.uv.Length; ++k)
                                {
                                    Vector2 vUV = theMesh.uv[k];
                                    vUV.x = Mathf.Clamp01(theMesh.uv[k].x);
                                    vUV.y = Mathf.Clamp01(theMesh.uv[k].y);
                                    Rect rect1 = rectList[sTexture1];

                                    uvs1.Add(new Vector2(vUV.x * rect1.width + rect1.xMin, vUV.y * rect1.height + rect1.yMin));
                                }
                            }

                            Mesh theMesh1 = new Mesh();

                            theMesh1.SetVertices(poses);
                            theMesh1.SetTriangles(indexes, 0);
                            theMesh1.uv = uvs1.ToArray();

                            theMesh1.uv2 = null;
                            theMesh1.normals = null;
                            theMesh1.colors = null;
                            theMesh1.tangents = null;

                            ;

                            //Create Mesh
                            string sName1 = string.Format("M{0}S{1}/cmb_", i, j) +
                                            CommonFunctions.GetLastName(info.m_sMeshFilterName);

                            AssetDatabase.CreateAsset(theMesh1, "Assets/" + sFileName + sName1 + ".asset");
                            if (ret.ContainsKey(sName1))
                            {
                                CRuntimeLogger.LogWarning("有mesh的FBX文件重名！" + sName1);
                            }
                            else
                            {
                                ret.Add(sName1, "Assets/" + sFileName + sName1 + ".asset");
                            }
                            #endregion
                        }
                    }
                }                
            }
        }

        return ret;
    }

    public static void CreateObj(int i, int j, List<SCharRendererFullInfo> info, Dictionary<string, string> meshDic,
        string sMatName, GameObject pObj, int iProgressNow, int iProgressFull)
    {
        GameObject copy = Object.Instantiate(pObj, pObj.transform.position + Vector3.left * 3.0f * (i + 1) 
            + Vector3.back * 3.0f * j, pObj.transform.rotation) as GameObject;
        Debug.Assert(copy != null, "copy != null");
        string sTypeName = string.Format("M{0}S{1}", i, j);
        string sObjName = pObj.name + sTypeName;
        copy.name = sObjName;
        
        foreach (SCharRendererFullInfo oneinfo in info)
        {
            ++iProgressNow;
            EditorUtility.DisplayProgressBar("正在组装", string.Format("{0}/{1}", iProgressNow, iProgressFull),
                iProgressNow / (float)iProgressFull);

            if (!oneinfo.m_bDiscard)
            {
                if (oneinfo.m_bCombine)
                {
                    //Create the new obj
                    GameObject newObj = new GameObject(CommonFunctions.GetLastName(oneinfo.m_sMeshFilterName));
                    GameObject parentObj = GameObject.Find(oneinfo.m_pTransfParentPath.Replace(pObj.name, sObjName));
                    newObj.transform.parent = parentObj.transform.parent;
                    newObj.transform.localPosition = parentObj.transform.localPosition;
                    newObj.transform.localRotation = parentObj.transform.localRotation;
                    newObj.transform.localScale = parentObj.transform.localScale;

                    MeshFilter filter = newObj.AddComponent<MeshFilter>();
                    string sName = sTypeName + "/cmb_"  + CommonFunctions.GetLastName(oneinfo.m_sMeshFilterName);
                    string sPath = meshDic[sName];
                    filter.sharedMesh = AssetDatabase.LoadAssetAtPath(sPath, typeof(Mesh)) as Mesh;

                    MeshRenderer renderer = newObj.AddComponent<MeshRenderer>();
                    renderer.sharedMaterial = AssetDatabase.LoadAssetAtPath(sMatName, typeof(Material)) as Material;
                    renderer.lightProbeUsage = LightProbeUsage.Off;
                    renderer.shadowCastingMode = ShadowCastingMode.Off;
                    renderer.receiveShadows = false;
                    renderer.reflectionProbeUsage = ReflectionProbeUsage.Off;
                }
                else
                {
                    string sName = sTypeName + "/" + CommonFunctions.GetLastName(oneinfo.m_sMeshFilterName)
                                            + "_" + oneinfo.m_pMesh.name
                                            + "_" + oneinfo.m_sFullTexName;
                    string sPath = meshDic[sName];
                    GameObject toReplace = GameObject.Find(oneinfo.m_sObjectPath.Replace(pObj.name, sObjName));
                    toReplace.GetComponent<Renderer>().sharedMaterial = AssetDatabase.LoadAssetAtPath(sMatName, typeof(Material)) as Material;
                    toReplace.GetComponent<Renderer>().lightProbeUsage = LightProbeUsage.Off;
                    toReplace.GetComponent<Renderer>().shadowCastingMode = ShadowCastingMode.Off;
                    toReplace.GetComponent<Renderer>().receiveShadows = false;
                    toReplace.GetComponent<Renderer>().reflectionProbeUsage = ReflectionProbeUsage.Off;
                    toReplace.GetComponent<MeshFilter>().sharedMesh = AssetDatabase.LoadAssetAtPath(sPath, typeof(Mesh)) as Mesh;
                }
            }
        }

        foreach (SCharRendererFullInfo oneinfo in info)
        {
            if (oneinfo.m_bDiscard)
            {
                GameObject toReplace = GameObject.Find(oneinfo.m_sObjectPath.Replace(pObj.name, sObjName));
                Object.DestroyImmediate(toReplace);
            }
        }
    }

    #endregion

    #region Old Version

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

    public static void MakeAtlasReplace(GameObject pObj, string sParentFolder, bool bForceTransparent = false)
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
            Dictionary<string, Rect> textureDic = EditorCommonFunctions.PackTextureInDataBase(allTextures,
                sTextureFileName,
                kvp.Key,
                EditorCommonFunctions.EPackColorFormat.ForcePng);

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
                toReplace.GetComponent<Renderer>().lightProbeUsage = LightProbeUsage.Off;
                toReplace.GetComponent<Renderer>().shadowCastingMode = ShadowCastingMode.Off;
                toReplace.GetComponent<Renderer>().receiveShadows = false;
                toReplace.GetComponent<Renderer>().reflectionProbeUsage = ReflectionProbeUsage.Off;
                toReplace.GetComponent<MeshFilter>().sharedMesh = AssetDatabase.LoadAssetAtPath(sPath, typeof(Mesh)) as Mesh;
            }

            iProgressNow += oldNewMeshDic.Count;
        }
        EditorUtility.ClearProgressBar();
    }

    public static Dictionary<string, string> CreateMeshes(SRendererInfo[] mesh, Dictionary<string, Rect> rectList, string sFileName, int iProgress, int iProgressFull)
    {
        Dictionary<string, string> ret = new Dictionary<string, string>();
        //Load Mesh
        foreach (SRendererInfo info in mesh)
        {
            Mesh theMesh = EditorCommonFunctions.LoadMeshAtPathWithName(info.m_sMeshFilterName, info.m_pMesh.name);
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
                    CRuntimeLogger.LogWarning("UV is not in 0 - 1, clampled!: FBX:" +
                                              CommonFunctions.GetLastName(info.m_sMeshFilterName) 
                                              + " Model:" + info.m_pMesh.name);
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

            ;

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

    #endregion
}
