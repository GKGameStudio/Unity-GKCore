/*           INFINITY CODE          */
/*     https://infinity-code.com    */

using UnityEngine;
using UnityEngine.Events;

namespace GKCore.Observers.Editors
{
    public static class EventButtonContent
    {
        private static GUIContent _buttonContent;
        private static GUIContent _disabledButtonContent;

        private static GUIContent ButtonContent
        {
            get
            {
                if (_buttonContent == null) _buttonContent = new GUIContent(EditorUtils.LoadTexture("Event"), "Show/Hide Changed Events");
                return _buttonContent;
            }
        }

        private static GUIContent DisabledButtonContent
        {
            get
            {
                if (_disabledButtonContent == null) _disabledButtonContent = new GUIContent(EditorUtils.LoadTexture("Event-Disabled"), "Show/Hide Changed Events");
                return _disabledButtonContent;
            }
        }
        
        public static GUIContent GetContent(UnityEventBase unityEvent)
        {
            return unityEvent.GetPersistentEventCount() > 0 ? ButtonContent : DisabledButtonContent;
        }
    }
}