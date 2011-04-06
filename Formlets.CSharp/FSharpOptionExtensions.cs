using System;
using Microsoft.FSharp.Core;

namespace Formlets.CSharp {
    /// <summary>
    /// Extensions to make F# options more usable in C#
    /// </summary>
    public static class FSharpOptionExtensions {
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
    }
}