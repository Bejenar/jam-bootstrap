using Cysharp.Threading.Tasks;
using UnityEngine;

namespace JamBootstrap.jam_bootstrap.Runtime
{
    public class SmartWaitController : MonoBehaviour
    {
        private bool skip = false;
        
        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                skip = true;
            }
        }
        
        public async UniTask SmartWait(float f)
        {
            skip = false;
            while (f > 0 && !skip)
            {
                f -= Time.deltaTime;
                await UniTask.WaitForEndOfFrame();
            }
        }
    }
}