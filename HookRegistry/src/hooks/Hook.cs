using System;
using System.Reflection;

namespace Hooks
{
	abstract class Hook
	{
		protected const string HOOK_FAILED = "[PulsarHook] Failed to hook! {0}";

		protected bool isReentry;

		public Hook()
		{
			HookRegistry.Register(OnCall);
			isReentry = false;
		}

		public static bool IsExpectedMethod(string[] expectedMethods, string typeName, string methodName)
		{
			for (int i = 0; i < expectedMethods.Length; i++)
			{
				string[] names = expectedMethods[i].Split(new string[] { "::" }, StringSplitOptions.RemoveEmptyEntries);

				if (typeName == names[0] && methodName == names[1])
				{
					return true;
				}
			}

			return false;
		}
		/*
		 * Must be implemented individually in each hook because UnityHook expects it to be static,
		 * but C# doesn't allow overriding statics from parent classes.
		 */
		public static string[] GetExpectedMethods()
		{
			return new string[] { };
		}

		protected virtual void InitDynamicTypes()
		{

		}

		protected virtual object ReenterMethod<T>(ref object objectInstance, string methodName, object[] args)
		{
			isReentry = true;
			MethodInfo initMethod = typeof(T).GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);
			return initMethod.Invoke(objectInstance, args);
		}

		protected virtual object OnCall(string typeName, string methodName, object thisObj, object[] args, IntPtr[] refArgs, int[] refIdxMatch)
		{
			return null;
		}
	}
}
