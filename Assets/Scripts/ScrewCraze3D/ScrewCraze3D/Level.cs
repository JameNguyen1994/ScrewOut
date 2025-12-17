using UnityEngine;
using System.Collections.Generic;
using System.Linq;


#if UNITY_EDITOR
using UnityEditor;
#endif

public class Level : MonoBehaviour
{
	public float _ScrewScale = 0.35f;
	public string ObstacleContainerName = "_obstacleContainer";
	public bool IsConvertCubeWithParent = true;

	[ContextMenu("Convert Level To LevelMap Data")]
	public void ConvertLevelToLevelMapData()
	{
		UnpackIfPrefab();
		NormalizedLayer();
		ReplaceLinkName();
		ReplaceScrew();
		ReplaceCubeName();
		ReplaceStaticObstacleName();

		var findPlank = GetComponentsInChildren<ScrewCraze3D.Plank>(true);
		var IsNeedLinkObstacle = findPlank.Length > 0;
		if (IsNeedLinkObstacle)
			ScrewFindLinkAndShape();
		ReplaceScripts();
		Debug.Log("Level converted to LevelMap Data successfully.");
	}

	public void ReplaceScrew()
	{
#if UNITY_EDITOR
		var listscrew = GetComponentsInChildren<ScrewCraze3D.Screw>(true);
		foreach (var screw in listscrew)
		{
			Object prefabScrewAsset = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Game/Prefabs/level_new_control/screw.prefab");
			GameObject prefabScrewInstance = (GameObject)PrefabUtility.InstantiatePrefab(prefabScrewAsset);
			prefabScrewInstance.transform.SetParent(screw.transform.parent, false);
			prefabScrewInstance.transform.localPosition = screw.transform.localPosition;
			prefabScrewInstance.transform.localRotation = screw.transform.localRotation;
			var cacheScale = screw.transform.localScale;
			prefabScrewInstance.transform.localScale = cacheScale * _ScrewScale;
		}
#endif
	}

	public void ReplaceScripts()
	{
		var listScrew = GetComponentsInChildren<ScrewCraze3D.Screw>(true);
		foreach (var screw in listScrew)
		{
			var rigid = screw.GetComponent<Rigidbody>();
			if (rigid != null)
			{
				DestroyImmediate(rigid);
			}
			var collider = screw.GetComponent<Collider>();
			if (collider != null)
			{
				DestroyImmediate(collider);
			}
			DestroyImmediate(screw.gameObject);
		}

		var listCube = GetComponentsInChildren<ScrewCraze3D.Cube>(true);
		foreach (var shape in listCube)
		{
			var collider = shape.GetComponent<Collider>();
			if (collider != null)
			{
				DestroyImmediate(collider);
			}
			var rigid = shape.GetComponent<Rigidbody>();
			if (rigid != null)
			{
				DestroyImmediate(rigid);
			}
			DestroyImmediate(shape);
		}

		var listPlankContainers = GetComponentsInChildren<ScrewCraze3D.PlankContainer>(true);
		foreach (var plankContainer in listPlankContainers)
		{
			DestroyImmediate(plankContainer);
		}

		var listPlank = GetComponentsInChildren<ScrewCraze3D.Plank>(true);
		foreach (var plank in listPlank)
		{
			var colliders = plank.GetComponents<Collider>();
			foreach (var collider in colliders)
			{
				DestroyImmediate(collider);
			}
			var rigid = plank.GetComponent<Rigidbody>();
			if (rigid != null)
			{
				DestroyImmediate(rigid);
			}
			DestroyImmediate(plank);
		}

		List<GameObject> matches = new();
		ScanChildrenRecursive(this.transform, "Cube", matches);

		foreach (var obj in matches)
		{
			if (obj.GetComponent<MeshRenderer>() == null)
			{
				obj.gameObject.name = "C_Holder";
			}
		}

		matches.Clear();
		ScanChildrenRecursive(this.transform, "Screw", matches);

		foreach (var obj in matches)
		{
			if (obj.GetComponent<MeshRenderer>() == null)
			{
				obj.gameObject.name = "S_Holder";
			}
		}

		matches.Clear();
		ScanChildrenRecursive(this.transform, "Link", matches);

		foreach (var obj in matches)
		{
			if (obj.GetComponent<MeshRenderer>() == null)
			{
				obj.gameObject.name = "L_Holder";
			}
		}
	}

	public void NormalizedLayer()
	{
		Transform target = null;
		foreach (var child in GetAllChildren(this.gameObject))
		{
			if (child.name == ObstacleContainerName)
			{
				target = child.transform;
				break;
			}
		}

		if (target == null)
		{
			Debug.LogError($"Obstacle container not found. Please create a GameObject named '{ObstacleContainerName}' in the scene.");
		}
		else
		{
			int index = 1;
			foreach (Transform child in target)
			{
				child.name = $"Layer {index}";
				index++;
			}
			Debug.Log($"Indexed all children of {ObstacleContainerName} and set their names to 'Layer X'.");
			this.gameObject.name += $"L_{index}";
		}

		var allTransform = GetComponentsInChildren<Transform>(true);
		foreach (var child in allTransform)
		{
			if (child.gameObject.name == "Collider")
			{
				DestroyImmediate(child.gameObject);
				break; ;
			}
		}
	}

