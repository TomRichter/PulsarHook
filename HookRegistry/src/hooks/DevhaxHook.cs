using System;

namespace Hooks
{
	[RuntimeHook]
	class DevhaxHook : Hook
	{
		public DevhaxHook()
		{
			HookRegistry.Register(OnCall);
			isReentry = false;
		}

		public new static string[] GetExpectedMethods()
		{
			return new string[] { "PLNetworkManager::Start" };
		}

		private void SetInternalBuild(ref object plNetworkManagerObject, bool isInternalBuild)
		{
			PLNetworkManager netMgr = (PLNetworkManager)plNetworkManagerObject;

			if (isInternalBuild)
			{
				netMgr.VersionString += "i (pulsar-hook)";
			}
			else
			{
				netMgr.VersionString = netMgr.VersionString.Replace("i (pulsar-hook)", String.Empty);
			}
		}

		protected override object OnCall(string typeName, string methodName, object thisObj, object[] args, IntPtr[] refArgs, int[] refIdxMatch)
		{
			if (!IsExpectedMethod(GetExpectedMethods(), typeName, methodName))
			{
				return null;
			}

			if (isReentry)
			{
				// Prevent infinite recursion below, and allow original method to continue as normal.
				return null;
			}

			try
			{
				// Do stuff before the original method is called.
				SetInternalBuild(ref thisObj, true);

				// Call the original method.
				return ReenterMethod<PLNetworkManager>(ref thisObj, "Start", args);
			}
			catch (Exception e)
			{
				string message = String.Format(HOOK_FAILED, e);
				HookRegistry.Panic(message);
			}

			// Dummy value.  Returning anything other than null skips the original method.
			return true;
		}
	}
}
