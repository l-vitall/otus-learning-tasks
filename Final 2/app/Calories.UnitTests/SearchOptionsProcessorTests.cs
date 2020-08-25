using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using Calories.Common.Infrastructure;
using Calories.Common.Models;
using Calories.Records.Models;
using Xunit;

namespace Calories.UnitTests
{
    public class SearchOptionsProcessorTests
    {
        [Theory]
        [InlineData("(Date eq '2016-05-01') AND ((NumberOfCalories gt 20) OR (Text eq 'Bread'))")]
        [InlineData("((NumberOfCalories gt 20) OR (NumberOfCalories lt 10))")]
        [InlineData("Date eq '2016-05-01'")]
        public void GetValidTerms_ValidInput(string expression)
        {
             var processor = new SearchOptionsProcessor<CaloriesRecord,CaloriesRecordEntity>(expression);
            
             Assert.Null(processor.TryGetInvalidTerm());
        }
        
        [Fact]
        public void GetValidTerms_InvalidColumn()
        {
            var processor = new SearchOptionsProcessor<CaloriesRecord,CaloriesRecordEntity>("(CreationDate eq '2016-05-01') AND ((NumberOfCalories gt 20) OR (NumberOfCalories lt 10))");
            
            Assert.Equal("CreationDate", processor.TryGetInvalidTerm());
        }
        
        [Fact]
        public void GetValidTerms_InvalidOperator()
        {
            var expression = "(Date eqe '2016-05-01') AND ((NumberOfCalories gt 20) OR (NumberOfCalories lt 10))";
            var processor = new SearchOptionsProcessor<CaloriesRecord,CaloriesRecordEntity>(expression);
            
            Assert.Equal(expression, processor.TryGetInvalidTerm());
        }
        
        [Fact]
        public void GetValidTerms_InvalidSyntax()
        {
            var expression = "(Date eq '2016-05-01')) AND ((NumberOfCalories gt 20) OR (NumberOfCalories lt 10))";
            var processor = new SearchOptionsProcessor<CaloriesRecord,CaloriesRecordEntity>(expression);
            
            Assert.Equal(expression, processor.TryGetInvalidTerm());
        }
        
        [Fact]
        public void TimeSpanTests()
        {
            List<CaloriesRecordEntity> records = new List<CaloriesRecordEntity>();
            int idCounter = 1;
            records.Add(new CaloriesRecordEntity()
            {
                Id = idCounter++,
                Date = DateTime.Parse("2020-07-21"),
                Time = TimeSpan.Parse("14:31"),
                Text = "Hamburger",
                NumberOfCalories = 900
            });
            
            records.Add(new CaloriesRecordEntity()
            {
                Id = idCounter++,
                Date = DateTime.Parse("2020-07-21"),
                Time = TimeSpan.Parse("15:31"),
                Text = "Cheeseburger",
                NumberOfCalories = 600
            });
            
            records.Add(new CaloriesRecordEntity()
            {
                Id = idCounter++,
                Date = DateTime.Parse("2020-07-21"),
                Time = TimeSpan.Parse("16:31"),
                Text = "Bread",
                NumberOfCalories = 50
            });
            
            records.Add(new CaloriesRecordEntity()
            {
                Id = idCounter++,
                Date = DateTime.Parse("2020-07-21"),
                Time = TimeSpan.Parse("17:31"),
                Text = "Tea",
                NumberOfCalories = 10
            });
            
            records.Add(new CaloriesRecordEntity()
            {
                Id = idCounter++,
                Date = DateTime.Parse("2020-07-22T13:00:00Z").Date,
                Time = TimeSpan.Parse("18:31"),
                Text = "Pizza",
                NumberOfCalories = 1000
            });

            //var processor = new SearchOptionsProcessor<CaloriesRecord,CaloriesRecordEntity>("Time gt '17:00:00'");
            var processor = new SearchOptionsProcessor<CaloriesRecord,CaloriesRecordEntity>("Date eq '2020-07-22'");
            var result = records.AsQueryable().Where(processor.SearchQuery).ToList();
            
            //Assert.Equal("CreationDate", processor.TryGetInvalidTerm());
        }

    }
    
    // public static class QueryableExtensions
    // {
    //     public static IQueryable<T> BindDbFunctions<T>(this IQueryable<T> source)
    //     {
    //         var expression = new DbFunctionsBinder().Visit(source.Expression);
    //         if (expression == source.Expression) return source;
    //         return source.Provider.CreateQuery<T>(expression);
    //     }
    //
    //     public static IQueryable BindDbFunctions(this IQueryable source)
    //     {
    //         var expression = new DbFunctionsBinder().Visit(source.Expression);
    //         if (expression == source.Expression) return source;
    //         return source.Provider.CreateQuery(expression);
    //     }
    //
    //     class DbFunctionsBinder : ExpressionVisitor
    //     {
    //         protected override Expression VisitMember(MemberExpression node)
    //         {
    //             if (node.Expression != null && node.Expression.Type == typeof(DateTime) && node.Member.Name == "Date")
    //             {
    //                 var dateValue = Expression.Convert(Visit(node.Expression), typeof(DateTime?));
    //                 var methodCall = Expression.Call(typeof(DbFunctions), "TruncateTime", Type.EmptyTypes, dateValue);
    //                 return Expression.Convert(methodCall, typeof(DateTime));
    //             }
    //             return base.VisitMember(node);
    //         }
    //     }
    //}
}