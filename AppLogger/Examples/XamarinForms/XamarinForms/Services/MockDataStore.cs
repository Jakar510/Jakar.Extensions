using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XamarinForms.Models;



namespace XamarinForms.Services
{
    public class MockDataStore : IDataStore<Item>
    {
        private readonly List<Item> _items;

        public MockDataStore() => _items = new List<Item>
                                           {
                                               new Item
                                               {
                                                   Id = Guid.NewGuid()
                                                            .ToString(),
                                                   Text        = "First item",
                                                   Description = "This is an item description."
                                               },
                                               new Item
                                               {
                                                   Id = Guid.NewGuid()
                                                            .ToString(),
                                                   Text        = "Second item",
                                                   Description = "This is an item description."
                                               },
                                               new Item
                                               {
                                                   Id = Guid.NewGuid()
                                                            .ToString(),
                                                   Text        = "Third item",
                                                   Description = "This is an item description."
                                               },
                                               new Item
                                               {
                                                   Id = Guid.NewGuid()
                                                            .ToString(),
                                                   Text        = "Fourth item",
                                                   Description = "This is an item description."
                                               },
                                               new Item
                                               {
                                                   Id = Guid.NewGuid()
                                                            .ToString(),
                                                   Text        = "Fifth item",
                                                   Description = "This is an item description."
                                               },
                                               new Item
                                               {
                                                   Id = Guid.NewGuid()
                                                            .ToString(),
                                                   Text        = "Sixth item",
                                                   Description = "This is an item description."
                                               }
                                           };

        public async Task<bool> AddItemAsync( Item item )
        {
            _items.Add( item );

            return await Task.FromResult( true );
        }

        public async Task<bool> UpdateItemAsync( Item item )
        {
            Item? oldItem = _items.FirstOrDefault( arg => arg.Id == item.Id );

            _items.Remove( oldItem );
            _items.Add( item );

            return await Task.FromResult( true );
        }

        public async Task<bool> DeleteItemAsync( string id )
        {
            Item? oldItem = _items.FirstOrDefault( arg => arg.Id == id );

            _items.Remove( oldItem );

            return await Task.FromResult( true );
        }

        public async Task<Item> GetItemAsync( string id ) => await Task.FromResult( _items.FirstOrDefault( s => s.Id == id ) );

        public async Task<IEnumerable<Item>> GetItemsAsync( bool forceRefresh = false ) => await Task.FromResult( _items );
    }
}
