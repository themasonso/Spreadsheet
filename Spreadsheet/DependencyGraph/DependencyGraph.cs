// Skeleton implementation written by Joe Zachary for CS 3500, September 2013.
// Version 1.1 (Fixed error in comment for RemoveDependency.)
// Version 1.2 - Daniel Kopta 
//               (Clarified meaning of dependent and dependee.)
//               (Clarified names in solution/project structure.)
//
// Implementation completed by Mason Seppi for CS 3500, February 2021.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpreadsheetUtilities
{

    /// <summary>
    /// (s1,t1) is an ordered pair of strings
    /// t1 depends on s1; s1 must be evaluated before t1
    /// 
    /// A DependencyGraph can be modeled as a set of ordered pairs of strings.  Two ordered pairs
    /// (s1,t1) and (s2,t2) are considered equal if and only if s1 equals s2 and t1 equals t2.
    /// Recall that sets never contain duplicates.  If an attempt is made to add an element to a 
    /// set, and the element is already in the set, the set remains unchanged.
    /// 
    /// Given a DependencyGraph DG:
    /// 
    ///    (1) If s is a string, the set of all strings t such that (s,t) is in DG is called dependents(s).
    ///        (The set of things that depend on s)    
    ///        
    ///    (2) If s is a string, the set of all strings t such that (t,s) is in DG is called dependees(s).
    ///        (The set of things that s depends on) 
    //
    // For example, suppose DG = {("a", "b"), ("a", "c"), ("b", "d"), ("d", "d")}
    //     dependents("a") = {"b", "c"}
    //     dependents("b") = {"d"}
    //     dependents("c") = {}
    //     dependents("d") = {"d"}
    //     dependees("a") = {}
    //     dependees("b") = {"a"}
    //     dependees("c") = {"a"}
    //     dependees("d") = {"b", "d"}
    /// </summary>
    public class DependencyGraph
    {
        // a dictionary whose key is a node, and whose value is a list of nodes who depend on it.
        private Dictionary<string, HashSet<string>> dependents;
        // a dictionary whose key is a node, and whose value is a list of nodes who it depends on.
        private Dictionary<string, HashSet<string>> dependees;
        // the number of pairs (dependencies) we have.
        private int size = 0;
        /// <summary>
        /// Creates an empty DependencyGraph.
        /// </summary>
        public DependencyGraph()
        {
            dependents = new Dictionary<string, HashSet<string>>();
            dependees = new Dictionary<string, HashSet<string>>();
        }


        /// <summary>
        /// The number of ordered pairs in the DependencyGraph.
        /// </summary>
        public int Size
        {
            get { return size;}
        }


        /// <summary>
        /// The size of dependees(s).
        /// This property is an example of an indexer.  If dg is a DependencyGraph, you would
        /// invoke it like this:
        /// dg["a"]
        /// It should return the size of dependees("a")
        /// </summary>
        public int this[string s]
        {
            get {
                dependees.TryGetValue(s, out HashSet<string> output);
                if(output == null)
                {
                    return 0;
                }
                return output.Count; }
        }


        /// <summary>
        /// Reports whether dependents(s) is non-empty.
        /// </summary>
        public bool HasDependents(string s)
        {
            return dependents.TryGetValue(s, out HashSet<string> output);
        }


        /// <summary>
        /// Reports whether dependees(s) is non-empty.
        /// </summary>
        public bool HasDependees(string s)
        {
            return dependees.TryGetValue(s, out HashSet<string> output);
        }


        /// <summary>
        /// Enumerates dependents(s).
        /// </summary>
        public IEnumerable<string> GetDependents(string s)
        {
            dependents.TryGetValue(s, out HashSet<string> output);
            //if there is no list, send them an empty dummy list
            if (output == null)
            {
                return new List<string>();
            }
            return output;
        }

        /// <summary>
        /// Enumerates dependees(s).
        /// </summary>
        public IEnumerable<string> GetDependees(string s)
        {
            dependees.TryGetValue(s, out HashSet<string> output);
            //if there is no list, send them an empty dummy list
            if(output == null)
            {
                return new List<string>();
            }
            return output;
        }


        /// <summary>
        /// <para>Adds the ordered pair (s,t), if it doesn't exist</para>
        /// 
        /// <para>This should be thought of as:</para>   
        /// 
        ///   t depends on s
        ///
        /// </summary>
        /// <param name="s"> s must be evaluated first. T depends on S</param>
        /// <param name="t"> t cannot be evaluated until s is</param>        /// 
        public void AddDependency(string s, string t)
        {
            dependents.TryGetValue(s, out HashSet<string> output);
            dependees.TryGetValue(t, out HashSet<string> output2);
            //if dependents doesn't contain s, we make a new HashSet of values for dependents.
            if (!dependents.ContainsKey(s))
            {
                HashSet<string> values = new HashSet<string>();
                values.Add(t);
                dependents.Add(s, values);
                size++;
            }
            // if it does contain s we get the HashSet we've already made and add to it.
            else if(!output.Contains(t))
            {
                    output.Add(t);
                    size++;
            }
            //if dependees doesn't contain t, we make a HashSet of values for dependees
            if(!dependees.ContainsKey(t))
            {
                HashSet<string> keys = new HashSet<string>();
                keys.Add(s);
                dependees.Add(t, keys);
            }
            else if (!output2.Contains(s))
            //if it does contain t we add to the currently existing HashSet.
            {
                    output2.Add(s);
            }
        }


        /// <summary>
        /// Removes the ordered pair (s,t), if it exists
        /// </summary>
        /// <param name="s"></param>
        /// <param name="t"></param>
        public void RemoveDependency(string s, string t)
        {
            if (dependents.TryGetValue(s, out HashSet<string> output) && output.Contains(t))
            {
                dependees.TryGetValue(t, out HashSet<string> output2);
                output.Remove(t);
                output2.Remove(s);
                if(output.Count == 0)
                {
                    dependents.Remove(s);
                }
                if(output2.Count == 0)
                {
                    dependees.Remove(t);
                }
                size--;
            }
        }


        /// <summary>
        /// Removes all existing ordered pairs of the form (s,r).  Then, for each
        /// t in newDependents, adds the ordered pair (s,t).
        /// </summary>
        public void ReplaceDependents(string s, IEnumerable<string> newDependents)
        {
            // Get our new Hashset of values and our old HashSet of values
            HashSet<string> newValues = (HashSet<string>)newDependents;
            dependents.TryGetValue(s, out HashSet<string> output);
            // if the output isn't null, copy it so that we can work with it. otherwise, make a new empty HashSet.
            HashSet<string> outputCopy;
            if (output == null)
            {
                outputCopy = new HashSet<string>();
            }
            else
            {
                outputCopy = new HashSet<string>(output);
            }
            // remove key s and all of it's pairs from the dictionary.
            foreach (string deleter in outputCopy)
            {
                RemoveDependency(s, deleter);
            }
            //iterate through each new value and add a dependency for each, then increment size based on the size of newValues.
            foreach (string update in newValues)
            {
                AddDependency(s, update);
            }
        }


        /// <summary>
        /// Removes all existing ordered pairs of the form (r,s).  Then, for each 
        /// t in newDependees, adds the ordered pair (t,s).
        /// </summary>
        public void ReplaceDependees(string s, IEnumerable<string> newDependees)
        {
            // Get our new Hashset of values and our old HashSet of values
            HashSet<string> newValues = (HashSet<string>)newDependees;
            dependees.TryGetValue(s, out HashSet<string> output);
            // if the output isn't null, copy it so that we can work with it. otherwise, make a new empty HashSet.
            HashSet<string> outputCopy;
            if (output == null)
            {
                outputCopy = new HashSet<string>();
            }
            else
            {
                outputCopy = new HashSet<string>(output);
            }
            // remove key s and all of it's pairs from the dictionary.
            foreach (string deleter in outputCopy)
            {
                RemoveDependency(deleter,s);
            }
            //iterate through each new value and add a dependency for each, then increment size based on the size of newValues.
            foreach (string update in newValues)
            {
                AddDependency(update,s);
            }
        }

    }

}