using UnityEngine;

public class PersistentSingleton<T> : Singleton<T> where T : Component
{
    protected override void Awake()
    {
        if (instance == null)
        {
            instance = this as T;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
