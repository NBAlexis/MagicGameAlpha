using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEditor;

public class SceneEditorUtility
{
    protected class CDecorateInfo
    {
        public int m_iCh;
        public Vector3 m_vMid;
        public int m_iSize;
    }

    public static void CreateGround(Texture2D heighttx, Texture2D groundtx, Texture2D doctx, 
        string sGroundFileName, string sGroundType, bool bCreatePathmap, out Texture2D wmap, out Texture2D nmap)
    {
        #region Step1: Check files

        wmap = null;
        nmap = null;
        CSceneTexture sceneTx = new CSceneTexture { m_sSceneType = sGroundType };
        if (!sceneTx.Load() || 81 != sceneTx.m_pElement.Count)
        {
            CRuntimeLogger.LogError(sGroundType + "贴图配置文件有问题");
            return;
        }

        CSceneGroudTemplate groundTemplate = new CSceneGroudTemplate { m_sSceneType = sGroundType };
        if (!groundTemplate.Load())
        {
            CRuntimeLogger.LogError(sGroundType + "贴图配置文件有问题");
            return;
        }

        CSceneType type = new CSceneType();
        type.Load();
        if (null == type[sGroundType])
        {
            CRuntimeLogger.LogError(sGroundType + "的配置找不到？");
            return;
        }

        #endregion

        #region Step2: Read colors and make cliff ground type replace, 4096 x 4

        int iProgressOne = heighttx.width*heighttx.height;
        int iProgressTotal = iProgressOne*10;
        int iProgressNow = 0;

        if (bCreatePathmap)
        {
            wmap = new Texture2D(heighttx.width - 1, heighttx.height - 1, TextureFormat.RGB24, false);
            nmap = new Texture2D(heighttx.width - 1, heighttx.height - 1, TextureFormat.RGB24, false);
        }

        Vector3[,] verts = new Vector3[heighttx.width, heighttx.height];
        int[,] gtypes = new int[heighttx.width, heighttx.height];
        for (int i = 0; i < heighttx.width; ++i)
        {
            for (int j = 0; j < heighttx.height; ++j)
            {
                ++iProgressNow;
                EditorUtility.DisplayProgressBar("创建中", string.Format("{0}...{1}/{2}", 
                    "读高度图数据",
                    iProgressNow,
                    iProgressTotal), iProgressNow / (float)iProgressTotal);

                Color h = heighttx.GetPixel(i, j);
                Color g = groundtx.GetPixel(i, j);
                verts[i, j] = new Vector3(
                    i - (heighttx.width - 1) / 2,
                    (h.r - 0.5f) * 2.0f * SceneConstant.m_fHighgroundHeight,
                    j - (heighttx.height - 1) / 2);
                gtypes[i, j] = CompareRGB(g.r, g.g, g.b);
            }
        }

        iProgressNow = iProgressOne;
        EditorUtility.DisplayProgressBar("创建中", string.Format("{0}...{1}/{2}",
            "读高度图数据",
            iProgressNow,
            iProgressTotal), iProgressNow / (float)iProgressTotal);

        //Make Clifs
        for (int i = 0; i < heighttx.width - 1; ++i)
        {
            for (int j = 0; j < heighttx.height - 1; ++j)
            {
                if (Mathf.Max(verts[i, j].y, verts[i + 1, j].y, verts[i + 1, j + 1].y, verts[i, j + 1].y)
                  - Mathf.Min(verts[i, j].y, verts[i + 1, j].y, verts[i + 1, j + 1].y, verts[i, j + 1].y)
                  > SceneConstant.m_fCliffRate * SceneConstant.m_fHighgroundHeight)
                {
                    gtypes[i, j] = type[sGroundType].m_iCliffType;
                    gtypes[i + 1, j] = type[sGroundType].m_iCliffType;
                    gtypes[i + 1, j + 1] = type[sGroundType].m_iCliffType;
                    gtypes[i, j + 1] = type[sGroundType].m_iCliffType;
                }

                ++iProgressNow;
                EditorUtility.DisplayProgressBar("创建中", string.Format("{0}...{1}/{2}",
                    "改写悬崖",
                    iProgressNow,
                    iProgressTotal), iProgressNow / (float)iProgressTotal);
            }
        }

        iProgressNow = iProgressOne * 2;
        EditorUtility.DisplayProgressBar("创建中", string.Format("{0}...{1}/{2}",
            "改写悬崖",
            iProgressNow,
            iProgressTotal), iProgressNow / (float)iProgressTotal);

        if (type[sGroundType].m_bHasGroundOffset)
        {
            for (int i = 0; i < heighttx.width; ++i)
            {
                for (int j = 0; j < heighttx.height; ++j)
                {
                    verts[i, j] += Vector3.up * gtypes[i, j] * 0.05f * SceneConstant.m_fHighgroundHeight;

                    ++iProgressNow;
                    EditorUtility.DisplayProgressBar("创建中", string.Format("{0}...{1}/{2}",
                        "改写地皮高度",
                        iProgressNow,
                        iProgressTotal), iProgressNow / (float)iProgressTotal);
                }
            }
        }

        iProgressNow = iProgressOne * 3;
        EditorUtility.DisplayProgressBar("创建中", string.Format("{0}...{1}/{2}",
            "改写地皮高度",
            iProgressNow,
            iProgressTotal), iProgressNow / (float)iProgressTotal);

        if (bCreatePathmap)
        {
            for (int i = 0; i < heighttx.width - 1; ++i)
            {
                for (int j = 0; j < heighttx.height - 1; ++j)
                {
                    float fR, fG, fB;
                    Vector3 vNormal = Vector3.Cross(verts[i, j + 1] - verts[i + 1, j], verts[i + 1, j + 1] - verts[i, j]);
                    vNormal = vNormal.normalized * 0.5f + new Vector3(0.5f, 0.5f, 0.5f);

                    float fHeight = (verts[i, j].y + verts[i + 1, j].y + verts[i + 1, j + 1].y + verts[i, j + 1].y) * 0.25f;
                    fR = fHeight / (SceneConstant.m_fHighgroundHeight * 4.0f) + 0.5f;

                    bool bHasCutOff = verts[i, j].y < SceneConstant.m_fHighgroundHeight * SceneConstant.m_fCutOffHeightRate
                                   || verts[i + 1, j].y < SceneConstant.m_fHighgroundHeight * SceneConstant.m_fCutOffHeightRate
                                   || verts[i + 1, j + 1].y < SceneConstant.m_fHighgroundHeight * SceneConstant.m_fCutOffHeightRate
                                   || verts[i, j + 1].y < SceneConstant.m_fHighgroundHeight * SceneConstant.m_fCutOffHeightRate;

                    if (Mathf.Max(verts[i, j].y, verts[i + 1, j].y, verts[i + 1, j + 1].y, verts[i, j + 1].y)
                      - Mathf.Min(verts[i, j].y, verts[i + 1, j].y, verts[i + 1, j + 1].y, verts[i, j + 1].y)
                      > SceneConstant.m_fCliffRate * SceneConstant.m_fHighgroundHeight)
                    {
                        bHasCutOff = true;
                    }
                    fB = bHasCutOff ? 1.0f : 0.0f;
                    fG = bHasCutOff ? 0.0f : 1.0f;

                    wmap.SetPixel(i, j, new Color(fR, fG, fB, 1.0f));
                    nmap.SetPixel(i, j, new Color(vNormal.x, vNormal.y, vNormal.z, 1.0f));

                    ++iProgressNow;
                    EditorUtility.DisplayProgressBar("创建中", string.Format("{0}...{1}/{2}",
                        "创建寻路地图",
                        iProgressNow,
                        iProgressTotal), iProgressNow / (float)iProgressTotal);
                }
            }
            wmap.Apply(); //apply for farther read
        }

        iProgressNow = iProgressOne * 4;
        EditorUtility.DisplayProgressBar("创建中", string.Format("{0}...{1}/{2}",
            "创建寻路地图",
            iProgressNow,
            iProgressTotal), iProgressNow / (float)iProgressTotal);

        #endregion

        #region Step3: Build the ground, 4096

        List<Vector3> poses = new List<Vector3>();
        List<Vector2> theuvs = new List<Vector2>();
        List<int> trangles = new List<int>();

        //Create Mesh
        for (int i = 0; i < (heighttx.width - 1); ++i)
        {
            for (int j = 0; j < (heighttx.height - 1); ++j)
            {
                ++iProgressNow;
                EditorUtility.DisplayProgressBar("创建中", string.Format("{0}...{1}/{2}",
                    "创建地面",
                    iProgressNow,
                    iProgressTotal), iProgressNow / (float)iProgressTotal);

                Vector2[] uvs = GetUVs(gtypes[i, j], gtypes[i + 1, j], gtypes[i + 1, j + 1], gtypes[i, j + 1], sceneTx, groundTemplate, type[sGroundType].m_bCanRot);
                Vector3[] vs = { verts[i, j], verts[i + 1, j], verts[i + 1, j + 1], verts[i, j + 1] };
                //Check Vanish
                if (vs[0].y < SceneConstant.m_fCutOffHeightRate * SceneConstant.m_fHighgroundHeight
                 && vs[1].y < SceneConstant.m_fCutOffHeightRate * SceneConstant.m_fHighgroundHeight
                 && vs[2].y < SceneConstant.m_fCutOffHeightRate * SceneConstant.m_fHighgroundHeight
                 && vs[3].y < SceneConstant.m_fCutOffHeightRate * SceneConstant.m_fHighgroundHeight)
                {
                    //No trangles
                }
                else if (vs[0].y < SceneConstant.m_fCutOffHeightRate * SceneConstant.m_fHighgroundHeight
                      && vs[1].y < SceneConstant.m_fCutOffHeightRate * SceneConstant.m_fHighgroundHeight
                      && vs[2].y < SceneConstant.m_fCutOffHeightRate * SceneConstant.m_fHighgroundHeight)
                {
                    //Hide 0-1-2, add 0-2-3
                    int iIndexNow = poses.Count;
                    poses.Add(vs[0]);
                    poses.Add(vs[2]);
                    poses.Add(vs[3]);

                    theuvs.Add(uvs[0]);
                    theuvs.Add(uvs[2]);
                    theuvs.Add(uvs[3]);

                    trangles.Add(iIndexNow + 0);
                    trangles.Add(iIndexNow + 2);
                    trangles.Add(iIndexNow + 1);
                }
                else if (vs[1].y < SceneConstant.m_fCutOffHeightRate * SceneConstant.m_fHighgroundHeight
                      && vs[2].y < SceneConstant.m_fCutOffHeightRate * SceneConstant.m_fHighgroundHeight
                      && vs[3].y < SceneConstant.m_fCutOffHeightRate * SceneConstant.m_fHighgroundHeight)
                {
                    //Hide 1-2-3, add 0-1-3
                    int iIndexNow = poses.Count;
                    poses.Add(vs[0]);
                    poses.Add(vs[1]);
                    poses.Add(vs[3]);

                    theuvs.Add(uvs[0]);
                    theuvs.Add(uvs[1]);
                    theuvs.Add(uvs[3]);

                    trangles.Add(iIndexNow + 0);
                    trangles.Add(iIndexNow + 2);
                    trangles.Add(iIndexNow + 1);
                }
                else if (vs[0].y < SceneConstant.m_fCutOffHeightRate * SceneConstant.m_fHighgroundHeight
                      && vs[2].y < SceneConstant.m_fCutOffHeightRate * SceneConstant.m_fHighgroundHeight
                      && vs[3].y < SceneConstant.m_fCutOffHeightRate * SceneConstant.m_fHighgroundHeight)
                {
                    //Hide 0-2-3, add 0-1-2
                    int iIndexNow = poses.Count;
                    poses.Add(vs[0]);
                    poses.Add(vs[1]);
                    poses.Add(vs[2]);

                    theuvs.Add(uvs[0]);
                    theuvs.Add(uvs[1]);
                    theuvs.Add(uvs[2]);

                    trangles.Add(iIndexNow + 0);
                    trangles.Add(iIndexNow + 2);
                    trangles.Add(iIndexNow + 1);
                }
                else if (vs[0].y < SceneConstant.m_fCutOffHeightRate * SceneConstant.m_fHighgroundHeight
                      && vs[1].y < SceneConstant.m_fCutOffHeightRate * SceneConstant.m_fHighgroundHeight
                      && vs[3].y < SceneConstant.m_fCutOffHeightRate * SceneConstant.m_fHighgroundHeight)
                {
                    //Hide 0-1-3, add 1-2-3
                    int iIndexNow = poses.Count;
                    poses.Add(vs[1]);
                    poses.Add(vs[2]);
                    poses.Add(vs[3]);

                    theuvs.Add(uvs[1]);
                    theuvs.Add(uvs[2]);
                    theuvs.Add(uvs[3]);

                    trangles.Add(iIndexNow + 0);
                    trangles.Add(iIndexNow + 2);
                    trangles.Add(iIndexNow + 1);
                }
                else
                {
                    //Add all
                    int iIndexNow = poses.Count;
                    poses.Add(vs[0]);
                    poses.Add(vs[1]);
                    poses.Add(vs[2]);
                    poses.Add(vs[3]);

                    theuvs.Add(uvs[0]);
                    theuvs.Add(uvs[1]);
                    theuvs.Add(uvs[2]);
                    theuvs.Add(uvs[3]);

                    trangles.Add(iIndexNow + 0);
                    trangles.Add(iIndexNow + 3);
                    trangles.Add(iIndexNow + 1);

                    trangles.Add(iIndexNow + 1);
                    trangles.Add(iIndexNow + 3);
                    trangles.Add(iIndexNow + 2);
                }
            }
        }

        iProgressNow = iProgressOne * 5;
        EditorUtility.DisplayProgressBar("创建中", string.Format("{0}...{1}/{2}",
            "保存地面（需要等一下）...",
            iProgressNow,
            iProgressTotal), iProgressNow / (float)iProgressTotal);

        Mesh theMesh = new Mesh { vertices = poses.ToArray(), uv = theuvs.ToArray(), triangles = trangles.ToArray() };
        theMesh.RecalculateNormals();
        Unwrapping.GenerateSecondaryUVSet(theMesh);
        ;
        AssetDatabase.CreateAsset(theMesh, "Assets/" + sGroundFileName);
        AssetDatabase.Refresh();

        GameObject newGround = new GameObject();
        newGround.transform.position = Vector3.zero;
        newGround.AddComponent<MeshRenderer>().sharedMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/" + SceneConstant.m_sArtworkPath + sGroundType + SceneConstant.m_sSceneTexturesPath + ".mat");
        newGround.AddComponent<MeshFilter>().sharedMesh = AssetDatabase.LoadAssetAtPath<Mesh>("Assets/" + sGroundFileName);
        newGround.name = "Ground";
        
        #endregion

        #region Step4: Put Decorates 4096 x 5

        CSceneDecorate doc = new CSceneDecorate { m_sSceneType = sGroundType };
        doc.Load();

        bool[, ,] chs = new bool[3, doctx.width, doctx.height];
        for (int i = 0; i < doctx.width; ++i)
        {
            for (int j = 0; j < doctx.height; ++j)
            {
                Color h = doctx.GetPixel(i, j);
                chs[0, i, j] = h.r > 0.5f;
                chs[1, i, j] = h.g > 0.5f;
                chs[2, i, j] = h.b > 0.5f;

                ++iProgressNow;
                EditorUtility.DisplayProgressBar("创建中", string.Format("{0}...{1}/{2}",
                    "读障碍物分布图",
                    iProgressNow,
                    iProgressTotal), iProgressNow / (float)iProgressTotal);
            }
        }

        iProgressNow = iProgressOne * 6;
        EditorUtility.DisplayProgressBar("创建中", string.Format("{0}...{1}/{2}",
            "读障碍物分布图",
            iProgressNow,
            iProgressTotal), iProgressNow / (float)iProgressTotal);

        List<CDecorateInfo> addDocs = new List<CDecorateInfo>();

        for (int i = 0; i < doctx.width; ++i)
        {
            for (int j = 0; j < doctx.height; ++j)
            {
                for (int k = 0; k < 3; ++k)
                {
                    ++iProgressNow;
                    EditorUtility.DisplayProgressBar("创建中", string.Format("{0}...{1}/{2}",
                        "放置障碍物",
                        iProgressNow,
                        iProgressTotal), iProgressNow / (float)iProgressTotal);

                    if (null != doc[k])
                    {
                        if (chs[k, i, j])
                        {
                            if (bCreatePathmap && doc[k].m_bBlockPathfinding)
                            {
                                Color colorNow = wmap.GetPixel(i, j);
                                wmap.SetPixel(i, j, new Color(colorNow.r, 0.0f, 1.0f));
                            }

                            if (2 == doc[k].m_iDecrateSize
                                && i < (doctx.width - 1)
                                && j < (doctx.height - 1)
                                && chs[k, i + 1, j]
                                && chs[k, i, j + 1]
                                && chs[k, i + 1, j + 1])
                            {
                                addDocs.Add(new CDecorateInfo
                                {
                                    m_iCh = k,
                                    m_vMid = new Vector3(
                                        i - (doctx.width / 2 - 1),
                                        verts[i + 1, j + 1].y,
                                        j - (doctx.height / 2 - 1)),
                                    m_iSize = 2
                                });
                                chs[k, i + 1, j] = false;
                                chs[k, i, j + 1] = false;
                                chs[k, i + 1, j + 1] = false;

                                if (bCreatePathmap && doc[k].m_bBlockPathfinding)
                                {
                                    Color colorNow1 = wmap.GetPixel(i + 1, j);
                                    wmap.SetPixel(i + 1, j, new Color(colorNow1.r, 0.0f, 1.0f));
                                    Color colorNow2 = wmap.GetPixel(i, j + 1);
                                    wmap.SetPixel(i, j + 1, new Color(colorNow2.r, 0.0f, 1.0f));
                                    Color colorNow3 = wmap.GetPixel(i + 1, j + 1);
                                    wmap.SetPixel(i + 1, j + 1, new Color(colorNow3.r, 0.0f, 1.0f));
                                }
                            }
                            else
                            {
                                addDocs.Add(new CDecorateInfo
                                {
                                    m_iCh = k,
                                    m_vMid = new Vector3(
                                        i - (doctx.width / 2 - 1) - 0.5f,
                                        0.25f * (verts[i, j].y + verts[i + 1, j].y + verts[i, j + 1].y + verts[i + 1, j + 1].y),
                                        j - (doctx.height / 2 - 1) - 0.5f),
                                    m_iSize = 1
                                });
                            }
                        }
                    }
                }
            }
        }

        iProgressNow = iProgressOne * 9;
        EditorUtility.DisplayProgressBar("创建中", string.Format("{0}...{1}/{2}",
            "放置障碍物",
            iProgressNow,
            iProgressTotal), iProgressNow / (float)iProgressTotal);

        int iDecIndex = 0;
        foreach (CDecorateInfo dc in addDocs)
        {
            ++iProgressNow;
            EditorUtility.DisplayProgressBar("创建中", string.Format("{0}...{1}/{2}",
                "创建障碍物",
                iProgressNow,
                iProgressTotal), iProgressNow / (float)iProgressTotal);

            GameObject newDec = new GameObject();
            newDec.transform.position = dc.m_vMid
                + new Vector3(
                    Random.Range(-0.2f, 0.2f),
                    doc[dc.m_iCh].m_bOnlyRotateY ? 0.0f : Random.Range(-0.2f, 0.2f),
                    Random.Range(-0.2f, 0.2f));
            newDec.transform.eulerAngles = doc[dc.m_iCh].m_bOnlyRotateY
                ? new Vector3(0.0f, Random.Range(-360.0f, 360.0f), 0.0f)
                : new Vector3(Random.Range(-360.0f, 360.0f), Random.Range(-360.0f, 360.0f), Random.Range(-360.0f, 360.0f));

            if (2 == dc.m_iSize)
            {
                if (!doc[dc.m_iCh].m_bOnlyRotateY)
                {
                    newDec.transform.localScale = new Vector3(
                        Random.Range(1.6f, 2.4f),
                        Random.Range(1.6f, 2.4f),
                        Random.Range(1.6f, 2.4f)
                        );
                }
                else
                {
                    newDec.transform.localScale = new Vector3(
                        Random.Range(1.6f, 2.4f),
                        Random.Range(0.8f, 1.2f),
                        Random.Range(1.6f, 2.4f)
                        );
                }
            }
            else
            {
                newDec.transform.localScale = new Vector3(
                    Random.Range(0.8f, 1.2f),
                    Random.Range(0.8f, 1.2f),
                    Random.Range(0.8f, 1.2f)
                    );
            }
            string sMeshName = doc[dc.m_iCh].m_sElementName + "_" + Random.Range(1, doc[dc.m_iCh].m_iDecrateRepeat);
            newDec.AddComponent<MeshRenderer>().sharedMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/" + SceneConstant.m_sArtworkPath + sGroundType + SceneConstant.m_sSceneTexturesPath + ".mat");
            newDec.AddComponent<MeshFilter>().sharedMesh = AssetDatabase.LoadAssetAtPath<Mesh>("Assets/" + SceneConstant.m_sArtworkPath + sGroundType + SceneConstant.m_sDecoratesPath + "/Generate/" + sMeshName + ".asset");

            newDec.name = "Dec" + iDecIndex;
            newDec.transform.parent = newGround.transform;
            ++iDecIndex;
        }

        #endregion

        EditorUtility.ClearProgressBar();

        if (bCreatePathmap)
        {
            wmap.Apply();
            nmap.Apply();
        }

    }

