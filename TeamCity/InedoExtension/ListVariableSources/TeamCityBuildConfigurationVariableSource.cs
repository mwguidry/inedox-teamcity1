﻿using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Inedo.Extensions.TeamCity.Credentials;
using Inedo.Documentation;
using Inedo.Extensibility.Credentials;
using Inedo.Extensibility.ListVariableSources;
using Inedo.Serialization;
using Inedo.Web;
using Inedo.Extensions.TeamCity.SuggestionProviders;

namespace Inedo.Extensions.TeamCity.ListVariableSources
{
    [DisplayName("TeamCity Build Configuration")]
    [Description("Build configurations in a specified project in a TeamCity instance.")]
    public sealed class TeamCityBuildConfigurationVariableSource : ListVariableSource, IHasCredentials<TeamCityCredentials>
    {
        [Persistent]
        [DisplayName("Credentials")]
        [TriggerPostBackOnChange]
        [Required]
        public string CredentialName { get; set; }

        [Persistent]
        [DisplayName("Project name")]
        [SuggestableValue(typeof(ProjectNameSuggestionProvider))]
        [Required]
        public string ProjectName { get; set; }

        public override async Task<IEnumerable<string>> EnumerateValuesAsync(ValueEnumerationContext context)
        {
            var credentials = ResourceCredentials.Create<TeamCityCredentials>(this.CredentialName);

            using (var client = new TeamCityWebClient(credentials))
            {
                return await client.GetBuildTypeNamesAsync(this.ProjectName).ConfigureAwait(false);
            }
        }

        public override RichDescription GetDescription() =>
            new RichDescription("TeamCity (", new Hilite(this.CredentialName), ") ", " build configurations in ", new Hilite(this.ProjectName), ".");
    }
}
