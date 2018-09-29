using System;
using System.IO;
using System.Reflection;

namespace Hooks
{
	[RuntimeHook]
	class PluginHook : Hook
	{
		public PluginHook()
		{
			HookRegistry.Register(OnCall);
			isReentry = false;
		}

		public new static string[] GetExpectedMethods()
		{
			return new string[] { "PLGlobal::Awake" };
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
				// Call the original method.
				LoadPlugins(typeName, methodName);

				return ReenterMethod<PLGlobal>(ref thisObj, "Awake", args);
			}
			catch (Exception e)
			{
				string message = String.Format(HOOK_FAILED, e);
				HookRegistry.Panic(message);
			}

			// Dummy value.  Returning anything other than null skips the original method.
			return true;
		}

		private static void LoadPlugins(string typeName, string methodName)
		{
			HookRegistry.Log(String.Format("Attempting to load plugins via {0}::{1}", typeName, methodName));

			string pluginsDir = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Plugins");

			if (!Directory.Exists(pluginsDir))
			{
				Directory.CreateDirectory(pluginsDir);
			}

			foreach (string dllPath in Directory.GetFiles(pluginsDir, "*.dll"))
			{
				string fileName = Path.GetFileName(dllPath);
				if(fileName == "0Harmony.dll")
				{
					continue;
				}

				Assembly asm = Assembly.LoadFile(dllPath);
				IPlugin plugin = Activator.CreateInstance(asm.GetType("Plugin")) as IPlugin;

				if (plugin == null)
				{
					throw new ApplicationException(String.Format("Couldn't load plugin assembly: {0}", dllPath));
				}
				else
				{
					HookRegistry.Log(String.Format("Loading plugin: {0}", fileName));
					plugin.Run();
				}
			}

			HookRegistry.Log("Finished loading plugins!");
		}
	}
}
