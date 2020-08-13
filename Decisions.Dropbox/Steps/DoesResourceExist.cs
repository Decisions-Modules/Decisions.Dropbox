using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DecisionsFramework.Design.ConfigurationStorage.Attributes;
using DecisionsFramework.Design.Flow;
using DecisionsFramework.Design.Flow.Mapping;
using DecisionsFramework.Design.Properties;

namespace Decisions.DropboxApi
{
    [AutoRegisterStep("Does Resource Exist", DropboxCategory)]
    [Writable]
    public class DoesResourceExist : AbstractStep
    {
        [PropertyHidden]
        public override DataDescription[] InputData
        {
            get
            {
                var data = new DataDescription[] { new DataDescription(typeof(string), fileOrFolderLabel) };
                return base.InputData.Concat(data).ToArray();
            }
        }

        public override OutcomeScenarioData[] OutcomeScenarios
        {
            get
            {
                var data = new OutcomeScenarioData[] { new OutcomeScenarioData(resultOutcomeLabel, new DataDescription(typeof(DropboxResourceType), resultLabel)) };
                return base.OutcomeScenarios.Concat(data).ToArray();
            }
        }

        protected override Object ExecuteStep(string token, StepStartData data)
        {
            string fileOrFolder = (string)data.Data[fileOrFolderLabel];

            var metadata = DropBoxWebClientAPI.GetMetadata(token, fileOrFolder);
            
            if (metadata == null) return DropboxResourceType.Unavailable;
            if (metadata.IsFolder) return DropboxResourceType.Folder;
            return DropboxResourceType.File;
        }
    }
}