    private static int CompareRGB(float r, float g, float b)
    {
        return r > g ? (b > r ? 2 : 0) : (b > g ? 2 : 1);
    }

    private static Vector2[] GetUVs(int lt, int rt, int rb, int lb, CSceneTexture tx, CSceneGroudTemplate tem, bool bCanRot)
    {
        int iCode2 = lt * CommonFunctions.IntPow(3, 0)
                   + rt * CommonFunctions.IntPow(3, 1)
                   + rb * CommonFunctions.IntPow(3, 2)
                   + lb * CommonFunctions.IntPow(3, 3);

        CSceneTextureElement txelement = tx[iCode2];
        if (null == txelement)
        {
            CRuntimeLogger.LogError("贴图配置文件有问题");
            return null;
        }

        string sTxName = string.Format("T{0}_{1}", txelement.m_iTemplate, Random.Range(0, txelement.m_iTextureCount) + 1);
        Rect uv = CommonFunctions.V42Rect(tem[sTxName].m_vUV);

        int iRot = txelement.m_iRotNumber;
        bool bRot = lt == rt && lt == rb && lt == lb;
        if (bRot && bCanRot)
        {
            iRot += Random.Range(0, 4);
        }

        Vector2[] ret =
        {
            new Vector2(uv.xMin, uv.yMin + uv.height), 
            new Vector2(uv.xMin + uv.width, uv.yMin + uv.height), 
            new Vector2(uv.xMin + uv.width, uv.yMin), 
            new Vector2(uv.xMin, uv.yMin)
        };

        for (int i = 0; i < iRot; ++i)
        {
            Vector2 last = ret[0];
            ret[0] = ret[1];
            ret[1] = ret[2];
            ret[2] = ret[3];
            ret[3] = last;

            //Vector2 last = ret[3];
            //ret[3] = ret[2];
            //ret[2] = ret[1];
            //ret[1] = ret[0];
            //ret[0] = last;
        }

        return ret;
    }

    static public void BakeEdgeWater(Texture2D hmap, Texture2D gmap, Texture2D decmap)
    {
        for (int i = 0; i < hmap.width; ++i)
        {
            for (int j = 0; j < hmap.height; ++j)
            {
                if (i < 2 || i >= hmap.width - 2
                 || j < 2 || j >= hmap.height - 2)
                {
                    hmap.SetPixel(i, j, Color.black);
                    gmap.SetPixel(i, j, Color.red);
                }

                if (i < 2 || (i >= hmap.width - 3 && i < hmap.width - 1)
                 || j < 2 || (j >= hmap.height - 3 && i < hmap.height - 1)
                    )
                {
                    decmap.SetPixel(i, j, Color.black);
                }
            }            
        }
        hmap.Apply();
        gmap.Apply();
        decmap.Apply();
    }
}
