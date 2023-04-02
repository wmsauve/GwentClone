using System;

namespace BackendFunctionality
{
    public class GlobalBackendActions 
    {
        /// <summary>
        /// Used for returning back to whoever made the api call.
        /// </summary>
        public static Action<EnumAPIType, ResponseFromServer> OnFinishedAPICall;
    }

}
