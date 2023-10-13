using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace QT
{
    public class StringListSearchProvider : ScriptableObject, ISearchWindowProvider
    {
        private string[] _listItems;
        private Action<string> _onSelected;

        public StringListSearchProvider(string[] listItems, Action<string> callback)
        {
            _listItems = listItems;
            _onSelected = callback;
        }
        
        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            var list = new List<SearchTreeEntry>();
            list.Add(new SearchTreeGroupEntry(new GUIContent("List"), 0));


            var sortedListItems = _listItems.ToList();
            sortedListItems.Sort((a, b)=>
            {
                var splits1 = a.Split('_');
                var splits2 = b.Split('_');

                for (int i = 0; i < splits1.Length; i++)
                {
                    if(i >= splits2.Length)
                    {
                        return 1;
                    }
                    
                    //int value = String.Compare(splits1[i], splits2[i], StringComparison.Ordinal);
                    int value = splits1[i].CompareTo(splits2[i]);
                    if (value != 0)
                    {
                        if(splits1.Length != splits2.Length && (i == splits1.Length - 1 || i == splits2.Length - 1))
                        {
                            return splits1.Length < splits2.Length ? 1 : -1;
                        }
                        return value;
                    }
                }
                
                return 0;
            });
            
            
            var groups = new List<string>();
            foreach (string item in sortedListItems)
            {
                var entryTitle = item.Split('_');
                var groupName = "";

                for (int i = 0; i < entryTitle.Length - 1; i++)
                {
                    groupName += entryTitle[i];

                    if (!groups.Contains(groupName))
                    {
                        list.Add(new SearchTreeGroupEntry(new GUIContent(entryTitle[i]), i + 1));
                        groups.Add(groupName);
                    }

                    groupName += "_";
                }

                var entry = new SearchTreeEntry(new GUIContent(entryTitle.Last()))
                {
                    level = entryTitle.Length,
                    userData = item.Split('/').FirstOrDefault()
                };

                list.Add(entry);
            }

            return list;
        }

        public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
        {
            _onSelected?.Invoke((string)SearchTreeEntry.userData);
            return true;
        }
    }
}
