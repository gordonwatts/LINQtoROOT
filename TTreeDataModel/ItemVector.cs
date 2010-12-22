using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TTreeDataModel
{
    /// <summary>
    /// An item that references some sort of a STL vector. This is a STL vector, not a C styl earray.
    /// This is important because you can do things like ask for the "size()" of the thing, how we deal
    /// with those things have to be kept seperate!
    /// </summary>
    public class ItemVector : IClassItem
    {
        private string _vectorType;

        public ItemVector(string vectorCSDecl, string vectorName)
        {
            // TODO: Complete member initialization
            _vectorType = vectorCSDecl;
            ItemType = vectorCSDecl;
            Name = vectorName;

        }
        public string ItemType { get; private set; }
        public string Name { get; private set; }
    }
}
