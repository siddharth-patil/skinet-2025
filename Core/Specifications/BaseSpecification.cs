using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Core.Specifications
{
    public class BaseSpecification<T>(Expression<Func<T, bool>>? criteria) : ISpecification<T>
    {
        protected BaseSpecification() : this(null) 
        {
        
        }
        
        public Expression<Func<T, bool>>? Criteria => criteria;

        public Expression<Func<T, object>>? OrderBy{get; private set;}

        public Expression<Func<T, object>>? OrderByDescending { get; private set; }

        public bool IsDistinct { get; private set; }

        public int Take { get; private set; }

        public int Skip { get; private set; }

        public bool IsPagingEnabled { get; private set; }

        public List<Expression<Func<T, object>>> Includes { get; } = [];

        public List<string> IncludeStrings { get; } = []; // for ThenInclude

        public IQueryable<T> ApplyCriteria(IQueryable<T> query)
        {
            if (Criteria != null)
            {
                query = query.Where(Criteria); // p => p.Brand == brand
            }

            return query;
        }

        protected void AddInclude(Expression<Func<T, object>> includeExpression)
        {
            Includes.Add(includeExpression);
        }
        protected void AddInclude(string includeString)
        {
            IncludeStrings.Add(includeString); // for ThenInclude
        }

        protected void AddOrderBy(Expression<Func<T,object>> orderByExpression)
        {
            OrderBy = orderByExpression;
        }
        protected void AddOrderByDescinding(Expression<Func<T,object>> orderByDescindingExpression)
        {
            OrderByDescending = orderByDescindingExpression;
        }
        protected void ApplyDistinct()
        {
            IsDistinct = true;
        }

        protected void ApplyPaging(int skip, int take)
        {
            Skip = skip;
            Take = take;
            IsPagingEnabled = true;
        }
    }

    public class BaseSpecification<T, TResult>(Expression<Func<T, bool>>? criteria) : 
        BaseSpecification<T>(criteria), ISpecification<T, TResult>
    {
        public BaseSpecification() : this(null)
        {
        }
        public Expression<Func<T, TResult>>? Select { get; private set; }
        protected void AddSelect(Expression<Func<T, TResult>> selectExpression)
        {
            Select = selectExpression;
        }
    }
}
