using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Xunit;
using SampleWebApp.Formlets;
using Formlets.CSharp;

namespace SampleWebApp.Tests
{
    public class FormletsTests
    {
        [Fact]
        public void CardExpiration_validates_past_dates()
        {
            var twoMonthsAgo = DateTime.Now.AddMonths(-2);
            var result = SignupFormlet.CardExpiration().Run(new NameValueCollection { 
                {"f0", twoMonthsAgo.Month.ToString()},
                {"f1", twoMonthsAgo.Year.ToString()},
            });
            Assert.True(result.Value.IsNone());
            Assert.True(result.Errors.Any(e => e.Contains("Card expired")));
        }

        [Fact]
        public void CardExpiration_collects_correct_expiration_date()
        {
            var now = DateTime.Now;
            var twoMonthsFuture = now.AddMonths(2);
            var result = SignupFormlet.CardExpiration().Run(new NameValueCollection { 
                {"f0", twoMonthsFuture.Month.ToString()},
                {"f1", twoMonthsFuture.Year.ToString()},
            });
            Assert.True(result.Value.IsSome());
            Assert.Equal(0, result.Errors.Count);
            var threeMonthsFuture = now.AddMonths(3);
            Assert.Equal(new DateTime(threeMonthsFuture.Year, threeMonthsFuture.Month, 1), result.Value.Value);
        }
    }
}
