using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;

namespace btree
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			var tree = new BTree<int,int> ();
			var rand = new Random (1);
			var items = Enumerable.Range (0, 3000000).Select (n => rand.Next ()).Distinct ().ToArray ();

            PerformanceTest(items);

			TestAdd (tree, items);
			var set = new HashSet<int> (items);
            int i = 0;
            
			foreach (var item in items) {
				tree.Remove (item);
				set.Remove (item);
              
                //if (tree.Count() != set.Count())
                //    throw new Exception("Count mismatch at i=" + i);
                //if(++i % 10001 == 0)
                //Verify (tree);
                //AssertEqual(tree, set);
                i++;
                if (i % 10001 == 0 )
                {
                    Console.WriteLine(i);
                    //AssertEqual(tree, set);
                    //if (!tree.Verify())
                    //    throw new Exception("Invalid tree at " + i);
                   // Verify(tree);
                  
                }
			}
            tree.Verify();
		}

        static void PerformanceTest(IEnumerable<int> items)
        {
            var map = new Dictionary<int, int>();
            var tree = new BTree<int, int>();

            var gc = GC.GetTotalMemory(true);
            var sw = Stopwatch.StartNew();
            foreach (var item in items)            
                map[item] = item;
            var e = sw.Elapsed;
            var mem = GC.GetTotalMemory(true) - gc;
            Console.WriteLine("map write " + e + " mem: " + mem);

            gc = GC.GetTotalMemory(true);
            sw = Stopwatch.StartNew();
            foreach (var item in items)            
                tree[item] = item;
            e = sw.Elapsed;
            var mem2 = GC.GetTotalMemory(true) - gc;
            Console.WriteLine("tree write " + e + "  mem: " + mem2);
            Console.WriteLine("Memory diff: tree uses " + Math.Round((double)mem2 / mem * 100, 2) + "% of dictionary memory");

            sw = Stopwatch.StartNew();
            foreach (var item in items)
            {
                var x = map[item];
            }         
            Console.WriteLine("map read " + sw.Elapsed);

            sw = Stopwatch.StartNew();
            foreach (var item in items)
            {
                var x = tree[item];
            }
            Console.WriteLine("tree read " + sw.Elapsed);

            sw = Stopwatch.StartNew();
            foreach (var item in items)
            {
                var x = tree[item];
                var y = map[item];
                if (x != y) throw new Exception("mismatch " + item);
            }
            Console.WriteLine("tree read " + sw.Elapsed);
        }

        static void AssertEqual(BTree<int, int> tree, HashSet<int> set)
        {
            var s2 = new HashSet<int>(set);
            foreach (var t in tree)            
                if (!s2.Remove(t.Key))
                    throw new Exception("Missing " + t.Key);
            if (s2.Count != 0)
                throw new Exception("remove failed " + s2.Count);
        }

		static void TestAdd (BTree<int, int> tree, int[] items)
		{
			int i = 0;
			foreach (var item in items) {
				tree.Add (item, item);
				if (++i % 1000 == 0) {
					Console.WriteLine (i);
					//Verify (tree);
				}
			}
			Verify (tree);
            tree.Verify();
		}

		static void Verify (BTree<int, int> tree)
		{
			int? i = null;
			foreach (var item in tree.Select (n => n.Key)) {
				if (i.HasValue)
					if (item <= i.Value)
						throw new Exception ("dup or out of order " + item + " and " + i.Value);
				i = item;
			}
		}
	}
}
