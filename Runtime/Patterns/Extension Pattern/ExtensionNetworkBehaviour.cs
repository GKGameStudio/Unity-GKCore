#if FISHNET_V4
using FishNet.Object;

namespace GKCore.FishNet{
    public class ExtensionNetworkBehaviour<T> : NetworkBehaviour
    {
        public T master{
            get{
                return GetComponent<T>();
            }
        }
    }
}
#endif