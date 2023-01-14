using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
#endif

using System;
using System.Linq;
//using VRC.Core;
//using Toolkit2;

#if UNITY_EDITOR
public class UnrealLoader : EditorWindow
{
	public bool bWorld;
	public string World_AssetURL="http://";
	public bool bLocalWorld;
	public string InputWorldPath="C:\\";
	public bool bAvatar=true;
	public string Avatar_AssetURL="http://";
	public bool bLocalAvatar;
	public string InputAvatarPath="C:\\";
	public bool bCompactInterface=false;
	public bool bShowIncludedAssets=false;
	Vector2 MainScroll=Vector2.zero;

	public struct LoadedAssetsStruct
	{
		public AssetBundle AssetBundle;
		public string Path;
		public bool bLocal,bWorld;
	}

	public List<LoadedAssetsStruct> LoadedAssets=new List<LoadedAssetsStruct>();

	public struct AssetInfoStruct
	{
		public string Path;
		public bool bLocal,bWorld;
	}

	public List<AssetInfoStruct> HistoryAssets=new List<AssetInfoStruct>();

	[MenuItem("Zenith/Recovery/Compiled Asset Loader")]
	static void Init()
	{
		UnrealLoader window=(UnrealLoader)EditorWindow.GetWindow(typeof(UnrealLoader));
		window.Show();
	}

	public LoadedAssetsStruct CreateLoadedAsset(AssetBundle AB, string S, bool bL, bool bW)
	{
		LoadedAssetsStruct LA=new LoadedAssetsStruct();
		LA.AssetBundle=AB;
		LA.Path=S;
		LA.bLocal=bL;
		LA.bWorld=bW;
		return LA;
	}

	public AssetInfoStruct CreateAssetInfo(string S, bool bL, bool bW)
	{
		AssetInfoStruct HA=new AssetInfoStruct();
		HA.Path=S;
		HA.bLocal=bL;
		HA.bWorld=bW;
		return HA;
	}

	IEnumerator Sleep(double f)
	{
		while(f>0)
		{
			f-=Time.deltaTime;
			yield return null;
		}
	}

	string Eval(bool a, string b, string c)
	{
		if(a)
		{
			return b;
		}

		return c;
	}

	int Min(int i, int l)
	{
		if(i>l)
		{
			return l;
		}

		return i;
	}

	int Max(int i, int l)
	{
		if(i>l)
		{
			return i;
		}

		return l;
	}

	int Len(string S)
	{
		return S.Length;
	}

	public string Left(string S, int i)
	{
		if (S.Length <= i)
		{
			return S;
		}

		return S.Substring(0, i);
	}

	public string Right(string S, int i)
	{
		if (i >= S.Length)
		{
			return S;
		}

		return S.Substring(S.Length - i);
	}

	public string Mid(string S, int i, int l = 0)
	{
		if (l > 0)
		{
			return S.Substring(i, l);
		}

		return S.Substring(i);
	}

