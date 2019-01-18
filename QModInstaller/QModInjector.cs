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

            if (managedDir == null)
			{
				managedDirectory = Path.Combine(graveyardKeeperDirectory, @"Graveyard Keeper_Data\Managed");
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

            // read dll
            var gameLib = AssemblyDefinition.ReadAssembly(mainFilename);

            // delete old backups
            if (File.Exists(backupFilename))
                File.Delete(backupFilename);
            
            
            Logger.WriteLog("DEBUG: Creating new backup at " + backupFilename);
            // save a copy of the dll as a backup

            File.Copy(mainFilename, backupFilename);

            // load patcher module
            var modLib = AssemblyDefinition.ReadAssembly(installerFilename);
            var patchMethod = modLib.MainModule.GetType("QModInstaller.QModPatcher").Methods.Single(x => x.Name == "Patch");
            // target the injection method
            var type = gameLib.MainModule.GetType("MainMenuGUI");
            var method = type.Methods.First(x => x.Name == "Open");
            // inject
            method.Body.GetILProcessor().InsertBefore(method.Body.Instructions[0], Instruction.Create(OpCodes.Call, method.Module.Import(patchMethod)));

            // save changes under original filename
            try
            {
                gameLib.Write(mainFilename);
            }
            catch (Exception e)
            {

                Logger.WriteLog("ERROR: " + e.ToString());
            }
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
