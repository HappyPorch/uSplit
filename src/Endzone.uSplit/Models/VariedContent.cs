using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;

namespace Endzone.uSplit.Models
{
    public class VariedContent : IPublishedContent
    {
        private readonly IPublishedContent original;

        //an ordered list of variations that overwrite the page.
        //if multiple items overwrite the same properties the last item wins.
        public IPublishedContentVariation[] AppliedVariations { get; }

        /// <summary>
        /// A case-insensitive map of property overrides.
        /// </summary>
        private readonly Dictionary<string, IPublishedProperty> overrides;

        public VariedContent(IPublishedContent original, IPublishedContentVariation[] variations)
        {
            this.original = original;
            TemplateId = original.TemplateId;

            AppliedVariations = variations;

            //todo: can the dict be constructed lazily, to avoid any potential sideeffects when going throgh all the properties?
            overrides = new Dictionary<string, IPublishedProperty>();

            //the order of the variations matters. If they overwrite the same field the last variation wins.
            foreach (var contentVariation in AppliedVariations)
            {
                foreach (var variationProperty in contentVariation.Content.Properties)
                {
                    if (variationProperty.HasValue)
                    {
                        overrides[variationProperty.PropertyTypeAlias.ToLowerInvariant()] = variationProperty;
                    }
                }
                TemplateId = contentVariation.Content.TemplateId;
            }
        }

        #region IPublishedContent

        public int GetIndex()
        {
            return original.GetIndex();
        }

        public IPublishedProperty GetProperty(string alias)
        {
            return GetProperty(alias, false);
        }

        public IPublishedProperty GetProperty(string alias, bool recurse)
        {
            IPublishedProperty property;
            return overrides.TryGetValue(alias.ToLowerInvariant(), out property) ? property 
                : original.GetProperty(alias, recurse);
        }

        public IEnumerable<IPublishedContent> ContentSet => original.ContentSet;

        public PublishedContentType ContentType => original.ContentType;

        public int Id => original.Id;

        public int TemplateId { get; }

        public int SortOrder => original.SortOrder;

        public string Name => original.Name;

        public string UrlName => original.UrlName;

        public string DocumentTypeAlias => original.DocumentTypeAlias;

        public int DocumentTypeId => original.DocumentTypeId;

        public string WriterName => original.WriterName;

        public string CreatorName => original.CreatorName;

        public int WriterId => original.WriterId;

        public int CreatorId => original.CreatorId;

        public string Path => original.Path;

        public DateTime CreateDate => original.CreateDate;

        public DateTime UpdateDate => original.UpdateDate;

        public Guid Version => original.Version;

        public int Level => original.Level;

        public string Url => original.Url;

        public PublishedItemType ItemType => original.ItemType;

        public bool IsDraft => original.IsDraft;

        public IPublishedContent Parent => original.Parent;

        public IEnumerable<IPublishedContent> Children => original.Children;

        public ICollection<IPublishedProperty> Properties => overrides.Values;

        public object this[string alias] => GetProperty(alias)?.Value;

        #endregion
    }
}