	public void OnGUI()
	{
		string S;

		MainScroll=EditorGUILayout.BeginScrollView(MainScroll);
		bCompactInterface=EditorGUILayout.Toggle("Compact Interface", bCompactInterface);
		bShowIncludedAssets=EditorGUILayout.Toggle("Show Included Assets", bShowIncludedAssets);
		bWorld=EditorGUILayout.Toggle("World", bWorld);

		if(bWorld)
		{
			bLocalWorld=EditorGUILayout.Toggle("Local", bLocalWorld);

			if(bLocalWorld)
			{
				InputWorldPath=EditorGUILayout.TextField("Input Path", InputWorldPath);
			}
			else
			{
				World_AssetURL=EditorGUILayout.TextField("World Asset URL", World_AssetURL);
			}

			bAvatar=false;
		}

		bAvatar=EditorGUILayout.Toggle("Avatar", bAvatar);

		if(bAvatar)
		{
			bLocalAvatar=EditorGUILayout.Toggle("Local", bLocalAvatar);

			if(bLocalAvatar)
			{
				InputAvatarPath=EditorGUILayout.TextField("Input Path", InputAvatarPath);
			}
			else
			{
				Avatar_AssetURL=EditorGUILayout.TextField("Avatar Asset URL", Avatar_AssetURL);
			}

			bWorld=false;
		}

		if(bWorld || bAvatar)
		{
			if(bCompactInterface)
			{
				EditorGUILayout.BeginHorizontal();
			}

			if(GUILayout.Button("Load"))
			{
				LoadBundle();
			}

			if(GUILayout.Button("Add To Scene"))
			{
				AddToScene();
			}

			if(bWorld)
			{
				if(GUILayout.Button("Open Scene"))
				{
					OpenScene();
				}
			}

			if(GUILayout.Button("Add To History"))
			{
				AddHistoryAsset(GetCurrentAssetInfo());
			}

			if(bCompactInterface)
			{
				EditorGUILayout.EndHorizontal();
			}
		}

		if(GUILayout.Button("Force Unload Everything"))
		{
			ForceUnLoadEverything();
		}

		if(LoadedAssets.Count>0)
		{
			EditorGUILayout.LabelField("Loaded Assets Count: "+LoadedAssets.Count);

			for(int i=0;i<LoadedAssets.Count;i++)
			{
				EditorGUILayout.LabelField(i+": "+LoadedAssets[i].Path);

				if(bShowIncludedAssets)
				{
					string[] AssetNames=LoadedAssets[i].AssetBundle.GetAllAssetNames();

					foreach(string AssetName in AssetNames)
					{
						EditorGUILayout.LabelField(AssetName);
					}

					AssetNames=LoadedAssets[i].AssetBundle.GetAllScenePaths();

					foreach(string AssetName in AssetNames)
					{
						EditorGUILayout.LabelField(AssetName);
					}
				}

				if(bCompactInterface)
				{
					EditorGUILayout.BeginHorizontal();
				}

				if(GUILayout.Button("Add To Scene"))
				{
					AddToScene(LoadedAssets[i]);
				}

				if(LoadedAssets[i].bWorld)
				{
					if(GUILayout.Button("Open Scene"))
					{
						OpenScene(LoadedAssets[i]);
					}
				}

				if(bCompactInterface)
				{
					EditorGUILayout.EndHorizontal();
				}
			}

			if(GUILayout.Button("Unload Everything"))
			{
				UnLoadEverything();
			}
		}

		if(HistoryAssets.Count>0)
		{
			EditorGUILayout.LabelField("History Count: "+HistoryAssets.Count);

			for(int i=HistoryAssets.Count-1;i>=0;i--)
			{
				EditorGUILayout.BeginHorizontal();

				if(GUILayout.Button("Remove"))
				{
					RemoveFromHistory(HistoryAssets[i]);
					i--;
					continue;
				}

				if(i<HistoryAssets.Count-1)
				{
					if(GUILayout.Button("Up"))
					{
						MoveFromHistory(i,1);
					}
				}

				if(i>0)
				{
					if(GUILayout.Button("Down"))
					{
						MoveFromHistory(i,-1);
					}
				}

				if(GUILayout.Button(HistoryAssets[i].Path))
				{
					LoadFromHistory(HistoryAssets[i]);
				}

				EditorGUILayout.EndHorizontal();
			}

			if(GUILayout.Button("Clear History"))
			{
				ClearHistoryAssets();
			}
		}

		EditorGUILayout.EndScrollView();
	}

	public bool MoveFromHistory(int l, int m)
	{
		AssetInfoStruct AI;

		if(m==0 || l<0 || l>=HistoryAssets.Count || l+m<0 || l+m>=HistoryAssets.Count)
		{
			return false;
		}

		AI=HistoryAssets[l];
		HistoryAssets[l]=HistoryAssets[l+m];
		HistoryAssets[l+m]=AI;
		return true;
	}

	public bool RemoveFromHistory(AssetInfoStruct AI)
	{
		for(int i=0;i<HistoryAssets.Count;i++)
		{
			if(HistoryAssets[i].Path==AI.Path)
			{
				HistoryAssets.RemoveAt(i);
				return true;
			}
		}

		return false;
	}

	public void LoadFromHistory(AssetInfoStruct AI)
	{
		LoadAsset(AI);
	}

	public bool AddHistoryAsset(AssetInfoStruct AI)
	{
		for(int i=0;i<HistoryAssets.Count;i++)
		{
			if(HistoryAssets[i].Path==AI.Path)
			{
				return false;
			}
		}

		HistoryAssets.Add(AI);
		return true;
	}

	public void ClearHistoryAssets()
	{
		HistoryAssets.Clear();
	}

	public bool AddLoadedAsset(LoadedAssetsStruct LA)
	{
		for(int i=0;i<LoadedAssets.Count;i++)
		{
			if(LoadedAssets[i].Path==LA.Path)
			{
				return false;
			}
		}

		LoadedAssets.Add(LA);
		AddHistoryAsset(CreateAssetInfo(LA.Path,LA.bLocal,bWorld));
		return true;
	}

	public void ClearLoadedAssets()
	{
		LoadedAssets.Clear();
	}

	public void UnLoadEverything()
	{
		for(int i=LoadedAssets.Count-1;i>=0;i--)
		{
			if(LoadedAssets[i].AssetBundle!=null)
			{
				LoadedAssets[i].AssetBundle.Unload(true);
			}
		}

		ClearLoadedAssets();
	}

	public void ForceUnLoadEverything()
	{
	//	AssetBundle.UnloadAllAssetBundles(true);
	}

	public AssetBundle FindAsset(string S)
	{
		for(int i=0;i<LoadedAssets.Count;i++)
		{
			if(LoadedAssets[i].Path==S)
			{
				return LoadedAssets[i].AssetBundle;
			}
		}

		return null;
	}

	public int GetIndexOfAsset(string S)
	{
		for(int i=0;i<LoadedAssets.Count;i++)
		{
			if(LoadedAssets[i].Path==S)
			{
				return i;
			}
		}

		return -1;
	}

	public bool bIsWorldAsset(string S)
	{
		for(int i=0;i<LoadedAssets.Count;i++)
		{
			if(LoadedAssets[i].Path==S)
			{
				return LoadedAssets[i].bWorld;
			}
		}

		return false;
	}