	public void ReplaceCubeName()
	{
		var listCube = GetComponentsInChildren<ScrewCraze3D.Cube>(true);
		foreach (var shape in listCube)
		{
			var meshRenderer = shape.GetComponent<MeshRenderer>();
			if (meshRenderer == null)
			{
				meshRenderer = shape.GetComponentInChildren<MeshRenderer>(true);
			}
			var meshCollider = meshRenderer.GetComponent<MeshCollider>();
			if (meshCollider == null)
			{
				meshCollider = meshRenderer.gameObject.AddComponent<MeshCollider>();
			}
			var index = FindLayerIndexFromParents(shape.transform);
			if (meshRenderer != null && index.HasValue)
			{
				meshRenderer.gameObject.layer = 7;
				meshRenderer.gameObject.name = $"Cube_L{index.Value}";
			}
			else
			{
				Debug.LogWarning($"MeshRenderer not found or id not set -- {meshRenderer != null}  --- {index.HasValue}");
			}
		}
	}

	public void ReplaceStaticObstacleName()
	{
		var listCube = GetComponentsInChildren<ScrewCraze3D.StaticObstacle>(true);
		foreach (var shape in listCube)
		{
			var meshRenderer = shape.GetComponent<MeshRenderer>();
			if (meshRenderer == null)
			{
				meshRenderer = shape.GetComponentInChildren<MeshRenderer>(true);
			}
			var meshCollider = meshRenderer.GetComponent<MeshCollider>();
			if (meshCollider == null)
			{
				meshCollider = meshRenderer.gameObject.AddComponent<MeshCollider>();
			}
			var index = FindLayerIndexFromParents(shape.transform);
			if (meshRenderer != null && index.HasValue)
			{
				meshRenderer.gameObject.layer = 7;
				meshRenderer.gameObject.name = $"Cube_L{index.Value}";
			}
			else
			{
				Debug.LogWarning($"MeshRenderer not found or id not set -- {meshRenderer != null}  --- {index.HasValue}");
			}
		}
	}

	public void ReplaceLinkName()
	{
		var listCube = GetComponentsInChildren<ScrewCraze3D.Plank>(true);
		foreach (var shape in listCube)
		{
			var meshRenderer = shape.GetComponent<MeshRenderer>();
			if (meshRenderer == null)
			{
				meshRenderer = shape.GetComponentInChildren<MeshRenderer>(true);
			}
			var meshCollider = meshRenderer.GetComponent<MeshCollider>();
			if (meshCollider == null)
			{
				meshCollider = meshRenderer.gameObject.AddComponent<MeshCollider>();
			}

			// Get the MeshFilter component from the MeshRenderer
			MeshFilter meshFilter = meshRenderer.GetComponent<MeshFilter>();

			// Get the Mesh object from the MeshFilter
			Mesh mesh = meshFilter.sharedMesh;

			// Check if the Mesh is not null\
			string meshName = string.Empty;
			if (mesh != null)
			{
				// Get the name of the Mesh
				meshName = mesh.name;
				Debug.Log("Mesh Name: " + meshName);
			}
			else
			{
				Debug.LogError("Mesh is null for this MeshRenderer.");
			}

			if (meshName == "ban1_1x5_0" || meshName == "ban1_0_0" || meshName == "")
			{
				meshRenderer.gameObject.SetActive(false);
				var parent = shape.GetComponentInParent<ScrewCraze3D.PlankContainer>();
				if (parent != null)
				{
					// try to get screw inside this type of plank
					var screws = parent.GetComponentsInChildren<ScrewCraze3D.Screw>(true);
					// because this mesh will be removed, we'll need to push our screws closer to object;
					foreach (var screw in screws)
					{
						Debug.Log("Found screw in plank container: " + screw.name);
						screw.transform.position += screw.transform.up * -0.1f * screw.transform.localScale.y; // Move screw up by 0.2 units
					}
				}


				continue; // Skip if mesh name is empty
			}

			meshRenderer.gameObject.layer = 0;
			meshRenderer.gameObject.name = $"Link";
			meshCollider.convex = true;
		}
	}

