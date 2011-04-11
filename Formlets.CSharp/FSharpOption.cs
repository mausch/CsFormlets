using System;
using Microsoft.FSharp.Core;
using System.Collections.Generic;

namespace Formlets.CSharp {
    /// <summary>
    /// Extensions to make F# options more usable in C#
    /// </summary>
    public static class FSharpOption {
        /// <summary>
        /// Gets a value indicating whether the current <see cref="FSharpOption{T}"/> object has a value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="option"></param>
        /// <returns>true if the current <see cref="FSharpOption{T}"/> object has a value; false if the current <see cref="FSharpOption{T}"/> object has no value</returns>
        public static bool HasValue<T>(this FSharpOption<T> option) {
            return FSharpOption<T>.get_IsSome(option);
        }

        /// <summary>
        /// Converts this <see cref="Nullable{T}"/> to <see cref="FSharpOption{T}"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static FSharpOption<T> ToOption<T>(this Nullable<T> value) where T: struct {
            if (!value.HasValue)
                return FSharpOption<T>.None;
            return value.Value.ToOption();
        }

        /// <summary>
        /// If this value is null, returns None, otherwise Some(value)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static FSharpOption<T> ToOption<T>(this T value) {
            if (value == null)
                return FSharpOption<T>.None;
            return FSharpOption<T>.Some(value);
        }

        /// <summary>
        /// Converts this <see cref="FSharpOption{T}"/> to <see cref="Nullable{T}"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Nullable<T> ToNullable<T>(this FSharpOption<T> value) where T : struct {
            if (!value.HasValue())
                return null;
            return new Nullable<T>(value.Value);
        }

        public static FSharpOption<R> SelectMany<T, R>(this FSharpOption<T> option, Func<T, FSharpOption<R>> selector) {
            return OptionModule.Bind(FFunc.FromFunc(selector), option);
        }

        public static FSharpOption<R> SelectMany<T, C, R>(this FSharpOption<T> option, Func<T, FSharpOption<C>> optionSelector, Func<T, C, R> resultSelector) {
            var opt = option.SelectMany(optionSelector);
            Func<FSharpOption<T>, FSharpOption<C>, FSharpOption<R>> liftedResultSelector = (t,c) => t.SelectMany(tt => c.SelectMany(cc => resultSelector(tt, cc).ToOption()));
            return liftedResultSelector(option, opt);
        }

        public static FSharpOption<R> Select<T, R>(this FSharpOption<T> option, Func<T,R> selector) {
            return OptionModule.Map(FFunc.FromFunc(selector), option);
        }

        public static FSharpOption<T> Where<T>(this FSharpOption<T> option, Func<T, bool> predicate) {
            if (OptionModule.Exists(FFunc.FromFunc(predicate), option))
                return option;
            return FSharpOption<T>.None;
        }

        public static readonly FSharpOption<Unit> SomeUnit = FSharpOption<Unit>.Some(null);
    }
}