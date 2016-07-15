using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

using TDTK;

namespace TDTK {

	public class MakeCreepEditorWindow : UnitEditorWindow {

		private static MakeCreepEditorWindow window;
		
		
		public static void Init() {
			 // Get existing open window or if none, make a new one:
			window = (MakeCreepEditorWindow)EditorWindow.GetWindow(typeof (MakeCreepEditorWindow));
			//~ window.minSize=new Vector2(375, 449);
			//~ window.maxSize=new Vector2(375, 800);
			
			EditorDBManager.Init();

			window.mNewCreepName = "";
			//InitLabel();
			//UpdateObjectHierarchyList();
		}
		
		private string mNewCreepName = "";
		private Sprite mNewCreepIcon = null;
		private GameObject mCreepModelObj = null;
		private GameObject mCreepModelAssetObj = null;
		private Avatar mCreepModelAvatar = null;
		private UnitCreep mCreep = null;
		private GameObject mCreepPrefab = null;
		private UnitCreepAnimation mCreepAnim = null;

		void OnGUI () {
			float fWidth = 35;

			if (window==null) Init();

			float startX = 5;
			float startY = 10;

			if (GUI.Button(new Rect(startX, startY, 80, height + 2), "新建怪物"))
			{
				mNewCreepName = "NewCreep";
				mNewCreepIcon = null;
				mCreepModelObj = null;
				mCreepModelAssetObj = null;
				mCreepModelAvatar = null;
				mCreep = null;
				mCreepPrefab = null;
				mCreepAnim = null;
			}

			startY += spaceY + 5;
			if (GUI.Button(new Rect(startX, startY, 80, height + 2), "打开怪物"))
			{
				if (mCreepPrefab != null)
				{
					GameObject openGo = GameObject.Instantiate(mCreepPrefab);
					UnitCreep creep = mCreepPrefab.GetComponent<UnitCreep>();
					if (creep != null)
					{
						foreach (UnitCreep c in EditorDBManager.GetCreepList())
						{
							if (c.prefabID == creep.prefabID)
							{
								mCreep = c;
								break;
                            }
						}

						if (mCreep != null)
						{
							mNewCreepName = mCreep.unitName;
							mNewCreepIcon = mCreep.iconSprite;
							mCreepModelAvatar = null;

							mCreepAnim = openGo.GetComponent<UnitCreepAnimation>();

							UpdateObjectHierarchyList(mCreep);
						}
					}
				}
			}
			cont = new GUIContent("怪物Prefab:", "The Creep ID");
				EditorGUI.LabelField(new Rect(startX+100, startY, width, height), cont);
				mCreepPrefab = (GameObject)EditorGUI.ObjectField(new Rect(startX + 200, startY, 4 * fWidth - 20, height), mCreepPrefab, typeof(GameObject), false);

			if (mNewCreepName != "")
			{
				startY += spaceY+20;
				cont = new GUIContent("1、创建:", "The Creep ID");
				EditorGUI.LabelField(new Rect(startX, startY, width, height), cont);
				startX += 30;
				startY += spaceY;

				cont = new GUIContent("--怪物FBX:", "The Creep ID");
				EditorGUI.LabelField(new Rect(startX, startY, width, height), cont);
				if (mCreep != null)
				{
					Transform creepFbxTrans = mCreep.gameObject.transform.GetChild(0);
					int objID = GetObjectIDFromHList(creepFbxTrans, objHList);
					objID = EditorGUI.Popup(new Rect(startX + spaceX, startY, width, height), objID, objHLabelList);
				}
				else
				{
					mCreepModelAssetObj = (GameObject)EditorGUI.ObjectField(new Rect(startX + spaceX - 30, startY, 4 * fWidth - 20, height), mCreepModelAssetObj, typeof(GameObject), false);
					if ((mCreepModelAssetObj != null) && (mNewCreepName == "NewCreep"))
					{
						mNewCreepName = mCreepModelAssetObj.name;
					}
				}

				startY += spaceY + 5;
				if (mCreep != null)
				{
					EditorUtilities.DrawSprite(new Rect(startX, startY, 60, 60), mCreep.iconSprite);
				}
				else
				{
					EditorUtilities.DrawSprite(new Rect(startX, startY, 60, 60), mNewCreepIcon);
				}
				startX += 65;
				cont = new GUIContent("Name:", "The unit name to be displayed in game");
				EditorGUI.LabelField(new Rect(startX, startY, width, height), cont);
				if (mCreep != null)
				{
					mCreep.unitName = EditorGUI.TextField(new Rect(startX + spaceX - 65, startY, width - 5, height), mCreep.unitName);
				}
				else
				{
					mNewCreepName = EditorGUI.TextField(new Rect(startX + spaceX - 65, startY, width - 5, height), mNewCreepName);
				}
				startY += 5;

				cont = new GUIContent("Icon:", "The unit icon to be displayed in game, must be a sprite");
				EditorGUI.LabelField(new Rect(startX, startY += spaceY, width, height), cont);
				if (mCreep != null)
				{
					mCreep.iconSprite = (Sprite)EditorGUI.ObjectField(new Rect(startX + spaceX - 65, startY, width - 5, height), mCreep.iconSprite, typeof(Sprite), false);
				}
				else
				{
					mNewCreepIcon = (Sprite)EditorGUI.ObjectField(new Rect(startX + spaceX - 65, startY, width - 5, height), mNewCreepIcon, typeof(Sprite), false);
				}

				startX -= 65;
				startY += spaceY + 5;
				if (mCreep == null)
				{
					if (GUI.Button(new Rect(startX + 180, startY, 80, 25), "创建"))
					{
						string pathRef = "Prefabs/assetBundle/Models/Towers" + "/" + mNewCreepName + ".prefab";
						if (File.Exists(Application.dataPath + "/" + pathRef))
						{
							EditorUtility.DisplayDialog("警告", "预制块" + mNewCreepName + "已经存在，请改名字", "确定");
						}
						else
						{
							GameObject oldCreep = GameObject.Find(mNewCreepName);
							if (oldCreep != null)
								DestroyImmediate(oldCreep);
							GameObject newCreep = new GameObject(mNewCreepName);

							mCreepModelObj = GameObject.Instantiate(mCreepModelAssetObj);
							mCreepModelObj.transform.parent = newCreep.transform;
							mCreepModelObj.name = mCreepModelAssetObj.name;

							mCreepModelAvatar = null;
							UnityEngine.Object[] allassets = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(mCreepModelAssetObj));
							foreach (UnityEngine.Object subAsset in allassets)
							{
								mCreepModelAvatar = subAsset as Avatar;
								if (mCreepModelAvatar != null)
									break;
							}

							//ModelImporter turretModelImport = (ModelImporter)AssetImporter.GetAtPath(mTurretObjPath);
							//if (turretModelImport != null)
							//{

							//                turretModelImport.animationType = ModelImporterAnimationType.Generic;
							//	mTurretAvatar = turretModelImport.sourceAvatar;
							//	if (mTurretAvatar == null)
							//	{
							//		mTurretAvatar = AvatarBuilder.BuildGenericAvatar(mTurretObj, "Bip001");
							//		turretModelImport.sourceAvatar = mTurretAvatar;
							//	}
							//}
							mCreep = newCreep.AddComponent<UnitCreep>();
							mCreep.unitName = mNewCreepName;
							mCreep.iconSprite = mNewCreepIcon;

							newCreep.AddComponent<SphereCollider>();

							Animator creepAnimator = newCreep.AddComponent<Animator>();

							AnimatorController animatorController = AssetDatabase.LoadAssetAtPath<AnimatorController>("Assets/Prefabs/prefabResources/Animation/Tower/TowerAnimatorControllerTemplate.controller");
							creepAnimator.runtimeAnimatorController = animatorController;
							creepAnimator.avatar = mCreepModelAvatar;
							creepAnimator.applyRootMotion = true;

							{   // 动画
								mCreepAnim = newCreep.AddComponent<UnitCreepAnimation>();
								mCreepAnim.type = UnitCreepAnimation._AniType.Mecanim;

								string path = AssetDatabase.GetAssetPath(mCreepModelAssetObj);
								string file = Path.GetFileNameWithoutExtension(path);
								string dir = Path.GetDirectoryName(path).Replace("\\", "/");

								// spawn
								{
									string buildAnimPath = dir + "/" + file + "@spawn";
									AnimationClip build = AssetDatabase.LoadAssetAtPath<AnimationClip>(buildAnimPath + ".anim");
									if (build == null)
									{
										UnityEngine.Object[] allassets1 = AssetDatabase.LoadAllAssetsAtPath(buildAnimPath + ".FBX");
										foreach (UnityEngine.Object subAsset in allassets1)
										{
											build = subAsset as AnimationClip;
											if (build != null)
												break;
										}
										if (build != null)
										{
											AnimationClip newClip = new AnimationClip();
											EditorUtility.CopySerialized(build, newClip);
											AssetDatabase.CreateAsset(newClip, buildAnimPath + ".anim");
											build = newClip;
										}
									}
									AssetDatabase.DeleteAsset(buildAnimPath + ".FBX");
									AssetDatabase.Refresh();

									if (build != null)
									{
										mCreepAnim.clipSpawn = build;
									}
								}
								// dest
								{
									string buildAnimPath = dir + "/" + file + "@dest";
									AnimationClip build = AssetDatabase.LoadAssetAtPath<AnimationClip>(buildAnimPath + ".anim");
									if (build == null)
									{
										UnityEngine.Object[] allassets1 = AssetDatabase.LoadAllAssetsAtPath(buildAnimPath + ".FBX");
										foreach (UnityEngine.Object subAsset in allassets1)
										{
											build = subAsset as AnimationClip;
											if (build != null)
												break;
										}
										if (build != null)
										{
											AnimationClip newClip = new AnimationClip();
											EditorUtility.CopySerialized(build, newClip);
											AssetDatabase.CreateAsset(newClip, buildAnimPath + ".anim");
											build = newClip;
										}
									}
									AssetDatabase.DeleteAsset(buildAnimPath + ".FBX");
									AssetDatabase.Refresh();

									if (build != null)
									{
										mCreepAnim.clipDestination = build;
									}
								}
								// dead
								{
									string buildAnimPath = dir + "/" + file + "@dead";
									AnimationClip build = AssetDatabase.LoadAssetAtPath<AnimationClip>(buildAnimPath + ".anim");
									if (build == null)
									{
										UnityEngine.Object[] allassets1 = AssetDatabase.LoadAllAssetsAtPath(buildAnimPath + ".FBX");
										foreach (UnityEngine.Object subAsset in allassets1)
										{
											build = subAsset as AnimationClip;
											if (build != null)
												break;
										}
										if (build != null)
										{
											AnimationClip newClip = new AnimationClip();
											EditorUtility.CopySerialized(build, newClip);
											AssetDatabase.CreateAsset(newClip, buildAnimPath + ".anim");
											build = newClip;
										}
									}
									AssetDatabase.DeleteAsset(buildAnimPath + ".FBX");
									AssetDatabase.Refresh();

									if (build != null)
									{
										mCreepAnim.clipDead = build;
									}
								}
								// hit
								{
									string buildAnimPath = dir + "/" + file + "@hit";
									AnimationClip build = AssetDatabase.LoadAssetAtPath<AnimationClip>(buildAnimPath + ".anim");
									if (build == null)
									{
										UnityEngine.Object[] allassets1 = AssetDatabase.LoadAllAssetsAtPath(buildAnimPath + ".FBX");
										foreach (UnityEngine.Object subAsset in allassets1)
										{
											build = subAsset as AnimationClip;
											if (build != null)
												break;
										}
										if (build != null)
										{
											AnimationClip newClip = new AnimationClip();
											EditorUtility.CopySerialized(build, newClip);
											AssetDatabase.CreateAsset(newClip, buildAnimPath + ".anim");
											build = newClip;
										}
									}
									AssetDatabase.DeleteAsset(buildAnimPath + ".FBX");
									AssetDatabase.Refresh();

									if (build != null)
									{
										mCreepAnim.clipHit = build;
									}
								}

								// move
								{
									string buildAnimPath = dir + "/" + file + "@move";
									AnimationClip build = AssetDatabase.LoadAssetAtPath<AnimationClip>(buildAnimPath + ".anim");
									if (build == null)
									{
										UnityEngine.Object[] allassets1 = AssetDatabase.LoadAllAssetsAtPath(buildAnimPath + ".FBX");
										foreach (UnityEngine.Object subAsset in allassets1)
										{
											build = subAsset as AnimationClip;
											if (build != null)
												break;
										}
										if (build != null)
										{
											AnimationClip newClip = new AnimationClip();
											EditorUtility.CopySerialized(build, newClip);
											//设置idle文件为循环动画
											//Debug.Log("loop:"+ newClip.wrapMode);
											//newClip.wrapMode = WrapMode.Loop; //no use
											//Debug.Log("loop2:" + newClip.wrapMode);
											SerializedObject serializedClip = new SerializedObject(newClip);
											AnimationClipSettings clipSettings = new AnimationClipSettings(serializedClip.FindProperty("m_AnimationClipSettings"));
											clipSettings.loopTime = true;
											serializedClip.ApplyModifiedProperties();

											AssetDatabase.CreateAsset(newClip, buildAnimPath + ".anim");
											build = newClip;
										}
									}
									AssetDatabase.DeleteAsset(buildAnimPath + ".FBX");
									AssetDatabase.Refresh();

									if (build != null)
									{
										mCreepAnim.clipMove = build;
									}
								}


							}


								mCreepPrefab = PrefabUtility.CreatePrefab("Assets/" + pathRef, mCreep.gameObject);

								EditorUtility.SetDirty(mCreepPrefab);   // Save
								AssetDatabase.SaveAssets();

								UpdateObjectHierarchyList(mCreep);

								SetAssetName("Assets/" + pathRef, "monsters.assetbundle");

						}

					}

				}

				if (mCreep != null)
				{
					startY += spaceY + 20;
					startX -= 30;

					cont = new GUIContent("2、设置:", "The Creep ID");
					EditorGUI.LabelField(new Rect(startX, startY, width, height), cont);
					startY += spaceY + 5;

					startX += 30;
					int objID = GetObjectIDFromHList(mCreep.targetPoint, objHList);
					cont = new GUIContent("被攻击点:", "The transform object which indicate the center point of the unit\nThis would be the point where the shootObject and effect will be aiming at");
					EditorGUI.LabelField(new Rect(startX, startY += spaceY, width, height), cont);
					objID = EditorGUI.Popup(new Rect(startX + spaceX, startY, width, height), objID, objHLabelList);
					mCreep.targetPoint = (objHList[objID] == null) ? null : objHList[objID].transform;


					startY += spaceY + 20;
					startX -= 30;
					cont = new GUIContent("3、动画:", "The Creep ID");
					EditorGUI.LabelField(new Rect(startX, startY, width, height), cont);

					if (mCreepAnim != null)
					{
						startY += 5;
						startX += 30;

						cont = new GUIContent("出生动画:", "The animation clip to be played when the creep is destroyed");
						EditorGUI.LabelField(new Rect(startX, startY += spaceY, width, height), cont);
						mCreepAnim.clipSpawn = (AnimationClip)EditorGUI.ObjectField(new Rect(startX + spaceX, startY, width, height), mCreepAnim.clipSpawn, typeof(AnimationClip), false);

						cont = new GUIContent("到达终点动画:", "The animation clip to be played when the creep is moving");
						EditorGUI.LabelField(new Rect(startX, startY += spaceY, width, height), cont);
						mCreepAnim.clipDestination = (AnimationClip)EditorGUI.ObjectField(new Rect(startX + spaceX, startY, width, height), mCreepAnim.clipDestination, typeof(AnimationClip), false);

						cont = new GUIContent("死亡动画:", "The animation clip to be played when the creep is destroyed");
						EditorGUI.LabelField(new Rect(startX, startY += spaceY, width, height), cont);
						mCreepAnim.clipDead = (AnimationClip)EditorGUI.ObjectField(new Rect(startX + spaceX, startY, width, height), mCreepAnim.clipDead, typeof(AnimationClip), false);

						cont = new GUIContent("移动动画:", "The animation clip to be played when the creep reach its destination");
						EditorGUI.LabelField(new Rect(startX, startY += spaceY, width, height), cont);
						mCreepAnim.clipMove = (AnimationClip)EditorGUI.ObjectField(new Rect(startX + spaceX, startY, width, height), mCreepAnim.clipMove, typeof(AnimationClip), false);

						cont = new GUIContent("攻击动画（可选）:", "The animation clip to be played when the creep reach its destination");
						EditorGUI.LabelField(new Rect(startX, startY += spaceY, width, height), cont);
						mCreepAnim.clipHit = (AnimationClip)EditorGUI.ObjectField(new Rect(startX + spaceX, startY, width, height), mCreepAnim.clipHit, typeof(AnimationClip), false);
					}

					startY += spaceY + 20;
					if (GUI.Button(new Rect(startX + spaceX - 30, startY, 80, height + 2), "保存"))
					{
						if (mCreep != null && mCreep.prefabID == -1)
						{
							mCreepPrefab = PrefabUtility.ReplacePrefab(mCreep.gameObject, mCreepPrefab);

							EditorUtility.SetDirty(mCreepPrefab);   // Save
							AssetDatabase.SaveAssets();
							AssetDatabase.Refresh();

							UnitCreep creep = mCreepPrefab.GetComponent<UnitCreep>();

							int index = EditorDBManager.AddNewCreep(creep);
							EditorDBManager.SetDirtyCreep();

							mCreep = EditorDBManager.GetCreepList()[index];
       //                     GameObject openGo = GameObject.Instantiate(mCreepPrefab);
							//mCreep = openGo.GetComponent<UnitCreep>();
							//mNewCreepName = mCreep.unitName;
							//mNewCreepIcon = mCreep.iconSprite;
							//mCreepModelAvatar = null;
							//mCreepAnim = openGo.GetComponent<UnitCreepAnimation>();

							UpdateObjectHierarchyList(mCreep);

						}
						else
						{
							EditorUtility.SetDirty(mCreep);   // Save
						}

						AssetDatabase.SaveAssets();
					}
				}
			}

		}

		private static List<GameObject> objHList = new List<GameObject>();
		private static string[] objHLabelList = new string[0];
		private static void UpdateObjectHierarchyList(UnitCreep tower)
		{ 
			if (tower != null)
			{
				EditorUtilities.GetObjectHierarchyList(tower.gameObject, SetObjListCallback);
			}
		}
		public static void SetObjListCallback(List<GameObject> objList, string[] labelList)
		{
			objHList = objList;
			objHLabelList = labelList;
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