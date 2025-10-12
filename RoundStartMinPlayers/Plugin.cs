using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using LabApi.Features;
using LabApi.Loader.Features.Plugins;
using NorthwoodLib.Pools;
using RoundStartMinPlayers.Configs;
using Version = System.Version;

namespace RoundStartMinPlayers
{
    public class RoundStartMinPlayers : Plugin<Config>
    {
        public static RoundStartMinPlayers Instance;

        private Harmony _harmony;
        public override string Author => "scp252arc";

        public override string Name => "RoundStartMinPlayers";

        public override string Description => "RoundStartMinPlayers";

        public override Version Version => new Version(1, 0, 0);

        public override Version RequiredApiVersion => new Version(LabApiProperties.CompiledVersion);

        public override void Enable()
        {
            Instance = this;

            _harmony = new Harmony("com.RoundStartMinPlayers");
            _harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        public override void Disable()
        {
            Instance = null;

            _harmony.UnpatchAll(_harmony.Id);
            _harmony = null;
        }
    }

    [HarmonyPatch]
    internal static class Patch
    {
        private static IEnumerable<MethodBase> TargetMethods()
        {
            var type = typeof(CharacterClassManager).Assembly.GetType("CharacterClassManager+<Init>d__26");
            var method = type.GetMethod("MoveNext",
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            yield return method;
        }

        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            int offset = 3;
            int index = newInstructions.FindIndex(instruction =>
                instruction.opcode == OpCodes.Ldsfld && instruction.operand is FieldInfo { Name: "LobbyLock" }) + offset;

            newInstructions[index] =
                new CodeInstruction(OpCodes.Ldc_I4, RoundStartMinPlayers.Instance.Config!.MinPlayers - 1);

            foreach (var t in newInstructions)
                yield return t;

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}