using System;

namespace Hooks
{
	[RuntimeHook]
	class PulsarVersionHook : Hook
	{
		public PulsarVersionHook()
		{
			HookRegistry.Register(OnCall);
			isReentry = false;
		}

		public new static string[] GetExpectedMethods()
		{
			return new string[] { "PLNetworkManager::Start" };
		}

		private void SetPulsarHookVersion(ref object plNetworkManagerObject, bool isPulsarHooked)
		{
			PLNetworkManager netMgr = (PLNetworkManager)plNetworkManagerObject;

			if (isPulsarHooked)
			{
				netMgr.VersionString += " (pulsar-hook)";
			}
			else
			{
				netMgr.VersionString = netMgr.VersionString.Replace(" (pulsar-hook)", String.Empty);
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
				SetPulsarHookVersion(ref thisObj, true);

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
