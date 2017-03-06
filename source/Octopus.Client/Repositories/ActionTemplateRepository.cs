using System;
using System.Collections.Generic;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories
{
    public interface IActionTemplateRepository : ICreate<ActionTemplateResource>, IModify<ActionTemplateResource>, IDelete<ActionTemplateResource>, IGet<ActionTemplateResource>, IFindByName<ActionTemplateResource>, IGetAll<ActionTemplateResource>
    {
        List<ActionTemplateSearchResource> Search();
        ActionTemplateVersionResource GetVersion(ActionTemplateResource resource, int version);
        ActionUpdateResultResource[] UpdateActions(ActionTemplateResource actionTemplate, ActionsUpdateResource update);
    }
    
    class ActionTemplateRepository : BasicRepository<ActionTemplateResource>, IActionTemplateRepository
    {
        public ActionTemplateRepository(IOctopusClient client) : base(client, "ActionTemplates")
        {
        }

        public List<ActionTemplateSearchResource> Search()
        {
            return Client.Get<List<ActionTemplateSearchResource>>(Client.RootDocument.Link("ActionTemplatesSearch"));
        }

        public ActionTemplateVersionResource GetVersion(ActionTemplateResource resource, int version)
        {
            return Client.Get<ActionTemplateVersionResource>(resource.Links["Version"], new {version});
        }

        public ActionUpdateResultResource[] UpdateActions(ActionTemplateResource actionTemplate, ActionsUpdateResource update)
        {
            return Client.Post<ActionsUpdateResource, ActionUpdateResultResource[]>(actionTemplate.Links["ActionsUpdate"], update, new { actionTemplate.Id });
        }
    }
}