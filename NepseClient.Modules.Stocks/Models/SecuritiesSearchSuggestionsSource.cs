using MaterialDesignExtensions.Model;

using NepseClient.Libraries.NepalStockExchange.Responses;

using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NepseClient.Modules.Stocks.Models
{
    public class SecuritiesSearchSuggestionsSource : ISearchSuggestionsSource, ITextBoxSuggestionsSource
    {
        private IEnumerable<SecurityResponse> _securities;

        public SecuritiesSearchSuggestionsSource()
        {
            _securities = Enumerable.Empty<SecurityResponse>();
        }

        public IList<string> GetAutoCompletion(string searchTerm)
        {
            return _securities
                .Where(security => security.Name.ToLower().Contains(searchTerm.ToLower()))
                .Select(x => x.Name)
                .ToList();
        }

        public IList<string> GetSearchSuggestions()
        {
            return _securities
                .Select(x => x.Name)
                .ToList();
        }

        public IEnumerable<string> Search(string searchTerm)
        {
           return _securities
                .Where(security => security.Name.ToLower().Contains(searchTerm.ToLower()))
                .Select(x => x.Name);
        }

        IEnumerable IAutocompleteSource.Search(string searchTerm)
        {
            return Search(searchTerm);
        }
        

        public void UpdateSecurities(IEnumerable<SecurityResponse> securities)
        {
            _securities = securities;
        }
    }
}
