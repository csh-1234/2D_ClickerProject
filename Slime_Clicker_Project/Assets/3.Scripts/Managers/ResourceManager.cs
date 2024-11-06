using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class ResourceManager
{
    // 로드된 리소스를 캐싱하는 딕셔너리
    // Key: 리소스의 고유 식별자(주소/경로)
    // Value: 실제 로드된 리소스 객체
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
            // 전체 Resources 폴더에서 검색
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
    #region Addressables 비동기 데이터처리

    /// <summary>
    /// 단일 리소스를 비동기적으로 로드합니다.
    /// </summary>
    /// <typeparam name="T">로드할 리소스의 타입 (예: GameObject, Sprite 등)</typeparam>
    /// <param name="key">리소스의 고유 키(Addressables 주소)</param>
    /// <param name="callback">로드 완료 시 실행될 콜백 함수</param>
    public void LoadRescourceAsync<T>(string key, Action<T> callback = null) where T : UnityEngine.Object
    {
        //addressables에서 해당 key를 가진 데이터를 탐색 후 => 있으면 resource에 할당 없으면 null 할당
        string loadKey = key;
        //AsyncOperationHandle : Addressables 에서 비동기 작업을 나타내는 구조체
        // Addressables에서 리소스를 비동기적으로 로드하는 핸들을 가져옴
        var asyncOperationHandle = Addressables.LoadAssetAsync<T>(loadKey);

        //위의 비동기 처리가 완료되면 Completed 이벤트가 발생
        //op에는 비동기 처리로 가져온 데이터가  담김
        //op.result => AsyncOperationHandle의 데이터(T 타입)
        asyncOperationHandle.Completed += (op) =>
        {
            // 이미 같은 키로 로드된 리소스가 있는지 확인
            if (resources.TryGetValue(key, out UnityEngine.Object resource))
            {
                // 이미 있다면 캐시된 리소스를 콜백으로 전달
                callback?.Invoke(op.Result);
                return;
            }
            // 새로운 리소스라면 딕셔너리에 추가
            resources.Add(key, op.Result);
            // 로드된 리소스를 콜백으로 전달
            callback?.Invoke(op.Result);
        };
    }

    /// <summary>
    /// 특정 라벨에 속한 모든 리소스를 비동기적으로 로드합니다.
    /// </summary>
    /// <typeparam name="T">로드할 리소스들의 타입</typeparam>
    /// <param name="label">Addressables 라벨</param>
    /// <param name="callback">각 리소스 로드 완료시 호출될 콜백(키, 현재 로드된 수, 전체 수)</param>
    public void LoadAllResourceAsync<T>(string label, Action<string, int, int> callback) where T : UnityEngine.Object
    {
        // label과 type에 해당하는 리소스의 위치 정보(에셋번들의 경로)를 가져온다.(비동기)
        var asyncOperationHandle = Addressables.LoadResourceLocationsAsync(label, typeof(T));

        //비동기 작업이 완료되면 실행
        asyncOperationHandle.Completed += (op) =>
        {
            int loadCount = 0;
            int totalCount = op.Result.Count;

            // 각 리소스에 대해 개별적으로 로드 실행
            foreach (var result in op.Result)
            {
                if (result.PrimaryKey.Contains(".sprite"))
                {
                    LoadRescourceAsync<Sprite>(result.PrimaryKey, (obj) =>
                    {
                        // 진행 상황을 콜백으로 전달
                        loadCount++;
                        callback?.Invoke(result.PrimaryKey, loadCount, totalCount);
                    });
                }
                else
                {
                    LoadRescourceAsync<T>(result.PrimaryKey, (obj) =>
                    {
                        // 진행 상황을 콜백으로 전달
                        loadCount++;
                        callback?.Invoke(result.PrimaryKey, loadCount, totalCount);
                    });
                }
            }
        };
    }
    #endregion
}