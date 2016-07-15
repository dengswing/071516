using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

using TDTK;

namespace TDTK {

	public class MakeShootEditorWindow : UnitEditorWindow {

		private static MakeShootEditorWindow window;

		private static bool init = false;
		private static string[] typeLabel = new string[4];
		private static string[] typeTooltip = new string[4];
		private GUIContent[] contList;
		private static bool showLineRendererList = false;



		public static void Init() {
			 // Get existing open window or if none, make a new one:
			window = (MakeShootEditorWindow)EditorWindow.GetWindow(typeof (MakeShootEditorWindow));
			//~ window.minSize=new Vector2(375, 449);
			//~ window.maxSize=new Vector2(375, 800);

			init = true;
			//public enum _ShootObjectType{Projectile, Missile, Beam, Effect, FPSRaycast, FPSDirect}
			int enumLength = Enum.GetValues(typeof(_ShootObjectType)).Length;
			typeLabel = new string[enumLength-3];
			typeTooltip = new string[enumLength-3];
			for (int i = 0; i < enumLength; i++)
			{
				if ((_ShootObjectType)i == _ShootObjectType.Projectile)
				{
					typeLabel[i] = "抛射物";
					typeTooltip[i] = "抛射物: 最常见的子弹";
				}
				if ((_ShootObjectType)i == _ShootObjectType.Missile)
				{
					typeLabel[i] = "导弹";
					typeTooltip[i] = "导弹：跟抛射物不同的是轨迹随即并且会突然转向";
				}
				if ((_ShootObjectType)i == _ShootObjectType.Beam)
				{
					typeLabel[i] = "激光";
					typeTooltip[i] = "激光";
				}
				if ((_ShootObjectType)i == _ShootObjectType.Effect)
				{
					typeLabel[i] = "效果";
					typeTooltip[i] = "效果：没有轨迹，立刻命中播放特效";
				}
				//if ((_ShootObjectType)i == _ShootObjectType.FPSProjectile)
				//	typeTooltip[i] = "Projectile type shootObject used in First-Person-Shooter mode. Only travel in straight line. Require trigger collider and rigidbody to detect collision with in game object";
				//if ((_ShootObjectType)i == _ShootObjectType.FPSProjectile)
				//	typeTooltip[i] = "Beam type shootObject used in First-Person-Shooter mode. Uses a spherecast to detect if it hits target. The LineRenderer component must use local-space to work properly";
				//if ((_ShootObjectType)i == _ShootObjectType.FPSProjectile)
				//	typeTooltip[i] = "Effect type shootObject used in First-Person-Shooter mode. Uses a spherecast to detect if it hits target.";
			}
		}

		private string mShootName;
		private GameObject mShootModelObj = null;
		private GameObject mShootModelAssetObj = null;
		private ShootObject mShoot = null;
		private GameObject mShootPrefab = null;

