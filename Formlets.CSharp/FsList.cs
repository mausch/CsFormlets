using Microsoft.FSharp.Collections;

namespace Formlets.CSharp {
    public static class FsList {
        public static FSharpList<T> New<T>(params T[] elems) {
            return SeqModule.ToList(elems);
        }
    }
}