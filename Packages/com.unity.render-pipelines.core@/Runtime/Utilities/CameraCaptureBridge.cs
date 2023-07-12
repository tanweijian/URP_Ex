using System;
using System.Collections.Generic;

namespace UnityEngine.Rendering
{
    /// <summary>
    /// Bridge class for camera captures.
    /// </summary>
    public static class CameraCaptureBridge
    {
// extensions modify begin;
        private static Dictionary<Camera, HashSet<Action<RTHandle, ScriptableRenderContext, CommandBuffer>>> actionDict =
            new Dictionary<Camera, HashSet<Action<RTHandle, ScriptableRenderContext, CommandBuffer>>>();
// extensions modify end;

        private static bool _enabled;

        /// <summary>
        /// Enable camera capture.
        /// </summary>
        public static bool enabled
        {
            get
            {
                return _enabled;
            }
            set
            {
                _enabled = value;
            }
        }

        /// <summary>
        /// Provides the set actions to the renderer to be triggered at the end of the render loop for camera capture
        /// </summary>
        /// <param name="camera">The camera to get actions for</param>
        /// <returns>Enumeration of actions</returns>
// extensions modify begin;
        public static IEnumerator<Action<RTHandle, ScriptableRenderContext, CommandBuffer>> GetCaptureActions(Camera camera)
        {
            if (!actionDict.TryGetValue(camera, out var actions) || actions.Count == 0)
                return null;

            return actions.GetEnumerator();
        }
// extensions modify end;

        /// <summary>
        /// Adds actions for camera capture
        /// </summary>
        /// <param name="camera">The camera to add actions for</param>
        /// <param name="action">The action to add</param>
// extensions modify begin;
        public static void AddCaptureAction(Camera camera, Action<RTHandle, ScriptableRenderContext, CommandBuffer> action)
// extensions modify end;
        {
            actionDict.TryGetValue(camera, out var actions);
            if (actions == null)
            {
// extensions modify begin;
                actions = new HashSet<Action<RTHandle, ScriptableRenderContext, CommandBuffer>>();
// extensions modify end;
                actionDict.Add(camera, actions);
            }

            actions.Add(action);
        }

        /// <summary>
        /// Removes actions for camera capture
        /// </summary>
        /// <param name="camera">The camera to remove actions for</param>
        /// <param name="action">The action to remove</param>
// extensions modify begin;
        public static void RemoveCaptureAction(Camera camera, Action<RTHandle, ScriptableRenderContext, CommandBuffer> action)
// extensions modify end;
        {
            if (camera == null)
                return;

            if (actionDict.TryGetValue(camera, out var actions))
                actions.Remove(action);
        }
    }
}
