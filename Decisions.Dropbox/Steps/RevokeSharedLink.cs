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
    [AutoRegisterStep("Revoke Shared Link", DropboxCategory)]
    [Writable]
    class RevokeSharedLink : AbstractStep
    {
        [PropertyHidden]
        public override DataDescription[] InputData
        {
            get
            {
                var data = new DataDescription[] { new DataDescription(typeof(string), urlLabel) };
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
            var link = (string)data.Data[urlLabel];
            DropBoxWebClientAPI.RevokeSharedLink(token, link);
            return null;
        }
    }
}



