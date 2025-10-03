using Grocery.Core.Interfaces.Repositories;
using Grocery.Core.Interfaces.Services;
using Grocery.Core.Models;

namespace Grocery.Core.Services
{
    public class GroceryListItemsService : IGroceryListItemsService
    {
        private readonly IGroceryListItemsRepository _groceriesRepository;
        private readonly IProductRepository _productRepository;

        public GroceryListItemsService(IGroceryListItemsRepository groceriesRepository, IProductRepository productRepository)
        {
            _groceriesRepository = groceriesRepository;
            _productRepository = productRepository;
        }

        public List<GroceryListItem> GetAll()
        {
            List<GroceryListItem> groceryListItems = _groceriesRepository.GetAll();
            FillService(groceryListItems);
            return groceryListItems;
        }

        public List<GroceryListItem> GetAllOnGroceryListId(int groceryListId)
        {
            List<GroceryListItem> groceryListItems = _groceriesRepository.GetAll().Where(g => g.GroceryListId == groceryListId).ToList();
            FillService(groceryListItems);
            return groceryListItems;
        }

        public GroceryListItem Add(GroceryListItem item)
        {
            return _groceriesRepository.Add(item);
        }

        public GroceryListItem? Delete(GroceryListItem item)
        {
            throw new NotImplementedException();
        }

        public GroceryListItem? Get(int id)
        {
            return _groceriesRepository.Get(id);
        }

        public GroceryListItem? Update(GroceryListItem item)
        {
            return _groceriesRepository.Update(item);
        }

        public List<BestSellingProducts> GetBestSellingProducts(int topX = 5)
        {
            List<GroceryListItem> groceryListItems = _groceriesRepository.GetAll();
            
            Dictionary<int, int> productAmounts = new Dictionary<int, int>();
            foreach (GroceryListItem item in groceryListItems)
            {
                if (productAmounts.ContainsKey(item.ProductId))
                {
                    productAmounts[item.ProductId] += item.Amount;
                }
                else
                {
                    productAmounts[item.ProductId] = item.Amount;
                }
            }
            
            List<KeyValuePair<int, int>> productList = new List<KeyValuePair<int, int>>();
            foreach (KeyValuePair<int, int> pair in productAmounts)
            {
                productList.Add(pair);
            }
            
            productList.Sort((a, b) => b.Value.CompareTo(a.Value));
            
            List<KeyValuePair<int, int>> topProductsList = new List<KeyValuePair<int, int>>();
            int count = Math.Min(topX, productList.Count);
            for (int i = 0; i < count; i++)
            {
                topProductsList.Add(productList[i]);
            }
            
            List<int> productIds = new List<int>();
            foreach (var pair in topProductsList)
            {
                productIds.Add(pair.Key);
            }
            
            List<Product> allProducts = _productRepository.GetAll();
            
            Dictionary<int, Product> productDictionary = new Dictionary<int, Product>();
            foreach (Product product in allProducts)
            {
                if (productIds.Contains(product.Id))
                {
                    productDictionary[product.Id] = product;
                }
            }
            
            List<BestSellingProducts> bestSellingProducts = new List<BestSellingProducts>();
            for (int i = 0; i < topProductsList.Count; i++)
            {
                int productId = topProductsList[i].Key;
                int totalAmount = topProductsList[i].Value;
                Product product;
                
                if (productDictionary.TryGetValue(productId, out product))
                {
                 
                }
                else
                {
              
                    product = new Product(0, "None", 0);
                }
                
                bestSellingProducts.Add(new BestSellingProducts(
                    product.Id,
                    product.Name,
                    product.Stock,
                    totalAmount,
                    i + 1 
                ));
            }
            
            return bestSellingProducts;
        }

        private void FillService(List<GroceryListItem> groceryListItems)
        {
            foreach (GroceryListItem g in groceryListItems)
            {
                g.Product = _productRepository.Get(g.ProductId) ?? new(0, "", 0);
            }
        }
    }
}