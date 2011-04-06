using Microsoft.FSharp.Collections;
using System.Collections.Generic;

namespace Formlets.CSharp {
    /// <summary>
    /// F# list helpers
    /// </summary>
    public static class FsList {
        /// <summary>
        /// Creates a new F# list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="elems"></param>
        /// <returns></returns>
        public static FSharpList<T> New<T>(params T[] elems) {
            return SeqModule.ToList(elems);
        }

        /// <summary>
        /// Converts to F# list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static FSharpList<T> ToFsList<T>(this IEnumerable<T> source) {
            return SeqModule.ToList(source);
        }
    }
}