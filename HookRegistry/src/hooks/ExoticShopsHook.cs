using System;

namespace Hooks
{
	[RuntimeHook]
	class ExoticShopsHook : Hook
	{
		public ExoticShopsHook()
		{
			HookRegistry.Register(OnCall);
			isReentry = false;
		}

		public new static string[] GetExpectedMethods()
		{
			return new string[] { "PLShop_Exotic1::Start", "PLShop_Exotic2::Start", "PLShop_Exotic3::Start" };
		}

		private void SetContrabandDealer(ref object shopObject, bool isContrabandDealer)
		{
			PLShop shop = (PLShop)shopObject;
			shop.ContrabandDealer = isContrabandDealer;
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
				SetContrabandDealer(ref thisObj, true);


				// Call the original method.
				// Let's capture the method call's return value instead of returning it directly.
				// This allows us to do more things after the original function runs its course.
				object returnValue = null;

				// This looks janky because we're handling 3 similar but distinct classes in the same hook.
				// Most hooks will be simpler due to one hooking method in a single class or common superclass.
				if (thisObj is PLShop_Exotic1)
				{
					returnValue = ReenterMethod<PLShop_Exotic1>(ref thisObj, "Start", args);
				}
				else if (thisObj is PLShop_Exotic2)
				{
					returnValue = ReenterMethod<PLShop_Exotic2>(ref thisObj, "Start", args);
				}
				else if (thisObj is PLShop_Exotic3)
				{
					returnValue = ReenterMethod<PLShop_Exotic3>(ref thisObj, "Start", args);
				}

				// Do stuff after the original method completes.
				// This allows the hook to run again (and set contraband dealer) so every shop is configured
				// instead of only the first one we warp to.
				isReentry = false;

				return returnValue;
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
