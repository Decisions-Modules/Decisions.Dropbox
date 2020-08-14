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
    [AutoRegisterStep("Remove Member from File", DropboxCategory)]
    [Writable]
    class RemoveMemberFromFile : AbstractStep
    {
        [PropertyHidden]
        public override DataDescription[] InputData
        {
            get
            {
                var data = new DataDescription[] {
                                                   new DataDescription(typeof(string), fileLabel),
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
            var filePath = (string)data.Data[fileLabel];
            var email = (string)data.Data[EmailLabel];

            DropBoxWebClientAPI.RemoveMemberFromFile(token, filePath, email);
            return null;
        }
    }
}




