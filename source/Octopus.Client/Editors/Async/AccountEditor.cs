﻿using System;
using System.Threading.Tasks;
using Octopus.Client.Model.Accounts;
using Octopus.Client.Model.Accounts.Usages;
using Octopus.Client.Repositories.Async;

namespace Octopus.Client.Editors.Async
{
    public class AccountEditor<TAccountResource, TAccountEditor> : IResourceEditor<TAccountResource, TAccountEditor>
        where TAccountResource : AccountResource, new()
        where TAccountEditor : AccountEditor<TAccountResource, TAccountEditor>
    {
        protected readonly IAccountRepository Repository;

        public AccountEditor(IAccountRepository repository)
        {
            Repository = repository;
        }

        public TAccountResource Instance { get; private set; }

        public async Task<TAccountEditor> CreateOrModify(string name)
        {
            var existing = await Repository.FindByName(name);
            if (existing == null)
            {
                Instance = (TAccountResource)await Repository.Create(new TAccountResource
                {
                    Name = name
                });
            }
            else
            {
                if (!(existing is TAccountResource))
                {
                    throw new ArgumentException($"An account with that name exists but is not of type {typeof(TAccountResource).Name}");
                }

                existing.Name = name;

                Instance = (TAccountResource)await Repository.Modify(existing);
            }

            return (TAccountEditor)this;
        }

        public virtual TAccountEditor Customize(Action<TAccountResource> customize)
        {
            customize?.Invoke(Instance);
            return (TAccountEditor)this;
        }

        public virtual async Task<TAccountEditor> Save()
        {
            Instance = (TAccountResource)await Repository.Modify(Instance).ConfigureAwait(false);
            return (TAccountEditor)this;
        }

        public Task<AccountUsageResource> Usage()
        {
            return Repository.Client.Get<AccountUsageResource>(Instance.Link("Usage"));
        }
    }
}