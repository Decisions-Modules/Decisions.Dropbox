using Decisions.DropboxApi;
using DecisionsFramework.Design.ConfigurationStorage.Attributes;
using DecisionsFramework.Design.Flow;
using DecisionsFramework.Design.Flow.Mapping;
using DecisionsFramework.Design.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decisions.DropboxApi
{
    [AutoRegisterStep("Remove Member from Folder", DropboxCategory)]
    [Writable]
    class RemoveMemberFromFolder : AbstractStep
    {
        [PropertyHidden]
        public override DataDescription[] InputData
        {
            get
            {
                var data = new DataDescription[] { new DataDescription(typeof(string), folderLabel),
                                                   new DataDescription(typeof(string), EmailLabel),
                                                 };
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
            var folderPath = (string)data.Data[folderLabel];
            var email = (string)data.Data[EmailLabel];

            DropBoxWebClientAPI.RemoveMemberFromFolder(token, folderPath, email);
            return null;
        }
    }
}






