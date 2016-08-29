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
        private readonly IPublishedContent variation;
        private readonly Dictionary<string, IPublishedProperty> properties;

        public VariedContent(IPublishedContent original, IPublishedContent variation, IExperiment experiment, int variationId)
        {
            this.original = original;
            this.variation = variation;
            Experiment = experiment;
            VariationId = variationId;

            properties = original.Properties.ToDictionary(p => p.PropertyTypeAlias, p => p);
            foreach (var variationProperty in variation.Properties)
            {
                if (variationProperty.HasValue)
                {
                    properties[variationProperty.PropertyTypeAlias] = variationProperty;
                }
            }
        }

        public int VariationId { get; }

        public IExperiment Experiment { get; }

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
            var property = variation.GetProperty(alias, recurse);
            if (property?.HasValue == true)
                return property;
            return original.GetProperty(alias, recurse);
        }

        public IEnumerable<IPublishedContent> ContentSet => original.ContentSet;

        public PublishedContentType ContentType => original.ContentType;

        public int Id => original.Id;

        public int TemplateId => variation.TemplateId;

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

        public ICollection<IPublishedProperty> Properties => properties.Values;

        public object this[string alias] => properties[alias]?.Value;

        #endregion
    }
}
