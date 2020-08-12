using Decisions.OAuth;
using DecisionsFramework;
using DecisionsFramework.Data.ORMapper;
using DecisionsFramework.Design.ConfigurationStorage.Attributes;
using DecisionsFramework.Design.Flow;
using DecisionsFramework.Design.Flow.Mapping;
using DecisionsFramework.Design.Properties;
using DecisionsFramework.Design.Properties.Attributes;
using DecisionsFramework.ServiceLayer.Services.ContextData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decisions.DropboxApi
{
    [Writable]
    public abstract class AbstractStep : ISyncStep, IDataConsumer, IDataProducer, IValidationSource
    {
        public const string DropboxCategory = "Integration/Dropbox";

        protected const string errorOutcomeLabel = "Error";
        protected const string resultOutcomeLabel = "Result";
        //protected const string doneOutcomeLabel = "Done";
        protected const string errorOutcomeDataLabel = "Error info";
        protected const string resultLabel = "Result";

        protected const string tokenLabel = "Token";
        protected const string fileOrFolderLabel = "File Or Folder Path";
        protected const string fileLabel = "File Path";
        protected const string folderLabel = "Folder Path";
        protected const string localFilePathLabel = "Local File Path";
        protected const string localFolderPathLabel = "Local Folder Path";

        protected const string newFolderPath = "New Folder Path";

        [PropertyHidden]
        public virtual DataDescription[] InputData
        {
            get
            {
                return new DataDescription[] { };
            }
        }

        private const int errorOutcomeIndex = 0;
        private const int resultOutcomeIndex = 1;
        public virtual OutcomeScenarioData[] OutcomeScenarios
        {
            get
            {
                return new OutcomeScenarioData[] { new OutcomeScenarioData(errorOutcomeLabel, new DataDescription(typeof(DropboxErrorInfo), errorOutcomeDataLabel)) };
            }
        }

        [TokenPicker]
        [WritableValue]
        public string Token { get; set; }

        private string FindAccessToken(string id)
        {
            ORM<OAuthToken> orm = new ORM<OAuthToken>();
            var token = orm.Fetch(id);
            if (token != null)
                return token.TokenData;
            throw new EntityNotFoundException($"Can not find token with TokenId=\"{id}\"");
        }

        public ResultData Run(StepStartData data)
        {
            try
            {
                var accessToken = FindAccessToken(Token);

                Object res = ExecuteStep(accessToken, data);

                var outputData = OutcomeScenarios[resultOutcomeIndex].OutputData;
                var exitPointName = OutcomeScenarios[resultOutcomeIndex].ExitPointName;

                if (outputData != null && outputData.Length > 0)
                    return new ResultData(exitPointName, new DataPair[] { new DataPair(outputData[0].Name, res) });
                else
                    return new ResultData(exitPointName);
            }
            catch (Exception ex)
            {
                DropboxErrorInfo ErrInfo = DropboxErrorInfo.FromException(ex);
                return new ResultData(errorOutcomeLabel, new DataPair[] { new DataPair(errorOutcomeDataLabel, ErrInfo) });
            }
        }

        protected abstract Object ExecuteStep(string token, StepStartData data);

        public ValidationIssue[] GetValidationIssues()
        {
            if (Token == null)
                return new ValidationIssue[] { new ValidationIssue("Token cannot be null") };

            try
            {
                var accessToken = FindAccessToken(Token);
            }
            catch (Exception ex)
            {
                return new ValidationIssue[] { new ValidationIssue(ex.Message ?? ex.ToString()) };
            }
            return new ValidationIssue[0];
        }

    }
}