	public int GetIndexOfAsset(AssetBundle S)
	{
		for(int i=0;i<LoadedAssets.Count;i++)
		{
			if(LoadedAssets[i].AssetBundle==S)
			{
				return i;
			}
		}

		return -1;
	}

	public AssetBundle LoadAsset(AssetInfoStruct AI)
	{
		AssetBundle MyAssetBundle;

		if(AI.Path=="")
		{
			return null;
		}

		MyAssetBundle=FindAsset(AI.Path);

		if(MyAssetBundle!=null)
		{
			return MyAssetBundle;
		}

		if(AI.bLocal)
		{
			MyAssetBundle=AssetBundle.LoadFromFile(AI.Path);
		}
		else
		{
			WWW www=new WWW(AI.Path);

			while(!www.isDone)
			{
				Sleep(0.5);
			}

			MyAssetBundle=www.assetBundle;
		}

		if(MyAssetBundle!=null)
		{
			AddLoadedAsset(CreateLoadedAsset(MyAssetBundle,AI.Path,AI.bLocal,AI.bWorld));
		}

		return MyAssetBundle;
	}

	public AssetInfoStruct GetCurrentAssetInfo()
	{
		string AssetPath="";
		bool bLocalAsset=false;

		if(bWorld)
		{
			if(bLocalWorld)
			{
				AssetPath=InputWorldPath;
				bLocalAsset=true;
			}
			else
			{
				AssetPath=World_AssetURL;
			}
		}
		else if(bAvatar)
		{
			if(bLocalAvatar)
			{
				AssetPath=InputAvatarPath;
				bLocalAsset=true;
			}
			else
			{
				AssetPath=Avatar_AssetURL;
			}
		}

		return CreateAssetInfo(AssetPath,bLocalAsset,bWorld);
	}

	public AssetBundle GetCurrentAsset()
	{
		return LoadAsset(GetCurrentAssetInfo());
	}

	public void LoadBundle()
	{
		GetCurrentAsset();
	}

	public void AddToScene(AssetBundle MyAssetBundle, bool MybWorld=false)
	{
		if(MyAssetBundle==null)
		{
			return;
		}

		if(MybWorld)
		{
			string[] scenePaths=MyAssetBundle.GetAllScenePaths();
			string sceneName=System.IO.Path.GetFileNameWithoutExtension(scenePaths[0]);
			SceneManager.LoadScene(sceneName);
		}
		else
		{
			GameObject ava=LoadAvatar(MyAssetBundle);
			Instantiate(ava);
		}
	}

	public GameObject LoadAvatar(AssetBundle MyAssetBundle)
	{
		string[] AssetsNames;

		if(MyAssetBundle==null)
		{
			return null;
		}

		AssetsNames=MyAssetBundle.GetAllAssetNames();

		if(AssetsNames.Length<=0)
		{
			return null;
		}

		string S="_CustomAvatar";
		int l=0;

		for(l=0;l<AssetsNames.Length;l++)
		{
			if(AssetsNames[l]==S)
			{
				break;
			}
		}

		if(l==AssetsNames.Length)
		{
			S=AssetsNames[0];
		}

		return MyAssetBundle.LoadAsset(S) as GameObject;
	}

	public void AddToScene(LoadedAssetsStruct LoadedAssets)
	{
		if(LoadedAssets.AssetBundle==null)
		{
			return;
		}

		if(LoadedAssets.bWorld)
		{
			string[] scenePaths=LoadedAssets.AssetBundle.GetAllScenePaths();
			string sceneName=System.IO.Path.GetFileNameWithoutExtension(scenePaths[0]);
			SceneManager.LoadScene(sceneName);
		}
		else
		{
			GameObject ava=LoadAvatar(LoadedAssets.AssetBundle);
			Instantiate(ava);
		}
	}

	public void AddToScene()
	{
		AddToScene(GetCurrentAsset());
	}

	public void OpenScene(AssetBundle MyAssetBundle, bool MybWorld=false)
	{
		if(MyAssetBundle==null)
		{
			return;
		}

		if(MybWorld)
		{
			string[] scenePaths=MyAssetBundle.GetAllScenePaths();
			string sceneName=System.IO.Path.GetFileNameWithoutExtension(scenePaths[0]);
		//	EditorSceneManager.OpenScene(sceneName);
			EditorSceneManager.LoadScene(sceneName);
		//	EditorApplication.OpenScene(sceneName);
		}
	}

	public void OpenScene(LoadedAssetsStruct LoadedAssets)
	{
		if(LoadedAssets.AssetBundle==null)
		{
			return;
		}

		if(LoadedAssets.bWorld)
		{
			string[] scenePaths=LoadedAssets.AssetBundle.GetAllScenePaths();
			string sceneName=System.IO.Path.GetFileNameWithoutExtension(scenePaths[0]);
		//	EditorSceneManager.OpenScene(sceneName);
			EditorSceneManager.LoadScene(sceneName);
		//	EditorApplication.OpenScene(sceneName);
		}
	}

	public void OpenScene()
	{
		OpenScene(GetCurrentAsset(),bWorld);
	}
}
#endif
