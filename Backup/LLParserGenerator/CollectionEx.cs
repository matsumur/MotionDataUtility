using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace LLParserGenerator {
    public static class CollectionEx {
        public static IEnumerable<TResult> Zip<TFirst, TSecond, TResult>(IEnumerable<TFirst> first, IEnumerable<TSecond> second, Func<TFirst, TSecond, TResult> resultSelector) {
            using(IEnumerator<TFirst> f = first.GetEnumerator())
            using(IEnumerator<TSecond> s = second.GetEnumerator()) {
                while(f.MoveNext() && s.MoveNext()) {
                    yield return resultSelector(f.Current, s.Current);
                }
            }
        }
    }
}
