using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

using TDTK;

namespace TDTK {

	public class MakeTowerEditorWindow : UnitEditorWindow {

		private static MakeTowerEditorWindow window;
		
		
		public static void Init() {
			 // Get existing open window or if none, make a new one:
			window = (MakeTowerEditorWindow)EditorWindow.GetWindow(typeof (MakeTowerEditorWindow));
			//~ window.minSize=new Vector2(375, 449);
			//~ window.maxSize=new Vector2(375, 800);
			
			EditorDBManager.Init();

			window.mNewTowerName = "";
			//InitLabel();
			//UpdateObjectHierarchyList();
		}
		
		private string mNewTowerName = "";
		private Sprite mNewTowerIcon = null;
		private GameObject mTurretObj = null;
		private GameObject mTurretAssetObj = null;
		private Avatar mTurretAvatar = null;
		private GameObject mBaseObj = null;
		private UnitTower mTower = null;
		private GameObject mTowerPrefab = null;
		private UnitTowerAnimation mTowerAnim = null;

		void OnGUI () {
			float fWidth = 35;

			if (window==null) Init();

			float startX = 5;
			float startY = 10;

			if (GUI.Button(new Rect(startX, startY, 80, height + 2), "新建炮塔"))
			{
				mNewTowerName = "NewTower";
				mNewTowerIcon = null;
				mTurretObj = null;
				mTurretAssetObj = null;
				mTurretAvatar = null;
				mBaseObj = null;
				mTower = null;
				mTowerPrefab = null;
				mTowerAnim = null;
			}

			startY += spaceY + 5;
			if (GUI.Button(new Rect(startX, startY, 80, height + 2), "打开炮塔"))
			{
				if (mTowerPrefab != null)
				{
					GameObject openGo = GameObject.Instantiate(mTowerPrefab);
					UnitTower tower = mTowerPrefab.GetComponent<UnitTower>();
					if (tower != null)
					{
						foreach (UnitTower t in EditorDBManager.GetTowerList())
						{
							if (t.prefabID == tower.prefabID)
							{
								mTower = t;
								break;
							}
						}

						if (mTower != null)
						{
							mNewTowerName = mTower.unitName;
							mNewTowerIcon = mTower.iconSprite;
							mTurretObj = mTower.turretObject.gameObject;
							mTurretAvatar = null;
							Transform baseTrans = mTower.transform.Find("BaseObj");
							if (baseTrans != null)
								mBaseObj = baseTrans.gameObject;

							mTowerAnim = openGo.GetComponent<UnitTowerAnimation>();

							UpdateObjectHierarchyList(mTower);
						}
					}
				}
			}

			cont = new GUIContent("炮塔Prefab:", "The Creep ID");
			EditorGUI.LabelField(new Rect(startX + 100, startY, width, height), cont);
			mTowerPrefab = (GameObject)EditorGUI.ObjectField(new Rect(startX + 200, startY, 4 * fWidth - 20, height), mTowerPrefab, typeof(GameObject), false);

			if (mNewTowerName != "")
			{
				startY += spaceY+20;
				cont = new GUIContent("1、创建:", "The Creep ID");
				EditorGUI.LabelField(new Rect(startX, startY, width, height), cont);
				startX += 30;
				startY += spaceY;

				cont = new GUIContent("--炮身FBX:", "The Creep ID");
				EditorGUI.LabelField(new Rect(startX, startY, width, height), cont);
				if (mTower != null)
				{
					int objID = GetObjectIDFromHList(mTower.turretObject, objHList);
					objID = EditorGUI.Popup(new Rect(startX + spaceX, startY, width, height), objID, objHLabelList);
					mTower.turretObject = (objHList[objID] == null) ? null : objHList[objID].transform;
				}
				else
				{
					mTurretAssetObj = (GameObject)EditorGUI.ObjectField(new Rect(startX + spaceX, startY, 4 * fWidth - 20, height), mTurretAssetObj, typeof(GameObject), false);
					if ((mTurretAssetObj != null) && (mNewTowerName == "NewTower"))
					{
						mNewTowerName = mTurretAssetObj.name;
					}
				}

				startY += spaceY + 5;
				cont = new GUIContent("--炮座FBX（可选）:", "The Creep ID");
				EditorGUI.LabelField(new Rect(startX, startY, width, height), cont);
				mBaseObj = (GameObject)EditorGUI.ObjectField(new Rect(startX + spaceX, startY, 4 * fWidth - 20, height), mBaseObj, typeof(GameObject), false);
				startY += spaceY + 5;

				if (mTower != null)
				{
					EditorUtilities.DrawSprite(new Rect(startX, startY, 60, 60), mTower.iconSprite);
				}
				else
				{
					EditorUtilities.DrawSprite(new Rect(startX, startY, 60, 60), mNewTowerIcon);
				}
				startX += 65;
				cont = new GUIContent("Name:", "The unit name to be displayed in game");
				EditorGUI.LabelField(new Rect(startX, startY, width, height), cont);
				if (mTower != null)
				{
					mTower.unitName = EditorGUI.TextField(new Rect(startX + spaceX - 65, startY, width - 5, height), mTower.unitName);
				}
				else
				{
					mNewTowerName = EditorGUI.TextField(new Rect(startX + spaceX - 65, startY, width - 5, height), mNewTowerName);
				}
				startY += 5;

				cont = new GUIContent("Icon:", "The unit icon to be displayed in game, must be a sprite");
				EditorGUI.LabelField(new Rect(startX, startY += spaceY, width, height), cont);
				if (mTower != null)
				{
					mTower.iconSprite = (Sprite)EditorGUI.ObjectField(new Rect(startX + spaceX - 65, startY, width - 5, height), mTower.iconSprite, typeof(Sprite), false);
				}
				else
				{
					mNewTowerIcon = (Sprite)EditorGUI.ObjectField(new Rect(startX + spaceX - 65, startY, width - 5, height), mNewTowerIcon, typeof(Sprite), false);
				}

				startX -= 65;
				startY += spaceY + 5;
				if (mTower == null)
				{
					if (GUI.Button(new Rect(startX + 180, startY, 80, 25), "创建"))
					{
						string pathRef = "Prefabs/assetBundle/Models/Towers" + "/" + mNewTowerName + ".prefab";
						if (File.Exists(Application.dataPath + "/" + pathRef))
						{
							EditorUtility.DisplayDialog("警告", "预制块" + mNewTowerName + "已经存在，请改名字", "确定");
						}
						else
						{
							GameObject oldTower = GameObject.Find(mNewTowerName);
							if (oldTower != null)
								DestroyImmediate(oldTower);
							GameObject newTower = new GameObject(mNewTowerName);


							//if (mTurretObj != null)
							//{
							GameObject turretObj = new GameObject("TurretObj");
							turretObj.transform.parent = newTower.transform;

							mTurretObj = GameObject.Instantiate(mTurretAssetObj);
							mTurretObj.transform.parent = turretObj.transform;
							mTurretObj.name = mTurretAssetObj.name;
							//}

							if (mBaseObj != null)
							{
								GameObject baseObj = new GameObject("BaseObj");
								baseObj.transform.parent = newTower.transform;

								GameObject go = GameObject.Instantiate(mBaseObj);
								go.transform.parent = baseObj.transform;
							}

							Bounds b = new Bounds();
							Renderer[] renders = newTower.GetComponentsInChildren<Renderer>();
							foreach (Renderer r in renders)
							{
								b.Encapsulate(r.bounds);
							}
							Renderer render = newTower.GetComponent<Renderer>();
							BoxCollider box = newTower.AddComponent<BoxCollider>();
							box.center = b.center;
							box.size = b.size;

							mTurretAvatar = null;
							UnityEngine.Object[] allassets = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(mTurretAssetObj));
							foreach (UnityEngine.Object subAsset in allassets)
							{
								mTurretAvatar = subAsset as Avatar;
								if (mTurretAvatar != null)
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
							mTower = newTower.AddComponent<UnitTower>();
							mTower.unitName = mNewTowerName;
							mTower.stats.Add(new UnitStat());
							List<Rsc> rscList = EditorDBManager.GetRscList();

							if (mTower.stats[0].cost.Count != rscList.Count)
							{
								while (mTower.stats[0].cost.Count > rscList.Count) mTower.stats[0].cost.RemoveAt(mTower.stats[0].cost.Count - 1);
								while (mTower.stats[0].cost.Count < rscList.Count) mTower.stats[0].cost.Add(0);
							}

							mTower.shootPoints.Add(mTurretObj.transform);

							mTower.iconSprite = mNewTowerIcon;
							mTower.turretObject = mTurretObj.transform;
							mTower.rotateTurretAimInXAxis = false;

							Animator towerAnimator = newTower.AddComponent<Animator>();

							AnimatorController animatorController = AssetDatabase.LoadAssetAtPath<AnimatorController>("Assets/Prefabs/prefabResources/Animation/Tower/TowerAnimatorControllerTemplate.controller");
							towerAnimator.runtimeAnimatorController = animatorController;
							towerAnimator.avatar = mTurretAvatar;
							towerAnimator.applyRootMotion = true;

							{   // 动画
								mTowerAnim = newTower.AddComponent<UnitTowerAnimation>();
								mTowerAnim.tower = mTower;

								string path = AssetDatabase.GetAssetPath(mTurretAssetObj);
								string file = Path.GetFileNameWithoutExtension(path);
								string dir = Path.GetDirectoryName(path).Replace("\\", "/");

								// build
								{
									string buildAnimPath = dir + "/" + file + "@build";
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
										mTowerAnim.clipConstruct = build;
									}
								}
								// upgrade
								{
									string buildAnimPath = dir + "/" + file + "@upgrade";
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
										mTowerAnim.clipUpgrade = build;
									}
								}

								// unbuild
								{
									string buildAnimPath = dir + "/" + file + "@unbuild";
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
										mTowerAnim.clipDeconstruct = build;
									}
								}
								// shoot
								{
									string buildAnimPath = dir + "/" + file + "@shoot";
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
										mTowerAnim.clipShoot = build;
									}
								}
								// idle
								{
									string buildAnimPath = dir + "/" + file + "@idle";
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
										mTowerAnim.clipIdle = build;
									}
								}


							}

