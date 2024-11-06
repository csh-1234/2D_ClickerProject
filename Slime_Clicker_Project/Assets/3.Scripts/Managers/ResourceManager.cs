using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class ResourceManager
{
    // �ε�� ���ҽ��� ĳ���ϴ� ��ųʸ�
    // Key: ���ҽ��� ���� �ĺ���(�ּ�/���)
    // Value: ���� �ε�� ���ҽ� ��ü
    Dictionary<string, UnityEngine.Object> resources = new Dictionary<string, UnityEngine.Object>();

    public T Load<T>(string key) where T : UnityEngine.Object
    {
        if (resources.TryGetValue(key, out UnityEngine.Object resource))
        {
            return resource as T;
        }

        if (typeof(T) == typeof(Sprite))
        {
            key = key + ".sprite";
            if (resources.TryGetValue(key, out UnityEngine.Object temp))
            {   
                return temp as T;
            }
        }
        if (typeof(T) == typeof(AudioClip))
        {
            // ��ü Resources �������� �˻�
            AudioClip[] allClips = Resources.LoadAll<AudioClip>("");
            foreach (var clip in allClips)
            {
                Debug.Log($"Found clip: {clip.name} in Resources folder");
                if (clip.name == key.Replace("Sounds/", ""))
                {
                    Debug.Log($"Found matching clip: {clip.name}");
                    resources.Add(key, clip);
                    return clip as T;
                }
            }
        }

        Debug.LogError($"Failed to load resource: {key}");
        return null;
    }
    public GameObject Instantiate(string key, Transform parent = null, bool pooling = false)
    {
        GameObject prefab = Load<GameObject>($"{key}");
        if (prefab == null)
        {
            Debug.LogError($"Failed to load prefab : {key}");
            return null;
        }

        if (pooling)
        {
            return Managers.Instance.Pool.Pop(prefab);
        }

        GameObject go = UnityEngine.Object.Instantiate(prefab, parent);
        go.name = prefab.name;

        return go;
    }

    public void Destroy(GameObject go)
    {
        if (go == null)
            return;

        if (Managers.Instance.Pool.Push(go))
        {
            return;
        }
        UnityEngine.Object.Destroy(go);
    }
    #region Addressables �񵿱� ������ó��

    /// <summary>
    /// ���� ���ҽ��� �񵿱������� �ε��մϴ�.
    /// </summary>
    /// <typeparam name="T">�ε��� ���ҽ��� Ÿ�� (��: GameObject, Sprite ��)</typeparam>
    /// <param name="key">���ҽ��� ���� Ű(Addressables �ּ�)</param>
    /// <param name="callback">�ε� �Ϸ� �� ����� �ݹ� �Լ�</param>
    public void LoadRescourceAsync<T>(string key, Action<T> callback = null) where T : UnityEngine.Object
    {
        //addressables���� �ش� key�� ���� �����͸� Ž�� �� => ������ resource�� �Ҵ� ������ null �Ҵ�
        string loadKey = key;
        //AsyncOperationHandle : Addressables ���� �񵿱� �۾��� ��Ÿ���� ����ü
        // Addressables���� ���ҽ��� �񵿱������� �ε��ϴ� �ڵ��� ������
        var asyncOperationHandle = Addressables.LoadAssetAsync<T>(loadKey);

        //���� �񵿱� ó���� �Ϸ�Ǹ� Completed �̺�Ʈ�� �߻�
        //op���� �񵿱� ó���� ������ �����Ͱ�  ���
        //op.result => AsyncOperationHandle�� ������(T Ÿ��)
        asyncOperationHandle.Completed += (op) =>
        {
            // �̹� ���� Ű�� �ε�� ���ҽ��� �ִ��� Ȯ��
            if (resources.TryGetValue(key, out UnityEngine.Object resource))
            {
                // �̹� �ִٸ� ĳ�õ� ���ҽ��� �ݹ����� ����
                callback?.Invoke(op.Result);
                return;
            }
            // ���ο� ���ҽ���� ��ųʸ��� �߰�
            resources.Add(key, op.Result);
            // �ε�� ���ҽ��� �ݹ����� ����
            callback?.Invoke(op.Result);
        };
    }

    /// <summary>
    /// Ư�� �󺧿� ���� ��� ���ҽ��� �񵿱������� �ε��մϴ�.
    /// </summary>
    /// <typeparam name="T">�ε��� ���ҽ����� Ÿ��</typeparam>
    /// <param name="label">Addressables ��</param>
    /// <param name="callback">�� ���ҽ� �ε� �Ϸ�� ȣ��� �ݹ�(Ű, ���� �ε�� ��, ��ü ��)</param>
    public void LoadAllResourceAsync<T>(string label, Action<string, int, int> callback) where T : UnityEngine.Object
    {
        // label�� type�� �ش��ϴ� ���ҽ��� ��ġ ����(���¹����� ���)�� �����´�.(�񵿱�)
        var asyncOperationHandle = Addressables.LoadResourceLocationsAsync(label, typeof(T));

        //�񵿱� �۾��� �Ϸ�Ǹ� ����
        asyncOperationHandle.Completed += (op) =>
        {
            int loadCount = 0;
            int totalCount = op.Result.Count;

            // �� ���ҽ��� ���� ���������� �ε� ����
            foreach (var result in op.Result)
            {
                if (result.PrimaryKey.Contains(".sprite"))
                {
                    LoadRescourceAsync<Sprite>(result.PrimaryKey, (obj) =>
                    {
                        // ���� ��Ȳ�� �ݹ����� ����
                        loadCount++;
                        callback?.Invoke(result.PrimaryKey, loadCount, totalCount);
                    });
                }
                else
                {
                    LoadRescourceAsync<T>(result.PrimaryKey, (obj) =>
                    {
                        // ���� ��Ȳ�� �ݹ����� ����
                        loadCount++;
                        callback?.Invoke(result.PrimaryKey, loadCount, totalCount);
                    });
                }
            }
        };
    }
    #endregion
}