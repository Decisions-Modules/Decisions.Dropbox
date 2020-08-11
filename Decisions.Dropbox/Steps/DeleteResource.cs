using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DecisionsFramework.Design.ConfigurationStorage.Attributes;
using DecisionsFramework.Design.Flow;
using DecisionsFramework.Design.Flow.Mapping;
using DecisionsFramework.Design.Properties;

namespace Decisions.DropboxApi
{
    [AutoRegisterStep("Delete Resource", DropboxCategory)]
    [Writable]
    public class DeleteResource : AbstractStep
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
                var data = new OutcomeScenarioData[] { new OutcomeScenarioData(resultOutcomeLabel) };
                return base.OutcomeScenarios.Concat(data).ToArray();
            }
        }

        protected override Object ExecuteStep(string token, StepStartData data)
        {
            var fileOrFolderId = (string)data.Data[fileOrFolderLabel];

            DropBoxWebClientAPI.DeleteResource(token, fileOrFolderId);
            return null;
        }
    }
}
