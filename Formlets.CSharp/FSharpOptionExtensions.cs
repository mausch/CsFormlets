using System;
using Microsoft.FSharp.Core;

namespace Formlets.CSharp {
    /// <summary>
    /// Extensions to make F# options more usable in C#
    /// </summary>
    public static class FSharpOptionExtensions {
        public static bool IsSome<T>(this FSharpOption<T> option) {
            return FSharpOption<T>.get_IsSome(option);
        }

        public static bool IsNone<T>(this FSharpOption<T> option) {
            return FSharpOption<T>.get_IsNone(option);
        }

        public static FSharpOption<T> ToOption<T>(this Nullable<T> value) where T: struct {
            if (!value.HasValue)
                return FSharpOption<T>.None;
            return value.Value.ToOption();
        }

        public static FSharpOption<T> ToOption<T>(this T value) {
            if (value == null)
                return FSharpOption<T>.None;
            return FSharpOption<T>.Some(value);
        }
    }
}