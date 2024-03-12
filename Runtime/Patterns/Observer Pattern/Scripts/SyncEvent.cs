#if FISHNET_V4
using System;
using System.Collections.Generic;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet.Object.Synchronizing.Internal;
using FishNet.Serializing;
using UnityEngine;
using UnityEngine.Events;


namespace FishNet.Object
{
    // 1 arguments
    public class SyncEvent<T0> : SyncBase, ICustomSync
    {
        /// <summary>
        /// Class for synchronizing arguments through the network and invoking a UnityEvent with the received arguments.
        /// </summary>
            #region Public
            public SyncEvent() : base() {
                // Set the send rate to 0, so that the event is only sent when invoked.
                base.UpdateSendRate(0);
            }
            /// <summary>
            /// The unityEvent to be invoked when receiving the arguments.
            /// </summary>
            public UnityEvent<T0> unityEvent = new UnityEvent<T0>();
            #endregion

            #region Private
            private struct InvokeData
            {
                internal readonly T0 arg0;

                public InvokeData(T0 arg0)
                {
                    this.arg0 = arg0;
                }
            }
            List<InvokeData> _invokes = new List<InvokeData>();
            #endregion

            /// <summary>
            /// Writes the arguments to the network writer.
            /// </summary>
            /// <param name="writer">The network writer.</param>
            protected override void WriteDelta(PooledWriter writer, bool resetSyncTick = true)
            {
                base.WriteDelta(writer, resetSyncTick);
                //Number of entries expected.
                writer.WriteInt32(_invokes.Count);
                for (int i = 0; i < _invokes.Count; i++)
                {
                    writer.Write<T0>(_invokes[i].arg0);
                }
                _invokes.Clear();
            }

            /// <summary>
            /// Reads and sets the arguments from the network reader.
            /// </summary>
            /// <param name="reader">The network reader.</param>
            /// <param name="asServer">True if running on the server side.</param>
            protected override void Read(PooledReader reader, bool asServer)
            { 

                int invokes = reader.ReadInt32();
                for (int i = 0; i < invokes; i++)
                {
                    T0 arg0 = reader.Read<T0>();
                    unityEvent?.Invoke(arg0);
                }
            }

            /// <summary>
            /// Resets the state of the SyncEvent.
            /// </summary>
            protected override void ResetState()
            {
                base.ResetState();
            }
            public void Invoke(T0 arg0){
                unityEvent?.Invoke(arg0);
                
                // Store args
                _invokes.Add(new InvokeData(arg0));

                // Write to network
                base.Dirty();
            }

            //Operator += and -=
            public SyncEvent<T0> AddListener(UnityAction<T0> b){
                unityEvent.AddListener(b);
                return this;
            }
            public SyncEvent<T0> RemoveListener(UnityAction<T0> b){
                unityEvent.RemoveListener(b);
                return this;
            }
            
            public object GetSerializedType() => typeof(UnityEvent<T0>);
            
    }
}
#endif
