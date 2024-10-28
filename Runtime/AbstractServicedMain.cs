using UnityEngine;

namespace JamBootstrap.jam_bootstrap.Runtime
{
    public class AbstractServicedMain : MonoBehaviour
    {
        static bool isInitialized = false;
        
        protected static void InstantiateAutoSaveSystem<T>() where T : AbstractServicedMain
        {
            if (!isInitialized)
            {
                GameObject servicedMain = new GameObject("GameMain");
                servicedMain.AddComponent<T>();
                DontDestroyOnLoad(servicedMain);
                isInitialized = true;
            }
        }
        
        protected virtual void Awake()
        {
            Debug.Log("================");
            Debug.Log("entrypoint hit");
        
            Core.interactor = new Interactor(this.GetType());
            
            // game entrypoint
            CMS.Init(this.GetType());
            Core.interactor.Init();
            
            Core.audio = gameObject.AddComponent<AudioSystem>();
            Core.camera = gameObject.AddComponent<CameraHandle>();
            Core.smartWait = gameObject.AddComponent<SmartWaitController>();
        }


        void OnDestroy()
        {
            isInitialized = false;
        }
    }
}