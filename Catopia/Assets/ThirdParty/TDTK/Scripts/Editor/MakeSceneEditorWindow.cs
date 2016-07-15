using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

using TDTK;

namespace TDTK {

	public class MakeSceneEditorWindow : UnitEditorWindow {

		private static MakeSceneEditorWindow window;

		public static void Init() {
			 // Get existing open window or if none, make a new one:
			window = (MakeSceneEditorWindow)EditorWindow.GetWindow(typeof (MakeSceneEditorWindow));
			//~ window.minSize=new Vector2(375, 449);
			//~ window.maxSize=new Vector2(375, 800);
		}

		private int mSubjectID = 1;
		private int mLevelID = 1;

		void OnGUI () {
			float fWidth = 35;

			if (window==null) Init();

			float startX = 5;
			float startY = 10;

			if (GUI.Button(new Rect(startX, startY, 60, height + 2), "新建关卡"))
			{
				// 创建场景 
				EditorApplication.NewScene();
				GameObject camObj = Camera.main.gameObject; DestroyImmediate(camObj);

				// 从Prefab中的模板创建场景
				GameObject rootObj = (GameObject)Instantiate(Resources.Load("ScenePrefab/DoraTDLogic", typeof(GameObject)));
				rootObj.name = "TDLogic";
				rootObj = (GameObject)Instantiate(Resources.Load("ScenePrefab/DoraTDCamera", typeof(GameObject)));
				rootObj.name = "Camera";
				rootObj = (GameObject)Instantiate(Resources.Load("ScenePrefab/DoraTDBg", typeof(GameObject)));
				rootObj.name = "Bg";
				rootObj = (GameObject)Instantiate(Resources.Load("ScenePrefab/DoraTDTerrain", typeof(GameObject)));
				rootObj.name = "Terrain";
				rootObj = (GameObject)Instantiate(Resources.Load("ScenePrefab/DoraTDUI", typeof(GameObject)));
				rootObj.name = "UI";
			}

			cont = new GUIContent("主题:", "关卡主题ID");
			EditorGUI.LabelField(new Rect(startX, startY += spaceY * 2, width, height), cont);
			mSubjectID = EditorGUI.IntField(new Rect(startX + 40, startY, fWidth, height), mSubjectID);
			cont = new GUIContent("关卡:", "关卡ID");
			EditorGUI.LabelField(new Rect(startX+100, startY, width, height), cont);
			mLevelID = EditorGUI.IntField(new Rect(startX + 140, startY, fWidth, height), mLevelID);


			if (GUI.Button(new Rect(startX, startY += spaceY * 2, 60, height + 2), "场景导入"))
			{
				if (mSubjectID < 1 || mLevelID < 1)
				{
					if (mSubjectID < 1)
					{
						EditorUtility.DisplayDialog("警告", "请输入正确的主题ID（ID必须大于0）", "确定");
					}
					else
					{
						EditorUtility.DisplayDialog("警告", "请输入正确的关卡ID（ID必须大于0）", "确定");
					}
				}
				else
				{
					string subject = string.Format("{0:D2}", mSubjectID);
					string level = string.Format("{0:D3}", mLevelID);
					string scenePath = Application.dataPath + @"/Prefabs/assetBundle/Levels/Subject" + subject + "/";
					string sceneFile = scenePath + "level"+ level + ".bytes";
					if (!File.Exists(sceneFile))
					{
						EditorUtility.DisplayDialog("警告", "主题" + mSubjectID + "关卡" + mLevelID + " 不存在！", "确定");
					}
					else
					{
						EditorApplication.NewScene();
						GameObject camObj = Camera.main.gameObject; DestroyImmediate(camObj);
						GameObject lightObj = GameObject.Find("Directional Light"); DestroyImmediate(lightObj);

						try
						{
							FileStream fs = new FileStream(sceneFile, FileMode.Open);
							BinaryReader br = new BinaryReader(fs);
							byte[] tempall = br.ReadBytes((int)fs.Length);

							int index = 0;
							while (index < tempall.Length)
							{
								//得到第一个byte 也就是得到字符串的长度
								int objectLength = tempall[index];
								++index;
								byte[] objectName = new byte[objectLength];
								//根据长度拷贝出对应长度的字节数组
								System.Array.Copy(tempall, index, objectName, 0, objectLength);
								index += objectLength;
								//然后把字节数组对应转换成字符串
								string objName = System.Text.Encoding.Default.GetString(objectName);

								//这里和上面原理一样就不赘述
								int prefabPathLength = tempall[index];
								++index;
								byte[] prefabPathName = new byte[prefabPathLength];
								System.Array.Copy(tempall, index, prefabPathName, 0, prefabPathLength);
								index += prefabPathLength;
								string prefabPath = System.Text.Encoding.Default.GetString(prefabPathName);

								byte[] posx = new byte[2];
								System.Array.Copy(tempall, index, posx, 0, posx.Length);
								index += posx.Length;
								float x = System.BitConverter.ToInt16(posx, 0) / 100.0f;

								byte[] posy = new byte[2];
								System.Array.Copy(tempall, index, posy, 0, posy.Length);
								index += posy.Length;
								float y = System.BitConverter.ToInt16(posy, 0) / 100.0f;

								byte[] posz = new byte[2];
								System.Array.Copy(tempall, index, posz, 0, posz.Length);
								index += posz.Length;
								float z = System.BitConverter.ToInt16(posz, 0) / 100.0f;

								byte[] rotx = new byte[2];
								System.Array.Copy(tempall, index, rotx, 0, rotx.Length);
								index += rotx.Length;
								float rx = System.BitConverter.ToInt16(rotx, 0) / 100.0f;

								byte[] roty = new byte[2];
								System.Array.Copy(tempall, index, roty, 0, roty.Length);
								index += roty.Length;
								float ry = System.BitConverter.ToInt16(roty, 0) / 100.0f;

								byte[] rotz = new byte[2];
								System.Array.Copy(tempall, index, rotz, 0, rotz.Length);
								index += rotz.Length;
								float rz = System.BitConverter.ToInt16(rotz, 0) / 100.0f;

								byte[] scax = new byte[2];
								System.Array.Copy(tempall, index, scax, 0, scax.Length);
								index += scax.Length;
								float sx = System.BitConverter.ToInt16(scax, 0) / 100.0f;

								byte[] scay = new byte[2];
								System.Array.Copy(tempall, index, scay, 0, scay.Length);
								index += scay.Length;
								float sy = System.BitConverter.ToInt16(scay, 0) / 100.0f;

								byte[] scaz = new byte[2];
								System.Array.Copy(tempall, index, scaz, 0, scaz.Length);
								index += scaz.Length;
								float sz = System.BitConverter.ToInt16(scaz, 0) / 100.0f;


								GameObject objPref = (GameObject)AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject));
								if (objPref == null)
								{
									continue;
								}
								GameObject obj2 = (GameObject)PrefabUtility.InstantiatePrefab(objPref);
								if (obj2 == null)
								{
									continue;
								}
								obj2.transform.localPosition = new Vector3(x, y, z);
								obj2.transform.localRotation = Quaternion.Euler(new Vector3(rx, ry, rz));
								obj2.transform.localScale = new Vector3(sx, sy, sz);
								obj2.name = objName;

								if (obj2.name == "Terrain")
								{
									tk2dTileMap tileMap = obj2.GetComponentInChildren<tk2dTileMap>();
									if (tileMap != null)
									{

										Dictionary<tk2dRuntime.TileMap.Layer, bool> layersActive = new Dictionary<tk2dRuntime.TileMap.Layer, bool>();
										if (tileMap.Layers != null)
										{
											for (int layerIdx = 0; layerIdx < tileMap.Layers.Length; ++layerIdx)
											{
												tk2dRuntime.TileMap.Layer layer = tileMap.Layers[layerIdx];
												if (layer != null && layer.gameObject != null)
												{
													layersActive[layer] = layer.gameObject.activeSelf;
												}
											}
										}

										tk2dRuntime.TileMap.BuilderUtil.CreateRenderData(tileMap, false, layersActive);

										tk2dRuntime.TileMap.RenderMeshBuilder.Build(tileMap, false, true);
									}
								}
							}

							GameObject go = (GameObject)Instantiate(Resources.Load("ScenePrefab/DoraTDUI", typeof(GameObject)));
							go.name = "UI";
							go = (GameObject)Instantiate(Resources.Load("ScenePrefab/DoraTDCamera", typeof(GameObject)));
							go.name = "Camera";
							GameObject tdPrefab = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Prefabs/assetBundle/Levels/Common/DoraTDLogic.prefab", typeof(GameObject));
                            go = (GameObject)Instantiate(tdPrefab);
							go.name = "TDLogic";

							br.Close();
							fs.Close();
						}
						catch (IOException e)
						{
							Debug.Log("Delete File IOException:" + e.ToString());
							EditorUtility.DisplayDialog("警告", "主题" + mSubjectID + "关卡" + mLevelID + " 场景文件正在被Unity占用，请稍后重试！", "确定");
						}

					}
				}
			}

