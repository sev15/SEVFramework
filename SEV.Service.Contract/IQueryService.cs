﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using SEV.Domain.Model;

namespace SEV.Service.Contract
{
    public interface IQueryService
    {
        IEnumerable<T> Read<T>(params Expression<Func<T, object>>[] includes)
            where T : Entity;
        T FindById<T>(string id, params Expression<Func<T, object>>[] includes)
            where T : Entity;
        IEnumerable<T> FindByIdList<T>(IEnumerable<string> ids, params Expression<Func<T, object>>[] includes)
            where T : Entity;
        IEnumerable<T> FindByQuery<T>(IQuery<T> query)
            where T : Entity;

        Task<IEnumerable<T>> ReadAsync<T>(params Expression<Func<T, object>>[] includes)
            where T : Entity;
        Task<T> FindByIdAsync<T>(string id, params Expression<Func<T, object>>[] includes)
            where T : Entity;
        Task<IEnumerable<T>> FindByIdListAsync<T>(IEnumerable<string> ids, params Expression<Func<T, object>>[] includes)
            where T : Entity;
        Task<IEnumerable<T>> FindByQueryAsync<T>(IQuery<T> query)
            where T : Entity;
    }
}