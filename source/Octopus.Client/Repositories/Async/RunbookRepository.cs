﻿using System.Threading.Tasks;
using Octopus.Client.Editors.Async;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories.Async
{
    public interface IRunbookRepository : IFindByName<RunbookResource>, IGet<RunbookResource>, ICreate<RunbookResource>, IModify<RunbookResource>, IDelete<RunbookResource>
    {
        Task<RunbookResource> FindByName(ProjectResource project, string name);
        Task<RunbookEditor> CreateOrModify(ProjectResource project, string name, string description);
        Task<RunbookSnapshotTemplateResource> GetRunbookSnapshotTemplate(RunbookResource runbook);
        Task<RunbookRunTemplateResource> GetRunbookRunTemplate(RunbookResource runbook);
        Task<RunbookRunPreviewResource> GetPreview(DeploymentPromotionTarget promotionTarget);
        Task<RunbookRunResource> RunPublished(string projectName, string runbookName, string environmentName, string tenantName);
    }

    class RunbookRepository : BasicRepository<RunbookResource>, IRunbookRepository
    {
        public RunbookRepository(IOctopusAsyncRepository repository)
            : base(repository, "Runbooks")
        {
        }

        public Task<RunbookResource> FindByName(ProjectResource project, string name)
        {
            return FindByName(name, path: project.Link("Runbooks"));
        }

        public Task<RunbookEditor> CreateOrModify(ProjectResource project, string name, string description)
        {
            return new RunbookEditor(this, new RunbookProcessRepository(Repository)).CreateOrModify(project, name, description);
        }

        public Task<RunbookSnapshotTemplateResource> GetRunbookSnapshotTemplate(RunbookResource runbook)
        {
            return Client.Get<RunbookSnapshotTemplateResource>(runbook.Link("RunbookSnapshotTemplate"));
        }

        public Task<RunbookRunTemplateResource> GetRunbookRunTemplate(RunbookResource runbook)
        {
            return Client.Get<RunbookRunTemplateResource>(runbook.Link("RunbookRunTemplate"));
        }

        public Task<RunbookRunPreviewResource> GetPreview(DeploymentPromotionTarget promotionTarget)
        {
            return Client.Get<RunbookRunPreviewResource>(promotionTarget.Link("RunbookRunPreview"));
        }
        
        public async Task<RunbookRunResource> RunPublished(string projectName, string runbookName, string environmentName, string tenantName)
        {
            var root = await Repository.LoadRootDocument().ConfigureAwait(false);
            
            RunPublishedRunbookResource runPublished = new RunPublishedRunbookResource()
            {
                ProjectName = projectName,
                RunbookName = runbookName,
                EnvironmentName = environmentName,
                TenantName = tenantName
            };
            
            return await Client.Post<object, RunbookRunResource>(root.LinkToRunbooksRunPublished(), runPublished).ConfigureAwait(false);
        }
    }

}