			if (GUI.Button(new Rect(startX + spaceX, startY, 60, height + 2), "场景导出"))
			{
				string subject = string.Format("{0:D2}", mSubjectID);
				string level = string.Format("{0:D3}", mLevelID);
				string scenePath = @"/Prefabs/assetBundle/Levels/Subject" + subject + "/";
				string sceneFile = scenePath + "level" + level + ".bytes";
				if (!File.Exists(sceneFile) || EditorUtility.DisplayDialog("警告", "主题" + mSubjectID + "关卡" + mLevelID + " 已经存在，是否覆盖？", "确定", "取消"))
				{
					AssetDatabase.SaveAssets();

					if (File.Exists(Application.dataPath + sceneFile))
					{
						//try
						//{
						//	File.Delete(Application.dataPath + sceneFile);
						//}
						//catch (IOException e)
						//{
						//	Debug.Log("Delete File IOException:"+e.ToString());
						//	EditorUtility.DisplayDialog("警告", "主题" + mSubjectID + "关卡" + mLevelID + " 场景文件正在被Unity占用，请稍后重试！", "确定");
						//	return;
						//}
					}

					string commonPath = Application.dataPath + @"/Prefabs/assetBundle/Levels/Common/";
					//string subjectCommonPath = Application.dataPath + @"/Prefabs/assetBundle/Levels/Subject" + subject + "/Common/";
					string levelPath = Application.dataPath + @"/Prefabs/assetBundle/Levels/Subject" + subject + "/";
					if (!Directory.Exists(commonPath))
					{
						Directory.CreateDirectory(commonPath);
					}
					commonPath = @"Assets/Prefabs/assetBundle/Levels/Common/";

					//if (!Directory.Exists(subjectCommonPath))
					//{
					//	Directory.CreateDirectory(subjectCommonPath);
					//}
					//subjectCommonPath = @"Assets/Prefabs/assetBundle/Levels/Subject" + subject + "/Common/";

					if (!Directory.Exists(levelPath))
					{
						Directory.CreateDirectory(levelPath);
					}
					levelPath = @"Assets/Prefabs/assetBundle/Levels/Subject" + subject + "/";

					try
					{
						FileStream fs = new FileStream(Application.dataPath + sceneFile, FileMode.OpenOrCreate);
						fs.SetLength(0);
                        BinaryWriter bw = new BinaryWriter(fs);

						foreach (GameObject obj in UnityEngine.Object.FindObjectsOfType(typeof(GameObject)))
						{
							if (obj.transform.parent == null)
							{
								if (obj.name == "Main Camera" || obj.name == "Camera" || obj.name == "UI" || obj.name == "TDLogic")
								{
									continue;
								}

								bw.Write(obj.name);

								UnityEngine.Object prefab = PrefabUtility.GetPrefabParent(obj);
								if (prefab == null)
								{
									//if (obj.name == "TDLogic")
									//{
									//	bw.Write(commonPath + obj.name + ".prefab");
									//	UnityEngine.Object prefabGo = PrefabUtility.CreateEmptyPrefab(commonPath + obj.name + ".prefab");
									//	PrefabUtility.ReplacePrefab(obj, prefabGo);
									//	EditorUtility.SetDirty(prefabGo);
									//	SetAssetName(commonPath + obj.name + ".prefab", "tdcommon.assetbundle");
									//}
									if (obj.name == "Bg" || obj.name == "Terrain")
									{
										bw.Write(levelPath + obj.name + level + ".prefab");
										UnityEngine.Object prefabGo = PrefabUtility.CreateEmptyPrefab(levelPath + obj.name + level + ".prefab");
										PrefabUtility.ReplacePrefab(obj, prefabGo);
										EditorUtility.SetDirty(prefabGo);
										AssetDatabase.Refresh();
										SetAssetName(levelPath + obj.name + level + ".prefab", "tdsubject"+ subject + ".assetbundle");
									}
								}
								else
								{
									string path = AssetDatabase.GetAssetPath(prefab);
									bw.Write(path);
									SetAssetName(path, "tdsubject" + subject + ".assetbundle");
								}

								bw.Write((short)(obj.transform.position.x * 100.0f));
								bw.Write((short)(obj.transform.position.y * 100.0f));
								bw.Write((short)(obj.transform.position.z * 100.0f));
								bw.Write((short)(obj.transform.rotation.eulerAngles.x * 100.0f));
								bw.Write((short)(obj.transform.rotation.eulerAngles.y * 100.0f));
								bw.Write((short)(obj.transform.rotation.eulerAngles.z * 100.0f));
								bw.Write((short)(obj.transform.localScale.x * 100.0f));
								bw.Write((short)(obj.transform.localScale.y * 100.0f));
								bw.Write((short)(obj.transform.localScale.z * 100.0f));
							}
						}

						bw.Flush();
						bw.Close();
						fs.Close();
						AssetDatabase.SaveAssets();
						AssetDatabase.Refresh();
						SetAssetName("Assets" + sceneFile, "tdsubject" + subject + ".assetbundle");
					}
					catch (IOException e)
					{
						Debug.Log("Delete File IOException:" + e.ToString());
						EditorUtility.DisplayDialog("警告", "主题" + mSubjectID + "关卡" + mLevelID + " 场景文件正在被Unity占用，请稍后重试！", "确定");
						return;
					}

				}
			}
		}

		public void SetAssetName(string path, string name)
		{
			AssetImporter importer = AssetImporter.GetAtPath(path);
			if (importer && importer.assetBundleName != name)
			{
				importer.assetBundleName = name;
			}
		}
	}
}