							mTowerPrefab = PrefabUtility.CreatePrefab("Assets/" + pathRef, mTower.gameObject);

							EditorUtility.SetDirty(mTowerPrefab);   // Save
							AssetDatabase.SaveAssets();

							UpdateObjectHierarchyList(mTower);

							SetAssetName("Assets/" + pathRef, "towers.assetbundle");
                        }
					}

				}

				if (mTower != null)
				{
					startY += spaceY + 20;
					startX -= 30;

					cont = new GUIContent("2、设置:", "The Creep ID");
					EditorGUI.LabelField(new Rect(startX, startY, width, height), cont);
					startY += spaceY + 5;

					startX += 30;
					cont = new GUIContent("--发射子弹:", "The shootObject used by the unit.\nUnit that intended to shoot at the target will not function correctly if this is left unassigned.");
					EditorGUI.LabelField(new Rect(startX, startY, width, height), cont);
					mTower.stats[0].shootObject = (ShootObject)EditorGUI.ObjectField(new Rect(startX + spaceX - 30, startY, 4 * fWidth - 20, height), mTower.stats[0].shootObject, typeof(ShootObject), false);
					startY += spaceY + 5;

					cont = new GUIContent("--子弹发射口:", "The transform which indicate the position where the shootObject will be fired from (Optional)\nEach shootPoint assigned will fire a shootObject instance in each attack\nIf left empty, the unit transform itself will be use as the shootPoint\nThe orientation of the shootPoint matter as they dictate the orientation of the shootObject starting orientation.\n");
					shootPointFoldout = EditorGUI.Foldout(new Rect(startX, startY, spaceX, height), shootPointFoldout, cont);
					int shootPointCount = mTower.shootPoints.Count;
					shootPointCount = EditorGUI.IntField(new Rect(startX + spaceX, startY, 40, height), shootPointCount);

					if (shootPointCount != mTower.shootPoints.Count)
					{
						while (mTower.shootPoints.Count < shootPointCount) mTower.shootPoints.Add(null);
						while (mTower.shootPoints.Count > shootPointCount) mTower.shootPoints.RemoveAt(mTower.shootPoints.Count - 1);
					}
					if (shootPointFoldout)
					{
						for (int i = 0; i < mTower.shootPoints.Count; i++)
						{
							int objID1 = GetObjectIDFromHList(mTower.shootPoints[i], objHList);
							EditorGUI.LabelField(new Rect(startX, startY += spaceY, width, height), "    - Element " + (i + 1));
							objID1 = EditorGUI.Popup(new Rect(startX + spaceX, startY, width, height), objID1, objHLabelList);
							mTower.shootPoints[i] = (objHList[objID1] == null) ? null : objHList[objID1].transform;
						}
					}
					if (mTower.shootPoints.Count > 1)
					{
						cont = new GUIContent("Shots delay Between ShootPoint:", "Delay in second between shot fired at each shootPoint. When set to zero all shootPoint fire simulteneously");
						EditorGUI.LabelField(new Rect(startX, startY += spaceY, width + 60, height), cont);
						mTower.delayBetweenShootPoint = EditorGUI.FloatField(new Rect(startX + spaceX + 90, startY - 1, 55, height - 1), mTower.delayBetweenShootPoint);
					}

					startY += spaceY + 20;
					startX -= 30;
					cont = new GUIContent("3、动画:", "The Creep ID");
					EditorGUI.LabelField(new Rect(startX, startY, width, height), cont);

					if (mTowerAnim != null)
					{
						startY += 5;
						startX += 30;

						cont = new GUIContent("建造动画:", "The animation clip to be played when the creep is destroyed");
						EditorGUI.LabelField(new Rect(startX, startY += spaceY, width, height), cont);
						mTowerAnim.clipConstruct = (AnimationClip)EditorGUI.ObjectField(new Rect(startX + spaceX, startY, width, height), mTowerAnim.clipConstruct, typeof(AnimationClip), false);

						cont = new GUIContent("升级动画:", "The animation clip to be played when the creep is destroyed");
						EditorGUI.LabelField(new Rect(startX, startY += spaceY, width, height), cont);
						mTowerAnim.clipUpgrade = (AnimationClip)EditorGUI.ObjectField(new Rect(startX + spaceX, startY, width, height), mTowerAnim.clipUpgrade, typeof(AnimationClip), false);

						cont = new GUIContent("拆除动画:", "The animation clip to be played when the creep is moving");
						EditorGUI.LabelField(new Rect(startX, startY += spaceY, width, height), cont);
						mTowerAnim.clipDeconstruct = (AnimationClip)EditorGUI.ObjectField(new Rect(startX + spaceX, startY, width, height), mTowerAnim.clipDeconstruct, typeof(AnimationClip), false);

						cont = new GUIContent("发射动画:", "The animation clip to be played when the creep is destroyed");
						EditorGUI.LabelField(new Rect(startX, startY += spaceY, width, height), cont);
						mTowerAnim.clipShoot = (AnimationClip)EditorGUI.ObjectField(new Rect(startX + spaceX, startY, width, height), mTowerAnim.clipShoot, typeof(AnimationClip), false);

						cont = new GUIContent("待机动画:", "The animation clip to be played when the creep reach its destination");
						EditorGUI.LabelField(new Rect(startX, startY += spaceY, width, height), cont);
						mTowerAnim.clipIdle = (AnimationClip)EditorGUI.ObjectField(new Rect(startX + spaceX, startY, width, height), mTowerAnim.clipIdle, typeof(AnimationClip), false);

						cont = new GUIContent("发射延迟:", "The animation clip to be played when the creep reach its destination");
						EditorGUI.LabelField(new Rect(startX, startY += spaceY, width, height), cont);
						mTowerAnim.shootDelay = EditorGUI.FloatField(new Rect(startX + 90, startY, width - 5, height), mTowerAnim.shootDelay);

					}

					startY += spaceY + 20;
					if (GUI.Button(new Rect(startX + spaceX - 30, startY, 80, height + 2), "保存"))
					{
						if (mTower != null && mTower.prefabID == -1)
						{
							mTowerPrefab = PrefabUtility.ReplacePrefab(mTower.gameObject, mTowerPrefab);

							EditorUtility.SetDirty(mTowerPrefab);   // Save
							AssetDatabase.SaveAssets();
							AssetDatabase.Refresh();

							UnitTower tower = mTowerPrefab.GetComponent<UnitTower>();

							int index = EditorDBManager.AddNewTower(tower);
							EditorDBManager.SetDirtyTower();

							mTower = EditorDBManager.GetTowerList()[index];

							UpdateObjectHierarchyList(mTower);

						}
						else
						{
							EditorUtility.SetDirty(mTower);   // Save
						}

						AssetDatabase.SaveAssets();
					}
				}
			}

		}

		private static List<GameObject> objHList = new List<GameObject>();
		private static string[] objHLabelList = new string[0];
		private static void UpdateObjectHierarchyList(UnitTower tower)
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