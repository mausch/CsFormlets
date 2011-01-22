using Microsoft.FSharp.Collections;
using System.Collections.Generic;

namespace Formlets.CSharp {
    public static class FsList {
        public static FSharpList<T> New<T>(params T[] elems) {
            return SeqModule.ToList(elems);
        }

        public static FSharpList<T> ToFsList<T>(this IEnumerable<T> source) {
            return SeqModule.ToList(source);
        }
    }
}