using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Calories.Common.Infrastructure;

namespace Calories.Common.Models
{
    public class SearchOptions<T,TEntity> : IValidatableObject
    {
        public string Search { get; set; }
        
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if(string.IsNullOrEmpty(Search))
                yield break;
            
            var processor = new SearchOptionsProcessor<T, TEntity>(Search);
            string invalidTerm = processor.TryGetInvalidTerm();

            if (!string.IsNullOrEmpty(invalidTerm))
            {
                yield return new ValidationResult(
                    $"Invalid search term: '{invalidTerm}'.",
                    new[] {nameof(Search)});
            }
        }

        public IQueryable<TEntity> Apply(IQueryable<TEntity> query)
        {
            if (string.IsNullOrEmpty(Search))
                return query;
            
            var processor = new SearchOptionsProcessor<T, TEntity>(Search);
            return processor.Apply(query);
        }
    }
}