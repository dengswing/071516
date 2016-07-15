using UnityEngine;
using UnityEditor;

using System.Collections;

public class MenuExtensionDora : EditorWindow {

	[MenuItem("Dora/New DoraTD", false, -300)]
	static void NewDoraTD()
	{
		// 创建场景 
		EditorApplication.NewScene();
		GameObject camObj = Camera.main.gameObject; DestroyImmediate(camObj);

		// 从Prefab中的模板创建场景
		GameObject tdPrefab = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Prefabs/assetBundle/Levels/Common/DoraTDLogic.prefab", typeof(GameObject));
		GameObject rootObj = (GameObject)Instantiate(tdPrefab);
		rootObj.name = "TDLogic";

		rootObj = (GameObject)Instantiate(Resources.Load("ScenePrefab/DoraTDCamera", typeof(GameObject)));
		rootObj.name = "Camera";
		rootObj = (GameObject)Instantiate(Resources.Load("ScenePrefab/DoraTDBg", typeof(GameObject)));
		rootObj.name = "Bg";
		rootObj = (GameObject)Instantiate(Resources.Load("ScenePrefab/DoraTDTerrain", typeof(GameObject)));
		rootObj.name = "Terrain";
		rootObj = (GameObject)Instantiate(Resources.Load("ScenePrefab/DoraTDUI", typeof(GameObject)));
		rootObj.name = "UI";

		//SpawnManager spawnManager = (SpawnManager)FindObjectOfType(typeof(SpawnManager));
		//if (spawnManager.waveList[0].subWaveList[0].unit == null)
		//    spawnManager.waveList[0].subWaveList[0].unit = CreepDB.GetFirstPrefab().gameObject;
	}

	//[MenuItem("Dora/New DoraScene", false, -200)]
	static void NewDoraScene()
	{
		// 创建场景 
		EditorApplication.NewScene();
		GameObject camObj = Camera.main.gameObject; DestroyImmediate(camObj);

		// 从Prefab中的模板创建场景
		GameObject rootObj = (GameObject)Instantiate(Resources.Load("ScenePrefab/DoraTDScene", typeof(GameObject)));
		rootObj.name = "DoraTD";

		//SpawnManager spawnManager = (SpawnManager)FindObjectOfType(typeof(SpawnManager));
		//if (spawnManager.waveList[0].subWaveList[0].unit == null)
		//    spawnManager.waveList[0].subWaveList[0].unit = CreepDB.GetFirstPrefab().gameObject;
	}

	//[MenuItem("Dora/Add DoraTerrain", false, -100)]
	static void AddDoraTerrain()
	{
		GameObject obj = PrefabUtility.InstantiatePrefab(Resources.Load("ScenePrefab/DoraTerrain", typeof(GameObject))) as GameObject;
		obj.name = "Terrain";

		GameObject rootObj = GameObject.Find("DoraTD");
		if (rootObj != null) obj.transform.parent = rootObj.transform;
	}

	//[MenuItem("Dora/Add DoraUI", false, -100)]
	static void AddDoraUI()
	{
		GameObject obj = PrefabUtility.InstantiatePrefab(Resources.Load("ScenePrefab/DoraTDUI", typeof(GameObject))) as GameObject;
		obj.name = "UI";

		GameObject rootObj = GameObject.Find("DoraTD");
		if (rootObj != null) obj.transform.parent = rootObj.transform;
	}

}


