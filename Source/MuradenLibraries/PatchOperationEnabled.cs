using System.Collections.Generic;
using System.Xml;
using Verse;

namespace MuridenLibraries
{
    public class PatchOperationEnabled : PatchOperation
    {
        // All child operations inside this wrapper
        public List<PatchOperation> operations;

        protected override bool ApplyWorker(XmlDocument xml)
        {
            // The variable you already have in settings
            if (!MuridenMod.Settings.dirtmolePatch)
            {
                // Skip all operations when disabled
                return true;
            }

            bool result = true;

            // Execute inner operations normally
            foreach (PatchOperation op in operations)
            {
                if (!op.Apply(xml))
                    result = false;
            }

            return result;
        }
    }
}