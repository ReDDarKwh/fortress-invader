using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Scripts.Characters
{

    [CreateAssetMenu(fileName = "StateLinker", menuName = "StateMachine/StateLinker", order = 1)]
    public class BaseEventStateLinker : ScriptableObject
    {

        public List<EventStateLinking> links;
        private Dictionary<string/*stateName*/, HashSet<EventStateLinking>> LinkDictionnary
        = new Dictionary<string/*stateName*/, HashSet<EventStateLinking>>();

        void OnEnable()
        {
            LinkDictionnary = links.GroupBy(x => x.tagName)
            .Select(x => new HashSet<EventStateLinking>(x))
            .ToDictionary(x => x.First().tagName);
        }

        public IEnumerable<EventStateLinking> GetLinksForState(IEnumerable<string> tags)
        {
            var result = new HashSet<EventStateLinking>();

            foreach (var tag in tags)
            {
                if (LinkDictionnary.ContainsKey(tag))
                {
                    foreach (var link in LinkDictionnary[tag])
                    {
                        result.Add(link);
                    }
                }
            }
            return result;
        }
    }




}
