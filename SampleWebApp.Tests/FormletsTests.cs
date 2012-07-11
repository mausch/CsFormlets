using System;
using System.Linq;
using Xunit;
using SampleWebApp.Formlets;
using Microsoft.FSharp.Core;
using FSharpx;

namespace SampleWebApp.Tests
{
    public class FormletsTests
    {
        [Fact]
        public void CardExpiration_validates_past_dates() {
            var twoMonthsAgo = DateTime.Now.AddMonths(-2);
            var result = SignupFormlet.CardExpiration(DateTime.Now).Run(new[] {
                twoMonthsAgo.Month.ToString(),
                twoMonthsAgo.Year.ToString(),
            });

            Assert.False(result.Value.HasValue());
            Assert.True(result.Errors.Any(e => e.Contains("Card expired")));
        }

        [Fact]
        public void CardExpiration_collects_correct_expiration_date() {
            var now = DateTime.Now;
            var twoMonthsFuture = now.AddMonths(2);
            var result = SignupFormlet.CardExpiration(now).Run(new[] {
                twoMonthsFuture.Month.ToString(),
                twoMonthsFuture.Year.ToString(),
            });
            Assert.True(result.Value.HasValue());
            Assert.Equal(0, result.Errors.Length);
            var threeMonthsFuture = now.AddMonths(3);
            Assert.Equal(new DateTime(threeMonthsFuture.Year, threeMonthsFuture.Month, 1), result.Value.Value);
        }
    }
}
