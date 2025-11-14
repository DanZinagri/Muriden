using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml;
using Verse;

namespace MuridenLibraries
{
    public abstract class PatchOperationEnabled : PatchOperation
    {
        protected readonly PatchOperation match;
        protected readonly PatchOperation nomatch;
        abstract protected bool ShouldApply();
        protected override bool ApplyWorker(XmlDocument xml)
        {
            if (ShouldApply())
            {
                if (match != null)
                {
                    return match.Apply(xml);
                }
            }
            else if (nomatch != null)
            {
                return nomatch.Apply(xml);
            }
            return true;
        }
    }

    public class PatchOp_dirtmolePatch : PatchOperationEnabled { protected override bool ShouldApply() => MuridenMod.Settings.dirtmolePatch; }
    public class PatchOp_factionPatch : PatchOperationEnabled { protected override bool ShouldApply() => MuridenMod.Settings.factionPatch; }
}