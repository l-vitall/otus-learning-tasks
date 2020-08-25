using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Dynamic.Core.Exceptions;
using System.Linq.Expressions;
using AutoMapper.Internal;

namespace Calories.Common.Infrastructure
{
    public class SearchOptionsProcessor<T,TEntity>
    {
        private readonly string _searchQueryOrigin;
        public string SearchQuery { get; }
        
        private static Dictionary<string, string> _operators = new Dictionary<string, string>() {
            {" eq ", " == "}
            , {" ne ", " != "}
            , {" gt ", " > "}
            , {" lt ", " < "}
            , {" AND ", " && "}
            , {" OR ", " || "}
        };

        public SearchOptionsProcessor(string searchQuery)
        {
            _searchQueryOrigin = searchQuery;
            SearchQuery = PrepareExpression(searchQuery);
        }

        public string TryGetInvalidTerm()
        {
            try
            {
                var p = Expression.Parameter(typeof(TEntity), typeof(TEntity).Name);
                var e = DynamicExpressionParser.ParseLambda(new[] {p}, null, SearchQuery);
                return null;
            }
            catch (ParseException exception) when (exception.Message.StartsWith("No property or field"))
            {
                var startIndex = exception.Message.IndexOf("'", StringComparison.Ordinal);
                if (startIndex > 0)
                {
                    var endIndex = exception.Message.IndexOf("'", startIndex + 1, StringComparison.Ordinal);
                    if (endIndex > 0)
                        return exception.Message.Substring(startIndex + 1, endIndex - startIndex - 1);
                }
                
                return _searchQueryOrigin;
            }
            catch
            {
                return _searchQueryOrigin;
            }
        }

        public static string[] GetOperators()
        {
            return _operators.Keys.Select(k => k.Trim()).ToArray();
        }
        
        private static string PrepareExpression(string expression)
        {
            _operators.ForAll(o => expression = expression.Replace(o.Key, o.Value, StringComparison.OrdinalIgnoreCase));
            return expression.Replace("'", "\"");
        }
        
        public IQueryable<TEntity> Apply(IQueryable<TEntity> query)
        {
            return query.Where(SearchQuery);
        }
    }
}