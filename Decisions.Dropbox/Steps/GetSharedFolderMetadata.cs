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
    [AutoRegisterStep("Get Shared Folder Metadata", DropboxCategory)]
    [Writable]
    class GetSharedFolderMetadata : AbstractStep
    {
        [PropertyHidden]
        public override DataDescription[] InputData
        {
            get
            {
                var data = new DataDescription[] { new DataDescription(typeof(string), folderLabel) };
                return base.InputData.Concat(data).ToArray();
            }
        }

        public override OutcomeScenarioData[] OutcomeScenarios
        {
            get
            {
                var data = new OutcomeScenarioData[] { new OutcomeScenarioData(resultOutcomeLabel, new DataDescription(typeof(DropboxSharedFolderMetadata), resultLabel)) };
                return base.OutcomeScenarios.Concat(data).ToArray();
            }
        }

        protected override Object ExecuteStep(string token, StepStartData data)
        {
            var folder = (string)data.Data[folderLabel];
            return DropBoxWebClientAPI.GetSharedFolderMetadata(token, folder);
        }
    }
}
