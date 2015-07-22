using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
#if UNITY_EDITOR
using UnityEditor;
#endif // UNITY_EDITOR

namespace Ist
{

    public abstract class ICommandBufferExecuter<T> : MonoBehaviour where T : MonoBehaviour
    {
        #region static
        static HashSet<T> s_instances;
        static CommandBuffer s_commandbuffer;
        static HashSet<Camera> s_cameras;
        static int s_nth;

        static public HashSet<T> GetInstances()
        {
            if (s_instances == null) { s_instances = new HashSet<T>(); }
            return s_instances;
        }

        static public CommandBuffer GetCommandBuffer()
        {
            if (s_commandbuffer == null) { s_commandbuffer = new CommandBuffer(); }
            return s_commandbuffer;
        }

        static public HashSet<Camera> GetCameraTable()
        {
            if (s_cameras == null) { s_cameras = new HashSet<Camera>(); }
            return s_cameras;
        }
        #endregion


        public virtual void OnEnable()
        {
            GetInstances().Add((T)(System.Object)this);
        }

        public virtual void OnDisable()
        {
            var intances = GetInstances();
            intances.Remove((T)(System.Object)this);

            if (intances.Count == 0)
            {
                var cb = GetCommandBuffer();
                var cam_table = GetCameraTable();
                foreach (var c in cam_table)
                {
                    if (c != null)
                    {
                        RemoveCommandBuffer(c, cb);
                    }
                }
                cam_table.Clear();
            }
        }

        public virtual void OnWillRenderObject()
        {
            if (s_nth++ == 0)
            {
                var cam = Camera.current;
                if (cam == null) { return; }

                var cb = GetCommandBuffer();
                var cam_table = GetCameraTable();
                if (!cam_table.Contains(cam))
                {
                    cb.name = GetCommandBufferName();
                    AddCommandBuffer(cam, cb);
                    cam_table.Add(cam);
                }

                UpdateCommandBuffer(cb);
            }
        }

        public virtual void OnPostRender()
        {
            s_nth = 0;
        }

        public virtual string GetCommandBufferName() { return "ICommandBufferExecuter<" + typeof(T).Name + ">"; }
        public abstract void AddCommandBuffer(Camera c, CommandBuffer cb);
        public abstract void RemoveCommandBuffer(Camera c, CommandBuffer cb);
        public abstract void UpdateCommandBuffer(CommandBuffer commands);
    }

}