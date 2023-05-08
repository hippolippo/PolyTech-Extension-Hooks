using System;
using BepInEx;
using HarmonyLib;
using BepInEx.Configuration;
using System.Reflection;
using MoonSharp.Interpreter;
namespace PTXH {
    [BepInPlugin(pluginGuid, pluginName, pluginVersion)]
    [BepInProcess("Poly Bridge 3")]
    [BepInDependency(ConfigurationManager.ConfigurationManager.GUID, BepInDependency.DependencyFlags.HardDependency)]
    
    
    public class PTXH : BaseUnityPlugin {
        
        
        public const string pluginGuid = "org.bepinex.plugins.PTXH";
        public const string pluginName = "PTXH";
        public const string pluginVersion = "1.0.0";
        
        public static ConfigEntry<bool> mEnabled;
        
        public ConfigDefinition mEnabledDef = new ConfigDefinition("PolyTech Extension Hooks", "Enable/Disable Mod");
        
        public Table globals;
        
        private static PTXH _instance;
        
        
        public PTXH(){
            _instance = this;
            mEnabled = Config.Bind(mEnabledDef, false, new ConfigDescription("Controls if the mod should be enabled or disabled", null, new ConfigurationManagerAttributes {Order = 0}));
        }
        
        public static void WriteLog(string message){
            _instance.Logger.LogInfo(message);
        }
        
        void Awake(){
            Harmony.CreateAndPatchAll(typeof(PTXH));
            FieldInfo scriptInfo = typeof(ModApi).GetField("m_Api", BindingFlags.NonPublic | BindingFlags.Static);
            FieldInfo tableInfo = typeof(Script).GetField("m_GlobalTable", BindingFlags.NonPublic | BindingFlags.Instance);
            Script api = (Script)scriptInfo.GetValue(null);
            globals = (Table)tableInfo.GetValue(api);
            RegisterCommand<Action<string>>("log", new Action<string>(WriteLog));
            
        }
        void Update(){
            
        }
        
        private void AlertBox(string message){
            ModApi.AddErrorMessageToQueue(message);
        }
        
        private bool RegisterCommand<T>(string command, T action){
            return AttemptCommandRegistration(command, action, "PTXH Core");
        }
        
        private bool verify_command(string command, string sourceName){
            if (globals.Get(command).Type != DataType.Nil){
                AlertBox(sourceName + " - Command Already Registered: " + command);
                return false;
            }
            return true;
        }
        
        public bool AttemptCommandRegistration<T>(string command, T action, string sourceName){
            bool valid = verify_command(command, sourceName);
            if(valid){
                globals[command] = action;
            }
            return valid;
        }
    }
}