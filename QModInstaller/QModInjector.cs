using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.IO;
using System.Linq;

namespace QModInstaller
{
    public class QModInjector
    {
        private string graveyardKeeperDirectory;
        private string managedDirectory;
        private string installerFilename = @"QModInstaller.dll";
        private string mainFilename = @"\Assembly-CSharp.dll";
        private string backupFilename = @"\Assembly-CSharp.qoriginal.dll";

        public QModInjector(string dir, string managedDir = null)
        {
            graveyardKeeperDirectory = dir;
            Logger.WriteLog("DEBUG: In QModInjector Constructor");
            Logger.WriteLog("Graveyard Keeper directory: " + managedDir);

            if (managedDir == null)
			{
				managedDirectory = Path.Combine(graveyardKeeperDirectory, @"Graveyard Keeper_Data\Managed");
                Logger.WriteLog("Null managed: " + managedDirectory);
			}
			else
			{
				managedDirectory = managedDir;
			}
            mainFilename = managedDirectory + mainFilename;
            backupFilename = managedDirectory + backupFilename;
        }


        public bool IsPatcherInjected()
        {
            Logger.WriteLog("DEBUG: QModInjector.IsPatcherInjected");
            try
            {
                return IsInjected();
            }
            catch(Exception e)
            {
                Logger.WriteLog(e.ToString());
            }
            return false;

        }


        public bool Inject()
        {
            if (IsInjected()) return false;
            Logger.WriteLog("DEBUG: Injector.Inject");

            // read dll
            Logger.WriteLog("DEBUG: Reading in game assembly");
            var gameLib = AssemblyDefinition.ReadAssembly(mainFilename);

            Logger.WriteLog("DEBUG: Deleting old backup");
            // delete old backups
            if (File.Exists(backupFilename))
                File.Delete(backupFilename);
            
            
            Logger.WriteLog("DEBUG: Creating new backup at " + backupFilename);
            // save a copy of the dll as a backup

            File.Copy(mainFilename, backupFilename);

            Logger.WriteLog("DEBUG: Reading in mod assembly");
            // load patcher module
            var modLib = AssemblyDefinition.ReadAssembly(installerFilename);
            var patchMethod = modLib.MainModule.GetType("QModInstaller.QModPatcher").Methods.Single(x => x.Name == "Patch");
            Logger.WriteLog("DEBUG: Targetting the injection method - MainMenuGUI.Open");
            // target the injection method
            var type = gameLib.MainModule.GetType("MainMenuGUI");
            var method = type.Methods.First(x => x.Name == "Open");
            Logger.WriteLog("DEBUG: Beginning Injection");
            // inject
            method.Body.GetILProcessor().InsertBefore(method.Body.Instructions[0], Instruction.Create(OpCodes.Call, method.Module.Import(patchMethod)));
            Logger.WriteLog("DEBUG: Attempting to write the assembly changes");

            // save changes under original filename
            try
            {
                gameLib.Write(mainFilename);
            }
            catch (Exception e)
            {

                Logger.WriteLog("ERROR: " + e.ToString());
            }
            Logger.WriteLog("DEBUG: Creating QMods dir if it's been deleted.");
            if (!Directory.Exists(graveyardKeeperDirectory + @"\QMods"))
                Directory.CreateDirectory(graveyardKeeperDirectory + @"\QMods");

            return true;
        }


        public bool Remove()
        {
            // if a backup exists
            if (File.Exists(backupFilename))
            {
                // remove the dirty dll
                File.Delete(mainFilename);

                // move the backup into its place
                File.Move(backupFilename, mainFilename);

                return true;
            }

            return false;
        }


        private bool IsInjected()
        {
            Logger.WriteLog("DEBUG: QModInjector.isInjected");
            Logger.WriteLog("DEBUG: Reading main assembly file " + mainFilename);
           
            var gameLib = AssemblyDefinition.ReadAssembly(mainFilename);
            var type = gameLib.MainModule.GetType("MainMenuGUI");

            var method = type.Methods.First(x => x.Name == "Open");
            
            var installer = AssemblyDefinition.ReadAssembly(installerFilename);

            var patchMethod = installer.MainModule.GetType("QModInstaller.QModPatcher").Methods.FirstOrDefault(x => x.Name == "Patch");
            bool patched = false;
            foreach (var instruction in method.Body.Instructions)
            {
                if (instruction.OpCode.Equals(OpCodes.Call) && instruction.Operand.ToString().Equals("System.Void QModInstaller.QModPatcher::Patch()"))
                {
                    return true;
                }
            }
            return patched;
        }
    }
}
