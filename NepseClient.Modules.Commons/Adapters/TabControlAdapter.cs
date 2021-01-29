using System;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Controls;

using NepseClient.Commons.Contracts;

using Prism.Regions;
using Prism.Regions.Behaviors;

namespace NepseClient.Modules.Commons.Adapters
{
    public class TabControlAdapter : RegionAdapterBase<TabControl> 
    {
        public TabControlAdapter(IRegionBehaviorFactory regionBehaviorFactory) : base(regionBehaviorFactory)
        {
        }
        protected override void Adapt(IRegion region, TabControl regionTarget)
        {
            region.Views.CollectionChanged += (s, e) =>
            {
                if (e.Action == NotifyCollectionChangedAction.Add)
                    foreach (UserControl item in e.NewItems)
                    {
                        if (item.DataContext is ITabPage tab)
                        {
                            regionTarget.Items.Add(new TabItem { Header = tab.Title, Content = item });
                        }
                        else
                        {
                            regionTarget.Items.Add(new TabItem { Header = item.Name, Content = item });

                        }
                    }
                else if (e.Action == NotifyCollectionChangedAction.Remove)
                    foreach (UserControl item in e.OldItems)
                    {
                        var tabTodelete = regionTarget.Items.OfType<TabItem>().FirstOrDefault(n => n.Content == item);
                        regionTarget.Items.Remove(tabTodelete);
                    }

            };
        }

        protected override IRegion CreateRegion()
        {
            return new SingleActiveRegion();
        }
    }
}
