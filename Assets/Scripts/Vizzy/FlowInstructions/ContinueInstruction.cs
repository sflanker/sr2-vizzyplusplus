using System;
using ModApi.Craft.Program;
using ModApi.Craft.Program.Instructions;

namespace Assets.Scripts.Vizzy.FlowInstructions {
    /// <summary>
    /// Jumps back to the top of the closest loop without executing any more instructions.
    /// </summary>
    [Serializable]
    public class ContinueInstruction : ProgramInstruction, IVizzyPlusPlusProgramNode {
        public const String XmlName = "Continue";

        public override ProgramInstruction Execute(IThreadContext context) {
            var stackFrame = context.PopStackFrame();
            while (context.CallStackSize > 0 && (stackFrame?.ReturnInstruction == null || !stackFrame.ReturnInstruction.StopBreakPropagation)) {
                stackFrame = context.PopStackFrame();
            }

            return stackFrame?.ReturnInstruction;
        }
    }
}