		void OnGUI () {
			float fWidth = 35;

			if (window==null) Init();

			float startX = 5;
			float startY = 10;

			if (GUI.Button(new Rect(startX, startY, 80, height + 2), "新建子弹"))
			{
				mShootModelObj = null;
				mShootModelAssetObj = null;
				mShoot = null;
				mShootPrefab = null;
				mShootName = "NewShoot";
            }

			startY += spaceY + 5;
			if (GUI.Button(new Rect(startX, startY, 80, height + 2), "打开子弹"))
			{
				if (mShootPrefab != null)
				{
					GameObject openGo = GameObject.Instantiate(mShootPrefab);
					mShoot = mShootPrefab.GetComponent<ShootObject>();
					mShootName = mShootPrefab.name;

				}
			}
			cont = new GUIContent("子弹Prefab:", "The Creep ID");
				EditorGUI.LabelField(new Rect(startX+100, startY, width, height), cont);
				mShootPrefab = (GameObject)EditorGUI.ObjectField(new Rect(startX + 200, startY, 4 * fWidth - 20, height), mShootPrefab, typeof(GameObject), false);

			if (mShootName != null)
			{
				startY += spaceY+20;
				cont = new GUIContent("1、创建:", "The Creep ID");
				EditorGUI.LabelField(new Rect(startX, startY, width, height), cont);
				startX += 30;
				startY += spaceY;

				cont = new GUIContent("--子弹FBX:", "The Creep ID");
				EditorGUI.LabelField(new Rect(startX, startY, width, height), cont);
				if (mShootPrefab != null)
				{
					cont = new GUIContent(mShootPrefab.name, "The Creep ID");
					EditorGUI.LabelField(new Rect(startX + spaceX, startY, width, height), cont);
				}
				else
				{
					mShootModelAssetObj = (GameObject)EditorGUI.ObjectField(new Rect(startX + spaceX - 30, startY, 4 * fWidth - 20, height), mShootModelAssetObj, typeof(GameObject), false);
					if ((mShootModelAssetObj != null) && (mShootName == "NewShoot"))
					{
						mShootName = mShootModelAssetObj.name;
					}
				}

				startY += spaceY;
				cont = new GUIContent("Name:", "The unit name to be displayed in game");
				EditorGUI.LabelField(new Rect(startX, startY, width, height), cont);
				if (mShoot != null)
				{
					mShoot.name = EditorGUI.TextField(new Rect(startX + spaceX - 65, startY, width - 5, height), mShoot.name);
				}
				else
				{
					mShootName = EditorGUI.TextField(new Rect(startX + spaceX - 65, startY, width - 5, height), mShootName);
				}



				startY += spaceY + 5;
				if (mShoot == null)
				{
					if (GUI.Button(new Rect(startX + 180, startY, 80, 25), "创建"))
					{
						string pathRef = "Prefabs/assetBundle/Models/shoots" + "/" + mShootName + ".prefab";
						if (File.Exists(Application.dataPath + "/" + pathRef))
						{
							EditorUtility.DisplayDialog("警告", "预制块" + mShootName + "已经存在，请改名字", "确定");
						}
						else
						{
							GameObject go = GameObject.Instantiate(mShootModelAssetObj);
							go.name = mShootName;
							mShoot = go.AddComponent<ShootObject>();

							mShootPrefab = PrefabUtility.CreatePrefab("Assets/" + pathRef, mShoot.gameObject);

							EditorUtility.SetDirty(mShootPrefab);   // Save
							AssetDatabase.SaveAssets();

							SetAssetName("Assets/" + pathRef, "shoots.assetbundle");

						}

					}

				}

				if (mShoot != null)
				{
					startY += spaceY + 20;
					startX -= 30;

					cont = new GUIContent("2、设置:", "The Creep ID");
					EditorGUI.LabelField(new Rect(startX, startY, width, height), cont);
					startX += 30;

					int type = (int)mShoot.type;
					cont = new GUIContent("类型:", "Type of the shootObject, each shootObject type works different from another, covering various requirement");
					EditorGUI.LabelField(new Rect(startX, startY += spaceY, width, height), cont);
					contList = new GUIContent[typeLabel.Length];
					for (int i = 0; i < contList.Length; i++) contList[i] = new GUIContent(typeLabel[i], typeTooltip[i]);
					type = EditorGUI.Popup(new Rect(startX + 80, startY, width - 40, 15), new GUIContent(""), type, contList);
					mShoot.type = (_ShootObjectType)type;

					if (mShoot.type == _ShootObjectType.Projectile || mShoot.type == _ShootObjectType.Missile || mShoot.type == _ShootObjectType.FPSProjectile)
					{
                        cont = new GUIContent("速度:", "The travel speed of the shootObject");
						EditorGUI.LabelField(new Rect(startX, startY += spaceY, width, height), cont);
						mShoot.speed = EditorGUI.FloatField(new Rect(startX + spaceX, startY, 50, height), mShoot.speed);
					}

					if (mShoot.type == _ShootObjectType.Projectile)
					{
						cont = new GUIContent("Max Shoot Elevation:", "The maximum elevation at which the shootObject will be fired. The firing elevation depends on the target distance. The further the target, the higher the elevation. ");
						EditorGUI.LabelField(new Rect(startX, startY += spaceY, width, height), cont);
						mShoot.maxShootAngle = EditorGUI.FloatField(new Rect(startX + spaceX, startY, 50, height), mShoot.maxShootAngle);

						cont = new GUIContent("Max Shoot Range:", "The maximum range of the shootObject. This is used to govern the elevation, not the actual range limit. When a target exceed this distance, the shootObject will be fired at the maximum elevation");
						EditorGUI.LabelField(new Rect(startX, startY += spaceY, width, height), cont);
						mShoot.maxShootAngle = EditorGUI.FloatField(new Rect(startX + spaceX, startY, 50, height), mShoot.maxShootAngle);
					}
					else if (mShoot.type == _ShootObjectType.Missile)
					{
						cont = new GUIContent("Max Shoot Angle X:", "The maximum elevation at which the shootObject will be fired. The shoot angle in x-axis will not exceed specified value.");
						EditorGUI.LabelField(new Rect(startX, startY += spaceY, width, height), cont);
						mShoot.maxShootAngle = EditorGUI.FloatField(new Rect(startX + spaceX, startY, 50, height), mShoot.maxShootAngle);

						cont = new GUIContent("Max Shoot Angle Y:", "The maximum shoot angle in y-axis (horizontal).");
						EditorGUI.LabelField(new Rect(startX, startY += spaceY, width, height), cont);
						mShoot.shootAngleY = EditorGUI.FloatField(new Rect(startX + spaceX, startY, 50, height), mShoot.shootAngleY);
					}
					else if (mShoot.type == _ShootObjectType.Beam || mShoot.type == _ShootObjectType.FPSBeam)
					{
						cont = new GUIContent("Beam Duration:", "The active duration of the beam");
						EditorGUI.LabelField(new Rect(startX, startY += spaceY, width, height), cont);
						mShoot.beamDuration = EditorGUI.FloatField(new Rect(startX + spaceX, startY, 50, height), mShoot.beamDuration);

						cont = new GUIContent("AutoSearchForLineRenderer:", "Check to let the script automatically search for all the LineRenderer component on the prefab instead of assign it manually");
						EditorGUI.LabelField(new Rect(startX, startY += spaceY, width, height), cont);
						mShoot.autoSearchLineRenderer = EditorGUI.Toggle(new Rect(startX + spaceX, startY, 50, height), mShoot.autoSearchLineRenderer);

						if (!mShoot.autoSearchLineRenderer)
						{
							cont = new GUIContent("LineRenderers", "The LineRenderer component in this prefab\nOnly applicable when AutoSearchForLineRenderer is unchecked");
							showLineRendererList = EditorGUI.Foldout(new Rect(startX, startY += spaceY, spaceX, height), showLineRendererList, cont);

							if (showLineRendererList)
							{
								cont = new GUIContent("LineRenderers:", "The LineRenderer component on the prefab to be controlled by the script");
								EditorGUI.LabelField(new Rect(startX, startY += spaceY, width, height), cont);
								float listSize = mShoot.lineList.Count;
								listSize = EditorGUI.FloatField(new Rect(startX + spaceX, startY, 40, height), listSize);

								if (listSize != mShoot.lineList.Count)
								{
									while (mShoot.lineList.Count < listSize) mShoot.lineList.Add(null);
									while (mShoot.lineList.Count > listSize) mShoot.lineList.RemoveAt(mShoot.lineList.Count - 1);
								}

								for (int i = 0; i < mShoot.lineList.Count; i++)
								{
									EditorGUI.LabelField(new Rect(startX, startY += spaceY, width, height), "    - Element " + (i + 1));
									mShoot.lineList[i] = (LineRenderer)EditorGUI.ObjectField(new Rect(startX + spaceX, startY, width, height), mShoot.lineList[i], typeof(LineRenderer), true);
								}
							}

						}
					}
					else if (mShoot.type == _ShootObjectType.Effect)
					{

					}

					if (mShoot.type == _ShootObjectType.FPSBeam || mShoot.type == _ShootObjectType.FPSEffect)
					{
						cont = new GUIContent("Sphere Cast Radius:", "The radius of the SphereCast used to detect target hit. The bigger the value, the easier to hit a target");
						EditorGUI.LabelField(new Rect(startX, startY += spaceY, width, height), cont);
						mShoot.hitRadius = EditorGUI.FloatField(new Rect(startX + spaceX, startY, width, height), mShoot.hitRadius);
					}

					startY += spaceY + 20;
					startX -= 30;
					cont = new GUIContent("3、特效:", "The Creep ID");
					EditorGUI.LabelField(new Rect(startX, startY, width, height), cont);
					startX += 30;

                    cont = new GUIContent("发射特效:", "The gameObject (as visual effect) to be spawn at shootPoint when the shootObject is fired");
					EditorGUI.LabelField(new Rect(startX, startY += spaceY, width, height), cont);
					mShoot.shootEffect = (GameObject)EditorGUI.ObjectField(new Rect(startX + spaceX, startY, width, height), mShoot.shootEffect, typeof(GameObject), true);

					cont = new GUIContent("命中特效:", "The gameObject (as visual effect) to be spawn at hit point when the shootObject hit it's target");
					EditorGUI.LabelField(new Rect(startX, startY += spaceY, width, height), cont);
					mShoot.hitEffect = (GameObject)EditorGUI.ObjectField(new Rect(startX + spaceX, startY, width, height), mShoot.hitEffect, typeof(GameObject), true);

					startY += spaceY + 20;
					if (GUI.Button(new Rect(startX + spaceX - 30, startY, 80, height + 2), "保存"))
					{
						mShootPrefab = PrefabUtility.ReplacePrefab(mShoot.gameObject, mShootPrefab);
						EditorUtility.SetDirty(mShootPrefab);
						AssetDatabase.SaveAssets();
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