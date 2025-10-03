
using Grocery.Core.Interfaces.Repositories;
using Grocery.Core.Interfaces.Services;
using Grocery.Core.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Grocery.Core.Services
{
    public class BoughtProductsService : IBoughtProductsService
    {
        private readonly IGroceryListItemsRepository _groceryListItemsRepository;
        private readonly IClientRepository _clientRepository;
        private readonly IProductRepository _productRepository;
        private readonly IGroceryListRepository _groceryListRepository;
        public BoughtProductsService(IGroceryListItemsRepository groceryListItemsRepository, IGroceryListRepository groceryListRepository, IClientRepository clientRepository, IProductRepository productRepository)
        {
            _groceryListItemsRepository = groceryListItemsRepository;
            _groceryListRepository = groceryListRepository;
            _clientRepository = clientRepository;
            _productRepository = productRepository;
        }
        public List<BoughtProducts> Get(int? productId)
        {
            //In BoughtProductsService werk je de Get(productid) functie uit zodat alle Clients die product met productid hebben gekocht met client, boodschappenlijst en product in de lijst staan die wordt geretourneerd.
            // alle grocery list items ophalen met dat id
            List<BoughtProducts> boughtProductsList = new List<BoughtProducts>();

            // doorheen loopen en daar moet je het een object maken en tooevoegen aan een lijst
            var product = _productRepository.Get(productId.Value);
            if (product == null) return boughtProductsList;

            var clients = _clientRepository.GetAll();

            // alle boodschappenlijst items ophalen met dat product id
            var groceryListItems = _groceryListItemsRepository.GetAll().Where(gli => gli.ProductId == productId).ToList();

            foreach (var gli in groceryListItems)
            {
                // voor elk grocery list item de boodschappenlijst ophalen
                var groceryList = _groceryListRepository.Get(gli.GroceryListId);
                if (groceryList == null) continue;

                // voor elke boodschappenlijst de client ophalen
                var client = clients.FirstOrDefault(c => c.Id == groceryList.ClientId);
                if (client == null) continue;

                // een nieuw object maken van BoughtProducts
                var boughtProduct = new BoughtProducts(client, groceryList, product);

                boughtProductsList.Add(boughtProduct);
            }

            return boughtProductsList;
        }
    }
}