	public void ScrewFindLinkAndShape()
	{
		var listScrew = GetComponentsInChildren<Screw>(true);
		foreach (var screw in listScrew)
		{
			bool isDebug = screw.gameObject.name.Contains("Debug");
			if (isDebug)
			{
				Debug.Log($"Processing Screw: {screw.name}");
			}
			Ray ray = new Ray(screw.transform.position + screw.transform.up * 0.2f, -screw.transform.up);
			RaycastHit[] hits = Physics.RaycastAll(ray, .7f);

			GameObject firstShape = null;
			GameObject firstLinkObstacle = null;

			foreach (var hit in hits)
			{
				GameObject obj = hit.collider.gameObject;

				// Check for LinkObstacle first (most derived)
				if (firstLinkObstacle == null && obj.gameObject.name.Contains("Link"))
				{
					firstLinkObstacle = obj;
					Debug.Log($"Found LinkObstacle: {obj.name}", obj);
				}

				// Then check for Shape, but skip if it's already a LinkObstacle
				if (firstShape == null && obj.gameObject.name.Contains("Cube_L"))
				{
					firstShape = obj;
					Debug.Log($"Found Shape (not LinkObstacle): {obj.name}", obj);
				}

				if (firstShape != null && firstLinkObstacle != null)
					break;
			}

			if (firstLinkObstacle != null)
			{
				screw.transform.SetParent(firstLinkObstacle.transform, true);
				Debug.DrawRay(ray.origin, ray.direction, Color.blue, 10f);
			}
			else if (firstShape != null)
			{
				screw.transform.SetParent(firstShape.transform, true);
				Debug.DrawRay(ray.origin, ray.direction, Color.blue, 10f);
			}
			else
			{
				Debug.LogWarning($"No LinkObstacle or Shape found for Screw: {screw.name}");
				Debug.DrawRay(ray.origin, ray.direction, Color.red, 10f);
			}

			if (isDebug)
			{
				Vector3 origin = ray.origin;
				Vector3 destination = ray.origin + ray.direction * .7f;

				DrawCrossMarker(origin, 0.05f, Color.green);
				DrawCrossMarker(destination, 0.05f, Color.red);
			}

		}
	}

	void DrawCrossMarker(Vector3 position, float size, Color color)
	{
		Debug.DrawLine(position - Vector3.right * size, position + Vector3.right * size, color, 2f);
		Debug.DrawLine(position - Vector3.up * size, position + Vector3.up * size, color, 2f);
		Debug.DrawLine(position - Vector3.forward * size, position + Vector3.forward * size, color, 2f);
	}

	public static int? FindLayerIndexFromParents(Transform start)
	{
		Transform current = start;

		while (current != null)
		{
			if (current.name.StartsWith("layer ", System.StringComparison.OrdinalIgnoreCase))
			{
				string[] parts = current.name.Split(' ');
				if (parts.Length == 2 && int.TryParse(parts[1], out int layerIndex))
				{
					return layerIndex;
				}
			}

			if (current.name.StartsWith("layer_", System.StringComparison.OrdinalIgnoreCase))
			{
				string[] parts = current.name.Split('_');
				if (parts.Length == 2 && int.TryParse(parts[1], out int layerIndex))
				{
					return layerIndex;
				}
			}

			current = current.parent;
		}

		return null; // Not found
	}

	private static void ScanChildrenRecursive(Transform parent, string keyword, List<GameObject> results)
	{
		foreach (Transform child in parent)
		{
			if (child.name.Contains(keyword))
			{
				results.Add(child.gameObject);
			}

			// Recurse
			ScanChildrenRecursive(child, keyword, results);
		}
	}

	[ContextMenu("Convert Cube name With Level")] 
	public void ConvertCubeNameWithLevel()
	{
		var listCube =  GetComponentsInChildren<Transform>(true)
			.Where(t => t.name.StartsWith("Cube"))
			.Select(t => t.gameObject)
			.ToList();;
		string suffix = "_" + this.gameObject.transform.parent.name;
		if (IsConvertCubeWithParent)
		{
			foreach (var shape in listCube)
			{
				if (!shape.name.EndsWith(suffix))
				{
					shape.name += suffix;
				}
			}
		}
		else
		{
			foreach (var shape in listCube)
			{
				if (shape.name.EndsWith(suffix))
				{
					shape.name = shape.name.Substring(0, shape.name.Length - suffix.Length);
				}
			}
		}
	}
	
	public static List<GameObject> GetAllChildren(GameObject parent)
	{
		List<GameObject> children = new List<GameObject>();
		foreach (Transform child in parent.transform)
		{
			children.Add(child.gameObject);
			children.AddRange(GetAllChildren(child.gameObject)); // Recursive
		}
		return children;
	}

	[ContextMenu("Unpack If Prefab Instance")]
	private void UnpackIfPrefab()
	{
#if UNITY_EDITOR
		if (PrefabUtility.IsPartOfPrefabInstance(gameObject))
		{
			PrefabUtility.UnpackPrefabInstance(gameObject, PrefabUnpackMode.Completely, InteractionMode.UserAction);
			Debug.Log($"Unpacked prefab: {gameObject.name}");
		}
		else
		{
			Debug.Log($"Not a prefab instance: {gameObject.name}");
		}
#endif
		this.transform.localPosition = Vector3.zero;
	}
}